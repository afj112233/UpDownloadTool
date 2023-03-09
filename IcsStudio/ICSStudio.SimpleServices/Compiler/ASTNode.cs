using System;
using System.Collections.Generic;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Instruction;
using System.Numerics;
using Type = System.Type;
using AstMarker = System.Tuple<string, int, int, System.Windows.Media.Color>;

namespace ICSStudio.SimpleServices.Compiler
{
    public class RefType : ICSStudio.SimpleServices.DataType.DataType
    {
        public readonly IDataType type;
        public int bit_offset;

        public RefType(IDataType type, int bit_offset)
        {
            this.type = type;
            this.bit_offset = bit_offset;
        }

        public override string ToString()
        {
            return $"ref:{this.type},{this.bit_offset}";
        }
    }

    public class ArrayTypeNormal : ICSStudio.SimpleServices.DataType.DataType
    {
        public readonly IDataType Type;

        public ArrayTypeNormal(IDataType type)
        {
            this.Type = type;

        }
    }

    public class ArrayTypeDimOne : ICSStudio.SimpleServices.DataType.DataType
    {
        public readonly IDataType type;
        public bool AllowNull { get; }

        public ArrayTypeDimOne(IDataType type, bool allowNull = false)
        {
            this.type = type;
            AllowNull = allowNull;
        }
    }

    public class ExceptType : ICSStudio.SimpleServices.DataType.DataType
    {
        public readonly IDataType[] ExceptTypes;

        public ExceptType(params IDataType[] exceptTypes)
        {
            ExceptTypes = exceptTypes;
        }

        public bool IsMatched(IDataType dateType)
        {
            foreach (var exceptType in ExceptTypes)
            {
                if (dateType.Equal(exceptType,true)) return false;
            }

            return true;
        }

        public override string ToString()
        {
            var info = "";
            foreach (var exceptType in ExceptTypes)
            {
                info += $",{exceptType.Name}";
            }

            return $"ANY_DATA_TYPE except {info.Substring(1)}";
        }
    }

    public class ZeroType : ICSStudio.SimpleServices.DataType.DataType
    {
        
    }

    public class ExpectType : ICSStudio.SimpleServices.DataType.DataType
    {
        public readonly IDataType[] ExpectTypes;

        public ExpectType(params IDataType[] expectTypes)
        {
            ExpectTypes = expectTypes;
        }

        public bool IsMatched(IDataType dateType)
        {
            foreach (var expectType in ExpectTypes)
            {
                if (dateType.Equal(expectType,true)) return true;
            }

            return false;
        }

        public override string ToString()
        {
            var info = "";
            foreach (var expectType in ExpectTypes)
            {
                info += $",{expectType.Name}";
            }

            return info.Substring(1);
        }
    }

    public abstract class ASTNode
    {
        private string _error;
        public abstract T Accept<T>(IASTVisitor<T> visitor);

        public static ASTLIRIf CreateLIRIf(ASTExpr expr, ASTNode left, ASTNode right)
        {
            return new ASTLIRIf(expr, left, right);
        }

        public string Error
        {
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (value.Contains("<EOF>"))
                {
                    _error = "Statement not terminated by ';'.";
                    return;
                }
                _error = value;
            }
            get { return _error; }
        }

        public virtual int ContextStart { set; get; }

        public virtual int ContextEnd { set; get; }

        public int Line { set; get; }

        public int Column { set; get; }
        
        public int ErrorStart { set; get; }

        public int ErrorEnd { set; get; }

        public bool IsMarked { set; get; }

        public ASTNode Parent { set; get; }

        public ASTNode Children { set; get; }
    }

    public class ASTError : ASTExpr
    {
        public ASTError(object errorObject)
        {
            ErrorObject = errorObject;
        }
        public bool IsEnum { set; get; }

        public Type EnumType { set; get; }
        public object ErrorObject { get; }
        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            throw new Exception(Error);
            //return visitor.VisitError(this);
        }
    }

    public class ASTUnexpectedError : ASTError
    {
        public ASTUnexpectedError(object errorObject,ASTNode item,ASTNode expr) : base(errorObject)
        {
            Item = item;
            Expr = expr;
        }

        public ASTNode Item { get; }

        public ASTNode Expr { get; }
    }

    public class ASTErrorStmt : ASTError
    {
        public ASTErrorStmt(ASTName item,ASTExpr number,object errorObject):base(errorObject)
        {
            this.item = item;
            this.number = number;
        }

        public ASTName item;

        public ASTExpr number;
    }

    public class ASTEmpty : ASTNode
    {
        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitEmpty(this);
        }
    }

    public class ASTNodeList : ASTNode
    {
        public ASTNodeList OriginalNodeList { internal set; get; }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            OriginalNodeList = this;
            return visitor.VisitNodeList(this);
        }

        public void AddNode(ASTNode node)
        {
            if(node==null)return;
            nodes.Add(node);
        }

        public void AddNodes(ASTNodeList nodes)
        {
            this.nodes.AddRange(nodes.nodes);
        }

        public void InsertNode(ASTNode node)
        {
            nodes.Insert(0, node);
        }

        public int Count()
        {
            return nodes.Count;
        }

        public override string ToString()
        {
            return string.Join(".", this.nodes);
        }

        public ASTNodeList Reverse()
        {
            var res = new ASTNodeList();
            for (int i = 0; i < nodes.Count; ++i)
            {
                res.AddNode(nodes[nodes.Count - i - 1]);
            }

            res.Line = Line;
            return res;
        }

        public List<ASTNode> nodes { get; } = new List<ASTNode>();
    }

    public abstract class ASTStmt : ASTNode
    {

    }

    public class ASTStmtMod : ASTStmt
    {
        public ASTStmtMod(ASTNodeList li)
        {
            Debug.Assert(li != null);
            list = li;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitStmtMod(this);
        }

        public ASTNodeList list;

        /// <summary>
        /// 用于储存通过语法解析获得的Region标签信息
        /// </summary>
        public ASTNodeList RegionLableNodeList { get; set; }

        public List<AstMarker> TextMarkers { get; }=new List<AstMarker>();
    }

    public enum AssignType
    {
        NORMAL,
        NONRETENTIVE
    }

    public class ASTAssignStmt : ASTStmt
    {
        public ASTAssignStmt(ASTNode name, ASTNode expr, AssignType op = AssignType.NORMAL)
        {
            Debug.Assert(name != null && expr != null && op == AssignType.NORMAL);
            this.op = op;
            this.name = name;
            this.expr = expr;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitAssignStmt(this);
        }

        public ASTNode name;
        public ASTNode expr;
        //public ASTName name;
        //public ASTExpr expr;
        public AssignType op { get; }
    }

    public class ASTInstr : ASTStmt
    {
        public ASTInstr(string name, ASTNodeList param_list)
        {
            this.name = name;
            this.param_list = param_list;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitInstr(this);
        }

        public string name { get; set; }
        public ASTNodeList param_list { get; set; }
        public IXInstruction instr { get; set; }
    }

    public class ASTCall : ASTExpr
    {
        public ASTCall(string name, ASTNodeList param_list)
        {
            this.name = name;
            this.param_list = param_list;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitCall(this);
        }

        public string name { get; }
        public ASTNodeList param_list { get; set; }
        public IXInstruction instr { get; set; }
    }

    public class ASTPrescan : ASTNode
    {
        public ASTPrescan(ASTNodeList paramList, IXInstruction instr)
        {
            this.ParamList = paramList;
            this.Instr = instr;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitPrescan(this);
        }

        public ASTNodeList ParamList { get; }
        public IXInstruction Instr { get; }
    }

    public class ASTPair : ASTNode
    {
        public ASTPair(ASTNode l, ASTNode r)
        {
            left = l;
            right = r;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitPair(this);
        }

        public ASTNode left { get; set; }
        public ASTNode right { get; set; }

        public bool IsNeedBoolExpr { get; set; }
    }

    public class ASTIf : ASTStmt
    {
        public ASTIf(ASTNode expr, ASTNode stmt_list, ASTNode elsif_stmt_list, ASTNode if_else_stmts)
        {
            cond = expr;
            then_list = stmt_list;
            elsif_list = elsif_stmt_list;
            else_list = if_else_stmts;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitIf(this);
        }

        //public ASTExpr cond { get; set; }
        //public ASTNodeList then_list { get; set; }
        //public ASTNodeList elsif_list { get; set; }
        //public ASTNodeList else_list { get; set; }
        public ASTNode cond { get; set; }
        public ASTNode then_list { get; set; }
        public ASTNode elsif_list { get; set; }
        public ASTNode else_list { get; set; }
    }

    public class ASTRet : ASTNode
    {
        public ASTRet()
        {

        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitRet(this);
        }
    }

    public class ASTLIRIf : ASTStmt
    {
        public readonly ASTExpr expr;
        public readonly ASTNode if_stmts;
        public readonly ASTNode else_stmts;

        public ASTLIRIf(ASTExpr expr, ASTNode if_stmts, ASTNode else_stmts)
        {
            this.expr = expr;
            this.if_stmts = if_stmts;
            this.else_stmts = else_stmts;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitLIRIf(this);
        }
    }

    public class ASTCase : ASTStmt
    {
        public ASTCase(ASTNode ex, ASTNodeList case_elem_list, ASTNodeList case_else_stmts)
        {
            expr = ex;
            elem_list = case_elem_list;
            else_stmts = case_else_stmts;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitCase(this);
        }

        public ASTNode expr { get; set; }
        //public ASTExpr expr { get; set; }
        public ASTNodeList elem_list { get; set; }
        public ASTNodeList else_stmts { get; set; }
    }

    public class ASTExit : ASTStmt
    {
        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitExit(this);
        }
    }

    public class ASTFor : ASTStmt
    {
        public ASTFor(ASTNode ass_stmt, ASTNode ex, ASTNode opt, ASTNodeList st_list)
        {
            assign_stmt = ass_stmt;
            expr = ex;
            optional = opt;
            stmt_list = st_list;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitFor(this);
        }

        //public ASTAssignStmt assign_stmt { get; set; }
        public ASTNode assign_stmt { get; set; }
        public ASTName name { get; set; }
        public ASTStore init { get; set; }

        public ASTNode expr { get; set; }
        //public ASTExpr expr { get; set; }
        //public ASTExpr optional { get; set; }
        public ASTNode optional { get; set; }
        public ASTNodeList stmt_list { get; set; }
    }

    public class ASTRepeat : ASTStmt
    {
        public ASTRepeat(ASTNodeList sts, ASTNode ex)
        {
            stmts = sts;
            expr = ex;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitRepeat(this);
        }

        public ASTNodeList stmts { get; set; }
        public ASTNode expr { get; set; }
        //public ASTExpr expr { get; set; }
    }

    public class ASTWhile : ASTStmt
    {
        public ASTWhile(ASTNode ex, ASTNodeList sts)
        {
            expr = ex;
            stmts = sts;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitWhile(this);
        }

        //public ASTExpr expr { get; set; }
        public ASTNode expr { get; set; }
        public ASTNodeList stmts { get; set; }
    }

    public abstract class ASTExpr : ASTNode
    {
        public IDataType type { get; set; }
        public bool IsLValue { get; set; } = false;

        protected ASTExpr(IDataType type = null, bool isLValue = false)
        {
            this.type = type;
            this.IsLValue = isLValue;
        }
    }

    public class ASTNameItem : ASTNode
    {
        public ASTNameItem(string id, ASTNodeList arr_list)
        {
            this.id = id;
            this.arr_list = arr_list?.Reverse();
        }

        public ASTNameItem(string id, DataTypeInfo type_info)
        {
            this.id = id;
            this.type_info = type_info;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            Debug.Assert(false);
            return default(T);
        }

        public string id { get; }
        public ASTNodeList arr_list { set; get; }
        public DataTypeInfo type_info { get; set; }
        public string FormatId { internal set; get; }

        public override string ToString()
        {
            return $"{this.id}:{this.arr_list}";
        }
    }

    public class ASTExprTag : ASTExpr
    {
        public readonly string Name;
        public readonly int Size;

        public ASTExprTag(string name, int size, IDataType type) : base(type, true)
        {
            this.Name = name;
            this.Size = size;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitExprTag(this);
        }
    }

    public class ASTExprArraySubscript : ASTExpr
    {
        public readonly ASTExpr Index0;
        public readonly ASTExpr Index1;
        public readonly ASTExpr Index2;
        public readonly ASTExpr Expr;


        public ASTExprArraySubscript(ASTExpr expr, ASTExpr index0, ASTExpr index1=null, ASTExpr index2=null) :base(GetType(expr), expr.IsLValue)
        {
            Index0 = index0;
            Index1 = index1;
            Index2 = index2;
            Expr = expr;
        }

        private static IDataType GetType(ASTExpr expr)
        {
            var type = expr.type as ArrayType;
            Debug.Assert(type != null);
            return type.Type;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitExprArraySubscript(this);
        }
    }

    public class ASTExprMember : ASTExpr
    {
        public readonly string Name;
        public readonly ASTExpr Expr;
        public readonly int Offset;
        public readonly int BitOffset;

        public ASTExprMember(ASTExpr expr, string name) : base(GetType(expr.type as CompositiveType, name), expr.IsLValue)
        {
            this.Name = name;
            this.Expr = expr;
            this.IsLValue = true;
            this.Offset = GetOffset(expr.type as CompositiveType, name);
            this.BitOffset = GetBitOffset(expr.type as CompositiveType, name);
        }

        private static IDataType GetType(CompositiveType type, string name)
        {
            Debug.Assert(type != null);
            var info = type[name];
            return ArrayType.InfoToType(info.DataTypeInfo);
        }

        private static int GetOffset(CompositiveType type, string name)
        {
            Debug.Assert(type != null);
            var info = type[name];
            return info.ByteOffset;
        }
        
        private static int GetBitOffset(CompositiveType type, string name)
        {
            Debug.Assert(type != null);
            var info = type[name];
            return info.BitOffset;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitExprMember(this);
        }
    }

    public class ASTExprConstString : ASTExpr
    {
        public readonly string Value;
        public ASTExprConstString(string value) : base(ConstStringType.Inst, false)
        {
            this.Value = value;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitExprConstString(this);
        }
    }

    public class ASTExprStatus : ASTExpr
    {
        public readonly int Index;

        public ASTExprStatus(int index)
        {
            this.Index = index;
            this.type = PredefinedType.BOOL.DInst;
            this.IsLValue = true;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitExprStatus(this);
        }
    }

    public class ASTExprParameter : ASTExpr
    {
        public readonly int SlotNo;

        public ASTExprParameter(IDataType type, int no) : base(type, false)
        {
            this.SlotNo = no;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitExprParameter(this);
        }
    }

    public class ASTExprBitSel : ASTExpr
    {
        public readonly ASTExpr Expr;
        public readonly ASTExpr Index;

        public ASTExprBitSel(ASTExpr expr, ASTExpr index) : base(BOOL.Inst, false)
        {
            Expr = expr;
            Index = index;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitExprBitSel(this);
        }

    }

    public class ASTTag : ASTNode
    {
        public readonly string name;
        public readonly int size;

        public ASTTag(string name, int size)
        {
            this.name = name;
            this.size = size;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitTag(this);
        }
    }

    public class ASTStackSlot : ASTNode
    {
        public readonly int no = -1;

        public ASTStackSlot(int no)
        {
            Debug.Assert(no >= 0 && no <= ushort.MaxValue, no.ToString());
            this.no = no;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitStackSlot(this);
        }
    }

    public class ASTTagOffset : ASTNode
    {
        public readonly int offset;

        public ASTTagOffset(int offset)
        {
            this.offset = offset;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitTagOffset(this);
        }
    }

    public class ASTArrayLoader : ASTNode
    {
        public ASTNodeList dims { get; }
        public DataTypeInfo info { get; }

        public ASTArrayLoader(ASTNodeList dims, DataTypeInfo info)
        {
            this.dims = dims;
            this.info = info;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitArrayLoader(this);
        }
    }

    public class ASTBitSelLoader : ASTNode
    {
        private int offset;

        public ASTBitSelLoader(IDataType tp, int offset)
        {
            this.offset = offset;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitBitSelLoader(this);
        }
    }

    public class ASTName : ASTExpr
    {

        public ASTName(ASTNodeList list, ASTExpr sel)
        {
            id_list = list;
            bit_sel = sel;
        }

        public ASTName(string item1)
        {
            id_list = new ASTNodeList();
            id_list.AddNode(new ASTNameItem(item1, null));
            bit_sel = null;
        }

        public ASTName(string item1, string item2)
        {
            id_list = new ASTNodeList();
            id_list.AddNode(new ASTNameItem(item1, null));
            id_list.AddNode(new ASTNameItem(item2, null));
            bit_sel = null;
        }

        //already type checked
        public ASTName(ASTNameItem b, ASTNameItem expr)
        {
            this.loaders.AddNode(new ASTTag(b.id, b.type_info.DataType.ByteSize));
            var type = b.type_info.DataType as CompositiveType;
            Debug.Assert(type != null, b.id);
            this.loaders.AddNode(new ASTTagOffset(type[expr.id].ByteOffset));

            this.id_list = new ASTNodeList();
            this.id_list.AddNode(b);
            this.id_list.AddNode(expr);
            this.type = expr.type_info.DataType;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitName(this);
        }

        public override string ToString()
        {
            if (bit_sel == null)
            {
                return $"ASTName:{id_list}";
            }
            else
            {
                return $"ASTName:{id_list}:{bit_sel}";
            }
        }

        public ASTNodeList id_list { get; }
        public ASTExpr bit_sel { get; } = null;

        public ASTNodeList loaders { get; } = new ASTNodeList();

        public int size { get; set; } = 0;
        public int bit_offset { get; set; } = 0;
        public int dim1 { get; set; } = 0;
        public int dim2 { get; set; } = 0;
        public int dim3 { get; set; } = 0;

        public int base_dim1 { get; set; } = 0;
        public int base_dim2 { get; set; } = 0;
        public int base_dim3 { get; set; } = 0;
        public bool IsConstant { set; get; }
        public IDataType ExpectDataType { set; get; } = DINT.Inst;

        public ASTExpr Expr { get; set; }
        public bool IsEnum { set; get; }

        public ASTNodeList ArrayNodeList { set; get; }

        public ITag Tag { internal set; get; }
        public IField Field { internal set; get; }
        public int FieldIndex { internal set; get; } = -1;
    }



    public class ASTStore : ASTNode
    {
        public readonly ASTNameAddr addr;
        public readonly ASTExpr value;

        public ASTStore(ASTNameAddr addr, ASTExpr value)
        {
            Debug.Assert(addr.type is RefType, addr.ToString());
            this.addr = addr;
            this.value = value;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitStore(this);
        }
    }

    //name must be atomic
    public class ASTNameValue : ASTExpr
    {
        public readonly ASTName name;

        public ASTNameValue(ASTName name)
        {
            this.name = name;
            this.type = name.type;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitNameValue(this);
        }
    }

    public class ASTNullAddr : ASTExpr
    {
        public ASTNullAddr(IDataType type)
        {
            this.type = new RefType(type, 0);
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitNullAddr(this);
        }
    }

    //name must be address
    public class ASTNameAddr : ASTExpr
    {
        public readonly ASTName name;
        public readonly RefType ref_type;

        public ASTNameAddr(ASTName name)
        {
            Debug.Assert(name != null);
            this.name = name;
            ContextStart = name.ContextStart;
            ContextEnd = name.ContextEnd;
            this.ref_type = new RefType(name.type, name.bit_offset);
            this.type = this.ref_type;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitNameAddr(this);
        }

        public override string ToString()
        {
            return $"ASTNameAddr:{this.name}";
        }
    }

    public class TempSlot
    {
        public IDataType type { get; set; }
        public int slot_no = -1;

        public TempSlot(IDataType type = null)
        {
            this.type = type;
            this.slot_no = -1;
        }
    }

    public class ASTTempValue : ASTExpr
    {
        public TempSlot slot;

        public ASTTempValue(TempSlot slot)
        {
            this.type = slot.type;
            this.slot = slot;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitTempValue(this);
        }
    }

    /*
    public class ASTTempAddr : ASTExpr
    {
        public TempSlot slot;
        public ASTTempAddr(TempSlot slot)
        {
            this.type = new RefType(slot.type, 0);
            this.slot = slot;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitTempAddr(this);
        }
    }
    */

    public class ASTFloat : ASTExpr
    {
        private string _context;

        public ASTFloat(double v)
        {
            value = v;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitFloat(this);
        }

        public double value { get; }

        public string Context
        {
            set { _context = value; }
            get
            {
                var context = _context ?? value.ToString("e7");
                return context;
            }
        }
    }

    public class ASTInteger : ASTExpr
    {
        private string _context;

        public ASTInteger(BigInteger value)
        {
            if (value >= Int32.MinValue && value <= Int32.MaxValue)
            {
                type = DINT.Inst;
            }
            else if (value >= Int64.MinValue && value <= Int64.MaxValue)
            {
                type = LINT.Inst;
            }
            else
            {
                Debug.Assert(false, value.ToString());
            }

            this.value = value;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitInteger(this);
        }

        public bool IsEnum { set; get; }

        public Type EnumType { set; get; }

        public BigInteger value { get; }

        public string Context
        {
            set { _context = value; }
            get
            {
                var context = _context ?? value.ToString();
                return context;
            }
        }
    }

    public class ASTUnaryOp : ASTExpr
    {
        public enum Op
        {
            NOP,
            NOT,
            NEG,
            PLUS,
        }

        public ASTUnaryOp(Op o, ASTExpr ex)
        {
            op = o;
            expr = ex;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitUnaryOp(this);
        }

        public Op op { get; set; }
        public ASTExpr expr { get; set; }
    }

    public abstract class ASTBinOp : ASTExpr
    {
        public enum Op
        {
            NOP,
            OR,
            XOR,
            AND,
            EQ,
            NEQ,
            LE,
            LT,
            GE,
            GT,
            PLUS,
            MINUS,
            TIMES,
            DIVIDE,
            MOD,
            POW,
        }

        protected ASTBinOp(Op o, ASTExpr l, ASTExpr r)
        {
            op = o;
            left = l;
            right = r;
        }

        static public bool IsArithOp(ASTBinOp.Op op)
        {
            return op == ASTBinOp.Op.PLUS ||
                   op == ASTBinOp.Op.MINUS ||
                   op == ASTBinOp.Op.TIMES ||
                   op == ASTBinOp.Op.DIVIDE ||
                   op == ASTBinOp.Op.MOD ||
                   op == ASTBinOp.Op.POW;
        }

        static public bool IsLogicOp(ASTBinOp.Op op)
        {
            return op == ASTBinOp.Op.OR ||
                   op == ASTBinOp.Op.XOR ||
                   op == ASTBinOp.Op.AND;
        }

        static public bool IsRelOp(ASTBinOp.Op op)
        {
            return op == ASTBinOp.Op.EQ ||
                   op == ASTBinOp.Op.NEQ ||
                   op == ASTBinOp.Op.GE ||
                   op == ASTBinOp.Op.GT ||
                   op == ASTBinOp.Op.GE ||
                   op == ASTBinOp.Op.GT;
        }

        public Op op { get; }
        public ASTExpr left { get; set; }
        public ASTExpr right { get; set; }

    }

    public class ASTBinRelOp : ASTBinOp
    {

        public IDataType op_type { get; set; }

        public ASTBinRelOp(Op op, ASTExpr left, ASTExpr right) : base(op, left, right)
        {
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitBinRelOp(this);
        }
    }

    public class ASTBinLogicOp : ASTBinOp
    {

        public IDataType op_type { get; set; }

        public ASTBinLogicOp(Op op, ASTExpr left, ASTExpr right) : base(op, left, right)
        {
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitBinLogicOp(this);
        }
    }

    public class ASTBinArithOp : ASTBinOp
    {

        public IDataType op_type { get; set; }

        public ASTBinArithOp(Op op, ASTExpr left, ASTExpr right) : base(op, left, right)
        {
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitBinArithOp(this);
        }
    }

    public class ASTRTCall : ASTExpr
    {
        public string name { get; set; }
        public ASTNodeList list { get; }

        public ASTRTCall(string name, ASTNodeList list)
        {
            this.name = name;
            this.list = list;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitRTCall(this);
        }
    }

    public class ASTNop : ASTExpr
    {
        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitNop(this);
        }
    }

    public class ASTTypeConv : ASTExpr
    {
        public readonly ASTExpr expr;

        public ASTTypeConv(ASTExpr expr, IDataType type)
        {
            this.expr = expr;
            this.type = type;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitTypeConv(this);
        }

        public override string ToString()
        {
            return $"ASTTypeConv:{this.type}:{this.expr}";
        }

        public override int ContextEnd => expr?.ContextEnd??0;

        public override int ContextStart => expr?.ContextStart ?? 0;
    }

    public class ASTDup : ASTExpr
    {
        public ASTDup(IDataType type)
        {
            this.type = type;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitDup(this);
        }
    }

    public class ASTPop : ASTNode
    {
        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitPop(this);
        }
    }

    public class ASTRLLRoutine : ASTNode
    {
        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitRLLRoutine(this);
        }

        public void Add(ASTRLLSequence rung)
        {
            list.AddNode(rung);
        }

        public ASTNodeList list { get; set; } = new ASTNodeList();
    }

    public class ASTRLLInstruction : ASTNode
    {
        public ASTRLLInstruction(string name, IXInstruction instr, List<string> param_list)
        {
            this.name = name;
            this.instr = instr;
            this.param_list = param_list;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitRLLInstruction(this);
        }

        public List<string> param_list { get; set; }
        public ASTNodeList param_list_ast { get; set; }

        public readonly IXInstruction instr;
        public readonly string name;
    }

    public class ASTRLLParallel : ASTNode
    {
        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitRLLParallel(this);
        }

        public void Add(ASTNode node)
        {
            list.AddNode(node);
        }

        public ASTNodeList list { get; set; } = new ASTNodeList();
    }

    public class ASTRLLSequence : ASTNode
    {
        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitRLLSequence(this);
        }

        public void Add(ASTNode node)
        {
            list.AddNode(node);
        }

        public ASTNodeList list { get; set; } = new ASTNodeList();
    }

    public class ASTFBDInstruction : ASTNode
    {
        public ASTNodeList parameters;
        public ASTNodeList arguments;
        public IXInstruction instr { get; set; }
        public string name { get; set; }
        public readonly bool isAOI = true;

        public ASTFBDInstruction(bool isAOI, string name, ASTNodeList parameters, ASTNodeList arguments)
        {
            this.isAOI = isAOI;
            this.name = name;
            this.arguments = arguments;
            this.parameters = parameters;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitFBDInstruction(this);
        }
    }

    public class ASTFBDIRef : ASTExpr
    {
        public ASTExpr operand { get; set; }
        public TempSlot slot { get; set; }

        public ASTFBDIRef(ASTExpr operand)
        {
            this.operand = operand;
            this.slot = new TempSlot();
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitFBDIRef(this);
        }
    }

    public class ASTFBDORef : ASTNode
    {
        public ASTName dest { get; }
        public ASTName src { get; }

        public ASTFBDORef(ASTName dest, ASTName src)
        {
            this.dest = dest;
            this.src = src;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitFBDORef(this);
        }
    }

    public class ASTFBDRoutine : ASTNode
    {
        public ASTNodeList list { get; set; }

        public ASTFBDRoutine(ASTNodeList list)
        {
            this.list = list;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitFBDRoutine(this);
        }
    }

    public class ASTTask : ASTExpr
    {
        public ASTTask(string name)
        {
            this.Name = name;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitTask(this);
        }

        public string Name { get; }
    }

    public class ASTString : ASTNode
    {
        public ASTString(string name)
        {
            this.Name = name;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }

        public string Name { get; }
    }

    public class ASTStatus : ASTNode
    {
        public ASTStatus(int index)
        {
            this.index = index;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            return visitor.VisitStatus(this);
        }

        public int index { get; }
    }

    public enum RegionLableType
    {
        None,
        Region,
        Endregion
    }

    public class ASTRegionLable : ASTNode
    {
        public RegionLableType LableType { get; }

        /// <summary>
        /// region标签后跟随的描述文字
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// 关键词长度
        /// </summary>
        public int KeywordLength
        {
            get
            {
                switch (LableType)
                {
                    case RegionLableType.Region:
                    case RegionLableType.Endregion:
                        return LableType.ToString().Length + 1;
                    default:
                        return 0;
                }
            }
        }

        public ASTRegionLable(RegionLableType type, string text)
        {
            LableType = type;
            Text = text;
        }

        public override T Accept<T>(IASTVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }
    }
}
