using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.Tags
{
    public class ObtainValue
    {
        public static Tuple<ITag, string> NameToTag(string name, Hashtable transformTable,
            IProgramModule program2 = null)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (GetLoadTag(name, program2, transformTable) == null)
            {
                return null;
            }

            string specifier = name;
            ITagCollection tagCollection = program2 == null ? Controller.GetInstance().Tags : program2.Tags;
            int index;
            bool isInProgram = false;
            if (name.StartsWith("\\"))
            {
                index = name.IndexOf(".", StringComparison.OrdinalIgnoreCase);
                string program = name.Substring(1, index - 1);
                specifier = name.Substring(index + 1, name.Length - index - 1);

                tagCollection = Controller.GetInstance().Programs[program]?.Tags;
                if (tagCollection == null) return null;
                isInProgram = true;
            }

            index = specifier.IndexOf(".");
            var arrayIndex = specifier.IndexOf("[");
            if (arrayIndex > -1)
            {
                index = index == -1 ? arrayIndex : Math.Min(index, arrayIndex);
            }

            string tagName = index > 0 ? specifier.Substring(0, index) : specifier;
            int flag = tagName.IndexOf("[");
            if (flag > 0)
                tagName = tagName.Substring(0, flag);
            if (transformTable != null)
            {
                var transformName = (string) transformTable[tagName.ToUpper()];
                if (transformName == "??")
                {
                    return new Tuple<ITag, string>(null, null);
                }

                if (!string.IsNullOrEmpty(transformName))
                {
                    var s = transformName.Split('.');
                    //Debug.Assert(s.Length < 3);
                    tagName = s[0];

                    if (tagName.IndexOf("[") > 0)
                    {
                        tagName = tagName.Substring(0, tagName.IndexOf("["));
                    }
                }

            }

            ITag tag = tagCollection[tagName];
            if (tag == null && !isInProgram)
                tag = Controller.GetInstance().Tags[tagName];
            return new Tuple<ITag, string>(tag, specifier);
        }

        public static ASTName GetLoadTag(string name, IProgramModule program, Hashtable transformTable)
        {
            try
            {
                var ast = LoadName(name);
                if (ast == null) return null;
                var typeChecker = new TypeChecker(Controller.GetInstance(), program as IProgram,
                    transformTable!=null?null:program as AoiDefinition,
                    transformTable);
                ast.Accept(typeChecker);
                return ast;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static bool IsConstant(string tagName, Hashtable transformTable=null,
            IProgramModule program2 = null)
        {
            return NameToTag(tagName, transformTable, program2)?.Item1?.IsConstant ?? false;
        }
        
        public static bool IsBaseTag(string name)
        {
            var count = name.Count(c => c == '.');
            if (count > 1) return false;
            if (name.StartsWith("\\") && count == 1)
            {
                return NameToTag(name, null)?.Item1 != null;
            }

            if (count == 0)
            {
                return NameToTag(name, null)?.Item1 != null;
            }

            return false;
        }

        public static ASTName LoadName(string name)
        {
            var stream = new AntlrInputStream(name);
            var lexer = new STGrammarLexer(stream);
            var token = new CommonTokenStream(lexer);
            var parser = new STGrammarParser(token);
            var ast = new STASTGenVisitor().Visit(parser.expr());
            if (!string.IsNullOrEmpty(parser.expr().GetText()))
            {
                return null;
            }
            return ast as ASTName;
        }

        public static IDataType GetDataTypeInfoIgnoreDims(string specifier,IProgramModule program)
        {
            if (string.IsNullOrEmpty(specifier)) return null;
            specifier = GetCodeIgnoreDims(specifier);
            var ctrl = Controller.GetInstance();
            var split = specifier.Split('.');
            var tagName = split[0];
            ITag tag = null;
            int i = 1;
            if (tagName.StartsWith("\\"))
            {
                var programName = tagName.Substring(1);
                var programReference = ctrl.Programs[programName];
                if (split.Length <= i) return null;
                tagName = split[i++];
                tag = programReference?.Tags[tagName];
            }
            else
            {
                tag = program?.Tags[tagName] ?? ctrl.Tags[tagName];
            }
            
            if (tag == null)
            {
                return null;
            }

            var currentDataType = tag.DataTypeInfo.DataType;
            for (; i < split.Length-1; i++)
            {
                if (currentDataType == null) return null;
                var name = split[i];
                var compositiveType = currentDataType as CompositiveType;
                if (compositiveType != null)
                {
                    foreach (var member in compositiveType.TypeMembers)
                    {
                        if (member.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            currentDataType = member.DataTypeInfo.DataType;
                            goto End;
                        }
                    }

                    currentDataType = null;
                    continue;
                }
                End:
                {
                    //ignore
                }
            }

            return currentDataType;
        }

        public static string GetCodeIgnoreDims(string code)
        {
            var str = "";
            int bracketCount = 0;
            foreach (var c in code)
            {
                if(char.IsWhiteSpace(c))continue;
                if (c == '[')
                {
                    bracketCount++;
                    continue;
                }

                if (c == ']')
                {
                    bracketCount--;
                    continue;
                }

                if (bracketCount > 0)
                {
                    continue;
                }
                str = str + c;
            }
            return str;
        }
        
        public static Tuple<int, DataTypeInfo, IField, Tag, DisplayStyle> GetSpecifierDataInfo(string specifier,
            Tag tag, IProgramModule program, Hashtable transport)
        {
            if (string.IsNullOrEmpty(specifier)) return null;
            if (tag == null) tag = ObtainValue.NameToTag(specifier, transport, program)?.Item1 as Tag;
            if (tag == null) return null;
            var astName = ObtainValue.GetLoadTag(specifier, program, transport);
            if (astName == null) return null;

            var count = astName.loaders.nodes.Count;
            if (program is AoiDefinition&&transport==null) count--;
            if (count == 1)
            {
                return new Tuple<int, DataTypeInfo, IField, Tag, DisplayStyle>(0, tag.DataTypeInfo,
                    tag.DataWrapper.Data, tag, tag.DisplayStyle);
            }
            else
            {
                if (program is AoiDefinition&&transport==null)
                {
                    var member = ObtainValue.GetAoiMember(astName, program);
                    if (member == null)
                    {
                        //var inout = GetAoiInoutTag(astName, program);
                        return new Tuple<int, DataTypeInfo, IField, Tag, DisplayStyle>(-1, tag.DataTypeInfo, null, tag,
                            tag.DisplayStyle);
                    }
                    else
                    {
                        var dataInfo = GetValue(member?.DataTypeInfo.DataType, tag.DataWrapper.Data, astName, 2,
                            program, member.DisplayStyle, null);
                        return dataInfo == null
                            ? null
                            : new Tuple<int, DataTypeInfo, IField, Tag, DisplayStyle>(dataInfo.Item1,
                                new DataTypeInfo()
                                {
                                    DataType = astName.Expr.type,
                                    Dim1 = astName.base_dim1,
                                    Dim2 = astName.base_dim2,
                                    Dim3 = astName.dim3
                                }, dataInfo.Item2, tag, member.DisplayStyle);
                    }

                }
                else
                {
                    var dataInfo = GetValue(tag.DataTypeInfo.DataType, tag.DataWrapper.Data, astName, 1, program,
                        tag.DisplayStyle, null);
                    return dataInfo == null
                        ? null
                        : new Tuple<int, DataTypeInfo, IField, Tag, DisplayStyle>(dataInfo.Item1,
                            new DataTypeInfo()
                            {
                                DataType = astName.Expr.type, Dim1 = astName.dim1, Dim2 = astName.dim2,
                                Dim3 = astName.dim3
                            }, dataInfo.Item2, tag, dataInfo.Item3);
                }
            }
        }

        private static Tuple<int, IField, DisplayStyle> GetValue(IDataType dataType, IField field, ASTName astName,
            int index,
            IProgramModule program, DisplayStyle style, List<Tag> dimTags)
        {
            if (index == astName.loaders.nodes.Count())
            {
                return new Tuple<int, IField, DisplayStyle>(0, field, style);
            }

            var node = astName.loaders.nodes[index];
            var array = node as ASTArrayLoader;
            if (array != null)
            {
                var dim = ObtainValue.GetDim(array, program, null, dimTags);
                // field = (field as ArrayField)?.fields[dim].Item1;
                field = GetArrayField(field, dim);
                //if (field is BoolArrayField) _index = dim;
                index++;
                return GetValue(dataType, field, astName, index, program, style, dimTags);
            }

            var tagOffset = node as ASTTagOffset;
            if (tagOffset != null)
            {
                var byteOffset = tagOffset.offset;
                index++;
                var compositiveType = dataType as CompositiveType;
                Debug.Assert(compositiveType != null);
                DataTypeMember member;
                var result = ObtainValue.TryGetBitMember(astName, compositiveType, index, byteOffset, out member);
                if (!result)
                    member = (DataTypeMember) compositiveType.TypeMembers.FirstOrDefault(
                        m => m.ByteOffset == byteOffset);
                Debug.Assert(member != null);
                return GetValue(member.DataTypeInfo.DataType,
                    (field as ICompositeField)?.fields[member.FieldIndex].Item1, astName, index, program,
                    member.DisplayStyle, dimTags);
            }

            var integer = node as ASTInteger;
            if (integer != null)
            {
                index++;
                Debug.Assert(astName.loaders.nodes.Count == index);
                //DataType = astName.type;
                //DisplayStyle = (DisplayStyle)displayStyle;
                if (field is BoolField) return new Tuple<int, IField, DisplayStyle>(0, field, style);
                //return field.GetBitValue((int) integer.value) ? "1" : "0";

                return new Tuple<int, IField, DisplayStyle>((int) integer.value, field, style);
            }

            var bitAstName = node as ASTName;
            if (bitAstName != null)
            {
                index++;
                Debug.Assert(astName.loaders.nodes.Count == index);
                var name = ObtainValue.GetAstName(bitAstName);
                Tag tag = null;
                var bit = ObtainValue.GetTagValue(name, program, null, ref tag);
                return new Tuple<int, IField, DisplayStyle>(int.Parse(bit), field, style);
            }

            var call = node as ASTCall;
            if (call != null)
            {
                //TODO(zyl):get value
                return null;
            }

            Debug.Assert(false);
            return null;
        }

        public static string GetASTNameItemName(ASTNameItem item)
        {
            var name = item.id;
            if (item.arr_list != null && item.arr_list.Count() > 0)
            {
                var arr = "";
                foreach (var node in item.arr_list.nodes)
                {
                    var integer = node as ASTInteger;
                    if (integer != null)
                    {
                        arr = arr + integer.value + ",";
                    }

                    var nameItem = node as ASTNameItem;
                    if (nameItem != null)
                    {
                        arr = arr + GetASTNameItemName(nameItem) + ",";
                    }
                }

                arr = arr.Remove(arr.Length - 1);
                return $"{name}[{arr}]";
            }
            else
            {
                return name;
            }

        }

        /// <summary>
        /// get format code 
        /// </summary>
        /// <param name="astName"></param>
        /// <param name="ss"></param>
        /// <returns></returns>
        public static string GetAstName(ASTName astName)
        {
            try
            {
                if (astName == null) return "";
                var name = "";
                foreach (var id in astName.id_list.nodes)
                {
                    var astNameItem = id as ASTNameItem;
                    if (astNameItem != null)
                    {
                        name = $"{name}.{GetNameItem(astNameItem)}";
                        continue;
                    }

                    var astUnaryOp = id as ASTUnaryOp;
                    if (astUnaryOp != null)
                    {

                        continue;
                    }

                    Debug.Assert(false, id.GetType().ToString());
                }

                name = name.Substring(1);
                if (astName.bit_sel != null)
                {
                    if (astName.bit_sel is ASTInteger)
                        name = $"{name}.{((ASTInteger) astName.bit_sel).value}";
                    else
                        name = $"{name}.[]";
                }

                return name;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetCrossReferenceRegexFromAstName(ASTName astName)
        {
            try
            {
                if (astName == null) return "";
                var name = "";
                foreach (var id in astName.id_list.nodes)
                {
                    var astNameItem = id as ASTNameItem;
                    if (astNameItem != null)
                    {
                        string item ;
                        List<string> dims = new List<string>();
                        if (astNameItem.arr_list?.nodes != null)
                        {
                            foreach (var arr in astNameItem.arr_list.nodes)
                            {
                                if(arr is ASTInteger)
                                {
                                    dims.Insert(0, $"[ ]*{((ASTInteger)arr).value}[ ]*");
                                }
                                else
                                {
                                    dims.Insert(0, "[0-9 ]+");
                                }
                            }

                            item= $"{astNameItem.id}\\[{string.Join(",", dims)}\\]";
                        }
                        else
                        {
                            item = $"{astNameItem.id}";
                        }
                        
                        name = $"{name}\\.{item}";
                        continue;
                    }

                    var astUnaryOp = id as ASTUnaryOp;
                    if (astUnaryOp != null)
                    {

                        continue;
                    }

                    Debug.Assert(false, id.GetType().ToString());
                }

                name = name.Substring(2);
                if (astName.bit_sel != null)
                {
                    if (astName.bit_sel is ASTInteger)
                        name = $"{name}\\.{((ASTInteger)astName.bit_sel).value}";
                    else
                        name = $"{name}\\.\\[\\]";
                }

                return $"^{name}$";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetCrossReferenceParentRegexFromAstName(ASTName astName)
        {
            try
            {
                if (astName == null) return "";
                var name = "";
                int index = 0;
                foreach (var id in astName.id_list.nodes)
                {
                    index++;
                    if (index == astName.id_list.nodes.Count && astName.bit_sel == null) break;
                    var astNameItem = id as ASTNameItem;
                    if (astNameItem != null)
                    {
                        string item;
                        List<string> dims = new List<string>();
                        if (astNameItem.arr_list?.nodes != null)
                        {
                            foreach (var arr in astNameItem.arr_list.nodes)
                            {
                                if (arr is ASTInteger)
                                {
                                    dims.Insert(0, $"[ ]*{((ASTInteger)arr).value}[ ]*");
                                }
                                else
                                {
                                    dims.Insert(0, "[0-9 ]+");
                                }
                            }

                            item = $"{astNameItem.id}\\[{string.Join(",", dims)}\\]";
                        }
                        else
                        {
                            item = $"{astNameItem.id}";
                        }

                        name = $"{name}\\.{item}";
                        continue;
                    }

                    var astUnaryOp = id as ASTUnaryOp;
                    if (astUnaryOp != null)
                    {

                        continue;
                    }

                    Debug.Assert(false, id.GetType().ToString());
                }

                if (!string.IsNullOrEmpty(name))
                {
                    name = name.Substring(2);
                    return name;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetParamName(ASTNode astNode,string codeText)
        {
            return codeText.Substring(astNode.ContextStart, astNode.ContextEnd - astNode.ContextStart + 1);
        }

        public static string GetTagValue(string specifier, IProgramModule program, Hashtable transformTable,
            ref Tag tag,
            bool format = false,bool isGetFirstArrValue=false)
        {
            try
            {
                if (string.IsNullOrEmpty(specifier)) return null;
                
                var astName = GetLoadTag(specifier, program, transformTable);

                if (astName == null) return null;
                tag = astName.Tag as Tag;
                if (tag == null)
                {
                    return null;
                }
                var count = astName.loaders.nodes.Count;
                if (program is AoiDefinition) count--;
                if (count == 1)
                {
                    return GetFieldValue(tag.DataWrapper.Data, format ? tag.DisplayStyle : (DisplayStyle?) null,
                        tag.DataWrapper.DataTypeInfo.DataType.FamilyType == FamilyType.StringFamily,isGetFirstArrValue);
                }
                else
                {

                    if (program is AoiDefinition && tag.Usage != Usage.InOut)
                    {
                        var member = GetAoiMember(astName, program);
                        return GetValue(member?.DataTypeInfo.DataType, tag.DataWrapper.Data, astName, 2,
                            format ? member?.DisplayStyle : (DisplayStyle?) null, program, transformTable, null,isGetFirstArrValue);
                    }
                    else
                    {
                        return GetValue(tag.DataTypeInfo.DataType, tag.DataWrapper.Data, astName, 1,
                            format ? tag.DisplayStyle : (DisplayStyle?) null, program, transformTable, null,isGetFirstArrValue);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.Message);
                return null;
            }
        }

        public static DataTypeInfo GetTargetDataTypeInfo(string specifier, IProgramModule program,
            Hashtable transformTable)
        {
            var dataTypeInfo = new DataTypeInfo();
            if (string.IsNullOrEmpty(specifier)) return dataTypeInfo;
            var number = LoadNumber(specifier);
            if (!string.IsNullOrEmpty(number))
            {
                if (number.IndexOf(".") > 0)
                    dataTypeInfo.DataType = REAL.Inst;
                else
                    dataTypeInfo.DataType = DINT.Inst;
                return dataTypeInfo;
            }

            var astName = GetLoadTag(specifier, program, transformTable);
            if (astName == null) return dataTypeInfo;
            dataTypeInfo.DataType = astName.type;
            dataTypeInfo.Dim1 = astName.dim1;
            dataTypeInfo.Dim2 = astName.dim2;
            dataTypeInfo.Dim3 = astName.dim3;
            return dataTypeInfo;
        }

        public static string LoadNumber(string value)
        {
            try
            {
                value = value?.Trim();
                if (string.IsNullOrEmpty(value)) return "";
                var stream = new AntlrInputStream(value);
                var lexer = new STGrammarLexer(stream);
                var token = new CommonTokenStream(lexer);
                var parser = new STGrammarParser(token);
                var ast = new STASTGenVisitor().Visit(parser.expr());
                var typeChecker = new TypeChecker(Controller.GetInstance(), null, null);
                if (ast is ASTInteger || ast is ASTFloat)
                {
                    ast.Accept(typeChecker);
                    var result = (ast as ASTInteger)?.value.ToString();
                    if (string.IsNullOrEmpty(result))
                    {
                        result = (ast as ASTFloat)?.value.ToString("e9");
                    }

                    return result;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return "";
        }

        public static int GetDim(ASTArrayLoader array, IProgramModule program, Hashtable transformTable,
            List<Tag> dimTags)
        {
            int dim1 = 0, dim2 = 0, dim3=0;
            if (array.dims.nodes.Count == 3)
            {
                var result3 = TryGetNodeValue(array.dims.nodes[2], program, transformTable, dimTags,ref dim3);
                var result2 = TryGetNodeValue(array.dims.nodes[1], program, transformTable, dimTags,ref dim2);
                var result1 = TryGetNodeValue(array.dims.nodes[0], program, transformTable, dimTags,ref dim1);
                if (!(result3&result2&result1)) return -1;
                return dim3 * array.info.Dim2 * array.info.Dim1 + dim2 * array.info.Dim1 + dim1;
            }
            else if (array.dims.nodes.Count == 2)
            {
                var result2 = TryGetNodeValue(array.dims.nodes[1], program, transformTable, dimTags,ref dim2);
                var result1 = TryGetNodeValue(array.dims.nodes[0], program, transformTable, dimTags,ref dim1);
                if (!(result1&result2)) return -1;
                return dim2 * array.info.Dim1 + dim1;
            }
            else if (array.dims.nodes.Count == 1)
            {
                var result= TryGetNodeValue(array.dims.nodes[0], program, transformTable, dimTags,ref dim1);
                if (!result) return -1;
                return dim1;
            }

            Debug.Assert(false);

            throw new Exception("error dim");
        }

        public static IDataTypeMember GetAoiMember(ASTName astName, IProgramModule program)
        {
            try
            {
                if (program is AoiDefinition)
                {
                    var astExprParameter = GetAstExprParameter(astName.Expr);
                    if (astExprParameter != null)
                    {
                        var slotNo = astExprParameter.SlotNo;
                        if (slotNo > 0)
                        {
                            return null;
                            //var inout = program.Tags.Where(t => t.Usage == Usage.InOut).ToArray()[slotNo - 1];
                            //return new DataTypeMember() {DataType = inout.DataTypeInfo.DataType,DisplayStyle = inout.DisplayStyle};
                        }
                        else
                        {
                            var tagOffset = astName.loaders.nodes[1] as ASTTagOffset;
                            Debug.Assert(tagOffset != null);
                            var aoiType = program.ParentController.DataTypes[program.Name];
                            return ((CompositiveType) aoiType).TypeMembers.FirstOrDefault(m =>
                                m.ByteOffset == tagOffset.offset);
                        }
                    }

                    //var astExprArray = astName.Expr as ASTExprArraySubscript;
                    //if (astExprArray != null)
                    //{
                    //    var astMember = astExprArray.Expr as ASTExprMember;
                    //    if (astMember != null)
                    //    {
                    //        var aoiType = program.ParentController.DataTypes[program.Name];
                    //        return ((CompositiveType)aoiType).TypeMembers.FirstOrDefault(m =>
                    //            m.ByteOffset == astMember.Offset);
                    //    }
                    //}

                    //var astExprBit = astName.Expr as ASTExprBitSel;
                    //if (astExprBit != null)
                    //{
                    //    var astMember = astExprBit.Expr as ASTExprMember;
                    //    if (astMember != null)
                    //    {
                    //        var aoiType = program.ParentController.DataTypes[program.Name];
                    //        return ((CompositiveType)aoiType).TypeMembers.FirstOrDefault(m =>
                    //            m.ByteOffset == astMember.Offset&&m.BitOffset==astExprBit.ContextEnd&&m.IsBit);
                    //    }
                    //}

                    //var astExprMember = astName.Expr as ASTExprMember;
                    //if (astExprMember != null)
                    //{
                    //    var aoiType = program.ParentController.DataTypes[program.Name];
                    //    return ((CompositiveType)aoiType).TypeMembers.FirstOrDefault(m =>
                    //        m.ByteOffset == astExprMember.Offset);
                    //}
                    Debug.Assert(false);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        public static string RemoveScope(string name)
        {
            if (name.StartsWith("\\"))
            {
                var dotIndex = name.IndexOf(".");
                if (dotIndex < 0) return name;
                return name.Substring(dotIndex + 1);
            }
            else
            {
                return name;
            }
        }

        private static ASTExprParameter GetAstExprParameter(ASTNode node)
        {
            var astExprParameter = node as ASTExprParameter;
            if (astExprParameter != null)
            {
                return astExprParameter;
            }

            var astExprArray = node as ASTExprArraySubscript;
            if (astExprArray != null)
            {
                return GetAstExprParameter(astExprArray.Expr);
            }

            var astExprBit = node as ASTExprBitSel;
            if (astExprBit != null)
            {
                return GetAstExprParameter(astExprBit.Expr);
            }

            var astExprMember = node as ASTExprMember;
            if (astExprMember != null)
            {
                return GetAstExprParameter(astExprMember.Expr);
            }

            return null;
        }

        public static ITag GetAoiInoutTag(ASTName astName, IProgramModule program)
        {
            try
            {
                var aoi = program as AoiDefinition;
                if (aoi == null) return null;
                var slotNo = (astName.Expr as ASTExprParameter).SlotNo;
                if (slotNo > 0)
                {
                    return program.Tags.Where(t => t.Usage == Usage.InOut).ToArray()[slotNo - 1];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

            return null;
        }

        public static IField GetArrayField(IField field, int index)
        {
            var boolArray = field as BoolArrayField;
            if (boolArray != null)
            {
                return new BoolField(boolArray.Get(index) ? 1 : 0);
            }

            var array = field as ArrayField;
            if (array != null)
            {
                if (array.fields.Count <= index || index < 0) return null;
                return array.fields[index].Item1;
            }

            //add other array
            Debug.Assert(false);
            return null;
        }

        public static Tuple<IField, ITag> GetTagField(string specifier, IProgramModule program, Hashtable transformTable)
        {
            if (string.IsNullOrEmpty(specifier)) return null;
            var tag = NameToTag(specifier, transformTable, program)?.Item1 as Tag;
            var astName = GetLoadTag(specifier, program, transformTable);
            if (astName == null) return null;
            var count = astName.loaders.nodes.Count;
            if (program is AoiDefinition) count--;
            if (count == 1)
            {
                return new Tuple<IField, ITag>(tag.DataWrapper.Data,tag);
            }
            else
            {

                if (program is AoiDefinition)
                {
                    var member = GetAoiMember(astName, program);
                    return new Tuple<IField, ITag>(GetField(member?.DataTypeInfo.DataType, tag.DataWrapper.Data, astName, 2, program,
                        transformTable, null),tag);
                }
                else
                {
                    return new Tuple<IField, ITag>(GetField(tag.DataTypeInfo.DataType, tag.DataWrapper.Data, astName, 1, program,
                        transformTable, null),tag);
                }
            }
        }

        public static bool TryGetBitMember(ASTName astName, CompositiveType compositiveType, int index, int byteOffset,
            out DataTypeMember bitMember)
        {
            bitMember = null;
            if (astName.loaders.nodes.Count > index)
            {
                var integer = astName.loaders.nodes[index] as ASTInteger;
                if (integer == null)
                {
                    return false;
                }
                else
                {
                    var member =
                        (DataTypeMember) compositiveType.TypeMembers.FirstOrDefault(m =>
                            m.ByteOffset == byteOffset && m.BitOffset == integer.value && m.IsBit);
                    if (member == null) return false;
                    else
                    {
                        bitMember = member;
                        return true;
                    }
                }
            }

            return false;
        }

        public static string GetParentName(IDataType dataType, List<string> name, int level, string currentName)
        {
            try
            {
                var sub = name[level];
                if (sub.StartsWith("\\"))
                {
                    return GetParentName(dataType, name, ++level, currentName);
                }

                if (string.IsNullOrEmpty(currentName))
                {
                    currentName += $".{sub}";
                    return GetParentName(dataType, name, ++level, currentName);
                }

                if (dataType.IsNumber)
                {
                    return currentName == "" ? "" : currentName.Substring(1);
                }

                var compositive = dataType as CompositiveType;
                if (compositive != null)
                {
                    foreach (DataTypeMember member in compositive.TypeMembers)
                    {
                        if (member.Name.Equals(sub, StringComparison.OrdinalIgnoreCase))
                        {
                            if (level < name.Count - 1)
                            {
                                currentName += $".{sub}";
                                return GetParentName(member.DataTypeInfo.DataType, name, ++level, currentName);
                            }
                            else
                            {
                                if (!(compositive is AOIDataType)&&member.IsBit && member.DataTypeInfo.Dim1 == 0)
                                {
                                    var parent = compositive.TypeMembers.FirstOrDefault(m =>
                                        !m.IsBit && ((DataTypeMember) m).FieldIndex == member.FieldIndex);
                                    if (parent == null)
                                        return "";
                                    currentName += $".{parent.Name}";
                                    return currentName.Substring(1);
                                }
                            }
                        }
                    }
                }

                return currentName == "" ? "" : currentName.Substring(1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

            return "";
        }

        public static List<string> GetChildrenName(IDataType dataType, List<string> name, int level, string currentName)
        {
            try
            {
                var sub = name[level];
                if (sub.StartsWith("\\"))
                {
                    return GetChildrenName(dataType, name, ++level, currentName);
                }

                if (string.IsNullOrEmpty(currentName))
                {
                    currentName += $".{sub}";
                    return GetChildrenName(dataType, name, ++level, currentName);
                }

                if (dataType.IsNumber)
                {
                    return null;
                }

                var compositive = dataType as CompositiveType;
                if (compositive != null)
                {
                    foreach (DataTypeMember member in compositive.TypeMembers)
                    {
                        if (member.Name.Equals(sub, StringComparison.OrdinalIgnoreCase))
                        {
                            if (level == name.Count - 1)
                            {
                                if (member.IsBit) return null;
                                return compositive.TypeMembers
                                    .Where(t => ((DataTypeMember) t).FieldIndex == member.FieldIndex && t.IsBit)
                                    .Select(t => $"{currentName}.{t.Name}".Substring(1)).ToList();
                            }

                            currentName += $".{sub}";
                            return GetChildrenName(member.DataTypeInfo.DataType, name, ++level, currentName);

                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static bool ParseDims(string dims, ref int dim1, ref int dim2, ref int dim3)
        {
            MatchCollection matchCollection =
                Regex.Matches(dims, @"(?<=\W+)\d+",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
            switch (matchCollection.Count)
            {
                case 1:
                    if (int.TryParse(matchCollection[0].Value, out dim1))
                    {
                        if (dim1 == 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                    break;
                case 2:
                    if (int.TryParse(matchCollection[1].Value, out dim1) &&
                        int.TryParse(matchCollection[0].Value, out dim2))
                    {
                        if (dim1 == 0 || dim2 == 0)
                        {
                            return false;
                        }
                    }
                    else
                    {
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
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }

                    break;
            }

            return true;
        }
        
        public static bool HasVariableInDim(ASTNode astNode)
        {
            var astName = astNode as ASTName;
            if (astName != null)
            {
                if (astName.Expr is ASTExprArraySubscript)
                {
                    var astExprArraySubscript = (ASTExprArraySubscript)astName.Expr;
                    if ((IsVariable(astExprArraySubscript.Index0) || IsVariable(astExprArraySubscript.Index1) ||
                         IsVariable(astExprArraySubscript.Index2))) return true;
                }

                return false;
            }

            var astNameAttr = astNode as ASTNameAddr;
            if (astNameAttr != null)
            {
                return HasVariableInDim(astNameAttr.name);
            }

            return false;
        }

        public static bool HasVariableInDim(string code)
        {
            var dimsRegex=new Regex(@"\[[\w ,]+\]");
            var matches = dimsRegex.Matches(code);
            if (matches.Count > 0)
            {
                var regex=new Regex(@"\[[\d ,]+\]");
                foreach (Match match in matches)
                {
                    if (!regex.IsMatch(match.Value)) return true;
                }
            }

            return false;
        }

        private static bool IsVariable(ASTExpr astExpr)
        {
            if (astExpr == null) return false;
            if (astExpr is ASTInteger) return false;
            return true;
        }


        private static string GetNameItem(ASTNameItem nameItem)
        {
            List<string> dims = new List<string>();
            if (nameItem.arr_list?.nodes != null)
            {
                foreach (var arr in nameItem.arr_list.nodes)
                {
                    dims.Insert(0, GetAstNodeOriginalName(arr));
                }

                return $"{nameItem.id}[{string.Join(",", dims)}]";
            }

            return $"{nameItem.id}";
        }

        private static string GetAstNodeOriginalName(ASTNode astNode)
        {
            var astInteger = astNode as ASTInteger;
            if (astInteger != null)
            {
                return astInteger.value.ToString();
            }

            var astName = astNode as ASTName;
            if (astName != null)
            {
                return GetAstName(astName);
            }

            var astBinArithOp = astNode as ASTBinArithOp;
            if (astBinArithOp != null)
            {
                return GetAstBinArithOp(astBinArithOp);
            }

            var astCall = astNode as ASTCall;
            if (astCall != null)
            {
                return GetAstCall(astCall);
            }

            var astUnaryOp = astNode as ASTUnaryOp;
            if (astUnaryOp != null)
            {
                if (astUnaryOp.expr is ASTInteger)
                {
                    return (astUnaryOp.expr as ASTInteger).value.ToString();
                }

                if (astUnaryOp.expr is ASTFloat)
                {
                    return (astUnaryOp.expr as ASTFloat).value.ToString();
                }
            }

            var astFloat = astNode as ASTFloat;
            if (astFloat != null)
            {
                return astFloat.value.ToString();
            }

            Debug.Assert(false, astNode?.GetType().FullName ?? "");

            return "";
        }

        private static string GetAstCall(ASTCall astCall)
        {
            List<string> paramsList = new List<string>();
            foreach (var param in astCall.param_list.nodes)
            {
                paramsList.Insert(0, GetAstNodeOriginalName(param));
            }

            return $"{astCall.name}({string.Join(",", paramsList)})";
        }

        private static string GetAstBinArithOp(ASTBinArithOp astBinArithOp)
        {
            var str = "";
            var left = astBinArithOp.left;
            {
                str = GetAstNodeOriginalName(left);
            }
            var op = astBinArithOp.op;
            {
                var o = "";
                switch (op)
                {
                    case ASTBinOp.Op.NOP:
                        o = " nop ";
                        break;
                    case ASTBinOp.Op.OR:
                        o = " or ";
                        break;
                    case ASTBinOp.Op.XOR:
                        o = " xor ";
                        break;
                    case ASTBinOp.Op.AND:
                        o = " & ";
                        break;
                    case ASTBinOp.Op.EQ:
                        o = " = ";
                        break;
                    case ASTBinOp.Op.NEQ:
                        o = " <> ";
                        break;
                    case ASTBinOp.Op.LE:
                        o = " <= ";
                        break;
                    case ASTBinOp.Op.LT:
                        o = " < ";
                        break;
                    case ASTBinOp.Op.GE:
                        o = " >= ";
                        break;
                    case ASTBinOp.Op.GT:
                        o = " > ";
                        break;
                    case ASTBinOp.Op.PLUS:
                        o = " + ";
                        break;
                    case ASTBinOp.Op.MINUS:
                        o = " - ";
                        break;
                    case ASTBinOp.Op.TIMES:
                        o = " * ";
                        break;
                    case ASTBinOp.Op.DIVIDE:
                        o = " / ";
                        break;
                    case ASTBinOp.Op.MOD:
                        o = " mod ";
                        break;
                    case ASTBinOp.Op.POW:
                        o = " ** ";
                        break;
                }

                str = $"{str}{o}";
            }

            var right = astBinArithOp.right;
            {
                str = $"{str}{GetAstNodeOriginalName(right)}";
            }
            return str;
        }

        public static string ConvertOp(ASTBinOp.Op op)
        {
            var o = "";
            switch (op)
            {
                case ASTBinOp.Op.NOP:
                    o = " nop ";
                    break;
                case ASTBinOp.Op.OR:
                    o = " or ";
                    break;
                case ASTBinOp.Op.XOR:
                    o = " xor ";
                    break;
                case ASTBinOp.Op.AND:
                    o = " & ";
                    break;
                case ASTBinOp.Op.EQ:
                    o = " = ";
                    break;
                case ASTBinOp.Op.NEQ:
                    o = " <> ";
                    break;
                case ASTBinOp.Op.LE:
                    o = " <= ";
                    break;
                case ASTBinOp.Op.LT:
                    o = " < ";
                    break;
                case ASTBinOp.Op.GE:
                    o = " >= ";
                    break;
                case ASTBinOp.Op.GT:
                    o = " > ";
                    break;
                case ASTBinOp.Op.PLUS:
                    o = " + ";
                    break;
                case ASTBinOp.Op.MINUS:
                    o = " - ";
                    break;
                case ASTBinOp.Op.TIMES:
                    o = " * ";
                    break;
                case ASTBinOp.Op.DIVIDE:
                    o = " / ";
                    break;
                case ASTBinOp.Op.MOD:
                    o = " mod ";
                    break;
                case ASTBinOp.Op.POW:
                    o = " ** ";
                    break;
            }

            return o;
        }

        public static string ConvertASTUnaryOp(ASTUnaryOp.Op op)
        {
            switch (op)
            {
                case ASTUnaryOp.Op.NEG:
                    return "-";
                case ASTUnaryOp.Op.PLUS:
                    return "+";
                case ASTUnaryOp.Op.NOT:
                    return "not";
                case ASTUnaryOp.Op.NOP:
                    return "";
            }
            Debug.Assert(false);
            return "";
        }

        private static bool TryGetNodeValue(ASTNode node, IProgramModule program, Hashtable transformTable,
            List<Tag> dimTags,ref int value)
        {
            var integer = node as ASTInteger;
            if (integer != null)
            {
                value = (int) integer.value;
                return true;
            }

            var astName = node as ASTName;
            if (astName != null)
            {
                if (astName.type.IsInteger)
                {
                    int result;
                    var name = GetAstName(astName);
                    if (name.EndsWith(":")) name = name.Substring(0, name.Length - 1);
                    Tag tag = null;
                    var val = GetTagValue(name, program, transformTable, ref tag);
                    if (tag != null && dimTags != null)
                    {
                        if (!dimTags.Contains(tag))
                            dimTags.Add(tag);
                    }

                    var flag = int.TryParse(val, out result);
                    value = result;
                    return flag;
                }

                Debug.Assert(false);
            }

            var astBinOp = node as ASTBinOp;
            if (astBinOp != null)
            {
                if (TryGetASTBinArithOpValue(astBinOp, program,dimTags,transformTable, ref value))
                    return true;
            }

            var astCall = node as ASTCall;
            if (astCall != null)
            {
                if ("ABS".Equals(astCall.instr.Name))
                {
                    var result = TryGetNodeValue(astCall.param_list.nodes[0], program, transformTable, dimTags,ref value);
                    value = Math.Abs(value);
                    return result;
                }
            }

            var astInstr = node as ASTInstr;
            if (astInstr != null)
            {
                if ("ABS".Equals(astInstr.instr.Name))
                {
                    var result = TryGetNodeValue(astCall.param_list.nodes[0], program, transformTable, dimTags, ref value);
                    value = Math.Abs(value);
                    return result;
                }
            }
            return false;
        }

        public static string GetStringData(IField field,IDataType dataType)
        {
            if (field is ArrayField) return null;
            if (dataType.FamilyType != FamilyType.StringFamily) return "";
            try
            {
                if (dataType is STRING)
                {
                    Debug.Assert(field is STRINGField);
                    var stringField = (STRINGField)field;
                    var len = ((Int32Field)stringField.fields[0].Item1).value;
                    var str = "";
                    var strFiled = (ArrayField)stringField.fields[1].Item1;
                    List<byte> bytes = new List<byte>();
                    for (int i = 0; i < len; i++)
                    {
                        if (strFiled.fields.Count > i)
                            bytes.Add((byte)((Int8Field)strFiled.fields[i].Item1).value);
                        else
                            bytes.Add(0);
                    }

                    str = Encoding.ASCII.GetString(bytes.ToArray());
                    return str;
                }
                else
                {
                    var arrayField = field as UserDefinedField;
                    Debug.Assert(arrayField != null && arrayField.fields.Count == 2);
                    var len = ((Int32Field)arrayField.fields[0].Item1).value;
                    var str = "";
                    var strFiled = (ArrayField)arrayField.fields[1].Item1;
                    List<byte> bytes = new List<byte>();
                    for (int i = 0; i < len; i++)
                    {
                        if (strFiled.fields.Count > i)
                            bytes.Add((byte)((Int8Field)strFiled.fields[i].Item1).value);
                        else
                            bytes.Add(0);
                    }

                    str = Encoding.ASCII.GetString(bytes.ToArray());
                    return str;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static string GetValue(IDataType dataType, IField field, ASTName astName, int index,
            DisplayStyle? displayStyle, IProgramModule program, Hashtable transformTable, List<Tag> dimTags,bool isGetFirstArrValue)
        {
            if (dataType == null) return null;
            if (index == astName.loaders.nodes.Count())
            {
                return GetFieldValue(field, displayStyle, dataType.FamilyType == FamilyType.StringFamily,isGetFirstArrValue);
            }

            var node = astName.loaders.nodes[index];
            var array = node as ASTArrayLoader;
            if (array != null)
            {
                var dim = GetDim(array, program, transformTable, dimTags);
                //field = (field as ArrayField)?.fields[dim].Item1;
                field = GetArrayField(field, dim);
                index++;
                return GetValue(dataType, field, astName, index, displayStyle, program, transformTable, dimTags,isGetFirstArrValue);
            }

            var tagOffset = node as ASTTagOffset;
            var compositiveType = dataType as CompositiveType;
            if (tagOffset != null)
            {
                Debug.Assert(compositiveType != null);
                var byteOffset = tagOffset.offset;
                index++;
                DataTypeMember member;
                var result = TryGetBitMember(astName, compositiveType, index + 1, byteOffset, out member);
                if (!result)
                    member = (DataTypeMember) compositiveType.TypeMembers.FirstOrDefault(
                        m => m.ByteOffset == byteOffset);
                Debug.Assert(member != null);
                if (displayStyle != null) displayStyle = member.DisplayStyle;
                return GetValue(member.DataTypeInfo.DataType,
                    (field as ICompositeField)?.fields[member.FieldIndex].Item1, astName, index, displayStyle,
                    program, transformTable, dimTags,isGetFirstArrValue);
            }

            var integer = node as ASTInteger;
            if (integer != null)
            {
                index++;
                Debug.Assert(astName.loaders.nodes.Count == index);
                Debug.Assert(!(field is BoolField));
                //if (field is BoolField) return (field as BoolField).value.ToString();
                return field.GetBitValue((int) integer.value) ? "1" : "0";
            }

            var bitAstName = node as ASTName;
            if (bitAstName != null)
            {
                index++;
                Debug.Assert(astName.loaders.nodes.Count == index);
                var name = ObtainValue.GetAstName(bitAstName);
                Tag tag = null;
                var bit = ObtainValue.GetTagValue(name, program, transformTable, ref tag);
                return field.GetBitValue(int.Parse(bit)) ? "1" : "0";
            }

            var call = node as ASTCall;
            if (call != null)
            {
                //TODO(zyl):get value
                return null;
            }

            Debug.Assert(false, node.ToString());
            return null;
        }

        private static IField GetField(IDataType dataType, IField field, ASTName astName, int index,
            IProgramModule program, Hashtable transformTable, List<Tag> dimTags)
        {
            if (dataType == null) return null;
            if (index == astName.loaders.nodes.Count())
            {
                return field;
            }

            var node = astName.loaders.nodes[index];
            var array = node as ASTArrayLoader;
            if (array != null)
            {
                var dim = GetDim(array, program, transformTable, dimTags);
                //field = (field as ArrayField)?.fields[dim].Item1;
                field = GetArrayField(field, dim);
                index++;
                return GetField(dataType, field, astName, index, program, transformTable, dimTags);
            }

            var tagOffset = node as ASTTagOffset;
            var compositiveType = dataType as CompositiveType;
            if (tagOffset != null)
            {
                Debug.Assert(compositiveType != null);
                var byteOffset = tagOffset.offset;
                index++;
                var member =
                    (DataTypeMember) compositiveType.TypeMembers.FirstOrDefault(m => m.ByteOffset == byteOffset);
                Debug.Assert(member != null);
                return GetField(member.DataTypeInfo.DataType,
                    (field as ICompositeField)?.fields[member.FieldIndex].Item1, astName, index,
                    program, transformTable, dimTags);
            }

            var integer = node as ASTInteger;
            if (integer != null)
            {
                index++;
                Debug.Assert(astName.loaders.nodes.Count == index);
                if (field is BoolField) return field;
                return new SpecialField(field, (int) integer.value);
            }

            Debug.Assert(false);
            return null;
        }

        public static bool TryGetASTBinArithOpValue(ASTBinOp astBinArithOp,IProgramModule program,List<Tag> dimTags, Hashtable transformTable, ref int value)
        {
            try
            {
                var right = astBinArithOp.right;
                int r=0;
                if (right is ASTBinOp)
                {
                    var res=TryGetASTBinArithOpValue((ASTBinOp) right, program, dimTags, transformTable, ref r);
                    if (!res) return false;
                }
                else
                {
                    var result = TryGetNodeValue(right, program, null, null,ref r);
                    if (!result)
                        return false;
                }

                var left = astBinArithOp.left;
                int l = 0;
                if (left is ASTBinOp)
                {
                    var res = TryGetASTBinArithOpValue((ASTBinOp)left, program, dimTags, transformTable, ref l);
                    if (!res) return false;
                }
                else
                {
                    var result = TryGetNodeValue(left, program, transformTable, dimTags,ref l);
                    if (!result)
                        return false;
                }
                
                return TryCalculate(l,r,astBinArithOp.op,ref value);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TryCalculate(int l,int r,ASTBinOp.Op op,ref int value)
        {
            unchecked
            {
                switch (op)
                {
                    case ASTBinOp.Op.MINUS:
                        value = l - r;
                        return true;
                    case ASTBinOp.Op.PLUS:
                        value = l + r;
                        return true;
                    case ASTBinOp.Op.TIMES:
                        value = l * r;
                        return true;
                    case ASTBinOp.Op.DIVIDE:
                        value = l / r;
                        return true;
                    case ASTBinOp.Op.MOD:
                        value = l % r;
                        return true;
                    case ASTBinOp.Op.POW:
                        value = (int)Math.Pow(l, r);
                        return true;
                    case ASTBinOp.Op.OR:
                        value = l | r;
                        return true;
                    case ASTBinOp.Op.AND:
                        value = l & r;
                        return true;
                    case ASTBinOp.Op.XOR:
                        value = l ^ r;
                        return true;
                    case ASTBinOp.Op.NEQ:
                        value = l != r ? 1 : 0;
                        return true;
                    case ASTBinOp.Op.EQ:
                        value = l == r ? 1 : 0;
                        return true;
                    case ASTBinOp.Op.LT:
                        value = l < r ? 1 : 0;
                        return true;
                    case ASTBinOp.Op.LE:
                        value = l <= r ? 1 : 0;
                        return true;
                    case ASTBinOp.Op.GT:
                        value = l > r ? 1 : 0;
                        return true;
                    case ASTBinOp.Op.GE:
                        value = 1 >= r ? 1 : 0;
                        return true;
                    default:
                        Debug.Assert(false,op.ToString());
                        return false;
                }
            }
        }

        private static string GetFieldValue(IField field, DisplayStyle? displayStyle, bool isString,bool isGetFirstArrValue)
        {
            if (isString)
            {
                return GetStringFieldValue(field);
            }

            if (field == null || field is UserDefinedField||field is ArrayField)
            {
                if (isGetFirstArrValue&&field is ArrayField)
                {
                    return GetFieldValue(((ArrayField) field).fields[0].Item1, displayStyle, isString,
                        isGetFirstArrValue);
                }
                return null;
            }
            if (displayStyle != null) return FormatOp.ConvertField(field, (DisplayStyle) displayStyle);
            var boolField = field as BoolField;
            if (boolField != null)
            {
                return boolField.value.ToString();
            }

            Int8Field int8Field = field as Int8Field;
            if (int8Field != null)
            {
                return int8Field.value.ToString();
            }

            Int16Field int16Field = field as Int16Field;
            if (int16Field != null)
            {
                return int16Field.value.ToString();
            }

            Int32Field int32Field = field as Int32Field;
            if (int32Field != null)
            {
                return int32Field.value.ToString();
            }

            Int64Field int64Field = field as Int64Field;
            if (int64Field != null)
            {
                return int64Field.value.ToString();
            }

            RealField realField = field as RealField;
            if (realField != null)
            {
                var result = realField.value.ToString();
                if (!result.Contains(".")) result += ".0";
                return result;
            }

            LRealField lRealField = field as LRealField;
            if (lRealField != null)
            {
                return lRealField.value.ToString();
            }

            Debug.Assert(field is ICompositeField, $"can not parse {field.GetType().FullName}");
            return null;
        }

        private static string GetStringFieldValue(IField field)
        {
            try
            {
                ArrayField arrayField;
                Int32Field lenField;
                if (field is UserDefinedField)
                {
                    lenField = (field as UserDefinedField).fields[0].Item1 as Int32Field;
                    arrayField = (field as UserDefinedField).fields[1].Item1 as ArrayField;
                }
                else
                {
                    lenField = (field as STRINGField)?.fields[0].Item1 as Int32Field;
                    arrayField = (field as STRINGField)?.fields[1].Item1 as ArrayField;
                }

                if (arrayField == null) throw new Exception("error string field");
                var len = lenField?.value ?? 0;
                var str = "";
                for (int i = 0; i < len; i++)
                {
                    var s = (arrayField.fields[i].Item1 as Int8Field)?.value.ToString(DisplayStyle.Ascii) ?? "";
                    s = s.Remove(s.Length - 1).Remove(0, 1);
                    str += s;
                }

                return $"'{str}'";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return "''";
        }
    }

    public class SpecialField : IField
    {
        private readonly IField _field;
        private readonly int _offset;

        public SpecialField(IField field, int offset)
        {
            _field = field;
            _offset = offset;
        }

        public IField Field => _field;

        public int Offset => _offset;

        public bool value => _field.GetBitValue(_offset);

        public void SetValue(bool val)
        {
            _field.SetBitValue(_offset, val);
        }

        public JToken ToJToken()
        {
            return null;
        }

        public IField DeepCopy()
        {
            return _field.DeepCopy();
        }

        public virtual void ToMsgPack(List<byte> data)
        {
            Debug.Assert(false);
        }

    }
}
