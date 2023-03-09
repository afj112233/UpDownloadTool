using System.Diagnostics;
using System.Xml;

namespace ICSStudio.FileConverter.L5XToJson.Objects
{
    public class DeviceModule
    {
        public string CatalogNumber { get; set; }
        public bool Inhibited { get; set; }
        public int Major { get; set; }
        public bool MajorFault { get; set; }
        public int Minor { get; set; }
        public string Name { get; set; }
        public int ParentModPortId { get; set; }
        public string ParentModule { get; set; }
        public int ProductCode { get; set; }
        public int ProductType { get; set; }
        public int Vendor { get; set; }
        public string EKey { get; set; }

        public DeviceModule(XmlElement xmlNode)
        {
            Name = xmlNode.HasAttribute("Name") ? xmlNode.Attributes["Name"].Value : string.Empty;

            CatalogNumber = xmlNode.Attributes["CatalogNumber"].Value;
            Inhibited = bool.Parse(xmlNode.Attributes["Inhibited"].Value);
            Major = int.Parse(xmlNode.Attributes["Major"].Value);
            MajorFault = bool.Parse(xmlNode.Attributes["MajorFault"].Value);
            Minor = int.Parse(xmlNode.Attributes["Minor"].Value);
            ParentModPortId = int.Parse(xmlNode.Attributes["ParentModPortId"].Value);
            ParentModule = xmlNode.Attributes["ParentModule"].Value;
            ProductCode = int.Parse(xmlNode.Attributes["ProductCode"].Value);
            ProductType = int.Parse(xmlNode.Attributes["ProductType"].Value);
            Vendor = int.Parse(xmlNode.Attributes["Vendor"].Value);

            XmlElement eKeyNode = (XmlElement)xmlNode.SelectSingleNode("EKey");
            Debug.Assert(eKeyNode != null);
            if (eKeyNode != null)
            {
                EKey = eKeyNode.Attributes["State"].Value;
            }
        }
    }
}
