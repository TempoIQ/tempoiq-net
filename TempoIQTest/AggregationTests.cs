using System;
using TempoIQ;
using TempoIQ.Models;
using TempoIQ.Querying;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;

namespace TempoIQTest
{

    [TestClass]
    public class AggregationTests
    {
        [TestMethod]
        public void Equality()
        {
            var aggregation1 = new Aggregation(Fold.Sum);
            var aggregation2 = new Aggregation(Fold.Sum);
            Assert.AreEqual(aggregation1, aggregation2);
        }

        [TestMethod]
        public void NotEquals()
        {
            var aggregation1 = new Aggregation(Fold.Sum);
            var aggregation2 = new Aggregation(Fold.Mean);
            Assert.AreNotEqual(aggregation1, aggregation2);
        }
    }

    [TestClass]
    public class RollupTests
    {
        [TestMethod]
        public void Defaults()
        {
            var period = Period.FromMinutes(1);
            var rollup = new Rollup(period, Fold.Sum);
            var defaultRollup = new Rollup(period);
            Assert.AreEqual(Fold.Sum, rollup.Fold);
            Assert.AreEqual(period, rollup.Period);
            Assert.AreEqual(defaultRollup.Period, rollup.Period);
        }

        [TestMethod]
        public void Equality()
        {
            var timestamp = DateTimeZone.Utc.AtStrictly(LocalDateTime.FromDateTime(DateTime.Now));
            var rollup1 = new Rollup(Period.FromMinutes(1), Fold.Sum, timestamp);
            var rollup2 = new Rollup(Period.FromMinutes(1), Fold.Sum, timestamp);
            Assert.AreEqual(rollup1, rollup2);
        }
    }
}