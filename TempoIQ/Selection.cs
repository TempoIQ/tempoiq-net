using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TempoIQ.Models;
using TempoIQ.Json;

namespace TempoIQ.Querying
{
    public class Selection
    {
        public IDictionary<Selectors.Type, Selector> Selectors { get; private set; }

        public Selection AddSelector(Selectors.Type type, Selector selector)
        {
            this.Selectors.Add(type, selector);
            Selector previous;
            if (this.Selectors.TryGetValue(type, out previous))
            {
                Selector current;
                if (previous is AndSelector)
                {
                    ((AndSelector)previous).Selectors.Add(selector);
                    current = previous;
                } else
                {
                    current = new AndSelector(new List<Selector> {previous, selector});
                }
                this.Selectors.Add(type, current);
            }
            return this;
        }

        public Selection()
        {
            this.Selectors = new SortedDictionary<Selectors.Type, Selector>();
        }

        public Selection(IDictionary<Selectors.Type, Selector> selectors)
        {
            this.Selectors = selectors;
        }
    }

    public static class Selectors
    {
        [JsonConverter(typeof(SelectorTypeConverter))]
        public enum Type
        {
            Sensors,
            Devices
        }

        [JsonConverter(typeof(AllSelectorConverter))]
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

        public static OrSelector Or(params Selector[] children)
        {
            return new OrSelector(children);
        }
    }

    public interface Selector { };

    [JsonConverter(typeof(AllSelectorConverter))]
    public class AllSelector : Selectors { };

    [JsonObject]
    public class AndSelector : Selector
    {
        [JsonProperty("and")]
        public IList<Selector> Selectors { get; set; }

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

        public OrSelector(IList<Selector> Selectors)
        {
            this.Selectors = Selectors;
        }
    }

    [JsonObject]
    public class AttributeKeySelector : Selector
    {
        [JsonProperty("key")]
        public string key;

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
}