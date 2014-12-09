using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempoIQ.Queries
{
    public enum DirectionFunction
    {
        /// <summary>
        /// The point with the latest timestamp. Does not take a timestamp as an argument
        /// </summary>
        Latest,

        /// <summary>
        /// The point with the earliest timestamp. Does not take a timestamp as an argument
        /// </summary>
        Earliest,

        /// <summary>
        /// The point with the timestamp nearest the one provided. Takes a timestamp as an argument
        /// </summary>
        Nearest,

        /// <summary>
        /// The point with the nearest timestamp before the one provided. Takes a timestamp as an argument
        /// </summary>
        Before,
        
        /// <summary>
        /// The point with the nearest timestamp after the one provided. Takes a timestamp as an argument
        /// </summary>
        After,

        /// <summary>
        /// The point with the exact timestamp provided if any. Takes a timestamp as an argument
        /// </summary>
        Exact
    }
}