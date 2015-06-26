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
    /// A pair of timestamps delineating the range of a delete request
    /// </summary>
    [JsonObject]
    public struct Delete : IModel
    {
        [JsonProperty("start")]
        public ZonedDateTime start;

        [JsonProperty("stop")]
        public ZonedDateTime stop;

        [JsonConstructor]
        public Delete(ZonedDateTime start, ZonedDateTime stop)
        {
            this.start = start;
            this.stop = stop;
        }

        public override bool Equals(object obj)
        {
            if (obj is Delete)
                return this.Equals((Delete)obj);
            else
                return false;
        }

        public bool Equals(Delete del)
        {
            return del.start.Equals(this.start) && del.stop.Equals(this.stop);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, this.start);
            hash = HashCodeHelper.Hash(hash, this.stop);
            return hash;
        }
    }
}
