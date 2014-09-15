using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using Newtonsoft.Json;

namespace TempoIQ.Models
{
    [JsonObject]
    public struct DataPoint
    {
        [JsonProperty("timestamp")]
        public ZonedDateTime timestamp;
        [JsonProperty("value")]
        public double value;
        public DataPoint(ZonedDateTime timestamp, double value)
        {
            this.timestamp = timestamp;
            this.value = value;
        }
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

        //SegmentEnumerator will have to be here
        //A SegmentEnumerator functionas as an async IEnumerator
        //Upon the current Segment's first access, we spin up a request to get 
        //the next segment and fire it off as a future or whatever, unless we're on the last segment

    public class Cursor<T> : IEnumerable<T>
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
}