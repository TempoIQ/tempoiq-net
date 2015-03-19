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
        public string Secret;

        public Credentials(string key, string secret)
        {
            this.Key = key;
            this.Secret = secret;
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
            return this.Key == credentials.Key && this.Secret == credentials.Secret;
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash<string>(hash, Key);
            hash = HashCodeHelper.Hash<string>(hash, Secret);
            return hash;
        }
    }
}