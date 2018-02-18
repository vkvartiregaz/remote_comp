using ComputationServer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Execution.Interfaces
{
    public interface IExecutionManager
    {
        string StartSession(Session session);
        Session GetSessionStatus(string id);
        bool StopSession(string id);
        bool ModifySession(Session session);
    }
}
