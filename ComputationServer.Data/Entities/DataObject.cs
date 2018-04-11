using System;
using System.Collections.Generic;
using System.Text;
using ComputationServer.Data.Enums;

namespace ComputationServer.Data.Entities
{
    public class DataObject
    {
        public DataType Type { get; set; }
        public DataStatus Status { get; set; }
        public Dictionary<string, double> Metrics { get; set; }
        public string Source { get; set; }
        public string Id { get; set; }
    }
}
