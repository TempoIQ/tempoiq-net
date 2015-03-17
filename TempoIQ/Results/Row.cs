using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NodaTime;
using TempoIQ.Utilities;
using TempoIQ.Models;

namespace TempoIQ.Results
{
    /// <summary>
    /// A Row represents the DataPoints of several Sensors and/or Devices
    /// for a given time.
    /// </summary>
    /// <para>A Row returns the "cells" from "table" that a 
    /// Cursor of Rows implicitly defines. Each cell denotes the 
    /// DeviceKey, SensorKey, and Value of the sensor at the row's Timestamp</para>
    [JsonObject]
    public class Row : IEnumerable<Tuple<string, string, double>>, IModel
    {
        [JsonProperty("t")]
        public ZonedDateTime Timestamp { get; set; }

        [JsonProperty("data")]
        public IDictionary<string, IDictionary<string, double>> Data { get; set; }

        [JsonIgnore]
        public static string CursoredMediaTypeVersion
        {
            get { return "application/prs.tempoiq.datapoint-collection.v2"; }
        }

        public IEnumerator<Tuple<string, string, double>> GetEnumerator()
        {
            foreach (var deviceSensorsPair in Data)
                foreach (var sensorDataPair in deviceSensorsPair.Value)
                    yield return Tuple.Create(deviceSensorsPair.Key, sensorDataPair.Key, sensorDataPair.Value);
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool HasSensor(string deviceKey, string sensorKey)
        {
            if (sensorKey == null)
                return false;
            IDictionary<string, double> deviceData;
            return Data.TryGetValue(deviceKey, out deviceData) && deviceData.ContainsKey(sensorKey);
        }

        public IDictionary<string, double> Get(string deviceKey)
        {
            IDictionary<string, double> deviceData;
            Data.TryGetValue(deviceKey, out deviceData);
            return deviceData;
        }

        public double? Get(string deviceKey, string sensorKey)
        {
            IDictionary<string, double> deviceData;
            if (Data.TryGetValue(deviceKey, out deviceData))
                if (deviceData.ContainsKey(sensorKey))
                    return deviceData[sensorKey];
            return null;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (obj is Row)
                return this.Equals(obj);
            else 
                return false;
        }

        public bool Equals(Row obj)
        {
            return obj.SequenceEqual(this);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, this.Data);
            return hash;
        }
    }
}
