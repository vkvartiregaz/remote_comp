using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ComputationServer.WebService.Models
{
    public class SessionRequest
    {
        [JsonProperty(PropertyName = "computationGraph")]
        public ComputationGraph ComputationGraph { get; set; }

        [JsonProperty(PropertyName = "deadline")]
        public DateTime Deadline { get; set; }

        [JsonProperty(PropertyName = "budget")]
        public decimal Budget { get; set; }
    }

    public class ComputationGraph
    {
        [JsonProperty(PropertyName = "operations")]
        public List<Operation> Operations { get; set; }

        [JsonProperty(PropertyName = "dependencies")]
        public List<List<int>> Dependencies { get; set; }

        [JsonProperty(PropertyName = "mnemonicsTable")]
        public Dictionary<string, MnemonicsValue> MnemonicsTable { get; set; }
    }

    public class Operation
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "input")]
        public List<string> input { get; set; }

        [JsonProperty(PropertyName = "output")]
        public List<string> output { get; set; }
    }

    public class DataType
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "parameters")]
        public List<DataType> Parameters { get; set; }
    }

    public class MnemonicsValue
    {
        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        [JsonProperty(PropertyName = "type")]
        public DataType Type { get; set; }
    }
}
