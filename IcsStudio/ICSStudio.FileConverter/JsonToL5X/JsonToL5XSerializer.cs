using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ICSStudio.FileConverter.JsonToL5X
{
    public class JsonToL5XSerializer
    {
        public void Serialize(string path, object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            string content = string.Empty;
            //serialize
            using (StringWriter writer = new StringWriter())
            {
                content = obj.ToXml();
            }

            //save to file
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(content);
            }
        }
    }

    public static class L5XToXml
    {
        public static string ToXml(
            this object serializableObject,
            bool includeXmlDeclaration = true,
            bool pretty = true)
        {
            string str = string.Empty;
            if (includeXmlDeclaration)
                str = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" + (pretty ? Environment.NewLine : string.Empty);
            var serialize = serializableObject.ToXml(new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8,
                Indent = pretty,
                IndentChars = "  ",
                NewLineChars = pretty ? Environment.NewLine : string.Empty,
            });

            return str + serialize;
        }

        internal static string ToXml(
            this object serializableObject,
            XmlWriterSettings xmlWriterSettings)
        {
            if (serializableObject == null)
                throw new ArgumentNullException(nameof(serializableObject));
            if (xmlWriterSettings == null)
                throw new ArgumentNullException(nameof(xmlWriterSettings));
            StringBuilder output = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(serializableObject.GetType());
            using (XmlWriter xmlWriter = XmlWriter.Create(output, xmlWriterSettings))
            {
                // 强制指定命名空间，覆盖默认的命名空间
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                xmlSerializer.Serialize(xmlWriter, serializableObject, namespaces);
                xmlWriter.Close();
            }

            ;

            return output.ToString();
        }
    }
}
