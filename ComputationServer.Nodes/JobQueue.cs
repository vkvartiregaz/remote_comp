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
        private object _hold = new object();
        
        public JobQueue(int maxConcurrent)
        {
            _maxConcurrent = maxConcurrent;
            _pendingJobs = new ConcurrentQueue<Operation>();
            _activeJobs = new ConcurrentQueue<Operation>();
            _completedJobs = new ConcurrentQueue<Operation>();
            _failedJobs = new ConcurrentQueue<Operation>();
        }

        #region Queue State Access

        public List<Operation> Complete
        {
            get
            {
                lock(_hold)
                {
                    return _completedJobs.ToList();
                }
            }
        }

        public List<Operation> Failed
        {
            get
            {
                lock (_hold)
                {
                    return _failedJobs.ToList();
                }
            }
        }

        public List<Operation> Active
        {
            get
            {
                lock (_hold)
                {
                    return _activeJobs.ToList();
                }
            }
        }

        #endregion

        public void Enqueue(Operation job)
        {
            lock(_hold)
            {
                _pendingJobs.Enqueue(job);
            }
        }

        public List<Operation> Find(Func<Operation, bool> condition)
        {
            var result = (from op in _pendingJobs
                          where condition(op)
                          select op).ToList();

            result.InsertRange(0, (from op in _activeJobs
                                   where condition(op)
                                   select op).ToList());

            result.InsertRange(0, (from op in _completedJobs
                                   where condition(op)
                                   select op).ToList());

            result.InsertRange(0, (from op in _failedJobs
                                   where condition(op)
                                   select op).ToList());

            return result;
        }

        public bool Update(Dictionary<string, Status> updatedActive)
        {
            lock (_hold)
            {
                var newActive = new ConcurrentQueue<Operation>();
                var newPending = new ConcurrentQueue<Operation>();
                var newCompleted = new ConcurrentQueue<Operation>();
                var newFailed = new ConcurrentQueue<Operation>();
                
                var activeCopy = _activeJobs.ToList();
                var pendingCopy = _pendingJobs.ToList();
                var completedCopy = _completedJobs.ToList();
                var failedCopy = _failedJobs.ToList();

                foreach (var job in pendingCopy)
                    newPending.Enqueue(job);

                foreach (var job in completedCopy)
                    newCompleted.Enqueue(job);

                foreach (var job in failedCopy)
                    newFailed.Enqueue(job);
                
                foreach (var op in activeCopy)
                {
                    if (!updatedActive.ContainsKey(op.Guid))
                        continue;

                    var newStatus = updatedActive[op.Guid];

                    switch (newStatus)
                    {
                        case Status.RUNNING:
                            {
                                newActive.Enqueue(op);
                                break;
                            }

                        case Status.COMPLETED:
                            {
                                newCompleted.Enqueue(op);
                                break;
                            }
                        case Status.FAILED:
                        case Status.UNKNOWN:
                            {
                                newFailed.Enqueue(op);
                                break;
                            }
                        default:
                            {
                                return false;
                            }
                    }
                }
                
                var deficit = _maxConcurrent - newActive.Count;

                while (deficit > 0)
                {
                    Operation toStart;
                    newPending.TryDequeue(out toStart);
                    newActive.Enqueue(toStart);
                    deficit--;
                }

                _pendingJobs = newPending;
                _activeJobs = newActive;
                _completedJobs = newCompleted;
                _failedJobs = newFailed;
            }

            return true;
        }
    }
}
