using ComputationServer.Data.Models;
using ComputationServer.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationServer.Nodes.AccessModules
{
    public class DataTransferComputer : IComputer
    {
        protected IDataStorage _storage;
        protected JobQueue _jobQueue;

        public bool AbortJob(string guid)
        {
            throw new NotImplementedException();
        }

        public void EnqueueJob(Operation operation)
        {
            _storage.RequestData("data id here", operation.Input[0]);
            _jobQueue.Enqueue(operation);
        }

        public List<Operation> FindJobs(Func<Operation, bool> condition)
        {
            throw new NotImplementedException();
        }

        public bool IsAlive()
        {
            throw new NotImplementedException();
        }

        public int TimeEstimate(Operation operation)
        {
            throw new NotImplementedException();
        }

        public List<Operation> Progress()
        {
            throw new NotImplementedException();
        }
    }
}
