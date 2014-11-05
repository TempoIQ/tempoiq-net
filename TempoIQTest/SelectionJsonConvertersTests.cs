using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using TempoIQ.Queries;
using TempoIQ.Json;
using Newtonsoft.Json;
using TempoIQ.Utilities.Internal;

namespace TempoIQTest
{
    [TestFixture]
    public class SelectionJsonConvertersTests 
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();

        [SetUp]
        public void before()
        {
            settings.Converters.Add(new SelectorConverter());
        }
        
        [Test]
        public void SerializeSensorsTypeJsonConverterTest()
        {
            string sensors = "\"sensors\"";
            string serialized = JsonConvert.SerializeObject(Select.Type.Sensors);
            Assert.AreEqual(sensors, serialized);
        }

        [Test]
        public void DeserializeSensorsTypeJsonConverterTest()
        {
            string sensors = "\"sensors\"";
            var deserialized = JsonConvert.DeserializeObject<Select.Type>(sensors);
            Assert.AreEqual(Select.Type.Sensors, deserialized);
        }

        [Test]
        public void SerializeDevicesTypeJsonConverterTest()
        {
            string devices = "\"devices\"";
            string serialized = JsonConvert.SerializeObject(Select.Type.Devices);
            Assert.AreEqual(devices, serialized);
        }

        [Test]
        public void DeserializeDevicesTypeJsonConverterTest()
        {
            string devices = "\"devices\"";
            var deserialized = JsonConvert.DeserializeObject<Select.Type>(devices);
            Assert.AreEqual(Select.Type.Devices, deserialized);
        }

        [Test]
        public void DeserializeAllSelector()
        {
            string[] anyWords = {"\"any\"", "\"all\"", "\"*\""};
            var allSelectors = from word in anyWords select JsonConvert.DeserializeObject<AllSelector>(word);
            var allSelector = new AllSelector();
            allSelectors.Each((selector) => { Assert.AreEqual(allSelector, selector); });
        }

        [Test]
        public void SerializeAllSelector()
        {
            var all = new AllSelector();
            string serialized = JsonConvert.SerializeObject(all);
            Assert.AreEqual("\"all\"", serialized);
        }

        [Test]
        public void SerializeAndSelector()
        {
            var and = Select.And(Select.Attributes("attrKey", "value"), Select.AttributeKey("otherKey"), Select.Key("someKey"));
            string serialized = JsonConvert.SerializeObject(and);
            string expected = "{\"and\":[{\"attributes\":{\"attrKey\":\"value\"}},{\"attribute_key\":\"otherKey\"},{\"key\":\"someKey\"}]}";
            Assert.AreEqual(expected, serialized);
        }

        [Test]
        public void SerializeOrSelector()
        {
            var or = Select.Or(Select.Attributes("attrKey", "value"), Select.AttributeKey("otherKey"), Select.Key("someKey"));
            string serialized = JsonConvert.SerializeObject(or);
            string expected = "{\"or\":[{\"attributes\":{\"attrKey\":\"value\"}},{\"attribute_key\":\"otherKey\"},{\"key\":\"someKey\"}]}";
            Assert.AreEqual(expected, serialized);
        }
    }
}