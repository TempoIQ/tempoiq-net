using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoIQ.Aggregation
{
    /// <summary>
    /// The representation of an aggregation between multiple <c>Sensor</c>s
    /// </summary>
    /// <para>
    ///  An Aggregation allows you to specify a new Sensor that is a mathematical
    ///  operation across multiple Sensor. For instance, the following Aggregation
    ///  specifies the sum of multiple Sensor:
    class Aggregation
    {
        /// <summary>
        /// The <c>Aggregation</c>'s underlying <c>Fold</c> function
        /// </summary>
        public Fold Fold { get; private set; }

        public Aggregation(Fold fold)
        {
            this.Fold = fold;
        }

        /// <para>
        /// <c>Aggregation aggregation = new Aggregation(Fold.SUM);</c>
        /// </para>
        public Aggregation()
        {
            this.Fold = Fold.Sum;
        }
    }
}