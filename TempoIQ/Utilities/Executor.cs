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

namespace TempoIQ.Utilities
{    
    public class Executor
    {
        private RestClient Rest { get; set; }
        
        private TempoIQSerializer Serialization { get; set; }

        public Executor(Uri uri, Credentials credentials, int timeout = 50000)
        {
            this.Serialization = new TempoIQSerializer();
            var rest = new RestClient(uri.AbsoluteUri);
            rest.Authenticator = new HttpBasicAuthenticator(credentials.Key, credentials.secret);
            rest.Timeout = timeout;
            this.Rest = rest;
        }

        public Result<T> Get<T>(string resource, string contentType, string[] mediaTypeVersions)
        {
            return Execute<T>(Method.GET, resource, null, contentType, mediaTypeVersions);
        }

        public Result<T> Post<T>(string resource, object body, string contentType, string[] mediaTypeVersions)
        {
            return Execute<T>(Method.POST, resource, body, contentType, mediaTypeVersions);
        }

        public Result<T> Put<T>(string resource, object body, string contentType, string[] mediaTypeVersions)
        {
            return Execute<T>(Method.PUT, resource, body, contentType, mediaTypeVersions);
        }

        public Result<T> Delete<T>(string resource, object body, string contentType, string[] mediaTypeVersions)
        {
            return Execute<T>(Method.DELETE, resource, body, contentType, mediaTypeVersions);
        }

        public Result<Unit> Delete<Unit>(string resource, string contentType, string[] mediaTypeVersions)
        {
            return Execute<Unit>(Method.DELETE, resource, null, contentType, mediaTypeVersions);
        }

        public Result<T> Execute<T>(Method method, string resource, object body, string contentType, string[] mediaTypeVersions)
        {
            var request = new RestRequest(resource, method);
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = this.Serialization;
            request.AddBody(body);
            if (mediaTypeVersions.Any())
                request.AddHeader("Accept", String.Join(",", mediaTypeVersions));
            if (contentType != null && contentType != "")
                request.AddHeader("Content-Type", contentType);
            var response = Rest.Execute(request);
            return response.ToResult<T>();
        }
    }

    public static class ResponseResultExtension
    {
        public static Result<T> ToResult<T>(this IRestResponse response)
        {
            T value = JsonConvert.DeserializeObject<T>(response.Content ?? "", TempoIQSerializer.Converters);
            int code = (int)response.StatusCode;
            string message = response.StatusDescription;
            MultiStatus multi;
            if (response.StatusCode == HttpStatusCode.OK)
                multi = new MultiStatus(new List<Status> { new Status(HttpStatusCode.OK, new List<string>()) });
            else if ((int)response.StatusCode == 207)
                multi = JsonConvert.DeserializeObject<MultiStatus>(response.Content);
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