using ComputationServer.Data.Entities;
using ComputationServer.Data.Enums;
using ComputationServer.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationServer.Nodes.AccessModules
{
    public class DataLoadingComputer : IComputer
    {
        protected IDataStorage _storage;
        protected JobQueue _jobQueue;

        public decimal ChargeRate => throw new NotImplementedException();

        public string ComputerId => throw new NotImplementedException();

        public bool AbortJob(string guid)
        {
            throw new NotImplementedException();
        }

        public void EnqueueJob(Job operation)
        {
            _storage.RequestData("data id here", operation.Input[0].Type);
            _jobQueue.Enqueue(operation);
        }

        public List<Job> FindJobs(Func<Job, bool> condition)
        {
            throw new NotImplementedException();
        }

        public bool IsAlive()
        {
            throw new NotImplementedException();
        }

        public int TimeEstimate(Job operation)
        {
            throw new NotImplementedException();
        }

        public List<Job> Progress()
        {
            throw new NotImplementedException();
        }

        Dictionary<string, ExecutionStatus> IComputer.Progress()
        {
            throw new NotImplementedException();
        }

        double IComputer.TimeEstimate(Job operation)
        {
            throw new NotImplementedException();
        }
    }
}
