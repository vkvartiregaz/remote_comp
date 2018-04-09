using System;
using System.Collections.Generic;
using System.Text;
using ComputationServer.Data.Interfaces;
using ComputationServer.Data.Entities;
using ComputationServer.Data.Enums;

namespace ComputationServer.Nodes.AccessModules
{
    public class DummyComputer : ProcessingComputer
    {
        public DummyComputer(int maxConcurrent, IMethodRepository methodRepository) 
            : base(maxConcurrent, methodRepository) { }

        protected override List<MnemonicValue> FetchResults(List<Job> completed)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, ExecutionStatus> PollJobs(List<Job> jobs)
        {
            var result = new Dictionary<string, ExecutionStatus>();

            if(jobs.Count > 0)
                result[jobs[0].Guid] = ExecutionStatus.COMPLETED;

            if (jobs.Count > 1)
                result[jobs[1].Guid] = ExecutionStatus.FAILED;

            for (int i = 2; i < jobs.Count; ++i)
                result[jobs[i].Guid] = jobs[i].Status;

            return result;
        }

        protected override bool StartJob(Job operation)
        {
            return true;
        }

        protected override bool StopJob(Job operation)
        {
            return true;
        }
    }
}
