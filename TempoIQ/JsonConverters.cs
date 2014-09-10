using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TempoIQ.Querying;

namespace TempoIQ.Json
{
    class SelectionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(Selection));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var selection = (Selection)value;
            writer.WriteValue(selection.Selectors);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var selectors = JsonConvert.DeserializeObject<Dictionary<Selectors.Type, Selector>>(object);
            return new Selection(selectors);
        }
    }

    class SelectorTypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Selectors.Type type = (Selectors.Type)value;
            if (type == Selectors.Type.Devices)
            {
                writer.WriteValue("sensors");
            } else
            {
                writer.WriteValue("devices");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var stringVal = ((string)reader.Value);
            if (stringVal.Equals("sensors"))
            {
                return Selectors.Type.Sensors;
            } else if (stringVal.Equals("devices"))
            {
                return Selectors.Type.Devices;
            } else 
            {
                throw new ArgumentException(String.Format("%s is not a valid selector type", stringVal));
            }
        }

        public override bool CanConvert(Type objectType)
        {
          return objectType.Equals(typeof(Selectors.Type));
        }
    }

    class AllSelectorConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            AllSelector all = (AllSelector)value;
            writer.WriteValue("all");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string[] allKeywords = {"all", "*", "any"};
            if (allKeywords.Any((string s) => s == (string)reader.Value))
            {
                return new AllSelector();
            } else 
            {
                throw new JsonException(String.Format("cannot deserialize an AllSelector from %s", (string)reader.Value));
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(Selection));
        }
    }

    class QueryConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
          throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
          return objectType.Equals(typeof(Query));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
          throw new NotImplementedException();
        }
    }
}