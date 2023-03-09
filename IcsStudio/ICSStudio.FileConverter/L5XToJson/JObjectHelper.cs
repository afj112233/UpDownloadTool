using System;
using Newtonsoft.Json.Linq;
using System.Xml;
using ICSStudio.Utils;

namespace ICSStudio.FileConverter.L5XToJson
{
    internal static class JObjectHelper
    {
        internal static void Add(this JObject @object, XmlElement xmlNode, string name)
        {
            @object.Add(name, xmlNode.GetAttribute(name));
        }

        internal static void Add<T>(this JObject @object, XmlElement xmlNode, string name)
            where T : struct, IConvertible
        {
            if (typeof(T).IsEnum)
            {
                @object.Add(name, Convert.ToInt32(EnumUtils.Parse<T>(xmlNode.GetAttribute(name))));
                return;
            }

            if (typeof(T) == typeof(bool))
            {
                @object.Add(name, bool.Parse(xmlNode.GetAttribute(name)));
                return;
            }

            if (typeof(T) == typeof(int))
            {
                @object.Add(name, int.Parse(xmlNode.GetAttribute(name)));
                return;
            }

            if (typeof(T) == typeof(uint))
            {
                @object.Add(name, uint.Parse(xmlNode.GetAttribute(name)));
                return;
            }

            if (typeof(T) == typeof(float))
            {
                @object.Add(name, float.Parse(xmlNode.GetAttribute(name)));
                return;
            }

            throw new NotImplementedException("Add type convert!");

        }

        internal static void AddOrIgnore(this JObject @object, XmlElement xmlNode, string name)
        {
            if (xmlNode.HasAttribute(name))
            {
                @object.Add(name, xmlNode.GetAttribute(name));
            }
        }

        internal static void AddOrIgnore<T>(this JObject @object, XmlElement xmlNode, string name)
            where T : struct, IConvertible
        {
            if (xmlNode.HasAttribute(name))
            {
                @object.Add<T>(xmlNode, name);
            }
        }
    }
}
