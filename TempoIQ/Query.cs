using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoIQ.Json;

namespace TempoIQ.Querying
{
    [JsonObject]
    public class Search
    {
        [JsonProperty("filters")]
        public Selection Selection { get; private set; }

        [JsonProperty("select")]
        public Selectors.Type Type { get; private set; }

        public Search(Selectors.Type type, Selection selection)
        {
            this.Type = type;
            this.Selection = selection;
        }
    }

    public interface Action
    {
        string Name { get; }
    }
    
    public class Find : Action
    {
        public string Name { get { return "find"; } }
    }

    [JsonObject]
    public class Read : Action
    {
        [JsonProperty("name")]
        public string Name { get { return "read"; } }

        [JsonProperty("start")]
        public DateTime Start { get; private set; }

        [JsonProperty("stop")]
        public DateTime Stop { get; private set; }

        public Read(DateTime start, DateTime stop)
        {
            this.Start = start;
            this.Stop = stop;
        }
    }

    [JsonConverter(typeof(QueryConverter))]
    public class Query
    {
        public Search Search { get; private set; }

        public Action Action { get; private set; }

        public Pipeline Pipeline { get; private set; }

        public Query(Search search, Action action, Pipeline pipeline)
        {
            this.Search = search;
            this.Action = action;
            this.Pipeline = pipeline;
        }
    }
}
