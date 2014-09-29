using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TempoIQ.Models;
using TempoIQ.Results;
using TempoIQ.Json;
using Newtonsoft.Json;

namespace TempoIQTest.Json
{
    [TestClass]
    public class SegmentJsonConverterTests
    {
        [TestMethod]
        public void SerializeEmptySegmentTest()
        {
            var segmentIn = new Segment<DataPoint>(new List<DataPoint>(), "no next");
            var segment = JsonConvert.SerializeObject(segmentIn);
            var segmentOut = JsonConvert.DeserializeObject<Segment<DataPoint>>(segment);
            Assert.IsInstanceOfType(segmentOut, typeof(Segment<DataPoint>));
        }

        [TestMethod]
        public void DeserializeEmptySegmentTest()
        {
            var segmentStr = "{\"data\":[]}";
            var segmentIn = JsonConvert.DeserializeObject<Segment<DataPoint>>(segmentStr);
            Assert.IsInstanceOfType(segmentIn, typeof(Segment<DataPoint>));
        }
    }
}