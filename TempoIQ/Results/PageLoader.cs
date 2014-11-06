using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TempoIQ.Utilities;
using TempoIQ.Json;
using TempoIQ.Models;

namespace TempoIQ.Results
{
    class PageLoader<T> : IEnumerator<Segment<T>>
    {
        public Segment<T> Current { get; private set; }

        private Executor Runner { get; set; }

        private Segment<T> First { get; set; }

        public PageLoader(Segment<T> first)
        {
            Current = first;
            First = first;
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            string resource = (String)(typeof(T).GetProperty("Resource").GetValue(null, null));
            string endpoint = String.Format("{0}/{1}/query", Client.API_VERSION, resource);
            var result = Runner.Execute<Segment<T>>(RestSharp.Method.POST, endpoint, new RawBodyWrapper(Current.Next));
            if (result.State.Equals(State.Success))
            {
                Current = result.Value;
                return true;
            } else
            {
                return false;
            }
        }

        public void Reset()
        {
            Current = First;
        }

        public void Dispose()
        {
            ;
        }

    }
}