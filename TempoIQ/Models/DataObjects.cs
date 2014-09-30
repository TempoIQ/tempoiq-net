using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodaTime;
using Newtonsoft.Json;
using TempoIQ.Utilities;

namespace TempoIQ.Models
{
    /// <summary>
    /// a TempoIQ data model
    /// </summary>
    public interface IModel { };

    /// <summary>
    /// Represents the empty set of values.
    /// Used in cases where the <code>Result</code> of a 
    /// query does not provide any other value
    /// </summary>
    public class Unit : IModel {
        public override bool Equals(object obj)
        {
            if (obj is Unit) 
                return true;
            else 
                return false;
        }

        public override int GetHashCode()
        {
            return HashCodeHelper.Initialize();
        }

        public Unit() { }
    };

    /// <summary>
    /// A pairing of timestamp to numeric value
    /// </summary>
    [JsonObject]
    public struct DataPoint : IModel
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

    /// <summary>
    /// the values for several of a device's sensors at a given point in time
    /// </summary>
    public struct MultiDataPoint : IModel
    {
        /// <summary>
        /// The timestamp of the sensors' readings
        /// </summary>
        [JsonProperty("t")]
        public ZonedDateTime t;

        /// <summary>
        /// A <code>Dictionary</code> from sensor keys to data values
        /// </summary>
        [JsonProperty("vs")]
        public IDictionary<string, double> vs;

        /// <summary>
        /// Preferred constructor
        /// </summary>
        /// <param name="t"></param>
        /// <param name="vs"></param>
        [JsonConstructor]
        public MultiDataPoint(ZonedDateTime t, IDictionary<string, double> vs)
        {
            this.t = t;
            this.vs = vs;
        }

        public override bool Equals(object obj)
        {
            if (obj is MultiDataPoint)
                return this.Equals((MultiDataPoint)obj);
            else
                return false;
        }

        public bool Equals(MultiDataPoint mdp)
        {
            return mdp.t.Equals(this.t) && mdp.vs.SequenceEqual(this.vs);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, this.t);
            hash = HashCodeHelper.Hash(hash, this.vs);
            return hash;
        }
    }
}