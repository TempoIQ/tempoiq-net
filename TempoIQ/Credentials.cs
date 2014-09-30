using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using TempoIQ.Utilities;

namespace TempoIQ
{
    /// <summary>
    /// Api Credentials
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// The credentials key
        /// </summary>
        [JsonProperty("key")]
        public string Key;

        /// <summary>
        /// The credentials secret
        /// </summary>
        [JsonProperty("secret")]
        public string secret;

        public Credentials(string key, string secret)
        {
            this.Key = key;
            this.secret = secret;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            if (obj == null)
                return false;
            if (obj is Credentials)
                return this.Equals((Credentials)obj);
            else
                return false;
        }

        public bool Equals(Credentials credentials)
        {
            return this.Key == credentials.Key && this.secret == credentials.secret;
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash<string>(hash, Key);
            hash = HashCodeHelper.Hash<string>(hash, secret);
            return hash;
        }
    }
}