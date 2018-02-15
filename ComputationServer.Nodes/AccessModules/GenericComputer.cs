using ComputationServer.Data.Models;
using ComputationServer.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using ComputationServer.Data.Interfaces;

namespace ComputationServer.Nodes.AccessModules
{
    public abstract class GenericComputer : IComputer
    {
        private JobQueue _jobQueue;
        private IMethodRepository _methodRepository;

        public GenericComputer(int maxConcurrent,
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

        public void Progress()
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

        public void EnqueueJob(Operation operation)
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
                
        public List<Operation> FindJobs(Func<Operation, bool> condition)
        {
            return _jobQueue.Find(condition);
        }

        public int TimeEstimate(Operation operation)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Abstract Methods

        protected abstract bool StartJob(Operation operation);

        protected abstract bool StopJob(Operation operation);

        protected abstract Dictionary<string, Status> PollJobs(List<Operation> jobs);

        protected abstract List<MnemonicValue> FetchResults(List<Operation> completed);

        #endregion
    }
}
