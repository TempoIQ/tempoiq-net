using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoIQ.Json;

namespace TempoIQ.Models
{
    [JsonObject]
    public class Device : Model
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("attributes")]
        public IDictionary<string, string> Attributes { get; set; }

        [JsonProperty("sensors")]
        public IList<Sensor> Sensors { get; set; }

        public Device(string key, string name, IDictionary<string, string> attributes, IList<Sensor> sensors)
        {
            this.Key = key;
            this.Name = name;
            this.Attributes = attributes;
            this.Sensors = sensors;
        }
        public Device(string key)
        {
            this.Key = key;
            this.Name = "";
            this.Attributes = new Dictionary<string, string>();
            this.Sensors = new List<Sensor>();
        }
    }

    [JsonObject]
    public class Sensor : Model
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("attributes")]
        public IDictionary<string, string> Attributes { get; set; }
        
        public Sensor(string key, string name, IDictionary<string, string> attributes)
        {
            this.Key = key;
            this.Name = name;
            this.Attributes = attributes;
        }
        public Sensor(string key)
        {
            this.Key = key;
            this.Name = "";
            this.Attributes = new Dictionary<string, string>();
        }
    }
}