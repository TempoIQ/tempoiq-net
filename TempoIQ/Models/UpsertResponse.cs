using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using TempoIQ.Json;

namespace TempoIQ.Models
{
    public enum DeviceState {
        Existing,
        Modified,
        Created
    }

    public class DeviceStatus
    {
        [JsonConverter(typeof(DeviceStateConverter))]
        public DeviceState State { get; set; }
        public bool Success { get; set; }
        public String Message { get; set; }
    }

    public class UpsertResponse : Dictionary<String, DeviceStatus>
    {
        [JsonConstructor]
        public UpsertResponse(IDictionary<String, DeviceStatus> data) : base()
        {
            foreach(var pair in data)
                this.Add(pair.Key, pair.Value);
        }

        public bool Success
        {
            get
            {
                return this.Values.All(status => status.Success);
            }
        }

        public bool PartialSuccess
        {
            get
            {
                return this.Values.Count(status => ! status.Success) < this.Values.Count;
            }
        }

        public IDictionary<String, DeviceStatus> Existing
        {
            get 
            {
                return this.Where(kvp => kvp.Value.State == DeviceState.Existing) as IDictionary<String, DeviceStatus>;
            }
        }

        public IDictionary<String, DeviceStatus> Created
        {
            get 
            {
                return this.Where(kvp => kvp.Value.State == DeviceState.Created) as IDictionary<String, DeviceStatus>;
            }
        }

        public IDictionary<String, DeviceStatus> Modified
        {
            get 
            {
                return this.Where(kvp => kvp.Value.State == DeviceState.Modified) as IDictionary<String, DeviceStatus>;
            }
        }
    }
}
