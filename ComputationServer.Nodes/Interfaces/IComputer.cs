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
        void Progress();
        int GetJobETA(Operation operation);
        bool EnqueueJob(Operation operation);
        bool AbortJob(Operation operation);
        List<Operation> FindJobs(Func<Operation, bool> condition);
        int TimeEstimate(Operation operation);
    }
}
