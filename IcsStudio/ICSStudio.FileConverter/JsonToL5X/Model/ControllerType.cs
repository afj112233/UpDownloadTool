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
    public class ControllerType
    {

        [XmlAttribute] public UseEnum Use { get; set; }

        [XmlAttribute] public string Name { get; set; }

        [XmlAttribute] public string ProcessorType { get; set; }

        [XmlAttribute] public ulong MajorRev { get; set; }

        [XmlAttribute] public ushort MinorRev { get; set; }

        [XmlAttribute] public ushort TimeSlice { get; set; }

        [XmlIgnore] public bool TimeSliceSpecified { get; set; }

        [XmlAttribute] public ushort ShareUnusedTimeSlice { get; set; }

        [XmlIgnore] public bool ShareUnusedTimeSliceSpecified { get; set; }

        [XmlAttribute] public string ProjectCreationDate { get; set; }

        [XmlAttribute] public string LastModifiedDate { get; set; }

        [XmlAttribute] public string SFCExecutionControl { get; set; }

        [XmlAttribute] public string SFCRestartPosition { get; set; }

        [XmlAttribute] public string SFCLastScan { get; set; }

        [XmlAttribute] public string ProjectSN { get; set; }

        [XmlAttribute] public BoolEnum MatchProjectToController { get; set; }

        [XmlAttribute] public BoolEnum CanUseRPIFromProducer { get; set; }

        [XmlAttribute] public byte InhibitAutomaticFirmwareUpdate { get; set; }

        [XmlAttribute] public string PassThroughConfiguration { get; set; }

        [XmlAttribute] public BoolEnum DownloadProjectDocumentationAndExtendedProperties { get; set; }

        [XmlAttribute] public BoolEnum DownloadProjectCustomProperties { get; set; }

        [XmlAttribute] public BoolEnum ReportMinorOverflow { get; set; }

        public DescriptionType Description { get; set; }

        public RedundancyInfoType RedundancyInfo { get; set; }

        public SecurityInfoType Security { get; set; }

        public SafetyInfoType SafetyInfo { get; set; }

        public DataTypeCollection DataTypes { get; set; }

        public ModuleCollection Modules { get; set; }

        public AOIDefinitionCollection AddOnInstructionDefinitions { get; set; }

        public TagCollection Tags { get; set; }

        public ProgramCollection Programs { get; set; }

        public TaskCollection Tasks { get; set; }

        public CSTType CST { get; set; }

        public WallClockTimeType WallClockTime { get; set; }

        public TrendCollection Trends { get; set; }

        public object DataLogs { get; set; }

        public TimeSynchronizeType TimeSynchronize { get; set; }

    }
}

