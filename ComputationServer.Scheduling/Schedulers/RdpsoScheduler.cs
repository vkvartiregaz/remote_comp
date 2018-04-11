using ComputationServer.Data.Entities;
using ComputationServer.Nodes.Interfaces;
using ComputationServer.Scheduling.Interfaces;
using ComputationServer.Scheduling.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ComputationServer.Scheduling.Schedulers
{
    public class RdpsoScheduler : IScheduler
    {
        private int _particleCount = 20;
        private int _itMax;
        private double _inertiaWeight;
        private double _accelCoef1;
        private double _accelCoef2;
        private double _constrictionFactor;
        private Random _rng;

        private double _fitnessTimeWeight = 0.5;
        private double _fitnessCostWeight = 0.5;
        
        public RdpsoScheduler()
        {
            _rng = new Random(DateTime.Now.Millisecond);
        }

        public async Task<Schedule> RescheduleSessionAsync(Session session, List<IComputer> computers)
        {
            var culled = CullSession(session);

            return await ScheduleSessionAsync(culled, computers);            
        }

        public async Task<Schedule> ScheduleSessionAsync(Session session, List<IComputer> computers)
        {
            var jobs = session.Jobs;

            var swarm = InitializeSwarm(jobs, computers);

            var bestFound = SwarmSchedule(swarm, session);

            return MakeSchedule(bestFound);
        }

        private Session CullSession(Session original)
        {
            throw new NotImplementedException();
        }

        private List<Particle> InitializeSwarm(List<Job> jobs, List<IComputer> computers)
        {
            var result = new List<Particle>();
            var computerCount = computers.Count;           

            for (int i = 0; i < _particleCount; ++i)
            {
                var particle = new Particle();

                foreach (var job in jobs)
                {
                    var idx = _rng.Next() % computerCount;
                    var assignment = new Assignment { Job = job, Computer = computers[idx] };
                    particle.Position.Add(assignment);
                }

                result.Add(particle);
            }

            return result;
        }

        private List<Assignment> SwarmSchedule(List<Particle> swarm,
            Session session)
        {
            foreach (var p in swarm)
                p.CalcBest();

            var globalBest = CalcGlobalBest(swarm, swarm.First().Position);
            var itCount = 0;

            while (itCount < _itMax &&
                !GoodEnough(globalBest, session))
            {
                foreach(var p in swarm)
                {
                    p.UpdateVelocity(globalBest);
                    p.Move();
                    p.CalcBest();
                }

                globalBest = CalcGlobalBest(swarm, globalBest);
            }

            return globalBest;            
        }

        private double CalcFitness(List<Assignment> position)
        {
            var execTime = 0.0;

            foreach(var pair in position)
                execTime += pair.Computer.TimeEstimate(pair.Job);

            var execCost = 0.0m;
            var dtTime = 0.0;
            var dtCost = 0.0m;

            var fitness = _fitnessTimeWeight * (execTime + dtTime) +
                _fitnessCostWeight * (double)(execCost + dtCost);

            return fitness;
        }

        private List<Assignment> CalcGlobalBest(List<Particle> swarm, List<Assignment> oldBest)
        {
            var bestFitness = CalcFitness(oldBest);
            List<Assignment> newBest = oldBest;

            foreach (var p in swarm)
            {
                var pBest = p.Best;

                if (CalcFitness(pBest) < bestFitness)
                    newBest = pBest;
            }

            return newBest;
        }

        private bool GoodEnough(List<Assignment> position,
            Session session)
        {
            var execTime = CalcTimeTotal(position);

            if (DateTime.UtcNow.AddSeconds(execTime) > session.Deadline)
                return false;

            var execCost = CalcCostTotal(position);

            if (execCost > session.Budget)
                return false;

            return true;
        }

        private Schedule MakeSchedule(List<Assignment> position)
        {
            var result = new Schedule();

            foreach (var pair in position)
                result.Assigned.Add(pair.Job, pair.Computer);

            result.EstimatedTime = CalcTimeTotal(position);
            result.EstimatedCost = CalcCostTotal(position);

            return result;
        }
        
        private double CalcTimeTotal(List<Assignment> position)
        {
            var result = 0.0;

            foreach (var pair in position)
                result += pair.Computer.TimeEstimate(pair.Job);

            //detect data trnsfers

            //calc time for each transfer and add to total

            return result;
        }

        private decimal CalcCostTotal(List<Assignment> position)
        {
            return 0.0m;
        }

        private class Particle
        {
            public List<Assignment> Position { get; set; } = new List<Assignment>();
            public List<Assignment> Best { get; set; }

            private Velocity _velocity = new Velocity();

            public void UpdateVelocity(List<Assignment> globalBest)
            {

            }

            public void Move()
            {

            }

            public void CalcBest()
            {
                 
            }

            public static Particle operator - (Particle v1, Particle v2)
            {
                var result = new Particle();

                foreach (var pair in v1.Position)
                    if (!v2.Position.Contains(pair))
                        result.Position.Add(pair);

                return result;
            }            
        }

        private class Velocity
        {
            public Dictionary<Assignment, double> Probabilities = new Dictionary<Assignment, double>();

            /*public static Velocity operator * (Velocity velocity, double coef)
            {
                var probabilities = new List<double>();

                foreach (var prob in velocity.Probabilities)
                {
                    var newProb = prob * coef > 1 ? 1 : prob * coef;
                    probabilities.Add(newProb);
                }

                var result = new Velocity
                {
                    Job = velocity.Job,
                    Probabilities = probabilities
                };

                return result;
            }

            public static Velocity operator + (Velocity v1, Velocity v2)
            {
                if (v1.Job.Guid != v2.Job.Guid
                    || v1.Probabilities.Count != v2.Probabilities.Count)                    
                    throw new InvalidOperationException("Only velocities on the same axis can be added");

                var probabilities = new List<double>();
                var count = v1.Probabilities.Count;

                for(int i = 0; i < count; ++i)
                {
                    var newProb = v1.Probabilities[i] > v2.Probabilities[i] ? 
                        v1.Probabilities[i] : 
                        v2.Probabilities[i];

                    probabilities.Add(newProb);
                }

                var result = new Velocity
                {
                    Job = v1.Job,
                    Probabilities = probabilities
                };

                return result;
            }*/
        }

        private class Assignment
        {
            public Job Job { get; set; }
            public IComputer Computer { get; set; }
        }
    }
}
