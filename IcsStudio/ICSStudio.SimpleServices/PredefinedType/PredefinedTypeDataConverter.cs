using System;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class PredefinedTypeDataConverter
    {
        public static JToken GetL5XData(IDataType type, JToken data)
        {
            if (type is FBD_LOGICAL)
            {
                return GetFBD_LOGICALL5XData(data);
            }
            if (type is FBD_BOOLEAN_AND)
            {
                return GetFBD_BOOLEAN_ANDL5XData(data);
            }
            if (type is FBD_BOOLEAN_NOT)
            {
                return GetFBD_BOOLEAN_NOTL5XData(data);
            }
            if (type is FBD_BOOLEAN_OR)
            {
                return GetFBD_BOOLEAN_ORL5XData(data);
            }
            if (type is FBD_BOOLEAN_XOR)
            {
                return GetFBD_BOOLEAN_XORL5XData(data);
            }
            if (type is FBD_MATH_ADVANCED)
            {
                return GetFBD_MATH_ADVANCEDL5XData(data);
            }
            if (type is FBD_COMPARE)
            {
                return GetFBD_COMPAREL5XData(data);
            }
            if (type is FBD_CONVERT)
            {
                return GetFBD_CONVERTL5XData(data);
            }
            if (type is FBD_MASK_EQUAL)
            {
                return GetFBD_MASK_EQUALL5XData(data);
            }
            if (type is FBD_TRUNCATE)
            {
                return GetFBD_TRUNCATEL5XData(data);
            }
            if (type is RAMP_SOAK)
            {
                return GetRAMP_SOAKL5XData(data);
            }
            if (type is PID_ENHANCED)
            {
                return GetPID_ENHANCEDL5XData(data);
            }
            if (type is CC)
            {
                return GetCCL5XData(data);
            }
            if (type is DISCRETE_2STATE)
            {
                return GetDISCRETE_2STATEL5XData(data);
            }
            if (type is DISCRETE_3STATE)
            {
                return GetDISCRETE_3STATEL5XData(data);
            }
            if (type is IMC)
            {
                return GetIMCL5XData(data);
            }
            if (type is DEADTIME)
            {
                return GetDEADTIMEL5XData(data);
            }
            if (type is SELECT_ENHANCED)
            {
                return GetSELECT_ENHANCEDL5XData(data);
            }
            if (type is FUNCTION_GENERATOR)
            {
                return GetFUNCTION_GENERATORL5XData(data);
            }
            if (type is FLIP_FLOP_JK)
            {
                return GetFLIP_FLOP_JKL5XData(data);
            }
            if (type is LEAD_LAG_SEC_ORDER)
            {
                return GetLEAD_LAG_SEC_ORDERL5XData(data);
            }
            if (type is MOVING_AVERAGE)
            {
                return GetMOVING_AVERAGEL5XData(data);
            }
            if (type is MMC)
            {
                return GetMMCL5XData(data);
            }
            if (type is FILTER_NOTCH)
            {
                return GetFILTER_NOTCHL5XData(data);
            }
            if (type is DOMINANT_RESET)
            {
                return GetDOMINANT_RESETL5XData(data);
            }
            if (type is TOTALIZER)
            {
                return GetTOTALIZERL5XData(data);
            }
            if (type is UP_DOWN_ACCUM)
            {
                return GetUP_DOWN_ACCUML5XData(data);
            }
            if (type is HL_LIMIT)
            {
                return GetHL_LIMITL5XData(data);
            }
            if (type is FLIP_FLOP_D)
            {
                return GetFLIP_FLOP_DL5XData(data);
            }
            if (type is SELECTED_SUMMER)
            {
                return GetSELECTED_SUMMERL5XData(data);
            }
            if (type is PULSE_MULTIPLIER)
            {
                return GetPULSE_MULTIPLIERL5XData(data);
            }
            if (type is DERIVATIVE)
            {
                return GetDERIVATIVEL5XData(data);
            }
            if (type is SCALE)
            {
                return GetSCALEL5XData(data);
            }
            if (type is PROP_INT)
            {
                return GetPROP_INTL5XData(data);
            }
            if (type is TIMER)
            {
                return GetTIMERL5XData(data);
            }
            if (type is FBD_MASKED_MOVE)
            {
                return GetFBD_MASKED_MOVEL5XData(data);
            }
            if (type is FBD_BIT_FIELD_DISTRIBUTE)
            {
                return GetFBD_BIT_FIELD_DISTRIBUTEL5XData(data);
            }
            if (type is INTEGRATOR)
            {
                return GetINTEGRATORL5XData(data);
            }
            if (type is LEAD_LAG)
            {
                return GetLEAD_LAGL5XData(data);
            }
            if (type is MAXIMUM_CAPTURE)
            {
                return GetMAXIMUM_CAPTUREL5XData(data);
            }
            if (type is MINIMUM_CAPTURE)
            {
                return GetMINIMUM_CAPTUREL5XData(data);
            }
            if (type is RATE_LIMITER)
            {
                return GetRATE_LIMITERL5XData(data);
            }
            if (type is DOMINANT_SET)
            {
                return GetDOMINANT_SETL5XData(data);
            }
            if (type is SELECTABLE_NEGATE)
            {
                return GetSELECTABLE_NEGATEL5XData(data);
            }
            return data;
        }

        private static JToken GetFBD_LOGICALL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 4);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add(data[2]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[3]);
            return res;
        }
        private static JToken GetFBD_BOOLEAN_ANDL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 3);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[0]) >> 3) & 0x01);
            res.Add((((int)data[0]) >> 4) & 0x01);
            res.Add((((int)data[0]) >> 5) & 0x01);
            res.Add((((int)data[0]) >> 6) & 0x01);
            res.Add((((int)data[0]) >> 7) & 0x01);
            res.Add((((int)data[0]) >> 8) & 0x01);
            res.Add((((int)data[1]) >> 0) & 0x01);
            res.Add((((int)data[1]) >> 1) & 0x01);
            return res;
        }
        private static JToken GetFBD_BOOLEAN_NOTL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 3);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[1]) >> 0) & 0x01);
            res.Add((((int)data[1]) >> 1) & 0x01);
            return res;
        }
        private static JToken GetFBD_BOOLEAN_ORL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 3);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[0]) >> 3) & 0x01);
            res.Add((((int)data[0]) >> 4) & 0x01);
            res.Add((((int)data[0]) >> 5) & 0x01);
            res.Add((((int)data[0]) >> 6) & 0x01);
            res.Add((((int)data[0]) >> 7) & 0x01);
            res.Add((((int)data[0]) >> 8) & 0x01);
            res.Add((((int)data[1]) >> 0) & 0x01);
            res.Add((((int)data[1]) >> 1) & 0x01);
            return res;
        }
        private static JToken GetFBD_BOOLEAN_XORL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 3);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[1]) >> 0) & 0x01);
            res.Add((((int)data[1]) >> 1) & 0x01);
            return res;
        }
        private static JToken GetFBD_MATH_ADVANCEDL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 3);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[2]);
            return res;
        }
        private static JToken GetFBD_COMPAREL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 3);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add(data[2]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[0]) >> 2) & 0x01);
            return res;
        }
        private static JToken GetFBD_CONVERTL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 3);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[2]);
            return res;
        }
        private static JToken GetFBD_MASK_EQUALL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 4);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[0]) >> 2) & 0x01);
            return res;
        }
        private static JToken GetFBD_TRUNCATEL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 3);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[2]);
            return res;
        }
        private static JToken GetRAMP_SOAKL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 27);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[2]);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[0]) >> 3) & 0x01);
            res.Add((((int)data[0]) >> 4) & 0x01);
            res.Add((((int)data[0]) >> 5) & 0x01);
            res.Add(data[3]);
            res.Add((((int)data[0]) >> 6) & 0x01);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add(data[6]);
            res.Add(data[7]);
            res.Add(data[8]);
            res.Add(data[9]);
            res.Add(data[10]);
            res.Add((((int)data[0]) >> 7) & 0x01);
            res.Add((((int)data[0]) >> 8) & 0x01);
            res.Add((((int)data[0]) >> 9) & 0x01);
            res.Add((((int)data[0]) >> 10) & 0x01);
            res.Add((((int)data[0]) >> 11) & 0x01);
            res.Add((((int)data[0]) >> 12) & 0x01);
            res.Add((((int)data[0]) >> 13) & 0x01);
            res.Add((((int)data[0]) >> 14) & 0x01);
            res.Add((((int)data[0]) >> 15) & 0x01);
            res.Add((((int)data[0]) >> 16) & 0x01);
            res.Add((((int)data[0]) >> 17) & 0x01);
            res.Add((((int)data[11]) >> 0) & 0x01);
            res.Add(data[12]);
            res.Add(data[13]);
            res.Add(data[14]);
            res.Add((((int)data[11]) >> 1) & 0x01);
            res.Add((((int)data[11]) >> 2) & 0x01);
            res.Add((((int)data[11]) >> 3) & 0x01);
            res.Add(0);
            res.Add((((int)data[11]) >> 5) & 0x01);
            res.Add((((int)data[11]) >> 6) & 0x01);
            res.Add(data[15]);
            return res;
        }
        private static JToken GetPID_ENHANCEDL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 100);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[2]);
            res.Add((((int)data[1]) >> 0) & 0x01);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add(data[6]);
            res.Add(data[7]);
            res.Add(data[8]);
            res.Add(data[9]);
            res.Add((((int)data[1]) >> 1) & 0x01);
            res.Add(data[10]);
            res.Add(data[11]);
            res.Add(data[12]);
            res.Add(data[13]);
            res.Add((((int)data[1]) >> 2) & 0x01);
            res.Add((((int)data[1]) >> 3) & 0x01);
            res.Add(data[14]);
            res.Add(data[15]);
            res.Add(data[16]);
            res.Add(data[17]);
            res.Add(data[18]);
            res.Add((((int)data[1]) >> 4) & 0x01);
            res.Add((((int)data[1]) >> 5) & 0x01);
            res.Add(data[19]);
            res.Add(data[20]);
            res.Add(data[21]);
            res.Add(data[22]);
            res.Add(data[23]);
            res.Add(data[24]);
            res.Add(data[25]);
            res.Add((((int)data[1]) >> 6) & 0x01);
            res.Add(data[26]);
            res.Add((((int)data[1]) >> 7) & 0x01);
            res.Add((((int)data[1]) >> 8) & 0x01);
            res.Add((((int)data[1]) >> 9) & 0x01);
            res.Add((((int)data[1]) >> 10) & 0x01);
            res.Add((((int)data[1]) >> 11) & 0x01);
            res.Add(data[27]);
            res.Add(data[28]);
            res.Add(data[29]);
            res.Add((((int)data[1]) >> 12) & 0x01);
            res.Add((((int)data[1]) >> 13) & 0x01);
            res.Add((((int)data[1]) >> 14) & 0x01);
            res.Add((((int)data[1]) >> 15) & 0x01);
            res.Add(data[30]);
            res.Add((((int)data[1]) >> 16) & 0x01);
            res.Add(data[31]);
            res.Add(data[32]);
            res.Add(data[33]);
            res.Add(data[34]);
            res.Add(data[35]);
            res.Add(data[36]);
            res.Add(data[37]);
            res.Add(data[38]);
            res.Add(data[39]);
            res.Add(data[40]);
            res.Add(data[41]);
            res.Add(data[42]);
            res.Add(data[43]);
            res.Add((((int)data[1]) >> 17) & 0x01);
            res.Add((((int)data[1]) >> 18) & 0x01);
            res.Add((((int)data[1]) >> 19) & 0x01);
            res.Add((((int)data[1]) >> 20) & 0x01);
            res.Add((((int)data[1]) >> 21) & 0x01);
            res.Add((((int)data[1]) >> 22) & 0x01);
            res.Add((((int)data[1]) >> 23) & 0x01);
            res.Add((((int)data[1]) >> 24) & 0x01);
            res.Add((((int)data[1]) >> 25) & 0x01);
            res.Add((((int)data[1]) >> 26) & 0x01);
            res.Add((((int)data[1]) >> 27) & 0x01);
            res.Add((((int)data[1]) >> 28) & 0x01);
            res.Add((((int)data[1]) >> 29) & 0x01);
            res.Add((((int)data[1]) >> 30) & 0x01);
            res.Add((((int)data[1]) >> 31) & 0x01);
            res.Add(data[44]);
            res.Add(data[45]);
            res.Add(data[46]);
            res.Add(data[47]);
            res.Add((((int)data[48]) >> 0) & 0x01);
            res.Add((((int)data[48]) >> 1) & 0x01);
            res.Add((((int)data[48]) >> 2) & 0x01);
            res.Add((((int)data[48]) >> 3) & 0x01);
            res.Add((((int)data[48]) >> 4) & 0x01);
            res.Add((((int)data[49]) >> 0) & 0x01);
            res.Add(data[51]);
            res.Add(data[52]);
            res.Add((((int)data[50]) >> 0) & 0x01);
            res.Add((((int)data[50]) >> 1) & 0x01);
            res.Add((((int)data[50]) >> 2) & 0x01);
            res.Add((((int)data[50]) >> 3) & 0x01);
            res.Add(data[53]);
            res.Add(data[54]);
            res.Add((((int)data[50]) >> 4) & 0x01);
            res.Add((((int)data[50]) >> 5) & 0x01);
            res.Add(data[55]);
            res.Add(data[56]);
            res.Add(data[57]);
            res.Add((((int)data[50]) >> 6) & 0x01);
            res.Add((((int)data[50]) >> 7) & 0x01);
            res.Add((((int)data[50]) >> 8) & 0x01);
            res.Add(data[58]);
            res.Add((((int)data[50]) >> 9) & 0x01);
            res.Add((((int)data[50]) >> 10) & 0x01);
            res.Add((((int)data[50]) >> 11) & 0x01);
            res.Add((((int)data[50]) >> 12) & 0x01);
            res.Add((((int)data[50]) >> 13) & 0x01);
            res.Add((((int)data[50]) >> 14) & 0x01);
            res.Add((((int)data[50]) >> 15) & 0x01);
            res.Add((((int)data[50]) >> 16) & 0x01);
            res.Add((((int)data[50]) >> 17) & 0x01);
            res.Add((((int)data[50]) >> 18) & 0x01);
            res.Add((((int)data[50]) >> 19) & 0x01);
            res.Add((((int)data[50]) >> 20) & 0x01);
            res.Add((((int)data[50]) >> 21) & 0x01);
            res.Add((((int)data[50]) >> 22) & 0x01);
            res.Add((((int)data[50]) >> 23) & 0x01);
            res.Add(0);
            res.Add((((int)data[50]) >> 25) & 0x01);
            res.Add((((int)data[50]) >> 26) & 0x01);
            res.Add((((int)data[50]) >> 27) & 0x01);
            res.Add(data[59]);
            res.Add((((int)data[60]) >> 0) & 0x01);
            res.Add((((int)data[60]) >> 1) & 0x01);
            res.Add((((int)data[60]) >> 2) & 0x01);
            res.Add((((int)data[60]) >> 3) & 0x01);
            res.Add((((int)data[60]) >> 4) & 0x01);
            res.Add(data[61]);
            res.Add(data[62]);
            res.Add(0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0);
            res.Add(0.0);
            res.Add(0.0);
            return res;
        }
        private static JToken GetCCL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 139);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[5]);
            res.Add((((int)data[1]) >> 0) & 0x01);
            res.Add(data[6]);
            res.Add(data[7]);
            res.Add(data[8]);
            res.Add(data[9]);
            res.Add(data[10]);
            res.Add(data[11]);
            res.Add((((int)data[2]) >> 0) & 0x01);
            res.Add((((int)data[3]) >> 0) & 0x01);
            res.Add((((int)data[4]) >> 0) & 0x01);
            res.Add((((int)data[2]) >> 1) & 0x01);
            res.Add((((int)data[3]) >> 1) & 0x01);
            res.Add((((int)data[4]) >> 1) & 0x01);
            res.Add(data[12]);
            res.Add(data[13]);
            res.Add(data[14]);
            res.Add(data[15]);
            res.Add(data[16]);
            res.Add(data[17]);
            res.Add(data[18]);
            res.Add(data[19]);
            res.Add(data[20]);
            res.Add(data[21]);
            res.Add(data[22]);
            res.Add(data[23]);
            res.Add(data[24]);
            res.Add(data[25]);
            res.Add(data[26]);
            res.Add((((int)data[1]) >> 1) & 0x01);
            res.Add(data[27]);
            res.Add(data[28]);
            res.Add(data[29]);
            res.Add(data[30]);
            res.Add(data[31]);
            res.Add(data[32]);
            res.Add(data[33]);
            res.Add(data[34]);
            res.Add(data[35]);
            res.Add(data[36]);
            res.Add(data[37]);
            res.Add(data[38]);
            res.Add(data[39]);
            res.Add(data[40]);
            res.Add(data[41]);
            res.Add(data[42]);
            res.Add(data[43]);
            res.Add(data[44]);
            res.Add(data[45]);
            res.Add(data[46]);
            res.Add(data[47]);
            res.Add((((int)data[2]) >> 2) & 0x01);
            res.Add((((int)data[3]) >> 2) & 0x01);
            res.Add((((int)data[4]) >> 2) & 0x01);
            res.Add(data[48]);
            res.Add(data[49]);
            res.Add(data[50]);
            res.Add((((int)data[2]) >> 3) & 0x01);
            res.Add((((int)data[3]) >> 3) & 0x01);
            res.Add((((int)data[4]) >> 3) & 0x01);
            res.Add((((int)data[2]) >> 4) & 0x01);
            res.Add((((int)data[3]) >> 4) & 0x01);
            res.Add((((int)data[4]) >> 4) & 0x01);
            res.Add((((int)data[1]) >> 2) & 0x01);
            res.Add((((int)data[2]) >> 5) & 0x01);
            res.Add((((int)data[3]) >> 5) & 0x01);
            res.Add((((int)data[4]) >> 5) & 0x01);
            res.Add(data[51]);
            res.Add(data[52]);
            res.Add(data[53]);
            res.Add(data[54]);
            res.Add(data[55]);
            res.Add(data[56]);
            res.Add(data[57]);
            res.Add(data[58]);
            res.Add(data[59]);
            res.Add(data[60]);
            res.Add(data[61]);
            res.Add(data[62]);
            res.Add(data[63]);
            res.Add(data[64]);
            res.Add(data[65]);
            res.Add(data[66]);
            res.Add(data[67]);
            res.Add(data[68]);
            res.Add(data[69]);
            res.Add(data[70]);
            res.Add((((int)data[1]) >> 3) & 0x01);
            res.Add((((int)data[1]) >> 4) & 0x01);
            res.Add((((int)data[1]) >> 5) & 0x01);
            res.Add((((int)data[1]) >> 6) & 0x01);
            res.Add((((int)data[1]) >> 7) & 0x01);
            res.Add((((int)data[2]) >> 6) & 0x01);
            res.Add((((int)data[3]) >> 6) & 0x01);
            res.Add((((int)data[4]) >> 6) & 0x01);
            res.Add((((int)data[2]) >> 7) & 0x01);
            res.Add((((int)data[3]) >> 7) & 0x01);
            res.Add((((int)data[4]) >> 7) & 0x01);
            res.Add((((int)data[2]) >> 8) & 0x01);
            res.Add((((int)data[3]) >> 8) & 0x01);
            res.Add((((int)data[4]) >> 8) & 0x01);
            res.Add((((int)data[2]) >> 9) & 0x01);
            res.Add((((int)data[3]) >> 9) & 0x01);
            res.Add((((int)data[4]) >> 9) & 0x01);
            res.Add((((int)data[1]) >> 8) & 0x01);
            res.Add((((int)data[1]) >> 9) & 0x01);
            res.Add((((int)data[2]) >> 10) & 0x01);
            res.Add((((int)data[3]) >> 10) & 0x01);
            res.Add((((int)data[4]) >> 10) & 0x01);
            res.Add((((int)data[2]) >> 11) & 0x01);
            res.Add((((int)data[3]) >> 11) & 0x01);
            res.Add((((int)data[4]) >> 11) & 0x01);
            res.Add((((int)data[1]) >> 10) & 0x01);
            res.Add(data[71]);
            res.Add(data[72]);
            res.Add(data[73]);
            res.Add(data[74]);
            res.Add(data[75]);
            res.Add(data[76]);
            res.Add(data[77]);
            res.Add(data[78]);
            res.Add(data[79]);
            res.Add(data[80]);
            res.Add(data[81]);
            res.Add(data[82]);
            res.Add(data[83]);
            res.Add((((int)data[2]) >> 12) & 0x01);
            res.Add((((int)data[3]) >> 12) & 0x01);
            res.Add((((int)data[4]) >> 12) & 0x01);
            res.Add(data[84]);
            res.Add((((int)data[85]) >> 0) & 0x01);
            res.Add((((int)data[85]) >> 1) & 0x01);
            res.Add((((int)data[85]) >> 2) & 0x01);
            res.Add((((int)data[85]) >> 3) & 0x01);
            res.Add((((int)data[85]) >> 4) & 0x01);
            res.Add((((int)data[85]) >> 5) & 0x01);
            res.Add((((int)data[85]) >> 6) & 0x01);
            res.Add((((int)data[85]) >> 7) & 0x01);
            res.Add((((int)data[85]) >> 8) & 0x01);
            res.Add((((int)data[86]) >> 0) & 0x01);
            res.Add(data[91]);
            res.Add(data[92]);
            res.Add(data[93]);
            res.Add(data[94]);
            res.Add(data[95]);
            res.Add(data[96]);
            res.Add(data[97]);
            res.Add(data[98]);
            res.Add(data[99]);
            res.Add((((int)data[88]) >> 0) & 0x01);
            res.Add((((int)data[89]) >> 0) & 0x01);
            res.Add((((int)data[90]) >> 0) & 0x01);
            res.Add((((int)data[88]) >> 1) & 0x01);
            res.Add((((int)data[89]) >> 1) & 0x01);
            res.Add((((int)data[90]) >> 1) & 0x01);
            res.Add((((int)data[88]) >> 2) & 0x01);
            res.Add((((int)data[89]) >> 2) & 0x01);
            res.Add((((int)data[90]) >> 2) & 0x01);
            res.Add((((int)data[88]) >> 3) & 0x01);
            res.Add((((int)data[89]) >> 3) & 0x01);
            res.Add((((int)data[90]) >> 3) & 0x01);
            res.Add((((int)data[88]) >> 4) & 0x01);
            res.Add((((int)data[89]) >> 4) & 0x01);
            res.Add((((int)data[90]) >> 4) & 0x01);
            res.Add(data[100]);
            res.Add(data[101]);
            res.Add((((int)data[87]) >> 0) & 0x01);
            res.Add((((int)data[87]) >> 1) & 0x01);
            res.Add(data[102]);
            res.Add(data[103]);
            res.Add(data[104]);
            res.Add((((int)data[88]) >> 5) & 0x01);
            res.Add((((int)data[89]) >> 5) & 0x01);
            res.Add((((int)data[90]) >> 5) & 0x01);
            res.Add((((int)data[88]) >> 6) & 0x01);
            res.Add((((int)data[89]) >> 6) & 0x01);
            res.Add((((int)data[90]) >> 6) & 0x01);
            res.Add((((int)data[87]) >> 2) & 0x01);
            res.Add((((int)data[88]) >> 7) & 0x01);
            res.Add((((int)data[89]) >> 7) & 0x01);
            res.Add((((int)data[90]) >> 7) & 0x01);
            res.Add((((int)data[88]) >> 8) & 0x01);
            res.Add((((int)data[89]) >> 8) & 0x01);
            res.Add((((int)data[90]) >> 8) & 0x01);
            res.Add((((int)data[88]) >> 9) & 0x01);
            res.Add((((int)data[89]) >> 9) & 0x01);
            res.Add((((int)data[90]) >> 9) & 0x01);
            res.Add((((int)data[88]) >> 10) & 0x01);
            res.Add((((int)data[89]) >> 10) & 0x01);
            res.Add((((int)data[90]) >> 10) & 0x01);
            res.Add(data[105]);
            res.Add(data[106]);
            res.Add(data[107]);
            res.Add(data[108]);
            res.Add(data[109]);
            res.Add(data[110]);
            res.Add(data[111]);
            res.Add(data[112]);
            res.Add(data[113]);
            res.Add(data[114]);
            res.Add(data[115]);
            res.Add(data[116]);
            res.Add(data[117]);
            res.Add(data[118]);
            res.Add(data[119]);
            res.Add(data[120]);
            res.Add(data[121]);
            res.Add(data[122]);
            res.Add(data[123]);
            res.Add(data[124]);
            res.Add(data[125]);
            res.Add(data[126]);
            res.Add((((int)data[127]) >> 0) & 0x01);
            res.Add((((int)data[127]) >> 1) & 0x01);
            res.Add((((int)data[127]) >> 2) & 0x01);
            res.Add((((int)data[127]) >> 3) & 0x01);
            res.Add((((int)data[127]) >> 4) & 0x01);
            res.Add((((int)data[127]) >> 5) & 0x01);
            res.Add((((int)data[127]) >> 6) & 0x01);
            res.Add((((int)data[127]) >> 7) & 0x01);
            res.Add((((int)data[127]) >> 8) & 0x01);
            res.Add(data[128]);
            res.Add(data[129]);
            res.Add(data[130]);
            res.Add(data[131]);
            res.Add(data[132]);
            res.Add(data[133]);
            res.Add(data[134]);
            res.Add(data[135]);
            return res;
        }
        private static JToken GetDISCRETE_2STATEL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 10);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[0]) >> 3) & 0x01);
            res.Add((((int)data[0]) >> 4) & 0x01);
            res.Add((((int)data[0]) >> 5) & 0x01);
            res.Add((((int)data[0]) >> 6) & 0x01);
            res.Add((((int)data[0]) >> 7) & 0x01);
            res.Add((((int)data[0]) >> 8) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 9) & 0x01);
            res.Add((((int)data[0]) >> 10) & 0x01);
            res.Add((((int)data[0]) >> 11) & 0x01);
            res.Add((((int)data[0]) >> 12) & 0x01);
            res.Add((((int)data[0]) >> 13) & 0x01);
            res.Add((((int)data[0]) >> 14) & 0x01);
            res.Add((((int)data[0]) >> 15) & 0x01);
            res.Add((((int)data[0]) >> 16) & 0x01);
            res.Add((((int)data[0]) >> 17) & 0x01);
            res.Add((((int)data[0]) >> 18) & 0x01);
            res.Add((((int)data[0]) >> 19) & 0x01);
            res.Add((((int)data[0]) >> 20) & 0x01);
            res.Add((((int)data[0]) >> 21) & 0x01);
            res.Add((((int)data[0]) >> 22) & 0x01);
            res.Add((((int)data[0]) >> 23) & 0x01);
            res.Add((((int)data[0]) >> 24) & 0x01);
            res.Add((((int)data[0]) >> 25) & 0x01);
            res.Add((((int)data[2]) >> 0) & 0x01);
            res.Add((((int)data[2]) >> 1) & 0x01);
            res.Add((((int)data[2]) >> 2) & 0x01);
            res.Add((((int)data[2]) >> 3) & 0x01);
            res.Add((((int)data[2]) >> 4) & 0x01);
            res.Add((((int)data[2]) >> 5) & 0x01);
            res.Add((((int)data[2]) >> 6) & 0x01);
            res.Add((((int)data[2]) >> 7) & 0x01);
            res.Add((((int)data[2]) >> 8) & 0x01);
            res.Add((((int)data[2]) >> 9) & 0x01);
            res.Add(data[3]);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0.0);
            res.Add(0.0);
            return res;
        }
        private static JToken GetDISCRETE_3STATEL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 12);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[0]) >> 3) & 0x01);
            res.Add((((int)data[0]) >> 4) & 0x01);
            res.Add((((int)data[0]) >> 5) & 0x01);
            res.Add((((int)data[0]) >> 6) & 0x01);
            res.Add((((int)data[0]) >> 7) & 0x01);
            res.Add((((int)data[0]) >> 8) & 0x01);
            res.Add((((int)data[0]) >> 9) & 0x01);
            res.Add((((int)data[0]) >> 10) & 0x01);
            res.Add((((int)data[0]) >> 11) & 0x01);
            res.Add((((int)data[0]) >> 12) & 0x01);
            res.Add((((int)data[0]) >> 13) & 0x01);
            res.Add((((int)data[0]) >> 14) & 0x01);
            res.Add((((int)data[0]) >> 15) & 0x01);
            res.Add((((int)data[0]) >> 16) & 0x01);
            res.Add(data[2]);
            res.Add((((int)data[0]) >> 17) & 0x01);
            res.Add((((int)data[0]) >> 18) & 0x01);
            res.Add((((int)data[0]) >> 19) & 0x01);
            res.Add((((int)data[0]) >> 20) & 0x01);
            res.Add((((int)data[0]) >> 21) & 0x01);
            res.Add((((int)data[0]) >> 22) & 0x01);
            res.Add((((int)data[0]) >> 23) & 0x01);
            res.Add((((int)data[0]) >> 24) & 0x01);
            res.Add((((int)data[0]) >> 25) & 0x01);
            res.Add((((int)data[0]) >> 26) & 0x01);
            res.Add((((int)data[0]) >> 27) & 0x01);
            res.Add((((int)data[0]) >> 28) & 0x01);
            res.Add((((int)data[0]) >> 29) & 0x01);
            res.Add(data[3]);
            res.Add((((int)data[0]) >> 30) & 0x01);
            res.Add((((int)data[0]) >> 31) & 0x01);
            res.Add((((int)data[1]) >> 0) & 0x01);
            res.Add((((int)data[1]) >> 1) & 0x01);
            res.Add((((int)data[1]) >> 2) & 0x01);
            res.Add((((int)data[1]) >> 3) & 0x01);
            res.Add((((int)data[1]) >> 4) & 0x01);
            res.Add((((int)data[1]) >> 5) & 0x01);
            res.Add((((int)data[1]) >> 6) & 0x01);
            res.Add((((int)data[1]) >> 7) & 0x01);
            res.Add((((int)data[1]) >> 8) & 0x01);
            res.Add((((int)data[1]) >> 9) & 0x01);
            res.Add((((int)data[1]) >> 10) & 0x01);
            res.Add((((int)data[1]) >> 11) & 0x01);
            res.Add((((int)data[1]) >> 12) & 0x01);
            res.Add((((int)data[1]) >> 13) & 0x01);
            res.Add((((int)data[1]) >> 14) & 0x01);
            res.Add((((int)data[1]) >> 15) & 0x01);
            res.Add((((int)data[1]) >> 16) & 0x01);
            res.Add((((int)data[4]) >> 0) & 0x01);
            res.Add((((int)data[4]) >> 1) & 0x01);
            res.Add((((int)data[4]) >> 2) & 0x01);
            res.Add((((int)data[4]) >> 3) & 0x01);
            res.Add((((int)data[4]) >> 4) & 0x01);
            res.Add((((int)data[4]) >> 5) & 0x01);
            res.Add((((int)data[4]) >> 6) & 0x01);
            res.Add((((int)data[4]) >> 7) & 0x01);
            res.Add((((int)data[4]) >> 8) & 0x01);
            res.Add((((int)data[4]) >> 9) & 0x01);
            res.Add((((int)data[4]) >> 10) & 0x01);
            res.Add((((int)data[4]) >> 11) & 0x01);
            res.Add((((int)data[4]) >> 12) & 0x01);
            res.Add((((int)data[4]) >> 13) & 0x01);
            res.Add((((int)data[4]) >> 14) & 0x01);
            res.Add(data[5]);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0.0);
            res.Add(0.0);
            return res;
        }
        private static JToken GetIMCL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 68);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[2]);
            res.Add((((int)data[1]) >> 0) & 0x01);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add(data[6]);
            res.Add(data[7]);
            res.Add(data[8]);
            res.Add(data[9]);
            res.Add((((int)data[1]) >> 1) & 0x01);
            res.Add(data[10]);
            res.Add(data[11]);
            res.Add(data[12]);
            res.Add(data[13]);
            res.Add((((int)data[1]) >> 2) & 0x01);
            res.Add((((int)data[1]) >> 3) & 0x01);
            res.Add(data[14]);
            res.Add(data[15]);
            res.Add(data[16]);
            res.Add(data[17]);
            res.Add(data[18]);
            res.Add((((int)data[1]) >> 4) & 0x01);
            res.Add(data[19]);
            res.Add(data[20]);
            res.Add(data[21]);
            res.Add(data[22]);
            res.Add(data[23]);
            res.Add(data[24]);
            res.Add(data[25]);
            res.Add((((int)data[1]) >> 5) & 0x01);
            res.Add((((int)data[1]) >> 6) & 0x01);
            res.Add((((int)data[1]) >> 7) & 0x01);
            res.Add((((int)data[1]) >> 8) & 0x01);
            res.Add((((int)data[1]) >> 9) & 0x01);
            res.Add(data[26]);
            res.Add(data[27]);
            res.Add(data[28]);
            res.Add(data[29]);
            res.Add(data[30]);
            res.Add((((int)data[1]) >> 10) & 0x01);
            res.Add((((int)data[1]) >> 11) & 0x01);
            res.Add((((int)data[1]) >> 12) & 0x01);
            res.Add((((int)data[1]) >> 13) & 0x01);
            res.Add((((int)data[1]) >> 14) & 0x01);
            res.Add((((int)data[1]) >> 15) & 0x01);
            res.Add((((int)data[1]) >> 16) & 0x01);
            res.Add((((int)data[1]) >> 17) & 0x01);
            res.Add((((int)data[1]) >> 18) & 0x01);
            res.Add((((int)data[1]) >> 19) & 0x01);
            res.Add((((int)data[1]) >> 20) & 0x01);
            res.Add((((int)data[1]) >> 21) & 0x01);
            res.Add((((int)data[1]) >> 22) & 0x01);
            res.Add((((int)data[1]) >> 23) & 0x01);
            res.Add((((int)data[1]) >> 24) & 0x01);
            res.Add((((int)data[1]) >> 25) & 0x01);
            res.Add((((int)data[1]) >> 26) & 0x01);
            res.Add(data[31]);
            res.Add(data[32]);
            res.Add(data[33]);
            res.Add(data[34]);
            res.Add(data[35]);
            res.Add(data[36]);
            res.Add(data[37]);
            res.Add(data[38]);
            res.Add(data[39]);
            res.Add((((int)data[1]) >> 27) & 0x01);
            res.Add(data[40]);
            res.Add((((int)data[41]) >> 0) & 0x01);
            res.Add((((int)data[41]) >> 1) & 0x01);
            res.Add((((int)data[41]) >> 2) & 0x01);
            res.Add((((int)data[42]) >> 0) & 0x01);
            res.Add(data[44]);
            res.Add(data[45]);
            res.Add(data[46]);
            res.Add((((int)data[43]) >> 0) & 0x01);
            res.Add((((int)data[43]) >> 1) & 0x01);
            res.Add((((int)data[43]) >> 2) & 0x01);
            res.Add((((int)data[43]) >> 3) & 0x01);
            res.Add((((int)data[43]) >> 4) & 0x01);
            res.Add(data[47]);
            res.Add(data[48]);
            res.Add((((int)data[43]) >> 5) & 0x01);
            res.Add((((int)data[43]) >> 6) & 0x01);
            res.Add(data[49]);
            res.Add(data[50]);
            res.Add(data[51]);
            res.Add((((int)data[43]) >> 7) & 0x01);
            res.Add((((int)data[43]) >> 8) & 0x01);
            res.Add((((int)data[43]) >> 9) & 0x01);
            res.Add(data[52]);
            res.Add((((int)data[43]) >> 10) & 0x01);
            res.Add((((int)data[43]) >> 11) & 0x01);
            res.Add((((int)data[43]) >> 12) & 0x01);
            res.Add((((int)data[43]) >> 13) & 0x01);
            res.Add(0);
            res.Add((((int)data[43]) >> 15) & 0x01);
            res.Add((((int)data[43]) >> 16) & 0x01);
            res.Add((((int)data[43]) >> 17) & 0x01);
            res.Add(data[53]);
            res.Add(data[54]);
            res.Add(data[55]);
            res.Add(data[56]);
            res.Add(data[57]);
            res.Add(data[58]);
            res.Add(data[59]);
            res.Add(data[60]);
            res.Add((((int)data[61]) >> 0) & 0x01);
            res.Add((((int)data[61]) >> 1) & 0x01);
            res.Add((((int)data[61]) >> 2) & 0x01);
            res.Add(data[62]);
            res.Add(data[63]);
            res.Add(data[64]);
            return res;
        }
        private static JToken GetDEADTIMEL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 28);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add(data[6]);
            res.Add(data[7]);
            res.Add(data[8]);
            res.Add((((int)data[9]) >> 0) & 0x01);
            res.Add(data[10]);
            res.Add(data[11]);
            res.Add(data[12]);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            return res;
        }
        private static JToken GetSELECT_ENHANCEDL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 16);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add(data[6]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[0]) >> 3) & 0x01);
            res.Add((((int)data[0]) >> 4) & 0x01);
            res.Add((((int)data[0]) >> 5) & 0x01);
            res.Add((((int)data[0]) >> 6) & 0x01);
            res.Add(data[7]);
            res.Add(data[8]);
            res.Add(data[9]);
            res.Add(data[10]);
            res.Add((((int)data[0]) >> 7) & 0x01);
            res.Add((((int)data[0]) >> 8) & 0x01);
            res.Add((((int)data[0]) >> 9) & 0x01);
            res.Add((((int)data[0]) >> 10) & 0x01);
            res.Add((((int)data[0]) >> 11) & 0x01);
            res.Add((((int)data[0]) >> 12) & 0x01);
            res.Add((((int)data[11]) >> 0) & 0x01);
            res.Add(data[12]);
            res.Add(data[13]);
            res.Add((((int)data[11]) >> 1) & 0x01);
            res.Add((((int)data[11]) >> 2) & 0x01);
            res.Add(data[14]);
            res.Add(0);
            return res;
        }
        private static JToken GetFUNCTION_GENERATORL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 13);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[4]) >> 0) & 0x01);
            res.Add(data[5]);
            res.Add(data[6]);
            return res;
        }
        private static JToken GetFLIP_FLOP_JKL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 3);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[0]) >> 3) & 0x01);
            res.Add((((int)data[1]) >> 0) & 0x01);
            res.Add((((int)data[1]) >> 1) & 0x01);
            res.Add((((int)data[1]) >> 2) & 0x01);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            return res;
        }
        private static JToken GetLEAD_LAG_SEC_ORDERL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 44);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add(data[6]);
            res.Add(data[7]);
            res.Add(data[8]);
            res.Add(data[9]);
            res.Add(data[10]);
            res.Add((((int)data[11]) >> 0) & 0x01);
            res.Add(data[12]);
            res.Add(data[13]);
            res.Add(data[14]);
            res.Add(0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0);
            res.Add(0.0);
            res.Add(0.0);
            return res;
        }
        private static JToken GetMOVING_AVERAGEL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 13);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[0]) >> 3) & 0x01);
            res.Add(data[2]);
            res.Add((((int)data[0]) >> 4) & 0x01);
            res.Add((((int)data[3]) >> 0) & 0x01);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            return res;
        }
        private static JToken GetMMCL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 187);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[5]);
            res.Add(data[6]);
            res.Add((((int)data[1]) >> 0) & 0x01);
            res.Add((((int)data[1]) >> 1) & 0x01);
            res.Add(data[7]);
            res.Add(data[8]);
            res.Add(data[9]);
            res.Add(data[10]);
            res.Add(data[11]);
            res.Add(data[12]);
            res.Add(data[13]);
            res.Add(data[14]);
            res.Add(data[15]);
            res.Add(data[16]);
            res.Add(data[17]);
            res.Add(data[18]);
            res.Add((((int)data[2]) >> 0) & 0x01);
            res.Add((((int)data[3]) >> 0) & 0x01);
            res.Add((((int)data[4]) >> 0) & 0x01);
            res.Add((((int)data[2]) >> 1) & 0x01);
            res.Add((((int)data[3]) >> 1) & 0x01);
            res.Add((((int)data[4]) >> 1) & 0x01);
            res.Add(data[19]);
            res.Add(data[20]);
            res.Add(data[21]);
            res.Add(data[22]);
            res.Add(data[23]);
            res.Add(data[24]);
            res.Add(data[25]);
            res.Add(data[26]);
            res.Add(data[27]);
            res.Add(data[28]);
            res.Add(data[29]);
            res.Add(data[30]);
            res.Add((((int)data[1]) >> 2) & 0x01);
            res.Add(data[31]);
            res.Add(data[32]);
            res.Add(data[33]);
            res.Add(data[34]);
            res.Add(data[35]);
            res.Add(data[36]);
            res.Add(data[37]);
            res.Add(data[38]);
            res.Add(data[39]);
            res.Add(data[40]);
            res.Add(data[41]);
            res.Add(data[42]);
            res.Add(data[43]);
            res.Add(data[44]);
            res.Add(data[45]);
            res.Add(data[46]);
            res.Add(data[47]);
            res.Add(data[48]);
            res.Add(data[49]);
            res.Add(data[50]);
            res.Add(data[51]);
            res.Add((((int)data[2]) >> 2) & 0x01);
            res.Add((((int)data[3]) >> 2) & 0x01);
            res.Add((((int)data[4]) >> 2) & 0x01);
            res.Add(data[52]);
            res.Add(data[53]);
            res.Add(data[54]);
            res.Add((((int)data[2]) >> 3) & 0x01);
            res.Add((((int)data[3]) >> 3) & 0x01);
            res.Add((((int)data[4]) >> 3) & 0x01);
            res.Add((((int)data[2]) >> 4) & 0x01);
            res.Add((((int)data[3]) >> 4) & 0x01);
            res.Add((((int)data[4]) >> 4) & 0x01);
            res.Add((((int)data[1]) >> 3) & 0x01);
            res.Add((((int)data[2]) >> 5) & 0x01);
            res.Add((((int)data[3]) >> 5) & 0x01);
            res.Add((((int)data[4]) >> 5) & 0x01);
            res.Add((((int)data[2]) >> 6) & 0x01);
            res.Add((((int)data[3]) >> 6) & 0x01);
            res.Add((((int)data[4]) >> 6) & 0x01);
            res.Add(data[55]);
            res.Add(data[56]);
            res.Add(data[57]);
            res.Add(data[58]);
            res.Add(data[59]);
            res.Add(data[60]);
            res.Add(data[61]);
            res.Add(data[62]);
            res.Add(data[63]);
            res.Add(data[64]);
            res.Add(data[65]);
            res.Add(data[66]);
            res.Add(data[67]);
            res.Add(data[68]);
            res.Add(data[69]);
            res.Add(data[70]);
            res.Add(data[71]);
            res.Add(data[72]);
            res.Add(data[73]);
            res.Add(data[74]);
            res.Add(data[75]);
            res.Add(data[76]);
            res.Add(data[77]);
            res.Add(data[78]);
            res.Add(data[79]);
            res.Add(data[80]);
            res.Add(data[81]);
            res.Add(data[82]);
            res.Add(data[83]);
            res.Add(data[84]);
            res.Add(data[85]);
            res.Add(data[86]);
            res.Add(data[87]);
            res.Add((((int)data[1]) >> 4) & 0x01);
            res.Add((((int)data[1]) >> 5) & 0x01);
            res.Add((((int)data[1]) >> 6) & 0x01);
            res.Add((((int)data[1]) >> 7) & 0x01);
            res.Add((((int)data[2]) >> 7) & 0x01);
            res.Add((((int)data[3]) >> 7) & 0x01);
            res.Add((((int)data[4]) >> 7) & 0x01);
            res.Add((((int)data[2]) >> 8) & 0x01);
            res.Add((((int)data[3]) >> 8) & 0x01);
            res.Add((((int)data[4]) >> 8) & 0x01);
            res.Add((((int)data[2]) >> 9) & 0x01);
            res.Add((((int)data[3]) >> 9) & 0x01);
            res.Add((((int)data[4]) >> 9) & 0x01);
            res.Add((((int)data[2]) >> 10) & 0x01);
            res.Add((((int)data[3]) >> 10) & 0x01);
            res.Add((((int)data[4]) >> 10) & 0x01);
            res.Add((((int)data[1]) >> 8) & 0x01);
            res.Add((((int)data[1]) >> 9) & 0x01);
            res.Add((((int)data[2]) >> 11) & 0x01);
            res.Add((((int)data[3]) >> 11) & 0x01);
            res.Add((((int)data[4]) >> 11) & 0x01);
            res.Add((((int)data[2]) >> 12) & 0x01);
            res.Add((((int)data[3]) >> 12) & 0x01);
            res.Add((((int)data[4]) >> 12) & 0x01);
            res.Add((((int)data[1]) >> 10) & 0x01);
            res.Add(data[88]);
            res.Add(data[89]);
            res.Add(data[90]);
            res.Add(data[91]);
            res.Add(data[92]);
            res.Add(data[93]);
            res.Add(data[94]);
            res.Add(data[95]);
            res.Add(data[96]);
            res.Add(data[97]);
            res.Add(data[98]);
            res.Add(data[99]);
            res.Add(data[100]);
            res.Add(data[101]);
            res.Add(data[102]);
            res.Add(data[103]);
            res.Add(data[104]);
            res.Add(data[105]);
            res.Add(data[106]);
            res.Add((((int)data[2]) >> 13) & 0x01);
            res.Add((((int)data[3]) >> 13) & 0x01);
            res.Add((((int)data[4]) >> 13) & 0x01);
            res.Add((((int)data[2]) >> 14) & 0x01);
            res.Add((((int)data[3]) >> 14) & 0x01);
            res.Add((((int)data[4]) >> 14) & 0x01);
            res.Add(data[107]);
            res.Add(data[108]);
            res.Add((((int)data[109]) >> 0) & 0x01);
            res.Add((((int)data[109]) >> 1) & 0x01);
            res.Add((((int)data[109]) >> 2) & 0x01);
            res.Add((((int)data[109]) >> 3) & 0x01);
            res.Add((((int)data[109]) >> 4) & 0x01);
            res.Add((((int)data[109]) >> 5) & 0x01);
            res.Add((((int)data[109]) >> 6) & 0x01);
            res.Add((((int)data[109]) >> 7) & 0x01);
            res.Add((((int)data[109]) >> 8) & 0x01);
            res.Add((((int)data[109]) >> 9) & 0x01);
            res.Add((((int)data[109]) >> 10) & 0x01);
            res.Add((((int)data[109]) >> 11) & 0x01);
            res.Add((((int)data[110]) >> 0) & 0x01);
            res.Add(data[115]);
            res.Add(data[116]);
            res.Add(data[117]);
            res.Add(data[118]);
            res.Add(data[119]);
            res.Add(data[120]);
            res.Add((((int)data[112]) >> 0) & 0x01);
            res.Add((((int)data[113]) >> 0) & 0x01);
            res.Add((((int)data[114]) >> 0) & 0x01);
            res.Add((((int)data[112]) >> 1) & 0x01);
            res.Add((((int)data[113]) >> 1) & 0x01);
            res.Add((((int)data[114]) >> 1) & 0x01);
            res.Add((((int)data[112]) >> 2) & 0x01);
            res.Add((((int)data[113]) >> 2) & 0x01);
            res.Add((((int)data[114]) >> 2) & 0x01);
            res.Add((((int)data[112]) >> 3) & 0x01);
            res.Add((((int)data[113]) >> 3) & 0x01);
            res.Add((((int)data[114]) >> 3) & 0x01);
            res.Add((((int)data[112]) >> 4) & 0x01);
            res.Add((((int)data[113]) >> 4) & 0x01);
            res.Add((((int)data[114]) >> 4) & 0x01);
            res.Add(data[121]);
            res.Add(data[122]);
            res.Add(data[123]);
            res.Add(data[124]);
            res.Add((((int)data[111]) >> 0) & 0x01);
            res.Add((((int)data[111]) >> 1) & 0x01);
            res.Add((((int)data[111]) >> 2) & 0x01);
            res.Add((((int)data[111]) >> 3) & 0x01);
            res.Add(data[125]);
            res.Add(data[126]);
            res.Add(data[127]);
            res.Add(data[128]);
            res.Add(data[129]);
            res.Add(data[130]);
            res.Add((((int)data[112]) >> 5) & 0x01);
            res.Add((((int)data[113]) >> 5) & 0x01);
            res.Add((((int)data[114]) >> 5) & 0x01);
            res.Add((((int)data[112]) >> 6) & 0x01);
            res.Add((((int)data[113]) >> 6) & 0x01);
            res.Add((((int)data[114]) >> 6) & 0x01);
            res.Add((((int)data[111]) >> 4) & 0x01);
            res.Add((((int)data[112]) >> 7) & 0x01);
            res.Add((((int)data[113]) >> 7) & 0x01);
            res.Add((((int)data[114]) >> 7) & 0x01);
            res.Add((((int)data[112]) >> 8) & 0x01);
            res.Add((((int)data[113]) >> 8) & 0x01);
            res.Add((((int)data[114]) >> 8) & 0x01);
            res.Add((((int)data[112]) >> 9) & 0x01);
            res.Add((((int)data[113]) >> 9) & 0x01);
            res.Add((((int)data[114]) >> 9) & 0x01);
            res.Add((((int)data[112]) >> 10) & 0x01);
            res.Add((((int)data[113]) >> 10) & 0x01);
            res.Add((((int)data[114]) >> 10) & 0x01);
            res.Add(data[131]);
            res.Add(data[132]);
            res.Add(data[133]);
            res.Add(data[134]);
            res.Add(data[135]);
            res.Add(data[136]);
            res.Add(data[137]);
            res.Add(data[138]);
            res.Add(data[139]);
            res.Add(data[140]);
            res.Add(data[141]);
            res.Add(data[142]);
            res.Add(data[143]);
            res.Add(data[144]);
            res.Add(data[145]);
            res.Add(data[146]);
            res.Add(data[147]);
            res.Add(data[148]);
            res.Add(data[149]);
            res.Add(data[150]);
            res.Add(data[151]);
            res.Add(data[152]);
            res.Add(data[153]);
            res.Add(data[154]);
            res.Add(data[155]);
            res.Add(data[156]);
            res.Add(data[157]);
            res.Add(data[158]);
            res.Add(data[159]);
            res.Add(data[160]);
            res.Add(data[161]);
            res.Add(data[162]);
            res.Add(data[163]);
            res.Add(data[164]);
            res.Add(data[165]);
            res.Add(data[166]);
            res.Add(data[167]);
            res.Add(data[168]);
            res.Add(data[169]);
            res.Add(data[170]);
            res.Add((((int)data[171]) >> 0) & 0x01);
            res.Add((((int)data[171]) >> 1) & 0x01);
            res.Add((((int)data[171]) >> 2) & 0x01);
            res.Add((((int)data[171]) >> 3) & 0x01);
            res.Add((((int)data[171]) >> 4) & 0x01);
            res.Add((((int)data[171]) >> 5) & 0x01);
            res.Add((((int)data[171]) >> 6) & 0x01);
            res.Add((((int)data[171]) >> 7) & 0x01);
            res.Add((((int)data[171]) >> 8) & 0x01);
            res.Add((((int)data[172]) >> 0) & 0x01);
            res.Add((((int)data[172]) >> 1) & 0x01);
            res.Add((((int)data[172]) >> 2) & 0x01);
            res.Add((((int)data[172]) >> 3) & 0x01);
            res.Add((((int)data[172]) >> 4) & 0x01);
            res.Add((((int)data[172]) >> 5) & 0x01);
            res.Add((((int)data[172]) >> 6) & 0x01);
            res.Add((((int)data[172]) >> 7) & 0x01);
            res.Add((((int)data[172]) >> 8) & 0x01);
            res.Add(data[173]);
            res.Add(data[174]);
            res.Add(data[175]);
            res.Add(data[176]);
            res.Add(data[177]);
            res.Add(data[178]);
            res.Add(data[179]);
            res.Add(data[180]);
            res.Add(data[181]);
            res.Add(data[182]);
            res.Add(data[183]);
            res.Add(0);
            return res;
        }
        private static JToken GetFILTER_NOTCHL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 46);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add(data[6]);
            res.Add(data[7]);
            res.Add(data[8]);
            res.Add((((int)data[9]) >> 0) & 0x01);
            res.Add(data[10]);
            res.Add(data[11]);
            res.Add(data[12]);
            res.Add(0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0.0);
            return res;
        }
        private static JToken GetDOMINANT_RESETL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 3);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[1]) >> 0) & 0x01);
            res.Add((((int)data[1]) >> 1) & 0x01);
            res.Add((((int)data[1]) >> 2) & 0x01);
            return res;
        }
        private static JToken GetTOTALIZERL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 29);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add(data[6]);
            res.Add(data[7]);
            res.Add(data[8]);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[0]) >> 3) & 0x01);
            res.Add((((int)data[0]) >> 4) & 0x01);
            res.Add((((int)data[0]) >> 5) & 0x01);
            res.Add((((int)data[0]) >> 6) & 0x01);
            res.Add((((int)data[0]) >> 7) & 0x01);
            res.Add((((int)data[0]) >> 8) & 0x01);
            res.Add((((int)data[0]) >> 9) & 0x01);
            res.Add((((int)data[0]) >> 10) & 0x01);
            res.Add((((int)data[0]) >> 11) & 0x01);
            res.Add((((int)data[0]) >> 12) & 0x01);
            res.Add(data[9]);
            res.Add(data[10]);
            res.Add(data[11]);
            res.Add(data[12]);
            res.Add((((int)data[13]) >> 0) & 0x01);
            res.Add(data[14]);
            res.Add(data[15]);
            res.Add((((int)data[13]) >> 1) & 0x01);
            res.Add((((int)data[13]) >> 2) & 0x01);
            res.Add((((int)data[13]) >> 3) & 0x01);
            res.Add((((int)data[13]) >> 4) & 0x01);
            res.Add((((int)data[13]) >> 5) & 0x01);
            res.Add((((int)data[13]) >> 6) & 0x01);
            res.Add((((int)data[13]) >> 7) & 0x01);
            res.Add(data[16]);
            res.Add(data[17]);
            res.Add(0);
            res.Add(0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0);
            res.Add(0);
            res.Add(0.0);
            res.Add(0);
            res.Add(0.0);
            return res;
        }
        private static JToken GetUP_DOWN_ACCUML5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 8);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[1]);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[4]) >> 0) & 0x01);
            res.Add(data[5]);
            res.Add(0);
            return res;
        }
        private static JToken GetHL_LIMITL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 9);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add((((int)data[5]) >> 0) & 0x01);
            res.Add(data[6]);
            res.Add((((int)data[5]) >> 1) & 0x01);
            res.Add((((int)data[5]) >> 2) & 0x01);
            res.Add(data[7]);
            return res;
        }
        private static JToken GetFLIP_FLOP_DL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 3);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[0]) >> 3) & 0x01);
            res.Add((((int)data[1]) >> 0) & 0x01);
            res.Add((((int)data[1]) >> 1) & 0x01);
            res.Add((((int)data[1]) >> 2) & 0x01);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            return res;
        }
        private static JToken GetSELECTED_SUMMERL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 21);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add(data[2]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add(data[5]);
            res.Add(data[6]);
            res.Add((((int)data[0]) >> 3) & 0x01);
            res.Add(data[7]);
            res.Add(data[8]);
            res.Add((((int)data[0]) >> 4) & 0x01);
            res.Add(data[9]);
            res.Add(data[10]);
            res.Add((((int)data[0]) >> 5) & 0x01);
            res.Add(data[11]);
            res.Add(data[12]);
            res.Add((((int)data[0]) >> 6) & 0x01);
            res.Add(data[13]);
            res.Add(data[14]);
            res.Add((((int)data[0]) >> 7) & 0x01);
            res.Add(data[15]);
            res.Add(data[16]);
            res.Add((((int)data[0]) >> 8) & 0x01);
            res.Add(data[17]);
            res.Add((((int)data[18]) >> 0) & 0x01);
            res.Add(data[19]);
            return res;
        }
        private static JToken GetPULSE_MULTIPLIERL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 12);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[2]);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add((((int)data[5]) >> 0) & 0x01);
            res.Add(data[6]);
            res.Add(data[7]);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            res.Add(0);
            return res;
        }
        private static JToken GetDERIVATIVEL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 22);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add(data[2]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add(data[6]);
            res.Add((((int)data[7]) >> 0) & 0x01);
            res.Add(data[8]);
            res.Add(data[9]);
            res.Add(data[10]);
            res.Add(0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0);
            return res;
        }
        private static JToken GetSCALEL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 13);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[6]) >> 0) & 0x01);
            res.Add(data[7]);
            res.Add((((int)data[6]) >> 1) & 0x01);
            res.Add((((int)data[6]) >> 2) & 0x01);
            res.Add(data[8]);
            return res;
        }
        private static JToken GetPROP_INTL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 34);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add(data[6]);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[0]) >> 3) & 0x01);
            res.Add(data[7]);
            res.Add(data[8]);
            res.Add(data[9]);
            res.Add(data[10]);
            res.Add(data[11]);
            res.Add(data[12]);
            res.Add((((int)data[0]) >> 4) & 0x01);
            res.Add((((int)data[0]) >> 5) & 0x01);
            res.Add(data[13]);
            res.Add(data[14]);
            res.Add(data[15]);
            res.Add(data[16]);
            res.Add((((int)data[17]) >> 0) & 0x01);
            res.Add(data[18]);
            res.Add((((int)data[17]) >> 1) & 0x01);
            res.Add((((int)data[17]) >> 2) & 0x01);
            res.Add(data[19]);
            res.Add(data[20]);
            res.Add(0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0);
            res.Add(0.0);
            return res;
        }
        private static JToken GetTIMERL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 3);
            res.Add(data[1]);
            res.Add(data[2]);
            res.Add((((int)data[0]) >> 31) & 0x01);
            res.Add((((int)data[0]) >> 30) & 0x01);
            res.Add((((int)data[0]) >> 29) & 0x01);
            res.Add(0);
            return res;
        }
        private static JToken GetFBD_MASKED_MOVEL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 5);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[4]);
            return res;
        }
        private static JToken GetFBD_BIT_FIELD_DISTRIBUTEL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 7);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[6]);
            return res;
        }
        private static JToken GetINTEGRATORL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 22);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[0]) >> 3) & 0x01);
            res.Add(data[6]);
            res.Add(data[7]);
            res.Add(data[8]);
            res.Add(data[9]);
            res.Add((((int)data[10]) >> 0) & 0x01);
            res.Add(data[11]);
            res.Add((((int)data[10]) >> 1) & 0x01);
            res.Add((((int)data[10]) >> 2) & 0x01);
            res.Add(data[12]);
            res.Add(data[13]);
            res.Add(0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0);
            res.Add(0.0);
            return res;
        }
        private static JToken GetLEAD_LAGL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 33);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add(data[6]);
            res.Add(data[7]);
            res.Add(data[8]);
            res.Add(data[9]);
            res.Add((((int)data[10]) >> 0) & 0x01);
            res.Add(data[11]);
            res.Add(data[12]);
            res.Add(data[13]);
            res.Add(0);
            res.Add(0.0);
            res.Add(0.0);
            res.Add(0);
            res.Add(0.0);
            return res;
        }
        private static JToken GetMAXIMUM_CAPTUREL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 7);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[2]);
            res.Add((((int)data[3]) >> 0) & 0x01);
            res.Add(data[4]);
            res.Add(0);
            res.Add(0.0);
            return res;
        }
        private static JToken GetMINIMUM_CAPTUREL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 7);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[2]);
            res.Add((((int)data[3]) >> 0) & 0x01);
            res.Add(data[4]);
            res.Add(0);
            res.Add(0.0);
            return res;
        }
        private static JToken GetRATE_LIMITERL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 24);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add(data[2]);
            res.Add(data[3]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add(data[4]);
            res.Add(data[5]);
            res.Add(data[6]);
            res.Add(data[7]);
            res.Add((((int)data[8]) >> 0) & 0x01);
            res.Add(data[9]);
            res.Add(data[10]);
            res.Add(data[11]);
            res.Add(0);
            res.Add(0.0);
            res.Add(0);
            res.Add(0.0);
            return res;
        }
        private static JToken GetDOMINANT_SETL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 3);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[0]) >> 2) & 0x01);
            res.Add((((int)data[1]) >> 0) & 0x01);
            res.Add((((int)data[1]) >> 1) & 0x01);
            res.Add((((int)data[1]) >> 2) & 0x01);
            return res;
        }
        private static JToken GetSELECTABLE_NEGATEL5XData(JToken data)
        {
            var res = new JArray();
            Debug.Assert((data is JArray) && ((JArray)data).Count == 5);
            res.Add((((int)data[0]) >> 0) & 0x01);
            res.Add(data[1]);
            res.Add((((int)data[0]) >> 1) & 0x01);
            res.Add((((int)data[2]) >> 0) & 0x01);
            res.Add(data[3]);
            return res;
        }
    }
}
