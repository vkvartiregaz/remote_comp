using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ComputationServer.Data.Models
{
    [DataContract]
    public class DataType
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "parameters")]
        public List<DataType> Parameters { get; set; }
    }
}
