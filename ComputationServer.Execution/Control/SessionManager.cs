using ComputationServer.Data.Interfaces;
using ComputationServer.Data.Entities;
using ComputationServer.Execution.Interfaces;
using ComputationServer.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ComputationServer.Data.Enums;

namespace ComputationServer.Execution.Control
{
    public class SessionManager : ISessionManager
    {        
        private ConcurrentDictionary<string, Session> _sessions = new ConcurrentDictionary<string, Session>();
        private object _sessionsState = new object();

        public List<Session> Active
        {
            get
            {
                lock (_sessionsState)
                {
                    var activeStatuses = new List<ExecutionStatus> { ExecutionStatus.PROCESSING, ExecutionStatus.QUEUED, ExecutionStatus.RUNNING };
                    var active = (from s in _sessions.Values
                                  where activeStatuses.Contains(s.Status)
                                  select s).ToList();

                    return Replicator.CopyAll(active);
                }
            }
        }

        public SessionManager()
        {
            
        }

        public bool ArchiveSession(Session session)
        {
            throw new NotImplementedException();
        }

        public Session FindSession(string id)
        {
            Session result;

            lock (_sessionsState)
            {
                var found = _sessions.TryGetValue(id, out result);

                if (!found)
                    return null;

                return result.Clone() as Session;
            }
        }

        public bool RegisterSession(Session session)
        {
            var id = Guid.NewGuid().ToString();
            session.Id = id;
            session.Status = ExecutionStatus.PROCESSING;

            lock (_sessionsState)
            {
                if (!_sessions.TryAdd(id, session))
                    return false;
            }

            return true;
        }

        public bool UpdateSession(Session session)
        {
            lock(_sessionsState)
            {
                if (!_sessions.ContainsKey(session.Id))
                    return false;

                Session oldSession;

                if (!_sessions.TryGetValue(session.Id, out oldSession))
                    return false;

                if (!_sessions.TryUpdate(session.Id, session, oldSession))
                    return false;
            }

            return true;
        }
    }
}
