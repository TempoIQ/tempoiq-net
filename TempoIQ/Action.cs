using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NodaTime;

namespace TempoIQ.Querying
{
    public interface Action
    {
        string Name { get; }
    }

    [JsonObject]
    public class Find : Action
    {
        [JsonIgnore]
        public string Name { get { return "find"; } }

        [JsonProperty("quantifier")]
        public string Quantifier { get { return "all"; } }

        [JsonConstructor]
        public Find()
        {
            ;
        }
    }

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
