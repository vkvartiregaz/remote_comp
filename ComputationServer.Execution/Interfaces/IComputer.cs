using System;
using System.Collections.Generic;
using System.Text;
using ComputationServer.Data.Entities;

namespace ComputationServer.Execution.Interfaces
{
    public interface IComputer
    {
        List<Guid> AssignedOps { get; set; }

        string Type { get; set; }

        string Address { get; set; }

        void Progress();

        bool StartJob(Operation task);

        Operation CheckJob(Guid globalId);

        void StopJob(Guid globalId);

        bool IsAlive();
    }
}
