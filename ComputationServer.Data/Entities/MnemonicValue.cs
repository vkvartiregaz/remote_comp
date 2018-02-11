using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ComputationServer.Data.Models
{
    [DataContract]
    public class MnemonicValue
    {
        [DataMember(Name = "value")]
        public string Value { get; set; }

        [DataMember(Name = "type")]
        public DataType Type { get; set; }
    }
}
