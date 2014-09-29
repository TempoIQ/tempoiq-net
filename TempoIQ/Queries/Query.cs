using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoIQ.Json;
using NodaTime;

namespace TempoIQ.Queries
{
    /// <summary>
    /// Defines the scope of a Query; limits it to a specific subset of all of sensors/devices
    /// </summary>
    [JsonObject]
    public class Search
    {
        [JsonProperty("filters")]
        public Selection Selection { get; private set; }

        [JsonProperty("select")]
        public Select.Type Type { get; private set; }

        public Search(Select.Type type, Selection selection)
        {
            this.Type = type;
            this.Selection = selection;
        }
    }

    /// <summary>
    /// The various querying operations TempoIQ will perform
    /// </summary>
    public interface Query { }
    
    /// <summary>
    /// A <code>Query</code> for reading data out of TempoIQ
    /// </summary>
    [JsonObject]
    public class ReadQuery : Query
    {
        [JsonProperty("search")]
        public Search Search { get; set; }

        [JsonProperty("read")]
        public Read Read { get; set; }

        public ReadQuery(Search search, Read read)
        {
            this.Search = search;
            this.Read = read;
        }
    }

    /// <summary>
    /// A <code>Query</code> for finding specific domain objects
    /// </summary>
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
