using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using ICSStudio.DeviceProfiles.Common;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils.TagExpression;

namespace ICSStudio.DeviceProperties.DiscreteIOs
{
    public class ModifiedDiscreteIO : ModifiedDIOModule
    {
        private Tag _configTag;
        private readonly IDataServer _dataServer;
        private readonly List<IDataOperand> _dataOperands;

        private readonly List<string> _editedList;
        private bool _isChangedExternally;

        private ushort[] _ptFilterOffOn;
        private ushort[] _ptFilterOnOff;

        private bool[] _ptFaultMode;
        private bool[] _ptFaultValue;
        private bool[] _ptProgMode;
        private bool[] _ptProgValue;

        public ModifiedDiscreteIO(IController controller, DiscreteIO discreteIO, IDataServer dataServer)
            : base(controller, discreteIO)
        {

            _dataServer = dataServer;
            _dataOperands = new List<IDataOperand>();

            _editedList = new List<string>();


            OriginalDiscreteIO = discreteIO;


            if (OriginalDiscreteIO?.Communications?.Connections != null &&
                OriginalDiscreteIO.Communications.Connections.Count > 0)
            {
                RPI = OriginalDiscreteIO.Communications.Connections[0].RPI;
                Unicast = OriginalDiscreteIO.Communications.Connections[0].Unicast.GetValueOrDefault();
            }

            _configTag = OriginalDiscreteIO?.ConfigTag;

            UpdateConfigValues(_configTag ?? CreateDefaultConfigTag(ConnectionConfigID, Major));

            if (_configTag != null)
                LinkConfigTag(_configTag);
        }


        public DiscreteIO OriginalDiscreteIO { get; }

        public event EventHandler ConfigValueChanged;

        public bool IsChangedExternally => _isChangedExternally;

        public ushort GetFilterOffOn(int index)
        {
            Contract.Assert(_ptFilterOffOn != null);
            Contract.Assert(index < _ptFilterOffOn.Length);

            return _ptFilterOffOn[index];
        }

        public void SetFilterOffOn(int index, ushort value)
        {
            Contract.Assert(_ptFilterOffOn != null);
            Contract.Assert(index < _ptFilterOffOn.Length);

            _ptFilterOffOn[index] = value;

            string propertyName = $"Pt{index}FilterOffOn";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public ushort GetFilterOnOff(int index)
        {
            Contract.Assert(_ptFilterOnOff != null);
            Contract.Assert(index < _ptFilterOnOff.Length);

            return _ptFilterOnOff[index];
        }

        public void SetFilterOnOff(int index, ushort value)
        {
            Contract.Assert(_ptFilterOnOff != null);
            Contract.Assert(index < _ptFilterOnOff.Length);

            _ptFilterOnOff[index] = value;

            string propertyName = $"Pt{index}FilterOnOff";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public bool GetFaultMode(int index)
        {
            Contract.Assert(_ptFaultMode != null);
            Contract.Assert(index < _ptFaultMode.Length);

            return _ptFaultMode[index];
        }

        public void SetFaultMode(int index, bool value)
        {
            Contract.Assert(_ptFaultMode != null);
            Contract.Assert(index < _ptFaultMode.Length);

            _ptFaultMode[index] = value;

            string propertyName = $"Pt{index}FaultMode";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public bool GetFaultValue(int index)
        {
            Contract.Assert(_ptFaultValue != null);
            Contract.Assert(index < _ptFaultValue.Length);

            return _ptFaultValue[index];
        }

        public void SetFaultValue(int index, bool value)
        {
            Contract.Assert(_ptFaultValue != null);
            Contract.Assert(index < _ptFaultValue.Length);

            _ptFaultValue[index] = value;

            string propertyName = $"Pt{index}FaultValue";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public bool GetProgMode(int index)
        {
            Contract.Assert(_ptProgMode != null);
            Contract.Assert(index < _ptProgMode.Length);

            return _ptProgMode[index];
        }

        public void SetProgMode(int index, bool value)
        {
            Contract.Assert(_ptProgMode != null);
            Contract.Assert(index < _ptProgMode.Length);

            _ptProgMode[index] = value;

            string propertyName = $"Pt{index}ProgMode";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public bool GetProgValue(int index)
        {
            Contract.Assert(_ptProgValue != null);
            Contract.Assert(index < _ptProgValue.Length);

            return _ptProgValue[index];
        }

        public void SetProgValue(int index, bool value)
        {
            Contract.Assert(_ptProgValue != null);
            Contract.Assert(index < _ptProgValue.Length);

            _ptProgValue[index] = value;

            string propertyName = $"Pt{index}ProgValue";
            if (!_editedList.Contains(propertyName))
                _editedList.Add(propertyName);
        }

        public void PostApply()
        {
            _editedList.Clear();
            _isChangedExternally = false;

            if (_configTag != null)
            {
                UnlinkConfigTag(_configTag);
            }

            _configTag = OriginalDiscreteIO.ConfigTag;

            if (_configTag != null)
            {
                LinkConfigTag(_configTag);
            }
        }

        public void ChangeConnectionConfigID(uint newConfigID)
        {
            if (ConnectionConfigID == newConfigID)
                return;

            _editedList.Clear();

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

        private void UpdateConfigValues(Tag configTag)
        {
            if (configTag == null)
                return;

            // PtFilterOffOn,PtFilterOnOff
            var module = Profiles.GetModule(Major);
            if (module != null && module.NumberOfInputs > 0)
            {
                _ptFilterOffOn = new ushort[module.NumberOfInputs];
                _ptFilterOnOff = new ushort[module.NumberOfInputs];

                for (int i = 0; i < module.NumberOfInputs; i++)
                {
                    ushort ushortValue;
                    short shortValue;

                    string stringValue = configTag.GetMemberValue($"Pt{i}FilterOffOn", true);
                    if (ushort.TryParse(stringValue, out ushortValue))
                    {
                        _ptFilterOffOn[i] = ushortValue;
                    }
                    else if (short.TryParse(stringValue, out shortValue))
                    {
                        _ptFilterOffOn[i] = (ushort) shortValue;
                    }
                    else
                    {
                        _ptFilterOffOn[i] = 1000;
                    }

                    stringValue = configTag.GetMemberValue($"Pt{i}FilterOnOff", true);
                    if (ushort.TryParse(stringValue, out ushortValue))
                    {
                        _ptFilterOnOff[i] = ushortValue;
                    }
                    else if (short.TryParse(stringValue, out shortValue))
                    {
                        _ptFilterOnOff[i] = (ushort) shortValue;
                    }
                    else
                    {
                        _ptFilterOnOff[i] = 1000;
                    }
                }

            }

            if (module != null && module.NumberOfOutputs > 0)
            {

                _ptFaultMode = new bool[module.NumberOfOutputs];
                _ptFaultValue = new bool[module.NumberOfOutputs];
                _ptProgMode = new bool[module.NumberOfOutputs];
                _ptProgValue = new bool[module.NumberOfOutputs];

                for (int i = 0; i < module.NumberOfOutputs; i++)
                {
                    bool memberValue;
                    string stringValue = configTag.GetMemberValue($"Pt{i}FaultMode", true);
                    if (bool.TryParse(stringValue, out memberValue))
                    {
                        _ptFaultMode[i] = memberValue;
                    }

                    stringValue = configTag.GetMemberValue($"Pt{i}FaultValue", true);
                    if (bool.TryParse(stringValue, out memberValue))
                    {
                        _ptFaultValue[i] = memberValue;
                    }

                    stringValue = configTag.GetMemberValue($"Pt{i}ProgMode", true);
                    if (bool.TryParse(stringValue, out memberValue))
                    {
                        _ptProgMode[i] = memberValue;
                    }

                    stringValue = configTag.GetMemberValue($"Pt{i}ProgValue", true);
                    if (bool.TryParse(stringValue, out memberValue))
                    {
                        _ptProgValue[i] = memberValue;
                    }
                }

            }
        }

        private void LinkConfigTag(Tag configTag)
        {
            if (configTag == null)
                return;

            UnlinkConfigTag(null);

            var module = Profiles.GetModule(Major);

            if (module != null && module.NumberOfInputs > 0)
            {
                for (int i = 0; i < module.NumberOfInputs; i++)
                {
                    var dataOperand = _dataServer.CreateDataOperand(configTag, $"Pt{i}FilterOffOn");
                    _dataOperands.Add(dataOperand);

                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Pt{i}FilterOnOff");
                    _dataOperands.Add(dataOperand);
                }
            }

            if (module != null && module.NumberOfOutputs > 0)
            {
                for (int i = 0; i < module.NumberOfOutputs; i++)
                {
                    var dataOperand = _dataServer.CreateDataOperand(configTag, $"Pt{i}FaultMode");
                    _dataOperands.Add(dataOperand);

                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Pt{i}FaultValue");
                    _dataOperands.Add(dataOperand);

                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Pt{i}ProgMode");
                    _dataOperands.Add(dataOperand);

                    dataOperand = _dataServer.CreateDataOperand(configTag, $"Pt{i}ProgValue");
                    _dataOperands.Add(dataOperand);
                }

            }

            foreach (var dataOperand in _dataOperands)
            {
                PropertyChangedEventManager.AddHandler(dataOperand, OnDataOperandValueChanged,
                    "RawValue");
                dataOperand.StartMonitoring(true, false);
            }

        }

        // ReSharper disable once UnusedParameter.Local
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

                string number = Regex.Replace(member, @"[^0-9]+", "");
                Contract.Assert(!string.IsNullOrEmpty(number));

                int index = int.Parse(number);

                var module = Profiles.GetModule(Major);
                if (module != null && module.NumberOfInputs > 0)
                {
                    Contract.Assert(index < module.NumberOfInputs);

                    ushort memberValue = dataOperand.UInt16Value;

                    if (member.EndsWith("FilterOffOn"))
                    {
                        _ptFilterOffOn[index] = memberValue;
                    }
                    else if (member.EndsWith("FilterOnOff"))
                    {
                        _ptFilterOnOff[index] = memberValue;
                    }
                }

                if (module != null && module.NumberOfOutputs > 0)
                {
                    Contract.Assert(index < module.NumberOfOutputs);

                    bool memberValue = dataOperand.BoolValue;

                    if (member.EndsWith("FaultMode"))
                    {
                        _ptFaultMode[index] = memberValue;
                    }
                    else if (member.EndsWith("FaultValue"))
                    {
                        _ptFaultValue[index] = memberValue;
                    }
                    else if (member.EndsWith("ProgMode"))
                    {
                        _ptProgMode[index] = memberValue;
                    }
                    else if (member.EndsWith("ProgValue"))
                    {
                        _ptProgValue[index] = memberValue;
                    }

                }

                ConfigValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
