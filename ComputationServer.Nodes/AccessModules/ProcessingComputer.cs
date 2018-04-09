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

        public ProcessingComputer(int maxConcurrent,
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
            var pollResults = PollJobs(_jobQueue.Active);

            try
            {
                _jobQueue.Update(pollResults);

                //check every pending job (is it ready to run)
                _jobQueue.Update(toRun);

                var toStart = (from j in toRun
                                where _jobQueue.Active.Contains(j)
                                select j).ToList();

                foreach (var job in toStart)
                    StartJob(job);                                
            }
            catch(Exception ex)
            {
                //log a failed Progress()
            }
                


            
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

            if (job.Status == ExecutionStatus.RUNNING)
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

        #endregion

        #region Abstract Methods

        protected abstract bool StartJob(Job operation);

        protected abstract bool StopJob(Job operation);

        protected abstract Dictionary<string, ExecutionStatus> PollJobs(List<Job> jobs);

        protected abstract List<MnemonicValue> FetchResults(List<Job> completed);

        #endregion
    }
}
