using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace ICSStudio.FileConverter.JsonToL5X.Model
{
    [GeneratedCode("xsd", "4.8.3928.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class RungType
    {
        [XmlAttribute] public ulong Number { get; set; }

        [XmlAttribute] public RungTypeEnum Type { get; set; }

        public XmlNode Text { get; set; }

        public RungCommentType Comment { get; set; }

    }

    [GeneratedCode("xsd", "4.8.3928.0")]
    [Serializable]
    public enum RungTypeEnum
    {
        N,
        I,
        D,
        IR,
        rR,
        R,
        rI,
        rN,
        e,
        er,
        dN,
        dD,
        dI,
        dIR,
    }
}
