using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
}