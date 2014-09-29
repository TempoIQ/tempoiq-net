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
using Newtonsoft.Json;
using NodaTime;

namespace TempoIQ
{
    /// <summary>
    /// The <code>Client</code> is the primary interface with TempoIQ
    /// </summary>
    public class Client
    {
        /// <summary> Handles the actual network operations </summary>
        public Executor Runner { get; set; }
        
        /// <summary>
        /// Create a new client from credentials, backend, port(optional) and timeout(optional, in milliseconds)
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="baseUrl"></param>
        /// <param name="port"></param>
        /// <param name="timeout"></param>
        public Client(Credentials credentials, string baseUrl, int port = 443, int timeout = 50000)
        {
            this.Runner = new Executor(baseUrl, credentials, port, timeout);
        }

        /// <summary>
        /// Create a new device
        /// </summary>
        /// <param name="device"></param>
        /// <returns>a <code>Result</code> with the created <code>Device</code></returns>
        public Result<Device> CreateDevice(Device device)
        {
            string target = String.Format("v2/devices/", device.Key);
            return Runner.Post<Device>(target, device);
        }
        
        /// <summary>
        /// Retrieve a device of a given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>a <code>Result</code> with the device of that key, if any</returns>
        public Result<Device> GetDevice(string key)
        {
            string target = String.Format("v2/devices/{0}/",  HttpUtility.UrlEncode(key));
            return Runner.Get<Device>(target);
        }

        /// <summary>
        /// Replace a device
        /// </summary>
        /// <param name="device"></param>
        /// <returns>a <code>Result</code> with the updated <code>Device</code></returns>
        public Result<Device> UpdateDevice(Device device)
        {
            string target = String.Format("v2/devices/{0}/", HttpUtility.UrlEncode(device.Key));
            return Runner.Put<Device>(target, device);
        }

        /// <summary>
        /// List the devices which meet a given selection
        /// </summary>
        /// <param name="selection"></param>
        /// <returns>a result with the selected <code>Device</code>s</returns>
        public Result<Cursor<Device>> ListDevics(Selection selection)
        {
            var query = new FindQuery(
                new Search(Select.Type.Devices, selection),
                new Find());
            var prelim = Runner.Post<Segment<Device>>("v2/devices/query/", query);
            return prelim.ToCursorResult<Device>();
        }

        /// <summary>
        /// Delete a device of a given key
        /// </summary>
        /// <param name="device"></param>
        /// <returns>a <code>Result</code> with the success or failure of the operation only</returns>
        public Result<Unit> DeleteDevice(Device device)
        {
            string target = String.Format("v2/devices/{0}/", HttpUtility.UrlEncode(device.Key));
            var result = Runner.Delete<Unit>(target);
            result.Value = new Unit();
            return result;
        }

        /// <summary>
        /// Delete the devices which meet a given selection
        /// </summary>
        /// <param name="selection"></param>
        /// <returns>a <code>Result</code> with the success or failure of the operation only</returns>
        public Result<DeleteSummary> DeleteDevices(Selection selection)
        {
            var query = new FindQuery(new Search(Select.Type.Devices, selection), new Find());
            return Runner.Delete<DeleteSummary>("v2/devices/", query);
        }

        /// <summary>
        /// Delete all devices
        /// </summary>
        /// <returns>a <code>Result</code> with the success or failure of the operation only</returns>
        public Result<DeleteSummary> DeleteAllDevices()
        {
            var allSelection = new Selection().Add(Select.Type.Devices, new AllSelector());
            return DeleteDevices(allSelection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="data"></param>
        /// <returns>a <code>Result</code> with the success or failure of the operation only</returns>
        public Result<Unit> WriteDataPoints(Device device, MultiDataPoint data)
        {
            var writeRequest = new WriteRequest();
            foreach(var pair in data.vs)
                writeRequest.Add(device.Key, pair.Key, new DataPoint(data.t, pair.Value));
            return WriteDataPoints(writeRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="data"></param>
        /// <returns>a <code>Result</code> with the success or failure of the operation only</returns>
        public Result<Unit> WriteDataPoints(Device device, IList<MultiDataPoint> data)
        {
            var writeRequest = data.Aggregate(new WriteRequest(),
                (acc, mdp) => mdp.vs.Aggregate(acc,
                    (req, pair) => req.Add(device.Key, pair.Key, new DataPoint(mdp.t, pair.Value))));
            var result = WriteDataPoints(writeRequest);
            result.Value = new Unit();
            return result;
        }

        /// <summary>
        /// Write data to a given sensor on a given device
        /// </summary>
        /// <param name="device"></param>
        /// <param name="sensor"></param>
        /// <param name="data"></param>
        /// <returns>a <code>Result</code> with the success or failure of the operation only</returns>
        public Result<Unit> WriteDataPoints(Device device, Sensor sensor, IList<DataPoint> data)
        {
            var result = WriteDataPoints(device.Key, sensor.Key, data);
            result.Value = new Unit();
            return result;
        }

        /// <summary>
        /// Write data to a given sensor on a given device
        /// </summary>
        /// <param name="deviceKey"></param>
        /// <param name="sensorKey"></param>
        /// <param name="data"></param>
        /// <returns>a <code>Result</code> with the success or failure of the operation only</returns>
        public Result<Unit> WriteDataPoints(string deviceKey, string sensorKey, IList<DataPoint> data)
        {
            var writeRequest = data.Aggregate(new WriteRequest(),
                (req, dp) => req.Add(deviceKey, sensorKey, dp));
            var result = WriteDataPoints(writeRequest);
            result.Value = new Unit();
            return result;
        }

        /// <summary>
        /// Write data from a WriteRequest object
        /// </summary>
        /// <param name="writeRequest"></param>
        /// <returns>a <code>Result</code> with the success or failure of the operation only</returns>
        public Result<Unit> WriteDataPoints(WriteRequest writeRequest)
        {
            var result =  Runner.Post<Unit>("v2/write/", writeRequest);
            result.Value = new Unit();
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
        public Result<Cursor<Row>> Read(Selection selection, ZonedDateTime start, ZonedDateTime stop)
        {
            return Read(selection, new Pipeline(), start, stop);
        }

        /// <summary>
        /// Read data from a <code>Selection</code>, function pipeline, start time and stop time
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="pipeline"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns>The data from the devices and sensors which match your selection, 
        /// as processed by the pipeline, and bookended by the start and stop times</returns>
        public Result<Cursor<Row>> Read(Selection selection, Pipeline pipeline, ZonedDateTime start, ZonedDateTime stop)
        {
            var query = new ReadQuery(new Search(Select.Type.Sensors, selection), new Read(start, stop));
            return Read(query);
        }

        /// <summary>
        /// Read data from a <code>ReadQuery</code>
        /// </summary>
        /// <param name="query"></param>
        /// <returns>The data from the devices and sensors which match your selection, 
        /// as processed by the pipeline, and bookended by the start and stop times</returns>
        public Result<Cursor<Row>> Read(ReadQuery query)
        {
            return Runner.Post<Segment<Row>>("v2/read/query", query).ToCursorResult<Row>();
        }
    }

    public static class SegmentCursorTableConversion
    {
        /// <summary>
        /// Extension method to transform Results 
        /// to <code>Result<Cursor></code>s from <code>Result<Segment>/s
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns>An <code>Result</code> wrapping the cursor equivalent to the 
        /// <code>Segment</code> in the original's <code>Value</code></returns>
        public static Result<Cursor<T>> ToCursorResult<T>(this Result<Segment<T>> result)
        {
            Cursor<T> cursor;
            if (result.Value == null)
                cursor = new Cursor<T>(new List<Segment<T>>());
            else
                cursor = new Cursor<T>(new List<Segment<T>> { result.Value });
            return new Result<Cursor<T>>(cursor, result.Code, result.Message, result.MultiStatus);
        }
    }
}