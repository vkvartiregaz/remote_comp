using ComputationServer.Data.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ComputationServer.Nodes
{
    public class JobQueue
    {
        private ConcurrentQueue<Operation> _pendingJobs;
        private ConcurrentQueue<Operation> _activeJobs;
        private ConcurrentQueue<Operation> _completedJobs;
        private ConcurrentQueue<Operation> _failedJobs;
        private int _maxConcurrent;
        private object _queueState = new object();
        
        public JobQueue(int maxConcurrent)
        {
            _maxConcurrent = maxConcurrent;
            _pendingJobs = new ConcurrentQueue<Operation>();
            _activeJobs = new ConcurrentQueue<Operation>();
            _completedJobs = new ConcurrentQueue<Operation>();
            _failedJobs = new ConcurrentQueue<Operation>();
        }

        #region Queue State Access

        public List<Operation> Completed
        {
            get
            {
                lock(_queueState)
                {
                    return CopyAll(_completedJobs.ToList());
                }
            }
        }

        public List<Operation> Failed
        {
            get
            {
                lock (_queueState)
                {
                    return CopyAll(_failedJobs.ToList());
                }
            }
        }

        public List<Operation> Active
        {
            get
            {
                lock (_queueState)
                {
                    return CopyAll(_activeJobs.ToList());
                }
            }
        }

        #endregion

        public void Enqueue(Operation job)
        {
            lock(_queueState)
            {
                _pendingJobs.Enqueue(job);
            }
        }

        public List<Operation> Find(Func<Operation, bool> condition)
        {
            lock (_queueState)
            {
                var inPending = (from op in _pendingJobs
                                 where condition(op)
                                 select op).ToList();

                var inActive = (from op in _activeJobs
                                where condition(op)
                                select op).ToList();

                var inCompleted = (from op in _completedJobs
                                   where condition(op)
                                   select op).ToList();

                var inFailed = (from op in _failedJobs
                                where condition(op)
                                select op).ToList();

                return CopyAll(inPending, inActive, inCompleted, inFailed);
            }
        }

        public List<Operation> Update(Dictionary<string, Status> updatedActive)
        {
            var newActive = new ConcurrentQueue<Operation>();
            var newPending = new ConcurrentQueue<Operation>();
            var newCompleted = new ConcurrentQueue<Operation>();
            var newFailed = new ConcurrentQueue<Operation>();
            var changed = new List<Operation>();

            lock (_queueState)
            {
                var activeCopy = _activeJobs.ToList();

                foreach (var job in _pendingJobs)
                    newPending.Enqueue(job);

                foreach (var job in _completedJobs)
                    newCompleted.Enqueue(job);

                foreach (var job in _failedJobs)
                    newFailed.Enqueue(job);
            
                
                foreach (var job in activeCopy)
                {
                    if (!updatedActive.ContainsKey(job.Guid))
                        continue;

                    var newStatus = updatedActive[job.Guid];
                    var clone = job.Clone();

                    clone.Status = newStatus;

                    switch (newStatus)
                    {
                        case Status.RUNNING:
                            {
                                newActive.Enqueue(clone);
                                break;
                            }

                        case Status.COMPLETED:
                            {
                                newCompleted.Enqueue(clone);
                                changed.Add(clone.Clone());
                                break;
                            }
                        case Status.FAILED:
                        case Status.UNKNOWN:
                            {                                
                                newFailed.Enqueue(clone);
                                changed.Add(clone.Clone());
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                
                var deficit = _maxConcurrent - newActive.Count;

                while (deficit > 0)
                {
                    Operation toStart;
                    if (newPending.TryDequeue(out toStart))
                    {
                        var clone = toStart.Clone();
                        clone.Status = Status.RUNNING;
                        newActive.Enqueue(clone);
                        changed.Add(clone.Clone());
                        deficit--;
                    }
                    else
                        break;
                }

                _pendingJobs = newPending;
                _activeJobs = newActive;
                _completedJobs = newCompleted;
                _failedJobs = newFailed;
            }

            return changed;
        }

        public List<Operation> GetAll()
        {
            lock (_queueState)
            {
                var pending = _pendingJobs.ToList();
                var active = _activeJobs.ToList();
                var completed = _completedJobs.ToList();
                var failed = _failedJobs.ToList();
                return CopyAll(pending, active, completed, failed);
            }            
        }

        private List<Operation> CopyAll(params List<Operation>[] toCopy)
        {
            var result = new List<Operation>();

            foreach (var list in toCopy)
                foreach(var job in list)
                    result.Add(job.Clone());

            return result;
        }
    }
}
