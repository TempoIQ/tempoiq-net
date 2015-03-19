using System;
using System.Collections.Generic;
using TempoIQ;
using TempoIQ.Models;
using TempoIQ.Queries;
using NUnit.Framework;
using NodaTime;

namespace TempoIQTests
{

    [TestFixture]
    public class AggregationTests
    {
        [Test]
        public void EqualsTest()
        {
            var max = new Aggregation(Fold.Max);
            var max2 = new Aggregation(Fold.Max);
            Assert.AreEqual(max, max2);
        }

        [Test]
        public void NotEqualsFoldTest()
        {
            var max = new Aggregation(Fold.Max);
            var min = new Aggregation(Fold.Min);
            Assert.AreNotEqual(max, min);
        }

        [Test]
        public void NotEqualsNullTest()
        {
            var agg = new Aggregation(Fold.Max);
            Assert.IsFalse(agg == null);
        }

        [Test]
        public void Equality()
        {
            var aggregation1 = new Aggregation(Fold.Sum);
            var aggregation2 = new Aggregation(Fold.Sum);
            Assert.AreEqual(aggregation1, aggregation2);
        }

        [Test]
        public void NotEquals()
        {
            var aggregation1 = new Aggregation(Fold.Sum);
            var aggregation2 = new Aggregation(Fold.Mean);
            Assert.AreNotEqual(aggregation1, aggregation2);
        }
    }

    [TestFixture]
    public class RollupTests
    {
        [Test]
        public void Equality()
        {
            var timestamp = DateTimeZone.Utc.AtStrictly(LocalDateTime.FromDateTime(DateTime.Now));
            var rollup1 = new Rollup(Period.FromMinutes(1), Fold.Sum, timestamp);
            var rollup2 = new Rollup(Period.FromMinutes(1), Fold.Sum, timestamp);
            Assert.AreEqual(rollup1, rollup2);
        }
    }
}
