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
    /// <remarks>available since version 1.0</remarks>
    public enum Fold
    {
        /// <summary>
        /// Count of Datapoints
        /// </summary>
        /// <remarks>available since version 1.0</remarks>
        Count,
        /// <summary>
        /// First Datapoint in interval (takes the leftmost/earliest Datapoint)
        /// </summary>
        /// <remarks>available since version 1.0</remarks>
        First,
        /// <summary>
        /// Maximum of Datapoints
        /// </summary>
        /// <remarks>available since version 1.0</remarks>
        Max,
        /// <summary>
        /// Arithmetic mean of Datapoints (average)
        /// </summary>
        /// <remarks>available since version 1.0</remarks>
        Mean,
        /// <summary>
        /// Minimum of Datapoints
        /// </summary>
        /// <remarks>available since version 1.0</remarks>
        Min,
        /// <summary>
        /// Multiplication of Datapoints
        /// </summary>
        /// <remarks>available since version 1.0</remarks>
        Mult,
        /// <summary>
        /// Range of Datapoints (max - min)
        /// </summary>
        /// <remarks>available since version 1.0</remarks>
        Range,
        /// <remarks>available since version 1.0</remarks>
        /// <summary>
        /// Standard deviation of Datapoints
        /// </summary>
        /// <remarks>available since version 1.0</remarks>
        StdDev,
        /// <summary>
        /// Sum of Datapoints
        /// </summary>
        /// <remarks>available since version 1.0</remarks>
        Sum,
        /// <summary>
        /// Variance of Datapoints
        /// </summary>
        /// <remarks>available since version 1.0</remarks>
        Variance
    };
}