using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NodaTime;
using TempoIQ.Utilities;
using TempoIQ.Models;

namespace TempoIQ.Results
{
    /// <summary>
    /// The <code>Cursor</code> is the toplevel collection of TempoIQ data from a query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Cursor<T> : IEnumerable<T>, Model
    {
        public IEnumerable<Segment<T>> Segments { get; private set; }

        public Cursor(IEnumerable<Segment<T>> segments)
        {
            this.Segments = segments;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var segment in Segments)
                foreach (var item in segment)
                    yield return item;
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (obj is Cursor<T>)
                return this.Equals(obj);
            else return false;
        }

        public bool Equals(Cursor<T> obj)
        {
            return obj.SequenceEqual(this);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash<int>(hash, this.Segments.GetHashCode());
            return hash;
        }
    }

    /// <summary>
    /// Extension methods for <code>Cursor<Row></code>
    /// </summary>
    public static class CursorRowExtension
    {
        /// <summary>
        /// exposes an enumeration of a <code>Cursor</code>'s data as a series of Tuples 
        /// of strings (device, sensor keys) by <code>DataPoint</code>
        /// </summary>
        /// <param name="cursor"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<string, string, DataPoint>> Flatten(this Cursor<Row> cursor)
        {
            return from row in cursor
                   from cell in row
                   select Tuple.Create(cell.Item1, cell.Item2, new DataPoint(row.Timestamp, cell.Item3));
        }
    }
}