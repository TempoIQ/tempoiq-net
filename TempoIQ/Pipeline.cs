using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NodaTime;

namespace TempoIQ
{
    public interface PipelineFunction
    {
        string Name { get; }

        IList<string> Arguments { get; }
    }

    public class Pipeline
    {
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

        public Pipeline Rollup(Period period, Fold fold, ZonedDateTime start)
        {
            this.AddFunction(new Rollup(period, fold, start));
            return this;
        }

        public Pipeline Aggregate(Fold fold)
        {
            this.AddFunction(new Aggregation(fold));
            return this;
        }
    }

    public class Rollup : PipelineFunction
    { 
        [JsonProperty("period")]
        public Period Period { get; set; } 

        [JsonIgnore]
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
                return new List<string>(new string[] { Fold.ToString().ToLower(), Period.ToString(), Start.ToString() });
            }
        }

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
        /// The <c>Aggregation</c>'s underlying <c>Fold</c> function
        /// </summary>
        public Fold Fold { get; private set; }

        [JsonProperty("name")]
        public string Name { get { return "aggregation"; } }

        [JsonProperty("arguments")]
        public IList<string> Arguments { get { return new List<string>(new string[] { Fold.ToString().ToLower() }); } }

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