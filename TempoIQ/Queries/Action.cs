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
	/// A Search's behavior towards the objects it selects
	/// </summary>
	public interface Action
	{
        string Name { get; }
        int? Limit { get; }
	}

	/// <summary>
	/// The behavior to find objects through a Query
	/// </summary>
	[JsonObject]
	public class Find : Action
	{
		[JsonIgnore]
        public string Name { get { return "find"; } }

        [JsonProperty("quantifier")]
        public string Quantifier { get { return "all"; } }

        /// <summary>
        /// The maximum number of items to return per network-loaded page of data. 
        /// If left untouched, Limit defaults to 5000
        /// </summary>
        [JsonProperty(PropertyName = "limit", NullValueHandling = NullValueHandling.Ignore)]
        public int? Limit { get; set; }

        [JsonConstructor]
        public Find(int? limit = null)
        {
            this.Limit = limit;
        }
    }

    /// <summary>
    /// The behavior to read from Start through Stop
    /// </summary>
    [JsonObject]
    public class Read : Action
    {
        /// <summary>
        /// Gets the name of the action-type.
        /// </summary>
        /// <value>The name.</value>
		[JsonIgnore]
		public string Name { get { return "read"; } }

		/// <summary>
		/// The start time of the Read
		/// </summary>
		[JsonProperty ("start")]
		public ZonedDateTime Start { get; private set; }

        /// <summary>
        /// The maximum number of items to return per network-loaded page of data. 
        /// If left untouched, Limit defaults to 5000
        /// </summary>
        [JsonProperty(PropertyName = "limit", NullValueHandling = NullValueHandling.Ignore)]
        public int? Limit { get; set; }

		/// <summary>
		/// The stop time of the Read
		/// </summary>
		[JsonProperty ("stop")]
		public ZonedDateTime Stop { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TempoIQ.Queries.Read"/> class.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="stop">Stop.</param>
		[JsonConstructor]
		public Read (ZonedDateTime start, ZonedDateTime stop, int? limit = null)
		{
			this.Start = start;
			this.Stop = stop;
            this.Limit = limit;
		}
	}

    /// <summary>
    /// The Action object for specifying latest-value queries 
    /// </summary>
	[JsonObject]
	public class SingleValueAction : Action
	{
        /// <summary>
        /// Gets the name of the action-type.
        /// </summary>
        /// <value>The name.</value>
		[JsonIgnore]
		public string Name { get { return "single"; } }

        /// <summary>
        /// The maximum number of items to return per network-loaded page of data. 
        /// If left untouched, Limit defaults to 5000
        /// </summary>
        [JsonProperty(PropertyName = "limit", NullValueHandling = NullValueHandling.Ignore)]
        public int? Limit { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether the single value query <see cref="TempoIQ.Queries.SingleValueAction"/> includes the devices selected as well as the data.
        /// </summary>
        /// <value><c>true</c> if include selection; otherwise, <c>false</c>.</value>
        [JsonProperty("include_selection")]
        public bool IncludeSelection { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TempoIQ.Queries.SingleValueAction"/> class.
        /// </summary>
        /// <param name="includeSelection">If set to <c>true</c> include selection.</param>
        public SingleValueAction(bool includeSelection = false, int? limit = null)
        {
            this.IncludeSelection = includeSelection;
            this.Limit = limit;
        }
    }
}