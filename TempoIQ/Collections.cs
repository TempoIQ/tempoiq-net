using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NodaTime;
using TempoIQ.Utilities;

namespace TempoIQ.Models.Collections
{
    [JsonObject]
    public class Segment<T> : IEnumerable<T>, Model
    {
        [JsonProperty("data")]
        public IList<T> Data{ get; set; }

        [JsonIgnore]
        public string Next { get; set; }

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

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (obj is Segment<T>)
                return this.Equals(obj);
            else return false;
        }

        public bool Equals(Segment<T> obj)
        {
            return obj.SequenceEqual(this);
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
            foreach (var segment in Segments)
            {
                foreach (var item in segment)
                {
                    yield return item;
                }
            }
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (obj is Cursor<T>)
                return this.Equals(obj);
            else return false;
        }

        public bool Equals(Cursor<T> obj)
        {
            return obj.SequenceEqual(this);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash<int>(hash, this.Segments.GetHashCode());
            return hash;
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

        public Dictionary<string, double> Get(string deviceKey)
        {
            return (from tuple in Data 
                    where tuple.Key.Item1.Equals(deviceKey)
                    select new { Key = tuple.Key.Item2, Value = tuple.Value }).ToDictionary(t => t.Key, t => t.Value);
        }

        public double Get(string deviceKey, string sensorKey)
        {
            return Data[Tuple.Create(deviceKey, sensorKey)];
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (obj is Row)
                return this.Equals(obj);
            else return false;
        }

        public bool Equals(Row obj)
        {
            return obj.SequenceEqual(this);
        }
    }
}