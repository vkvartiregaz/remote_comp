using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ComputationServer.Data.Enums;

namespace ComputationServer.Data.Entities
{
    public class Session : ICloneable
    {
        public string Id { get; set; }

        public List<Job> Jobs { get; set; } = new List<Job>();

        public List<DataTransfer> DataTransfers { get; set; } = new List<DataTransfer>();

        public DateTime Deadline { get; set; }

        public decimal Budget { get; set; }

        public ExecutionStatus Status { get; set; }

        public object Clone()
        {
            var result = new Session();
            result.Id = this.Id;
            result.Deadline = this.Deadline;
            result.Budget = this.Budget;
            result.Status = this.Status;

            foreach (var job in this.Jobs)
                result.Jobs.Add(job.Clone() as Job);

            return result;
        }
    }
}
