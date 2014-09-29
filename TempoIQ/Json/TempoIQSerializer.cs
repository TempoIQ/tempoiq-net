using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TempoIQ.Json;
using RestSharp;
using RestSharp.Serializers;

namespace TempoIQ.Json
{
    /// <summary>
    /// Compatibility between <code>Newtonsoft.Json</code> and <code>RestSharp</code>
    /// </summary>
    public class TempoIQSerializer : ISerializer
    {

        /// <summary>
        /// The <code>JsonConverter</code>s for the TempoIQ API
        /// </summary>
        public static JsonConverter[] Converters
        {
            get 
            {
                return new JsonConverter[]
                {
                    new AllSelectorConverter(),
                    new SelectionConverter(),
                    new SelectorConverter(),
                    new SelectorTypeConverter(),
                    new ZonedDateTimeConverter()
                };
            }
        }

        /// <summary>
        /// Serialize the object to a <code>string</code> for <code>RestSharp</code>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize(object obj)
        {
            this.ContentType = "application/json";
            return JsonConvert.SerializeObject(obj, Converters);//(obj);
        }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string RootElement { get; set; }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Content type for serialized content
        /// </summary>
        public string ContentType { get; set; }
    }
}