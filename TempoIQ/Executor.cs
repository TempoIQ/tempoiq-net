using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using TempoIQ.Queries;
using TempoIQ.Utilities;
using TempoIQ.Models;
using TempoIQ.Results;
using TempoIQ.Json;

namespace TempoIQ
{    
    public class Executor
    {
        public int Port { get; set; }

        public TempoIQSerializer TempoSerialization { get { return new TempoIQSerializer(); } }

        protected RestClient Rest { get; set; }

        public Executor(string baseUrl, Credentials credentials, int port = 443, int timeout = 50000)
        {
            this.Port = port; 
            var rest = new RestClient(String.Format("https://{0}:{1}/", baseUrl, port.ToString()));
            rest.Authenticator = new HttpBasicAuthenticator(credentials.key, credentials.secret);
            rest.Timeout = timeout;
            this.Rest = rest;
        }

        public Result<T> Get<T>(string resource, object body = null)
        {
            return Execute<T>(Method.GET, resource, body);
        }

        public Result<T> Post<T>(string resource, object body)
        {
            return Execute<T>(Method.POST, resource, body);
        }

        public Result<T> Put<T>(string resource, object body)
        {
            return Execute<T>(Method.PUT, resource, body);
        }

        public Result<Unit> Delete<Unit>(string resource, object body)
        {
            return Execute<Unit>(Method.DELETE, resource, body);
        }

        public Result<Unit> Delete<Unit>(string resource)
        {
            return Execute<Unit>(Method.DELETE, resource, null);
        }

        public Result<T> Execute<T>(Method method, string resource, object body)
        {
            var request = new RestRequest(resource, method);
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new TempoIQSerializer();
            request.AddBody(body);
            var response = Rest.Execute(request);
            Console.WriteLine(response.Content);
            return response.ToResult<T>(this.TempoSerialization);
        }
    }

    public static class ResponseResultExtension
    {
        public static Result<T> ToResult<T>(this IRestResponse response,
            TempoIQSerializer serialization)
        {
            T value = JsonConvert.DeserializeObject<T>(response.Content ?? "", TempoIQSerializer.Converters);
            int code = (int)response.StatusCode;
            string message = response.StatusDescription;
            MultiStatus multi;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                multi = new MultiStatus();
                multi.Statuses.Add(new Status(HttpStatusCode.OK, new List<string>()));
            }
            else if ((int)response.StatusCode == 207)
            {
                multi = JsonConvert.DeserializeObject<MultiStatus>(response.Content);
            }
            else
            {
                multi = new MultiStatus();
                multi.Statuses.Add(
                    new Status(response.StatusCode,
                    new List<string>(new string[] { response.ErrorMessage })));
                message = response.ErrorMessage;
            }
            return new Result<T>(value, code, message, multi);
        }
    }
}