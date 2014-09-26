using Newtonsoft.Json;
using System.IO;
using System.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoIQ.Utilities;
using TempoIQ.Json;
using TempoIQ.Models.Collections;

namespace TempoIQ.Models
{
    public enum State
    {
        Failure,
        PartialSuccess,
        Success
    };

    ///<summary>Response from an API call</summary>
    ///<para>The Result object returns the requested entity as well as provides
    ///information about the success state of the request. The Result state (getState()) should
    ///be inspected before trying to use the value.
    ///<para>In the event of a failure, the value is set to null. The code and message will provide more
    ///information about the failure.
    ///<para>In some cases, a request can partially succeed. This is indicated with state PARTIAL_SUCCESS.
    ///If a partial success occurs, the {@link MultiStatus} (member of Result) will be populated with
    ///more information about which request failed.
    public class Result<T>
    {
        public T Value { get; private set; }
        public int Code { get; private set; }
        public string Message { get; private set; }
        public MultiStatus MultiStatus { get; private set; }
        private Encoding DEFAULT_CHARSET = Encoding.Unicode;

        ///<summary> Base Constructor: usually created from REST responses </summary>
        ///<param name="Value"> The returned value.</param>
        ///<param name="Code"> The status code of the entire result.</param>
        ///<param name="Message"> Message providing information about the state of the result.</param>
        ///<param name="Multistatus"> Provides information about partially successful result.</param>
        public Result(T value, int code, String message = "", MultiStatus multistatus = null)
        {
            this.Value = value;
            this.Code = code;
            this.Message = message;
            this.MultiStatus = multistatus;
        }

        public State State
        {
            get
            {
                return CodeToState(this.Code);
            }
        }

        private static State CodeToState(int code)
        {
            if (code == 200) {
                return State.Success;
            } else if (code == 207) {
                return State.PartialSuccess;
            } else {
                return State.Failure;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (obj is Result<T>)
                return this.Equals((Result<T>)obj);
            else return false;
        }

        public bool Equals(Result<T> that)
        {
            bool values = false;
            if ((this.Value == null) && (that.Value == null))
                values = true;
            else if (this.Value == null)
                values = false;
            else values = this.Value.Equals(that.Value);
            bool states = this.State.Equals(that.State);
            bool multiStatuses = false;
            if ((this.MultiStatus == null) && (that.MultiStatus == null))
                multiStatuses = true;
            else if ((this.MultiStatus == null) || (that.MultiStatus == null))
                multiStatuses = false;
            else multiStatuses = this.MultiStatus.Equals(that.MultiStatus);
            bool codes = this.Code.Equals(that.Code);
            return values && states && multiStatuses && codes;
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Value);
            hash = HashCodeHelper.Hash(hash, Code);
            hash = HashCodeHelper.Hash(hash, Message);
            hash = HashCodeHelper.Hash(hash, State);
            hash = HashCodeHelper.Hash(hash, MultiStatus);
            return hash; 
        }
    }

    public class DeleteSummary : Model
    {
        private int deleted;

        [JsonProperty(PropertyName = "deleted")]
        public int Deleted
        {
            get { return deleted; }
            private set { this.deleted = value; }
        }

        public DeleteSummary(int deleted)
        {
            Deleted = deleted;
        }
    }


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
            if (statuses == null) {
                this.Statuses = new List<Status>();
            } else {
                this.Statuses = statuses;
            }
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