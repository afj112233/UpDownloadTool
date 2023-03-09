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
    public class TaskType
    {
        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public TaskTypeEnum Type { get; set; }

        [XmlAttribute] public float Rate { get; set; }

        [XmlAttribute] public ushort Priority { get; set; }

        [XmlAttribute] public ulong Watchdog { get; set; }

        [XmlAttribute] public BoolEnum DisableUpdateOutputs { get; set; }

        [XmlAttribute] public BoolEnum InhibitTask { get; set; }

        [XmlArrayItem("ScheduledProgram", IsNullable = false)]
        public ScheduledProgramType[] ScheduledPrograms { get; set; }


        public bool ShouldSerializeRate()
        {
            if (Type == TaskTypeEnum.PERIODIC)
                return true;

            return false;
        }
    }
}
