using ComputationServer.Data.Models;
using ComputationServer.Nodes.Interfaces;
using ComputationServer.Scheduling.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ComputationServer.Scheduling.Interfaces
{
    public interface IScheduler
    {
        Task<Schedule> ScheduleSessionAsync(Session session, List<IComputer> computers);
        Task<Schedule> RescheduleSessionAsync(Session session, List<IComputer> computers);
    }
}
