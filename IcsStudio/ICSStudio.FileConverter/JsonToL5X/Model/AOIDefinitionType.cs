using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace ICSStudio.FileConverter.JsonToL5X.Model
{
    [GeneratedCode("xsd", "4.6.81.0")]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [Serializable]
    public class AOIDefinitionType
    {
        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public string Revision { get; set; }

        [XmlAttribute] public BoolEnum ExecutePrescan { get; set; }

        [XmlAttribute] public BoolEnum ExecutePostscan { get; set; }

        [XmlAttribute] public BoolEnum ExecuteEnableInFalse { get; set; }

        [XmlAttribute] public string CreatedDate { get; set; }

        [XmlAttribute] public string CreatedBy { get; set; }

        [XmlAttribute] public string EditedDate { get; set; }

        [XmlAttribute] public string EditedBy { get; set; }

        [XmlAttribute] public string SoftwareRevision { get; set; } //"32.01"

        [XmlArrayItem("Parameter", IsNullable = false)]
        public AOIParameterType[] Parameters { get; set; }

        [XmlArrayItem("LocalTag", IsNullable = false)]
        public AOILocalTagType[] LocalTags { get; set; }

        public RoutineCollection Routines { get; set; }
    }
}