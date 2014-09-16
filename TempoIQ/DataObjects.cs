using RestSharp;
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
    public interface Model { };

    public struct Unit : Model { };

    [JsonObject]
    public class DataPoint
    {
        [JsonProperty("timestamp")]
        public ZonedDateTime Timestamp { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }

        public DataPoint(ZonedDateTime timestamp, double value)
        {
            this.Timestamp = timestamp;
            this.Value = value;
        }
    }

    public class MultiDataPoint : Model
    {
        [JsonProperty("timestamp")]
        public ZonedDateTime Timestamp;
        [JsonProperty("values")]
        public IDictionary<string, double> Values;

        public MultiDataPoint(ZonedDateTime timestamp, IDictionary<string, double> values)
        {
            this.Timestamp = timestamp;
            this.Values = values;
        }

        public MultiDataPoint()
        {
            this.Timestamp = new ZonedDateTime();
            this.Values = new Dictionary<string, double>();
        }
    }

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

        //SegmentEnumerator will have to be here
        //A SegmentEnumerator functionas as an async IEnumerator
        //Upon the current Segment's first access, we spin up a request to get 
        //the next segment and fire it off as a future or whatever, unless we're on the last segment

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

    ///<summary> Provides information about a partially successful API request. </summary>
    [JsonObject]
    public class MultiStatus : IEnumerable<ResponseStatus> 
    {
        [JsonProperty("multistatus")]
        public IList<ResponseStatus> Statuses { get; set; }

        public MultiStatus()
        {
            this.Statuses = new List<ResponseStatus>();
        }
      
        ///<summary>Base constructor</summary>
        ///<param name="Statuses"> List of <cref>ResponseStatus</cref> objects.</param>
        public MultiStatus(IList<ResponseStatus> statuses)
        {
            if (statuses == null) {
                this.Statuses = new List<ResponseStatus>();
            } else {
                this.Statuses = statuses;
            }
        }

        ///<summary> Returns iterator over the Statuses.</summary>
        public IEnumerator<ResponseStatus> GetEnumerator()
        { 
            return Statuses.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}