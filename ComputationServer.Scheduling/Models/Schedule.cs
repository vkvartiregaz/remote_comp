using ComputationServer.Data.Entities;
using ComputationServer.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Scheduling.Models
{
    public class Schedule
    {
        public Dictionary<Job, IComputer> Assigned { get; set; } = new Dictionary<Job, IComputer>();
        public double EstimatedTime { get; set; }
        public decimal EstimatedCost { get; set; }
    }
}
