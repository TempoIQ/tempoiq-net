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
    /// A Row represents the <code>DataPoints</code> of several <code>Sensors</code> and/or <code>Devices</code>
    /// for a given time.
    /// </summary>
    /// <para>A Row returns the "cells" from "table" that a 
    /// <code>Cursor</code> of <code>Row</code>s implicitly defines </para>
    [JsonObject]
    public class Row : IEnumerable<Tuple<string, string, double>>, IModel
    {
        [JsonProperty("t")]
        public ZonedDateTime Timestamp { get; set; }

        [JsonProperty("data")]
        public IDictionary<string, IDictionary<string, double>> Data { get; set; }

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
            return Data.ContainsKey(deviceKey) && (Data[deviceKey]).ContainsKey(sensorKey);
        }

        public IDictionary<string, double> Get(string deviceKey)
        {
            if (Data.ContainsKey(deviceKey)) 
                return Data[deviceKey];
            else 
                return new Dictionary<string, double>();
        }

        public double? Get(string deviceKey, string sensorKey)
        {
            var deviceData = Get(deviceKey);
            if (deviceData.ContainsKey(sensorKey))
                return deviceData[sensorKey];
            else 
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
