using ComputationServer.Data.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ComputationServer.Utility;

namespace ComputationServer.Nodes
{
    public class JobQueue
    {
        private ConcurrentQueue<Job> _pendingJobs;
        private ConcurrentQueue<Job> _activeJobs;
        private ConcurrentQueue<Job> _completedJobs;
        private ConcurrentQueue<Job> _failedJobs;
        private int _maxConcurrent;
        private object _queueState = new object();
        
        public JobQueue(int maxConcurrent)
        {
            _maxConcurrent = maxConcurrent;
            _pendingJobs = new ConcurrentQueue<Job>();
            _activeJobs = new ConcurrentQueue<Job>();
            _completedJobs = new ConcurrentQueue<Job>();
            _failedJobs = new ConcurrentQueue<Job>();
        }

        #region Queue State Access

        public List<Job> Completed
        {
            get
            {
                lock(_queueState)
                {
                    return Replicator.CopyAll(_completedJobs.ToList());
                }
            }
        }

        public List<Job> Failed
        {
            get
            {
                lock (_queueState)
                {
                    return Replicator.CopyAll(_failedJobs.ToList());
                }
            }
        }

        public List<Job> Active
        {
            get
            {
                lock (_queueState)
                {
                    return Replicator.CopyAll(_activeJobs.ToList());
                }
            }
        }

        #endregion

        public void Enqueue(Job job)
        {
            lock(_queueState)
            {
                _pendingJobs.Enqueue(job);
            }
        }

        public List<Job> Find(Func<Job, bool> condition)
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

                return Replicator.CopyAll(inPending, inActive, inCompleted, inFailed);
            }
        }

        public List<Job> Update(Dictionary<string, Status> updatedActive)
        {
            var newActive = new ConcurrentQueue<Job>();
            var newPending = new ConcurrentQueue<Job>();
            var newCompleted = new ConcurrentQueue<Job>();
            var newFailed = new ConcurrentQueue<Job>();
            var changed = new List<Job>();

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
                    if (updatedActive.ContainsKey(job.Guid))
                    {
                        var newStatus = updatedActive[job.Guid];
                        job.Status = newStatus;
                        changed.Add(job.Clone() as Job);
                    }                    
                    
                    switch (job.Status)
                    {
                        case Status.RUNNING:
                            {
                                newActive.Enqueue(job);
                                break;
                            }

                        case Status.COMPLETED:
                            {
                                newCompleted.Enqueue(job);                                
                                break;
                            }
                        case Status.FAILED:
                        case Status.UNKNOWN:
                            {                                
                                newFailed.Enqueue(job);
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
                    Job toStart;
                    if (newPending.TryDequeue(out toStart))
                    {                        
                        toStart.Status = Status.RUNNING;
                        newActive.Enqueue(toStart);
                        changed.Add(toStart.Clone() as Job);
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

        public List<Job> GetAll()
        {
            lock (_queueState)
            {
                var pending = _pendingJobs.ToList();
                var active = _activeJobs.ToList();
                var completed = _completedJobs.ToList();
                var failed = _failedJobs.ToList();
                return Replicator.CopyAll(pending, active, completed, failed);
            }            
        }
    }
}
