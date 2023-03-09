using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using ICSStudio.DeviceProfiles.Common;
using ICSStudio.DeviceProfiles.DIOModule.Common;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils.TagExpression;

namespace ICSStudio.DeviceProperties.AnalogIOs
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ModifiedAnalogIO : ModifiedDIOModule
    {
        private Tag _configTag;

        private readonly IDataServer _dataServer;
        private readonly List<IDataOperand> _dataOperands;

        private readonly List<string> _editedList;

        private bool _isChangedExternally;

        private short[] _chLowEngineering;
        private short[] _chHighEngineering;
        private short[] _chDigitalFilter;
        private sbyte[] _chRangeType;

        private sbyte[] _chSensorType;
        private sbyte[] _chTempMode;

        private short _realTimeSample;
        private sbyte _notchFilter;

        private short[] _hhAlarmLimit;
        private short[] _hAlarmLimit;
        private short[] _lAlarmLimit;
        private short[] _llAlarmLimit;
        private sbyte[] _alarmDisable;
        private sbyte[] _limitAlarmLatch;

        private short[] _highLimit;
        private short[] _lowLimit;

        private sbyte[] _faultMode;
        private short[] _faultValue;
        private sbyte[] _progMode;
        private short[] _progValue;

        public ModifiedAnalogIO(IController controller, AnalogIO analogIO, IDataServer dataServer)
            : base(controller, analogIO)
        {
            _dataServer = dataServer;
            _dataOperands = new List<IDataOperand>();

            _editedList = new List<string>();

            OriginalAnalogIO = analogIO;

            if (OriginalAnalogIO?.Communications?.Connections != null &&
                OriginalAnalogIO.Communications.Connections.Count > 0)
            {
                RPI = OriginalAnalogIO.Communications.Connections[0].RPI;
                Unicast = OriginalAnalogIO.Communications.Connections[0].Unicast.GetValueOrDefault();
            }

            _configTag = OriginalAnalogIO?.ConfigTag;

            UpdateConfigValues(_configTag ?? CreateDefaultConfigTag(ConnectionConfigID, Major));

            if (_configTag != null)
                LinkConfigTag(_configTag);
        }

        public AnalogIO OriginalAnalogIO { get; }

        public event EventHandler ConfigValueChanged;

        public bool IsChangedExternally => _isChangedExternally;

        public void ChangeConnectionConfigID(uint newConfigID)
        {
            if (ConnectionConfigID == newConfigID)
                return;

            //_editedList.Clear();

            ConnectionConfigID = newConfigID;

            var definition =
                Profiles.DIOModuleTypes.GetConnectionConfigDefinitionByID(ConnectionConfigID);

            var connectionDefinition =
                Profiles.DIOModuleTypes.GetConnectionDefinitionByID(definition.Connections[0]);

            if (connectionDefinition != null)
            {
                RPI = connectionDefinition.RPI.GetValueOrDefault();
                Unicast = true;
            }

            if (_configTag != null)
            {
                UnlinkConfigTag(_configTag);
                _configTag = null;
            }

            UpdateConfigValues(CreateDefaultConfigTag(newConfigID, Major));
        }

        public void PostApply()
        {
            _editedList.Clear();
            _isChangedExternally = false;

            if (_configTag != null)
            {
                UnlinkConfigTag(_configTag);
            }

            _configTag = OriginalAnalogIO.ConfigTag;

            if (_configTag != null)
            {
                LinkConfigTag(_configTag);
            }
        }

        private Tag CreateDefaultConfigTag(uint connectionConfigID, int major)
        {
            // DataTypeDefinition
            var connectionConfigDefinition =
                Profiles.DIOModuleTypes.GetConnectionConfigDefinitionByID(connectionConfigID);

            DataTypeDefinition dataTypeDefinition = null;

            if (connectionConfigDefinition?.ConfigTag != null)
                dataTypeDefinition = Profiles.DIOModuleTypes.GetDataTypeDefinitionByName(
                    connectionConfigDefinition.ConfigTag.DataType);

            var dataTypeJObject = dataTypeDefinition?.ToJObject();
            if (dataTypeJObject == null)
                return null;

            if (Profiles
                .GetConnectionStringByConfigID(connectionConfigID, major)
                .Contains("Listen Only"))
                return null;

            var dataType = new ModuleDefinedDataType(dataTypeJObject);


            // special use here!
            Tag tempTag = new Tag(null)
            {
                DataWrapper = new DataWrapper(dataType, 0, 0, 0, null)
            };

            var dataValue = Profiles.DIOModuleTypes
                .GetDataValueByID(connectionConfigDefinition.ConfigTag.ValueID).ToArray();

            tempTag.DataWrapper.Data.Update(dataValue, 0);

            return tempTag;
        }

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
        private void UpdateConfigValues(Tag configTag)
        {
            if (configTag == null)
                return;

            // if4, of4, ir4
            var module = Profiles.GetModule(Major);
            if (module == null)
                return;

            switch (Profiles.CatalogNumber)
            {
                case "ICD-IF4":
                    UpdateIF4ConfigValues(configTag, module);
                    break;

                case "ICD-OF4":
                    UpdateOF4ConfigValues(configTag, module);
                    break;

                case "ICD-IR4":
                    UpdateIR4ConfigValues(configTag, module);
                    break;

                default:
                    throw new NotImplementedException($"Add support for {Profiles.CatalogNumber}!");
            }
        }



        private void UpdateOF4ConfigValues(Tag configTag, IOModule module)
        {
            if (configTag == null || module == null)
                return;

            if (module.NumberOfOutputs > 0)
            {
                _chHighEngineering = new short[module.NumberOfOutputs];
                _chLowEngineering = new short[module.NumberOfOutputs];
                _chRangeType = new sbyte[module.NumberOfOutputs];

                _highLimit = new short[module.NumberOfOutputs];
                _lowLimit = new short[module.NumberOfOutputs];

                _alarmDisable = new sbyte[module.NumberOfOutputs];
                _limitAlarmLatch = new sbyte[module.NumberOfOutputs];

                _faultMode = new sbyte[module.NumberOfOutputs];
                _faultValue = new short[module.NumberOfOutputs];

                _progMode = new sbyte[module.NumberOfOutputs];
                _progValue = new short[module.NumberOfOutputs];

                for (int i = 0; i < module.NumberOfOutputs; i++)
                {
                    string stringValue = configTag.GetMemberValue($"Ch{i}LowEngineering", true);
                    _chLowEngineering[i] = TryParse<short>(stringValue, 3277);


                    //
                    stringValue = configTag.GetMemberValue($"Ch{i}HighEngineering", true);
                    _chHighEngineering[i] = TryParse<short>(stringValue, 10000);


                    //
                    stringValue = configTag.GetMemberValue($"Ch{i}RangeType", true);
                    _chRangeType[i] = TryParse<sbyte>(stringValue, 0);


                    // 
                    stringValue = configTag.GetMemberValue($"Ch{i}HighLimit", true);
                    _highLimit[i] = TryParse<short>(stringValue, 32767);


                    stringValue = configTag.GetMemberValue($"Ch{i}LowLimit", true);
                    _lowLimit[i] = TryParse<short>(stringValue, -32768);


                    //
                    stringValue = configTag.GetMemberValue($"Ch{i}AlarmDisable", true);
                    _alarmDisable[i] = TryParse<sbyte>(stringValue, 0);


                    stringValue = configTag.GetMemberValue($"Ch{i}LimitAlarmLatch", true);
                    _limitAlarmLatch[i] = TryParse<sbyte>(stringValue, 0);


                    stringValue = configTag.GetMemberValue($"Ch{i}FaultMode", true);
                    _faultMode[i] = TryParse<sbyte>(stringValue, 1);


                    stringValue = configTag.GetMemberValue($"Ch{i}FaultValue", true);
                    _faultValue[i] = TryParse<short>(stringValue, 0);


                    stringValue = configTag.GetMemberValue($"Ch{i}ProgMode", true);
                    _progMode[i] = TryParse<sbyte>(stringValue, 1);


                    stringValue = configTag.GetMemberValue($"Ch{i}ProgValue", true);
                    _progValue[i] = TryParse<short>(stringValue, 0);

                }

            }
        }

        private void UpdateIF4ConfigValues(Tag configTag, IOModule module)
        {
            if (configTag == null || module == null)
                return;

            if (module.NumberOfInputs > 0)
            {
                string stringValue;

                _chHighEngineering = new short[module.NumberOfInputs];
                _chLowEngineering = new short[module.NumberOfInputs];
                _chDigitalFilter = new short[module.NumberOfInputs];
                _chRangeType = new sbyte[module.NumberOfInputs];

                _hhAlarmLimit = new short[module.NumberOfInputs];
                _hAlarmLimit = new short[module.NumberOfInputs];
                _lAlarmLimit = new short[module.NumberOfInputs];
                _llAlarmLimit = new short[module.NumberOfInputs];

                _alarmDisable = new sbyte[module.NumberOfInputs];
                _limitAlarmLatch = new sbyte[module.NumberOfInputs];

                for (int i = 0; i < module.NumberOfInputs; i++)
                {
                    //
                    stringValue = configTag.GetMemberValue($"Ch{i}LowEngineering", true);
                    _chLowEngineering[i] = TryParse<short>(stringValue, 3277);

                    //
                    stringValue = configTag.GetMemberValue($"Ch{i}HighEngineering", true);
                    _chHighEngineering[i] = TryParse<short>(stringValue, 10000);


                    //
                    stringValue = configTag.GetMemberValue($"Ch{i}DigitalFilter", true);
                    _chDigitalFilter[i] = TryParse<short>(stringValue, 0);


                    //
                    stringValue = configTag.GetMemberValue($"Ch{i}RangeType", true);
                    _chRangeType[i] = TryParse<sbyte>(stringValue, 3);


                    // HHAlarmLimit
                    stringValue = configTag.GetMemberValue($"Ch{i}HHAlarmLimit", true);
                    _hhAlarmLimit[i] = TryParse<short>(stringValue, 16793);


                    stringValue = configTag.GetMemberValue($"Ch{i}HAlarmLimit", true);
                    _hAlarmLimit[i] = TryParse<short>(stringValue, 16547);


                    stringValue = configTag.GetMemberValue($"Ch{i}LAlarmLimit", true);
                    _lAlarmLimit[i] = TryParse<short>(stringValue, 3113);


                    stringValue = configTag.GetMemberValue($"Ch{i}LLAlarmLimit", true);
                    _llAlarmLimit[i] = TryParse<short>(stringValue, 2867);


                    //
                    stringValue = configTag.GetMemberValue($"Ch{i}AlarmDisable", true);
                    _alarmDisable[i] = TryParse<sbyte>(stringValue, 0);


                    stringValue = configTag.GetMemberValue($"Ch{i}LimitAlarmLatch", true);
                    _limitAlarmLatch[i] = TryParse<sbyte>(stringValue, 0);

                }

                stringValue = configTag.GetMemberValue("RealTimeSample", true);
                _realTimeSample = TryParse<short>(stringValue, 100);


                stringValue = configTag.GetMemberValue("NotchFilter", true);
                sbyte defaultNotchFilter = 0;
                if (module.NumberOfInputs == 2)
                {
                    defaultNotchFilter = 2;
                }
                else if (module.NumberOfInputs == 4 || module.NumberOfInputs == 8)
                {
                    defaultNotchFilter = 0;
                }

                _notchFilter = TryParse<sbyte>(stringValue, defaultNotchFilter);

            }
        }

        private void UpdateIR4ConfigValues(Tag configTag, IOModule module)
        {
            if (configTag == null || module == null)
                return;

            if (module.NumberOfInputs > 0)
            {
                //Ch0LowEngineering
                //Ch0HighEngineering
                //Ch0DigitalFilter
                //Ch0SensorType
                //Ch0TempMode

                //Ch0LAlarmLimit
                //Ch0HAlarmLimit
                //Ch0LLAlarmLimit
                //Ch0HHAlarmLimit
                //Ch0LimitAlarmLatch
                //Ch0AlarmDisable

                //NotchFilter

                string stringValue;
                _chLowEngineering = new short[module.NumberOfInputs];
                _chHighEngineering = new short[module.NumberOfInputs];
                _chDigitalFilter = new short[module.NumberOfInputs];
                _chSensorType = new sbyte[module.NumberOfInputs];
                _chTempMode = new sbyte[module.NumberOfInputs];

                _hhAlarmLimit = new short[module.NumberOfInputs];
                _hAlarmLimit = new short[module.NumberOfInputs];
                _lAlarmLimit = new short[module.NumberOfInputs];
                _llAlarmLimit = new short[module.NumberOfInputs];
                _alarmDisable = new sbyte[module.NumberOfInputs];
                _limitAlarmLatch = new sbyte[module.NumberOfInputs];

                for (int i = 0; i < module.NumberOfInputs; i++)
                {
                    stringValue = configTag.GetMemberValue($"Ch{i}LowEngineering", true);
                    _chLowEngineering[i] = TryParse<short>(stringValue, 1000);

                    //
                    stringValue = configTag.GetMemberValue($"Ch{i}HighEngineering", true);
                    _chHighEngineering[i] = TryParse<short>(stringValue, 5000);

                    //
                    stringValue = configTag.GetMemberValue($"Ch{i}DigitalFilter", true);
                    _chDigitalFilter[i] = TryParse<short>(stringValue, 0);

                    //
                    stringValue = configTag.GetMemberValue($"Ch{i}SensorType", true);
                    _chSensorType[i] = TryParse<sbyte>(stringValue, 1);

                    //
                    stringValue = configTag.GetMemberValue($"Ch{i}TempMode", true);
                    _chTempMode[i] = TryParse<sbyte>(stringValue, 1);

                    // HHAlarmLimit
                    stringValue = configTag.GetMemberValue($"Ch{i}HHAlarmLimit", true);
                    _hhAlarmLimit[i] = TryParse<short>(stringValue, 32767);


                    stringValue = configTag.GetMemberValue($"Ch{i}HAlarmLimit", true);
                    _hAlarmLimit[i] = TryParse<short>(stringValue, 32767);


                    stringValue = configTag.GetMemberValue($"Ch{i}LAlarmLimit", true);
                    _lAlarmLimit[i] = TryParse<short>(stringValue, -32768);


                    stringValue = configTag.GetMemberValue($"Ch{i}LLAlarmLimit", true);
                    _llAlarmLimit[i] = TryParse<short>(stringValue, -32768);


                    //
                    stringValue = configTag.GetMemberValue($"Ch{i}AlarmDisable", true);
                    _alarmDisable[i] = TryParse<sbyte>(stringValue, 0);


                    stringValue = configTag.GetMemberValue($"Ch{i}LimitAlarmLatch", true);
                    _limitAlarmLatch[i] = TryParse<sbyte>(stringValue, 0);
                }

                stringValue = configTag.GetMemberValue("NotchFilter", true);
                sbyte defaultNotchFilter = 1;
                _notchFilter = TryParse<sbyte>(stringValue, defaultNotchFilter);
            }
        }

        private void LinkConfigTag(Tag configTag)
        {
            if (configTag == null)
                return;

            UnlinkConfigTag(null);

            var module = Profiles.GetModule(Major);

            if (module == null)
                return;

            switch (Profiles.CatalogNumber)
            {
                case "ICD-IF4":
                    LinkIF4ConfigTag(configTag, module);
                    break;

                case "ICD-OF4":
                    LinkOF4ConfigTag(configTag, module);
                    break;

                case "ICD-IR4":
                    LinkIR4ConfigTag(configTag, module);
                    break;

                default:
                    throw new NotImplementedException($"Add support for {Profiles.CatalogNumber}!");
            }


            foreach (var dataOperand in _dataOperands)
            {
                PropertyChangedEventManager.AddHandler(dataOperand, OnDataOperandValueChanged,
                    "RawValue");
                dataOperand.StartMonitoring(true, false);
            }

        }

        private void LinkOF4ConfigTag(Tag configTag, IOModule module)
        {
            if (module != null && module.NumberOfOutputs > 0)
            {
                for (int i = 0; i < module.NumberOfOutputs; i++)
                {
                    var dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}LowEngineering");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}HighEngineering");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}RangeType");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}HighLimit");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}LowLimit");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}AlarmDisable");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}LimitAlarmLatch");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}FaultMode");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}FaultValue");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}ProgMode");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}ProgValue");
                    _dataOperands.Add(dataOperand);

                }

            }
        }

        private void LinkIF4ConfigTag(Tag configTag, IOModule module)
        {
            if (module != null && module.NumberOfInputs > 0)
            {
                IDataOperand dataOperand;
                for (int i = 0; i < module.NumberOfInputs; i++)
                {
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}LowEngineering");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}HighEngineering");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}DigitalFilter");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}RangeType");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}HHAlarmLimit");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}HAlarmLimit");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}LAlarmLimit");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}LLAlarmLimit");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}AlarmDisable");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}LimitAlarmLatch");
                    _dataOperands.Add(dataOperand);

                }

                dataOperand = _dataServer.CreateDataOperand(configTag, "RealTimeSample");
                _dataOperands.Add(dataOperand);
                dataOperand = _dataServer.CreateDataOperand(configTag, "NotchFilter");
                _dataOperands.Add(dataOperand);

            }
        }

        private void LinkIR4ConfigTag(Tag configTag, IOModule module)
        {
            if (module != null && module.NumberOfInputs > 0)
            {
                IDataOperand dataOperand;
                for (int i = 0; i < module.NumberOfInputs; i++)
                {
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}LowEngineering");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}HighEngineering");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}DigitalFilter");
                    _dataOperands.Add(dataOperand);

                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}HHAlarmLimit");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}HAlarmLimit");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}LAlarmLimit");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}LLAlarmLimit");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}AlarmDisable");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}LimitAlarmLatch");
                    _dataOperands.Add(dataOperand);

                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}SensorType");
                    _dataOperands.Add(dataOperand);
                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Ch{i}TempMode");
                    _dataOperands.Add(dataOperand);
                }

                dataOperand = _dataServer.CreateDataOperand(configTag, "NotchFilter");
                _dataOperands.Add(dataOperand);

            }
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private void UnlinkConfigTag(Tag configTag)
        {
            foreach (var dataOperand in _dataOperands)
            {
                dataOperand.StopMonitoring(true, true);
                PropertyChangedEventManager.RemoveHandler(dataOperand, OnDataOperandValueChanged, "RawValue");
            }

            _dataOperands.Clear();
        }

        private void OnDataOperandValueChanged(object sender, PropertyChangedEventArgs e)
        {
            //TODO(gjc): need edit here
            IDataOperand dataOperand = sender as IDataOperand;
            var memberAccess = dataOperand?.GetTagExpression() as MemberAccessExpression;

            if (memberAccess != null)
            {
                string member = memberAccess.Name;

                if (_editedList.Contains(member))
                {
                    _isChangedExternally = true;
                    return;
                }

                var module = Profiles.GetModule(Major);
                if (module != null && module.NumberOfInputs > 0)
                {
                    if (member == "RealTimeSample")
                    {
                        _realTimeSample = dataOperand.Int16Value;
                    }
                    else if (member == "NotchFilter")
                    {
                        _notchFilter = dataOperand.Int8Value;
                    }
                    else
                    {
                        string number = Regex.Replace(member, @"[^0-9]+", "");
                        Contract.Assert(!string.IsNullOrEmpty(number));

                        int index = int.Parse(number);
                        if (member.EndsWith("LowEngineering"))
                        {
                            _chLowEngineering[index] = dataOperand.Int16Value;
                        }
                        else if (member.EndsWith("HighEngineering"))
                        {
                            _chHighEngineering[index] = dataOperand.Int16Value;
                        }
                        else if (member.EndsWith("DigitalFilter"))
                        {
                            _chDigitalFilter[index] = dataOperand.Int16Value;
                        }
                        else if (member.EndsWith("RangeType"))
                        {
                            _chRangeType[index] = dataOperand.Int8Value;
                        }
                        else if (member.EndsWith("HHAlarmLimit"))
                        {
                            _hhAlarmLimit[index] = dataOperand.Int16Value;
                        }
                        else if (member.EndsWith("LLAlarmLimit"))
                        {
                            _llAlarmLimit[index] = dataOperand.Int16Value;
                        }
                        else if (member.EndsWith("HAlarmLimit"))
                        {
                            _hAlarmLimit[index] = dataOperand.Int16Value;
                        }
                        else if (member.EndsWith("LAlarmLimit"))
                        {
                            _lAlarmLimit[index] = dataOperand.Int16Value;
                        }
                        else if (member.EndsWith("AlarmDisable"))
                        {
                            _alarmDisable[index] = dataOperand.Int8Value;
                        }
                        else if (member.EndsWith("LimitAlarmLatch"))
                        {
                            _limitAlarmLatch[index] = dataOperand.Int8Value;
                        }
                        else if (member.EndsWith("SensorType"))
                        {
                            _chSensorType[index] = dataOperand.Int8Value;
                        }
                        else if (member.EndsWith("TempMode"))
                        {
                            _chTempMode[index] = dataOperand.Int8Value;
                        }
                    }

                }

                if (module != null && module.NumberOfOutputs > 0)
                {
                    string number = Regex.Replace(member, @"[^0-9]+", "");
                    Contract.Assert(!string.IsNullOrEmpty(number));

                    int index = int.Parse(number);

                    if (member.EndsWith("LowEngineering"))
                    {
                        _chLowEngineering[index] = dataOperand.Int16Value;
                    }
                    else if (member.EndsWith("HighEngineering"))
                    {
                        _chHighEngineering[index] = dataOperand.Int16Value;
                    }
                    else if (member.EndsWith("RangeType"))
                    {
                        _chRangeType[index] = dataOperand.Int8Value;
                    }
                    else if (member.EndsWith("HighLimit"))
                    {
                        _highLimit[index] = dataOperand.Int16Value;
                    }
                    else if (member.EndsWith("LowLimit"))
                    {
                        _lowLimit[index] = dataOperand.Int16Value;
                    }
                    else if (member.EndsWith("AlarmDisable"))
                    {
                        _alarmDisable[index] = dataOperand.Int8Value;
                    }
                    else if (member.EndsWith("LimitAlarmLatch"))
                    {
                        _limitAlarmLatch[index] = dataOperand.Int8Value;
                    }
                    else if (member.EndsWith("FaultMode"))
                    {
                        _faultMode[index] = dataOperand.Int8Value;
                    }
                    else if (member.EndsWith("FaultValue"))
                    {
                        _faultValue[index] = dataOperand.Int16Value;
                    }
                    else if (member.EndsWith("ProgMode"))
                    {
                        _progMode[index] = dataOperand.Int8Value;
                    }
                    else if (member.EndsWith("ProgValue"))
                    {
                        _progValue[index] = dataOperand.Int16Value;
                    }
                }

                ConfigValueChanged?.Invoke(this, EventArgs.Empty);
            }

        }

        public short GetHighEngineering(int index)
        {
            Contract.Assert(_chHighEngineering != null);
            Contract.Assert(index < _chHighEngineering.Length);

            return _chHighEngineering[index];
        }

        public short GetLowEngineering(int index)
        {
            Contract.Assert(_chLowEngineering != null);
            Contract.Assert(index < _chLowEngineering.Length);

            return _chLowEngineering[index];
        }

        public short GetDigitalFilter(int index)
        {
            Contract.Assert(_chDigitalFilter != null);
            Contract.Assert(index < _chDigitalFilter.Length);

            return _chDigitalFilter[index];
        }

        public sbyte GetRangeType(int index)
        {
            Contract.Assert(_chRangeType != null);
            Contract.Assert(index < _chRangeType.Length);

            return _chRangeType[index];
        }

        public void SetHighEngineering(int index, short value)
        {
            Contract.Assert(_chHighEngineering != null);
            Contract.Assert(index < _chHighEngineering.Length);

            _chHighEngineering[index] = value;

            string propertyName = $"Ch{index}HighEngineering";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);

        }

        public void SetLowEngineering(int index, short value)
        {
            Contract.Assert(_chLowEngineering != null);
            Contract.Assert(index < _chLowEngineering.Length);

            _chLowEngineering[index] = value;

            string propertyName = $"Ch{index}LowEngineering";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public void SetDigitalFilter(int index, short value)
        {
            Contract.Assert(_chDigitalFilter != null);
            Contract.Assert(index < _chDigitalFilter.Length);

            _chDigitalFilter[index] = value;

            string propertyName = $"Ch{index}DigitalFilter";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public void SetRangeType(int index, sbyte value)
        {
            Contract.Assert(_chRangeType != null);
            Contract.Assert(index < _chRangeType.Length);

            _chRangeType[index] = value;

            string propertyName = $"Ch{index}RangeType";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public short GetRealTimeSample()
        {
            return _realTimeSample;
        }

        public void SetRealTimeSample(short value)
        {
            _realTimeSample = value;

            string propertyName = "RealTimeSample";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public sbyte GetNotchFilter()
        {
            return _notchFilter;
        }

        public void SetNotchFilter(sbyte value)
        {
            _notchFilter = value;

            string propertyName = "NotchFilter";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public short GetHHAlarmLimit(int index)
        {
            Contract.Assert(_hhAlarmLimit != null);
            Contract.Assert(index < _hhAlarmLimit.Length);

            return _hhAlarmLimit[index];
        }

        public void SetHHAlarmLimit(int index, short value)
        {
            Contract.Assert(_hhAlarmLimit != null);
            Contract.Assert(index < _hhAlarmLimit.Length);

            _hhAlarmLimit[index] = value;

            string propertyName = $"Ch{index}HHAlarmLimit";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public short GetHAlarmLimit(int index)
        {
            Contract.Assert(_hAlarmLimit != null);
            Contract.Assert(index < _hAlarmLimit.Length);

            return _hAlarmLimit[index];
        }

        public void SetHAlarmLimit(int index, short value)
        {
            Contract.Assert(_hAlarmLimit != null);
            Contract.Assert(index < _hAlarmLimit.Length);

            _hAlarmLimit[index] = value;

            string propertyName = $"Ch{index}HAlarmLimit";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public short GetLAlarmLimit(int index)
        {
            Contract.Assert(_lAlarmLimit != null);
            Contract.Assert(index < _lAlarmLimit.Length);

            return _lAlarmLimit[index];
        }

        public void SetLAlarmLimit(int index, short value)
        {
            Contract.Assert(_lAlarmLimit != null);
            Contract.Assert(index < _lAlarmLimit.Length);

            _lAlarmLimit[index] = value;

            string propertyName = $"Ch{index}LAlarmLimit";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public short GetLLAlarmLimit(int index)
        {
            Contract.Assert(_llAlarmLimit != null);
            Contract.Assert(index < _llAlarmLimit.Length);

            return _llAlarmLimit[index];
        }

        public void SetLLAlarmLimit(int index, short value)
        {
            Contract.Assert(_llAlarmLimit != null);
            Contract.Assert(index < _llAlarmLimit.Length);

            _llAlarmLimit[index] = value;

            string propertyName = $"Ch{index}LLAlarmLimit";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public sbyte GetAlarmDisable(int index)
        {
            Contract.Assert(_alarmDisable != null);
            Contract.Assert(index < _alarmDisable.Length);

            return _alarmDisable[index];
        }

        public void SetAlarmDisable(int index, sbyte value)
        {
            Contract.Assert(_alarmDisable != null);
            Contract.Assert(index < _alarmDisable.Length);

            _alarmDisable[index] = value;

            string propertyName = $"Ch{index}AlarmDisable";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public sbyte GetLimitAlarmLatch(int index)
        {
            Contract.Assert(_limitAlarmLatch != null);
            Contract.Assert(index < _limitAlarmLatch.Length);

            return _limitAlarmLatch[index];
        }

        public void SetLimitAlarmLatch(int index, sbyte value)
        {
            Contract.Assert(_limitAlarmLatch != null);
            Contract.Assert(index < _limitAlarmLatch.Length);

            _limitAlarmLatch[index] = value;

            string propertyName = $"Ch{index}LimitAlarmLatch";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public short GetHighLimit(int index)
        {
            Contract.Assert(_highLimit != null);
            Contract.Assert(index < _highLimit.Length);

            return _highLimit[index];
        }

        public void SetHighLimit(int index, short value)
        {
            Contract.Assert(_highLimit != null);
            Contract.Assert(index < _highLimit.Length);

            _highLimit[index] = value;

            string propertyName = $"Ch{index}HighLimit";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public short GetLowLimit(int index)
        {
            Contract.Assert(_lowLimit != null);
            Contract.Assert(index < _lowLimit.Length);

            return _lowLimit[index];
        }

        public void SetLowLimit(int index, short value)
        {
            Contract.Assert(_lowLimit != null);
            Contract.Assert(index < _lowLimit.Length);

            _lowLimit[index] = value;

            string propertyName = $"Ch{index}LowLimit";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public sbyte GetFaultMode(int index)
        {
            Contract.Assert(_faultMode != null);
            Contract.Assert(index < _faultMode.Length);

            return _faultMode[index];
        }

        public short GetFaultValue(int index)
        {
            Contract.Assert(_faultValue != null);
            Contract.Assert(index < _faultValue.Length);

            return _faultValue[index];
        }

        public sbyte GetProgMode(int index)
        {
            Contract.Assert(_progMode != null);
            Contract.Assert(index < _progMode.Length);

            return _progMode[index];
        }

        public short GetProgValue(int index)
        {
            Contract.Assert(_progValue != null);
            Contract.Assert(index < _progValue.Length);

            return _progValue[index];
        }

        public void SetFaultMode(int index, sbyte value)
        {
            Contract.Assert(_faultMode != null);
            Contract.Assert(index < _faultMode.Length);

            _faultMode[index] = value;

            string propertyName = $"Ch{index}FaultMode";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public void SetFaultValue(int index, short value)
        {
            Contract.Assert(_faultValue != null);
            Contract.Assert(index < _faultValue.Length);

            _faultValue[index] = value;

            string propertyName = $"Ch{index}FaultValue";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);

        }

        public void SetProgMode(int index, sbyte value)
        {
            Contract.Assert(_progMode != null);
            Contract.Assert(index < _progMode.Length);

            _progMode[index] = value;

            string propertyName = $"Ch{index}ProgMode";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public void SetProgValue(int index, short value)
        {
            Contract.Assert(_progValue != null);
            Contract.Assert(index < _progValue.Length);

            _progValue[index] = value;

            string propertyName = $"Ch{index}ProgValue";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);

        }

        // ir4
        public sbyte GetSensorType(int index)
        {
            Contract.Assert(_chSensorType != null);
            Contract.Assert(index < _chSensorType.Length);

            return _chSensorType[index];
        }

        public void SetSensorType(int index, sbyte value)
        {
            Contract.Assert(_chSensorType != null);
            Contract.Assert(index < _chSensorType.Length);

            _chSensorType[index] = value;

            string propertyName = $"Ch{index}SensorType";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public sbyte GetTempMode(int index)
        {
            Contract.Assert(_chTempMode != null);
            Contract.Assert(index < _chTempMode.Length);

            return _chTempMode[index];
        }

        public void SetTempMode(int index, sbyte value)
        {
            Contract.Assert(_chTempMode != null);
            Contract.Assert(index < _chTempMode.Length);

            _chTempMode[index] = value;

            string propertyName = $"Ch{index}TempMode";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        // end ir4

        private static T TryParse<T>(string s, T defaultValue)
        {
            if (typeof(T) == typeof(short))
            {
                short result;
                if (short.TryParse(s, out result))
                    return (T)(object)result;

                return defaultValue;
            }

            if (typeof(T) == typeof(sbyte))
            {
                sbyte result;
                if (sbyte.TryParse(s, out result))
                    return (T)(object)result;

                return defaultValue;
            }

            throw new NotImplementedException("Add code for TryParse!");
        }
    }
}