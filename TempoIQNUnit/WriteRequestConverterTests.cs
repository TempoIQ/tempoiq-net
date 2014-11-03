using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;
using TempoIQ.Models;
using TempoIQ.Json;

namespace TempoIQNUnit
{
    [TestFixture]
    public class WriteRequestConverterTests
    {
        public ZonedDateTimeConverter TimeConverter { get { return new ZonedDateTimeConverter(); } }

        [Test]
        public void SerializeWriteRequestTest()
        {
            var json = "{" +
                       "\"device-1\":{\"sensor-1\":[{\"t\":\"2014-01-01T00:00:00Z\",\"v\":1.0}]}," +
                       "\"device-2\":{\"sensor-1\":[{\"t\":\"2014-01-01T00:00:00Z\",\"v\":1.0}]}" +
                       "}";
            var reqIn = JsonConvert.DeserializeObject<WriteRequest>(json, TempoIQSerializer.Converters);
            var reqOut = JsonConvert.SerializeObject(reqIn, TempoIQSerializer.Converters);
            JsonConvert.DeserializeObject<WriteRequest>(reqOut, TempoIQSerializer.Converters);
        }

        [Test]
        public void DeserializeWriteRequestTest()
        {
            var json = "{\"device-1\":{\"sensor-1\":[{\"t\":\"2014-01-01T00:00:00Z\",\"v\":1.0}]},\"device-2\":{\"sensor-1\":[{\"t\":\"2014-01-01T00:00:00Z\",\"v\":1.0}]}}";
            JsonConvert.DeserializeObject<WriteRequest>(json, TempoIQSerializer.Converters);
        }
    }
}
