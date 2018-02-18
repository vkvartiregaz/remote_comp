using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ComputationServer.Data.Models
{
    public class Session : ICloneable
    {
        [DataMember(Name = "id", IsRequired = false)]
        public string Id { get; set; }

        [DataMember(Name = "computationGraph")]
        public ComputationGraph CompGraph { get; set; }

        [DataMember(Name = "deadline", IsRequired = false)]
        public DateTime Deadline { get; set; }

        [DataMember(Name = "budget", IsRequired = false)]
        public decimal Budget { get; set; }

        public Status Status { get; set; }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
