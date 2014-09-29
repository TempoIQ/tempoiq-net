using System;
using TempoIQ;
using TempoIQ.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TempoIQTest
{
    [TestClass]
    public class AggregationTest
    {
        [TestMethod]
        public void EqualsTest()
        {
            var max = new Aggregation(Fold.Max);
            var max2 = new Aggregation(Fold.Max);
            Assert.IsFalse(max.Equals(max2));
        }

        [TestMethod]
        public void NotEqualsFoldTest()
        {
            var max = new Aggregation(Fold.Max);
            var min = new Aggregation(Fold.Min);
            Assert.IsFalse(max.Equals(min));
        }

        [TestMethod]
        public void NotEqualsNullTest()
        {
            var agg = new Aggregation(Fold.Max);
            Assert.IsFalse(agg == null);
        }
    }
}
