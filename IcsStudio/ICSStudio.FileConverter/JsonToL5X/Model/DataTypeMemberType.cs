using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ICSStudio.FileConverter.JsonToL5X.Model
{
    [GeneratedCode("xsd", "4.8.3928.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class DataTypeMemberType
    {
        public DescriptionType Description { get; set; }

        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public string DataType { get; set; }

        [XmlAttribute] public ushort Dimension { get; set; }

        [XmlAttribute] public RadixEnum Radix { get; set; }

        [XmlAttribute] public BoolEnum Hidden { get; set; }

        [XmlAttribute] public string Target { get; set; }

        [XmlAttribute] public ushort BitNumber { get; set; }
        
        [XmlAttribute] public ExternalAccessEnum ExternalAccess { get; set; }
        
        public bool ShouldSerializeBitNumber()
        {
            if (string.IsNullOrEmpty(Target))
                return false;

            return true;
        }
    }
}
