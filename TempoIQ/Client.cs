using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TempoIQ.Json;
using TempoIQ.Models;
using TempoIQ.Models.Collections;
using TempoIQ.Querying;
using Newtonsoft.Json;
using NodaTime;

namespace TempoIQ
{
    public class Client
    {
        public Executor RequestRunner { get; set; }
        private const string API_VERSION = "v2";

        public Client(Credentials credentials, string baseUrl, int port = 443, int timeout = 50000)
        {
            this.RequestRunner = new Executor(baseUrl, credentials, port, timeout);
        }

        public Result<Device> CreateDevice(Device device)
        {
            string target = String.Format("v2/devices/", device.Key);
            return RequestRunner.Post<Device>(target, device);
        }

        public Result<Device> GetDevice(string key)
        {
            string target = String.Format("v2/devices/{0}/",  HttpUtility.UrlEncode(key));
            return RequestRunner.Get<Device>(target);
        }

        public Result<Device> UpdateDevice(Device device)
        {
            string target = String.Format("v2/devices/{0}/", HttpUtility.UrlEncode(device.Key));
            return RequestRunner.Put<Device>(target, device);
        }

        public Result<Cursor<Device>> ListDevics(Selection selection)
        {
            var query = new FindQuery(
                new Search(Selectors.Type.Devices, selection),
                new Find());
            var prelim = RequestRunner.Post<Segment<Device>>("v2/devices/query/", query);
            return prelim.ToCursorResult<Device>();
        }

        public Result<Unit> DeleteDevice(Device device)
        {
            string target = String.Format("v2/devices/{0}/", HttpUtility.UrlEncode(device.Key));
            return RequestRunner.Delete<Unit>(target);
        }

        public Result<DeleteSummary> DeleteDevices(Selection selection)
        {
            var query = new Search(Selectors.Type.Devices, selection);
            return RequestRunner.Delete<DeleteSummary>("v2/devices/query/", query);
        }

        public Result<DeleteSummary> DeleteAllDevices()
        {
            var allSelection = new Selection().AddSelector(Selectors.Type.Devices, new AllSelector());
            return DeleteDevices(allSelection);
        }

        public Result<Unit> WriteDataPoints(Device device, MultiDataPoint data)
        {
            var writeRequest = new WriteRequest();
            foreach(var pair in data.vs)
            {
                var dp = new DataPoint(data.t, pair.Value);
                writeRequest.Add(device.Key, pair.Key, dp);
            }
            return WriteDataPoints(writeRequest);
        }

        public Result<Unit> WriteDataPoints(Device device, IList<MultiDataPoint> data)
        {
            var writeRequest = data.Aggregate(new WriteRequest(),
                (acc, mdp) => mdp.vs.Aggregate(acc,
                    (req, pair) => req.Add(device.Key, pair.Key, new DataPoint(mdp.t, pair.Value))));
            return WriteDataPoints(writeRequest);
        }

        public Result<Unit> WriteDataPoints(WriteRequest writeRequest)
        {
            return RequestRunner.Post<Unit>("v2/write/", writeRequest);
        }

        public Result<Cursor<Row>> Read(Selection selection, ZonedDateTime start, ZonedDateTime stop)
        {
            return Read(selection, new Pipeline(), start, stop);
        }

        public Result<Cursor<Row>> Read(Selection selection, Pipeline pipeline, ZonedDateTime start, ZonedDateTime stop)
        {
            var query = new ReadQuery(new Search(Selectors.Type.Devices, selection), new Read(start, stop));
            return RequestRunner.Post<Cursor<Row>>("v2/read/query", query);
        }
    }

    public static class SegmentCursorConversion
    {
        public static Result<Cursor<T>> ToCursorResult<T>(this Result<Segment<T>> result)
        {
            Cursor<T> cursor;
            if (result.Value == null)
            {
                cursor = new Cursor<T>(new List<Segment<T>>());
            }
            else
            {
                var lst = new List<Segment<T>>();
                lst.Add(result.Value);
                cursor = new Cursor<T>(lst);
            }
            return new Result<Cursor<T>>(cursor, result.Code, result.Message, result.MultiStatus);
        }
    }
}