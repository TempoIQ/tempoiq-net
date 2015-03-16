using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using TempoIQ.Queries;
using TempoIQ.Json;
using TempoIQ.Models;
using Newtonsoft.Json;
using TempoIQ.Utilities.Internal;

namespace TempoIQTests
{
    [TestFixture]
    public class UpsertJsonTests
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();

        [SetUp]
        public void before()
        {
            settings.Converters.Add(new DeviceStateConverter());
            //settings.Converters.Add(new UpsertResponseConverter());
        }

        [Test]
        public void TestDeserializeBasicUpsertResponse()
        {
            string response = "{" +
                                    "\"device1\": {" +
                                        "\"device_state\": \"existing\"," +
                                        "\"message\": null," +
                                        "\"success\": true" +
                                    "}" +
                              "}";
            UpsertResponse deserialized = JsonConvert.DeserializeObject<UpsertResponse>(response, settings);
            Assert.AreEqual(deserialized.Existing.First().Key, "device1");
            Assert.AreEqual(deserialized.Existing.First().Value.State, DeviceState.Existing);
        }
    }
}

