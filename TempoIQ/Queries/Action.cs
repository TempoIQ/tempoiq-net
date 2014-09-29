using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NodaTime;

namespace TempoIQ.Queries
{
    /// <summary>
    /// A <code>Search</code>'s behavior towards the objects it selects
    /// </summary>
    public interface Action
    {
        string Name { get; }
    }

    /// <summary>
    /// The behavior to find objects through a <code>Query</code>
    /// </summary>
    [JsonObject]
    public class Find : Action
    {
        [JsonIgnore]
        public string Name { get { return "find"; } }

        [JsonProperty("quantifier")]
        public string Quantifier { get { return "all"; } }

        [JsonConstructor]
        public Find() { }
    }

    /// <summary>
    /// The behavior to read from <code>Start</code> through <code>Stop</code>
    /// </summary>
    [JsonObject]
    public class Read : Action
    {
        [JsonIgnore]
        public string Name { get { return "read"; } }

        [JsonProperty("start")]
        public ZonedDateTime Start { get; private set; }

        [JsonProperty("stop")]
        public ZonedDateTime Stop { get; private set; }

        [JsonConstructor]
        public Read(ZonedDateTime start, ZonedDateTime stop)
        {
            this.Start = start;
            this.Stop = stop;
        }
    }
}