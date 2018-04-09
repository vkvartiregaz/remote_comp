using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ComputationServer.Data.Enums;

namespace ComputationServer.Data.Entities
{
    [DataContract]
    public class Job : ICloneable
    {
        public int LocalId { get; set; }

        public string Name { get; set; }

        public List<DataObject> Input { get; set; }

        public List<DataObject> Output { get; set; }

        public List<string> Dependencies { get; set; }

        private ExecutionStatus _status;

        public ExecutionStatus Status
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
            clone.Dependencies = Dependencies;
            clone.Status = Status;
            clone.Session = Session;
            return clone;
        }
    }
}
