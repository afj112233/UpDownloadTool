using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using ICSStudio.FileConverter.JsonToL5X.Model;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Tags;
using DataTypeCollection = ICSStudio.FileConverter.JsonToL5X.Model.DataTypeCollection;
using ProgramCollection = ICSStudio.FileConverter.JsonToL5X.Model.ProgramCollection;
using ProgramType = ICSStudio.FileConverter.JsonToL5X.Model.ProgramType;
using RoutineCollection = ICSStudio.FileConverter.JsonToL5X.Model.RoutineCollection;
using RoutineType = ICSStudio.FileConverter.JsonToL5X.Model.RoutineType;
using TagCollection = ICSStudio.FileConverter.JsonToL5X.Model.TagCollection;
using TagType = ICSStudio.FileConverter.JsonToL5X.Model.TagType;
using TaskCollection = ICSStudio.FileConverter.JsonToL5X.Model.TaskCollection;
using TaskType = ICSStudio.FileConverter.JsonToL5X.Model.TaskType;

namespace ICSStudio.FileConverter.JsonToL5X
{
    public static partial class Converter
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static void ExportL5X(this Controller controller, string l5xFile)
        {
            RSLogix5000ContentType rsLogix5000ContentType = ToRSLogix5000ContentType(controller);

            string content = rsLogix5000ContentType.ToXml(new XmlWriterSettings()
            {
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "  ",
                NewLineChars = Environment.NewLine
            });

            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>"
                         + Environment.NewLine + content;

            using (StreamWriter streamWriter = new StreamWriter(l5xFile))
            {
                streamWriter.Write(xml);
            }
        }

        private static RSLogix5000ContentType ToRSLogix5000ContentType(Controller controller)
        {
            RSLogix5000ContentType rsLogix5000ContentType = new RSLogix5000ContentType();

            Contract.Assert(controller != null);

            rsLogix5000ContentType.SchemaRevision = "1.0";
            rsLogix5000ContentType.SoftwareRevision = "32.01";
            rsLogix5000ContentType.TargetName = controller.Name;
            rsLogix5000ContentType.TargetType = "Controller";
            rsLogix5000ContentType.ContainsContext = BoolEnum.@false;
            rsLogix5000ContentType.ExportDate
                = DateTime.Now.ToString("ddd MMM dd hh:mm:ss yyyy", CultureInfo.GetCultureInfo("en-US"));
            rsLogix5000ContentType.ExportOptions =
                "NoRawData L5KData DecoratedData ForceProtectedEncoding AllProjDocTrans";

            rsLogix5000ContentType.Controller = ToController(controller);

            return rsLogix5000ContentType;
        }

        private static ControllerType ToController(Controller controller)
        {
            ControllerType controllerType = new ControllerType();

            controllerType.Use = UseEnum.Target;
            controllerType.Name = controller.Name;
            controllerType.ProcessorType = "1756-L84E";
            controllerType.MajorRev = 32;
            controllerType.MinorRev = 11;

            // not in L84e
            //controllerType.TimeSlice = (ushort) controller.TimeSlice;
            //controllerType.ShareUnusedTimeSlice = 1;
            //

            controllerType.ProjectCreationDate =
                controller.ProjectCreationDate.ToString("ddd MMM dd hh:mm:ss yyyy",
                    CultureInfo.GetCultureInfo("en-US"));
            controllerType.LastModifiedDate =
                controller.LastModifiedDate.ToString("ddd MMM dd hh:mm:ss yyyy",
                    CultureInfo.GetCultureInfo("en-US"));

            controllerType.SFCExecutionControl = "CurrentActive";
            controllerType.SFCRestartPosition = "MostRecent";
            controllerType.SFCLastScan = "DontScan";

            controllerType.ProjectSN = controller.ProjectSN.ToString(DisplayStyle.Hex);
            controllerType.MatchProjectToController = BoolEnum.@false;
            controllerType.CanUseRPIFromProducer = BoolEnum.@false;
            controllerType.InhibitAutomaticFirmwareUpdate = 0;
            controllerType.PassThroughConfiguration = "EnabledWithAppend";
            controllerType.DownloadProjectDocumentationAndExtendedProperties = BoolEnum.@true;
            controllerType.DownloadProjectCustomProperties = BoolEnum.@true;
            controllerType.ReportMinorOverflow = BoolEnum.@false;

            //AutoDiagsEnabled = "true"
            //WebServerEnabled = "false"

            controllerType.Description = ToDescription(controller.Description);
            controllerType.RedundancyInfo = ToRedundancyInfo(controller);
            controllerType.Security = ToSecurity(controller);
            controllerType.SafetyInfo = new SafetyInfoType();

            controllerType.DataTypes = ToDataTypes(controller);
            controllerType.Modules = ToModules(controller);
            controllerType.AddOnInstructionDefinitions = ToAddOnInstructionDefinitions(controller);
            controllerType.Tags = ToControllerTags(controller);
            controllerType.Programs = ToPrograms(controller);
            controllerType.Tasks = ToTasks(controller);

            controllerType.CST = new CSTType();
            controllerType.WallClockTime = new WallClockTimeType();
            controllerType.Trends = new Model.TrendCollection();
            controllerType.DataLogs = new object();

            controllerType.TimeSynchronize = ToTimeSynchronize(controller);

            //EthernetPorts

            return controllerType;
        }

        private static TimeSynchronizeType ToTimeSynchronize(Controller controller)
        {
            TimeSynchronizeType timeSynchronize = new TimeSynchronizeType();

            Contract.Assert(controller != null);

            if (controller.TimeSetting != null)
            {
                timeSynchronize.Priority1 = (byte)controller.TimeSetting.Priority1;
                timeSynchronize.Priority2 = (byte)controller.TimeSetting.Priority2;
                timeSynchronize.PTPEnable = controller.TimeSetting.PTPEnable ? BoolEnum.@true : BoolEnum.@false;
            }
            else
            {
                timeSynchronize.Priority1 = 128;
                timeSynchronize.Priority2 = 128;
                timeSynchronize.PTPEnable = BoolEnum.@false;
            }

            return timeSynchronize;
        }

        private static TaskCollection ToTasks(Controller controller)
        {
            TaskCollection taskCollection = new TaskCollection();

            List<TaskType> tasks = new List<TaskType>();
            foreach (var task in controller.Tasks.OfType<CTask>())
            {
                TaskType taskType = ToTaskType(task);
                tasks.Add(taskType);
            }

            if (tasks.Count > 0)
                taskCollection.Task = tasks.ToArray();

            //TODO(gjc): add code here

            return taskCollection;
        }

        private static TaskType ToTaskType(CTask task)
        {
            TaskType taskType = new TaskType();

            taskType.Name = task.Name;

            taskType.Type = ToTaskTypeEnum(task.Type);

            if (task.Type == Interfaces.Common.TaskType.Periodic)
                taskType.Rate = task.Rate;

            taskType.Priority = (ushort)task.Priority;
            taskType.Watchdog = (ulong)task.Watchdog;
            taskType.DisableUpdateOutputs = task.DisableUpdateOutputs ? BoolEnum.@true : BoolEnum.@false;
            taskType.InhibitTask = task.IsInhibited ? BoolEnum.@true : BoolEnum.@false;

            taskType.ScheduledPrograms = ToScheduledPrograms(task);

            //TODO(gjc): add code here

            return taskType;
        }

        private static ScheduledProgramType[] ToScheduledPrograms(CTask task)
        {
            List<ScheduledProgramType> scheduledProgramTypes = new List<ScheduledProgramType>();

            Controller controller = task.ParentController as Controller;
            Contract.Assert(controller != null);

            foreach (var program in controller.Programs)
            {
                if (program.ParentTask == task)
                {
                    ScheduledProgramType scheduledProgramType = new ScheduledProgramType
                    {
                        Name = program.Name
                    };

                    scheduledProgramTypes.Add(scheduledProgramType);
                }
            }

            if (scheduledProgramTypes.Count == 0)
                return null;

            return scheduledProgramTypes.ToArray();
        }

        private static TaskTypeEnum ToTaskTypeEnum(Interfaces.Common.TaskType taskType)
        {
            switch (taskType)
            {
                case Interfaces.Common.TaskType.Event:
                    return TaskTypeEnum.EVENT;
                case Interfaces.Common.TaskType.Periodic:
                    return TaskTypeEnum.PERIODIC;
                case Interfaces.Common.TaskType.Continuous:
                    return TaskTypeEnum.CONTINUOUS;
                default:
                    throw new ArgumentOutOfRangeException(nameof(taskType), taskType, null);
            }
        }

        private static ProgramCollection ToPrograms(Controller controller)
        {
            ProgramCollection programCollection = new ProgramCollection();

            List<ProgramType> programTypes = new List<ProgramType>();

            foreach (var program in controller.Programs.OfType<Program>())
            {
                ProgramType programType = ToProgramType(program);
                programTypes.Add(programType);
            }

            if (programTypes.Count > 0)
            {
                programCollection.Program = programTypes.ToArray();
            }

            return programCollection;
        }

        private static ProgramType ToProgramType(Program program)
        {
            ProgramType programType = new ProgramType();

            Contract.Assert(program != null);

            programType.Name = program.Name;
            programType.TestEdits = program.TestEditsMode == TestEditsModeType.Test ? BoolEnum.@true : BoolEnum.@false;
            programType.MainRoutineName = program.MainRoutineName;

            programType.Disabled = program.Inhibited ? BoolEnum.@true : BoolEnum.@false;

            programType.UseAsFolder = BoolEnum.@false;

            programType.Tags = ToProgramTags(program);

            programType.Routines = ToRoutines(program);

            return programType;
        }

        private static RoutineCollection ToRoutines(Program program)
        {
            RoutineCollection routineCollection = new RoutineCollection();

            List<object> routines = new List<object>();

            foreach (var routine in program.Routines)
            {
                if (routine.IsEncrypted)
                {
                    //TODO(gjc): check here
                    Debug.WriteLine($"routine:{routine.Name} is encrypted!");
                    continue;
                }

                RoutineType routineType = ToRoutineType(routine);
                routines.Add(routineType);
            }

            if (routines.Count > 0)
                routineCollection.Items = routines.ToArray();

            return routineCollection;
        }

        private static RoutineType ToRoutineType(IRoutine routine)
        {
            RoutineType routineType = new RoutineType();

            routineType.Name = routine.Name;

            routineType.Type = ToRoutineTypeEnum(routine.Type);

            if (routine.Type == Interfaces.Common.RoutineType.ST)
            {
                routineType.STContent = ToSTContent(routine as STRoutine);
            }

            if (routine.Type == Interfaces.Common.RoutineType.RLL)
            {
                routineType.RLLContent = ToRLLContent(routine as RLLRoutine);
            }

            routineType.Description = new[] { ToDescription(routine.Description) };

            return routineType;
        }

        private static RoutineTypeEnum ToRoutineTypeEnum(Interfaces.Common.RoutineType routineType)
        {
            switch (routineType)
            {
                case Interfaces.Common.RoutineType.Typeless:
                    return RoutineTypeEnum.Typeless;

                case Interfaces.Common.RoutineType.RLL:
                    return RoutineTypeEnum.RLL;

                case Interfaces.Common.RoutineType.FBD:
                    return RoutineTypeEnum.FBD;

                case Interfaces.Common.RoutineType.SFC:
                    return RoutineTypeEnum.SFC;

                case Interfaces.Common.RoutineType.ST:
                    return RoutineTypeEnum.ST;

                case Interfaces.Common.RoutineType.External:
                    return RoutineTypeEnum.External;

                case Interfaces.Common.RoutineType.Encrypted:
                    return RoutineTypeEnum.Encrypted;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static TagCollection ToProgramTags(Program program)
        {
            TagCollection tagCollection = new TagCollection();

            List<TagType> tagTypes = new List<TagType>();

            foreach (var tag in program.Tags.OfType<Tag>())
            {
                TagType tagType = ToTagType(tag);

                tagTypes.Add(tagType);
            }

            if (tagTypes.Count > 0)
                tagCollection.Tag = tagTypes.ToArray();

            return tagCollection;
        }

        private static TagCollection ToControllerTags(Controller controller)
        {
            TagCollection tagCollection = new TagCollection();

            List<TagType> tags = new List<TagType>();

            foreach (var tag in controller.Tags.OfType<Tag>())
            {
                if (tag.IsModuleTag)
                    continue;

                TagType tagType = ToTagType(tag);

                tags.Add(tagType);
            }

            if (tags.Count > 0)
                tagCollection.Tag = tags.ToArray();

            return tagCollection;
        }

        private static AOIDefinitionCollection ToAddOnInstructionDefinitions(Controller controller)
        {
            AOIDefinitionCollection aoiDefinitionCollection = new AOIDefinitionCollection();

            var aoiList = controller.AOIDefinitionCollection.OfType<AoiDefinition>().ToList();
            var sortedAoiList = SortAoiDefinitions(aoiList);

            List<object> aoiObjects = new List<object>();

            foreach (var aoiDefinition in sortedAoiList)
            {
                if (aoiDefinition.IsEncrypted)
                {
                    //TODO(gjc): check here
                    Debug.WriteLine($"AOI:{aoiDefinition.Name} is encrypted!");
                    continue;
                }

                AOIDefinitionType aoiObject = ToAOIDefinitionType(aoiDefinition);

                aoiObjects.Add(aoiObject);
            }

            if (aoiObjects.Count > 0)
                aoiDefinitionCollection.Items = aoiObjects.ToArray();

            return aoiDefinitionCollection;
        }

        private static List<AoiDefinition> SortAoiDefinitions(List<AoiDefinition> aoiList)
        {
            List<AoiDefinition> sortedList = new List<AoiDefinition>();

            Queue<AoiDefinition> unsortedQueue = new Queue<AoiDefinition>(aoiList);

            while (unsortedQueue.Count > 0)
            {
                var aoiDefinition = unsortedQueue.Dequeue();

                if (IsSelfReference(aoiDefinition.datatype))
                {
                    sortedList.Add(aoiDefinition);
                }
                else
                {
                    bool isReference = false;
                    foreach (var definition in unsortedQueue.ToArray())
                    {
                        if (IsReference(definition.datatype, aoiDefinition.datatype))
                        {
                            isReference = true;
                            break;
                        }
                    }

                    if (isReference)
                    {
                        unsortedQueue.Enqueue(aoiDefinition);
                    }
                    else
                    {
                        sortedList.Add(aoiDefinition);
                    }
                }
            }

            return sortedList;
        }

        private static bool IsSelfReference(AOIDataType datatype)
        {
            CompositiveType compositiveType = datatype as CompositiveType;
            if (compositiveType != null)
            {
                foreach (var member in compositiveType.TypeMembers)
                {
                    if (IsReference(datatype, member.DataTypeInfo.DataType))
                        return true;
                }
            }

            return false;
        }

        private static bool IsReference(IDataType parentType, IDataType memberType)
        {
            if (parentType == memberType)
                return true;

            CompositiveType compositiveType = memberType as CompositiveType;
            if (compositiveType != null)
            {
                foreach (var member in compositiveType.TypeMembers)
                {
                    if (IsReference(parentType, member.DataTypeInfo.DataType))
                        return true;
                }
            }

            return false;
        }

        private static ModuleCollection ToModules(Controller controller)
        {
            ModuleCollection moduleCollection = new ModuleCollection();

            List<ModuleType> moduleTypes = new List<ModuleType>();

            ModuleType localModuleType = CreateLocalModuleType();
            moduleTypes.Add(localModuleType);

            DeviceModuleCollection deviceModuleCollection = controller.DeviceModules as DeviceModuleCollection;
            deviceModuleCollection?.Sort();

            foreach (var deviceModule in controller.DeviceModules.OfType<DeviceModule>())
            {
                ModuleType moduleType = ToModuleType(deviceModule);
                if (moduleType != null)
                {
                    moduleTypes.Add(moduleType);
                }
            }

            moduleCollection.Module = moduleTypes.ToArray();

            return moduleCollection;
        }

        private static DataTypeCollection ToDataTypes(Controller controller)
        {
            DataTypeCollection dataTypeCollection = new DataTypeCollection();

            List<DataTypeType> dataTypeTypes = new List<DataTypeType>();
            foreach (var dataType in controller.DataTypes.OfType<UserDefinedDataType>())
            {
                DataTypeType dataTypeType = ToDataTypeType(dataType);
                dataTypeTypes.Add(dataTypeType);
            }

            if (dataTypeTypes.Count > 0)
                dataTypeCollection.DataType = dataTypeTypes.ToArray();

            return dataTypeCollection;
        }

        private static SecurityInfoType ToSecurity(Controller controller)
        {
            SecurityInfoType securityInfoType = new SecurityInfoType();

            securityInfoType.Code = 0;
            securityInfoType.ChangesToDetect = "16#ffff_ffff_ffff_ffff";

            return securityInfoType;
        }

        private static RedundancyInfoType ToRedundancyInfo(Controller controller)
        {
            RedundancyInfoType redundancyInfoType = new RedundancyInfoType();

            Contract.Assert(controller != null);

            redundancyInfoType.Enabled = BoolEnum.@false;
            redundancyInfoType.KeepTestEditsOnSwitchOver = BoolEnum.@false;

            // not in L84E
            //redundancyInfoType.IOMemoryPadPercentage = 90;
            //redundancyInfoType.DataTablePadPercentage = 50;
            //

            return redundancyInfoType;
        }

        private static DescriptionType ToDescription(string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                DescriptionType descriptionType = new DescriptionType();

                XmlDocument xmlDocument = new XmlDocument();
                descriptionType.Text = new XmlNode[] { xmlDocument.CreateCDataSection(description) };

                return descriptionType;
            }

            return null;
        }
    }
}
