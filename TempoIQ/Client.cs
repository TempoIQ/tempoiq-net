using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TempoIQ.Json;
using TempoIQ.Models;
using TempoIQ.Results;
using TempoIQ.Queries;
using TempoIQ.Utilities;
using Newtonsoft.Json;
using NodaTime;

namespace TempoIQ
{
    /// <summary>
    /// The Client is the primary interface with TempoIQ
    /// </summary>
    public class Client
    {
        /// <summary> Handles the actual network operations </summary>
        private Executor Runner { get; set; }

        public const string API_VERSION = "v2";

        private const string PAGINATED_READ_MEDIA_TYPE = "application/prs.tempoiq.datapoint-collection.v2+json";
        private const string SIMPLE_READ_MEDIA_TYPE = "application/prs.tempoiq.datapoint-collection.v1+json";

        private const string PAGINATED_SEARCH_MEDIA_TYPE = "application/prs.tempoiq.device-collection.v2+json";
        private const string SIMPLE_SEARCH_MEDIA_TYPE = "application/prs.tempoiq.device-collection.v1+json";

        /// <summary>
        /// Create a new client from credentials, backend, port(optional) and timeout(optional, in milliseconds)
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        public Client(Credentials credentials, string host, int port = 443, int timeout = 50000)
        {
            var builder = new UriBuilder {
                Scheme = "https",
                Host = host,
                Port = port
            };
            Runner = new Executor(builder.Uri, credentials, timeout);
        }

        /// <summary>
        /// Create a new device
        /// </summary>
        /// <param name="device"></param>
        /// <returns>a Result with the created Device</returns>
        public Result<Device> CreateDevice(Device device)
        {
            string target = String.Format("{0}/devices/", API_VERSION);
            return Runner.Post<Device>(target, device);
        }

        /// <summary>
        /// Retrieve a device of a given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>a Result with the device of that key, if any</returns>
        public Result<Device> GetDevice(string key)
        {
            var target = String.Format("{0}/devices/{1}/", API_VERSION, HttpUtility.UrlEncode(key));
            return Runner.Get<Device>(target);
        }

        /// <summary>
        /// Replace a device
        /// </summary>
        /// <param name="device"></param>
        /// <returns>a Result with the updated Device</returns>
        public Result<Device> UpdateDevice(Device device)
        {
            var target = String.Format("{0}/devices/{1}/", API_VERSION, HttpUtility.UrlEncode(device.Key));
            return Runner.Put<Device>(target, device);
        }

        /// <summary>
        /// List the devices which meet a given selection
        /// </summary>
        /// <param name="selection"></param>
        /// <returns>a result with the selected Devices</returns>
        public Cursor<Device> ListDevices(Selection selection)
        {
            var query = new FindQuery(new Search(Select.Type.Devices, selection), new Find());
            return ListDevices(query);
        }

        /// <summary>
        /// List the devices which meet the criteria for a given query
        /// </summary>
        /// <param name="selection"></param>
        /// <returns>a result with the selected Devices</returns>
        public Cursor<Device> ListDevices(FindQuery query)
        {
            var target = String.Format("{0}/devices/query/", API_VERSION);
            return Runner.Post<Segment<Device>>(target, query, PAGINATED_SEARCH_MEDIA_TYPE)
                .ToCursor<Device>(Runner, target, PAGINATED_SEARCH_MEDIA_TYPE);
        }

        /// <summary>
        /// Delete a device of a given key
        /// </summary>
        /// <param name="device"></param>
        /// <returns>a Result with the success or failure of the operation only</returns>
        public Result<Unit> DeleteDevice(Device device)
        {
            var target = String.Format("{0}/devices/{1}/", API_VERSION, HttpUtility.UrlEncode(device.Key));
            var result = Runner.Delete<Unit>(target);
            return result;
        }

        /// <summary>
        /// Delete the devices which meet a given selection
        /// </summary>
        /// <param name="selection"></param>
        /// <returns>a Result with the success or failure of the operation only</returns>
        public Result<DeleteSummary> DeleteDevices(Selection selection)
        {
            var target = String.Format("{0}/devices/", API_VERSION);
            var query = new FindQuery(new Search(Select.Type.Devices, selection), new Find());
            return Runner.Delete<DeleteSummary>(target, query);
        }

        /// <summary>
        /// Write datapoints with a MultiDataPoint
        /// </summary>
        /// <param name="device"></param>
        /// <param name="data"></param>
        /// <returns>a Result with the success or failure of the operation only</returns>
        public Result<Unit> WriteDataPoints(Device device, MultiDataPoint data)
        {
            var writeRequest = new WriteRequest();
            foreach (var pair in data.vs)
                writeRequest.Add(device.Key, pair.Key, new DataPoint(data.t, pair.Value));
            return WriteDataPoints(writeRequest);
        }

        /// <summary>
        /// Write datapoints with a List<MultiDataPoint>
        /// </summary>
        /// <param name="device"></param>
        /// <param name="data"></param>
        /// <returns>a Result with the success or failure of the operation only</returns>
        public Result<Unit> WriteDataPoints(Device device, IList<MultiDataPoint> data)
        {
            var writeRequest = data.Aggregate(new WriteRequest(),
                                   (acc, mdp) => mdp.vs.Aggregate(acc,
                                       (req, pair) => req.Add(device.Key, pair.Key, new DataPoint(mdp.t, pair.Value))));
            var result = WriteDataPoints(writeRequest);
            return result;
        }

        /// <summary>
        /// Write data to a given sensor on a given device
        /// </summary>
        /// <param name="device"></param>
        /// <param name="sensor"></param>
        /// <param name="data"></param>
        /// <returns>a Result with the success or failure of the operation only</returns>
        public Result<Unit> WriteDataPoints(Device device, Sensor sensor, IList<DataPoint> data)
        {
            var result = WriteDataPoints(device.Key, sensor.Key, data);
            return result;
        }

        /// <summary>
        /// Write data to a given sensor on a given device
        /// </summary>
        /// <param name="deviceKey"></param>
        /// <param name="sensorKey"></param>
        /// <param name="data"></param>
        /// <returns>a Result with the success or failure of the operation only</returns>
        public Result<Unit> WriteDataPoints(string deviceKey, string sensorKey, IList<DataPoint> data)
        {
            var writeRequest = data.Aggregate(new WriteRequest(), 
                (req, dp) => req.Add(deviceKey, sensorKey, dp));
            var result = WriteDataPoints(writeRequest);
            return result;
        }

        /// <summary>
        /// Write data from a WriteRequest object
        /// </summary>
        /// <param name="writeRequest"></param>
        /// <returns>a Result with the success or failure of the operation only</returns>
        public Result<Unit> WriteDataPoints(WriteRequest writeRequest)
        {
            var target = String.Format("{0}/write/", API_VERSION);
            var result = Runner.Post<Unit>(target, writeRequest);
            return result;
        }

        /// <summary>
        /// Read data from a selection, start time and stop time
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns>The data from the devices and sensors which match your selection, 
        /// as processed by the pipeline, and bookended by the start and stop times</returns>
        public Cursor<Row> Read(Selection selection, ZonedDateTime start, ZonedDateTime stop)
        {
            var search = new Search(Select.Type.Sensors, selection);
            var read = new Read(start, stop);
            var query = new ReadQuery(search, read);
            return Read(query);
        }

        /// <summary>
        /// Read data from a Selection, function pipeline, start time and stop time
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="pipeline"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns>The data from the devices and sensors which match your selection, 
        /// as processed by the pipeline, and bookended by the start and stop times</returns>
        public Cursor<Row> Read(Selection selection, Pipeline pipeline, ZonedDateTime start, ZonedDateTime stop)
        {
            var query = new ReadQuery(new Search(Select.Type.Sensors, selection), new Read(start, stop), pipeline);
            return Read(query);
        }

        /// <summary>
        /// Read data from a ReadQuery
        /// </summary>
        /// <param name="query"></param>
        /// <returns>The data from the devices and sensors which match your selection, 
        /// as processed by the pipeline, and bookended by the start and stop times</returns>
        public Cursor<Row> Read(ReadQuery query)
        {
            var target = String.Format("{0}/read/query/", API_VERSION);
            return Runner.Post<Segment<Row>>(target, query, PAGINATED_READ_MEDIA_TYPE).ToCursor<Row>(Runner, target, PAGINATED_READ_MEDIA_TYPE);
        }

        /// <summary>
        /// Read the latest datapoints for the items from a SingleValueQuery
        /// </summary>
        /// <param name="query"></param>
        /// <returns>The latest data from the devices and sensors which match your selection, 
        /// as processed by the pipeline, and bookended by the start and stop times</returns>
        public Cursor<Row> Latest(SingleValueQuery query)
        {
            var target = String.Format("{0}/single/query", API_VERSION);
            return Runner.Post<Segment<Row>>(target, query).ToCursor<Row>(Runner, target, SIMPLE_READ_MEDIA_TYPE);
        }

        /// <summary>
        /// Read the latest datapoints for the items from a Selection
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="pipeline"></param>
        /// <returns>The latest data from the devices and sensors which match your selection, 
        /// as processed by the pipeline, and bookended by the start and stop times</returns>
        public Cursor<Row> Latest(Selection selection, Pipeline pipeline = null)
        {
            var query = new SingleValueQuery(new Search(Select.Type.Sensors, selection), new SingleValueAction());
            return Latest(query);
        }

        public Result<DeleteSummary> DeleteDataPoints(Device device, Sensor sensor, ZonedDateTime start, ZonedDateTime stop)
        {
            return DeleteDataPoints(device.Key, sensor.Key, start, stop);
        }

        public Result<DeleteSummary> DeleteDataPoints(string deviceKey, string sensorKey, ZonedDateTime start, ZonedDateTime stop)
        {
            var del = new Delete{ start = start, stop = stop };
            var target = String.Format("{0}/devices/{1}/sensors/{2}/datapoints", API_VERSION, deviceKey, sensorKey);
            return Runner.Delete<DeleteSummary>(target, del);
        }
    }


    public static class SegmentCursorTableConversion
    {
        /// <summary>
        /// Extension method to convert Results 
        /// between Results of Cursors and Results of Segments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns>An Result wrapping the cursor equivalent to the 
        /// Segment in the original's Value</returns>
        public static Cursor<T> ToCursor<T>(this Result<Segment<T>> result, Executor runner, string endPoint, string mediaTypeVersion)
        {
            if (result.State == State.Success)
                return new Cursor<T>(result.Value, runner, endPoint, mediaTypeVersion);
            else
                throw new TempoIQException(result.Message);
        }
    }
}