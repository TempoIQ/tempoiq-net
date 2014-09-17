using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TempoIQ.Models;
using TempoIQ.Models.Collections;
using TempoIQ.Querying;
using Newtonsoft.Json;
using NodaTime;

namespace TempoIQ
{
    public class Client
    {
        public RestClient Rest { get; set; }

        public SimpleAuthenticator Authenticator { get; set;}
        public string API_VERSION { get { return "v2"; } }

        public Client(Credentials credentials, string baseUrl, string scheme)
        {
            this.Rest = new RestClient(scheme + baseUrl);
            this.Authenticator = new SimpleAuthenticator("key", credentials.key, "secret", credentials.secret);
        }

        public Result<Device> CreateDevice(Device device)
        { 
            var request = new RestRequest("devices", Method.POST);
            request.AddBody(device);
            return Execute<Device>(request);
        }

        public Result<Device> GetDevice(string key)
        {
            string target = String.Format("{0}/devices/{1}/", API_VERSION, HttpUtility.UrlEncode(key));
            var request = new RestRequest(target, Method.GET);
            return Execute<Device>(request);
        }

        public Result<Device> UpdateDevice(Device device)
        {
            string target = String.Format("{0}/devices/{1}/", API_VERSION, HttpUtility.UrlEncode(device.Key));
            var request = new RestRequest(target, Method.PUT);
            request.AddBody(device);
            return Execute<Device>(request);
        }

        public Result<Cursor<Device>> ListDevics(Selection selection)
        {
            var request = new RestRequest(String.Format("{0}/devices/"), Method.GET);
            var query = new Query(new Search(Selectors.Type.Devices, selection), new Find(), null);
            request.AddBody(query);
            return Execute<Cursor<Device>>(request);
        }

        public Result<Unit> DeleteDevice(Device device)
        {
            string target = String.Format("{0}/devices/{1}/", API_VERSION, HttpUtility.UrlEncode(device.Key));
            var request = new RestRequest(target, Method.DELETE);
            request.AddBody(device);
            return Execute<Unit>(request);
        }

        public Result<DeleteSummary> DeleteDevices(Selection selection)
        {
            var request = new RestRequest(String.Format("{0}/devices/"), Method.DELETE);
            var query = new Query(new Search(Selectors.Type.Devices, selection), new Find(), null);
            request.AddBody(query);
            return Execute<DeleteSummary>(request);
        }

        public Result<DeleteSummary> DeleteAllDevices()
        {
            return DeleteDevices(new Selection().AddSelector(Selectors.Type.Devices, Selectors.All()));
        }

        public Result<Unit> WriteDataPoints(Device device, MultiDataPoint data)
        {
            var writeRequest = new WriteRequest();
            foreach (var pair in data.Values)
            {
                var dp = new DataPoint(data.Timestamp, pair.Value);
                writeRequest.Add(device.Key, pair.Key, dp);
            }
            return WriteDataPoints(writeRequest);
        }

        public Result<Unit> WriteDataPoints(Device device, IList<MultiDataPoint> data)
        {
            var writeRequest = data.Aggregate(new WriteRequest(),
                (acc, mdp) => mdp.Values.Aggregate(acc,
                    (req, pair) => req.Add(device.Key, pair.Key, new DataPoint(mdp.Timestamp, pair.Value))));
            return WriteDataPoints(writeRequest);
        }

        public Result<Unit> WriteDataPoints(WriteRequest writeRequest)
        {
            var request = new RestRequest(String.Format("{0}/write/", API_VERSION), Method.POST);
            request.AddBody(JsonConvert.SerializeObject(writeRequest.Data));
            return Execute<Unit>(request);
        }

        public Result<Cursor<Row>> Read(Selection selection, ZonedDateTime start, ZonedDateTime stop)
        {
            return Read(selection, new Pipeline(), start, stop);
        }

        public Result<Cursor<Row>> Read(Selection selection, Pipeline pipeline, ZonedDateTime start, ZonedDateTime stop)
        {
            var request = new RestRequest(String.Format("{0}/read/", API_VERSION), Method.GET);
            var query = new Query(new Search(Selectors.Type.Devices, selection), new Read(start, stop), pipeline);
            request.AddBody(JsonConvert.SerializeObject(query));
            return Execute<Cursor<Row>>(request);
        }

        public Result<T> Execute<T>(RestRequest request) where T : Model
        {
            return Execute<T>(request, typeof(T));
        }

        public Result<T> Execute<T>(RestRequest request, Type type) where T : Model
        {
            var response = new Result<T>(Rest.Execute(request));
            return response;
        }
    }
}