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
    public class TorqueComputer : ProcessingComputer
    {
        public TorqueComputer(int maxConcurrent) : base(maxConcurrent, null) { }

        protected override bool StartJob(Job operation)
        {
            throw new NotImplementedException();
        }

        protected override bool StopJob(Job operation)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, ExecutionStatus> PollJobs(List<Job> jobs)
        {
            throw new NotImplementedException();
        }

        protected override List<MnemonicValue> FetchResults(List<Job> completed)
        {
            throw new NotImplementedException();
        }
    }
}
