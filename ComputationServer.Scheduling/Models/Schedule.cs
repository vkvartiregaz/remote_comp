using ComputationServer.Data.Models;
using ComputationServer.Nodes.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Scheduling.Models
{
    public class Schedule
    {
        public Dictionary<Operation, IComputer> Assigned { get; set; }
        public int EstimatedTime { get; set; }
        public decimal EstimatedCost { get; set; }
    }
}
