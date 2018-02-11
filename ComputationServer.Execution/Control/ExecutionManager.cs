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
        private List<IScheduler> _schedulers = new List<IScheduler> { new NaiveScheduler() };
        private List<IComputer> _computers = new List<IComputer> { new LocalComputer() };
        private ISessionRepository _sessionRepository;
        
        public ExecutionManager(List<IScheduler> schedulers, 
            List<IComputer> computers,
            ISessionRepository sessionRepository,
            int monitorInterval)
        {
            _schedulers = schedulers;
            _computers = computers;
            _sessionRepository = sessionRepository;

            StartMonitor(monitorInterval);
        }


        public string StartSession(Session session)
        {
            var id = Guid.NewGuid().ToString();
            session.Id = id;
            session.Status = Status.PROCESSING;
            _sessionRepository.Add(session);

            var schedule = ScheduleSession(session);

            if (schedule == null)
            {
                session.Status = Status.FAILED;
                _sessionRepository.Update(session);
                return null;
            }

            var started = StartScheduled(schedule);

            if (!started)
            {
                session.Status = Status.FAILED;
                _sessionRepository.Update(session);
                return null;
            }

            session.Status = Status.RUNNING;
            _sessionRepository.Update(session);

            return id;
        }

        public Session GetSessionStatus(string id)
        {
            return _sessionRepository.FindById(id);
        }

        public bool StopSession(string id)
        {
            var toStop = _sessionRepository.FindById(id);

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
                    //transaction start?
                    var aborted = AbortSession(toStop);

                    if (!aborted)
                        return false;

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
        
        private bool StartScheduled(Schedule schedule)
        {
            foreach (var entry in schedule.Assigned)
            {
                var computer = entry.Value;
                var job = entry.Key;

                if (!computer.EnqueueJob(job))
                    return false;
            }

            return true;
        }

        private bool AbortSession(Session session)
        {
            foreach (var c in _computers)
            {
                var fromSession = c.FindJobs(job => job.SessionId == session.Id);

                foreach(var job in fromSession)
                {
                    if(!c.StopJob(job))
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
                foreach (var c in _computers)
                    c.Progress();

                var activeStatuses = new List<Status> { Status.PROCESSING, Status.QUEUED, Status.RUNNING };
                var activeSessions = _sessionRepository.FindAll(s => activeStatuses.Contains(s.Status));

                foreach (var s in activeSessions)
                {
                    var sessionJobs = new List<Operation>();

                    foreach (var c in _computers)
                        sessionJobs.InsertRange(0, c.FindJobs(j => j.SessionId == s.Id));

                    if (sessionJobs.All(j => j.Status == Status.COMPLETED))
                        s.Status = Status.COMPLETED;
                    
                    if(sessionJobs.Any(j => j.Status == Status.FAILED || j.Status == Status.UNKNOWN))
                    {
                        var rescheduled = RescheduleSession(s);

                        if (!rescheduled)
                            s.Status = Status.FAILED;
                    }
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

            var started = StartScheduled(best);

            if (!started)
                return false;

            return true;
        }

    }
}
