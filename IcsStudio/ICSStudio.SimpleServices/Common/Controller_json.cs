using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ICSStudio.Cip;
using ICSStudio.DeviceProfiles;
using ICSStudio.DeviceProfiles.DIOEnetAdapter;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.DeviceProfiles.Generic;
using ICSStudio.DeviceProfiles.MotionDrive2;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Chart;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orientation = ICSStudio.Interfaces.Common.Orientation;
using TextBox = ICSStudio.SimpleServices.Chart.TextBox;
using Type = System.Type;

namespace ICSStudio.SimpleServices.Common
{
    public partial class Controller
    {
        //private static JObject OriginalConfig { get;  set; }
        public static Controller Open(string projectFile, bool needAddDataType = true)
        {
            Controller controller = GetInstance();
            controller.IsLoading = true;
            controller.Clear(needAddDataType);

            controller.ProjectLocaleName = projectFile;
            using (StreamReader file = File.OpenText(projectFile))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                var config = JToken.ReadFrom(reader);

                JObjectExtensions json = new JObjectExtensions((JObject) config);

                controller.Name = (string) json["Name"];

                if (json["Description"] != null)
                {
                    controller.Description = json["Description"].ToString();
                }

                controller.ProjectCommunicationPath =
                    json["CommPath"] != null ? json["CommPath"].ToString() : string.Empty;

                if (json["ProjectCreationDate"] != null)
                {
                    controller.ProjectCreationDate =
                        FormatOp.ParseL5XControllerDate((string) json["ProjectCreationDate"]) ?? DateTime.Now;
                }

                if (json["LastModifiedDate"] != null)
                {
                    controller.LastModifiedDate = FormatOp.ParseL5XControllerDate((string) json["LastModifiedDate"]) ??
                                                  DateTime.Now;
                }

                if (json["MajorFaultProgram"] != null)
                {
                    controller.MajorFaultProgram = (string) json["MajorFaultProgram"];
                }

                if (json["PowerLossProgram"] != null)
                {
                    controller.PowerLossProgram = (string) json["PowerLossProgram"];
                }

                if (json["ProjectSN"] != null)
                {
                    string serialNumber = (string) json["ProjectSN"];
                    if (!string.IsNullOrEmpty(serialNumber))
                    {
                        if (serialNumber.StartsWith("16#"))
                        {
                            string input = serialNumber.Substring(3, serialNumber.Length - 3);
                            input = input.Replace("_", "");

                            controller.ProjectSN = Convert.ToInt32(input, 16);
                        }
                    }
                    else
                    {
                        controller.ProjectSN = 0;
                    }

                }

                if (json["MatchProjectToController"] != null)
                {
                    controller.MatchProjectToController = (bool) json["MatchProjectToController"];
                }

                if (json["EtherNetIPMode"] != null)
                {
                    controller.EtherNetIPMode = (string) json["EtherNetIPMode"];
                }

                if (json["TimeSlice"] != null)
                {
                    controller.TimeSlice = (int) json["TimeSlice"];
                }

                if (needAddDataType)
                {
                    if (json["DataTypes"] != null)
                    {
                        foreach (var dataType in json["DataTypes"])
                        {
                            controller.AddDataType(dataType);
                        }
                    }

                    foreach (var aoi in json["AddOnInstructionDefinitions"])
                    {
                        controller.AddAOIDefinition(aoi as JObject);
                    }

                    controller.FinalizeTypeCreation();
                }

                if (json["Modules"] == null || !json["Modules"].Any())
                    throw new ArgumentException("Device module not found");

                try
                {
                    foreach (var module in json["Modules"])
                    {
                        controller.AddDeviceModule(module);
                    }
                    foreach (var program in json["Programs"])
                    {
                        controller.AddProgram(program);
                    }

                    foreach (var tag in json["Tags"])
                    {
                        controller.AddTag(tag);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                if (json["Tasks"] != null)
                {
                    foreach (var task in json["Tasks"])
                    {
                        controller.AddTask(task);
                    }
                }

                if (json["ParameterConnections"] != null)
                {
                    foreach (var connection in json["ParameterConnections"])
                    {
                        controller.AddParameterConnection(connection);
                    }
                }

                if (json["Trends"] != null)
                {
                    foreach (var trend in json["Trends"])
                    {
                        controller.AddTrend(trend);
                    }
                }

                if ((json["TimeSynchronize"] as JObject) != null)
                {
                    controller.AddTimeSetting(json["TimeSynchronize"] as JObject);
                }

                // change logs
                if (json["ChangeLogs"] != null)
                {
                    var transactionManager = controller.Lookup(typeof(TransactionManager)) as TransactionManager;
                    transactionManager?.AddTransactions(json["ChangeLogs"] as JArray);
                }

            }

            controller.PostLoadJson();
            return controller;
        }

        public void Save(string projectFile, bool needNativeCode = true)
        {
            using (var sw = File.CreateText(projectFile))
            using (var jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;

                var jObject = ConvertToJObject(true, needNativeCode);
                
                jObject.WriteTo(jw);
            }
        }

        private void ExportBackup()
        {
            if (string.IsNullOrEmpty(ProjectLocaleName)) return;
            using (var sw = File.CreateText($"{ProjectLocaleName}.Recovery"))
            using (var jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;
                ConvertToJObject(true, false).WriteTo(jw);
            }
        }

        public int Export(string exportFile)
        {
            var isExist = File.Exists(exportFile);
            if (exportFile == ProjectLocaleName)
                isExist = false;

            try
            {
                using (var sw = File.CreateText(exportFile))
                using (var jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Formatting.Indented;
                    ConvertToJObject(true, false).WriteTo(jw);
                }

                if (isExist)
                {
                    var recoveryPath = $"{exportFile}.Recovery";
                    if (File.Exists(recoveryPath))
                    {
                        File.Delete(recoveryPath);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Export {exportFile} failed: {e}");
                return -1;
            }

            return 0;

        }

        internal string ExportToString()
        {
            return ConvertToJObject(true, false).ToString();
        }

        public void FinalizeTypeCreation()
        {
            foreach (var tp in _dataTypeCollection)
            {
                try
                {
                    var type = tp as AssetDefinedDataType;
                    type?.PostInit(_dataTypeCollection);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    throw;
                }
            }

            //foreach (var aoi in AOIDefinitionCollection)
            //{
            //    ((AoiDefinition)aoi).ParserTags();
            //}

            //foreach (var def in _aoiDefinitionCollection)
            //{
            //    def.PostInit(_dataTypeCollection);
            //}
        }

        public void PostLoadJson()
        {
            // ParentModule
            foreach (var d in DeviceModules)
            {
                DeviceModule.DeviceModule deviceModule = d as DeviceModule.DeviceModule;
                if (deviceModule != null
                    && deviceModule.ParentModule == null
                    && !string.IsNullOrEmpty(deviceModule.ParentModuleName))
                {
                    deviceModule.ParentModule = DeviceModules[deviceModule.ParentModuleName];
                }
            }

            // Rebuild Device Tag
            foreach (var d in DeviceModules)
            {
                DeviceModule.DeviceModule deviceModule = d as DeviceModule.DeviceModule;
                deviceModule?.PostLoadJson();
            }

            foreach (var t in Tags)
            {
                Tag tag = t as Tag;
                AxisCIPDrive axisCIPDrive = tag?.DataWrapper as AxisCIPDrive;
                axisCIPDrive?.PostLoadJson();
                axisCIPDrive?.CIPAxisToTagMembers(t);

                AxisVirtual axisVirtual = tag?.DataWrapper as AxisVirtual;
                axisVirtual?.PostLoadJson();
            }

            // verify alias tag 
            VerifyAliasTags();

            SourceProtectionManager.Update();
            SourceProtectionManager.Decode();

            Loaded?.Invoke(this, EventArgs.Empty);
        }

        public void PostImportJson(List<IDeviceModule> deviceModules, List<ITag> tags, bool ignoreAxisPass)
        {
            // ParentModule
            foreach (var d in deviceModules)
            {
                DeviceModule.DeviceModule deviceModule = d as DeviceModule.DeviceModule;
                if (deviceModule != null
                    && deviceModule.ParentModule == null
                    && !string.IsNullOrEmpty(deviceModule.ParentModuleName))
                {
                    deviceModule.ParentModule = DeviceModules[deviceModule.ParentModuleName];
                }
            }

            // Rebuild Device Tag
            foreach (var d in deviceModules)
            {
                DeviceModule.DeviceModule deviceModule = d as DeviceModule.DeviceModule;
                deviceModule?.PostLoadJson();
            }

            foreach (var t in tags)
            {
                Tag tag = t as Tag;
                AxisCIPDrive axisCIPDrive = tag?.DataWrapper as AxisCIPDrive;
                if (axisCIPDrive != null)
                {
                    if (!axisCIPDrive.Pass(t, ignoreAxisPass))
                    {
                        ((TagCollection) t.ParentCollection).DeleteTag(t, true, true, true);
                        return;
                    }
                }

                axisCIPDrive?.PostLoadJson();
                axisCIPDrive?.CIPAxisToTagMembers(t);

                AxisVirtual axisVirtual = tag?.DataWrapper as AxisVirtual;
                axisVirtual?.PostLoadJson();
            }

            // verify alias tag 
            VerifyAliasTags();

            SourceProtectionManager.Update();
            SourceProtectionManager.Decode();

            Loaded?.Invoke(this, EventArgs.Empty);
        }

        public List<Tuple<Type, string, string>> Changed { get; } = new List<Tuple<Type, string, string>>();

        public string GetFinalName(Type type, string name)
        {
            foreach (var tuple in Changed)
            {
                if (tuple.Item1 == type && tuple.Item2.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return tuple.Item3;
            }

            return name;
        }

        public void AddDataType(JToken jsonDataType, bool isTmp = false)
        {
            UserDefinedDataType userDefinedDataType =
                new UserDefinedDataType(_dataTypeCollection, (JObject) jsonDataType);
            userDefinedDataType.IsTmp = isTmp;
            _dataTypeCollection.AddDataType(userDefinedDataType);
        }

        public void AddTask(JToken json)
        {
            JObjectExtensions jsonTask = new JObjectExtensions((JObject) json);

            CTask task = new CTask(this)
            {
                DisableUpdateOutputs = (bool) jsonTask["DisableUpdateOutputs"],
                IsInhibited = (bool) jsonTask["InhibitTask"],
                Name = (string) jsonTask["Name"],
                Priority = (int) jsonTask["Priority"],
                Type = jsonTask["Type"].ToObject<TaskType>(),
                Watchdog = (float) jsonTask["Watchdog"],
                Description = (string) jsonTask["Description"]
            };

            if (task.Type == TaskType.Periodic)
            {
                if (jsonTask["Rate"] == null)
                {
                    task.Rate = 10;
                }
                else
                {
                    task.Rate = (float) jsonTask["Rate"];
                }
            }

            foreach (var programName in jsonTask["SchededPrograms"])
            {
                Program program = _programCollection[programName.ToString()] as Program;
                if (program != null)
                {
                    task.ScheduleProgram(program);
                }
                else
                {
                    throw new Exception("program is null");
                }
            }

            _taskCollection.AddTask(task);
        }

        private void AddParameterConnection(JToken json)
        {
            JObjectExtensions jsonConnection = new JObjectExtensions((JObject) json);

            ParameterConnection parameterConnection = new ParameterConnection
            {
                SourcePath = (string) jsonConnection["EndPoint1"],
                DestinationPath = (string) jsonConnection["EndPoint2"]
            };

            _parameterConnections.Add(parameterConnection);
        }

        public void AddTag(JToken jsonTag)
        {
            var tag = TagsFactory.CreateTag(_tagCollection, jsonTag);

            _tagCollection.AddTag(tag, false, false);
        }

        public IRoutine CreateRoutine(JObject json)
        {
            var jsonRoutine = new JObjectExtensions(json);

            var type = (RoutineType) (int) jsonRoutine["Type"];
            if (type == RoutineType.ST)
            {
                return STRoutine.Create(ParentController, jsonRoutine);
            }

            if (type == RoutineType.RLL)
            {
                return RLLRoutine.Create(ParentController, jsonRoutine);
            }

            if (type == RoutineType.FBD)
            {
                var routine = new FBDRoutine(ParentController, json) {Name = (string) jsonRoutine["Name"]};
                if (jsonRoutine["Description"] != null)
                    routine.Description = (string) jsonRoutine["Description"];
                return routine;
            }

            if (type == RoutineType.SFC)
            {
                var routine = new SFCRoutine(ParentController);
                routine.Name = (string) jsonRoutine["Name"];
                var sfcContent = (JObject) jsonRoutine["SFCContent"];
                routine.SheetSize = EnumUtils.Parse<SheetSize>((string) sfcContent["SheetSize"]);
                routine.SheetOrientation = EnumUtils.Parse<Orientation>((string) sfcContent["SheetOrientation"]);
                routine.StepName = (string) sfcContent["StepName"];
                routine.TransitionName = (string) sfcContent["TransitionName"];
                routine.ActionName = (string) sfcContent["ActionName"];
                routine.StopName = (string) sfcContent["StopName"];
                if (sfcContent["Contents"] != null)
                {
                    var contents = (JArray) sfcContent["Contents"];
                    foreach (var content in contents)
                    {
                        var c = CreateContent((JObject) content, routine);
                        routine.Contents.Add(c);
                    }
                }

                if (jsonRoutine["Description"] != null)
                    routine.Description = (string) jsonRoutine["Description"];
                return routine;
            }
            else
            {
                Debug.Assert(false, type.ToString());
                return null;
            }
        }

        public Program AddProgram(JToken json)
        {
            Program program = new Program(this);
            program.Override((JObject) json, _programCollection);
            //JObjectExtensions jsonProgram = new JObjectExtensions((JObject) json);

            //program.Inhibited = (bool) jsonProgram["Inhibited"];
            //program.MainRoutineName = (string) jsonProgram["MainRoutineName"];
            //program.Name = (string) jsonProgram["Name"];
            //program.FaultRoutineName = (string) jsonProgram["FaultRoutineName"];
            //if (jsonProgram["Description"] != null)
            //{
            //    program.Description = (string) jsonProgram["Description"];
            //}

            //if (jsonProgram["Type"] != null)
            //{
            //    program.Type = (ProgramType) ((int) jsonProgram["Type"]);
            //}
            //else
            //{
            //    program.Type = ProgramType.Normal;
            //}

            //if (jsonProgram["InitialStepIndex"] != null)
            //{
            //    program.ProgramProperties.InitialStepIndex = (byte) jsonProgram["InitialStepIndex"];
            //}

            //if (jsonProgram["InitialState"] != null)
            //{
            //    program.ProgramProperties.InitialState = (InitialStateType) (int) jsonProgram["InitialState"];
            //}

            //if (jsonProgram["CompleteStateIfNotImpl"] != null)
            //{
            //    program.ProgramProperties.CompleteStateIfNotImpl =
            //        (CompleteStateIfNotImplType) (int) jsonProgram["CompleteStateIfNotImpl"];
            //}

            //if (jsonProgram["LossOfCommCmd"] != null)
            //{
            //    program.ProgramProperties.LossOfCommCmd = (LossOfCommCmdType) (int) jsonProgram["LossOfCommCmd"];
            //}

            //if (jsonProgram["ExternalRequestAction"] != null)
            //{
            //    program.ProgramProperties.ExternalRequestAction =
            //        (ExternalRequestActionType) (int) jsonProgram["ExternalRequestAction"];
            //}

            //if (jsonProgram["UseAsFolder"] != null)
            //{
            //    program.ProgramProperties.UseAsFolder = (bool) jsonProgram["UseAsFolder"];
            //}

            //if (jsonProgram["AutoValueAssignStepToPhase"] != null)
            //{
            //    program.ProgramProperties.AutoValueAssignStepToPhase = (bool) jsonProgram["AutoValueAssignStepToPhase"];
            //}

            //if (jsonProgram["AutoValueAssignPhaseToStepOnComplete"] != null)
            //{
            //    program.ProgramProperties.AutoValueAssignPhaseToStepOnComplete =
            //        (bool) jsonProgram["AutoValueAssignPhaseToStepOnComplete"];
            //}

            //if (jsonProgram["AutoValueAssignPhaseToStepOnStopped"] != null)
            //{
            //    program.ProgramProperties.AutoValueAssignPhaseToStepOnStopped =
            //        (bool) jsonProgram["AutoValueAssignPhaseToStepOnStopped"];
            //}

            //if (jsonProgram["AutoValueAssignPhaseToStepOnAborted"] != null)
            //{
            //    program.ProgramProperties.AutoValueAssignPhaseToStepOnAborted =
            //        (bool) jsonProgram["AutoValueAssignPhaseToStepOnAborted"];
            //}

            //// Routines
            //foreach (var routine in jsonProgram["Routines"])
            //{
            //    program.AddRoutine(CreateRoutine(routine as JObject));
            //}

            //// Tags
            //foreach (var tag in jsonProgram["Tags"])
            //{
            //    program.AddTag(tag);
            //}

            //_programCollection.AddProgram(program);
            //program.CheckTestStatus();
            return program;
        }

        private IContent CreateContent(JObject content, IRoutine routine)
        {
            string name = (string) content["Name"];
            if (name == "Step")
            {
                var step = new Step
                {
                    ID = (int) content["ID"],
                    X = (double) content["X"],
                    Y = (double) content["Y"],
                    Operand = (string) content["Operand"],
                    HideDesc = (bool) content["HideDesc"],
                    DescX = (double) content["DescX"],
                    DescY = (double) content["DescY"],
                    DescWidth = (double) content["DescWidth"],
                    InitialStep = (bool) content["InitialStep"],
                    PresetUsesExpr = (bool) content["PresetUsesExpr"],
                    LimitHighUsesExpr = (bool) content["LimitHighUsesExpr"],
                    LimitLowUsesExpr = (bool) content["LimitLowUsesExpr"],
                    ShowActions = (bool) content["ShowActions"]
                };
                return step;
            }

            if (name == "Transition")
            {
                var transition = new Transition
                {
                    ID = (int) content["ID"],
                    X = (double) content["X"],
                    Y = (double) content["Y"],
                    Operand = (string) content["Operand"],
                    HideDesc = (bool) content["HideDesc"],
                    DescX = (double) content["DescX"],
                    DescY = (double) content["DescY"],
                    DescWidth = (double) content["DescWidth"]
                };
                var stContent = (JArray) content["STContent"];
                if (stContent != null)
                {
                    foreach (var line in stContent)
                    {
                        transition.CodeText.Add(line.Value<string>());
                    }
                }

                return transition;
            }

            if (name == "Stop")
            {
                var stop = new Stop
                {
                    ID = (int) content["ID"],
                    X = (double) content["X"],
                    Y = (double) content["Y"],
                    Operand = (string) content["Operand"],
                    HideDesc = (bool) content["HideDesc"],
                    DescX = (double) content["DescX"],
                    DescY = (double) content["DescY"],
                    DescWidth = (double) content["DescWidth"]
                };
                return stop;
            }

            if (name == "Branch")
            {
                var branch = new Branch
                {
                    ID = (int) content["ID"],
                    Y = (double) content["Y"],
                    BranchType = EnumUtils.Parse<BranchType>((string) content["BranchType"]),
                    BranchFlow = EnumUtils.Parse<BranchFlowType>((string) content["BranchFlow"])
                };
                var legs = (JArray) content["Legs"];
                foreach (var leg in legs)
                {
                    branch.Legs.Add((int) leg["ID"]);
                }

                return branch;
            }

            if (name == "TextBox")
            {
                var textBox = new TextBox
                {
                    ID = (int) content["ID"],
                    X = (double) content["X"],
                    Y = (double) content["Y"],
                    Width = (double) content["Width"],
                    Text = (string) content["Text"]
                };
                return textBox;
            }

            if (name == "DirectedLink")
            {
                var link = new DirectedLink
                {
                    FromID = (int) content["FromID"],
                    ToID = (int) content["ToID"],
                    Show = (bool) content["Show"]
                };
                return link;
            }

            if (name == "Attachment")
            {
                var attachment = new Attachment
                {
                    FromID = (int) content["FromID"],
                    ToID = (int) content["ToID"]
                };
                return attachment;
            }

            Debug.Assert(false, name ?? string.Empty);
            return null;
        }

        private class FakeModule : DeviceModule.DeviceModule
        {
            private readonly JObject _config;

            public FakeModule(IController ctrl, JObject config) : base(ctrl)
            {
                Debug.WriteLine($"Unsupported module type {config["ProductType"]}");
                _config = config;
            }

            public override JObject ConvertToJObject()
            {
                return _config;
            }
        }

        //TODO(gjc): need change to private
        public void AddDeviceModule(JToken json)
        {
            JObjectExtensions module = new JObjectExtensions((JObject) json);

            DeviceModule.DeviceModule deviceModule;

            var productType = module["ProductType"].ToObject<CipDeviceType>();
            string catalogNumber = (string) module["CatalogNumber"];
            int productCode = (int) module["ProductCode"];
            int vendor = (int) module["Vendor"];
            var description = module["Description"] == null ? "" : (string) module["Description"];
            string dllPath = AssemblyUtils.AssemblyDirectory;
            string folder;
            string fileName;


            switch (productType)
            {
                case CipDeviceType.CIPMotionDrive:
                    // Profiles
                    folder = dllPath + @"\ModuleProfiles\MotionDrive\";

                    fileName = folder + $@"{catalogNumber}.json";

                    var driveProfiles = ProfilesExtension.DeserializeFromFile<MotionDriveProfiles>(fileName);

                    var moduleTypes =
                        ProfilesExtension.DeserializeFromFile<MotionDriveModuleTypes>(
                            folder + driveProfiles.ExtendedProperties.PSFile);

                    driveProfiles.ModuleTypes = moduleTypes;

                    deviceModule = new CIPMotionDrive(this, driveProfiles) {Description = description};
                    break;
                case CipDeviceType.ProgrammableLogicController:
                    if ((string) module["Name"] == "Local")
                        deviceModule = new LocalModule(this, catalogNumber);
                    else
                        //TODO(gjc):need edit here
                        deviceModule = new FakeModule(this, (JObject) json);
                    break;
                case CipDeviceType.CommunicationsAdapter:
                    fileName = dllPath + $@"\ModuleProfiles\DIO Enet Adapter\{catalogNumber.RemoveSeries()}.json";
                    var adapterProfiles = ProfilesExtension.DeserializeFromFile<DIOEnetAdapterProfiles>(fileName);

                    deviceModule = new CommunicationsAdapter(this, adapterProfiles) {Description = description};
                    break;
                case CipDeviceType.GeneralPurposeDiscreteIO:
                    if (catalogNumber == "Embedded")
                    {
                        if (vendor == 1 && productCode == 1140)
                        {
                            folder = dllPath + @"\ModuleProfiles\Embedded\";

                            fileName =
                                folder + "Embedded_DiscreteIO.json";
                            var discreteIOProfiles =
                                ProfilesExtension.DeserializeFromFile<DIOModuleProfiles>(fileName);

                            var discreteModuleTypes =
                                ProfilesExtension.DeserializeFromFile<DIOModuleTypes>(
                                    folder + discreteIOProfiles.PSFile);

                            discreteIOProfiles.DIOModuleTypes = discreteModuleTypes;

                            deviceModule = new DiscreteIO(this, discreteIOProfiles) {Description = description};
                        }
                        else
                        {
                            //TODO(gjc):need edit here
                            deviceModule = new FakeModule(this, (JObject) json);
                        }
                    }
                    else
                    {
                        folder = dllPath + $@"\ModuleProfiles\DIO Discrete\";

                        fileName = folder + $"{catalogNumber.RemoveSeries()}.json";
                        var discreteIOProfiles =
                            ProfilesExtension.DeserializeFromFile<DIOModuleProfiles>(fileName);

                        var discreteModuleTypes =
                            ProfilesExtension.DeserializeFromFile<DIOModuleTypes>(
                                folder + discreteIOProfiles.PSFile);

                        discreteIOProfiles.DIOModuleTypes = discreteModuleTypes;

                        deviceModule = new DiscreteIO(this, discreteIOProfiles) {Description = description};
                    }

                    break;

                case CipDeviceType.GeneralPurposeAnalogIO:
                    folder = dllPath + $@"\ModuleProfiles\DIO Analog\";

                    fileName = folder + $"{catalogNumber.RemoveSeries()}.json";

                    var analogIOProfiles =
                        ProfilesExtension.DeserializeFromFile<DIOModuleProfiles>(fileName);

                    var analogModuleTypes =
                        ProfilesExtension.DeserializeFromFile<DIOModuleTypes>(
                            folder + analogIOProfiles.PSFile);

                    analogIOProfiles.DIOModuleTypes = analogModuleTypes;

                    deviceModule = new AnalogIO(this, analogIOProfiles) {Description = description};

                    break;

                case CipDeviceType.Generic:
                    folder = dllPath + $@"\ModuleProfiles\Generic\";

                    fileName = folder + $"{catalogNumber}.json";

                    var profiles =
                        ProfilesExtension.DeserializeFromFile<GenericEnetModuleProfiles>(fileName);

                    deviceModule = new GeneralEthernet(this, profiles) {Description = description};
                    break;


                default:
                    //TODO(gjc):need edit here
                    deviceModule = new FakeModule(this, json as JObject);
                    break;
            }

            Debug.Assert(deviceModule != null);

            deviceModule.CatalogNumber = (string) module["CatalogNumber"];
            deviceModule.Inhibited = (bool) module["Inhibited"];
            deviceModule.Major = (int) module["Major"];
            deviceModule.MajorFault = (bool) module["MajorFault"];
            deviceModule.Minor = (int) module["Minor"];
            deviceModule.Name = (string) module["Name"];
            deviceModule.ParentModPortId = (int) module["ParentModPortId"];
            deviceModule.ParentModuleName = GetFinalName(typeof(IDeviceModule), (string) module["ParentModule"]);
            deviceModule.ProductCode = (int) module["ProductCode"];
            deviceModule.ProductType = (int) module["ProductType"];
            deviceModule.Vendor = (int) module["Vendor"];

            Debug.Assert(module["EKey"] != null);
            deviceModule.EKey = module["EKey"].ToObject<ElectronicKeyingType>();

            //Ports
            Debug.Assert(module["Ports"] != null);
            if (module["Ports"] != null)
            {
                foreach (var jsonPort in module["Ports"])
                {
                    Port port = new Port
                    {
                        Address = (string) jsonPort["Address"],
                        Id = (int) jsonPort["Id"],
                        Type = jsonPort["Type"].ToObject<PortType>(),
                        Upstream = (bool) jsonPort["Upstream"],
                    };

                    if (jsonPort["Bus"] != null)
                        port.Bus = jsonPort["Bus"].ToObject<Bus>();

                    // update
                    foreach (var p in deviceModule.Ports)
                    {
                        if (p.Type == port.Type && p.Id == port.Id && p.Upstream == port.Upstream)
                        {
                            p.Address = port.Address;
                            p.Bus = port.Bus;
                        }
                    }
                }
            }

            if (module["ExtendedProperties"] != null)
                deviceModule.ExtendedProperties = module["ExtendedProperties"].ToObject<ExtendedProperties>();

            _deviceModuleCollection.AddDeviceModule(deviceModule);

            CIPMotionDrive cipMotionDrive = deviceModule as CIPMotionDrive;
            if (cipMotionDrive != null)
            {
                // ConfigData
                if (module["ConfigData"] != null)
                    cipMotionDrive.ConfigData = module["ConfigData"].ToObject<CIPMotionDriveConfigData>();
            }

            DiscreteIO discreteIO = deviceModule as DiscreteIO;
            if (discreteIO != null)
            {
                if (module["Communications"] != null)
                    discreteIO.Communications = module["Communications"].ToObject<DiscreteIOCommunications>();

                discreteIO.ConfigID = uint.Parse(discreteIO.ExtendedProperties.Public["ConfigID"]);
            }

            AnalogIO analogIO = deviceModule as AnalogIO;
            if (analogIO != null)
            {
                if (module["Communications"] != null)
                    analogIO.Communications = module["Communications"].ToObject<DiscreteIOCommunications>();

                analogIO.ConfigID = uint.Parse(analogIO.ExtendedProperties.Public["ConfigID"]);
            }

            CommunicationsAdapter adapter = deviceModule as CommunicationsAdapter;
            if (adapter != null)
            {
                if (module["Communications"] != null)
                    adapter.Communications = module["Communications"].ToObject<DiscreteIOCommunications>();

                adapter.ConfigID = uint.Parse(adapter.ExtendedProperties.Public["ConfigID"]);

                adapter.CheckDataType();
            }

            GeneralEthernet generalEthernet = deviceModule as GeneralEthernet;
            if (generalEthernet != null)
            {
                if (module["Communications"] != null)
                    generalEthernet.Communications = module["Communications"].ToObject<GeneralEthernetCommunications>();
            }

        }

        public AoiDefinition AddAOIDefinition(JObject json, bool isTmp = false)
        {
            var def = new AoiDefinition(json, this, isTmp);
            _aoiDefinitionCollection.Add(def);
            _dataTypeCollection.AddDataType(def.datatype);
            return def;
        }

        public void AddTrend(JToken json)
        {
            Debug.Assert(json is JObject);
            _trends.Add(new Trend(json as JObject, this));
        }

        private void AddTimeSetting(JObject json)
        {
            TimeSetting.Priority1 = (int) json["Priority1"];
            TimeSetting.Priority2 = (int) json["Priority2"];
            TimeSetting.PTPEnable = (bool) json["PTPEnable"];
        }

        public JObject ConvertToJObject(bool useCode, bool needNativeCode)
        {
            JObject controller = new JObject();

            // Modules
            JArray modules = new JArray();

            DeviceModuleCollection deviceModuleCollection = DeviceModules as DeviceModuleCollection;
            deviceModuleCollection?.Sort();

            foreach (var d in DeviceModules)
            {
                var deviceModule = d as DeviceModule.DeviceModule;
                if (deviceModule != null)
                    modules.Add(deviceModule.ConvertToJObject());
            }

            controller.Add("Modules", modules);

            controller.Add("Name", Name);

            string serialNumber = Convert.ToString(ProjectSN, 16).PadLeft(8, '0');
            serialNumber = Regex.Replace(serialNumber, @"((?<=\w)(?=(\w{4})+$))", "$1_");
            serialNumber = "16#" + serialNumber;
            controller.Add("ProjectSN", serialNumber);

            controller.Add("MatchProjectToController", MatchProjectToController);

            if (!string.IsNullOrEmpty(EtherNetIPMode))
            {
                controller.Add("EtherNetIPMode", EtherNetIPMode);
            }

            if (!string.IsNullOrEmpty(Description))
            {
                controller.Add("Description", Description);
            }

            if (!string.IsNullOrEmpty(ProjectCommunicationPath))
            {
                controller.Add("CommPath", ProjectCommunicationPath);
            }

            if (!string.IsNullOrEmpty(MajorFaultProgram))
            {
                controller.Add("MajorFaultProgram", MajorFaultProgram);
            }

            if (!string.IsNullOrEmpty(PowerLossProgram))
            {
                controller.Add("PowerLossProgram", PowerLossProgram);
            }

            // Programs
            JArray programs = new JArray();
            foreach (var p in Programs)
            {
                var program = p as Program;
                if (program != null)
                    programs.Add(program.ConvertToJObject(useCode, needNativeCode));
            }

            controller.Add("Programs", programs);

            // Tags
            JArray tags = new JArray();
            foreach (var t in Tags)
            {
                var tag = t as Tag;

                if (tag != null && !tag.IsModuleTag)
                {
                    tags.Add(tag.ConvertToJObject());
                }
            }

            controller.Add("Tags", tags);

            // Tasks
            JArray tasks = new JArray();
            foreach (var t in Tasks)
            {
                var task = t as CTask;
                if (task != null)
                {
                    tasks.Add(task.ConvertToJObject());
                }
            }

            controller.Add("Tasks", tasks);

            // ParameterConnections
            if (ParameterConnections.Count > 0)
            {
                JArray connections = new JArray();
                foreach (var p in ParameterConnections)
                {
                    var parameterConnection = p as ParameterConnection;
                    if (parameterConnection != null)
                    {
                        connections.Add(parameterConnection.ConvertToJObject());
                    }
                }

                controller.Add("ParameterConnections", connections);
            }

            // DataTypes
            var userDefinedDataTypes = _dataTypeCollection.OfType<UserDefinedDataType>().ToArray();
            //if (userDefinedDataTypes.Any())
            {
                JArray dataTypes = new JArray();
                foreach (var userDefinedDataType in userDefinedDataTypes)
                {
                    dataTypes.Add(userDefinedDataType.ConvertToJObject());
                }

                controller.Add("DataTypes", dataTypes);
            }

            // Module Defined Data Types
            var moduleDefinedDataTypes = _dataTypeCollection.OfType<ModuleDefinedDataType>().ToArray();
            /*
            if (moduleDefinedDataTypes.Any())
            {
            */
            {
                JArray dataTypes = new JArray();
                foreach (var moduleDefinedDataType in moduleDefinedDataTypes)
                {
                    dataTypes.Add(moduleDefinedDataType.ConvertToJObject());
                }

                controller.Add("ModuleDefinedDataTypes", dataTypes);
            }
            //}

            // AddOnInstructionDefinitions
            JArray aoiDefinitions = new JArray();
            foreach (var def in _aoiDefinitionCollection)
            {
                aoiDefinitions.Add(def.ConvertToJObject(needNativeCode));
            }

            controller.Add("AddOnInstructionDefinitions", aoiDefinitions);

            // TimeSynchronize
            controller.Add("TimeSynchronize", TimeSetting.ConvertToJObject());

            controller.Add("Trends", _trends.ToJson());

            if (needNativeCode && NativeCode != null)
                controller.Add("NativeCode", System.Convert.ToBase64String(NativeCode));

            // Change logs
            var changeLogs = _transactionManager?.ConvertToJArray();
            if (changeLogs != null)
            {
                controller.Add("ChangeLogs", changeLogs);
            }
            
            return controller;
        }

        public static int GetProductCode(string projectName)
        {
            if ("ICC-T0100ERM".Equals(projectName, StringComparison.OrdinalIgnoreCase))
            {
                return 608;
            }
            if ("ICC-P0100ERM".Equals(projectName, StringComparison.OrdinalIgnoreCase))
            {
                return 408;
            }

            if ("ICC-P010ERM".Equals(projectName, StringComparison.OrdinalIgnoreCase))
            {
                return 408;
            }

            if ("ICC-P020ERM".Equals(projectName, StringComparison.OrdinalIgnoreCase))
            {
                return 408;
            }

            if ("ICC-B010ERM".Equals(projectName, StringComparison.OrdinalIgnoreCase))
            {
                return 108;
            }

            return 0;
        }
    }
}