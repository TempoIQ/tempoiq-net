using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoIQ
{
    /// <summary>
    /// Api Credentials
    /// </summary>
    class Credentials
    {
        /// <summary>
        /// The credentials key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The credentials secret
        /// </summary>
        public string Secret { get; set; }

        public Credentials()
        {
            this.Key = "";
            this.Secret = "";
        }

        public Credentials(string key, string secret)
        {
            this.Key = key;
            this.Secret = secret;
        }
    }
}
