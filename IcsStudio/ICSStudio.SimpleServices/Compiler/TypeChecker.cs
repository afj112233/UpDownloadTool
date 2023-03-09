using System.Diagnostics;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using System;
using System.Collections;
using System.Linq;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.SimpleServices.Compiler
{
    public class TypeChecker : IASTBaseVisitor
    {
        private readonly Controller controller;
        private readonly IProgram program;
        private readonly AoiDefinition context;
        private readonly IDataType BOOL = PredefinedType.BOOL.Inst;
        private readonly IDataType DBOOL = PredefinedType.BOOL.DInst;
        private readonly IDataType REAL = PredefinedType.REAL.Inst;
        private readonly IDataType LREAL = PredefinedType.LREAL.Inst;

        private readonly IDataType SINT = PredefinedType.SINT.Inst;
        private readonly IDataType INT = PredefinedType.INT.Inst;
        private readonly IDataType DINT = PredefinedType.DINT.Inst;
        private readonly IDataType LINT = PredefinedType.LINT.Inst;

        private readonly Hashtable _transformTable;

        /*
        public static bool IsMatch(IDataType src, IDataType dest)
        {
            if (dest is AXIS_COMMON)
                return src is AXIS_COMMON;
            if (dest.IsNumber) return src.IsNumber;
            return src == dest;
        }
        */

        bool IsInAOI()
        {
            return this.context != null;
        }

        //FIXME not only AOIDataType 
        public TypeChecker(Controller controller, IProgram program, AoiDefinition context,
            Hashtable transformTable = null)
        {
            this._transformTable = transformTable;
            this.controller = controller;
            this.program = program;
            this.context = context;
        }

        #region

        private bool IsNumber(IDataType type)
        {
            return type.IsReal || type.IsInteger;
        }

        private IDataType ArithCommonType(ASTBinArithOp.Op op, IDataType left, IDataType right)
        {
            if (op == ASTBinArithOp.Op.POW)
            {
                return LREAL;
            }

            if (left.IsReal || right.IsReal)
            {
                return REAL;
            }

            return DINT;
        }

        private IDataType LogicCommonType(IDataType left, IDataType right)
        {
            if (left.IsBool && right.IsBool)
            {
                return BOOL;
            }
            else if (IsNumber(left) && IsNumber(right))
            {
                return DINT;
            }
            else if (left.IsBool || right.IsBool)
            {
                return BOOL;
            }
            else
            {
                Debug.Assert(false);
            }

            return DINT;
        }

        private IDataType RelCommonType(IDataType left, IDataType right)
        {
            if (left.IsReal || right.IsReal)
            {
                return REAL;
            }
            else if (left.IsInteger && right.IsInteger)
            {
                return DINT;
            }
            else
            {
                //Debug.Assert(false);
                throw new TypeCheckerException($"{left.Name} tag not expected in expression");
            }

            //return DINT;
        }

        #endregion

        public override ASTNode VisitStmtMod(ASTStmtMod context)
        {
            context.list = context.list.Accept(this) as ASTNodeList;
            return context;
        }

        public ASTStore GenStore(ASTName name, ASTExpr expr)
        {
            var tmp = new ASTTypeConv(expr, name.type);
            return new ASTStore(new ASTNameAddr(name), tmp) ;
        }

        public override ASTNode VisitAssignStmt(ASTAssignStmt context)
        {
            Debug.Assert(context.op == AssignType.NORMAL);
            //FIXME check type
            var name = context.name.Accept(this) as ASTName;
            //var addr = new ASTNameAddr(name);

            return GenStore(name, context.expr.Accept(this) as ASTExpr);
            /*
            var expr = new ASTTypeConv(context.expr.Accept(this) as ASTExpr, (addr.type as RefType).type);
            Debug.Assert(addr != null && expr != null);
            var store = new ASTStore(addr, expr);
            return store;
            */
        }

        public override ASTNode VisitNodeList(ASTNodeList context)
        {
            var list = new ASTNodeList();
            foreach (var node in context.nodes)
            {
                try
                {
                    list.AddNode(node.Accept(this));
                }
                catch (Exception e)
                {
                    throw new TypeCheckerException($"{e.Message}");
                }
            }

            return list;
        }
        
        public override ASTNode VisitPair(ASTPair context)
        {
            context.left = context.left.Accept(this);
            context.right = context.right.Accept(this);
            return context;
        }

        public override ASTNode VisitIf(ASTIf context)
        {
            context.cond = context.cond.Accept(this) as ASTExpr;
            context.then_list = context.then_list.Accept(this) as ASTNodeList;
            context.elsif_list = context.elsif_list.Accept(this) as ASTNodeList;
            context.else_list = context.else_list.Accept(this) as ASTNodeList;
            return context;
        }

        public override ASTNode VisitCase(ASTCase context)
        {
            context.expr = context.expr.Accept(this) as ASTExpr;
            context.elem_list = context.elem_list.Accept(this) as ASTNodeList;
            context.else_stmts = context.else_stmts.Accept(this) as ASTNodeList;
            return context;
        }

        public override ASTNode VisitExit(ASTExit context)
        {
            return context;
        }

        public override ASTNode VisitFor(ASTFor context)
        {
            //FIXME remove assign_stmt

            var store = context.assign_stmt.Accept(this) as ASTStore;
            //Debug.Assert(store != null);
            if (store == null) throw new TypeCheckerException();
            ///context.assign_stmt = null;
            context.init = store;
            context.expr = context.expr.Accept(this) as ASTExpr;
            context.optional = context.optional.Accept(this) as ASTExpr;
            context.stmt_list = context.stmt_list.Accept(this) as ASTNodeList;
            return context;
        }

        public override ASTNode VisitRepeat(ASTRepeat context)
        {
            context.stmts = context.stmts.Accept(this) as ASTNodeList;
            context.expr = context.expr.Accept(this) as ASTExpr;
            return context;
        }

        public override ASTNode VisitWhile(ASTWhile context)
        {
            context.expr = context.expr.Accept(this) as ASTExpr;
            context.stmts = context.stmts.Accept(this) as ASTNodeList;
            return context;
        }

        private void VerifyArrayList(ASTNameItem item, string name)
        {
            if (item.type_info.DataType == null || string.IsNullOrEmpty(name))
            {
                Debug.Assert(false);
                return;
            }
            var dim1 = item.type_info.Dim1;
            var dim2 = item.type_info.Dim2;
            var dim3 = item.type_info.Dim3;

            if ((dim1 == 0 && dim2 == 0 && dim3 == 0))
                throw new TypeCheckerException($"'{name}':Invalid array subscript specifier.");

            int len = item.arr_list.Count();
            if (len == 3)
            {
                if (item.type_info.Dim3 == 0)
                    throw new TypeCheckerException($"'{name}':Invalid array subscript specifier.");
                if (!VerifyArrayItem(item.arr_list.nodes[0], dim1) || !VerifyArrayItem(item.arr_list.nodes[1], dim2) || !VerifyArrayItem(item.arr_list.nodes[2], dim3))
                    throw new TypeCheckerException($"'{name}':Invalid array subscript specifier.");
            }
            else if (len == 2)
            {
                if (!(item.type_info.Dim3 == 0 && item.type_info.Dim2 != 0))
                    throw new TypeCheckerException($"'{name}':Invalid array subscript specifier.");
                if(!VerifyArrayItem(item.arr_list.nodes[0], dim1)|| !VerifyArrayItem(item.arr_list.nodes[1], dim2))
                    throw new TypeCheckerException($"'{name}':Invalid array subscript specifier.");
            }
            else if (len == 1)
            {
                if (!(item.type_info.Dim3 == 0 && item.type_info.Dim2 == 0 && item.type_info.Dim1 != 0))
                    throw new TypeCheckerException($"'{name}':Invalid array subscript specifier.");

                if (!VerifyArrayItem(item.arr_list.nodes[0], dim1))
                    throw new TypeCheckerException($"'{name}':Invalid array subscript specifier.");
            }

            if (!ArrCheck(item.arr_list))
                throw new TypeCheckerException($"'{name}':Invalid array subscript specifier.");
        }

        private bool VerifyArrayItem(ASTNode arrItem, int max)
        {
            var astInteger = arrItem as ASTInteger;
            if (astInteger != null)
            {
                if (astInteger.value < 0 || astInteger.value >= max) return false;
                return true;
            }

            var astFloat = arrItem as ASTFloat;
            if (astFloat != null)
            {
                return false;
            }

            var astName = arrItem as ASTName;
            if (astName != null)
            {
                return true;
            }

            var astInstr = arrItem as ASTInstr;
            if (astInstr != null)
            {
                //FIXME:check node
                return true;
            }

            var astUnaryOp = arrItem as ASTUnaryOp;
            if (astUnaryOp != null)
            {
                return false;
            }
            
            //Debug.Assert(false);
            return true;
        }

        private void VerifyBitOffset(IDataType type, int offset,string specific)
        {
            if (type == SINT)
            {
                if (offset > 7||offset<0) throw new TypeCheckerException($"'{specific}':Invalid bit specifier.");
            }
            else if (type == INT)
            {
                if (offset > 15 || offset < 0) throw new TypeCheckerException($"'{specific}':Invalid bit specifier.");
            }
            else if (type == DINT)
            {
                if (offset > 31 || offset < 0) throw new TypeCheckerException($"'{specific}':Invalid bit specifier.");
            }
            else if (type == LINT)
            {
                if (offset > 63 || offset < 0) throw new TypeCheckerException($"'{specific}':Invalid bit specifier.");
            }
            else
            {
                throw new TypeCheckerException();
            }
        }

        private IDataType GetBoolType(IDataType type)
        {
            if (type == SINT)
            {
                return PredefinedType.BOOL.SInst;
            }
            else if (type == INT)
            {
                return PredefinedType.BOOL.IInst;
            }
            else if (type == DINT)
            {
                return PredefinedType.BOOL.DInst;
            }
            else if (type == LINT)
            {
                return PredefinedType.BOOL.LInst;
            }
            else
            {
                throw new TypeCheckerException();
            }
        }

        private static ASTStatus GetASTStatus(string name)
        {
            if (name == "S:FS")
            {
                return (new ASTStatus(1));
            }

            if (name == "S:N")
            {
                return (new ASTStatus(2));
            }

            if (name == "S:Z")
            {
                return (new ASTStatus(3));
            }

            if (name == "S:V")
            {
                return new ASTStatus(4);

            }

            if (name == "S:C")
            {
                return new ASTStatus(5);

            }

            if (name == "S:MINOR")
            {
                return new ASTStatus(6);
            }

            throw new NotImplementedException();
        }

        private static int GetTagSize(ITag tag)
        {
            var info = tag.DataTypeInfo;

            var dim1 = info.Dim1;
            var dim2 = info.Dim2;
            var dim3 = info.Dim3;
            var type = tag.DataTypeInfo.DataType;

            var is_array = !(dim1 == 0 && dim2 == 0 && dim3 == 0);
            if (!is_array)
            {
                if (type is BOOL)
                {
                    var underlyingType = (type as BOOL).RefDataType;
                    if (underlyingType is SINT)
                        return 1;
                    if (underlyingType is INT)
                        return 2;
                    if (underlyingType is DINT)
                        return 4;
                    if (underlyingType is LINT)
                        return 8;
                    Debug.Assert(false, type.ToString());

                }
                else
                {
                    return type.ByteSize;
                }
            }

            if (type is BOOL)
            {
                Debug.Assert(dim2 == 0 && dim3 == 0, $"{dim1}:{dim2}:{dim3}");
                var underlyingType = (type as BOOL).RefDataType;
                Debug.Assert(dim1 % 32 == 0, dim1.ToString());
                return dim1 / 8;

            }

            return type.ByteSize * Math.Max(1, dim1) * Math.Max(1, dim2) * Math.Max(1, dim3);

        }

        public override ASTNode VisitName(ASTName context)
        {
            // if (context.bit_sel != null) throw new TypeCheckerException();
            if (!(context.id_list.nodes.Count >= 1)) throw new TypeCheckerException($"Reference tag  is undefined.");
            //if(context.loaders.Count() == 0)throw new TypeCheckerException();
            var item = context.id_list.nodes[0] as ASTNameItem;
            var tag_name = item.id;
            if (_transformTable != null)
            {
                var transformName = (string) _transformTable[tag_name.ToUpper()];
                if (!string.IsNullOrEmpty(transformName))
                {
                    var astName = ObtainValue.LoadName(transformName);
                    context.id_list.nodes.RemoveAt(0);
                    if (astName != null)
                    {
                        for (int i = astName.id_list.nodes.Count - 1; i >= 0; i--)
                        {
                            context.id_list.nodes.Insert(0, astName.id_list.nodes[i]);
                        }
                    }

                    if (item.arr_list != null)
                    {
                        var transformItem = astName?.id_list.nodes.Last() as ASTNameItem;
                        Debug.Assert(transformItem != null);
                        if (transformItem != null)
                        {
                            transformItem.arr_list = item.arr_list;
                        }
                    }

                    item = context.id_list.nodes[0] as ASTNameItem;
                    tag_name = item.id;
                }
            }
            
            IDataType type;
            int dim1 = 0;
            int dim2 = 0;
            int dim3 = 0;
            int start = 0;
            if (context.id_list.nodes.Count == 1 &&
                (tag_name.Equals("S:FS", StringComparison.OrdinalIgnoreCase) ||
                 tag_name.Equals("S:N", StringComparison.OrdinalIgnoreCase) ||
                 tag_name.Equals("S:Z", StringComparison.OrdinalIgnoreCase) ||
                 tag_name.Equals("S:V", StringComparison.OrdinalIgnoreCase) ||
                 tag_name.Equals("S:C", StringComparison.OrdinalIgnoreCase) ||
                 tag_name.Equals("S:MINOR", StringComparison.OrdinalIgnoreCase)))
            {
                tag_name = tag_name.ToUpper();
                context.type = PredefinedType.BOOL.DInst;
                context.bit_offset = -1;
                var status = GetASTStatus(tag_name);
                context.loaders.AddNode(status);
                context.loaders.AddNode(new ASTInteger(0));

                context.Expr = new ASTExprStatus(status.index);
                return context;
            }

            bool is_bool_array = false;
            ASTExpr expr = null;
            bool isAoiDataType = false;
            if (IsInAOI())
            {

                var index = this.context.FindParameterIndex(item.id);
                //这个是判断是否是不依赖于AOI的inout参数
                if (index != -1)
                {
                    item.FormatId = this.context.GetParameterName(index);
                    type = this.context.GetParameterType(index);
                    dim1 = 0;
                    context.loaders.AddNode(new ASTStackSlot(index + 1));
                    Debug.Assert(expr == null, "");
                    expr = new ASTExprParameter(type, this.context.GetParameterPos(index));
                    if (type is ArrayType)
                    {
                        var t = type as ArrayType;
                        context.base_dim1 = t.Dim1;
                        context.base_dim2 = t.Dim2;
                        context.base_dim3 = t.Dim3;
                    }

                    start = 1;
                    var aoiTag = this.context.Tags[tag_name];
                    if (aoiTag != null)
                    {
                        context.IsConstant = aoiTag.IsConstant;
                        item.type_info = aoiTag.DataTypeInfo;
                        /*
                        var node = context.id_list.nodes[0] as ASTNameItem;
                        Debug.Assert(node != null);
                        node.type_info = aoiTag.DataTypeInfo;
                        */
                    }

                    if (item.arr_list != null)
                    {
                        dim1 = 0;
                        dim2 = 0;
                        dim3 = 0;
                        var node = (ASTNodeList)item.arr_list.Accept(this);
                        VerifyArrayList(item, ObtainValue.GetAstName(context));
                        context.loaders.AddNode(new ASTArrayLoader(node, item.type_info));
                        var indexes = item.arr_list.nodes;
                        expr = new ASTExprArraySubscript(expr, indexes[0] as ASTExpr, indexes.Count >= 2 ? indexes[1] as ASTExpr : null, indexes.Count >= 3 ? indexes[2] as ASTExpr : null);
                        type = expr.type;

                        if (type is BOOL)
                        {
                            is_bool_array = true;
                        }
                    }

                    context.Tag = aoiTag;
                }
                else
                {
                    type = this.context.datatype;
                    dim1 = 0;
                    context.loaders.AddNode(new ASTStackSlot(0));
                    Debug.Assert(expr == null, "");
                    expr = new ASTExprParameter(type, 0);
                    context.Tag = this.context.Tags[tag_name];
                }

            }
            else
            {
                start = 1;
                var tag = program?.Tags[tag_name];
                bool flag = false;
                if (tag_name.IndexOf("\\") == 0)
                {
                    start = 2;
                    var otherProgram = controller.Programs[tag_name.Substring(1)];
                    if(otherProgram==null)
                        throw new TypeCheckerException($"Reference tag '{tag_name}' is undefined.");
                    if (context.id_list.nodes.Count == 1)
                        throw new TypeCheckerException($"Reference tag '{tag_name}' is undefined.");
                    //var tmp = context.id_list.nodes[1] as ASTNameItem;
                    item.FormatId = "\\" + otherProgram.Name;
                    item=context.id_list.nodes[1] as ASTNameItem;
                    var tagName = item.id;
                    tag_name = $"{tag_name}.{tagName}";
                    tag = otherProgram.Tags[tagName];
                    if(tag==null)
                        throw new TypeCheckerException($"Reference tag '{tagName}' is undefined.");
                    item.FormatId = tag.Name;
                    item.type_info = tag.DataTypeInfo;
                    flag = true;
                }

                if (tag == null)
                {
                    tag = controller.Tags[tag_name];
                }

                if (tag == null)
                {
                    throw new TypeCheckerException($"Reference tag '{tag_name}' is undefined.");
                }

                context.Tag = tag;
                isAoiDataType = tag.DataTypeInfo.DataType is AOIDataType;
                if (!flag)
                    item.FormatId = tag.Name;
                context.IsConstant = tag.IsConstant;
                item.type_info = tag.DataTypeInfo;

                type = tag.DataTypeInfo.DataType;

                context.size = GetTagSize(tag);

                context.loaders.AddNode(new ASTTag(tag_name, context.size));
                Debug.Assert(expr == null, "");
                //这里的type是不是要处理一下
                expr = new ASTExprTag(tag_name, context.size, ArrayType.InfoToType(item.type_info));

                context.base_dim1 = item.type_info.Dim1;
                context.base_dim2 = item.type_info.Dim2;
                context.base_dim3 = item.type_info.Dim3;
                if (item.arr_list == null)
                {
                    if (start < context.id_list.nodes.Count  && item.type_info.Dim1 > 0)
                        throw new TypeCheckerException(
                            $"'{ObtainValue.GetAstName(context)}':Invalid member specifier.");
                    dim1 = item.type_info.Dim1;
                    dim2 = item.type_info.Dim2;
                    dim3 = item.type_info.Dim3;
                    if (type is BOOL && dim1 > 0)
                        is_bool_array = true;
                }
                else
                {
                    dim1 = 0;
                    dim2 = 0;
                    dim3 = 0;
                    var node = (ASTNodeList) item.arr_list.Accept(this);
                    VerifyArrayList(item, ObtainValue.GetAstName(context));
                    context.loaders.AddNode(new ASTArrayLoader(node, item.type_info));
                    var indexes = item.arr_list.nodes;
                    context.ArrayNodeList = item.arr_list;
                    expr = new ASTExprArraySubscript(expr, indexes[0] as ASTExpr, indexes.Count >= 2 ? indexes[1] as ASTExpr : null, indexes.Count >= 3 ? indexes[2] as ASTExpr: null);
                    expr.Line = item.arr_list.Line;
                    if (type is BOOL)
                    {
                        is_bool_array = true;
                    }
                }
            }

            if (!(dim1 == 0 || context.id_list.nodes.Count == 1) && start == 1) throw new TypeCheckerException();
            int offset = 0;
            byte bit_offset = 0;

            for (int i = start; i < context.id_list.nodes.Count; ++i)
            {
                var tmp = context.id_list.nodes[i] as ASTNameItem;
                var member = (type as CompositiveType)[tmp.id];
                if (member == null || (member.IsHidden && this.context?.Name != type.Name))
                {
                    if (!(isAoiDataType && _transformTable != null))
                    {
                        if (i == start && member == null)
                        {
                            throw new TypeCheckerException($"'{tmp.id}':Referenced  tag is undefined.");
                        }

                        throw new TypeCheckerException(
                            $"'{ObtainValue.GetAstName(context)}':Invalid member specifier.");
                    }
                }

                tmp.type_info = member.DataTypeInfo;
                tmp.FormatId = member.Name;
                var member_offset = member.ByteOffset;
                offset += member_offset;
                context.loaders.AddNode(new ASTTagOffset(member_offset));

                bit_offset = (byte)member.BitOffset;
                type = tmp.type_info.DataType;

                context.base_dim1 = tmp.type_info.Dim1;
                context.base_dim2 = tmp.type_info.Dim2;
                context.base_dim3 = tmp.type_info.Dim3;
                expr = new ASTExprMember(expr, tmp.id);
                if (tmp.arr_list == null)
                {
                    if (i < context.id_list.nodes.Count - 1 && tmp.type_info.Dim1 > 0)
                        throw new TypeCheckerException(
                            $"'{ObtainValue.GetAstName(context)}':Invalid member specifier.");
                    dim1 = tmp.type_info.Dim1;
                    dim2 = tmp.type_info.Dim2;
                    dim3 = tmp.type_info.Dim3;
                    if (type is BOOL && dim1 > 0)
                        is_bool_array = true;
                }
                else
                {
                    dim1 = 0;
                    dim2 = 0;
                    dim3 = 0;
                    var node = (ASTNodeList) tmp.arr_list.Accept(this);
                    VerifyArrayList(tmp, ObtainValue.GetAstName(context));
                    context.loaders.AddNode(new ASTArrayLoader(node, tmp.type_info));

                    var indexes = tmp.arr_list.nodes;
                    context.ArrayNodeList = tmp.arr_list;
                    expr = new ASTExprArraySubscript(expr, indexes[0] as ASTExpr, indexes.Count >= 2 ? indexes[1] as ASTExpr : null, indexes.Count >= 3 ? indexes[2] as ASTExpr : null);
                    expr.Line = tmp.arr_list.Line;
                    if (type is BOOL)
                    {
                        is_bool_array = true;
                    }
                }
            }

            if (context.bit_sel != null)
            {
                var lastId = context.id_list.nodes[context.id_list.Count() - 1] as ASTNameItem;
                if (!(lastId.type_info.DataType.IsInteger &&
                      (lastId.type_info.Dim1 == 0 || (lastId.type_info.Dim1 > 0 && lastId.arr_list != null))))
                    throw new TypeCheckerException($"'{ObtainValue.GetAstName(context)}':Invalid member specifier.");
                context.type = BOOL;
                if (context.bit_sel is ASTInteger)
                {
                    VerifyBitOffset(type, (int)((ASTInteger)context.bit_sel).value,ObtainValue.GetAstName(context));
                }
                else
                {
                    context.bit_sel.Accept(this);
                }
                context.type = GetBoolType(type);
                context.bit_offset = -1; // (byte)context.bit_sel.value;
                context.loaders.AddNode(context.bit_sel);

                expr = new ASTExprBitSel(expr, context.bit_sel);
            }
            else
            {
                context.type = type;

                if (type is BOOL && is_bool_array)
                {
                    context.type = DBOOL;
                    context.bit_offset = -1;
                } 
                else if (type is BOOL && dim1 == 0)
                {
                    context.bit_offset = -1;
                    //expr = new ASTExprBitSel(expr, bit_offset);
                    context.loaders.AddNode(new ASTInteger(bit_offset));

                }
                else
                {
                    Debug.Assert(bit_offset == 0, bit_offset.ToString());
                    context.bit_offset = 0;
                }
            }

            context.dim1 = dim1;
            context.dim2 = dim2;
            context.dim3 = dim3;
            context.Expr = expr;
            if (context.type is USINT||context.type is UINT||context.type is UDINT ||context.type is ULINT||context.type is LREAL)
            {
                throw new TypeCheckerException("Not support unsigned data type");
            }
            return context;
        }

        public override ASTNode VisitFloat(ASTFloat context)
        {
            context.type = REAL;
            return context;
        }

        public override ASTNode VisitInteger(ASTInteger context)
        {

            return context;
        }

        public override ASTNode VisitUnaryOp(ASTUnaryOp context)
        {
            context.expr = context.expr.Accept(this) as ASTExpr;
            var op = context.op;
            //Debug.Assert(op != ASTUnaryOp.Op.NOP);
            if (op == ASTUnaryOp.Op.NOP) throw new TypeCheckerException();
            var type = context.expr.type;
            if (op == ASTUnaryOp.Op.NOT && IsNumber(type))
            {
                context.type = DINT;
            }
            else if (op == ASTUnaryOp.Op.NOT && type.IsBool)
            {
                context.type = BOOL;
            }
            //else if (op == ASTUnaryOp.Op.NEG && context.expr is ASTInteger)
            //{
            //    var expr = context.expr as ASTInteger;
            //    context.op = ASTUnaryOp.Op.PLUS;
            //    context.expr = new ASTInteger(-expr.value)
            //        {Context = expr.Context, ContextStart = expr.ContextStart - 1, ContextEnd = expr.ContextEnd};
            //    context.type = context.expr.type;
            //}
            else if ((op == ASTUnaryOp.Op.NEG || op == ASTUnaryOp.Op.PLUS) && type.IsInteger)
            {
                context.type = DINT;
            }
            else if ((op == ASTUnaryOp.Op.NEG || op == ASTUnaryOp.Op.PLUS) && type.IsReal)
            {
                context.type = type;
            }
            else
            {
                throw new TypeCheckerException($"'{ObtainValue.ConvertASTUnaryOp(op)}':not expected in expression.");
                //Debug.Assert(false);
            }

            return context;
        }

        public override ASTNode VisitBinRelOp(ASTBinRelOp context)
        {
            if (context.left is ASTBinRelOp || context.right is ASTBinRelOp)
            {
                throw new TypeCheckerException("Operator not expected in numerical expression");
            }
            var op = context.op;
            context.left = context.left.Accept(this) as ASTExpr;
            context.right = context.right.Accept(this) as ASTExpr;

            var left_type = context.left.type;
            var right_type = context.right.type;

            context.op_type = RelCommonType(left_type, right_type);
            context.type = BOOL;
            Debug.Assert(context.op_type is DINT || context.op_type is REAL, context.op_type.ToString());

            return context;
        }

        public override ASTNode VisitBinArithOp(ASTBinArithOp context)
        {
            var op = context.op;
            context.left = context.left.Accept(this) as ASTExpr;
            context.right = context.right.Accept(this) as ASTExpr;
            var left_type = context.left.type;
            var right_type = context.right.type;
            Debug.Assert(ASTBinOp.IsArithOp(op) && op != ASTBinOp.Op.NOP && left_type != null && right_type != null);
            context.type = ArithCommonType(op, left_type, right_type);
            return context;
        }

        public override ASTNode VisitBinLogicOp(ASTBinLogicOp context)
        {
            var op = context.op;
            context.left = context.left.Accept(this) as ASTExpr;
            context.right = context.right.Accept(this) as ASTExpr;
            var left_type = context.left.type;
            var right_type = context.right.type;
            Debug.Assert(ASTBinOp.IsLogicOp(op) && op != ASTBinOp.Op.NOP && left_type != null && right_type != null);
            context.type = LogicCommonType(left_type, right_type);
            return context;
        }

        // private void FindSTInstruction(ASTInstr)

        public override ASTNode VisitInstr(ASTInstr context)
        {
            var instr = (controller.AOIDefinitionCollection as AoiDefinitionCollection).Find(context.name)?.instr;
            if (instr == null)
            {
                instr = controller.STInstructionCollection.FindInstruction(context.name);
            }

            if (instr == null) throw new TypeCheckerException(context.name);
            context.instr = instr;
            context.param_list = context.instr.TypeCheck(this, instr.ParseSTParameters(context.param_list), Context.ST);

            return context;
        }

        public override ASTNode VisitCall(ASTCall context)
        {
            var instr = controller.STInstructionCollection.FindInstruction(context.name);
            //Debug.Assert(instr != null, context.name);
            if (instr == null) throw new TypeCheckerException();
            context.instr = instr;
            context.param_list = context.instr.TypeCheck(this, instr.ParseSTParameters(context.param_list), Context.ST);
            context.type = instr.Type(context.param_list);

            return context;
        }

        public override ASTNode VisitRLLRoutine(ASTRLLRoutine context)
        {
            context.list = context.list.Accept(this) as ASTNodeList;
            return context;
        }

        public override ASTNode VisitRLLParallel(ASTRLLParallel context)
        {
            context.list = context.list.Accept(this) as ASTNodeList;
            return context;
        }

        public override ASTNode VisitRLLSequence(ASTRLLSequence context)
        {
            context.list = context.list.Accept(this) as ASTNodeList;
            return context;
        }

        public override ASTNode VisitRLLInstruction(ASTRLLInstruction context)
        {
            var paramListAst = context.instr.ParseRLLParameters(context.param_list);
            context.param_list_ast =
                context.instr.TypeCheck(this, paramListAst, Context.RLL);

            return context;
        }

        public override ASTNode VisitFBDIRef(ASTFBDIRef context)
        {
            context.operand = context.operand.Accept(this) as ASTExpr;
            context.type = context.operand.type;
            context.slot.type = context.operand.type;
            return context;
        }

        public override ASTNode VisitFBDInstruction(ASTFBDInstruction context)
        {
            context.parameters = context.parameters.Accept(this) as ASTNodeList;

            IXInstruction instr = null;

            if (context.isAOI)
            {
                instr = (Controller.GetInstance().AOIDefinitionCollection as AoiDefinitionCollection).Find(context.name)
                    .instr;
            }
            else
            {
                instr = controller.FBDInstructionCollection.FindInstruction(context.name);
            }

            if (instr == null) throw new TypeCheckerException($"{context.isAOI}:{context.name}");

            context.instr = instr;
            //Console.WriteLine($"{context.arguments}");
            context.arguments = context.instr.TypeCheck(this, context.arguments, Context.FBD);

            return context;
        }

        public override ASTNode VisitFBDRoutine(ASTFBDRoutine context)
        {
            context.list = context.list.Accept(this) as ASTNodeList;
            return context;
        }

        public override ASTNode VisitTempValue(ASTTempValue context)
        {
            Debug.Assert(context.slot.type != null);
            context.type = context.slot.type;
            return context;
        }

        public override ASTNode VisitNameAddr(ASTNameAddr context)
        {
            context.name.Accept(this);
            return context;
        }

        public override ASTNode VisitError(ASTError context)
        {
            //return context;
            throw new TypeCheckerException(context.Error);
        }

        public override ASTNode VisitEmpty(ASTEmpty context)
        {
            return context;
        }

        private bool ArrCheck(ASTNodeList astNodeList)
        {
            foreach (var node in astNodeList.nodes)
            {
                var astName = node as ASTName;
                if (astName != null)
                {
                    if (!(astName.type.IsInteger && astName.dim1 == 0)) return false;
                    continue;
                }

                if (node is ASTInteger) continue;

                //Debug.Assert(false, node.GetType().Name);
            }

            return true;
        }
    }
}
