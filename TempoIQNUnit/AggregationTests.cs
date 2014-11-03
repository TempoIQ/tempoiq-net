using System;
using TempoIQ;
using TempoIQ.Models;
using TempoIQ.Queries;
using NUnit.Framework;
using NodaTime;

namespace TempoIQNUnit
{

    [TestFixture]
    public class AggregationTests
    {
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