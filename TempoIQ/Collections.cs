using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NodaTime;

namespace TempoIQ.Models.Collections
{
    [JsonObject]
    public class Segment<T> : IEnumerable<T>, Model
    {
        [JsonProperty("data")]
        protected IList<T> Data{ get; set; }

        [JsonProperty("next")]
        protected string Next { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            foreach(var item in this.Data)
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

    public class Cursor<T> : IEnumerable<T>, Model
    {
        public IEnumerable<Segment<T>> Segments { get; private set; }

        public Cursor(IEnumerable<Segment<T>> segments)
        {
            this.Segments = segments;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach(var segment in this.Segments)
            {
                foreach(var item in segment)
                {
                    yield return item;
                }
            }
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    [JsonObject]
    public class Row : IEnumerable<Tuple<string, string, double>>, Model
    {
        [JsonProperty("t")]
        public ZonedDateTime Timestamp { get; set; }

        [JsonProperty("data")]
        public IDictionary<Tuple<string, string>, double> Data { get; set; }

        public IEnumerator<Tuple<String, String, double>> GetEnumerator()
        {
            foreach(var kvPair in Data)
            {
                yield return Tuple.Create(kvPair.Key.Item1, kvPair.Key.Item2, kvPair.Value);
            }
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool HasSensor(string deviceKey, string sensorKey)
        {
            return Data.ContainsKey(Tuple.Create(deviceKey, sensorKey));
        }
    }
}