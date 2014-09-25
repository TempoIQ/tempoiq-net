using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoIQ.Json;
using NodaTime;

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

    public interface Query { }
    
    [JsonObject]
    public class ReadQuery : Query
    {
        [JsonProperty("search")]
        public Search Search { get; set; }

        [JsonProperty("read")]
        public Read Read { get; set; }

        public ReadQuery(Search search, Read read)
        {
            this.Search = Search;
            this.Read = read;
        }
    }

    [JsonObject]
    public class FindQuery : Query
    {
        [JsonProperty("search")]
        public Search Search { get; set; }

        [JsonProperty("find")]
        public Find Find { get; set; }

        public FindQuery(Search search, Find find)
        {
            this.Search = search;
            this.Find = find;
        }
    }
}
