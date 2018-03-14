using ComputationServer.Data.Entities;
using ComputationServer.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chilkat;
using ComputationServer.Data.Enums;

namespace ComputationServer.Nodes.AccessModules
{
    public class LinuxComputer : DataProcessingComputer
    {
        public LinuxComputer(int maxConcurrent) : base(maxConcurrent, null) { }

        protected override bool StartJob(Job operation)
        {
            var ssh = new Ssh();
            return false;
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
