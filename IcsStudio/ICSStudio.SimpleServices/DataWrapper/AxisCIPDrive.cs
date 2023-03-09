using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using AutoMapper;
using ICSStudio.Cip.DataTypes;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Exceptions;
using ICSStudio.Database.Database;
using ICSStudio.Database.Table.Motion;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.SimpleServices.DataWrapper
{
    public class AssociatedChannelChangedEventArgs : EventArgs
    {
        public IDeviceModule OldModule { get; set; }
        public int OldNumber { get; set; }
        public IDeviceModule NewModule { get; set; }
        public int NewNumber { get; set; }
    }

    public delegate void AssociatedChannelChangedEventHandler(
        object sender, AssociatedChannelChangedEventArgs e);

    public class AxisCIPDrive : DataWrapper, ICloneable
    {
        private static readonly MapperConfiguration MapperConfig;

        private AxisCIPDriveParameters _parameters;

        private CIPAxis _cipAxis;

        private List<string> _cyclicReadUpdateList;
        private List<string> _cyclicWriteUpdateList;

        private ITag _assignedGroup;

        public static AxisCIPDrive Create(IDataType dataType, IController controller)
        {
            if (dataType == null
                || !dataType.Name.Equals("AXIS_CIP_DRIVE", StringComparison.OrdinalIgnoreCase))
                return null;

            AxisCIPDrive axisCIPDrive = new AxisCIPDrive(dataType, controller);

            // create axis id
            axisCIPDrive.CIPAxis.AxisID = controller.CreateAxisID();

            return axisCIPDrive;
        }

        static AxisCIPDrive()
        {
            MapperConfig = new MapperConfiguration(cfg =>
            {
                // cip type
                cfg.CreateMap<bool, CipUsint>().ConstructUsing(s => s ? (CipUsint)1 : (CipUsint)0);
                cfg.CreateMap<byte, CipUsint>().ConstructUsing(s => (CipUsint)s);
                cfg.CreateMap<float, CipReal>().ConstructUsing(s => (CipReal)s);
                cfg.CreateMap<uint, CipUdint>().ConstructUsing(s => (CipUdint)s);
                cfg.CreateMap<ushort, CipUint>().ConstructUsing(s => (CipUint)s);
                cfg.CreateMap<short, CipInt>().ConstructUsing(s => (CipInt)s);
                cfg.CreateMap<int, CipDint>().ConstructUsing(s => (CipDint)s);

                cfg.CreateMap<CipUsint, bool>().ConstructUsing(s => Convert.ToByte(s) != 0);
                cfg.CreateMap<CipUsint, byte>().ConstructUsing(s => Convert.ToByte(s));
                cfg.CreateMap<CipReal, float>().ConstructUsing(s => Convert.ToSingle(s));
                cfg.CreateMap<CipUdint, uint>().ConstructUsing(s => Convert.ToUInt32(s));
                cfg.CreateMap<CipUint, ushort>().ConstructUsing(s => Convert.ToUInt16(s));
                cfg.CreateMap<CipInt, short>().ConstructUsing(s => Convert.ToInt16(s));
                cfg.CreateMap<CipDint, int>().ConstructUsing(s => Convert.ToInt32(s));

                //TODO(gjc): wrong for string convert
                // 目标中的引用类型不为null，则不转换吗？
                //cfg.CreateMap<CipString, string>().ConstructUsing(
                //    s => s.GetString());
                //cfg.CreateMap<CipShortString, string>().ConstructUsing(
                //    s => s.GetString());

                //
                cfg.CreateMap<AxisCIPDriveParameters, CIPAxis>()
                    .ForMember(d => d.PMMotorFluxSaturation, opt => opt.Ignore())
                    .ForMember(d => d.CIPAxisExceptionAction, opt => opt.Ignore())
                    .ForMember(d => d.CIPAxisExceptionActionMfg, opt => opt.Ignore())
                    //.ForMember(d => d.CIPAxisExceptionActionRA, opt => opt.Ignore())
                    .ForMember(d => d.MotionExceptionAction, opt => opt.Ignore());

                cfg.CreateMap<CIPAxis, AxisCIPDriveParameters>()
                    .ForMember(d => d.PMMotorFluxSaturation, opt => opt.Ignore())
                    .ForMember(d => d.CIPAxisExceptionAction, opt => opt.Ignore())
                    .ForMember(d => d.CIPAxisExceptionActionMfg, opt => opt.Ignore())
                    //.ForMember(d => d.CIPAxisExceptionActionRA, opt => opt.Ignore())
                    .ForMember(d => d.MotionExceptionAction, opt => opt.Ignore());
            });
        }

        private AxisCIPDrive(IDataType dataType, IController controller) : base(dataType, 0, 0, 0, null)
        {
            Controller = controller;

            _cipAxis = new CIPAxis(0, null);

            _parameters = new AxisCIPDriveParameters() { AxisCIPDrive = this };
        }

        public IController Controller { get; }

        public ITag AssignedGroup
        {
            get { return _assignedGroup; }
            set
            {
                if (_assignedGroup != value)
                {
                    _assignedGroup = value;
                    NotifyParentPropertyChanged();
                }
            }
        }

        public IDeviceModule AssociatedModule { get; private set; }

        public int AxisNumber { get; private set; }

        public event AssociatedChannelChangedEventHandler AssociatedChannelChanged;

        public void UpdateAxisChannel(
            IDeviceModule deviceModule, int axisNumber)
        {
            IDeviceModule oldModule = AssociatedModule;
            int oldNumber = AxisNumber;
            IDeviceModule newModule = deviceModule;
            int newNumber = axisNumber;

            var propertyNames = new List<string>();

            if (AssociatedModule != deviceModule)
            {
                AssociatedModule = deviceModule;
                propertyNames.Add("AssociatedModule");
            }

            if (AxisNumber != axisNumber)
            {
                AxisNumber = axisNumber;
                propertyNames.Add("AxisNumber");
            }

            if (propertyNames.Count > 0)
            {
                LoadDefaultDrivePropertiesValue();

                NotifyParentPropertyChanged(propertyNames.ToArray());
                AssociatedChannelChanged?.Invoke(this, new AssociatedChannelChangedEventArgs()
                {
                    OldModule = oldModule,
                    OldNumber = oldNumber,
                    NewModule = newModule,
                    NewNumber = newNumber
                });
            }

        }

        public void UpdateRegistrationInputs()
        {
            var cipMotionDrive = AssociatedModule as CIPMotionDrive;
            if (cipMotionDrive == null)
                return;

            var feedback1Type = (FeedbackType)Convert.ToByte(CIPAxis.Feedback1Type);
            if (feedback1Type == FeedbackType.NotSpecified)
                return;

            int motorFeedbackPort = cipMotionDrive.ConfigData.FeedbackPortSelect[(AxisNumber - 1) * 4];
            var feedbackDevice = cipMotionDrive.Profiles.Schema.Feedback.GetDeviceByPortNumber(motorFeedbackPort);
            if (feedbackDevice != null)
                CIPAxis.RegistrationInputs = (byte)feedbackDevice.RegistrationInputs;
        }

        public CIPAxis CIPAxis => _cipAxis;

        public List<string> CyclicReadUpdateList
        {
            get { return _cyclicReadUpdateList; }
            set
            {
                if (_cyclicReadUpdateList != value)
                {
                    _cyclicReadUpdateList = value;

                    NotifyParentPropertyChanged();
                }
            }
        }

        public List<string> CyclicWriteUpdateList
        {
            get { return _cyclicWriteUpdateList; }
            set
            {
                if (_cyclicWriteUpdateList != value)
                {
                    _cyclicWriteUpdateList = value;

                    NotifyParentPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 用于外部调用
        /// </summary>
        public AxisCIPDriveParameters Parameters
        {
            get
            {
                if (_cipAxis != null)
                    CIPAxisToParameters();

                _parameters.MotionGroup = AssignedGroup?.Name;

                _parameters.MotionModule =
                    AssociatedModule != null ? $"{AssociatedModule.Name}:Ch{AxisNumber}" : "<NA>";

                return _parameters;
            }
            set
            {
                _parameters = value;
                if (_parameters != null)
                    _parameters.AxisCIPDrive = this;
            }
        }

        /// <summary>
        /// 导入时添加一些验证
        /// </summary>
        /// <returns></returns>
        public bool Pass(ITag target, bool ignoreAxisPass)
        {
            if (_parameters.MotionModule.Equals("<NA>"))
            {
                return true;
            }
            else
            {
                var results = _parameters.MotionModule.Split(':');
                AssociatedModule = Controller.DeviceModules[results[0]];
                int axisNumber = int.Parse(results[1].Substring(2));
                var cipMotionDrive = AssociatedModule as CIPMotionDrive;
                if (cipMotionDrive == null)
                {
                    if (ignoreAxisPass)
                    {
                        _parameters.MotionModule = "<NA>";
                        return true;
                    }

                    return false;
                }

                var tag = cipMotionDrive.GetAxis(axisNumber);
                if (tag != null)
                {
                    if (tag == target) return true;
                    if (ignoreAxisPass)
                    {
                        _parameters.MotionModule = "<NA>";
                        return true;
                    }

                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void PostLoadJson()
        {
            ParametersToCIPAxis();

            AssignedGroup = Controller.Tags[_parameters.MotionGroup];

            if (_parameters.MotionModule.Equals("<NA>"))
            {
                AssociatedModule = null;
                AxisNumber = 0;
            }
            else
            {
                var results = _parameters.MotionModule.Split(':');
                AssociatedModule = Controller.DeviceModules[results[0]];
                int axisNumber = int.Parse(results[1].Substring(2));
                var cipMotionDrive = AssociatedModule as CIPMotionDrive;

                Contract.Assert(cipMotionDrive != null);
                if (cipMotionDrive.AddAxis(ParentTag, axisNumber) == 0)
                {
                    AxisNumber = axisNumber;
                    LoadDefaultDrivePropertiesValue();
                }
                else
                {
                    throw new LoadJsonException($"add axis number:{axisNumber} failed!");
                }
            }
        }

        public void CIPAxisToTagMembers(ITag tag)
        {
            string[] attributes =
            {
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

            //sync data from CIPAxis to tag member
            foreach (var attribute in attributes)
            {
                var propertyInfo = typeof(CIPAxis).GetProperty(attribute);
                object value = propertyInfo?.GetValue(_cipAxis);
                if (value != null)
                {
                    tag.SetStringValue(attribute, value.ToString());
                }

            }

        }

        public void TagMembersToCIPAxis(ITag tag)
        {
            var attributes = new List<string>
            {
                "CIPAxisState",

                "AxisFault,AxisFaultBits",
                "CIPAxisFaults",
                "CIPAxisFaultsRA",
                "ModuleFaults,ModuleFaultBits",
                "MotionFaultStatus,MotionFaultBits",
                "CIPInitializationFaults",
                "CIPAPRFaults",
                "GuardFaults",
                "CIPStartInhibits",

                "ActualPosition",
                "CommandPosition",
                "ActualVelocity",
                "CommandVelocity",
                "CIPAxisStatus",
                "CIPAxisIOStatus",
                "CIPAxisIOStatusRA",

            };

            var cycleReadList = new List<string>
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
            attributes.AddRange(cycleReadList);

            //var cycleWriteList = new List<string>
            //{
            //    "PositionTrim",
            //    "VelocityTrim",
            //    "TorqueTrim",
            //    "VelocityFeedforwardGain",
            //    "AccelerationFeedforwardGain",
            //    "PositionLoopBandwidth",
            //    "PositionIntegratorBandwidth",
            //    "VelocityLoopBandwidth",
            //    "VelocityIntegratorBandwidth",
            //    "LoadObserverBandwidth",
            //    "LoadObserverIntegratorBandwidth",
            //    "TorqueLimitPositive",
            //    "TorqueLimitNegative",
            //    "VelocityLowPassFilterBandwidth",
            //    "TorqueLowPassFilterBandwidth",
            //    "SystemInertia"
            //};
            //attributes.AddRange(cycleWriteList);

            //sync data from tag member to CIPAxis
            foreach (var attribute in attributes)
            {
                string tagMember;
                string propertyName;

                if (attribute.Contains(","))
                {
                    var result = attribute.Split(',');
                    tagMember = result[0].Trim();
                    propertyName = result[1].Trim();
                }
                else
                {
                    tagMember = attribute;
                    propertyName = attribute;
                }

                string value = tag.GetMemberValue(tagMember, true);
                var propertyInfo = typeof(CIPAxis).GetProperty(propertyName);

                if (propertyInfo != null)
                {
                    if (propertyInfo.PropertyType == typeof(CipUsint))
                        SetPropertyInfo(propertyInfo, (CipUsint)value);
                    else if (propertyInfo.PropertyType == typeof(CipUdint))
                        SetPropertyInfo(propertyInfo, (CipUdint)value);
                    else if (propertyInfo.PropertyType == typeof(CipUlint))
                        SetPropertyInfo(propertyInfo, (CipUlint)value);
                    else if (propertyInfo.PropertyType == typeof(CipDint))
                        SetPropertyInfo(propertyInfo, (CipDint)value);
                    else if (propertyInfo.PropertyType == typeof(CipReal))
                    {
                        var floatValue = ValueConverter.ToFloat(value);
                        SetPropertyInfo(propertyInfo, (CipReal)floatValue);
                    }
                    else if (propertyInfo.PropertyType == typeof(CipUint))
                        SetPropertyInfo(propertyInfo, (CipUint)value);
                    else
                        throw new NotSupportedException("add code here!");

                }
                else
                {
                    Debug.WriteLine($"Not found {propertyName} in CIPAxis.");
                }

            }

        }

        private void SetPropertyInfo<T>(PropertyInfo propertyInfo, T newValue) where T : IComparable
        {
            T oldValue = (T)propertyInfo.GetValue(_cipAxis);
            if (oldValue.CompareTo(newValue) != 0)
            {
                propertyInfo.SetValue(_cipAxis, newValue);

                NotifyParentPropertyChanged(propertyInfo.Name);
            }
        }

        private void ParametersToCIPAxis()
        {
            try
            {
                var mapper = MapperConfig.CreateMapper();
                mapper.Map(_parameters, _cipAxis);

                _cipAxis.MotorCatalogNumber =
                    string.IsNullOrEmpty(_parameters.MotorCatalogNumber)
                        ? "<none>"
                        : _parameters.MotorCatalogNumber;

                _cipAxis.PositionUnits =
                    string.IsNullOrEmpty(_parameters.PositionUnits)
                        ? "Position Units"
                        : _parameters.PositionUnits;

                if (_parameters.PMMotorFluxSaturation != null)
                {
                    for (int i = 0; i < _parameters.PMMotorFluxSaturation.Count; i++)
                        _cipAxis.PMMotorFluxSaturation.SetValue(i, _parameters.PMMotorFluxSaturation[i]);
                }

                if (_parameters.CIPAxisExceptionAction != null)
                {
                    for (int i = 0; i < _parameters.CIPAxisExceptionAction.Count; i++)
                        _cipAxis.CIPAxisExceptionAction.SetValue(i, _parameters.CIPAxisExceptionAction[i]);
                }

                if (_parameters.CIPAxisExceptionActionMfg != null)
                {
                    for (int i = 0; i < _parameters.CIPAxisExceptionActionMfg.Count; i++)
                        _cipAxis.CIPAxisExceptionActionMfg.SetValue(i, _parameters.CIPAxisExceptionActionMfg[i]);
                }

                //if (_parameters.CIPAxisExceptionActionRA != null)
                //{
                //    for (int i = 0; i < _parameters.CIPAxisExceptionActionRA.Count; i++)
                //        _cipAxis.CIPAxisExceptionActionRA.SetValue(i, _parameters.CIPAxisExceptionActionRA[i]);
                //}

                if (_parameters.MotionExceptionAction != null)
                {
                    for (int i = 0; i < _parameters.MotionExceptionAction.Count; i++)
                        _cipAxis.MotionExceptionAction.SetValue(i, _parameters.MotionExceptionAction[i]);
                }

                if (_parameters.CyclicReadUpdateList != null)
                    CyclicReadUpdateList = new List<string>(_parameters.CyclicReadUpdateList);
                if (_parameters.CyclicWriteUpdateList != null)
                    CyclicWriteUpdateList = new List<string>(_parameters.CyclicWriteUpdateList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void CIPAxisToParameters()
        {
            try
            {
                var mapper = MapperConfig.CreateMapper();
                mapper.Map(_cipAxis, _parameters);

                _parameters.MotorCatalogNumber = _cipAxis.MotorCatalogNumber.GetString();
                _parameters.PositionUnits = _cipAxis.PositionUnits.GetString();

                _parameters.PMMotorFluxSaturation = new List<float>();
                for (int i = 0; i < _cipAxis.PMMotorFluxSaturation.GetCount(); i++)
                    _parameters.PMMotorFluxSaturation.Add(_cipAxis.PMMotorFluxSaturation.GetValue(i));

                _parameters.CIPAxisExceptionAction = new List<byte>();
                for (int i = 0; i < _cipAxis.CIPAxisExceptionAction.GetCount(); i++)
                    _parameters.CIPAxisExceptionAction.Add(_cipAxis.CIPAxisExceptionAction.GetValue(i));

                _parameters.CIPAxisExceptionActionMfg = new List<byte>();
                for (int i = 0; i < _cipAxis.CIPAxisExceptionActionMfg.GetCount(); i++)
                    _parameters.CIPAxisExceptionActionMfg.Add(_cipAxis.CIPAxisExceptionActionMfg.GetValue(i));

                //_parameters.CIPAxisExceptionActionRA = new List<byte>();
                //for (int i = 0; i < _cipAxis.CIPAxisExceptionActionRA.GetCount(); i++)
                //    _parameters.CIPAxisExceptionActionRA.Add(_cipAxis.CIPAxisExceptionActionRA.GetValue(i));

                _parameters.MotionExceptionAction = new List<byte>();
                for (int i = 0; i < _cipAxis.MotionExceptionAction.GetCount(); i++)
                    _parameters.MotionExceptionAction.Add(_cipAxis.MotionExceptionAction.GetValue(i));

                if (CyclicReadUpdateList != null)
                    _parameters.CyclicReadUpdateList = new List<string>(CyclicReadUpdateList);
                if (CyclicWriteUpdateList != null)
                    _parameters.CyclicWriteUpdateList = new List<string>(CyclicWriteUpdateList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public object Clone()
        {
            var clone = (AxisCIPDrive)MemberwiseClone();

            clone.ParentTag = null;
            clone._cipAxis = (CIPAxis)CIPAxis.Clone();

            if (CyclicReadUpdateList != null)
                clone.CyclicReadUpdateList = new List<string>(CyclicReadUpdateList);

            if (CyclicWriteUpdateList != null)
                clone.CyclicWriteUpdateList = new List<string>(CyclicWriteUpdateList);

            return clone;
        }

        private void LoadDefaultDrivePropertiesValue()
        {
            var cipMotionDrive = AssociatedModule as CIPMotionDrive;
            if (cipMotionDrive == null)
                return;

            var motionDbHelper = new MotionDbHelper();

            bool isSupportConverterACInputVoltage =
                cipMotionDrive.Profiles.SupportDriveAttribute("ConverterACInputVoltage", cipMotionDrive.Major);
            bool isSupportConverterACInputPhasing =
                cipMotionDrive.Profiles.SupportDriveAttribute("ConverterACInputPhasing", cipMotionDrive.Major);

            Drive drive = null;

            if (isSupportConverterACInputVoltage && isSupportConverterACInputPhasing)
            {
                drive = motionDbHelper.GetMotionDrive(
                    cipMotionDrive.CatalogNumber,
                    cipMotionDrive.ConfigData.ConverterACInputVoltage,
                    cipMotionDrive.ConfigData.ConverterACInputPhasing);
            }
            else if (isSupportConverterACInputVoltage)
            {
                //TODO(gjc): add code here
            }
            else if (isSupportConverterACInputPhasing)
            {
                //TODO(gjc): add code here
            }
            else
            {
                var drives = motionDbHelper.GetMotionDrive(cipMotionDrive.CatalogNumber);
                if (drives != null && drives.Count == 1)
                {
                    drive = drives[0];
                }
            }

            if (drive != null)
            {
                CIPAxis.DriveModelTimeConstantBase = drive.BaseTimeConstant;
                CIPAxis.DriveRatedPeakCurrent = drive.PeakCurrent;
                CIPAxis.DriveMaxOutputFrequency = drive.MaxOutputFrequency;
                CIPAxis.BusOvervoltageOperationalLimit = drive.BusOvervoltageOperationalLimit;
            }
            else
            {
                CIPAxis.DriveModelTimeConstantBase = 0.0015f;
                CIPAxis.DriveRatedPeakCurrent = 0.0f;
                CIPAxis.DriveMaxOutputFrequency = 0;
                CIPAxis.BusUndervoltageUserLimit = 0.0f;
            }

        }
    }

    public static class ControllerExtensionsForAxisID
    {
        private static readonly object AxisIDLock = new object();

        private static readonly List<uint> AxisCache = new List<uint>();

        public static uint CreateAxisID(this IController controller)
        {
            lock (AxisIDLock)
            {
                List<uint> axisIDList = new List<uint>();
                foreach (var tag in controller.Tags.OfType<Tag>())
                {
                    AxisCIPDrive axisCIPDrive = tag.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null)
                    {
                        axisIDList.Add(Convert.ToUInt32(axisCIPDrive.CIPAxis.AxisID));
                    }
                }

                Random random = new Random();

                while (true)
                {
                    uint result = (uint)random.Next(int.MinValue, int.MaxValue);
                    if (result != 0 && !axisIDList.Contains(result) && !AxisCache.Contains(result))
                    {
                        AxisCache.Add(result);

                        if (AxisCache.Count > 1000)
                        {
                            AxisCache.RemoveAt(0);
                        }

                        return result;
                    }
                }
            }
        }

        public static bool ContainsAxisID(this IController controller, uint axisID)
        {
            foreach (var tag in controller.Tags.OfType<Tag>())
            {
                AxisCIPDrive axisCIPDrive = tag.DataWrapper as AxisCIPDrive;
                if (axisCIPDrive != null)
                {
                    if (Convert.ToUInt32(axisCIPDrive.CIPAxis.AxisID) == axisID)
                        return true;
                }
            }

            return false;
        }
    }
}
