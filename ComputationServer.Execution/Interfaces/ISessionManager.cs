using ComputationServer.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Execution.Interfaces
{
    public interface ISessionManager
    {
        bool RegisterSession(Session session);
        bool UpdateSession(Session session);
        bool ArchiveSession(Session session);
        Session FindSession(string id);
        List<Session> Active { get; }
    }
}
