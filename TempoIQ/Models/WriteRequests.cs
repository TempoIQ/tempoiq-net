using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoIQ.Json;
using TempoIQ.Models;
using Newtonsoft.Json;

namespace TempoIQ.Models
{
    using IWriteRequest = IDictionary<String, IDictionary<String, IList<DataPoint>>>;
    using WriteRequest = Dictionary<String, IDictionary<String, IList<DataPoint>>>;

    public static class WriteRequests {
        ///<summary>Adds a DataPoint to the request for a Device and Sensor.</summary>
        ///<param name="device">the Device to write to</param>
        ///<param name="sensor">the Sensor to write to</param>
        ///<param name="datapoint">the DataPoint to write</param>
        ///<returns>the updated request</returns>
        public static IWriteRequest Add(this IWriteRequest data, Device device, Sensor sensor, DataPoint datapoint)
        {
            return data.Add(device.Key, sensor.Key, datapoint);
        }

        ///<sumamary>Adds a DataPoint to the request for a Sensor.</summary>
        ///<param name="deviceKey"> The Device to write to's key.</param>
        ///<param name="sensorKey"> The Sensor to write to's key.</param>
        ///<param name="datapoint"> The DataPoint to write to.</param>
        ///<returns>the updated request</returns>
        public static IWriteRequest Add(this IWriteRequest data, string deviceKey, string sensorKey, DataPoint datapoint)
        {
            if (data.ContainsKey(deviceKey))
            {
                var innerDict = data[deviceKey];
                if (innerDict.ContainsKey(sensorKey))
                    innerDict[sensorKey].Add(datapoint);
                else
                    innerDict[sensorKey] = new List<DataPoint> { datapoint };
            }
            else
            {
                var map = new Dictionary<string, IList<DataPoint>>();
                map.Add(sensorKey, new List<DataPoint> { datapoint });
                data.Add(deviceKey, map);
            }
            return data;
        }

        ///<sumamary>Adds a list of DataPoints to the request for a Sensor.</summary>
        ///<param name="deviceKey"> The Device to write to's key.</param>
        ///<param name="sensorKey"> The Sensor to write to's key.</param>
        ///<param name="datapoints"> The DataPoints to write to.</param>
        ///<returns>the updated request</returns>
        public static IWriteRequest Add(this IWriteRequest data, string deviceKey, string sensorKey, IList<DataPoint> datapoints)
        {
            if (data.ContainsKey(deviceKey))
            {
                var innerDict = data[deviceKey];
                if (innerDict.ContainsKey(sensorKey))
                    foreach (var dp in datapoints)
                        innerDict[sensorKey].Add(dp);
                else
                    innerDict[sensorKey] = datapoints;
            }
            else
            {
                var innerDict = new Dictionary<string, IList<DataPoint>>();
                innerDict.Add(sensorKey, datapoints);
                data.Add(deviceKey, innerDict);
            }
            return data;
        }

        ///<sumamary>Adds a list of DataPoints to the request for a Sensor.</summary>
        ///<param name="device"> The Device to write to.</param>
        ///<param name="sensor"> The Sensor to write to.</param>
        ///<param name="datapoints"> The DataPoints to write to.</param>
        ///<returns>the updated request</returns>
        public static IWriteRequest Add(this IWriteRequest data, Device device, Sensor sensor, IList<DataPoint> datapoints)
        {
            return data.Add(device.Key, sensor.Key, datapoints);
        }
    }
}
