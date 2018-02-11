using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ComputationServer.Data.Models
{
    [DataContract]
    public class Operation
    {
        [DataMember(Name = "id")]
        public int LocalId { get; set; }

        [DataMember(Name = "name")] //IsRequired = true is to be negotiated
        public string Name { get; set; }

        [DataMember(Name = "input", IsRequired = false)]
        public List<DataType> Input { get; set; }

        [DataMember(Name = "output", IsRequired = false)]
        public List<DataType> Output { get; set; }

        private Status _status;

        [DataMember(Name = "status", IsRequired = false)]
        public Status Status
        {
            get { return _status; }

            set
            {
                lock(this)
                {
                    _status = value;
                }
            }
        }

        public string SessionId { get; set; }
    }
}
