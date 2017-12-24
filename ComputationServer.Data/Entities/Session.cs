using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ComputationServer.Data.Entities
{
    public class Session
    {
        [DataMember(Name = "id", IsRequired = false)]
        public int Id { get; set; }

        [DataMember(Name = "computationGraph")]
        public ComputationGraph CompGraph { get; set; }

        [DataMember(Name = "deadline", IsRequired = false)]
        public DateTime Deadline { get; set; }

        [DataMember(Name = "budget", IsRequired = false)]
        public decimal Budget { get; set; }

        public Status Status { get; set; }

        public void Link()
        {
            foreach (var op in CompGraph.Operations)
                op.SessionId = Id;
        }
    }
}
