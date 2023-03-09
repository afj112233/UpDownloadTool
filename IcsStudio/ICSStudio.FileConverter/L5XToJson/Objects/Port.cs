using Newtonsoft.Json;
using System.Xml;

namespace ICSStudio.FileConverter.L5XToJson.Objects
{
    public class Bus
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Size { get; set; } = -1;

        public Bus(XmlElement element)
        {
            Size = element.HasAttribute("Size") ? int.Parse(element.Attributes["Size"].Value) : -1;
        }
    }

    public class Port
    {
        public Port(XmlElement element)
        {
            Address = element.Attributes["Address"]?.Value;
            Id = int.Parse(element.Attributes["Id"].Value);
            Type = element.Attributes["Type"].Value;
            Upstream = bool.Parse((element.Attributes["Upstream"].Value));
            XmlElement bus = (XmlElement) element.SelectSingleNode("Bus");
            if (bus != null)
            {
                Bus = new Bus(bus);
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Address { get; set; }

        public int Id { get; set; }
        public string Type { get; set; }
        public bool Upstream { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Bus Bus { get; set; } = null;
    }
}
