using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json.Linq;
using Type = System.Type;

namespace ICSStudio.ToolsPackage.Import
{
    public class ImportDialogVM: ViewModelBase
    {
        private double _v;
        private BackgroundWorker _backgroundWorker;
        private string _info="Importing...";
        private bool? _dialogResult;

        public ImportDialogVM()
        {
            _backgroundWorker = new BackgroundWorker();
            //_backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult , value); }
            get { return _dialogResult; }
        }

        public void Start(List<JObject> tags)
        {
            _backgroundWorker.RunWorkerAsync(tags);
        }

        public RelayCommand CancelCommand { get; }

        private void ExecuteCancelCommand()
        {
            Info = "Cancelling...";
            _backgroundWorker.CancelAsync();
            DialogResult = false;
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var controller = Controller.GetInstance();
            var tags = (List<JObject>)e.Argument;
            ConcurrentDictionary<TagCollection, Tuple<ConcurrentQueue<ITag>>> dic = new ConcurrentDictionary<TagCollection, Tuple<ConcurrentQueue<ITag>>>();
            var end = tags.Count;
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = System.Environment.ProcessorCount;
            var syncRoot=new object();
            Parallel.ForEach(tags, po, (tag, state) =>
            {
                if (((BackgroundWorker)sender).CancellationPending)
                {
                    state.Break();
                }

                var scope = (string)tag["Scope"];
                IProgramModule program = null;
                if (!string.IsNullOrEmpty(scope))
                {
                    program = controller.Programs[scope] ??
                              (IProgramModule)((AoiDefinitionCollection)controller.AOIDefinitionCollection
                              )
                              .Find(scope);
                }

                Info = $"Creating tag:{tag["Name"]}";
                var tagCollection = (TagCollection)(program != null ? program.Tags : controller.Tags);
                ConcurrentQueue<ITag> list;
                lock (syncRoot)
                {
                    if (!dic.ContainsKey(tagCollection))
                    {
                        list = new ConcurrentQueue<ITag>();
                        dic.GetOrAdd(tagCollection, new Tuple<ConcurrentQueue<ITag>>(list));
                    }
                    else
                    {
                        var v = dic[tagCollection];
                        list = v.Item1;
                    }
                }

                bool isExist = false;
                var newTag = tagCollection[(string)tag["Name"]] as Tag;
                if (newTag != null)
                {
                    isExist = true;
                }
                else
                {

                    newTag = new Tag(tagCollection);
                }

                newTag.Name = (string)tag["Name"];
                var dataTypeInfo = controller.DataTypes.ParseDataTypeInfo((string)tag["DataType"]);


                newTag.Description = (string)tag["Description"];
                var str = ((string)tag["Attributes"]);
                str = FixAttributes(str);
                var attributes = str.Split(',');
                if (dataTypeInfo.DataType.IsMotionGroupType)
                {
                    if (!isExist)
                    {
                        var data = MotionGroup.Create(dataTypeInfo.DataType);
                        newTag.DataWrapper = data;
                    }

                    ParseMotionGroupAttr(attributes, newTag);
                }
                else if (dataTypeInfo.DataType is AXIS_CIP_DRIVE)
                {
                    if (!isExist)
                    {
                        var data = AxisCIPDrive.Create(dataTypeInfo.DataType,
                            tagCollection.ParentController);
                        newTag.DataWrapper = data;
                    }

                    var axisCIPDrive = newTag.DataWrapper as AxisCIPDrive;
                    //Debug.Assert(axisCIPDrive != null);
                    if (axisCIPDrive == null) return;
                    var axis = axisCIPDrive.CIPAxis;
                    ParseAXIS_CIP_DRIVEAttr(attributes, newTag, axis);
                }
                else if (dataTypeInfo.DataType is AXIS_VIRTUAL)
                {
                    if (!isExist)
                    {
                        var data = AxisVirtual.Create(dataTypeInfo.DataType,
                            tagCollection.ParentController);
                        newTag.DataWrapper = data;
                    }

                    var axisVirtual = newTag.DataWrapper as AxisVirtual;
                    //Debug.Assert(axisVirtual != null);
                    if (axisVirtual == null) return;
                    var axis = axisVirtual.CIPAxis;
                    ParseAXIS_VIRTUALAttr(attributes, newTag, axis);
                }
                else if (dataTypeInfo.DataType.IsMessageType)
                {
                    if (!isExist)
                    {
                        var data = MessageDataWrapper.Create(dataTypeInfo.DataType,
                            tagCollection.ParentController);
                        newTag.DataWrapper = data;
                    }

                    ParseMessageAttr(attributes, newTag);
                }
                else
                {
                    if (!isExist)
                    {
                        var data = new DataWrapper(dataTypeInfo.DataType, dataTypeInfo.Dim1,
                            dataTypeInfo.Dim2,
                            dataTypeInfo.Dim3, null);
                        newTag.DataWrapper = data;
                    }

                    ParseNormalAttr(attributes, newTag);
                }
                list.Enqueue(newTag);
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    V = V + ((double)90 / end);
                });
            });

            if (_backgroundWorker.CancellationPending)
            {
                DialogResult = false;
                return;
            }

            Info = $"Adding tags...";

            foreach (var vk in dic)
            {
                vk.Key.AddTags(vk.Value.Item1.ToList(), vk.Key,false, false);
            }
            V = 100;
            DialogResult = true;
        }

        public string Info
        {
            set { Set(ref _info , value); }
            get { return _info; }
        }

        public double V
        {
            set { Set(ref _v, value); }
            get { return _v; }
        }

        #region import

        private string FixAttributes(string attributes)
        {
            attributes = FixValue(attributes);
            if (attributes.StartsWith("("))
                attributes = attributes.Substring(1);
            if (attributes.EndsWith(")"))
                attributes = attributes.Substring(0, attributes.Length - 1);
            return attributes;
        }


        public static string FixValue(string val)
        {
            if (val.StartsWith("\""))
                val = val.Substring(1);
            if (val.EndsWith("\""))
                val = val.Substring(0, val.Length - 1);
            return val;
        }

        private void ParseNormalAttr(string[] attributes, Tag newTag)
        {
            foreach (var attribute in attributes)
            {
                var attr = attribute.Split(new[] { ":=" }, StringSplitOptions.None);
                var attrName = attr[0].Trim();
                var val = GetAttributeValue(attribute);
                if ("Radix".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        DisplayStyle style;
                        var res = GetEnum(val, out style);
                        if (res)
                        {
                            newTag.DisplayStyle = style;
                        }
                    }

                    continue;
                }
                if ("Usage".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    Usage v;
                    var res = GetEnum(val, out v);
                    if (res)
                    {
                        newTag.Usage = v;
                    }
                    continue;
                }
                if ("Constant".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        if ("false".Equals(val, StringComparison.OrdinalIgnoreCase))
                        {
                            newTag.IsConstant = false;
                        }
                        else
                        {
                            newTag.IsConstant = true;
                        }
                    }

                    continue;
                }

                if ("ExternalAccess".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        ExternalAccess externalAccess;
                        var res = GetEnum(val, out externalAccess);
                        if (res)
                        {
                            newTag.ExternalAccess = externalAccess;
                        }
                    }

                    continue;
                }
            }
        }

        private void ParseMessageAttr(string[] attributes, Tag newTag)
        {
            var messageData = newTag.DataWrapper as MessageDataWrapper;
            if(messageData==null)return;
            foreach (var attribute in attributes)
            {
                var attr = attribute.Split(new[] { ":=" }, StringSplitOptions.None);
                var attrName = attr[0].Trim();
                var val = GetAttributeValue(attribute);
                if ("ExternalAccess".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        ExternalAccess externalAccess;
                        var res = GetEnum(val, out externalAccess);
                        if (res)
                        {
                            newTag.ExternalAccess = externalAccess;
                        }
                    }

                    continue;
                }
                if ("Usage".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    Usage v;
                    var res = GetEnum(val, out v);
                    if (res)
                    {
                        newTag.Usage = v;
                    }
                    continue;
                }
                if ("MessageType".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        MessageTypeEnum messageType;
                        var res = GetEnum(val, out messageType);
                        if (res)
                        {
                            messageData.Parameters.MessageType = (byte)messageType;
                        }
                    }

                    continue;
                }

                if ("RequestedLength".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        ushort len;
                        var res = ushort.TryParse(val, out len);
                        if (res)
                        {
                            messageData.Parameters.RequestedLength = len;
                        }
                    }

                    continue;
                }

                if ("CommTypeCode".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        byte code;
                        var res = byte.TryParse(val, out code);
                        if (res)
                        {
                            messageData.Parameters.CommTypeCode = code;
                        }
                    }

                    continue;
                }

                if ("LocalIndex".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        ulong index;
                        var res = ulong.TryParse(val, out index);
                        if (res)
                        {
                            messageData.Parameters.LocalIndex = index;
                        }
                    }

                    continue;
                }
            }
        }

        private void ParseMotionGroupAttr(string[] attributes, Tag newTag)
        {
            var motionGroup = newTag.DataWrapper as MotionGroup;
            if(motionGroup==null)return;
            foreach (var attribute in attributes)
            {
                var attr = attribute.Split(new[] { ":=" }, StringSplitOptions.None);
                var attrName = attr[0].Trim();
                var val = GetAttributeValue(attribute);
                if ("ExternalAccess".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        ExternalAccess externalAccess;
                        var res = GetEnum(val, out externalAccess);
                        if (res)
                        {
                            newTag.ExternalAccess = externalAccess;
                        }
                    }

                    continue;
                }

                if ("Usage".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    Usage v;
                    var res = GetEnum(val, out v);
                    if (res)
                    {
                        newTag.Usage = v;
                    }
                    continue;
                }
                if ("CoarseUpdatePeriod".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        int coarseUpdatePeriod;
                        var res = int.TryParse(val, out coarseUpdatePeriod);
                        if (res)
                        {
                            motionGroup.CoarseUpdatePeriod = coarseUpdatePeriod;
                        }
                    }

                    continue;
                }

                if ("PhaseShift".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        int phaseShift;
                        var res = int.TryParse(val, out phaseShift);
                        if (res)
                        {
                            motionGroup.PhaseShift = phaseShift;
                        }
                    }

                    continue;
                }

                if ("GeneralFaultType".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        GeneralFaultType type;
                        var res = GetEnum(val, out type);
                        if (res)
                        {
                            motionGroup.GeneralFaultType = type;
                        }
                    }

                    continue;
                }

                if ("AutoTagUpdate".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        if ("Enabled".Equals(val, StringComparison.OrdinalIgnoreCase))
                        {
                            motionGroup.AutoTagUpdate = true;
                        }
                        else
                        {
                            motionGroup.AutoTagUpdate = false;
                        }
                    }
                }

                if ("Alternate1UpdateMultiplier".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        int v;
                        var res = int.TryParse(val, out v);
                        if (res)
                        {
                            motionGroup.Alternate1UpdateMultiplier = v;
                        }
                    }
                }

                if ("Alternate2UpdateMultiplier".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        int v;
                        var res = int.TryParse(val, out v);
                        if (res)
                        {
                            motionGroup.Alternate2UpdateMultiplier = v;
                        }
                    }
                }
            }
        }

        private void ParseAXIS_VIRTUALAttr(string[] attributes, Tag newTag, CIPAxis axis)
        {
            var axisVirtual = newTag.DataWrapper as AxisVirtual;
            foreach (var attribute in attributes)
            {
                var str = attribute;
                if (str.StartsWith("\""))
                    str = str.Substring(1);
                if (str.EndsWith("\""))
                    str = str.Remove(0, str.Length - 1);
                var attr = str.Split(new[] { ":=" }, StringSplitOptions.None);
                var val = GetAttributeValue(str);
                if (string.IsNullOrEmpty(val)) continue;
                var attrName = attr[0].Trim();
                if ("MotionGroup".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    var mg = newTag.ParentController.Tags[val];
                    if (mg != null)
                    {
                        ((AxisVirtual)newTag.DataWrapper).AssignedGroup = mg;
                    }
                    continue;
                }
                if ("ExternalAccess".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    ExternalAccess externalAccess;
                    var res = GetEnum(val, out externalAccess);
                    if (res)
                    {
                        newTag.ExternalAccess = (ExternalAccess)externalAccess;
                    }

                    continue;
                }

                if ("RotaryAxis".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    AxisVirtualParameters.PositioningMode v;
                    var res = AxisVirtualParameters.PositioningMode.TryParse(val, out v);
                    if (res)
                    {
                        axisVirtual.RotaryAxis = (byte)v;
                    }

                    continue;
                }

                if ("InterpolatedPositionConfiguration".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    uint v;
                    var res = ConvertInteger(val, out v);
                    if (res)
                    {
                        axis.GainTuningConfigurationBits = (CipUint)v;
                    }

                    continue;
                }

                var unknownList = new List<string>()
                {
                    "CurrentLoopBandwidthScalingFactor", "CIPAxisExceptionActionRA", "CIPAxisExceptionAction",
                    "MotionModule", "MotionExceptionAction", "CurrentLoopBandwidth", "DriveRatedVoltage",
                    "MaxOutputFrequency"
                };
                if (unknownList.Exists(e => e.Equals(attrName, StringComparison.OrdinalIgnoreCase)))
                {
                    //TODO(zyl);add attr
                    continue;
                }

                var enumList = new List<string>()
                {
                    "AxisUpdateSchedule",
                    "ActuatorType", "TravelMode", "MotionPolarity", "TuningSelect", "TuningDirection",
                    "ApplicationType", "LoopResponse", "ActuatorLeadUnit", "ActuatorDiameterUnit",
                    "HookupTestFeedbackChannel", "LoadCoupling",
                    "LoadType", "ScalingSource", "StoppingAction", "Feedback1Unit", "MotorUnit", "ProgrammedStopMode",
                    "HomeSequence", "HomeDirection", "HomeMode", "MotionScalingConfiguration", "MotorType",
                    "Feedback1Type", "MotorDataSource", "FeedbackConfiguration", "AxisConfiguration",
                };
                var boolList = new List<string>() { "SoftTravelLimitChecking" };
                var type1 = axis.GetType().GetProperty(attrName);
                Debug.Assert(type1 != null, attrName);
                if (boolList.Exists(e => e.Equals(attrName, StringComparison.OrdinalIgnoreCase)))
                {
                    bool b;
                    var res = bool.TryParse(val, out b);
                    if (res)
                    {
                        type1.SetValue(axis, (CipUsint)(b ? 1 : 0));
                    }

                    continue;
                }

                if (enumList.Exists(e => e.Equals(attrName, StringComparison.OrdinalIgnoreCase)))
                {
                    var enumName = attrName;
                    if (enumName.Equals("Feedback1Unit", StringComparison.OrdinalIgnoreCase))
                    {
                        enumName = "MotorUnitType";
                    }
                    else if (enumName.Equals("Feedback1Type", StringComparison.OrdinalIgnoreCase))
                    {
                        enumName = "FeedbackType";
                    }

                    if (!enumName.EndsWith("Type", StringComparison.OrdinalIgnoreCase))
                    {
                        enumName = $"{enumName}Type";
                    }

                    enumName = $"ICSStudio.Cip.Objects.{enumName}";
                    var assembly = Assembly.Load("ICSStudio.Cip");
                    var type = assembly.GetType(enumName);
                    Debug.Assert(type != null, $"eeeeeee:{enumName}");
                    byte code;
                    var res = GetEnum(val, out code, type);
                    if (res)
                    {
                        type1.SetValue(axis, (CipUsint)code);
                    }

                    continue;
                }

                AxisCommonParse(type1, axis, val);
            }
        }

        private void ParseAXIS_CIP_DRIVEAttr(string[] attributes, Tag newTag, CIPAxis axis)
        {
            foreach (var attribute in attributes)
            {
                var str = attribute;
                if (str.StartsWith("\""))
                    str = str.Substring(1);
                if (str.EndsWith("\""))
                    str = str.Remove(0, str.Length - 1);
                var attr = str.Split(new[] { ":=" }, StringSplitOptions.None);
                var val = GetAttributeValue(str);
                if (string.IsNullOrEmpty(val)) continue;
                var attrName = attr[0].Trim();
                if ("MotionGroup".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    var mg = newTag.ParentController.Tags[val];
                    if (mg != null)
                    {
                        ((AxisCIPDrive)newTag.DataWrapper).AssignedGroup = mg;
                    }
                    continue;
                }
                if ("ExternalAccess".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    ExternalAccess externalAccess;
                    var res = GetEnum(val, out externalAccess);
                    if (res)
                    {
                        newTag.ExternalAccess = (ExternalAccess)externalAccess;
                    }

                    continue;
                }
                if ("RotaryAxis".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    AxisVirtualParameters.PositioningMode v;
                    var res = AxisVirtualParameters.PositioningMode.TryParse(val, out v);
                    if (res)
                    {
                        axis.RotaryMotorInertia = (byte)v;
                    }

                    continue;
                }
                if ("HomeConfigurationBits".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    uint v;
                    var res = ConvertInteger(val, out v);
                    if (res)
                    {
                        axis.HomeConfigurationBits = v;
                    }

                    continue;
                }

                if ("GainTuningConfigurationBits".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    uint v;
                    var res = ConvertInteger(val, out v);
                    if (res)
                    {
                        axis.GainTuningConfigurationBits = (CipUint)v;
                    }

                    continue;
                }

                if ("InterpolatedPositionConfiguration".Equals(attrName, StringComparison.OrdinalIgnoreCase))
                {
                    uint v;
                    var res = ConvertInteger(val, out v);
                    if (res)
                    {
                        axis.GainTuningConfigurationBits = (CipUint)v;
                    }

                    continue;
                }

                var unknownList = new List<string>()
                {
                    "CurrentLoopBandwidthScalingFactor", "CIPAxisExceptionActionRA", "CIPAxisExceptionAction",
                    "MotionModule", "MotionExceptionAction", "CurrentLoopBandwidth", "DriveRatedVoltage",
                    "MaxOutputFrequency"
                };
                if (unknownList.Exists(e => e.Equals(attrName, StringComparison.OrdinalIgnoreCase)))
                {
                    //TODO(zyl);add attr
                    continue;
                }

                var enumList = new List<string>()
                {
                    "AxisUpdateSchedule",
                    "ActuatorType", "TravelMode", "MotionPolarity", "TuningSelect", "TuningDirection",
                    "ApplicationType", "LoopResponse", "ActuatorLeadUnit", "ActuatorDiameterUnit",
                    "HookupTestFeedbackChannel", "LoadCoupling",
                    "LoadType", "ScalingSource", "StoppingAction", "Feedback1Unit", "MotorUnit", "ProgrammedStopMode",
                    "HomeSequence", "HomeDirection", "HomeMode", "MotionScalingConfiguration", "MotorType",
                    "Feedback1Type", "MotorDataSource", "FeedbackConfiguration", "AxisConfiguration",
                };
                var boolList = new List<string>() { "SoftTravelLimitChecking" };
                var type1 = axis.GetType().GetProperty(attrName);
                if(type1 == null) continue; //attrName == Usage 时 type1 == null
                Debug.Assert(type1 != null, attrName);
                if (boolList.Exists(e => e.Equals(attrName, StringComparison.OrdinalIgnoreCase)))
                {
                    bool b;
                    var res = bool.TryParse(val, out b);
                    if (res)
                    {
                        type1.SetValue(axis, (CipUsint)(b ? 1 : 0));
                    }

                    continue;
                }

                if (enumList.Exists(e => e.Equals(attrName, StringComparison.OrdinalIgnoreCase)))
                {
                    var enumName = attrName;
                    if (enumName.Equals("Feedback1Unit", StringComparison.OrdinalIgnoreCase))
                    {
                        enumName = "MotorUnitType";
                    }
                    else if (enumName.Equals("Feedback1Type", StringComparison.OrdinalIgnoreCase))
                    {
                        enumName = "FeedbackType";
                    }

                    if (!enumName.EndsWith("Type", StringComparison.OrdinalIgnoreCase))
                    {
                        enumName = $"{enumName}Type";
                    }

                    enumName = $"ICSStudio.Cip.Objects.{enumName}";
                    var assembly = Assembly.Load("ICSStudio.Cip");
                    var type = assembly.GetType(enumName);
                    Debug.Assert(type != null, $"eeeeeee:{enumName}");
                    byte code;
                    var res = GetEnum(val, out code, type);
                    if (res)
                    {
                        type1.SetValue(axis, (CipUsint)code);
                    }

                    continue;
                }

                AxisCommonParse(type1, axis, val);
            }
        }

        private void AxisCommonParse(PropertyInfo type1, CIPAxis axis, string val)
        {
            if (type1.PropertyType == typeof(CipString))
            {
                type1.SetValue(axis, (CipString)val);
                return;
            }

            if (type1.PropertyType == typeof(CipShortString))
            {
                type1.SetValue(axis, (CipShortString)val);
                return;
            }

            if (type1.PropertyType == typeof(CipUsint))
            {
                byte v;
                var res = byte.TryParse(val, out v);
                if (res)
                {
                    type1.SetValue(axis, (CipUsint)v);
                }

                return;
            }

            if (type1.PropertyType == typeof(CipUdint))
            {
                uint v;
                var res = uint.TryParse(val, out v);
                if (res)
                {
                    type1.SetValue(axis, (CipUdint)v);
                }

                return;
            }

            if (type1.PropertyType == typeof(CipReal))
            {
                float v;
                var res = float.TryParse(val, out v);
                if (res)
                {
                    type1.SetValue(axis, (CipReal)v);
                }

                return;
            }
        }

        private bool GetEnum(string val, out byte res, Type type)
        {
            try
            {
                res = (byte)Enum.Parse(type, val);
                return true;
            }
            catch (Exception)
            {
                var fields = Enum.GetValues(type);
                foreach (var field in fields)
                {
                    var e = Attribute.GetCustomAttribute(field.GetType().GetField(field.ToString()),
                        typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                    if (e != null && e.Value.Equals(val, StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            res = (byte)field;
                        }
                        catch (Exception)
                        {
                            Debug.Assert(false, val);
                            res = 0;
                            return false;
                        }

                        return true;
                    }
                }
            }

            res = 0;
            return false;
        }

        private bool GetEnum<T>(string val, out T res)
        {
            try
            {
                res = (T)Enum.Parse(typeof(T), val);
                return true;
            }
            catch (Exception)
            {
                var fields = Enum.GetValues(typeof(T));
                foreach (var field in fields)
                {
                    var e = Attribute.GetCustomAttribute(field.GetType().GetField(field.ToString()),
                        typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                    if (e != null && e.Value.Equals(val, StringComparison.OrdinalIgnoreCase))
                    {
                        res = (T)field;
                        return true;
                    }
                }
            }

            res = res = default(T);
            return false;
        }

        private bool ConvertInteger(string val, out uint result)
        {
            try
            {
                if (val.StartsWith("16#"))
                {
                    result = Convert.ToUInt32(val.Replace("16#", "").Replace("_", ""), 16);
                    return true;

                }
                else if (val.StartsWith("8#"))
                {
                    result = Convert.ToUInt32(val.Replace("8#", "").Replace("_", ""), 8);
                    return true;
                }
                else if (val.StartsWith("2#"))
                {
                    result = Convert.ToUInt32(val.Replace("2#", "").Replace("_", ""), 2);
                    return true;
                }
                else
                {
                    result = Convert.ToUInt32(val);
                    return true;
                }
            }
            catch (Exception)
            {
                result = 0;
                return false;
            }
        }

        private string GetAttributeValue(string attr)
        {
            var index = attr.IndexOf(":=");
            if (index > 0)
            {
                return attr.Substring(index + 2).Trim();
            }

            return null;
        }

        #endregion
    }
}
