using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.PredefinedType;
using MessagePack.Formatters;

namespace ICSStudio.SimpleServices.Instruction.Instructions
{
    using ParamInfo = Tuple<Predicate<ASTExpr>, IDataType, ParameterType>;
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;

    internal class Impl
    {
        public static void SetBitInStack(CodeGenVisitor gen, ASTNameAddr name)
        {
            name.Accept(gen);
            gen.masm().SwapX1();
            gen.masm().Store(name.ref_type.type, BOOL.Inst);
            gen.masm().Dup();
        }
    }

    internal class OSRIInstruction : FixedFBDInstruction
    {
        public OSRIInstruction(ParamInfoList infos) : base("OSRI", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FBD_ONESHOT.Inst);
            addr.Accept(gen);
            var enableOut = FBD_ONESHOT.Inst["EnableOut"];
            var enableIn = FBD_ONESHOT.Inst["EnableIn"];
            var savedBit = FBD_ONESHOT.Inst["SavedBit"];
            var firstScan = FBD_ONESHOT.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenSetBit(gen.masm(), savedBit);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("OSRITag", FBD_ONESHOT.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => FBD_ONESHOT.Inst["EnableIn"];

        protected override string InstrName => "OSRI";
    }

    internal class OSFIInstruction : FixedFBDInstruction
    {
        public OSFIInstruction(ParamInfoList infos) : base("OSFI", infos)
        {

        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == FBD_ONESHOT.Inst);
            addr.Accept(gen);
            var enableOut = FBD_ONESHOT.Inst["EnableOut"];
            var enableIn = FBD_ONESHOT.Inst["EnableIn"];
            var savedBit = FBD_ONESHOT.Inst["SavedBit"];
            var firstScan = FBD_ONESHOT.Inst["FirstScan"];
            Utils.GenClearBit(gen.masm(), enableOut);
            Utils.GenClearBit(gen.masm(), enableIn);
            Utils.GenClearBit(gen.masm(), savedBit);
            Utils.GenSetBit(gen.masm(), firstScan);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("OSFITag", FBD_ONESHOT.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }

        protected override IDataTypeMember EnableInMember => FBD_ONESHOT.Inst["EnableIn"];

        protected override string InstrName => "OSFI";
    }

    internal class XICInstruction : FixedInstruction
    {
        public XICInstruction(ParamInfoList infos) : base("XIC", infos)
        {
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            gen.masm().Dup();
            var else_label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();
            gen.masm().JeqL(else_label);
            paramList.nodes[0].Accept(gen); // 这个会抛出异常
            gen.masm().JmpL(exit_label);
            gen.masm().Bind(else_label);
            gen.masm().BiPush(0);
            gen.masm().Bind(exit_label);

            //if else 都往里放了个东西
            gen.masm().stack_size -= 1;
        }
    }

    internal class XIOInstruction : FixedInstruction
    {
        public XIOInstruction(ParamInfoList infos) : base("XIO", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            gen.masm().Dup();
            var else_label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();
            gen.masm().JeqL(else_label);
            paramList.nodes[0].Accept(gen); //  这个会抛出异常
            gen.masm().BNot();
            gen.masm().JmpL(exit_label);
            gen.masm().Bind(else_label);
            gen.masm().BiPush(0);
            gen.masm().Bind(exit_label);
            //if else 都往里放了个东西
            gen.masm().stack_size -= 1;

        }
    }

    //prescan:The data bit is cleared to false
    internal class OTEInstruction : FixedInstruction
    {
        public OTEInstruction(ParamInfoList infos) : base("OTE", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {

            Debug.Assert(context == Context.RLL);
            Debug.Assert(paramList.Count() == 1, paramList.Count().ToString());
            gen.masm().Dup();

            Impl.SetBitInStack(gen, paramList.nodes[0] as ASTNameAddr);

            /*
            var before = gen.masm().stack_size;
            paramList.nodes[0].Accept(gen);
            var after = gen.masm().stack_size;
            //Debug.Assert(before + 2 == after, $"{before}:{after}");
            gen.masm().SwapX1();
            gen.masm().Store((paramList.nodes[0] as ASTNameAddr).ref_type.type, BOOL.Inst, (paramList.nodes[0] as ASTNameAddr).ref_type.bit_offset);
            gen.masm().Dup();
            */
        }
    }

    internal class OTLInstruction : FixedInstruction
    {
        public OTLInstruction(ParamInfoList infos) : base("OTL", infos)
        {
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            gen.masm().Dup();

            var label = new MacroAssembler.Label();
            gen.masm().JeqL(label);
            paramList.nodes[0].Accept(gen);
            gen.masm().BiPush(1);
            gen.masm().Store((paramList.nodes[0] as ASTNameAddr).ref_type.type, BOOL.Inst);

            gen.masm().Bind(label);
            gen.masm().Dup();

        }

    }

    internal class OTUInstruction : FixedInstruction
    {
        public OTUInstruction(ParamInfoList infos) : base("OTU", infos)
        {
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            gen.masm().Dup();

            var label = new MacroAssembler.Label();
            gen.masm().JeqL(label);
            paramList.nodes[0].Accept(gen);
            gen.masm().BiPush(0);
            gen.masm().Store((paramList.nodes[0] as ASTNameAddr).ref_type.type, BOOL.Inst);

            gen.masm().Bind(label);
            gen.masm().Dup();

        }
    }

    internal class ONSInstruction : FixedInstruction
    {
        public ONSInstruction(ParamInfoList infos) : base("ONS", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            //Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type == ALARM_DIGITAL.Inst);

            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type.IsBool);

            Debug.Assert(addr != null);
            var type = (addr.type as RefType);
            Debug.Assert(type != null);
            addr.Accept(gen);
            gen.masm().BiPush(1);
            gen.masm().Store(type.type, BOOL.DInst);
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);

            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null);
            Debug.Assert(addr.ref_type.type.IsBool, addr.type.ToString());

            gen.masm().Dup();
            addr.Accept(gen);
            var addr_id = (UInt16) gen.AllocateTempSlot();
            var offset_id = (UInt16) gen.AllocateTempSlot();
            gen.masm().StoreLocal(offset_id);
            gen.masm().StoreLocal(addr_id);

            var label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();

            gen.masm().JeqL(label);

            gen.masm().LoadLocal(addr_id);
            gen.masm().LoadLocal(offset_id);
            gen.masm().Load(addr.ref_type.type);

            var inner_label = new MacroAssembler.Label();

            gen.masm().JeqL(inner_label);

            gen.masm().BiPush(0);
            gen.masm().JmpL(exit_label);
            gen.masm().Bind(inner_label);

            gen.masm().LoadLocal(addr_id);
            gen.masm().LoadLocal(offset_id);
            gen.masm().BiPush(1);
            gen.masm().Store(addr.ref_type.type, BOOL.Inst);
            gen.masm().BiPush(1);

            gen.masm().JmpL(exit_label);


            gen.masm().Bind(label);
            gen.masm().LoadLocal(addr_id);
            gen.masm().LoadLocal(offset_id);
            gen.masm().BiPush(0);
            gen.masm().Store(addr.ref_type.type, BOOL.Inst);
            gen.masm().BiPush(0);
            gen.masm().Bind(exit_label);

            //这里有三个分支都放了一个数据在栈上
            gen.masm().stack_size -= 2;

        }
    }

    internal class OSFInstruction : FixedInstruction
    {
        public OSFInstruction(ParamInfoList infos) : base("OSF", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            for (int i = 0; i < 2; i++)
            {
                var addr = paramList.nodes[i] as ASTNameAddr;
                Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type.IsBool);

                Debug.Assert(addr != null);
                var type = (addr.type as RefType);
                Debug.Assert(type != null);
                addr.Accept(gen);
                gen.masm().BiPush(0);
                gen.masm().Store(BOOL.Inst, BOOL.Inst);
            }

        }

        static private void GenTrueIn(CodeGenVisitor gen, ASTNameAddr storage, ASTNameAddr output)
        {
            storage.Accept(gen);
            gen.masm().BiPush(1);
            gen.masm().Store(storage.ref_type.type, BOOL.Inst);

            output.Accept(gen);
            gen.masm().BiPush(0);
            gen.masm().Store(output.ref_type.type, BOOL.Inst);
        }

        static private void GenFalseIn(CodeGenVisitor gen, ASTNameAddr storage, ASTNameAddr output,
            MacroAssembler.Label exit_label)
        {
            storage.Accept(gen);
            var addr_id = gen.AllocateTempSlot();
            var offset_id = gen.AllocateTempSlot();
            gen.masm().StoreLocal(offset_id);
            gen.masm().StoreLocal(addr_id);

            gen.masm().LoadLocal(addr_id);
            gen.masm().LoadLocal(offset_id);
            var inner_label = new MacroAssembler.Label();
            gen.masm().Load(storage.ref_type.type);
            gen.masm().JeqL(inner_label);

            gen.masm().LoadLocal(addr_id);
            gen.masm().LoadLocal(offset_id);
            gen.masm().BiPush(0);
            gen.masm().Store(storage.ref_type.type, BOOL.Inst);

            output.Accept(gen);
            gen.masm().BiPush(1);
            gen.masm().Store(output.ref_type.type, BOOL.Inst);

            gen.masm().JmpL(exit_label);
            gen.masm().Bind(inner_label);


            output.Accept(gen);
            gen.masm().BiPush(0);
            gen.masm().Store(output.ref_type.type, BOOL.Inst);


        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);

            var storage = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(storage != null);
            var output = paramList.nodes[1] as ASTNameAddr;
            Debug.Assert(output != null);

            var label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();

            gen.masm().Dup();
            gen.masm().JeqL(label);
            GenTrueIn(gen, storage, output);
            gen.masm().JmpL(exit_label);
            gen.masm().Bind(label);

            GenFalseIn(gen, storage, output, exit_label);

            gen.masm().Bind(exit_label);

            gen.masm().Dup();

        }
    }

    internal class OSRInstruction : FixedInstruction
    {
        public OSRInstruction(ParamInfoList infos) : base("OSR", infos)
        {
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            var addr = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type.IsBool);

            Debug.Assert(addr != null);
            var type = (addr.type as RefType);
            Debug.Assert(type != null);
            addr.Accept(gen);
            gen.masm().BiPush(1);
            gen.masm().Store(BOOL.Inst, BOOL.Inst);

            addr = paramList.nodes[1] as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType).type.IsBool);

            Debug.Assert(addr != null);
            type = (addr.type as RefType);
            Debug.Assert(type != null);
            addr.Accept(gen);
            gen.masm().BiPush(0);
            gen.masm().Store(BOOL.Inst, BOOL.Inst);

        }

        static private void GenFalseIn(CodeGenVisitor gen, ASTNameAddr storage, ASTNameAddr output)
        {
            storage.Accept(gen);
            gen.masm().BiPush(0);
            //gen.masm().Swap();
            gen.masm().Store(storage.ref_type.type, BOOL.Inst);

            output.Accept(gen);
            gen.masm().BiPush(0);
            //gen.masm().Swap();
            gen.masm().Store(output.ref_type.type, BOOL.Inst);
        }

        static private void GenTrueIn(CodeGenVisitor gen, ASTNameAddr storage, ASTNameAddr output,
            MacroAssembler.Label exit_label)
        {
            storage.Accept(gen);

            var addr_id = gen.AllocateTempSlot();
            var offset_id = gen.AllocateTempSlot();
            gen.masm().StoreLocal(offset_id);
            gen.masm().StoreLocal(addr_id);

            gen.masm().LoadLocal(addr_id);
            gen.masm().LoadLocal(offset_id);
            var inner_label = new MacroAssembler.Label();
            gen.masm().Load(storage.ref_type.type);
            gen.masm().JneL(inner_label);

            gen.masm().LoadLocal(addr_id);
            gen.masm().LoadLocal(offset_id);
            gen.masm().BiPush(1);
            gen.masm().Store(storage.ref_type.type, BOOL.Inst);

            output.Accept(gen);
            gen.masm().BiPush(1);
            gen.masm().Store(output.ref_type.type, BOOL.Inst);

            gen.masm().JmpL(exit_label);
            gen.masm().Bind(inner_label);


            output.Accept(gen);
            gen.masm().BiPush(0);
            gen.masm().Store(output.ref_type.type, BOOL.Inst);


        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);

            var storage = paramList.nodes[0] as ASTNameAddr;
            Debug.Assert(storage != null);
            var output = paramList.nodes[1] as ASTNameAddr;
            Debug.Assert(output != null);

            var label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();

            gen.masm().Dup();
            gen.masm().JeqL(label);
            GenTrueIn(gen, storage, output, exit_label);
            gen.masm().JmpL(exit_label);
            gen.masm().Bind(label);

            GenFalseIn(gen, storage, output);

            gen.masm().Bind(exit_label);

            gen.masm().Dup();

        }
    }
}
