using ComputationServer.Data.Entities;
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
        List<Job> Progress();
        void EnqueueJob(Job operation);
        bool AbortJob(string guid);
        List<Job> FindJobs(Func<Job, bool> condition);
        int TimeEstimate(Job operation);
    }
}
