using ComputationServer.Data.Entities;
using ComputationServer.Data.Enums;
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
        Dictionary<string, ExecutionStatus> Progress();
        void EnqueueJob(Job operation);
        bool AbortJob(string guid);
        List<Job> FindJobs(Func<Job, bool> condition);
        double TimeEstimate(Job operation);

        decimal ChargeRate { get; }
        string ComputerId { get; }
    }
}
