using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace ICSStudio.FileConverter.JsonToL5X.Model
{
    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum BoolEnum
    {
        @false,
        @true,
        No,
        Yes,
        [XmlEnum("0")] Item0,
        [XmlEnum("1")] Item1,
    }
}
