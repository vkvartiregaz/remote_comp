﻿using ComputationServer.Data.Models;
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
        public Task<Schedule> RescheduleSessionAsync(Session session, List<IComputer> computers)
        {
            throw new NotImplementedException();
        }

        public Task<Schedule> ScheduleSessionAsync(Session session, List<IComputer> computers)
        {
            throw new NotImplementedException();
        }
    }
}
