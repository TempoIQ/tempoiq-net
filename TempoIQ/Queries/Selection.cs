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

        public Selection AddSelector(Select.Type type, Selector selector)
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

    [JsonConverter(typeof(AllSelectorConverter))]
    public struct AllSelector : Selector { };

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
}