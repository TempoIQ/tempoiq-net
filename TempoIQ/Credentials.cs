using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace TempoIQ
{
    /// <summary>
    /// Api Credentials
    /// </summary>
    public struct Credentials
    {
        /// <summary>
        /// The credentials key
        /// </summary>
        public string key;

        /// <summary>
        /// The credentials secret
        /// </summary>
        public string secret;

        public Credentials(string key, string secret)
        {
            this.key = key;
            this.secret = secret;
        }

        public string ToBase64()
        {
            var keySecret = key + ":" + secret;
            var bytes = Encoding.ASCII.GetBytes(keySecret);
            return Convert.ToBase64String(bytes);
        }
    }
}