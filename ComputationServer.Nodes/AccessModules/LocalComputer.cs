using ComputationServer.Data.Models;
using ComputationServer.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace ComputationServer.Nodes.AccessModules
{
    public class LocalComputer : IComputer
    {
        private JobQueue _jobQueue;
        private List<Operation> _running;

        public LocalComputer(int maxConcurrent)
        {
            _jobQueue = new JobQueue(maxConcurrent);
            _running = new List<Operation>();
        }

        public void EnqueueJob(Operation operation)
        {
            _jobQueue.Enqueue(operation);
        }
        
        public int GetJobETA(Operation operation)
        {
            return 1;
        }

        public bool IsAlive()
        {
            return true;
        }

        public void Progress()
        {
            var active = _jobQueue.Active;
            var pollResults = PollJobs(active);
            var oldActive = _jobQueue.Active;
            var updated = _jobQueue.Update(pollResults);

            if(!updated)
                throw new Exception("Progress failed");

            var newActive = _jobQueue.Active;
            var toStart = (from j in newActive
                           where !oldActive.Contains(j)
                           select j).ToList();

            foreach (var job in toStart)
                StartJob(job);
        }

        public bool AbortJob(Operation operation)
        {
            return false;
        }

        private bool StartJob(Operation operation)
        {
            return false;
        }

        private bool StopJob(Operation operation)
        {
            return false;
        }

        private Dictionary<string, Status> PollJobs(List<Operation> jobs)
        {
            return null;
        }
    }
}
