using System;
using System.Collections.Generic;
using System.Text;
using ComputationServer.Data.Entities;
using ComputationServer.Execution.Interfaces;

namespace ComputationServer.Execution.ComputerAccess
{
    class LocalComputer : IComputer
    {
        public List<Guid> AssignedOps { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }

        public Operation CheckJob(Guid globalId)
        {
            throw new NotImplementedException();
        }

        public bool IsAlive()
        {
            //started server == can start processes == is alive
            //should be ok unless some extreme cases make OS refuse new processes being created
            //hope to implement in-memory data processing by the time this concern becomes relevant
            return true;
        }

        public void Progress()
        {
            throw new NotImplementedException();
        }

        public bool StartJob(Operation task)
        {
            throw new NotImplementedException();
        }

        public void StopJob(Guid globalId)
        {
            throw new NotImplementedException();
        }
    }
}
