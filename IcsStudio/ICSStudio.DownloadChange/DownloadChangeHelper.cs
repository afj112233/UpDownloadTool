using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace ICSStudio.DownloadChange
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public class DownloadChangeHelper
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly JsonDiffPatch.JsonDiffPatch _diffPath;

        #region Axis Parameters

        private readonly List<string> _allowedAxisParameters = new List<string>
        {
            "TorqueOffset",
            "TorqueLimitPositive",
            "TorqueLimitNegative",

            "MaximumSpeed",
            "MaximumAcceleration",
            "MaximumDeceleration",

            "PositionErrorTolerance",
            "VelocityErrorTolerance",

            "VelocityLimitPositive",
            "VelocityLimitNegative",

            //"TorqueLeadLagFilterBandwidth",
            //"TorqueLeadLagFilterGain",

            "PositionLoopBandwidth",
            "PositionIntegratorBandwidth",
            "VelocityFeedforwardGain",
            "VelocityLoopBandwidth",
            "VelocityIntegratorBandwidth",
            "AccelerationFeedforwardGain",

            "AccelerationLimit",
            "DecelerationLimit",

            "LoadObserverBandwidth",
            "LoadObserverIntegratorBandwidth",

            "FrictionCompensationSliding"
        };

        private readonly List<string> _cyclicReadParameters = new List<string>
        {
            "PositionFineCommand",
            "PositionReference",
            "PositionFeedback1", // DINT
            "PositionError",
            "PositionIntegratorOutput",
            "PositionLoopOutput",
            "VelocityFineCommand",
            "VelocityFeedforwardCommand",
            "VelocityReference",
            "VelocityFeedback",
            "VelocityError",
            "VelocityIntegratorOutput",
            "VelocityLoopOutput",
            "VelocityLimitSource", //DINT
            "AccelerationFineCommand",
            "AccelerationFeedforwardCommand",
            "AccelerationReference",
            "AccelerationFeedback",
            "LoadObserverAccelerationEstimate",
            "LoadObserverTorqueEstimate",
            "TorqueReference",
            "TorqueReferenceFiltered",
            "TorqueReferenceLimited",
            "TorqueNotchFilterFrequencyEstimate",
            "TorqueNotchFilterMagnitudeEstimate",
            "TorqueLowPassFilterBandwidthEstimate",
            "AdaptiveTuningGainScalingFactor",
            "CurrentCommand",
            "CurrentReference",
            "CurrentFeedback",
            "CurrentError",
            "FluxCurrentReference",
            "FluxCurrentFeedback",
            "FluxCurrentError",
            "OperativeCurrentLimit",
            "CurrentLimitSource", //DINT
            "MotorElectricalAngle",
            "OutputFrequency",
            "OutputCurrent",
            "OutputVoltage",
            "OutputPower",
            "ConverterOutputCurrent",
            "ConverterOutputPower",
            "DCBusVoltage",
            "MotorCapacity",
            "InverterCapacity",
            "ConverterCapacity",
            "BusRegulatorCapacity"
        };

        private readonly List<string> _cyclicWriteParameters = new List<string>
        {
            "PositionTrim",
            "VelocityTrim",
            "TorqueTrim",
            "VelocityFeedforwardGain",
            "AccelerationFeedforwardGain",
            "PositionLoopBandwidth",
            "PositionIntegratorBandwidth",
            "VelocityLoopBandwidth",
            "VelocityIntegratorBandwidth",
            "LoadObserverBandwidth",
            "LoadObserverIntegratorBandwidth",
            "TorqueLimitPositive",
            "TorqueLimitNegative",
            "VelocityLowPassFilterBandwidth",
            "TorqueLowPassFilterBandwidth",
            "SystemInertia"
        };

        #endregion

        private readonly List<string> _allowedMsgParameters = new List<string>()
        {
            "ConnectionPath", "TargetObject", "RequestedLength"
        };

        public DownloadChangeHelper(ProjectDiffModel diffModel) : this(diffModel, null)
        {
        }

        public DownloadChangeHelper(ProjectDiffModel diffModel, Controller controller)
        {
            DiffModel = diffModel;
            Controller = controller;

            _diffPath = new JsonDiffPatch.JsonDiffPatch();
        }

        public ProjectDiffModel DiffModel { get; }

        public ICipMessager Messager => Controller?.CipMessager;

        public Controller Controller { get; }

        public bool CanDownload
        {
            get
            {
                if (DiffModel == null)
                    return true;

                if (DiffModel.ControllerProperties.ChangeType != ItemChangeType.Unchanged)
                    return false;

                if (DiffModel.ControllerTags != null && DiffModel.ControllerTags.Count > 0)
                {
                    foreach (var diffItem in DiffModel.ControllerTags)
                    {
                        if (diffItem.ChangeType == ItemChangeType.Deleted)
                            return false;

                        if (diffItem.ChangeType == ItemChangeType.Modified)
                        {
                            string dataType = ((JObject)diffItem.OldValue)["DataType"]?.ToString();
                            if (string.Equals(dataType, "AXIS_CIP_DRIVE"))
                            {
                                if (!CanDownloadAxisChange(diffItem))
                                    return false;
                            }
                            else if (string.Equals(dataType, "MESSAGE"))
                            {
                                if (!CanDownloadMessageChange(diffItem))
                                    return false;
                            }
                            else
                            {
                                return false;
                            }

                        }

                        if (diffItem.ChangeType == ItemChangeType.Added)
                        {
                            string dataType = ((JObject)diffItem.NewValue)["DataType"]?.ToString();
                            if (string.Equals(dataType, "MOTION_GROUP") ||
                                string.Equals(dataType, "AXIS_CIP_DRIVE") ||
                                string.Equals(dataType, "AXIS_VIRTUAL"))
                                return false;

                        }
                    }
                }

                if (DiffModel.DataTypes != null && DiffModel.DataTypes.Count > 0)
                {
                    foreach (var diffItem in DiffModel.DataTypes)
                    {
                        if (diffItem.ChangeType != ItemChangeType.Unchanged)
                            return false;
                    }
                }

                if (DiffModel.AOIDefinitions != null && DiffModel.AOIDefinitions.Count > 0)
                {
                    foreach (var diffItem in DiffModel.AOIDefinitions)
                    {
                        if (diffItem.ChangeType != ItemChangeType.Unchanged)
                            return false;
                    }
                }

                if (DiffModel.Modules != null && DiffModel.Modules.Count > 0)
                {
                    foreach (var diffItem in DiffModel.Modules)
                    {
                        if (diffItem.ChangeType == ItemChangeType.Modified)
                        {
                            return false;
                        }

                        if (diffItem.ChangeType == ItemChangeType.Added ||
                            diffItem.ChangeType == ItemChangeType.Deleted)
                            return false;
                    }
                }

                if (DiffModel.Tasks != null && DiffModel.Tasks.Count > 0)
                {
                    foreach (var diffItem in DiffModel.Tasks)
                    {
                        if (diffItem.ChangeType == ItemChangeType.Added ||
                            diffItem.ChangeType == ItemChangeType.Deleted)
                            return false;

                        if (diffItem.ChangeType == ItemChangeType.Modified)
                        {
                            if (!CanDownloadTaskChange(diffItem))
                                return false;
                        }
                    }
                }

                if (DiffModel.Programs != null && DiffModel.Programs.Count > 0)
                {
                    foreach (var diffItem in DiffModel.Programs)
                    {
                        if (diffItem.ChangeType == ItemChangeType.Added ||
                            diffItem.ChangeType == ItemChangeType.Deleted)
                            return false;

                        if (diffItem.ChangeType == ItemChangeType.Modified)
                        {
                            if (!CanDownloadProgramChange(diffItem))
                                return false;
                        }
                    }
                }

                return true;
            }
        }

        private bool CanDownloadMessageChange(IDiffItem diffItem)
        {
            string dataType = ((JObject)diffItem.OldValue)["DataType"]?.ToString();
            Debug.Assert(string.Equals(dataType, "MESSAGE"));

            var result = _diffPath.Diff(diffItem.OldValue, diffItem.NewValue) as JObject;
            if (result != null)
            {
                JObject parameters = null;

                if (result.ContainsKey("Parameters"))
                {
                    parameters = result["Parameters"] as JObject;
                    result.Remove("Parameters");
                }

                if (result.Count > 0)
                {
                    string tagName = ((JObject)diffItem.OldValue)["Name"]?.ToString();
                    Logger.Info($"{tagName} diff: {parameters}");

                    return false;
                }

                if (parameters != null)
                {
                    foreach (var canChangeParameter in _allowedMsgParameters)
                    {
                        if (parameters.ContainsKey(canChangeParameter))
                            parameters.Remove(canChangeParameter);
                    }


                    if (parameters.Count > 0)
                    {
                        string tagName = ((JObject)diffItem.OldValue)["Name"]?.ToString();
                        Logger.Info($"{tagName} parameters diff: {parameters}");

                        return false;
                    }

                }
            }


            return true;
        }

        private bool CanDownloadAxisChange(IDiffItem diffItem)
        {
            string dataType = ((JObject)diffItem.OldValue)["DataType"]?.ToString();
            Debug.Assert(string.Equals(dataType, "AXIS_CIP_DRIVE"));

            var result = _diffPath.Diff(diffItem.OldValue, diffItem.NewValue) as JObject;
            if (result != null)
            {
                JObject parameters = null;

                if (result.ContainsKey("Parameters"))
                {
                    parameters = result["Parameters"] as JObject;
                    result.Remove("Parameters");
                }

                if (result.Count > 0)
                {
                    string tagName = ((JObject)diffItem.OldValue)["Name"]?.ToString();
                    Logger.Info($"{tagName} diff: {parameters}");

                    return false;
                }


                if (parameters != null)
                {
                    //TODO(gjc): add code here

                    foreach (var canChangeParameter in _allowedAxisParameters)
                    {
                        if (parameters.ContainsKey(canChangeParameter))
                            parameters.Remove(canChangeParameter);
                    }

                    foreach (var readParameter in _cyclicReadParameters)
                    {
                        if (parameters.ContainsKey(readParameter))
                            parameters.Remove(readParameter);
                    }

                    foreach (var writeParameter in _cyclicWriteParameters)
                    {
                        if (parameters.ContainsKey(writeParameter))
                            parameters.Remove(writeParameter);
                    }

                    if (parameters.Count > 0)
                    {
                        string tagName = ((JObject)diffItem.OldValue)["Name"]?.ToString();
                        Logger.Info($"{tagName} parameters diff: {parameters}");

                        return false;
                    }

                }

            }

            return true;
        }

        private bool CanDownloadProgramChange(IDiffItem diffItem)
        {
            Debug.Assert(diffItem.ChangeType == ItemChangeType.Modified);

            ProgramDiffItem programDiffItem = diffItem as ProgramDiffItem;
            Debug.Assert(programDiffItem != null);

            // Properties
            var propertiesDiff = programDiffItem.Properties;
            if (propertiesDiff.ChangeType == ItemChangeType.Modified)
            {
                var result = _diffPath.Diff(propertiesDiff.OldValue, propertiesDiff.NewValue) as JObject;

                if (result != null)
                {
                    // Inhibited
                    if (result.ContainsKey("Inhibited"))
                        result.Remove("Inhibited");

                    if (result.Count > 0)
                        return false;
                }

            }

            // Tags
            var tagsDiff = programDiffItem.Tags;
            if (tagsDiff != null && tagsDiff.Count > 0)
            {
                foreach (var tagDiff in tagsDiff)
                {
                    if (tagDiff.ChangeType == ItemChangeType.Unchanged)
                        continue;

                    if (tagDiff.ChangeType == ItemChangeType.Added)
                        continue;

                    return false;
                }
            }

            // Routines
            var routinesDiff = programDiffItem.Routines;
            if (tagsDiff != null && routinesDiff.Count > 0)
            {
                foreach (var routineDiff in routinesDiff)
                {
                    if (routineDiff.ChangeType == ItemChangeType.Unchanged)
                        continue;

                    if (routineDiff.ChangeType == ItemChangeType.Added ||
                        routineDiff.ChangeType == ItemChangeType.Deleted)
                        return false;

                    if (routineDiff.ChangeType == ItemChangeType.Modified)
                    {
                        // only for code
                        var result = _diffPath.Diff(routineDiff.OldValue, routineDiff.NewValue) as JObject;
                        if (result != null)
                        {
                            if (result.ContainsKey("CodeText"))
                                result.Remove("CodeText");

                            if (result.ContainsKey("PendingCodeText"))
                                result.Remove("PendingCodeText");

                            if (result.ContainsKey("TestCodeText"))
                                result.Remove("TestCodeText");

                            if (result.Count > 0)
                                return false;
                        }

                    }
                }
            }

            return true;
        }

        private bool CanDownloadTaskChange(IDiffItem diffItem)
        {
            Debug.Assert(diffItem.ChangeType == ItemChangeType.Modified);

            var result = _diffPath.Diff(diffItem.OldValue, diffItem.NewValue) as JObject;

            if (result != null)
            {
                // InhibitTask, Rate
                if (result.ContainsKey("InhibitTask"))
                    result.Remove("InhibitTask");

                if (result.ContainsKey("Rate"))
                    result.Remove("Rate");

                if (result.ContainsKey("Watchdog"))
                {
                    JArray diff = result["Watchdog"] as JArray;
                    if (diff != null && diff.Count == 2)
                    {
                        float oldValue = float.Parse(diff[0].ToString());
                        float newValue = float.Parse(diff[1].ToString());

                        if (Math.Abs(oldValue - newValue) < float.Epsilon)
                            result.Remove("Watchdog");

                    }
                }

                if (result.Count > 0)
                    return false;
            }

            return true;
        }

        public async Task Lock()
        {
            CIPController cipController = new CIPController(0, Messager);

            await cipController.WriterLockRetry();
        }

        public async Task Unlock()
        {
            CIPController cipController = new CIPController(0, Messager);

            await cipController.WriterUnLock();
        }

        public async Task<int> CreateControllerTags()
        {
            try
            {
                CIPController cipController = new CIPController(0, Messager);

                if (DiffModel.ControllerTags != null && DiffModel.ControllerTags.Count > 0)
                {
                    foreach (var diffItem in DiffModel.ControllerTags)
                    {
                        if (diffItem.ChangeType == ItemChangeType.Added)
                        {
                            string tagName = diffItem.NewValue["Name"]?.ToString();
                            Debug.Assert(!string.IsNullOrEmpty(tagName));

                            var tag = Controller.Tags[tagName] as Tag;
                            Debug.Assert(tag != null);

                            await cipController.TransmitCfg(
                                ToMsgPack(tag.ConvertToJObject()));

                            await cipController.CreateTag();
                        }
                    }
                }

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<int> CreateProgramTags()
        {
            try
            {
                if (DiffModel.Programs != null && DiffModel.Programs.Count > 0)
                {
                    CIPController cipController = new CIPController(0, Messager);

                    foreach (var diffItem in DiffModel.Programs.OfType<ProgramDiffItem>())
                    {
                        string programName = diffItem.OldValue["Name"]?.ToString();
                        Debug.Assert(!string.IsNullOrEmpty(programName));

                        Program program = Controller.Programs[programName] as Program;
                        Debug.Assert(program != null);

                        if (diffItem.Tags != null && diffItem.Tags.Count > 0)
                        {
                            int programId = await cipController.FindProgramId(programName);
                            CIPProgram cipProgram = new CIPProgram(programId, Messager);

                            foreach (var item in diffItem.Tags)
                            {
                                if (item.ChangeType == ItemChangeType.Added)
                                {
                                    string tagName = item.NewValue["Name"]?.ToString();
                                    Debug.Assert(!string.IsNullOrEmpty(tagName));

                                    Tag tag = program.Tags[tagName] as Tag;
                                    Debug.Assert(tag != null);

                                    await cipController.TransmitCfg(
                                        ToMsgPack(tag.ConvertToJObject()));

                                    await cipProgram.CreateTag();
                                }
                            }
                        }
                    }
                }

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<int> UpdatePrograms()
        {
            try
            {
                if (DiffModel.Programs != null && DiffModel.Programs.Count > 0)
                {
                    CIPController cipController = new CIPController(0, Messager);

                    foreach (var diffItem in DiffModel.Programs.OfType<ProgramDiffItem>())
                    {
                        string programName = diffItem.OldValue["Name"]?.ToString();
                        Debug.Assert(!string.IsNullOrEmpty(programName));

                        var program = Controller.Programs[programName] as Program;
                        Debug.Assert(program != null);

                        bool needUpdateProgram = false;

                        int programId = await cipController.FindProgramId(programName);
                        CIPProgram cipProgram = new CIPProgram(programId, Messager);

                        if (diffItem.Routines != null && diffItem.Routines.Count > 0)
                        {
                            foreach (var diffItemRoutine in diffItem.Routines)
                            {
                                if (diffItemRoutine.ChangeType == ItemChangeType.Modified)
                                {
                                    needUpdateProgram = true;

                                    string routineName = diffItemRoutine.NewValue["Name"]?.ToString();

                                    var routine = program.Routines[routineName];
                                    Debug.Assert(routine != null);

                                    routine.GenCode(program);

                                    var routineId = await cipProgram.FindRoutineId(routine.Name);
                                    CIPRoutine cipRoutine = new CIPRoutine((ushort)routineId, Messager);

                                    // replace code
                                    await cipController.TransmitCfg(
                                        ToMsgPack(routine.ConvertToJObject(true)));
                                    await cipRoutine.Replace();

                                }
                            }
                        }

                        if (needUpdateProgram)
                        {
                            program.GenSepNativeCode(Controller);

                            JObject programUpdateInfo = program.CreateProgramUpdateInfo();

                            int taskId = await cipController.FindTaskId(program.ParentTask?.Name);

                            await cipController.TransmitCfg(ToMsgPack(programUpdateInfo));
                            await cipProgram.Update(taskId);
                        }
                    }
                }

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<int> InhibitPrograms()
        {
            return await SetProgramsInhibit(true);
        }

        public async Task<int> UninhibitPrograms()
        {
            return await SetProgramsInhibit(false);
        }

        private async Task<int> SetProgramsInhibit(bool inhibit)
        {
            try
            {
                if (DiffModel.Programs != null && DiffModel.Programs.Count > 0)
                {
                    CIPController cipController = new CIPController(0, Messager);

                    foreach (var diffItem in DiffModel.Programs.OfType<ProgramDiffItem>())
                    {
                        string programName = diffItem.OldValue["Name"]?.ToString();
                        Debug.Assert(!string.IsNullOrEmpty(programName));

                        var program = Controller.Programs[programName];
                        Debug.Assert(program != null);

                        if (diffItem.Properties.ChangeType == ItemChangeType.Modified)
                        {
                            var result =
                                _diffPath.Diff(diffItem.Properties.OldValue, diffItem.Properties.NewValue) as JObject;
                            if (result != null)
                            {
                                if (result.ContainsKey("Inhibited") && program.Inhibited == inhibit)
                                {
                                    int programId = await cipController.FindProgramId(programName);
                                    CIPProgram cipProgram = new CIPProgram(programId, Messager);

                                    await cipProgram.SetInhibit(program.Inhibited);
                                }
                            }
                        }
                    }
                }

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<int> InhibitTasks()
        {
            return await SetTasksInhibit(true);
        }

        public async Task<int> UninhibitTasks()
        {
            return await SetTasksInhibit(false);
        }

        private async Task<int> SetTasksInhibit(bool inhibit)
        {
            try
            {
                if (DiffModel.Tasks != null && DiffModel.Tasks.Count > 0)
                {
                    CIPController cipController = new CIPController(0, Messager);

                    foreach (var diffItem in DiffModel.Tasks)
                    {
                        if (diffItem.ChangeType == ItemChangeType.Modified)
                        {
                            var result = _diffPath.Diff(diffItem.OldValue, diffItem.NewValue) as JObject;

                            if (result != null)
                            {
                                string taskName = diffItem.NewValue["Name"]?.ToString();
                                Debug.Assert(!string.IsNullOrEmpty(taskName));

                                CTask task = Controller.Tasks[taskName] as CTask;
                                Debug.Assert(task != null);

                                if (result.ContainsKey("InhibitTask") && task.IsInhibited == inhibit)
                                {
                                    int taskId = await cipController.FindTaskId(taskName);
                                    CIPTask cipTask = new CIPTask(taskId, Messager);

                                    await cipTask.SetInhibit(task.IsInhibited);
                                }

                            }

                        }
                    }
                }

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<int> UpdateTasksRate()
        {
            try
            {
                if (DiffModel.Tasks != null && DiffModel.Tasks.Count > 0)
                {
                    CIPController cipController = new CIPController(0, Messager);

                    foreach (var diffItem in DiffModel.Tasks)
                    {
                        if (diffItem.ChangeType == ItemChangeType.Modified)
                        {
                            var result = _diffPath.Diff(diffItem.OldValue, diffItem.NewValue) as JObject;

                            if (result != null)
                            {
                                string taskName = diffItem.NewValue["Name"]?.ToString();
                                Debug.Assert(!string.IsNullOrEmpty(taskName));

                                CTask task = Controller.Tasks[taskName] as CTask;
                                Debug.Assert(task != null);

                                if (result.ContainsKey("Rate"))
                                {
                                    int taskId = await cipController.FindTaskId(taskName);
                                    CIPTask cipTask = new CIPTask(taskId, Messager);

                                    await cipTask.SetPeriod(task.Rate);
                                }
                            }

                        }
                    }
                }

                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public Dictionary<string, List<string>> GetAxisPropertiesChange()
        {
            Dictionary<string, List<string>> axisChange = new Dictionary<string, List<string>>();

            if (DiffModel.ControllerTags != null && DiffModel.ControllerTags.Count > 0)
            {
                foreach (var diffItem in DiffModel.ControllerTags)
                {
                    if (diffItem.ChangeType == ItemChangeType.Modified)
                    {
                        string dataType = ((JObject)diffItem.OldValue)["DataType"]?.ToString();
                        if (string.Equals(dataType, "AXIS_CIP_DRIVE"))
                        {
                            string axisName = diffItem.OldValue["Name"]?.ToString();
                            Debug.Assert(!string.IsNullOrEmpty(axisName));

                            List<string> changeParameters = new List<string>();

                            var result = _diffPath.Diff(diffItem.OldValue, diffItem.NewValue) as JObject;

                            if (result != null)
                            {
                                if (result.ContainsKey("Parameters"))
                                {
                                    var parameters = result["Parameters"] as JObject;
                                    if (parameters != null)
                                    {
                                        foreach (var axisParameter in _allowedAxisParameters)
                                        {
                                            if (parameters.ContainsKey(axisParameter))
                                            {
                                                changeParameters.Add(axisParameter);
                                            }
                                        }
                                    }
                                }
                            }

                            if (changeParameters.Count > 0)
                            {
                                axisChange.Add(axisName, changeParameters);
                            }

                        }
                    }
                }
            }

            return axisChange;
        }

        internal static List<byte> ToMsgPack(JObject obj)
        {
            var res = MessagePackSerializer.FromJson(JsonConvert.SerializeObject(obj)).ToList();
            for (int i = 0; i < res.Count; ++i)
            {
                res[i] = (byte)(res[i] ^ 0x5A);
            }

            return res;
        }
    }
}
