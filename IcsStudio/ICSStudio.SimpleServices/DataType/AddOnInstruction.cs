using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.DataType
{
    public class History
    {
        public string User { set; get; }
        public string SignatureID { set; get; }
        public string Timestamp { set; get; }
        public string Description { set; get; }
    }

    public static class AOIExtend
    {
        public static bool VerifyLocalDataType(string dataType)
        {
            var type = Controller.GetInstance().DataTypes[dataType];
            if (type == null) return false;
            if (type is MESSAGE) return false;

            if (type is ALARM_ANALOG || type is ALARM_DIGITAL) return false;
            if (type is AXIS_COMMON || type is AXIS_CONSUMED || type is AXIS_GENERIC || type is AXIS_GENERIC_DRIVE ||
                type is AXIS_SERVO || type is AXIS_SERVO_DRIVE) return false;

            if (type is COORDINATE_SYSTEM) return false;
            if (type is ENERGY_BASE || type is ENERGY_ELECTRICAL) return false;
            if (type is HMIBC) return false;
            if (type is MOTION_GROUP) return false;
            return true;
        }

        public static void ToAOIParameterConfig(JArray parameters, IEnumerable collection)
        {
            foreach (var tag in collection)
            {
                var item = (Tag) tag;
                JObject parameter = new JObject();
                parameter["Name"] = item.Name ?? "";
                parameter["Usage"] = (byte) item.Usage;
                parameter["DataType"] = item.DataTypeInfo.DataType.Name ?? "";
                string dims = "";
                if (item.DataTypeInfo.Dim3 > 0)
                {
                    dims = $"{item.DataTypeInfo.Dim3} {item.DataTypeInfo.Dim2} {item.DataTypeInfo.Dim1}";
                }
                else if (item.DataTypeInfo.Dim2 > 0)
                {
                    dims = $"{item.DataTypeInfo.Dim2} {item.DataTypeInfo.Dim1}";
                }
                else if (item.DataTypeInfo.Dim1 > 0)
                {
                    dims = $"{item.DataTypeInfo.Dim1}";
                }

                if (dims != "")
                    parameter["Dimensions"] = dims;
                if (item.Usage != Usage.InOut)
                {
                    var value = item.DataWrapper.Data?.ToJToken();

                    parameter["DefaultData"] = value;
                    parameter["ExternalAccess"] = (byte) item.ExternalAccess;
                }

                parameter["Radix"] = (byte) item.DisplayStyle;
                parameter["Required"] = item.IsRequired;
                parameter["Visible"] = item.IsVisible;
                parameter["Description"] = item.Description ?? "";
                parameter["Constant"] = item.IsConstant;
                if (item.ChildDescription != null && item.ChildDescription is JArray &&
                    ((JArray) item.ChildDescription).Count > 0)
                {
                    parameter["Comments"] = item.ChildDescription;
                }

                parameters.Add(parameter);
                //TODO(ZYL):AliasFor

            }
        }

        public static void ToAOILocalTagConfig(JArray localTags, IEnumerable collection)
        {
            foreach (var tag in collection)
            {
                var item = (Tag) tag;
                JObject localTag = new JObject();
                localTag["Name"] = item.Name ?? "";
                localTag["DataType"] = item.DataTypeInfo.DataType.Name;
                localTag["Dimensions"] = item.DataTypeInfo.Dim1;
                localTag["Description"] = item.Description ?? "";
                localTag["Radix"] = (byte) item.DisplayStyle;
                if (item.ChildDescription != null && item.ChildDescription is JArray &&
                    ((JArray) item.ChildDescription).Count > 0)
                {
                    localTag["Comments"] = item.ChildDescription;
                }

                localTag["DefaultData"] = item.DataWrapper.Data?.ToJToken();
                localTag["ExternalAccess"] = (byte) item.ExternalAccess;
                localTags.Add(localTag);
            }
        }

        public static void ToAOIRoutineConfig(JArray routines, IEnumerable collection)
        {
            foreach (IRoutine item in collection)
            {
                JObject routine = new JObject();
                routine["Name"] = item.Name;
                routine["Type"] = (byte) item.Type;
                routine["Description"] = item.Description;
                if (item.Type == RoutineType.RLL)
                {
                    var codes = new JArray();
                    if ((item as RLLRoutine)?.CodeText != null)
                    {
                        foreach (var item2 in (item as RLLRoutine).CodeText)
                        {
                            var value = new JValue(item2);
                            codes.Add(value);
                        }

                        routine["CodeText"] = codes;
                    }
                }
                else if (item.Type == RoutineType.ST)
                {
                    var codes = new JArray();
                    if ((item as STRoutine)?.CodeText != null)
                    {
                        foreach (var item2 in (item as STRoutine).CodeText)
                        {
                            var value = new JValue(item2);
                            codes.Add(value);
                        }

                        routine["CodeText"] = codes;
                    }
                }
                else
                {
                    Debug.Assert(false, item.Type.ToString());
                }

                routines.Add(routine);
            }
        }

        public static JToken ConvertDecoratedToken(int dim, JToken data, IController controller, string dataType)
        {
            if (dim != 0 && dataType == "BOOL" && dim % 32 == 0)
            {
                var defaultData = ChangeToBOOLFiled(data as JArray, controller);
                return defaultData;
            }
            else
            {
                if (((JArray) data).Count == 1 && data[0] is JObject)
                {
                    if (data[0]?["Format"] != null)
                    {
                        return data[0]["Value"];
                    }
                    else
                    {
                        JValue jValue = ConvertToInt((data as JArray)[0] as JObject,
                            controller);
                        if ("bool".Equals(data[0]["DataType"]?.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            var m = new JObject();
                            m["Value"] = data[0]["Value"];
                            m["isArray"] = "Element".Equals(data[0]["Type"]?.ToString());
                            return m;
                        }
                        else
                        {
                            if (dim != 0 || controller.DataTypes[dataType?.ToString()] is CompositiveType)
                            {
                                return new JArray(jValue);
                            }
                            else
                            {
                                return jValue;
                            }
                        }
                    }

                }
                else
                {
                    var defaultData = new JArray();
                    foreach (var item2 in data)
                    {
                        if (dim > 0)
                        {
                            if (item2 is JObject)
                            {
                                GetArrayValue(defaultData, item2, controller);
                            }
                            else
                            {
                                JArray array = new JArray();
                                GetArrayValue(array, item2, controller);
                                defaultData.Add(array);
                            }
                        }
                        else
                        {
                            if (item2 is JArray)
                            {
                                JArray newArray = new JArray();
                                GetArrayValue(newArray, item2, controller);
                                defaultData.Add(newArray);
                            }
                            else
                            {
                                GetArrayValue(defaultData, item2, controller);
                            }
                        }

                    }

                    if (controller.DataTypes[dataType?.ToString()] is AOIDataType &&
                        ((AOIDataType) controller.DataTypes[dataType?.ToString()]).TypeMembers
                        .Count != defaultData.Count)
                    {
                        var aoi = (controller.AOIDefinitionCollection as AoiDefinitionCollection)?.Find(
                            dataType);
                        var localTags = aoi?.Tags.Where(t => t.Usage == Usage.Local).ToList();
                        if (localTags != null)
                            foreach (var l in localTags)
                            {
                                defaultData.Add((l as Tag)?.DataWrapper.Data.ToJToken());
                            }
                    }

                    return defaultData;
                }
            }
        }

        public static JValue ConvertToInt(JObject member, IController controller)
        {
            JValue jValue = null;
            var dataType = controller.DataTypes[member["DataType"]?.ToString()];
            var value = member["Value"]?.ToString();
            if (DisplayStyle.Exponential.ToString()
                    .Equals(member["Radix"]?.ToString(), StringComparison.OrdinalIgnoreCase) ||
                DisplayStyle.Float.ToString().Equals(member["Radix"]?.ToString(), StringComparison.OrdinalIgnoreCase)
                || ((string) member["DataType"]).Equals("REAL", StringComparison.OrdinalIgnoreCase))
            {
                var v = member["Value"];
                if (v.Type == JTokenType.String)
                {
                    if ("1.$".Equals((string) v))
                    {
                        jValue=new JValue(float.PositiveInfinity);
                    }
                    else if ("-1.$".Equals((string) v))
                    {
                        jValue = new JValue(float.NegativeInfinity);
                    }
                    else if ("1.#QNAN".Equals((string) v))
                    {
                        jValue=new JValue(float.NaN);
                    }
                    else if ("-1.#QNAN".Equals((string)v))
                    {
                        jValue = new JValue(float.NaN);
                    }
                    else
                    {
                        jValue = new JValue(float.Parse((string)member["Value"]));
                    }
                }
                else
                {
                    jValue = new JValue(float.Parse((string)member["Value"]));
                }
            }
            else if (value.IndexOf("2#") > -1)
            {
                jValue = new JValue(GetIntValue(value.Replace("2#", "").Replace("_", ""), dataType, 2));
            }
            else if (value.IndexOf("8#") > -1)
            {
                jValue = new JValue(GetIntValue(value.Replace("8#", "").Replace("_", ""), dataType, 8));
            }
            else if (value.IndexOf("16#") > -1)
            {
                jValue = new JValue(GetIntValue(value.Replace("16#", "").Replace("_", ""), dataType,
                    16));
            }
            else if (DisplayStyle.Ascii.ToString()
                .Equals(member["Radix"]?.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                value = FormatOp.RemoveFormat(value);
                try
                {
                    //jValue = new JValue(AsciiToInt(value, dataType.Name, controller));
                    jValue = new JValue(ValueConverter.ToInt(value, DisplayStyle.Ascii));
                }
                catch (Exception)
                {
                    jValue = new JValue(FormatOp.FormatSpecial(value));
                }
            }
            else
            {
                jValue = new JValue(Convert.ToInt32(value));
            }

            return jValue;
        }

        public static long GetIntValue(string value, IDataType dataType, int fromBase)
        {
            if (dataType is SINT)
            {
                return Convert.ToSByte(value, fromBase);
            }

            if (dataType is INT)
            {
                return Convert.ToInt16(value, fromBase);
            }

            if (dataType is DINT)
            {
                return Convert.ToInt32(value, fromBase);
            }

            if (dataType is LINT)
            {
                return Convert.ToInt64(value, fromBase);
            }

            return 0;
        }

        public static JArray ChangeToBOOLFiled(JArray array, IController controller)
        {
            var values = new JArray();
            string binary = "";
            int count = 0;
            foreach (var jToken in array)
            {
                var data = (JObject)jToken;
                binary = ConvertToInt(data, controller) + binary;
                count++;
                if (count == 32)
                {
                    values.Add((int)Convert.ToUInt32(binary, 2));
                    binary = "";
                    count = 0;
                }
            }
            
            return values;
        }

        public static void GetArrayValue(JArray defaultData, JToken values, IController controller)
        {
            if (values is JValue)
            {
                return;
            }
            var o = values as JObject;
            if (o != null)
            {
                if ("DATA".Equals(o["Name"]?.ToString()) && "ASCII".Equals(o["Radix"]?.ToString()))
                {
                    var array=new JArray();
                    foreach (var b in ValueConverter.ToBytes(o["Value"]?.ToString()))
                    {
                        array.Add(new JValue(b));
                    }
                    defaultData.Add(array);
                }
                else
                {
                    defaultData.Add(ConvertToInt(o, controller));
                }
            }
            else
            {
                if (values[0] is JObject && values[0]["DataType"]?.ToString() == "BOOL" && values[0]["Index"] != null)
                {
                    foreach (JToken jv in ChangeToBOOLFiled(values as JArray, controller))
                    {
                        defaultData.Add(jv);
                    }

                }
                else
                    foreach (var item2 in values)
                    {
                        if (item2 is JArray)
                        {
                            JArray newArray = new JArray();
                            GetArrayValue(newArray, item2 as JArray, controller);
                            defaultData.Add(newArray);
                        }

                        else
                            defaultData.Add(ConvertToInt(item2 as JObject, controller));
                    }
            }
        }

        public static void ChangeToBOOLDefaultValue(int value, JArray array, int index)
        {
            for (int i = 0; i < 32; i++)
            {
                array.Add(value >> i & 1);
            }
        }

        public static void GetValues(JArray values, IDataType dataType, JToken data)
        {
            if (dataType == null) return;

            int index2 = 0;
            var typeMemberComponentCollection = (dataType as CompositiveType)?.TypeMembers;
            if (typeMemberComponentCollection != null)
                foreach (var member in typeMemberComponentCollection)
                {
                    if (dataType is AOIDataType &&
                        !(dataType as AOIDataType).IsMemberShowInOtherAoi(member.Name)) continue;
                    int dim = member.DataTypeInfo.Dim1;
                    if (dim > 0)
                    {
                        if (member.DataTypeInfo.DataType is BOOL)
                        {
                            JArray array = new JArray();
                            int index = 0;
                            JToken newData = data[0];
                            for (int i = ((JArray) newData).Count - 1; i >= 0; i--)
                            {
                                AOIExtend.ChangeToBOOLDefaultValue((int) (newData as JArray)?[i], array, index);
                                index++;
                            }

                            values.Add(array);
                            index2++;
                        }
                        else
                        {
                            if (member.DataTypeInfo.DataType is CompositiveType)
                            {
                                JArray array2 = new JArray();
                                for (int i = 0; i < dim; i++)
                                {
                                    JArray array = new JArray();
                                    GetValues(array, member.DataTypeInfo.DataType, data?[index2]?[i] as JArray);
                                    array2.Add(array);
                                }

                                values.Add(array2);
                            }
                            else
                            {
                                JArray array = new JArray();
                                for (int i = 0; i < dim; i++)
                                {
                                    //JObject value = new JObject();
                                    //value["Name"] = member.Name;
                                    //value["Radix"] = member.DisplayStyle.ToString();
                                    //if (member.DataTypeInfo.DataType.IsInteger || member.DataTypeInfo.DataType.IsReal)
                                    //{
                                    //    value["Value"] = FormatOp.FormatValue(GetValue(data, $"{index2},{i}"),
                                    //        member.DisplayStyle, member.DataTypeInfo.DataType);
                                    //}
                                    //else
                                    //{
                                    //    value["Value"] = GetValue(data, $"{index2},{i}");
                                    //}

                                    //value["DataType"] = FormatOp.ConvertMemberName(member.DataTypeInfo.DataType.Name);

                                    //array.Add(value);
                                    if (member.DataTypeInfo.DataType.IsInteger || member.DataTypeInfo.DataType.IsReal)
                                    {
                                        array.Add(FormatOp.FormatValue(GetValue(data, $"{index2},{i}"),
                                            member.DisplayStyle, member.DataTypeInfo.DataType));
                                    }
                                    else
                                    {
                                        array.Add(GetValue(data, $"{index2},{i}"));
                                    }
                                }

                                values.Add(new JArray(array));
                            }

                            index2++;
                        }
                    }
                    else
                    {
                        if (member.DataTypeInfo.DataType is CompositiveType)
                        {
                            JArray array = new JArray();
                            GetValues(array, member.DataTypeInfo.DataType, data?[index2] as JArray);
                            values.Add(array);
                            index2++;
                        }
                        else
                        {
                            //JObject value = new JObject();
                            //value["Radix"] = member.DisplayStyle.ToString();
                            //value["Name"] = member.Name;
                            ////value["Value"] = data?[index2] ?? "0";

                            //if (member.DataTypeInfo.DataType.IsInteger || member.DataTypeInfo.DataType.IsReal)
                            //{
                            //    value["Value"] = FormatOp.FormatValue(
                            //        ValueConverter.ConvertValue(GetValue(data, $"{index2}"), DisplayStyle.Decimal,
                            //            member.DisplayStyle, member.DataTypeInfo.DataType.BitSize,
                            //            ValueConverter.SelectIntType(member.DataTypeInfo.DataType)),
                            //        member.DisplayStyle, member.DataTypeInfo.DataType);
                            //}
                            //else
                            //{
                            //    value["Value"] = ValueConverter.ConvertValue(GetValue(data, $"{index2}"),
                            //        DisplayStyle.Decimal, member.DisplayStyle, member.DataTypeInfo.DataType.BitSize,
                            //        ValueConverter.SelectIntType(member.DataTypeInfo.DataType));
                            //}

                            //index2++;
                            //value["DataType"] = FormatOp.ConvertMemberName(member.DataTypeInfo.DataType.Name);
                            //values.Add(value);
                            if (member.DataTypeInfo.DataType.IsInteger || member.DataTypeInfo.DataType.IsReal)
                            {
                                values.Add(FormatOp.FormatValue(
                                    ValueConverter.ConvertValue(GetValue(data, $"{index2}"), DisplayStyle.Decimal,
                                        member.DisplayStyle, member.DataTypeInfo.DataType.BitSize,
                                        ValueConverter.SelectIntType(member.DataTypeInfo.DataType)),
                                    member.DisplayStyle, member.DataTypeInfo.DataType));
                            }
                            else
                            {
                                values.Add(ValueConverter.ConvertValue(GetValue(data, $"{index2}"),
                                    DisplayStyle.Decimal, member.DisplayStyle, member.DataTypeInfo.DataType.BitSize,
                                    ValueConverter.SelectIntType(member.DataTypeInfo.DataType)));
                            }
                        }
                    }
                }
        }

        public static string GetValue(JToken data, string level)
        {
            if (data == null) return "0";
            var value = data as JValue;
            if (value != null)
            {
                return value.Value?.ToString();
            }

            if (((JArray) data).Count <= int.Parse(level.Split(',')[0])) return "0";
            if (level.Split(',').Length > 1)
                return GetValue(data[int.Parse(level.Split(',')[0])], level.Substring(level.IndexOf(",") + 1));

            return data[int.Parse(level.Split(',')[0])]?.ToString();
        }
    }
}
