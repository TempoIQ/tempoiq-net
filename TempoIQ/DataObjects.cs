using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TempoIQ.Models
{
    [JsonObject]
    public struct Datapoint
    {
        [JsonProperty("timestamp")]
        public DateTime timestamp;
        [JsonProperty("value")]
        public double value;
    }

    public interface Cursor<T> : IEnumerable<T> { }

    [JsonObject]
    public class Segment<T> : IEnumerable<T>
    {
        [JsonArray("data")]
        protected IList<T> Data{ get; set; }

        [JsonProperty("next")]
        protected string Next { get; set; }

        public Segment(IList<T> data, string next)
        {
            this.Data = data;
            this.Next = next;
        }
    }
}