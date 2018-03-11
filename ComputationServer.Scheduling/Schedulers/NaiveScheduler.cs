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
    public class NaiveScheduler : IScheduler
    {
        public async Task<Schedule> RescheduleSessionAsync(Session session, List<IComputer> computers)
        {
            throw new NotImplementedException();
        }

        public async Task<Schedule> ScheduleSessionAsync(Session session, List<IComputer> computers)
        {
            var result = new Schedule();

            foreach (var job in session.Jobs)
            {
                result.Assigned.Add(job, computers[0]);
            }

            return result;
        }
    }
}
