using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoIQ.Json;
using Newtonsoft.Json;

namespace TempoIQ.Models
{
    ///<summary>A request for writing multiple DataPoints to multiple Sensors.</summary>
    ///<para>The request is created and datapoints are added for a Sensor.</para>
    
    //[JsonConverter(typeof(WriteRequestConverter))]
    public class WriteRequest : Dictionary<String, IDictionary<String, IList<DataPoint>>>
    {
        [JsonConstructor]
        public WriteRequest(IDictionary<String, IDictionary<String, IList<DataPoint>>> data) 
            : base(new Dictionary<String, IDictionary<String, IList<DataPoint>>>())
        {
            foreach(var pair in data)
            {
                this.Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Base constructor; create a new WriteRequest
        /// </summary>
        public WriteRequest() : base(new Dictionary<String, IDictionary<String, IList<DataPoint>>>())
        {
            ;
        }

        ///<summary>Adds a DataPoint to the request for a Device and Sensor.</summary>
        ///<param name="device">the Device to write to</param>
        ///<param name="sensor">the Sensor to write to</param>
        ///<param name="datapoint">the DataPoint to write</param>
        ///<returns>the updated request</returns>
        public WriteRequest Add(Device device, Sensor sensor, DataPoint datapoint)
        {
            return this.Add(device.Key, sensor.Key, datapoint);
        }

        ///<sumamary>Adds a DataPoint to the request for a Sensor.</summary>
        ///<param name="deviceKey"> The Device to write to's key.</param>
        ///<param name="sensorKey"> The Sensor to write to's key.</param>
        ///<param name="datapoint"> The DataPoint to write to.</param>
        ///<returns>the updated request</returns>
        public WriteRequest Add(string deviceKey, string sensorKey, DataPoint datapoint)
        {
            if (this.ContainsKey(deviceKey))
            {
                var innerDict = this[deviceKey];
                if (innerDict.ContainsKey(sensorKey))
                {
                    innerDict[sensorKey].Add(datapoint);
                }
                else 
                {
                    innerDict[sensorKey] = new List<DataPoint>();
                    innerDict[sensorKey].Add(datapoint);
                }
            }
            else
            {
                var map = new Dictionary<string, IList<DataPoint>>();
                map.Add(sensorKey, new List<DataPoint>(new DataPoint[]{datapoint}));
                this.Add(deviceKey, map);
            }
            return this;
        }

        ///<sumamary>Adds a list of DataPoints to the request for a Sensor.</summary>
        ///<param name="deviceKey"> The Device to write to's key.</param>
        ///<param name="sensorKey"> The Sensor to write to's key.</param>
        ///<param name="datapoints"> The DataPoints to write to.</param>
        ///<returns>the updated request</returns>
        public WriteRequest Add(string deviceKey, string sensorKey, IList<DataPoint> datapoints)
        {
            if (this.ContainsKey(deviceKey))
            {
                var innerDict = this[deviceKey];
                if (innerDict.ContainsKey(sensorKey))
                {
                    foreach (var dp in datapoints)
                    {
                        innerDict[sensorKey].Add(dp);
                    }
                }
                else 
                {
                    innerDict[sensorKey] = datapoints;
                }
            }
            else
            {
                var innerDict = new Dictionary<string, IList<DataPoint>>();
                innerDict.Add(sensorKey, datapoints);
                this.Add(deviceKey, innerDict);
            }
            return this;
        }

        ///<sumamary>Adds a list of DataPoints to the request for a Sensor.</summary>
        ///<param name="device"> The Device to write to.</param>
        ///<param name="sensor"> The Sensor to write to.</param>
        ///<param name="datapoints"> The DataPoints to write to.</param>
        ///<returns>the updated request</returns>
        public WriteRequest Add(Device device, Sensor sensor, IList<DataPoint> datapoints)
        {
            return Add(device.Key, sensor.Key, datapoints);
        }
    }
}