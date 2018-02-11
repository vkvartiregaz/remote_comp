using ComputationServer.Data.Models;
using ComputationServer.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Data.Repositories
{
    public class DbSessionRepository : ISessionRepository
    {
        public bool Add(Session session)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Session session)
        {
            throw new NotImplementedException();
        }

        public Session FindById(string sessionId)
        {
            throw new NotImplementedException();
        }

        public List<Session> FindAll(Func<Session, bool> condition)
        {
            throw new NotImplementedException();
        }

        public bool Update(Session session)
        {
            throw new NotImplementedException();
        }
    }
}
