using System;
using System.Collections.Generic;
using NUnit.Framework;
using TempoIQ.Models;
using TempoIQ.Results;
using TempoIQ.Json;
using Newtonsoft.Json;

namespace TempoIQNUnit
{
    [TestFixture]
    public class SegmentJsonConverterTests
    {
        [Test]
        public void SerializeEmptySegmentTest()
        {
            var segmentIn = new Segment<DataPoint>(new List<DataPoint>(), "no next");
            var segment = JsonConvert.SerializeObject(segmentIn);
            var segmentOut = JsonConvert.DeserializeObject<Segment<DataPoint>>(segment);
            Assert.IsInstanceOfType(typeof(Segment<DataPoint>), segmentOut);
        }

        [Test]
        public void DeserializeEmptySegmentTest()
        {
            var segmentStr = "{\"data\":[]}";
            var segmentIn = JsonConvert.DeserializeObject<Segment<DataPoint>>(segmentStr);
            Assert.IsInstanceOfType(typeof(Segment<DataPoint>), segmentIn);
        }
    }
}