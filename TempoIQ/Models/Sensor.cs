using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TempoIQ.Json;
using TempoIQ.Utilities;

namespace TempoIQ.Models
{

    /// <summary>
    /// The TempoIQ notion of a Sensor.
    /// Sensors emit streams of numeric data.
    /// </summary> 
    [JsonObject]
    public class Sensor : Model
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("attributes")]
        public IDictionary<string, string> Attributes { get; set; }
        
        [JsonConstructor]
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

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            else if (obj == this)
                return true;
            else if (obj is Sensor)
                return this.Equals((Sensor)obj);
            else
                return false;
        }

        public bool Equals(Device that)
        {
            return this.Key.Equals(that.Key)
                && this.Attributes.SequenceEqual(that.Attributes)
                && this.Name.Equals(that.Name);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Attributes);
            hash = HashCodeHelper.Hash(hash, Key);
            hash = HashCodeHelper.Hash(hash, Name);
            return hash;
        }
    }
}
