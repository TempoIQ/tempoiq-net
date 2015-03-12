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
                foreach (DeviceStatus status in this.Values) 
                {
                    if (status.Success == false)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool PartialSuccess
        {
            get
            {
                int failures = 0;
                foreach (DeviceStatus status in this.Values) 
                {
                    if (status.Success == false) {
                        failures += 1;
                    }
                }
                if (failures < this.Values.Count) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        public IDictionary<String, DeviceStatus> Existing
        {
            get 
            {
                return this.Where(kvp => kvp.Value.State == DeviceState.Existing).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }

        public IDictionary<String, DeviceStatus> Created
        {
            get 
            {
                return this.Where(kvp => kvp.Value.State == DeviceState.Created).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }

        public IDictionary<String, DeviceStatus> Modified
        {
            get 
            {
                return this.Where(kvp => kvp.Value.State == DeviceState.Modified).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }
    }
}

