using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ComputationServer.WebService.Entities
{
    [DataContract]
    public class ModifySessionParameters
    {
        [DataMember(Name = "deadline")]
        public DateTime Deadline { get; set; }

        [DataMember(Name = "budget")]
        public decimal Budget { get; set; }        
    }
}
