using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace ICSStudio.FileConverter.JsonToL5X.Model
{
    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class Data
    {
        [XmlElement("AlarmAnalogParameters", typeof(AlmaType))]
        [XmlElement("AlarmConfig", typeof(AlarmConfigType))]
        [XmlElement("AlarmDigitalParameters", typeof(AlmdType))]
        [XmlElement("Array", typeof(DataArray))]
        [XmlElement("AxisParameters", typeof(AxisType))]
        [XmlElement("CoordinateSystemParameters", typeof(CoordinateSystemType))]
        [XmlElement("DataValue", typeof(DataValue))]
        [XmlElement("MessageParameters", typeof(MessageType))]
        [XmlElement("MotionGroupParameters", typeof(MotionGroupType))]
        [XmlElement("Structure", typeof(DataStructure))]
        public object[] Items { get; set; }

        [XmlText] public XmlNode[] Text { get; set; }

        [XmlAttribute] public string Format { get; set; }

        [XmlAttribute] public ulong Length { get; set; }

        [XmlIgnore] public bool LengthSpecified { get; set; }
    }
}
