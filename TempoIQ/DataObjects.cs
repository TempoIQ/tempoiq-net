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
    public struct DataPoint
    {
        [JsonProperty("t")]
        public ZonedDateTime t;

        [JsonProperty("v")]
        public double v;

        [JsonConstructor]
        public DataPoint(ZonedDateTime t, double v)
        {
            this.t = t;
            this.v = v;
        }
    }

    public struct MultiDataPoint : Model
    {
        [JsonProperty("t")]
        public ZonedDateTime t;

        [JsonProperty("vs")]
        public IDictionary<string, double> vs;

        [JsonConstructor]
        public MultiDataPoint(ZonedDateTime t, IDictionary<string, double> vs)
        {
            this.t = t;
            this.vs = vs;
        }

        public override bool Equals(object obj)
        {
            if (obj is MultiDataPoint)
            {
                return this.Equals((MultiDataPoint)obj);
            } else
            {
                return false;
            }
        }

        public bool Equals(MultiDataPoint mdp)
        {
            return mdp.t.Equals(this.t) && mdp.vs.SequenceEqual(this.vs);
        }
    }
}