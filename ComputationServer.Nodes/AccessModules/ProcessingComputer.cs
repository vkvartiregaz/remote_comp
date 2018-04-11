using ComputationServer.Data.Entities;
using ComputationServer.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using ComputationServer.Data.Interfaces;
using ComputationServer.Data.Enums;

namespace ComputationServer.Nodes.AccessModules
{
    public abstract class ProcessingComputer : IComputer
    {
        protected JobQueue _jobQueue;
        protected IMethodRepository _methodRepository;

        public string ComputerId { get; private set; }

        public decimal ChargeRate { get; private set; }

        public ProcessingComputer(int maxConcurrent,
            IMethodRepository methodRepository)
        {
            _jobQueue = new JobQueue(maxConcurrent);
            _methodRepository = methodRepository;
            ComputerId = "abacaba";
            ChargeRate = 1.2m;
        }

        #region IComputer Methods

        public virtual bool IsAlive()
        {
            return Ping();
        }

        public Dictionary<string, ExecutionStatus> Progress()
        {
            var pollResults = PollJobs(_jobQueue.Active);
            var result = new Dictionary<string, ExecutionStatus>();

            try
            {
                var firstWave = _jobQueue.Update(pollResults);

                var toRun = new Dictionary<string, ExecutionStatus>();

                foreach(var job in _jobQueue.Pending)
                    if (ReadyToRun(job))
                        toRun.Add(job.Guid, ExecutionStatus.RUNNING);
                
                var secondWave = _jobQueue.Update(toRun);

                var toStart = (from job in _jobQueue.Active
                               where toRun.Keys.Contains(job.Guid)
                               select job).ToList();

                foreach (var job in toStart)
                    StartJob(job);

                foreach (var change in secondWave)
                    result.Add(change.Key, change.Value);

                foreach (var change in firstWave)
                    if (!result.ContainsKey(change.Key))
                        result.Add(change.Key, change.Value);
            }
            catch(Exception ex)
            {
                //log a failed Progress()
            }

            return result;
        }

        public void EnqueueJob(Job operation)
        {
            _jobQueue.Enqueue(operation);
        }

        public bool AbortJob(string guid)
        {
            var job = _jobQueue.Find(j => j.Guid == guid).FirstOrDefault();

            if (job == null)
                return false;

            if (job.Status == ExecutionStatus.RUNNING ||
                job.Status == ExecutionStatus.QUEUED)
                if(!StopJob(job))
                    return false;

            _jobQueue.Update(new Dictionary<string, ExecutionStatus> { { job.Guid, ExecutionStatus.ABORTED } });
            
            return true;
        }
                
        public List<Job> FindJobs(Func<Job, bool> condition)
        {
            return _jobQueue.Find(condition);
        }

        public int TimeEstimate(Job operation)
        {
            Console.WriteLine("TimeEstimate stub");
            return 12;
        }

        private bool ReadyToRun(Job job)
        {
            if (job.Input.Any(d => d.Status != DataStatus.Available || d.Source != ComputerId))
                return false;

            var otherJobs = job.Session.Jobs;

            var prevJobs = (from j in otherJobs
                            where job.Dependencies.Contains(j.Guid)
                            select j).ToList();

            if (prevJobs.Any(j => j.Status != ExecutionStatus.COMPLETED))
                return false;

            return true;
        }

        #endregion

        #region Abstract Methods

        protected abstract bool StartJob(Job operation);

        protected abstract bool StopJob(Job operation);

        protected abstract Dictionary<string, ExecutionStatus> PollJobs(List<Job> jobs);

        protected abstract List<MnemonicValue> FetchResults(List<Job> completed);

        protected abstract bool Ping();

        double IComputer.TimeEstimate(Job operation)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
