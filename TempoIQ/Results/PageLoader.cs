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
        private string EndPoint { get; set; }
        private string ContentType { get; set; }
        private string[] MediaTypeVersion { get; set; }

        public PageLoader(Executor runner, Segment<T> first, string endPoint, string contentType, params string[] mediaTypeVersion)
        {
            this.Runner = runner;
            this.Current = first;
            this.First = first;
            this.EndPoint = endPoint;
            this.ContentType = contentType;
            this.MediaTypeVersion = mediaTypeVersion;
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            if (Current == null || Current.Next == null || Current.Next.Query == null)
                return false;
            var result = Runner.Execute<Segment<T>>(RestSharp.Method.POST, EndPoint, Current.Next.Query, ContentType, MediaTypeVersion);
            if (result.State.Equals(State.Success))
            {
                Current = result.Value;
                return true;
            }
            return false;
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