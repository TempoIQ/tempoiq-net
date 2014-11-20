using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NodaTime;
using TempoIQ.Utilities;
using TempoIQ.Models;
using TempoIQ.Json;

namespace TempoIQ.Results
{
    /// <summary>
    /// The Cursor is the toplevel collection of TempoIQ data from a query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Cursor<T> : IEnumerable<T>
    {
        public Segment<T> First { get; private set; }
        private Executor Runner { get; set; }
        private string EndPoint { get; set; }
        private string MediaTypeVersion { get; set; }

        public Cursor(Segment<T> segment, Executor runner, string endPoint, string mediaTypeVersion)
        {
            this.First = segment;
            this.Runner = runner;
            this.EndPoint = endPoint;
            this.MediaTypeVersion = mediaTypeVersion;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var pages = new PageLoader<T>(Runner, First, EndPoint, MediaTypeVersion);
            do
            {
                foreach (var t in pages.Current)
                    yield return t;
            } while (pages.MoveNext());
            yield break;
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (obj is Cursor<T>)
                return this.Equals(obj);
            else return false;
        }

        public bool Equals(Cursor<T> obj)
        {
            return obj.SequenceEqual(this);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash<int>(hash, this.First.GetHashCode());
            return hash;
        }
    }

    /// <summary>
    /// Extension methods for Cursor<Row>
    /// </summary>
    public static class CursorRowExtension
    {
        /// <summary>
        /// exposes an enumeration of a Cursor's data as a series of Tuples 
        /// of strings (device, sensor keys) by DataPoint
        /// </summary>
        /// <param name="cursor"></param>
        /// <returns>The flattened version of the cursor</returns>
        public static IEnumerable<Tuple<string, string, DataPoint>> Flatten(this Cursor<Row> cursor)
        {
            return from row in cursor
                   from cell in row
                   select Tuple.Create(cell.Item1, cell.Item2, new DataPoint(row.Timestamp, cell.Item3));
        }

        /// <summary>
        /// Expose an IEnumerable over the data from a given device and sensor
        /// </summary>
        /// <param name="cursor"></param>
        /// <param name="deviceKey"></param>
        /// <param name="sensorKey"></param>
        /// <returns>The Cursor's data for a given device/sensor pair</returns>
        public static IEnumerable<DataPoint> PointsForDeviceAndSensor(this Cursor<Row> cursor, string deviceKey, string sensorKey)
        {
            return from row in cursor
                   from cell in row
                   where cell.Item1 == deviceKey && cell.Item2 == sensorKey
                   select new DataPoint(row.Timestamp, cell.Item3);
        }

        /// <summary>
        /// Expose an IDictionary of the data from a given device key indexed by sensor key
        /// </summary>
        /// <param name="cursor"></param>
        /// <param name="deviceKey"></param>
        /// <returns>The Cursor's data for a given device</returns>
        public static IDictionary<string, IEnumerable<DataPoint>> PointsForDevice(this Cursor<Row> cursor, string deviceKey)
        {
            var sensors = from row in cursor
                          from cell in row
                          where cell.Item1 == deviceKey
                          select cell.Item2;

            return sensors.Distinct().ToDictionary(s => s, s => cursor.PointsForDeviceAndSensor(deviceKey, s));
        }

        /// <summary>
        /// Expose an IDictionary of the data from a Cursor as an IDictionary indexed by Device and Sensor
        /// </summary>
        /// <param name="cursor"></param>
        /// <returns>The Cursor's data indexed by device and sensor, sorted by timestamp</returns>
        public static IDictionary<string, IDictionary<string, IEnumerable<DataPoint>>> PointsByStream(this Cursor<Row> cursor)
        {
            var devices = from row in cursor
                          from cell in row
                          select cell.Item2;

            return devices.Distinct().ToDictionary(d => d, d => cursor.PointsForDevice(d));
        }
    }
}