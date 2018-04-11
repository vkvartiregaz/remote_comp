using ComputationServer.Data.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ComputationServer.Utility;
using ComputationServer.Data.Enums;
using System.Threading;

namespace ComputationServer.Nodes
{
    public class JobQueue
    {
        //valid status changes:
        //queued -> pending
        //queued -> aborted
        //pending -> active
        //pending -> aborted
        //active -> completed
        //active -> failed
        //active -> aborted
        //ANY OTHER CHANGES WILL CAUSE AN EXCEPTION
        private Queue<Job> _queuedJobs;
        private Queue<Job> _pendingJobs;
        private List<Job> _activeJobs;
        private List<Job> _completedJobs;
        private List<Job> _failedJobs;
        private List<Job> _abortedJobs;
        
        //active <= _maxConcurrent, == _maxConcurrent if possible
        //active + pending >= _maxConcurrent if queued + active >= _maxConcurrent
        private int _maxConcurrent;
                
        private object _queueState = new object();
        
        public JobQueue(int maxConcurrent)
        {
            _maxConcurrent = maxConcurrent;
            _queuedJobs = new Queue<Job>();
            _pendingJobs = new Queue<Job>();
            _activeJobs = new List<Job>();
            _completedJobs = new List<Job>();
            _failedJobs = new List<Job>();
            _abortedJobs = new List<Job>();
        }

        #region Queue State Access

        public List<Job> Queued
        {
            get
            {
                lock (_queueState)
                {
                    return Replicator.CopyAll(_queuedJobs.ToList());
                }
            }
        }

        public List<Job> Aborted
        {
            get
            {
                lock (_queueState)
                {
                    return Replicator.CopyAll(_abortedJobs.ToList());
                }
            }
        }

        public List<Job> Pending
        {
            get
            {
                lock (_queueState)
                {
                    return Replicator.CopyAll(_pendingJobs.ToList());
                }
            }
        }

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
                _queuedJobs.Enqueue(job);
            }
        }

        public List<Job> Find(Func<Job, bool> condition)
        {
            lock (_queueState)
            {
                var aggregated = Replicator.CopyAll(_queuedJobs.ToList(), 
                    _pendingJobs.ToList(),
                    _activeJobs, 
                    _completedJobs, 
                    _failedJobs,
                    _abortedJobs);

                return (from j in aggregated
                        where condition(j)
                        select j).ToList();
            }
        }

        public Dictionary<string, ExecutionStatus> Update(Dictionary<string, ExecutionStatus> updates)
        {
            var result = new Dictionary<string, ExecutionStatus>();

            lock (_queueState)
            {
                UpdateActive(updates, result);
                UpdateQueued(updates, result);                
                UpdatePending(updates, result);
            }

            return result;
        }

        public double DelayEstimate()
        {
            var result = 0.0;

            lock (_queueState)
            {
                foreach (var job in _queuedJobs)
                    result += job.TimeEstimate();

                foreach (var job in _pendingJobs)
                    result += job.TimeEstimate();

                foreach (var job in _activeJobs)
                    result += job.TimeEstimate();
            }

            return result;
        }

        private void UpdateQueued(Dictionary<string, ExecutionStatus> updates,
            Dictionary<string, ExecutionStatus> result)
        {
            if (Monitor.TryEnter(_queueState))
                throw new Exception("Job queue access violation: UpdateQueued call without a state lock");

            var toAborted = new List<string>();

            foreach (var u in updates)
            {
                var queued = _queuedJobs.Where(j => j.Guid == u.Key).FirstOrDefault();

                if (queued != null)
                {
                    if (u.Value == ExecutionStatus.ABORTED)                                          
                        toAborted.Add(queued.Guid);
                    else
                        throw new Exception($"Unexpected status change for an active job: status {u.Value.ToString()}, job guid {queued.Guid}");

                }
            }

            for(int i = 0; i < _queuedJobs.Count; ++i)
            {
                var job = _queuedJobs.Dequeue();

                if (toAborted.Contains(job.Guid))
                {
                    _abortedJobs.Add(job);
                    result.Add(job.Guid, ExecutionStatus.ABORTED);
                }
                else if (_activeJobs.Count + _pendingJobs.Count < _maxConcurrent)
                    _pendingJobs.Enqueue(job);
                else
                    _queuedJobs.Enqueue(job);
            }
        }

        private void UpdateActive(Dictionary<string, ExecutionStatus> updates,
            Dictionary<string, ExecutionStatus> result)
        {
            if (Monitor.TryEnter(_queueState))
                throw new Exception("Job queue access violation: UpdateActive call without a state lock");

            foreach (var u in updates)
            {
                var active = _activeJobs.Where(j => j.Guid == u.Key).FirstOrDefault();

                if (active != null)
                {
                    switch (u.Value)
                    {
                        case ExecutionStatus.COMPLETED:
                            {
                                _activeJobs.Remove(active);
                                _completedJobs.Add(active);
                                result.Add(active.Guid, ExecutionStatus.COMPLETED);
                                break;
                            }
                        case ExecutionStatus.FAILED:
                        case ExecutionStatus.UNKNOWN:
                            {
                                _activeJobs.Remove(active);
                                _failedJobs.Add(active);
                                result.Add(active.Guid, ExecutionStatus.FAILED);
                                break;
                            }
                        case ExecutionStatus.ABORTED:
                            {
                                _activeJobs.Remove(active);
                                _abortedJobs.Add(active);
                                result.Add(active.Guid, ExecutionStatus.ABORTED);
                                break;
                            }
                        default:
                            {
                                throw new Exception($"Unexpected status change for an active job: status {u.Value.ToString()}, job guid {active.Guid}");
                            }
                    }
                }
            }
        }

        private void UpdatePending(Dictionary<string, ExecutionStatus> updates,
            Dictionary<string, ExecutionStatus> result)
        {
            if (Monitor.TryEnter(_queueState))
                throw new Exception("Job queue access violation: UpdatePending call without a state lock");

            var toActive = new List<string>();
            var toAborted = new List<string>();

            foreach (var u in updates)
            {
                var pending = _pendingJobs.Where(j => j.Guid == u.Key).FirstOrDefault();

                if (pending != null)
                {
                    switch (u.Value)
                    {
                        case ExecutionStatus.RUNNING:
                            {
                                toActive.Add(pending.Guid);
                                break;
                            }
                        case ExecutionStatus.ABORTED:
                            {
                                toAborted.Add(pending.Guid);
                                break;
                            }
                        default:
                            {
                                throw new Exception($"Unexpected status change for a pending job: status {u.Value.ToString()}, job guid {pending.Guid}");
                            }
                    }
                }
            }

            for (int i = 0; i < _pendingJobs.Count; ++i)
            {
                var job = _pendingJobs.Dequeue();

                if (toAborted.Contains(job.Guid))
                {
                    _abortedJobs.Add(job);
                    result.Add(job.Guid, ExecutionStatus.ABORTED);
                }
                else if (toActive.Contains(job.Guid) && _activeJobs.Count < _maxConcurrent)
                {
                    _activeJobs.Add(job);
                    result.Add(job.Guid, ExecutionStatus.RUNNING);
                }
                else
                    _pendingJobs.Enqueue(job);                
            }
        }
    }
}
