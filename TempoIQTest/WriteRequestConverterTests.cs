using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TempoIQ.Models;
using TempoIQ.Json;

namespace TempoIQTest
{
    [TestClass]
    public class WriteRequestConverterTests
    {
        [TestMethod]
        public void SerializeWriteRequestTest()
        {
            var json = "{" +
                        "\"device-1\":{\"sensor-1\":[{\"t\":\"2014-01-01T00:00:00Z\",\"v\":1.0}]},"+
                        "\"device-2\":{\"sensor-1\":[{\"t\":\"2014-01-01T00:00:00Z\",\"v\":1.0}]}"+
                        "}";
            var reqIn = JsonConvert.DeserializeObject<WriteRequest>(json, TempoIQSerializer.Converters);
            var reqOut = JsonConvert.SerializeObject(reqIn);
            Assert.AreEqual(json, reqOut);
            Assert.AreEqual(reqIn, JsonConvert.DeserializeObject<WriteRequest>(reqOut));
        }

        [TestMethod]
        public void DeserializeWriteRequestTest()
        {
            var json = "{\"device-1\":{\"sensor-1\":[{\"t\":\"2014-01-01T00:00:00Z\",\"v\":1.0}]},\"device-2\":{\"sensor-1\":[{\"t\":\"2014-01-01T00:00:00Z\",\"v\":1.0}]}}";
            var req = JsonConvert.DeserializeObject<WriteRequest>(json, TempoIQSerializer.Converters);
            Assert.IsInstanceOfType(req, typeof(WriteRequest));
        }
    }
}
