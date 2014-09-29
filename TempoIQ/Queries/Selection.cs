using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TempoIQ.Models;
using TempoIQ.Json;

namespace TempoIQ.Queries
{
    /// <summary>
    /// A selector works as a search term or a filter upon the universe of devices/sensors.
    /// It scopes a query so that it only yields the relevant results.
    /// </summary>
    public interface Selector { };

    /// <summary>
    /// Represents the combination of selector-type (sensors or devices) 
    /// and a selection limiting your query to the correct sensors or devices
    /// </summary>
    [JsonConverter(typeof(SelectionConverter))]
    public class Selection
    {
        public IDictionary<Select.Type, Selector> Selectors { get; private set; }

        /// <summary>
        /// Add an additional <code>Selector</code> to a <code>Selection</code>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public Selection Add(Select.Type type, Selector selector)
        {
            this.Selectors[type] = selector;
            return this;
        }

        public Selection()
        {
            this.Selectors = new SortedDictionary<Select.Type, Selector>();
        }

        public Selection(Select.Type type, Selector selector)
        {
            var dict = new Dictionary<Select.Type, Selector>();
            dict.Add(type, selector);
            this.Selectors = dict;
        }

        [JsonConstructor]
        public Selection(IDictionary<Select.Type, Selector> selectors)
        {
            this.Selectors = selectors;
        }
    }

    /// <summary>
    /// An <code>AllSelector</code> selects all objects within its scope (i.e. all <code>Devices</code>, for instance)
    /// </summary>
    [JsonConverter(typeof(AllSelectorConverter))]
    public struct AllSelector : Selector { };

    /// <summary>
    /// Combines multiple selectors, and yields the intersect of all of their evaluations
    /// </summary>
    /// <param name="children"></param>
    [JsonObject]
    public class AndSelector : Selector
    {
        [JsonProperty("and")]
        public IList<Selector> Selectors { get; set; }

        [JsonConstructor]
        public AndSelector(IList<Selector> Selectors)
        {
            this.Selectors = Selectors;
        }
    }

    /// <summary>
    /// Selects <code>Device</code>s or <code>Sensor</code>s with the
    /// any value for the given attribute key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    [JsonObject]
    public class AttributeKeySelector : Selector
    {
        [JsonProperty("attribute_key")]
        public string key;

        [JsonConstructor]
        public AttributeKeySelector(string k)
        {
            key = k;
        }
    }

    /// <summary>
    /// Selects <code>Device</code>s or <code>Sensor</code>s with the
    /// appropriate attribute key/value pairs
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    [JsonObject]
    public class AttributesSelector : Selector
    {
        [JsonProperty("attributes")]
        public IDictionary<string, string> Attributes { get; set; }

        [JsonConstructor]
        public AttributesSelector(IDictionary<string, string> attrs)
        {
            this.Attributes = attrs;
        }

        public AttributesSelector(string key, string value)
        {
            var pair = new Dictionary<string, string>(1);
            pair.Add(key, value);
            this.Attributes = pair;
        }
    }

    /// <summary>
    /// Selects <code>Device</code>s or <code>Sensor</code>s with the
    /// correct unique key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    [JsonObject]
    public class KeySelector : Selector
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        
        [JsonConstructor]
        public KeySelector(string key)
        {
            this.Key = key;
        }
    }

    /// <summary>
    /// Combines several other selectors, and yields the union of all of their evaluations
    /// </summary>
    /// <param name="children"></param>
    [JsonObject]
    public class OrSelector : Selector
    {
        [JsonProperty("or")]
        public IList<Selector> Selectors { get; set; }

        [JsonConstructor]
        public OrSelector(IList<Selector> Selectors)
        {
            this.Selectors = Selectors;
        }
    }

}