using ICSStudio.Interfaces.DataType;
using System;
using System.Linq;
using ICSStudio.SimpleServices.PredefinedType;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.DataType
{

    public class CompositiveType : DataType
    {
        protected int _BitSize;
        protected int _ByteSize;
        public override int BitSize => _BitSize;
        public override int ByteSize => _ByteSize;
        private string _engineeringUnit;

        protected readonly TypeMemberComponentCollection _viewTypeMemberComponentCollection =
            new TypeMemberComponentCollection();

        public ITypeMemberComponentCollection TypeMembers => _viewTypeMemberComponentCollection;
        public IDataTypeMember this[int typeIndex] => TypeMembers[typeIndex];
        public IDataTypeMember this[string typeName] => TypeMembers[typeName];

        protected void FixUp(JToken data,IField res)
        {
            if (data != null) return;
            if (this is ALARM)
            {
                var alarmFiled = (ALARMField)res;
                var member = (DataTypeMember)TypeMembers["HHLimit"];
                ((RealField)alarmFiled.fields[member.FieldIndex].Item1).value = float.MaxValue;

                member = (DataTypeMember)TypeMembers["HLimit"];
                ((RealField)alarmFiled.fields[member.FieldIndex].Item1).value = float.MaxValue;

                member = (DataTypeMember)TypeMembers["LLimit"];
                ((RealField)alarmFiled.fields[member.FieldIndex].Item1).value = float.MinValue;

                member = (DataTypeMember)TypeMembers["LLLimit"];
                ((RealField)alarmFiled.fields[member.FieldIndex].Item1).value = float.MinValue;
            }
            else if(this is ALARM_DIGITAL)
            {
                var alarmDigital = (ALARM_DIGITALField) res;
                var member = (DataTypeMember)TypeMembers["Condition"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["AckRequired"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["Severity"];
                ((Int32Field)alarmDigital.fields[member.FieldIndex].Item1).value = 500;

                member = (DataTypeMember)TypeMembers["Acked"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["Commissioned"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;
            }
            else if (this is ALARM_ANALOG)
            {
                var alarmDigital = (ALARM_ANALOGField)res;
                var member = (DataTypeMember)TypeMembers["AckRequired"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["HHMinDurationEnable"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["HMinDurationEnable"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["LMinDurationEnable"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["LLMinDurationEnable"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["HHSeverity"];
                ((Int32Field)alarmDigital.fields[member.FieldIndex].Item1).value = 500;

                member = (DataTypeMember)TypeMembers["HSeverity"];
                ((Int32Field)alarmDigital.fields[member.FieldIndex].Item1).value = 500;

                member = (DataTypeMember)TypeMembers["LSeverity"];
                ((Int32Field)alarmDigital.fields[member.FieldIndex].Item1).value = 500;

                member = (DataTypeMember)TypeMembers["LLSeverity"];
                ((Int32Field)alarmDigital.fields[member.FieldIndex].Item1).value = 500;

                member = (DataTypeMember)TypeMembers["ROCPosSeverity"];
                ((Int32Field)alarmDigital.fields[member.FieldIndex].Item1).value = 500;

                member = (DataTypeMember)TypeMembers["ROCNegSeverity"];
                ((Int32Field)alarmDigital.fields[member.FieldIndex].Item1).value = 500;

                member = (DataTypeMember)TypeMembers["HHAcked"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["HAcked"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["LAcked"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["LLAcked"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["ROCPosAcked"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;
                
                member = (DataTypeMember)TypeMembers["ROCNegAcked"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["Commissioned"];
                ((Int8Field)alarmDigital.fields[member.FieldIndex].Item1).value = 1;
            }
            else if (this is PID_ENHANCED)
            {
                var pideFiled = (PID_ENHANCEDField)res;
                var member = (DataTypeMember)TypeMembers["PVEUMax"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["SPHLimit"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["RatioProg"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["RatioOper"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["RatioHLimit"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["RatioLLimit"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["CVEUMax"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CVHLimit"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["PVEDerivative"];
                ((Int8Field)pideFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["PVHHLimit"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = float.MaxValue;

                member = (DataTypeMember)TypeMembers["PVHLimit"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = float.MaxValue;

                member = (DataTypeMember)TypeMembers["PVLLimit"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = float.MinValue;

                member = (DataTypeMember)TypeMembers["PVLLLimit"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = float.MinValue;

                member = (DataTypeMember)TypeMembers["DevHHLimit"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = float.MaxValue;

                member = (DataTypeMember)TypeMembers["DevHLimit"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = float.MaxValue;

                member = (DataTypeMember)TypeMembers["DevLLimit"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = float.MaxValue;

                member = (DataTypeMember)TypeMembers["DevLLLimit"];
                ((RealField)pideFiled.fields[member.FieldIndex].Item1).value = float.MaxValue;

                member = (DataTypeMember)TypeMembers["RTSTime"];
                ((Int32Field)pideFiled.fields[member.FieldIndex].Item1).value = 1;
            }
            else if (this is RAMP_SOAK)
            {
                var rmpsFiled = (RAMP_SOAKField)res;
                var member = (DataTypeMember)TypeMembers["NumberOfSegs"];
                ((Int32Field)rmpsFiled.fields[member.FieldIndex].Item1).value = 1;
            }
            else if (this is POSITION_PROP)
            {
                var pospFiled = (POSITION_PROPField)res;
                var member = (DataTypeMember)TypeMembers["PositionEUMax"];
                ((RealField)pospFiled.fields[member.FieldIndex].Item1).value = 100;
            }
            else if (this is SPLIT_RANGE)
            {
                var srtpFiled = (SPLIT_RANGEField)res;
                var member = (DataTypeMember)TypeMembers["MaxHeatIn"];
                ((RealField)srtpFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["MinHeatIn"];
                ((RealField)srtpFiled.fields[member.FieldIndex].Item1).value = 50;

                member = (DataTypeMember)TypeMembers["MinCoolIn"];
                ((RealField)srtpFiled.fields[member.FieldIndex].Item1).value = 50;
            }
            else if (this is LEAD_LAG)
            {
                var ldlgFiled = (LEAD_LAGField)res;
                var member = (DataTypeMember)TypeMembers["Gain"];
                ((RealField)ldlgFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["RTSTime"];
                ((Int32Field)ldlgFiled.fields[member.FieldIndex].Item1).value = 1;
            }
            else if (this is FUNCTION_GENERATOR)
            {
                var fgenFiled = (FUNCTION_GENERATORField)res;
                var member = (DataTypeMember)TypeMembers["XY1Size"];
                ((Int32Field)fgenFiled.fields[member.FieldIndex].Item1).value = 1;
            }
            else if (this is TOTALIZER)
            {
                var totFiled = (TOTALIZERField)res;
                var member = (DataTypeMember)TypeMembers["Gain"];
                ((RealField)totFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["RTSTime"];
                ((Int32Field)totFiled.fields[member.FieldIndex].Item1).value = 1;
            }
            else if (this is DEADTIME)
            {
                var dedtFiled = (DEADTIMEField)res;
                var member = (DataTypeMember)TypeMembers["Gain"];
                ((RealField)dedtFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["RTSTime"];
                ((Int32Field)dedtFiled.fields[member.FieldIndex].Item1).value = 1;
            }
            else if (this is DISCRETE_2STATE)
            {
                var d2sdFiled = (DISCRETE_2STATEField)res;
                var member = (DataTypeMember)TypeMembers["State0Perm"];
                ((Int8Field)d2sdFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["State1Perm"];
                ((Int8Field)d2sdFiled.fields[member.FieldIndex].Item1).value = 1;
            }
            else if (this is DISCRETE_3STATE)
            {
                var d3sdFiled = (DISCRETE_3STATEField)res;
                var member = (DataTypeMember)TypeMembers["State0Perm"];
                ((Int8Field)d3sdFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["State1Perm"];
                ((Int8Field)d3sdFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["State2Perm"];
                ((Int8Field)d3sdFiled.fields[member.FieldIndex].Item1).value = 1;
            }
            else if (this is IMC)
            {
                var imcFiled = (IMCField)res;
                var member = (DataTypeMember)TypeMembers["PVEUMax"];
                ((RealField)imcFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["SPHLimit"];
                ((RealField)imcFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["RatioProg"];
                ((RealField)imcFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["RatioOper"];
                ((RealField)imcFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["RatioHLimit"];
                ((RealField)imcFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["RatioLLimit"];
                ((RealField)imcFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["CVEUMax"];
                ((RealField)imcFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CVHLimit"];
                ((RealField)imcFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["ModelGain"];
                ((RealField)imcFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["RTSTime"];
                ((Int32Field)imcFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["AtuneTimeLimit"];
                ((RealField)imcFiled.fields[member.FieldIndex].Item1).value = 60;

                member = (DataTypeMember)TypeMembers["CVStepSize"];
                ((RealField)imcFiled.fields[member.FieldIndex].Item1).value = 10;

                member = (DataTypeMember)TypeMembers["Factor"];
                ((RealField)imcFiled.fields[member.FieldIndex].Item1).value = 100;
            }
            else if (this is CC)
            {
                var ccFiled = (CCField)res;
                var member = (DataTypeMember)TypeMembers["PVEUMax"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["SPHLimit"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CV1EUMax"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CV2EUMax"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CV3EUMax"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CV1HLimit"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CV2HLimit"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CV3HLimit"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CV1ModelGain"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["CV2ModelGain"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["CV3ModelGain"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["Act1stCV"];
                ((Int32Field)ccFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["Act2ndCV"];
                ((Int32Field)ccFiled.fields[member.FieldIndex].Item1).value = 2;

                member = (DataTypeMember)TypeMembers["Act3rdCV"];
                ((Int32Field)ccFiled.fields[member.FieldIndex].Item1).value = 3;

                member = (DataTypeMember)TypeMembers["Target1stCV"];
                ((Int32Field)ccFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["Target2ndCV"];
                ((Int32Field)ccFiled.fields[member.FieldIndex].Item1).value = 2;

                member = (DataTypeMember)TypeMembers["Target3rdCV"];
                ((Int32Field)ccFiled.fields[member.FieldIndex].Item1).value = 3;

                member = (DataTypeMember)TypeMembers["RTSTime"];
                ((Int32Field)ccFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["AtuneTimeLimit"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 60;

                member = (DataTypeMember)TypeMembers["NoiseLevel"];
                ((Int32Field)ccFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["CV1StepSize"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 10;

                member = (DataTypeMember)TypeMembers["CV2StepSize"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 10;

                member = (DataTypeMember)TypeMembers["CV3StepSize"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 10;

                member = (DataTypeMember)TypeMembers["CV1ResponseSpeed"];
                ((Int32Field)ccFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["CV2ResponseSpeed"];
                ((Int32Field)ccFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["CV3ResponseSpeed"];
                ((Int32Field)ccFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["Factor"];
                ((RealField)ccFiled.fields[member.FieldIndex].Item1).value = 100;
            }
            else if (this is MMC)
            {
                var mmcFiled = (MMCField)res;
                var member = (DataTypeMember)TypeMembers["PV1EUMax"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["PV2EUMax"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["SP1HLimit"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["SP2HLimit"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CV1EUMax"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CV2EUMax"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CV3EUMax"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CV1HLimit"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CV2HLimit"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CV3HLimit"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 100;

                member = (DataTypeMember)TypeMembers["CV1PV1ModelGain"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["CV2PV1ModelGain"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["CV3PV1ModelGain"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["CV1PV2ModelGain"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["CV2PV2ModelGain"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["CV3PV2ModelGain"];
                ((RealField)mmcFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is INTEGRATOR)
            {
                var intgFiled = (INTEGRATORField) res;
                var member = (DataTypeMember) TypeMembers["HighLimit"];
                ((RealField) intgFiled.fields[member.FieldIndex].Item1).value = float.MaxValue;

                member = (DataTypeMember) TypeMembers["LowLimit"];
                ((RealField) intgFiled.fields[member.FieldIndex].Item1).value = float.MinValue;

                member = (DataTypeMember) TypeMembers["RTSTime"];
                ((Int32Field) intgFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is PROP_INT)
            {
                var piFiled = (PROP_INTField) res;
                var member = (DataTypeMember) TypeMembers["Kp"];
                ((RealField) piFiled.fields[member.FieldIndex].Item1).value = (float)1.17549435E-38;

                member = (DataTypeMember) TypeMembers["HighLimit"];
                ((RealField) piFiled.fields[member.FieldIndex].Item1).value = float.MaxValue;

                member = (DataTypeMember) TypeMembers["LowLimit"];
                ((RealField) piFiled.fields[member.FieldIndex].Item1).value = float.MinValue;

                member = (DataTypeMember) TypeMembers["ShapeKpPlus"];
                ((RealField) piFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember) TypeMembers["ShapeKpMinus"];
                ((RealField) piFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember) TypeMembers["KpInRange"];
                ((RealField) piFiled.fields[member.FieldIndex].Item1).value = float.MaxValue;

                member = (DataTypeMember) TypeMembers["ShapeWldPlus"];
                ((RealField) piFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["ShapeWldMinus"];
                ((RealField)piFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["WldInRange"];
                ((RealField)piFiled.fields[member.FieldIndex].Item1).value = float.MaxValue;

                member = (DataTypeMember)TypeMembers["RTSTime"];
                ((Int32Field)piFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is PULSE_MULTIPLIER)
            {
                var pumlFiled = (PULSE_MULTIPLIERField) res;
                var member = (DataTypeMember) TypeMembers["Mode"];
                ((Int8Field) pumlFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["WordSize"];
                ((Int32Field)pumlFiled.fields[member.FieldIndex].Item1).value = 14;

                member = (DataTypeMember)TypeMembers["Multiplier"];
                ((Int32Field)pumlFiled.fields[member.FieldIndex].Item1).value = 100000;
            }else if (this is S_CURVE)
            {
                var scrvFiled = (S_CURVEField) res;
                var member = (DataTypeMember) TypeMembers["RTSTime"];
                ((Int32Field) scrvFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is SEC_ORDER_CONTROLLER)
            {
                var socFiled = (SEC_ORDER_CONTROLLERField) res;
                var member = (DataTypeMember)TypeMembers["Gain"];
                ((RealField)socFiled.fields[member.FieldIndex].Item1).value = (float)1.17549435E-38;

                member = (DataTypeMember)TypeMembers["HighLimit"];
                ((RealField)socFiled.fields[member.FieldIndex].Item1).value = float.MaxValue;

                member = (DataTypeMember)TypeMembers["LowLimit"];
                ((RealField)socFiled.fields[member.FieldIndex].Item1).value = float.MinValue;

                member = (DataTypeMember)TypeMembers["RTSTime"];
                ((Int32Field)socFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is DERIVATIVE)
            {
                var dervFiled = (DERIVATIVEField) res;
                var member = (DataTypeMember)TypeMembers["Gain"];
                ((RealField)dervFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["RTSTime"];
                ((Int32Field)dervFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is FILTER_HIGH_PASS)
            {
                var hpfFiled = (FILTER_HIGH_PASSField) res;
                var member = (DataTypeMember)TypeMembers["Order"];
                ((Int32Field)hpfFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["RTSTime"];
                ((Int32Field)hpfFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is LEAD_LAG_SEC_ORDER)
            {
                var ldl2Filed = (LEAD_LAG_SEC_ORDERField) res;
                var member = (DataTypeMember)TypeMembers["ZetaLag"];
                ((RealField)ldl2Filed.fields[member.FieldIndex].Item1).value = 0.05f;

                 member = (DataTypeMember)TypeMembers["Order"];
                ((Int32Field)ldl2Filed.fields[member.FieldIndex].Item1).value = 2;

                member = (DataTypeMember)TypeMembers["RTSTime"];
                ((Int32Field)ldl2Filed.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is FILTER_LOW_PASS)
            {
                var lpfFiled = (FILTER_LOW_PASSField) res;
                var member = (DataTypeMember)TypeMembers["WLag"];
                ((RealField)lpfFiled.fields[member.FieldIndex].Item1).value = 0.0f;

                member = (DataTypeMember)TypeMembers["Order"];
                ((Int32Field)lpfFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["RTSTime"];
                ((Int32Field)lpfFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is FILTER_NOTCH)
            {
                var ntchFiled = (FILTER_NOTCHField) res;
                var member = (DataTypeMember)TypeMembers["WNotch"];
                ((RealField)ntchFiled.fields[member.FieldIndex].Item1).value = float.MaxValue;

                member = (DataTypeMember)TypeMembers["QFactor"];
                ((RealField)ntchFiled.fields[member.FieldIndex].Item1).value = 0.5f;

                member = (DataTypeMember)TypeMembers["Order"];
                ((Int32Field)ntchFiled.fields[member.FieldIndex].Item1).value = 2;

                member = (DataTypeMember)TypeMembers["RTSTime"];
                ((Int32Field)ntchFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is SELECT_ENHANCED)
            {
                var eselFiled = (SELECT_ENHANCEDField) res;
                var member = (DataTypeMember)TypeMembers["InsUsed"];
                ((Int32Field)eselFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is RATE_LIMITER)
            {
                var rlimFiled = (RATE_LIMITERField) res;
                var member = (DataTypeMember)TypeMembers["RTSTime"];
                ((Int32Field)rlimFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is SELECTABLE_NEGATE)
            {
                var snegFiled = (SELECTABLE_NEGATEField) res;
                var member = (DataTypeMember)TypeMembers["NegateEnable"];
                ((Int8Field)snegFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is SELECTED_SUMMER)
            {
                var ssumFiled = (SELECTED_SUMMERField) res;
                var member = (DataTypeMember)TypeMembers["Gain1"];
                ((RealField)ssumFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["Gain2"];
                ((RealField)ssumFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["Gain3"];
                ((RealField)ssumFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["Gain4"];
                ((RealField)ssumFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["Gain5"];
                ((RealField)ssumFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["Gain6"];
                ((RealField)ssumFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["Gain7"];
                ((RealField)ssumFiled.fields[member.FieldIndex].Item1).value = 1;

                member = (DataTypeMember)TypeMembers["Gain8"];
                ((RealField)ssumFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is MOVING_AVERAGE)
            {
                var maveFiled = (MOVING_AVERAGEField) res;
                var member = (DataTypeMember)TypeMembers["SampleEnable"];
                ((Int8Field)maveFiled.fields[member.FieldIndex].Item1).value = 1;

                member=(DataTypeMember)TypeMembers["NumberOfSamples"];
                ((Int32Field)maveFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is MOVING_STD_DEV)
            {
                var mstdFiled = (MOVING_STD_DEVField) res;
                var member = (DataTypeMember)TypeMembers["NumberOfSamples"];
                ((Int32Field)mstdFiled.fields[member.FieldIndex].Item1).value = 1;
            }else if (this is MESSAGE)
            {
                var messageField = (MESSAGEField) res;
                var member = (DataTypeMember)TypeMembers["Flags"];

                var bitMember = (DataTypeMember) TypeMembers["EN_CC"];
                ((Int16Field)messageField.fields[member.FieldIndex].Item1).SetBitValue(bitMember.BitOffset,true);

                bitMember = (DataTypeMember)TypeMembers["REQ_LEN"];
                ((Int16Field)messageField.fields[member.FieldIndex].Item1).SetBitValue(bitMember.BitOffset, true);

                member = (DataTypeMember)TypeMembers["UnconnectedTimeout"];
                ((Int32Field) messageField.fields[member.FieldIndex].Item1).value = 30000000;

                member = (DataTypeMember)TypeMembers["ConnectionRate"];
                ((Int32Field)messageField.fields[member.FieldIndex].Item1).value = 7500000;
            }
        }

        public string EngineeringUnit
        {
            get { return _engineeringUnit; }
            set
            {
                if (_engineeringUnit != value)
                {
                    _engineeringUnit = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string GetDescription(string operand)
        {
            try
            {
                if (string.IsNullOrEmpty(operand)) return Description;
                var s = operand.Split('.');
                if (s.Length < 2) return "";
                foreach (var member in TypeMembers)
                {
                    var pIndex = s[1].IndexOf("[");
                    var baseId = pIndex > 0 ? s[1].Substring(0, pIndex) : s[1];
                    if (member.Name.Equals(baseId, StringComparison.OrdinalIgnoreCase))
                    {
                        var description = member.Description;
                        var aoiMember = member as AoiDataTypeMember;
                        if (aoiMember != null)
                        {

                            var description2 = Tags.Tag.GetChildDescription(aoiMember.AoiTag.Description,
                                aoiMember.AoiTag.DataTypeInfo, aoiMember.AoiTag.ChildDescription,
                                Tags.Tag.GetOperand(string.Join(".", s, 1, s.Length - 1)), false);
                            if (!string.IsNullOrEmpty(description2))
                                description = description2;
                        }
                        if (s.Length == 2)
                        {
                            if (pIndex > 0) return member.DataTypeInfo.DataType.Description;
                            
                            return string.IsNullOrEmpty(description)
                                ? member.DataTypeInfo.DataType.Description
                                : description;
                        }

                        return (member.DataTypeInfo.DataType as CompositiveType)?.GetDescription(string.Join(".",
                                   s.ToArray(), 1, s.Length - 1)) ??
                               description;
                    }

                }

                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return "";
        }
    }
}