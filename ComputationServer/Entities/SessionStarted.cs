using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ComputationServer.WebService.Entities
{
    [DataContract]
    public class SessionStarted
    {
        [DataMember(Name = "sessionId")]
        public int SessionId { get; set; }
    }
}
