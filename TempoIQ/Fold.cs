using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoIQ
{
    /// <summary>
    /// Folding function for <see cref="TempoIQ.Aggregation"/>
    /// </summary>
    public enum Fold
    {
        /// <summary>
        /// Count of Datapoints
        /// </summary>
        Count,

        /// <summary>
        /// First Datapoint in interval (takes the leftmost/earliest Datapoint)
        /// </summary>
        First,

        /// <summary>
        /// Maximum of Datapoints
        /// </summary>
        Max,
        
        /// <summary>
        /// Arithmetic mean of Datapoints (average)
        /// </summary>
        Mean,

        /// <summary>
        /// Minimum of Datapoints
        /// </summary>
        Min,

        /// <summary>
        /// Multiplication of Datapoints
        /// </summary>
        Mult,

        /// <summary>
        /// Range of Datapoints (max - min)
        /// </summary>
        Range,

        /// <remarks>available since version 1.0</remarks>
        /// <summary>
        /// Standard deviation of Datapoints
        /// </summary>
        StdDev,

        /// <summary>
        /// Sum of Datapoints
        /// </summary>
        Sum,

        /// <summary>
        /// Variance of Datapoints
        /// </summary>
        Variance
    };
}