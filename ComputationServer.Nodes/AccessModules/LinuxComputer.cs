using ComputationServer.Data.Models;
using ComputationServer.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationServer.Nodes.AccessModules
{
    public class LinuxComputer : IComputer
    {
        public bool EnqueueJob(Operation operation)
        {
            throw new NotImplementedException();
        }

        public DateTime GetJobETA(Operation operation)
        {
            throw new NotImplementedException();
        }

        public bool IsAlive()
        {
            throw new NotImplementedException();
        }

        public void Progress()
        {
            throw new NotImplementedException();
        }

        public bool StopJob(Operation operation)
        {
            throw new NotImplementedException();
        }
    }
}
