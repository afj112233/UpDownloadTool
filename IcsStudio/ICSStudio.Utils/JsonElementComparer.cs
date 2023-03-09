using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ICSStudio.Utils
{
    public class JsonElementComparer : IEqualityComparer<JToken>
    {
        public bool Equals(JToken x, JToken y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            if (ReferenceEquals(x, y))
                return true;

            if (x.Type != y.Type)
            {
                if ((x.Type == JTokenType.Float && y.Type == JTokenType.Integer) ||
                    (x.Type == JTokenType.Integer && y.Type == JTokenType.Float))
                {
                    double double0 = x.Value<double>();
                    double double1 = y.Value<double>();

                    return double0.Equals(double1);
                }
                else
                {
                    return false;
                }
            }

            switch (x.Type)
            {
                case JTokenType.None:
                    break;

                case JTokenType.Object:
                    return Equals(x as JObject, y as JObject);

                case JTokenType.Array:
                    return Equals(x as JArray, y as JArray);

                case JTokenType.Constructor:
                    break;
                case JTokenType.Property:
                    break;
                case JTokenType.Comment:
                    break;
                case JTokenType.Integer:
                    return JToken.DeepEquals(x, y);

                case JTokenType.Float:
                {
                    JValue xValue = x as JValue;
                    JValue yValue = y as JValue;

                    if (xValue?.Value != null && yValue?.Value != null)
                    {
                        var xType = xValue.Value.GetType();
                        var yType = yValue.Value.GetType();
                        if (xType != yType && (xType == typeof(float) || yType == typeof(float)))
                        {
                            float xFloat = Convert.ToSingle(xValue.Value);
                            float yFloat = Convert.ToSingle(yValue.Value);

                            return xFloat.Equals(yFloat);
                        }
                    }

                    return JToken.DeepEquals(x, y);
                        
                }

                case JTokenType.String:
                    return JToken.DeepEquals(x, y);

                case JTokenType.Boolean:
                    return JToken.DeepEquals(x, y);

                case JTokenType.Null:
                    return JToken.DeepEquals(x, y);

                case JTokenType.Undefined:
                    break;
                case JTokenType.Date:
                    return JToken.DeepEquals(x, y);

                case JTokenType.Raw:
                    break;
                case JTokenType.Bytes:
                    break;
                case JTokenType.Guid:
                    break;
                case JTokenType.Uri:
                    break;
                case JTokenType.TimeSpan:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new NotImplementedException();
        }

        public int GetHashCode(JToken obj)
        {
            return obj.GetHashCode();
        }

        private bool Equals(JObject x, JObject y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            if (ReferenceEquals(x, y))
                return true;

            if (x.Properties().Count() != y.Properties().Count())
                return false;

            foreach (KeyValuePair<string, JToken> keyValuePair in x)
            {
                string key = keyValuePair.Key;
                
                if (!y.ContainsKey(key))
                    return false;

                var xValue = keyValuePair.Value;
                var yValue = y.GetValue(key);

                if (!Equals(xValue, yValue))
                    return false;
            }

            return true;
        }

        private bool Equals(JArray x, JArray y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null)
                return false;

            if (ReferenceEquals(x, y))
                return true;

            if (x.Count != y.Count)
                return false;

            return x.SequenceEqual(y, this);
        }
    }
}
