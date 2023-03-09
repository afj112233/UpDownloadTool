using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Common;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.DataType
{
    public class DataTypeCollection : IDataTypeCollection
    {
        private readonly Controller _parentController;
        private readonly List<IDataType> _dataTypes;

        public DataTypeCollection(Controller parentController)
        {
            _parentController = parentController;
            _dataTypes = new List<IDataType>();

            Uid = Guid.NewGuid().GetHashCode();

            InitPredefinedTypes();

        }


        public IEnumerable<int> GetAllReferencerDataTypeUids(IDataType dataType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDataType> GetFamilyDataTypes(FamilyType family)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IDataType> GetEnumerator() => _dataTypes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IController ParentController => _parentController;
        public int Uid { get; }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => _dataTypes.Count;

        public IDataType this[int uid]
        {
            get
            {
                foreach (IDataType dataType in _dataTypes)
                {
                    if (dataType.Uid == uid)
                        return dataType;
                }

                return null;
            }
        }

        public IDataType this[string name]
        {
            get
            {
                for (int i = _dataTypes.Count - 1; i >= 0; i--)
                {
                    var dataType = _dataTypes[i];
                    if (dataType.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        return dataType;
                }

                Debug.Assert(name != null);
                return null;
            }
        }
        /// <summary>
        /// for import
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isTmp"></param>
        /// <returns></returns>
        public IDataType FindUserDataType(string name, bool isTmp)
        {
            for (int i = _dataTypes.Count - 1; i >= 0; i--)
            {
                var dataType = _dataTypes[i] as AssetDefinedDataType;
                if (dataType == null) return null;
                if (dataType.Name.Equals(name, StringComparison.OrdinalIgnoreCase)&&dataType.IsTmp==isTmp)
                    return dataType;
            }

            Debug.Assert(name != null);
            return null;
        }

        public IDataType TryGetChildByUid(int uid)
        {
            throw new NotImplementedException();
        }

        public IDataType TryGetChildByName(string name)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<ComponentCoreInfo> GetComponentCoreInfoList()
        {
            throw new NotImplementedException();
        }

        public ComponentCoreInfo GetComponentCoreInfo(int uid)
        {
            throw new NotImplementedException();
        }

        public void AddDataType(DataType dataType)
        {
            _dataTypes.Add(dataType);

            if (!((dataType as AssetDefinedDataType)?.IsTmp ?? false))
            {
                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, dataType));
            }
        }

        public void DeleteDataType(IDataType dataType)
        {
            if (_dataTypes.Contains(dataType))
            {
                _dataTypes.Remove(dataType);
                dataType.Dispose();
                if (!((dataType as AssetDefinedDataType)?.IsTmp ?? false))
                {
                    CollectionChanged?.Invoke(this,
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, dataType));
                }
            }
        }


        public bool ParseDataType(
            string dataType,
            out string dataTypeName,
            out int dim1, out int dim2, out int dim3,
            out int errorCode)
        {
            dataTypeName = string.Empty;
            dim1 = 0;
            dim2 = 0;
            dim3 = 0;
            errorCode = 0;

            if (string.IsNullOrWhiteSpace(dataType))
            {
                errorCode = -1;
                return false;
            }

            if (!Regex.IsMatch(dataType, @"\A[a-z_][a-z0-9_:]*(\[(\d+,){0,2}\d+\])?\Z",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant))
            {
                errorCode = -2;
                return false;
            }

            Match match = Regex.Match(dataType, @"^[a-z0-9_:]+",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
            dataTypeName = match.Value;

            MatchCollection matchCollection = 
                Regex.Matches(
                    dataType.Remove(0,dataTypeName.Length), 
                    @"(?<=\W+)\d+",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
            switch (matchCollection.Count)
            {
                case 1:
                    if (int.TryParse(matchCollection[0].Value, out dim1))
                    {
                        if (dim1 == 0)
                        {
                            errorCode = -5;
                            return false;
                        }
                    }
                    else
                    {
                        errorCode = -5;
                        return false;
                    }

                    break;
                case 2:
                    if (int.TryParse(matchCollection[1].Value, out dim1) &&
                        int.TryParse(matchCollection[0].Value, out dim2))
                    {
                        if (dim1 == 0 || dim2 == 0)
                        {
                            errorCode = -5;
                            return false;
                        }
                    }
                    else
                    {
                        errorCode = -5;
                        return false;
                    }

                    break;
                case 3:

                    if (int.TryParse(matchCollection[2].Value, out dim1) &&
                        int.TryParse(matchCollection[1].Value, out dim2) &&
                        int.TryParse(matchCollection[0].Value, out dim3))
                    {
                        if (dim1 == 0 || dim2 == 0 || dim3 == 0)
                        {
                            errorCode = -5;
                            return false;
                        }
                    }
                    else
                    {
                        errorCode = -5;
                        return false;
                    }

                    break;
            }

            try
            {
                var foundDataType = this[dataTypeName];
                if (foundDataType != null)
                    return true;

                errorCode = -4;
                return false;
            }
            catch (Exception)
            {
                errorCode = -4;
                return false;
            }

        }

        public DataTypeInfo ParseDataTypeInfo(string dataType)
        {
            string typeName;
            int dim1, dim2, dim3;
            int errorCode;
            var isValid = ParseDataType(
                dataType, out typeName,
                out dim1, out dim2, out dim3, out errorCode);

            if (isValid)
            {
                return new DataTypeInfo
                {
                    DataType = this[typeName],
                    Dim1 = dim1,
                    Dim2 = dim2,
                    Dim3 = dim3
                };
            }

            return new DataTypeInfo {DataType = null, Dim1 = 0, Dim2 = 0, Dim3 = 0};
        }

        private void InitPredefinedTypes()
        {
            AddDataType(CB_CONTINUOUS_MODE.Inst);
            AddDataType(CB_CRANKSHAFT_POS_MONITOR.Inst);
            AddDataType(CB_INCH_MODE.Inst);
            AddDataType(CB_SINGLE_STROKE_MODE.Inst);
            AddDataType(CC.Inst);
            AddDataType(CONFIGURABLE_ROUT.Inst);
            AddDataType(CONNECTION_STATUS.Inst);
            AddDataType(CONTROL.Inst);
            AddDataType(COORDINATE_SYSTEM.Inst);
            AddDataType(COUNTER.Inst);
            AddDataType(DATALOG_INSTRUCTION.Inst);
            AddDataType(DCAF_INPUT.Inst);
            AddDataType(DCA_INPUT.Inst);
            AddDataType(DCI_MONITOR.Inst);
            AddDataType(DCI_START.Inst);
            AddDataType(DCI_STOP.Inst);
            AddDataType(DCI_STOP_TEST.Inst);
            AddDataType(DCI_STOP_TEST_LOCK.Inst);
            AddDataType(DCI_STOP_TEST_MUTE.Inst);
            AddDataType(DEADTIME.Inst);
            AddDataType(DERIVATIVE.Inst);
            AddDataType(DISCRETE_2STATE.Inst);
            AddDataType(DISCRETE_3STATE.Inst);
            AddDataType(DIVERSE_INPUT.Inst);
            AddDataType(DOMINANT_RESET.Inst);
            AddDataType(DOMINANT_SET.Inst);
            AddDataType(DYNAMICS_DATA.Inst);
            AddDataType(EIGHT_POS_MODE_SELECTOR.Inst);
            AddDataType(EMERGENCY_STOP.Inst);
            AddDataType(ENABLE_PENDANT.Inst);
            AddDataType(ENERGY_BASE.Inst);
            AddDataType(ENERGY_ELECTRICAL.Inst);
            AddDataType(EXT_ROUTINE_CONTROL.Inst);
            AddDataType(FBD_BIT_FIELD_DISTRIBUTE.Inst);
            AddDataType(FBD_BOOLEAN_AND.Inst);
            AddDataType(FBD_BOOLEAN_NOT.Inst);
            AddDataType(FBD_BOOLEAN_OR.Inst);
            AddDataType(FBD_BOOLEAN_XOR.Inst);
            AddDataType(FBD_COMPARE.Inst);
            AddDataType(FBD_CONVERT.Inst);
            AddDataType(FBD_COUNTER.Inst);
            AddDataType(FBD_LIMIT.Inst);
            AddDataType(FBD_LOGICAL.Inst);
            AddDataType(FBD_MASKED_MOVE.Inst);
            AddDataType(FBD_MASK_EQUAL.Inst);
            AddDataType(FBD_MATH.Inst);
            AddDataType(FBD_MATH_ADVANCED.Inst);
            AddDataType(FBD_TIMER.Inst);
            AddDataType(FBD_TRUNCATE.Inst);
            AddDataType(FILTER_HIGH_PASS.Inst);
            AddDataType(FILTER_LOW_PASS.Inst);
            AddDataType(FILTER_NOTCH.Inst);
            AddDataType(FIVE_POS_MODE_SELECTOR.Inst);
            AddDataType(FLIP_FLOP_D.Inst);
            AddDataType(FLIP_FLOP_JK.Inst);
            AddDataType(FUNCTION_GENERATOR.Inst);
            AddDataType(HL_LIMIT.Inst);
            AddDataType(HMIBC.Inst);
            AddDataType(IMC.Inst);
            AddDataType(INTEGRATOR.Inst);
            AddDataType(LEAD_LAG.Inst);
            AddDataType(LEAD_LAG_SEC_ORDER.Inst);
            AddDataType(LIGHT_CURTAIN.Inst);
            AddDataType(LREAL.Inst);
            AddDataType(MAIN_VALVE_CONTROL.Inst);
            AddDataType(MANUAL_VALVE_CONTROL.Inst);
            AddDataType(MAXIMUM_CAPTURE.Inst);
            AddDataType(MESSAGE.Inst);
            AddDataType(MINIMUM_CAPTURE.Inst);
            AddDataType(MMC.Inst);
            AddDataType(MOVING_AVERAGE.Inst);
            AddDataType(MOVING_STD_DEV.Inst);
            AddDataType(MULTIPLEXER.Inst);
            AddDataType(MUTING_FOUR_SENSOR_BIDIR.Inst);
            AddDataType(MUTING_TWO_SENSOR_ASYM.Inst);
            AddDataType(MUTING_TWO_SENSOR_SYM.Inst);
            AddDataType(OUTPUT_CAM.Inst);
            AddDataType(OUTPUT_COMPENSATION.Inst);
            AddDataType(PATH_DATA.Inst);
            AddDataType(PHASE.Inst);
            AddDataType(PHASE_INSTRUCTION.Inst);
            AddDataType(PID.Inst);
            AddDataType(PIDE_AUTOTUNE.Inst);
            AddDataType(PID_ENHANCED.Inst);
            AddDataType(POSITION_DATA.Inst);
            AddDataType(POSITION_PROP.Inst);
            AddDataType(PROP_INT.Inst);
            AddDataType(PULSE_MULTIPLIER.Inst);
            AddDataType(RAMP_SOAK.Inst);
            AddDataType(RATE_LIMITER.Inst);
            AddDataType(REDUNDANT_INPUT.Inst);
            AddDataType(REDUNDANT_OUTPUT.Inst);
            AddDataType(SAFELY_LIMITED_POSITION.Inst);
            AddDataType(SAFELY_LIMITED_SPEED.Inst);
            AddDataType(SAFETY_FEEDBACK_INTERFACE.Inst);
            AddDataType(SAFETY_MAT.Inst);
            AddDataType(SAFE_BRAKE_CONTROL.Inst);
            AddDataType(SAFE_DIRECTION.Inst);
            AddDataType(SAFE_OPERATING_STOP.Inst);
            AddDataType(SAFE_STOP_1.Inst);
            AddDataType(SAFE_STOP_2.Inst);
            AddDataType(SCALE.Inst);
            AddDataType(SEC_ORDER_CONTROLLER.Inst);
            AddDataType(SELECT.Inst);
            AddDataType(SELECTABLE_NEGATE.Inst);
            AddDataType(SELECTED_SUMMER.Inst);
            AddDataType(SELECT_ENHANCED.Inst);
            AddDataType(SEQUENCE.Inst);
            AddDataType(SEQ_BOOL.Inst);
            AddDataType(SEQ_DINT.Inst);
            AddDataType(SEQ_INT.Inst);
            AddDataType(SEQ_REAL.Inst);
            AddDataType(SEQ_SINT.Inst);
            AddDataType(SEQ_STEP.Inst);
            AddDataType(SEQ_STRING.Inst);
            AddDataType(SEQ_TRANSITION.Inst);
            AddDataType(SERIAL_PORT_CONTROL.Inst);
            AddDataType(SFC_ACTION.Inst);
            AddDataType(SFC_STEP.Inst);
            AddDataType(SFC_STOP.Inst);
            AddDataType(SPLIT_RANGE.Inst);
            AddDataType(S_CURVE.Inst);
            AddDataType(THRS_ENHANCED.Inst);
            AddDataType(TIMER.Inst);
            AddDataType(TOTALIZER.Inst);
            AddDataType(TWO_HAND_RUN_STATION.Inst);
            AddDataType(UP_DOWN_ACCUM.Inst);
            AddDataType(UDINT.Inst);
            AddDataType(UINT.Inst);
            AddDataType(USINT.Inst);
            AddDataType(ULINT.Inst);

            // base type
            AddDataType(BOOL.Inst);
            AddDataType(SINT.Inst);
            //AddDataType(USINT.Inst);
            AddDataType(INT.Inst);
            //AddDataType(UINT.Inst);
            AddDataType(DINT.Inst);
            //AddDataType(UDINT.Inst);
            AddDataType(LINT.Inst);
            //AddDataType(ULINT.Inst);
            AddDataType(REAL.Inst);

            // bool ref type
            AddDataType(BOOL.SInst);
            AddDataType(BOOL.IInst);
            AddDataType(BOOL.DInst);
            AddDataType(BOOL.LInst);

            AddDataType(AXIS_COMMON.Inst);

            AddDataType(STRING.Inst);
            AddDataType(ODOMETER.Inst);
            AddDataType(SIGNED_ODOMETER.Inst);
            AddDataType(EXT_ROUTINE_PARAMETERS.Inst);
            AddDataType(AXIS_CIP_DRIVE.Inst);
            AddDataType(MOTION_INSTRUCTION.Inst);
            AddDataType(FBD_ONESHOT.Inst);
            AddDataType(MOTION_GROUP.Inst);
            AddDataType(ALARM.Inst);
            AddDataType(ALARM_ANALOG.Inst);
            AddDataType(ALARM_DIGITAL.Inst);
            AddDataType(AUX_VALVE_CONTROL.Inst);
            AddDataType(AXIS_CONSUMED.Inst);
            AddDataType(AXIS_GENERIC.Inst);
            AddDataType(AXIS_GENERIC_DRIVE.Inst);
            AddDataType(AXIS_SERVO.Inst);
            AddDataType(AXIS_SERVO_DRIVE.Inst);
            AddDataType(AXIS_VIRTUAL.Inst);
            AddDataType(CAM.Inst);
            AddDataType(CAMSHAFT_MONITOR.Inst);
            AddDataType(CAM_PROFILE.Inst);
        }

        public void RemoveNoPredefinedType()
        {
            for (int i = _dataTypes.Count - 1; i >= 0; i--)
            {
                var dataType = _dataTypes[i];

                if (dataType is AOIDataType || dataType is UserDefinedDataType || dataType is ModuleDefinedDataType)
                {
                    _dataTypes.Remove(dataType);
                    dataType.Dispose();
                }
            }
        }
    }
}
