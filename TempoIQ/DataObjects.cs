using System;
using System.Collections;
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

    [JsonObject]
    public class Segment<T> : IEnumerable<T>
    {
        [JsonProperty("data")]
        protected IList<T> Data{ get; set; }

        [JsonProperty("next")]
        protected string Next { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in this.Data)
            {
                yield return item;
            }
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Segment(IList<T> data, string next)
        {
            this.Data = data;
            this.Next = next;
        }
    }



}