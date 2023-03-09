using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSStudio.Descriptor
{
    class DictionaryJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //ignore
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);

            Dictionary<ushort, string> result = new Dictionary<ushort, string>();

            foreach (JProperty property in jObject.Properties())
            {
                ushort key = Convert.ToUInt16(property.Name, 16);
                result.Add(key, property.Value.ToString());
            }

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Dictionary<ushort, string>) == objectType;
        }
    }
}
