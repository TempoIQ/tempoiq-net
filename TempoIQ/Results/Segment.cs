using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NodaTime;
using TempoIQ.Utilities;
using TempoIQ.Models;

namespace TempoIQ.Results
{
    /// <summary>
    /// The Segment represents a Chunk of some object from TempoIQ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [JsonObject]
    public class Segment<T> : IEnumerable<T>, IModel
    {
        /// <summary>
        /// the underlying chunk of data
        /// </summary>
        [JsonProperty("data")]
        public IList<T> Data{ get; set; }

        /// <summary>
        /// a pointer to the next segment
        /// </summary>
        [JsonIgnore]
        public string Next { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            foreach(var item in this.Data)
                yield return item;
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="next"></param>
        public Segment(IList<T> data, string next)
        {
            this.Data = data;
            this.Next = next;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (obj is Segment<T>)
                return this.Equals(obj);
            else return false;
        }

        public bool Equals(Segment<T> obj)
        {
            return obj.SequenceEqual(this);
        }

        public override int GetHashCode ()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Data);
            hash = HashCodeHelper.Hash(hash, Next);
            return hash;
        }
    }
}
