using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TempoIQ.Utilities;
using TempoIQ.Json;
using TempoIQ.Models;

namespace TempoIQ.Results
{
    /// <summary>
    /// Fine-grain information regarding the status of a TempoIQ operation
    /// </summary>
    [JsonObject]
    public class Status
    {
        [JsonProperty("status")]
        public HttpStatusCode Code { get; set; }

        [JsonProperty("messages")]
        public List<string> Messages { get; set; }

        [JsonConstructor]
        public Status(HttpStatusCode code, List<string> messages)
        {
            this.Code = code;
            this.Messages = messages;
        }

        public bool IsSuccess { get { return this.Code == HttpStatusCode.OK; } }
    }

    ///<summary> Provides information about a partially successful API request. </summary>
    [JsonObject]
    public class MultiStatus : IEnumerable<Status>
    {
        [JsonProperty("multistatus")]
        public IList<Status> Statuses { get; set; }

        public bool IsSuccess { get { return this.Statuses.All(s => s.IsSuccess); } }

        public bool IsPartialSuccess
        {
            get { return this.Statuses.Any(s => s.IsSuccess) && !this.IsSuccess; }
        }

        public IEnumerable<Status> Failures
        {
            get
            {
                return from s in this.Statuses
                       where !s.IsSuccess
                       select s;
            }
        }

        ///<summary>Base constructor</summary>
        ///<param name="Statuses"> List of <cref>ResponseStatus</cref> objects.</param>
        public MultiStatus(IList<Status> statuses = null)
        {
            if (statuses == null)
                this.Statuses = new List<Status>();
            else
                this.Statuses = statuses;
        }

        ///<summary> Returns iterator over the Statuses.</summary>
        public IEnumerator<Status> GetEnumerator()
        {
            return Statuses.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            else if (!(obj is MultiStatus))
                return false;
            else
                return this.Equals((MultiStatus)obj);
        }

        public bool Equals(MultiStatus m)
        {
            return this.Statuses.SequenceEqual(m.Statuses);
        }
    }
}
