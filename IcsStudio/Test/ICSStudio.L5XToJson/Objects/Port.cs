using System.Xml;

namespace ICSStudio.L5XToJson.Objects
{
    public class Port
    {
        public Port(XmlElement element)
        {
            Address = element.Attributes["Address"].Value;
            Id = element.Attributes["Id"].Value;
            Type = element.Attributes["Type"].Value;
            Upstream = element.Attributes["Upstream"].Value;
        }

        public string Address { get; set; }
        public string Id { get; set; }
        public string Type { get; set; }
        public string Upstream { get; set; }
    }
}
