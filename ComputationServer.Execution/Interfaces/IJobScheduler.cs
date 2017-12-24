using System;
using System.Collections.Generic;
using System.Text;
using ComputationServer.Data.Entities;
using ComputationServer.Execution.ComputerAccess;

namespace ComputationServer.Execution.Interfaces
{
    public interface IJobScheduler
    {
        Dictionary<Operation, IComputer> Schedule(Session session, List<IComputer> computers);
        Dictionary<Operation, IComputer> Reschedule(Session session, List<IComputer> computers);
    }
}
