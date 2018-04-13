using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Data.Entities
{
    public class DataTransfer
    {
        public DataObject Data { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
