using System;
using System.Collections.Generic;
using System.Text;
using ComputationServer.Data.Interfaces;
using ComputationServer.Data.Models;

namespace ComputationServer.Nodes.AccessModules
{
    public class DummyComputer : DataProcessingComputer
    {
        public DummyComputer(int maxConcurrent, IMethodRepository methodRepository) 
            : base(maxConcurrent, methodRepository) { }

        protected override List<MnemonicValue> FetchResults(List<Operation> completed)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, Status> PollJobs(List<Operation> jobs)
        {
            var result = new Dictionary<string, Status>();

            if(jobs.Count > 0)
                result[jobs[0].Guid] = Status.COMPLETED;

            if (jobs.Count > 1)
                result[jobs[1].Guid] = Status.FAILED;

            for (int i = 2; i < jobs.Count; ++i)
                result[jobs[i].Guid] = jobs[i].Status;

            return result;
        }

        protected override bool StartJob(Operation operation)
        {
            return true;
        }

        protected override bool StopJob(Operation operation)
        {
            return true;
        }
    }
}
