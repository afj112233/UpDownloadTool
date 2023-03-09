using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Media.Converters;
using System.Xml.Schema;
using ICSStudio.DeviceProfiles;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Xunit;
using Xunit.Sdk;

namespace ICSStudio.SimpleServices.Compiler
{
    public class CodeGenVisitor : IASTBaseVisitor
    {
        public ConstPool consts_pool;
        private bool is_catch_exception = true;
        private UInt16 slot_counter = 0;
        private MacroAssembler _masm;
        private List<MacroAssembler.Label> _loop_labels = new List<MacroAssembler.Label>();

        private Stack<Tuple<MacroAssembler.Label, int, int>> _safe_points =
            new Stack<Tuple<MacroAssembler.Label, int, int>>();

        public List<Tuple<int, int, int>> _real_safe_points = new List<Tuple<int, int, int>>();

        //for JMP LBL
        private Dictionary<string, MacroAssembler.Label> _labels = new Dictionary<string, MacroAssembler.Label>();

        public CodeGenVisitor(ConstPool consts_pool, int args_size = 0, bool is_catch_exception = true)
        {
            this.is_catch_exception = is_catch_exception;
            this.slot_counter = (UInt16) args_size;
            this.consts_pool = consts_pool;
            _masm = new MacroAssembler(consts_pool);
        }

        public void VisitParamList(ASTNodeList paramList, int start = 0)
        {
            /*
            for (int i = paramList.Count() - 1; i >= 0; --i)
            {
                paramList.nodes[i].Accept(this);
            }
            */
            Debug.Assert(start <= paramList.Count(), $"{start}:{paramList.Count()}");
            for (int i = start; i < paramList.Count(); ++i)
            {
                paramList.nodes[i].Accept(this);
            }
        }

        void BeginBlock()
        {
            var label = new MacroAssembler.Label();
            _safe_points.Push(Tuple.Create(label, _masm.stack_size, _masm.PC));
        }

        void EndBlock()
        {
            Debug.Assert(_safe_points.Count >= 1, _safe_points.Count.ToString());
            Debug.Assert(_masm.stack_size == _safe_points.Peek().Item2,
                $"now:{_masm.stack_size}:origin:{_safe_points.Peek().Item2}");
            _masm.Bind(_safe_points.Peek().Item1);
            var item = Tuple.Create(_safe_points.Peek().Item3, _masm.PC, _masm.stack_size);
            if (_real_safe_points.Count == 0 || _real_safe_points[_real_safe_points.Count - 1] != item)
            {
                _real_safe_points.Add(item);
            }

            _safe_points.Pop();
        }

        public MacroAssembler masm()
        {
            return _masm;
        }

        public List<byte> Codes
        {
            get { return _masm.codes; }
        }

        public int LocalsSize
        {
            get { return slot_counter; }
        }

        public List<byte> Pool
        {
            get
            {
                var res = new List<byte>();
                foreach (var arr in consts_pool.consts_pool)
                {
                    res.AddRange(arr);
                }

                return res;
            }
        }

        public List<int> SafePoints
        {
            get
            {
                var res = new List<int>();
                foreach (var point in _real_safe_points)
                {
                    res.Add(point.Item1);
                    res.Add(point.Item2);
                    res.Add(point.Item3);
                }

                return res;

            }
        }

        public override ASTNode VisitStmtMod(ASTStmtMod context)
        {
            BeginBlock();
            context.list.Accept(this);
            EndBlock();
            Debug.Assert(_safe_points.Count == 0, _safe_points.Count.ToString());
            Debug.Assert(masm().stack_size == 0, masm().stack_size.ToString());
            _masm.Ret();
            return context;
        }

        public override ASTNode VisitStore(ASTStore context)
        {

            context.addr.Accept(this);
            context.value.Accept(this);
            //FIXME add check
            //Debug.Assert(context.value.type.Equals((context.addr.type as RefType).type), $"{context.value.type}:{context.addr.type}");
            _masm.Store((context.addr.type as RefType).type, context.value.type);
            return context;
        }

        public override ASTNode VisitNodeList(ASTNodeList context)
        {
            foreach (var node in context.nodes)
            {
                node.Accept(this);
            }

            return context;
        }

        public override ASTNode VisitPair(ASTPair context)
        {
            context.left.Accept(this);
            context.right.Accept(this);
            return context;
        }

        public override ASTNode VisitIf(ASTIf context)
        {
            context.cond.Accept(this);

            var label = new MacroAssembler.Label();
            _masm.JeqL(label);
            context.then_list.Accept(this);

            var else_label = new MacroAssembler.Label();
            _masm.JmpL(else_label);
            _masm.Bind(label);
            foreach (var tmp in ((ASTNodeList) context.elsif_list).nodes)
            {
                var pair = tmp as ASTPair;
                pair.left.Accept(this);
                var l = new MacroAssembler.Label();
                _masm.JeqL(l);
                foreach (var st in (pair.right as ASTNodeList).nodes)
                {
                    st.Accept(this);
                }

                _masm.JmpL(else_label);
                _masm.Bind(l);
            }

            context.else_list.Accept(this);

            _masm.Bind(else_label);
            return context;
        }

        /*
        private void LoadFloat(ASTExpr expr)
        {
            Debug.Assert(expr is ASTFloat || expr is ASTInteger, expr.ToString());
            if (expr is ASTFloat)
            {
                masm().CLoadFloat((float)((expr as ASTFloat).value));
            } else if (expr is ASTInteger)
            {
                masm().CLoadFloat((float)(expr as ASTInteger).value);
            }

        }

        private void LoadInteger(ASTExpr expr)
        {
            Debug.Assert(expr is ASTFloat || expr is ASTInteger, expr.ToString());
            if (expr is ASTFloat)
            {
                masm().CLoadInteger((int)((expr as ASTFloat).value));
            }
            else if (expr is ASTInteger)
            {
    
                masm().CLoadInteger((int)(expr as ASTInteger).value);
            }
        }
        */


        private void LoadNumber(ASTExpr expr, IDataType type)
        {
            //Debug.Assert(!(type is LREAL), type.ToString());
            Debug.Assert(!(type is LINT), type.ToString());
            expr.Accept(this);

            masm().CallTypeConv(expr.type, type);
            /*
           if (type.IsInteger)
           {
               LoadInteger(expr);
           } else if (type.IsReal)
           {
               LoadFloat(expr);
           }
           else
           {
               Debug.Assert(false, type.ToString());
           }
           */
        }

        private void CaseCondition(
            MacroAssembler.Label curr_label,
            MacroAssembler.Label next_label,
            ASTNodeList conds,
            IDataType expr_type)
        {
            Debug.Assert(conds != null);

            foreach (var cond in conds.nodes)
            {
                var label = new MacroAssembler.Label();
                if (cond is ASTPair)
                {
                    var min = (cond as ASTPair).left as ASTExpr;
                    var max = (cond as ASTPair).right as ASTExpr;
                    IfLt(() =>
                    {
                        var type = CommonType(expr_type, min.type);
                        masm().Dup();
                        TypeConv(expr_type, type);
                        LoadNumber(min as ASTExpr, type);
                        masm().Cmp(type);
                    }, () => { masm().JmpL(label); });

                    IfGt(() =>
                    {
                        var type = CommonType(expr_type, max.type);
                        masm().Dup();
                        TypeConv(expr_type, type);
                        LoadNumber(max as ASTExpr, type);
                        masm().Cmp(type);
                    }, () => { masm().JmpL(label); });

                }
                else
                {

                    Debug.Assert(cond is ASTInteger || cond is ASTFloat);
                    IfNe(() =>
                    {
                        var type = CommonType(expr_type, (cond as ASTExpr).type);
                        masm().Dup();
                        TypeConv(expr_type, type);
                        LoadNumber(cond as ASTExpr, type);
                        masm().Cmp(type);

                    }, () => { masm().JmpL(label); });

                }

                masm().JmpL(curr_label);
                masm().Bind(label);
            }

            masm().JmpL(next_label);

        }

        public override ASTNode VisitCase(ASTCase context)
        {
            //var stack_size = masm().stack_size;
            context.expr.Accept(this);
            var exit_label = new MacroAssembler.Label();
            foreach (var tmp in context.elem_list.nodes)
            {
                var pair = tmp as ASTPair;
                var next_label = new MacroAssembler.Label();
                var curr_label = new MacroAssembler.Label();

                CaseCondition(curr_label, next_label, pair.left as ASTNodeList, ((ASTExpr) context.expr).type);

                masm().Bind(curr_label);
                pair.right.Accept(this);
                _masm.JmpL(exit_label);
                _masm.Bind(next_label);
            }

            context.else_stmts.Accept(this);
            //masm().stack_size = stack_size;
            _masm.Bind(exit_label);
            masm().Pop();
            return context;
        }

        public override ASTNode VisitExit(ASTExit context)
        {
            var label = _loop_labels.Last();
            _masm.JmpL(label);
            return context;
        }

        public override ASTNode VisitFor(ASTFor context)
        {
            //context.assign_stmt.Accept(this);
            //var stack_size = masm().stack_size;

            context.init.addr.Accept(this);
            _masm.Dup();
            var addr_id = (ushort) AllocateTempSlot();
            _masm.StoreLocal(addr_id);

            var commontype1 = CommonType(((ASTExpr) context.optional).type,
                CommonType(context.init.addr.ref_type.type, ((ASTExpr) context.expr).type));
            var commontype = GetPrimitiveType(commontype1);

            context.init.value.Accept(this);
            _masm.Store(context.init.addr.ref_type.type, context.init.value.type);

            context.optional.Accept(this);
            _masm.TypeConv(GetPrimitiveType(((ASTExpr) context.optional).type), commontype);
            _masm.Dup();
            var inc_id = (ushort) AllocateTempSlot();
            _masm.StoreLocal(inc_id);

            ConstType(commontype, 0);
            _masm.Cmp(commontype);
            var sign_id = (ushort) AllocateTempSlot();
            _masm.StoreLocal(sign_id);

            context.expr.Accept(this);
            _masm.TypeConv(GetPrimitiveType(((ASTExpr) context.expr).type), commontype);

            var label = new MacroAssembler.Label();
            var exit = new MacroAssembler.Label();
            _loop_labels.Add(exit);
            _masm.Bind(label);

            IfLt(() =>
            {
                _masm.LoadLocal(sign_id);
                _masm.CLoadInteger(0);
                _masm.ICmp();
            }, () =>
            {
                _masm.Dup();
                _masm.LoadLocal(addr_id);
                _masm.Load(context.init.addr.ref_type.type);
                _masm.TypeConv(GetPrimitiveType(context.init.addr.ref_type.type), commontype);
                _masm.Cmp(commontype);
                _masm.JgtL(exit);
            });

            IfGt(() =>
            {
                _masm.LoadLocal(sign_id);
                _masm.CLoadInteger(0);
                _masm.ICmp();
            }, () =>
            {
                _masm.Dup();
                _masm.LoadLocal(addr_id);
                _masm.Load(context.init.addr.ref_type.type);
                _masm.TypeConv(GetPrimitiveType(context.init.addr.ref_type.type), commontype);
                _masm.Cmp(commontype);
                _masm.JltL(exit);
            });

            context.stmt_list.Accept(this);

            _masm.LoadLocal(addr_id);
            _masm.Dup();
            _masm.Load(context.init.addr.ref_type.type);
            _masm.TypeConv(GetPrimitiveType(context.init.addr.ref_type.type), commontype);

            _masm.LoadLocal(inc_id);
            _masm.Add(commontype);
            _masm.Store(context.init.addr.ref_type.type, commontype1);
            _masm.JmpL(label);
            _masm.Bind(exit);
            _loop_labels.RemoveAt(_loop_labels.Count - 1);

            _masm.Pop();
            //masm().stack_size = stack_size;

            return context;
        }

        public override ASTNode VisitRepeat(ASTRepeat context)
        {
            var label = new MacroAssembler.Label();
            var exit = new MacroAssembler.Label();
            _loop_labels.Add(exit);
            _masm.Bind(label);
            context.stmts.Accept(this);
            context.expr.Accept(this);
            _masm.JeqL(label);
            _masm.Bind(exit);
            _loop_labels.RemoveAt(_loop_labels.Count - 1);
            return context;
        }

        public override ASTNode VisitWhile(ASTWhile context)
        {
            var label = new MacroAssembler.Label();
            var exit = new MacroAssembler.Label();
            _loop_labels.Add(exit);
            _masm.Bind(label);
            context.expr.Accept(this);
            _masm.JeqL(exit);
            context.stmts.Accept(this);
            _masm.JmpL(label);
            _masm.Bind(exit);
            _loop_labels.RemoveAt(_loop_labels.Count - 1);
            return context;
        }

        public override ASTNode VisitNameValue(ASTNameValue context)
        {
            LoadNameValue(context.name);
            return context;
        }

        public override ASTNode VisitNameAddr(ASTNameAddr context)
        {
            LoadNameAddr(context.name);

            return context;
        }

        private void LoadTypeMember(IDataType type, string member)
        {
            var tp = type as CompositiveType;
            Debug.Assert(tp != null, type.Name);
            var info = tp[member];
            var offset = info.ByteOffset;
            Debug.Assert(offset >= 0 && offset <= 32767);
            _masm.PSINC((short) offset);
        }

        private void LoadNameAddr(ASTName context)
        {
            context.Expr.Accept(this);
        }

        private void LoadNameValue(ASTName context)
        {
            LoadNameAddr(context);
            _masm.Load(context.type);
        }

        public override ASTNode VisitName(ASTName context)
        {
            LoadNameValue(context);
            return context;
        }

        public override ASTNode VisitTag(ASTTag context)
        {
            _masm.CLoadTag(context.name, (uint) context.size);
            return context;
        }

        private void CheckGuardSize(int guard_size)
        {
            var label = new MacroAssembler.Label();
            var label2 = new MacroAssembler.Label();
            _masm.Dup();
            _masm.CLoadInteger(guard_size);
            _masm.Cmp(DINT.Inst);
            _masm.JgeL(label2);
            _masm.Dup();
            _masm.CLoadInteger(0);
            _masm.Cmp(DINT.Inst);
            _masm.JgeL(label);
            _masm.Bind(label2);
            if (is_catch_exception)
            {
                _masm.BiPush(20);
                _masm.BiPush(4);
                _masm.Throw();
                int stack_size = _masm.stack_size;
                int orig_stack_size = _safe_points.Peek().Item2;
                Debug.Assert(stack_size >= orig_stack_size, $"{stack_size}:{orig_stack_size}");
                for (int i = 0; i < stack_size - orig_stack_size; ++i)
                {
                    _masm.Pop();
                }

                _masm.stack_size += stack_size - orig_stack_size;
                _masm.JmpL(_safe_points.Peek().Item1);
            }
            else
            {
                _masm.Ret(1);
                //ignore this push for next instrutions
                _masm.stack_size -= 1;
            }

            _masm.Bind(label);
        }

        public override ASTNode VisitArrayLoader(ASTArrayLoader context)
        {
            if (context.dims.Count() == 3)
            {
                Debug.Assert(!(context.info.DataType is BOOL));

                context.dims.nodes[2].Accept(this);
                CheckGuardSize(context.info.Dim3);
                _masm.CLoadInteger(context.info.Dim2 * context.info.Dim1 * context.info.DataType.ByteSize);
                _masm.IMul();
                _masm.PAdd();
            }

            if (context.dims.Count() >= 2)
            {
                Debug.Assert(!(context.info.DataType is BOOL));

                context.dims.nodes[1].Accept(this);
                CheckGuardSize(context.info.Dim2);
                _masm.CLoadInteger(context.info.Dim1 * context.info.DataType.ByteSize);
                _masm.IMul();
                _masm.PAdd();
            }

            Debug.Assert(context.dims.Count() >= 1, context.dims.ToString());
            if (context.dims.Count() >= 1)
            {
                context.dims.nodes[0].Accept(this);
                CheckGuardSize(context.info.Dim1);
                if (context.info.DataType is BOOL)
                {
                    _masm.DupX1();
                    _masm.CLoadInteger(32);
                    _masm.Div(MacroAssembler.PrimitiveType.DINT);
                    _masm.CLoadInteger(4);
                    _masm.Mul(MacroAssembler.PrimitiveType.DINT);
                    _masm.PAdd();
                    _masm.Swap();
                    _masm.CLoadInteger(32);
                    _masm.Mod(MacroAssembler.PrimitiveType.DINT);

                }
                else
                {
                    _masm.CLoadInteger(context.info.DataType.ByteSize);
                    _masm.IMul();
                    _masm.PAdd();

                }

            }

            return context;
        }

        public override ASTNode VisitStackSlot(ASTStackSlot context)
        {
            _masm.LoadLocal((ushort) context.no);
            return context;
        }

        public override ASTNode VisitTagOffset(ASTTagOffset context)
        {
            _masm.CLoadInteger(context.offset);
            _masm.PAdd();
            return context;
        }

        public override ASTNode VisitFloat(ASTFloat context)
        {
            _masm.CLoadFloat((float) context.value);
            return context;
        }

        public override ASTNode VisitInteger(ASTInteger context)
        {
            _masm.CLoadInteger(context.value);
            return context;
        }

        public override ASTNode VisitUnaryOp(ASTUnaryOp context)
        {
            context.expr.Accept(this);
            var type = context.type;
            if (context.op == ASTUnaryOp.Op.NEG)
            {
                _masm.Neg(type);
            }
            else if (context.op == ASTUnaryOp.Op.NOT)
            {
                Debug.Assert(type is DINT || type is BOOL);
                if (type is DINT)
                {
                    masm().TypeConv(GetPrimitiveType(context.expr.type), GetPrimitiveType(type));
                }

                _masm.Inv(type);
            }
            else
            {
                Debug.Assert(context.op == ASTUnaryOp.Op.PLUS);
            }

            return context;
        }

        public override ASTNode VisitBinRelOp(ASTBinRelOp context)
        {
            var op_type = context.op_type;
            context.left.Accept(this);
            _masm.RelConv(context.left.type, context.op_type);
            context.right.Accept(this);
            _masm.RelConv(context.right.type, context.op_type);
            var op = context.op;
            var label = new MacroAssembler.Label();
            _masm.Cmp(op_type);
            if (op == ASTBinOp.Op.EQ)
            {
                _masm.JeqL(label);
            }
            else if (op == ASTBinOp.Op.NEQ)
            {
                _masm.JneL(label);
            }
            else if (op == ASTBinOp.Op.GE)
            {
                _masm.JgeL(label);
            }
            else if (op == ASTBinOp.Op.GT)
            {
                _masm.JgtL(label);
            }
            else if (op == ASTBinOp.Op.LE)
            {
                _masm.JleL(label);
            }
            else if (op == ASTBinOp.Op.LT)
            {
                _masm.JltL(label);
            }
            else
            {
                Debug.Assert(false, op.ToString());
            }

            GenCond(label);

            return context;
        }

        public void TypeConv(IDataType from, IDataType to)
        {
            masm().TypeConv(GetPrimitiveType(from), GetPrimitiveType(to));
        }

        public override ASTNode VisitBinArithOp(ASTBinArithOp context)
        {
            var op = context.op;
            if (ASTBinOp.IsArithOp(op))
            {
                context.left.Accept(this);
                _masm.ArithConv(context.left.type, context.type);
                context.right.Accept(this);
                _masm.ArithConv(context.right.type, context.type);
            }
            else
            {
                Debug.Assert(false);
            }

            if (op == ASTBinOp.Op.PLUS)
            {
                _masm.Add(context.type);
            }
            else if (op == ASTBinOp.Op.MINUS)
            {
                _masm.Sub(context.type);
            }
            else if (op == ASTBinOp.Op.TIMES)
            {
                _masm.Mul(context.type);
            }
            else if (op == ASTBinOp.Op.DIVIDE)
            {
                _masm.Div(context.type);
            }
            else if (op == ASTBinOp.Op.MOD)
            {
                _masm.Mod(context.type);
            }
            else if (op == ASTBinOp.Op.POW)
            {
                _masm.CallName("POW", 2);
                //_masm.Pow(context.type);
            }
            else
            {
                Debug.Assert(false);
            }

            return context;
        }

        public override ASTNode VisitBinLogicOp(ASTBinLogicOp context)
        {
            var op = context.op;
            if (ASTBinOp.IsLogicOp(op))
            {
                context.left.Accept(this);
                _masm.LogicConv(context.left.type, context.type);
                context.right.Accept(this);
                _masm.LogicConv(context.right.type, context.type);
            }
            else
            {
                Debug.Assert(false);
            }

            if (op == ASTBinOp.Op.OR)
            {
                Debug.Assert(context.type is DINT || context.type is BOOL);
                _masm.IOr();
            }
            else if (op == ASTBinOp.Op.XOR)
            {
                Debug.Assert(context.type is DINT || context.type is BOOL);
                _masm.IXor();
            }
            else if (op == ASTBinOp.Op.AND)
            {
                Debug.Assert(context.type is DINT || context.type is BOOL);
                _masm.IAnd();
            }
            else
            {
                Debug.Assert(false);
            }

            return context;
        }

        private void GenCond(MacroAssembler.Label label)
        {
            _masm.ConstI0();
            var else_label = new MacroAssembler.Label();
            _masm.JmpL(else_label);
            _masm.Bind(label);
            _masm.ConstI1();
            _masm.Bind(else_label);
            //NOTE 这里往栈上放了两个值,但是实际执行路径只会放一个值
            _masm.stack_size -= 1;
        }

        public override ASTNode VisitPop(ASTPop context)
        {
            _masm.Pop();
            return context;
        }

        static bool IsCompatible(IDataType src, IDataType dest)
        {
            if (dest == AXIS_COMMON.Inst)
            {
                return src == AXIS_CIP_DRIVE.Inst || src == AXIS_VIRTUAL.Inst;
            }

            return src == dest;
        }

        static bool IsCompatible(ASTExpr expr, IDataType type, bool isRef)
        {
            if (isRef)
            {
                return expr is ASTName && IsCompatible(expr.type, type);
            }

            return type == expr.type;
        }

        public override ASTNode VisitCall(ASTCall context)
        {
            context.instr.Logic(this, context.param_list, Context.ST);

            return context;
        }

        public override ASTNode VisitInstr(ASTInstr context)
        {
            BeginBlock();

            context.instr.Logic(this, context.param_list, Context.ST);

            _masm.Pop();

            EndBlock();

            return context;
        }

        public override ASTNode VisitRLLInstruction(ASTRLLInstruction context)
        {
            context.instr.Logic(this, context.param_list_ast, Context.RLL);

            return context;
        }

        public override ASTNode VisitPrescan(ASTPrescan context)
        {

            BeginBlock();

            context.Instr.Prescan(this, context.ParamList);
            if (masm().stack_size == 1)
            {
                // 有些指令prescan没有清除栈
                masm().Pop();
            }

            EndBlock();

            Debug.Assert(masm().stack_size == 0);
            return context;
        }

        public override ASTNode VisitRTCall(ASTRTCall context)
        {
            context.list.nodes.Reverse();

            foreach (var param in context.list.nodes)
            {
                param.Accept(this);
            }

            _masm.CallName(context.name, context.list.nodes.Count);

            return context;
        }

        public override ASTNode VisitNop(ASTNop context)
        {
            return context;
        }

        public override ASTNode VisitTypeConv(ASTTypeConv context)
        {
            context.expr.Accept(this);
            _masm.CallTypeConv(context.expr.type, context.type);
            return context;
        }

        public override ASTNode VisitNullAddr(ASTNullAddr context)
        {
            _masm.ConstPnull();
            return context;
        }


        /*
        public override ASTNode VisitTempAddr(ASTTempAddr context)
        {
            if (context.slot.slot_no == -1)
            {
                context.slot.slot_no = this.slot_counter;
                this.slot_counter++;
            }
            _masm.LoadLocalAddr((ushort)context.slot.slot_no);
            return context;
        }
        */

        public override ASTNode VisitTempValue(ASTTempValue context)
        {
            if (context.slot.slot_no == -1)
            {
                context.slot.slot_no = this.slot_counter;
                this.slot_counter++;
            }

            _masm.LoadLocal((ushort) context.slot.slot_no);
            return context;
        }

        public UInt16 AllocateTempSlot()
        {
            var res = this.slot_counter;
            this.slot_counter++;
            return res;
        }

        public override ASTNode VisitRLLRoutine(ASTRLLRoutine context)
        {
            BeginBlock();
            foreach (var rung in context.list.nodes)
            {
                _masm.ConstI1();
                rung.Accept(this);
                //先吞掉上面这个表达式的值
                //再吞掉这次RUNG的输入值
                _masm.Pop();
                _masm.Pop();
            }

            EndBlock();

            Debug.Assert(masm().stack_size == 0, masm().stack_size.ToString());
            _masm.Ret();
            return context;
        }

        public override ASTNode VisitRLLParallel(ASTRLLParallel context)
        {
            _masm.Dup();
            _masm.CLoadInteger(0);
            foreach (var instr in context.list.nodes)
            {
                _masm.Swap();
                BeginBlock();
                _masm.Dup();
                instr.Accept(this);
                _masm.Swap();
                _masm.Pop();
                _masm.SwapX1();
                _masm.IOr();
                EndBlock();
            }

            _masm.Swap();
            _masm.Pop();
            return context;
        }

        public override ASTNode VisitRLLSequence(ASTRLLSequence context)
        {
            _masm.Dup();
            foreach (var instr in context.list.nodes)
            {
                BeginBlock();
                instr.Accept(this);
                _masm.Swap();
                _masm.Pop();
                EndBlock();
            }

            return context;
        }

        public override ASTNode VisitRet(ASTRet context)
        {
            _masm.Ret();
            return context;
        }

        public override ASTNode VisitDup(ASTDup context)
        {
            _masm.Dup();
            return context;
        }

        public override ASTNode VisitLIRIf(ASTLIRIf context)
        {
            context.expr.Accept(this);
            var label = new MacroAssembler.Label();
            _masm.JeqL(label);
            int begin_stack_size = _masm.stack_size;
            context.if_stmts.Accept(this);
            int if_stack_size = _masm.stack_size;
            var else_label = new MacroAssembler.Label();
            _masm.JmpL(else_label);
            _masm.Bind(label);
            context.else_stmts.Accept(this);
            int else_stack_size = _masm.stack_size;

            Debug.Assert(else_stack_size - begin_stack_size == (if_stack_size - begin_stack_size) * 2);
            _masm.stack_size -= (if_stack_size - begin_stack_size);
            _masm.Bind(else_label);
            return context;
        }

        public override ASTNode VisitFBDIRef(ASTFBDIRef context)
        {
            if (context.slot.slot_no == -1)
            {
                context.slot.slot_no = this.slot_counter;
                this.slot_counter++;
            }

            context.operand.Accept(this);
            _masm.StoreLocal((ushort) context.slot.slot_no);
            return context;
        }

        public override ASTNode VisitFBDInstruction(ASTFBDInstruction context)
        {
            context.parameters.Accept(this);

            var name = context.name;

            var tp = (context.arguments.nodes[0] as ASTNameAddr).type;
            Debug.Assert(tp is RefType);
            tp = (tp as RefType).type;

            Debug.Assert(context.instr != null);
            context.instr.Logic(this, context.arguments, Context.FBD);
            _masm.Pop();
            /*
            if (tp is AOIDataType)
            {
                name = tp.Name + ".Logic";

                //VisitParamList(context.arguments);

                for (int i = context.arguments.Count() - 1; i >= 0; --i)
                {
                    context.arguments.nodes[i].Accept(this);
                }

                _masm.CallName(name, context.arguments.Count());
                _masm.Pop();
            }
            else
            {
                context.instr.Logic(this, context.arguments, Context.FBD);
                Console.WriteLine($"{context.instr.Name}");
                _masm.Pop();
            }
            */

            return context;
        }

        public override ASTNode VisitFBDRoutine(ASTFBDRoutine context)
        {
            context.list.Accept(this);
            Debug.Assert(masm().stack_size == 0, masm().stack_size.ToString());

            _masm.Ret();
            return context;
        }


        public override ASTNode VisitStatus(ASTStatus context)
        {
            _masm.CLoadInteger(context.index);
            _masm.CallName("SYSLOAD", 1);
            return context;
        }

        public override ASTNode VisitExprStatus(ASTExprStatus context)
        {
            _masm.CLoadInteger(context.Index);
            _masm.CallName("SYSLOAD", 1);
            _masm.CLoadInteger(0);
            return context;
        }

        public override ASTNode VisitExprTag(ASTExprTag context)
        {
            _masm.CLoadTag(context.Name, (uint) context.Size);
            if (context.type.IsBool)
            {
                _masm.CLoadInteger(0);
            }

            return context;
        }

        public override ASTNode VisitExprArraySubscript(ASTExprArraySubscript context)
        {
            context.Expr.Accept(this);
            var type = context.Expr.type as ArrayType;
            if (type.Dim3 != 0)
            {
                context.Index2.Accept(this);
                CheckGuardSize(type.Dim3);
                _masm.CLoadInteger(type.Dim2 * type.Dim1 * type.BaseDataSize);
                _masm.IMul();
                _masm.PAdd();
            }

            if (type.Dim2 != 0)
            {
                //Debug.Assert(!(context.info.DataType is BOOL));
                context.Index1.Accept(this);
                //context.dims.nodes[1].Accept(this);
                CheckGuardSize(type.Dim2);
                _masm.CLoadInteger(type.Dim1 * type.BaseDataSize);
                _masm.IMul();
                _masm.PAdd();
            }

            //Debug.Assert(context.dims.Count() >= 1, context.dims.ToString
            Debug.Assert(type.Dim1 != 0, type.Dim1.ToString());
            if (type.Dim1 >= 1)
            {
                context.Index0.Accept(this);
                CheckGuardSize(type.Dim1);
                if (type.Type is BOOL)
                {
                    _masm.DupX1();
                    _masm.CLoadInteger(32);
                    _masm.Div(MacroAssembler.PrimitiveType.DINT);
                    _masm.CLoadInteger(4);
                    _masm.Mul(MacroAssembler.PrimitiveType.DINT);
                    _masm.PAdd();
                    _masm.Swap();
                    _masm.CLoadInteger(32);
                    _masm.Mod(MacroAssembler.PrimitiveType.DINT);

                }
                else
                {
                    _masm.CLoadInteger(type.BaseDataSize);
                    _masm.IMul();
                    _masm.PAdd();
                }
            }

            return context;
        }

        public override ASTNode VisitExprMember(ASTExprMember context)
        {
            context.Expr.Accept(this);
            _masm.CLoadInteger(context.Offset);
            _masm.PAdd();
            if (context.type.IsBool)
            {
                _masm.CLoadInteger(context.BitOffset);
            }

            return context;
        }

        public override ASTNode VisitExprConstString(ASTExprConstString context)
        {
            _masm.CLoadString(context.Value);
            return context;
        }

        public override ASTNode VisitExprParameter(ASTExprParameter context)
        {

            _masm.LoadLocal((ushort) (context.SlotNo));
            if (context.type.IsBool)
            {

                _masm.LoadLocal((ushort) (context.SlotNo + 1));
            }


            return context;
        }

        public override ASTNode VisitExprBitSel(ASTExprBitSel context)
        {

            context.Expr.Accept(this);
            var integer = context.Index as ASTInteger;
            if (integer == null)
            {
                context.Index.Accept(this);
            }
            else
            {
                _masm.CLoadInteger(integer.value);
            }
           
            return context;
        }

        public static MacroAssembler.PrimitiveType GetPrimitiveType(IDataType tp)
        {
            if (tp is SINT || tp is INT || tp is DINT || tp is BOOL)
                return MacroAssembler.PrimitiveType.DINT;
            if (tp is LINT)
                return MacroAssembler.PrimitiveType.LINT;
            if (tp is REAL)
                return MacroAssembler.PrimitiveType.REAL;
            if (tp is LREAL)
                return MacroAssembler.PrimitiveType.LREAL;
            Debug.Assert(false);
            return MacroAssembler.PrimitiveType.DINT;
        }

        public MacroAssembler.Label FindLabel(string name)
        {
            Debug.Assert(name != null);
            if (_labels.ContainsKey(name))
            {
                return _labels[name];
            }

            var res = new MacroAssembler.Label();
            _labels.Add(name, res);
            return res;
        }

        private void Cmp(ASTExpr lhs, ASTExpr rhs)
        {
            var type = CommonType(lhs.type, rhs.type);
            lhs.Accept(this);
            _masm.TypeConv(GetPrimitiveType(lhs.type), GetPrimitiveType(type));
            rhs.Accept(this);
            _masm.TypeConv(GetPrimitiveType(rhs.type), GetPrimitiveType(type));
            _masm.Cmp(GetPrimitiveType(type));

        }

        public static IDataType CommonType(IDataType lhs, IDataType rhs)
        {
            if (lhs.IsBool && rhs.IsBool)
            {
                return BOOL.Inst;
            }

            if (lhs is LREAL || rhs is LREAL)
            {
                Debug.Assert(!lhs.IsBool);
                Debug.Assert(!rhs.IsBool);
                return LREAL.Inst;
            }

            if (lhs.IsReal || rhs.IsReal)
            {
                Debug.Assert(!lhs.IsBool);
                Debug.Assert(!rhs.IsBool);
                return REAL.Inst;
            }

            if (lhs.IsLINT || rhs.IsLINT)
            {
                Debug.Assert(lhs.IsInteger);
                Debug.Assert(rhs.IsInteger);
                return LINT.Inst;
            }

            if (lhs.IsInteger && rhs.IsInteger)
            {
                return DINT.Inst;
            }

            throw new NotImplementedException();
        }

        private void Load(ASTExpr expr, IDataType to_type)
        {
            expr.Accept(this);
            _masm.TypeConv(GetPrimitiveType(expr.type), GetPrimitiveType(to_type));
        }

        private void Load(ASTNameAddr addr, IDataType to_type)
        {
            addr.Accept(this);
            _masm.Load(addr.ref_type.type);
            _masm.TypeConv(GetPrimitiveType(addr.ref_type.type), GetPrimitiveType(to_type));
        }

        private void ConstType(MacroAssembler.PrimitiveType type, int value)
        {
            if (type == MacroAssembler.PrimitiveType.DINT)
            {
                Debug.Assert(value == 0 || value == 1 || value == -1, value.ToString());
                _masm.CLoadInteger(value);
                return;
            }

            if (type == MacroAssembler.PrimitiveType.REAL)
            {
                Debug.Assert(value == 0 || value == 1, value.ToString());
                _masm.CLoadFloat(value);
                return;
            }

            throw new NotImplementedException();
        }

        private void ConstType(IDataType type, int value)
        {
            if (type is DINT)
            {
                Debug.Assert(value == 0 || value == 1 || value == -1, value.ToString());
                _masm.CLoadInteger(value);
                return;
            }

            if (type is REAL)
            {
                Debug.Assert(value == 0 || value == 1, value.ToString());
                _masm.CLoadFloat(value);
            }

            throw new NotImplementedException();
        }


        private void IfInstr(Action cond, Action trueIn, Action<MacroAssembler.Label> Jmp)
        {
            cond();
            var exit_label = new MacroAssembler.Label();
            Jmp(exit_label);
            trueIn();
            _masm.Bind(exit_label);

        }

        private void IfInstr(Action cond, Action trueIn, Action falseIn, Action<MacroAssembler.Label> Jmp)
        {
            cond();
            var else_label = new MacroAssembler.Label();
            Jmp(else_label);
            trueIn();
            var exit_label = new MacroAssembler.Label();
            _masm.JmpL(exit_label);
            _masm.Bind(else_label);
            falseIn();
            _masm.Bind(exit_label);

        }

        private void IfLt(Action cond, Action trueIn, Action falseIn) =>
            IfInstr(cond, trueIn, falseIn, (label) => _masm.JgeL(label));

        private void IfLt(Action cond, Action trueIn) =>
            IfInstr(cond, trueIn, (label) => _masm.JgeL(label));

        public void IfLe(Action cond, Action trueIn, Action falseIn) =>
            IfInstr(cond, trueIn, falseIn, (label) => _masm.JgtL(label));

        private void IfLe(Action cond, Action trueIn) =>
            IfInstr(cond, trueIn, (label) => _masm.JgtL(label));

        private void IfGt(Action cond, Action trueIn, Action falseIn) =>
            IfInstr(cond, trueIn, falseIn, (label) => _masm.JleL(label));

        private void IfGt(Action cond, Action trueIn) =>
            IfInstr(cond, trueIn, (label) => _masm.JleL(label));

        public void IfGe(Action cond, Action trueIn, Action falseIn) =>
            IfInstr(cond, trueIn, falseIn, (label) => _masm.JltL(label));

        private void IfGe(Action cond, Action trueIn) =>
            IfInstr(cond, trueIn, (label) => _masm.JltL(label));

        public void IfEq(Action cond, Action trueIn, Action falseIn) =>
            IfInstr(cond, trueIn, falseIn, (label) => _masm.JneL(label));

        public void IfEq(Action cond, Action trueIn) =>
            IfInstr(cond, trueIn, (label) => _masm.JneL(label));

        public void IfNe(Action cond, Action trueIn, Action falseIn) =>
            IfInstr(cond, trueIn, falseIn, (label) => _masm.JeqL(label));

        public void IfNe(Action cond, Action trueIn) =>
            IfInstr(cond, trueIn, (label) => _masm.JeqL(label));

        public int GenCopyParameter(ASTExpr expr)
        {
            if (expr is ASTExprTag)
            {
                var tag = (ASTExprTag) expr;
                expr.Accept(this);
                masm().Dup();
                masm().Dup();
                masm().CLoadInteger(((ASTExprTag) expr).Size);
                masm().PAdd();
                return tag.type.BaseDataSize;
            }
            else if (expr is ASTExprArraySubscript)
            {
                var tag = (ASTExprArraySubscript) expr;
                tag.Accept(this);

                tag.Expr.Accept(this);
                masm().Dup();
                masm().CLoadInteger(tag.Expr.type.ByteSize);
                masm().PAdd();
                return tag.type.BaseDataSize;
            }
            else if (expr is ASTExprMember)
            {
                var member = (ASTExprMember) expr;
                member.Accept(this);
                masm().Dup();
                masm().Dup();
                masm().CLoadInteger(member.type.ByteSize);
                masm().PAdd();
                return member.type.BaseDataSize;
            }else if (expr is ASTExprParameter)
            {
                var parameter = (ASTExprParameter) expr;
                parameter.Accept(this);
                masm().Dup();
                masm().Dup();
                masm().CLoadInteger(parameter.type.ByteSize);
                masm().PAdd();
                return parameter.type.BaseDataSize;
            }
            else
            {
                Debug.Assert(false, expr.ToString());
                return 1;
            }

        }

        public void RepeatAction(Action action, ASTExpr initial, ASTExpr terminal, ASTExpr step, MacroAssembler.Label exit)
        {
            initial.Accept(this);

            var addr_id = (ushort) AllocateTempSlot();
            _masm.StoreLocal(addr_id);
            
            step.Accept(this);
            _masm.Dup();
            var inc_id = (ushort) AllocateTempSlot();
            _masm.StoreLocal(inc_id);

            _masm.CLoadInteger(0);
            _masm.ICmp();
            var sign_id = (ushort) AllocateTempSlot();
            _masm.StoreLocal(sign_id);

            terminal.Accept(this);

            var label = new MacroAssembler.Label();
            _loop_labels.Add(exit);
            _masm.Bind(label);

            IfLt(() =>
            {
                _masm.LoadLocal(sign_id);
                _masm.CLoadInteger(0);
                _masm.ICmp();
            }, () =>
            {
                _masm.Dup();
                _masm.LoadLocal(addr_id);
                _masm.ICmp();
                _masm.JgtL(exit);
            });

            IfGt(() =>
            {
                _masm.LoadLocal(sign_id);
                _masm.CLoadInteger(0);
                _masm.ICmp();
            }, () =>
            {
                _masm.Dup();
                _masm.LoadLocal(addr_id);
                _masm.ICmp();
                _masm.JltL(exit);
            });

            action();

            _masm.LoadLocal(addr_id);
            _masm.LoadLocal(inc_id);
            _masm.IAdd();
            _masm.StoreLocal(addr_id);
            _masm.JmpL(label);
            _masm.Bind(exit);
            _loop_labels.RemoveAt(_loop_labels.Count - 1);

            _masm.Pop();
        }
    }
}
