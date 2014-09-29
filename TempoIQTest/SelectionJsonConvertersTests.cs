using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TempoIQ.Querying;
using TempoIQ.Json;
using Newtonsoft.Json;

namespace TempoIQTest
{
    [TestClass]
    public class SelectionJsonConvertersTests 
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        [TestInitialize]
        public void before()
        {
            settings.Converters.Add(new SelectorConverter());
        }
        
        [TestMethod]
        public void SerializeSensorsTypeJsonConverterTest()
        {
            string sensors = "\"sensors\"";
            string serialized = JsonConvert.SerializeObject(Selectors.Type.Sensors);
            Assert.AreEqual(sensors, serialized);
        }

        [TestMethod]
        public void DeserializeSensorsTypeJsonConverterTest()
        {
            string sensors = "\"sensors\"";
            var deserialized = JsonConvert.DeserializeObject<Selectors.Type>(sensors);
            Assert.AreEqual(Selectors.Type.Sensors, deserialized);
        }

        [TestMethod]
        public void SerializeDevicesTypeJsonConverterTest()
        {
            string devices = "\"devices\"";
            string serialized = JsonConvert.SerializeObject(Selectors.Type.Devices);
            Assert.AreEqual(devices, serialized);
        }

        [TestMethod]
        public void DeserializeDevicesTypeJsonConverterTest()
        {
            string devices = "\"devices\"";
            var deserialized = JsonConvert.DeserializeObject<Selectors.Type>(devices);
            Assert.AreEqual(Selectors.Type.Devices, deserialized);
        }

        [TestMethod]
        public void DeserializeAllSelector()
        {
            string[] anyWords = {"\"any\"", "\"all\"", "\"*\""};
            var allSelectors = from word in anyWords select JsonConvert.DeserializeObject<AllSelector>(word);
            foreach (var all in allSelectors);
            //force evaluation of lazy sequence. This will throw an exception if something is amiss
        }

        [TestMethod]
        public void SerializeAllSelector()
        {
            var all = new AllSelector();
            string serialized = JsonConvert.SerializeObject(all);
            Assert.AreEqual("\"all\"", serialized);
        }

        [TestMethod]
        public void SerializeAndSelector()
        {
            var and = Selectors.And(Selectors.Attribute("attrKey", "value"), Selectors.AttributeKey("otherKey"), Selectors.Key("someKey"));
            string serialized = JsonConvert.SerializeObject(and);
            string expected = "{\"and\":[{\"attributes\":{\"attrKey\":\"value\"}},{\"attribute_key\":\"otherKey\"},{\"key\":\"someKey\"}]}";
            Assert.AreEqual(expected, serialized);
        }

/*
        [TestMethod]
        public void DeserializeAndSelector()
        {
            string andIn = "{\"and\":[{\"attributes\":{\"attrKey\":\"value\"}},{\"attribute_key\":\"otherKey\"},{\"key\":\"someKey\"}]}";
            var expected = Selectors.And(Selectors.Attribute("attrKey", "value"), Selectors.AttributeKey("otherKey"), Selectors.Key("someKey"));
            var andOut = JsonConvert.DeserializeObject<AndSelector>(andIn);
            Assert.AreEqual(expected, andOut);
        }
*/

        [TestMethod]
        public void SerializeOrSelector()
        {
            var or = Selectors.Or(Selectors.Attribute("attrKey", "value"), Selectors.AttributeKey("otherKey"), Selectors.Key("someKey"));
            string serialized = JsonConvert.SerializeObject(or);
            string expected = "{\"or\":[{\"attributes\":{\"attrKey\":\"value\"}},{\"attribute_key\":\"otherKey\"},{\"key\":\"someKey\"}]}";
            Assert.AreEqual(expected, serialized);
        }

/*
        [TestMethod]
        public void DeserializeOrSelector()
        {
            string orIn = "{\"or\":[{\"attributes\":{\"attrKey\":\"value\"}},{\"attribute_key\":\"otherKey\"},{\"key\":\"someKey\"}]}";
            var expected = Selectors.And(Selectors.Attribute("attrKey", "value"), Selectors.AttributeKey("otherKey"), Selectors.Key("someKey"));
            var orOut = JsonConvert.DeserializeObject<OrSelector>(orIn);
            Assert.AreEqual(expected, orOut);
        }
*/
    }
}
