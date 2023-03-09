using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.Instruction.Instructions
{
    using ParamInfo = Tuple<Predicate<ASTExpr>, IDataType, ParameterType>;
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;

    internal class MVMTInstruction : FixedFBDInstruction
    {
        public MVMTInstruction(ParamInfoList infos) : base("MVMT", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FBD_MASKED_MOVE.Inst);
            addr.Accept(gen);
            var enableOut = FBD_MASKED_MOVE.Inst["EnableOut"];
            var enableIn = FBD_MASKED_MOVE.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("MVMTTag", FBD_MASKED_MOVE.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => FBD_MASKED_MOVE.Inst["EnableIn"];
        protected override string InstrName => "MVMT";
    }

    internal class SWPBInstruction : RLLSTInstruction
    {
        public SWPBInstruction() : base("SWPB")
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.SWPB.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.SWPB.ParseRLLParameters(parameters);
        }
        
        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(paramList.Count() == 3, paramList.Count().ToString());
            var type = (paramList.nodes[0] as ASTExpr).type;


            var name = paramList.nodes[2] as ASTNameAddr;
            name.Accept(gen);
            paramList.nodes[0].Accept(gen);
            if (type is INT)
            {
                gen.masm().CallName("SWPBW", 1);
            }
            else if (type is DINT)
            {
                paramList.nodes[1].Accept(gen);
                gen.masm().CallName("SWPBD", 2);
            }
            else
            {
                Debug.Assert(false, paramList.nodes[0].ToString());
            }



            gen.masm().Store(name.ref_type.type, type);

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var parameters = paramList.Accept(checker) as ASTNodeList;
            Debug.Assert(parameters.Count() == 3, parameters.Count().ToString());

            var source = paramList.nodes[0] as ASTName;
            if (source == null)
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid kind of operand or argument.");
            }

            if (source.type is INT||source.type is DINT)
            {
                if (source.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 1:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
            }



            var dest = paramList.nodes[2] as ASTName;
            if (dest == null)
            {
                throw new TypeCheckerException($"{Name},param 3:Invalid kind of operand or argument.");
            }

            if (dest.type is INT || dest.type is DINT)
            {
                if(source.type is DINT&&dest.type is INT)throw new TypeCheckerException($"{Name},param 3:Mixing of data types allowed only when source is INT and destination is DINT.");
                if (dest.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 3:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 3:Invalid data type.Argument must match parameter data type.");
            }

            var res = new ASTNodeList();
            res.AddNode(parameters.nodes[0]);
            res.AddNode(parameters.nodes[1]);
            res.AddNode(new ASTNameAddr(parameters.nodes[2] as ASTName));
            return res;
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source", new ExpectType(INT.Inst, DINT.Inst));
            var param2 = new Tuple<string, IDataType>("Order Mode", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param3 = new Tuple<string, IDataType>("Dest", new ExpectType(INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>()
                {param1, param2, param3};
        }

    }

    internal class BTDTInstruction : FixedFBDInstruction
    {
        public BTDTInstruction(ParamInfoList infos) : base("BTDT", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) &&
                         (addr.type as RefType).type == FBD_BIT_FIELD_DISTRIBUTE.Inst);
            addr.Accept(gen);
            var enableOut = FBD_BIT_FIELD_DISTRIBUTE.Inst["EnableOut"];
            var enableIn = FBD_BIT_FIELD_DISTRIBUTE.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("BTDTTag", FBD_BIT_FIELD_DISTRIBUTE.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => FBD_BIT_FIELD_DISTRIBUTE.Inst["EnableIn"];
        protected override string InstrName => "BTDT";
    }

    internal class DFFInstruction : FixedFBDInstruction
    {
        public DFFInstruction(ParamInfoList infos) : base("DFF", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FLIP_FLOP_D.Inst);
            addr.Accept(gen);
            var enableOut = FLIP_FLOP_D.Inst["EnableOut"];
            var enableIn = FLIP_FLOP_D.Inst["EnableIn"];
            var firstScan = FLIP_FLOP_D.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("DFFTag", FLIP_FLOP_D.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => FLIP_FLOP_D.Inst["EnableIn"];
        protected override string InstrName => "DFF";
    }

    internal class JKFFInstruction : FixedFBDInstruction
    {
        public JKFFInstruction(ParamInfoList infos) : base("JKFF", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FLIP_FLOP_JK.Inst);
            addr.Accept(gen);
            var enableOut = FLIP_FLOP_JK.Inst["EnableOut"];
            var enableIn = FLIP_FLOP_JK.Inst["EnableIn"];
            var firstScan = FLIP_FLOP_JK.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("JKFFTag", FLIP_FLOP_JK.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => FLIP_FLOP_JK.Inst["EnableIn"];
        protected override string InstrName => "JKFF";
    }

    internal class SETDInstruction : FixedFBDInstruction
    {
        public SETDInstruction(ParamInfoList infos) : base("SETD", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == DOMINANT_SET.Inst);
            addr.Accept(gen);
            var enableOut = DOMINANT_SET.Inst["EnableOut"];
            var enableIn = DOMINANT_SET.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("SETDTag", DOMINANT_SET.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => DOMINANT_SET.Inst["EnableIn"];
        protected override string InstrName => "SETD";
    }

    internal class RESDInstruction : FixedFBDInstruction
    {
        public RESDInstruction(ParamInfoList infos) : base("RESD", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == DOMINANT_RESET.Inst);
            addr.Accept(gen);
            var enableOut = DOMINANT_RESET.Inst["EnableOut"];
            var enableIn = DOMINANT_RESET.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("RESDTag", DOMINANT_RESET.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => DOMINANT_RESET.Inst["EnableIn"];
        protected override string InstrName => "RESD";
    }

    internal class MOVInstruction : IXInstruction
    {
        public MOVInstruction(ParamInfoList infos)
        {
            Name = "MOV";
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            Utils.ThrowNotImplemented("MOV");
            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return Utils.ParseExprList(parameters);
        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            var tmp = paramList.Accept(checker) as ASTNodeList;
            Debug.Assert(tmp != null);
            Debug.Assert(tmp.Count() == 2);

            var store = checker.GenStore(tmp.nodes[1] as ASTName, tmp.nodes[0] as ASTExpr);

            //这个是修改了对应的指令参数，会导致参数变少的
            var res = new ASTNodeList();
            res.AddNode(store);
            return res;
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(paramList.Count() == 1, paramList.Count().ToString());
            gen.masm().Dup();
            gen.masm().Dup();
            var label = new MacroAssembler.Label();
            gen.masm().JeqL(label);

            paramList.Accept(gen);

            gen.masm().Bind(label);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {

        }

        public override IDataType Type(ASTNodeList paramList)
        {
            return DINT.Inst;
        }

    }

    internal class MVMInstruction : RLLInstruction
    {
        public MVMInstruction(ParamInfoList infos) : base("MVM")
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList.Accept(checker) as ASTNodeList;
            res.nodes[2] = new ASTNameAddr(res.nodes[2] as ASTName);
            return res;
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            paramList.nodes[0].Accept(gen);
            paramList.nodes[1].Accept(gen);
            gen.masm().IAnd();
            var dest = paramList.nodes[2] as ASTNameAddr;
            dest.Accept(gen);
            gen.masm().Swap();
            gen.masm().Store(dest.ref_type.type, DINT.Inst);
        }
    }

    internal class ANDInstruction : FixedRLLInstruction
    {
        public ANDInstruction(ParamInfoList infos) : base("AND", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen, IDataType tp)
        {
            gen.masm().And(CodeGenVisitor.GetPrimitiveType(tp));
        }

    }

    internal class FANDInstruction : FixedFBDInstruction
    {
        public FANDInstruction(ParamInfoList infos) : base("AND", infos)
        {

        }

        protected override string InstrName => "FBDAND";

    }

    internal class ORInstruction : FixedRLLInstruction
    {
        public ORInstruction(ParamInfoList infos) : base("OR", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen, IDataType tp)
        {
            gen.masm().Or(CodeGenVisitor.GetPrimitiveType(tp));
        }

    }

    internal class F_ORInstruction : FixedFBDInstruction
    {
        public F_ORInstruction(ParamInfoList infos) : base("FOR", infos)
        {

        }

        protected override string InstrName => "FBDOR";
    }

    internal class XORInstruction : FixedRLLInstruction
    {
        public XORInstruction(ParamInfoList infos) : base("XOR", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen, IDataType type)
        {
            gen.masm().Xor(CodeGenVisitor.GetPrimitiveType(type));
        }
    }

    internal class FXORInstruction : FixedFBDInstruction
    {
        public FXORInstruction(ParamInfoList infos) : base("XOR", infos)
        {

        }

        protected override string InstrName => "FBDXOR";
    }

    internal class NOTInstruction : FixedRLLInstruction
    {
        public NOTInstruction(ParamInfoList infos) : base("NOT", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen, IDataType type)
        {
            if (type is BOOL)
            {
                gen.masm().BNot();
            }
            else if (type is DINT)
            {
                gen.masm().INot();
            }
            else if (type is LINT)
            {
                gen.masm().LNot();
            }
            else
            {
                Debug.Assert(false, type.ToString());
            }
        }
    }

    internal class FNOTInstruction : FixedFBDInstruction
    {
        public FNOTInstruction(ParamInfoList infos) : base("NOT", infos)
        {

        }

        protected override string InstrName => "FBDNOT";
    }

    internal class CLRInstruction : FixedRLLTrueInstruction
    {
        public CLRInstruction(ParamInfoList infos) : base("CLR", infos)
        {

        }

        protected override void Op(CodeGenVisitor gen, ASTNodeList paramList)
        {
            Debug.Assert(paramList.Count() == 1, paramList.ToString());
            paramList.Accept(gen);
            gen.masm().ConstI0();
            gen.masm().Store((paramList.nodes[0] as ASTNameAddr).ref_type.type, DINT.Inst);
        }
    }

    internal class BTDInstruction : RLLInstruction
    {
        public BTDInstruction(ParamInfoList infos) : base("BTD")
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList.Accept(checker) as ASTNodeList;
            res.nodes[2] = new ASTNameAddr(res.nodes[2] as ASTName);
            return res;
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            var source = paramList.nodes[0];
            var sourceBit = (paramList.nodes[1] as ASTInteger).value;
            var dest = (paramList.nodes[2]) as ASTNameAddr;
            var destBit = (paramList.nodes[3] as ASTInteger).value;
            var len = (paramList.nodes[4] as ASTInteger).value;

            source.Accept(gen);

            var multiBitOne = MultiBitOne((int) sourceBit, (int) len);
            gen.masm().CLoadInteger(BitConverter.ToInt32(BitConverter.GetBytes(multiBitOne), 0));
            gen.masm().And(CodeGenVisitor.GetPrimitiveType(DINT.Inst));
            
            if (sourceBit < destBit) 
            {
                for (int i = 0; i < destBit - sourceBit; i++)
                {
                    gen.masm().CLoadInteger(1);
                    gen.masm().IShiftL();
                }

            }else
            {
                for (int i = 0; i < sourceBit- destBit ; i++)
                {
                    gen.masm().CLoadInteger(1);
                    gen.masm().IShiftR();
                }
            };


            var multiBitZero = MultiBitZero((int) destBit, (int) len);
            dest.Accept(gen);
            gen.masm().Dup();
            gen.masm().Load(dest.ref_type.type);


            gen.masm().CLoadInteger(BitConverter.ToInt32(BitConverter.GetBytes(multiBitZero), 0));
            gen.masm().And(CodeGenVisitor.GetPrimitiveType(dest.ref_type.type));
            gen.masm().SwapX1();
            gen.masm().Or(CodeGenVisitor.GetPrimitiveType(dest.ref_type.type));
            gen.masm().Store(dest.ref_type.type, dest.ref_type.type);
  
        }

        private uint MultiBitZero(int start, int length)
        {
            Debug.Assert(start>=0&&start<=31);
            Debug.Assert(length>=0&&length<=32);

            uint bit = ~(uint)0;
            int end = start + length;
            bit = end < 32 ? bit << end : 0;

            return (bit | (((uint)1 << start) - 1));
        }

        private uint MultiBitOne(int start, int length)
        {
            Debug.Assert(start >= 0 && start <= 31);
            Debug.Assert(length >= 0 && length <= 32);
            uint bit = 0;
            bit = length < 32 ? bit | ((uint) 1 << length) - 1 : ~(uint) 0;
            return bit << start;
        }
    }

    internal class BANDInstruction : FixedFBDInstruction
    {
        public BANDInstruction(ParamInfoList infos) : base("BAND", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FBD_BOOLEAN_AND.Inst);
            addr.Accept(gen);
            var enableOut = FBD_BOOLEAN_AND.Inst["EnableOut"];
            var enableIn = FBD_BOOLEAN_AND.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        protected override string InstrName => "FBDBAND";

    }

    internal class BORInstruction : FixedFBDInstruction
    {
        public BORInstruction(ParamInfoList infos) : base("BOR", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FBD_BOOLEAN_OR.Inst);
            addr.Accept(gen);
            var enableOut = FBD_BOOLEAN_OR.Inst["EnableOut"];
            var enableIn = FBD_BOOLEAN_OR.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        protected override string InstrName => "FBDBOR";
    }

    internal class BXORInstruction : FixedFBDInstruction
    {
        public BXORInstruction(ParamInfoList infos) : base("BXOR", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FBD_BOOLEAN_XOR.Inst);
            addr.Accept(gen);
            var enableOut = FBD_BOOLEAN_XOR.Inst["EnableOut"];
            var enableIn = FBD_BOOLEAN_XOR.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }

        protected override string InstrName => "FBDBXOR";

    }

    internal class BNOTInstruction : FixedFBDInstruction
    {
        public BNOTInstruction(ParamInfoList infos) : base("BNOT", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FBD_BOOLEAN_NOT.Inst);
            addr.Accept(gen);
            var enableOut = FBD_BOOLEAN_NOT.Inst["EnableOut"];
            var enableIn = FBD_BOOLEAN_NOT.Inst["EnableIn"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            gen.masm().Pop();
        }


        protected override string InstrName => "FBDBNOT";

    }

}
