using ComputationServer.Data.Entities;
using ComputationServer.Nodes.Interfaces;
using ComputationServer.Scheduling.Interfaces;
using ComputationServer.Scheduling.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ComputationServer.Scheduling.Schedulers
{
    public class RdpsoScheduler : IScheduler
    {
        private int _particleCount;
        private int _itCount;
        private double _inertiaWeight;
        private double _accelCoef1;
        private double _accelCoef2;
        private double _constrictionFactor;
        private Particle _pBest;
        private double _gBest;
        private Random _rng;
        
        public RdpsoScheduler()
        {
            _rng = new Random(DateTime.Now.Millisecond);
        }

        public Task<Schedule> RescheduleSessionAsync(Session session, List<IComputer> computers)
        {
            throw new NotImplementedException();
            
        }

        public Task<Schedule> ScheduleSessionAsync(Session session, List<IComputer> computers)
        {
            var jobs = session.Jobs;

            var swarm = InitializeSwarm(jobs, computers);

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

        private Particle CalcPBest(List<Particle> swarm)
        {
            throw new NotImplementedException();
        }

        private Particle CalcGBest(List<Particle> swarm)
        {
            throw new NotImplementedException();
        }

        private void UpdateVelocity(Particle particle)
        {
            /*var oldV = particle.Velocity.Pairs;
            var newV = new List<List<Assignment>>();
            
            for(int i = 0; i < oldV.Count; ++i)
            {
                var r1 = _rng.NextDouble();
                var r2 = _rng.NextDouble();

                var curr = _constrictionFactor *
                    (
                        _inertiaWeight * oldV[i] +
                        _accelCoef1 * r1 * (pid - xid) +
                        _accelCoef2 * r2 * (pgb - xid)
                    );

                newV.Add(curr);
            }

            particle.Velocity = newV;*/
        }

        private class Particle
        {
            public List<Assignment> Position { get; set; } = new List<Assignment>();
            public List<Velocity> ResultVelocity { get; set; } = new List<Velocity>();

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
            public List<double> Probabilities { get; set; }
            public Job Job { get; set; }

            public static Velocity operator * (Velocity velocity, double coef)
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
            }
        }

        private class Assignment
        {
            public Job Job { get; set; }
            public IComputer Computer { get; set; }
        }
    }
}
