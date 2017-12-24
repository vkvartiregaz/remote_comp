using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ComputationServer.Data.Entities;
using ComputationServer.Execution.Scheduling;
using ComputationServer.Execution.ComputerAccess;

namespace ComputationServer.Execution.Control
{
    public class ExecutionManager
    {
        private IJobScheduler _scheduler = new NaiveScheduler();
        private List<Computer> _computers;
        //because keeping this pile in DB would be too much trouble
        private ConcurrentDictionary<Session, int> _activeSessions = new ConcurrentDictionary<Session, int>();
        
        //job monitor period in seconds
        private int _monitorPeriod = 30;
        private CancellationTokenSource janitorStopper = new CancellationTokenSource();

        //yes, this is a session id counter
        //no, I am not sure that I am ashamed, since I am bound into having an int id
        private int _sessionsProcessed = 0;

        public ExecutionManager()
        {
            //TODO: Initialize computer list from config file

            //set up monitor worker
            IObservable<long> janitor = Observable.Interval(TimeSpan.FromSeconds(_monitorPeriod));
            Action monitorAction = Monitor;
            janitor.Subscribe(x => Task.Run(monitorAction), janitorStopper.Token);
        }

        public bool StartSession(Session session)
        {
            var alive = AliveOnly();

            if (!alive.Any())
                return false;

            var sessionId = _sessionsProcessed++;
            session.Id = sessionId;
            session.Link();
            _activeSessions[session] = sessionId;

            var schedule = _scheduler.Schedule(session, alive);

            if (schedule ==  null)
            {
                session.Status = Status.FAILED;
                ArchiveSession(session);
                return false;
            }
            
            var started = StartScheduled(schedule);

            if (!started)
            {
                session.Status = Status.FAILED;
                ArchiveSession(session);
                return false;
            }

            session.Status = Status.RUNNING;
            return true;
        }

        public Session CheckSession(int sessionId)
        {
            return (from s in _activeSessions.Keys
                    where s.Id == sessionId
                    select s).ToList().FirstOrDefault();
        }

        public bool StopSession(int sessionId)
        {
            var toStop = (from s in _activeSessions.Keys
                          where s.Id == sessionId
                          select s).ToList().FirstOrDefault();

            if (toStop == null)
                return false;

            AbortJobs(toStop.CompGraph.Operations);
            int dummy;
            _activeSessions.Remove(toStop, out dummy);//remove from active
            //record session into archive

            return true;
        }

        public bool ModifySession(Session session)
        {
            //I am genuinely scared by the prospect
            throw new NotImplementedException();
        }
        
        private bool StartScheduled(Dictionary<Operation, Computer> schedule)
        {
            foreach (var entry in schedule)
            {
                var computer = entry.Value;
                var job = entry.Key;

                if (!computer.StartJob(job))
                {
                    AbortJobs(schedule.Keys.ToList());
                    return false;
                }
            }

            return true;
        }

        private void AbortJobs(List<Operation> jobs)
        {
            foreach(var job in jobs)
            {
                var assignedTo = (from c in _computers
                                  where c.AssignedOps.Contains(job.GlobalId)
                                  select c).FirstOrDefault();

                if (assignedTo != null)
                    assignedTo.StopJob(job.GlobalId);
            }
        }

        private List<Computer> AliveOnly()
        {
            return (from c in _computers
                    where c.IsAlive()
                    select c).ToList();
        }
             
        private void ArchiveSession(Session session)
        {
            //remove from active, probably write something somewhere (DB or log?)
            throw new NotImplementedException();
        }

        private async void Monitor()
        {
            foreach (var c in _computers)
                c.Progress();

            foreach (var s in _activeSessions.Keys)
            {
                bool completed = s.CompGraph.Operations
                    .All(op => op.Status == Status.COMPLETED);

                if(completed)
                    s.Status = Status.COMPLETED;

                bool failed = s.CompGraph.Operations
                    .Any(op => op.Status == Status.FAILED || op.Status == Status.UNKNOWN);

                if (failed)
                    s.Status = Status.FAILED;
            }
        }
    }
}
