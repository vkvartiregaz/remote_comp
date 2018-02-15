using ComputationServer.Data.Models;
using ComputationServer.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationServer.Nodes.AccessModules
{
    public class TorqueComputer : GenericComputer
    {
        public TorqueComputer(int maxConcurrent) : base(maxConcurrent) { }

        protected override bool StartJob(Operation operation)
        {
            throw new NotImplementedException();
        }

        protected override bool StopJob(Operation operation)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, Status> PollJobs(List<Operation> jobs)
        {
            throw new NotImplementedException();
        }

        protected override List<MnemonicValue> FetchResults(List<Operation> completed)
        {
            throw new NotImplementedException();
        }
    }
}
