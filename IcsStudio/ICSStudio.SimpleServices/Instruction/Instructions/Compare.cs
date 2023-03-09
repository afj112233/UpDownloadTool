using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.PredefinedType;
using NLog.Time;

namespace ICSStudio.SimpleServices.Instruction.Instructions
{
    using ParamInfo = Tuple<Predicate<ASTExpr>, IDataType, ParameterType>;
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;

    internal class CMPInstruction : IXInstruction
    {
        public CMPInstruction()
        {
            Name = "CMP";
        }

        public sealed override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            var label = new MacroAssembler.Label();
            var exit = new MacroAssembler.Label();
            gen.masm().Dup();
            gen.masm().JeqL(label);
            paramList.nodes[0].Accept(gen);
            var tp = (paramList.nodes[0] as ASTExpr).type;
            gen.IfEq(() =>
                {
                    LoadZero(gen, tp);
                    gen.masm().Cmp(tp);
                },
                () =>
                {
                    gen.masm().BiPush(0);
                },
                () => { gen.masm().BiPush(1); }
            );
            gen.masm().JmpL(exit);
            gen.masm().Bind(label);
            gen.masm().BiPush(0);
            gen.masm().Bind(exit);
            gen.masm().stack_size -= 2;
        }

        private static void LoadZero(CodeGenVisitor gen, IDataType tp)
        {
            if (tp is DINT || tp.IsBool)
            {
                gen.masm().CLoadInteger(0);
            } else if (tp is REAL)
            {
                gen.masm().CLoadFloat(0.0f);
            }
            else
            {
                Debug.Assert(false, tp.ToString());
            }
        }
        
    }
    internal class LIMInstruction : RLLInstruction
    {
        public LIMInstruction(ParamInfoList infos) : base("LIM")
        {
            
        }

        private static void GenLH(CodeGenVisitor gen, ASTExpr test)
        {
            test.Accept(gen);
            gen.masm().DupX1();

            var else_label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();

            gen.masm().JmpL(exit_label);
            gen.masm().Bind(else_label);

            gen.masm().Bind(exit_label);
        }

        private static void GenHL(CodeGenVisitor gen, ASTExpr test)
        {

            var else_label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();

            gen.masm().JmpL(exit_label);
            gen.masm().Bind(else_label);

            gen.masm().Bind(exit_label);

        }

        private static void GenTrueIn(CodeGenVisitor gen, ASTExpr low, ASTExpr test, ASTExpr hi)
        {
            low.Accept(gen);
            gen.masm().Dup();
            hi.Accept(gen);
            gen.masm().DupX1();
            gen.masm().Le(REAL.Inst);
            var else_label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();
            gen.masm().JeqL(else_label);
            GenLH(gen, test);
            gen.masm().JmpL(exit_label);
            gen.masm().Bind(else_label);
            GenHL(gen, test);
            gen.masm().Bind(exit_label);

        }
        
        private IDataType FixType(ASTExpr a,ASTExpr b, CodeGenVisitor gen)
        {
            if (a.type.IsReal || b.type.IsReal)
            {
                if (a.type.IsInteger)
                {
                    a.Accept(gen);
                    gen.masm().TypeConv(CodeGenVisitor.GetPrimitiveType(a.type), CodeGenVisitor.GetPrimitiveType(b.type));
                    b.Accept(gen);
                    return b.type;
                }
                else
                {
                    a.Accept(gen);
                    b.Accept(gen);
                    gen.masm().TypeConv(CodeGenVisitor.GetPrimitiveType(b.type), CodeGenVisitor.GetPrimitiveType(a.type));
                    return a.type;
                }
            }
            else
            {
                a.Accept(gen);
                b.Accept(gen);
                return a.type;
            }
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            //Utils.ThrowNotImplemented(this.Name);
            //gen.masm().BiPush(0);
            var low = paramList.nodes[0] as ASTExpr;
            var test = paramList.nodes[1] as ASTExpr;
            var high = paramList.nodes[2] as ASTExpr;
            var lTp = (low as ASTExpr).type;
            var tTp = (test as ASTExpr).type;
            var hTp = (test as ASTExpr).type;
            gen.masm().Pop();
            gen.IfGe(() =>
            {
                var tp= FixType(high,low,gen);
                gen.masm().Cmp(tp);
            }, () =>
            {
                //test.Accept(gen);
                //low.Accept(gen);
                var tp = FixType(test, low, gen);
                gen.masm().Ge(tp);
                //test.Accept(gen);
                //high.Accept(gen);
                tp = FixType(test, high, gen);
                gen.masm().Le(tp);
                gen.masm().And(MacroAssembler.PrimitiveType.DINT);
            }, () =>
            {
                //test.Accept(gen);
                //low.Accept(gen);
                var tp = FixType(test, low, gen);
                gen.masm().Ge(tp);
                //test.Accept(gen);
                //high.Accept(gen);
                tp = FixType(test, high, gen);
                gen.masm().Le(tp);
                gen.masm().Or(MacroAssembler.PrimitiveType.DINT);
            });
            gen.masm().stack_size-= 1;
            //gen.masm().CLoadInteger(1);
            //gen.masm().Eq(DINT.Inst);
            //test.Accept(gen);
            //low.Accept(gen);
            //gen.masm().Cmp(CodeGenVisitor.GetPrimitiveType((low as ASTExpr).type));
            //high.Accept(gen);
            //test.Accept(gen);
            //gen.masm().Cmp(CodeGenVisitor.GetPrimitiveType((test as ASTExpr).type));

            //gen.masm().Or(MacroAssembler.PrimitiveType.DINT);
            //gen.masm().If(() =>
            //{
            //    gen.masm().CLoadInteger(0);
            //    gen.masm().Cmp(MacroAssembler.PrimitiveType.DINT);
            //}, () =>
            //{
            //    gen.masm().BiPush(1);
            //}, () =>
            //{
            //    gen.masm().BiPush(0);
            //});

        }

        /*
        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            var else_label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();
            var low_limit = paramList.nodes[0] as ASTExpr;
            var test = paramList.nodes[0] as ASTExpr;
            var hi_limit = paramList.nodes[0] as ASTExpr;
            gen.masm().Dup();
            gen.masm().JeqL(else_label);
            GenTrueIn(gen, low_limit, test, hi_limit);
            gen.masm().JmpL(exit_label);
            gen.masm().Bind(else_label);
            gen.masm().BiPush(0);
            gen.masm().Bind(exit_label);

            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null, paramList.nodes[0].ToString());
            addr.Accept(gen);
            Utils.Store(gen, FBD_LIMIT.Inst["LowLimit"], paramList.nodes[1] as ASTExpr);
            Utils.Store(gen, ALARM_ANALOG.Inst["Test"], paramList.nodes[2] as ASTExpr);
            Utils.Store(gen, ALARM_ANALOG.Inst["HighLimit"], paramList.nodes[3] as ASTExpr);
            gen.masm().CallName("LIM", 1);
            gen.masm().JmpL(exit_label);
            gen.masm().BiPush(0);
            gen.masm().Bind(exit_label);

            gen.masm().If(() =>
            {

            }, () =>
            {

            }, () =>
            {

            });

            //if else 都往里放了个值
            gen.masm().stack_size -= 1;

        }
        */
    }

    internal class FLIMInstruction : FixedFBDInstruction
    {
        public FLIMInstruction(ParamInfoList infos) : base("LIM", infos)
        {

        }

        protected override IDataTypeMember EnableInMember => FBD_LIMIT.Inst["EnableIn"];
        protected override string InstrName => "FBDLIM";

    }
    internal class FMEQInstruction : FixedFBDInstruction
    {
        public FMEQInstruction(ParamInfoList infos) : base("MEQ", infos)
        {

        }

        protected override IDataTypeMember EnableInMember => FBD_MASK_EQUAL.Inst["EnableIn"];
        protected override string InstrName => "FBDMEQ";

    }
    internal class MEQInstruction : FixedInstruction
    {
        public MEQInstruction(ParamInfoList infos) : base("MEQ", infos)
        {

        }

        public sealed override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
          
            Debug.Assert(paramList.Count() == 3, paramList.Count().ToString());

            var exit = new MacroAssembler.Label();
            var elseLabel = new MacroAssembler.Label();
            var stack_size = gen.masm().stack_size;

            gen.masm().Dup();
            gen.masm().JeqL(elseLabel);
            paramList.nodes[1].Accept(gen);
            gen.masm().Dup();
            paramList.nodes[0].Accept(gen);
            gen.masm().And(CodeGenVisitor.GetPrimitiveType((paramList.nodes[0] as ASTExpr).type));
            gen.masm().Swap();
            paramList.nodes[2].Accept(gen);
            gen.masm().And(CodeGenVisitor.GetPrimitiveType((paramList.nodes[2] as ASTExpr).type));

            gen.masm().Cmp(CodeGenVisitor.GetPrimitiveType((paramList.nodes[0] as ASTExpr).type));
            gen.masm().JneL(elseLabel);

            gen.masm().BiPush(1);
            gen.masm().JmpL(exit);

            gen.masm().Bind(elseLabel);

            gen.masm().BiPush(0);
            gen.masm().Bind(exit);

            gen.masm().stack_size = stack_size + 1;
           
        }

    }

    internal abstract class RLLLogicBinOpInstruction : FixedInstruction
    {
        public RLLLogicBinOpInstruction(string name, ParamInfoList infos) : base(name, infos)
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(paramList.Count() == 2, paramList.Count().ToString());
            var tmp = paramList.Accept(checker) as ASTNodeList;
            var lhsType = (tmp.nodes[0] as ASTExpr).type;
            var rhsType = (tmp.nodes[1] as ASTExpr).type;
            var type = CommonType(lhsType, rhsType);
            var res = new ASTNodeList();
            var node = tmp.nodes[0];
            res.AddNode(new ASTTypeConv(node as ASTExpr, type)) ;
            node = tmp.nodes[1];
            res.AddNode(new ASTTypeConv(node as ASTExpr, type));
            return res;
        }

        public sealed override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {

            Debug.Assert(paramList.Count() == 2, paramList.Count().ToString());
            var left = paramList.nodes[0] as ASTExpr;
            var right = paramList.nodes[1] as ASTExpr;

            Debug.Assert(left != null && right != null);
            Debug.Assert(left.type == right.type);

            var exit = new MacroAssembler.Label();
            var elseLabel = new MacroAssembler.Label();
            var stack_size = gen.masm().stack_size;

            gen.masm().Dup();
            gen.masm().JeqL(elseLabel);
            left.Accept(gen);
            right.Accept(gen);
            if (left.type is STRING)
            {
                Debug.Assert(false);
            }
            else
            {
                Debug.Assert(left.type == right.type);
                gen.masm().Cmp(left.type);
               //gen.masm().Cmp(CodeGenVisitor.CommonType(left.type, right.type));
            }

            Branch(gen, elseLabel);

            gen.masm().BiPush(1);
            gen.masm().JmpL(exit);

            gen.masm().Bind(elseLabel);

            gen.masm().BiPush(0);
            gen.masm().Bind(exit);

            gen.masm().stack_size = stack_size + 1;
        }

        protected abstract void Branch(CodeGenVisitor gen, MacroAssembler.Label label);

    }

    internal class EQUInstruction : RLLLogicBinOpInstruction
    {
        public EQUInstruction(ParamInfoList infos) : base("EQU", infos)
        {
        }

        /*
        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(paramList.Count() == 2, paramList.Count().ToString());

            var tmp = paramList.Accept(checker) as ASTNodeList;
            Debug.Assert(tmp != null);
            var left = tmp.nodes[0] as ASTExpr;
            var right = tmp.nodes[1] as ASTExpr;
            Debug.Assert(left != null && right != null);
            var common = CommonType(left.type, right.type);
            Debug.Assert(common == BOOL.Inst || common == DINT.Inst || common == REAL.Inst);

            var res = new ASTNodeList();
            foreach (var expr in tmp.nodes)
            {
                res.AddNode(new ASTTypeConv(expr as ASTExpr, common));
            }
            //PrepareFixedParameters(tmp, res);
            return res;
        }


        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {

            Debug.Assert(paramList.Count() == 2, paramList.Count().ToString());
            var left = paramList.nodes[0] as ASTExpr;
            var right = paramList.nodes[1] as ASTExpr;

            Debug.Assert(left != null && right != null);
            Debug.Assert(left.type == right.type);

            var exit = new MacroAssembler.Label();
            var elseLabel = new MacroAssembler.Label();
            var stack_size = gen.masm().stack_size;

            gen.masm().Dup();
            gen.masm().JeqL(elseLabel);
            left.Accept(gen);
            right.Accept(gen);
            if (left.type is STRING)
            {

                throw new NotImplementedException();
            }
            else
            {
                gen.masm().Cmp(left.type);
            }
            gen.masm().JneL(exit);

            gen.masm().BiPush(1);

            gen.masm().Bind(elseLabel);

            gen.masm().BiPush(0);
            gen.masm().Bind(exit);

            gen.masm().stack_size = stack_size + 1;
        }
        */

        protected override void Branch(CodeGenVisitor gen, MacroAssembler.Label label)
        {
            gen.masm().JneL(label);
        }

    }

    internal class FEQUInstruction : FixedFBDInstruction
    {
        public FEQUInstruction(ParamInfoList infos) : base("EQU", infos)
        {

        }

        protected override string InstrName => "FBDEQU";
    }
    internal class NEQInstruction : RLLLogicBinOpInstruction
    {
        public NEQInstruction(ParamInfoList infos) : base("NEQ", infos)
        {

        }

        /*
        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }
        */

        protected override void Branch(CodeGenVisitor gen, MacroAssembler.Label label)
        {
            gen.masm().JeqL(label);
        }
    }
    internal class FNEQInstruction : FixedFBDInstruction
    {
        public FNEQInstruction(ParamInfoList infos) : base("NEQ", infos)
        {

        }

        protected override string InstrName => "FBDNEQ";
    }
    internal class RNEQInstruction : FixedInstruction
    {
        public RNEQInstruction(ParamInfoList infos) : base("NEQ", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Utils.ThrowNotImplemented(this.Name);
            gen.masm().BiPush(0);
        }

    }
    internal class LESInstruction : RLLLogicBinOpInstruction
    {
        public LESInstruction(ParamInfoList infos) : base("LES", infos)
        {

        }

        protected override void Branch(CodeGenVisitor gen, MacroAssembler.Label label)
        {
            gen.masm().JgeL(label);
        }

    }

    internal class FLESInstruction : FixedFBDInstruction
    {
        public FLESInstruction(ParamInfoList infos) : base("LES", infos)
        {

        }

        protected override string InstrName => "FBDLES";
    }

    internal class GRTInstruction : RLLLogicBinOpInstruction
    {
        public GRTInstruction(ParamInfoList infos) : base("GRT", infos)
        {

        }

        protected override void Branch(CodeGenVisitor gen, MacroAssembler.Label label)
        {
            gen.masm().JleL(label);
        }

    }

    internal class FGRTInstruction : FixedFBDInstruction
    {
        public FGRTInstruction(ParamInfoList infos) : base("GRT", infos)
        {

        }

        protected override string InstrName => "FBDGRT";
    }
    internal class LEQInstruction : RLLLogicBinOpInstruction
    {
        public LEQInstruction(ParamInfoList infos) : base("LEQ", infos)
        {

        }

        protected override void Branch(CodeGenVisitor gen, MacroAssembler.Label label)
        {
            gen.masm().JgtL(label);
        }

    }

    internal class FLEQInstruction : FixedFBDInstruction
    {
        public FLEQInstruction(ParamInfoList infos) : base("LEQ", infos)
        {

        }

        protected override string InstrName => "FBDLEQ";
    }

    internal class FGEQInstruction : FixedFBDInstruction
    {
        public FGEQInstruction(ParamInfoList infos) : base("GEQ", infos)
        {

        }

        protected override string InstrName => "FBDGEQ";
    }

    internal class GEQInstruction : RLLLogicBinOpInstruction
    {
        public GEQInstruction(ParamInfoList infos) : base("GEQ", infos)
        {

        }

        protected override void Branch(CodeGenVisitor gen, MacroAssembler.Label label)
        {
            gen.masm().JltL(label);
        }

    }

}
