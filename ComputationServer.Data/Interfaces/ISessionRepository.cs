using ComputationServer.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Data.Interfaces
{
    public interface ISessionRepository
    {
        bool Add(Session session);
        bool Update(Session session);
        Session FindById(string sessionId);
        List<Session> FindAll(Func<Session, bool> condition);
        bool Delete(Session session);

    }
}
