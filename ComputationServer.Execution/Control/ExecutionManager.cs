using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ComputationServer.Data.Models;
using ComputationServer.Scheduling.Interfaces;
using ComputationServer.Scheduling.Schedulers;
using ComputationServer.Nodes.Interfaces;
using ComputationServer.Nodes.AccessModules;
using ComputationServer.Data.Interfaces;
using ComputationServer.Execution.Interfaces;
using ComputationServer.Scheduling.Models;

namespace ComputationServer.Execution.Control
{
    public class ExecutionManager : IExecutionManager
    {
        private List<IScheduler> _schedulers;
        private List<IComputer> _computers;
        private ISessionManager _sessionManager;
        
        public ExecutionManager(List<IScheduler> schedulers, 
            List<IComputer> computers,
            ISessionManager sessionManager,
            int monitorInterval)
        {
            _schedulers = schedulers;
            _computers = computers;
            _sessionManager = sessionManager;

            StartMonitor(monitorInterval);
        }


        public string StartSession(Session session)
        {
            if(!_sessionManager.RegisterSession(session))
                return null;

            var schedule = ScheduleSession(session);

            if (schedule == null)
            {
                session.Status = Status.FAILED;
                _sessionManager.UpdateSession(session);
                return null;
            }

            StartScheduled(schedule);
            session.Status = Status.RUNNING;
            _sessionManager.UpdateSession(session);

            return session.Id;
        }

        public Session GetSessionStatus(string id)
        {
            return _sessionManager.FindSession(id);
        }

        public bool StopSession(string id)
        {
            var toStop = _sessionManager.FindSession(id);

            if (toStop == null)
                return true;

            switch(toStop.Status)
            {
                case Status.COMPLETED:
                case Status.ABORTED:
                case Status.FAILED:
                {
                    return true;
                }
                case Status.PROCESSING:
                case Status.QUEUED:
                case Status.RUNNING:
                {
                    var aborted = AbortSession(toStop);

                    if (!aborted)
                        return false;

                    toStop.Status = Status.ABORTED;
                    _sessionManager.UpdateSession(toStop);

                    return true;
                }

                case Status.UNKNOWN:
                {
                    return false;
                }

                default:
                {
                    return false;
                }
            }            
        }

        public bool ModifySession(Session session)
        {
            throw new NotImplementedException();
        }
        
        private void StartScheduled(Schedule schedule)
        {
            foreach (var entry in schedule.Assigned)
            {
                var computer = entry.Value;
                var job = entry.Key;
                computer.EnqueueJob(job);                    
            }            
        }

        private bool AbortSession(Session session)
        {
            foreach (var c in _computers)
            {
                var fromSession = c.FindJobs(job => job.SessionId == session.Id);

                foreach(var job in fromSession)
                {
                    if(!c.AbortJob(job.Guid))
                        return false;
                }
            }

            return true;
        }

        private List<IComputer> AliveOnly()
        {
            return (from c in _computers
                    where c.IsAlive()
                    select c).ToList();
        }

        private Schedule ScheduleSession(Session session)
        {
            var alive = AliveOnly();

            if (!alive.Any())
                return null;

            var schedules = Task.WhenAll(_schedulers.Select(s => s.ScheduleSessionAsync(session, alive))).Result;

            if (!schedules.Any(s => s != null))
                return null;

            var best = new Schedule { EstimatedTime = int.MaxValue };

            foreach (var s in schedules)
                if (s.EstimatedTime < best.EstimatedTime)
                    best = s;

            return best;
        }

        private void StartMonitor(int interval)
        {
            Task.Run(() => Monitor(interval));
        }

        private async void Monitor(int interval)
        {
            while (true)
            {
                var changed = new List<Operation>();

                foreach (var c in _computers)
                    changed.InsertRange(0, c.Progress());

                var activeSessions = _sessionManager.Active;

                foreach (var session in activeSessions)
                {
                    var toUpdate = (from op in changed
                                     where op.SessionId == session.Id
                                     select op).ToList();

                    foreach(var operation in toUpdate)
                    {
                        var old = session.CompGraph.Operations.Where(op => op.Guid == operation.Guid).FirstOrDefault();
                        old = operation;
                    }

                    var sessionJobs = session.CompGraph.Operations;

                    if (sessionJobs.All(j => j.Status == Status.COMPLETED))
                        session.Status = Status.COMPLETED;
                    
                    if(sessionJobs.Any(j => j.Status == Status.FAILED || j.Status == Status.UNKNOWN))
                    {
                        var rescheduled = RescheduleSession(session);

                        if (!rescheduled)
                            session.Status = Status.FAILED;
                    }

                    _sessionManager.UpdateSession(session);
                }

                await Task.Delay(interval * 1000);
            }
        }

        private bool RescheduleSession(Session session)
        {
            var alive = AliveOnly();

            if (!alive.Any())
                return false;

            var schedules = Task.WhenAll(_schedulers.Select(s => s.ScheduleSessionAsync(session, alive))).Result;

            if (!schedules.Any(s => s != null))
                return false;

            var best = new Schedule { EstimatedTime = int.MaxValue };

            foreach (var s in schedules)
                if (s.EstimatedTime < best.EstimatedTime)
                    best = s;

            StartScheduled(best);

            return true;
        }

    }
}
