using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoIQ
{
    class Client
    {
        public RestClient RestClient { get; set; }

        private SimpleAuthenticator Authenticator { get; set;}

        public Client(Credentials credentials, string baseUrl, string scheme)
        {
            this.RestClient = new RestClient(scheme + baseUrl);
            this.Authenticator = new SimpleAuthenticator("key", credentials.key, "secret", credentials.secret);
        }
    }
}