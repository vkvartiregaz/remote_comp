using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ComputationServer.Data.Entities
{
    [DataContract]
    public class DataType
    {
        public string Name { get; set; }
        
        public List<DataType> Parameters { get; set; }
    }
}
