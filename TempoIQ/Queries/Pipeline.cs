using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Text;
using TempoIQ.Utilities;

namespace TempoIQ.Queries
{
    /// <summary>
    /// Pipeline functions represent server-side transformations on data
    /// </summary>
    [JsonObject]
    public interface PipelineFunction
    {
        [JsonProperty("name")]
        string Name { get; }

        IList<string> Arguments { get; }
    }

    /// <summary>
    /// A Pipeline represents a series of transformations on a stream of sensor data
    /// </summary>
    [JsonObject]
    public class Pipeline
    {
        [JsonProperty("functions")]
        public IList<PipelineFunction> Functions { get; private set; }

        public Pipeline AddFunction(PipelineFunction function)
        {
            this.Functions.Add(function);
            return this;
        }

        public Pipeline(IList<PipelineFunction> functions)
        {
            this.Functions = functions;
        }

        public Pipeline()
        {
            this.Functions = new List<PipelineFunction>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="period"></param>
        /// <param name="fold"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public Pipeline Rollup(Period period, Fold fold, ZonedDateTime start)
        {
            this.AddFunction(new Rollup(period, fold, start));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fold"></param>
        /// <returns></returns>
        public Pipeline Aggregate(Fold fold)
        {
            this.AddFunction(new Aggregation(fold));
            return this;
        }
    }

    /// <summary>
    /// Rollups represent a function application over a data stream broken into chunks of a provided period's duration
    /// </summary>
    public class Rollup : PipelineFunction
    {
        /// <summary>
        /// The 'chunk-size' of the Rollup
        /// </summary>
        [JsonProperty("period")]
        public Period Period { get; set; }

        /// <summary>
        /// The folding function for the Rollup
        /// </summary>
        public Fold Fold { get; set; }

        [JsonProperty("start")]
        public ZonedDateTime Start { get; set; }

        [JsonProperty("name")]
        public string Name { get { return "rollup"; } }

        [JsonProperty("arguments")]
        public IList<string> Arguments
        {
            get
            {
                var startString = NodaTime.Text.InstantPattern.ExtendedIsoPattern.Format(Start.ToInstant());
                return new List<string> { Fold.ToString().ToLower(), Period.ToString(), startString };
            }
        }

        [JsonConstructor]
        public Rollup(Period period, Fold fold, ZonedDateTime start)
        {
            this.Period = period;
            this.Fold = fold;
            this.Start = start;
        }

        public Rollup()
        {
            this.Period = Period.FromMinutes(1);
            this.Fold = Fold.Sum;
            var now = SystemClock.Instance.Now;
            var tz = DateTimeZone.Utc;
            this.Start = tz.AtStrictly(LocalDateTime.FromDateTime(DateTime.Now));
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            else if (obj is Rollup)
                return ((Rollup)obj).Equals(this);
            else
                return false;
        }

        public bool Equals(Rollup rollup)
        {
            return EqualsBuilder.Equals(this.Period, rollup.Period)
                && EqualsBuilder.Equals(this.Fold, rollup.Fold)
                && EqualsBuilder.Equals(this.Start, rollup.Start);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, this.Period);
            hash = HashCodeHelper.Hash(hash, this.Fold);
            hash = HashCodeHelper.Hash(hash, this.Start);
            return hash;
        }
    }

    /// <summary>
    /// The representation of an aggregation between multiple <c>Sensor</c>s
    /// </summary>
    /// <para>
    ///  An Aggregation allows you to specify a new Sensor that is a mathematical
    ///  operation across multiple Sensor. For instance, the following Aggregation
    ///  specifies the sum of multiple Sensor:
    [JsonObject]
    public class Aggregation : PipelineFunction
    {
        /// <summary>
        /// The Aggregation's underlying Fold function
        /// </summary>
        [JsonProperty("fold")]
        public Fold Fold { get; private set; }

        [JsonProperty("name")]
        public string Name { get { return "aggregation"; } }

        [JsonProperty("arguments")]
        public IList<string> Arguments
        {
            get
            {
                return new List<string> { Fold.ToString().ToLower() };
            }
        }

        public Aggregation(Fold fold)
        {
            this.Fold = fold;
        }

        /// <para>
        /// Aggregation aggregation = new Aggregation(Fold.SUM);
        /// </para>
        public Aggregation()
        {
            this.Fold = Fold.Sum;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            else if (obj is Aggregation)
                return ((Aggregation)obj).Equals(this);
            else
                return false;
        }

        public bool Equals(Aggregation obj)
        {
            return EqualsBuilder.Equals(this.Fold, obj.Fold);
        }
    }
}