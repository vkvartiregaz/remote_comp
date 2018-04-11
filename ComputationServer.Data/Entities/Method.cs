using System;
using System.Collections.Generic;
using System.Text;

namespace ComputationServer.Data.Entities
{
    public abstract class Method
    {
        public string Name { get; set; }
        public byte[] Binary { get; set; }

        public abstract double TimeEstimate(List<DataObject> inputData);
    }
}
