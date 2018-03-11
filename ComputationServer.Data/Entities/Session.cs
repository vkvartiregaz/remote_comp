using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ComputationServer.Data.Entities
{
    public class Session : ICloneable
    {
        [DataMember(Name = "id", IsRequired = false)]
        public string Id { get; set; }

        //[DataMember(Name = "computationGraph")]
        public List<Job> Jobs { get; set; } = new List<Job>();

        [DataMember(Name = "deadline", IsRequired = false)]
        public DateTime Deadline { get; set; }

        [DataMember(Name = "budget", IsRequired = false)]
        public decimal Budget { get; set; }

        public Status Status { get; set; }

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
