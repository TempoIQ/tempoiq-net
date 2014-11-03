using System;
using TempoIQ;
using TempoIQ.Queries;
using NUnit.Framework;

namespace TempoIQNUnit
{
    [TestFixture]
    public class AggregationTest
    {
        [Test]
        public void EqualsTest()
        {
            var max = new Aggregation(Fold.Max);
            var max2 = new Aggregation(Fold.Max);
            Assert.IsFalse(max.Equals(max2));
        }

        [Test]
        public void NotEqualsFoldTest()
        {
            var max = new Aggregation(Fold.Max);
            var min = new Aggregation(Fold.Min);
            Assert.IsFalse(max.Equals(min));
        }

        [Test]
        public void NotEqualsNullTest()
        {
            var agg = new Aggregation(Fold.Max);
            Assert.IsFalse(agg == null);
        }
    }
}
