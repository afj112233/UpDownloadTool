using System;
using Newtonsoft.Json.Linq;

namespace ICSStudio.Utils
{
    public class JObjectExtensions
    {
        private readonly JObject _jObject;

        public override string ToString()
        {
            return _jObject.ToString();
        }
        public JObjectExtensions(JObject jObject)
        {
            _jObject = jObject;
        }

        public JToken this[string propertyName]
        {
            get
            {
                if (_jObject == null)
                    return null;

                foreach (var property in _jObject.Properties())
                {
                    if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                        return property.Value;
                }

                return null;
            }
        }
    }
}
