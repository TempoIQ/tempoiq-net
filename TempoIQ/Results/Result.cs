﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoIQ.Utilities;
using TempoIQ.Json;
using TempoIQ.Models;

namespace TempoIQ.Results
{
    /// <summary>
    /// Coarse-grain information regarding the success of a TempoIQ operation
    /// </summary>
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
        public T Value { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public UpsertResponse UpsertStatus { get; set; }
        private Encoding DEFAULT_CHARSET = Encoding.Unicode;

        ///<summary> Base Constructor: usually created from REST responses </summary>
        ///<param name="value"> The returned value.</param>
        ///<param name="code"> The status code of the entire result.</param>
        ///<param name="message"> Message providing information about the state of the result.</param>
        ///<param name="upsertStatus"> Provides information about upserts.</param>
        public Result(T value, int code, String message = "", UpsertResponse upsertStatus = null)
        {
            this.Value = value;
            this.Code = code;
            this.Message = message;
            this.UpsertStatus = upsertStatus;
        }

        public virtual State State
        {
            get
            {
                if (this.UpsertStatus != null)
                    return this.UpsertState;
                else
                {
                    if (this.Code == 200)
                        return State.Success;
                    else if (this.Code == 207)
                        return State.PartialSuccess;
                    else
                        return State.Failure;
                }
            }
        }

        private State UpsertState
        {
            get
            {
                if (this.UpsertStatus.Success == true)
                    return State.Success;
                else if (this.UpsertStatus.PartialSuccess == true)
                    return State.PartialSuccess;
                else
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
            else
                return false;
        }

        public bool Equals(Result<T> that)
        {
            bool values, states, multiStatuses;
            states = this.State.Equals(that.State);

            if ((this.Value == null) && (that.Value == null))
                values = true;
            else if (this.Value == null)
                values = false;
            else values = this.Value.Equals(that.Value);

            if ((this.UpsertStatus == null) && (that.UpsertStatus == null))
                multiStatuses = true;
            else if ((this.UpsertStatus == null) || (that.UpsertStatus == null))
                multiStatuses = false;
            else
                multiStatuses = this.UpsertStatus.Equals(that.UpsertStatus);

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
            hash = HashCodeHelper.Hash(hash, UpsertStatus);
            return hash;
        }
    }

    /// <summary>
    /// Information regarding the result of a deletion
    /// </summary>
    public class DeleteSummary : IModel
    {
        /// <summary>
        /// The number of objects deleted
        /// </summary>
        [JsonProperty(PropertyName = "deleted")]
        public int Deleted { get; set; }

        public DeleteSummary(int deleted)
        {
            Deleted = deleted;
        }
    }

    [Serializable]
    public class TempoIQException : Exception
    {
        public TempoIQException(string message) : base(message) { }
    }
}
