using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ComputationServer.Data.Entities
{
    [DataContract]
    public class Job : ICloneable
    {
        [DataMember(Name = "id")]
        public int LocalId { get; set; }

        [DataMember(Name = "name")] //IsRequired = true is to be negotiated
        public string Name { get; set; }

        [DataMember(Name = "input", IsRequired = false)]
        public List<DataObject> Input { get; set; }

        [DataMember(Name = "output", IsRequired = false)]
        public List<DataObject> Output { get; set; }

        public List<string> Dependencies { get; set; }

        private Status _status;

        [DataMember(Name = "status", IsRequired = false)]
        public Status Status
        {
            get
            {
                lock (_hold)
                {
                    return _status;
                }
            }

            set
            {
                lock (_hold)
                {
                    _status = value;
                }
            }
        }

        public Session Session { get; set; }

        public string Guid { get; set; }

        private object _hold = new object();

        public object Clone()
        {
            var clone = new Job();
            clone.Guid = Guid;
            clone.LocalId = LocalId;
            clone.Name = Name;
            clone.Input = Input;
            clone.Output = Output;
            clone.Status = Status;
            clone.Session = Session;
            return clone;
        }
    }
}
