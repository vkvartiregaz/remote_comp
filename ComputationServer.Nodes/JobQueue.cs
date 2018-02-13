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
        private ConcurrentQueue<Operation> _completeJobs;
        private ConcurrentQueue<Operation> _failedJobs;
        private int _maxConcurrent;
        private object _hold = new object();
        
        public JobQueue(int maxConcurrent)
        {
            _maxConcurrent = maxConcurrent;
            _pendingJobs = new ConcurrentQueue<Operation>();
            _activeJobs = new ConcurrentQueue<Operation>();
            _completeJobs = new ConcurrentQueue<Operation>();
            _failedJobs = new ConcurrentQueue<Operation>();
        }

        #region Queue State Access

        public List<Operation> Complete
        {
            get
            {
                lock(_hold)
                {
                    return _completeJobs.ToList();
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

            result.InsertRange(0, (from op in _completeJobs
                                   where condition(op)
                                   select op).ToList());

            result.InsertRange(0, (from op in _failedJobs
                                   where condition(op)
                                   select op).ToList());

            return result;
        }

        public bool Update(Dictionary<string, Status> updatedActive)
        {
            var activeCopy = _activeJobs.ToList();
            var newActive = new ConcurrentQueue<Operation>();
            var newPending = new ConcurrentQueue<Operation>();
            var newCompleted = new ConcurrentQueue<Operation>();
            var newFailed = new ConcurrentQueue<Operation>();

            foreach (var op in activeCopy)
            {
                switch (op.Status)
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
                            throw new Exception("Unexpected job status");
                        }
                }
            }

            var pendingCopy = _pendingJobs.ToList();

            var deficit = _maxConcurrent - newActive.Count;


            while (deficit > 0)
            {

            }
        }
    }
}
