using ComputationServer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationServer.Nodes.Interfaces
{
    public interface IComputer
    {
        bool IsAlive();
        List<Operation> Progress();
        void EnqueueJob(Operation operation);
        bool AbortJob(string guid);
        List<Operation> FindJobs(Func<Operation, bool> condition);
        int TimeEstimate(Operation operation);
    }
}
