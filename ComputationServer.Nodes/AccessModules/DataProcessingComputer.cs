using ComputationServer.Data.Entities;
using ComputationServer.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using ComputationServer.Data.Interfaces;

namespace ComputationServer.Nodes.AccessModules
{
    public abstract class DataProcessingComputer : IComputer
    {
        protected JobQueue _jobQueue;
        protected IMethodRepository _methodRepository;

        public DataProcessingComputer(int maxConcurrent,
            IMethodRepository methodRepository)
        {
            _jobQueue = new JobQueue(maxConcurrent);
            _methodRepository = methodRepository;
        }

        #region IComputer Methods

        public virtual bool IsAlive()
        {
            return true;
        }

        public List<Job> Progress()
        {
            var oldActive = _jobQueue.Active;
            var pollResults = PollJobs(oldActive);
            var updated = _jobQueue.Update(pollResults);

            if (updated == null)
                throw new Exception("Progress failed");

            var newActive = _jobQueue.Active;
            var toStart = (from j in newActive
                           where !oldActive.Contains(j)
                           select j).ToList();

            foreach (var job in toStart)
                StartJob(job);

            FetchResults(_jobQueue.Completed);

            return updated;
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

            if (job.Status == Status.RUNNING)
                if(!StopJob(job))
                    return false;

            _jobQueue.Update(new Dictionary<string, Status> { { job.Guid, Status.ABORTED } });
            
            return true;
        }
                
        public List<Job> FindJobs(Func<Job, bool> condition)
        {
            return _jobQueue.Find(condition);
        }

        public int TimeEstimate(Job operation)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Abstract Methods

        protected abstract bool StartJob(Job operation);

        protected abstract bool StopJob(Job operation);

        protected abstract Dictionary<string, Status> PollJobs(List<Job> jobs);

        protected abstract List<MnemonicValue> FetchResults(List<Job> completed);

        #endregion
    }
}
