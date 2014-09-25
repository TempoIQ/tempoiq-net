using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TempoIQ.Models;
using TempoIQ.Json;

namespace TempoIQ.Querying
{
    public interface Selector { };

    public static class Selectors
    {
        [JsonConverter(typeof(SelectorTypeConverter))]
        public enum Type
        {
            Sensors,
            Devices
        }

        public static AllSelector All()
        {
            return new AllSelector();
        }

        public static AndSelector And(params Selector[] children)
        {
            return new AndSelector(children);
        }

        public static AttributesSelector Attribute(string key, string value)
        {
            return new AttributesSelector(key, value);
        }

        public static AttributesSelector Attributes(IDictionary<string, string> attrs)
        {
            return new AttributesSelector(attrs);
        }

        public static AttributeKeySelector AttributeKey(string key)
        {
            return new AttributeKeySelector(key);
        }

        public static KeySelector Key(string key)
        {
            return new KeySelector(key);
        }

        public static OrSelector Or(params Selector[] children)
        {
            return new OrSelector(children);
        }
    }

    [JsonConverter(typeof(SelectionConverter))]
    public class Selection
    {
        public IDictionary<Selectors.Type, Selector> Selectors { get; private set; }

        public Selection AddSelector(Selectors.Type type, Selector selector)
        {
            this.Selectors[type] = selector;
            return this;
        }

        public Selection()
        {
            this.Selectors = new SortedDictionary<Selectors.Type, Selector>();
        }

        public Selection(Selectors.Type type, Selector selector)
        {
            var dict = new Dictionary<Selectors.Type, Selector>();
            dict.Add(type, selector);
            this.Selectors = dict;
        }

        [JsonConstructor]
        public Selection(IDictionary<Selectors.Type, Selector> selectors)
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