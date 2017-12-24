using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ComputationServer.Data.Entities;
using ComputationServer.Execution.ComputerAccess;

namespace ComputationServer.Execution.Scheduling
{
    //very simple scheduler: all jobs go to the first computer on the list
    //for testing purposes only
    class NaiveScheduler : IJobScheduler
    {
        public Dictionary<Operation, Computer> Reschedule(Session session, List<Computer> computers)
        {
            var theChosenOne = computers[0];
            var result = new Dictionary<Operation, Computer>();

            if (theChosenOne != null)
                foreach (var job in session.CompGraph.Operations)
                    result[job] = theChosenOne;

            return result;
        }

        public Dictionary<Operation, Computer> Schedule(Session session, List<Computer> computers)
        {
            var theChosenOne = computers[0];
            var result = new Dictionary<Operation, Computer>();

            var toReschedule = (from j in session.CompGraph.Operations
                                where j.Status != Status.COMPLETED || j.Status != Status.RUNNING
                                select j).ToList();

            if (theChosenOne != null)
                foreach (var job in toReschedule)
                    result[job] = theChosenOne;

            return result;
        }
    }
}
