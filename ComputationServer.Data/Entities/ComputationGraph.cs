using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;


namespace ComputationServer.Data.Models
{
    [DataContract]
    public class ComputationGraph
    {
        [DataMember(Name = "operations")]
        public List<Operation> Operations { get; set; }

        [DataMember(Name = "dependencies")]
        public List<List<int>> Dependencies { get; set; }

        [DataMember(Name = "mnemonicsTable")]
        public Dictionary<string, MnemonicValue> MnemonicsTable { get; set; }

        public ComputationGraph Diff(ComputationGraph from)
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            //TODO: Check for cycles
            return true;
        }
    }
}
