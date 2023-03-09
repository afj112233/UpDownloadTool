using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;
using ICSStudio.FileConverter.L5XToJson.Objects;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Annotations;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSStudio.FileConverter.L5XToJson
{
    public class DataParse
    {
        public enum DataParseType
        {
            Normal,
            Aoi,
        }

        public static JToken Parse(JObject tag, XmlElement xmlNode, string dataType, int[] dims,
            DataParseType dataParseType)
        {
            try
            {
                if (dataType.Equals("AXIS_GENERIC", StringComparison.OrdinalIgnoreCase) ||
                         dataType.Equals("AXIS_GENERIC_DRIVE", StringComparison.OrdinalIgnoreCase) ||
                         dataType.Equals("AXIS_SERVO", StringComparison.OrdinalIgnoreCase) ||
                         dataType.Equals("AXIS_SERVO_DRIVE", StringComparison.OrdinalIgnoreCase))
                {
                    //TODO(zyl):add data
                    return null;
                }
                else if (dataType.Equals("ALARM_ANALOG", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Assert(GetDims(dims) == 1, "ALARM_ANALOG");
                    return ALARM_ANALOGParse(xmlNode, dataParseType);
                }
                else if (dataType.Equals("ALARM_DIGITAL", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Assert(GetDims(dims) == 1, "ALARM_DIGITAL");
                    return ALARM_DIGITALParse(xmlNode, dataParseType);
                }
                else if (dataType.Equals("AXIS_CIP_DRIVE", StringComparison.OrdinalIgnoreCase))
                {
                    AXIS_CIP_DRIVEParse(tag, xmlNode);
                    return null;
                }
                else if (dataType.Equals("AXIS_VIRTUAL", StringComparison.OrdinalIgnoreCase))
                {
                    AXIS_VIRTUALParse(tag, xmlNode);
                    return null;
                }
                else if (dataType.Equals("HMIBC", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Assert(dims[1] == 0 && dims[2] == 0, "HMIBC");
                    //TODO(zyl):get data
                }
                else if (dataType.Equals("MESSAGE", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Assert(dims[0] == 0 && dims[1] == 0 && dims[2] == 0, "MESSAGE");

                    MESSAGEParse(tag, xmlNode);
                    return null;
                }
                else if (dataType.Equals("MOTION_GROUP", StringComparison.OrdinalIgnoreCase))
                {
                    MOTION_GROUPParse(tag, xmlNode);
                    return null;
                }
                else
                {
                    try
                    {
                        return GetL5KData(xmlNode);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return null;
                    }

                    //return DecoratedParse(xmlNode, dataParseType, dims, dataType);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //ignore
                //if (!e.Message.Equals("can parse aoi dataType or dataType contain aoi dataType"))
                //Debug.Assert(true, $"{tag["Name"]}--errorMessage:{e}");
                //throw new Exception($"error in convert tag {tag["Name"]}");
            }

            return null;
        }



        public static JToken GetXmlNodeL5KData([NotNull] XmlNodeList defaultData, string parentDataType)
        {
            var data = defaultData[0];
            //data in inout tag
            if (data == null)
            {
                return null;
            }
            var value = data.FirstChild.Value;

            if (value == null)
                return null;

            value = value.Replace("2#", "");

            //for 问题1414
            bool isString = false;

            foreach (var item in defaultData.OfType<XmlNode>())
            {
                if (string.Equals(item.Attributes?["Format"].Value, "String"))
                {
                    isString = true;
                    break;
                }
            }

            if (isString)
            {
                try
                {
                    value = value.TrimStart('[');
                    value = value.TrimEnd(']');

                    int index = value.IndexOf(',');
                    if (index > 0)
                    {
                        JArray jArray = new JArray();

                        jArray.Add(int.Parse(value.Substring(0, index)));
                        jArray.Add(value.Substring(index + 1).TrimStart('\'').TrimEnd('\'','\r','\n','\t'));

                        return jArray;
                    }

                    return null;

                }
                catch (Exception)
                {
                    //ignore
                }

            }
            //end for 问题1414

            //ID1001067
            if (value.Contains("-1.#INF0000e+000"))
            {
                value = value.Replace("-1.#INF0000e+000", "-Infinity");
            }

            if (value.Contains("1.#INF0000e+000"))
            {
                value = value.Replace("1.#INF0000e+000", "Infinity");
            }

            if (value.Contains("-1.#QNAN000e+000"))
            {
                value = value.Replace("-1.#QNAN000e+000", "NaN");
            }

            if (value.Contains("1.#QNAN000e+000"))
            {
                value = value.Replace("1.#QNAN000e+000", "NaN");
            }
            //

            try
            {
                var jValue = JToken.Parse(value);
                return jValue;
            }
            catch (Exception)
            {
                //ignore
            }

            try
            {
                var jArray = JArray.Parse(value);
                return jArray;
            }
            catch (Exception)
            {
                //ignore
            }

            return null;

        }

        private static int GetL5xUDTStringLen(XmlElement xmlNode, string type)
        {
            if ("STRING".Equals(type))
            {
                return 82;
            }

            XmlNode root = xmlNode;
            while (true)
            {
                if (xmlNode.ParentNode == null) break;
                root = xmlNode.ParentNode;
            }

            var dataTypesNode = root.SelectSingleNode("DataTypes");
            if (dataTypesNode != null)
            {
                var dataTypes = dataTypesNode.SelectNodes("DataType");
                foreach (XmlElement dataType in dataTypes)
                {
                    if (type.Equals(dataType.Attributes["Name"].Value))
                    {
                        var dataNode = dataType.FirstChild.ChildNodes[1];
                        if (dataNode == null) return -1;
                        var len = dataNode.Attributes["Dimension"].Value;
                        return int.Parse(len);
                    }
                }
            }

            return -1;
        }

        public static JToken GetL5KData(XmlElement xmlNode)
        {
            var dataTypeInfo = ParseDataTypeInfo(xmlNode);
            var nodeList = xmlNode.SelectNodes("Data");
            if (nodeList != null && nodeList.Count == 0)
                nodeList = xmlNode.SelectNodes("DefaultData");
            Debug.Assert(nodeList != null);
            var data = GetXmlNodeL5KData(nodeList, "");
            if (data == null) return null;
            //int offset = 0;
            if (dataTypeInfo.DataType is AOIDataType)
            {
                if (dataTypeInfo.Dim1 > 0)
                {
                    var arr = new JArray();
                    var total = TotalDim(dataTypeInfo);
                    for (int i = 0; i < total; i++)
                    {
                        arr.Add(GetAoiL5KData(((AOIDataType) dataTypeInfo.DataType), data[i]));
                    }

                    return arr;
                }
                else
                {
                    return GetAoiL5KData((AOIDataType) dataTypeInfo.DataType, data);
                }
            }

            if (dataTypeInfo.DataType is UserDefinedDataType)
            {
                if (dataTypeInfo.Dim1 > 0)
                {
                    var arr = new JArray();
                    var total = TotalDim(dataTypeInfo);
                    for (int i = 0; i < total; i++)
                    {
                        arr.Add(GetUserDefinedDataTypeL5XData((UserDefinedDataType) dataTypeInfo.DataType, data[i]));
                    }

                    return arr;
                }
                else
                    return GetUserDefinedDataTypeL5XData((UserDefinedDataType) dataTypeInfo.DataType, data);
            }

            if (dataTypeInfo.DataType.IsAtomic)
            {
                if (dataTypeInfo.DataType.IsBool && dataTypeInfo.Dim1 > 0)
                {
                    return ConvertBoolArr(data as JArray);
                }

                return data;
            }

            if (dataTypeInfo.Dim1 > 0)
            {
                return GetArrL5XData(dataTypeInfo, data);
            }

            return GetPredefinedTypeL5XData(dataTypeInfo.DataType, data);
        }

        private static JToken GetUserDefinedDataTypeL5XData(AssetDefinedDataType dataType, JToken origin)
        {
            if (dataType.FamilyType == FamilyType.StringFamily)
            {
                return GetStringL5XData(origin);
            }

            var convertData = new JArray();
            int offset = 0;
            int byteOffset = -1;
            if (!dataType.TypeMembers.Any())
                dataType.PostInit(Controller.GetInstance().DataTypes);
            foreach (var member in dataType.TypeMembers)
            {
                if (member.DataTypeInfo.DataType.IsBool)
                {
                    if (member.DataTypeInfo.Dim1 > 0)
                    {
                        convertData.Add(ConvertBoolArr(origin[offset++] as JArray));
                        continue;
                    }
                    else
                    {
                        if (byteOffset != member.ByteOffset)
                        {
                            byteOffset = member.ByteOffset;
                            convertData.Add(origin[offset++]);
                        }

                        continue;
                    }

                }

                if (member.DataTypeInfo.DataType.IsAtomic)
                {
                    convertData.Add(origin[offset++]);
                    continue;
                }

                if (member.DataTypeInfo.DataType.IsPredefinedType)
                {
                    if (member.DataTypeInfo.Dim1 > 0)
                    {
                        convertData.Add(GetArrL5XData(member.DataTypeInfo, origin[offset++]));
                    }
                    else
                    {
                        convertData.Add(GetPredefinedTypeL5XData(member.DataTypeInfo.DataType, origin[offset++]));
                    }
                }
                else
                {
                    var data = origin[offset++];
                    Debug.Assert(data is JArray);
                    if (member.DataTypeInfo.Dim1 > 0)
                    {
                        var total = TotalDim(member.DataTypeInfo);
                        var arr = new JArray();
                        for (int i = 0; i < total; i++)
                        {
                            arr.Add(member.DataTypeInfo.DataType is AOIDataType
                                ? GetAoiL5KData((AOIDataType) member.DataTypeInfo.DataType, data[i])
                                : GetUserDefinedDataTypeL5XData((AssetDefinedDataType) member.DataTypeInfo.DataType,
                                    data[i]));
                        }

                        convertData.Add(arr);
                    }
                    else
                    {
                        convertData.Add(member.DataTypeInfo.DataType is AOIDataType
                            ? GetAoiL5KData((AOIDataType) member.DataTypeInfo.DataType, data)
                            : GetUserDefinedDataTypeL5XData((AssetDefinedDataType) member.DataTypeInfo.DataType, data));
                    }
                }
            }

            return convertData;
        }

        private static JToken GetArrL5XData(DataTypeInfo dataTypeInfo, JToken jToken)
        {
            var totalDim = TotalDim(dataTypeInfo);
            var arr = new JArray();
            for (int i = 0; i < totalDim; i++)
            {
                arr.Add(GetPredefinedTypeL5XData(dataTypeInfo.DataType, jToken[i]));
            }

            return arr;
        }

        private static JToken GetPredefinedTypeL5XData(IDataType dataType, JToken jToken)
        {
            if (dataType is ALARM)
            {
                return GetAlarmL5XData(jToken);
            }
            if (dataType is FBD_ONESHOT)
            {
                return GetFbdOneShotL5XData(jToken);
            }

            if (dataType is FBD_TIMER)
            {
                return GetFbdTimerL5XData(jToken);
            }

            if (dataType is MOTION_INSTRUCTION)
            {
                return GetMotionInstructionL5XData(jToken);
            }

            if (dataType is CAM_PROFILE)
            {
                return GetCamProfileL5XData(jToken);
            }

            if (dataType is COORDINATE_SYSTEM)
            {
                //TODO(zyl):add convert
                return null;
            }

            if (dataType is DATALOG_INSTRUCTION)
            {
                return GetDataLogInstructionL5XData(jToken);
            }

            if (dataType is AXIS_CONSUMED)
            {
                //TODO(zyl):add convert
                return null;
            }

            if (dataType is ENERGY_BASE)
            {
                //TODO(zyl):add convert
                return null;
            }
            /*
            if (dataType is TIMER)
            {
                return GetTimerL5XData(jToken);
            }
            */

            if (dataType is COUNTER)
            {
                return GetCounterL5XData(jToken);
            }

            if (dataType is CONTROL)
            {
                return GetControlL5XData(jToken);
            }

            if (dataType is FILTER_LOW_PASS)
            {
                return GetFilterLowPassL5XData(jToken);
            }

            if (dataType is FILTER_HIGH_PASS)
            {
                return GetFilterLowPassL5XData(jToken);
            }

            if (dataType is FBD_MATH)
            {
                return GetFbdMathL5XData(jToken);
            }

            if (dataType is FBD_COUNTER)
            {
                return GetFbdCounterL5XData(jToken);
            }

            if (dataType.FamilyType == FamilyType.StringFamily)
            {
                return GetStringL5XData(jToken);
            }

            return PredefinedTypeDataConverter.GetL5XData(dataType, jToken);
        }

        private static JToken GetFbdMathL5XData(JToken origin)
        {
            var convertData = new JArray();
            var bit = (int) origin[0];
            convertData.Add(bit & 1);
            convertData.Add(origin[1]);
            convertData.Add(origin[2]);
            convertData.Add((bit >> 1) & 1);
            convertData.Add(origin[3]);
            return convertData;
        }

        private static JToken GetFbdCounterL5XData(JToken origin)
        {
            var convertData = new JArray();
            var bit = (int) origin[0];
            convertData.Add(bit&1);
            convertData.Add((bit >> 1) & 1);
            convertData.Add((bit >> 2) & 1);
            convertData.Add(origin[1]);
            convertData.Add((bit>>3)&1);
            bit = (int)origin[2];
            convertData.Add(bit&1);
            convertData.Add(origin[3]);
            convertData.Add((bit>>1)&1);
            convertData.Add((bit>>2)&1);
            convertData.Add((bit>>3)&1);
            convertData.Add((bit>>4)&1);
            convertData.Add((bit>>5)&1);
            convertData.Add(0);
            convertData.Add(0);
            return convertData;
        }
        
        private static JToken GetFilterLowPassL5XData(JToken origin)
        {
            var arr = origin as JArray;
            Debug.Assert(arr != null);
            if (arr == null) return origin;
            var convertData=new JArray();
            var bit = (int) arr[0];
            convertData.Add(bit & 1);
            convertData.Add(arr[1]);
            convertData.Add((bit >> 1) & 1);
            convertData.Add(arr[2]);
            convertData.Add(arr[3]);
            convertData.Add(arr[4]);
            convertData.Add(arr[5]);
            convertData.Add(arr[6]);
            convertData.Add(arr[7]);
            convertData.Add(arr[8]);
            convertData.Add(arr[9]);
            convertData.Add(arr[10]);
            convertData.Add(arr[11]);
            convertData.Add(0);
            convertData.Add((float)0);
            convertData.Add((float)0);
            convertData.Add((float)0);
            convertData.Add((float)0);
            convertData.Add(0);
            convertData.Add((float)0);
            convertData.Add((float)0);
            convertData.Add((float)0);
            return convertData;
        }

        private static JToken GetFbdOneShotL5XData(JToken origin)
        {
            var convertData = new JArray();
            Debug.Assert(origin.Count() == 3);
            convertData.Add(0);
            var data = (int) origin[0];
            convertData.Add(data & 1);
            convertData.Add((data >> 1) & 1);
            data = (int) origin[1];
            convertData.Add(data & 1);
            convertData.Add((data >> 1) & 1);
            convertData.Add(0);
            return convertData;
        }

        private static JToken GetFbdTimerL5XData(JToken origin)
        {
            var convertData = new JArray();
            var bitData = (int) origin[0];
            convertData.Add(bitData & 1);
            convertData.Add(bitData >> 1 & 1);
            convertData.Add((int) origin[2]);
            convertData.Add(bitData >> 2 & 1);
            bitData = (int) origin[1];
            convertData.Add(bitData & 1);
            convertData.Add((int) origin[3]);
            convertData.Add(bitData >> 1 & 1);
            convertData.Add(bitData >> 2 & 1);
            convertData.Add(bitData >> 3 & 1);
            convertData.Add((int) origin[5]);
            convertData.Add(0);
            convertData.Add(0);
            convertData.Add(0);
            return convertData;
        }

        private static JToken GetMotionInstructionL5XData(JToken origin)
        {
            var convertData = new JArray();
            convertData.Add(origin[0]);
            convertData.Add(origin[1]);
            convertData.Add(origin[2]);
            convertData.Add(origin[3]);
            convertData.Add(origin[4]);
            convertData.Add(origin[5]);
            convertData.Add(origin[6]);
            return convertData;
        }

        private static JToken GetAoiL5KData(AOIDataType dataType, JToken origin)
        {
            var convertData = new JArray();
            int bitCount = 0;
            JToken bitData = null;
            int offset = 0;
            if (!dataType.TypeMembers.Any())
            {
                dataType?.PostInit(Controller.GetInstance().DataTypes);
            }

            foreach (var member in dataType.TypeMembers)
            {

                try
                {
                    if (member.DataTypeInfo.DataType.IsBool)
                    {
                        convertData.Add(ParseAoiBoolL5XData(origin, member.DataTypeInfo, ref bitCount, ref offset,
                            ref bitData));
                        continue;
                    }


                    if (member.DataTypeInfo.DataType.IsAtomic)
                    {
                        convertData.Add(origin[offset++]);
                        continue;
                    }

                    if (member.DataTypeInfo.DataType.IsPredefinedType)
                    {
                        if (member.DataTypeInfo.Dim1 > 0)
                        {
                            convertData.Add(GetArrL5XData(member.DataTypeInfo, origin[offset++]));
                        }
                        else
                        {
                            convertData.Add(GetPredefinedTypeL5XData(member.DataTypeInfo.DataType, origin[offset++]));
                        }
                    }
                    else
                    {
                        var data = origin[offset++];
                        Debug.Assert(data is JArray);
                        if (member.DataTypeInfo.Dim1 > 0)
                        {
                            var total = TotalDim(member.DataTypeInfo);
                            var arr = new JArray();
                            for (int i = 0; i < total; i++)
                            {
                                arr.Add(member.DataTypeInfo.DataType is AOIDataType
                                    ? GetAoiL5KData((AOIDataType) member.DataTypeInfo.DataType, data[i])
                                    : GetUserDefinedDataTypeL5XData((AssetDefinedDataType) member.DataTypeInfo.DataType,
                                        data[i]));
                            }

                            convertData.Add(arr);
                        }
                        else
                        {
                            convertData.Add(member.DataTypeInfo.DataType is AOIDataType
                                ? GetAoiL5KData((AOIDataType) member.DataTypeInfo.DataType, data)
                                : GetUserDefinedDataTypeL5XData((AssetDefinedDataType) member.DataTypeInfo.DataType,
                                    data));
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return convertData;
        }

        private static int TotalDim(DataTypeInfo dataTypeInfo)
        {
            return Math.Max(1, dataTypeInfo.Dim1) * Math.Max(1, dataTypeInfo.Dim2) * Math.Max(1, dataTypeInfo.Dim3);
        }

        // ReSharper disable once RedundantAssignment
        private static JToken ParseAoiBoolL5XData(JToken origin, DataTypeInfo dataTypeInfo, ref int bitCount,
            ref int offset, ref JToken bitData)
        {
            if (bitCount == 32)
            {
                bitCount = 0;
                bitData = null;
            }

            var data = bitData ?? origin[offset++];
            if (dataTypeInfo.Dim1 > 0)
            {
                data = origin[offset++];
                var arr = data as JArray;
                Debug.Assert(arr != null);
                var convertData = new JArray();
                foreach (JValue v in arr)
                {
                    var val = (string) v;
                    if (val.StartsWith("2#"))
                    {
                        convertData.Add(int.Parse(val.Substring(2)));
                    }
                    else
                    {
                        convertData.Add(v);
                    }
                }

                //isLastBool = false;
                //offset++;

                return ConvertBoolArr(convertData);
            }
            else
            {
                var val = data as JValue;
                bitData = val;
                Debug.Assert(val != null && val.Type == JTokenType.Integer);
                //isLastBool = true;
                return ((int) val >> bitCount++) & 1;
            }
        }

        private static JToken ConvertBoolArr(JArray arr)
        {
            Debug.Assert(arr != null);
            var convertData = new JArray();
            Debug.Assert(arr.Count % 32 == 0);
            var bin = "";
            for (int i = 0; i < arr.Count; i++)
            {
                if (i != 0 && i % 32 == 0)
                {
                    convertData.Add(Convert.ToInt32(bin, 2));
                    bin = "";
                }

                bin = arr[i] + bin;
            }

            if (!string.IsNullOrEmpty(bin))
            {
                convertData.Add(Convert.ToInt32(bin, 2));
            }

            return convertData;
        }

        private static DataTypeInfo ParseDataTypeInfo(XmlElement node)
        {
            try
            {
                var dataType = Controller.GetInstance().DataTypes[node.Attributes["DataType"].Value];
                Debug.Assert(dataType != null, node.Attributes["DataType"].Value);
                var dataTypeInfo = new DataTypeInfo();
                dataTypeInfo.DataType = dataType;

                if (node.HasAttribute("Dimensions"))
                {
                    string[] dims = node.Attributes["Dimensions"].Value.Split(' ');
                    if (dims.Length == 3)
                    {
                        dataTypeInfo.Dim3 = int.Parse(dims[2]);
                        dataTypeInfo.Dim2 = int.Parse(dims[1]);
                        dataTypeInfo.Dim1 = int.Parse(dims[0]);
                    }
                    else if (dims.Length == 2)
                    {
                        dataTypeInfo.Dim2 = int.Parse(dims[1]);
                        dataTypeInfo.Dim1 = int.Parse(dims[0]);
                    }
                    else
                    {
                        dataTypeInfo.Dim1 = int.Parse(dims[0]);
                    }
                }

                return dataTypeInfo;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Assert(false, e.Message);
                throw;
            }
        }

        public static void FixData(JArray jArray)
        {
            if (jArray == null) return;
            for (int i = 0; jArray.Count > i; i++)
            {
                try
                {
                    var jToken = jArray[i];
                    var array = jToken as JArray;
                    if (array != null)
                    {
                        FixData(array);
                    }

                    if (jToken is JObject && "BOOL".Equals(jToken["DataType"]?.ToString()))
                    {
                        var bin = "";
                        int bit = 8;
                        if (jToken["isArray"] != null && (bool) jToken["isArray"])
                        {
                            bit = 32;
                        }

                        while (i < jArray.Count)
                        {
                            var nBit = jArray[i] as JObject;
                            if (nBit != null && "BOOL".Equals(nBit["DataType"]?.ToString()))
                            {
                                bin = (nBit["Value"] as JValue)?.Value + bin;
                                jArray.RemoveAt(i);
                                if (bin.Length == bit)
                                {
                                    jArray.Insert(i,
                                        new JValue(bit == 8 ? Convert.ToSByte(bin, 2) : Convert.ToInt32(bin, 2)));
                                    bin = "";
                                    break;
                                }

                                continue;
                            }
                            else
                            {
                                jArray.Insert(i,
                                    new JValue(bit == 8 ? Convert.ToSByte(bin, 2) : Convert.ToInt32(bin, 2)));
                                bin = "";
                                break;
                            }

                        }

                        if (!string.IsNullOrEmpty(bin))
                            jArray.Insert(i, new JValue(bit == 8 ? Convert.ToSByte(bin, 2) : Convert.ToInt32(bin, 2)));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static void MemberParse(JArray values, XmlElement node, DataParseType dataParseType, bool isAoi)
        {
            if (node.Name.Equals("Structure") && node.HasChildNodes)
            {
                var members = node.SelectNodes("StructureMember");
                foreach (XmlElement member in members)
                {
                    var memberJArray = new JArray();
                    MemberParse(memberJArray, member, dataParseType, isAoi);
                    values.Add(memberJArray);
                }
            }

            if (node.Name.Equals("StructureMember"))
            {
                var dataType = node.Attributes?["DataType"].Value ?? "DINT";

                bool isPredefined = Controller.GetInstance().DataTypes[dataType]?.IsPredefinedType ?? false;
                if (!isPredefined)
                {
                    var memberJArray = new JArray();
                    foreach (XmlElement child in node)
                    {
                        MemberParse(memberJArray, child, dataParseType, IsAoiDataType(node, dataType));
                    }

                    values.Add(memberJArray);
                }
                else
                {
                    var xmlDoc = node.OwnerDocument;
                    var tagNode = xmlDoc.CreateElement("Tag");
                    var dataL5K = xmlDoc.CreateElement(DataName(dataParseType));
                    var data = xmlDoc.CreateElement(DataName(dataParseType));
                    data.AppendChild(node.CloneNode(true));
                    tagNode.AppendChild(dataL5K);
                    tagNode.AppendChild(data);

                    var token = Parse(null, tagNode, dataType, new[] {0, 0, 0}, dataParseType);
                    if (token == null) return;
                    //GetArrayStructure(values, node, dataType);

                    values.Add(token);
                }
            }

            if (node.Name.Equals("ArrayMember"))
            {
                var memberJArray = new JArray();
                foreach (XmlElement elementNode in node.ChildNodes)
                {
                    var xmlDoc = node.OwnerDocument;
                    var tagNode = xmlDoc.CreateElement("Tag");
                    var dataL5K = xmlDoc.CreateElement(DataName(dataParseType));
                    var data = xmlDoc.CreateElement(DataName(dataParseType));
                    tagNode.AppendChild(dataL5K);
                    tagNode.AppendChild(data);
                    var dataType = node.Attributes["DataType"].Value;

                    data.SetAttribute("DataType", dataType);
                    if (elementNode.FirstChild == null)
                    {
                        if (elementNode.HasAttribute("Index"))
                            data.SetAttribute("Index", elementNode.Attributes["Index"].Value);
                        if (elementNode.HasAttribute("Format"))
                            data.SetAttribute("Format", elementNode.Attributes["Format"].Value);
                        data.SetAttribute("Value", elementNode.Attributes["Value"].Value);
                    }
                    else
                    {
                        data.AppendChild(elementNode.FirstChild.CloneNode(true));
                    }

                    if ("bool".Equals(dataType, StringComparison.OrdinalIgnoreCase))
                    {
                        var token = new JObject();
                        token["DataType"] = "BOOL";
                        token["isArray"] = true;
                        token["Value"] = data.Attributes["Value"].Value;

                        memberJArray.Add(token);
                    }
                    else
                    {
                        var token = Parse(null, tagNode, dataType, new[] {0, 0, 0}, dataParseType);
                        if (token == null) return;
                        memberJArray.Add(token);
                    }
                }

                values.Add(memberJArray);
            }

            if (node.Name.Equals("DataValueMember"))
            {
                var member = new JObject();
                var element = (XmlElement) node;
                if (element.HasAttribute("DataType"))
                    member["DataType"] = node.Attributes["DataType"].Value;
                if (element.HasAttribute("Radix"))
                    member["Radix"] = node.Attributes["Radix"].Value;
                if (element.HasAttribute("Value"))
                {
                    var value = node.Attributes["Value"].Value;
                    if ((string) member["Radix"] == "Radix")
                        value = value.Remove(value.Length - 1).Remove(0, 1);
                    member["Value"] = value;


                    var val = AOIExtend.ConvertToInt(member, Controller.GetInstance());
                    if (!isAoi && node.Attributes["DataType"].Value.Equals("bool", StringComparison.OrdinalIgnoreCase))
                    {
                        var bit = new JObject();
                        bit["Value"] = val;
                        bit["DataType"] = "BOOL";
                        values.Add(bit);
                    }
                    else
                        values.Add(val);
                }
                else
                {
                    if ("ASCII".Equals((string) member["Radix"]))
                    {
                        var str = element.FirstChild.Value;
                        if (str.Contains("'"))
                            str = str.Substring(0, str.Length - 1).Substring(1);
                        var chars = new JArray();
                        var d = str.Replace("\n", "").Replace("\r", "")
                            .Replace("\t", "");
                        d = FormatOp.FormatSpecial(d);
                        foreach (var c in d)
                        {
                            chars.Add(new JValue((int) c));
                        }

                        values.Add(chars);
                    }
                    else
                    {
                        Debug.Assert(false);
                    }
                }
            }
        }

        private static JToken ALARM_DIGITALParse(XmlElement xmlNode, DataParseType dataParseType)
        {
            var parameters = xmlNode.SelectSingleNode(DataName(dataParseType))
                ?.SelectSingleNode("AlarmAnalogParameters");
            if (parameters != null)
            {
                var data = new JArray();
                data.Add(new JValue(ConvertBOOL((parameters.Attributes["EnableIn"].Value))));
                data.Add(new JValue(float.Parse(parameters.Attributes["In"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["InFault"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["Condition"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["AckRequired"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["Latched"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ProgAck"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["OperAck"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ProgReset"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["OperReset"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ProgSuppress"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["OperSuppress"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ProgUnsuppress"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["OperUnsuppress"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["OperShelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ProgUnshelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["OperUnshelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ProgDisable"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["OperDisable"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ProgEnable"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["OperEnable"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["AlarmCountReset"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["UseProgTime"].Value)));
                data.Add(new JValue(DateTimeToLong(parameters.Attributes["UseProgTime"].Value)));
                data.Add(new JValue(int.Parse(parameters.Attributes["Severity"].Value)));
                data.Add(new JValue(int.Parse(parameters.Attributes["MinDurationPRE"].Value)));
                data.Add(new JValue(int.Parse(parameters.Attributes["ShelveDuration"].Value)));
                data.Add(new JValue(int.Parse(parameters.Attributes["MaxShelveDuration"].Value)));

                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(621356256000000000));
                data.Add(new JValue(621356256000000000));
                data.Add(new JValue(621356256000000000));
                data.Add(new JValue(621356256000000000));
                data.Add(new JValue(621356256000000000));
                data.Add(new JValue(621356256000000000));
                data.Add(new JValue(0));
                return data;
                //tag.Add(DataName(dataParseType), data);
            }

            return null;
        }

        private static JToken ALARM_ANALOGParse(XmlElement xmlNode, DataParseType dataParseType)
        {
            var parameters = xmlNode.SelectSingleNode(DataName(dataParseType))
                ?.SelectSingleNode("AlarmAnalogParameters");
            if (parameters != null)
            {
                var data = new JArray();
                data.Add(new JValue(ConvertBOOL((parameters.Attributes["EnableIn"].Value))));
                data.Add(new JValue(float.Parse(parameters.Attributes["In"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["InFault"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["HHEnabled"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["HEnabled"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["LEnabled"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["LLEnabled"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["AckRequired"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ProgAckAll"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["OperAckAll"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["HHProgAck"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["HHOperAck"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["HProgAck"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["HOperAck"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["LProgAck"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["LOperAck"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["LLProgAck"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["LLOperAck"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ROCPosProgAck"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ROCPosOperAck"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ROCNegProgAck"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ROCNegOperAck"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ProgSuppress"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["OperSuppress"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ProgUnsuppress"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["OperUnsuppress"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["HHOperShelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["HOperShelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["LOperShelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["LLOperShelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ROCPosOperShelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ROCNegOperShelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ProgUnshelveAll"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["HHOperUnshelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["HOperUnshelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["LOperUnshelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["LLOperUnshelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ROCPosOperUnshelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ROCNegOperUnshelve"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ProgDisable"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["OperDisable"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["ProgEnable"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["OperEnable"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["AlarmCountReset"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["HHMinDurationEnable"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["HMinDurationEnable"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["LMinDurationEnable"].Value)));
                data.Add(new JValue(ConvertBOOL(parameters.Attributes["LLMinDurationEnable"].Value)));
                data.Add(new JValue(float.Parse(parameters.Attributes["HHLimit"].Value)));
                data.Add(new JValue(int.Parse(parameters.Attributes["HHSeverity"].Value)));
                data.Add(new JValue(float.Parse(parameters.Attributes["HLimit"].Value)));
                data.Add(new JValue(int.Parse(parameters.Attributes["HSeverity"].Value)));
                data.Add(new JValue(float.Parse(parameters.Attributes["LLimit"].Value)));
                data.Add(new JValue(int.Parse(parameters.Attributes["LSeverity"].Value)));
                data.Add(new JValue(float.Parse(parameters.Attributes["LLLimit"].Value)));
                data.Add(new JValue(int.Parse(parameters.Attributes["LLSeverity"].Value)));
                data.Add(new JValue(int.Parse(parameters.Attributes["MinDurationPRE"].Value)));
                data.Add(new JValue(int.Parse(parameters.Attributes["ShelveDuration"].Value)));
                data.Add(new JValue(int.Parse(parameters.Attributes["MaxShelveDuration"].Value)));
                data.Add(new JValue(float.Parse(parameters.Attributes["Deadband"].Value)));
                data.Add(new JValue(float.Parse(parameters.Attributes["ROCPosLimit"].Value)));
                data.Add(new JValue(int.Parse(parameters.Attributes["ROCPosSeverity"].Value)));
                data.Add(new JValue(float.Parse(parameters.Attributes["ROCNegLimit"].Value)));
                data.Add(new JValue(int.Parse(parameters.Attributes["ROCNegSeverity"].Value)));
                data.Add(new JValue(float.Parse(parameters.Attributes["ROCPeriod"].Value)));

                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                data.Add(new JValue(0));
                return data;
                //tag.Add(DataName(dataParseType), data);
            }

            return null;
        }
        
        private static JToken GetAlarmL5XData(JToken jToken)
        {
            var convertData = new JArray();
            var bit = (int)jToken[0];
            convertData.Add(bit & 1);
            convertData.Add(jToken[1]);
            convertData.Add(jToken[2]);
            convertData.Add(jToken[3]);
            convertData.Add(jToken[4]);
            convertData.Add(jToken[5]);
            convertData.Add(jToken[6]);
            convertData.Add(jToken[7]);
            convertData.Add(jToken[8]);
            convertData.Add(jToken[9]);
            bit = (int) jToken[10];
            convertData.Add(bit&1);
            convertData.Add(bit>>1&1);
            convertData.Add(bit>>2&1);
            convertData.Add(bit>>3&1);
            convertData.Add(bit>>4&1);
            convertData.Add(bit>>5&1);
            convertData.Add(bit>>6&1);
            convertData.Add(jToken[11]);
            convertData.Add(jToken[12]);
            convertData.Add(0);
            convertData.Add(0);
            return convertData;
        }

        private static JToken GetCamProfileL5XData(JToken jToken)
        {
            return CAMProfile.ToJObject(jToken);
        }

        private static JToken GetDataLogInstructionL5XData(JToken jToken)
        {
            var convertData = new JArray();
            convertData.Add(jToken[1]);
            convertData.Add(jToken[2]);
            var bitData = (int) jToken[0];
            convertData.Add(bitData >> 31 & 1);
            convertData.Add(bitData >> 30 & 1);
            convertData.Add(bitData >> 29 & 1);
            convertData.Add(0);
            return convertData;
        }

        private static JToken GetTimerL5XData(JToken jToken)
        {
            var convertData = new JArray();
            convertData.Add(jToken[1]);
            convertData.Add(jToken[2]);

            //EN,TT,DN
            int temp = (int) jToken[0];

            convertData.Add((temp & (1 << 31)) != 0 ? 1 : 0);
            convertData.Add((temp & (1 << 30)) != 0 ? 1 : 0);
            convertData.Add((temp & (1 << 29)) != 0 ? 1 : 0);

            return convertData;
        }

        private static JToken GetCounterL5XData(JToken jToken)
        {
            JArray convertData = new JArray();

            convertData.Add(jToken[1]);
            convertData.Add(jToken[2]);

            // CU(31),CD,DN,OV,UN
            int temp = (int) jToken[0];

            convertData.Add((temp & (1 << 31)) != 0 ? 1 : 0);
            convertData.Add((temp & (1 << 30)) != 0 ? 1 : 0);
            convertData.Add((temp & (1 << 29)) != 0 ? 1 : 0);
            convertData.Add((temp & (1 << 28)) != 0 ? 1 : 0);
            convertData.Add((temp & (1 << 27)) != 0 ? 1 : 0);

            return convertData;
        }

        private static JToken GetControlL5XData(JToken jToken)
        {
            JArray convertData = new JArray();

            convertData.Add(jToken[1]);
            convertData.Add(jToken[2]);

            // EN,EU,DN,EM,ER,UL,IN,FD
            int temp = (int) jToken[0];

            convertData.Add((temp & (1 << 31)) != 0 ? 1 : 0);
            convertData.Add((temp & (1 << 30)) != 0 ? 1 : 0);
            convertData.Add((temp & (1 << 29)) != 0 ? 1 : 0);
            convertData.Add((temp & (1 << 28)) != 0 ? 1 : 0);
            convertData.Add((temp & (1 << 27)) != 0 ? 1 : 0);
            convertData.Add((temp & (1 << 26)) != 0 ? 1 : 0);
            convertData.Add((temp & (1 << 25)) != 0 ? 1 : 0);
            convertData.Add((temp & (1 << 24)) != 0 ? 1 : 0);

            return convertData;
        }

        private static JToken GetStringL5XData(JToken jToken)
        {
            var convertData = new JArray();
            convertData.Add(jToken[0]);
            var str = (string) jToken[1];
            str = FormatOp.FormatSpecial(str);
            var data = new JArray();
            foreach (char c in str)
            {
                data.Add(c);
            }

            convertData.Add(data);
            return convertData;
        }

        private static void AXIS_VIRTUALParse(JObject tag, XmlElement xmlNode)
        {
            var parameters = (XmlElement) xmlNode.SelectSingleNode("Data/AxisParameters");
            if (parameters != null)
            {
                //return AxisVirtualParameters.ToJObject(parameters);
                tag.Add("Parameters", AxisVirtualParameters.ToJObject(parameters));
            }
        }

        private static void AXIS_CIP_DRIVEParse(JObject tag, XmlElement xmlNode)
        {
            //TODO(gjc): add code here
            var parameters = (XmlElement) xmlNode.SelectSingleNode("Data/AxisParameters");
            if (parameters != null)
            {
                //return AxisParameters.ToJObject(parameters);
                tag.Add("Parameters", AxisParameters.ToJObject(parameters));
            }
        }

        private static void MESSAGEParse(JObject tag, XmlElement xmlNode)
        {
            var parameters = (XmlElement) xmlNode.SelectSingleNode("Data/MessageParameters");
            if (parameters != null)
            {
                tag.Add("Parameters", MessageParameters.ToJObject(parameters));
            }
        }

        private static void MOTION_GROUPParse(JObject tag, XmlElement xmlNode)
        {
            var parameters = (XmlElement) xmlNode.SelectSingleNode("Data/MotionGroupParameters");
            if (parameters != null)
            {
                tag.Add("Alternate1UpdateMultiplier",
                    int.Parse(parameters.Attributes["Alternate1UpdateMultiplier"].Value));
                tag.Add("Alternate2UpdateMultiplier",
                    int.Parse(parameters.Attributes["Alternate2UpdateMultiplier"].Value));
                tag.Add("AutoTagUpdate", parameters.Attributes["AutoTagUpdate"].Value == "Enabled" ? true : false);
                tag.Add("CoarseUpdatePeriod", int.Parse(parameters.Attributes["CoarseUpdatePeriod"].Value));
                tag.Add("GeneralFaultType", parameters.Attributes["GeneralFaultType"].Value);
                tag.Add("PhaseShift", parameters.Attributes["PhaseShift"].Value);
                if (parameters.HasAttribute("GroupType"))
                    tag.Add("GroupType", parameters.Attributes["GroupType"].Value);
            }
        }

        private static int GetDims(int[] dims)
        {
            return Math.Max(1, dims[0]) * Math.Max(1, dims[1]) * Math.Max(1, dims[2]);
        }

        public static JArray GetXmlNodeDecoratedData([NotNull] XmlNodeList defaultData, string parentDataType)
        {
            var values = new JArray();
            try
            {
                var data = ((XmlElement) defaultData[1])?.SelectSingleNode("Structure") ??
                           ((XmlElement) defaultData[1])?.SelectSingleNode("StructureMember");
                if (data != null && data.HasChildNodes)
                {
                    var dataType = data.Attributes?["DataType"].Value ?? "DINT";
                    GetArrayStructure(values, data, dataType);
                    return values;
                }

                data = ((XmlElement) defaultData[1])?.SelectSingleNode("Array") ??
                       ((XmlElement) defaultData[1])?.SelectSingleNode("ArrayMember");
                if (data != null)
                {
                    var singleNode = ((XmlElement) data)?.SelectSingleNode("Element");
                    singleNode = ((XmlElement) singleNode).SelectSingleNode("Structure") ??
                                 ((XmlElement) singleNode).SelectSingleNode("StructureMember");
                    if (singleNode != null && singleNode.HasChildNodes)
                    {
                        var dataType = singleNode.Attributes?["DataType"].Value ?? "DINT";
                        var singleNodeList = ((XmlElement) defaultData[1]).GetElementsByTagName("Element");
                        foreach (XmlNode item in singleNodeList)
                        {
                            if (item.ParentNode.ParentNode == defaultData[1])
                            {
                                GetArrayStructure(values, item, dataType);
                            }
                        }

                    }
                    else if (data.HasChildNodes)
                    {
                        var radix = data.Attributes["Radix"]?.Value;
                        foreach (XmlElement item in data)
                        {
                            var member = new JObject();
                            member["Index"] = item.Attributes["Index"].Value;
                            var value = item.Attributes["Value"].Value;
                            if (radix == "ASCII")
                                value = value.Remove(value.Length - 1).Remove(0, 1);
                            member["Value"] = value;
                            member["DataType"] = parentDataType;
                            if (radix != null)
                                member["Radix"] = radix;
                            member["Type"] = item.Name;
                            values.Add(member);
                        }
                    }

                    return values;
                }

                data = ((XmlElement) defaultData[1])?.SelectSingleNode("DataValue") ??
                       ((XmlElement) defaultData[1])?.SelectSingleNode("DataValueMember");
                if (data != null)
                {
                    var member = new JObject();
                    if (((XmlElement) data).HasAttribute("DataType"))
                        member["DataType"] = data.Attributes["DataType"].Value;
                    if (((XmlElement) data).HasAttribute("Radix"))
                        member["Radix"] = data.Attributes["Radix"].Value;
                    if (((XmlElement) data).HasAttribute("Value"))
                    {
                        var value = data.Attributes["Value"].Value;
                        if ((string) member["Radix"] == "Radix")
                            value = value.Remove(value.Length - 1).Remove(0, 1);
                        member["Value"] = value;
                    }

                    member["Type"] = defaultData[1].Name;
                    values.Add(member);
                    return values;
                }

                {
                    var value = defaultData[0]?.FirstChild?.Value;
                    if (value != null)
                    {
                        var member = new JObject();
                        member["Format"] = defaultData[1].Attributes["Format"].Value;
                        if (member["Format"].ToString() == "String")
                            value = FormatOp.FormatSpecial(value);
                        member["Value"] = value;
                        member["DataType"] = defaultData[1].Attributes["DataType"].Value;
                        member["Type"] = defaultData[1].Name;
                        values.Add(member);
                    }
                    else
                    {
                        var xmlAttributeCollection = defaultData[1].Attributes;
                        if (xmlAttributeCollection != null)
                        {
                            value = xmlAttributeCollection["Value"].Value;
                            var member = new JObject();
                            if (!string.IsNullOrEmpty(xmlAttributeCollection["Format"]?.Value))
                            {
                                member["Format"] = xmlAttributeCollection["Format"]?.Value;
                                if (member["Format"]?.ToString() == "String")
                                    value = FormatOp.FormatSpecial(value);
                            }

                            member["DataType"] = defaultData[1].Attributes["DataType"].Value;
                            member["Value"] = value;
                            member["Type"] = defaultData[1].Name;
                            values.Add(member);
                        }
                    }
                }

                return values;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static JArray GetXmlDecoratedData(XmlElement decoratedData, string parentDataType)
        {
            var values = new JArray();
            try
            {
                var data = decoratedData.SelectSingleNode("Structure") ??
                           decoratedData.SelectSingleNode("StructureMember");
                if (data != null && data.HasChildNodes)
                {
                    var dataType = data.Attributes?["DataType"].Value ?? "DINT";
                    GetArrayStructure(values, data, dataType);
                    return values;
                }

                data = decoratedData.SelectSingleNode("Array") ??
                       decoratedData.SelectSingleNode("ArrayMember");
                if (data != null)
                {
                    var singleNode = ((XmlElement) data)?.SelectSingleNode("Element");
                    singleNode = ((XmlElement) singleNode).SelectSingleNode("Structure") ??
                                 ((XmlElement) singleNode).SelectSingleNode("StructureMember");
                    if (singleNode != null && singleNode.HasChildNodes)
                    {
                        var dataType = singleNode.Attributes?["DataType"].Value ?? "DINT";
                        var singleNodeList = decoratedData.GetElementsByTagName("Element");
                        foreach (XmlNode item in singleNodeList)
                        {
                            if (item.ParentNode.ParentNode == decoratedData)
                            {
                                GetArrayStructure(values, item, dataType);
                            }
                        }

                    }
                    else if (data.HasChildNodes)
                    {
                        var radix = data.Attributes["Radix"]?.Value;
                        foreach (XmlElement item in data)
                        {
                            var member = new JObject();
                            member["Index"] = item.Attributes["Index"].Value;
                            var value = item.Attributes["Value"].Value;
                            if (radix == "ASCII")
                                value = value.Remove(value.Length - 1).Remove(0, 1);
                            member["Value"] = value;
                            member["DataType"] = parentDataType;
                            if (radix != null)
                                member["Radix"] = radix;
                            member["Type"] = item.Name;
                            values.Add(member);
                        }
                    }

                    return values;
                }

                data = decoratedData.SelectSingleNode("DataValue") ??
                       decoratedData.SelectSingleNode("DataValueMember");
                if (data != null)
                {
                    var member = new JObject();
                    if (((XmlElement) data).HasAttribute("DataType"))
                    {
                        member["DataType"] = data.Attributes["DataType"].Value;
                    }

                    if (((XmlElement) data).HasAttribute("Radix"))
                        member["Radix"] = data.Attributes["Radix"].Value;
                    if (((XmlElement) data).HasAttribute("Value"))
                    {
                        var value = data.Attributes["Value"].Value;
                        if ((string) member["Radix"] == "Radix")
                            value = value.Remove(value.Length - 1).Remove(0, 1);
                        member["Value"] = value;
                    }

                    member["Type"] = decoratedData.Name;
                    values.Add(member);
                    return values;
                }

                {
                    var xmlAttributeCollection = decoratedData.Attributes;
                    if (xmlAttributeCollection != null)
                    {
                        var value = "";
                        value = xmlAttributeCollection["Value"].Value;
                        var member = new JObject();
                        if (!string.IsNullOrEmpty(xmlAttributeCollection["Format"]?.Value))
                        {
                            member["Format"] = xmlAttributeCollection["Format"]?.Value;
                            if (member["Format"]?.ToString() == "String")
                                value = FormatOp.FormatSpecial(value);
                        }

                        member["DataType"] = decoratedData.Attributes["DataType"].Value;
                        member["Value"] = value;
                        member["Type"] = decoratedData.Name;
                        values.Add(member);
                    }
                }

                return values;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void ParseModuleTagComments(JObject tag, XmlElement tagElement)
        {
            var commentList = tagElement.SelectNodes("Comments/Comment");
            if(commentList == null) return;
            var comments = new JArray();
            tag.Add("Comments",comments);

            //The Comments of InputTag or OutputTag
            foreach (XmlElement item in commentList)
            {
                var comment = new JObject();
                comment.Add("Operand",item.GetAttribute("Operand"));
                var cdata = item.FirstChild;
                comment.Add("Value",(cdata as XmlCDataSection)?.Value);
                comments.Add(comment);
            }

            {
                //The Description of ConfigTag
                var descriptionNode = tagElement.SelectSingleNode("Description");
                if (descriptionNode == null) return;
                var comment = new JObject();
                comment.Add("Operand", string.Empty);
                comment.Add("Value", (descriptionNode.FirstChild as XmlCDataSection)?.Value);
                comments.Add(comment);
            }
        }

        public static void ParseModuleTagData(JObject tag, XmlElement tagElement)
        {
            var dataList = tagElement.GetElementsByTagName("Data");
            if (dataList.Count == 2)
            {
                var l5KData = dataList[0];
                var data = l5KData.FirstChild.Value;
                tag.Add("Data", JsonConvert.DeserializeObject(data) as JToken);
            }
            else
            {
                //TODO(gjc):need check here,for bool
                var data = DataParse.GetXmlDecoratedData((XmlElement) tagElement.SelectSingleNode("Data"),
                    (string) tag["DataType"]);
                FixData(data);
                var convertData = AOIExtend.ConvertDecoratedToken(0, data, Controller.GetInstance(),
                    (string) tag["DataType"]);

                //FixData(convertData as JArray);
                tag.Add("Data", convertData);
            }
        }

        private static JArray GetArrayStructure(JArray arrayStructure, XmlNode node, string dataType)
        {
            foreach (XmlElement item in node.ChildNodes)
            {
                if (item.Name == "StructureMember" || item.Name == "Structure" || item.Name == "ArrayMember")
                {
                    JArray newArray = new JArray();
                    var memberDataType = item.Attributes["DataType"]?.Value ?? dataType;
                    GetArrayStructure(newArray, item, memberDataType);
                    arrayStructure.Add(newArray);
                }
                else
                {
                    if (item.Name == "DataValueMember")
                    {
                        var member = new JObject();
                        member["Name"] = item.Attributes["Name"].Value;
                        member["DataType"] = item.Attributes["DataType"].Value;
                        if (item.HasAttribute("Radix"))
                            member["Radix"] = item.Attributes["Radix"].Value;
                        if (item.HasAttribute("Value"))
                            member["Value"] = item.Attributes["Value"].Value;
                        else
                        {
                            var value = item.FirstChild?.Value;
                            if (value != null)
                                member["Value"] = value;
                        }

                        if (member["Radix"]?.ToString() == "ASCII")
                        {
                            var value = member["Value"].ToString();
                            if (value.StartsWith("'"))
                                value = value.Remove(value.Length - 1).Remove(0, 1);

                            member["Value"] = FormatOp.FormatSpecial(value);
                            var len = GetL5xUDTStringLen(item, dataType);
                            if (len > 0)
                            {
                                value = (string) member["Value"];
                                value = value.PadRight(len, '\0');
                                member["Value"] = FormatOp.UnFormatSpecial(value);
                            }
                        }

                        if (member["Radix"]?.ToString() == "Binary")
                        {
                            var value = member["Value"].ToString();
                            member["Value"] = Convert.ToInt32(value.Replace("_", "").Replace("2#", ""), 2);
                        }

                        arrayStructure.Add(member);
                    }
                    else if (item.Name == "Element")
                    {
                        if (item.Attributes["Value"] == null)
                        {
                            GetArrayStructure(arrayStructure, item, dataType);
                        }
                        else
                        {
                            var member = new JObject();
                            member["Index"] = item.Attributes["Index"].Value;
                            member["Value"] = item.Attributes["Value"].Value;
                            member["DataType"] = dataType ?? "BOOL";
                            member["Type"] = item.Name;
                            if (member["Radix"]?.ToString() == "Binary")
                            {
                                var value = member["Value"].ToString();
                                member["Value"] = Convert.ToInt32(value.Replace("_", "").Replace("2#", ""), 2);
                            }

                            arrayStructure.Add(member);
                        }
                    }
                    else
                    {
                        GetArrayStructure(arrayStructure, item, dataType);
                    }
                }
            }

            return arrayStructure;
        }

        private static string DataName(DataParseType dataParseType)
        {
            switch (dataParseType)
            {
                case DataParseType.Aoi:
                    return "DefaultData";
                default:
                    return "Data";
            }
        }

        private static int ConvertBOOL(string value)
        {
            if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
                return 0;
            else if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
                return 1;
            throw new Exception("Error bool value");
        }

        private static long DateTimeToLong(string date)
        {
            string format = "DT#yyyy-MM-dd-hh:mm:ss.fff_fffZ";
            var time = DateTime.ParseExact(date, format, CultureInfo.CurrentCulture);
            return time.Ticks;
            //new DateTime(ticks);
        }

        private static JToken L5KParse(XmlElement xmlNode, DataParseType dataParseType, string dataType)
        {
            var data = xmlNode.SelectNodes(DataName(dataParseType));
            if (data == null) return null;
            var value = GetXmlNodeL5KData(data, dataType);
            if (value != null)
            {
                return value;
                //tag.Add(DataName(dataParseType), value);
            }

            return null;
        }

        //private static JToken DecoratedParse(XmlElement xmlNode, DataParseType dataParseType, int[] dims,
        //    string dataType)
        //{
        //    var data = xmlNode.SelectNodes(DataName(dataParseType));
        //    if (!IsAoiDataType(xmlNode, dataType) && data != null)
        //    {
        //        var decoratedData = GetXmlNodeDecoratedData(data, dataType);
        //        if (decoratedData == null)
        //        {
        //            if (Controller.GetInstance().DataTypes[dataType]?.IsAtomic ?? false)
        //            {
        //                return L5KParse(xmlNode, dataParseType, dataType);
        //            }
        //            else
        //            {
        //                throw new Exception("cannot parse unAtomic big data type now.");
        //            }
        //        }

        //        var dataToken = AOIExtend.ConvertDecoratedToken(dims[0], decoratedData, Controller.GetInstance(),
        //            dataType);
        //        if (dataToken is JObject && dataToken["Value"] != null)
        //            dataToken = new JValue(Convert.ToInt32(dataToken["Value"]));
        //        else
        //            FixData(dataToken as JArray);
        //        var value = dataToken;
        //        if (value != null)
        //        {
        //            return value;
        //            //tag.Add(DataName(dataParseType), value);
        //        }
        //    }

        //    return null;
        //}

        private static bool IsAoiDataType(XmlElement xmlNode, string dataType)
        {
            var aois =
                ((XmlElement) xmlNode?.OwnerDocument?.SelectSingleNode(
                    "RSLogix5000Content/Controller/AddOnInstructionDefinitions"))
                ?.GetElementsByTagName("AddOnInstructionDefinition");
            if (aois == null) return false;
            foreach (XmlElement aoi in aois)
            {
                if (aoi.Attributes["Name"].Value.Equals(dataType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
