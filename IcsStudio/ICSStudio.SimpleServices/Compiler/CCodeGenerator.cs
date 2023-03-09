using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.Instruction.Instructions;
using ICSStudio.SimpleServices.PredefinedType;
using MessagePack.Formatters;
using ICSStudio.SimpleServices.Common;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.SimpleServices.Compiler
{
    /*
    enum SlotDataType
    {
        UNKNOWN,
        INT32,
        INT64,
        FLOAT,
        DOUBLE,
        POINTER,
    }
    */

    internal class BasicBlock
    {
        public BasicBlock(int pos)
        {
            this.Pos = pos;
        }

        public int Pos { get; }
  
        public int BeginStackSize { get; set; }
        public int CurrStackSize { get; set; }

        public const int StackSizeDefault = Int32.MinValue;

        public Stack<SlotDataType> stack { get; set; } = new Stack<SlotDataType>();
        public Stack<SlotDataType> pre_stack { get; set; }
        public bool is_visited { set; get; } = false;
        public bool IsReachable { get; set; } = false;
        public int SeqNo { get; set; }
    }


    class CBasicBlockBuilder
    {

        public CBasicBlockBuilder(Function func, List<MacroAssembler.IConst> consts)
        {
            Initialize(func.Codes, func.LocalsSize, consts);
        }

        private void Initialize(List<byte> codes, int locals_size, List<MacroAssembler.IConst> consts)
        {
            codes_ = codes;
            locals_ = Enumerable.Repeat(SlotDataType.UNKNOWN, locals_size).ToList();
            consts_ = ConvertConstSlotType(consts);

            pool_ = consts;

            blocks_ = new List<BasicBlock>(codes.Count);
            for (int i = 0; i < codes.Count; ++i)
            {
                blocks_.Add(null);
            }
        }

        public void BuildBlocks()
        {
            MarkBasicBlock();
        }

        private static void AssertTopAndPop(Stack<SlotDataType> stack, SlotDataType type)
        {
            var tmp = stack.Pop();
            if (tmp != type)
            {
                Console.WriteLine($"fffffff:{tmp}");
                while (stack.Count != 0)
                {
                    Console.WriteLine($"xxxxxxxx:{stack.Pop()}");
                }

                Debug.Assert(false, $"need:{type.ToString()}:real:{tmp.ToString()}");

            }

            //Debug.Assert(tmp == type, $"{type.ToString()}:{tmp.ToString()}");
        }

        private static void AssertTop(Stack<SlotDataType> stack, SlotDataType type)
        {
            var tmp = stack.Peek();
            Debug.Assert(tmp == type, $"require:{type.ToString()},real:{tmp.ToString()}");
        }

        public void MarkBasicBlock()
        {
            MakeBlockAt(0);
            foreach (var tuple in Utils.GetByteCode(codes_))
            {
                var code = tuple.Item1;
                var args = tuple.Item2;
                var pos = tuple.Item3;
                switch (code)
                {
                    case Opcode.JEQ:
                    case Opcode.JNE:
                    case Opcode.JGE:
                    case Opcode.JGT:
                    case Opcode.JLE:
                    case Opcode.JLT:

                        MakeBlockAt(pos + 1 + args.Count);
                        MakeBlockAt(Dest(pos));
                        break;
                    case Opcode.JMP:
                        MakeBlockAt(Dest(pos));
                         break;
                    case Opcode.RET:
                        if (pos + 2 != codes_.Count)
                        {
                           MakeBlockAt(pos + 2);
                        }

                        break;
                }
            }

            int numBlocks = 0;
            foreach (var b in blocks_)
            {
                if (b == null)
                    continue;
                b.SeqNo = numBlocks;
                numBlocks++;
            }
        }

        public void GenerateStackInfo()
        {
            blocks_[0].BeginStackSize = 0;
            work_list_.Enqueue(blocks_[0]);

            while (work_list_.Count != 0)
            {
                var block = work_list_.Dequeue();
                if (block.is_visited)
                {
                    continue;
                }

                IteratorByteCodesForBlock(block);
                block.is_visited = true;
                block.IsReachable = true;
            }
        }

        void IteratorByteCodesForBlock(BasicBlock block)
        {
            int stackSize = block.BeginStackSize;
            var pos = block.Pos;
            while (pos != codes_.Count && (blocks_[pos] == null || blocks_[pos] == block))
            {
                var code = codes_[pos];
                stackSize += GetStackNetValue(code, pos);
                MaxStackSize = Math.Max(stackSize, MaxStackSize);
                switch (code)
                {
                    case Opcode.JEQ:
                    case Opcode.JNE:
                    case Opcode.JGE:
                    case Opcode.JGT:
                    case Opcode.JLE:
                    case Opcode.JLT:
                    case Opcode.JMP:
                        AddBlockToWorkList(blocks_[Dest(pos)], stackSize);
                        break;
                    case Opcode.RET:
                       break;
                }

                pos += (1 + Opcode.ARG_SIZE[code]);
                if (pos <= blocks_.Count - 1 && blocks_[pos] != null && code != Opcode.JMP && code != Opcode.RET)
                {
                    AddBlockToWorkList(blocks_[pos], stackSize);
                }
            }
        }

        private void AddBlockToWorkList(BasicBlock block, int stackSize)
        {
            if (block.is_visited)
            {
                Debug.Assert(block.BeginStackSize == stackSize, $"{stackSize}:{block.BeginStackSize}");
                return;
            }

            block.BeginStackSize = stackSize; 
            work_list_.Enqueue(block);
        }

        private int GetArg(List<byte> args)
        {
            switch (args.Count)
            {
                case 1:
                    return args[0];
                case 2:
                    return BitConverter.ToInt16(new byte[] {args[0], args[1]}, 0);
                case 4:
                    return BitConverter.ToInt32(new byte[] {args[0], args[1], args[2], args[3]}, 0);
                default:
                    //Debug.Assert(false, args.Count.ToString());
                    return 0;
            }
        }

        private int Dest(int pos)
        {
            var code = codes_[pos];
            Debug.Assert(
                code == Opcode.JEQ || code == Opcode.JNE || code == Opcode.JGE || code == Opcode.JGT ||
                code == Opcode.JLE || code == Opcode.JLT || code == Opcode.JMP, code.ToString());

            var offset =
                BitConverter.ToInt32(new byte[] {codes_[pos + 1], codes_[pos + 2], codes_[pos + 3], codes_[pos + 4]},
                    0);
            return pos + offset;
        }

        private BasicBlock MakeBlockAt(int pos)
        {
            if (blocks_[pos] == null)
            {
                blocks_[pos] = new BasicBlock(pos);
            }

            var block = blocks_[pos];
            return block;
        }

        private static SlotDataType ToSlotDataType(MacroAssembler.ConstantPoolType tp)
        {

            switch (tp)
            {
                case MacroAssembler.ConstantPoolType.INTEGER:
                    return SlotDataType.INT32;
                case MacroAssembler.ConstantPoolType.INTEGER64:
                    return SlotDataType.INT64;
                case MacroAssembler.ConstantPoolType.FLOAT:
                    return SlotDataType.FLOAT;
                case MacroAssembler.ConstantPoolType.TAG:
                case MacroAssembler.ConstantPoolType.FUNCTION:
                case MacroAssembler.ConstantPoolType.STRING:
                case MacroAssembler.ConstantPoolType.ROUTINE:
                case MacroAssembler.ConstantPoolType.TASK:
                    return SlotDataType.POINTER;
                default:
                    throw new NotImplementedException();
            }
        }

        private static List<SlotDataType> ConvertConstSlotType(List<MacroAssembler.IConst> consts)
        {
            var res = new List<SlotDataType>();

            foreach (var c in consts)
            {
                res.Add(ToSlotDataType(c.type()));
            }

            return res;
        }

        public BasicBlock GetBlockAt(int i)
        {
            return blocks_[i];
        }

        public int GetStackNetValue(byte code, int pos)
        {
            switch (code)
            {
                case Opcode.NOP:
                    return 0;
                case Opcode.BIPUSH:
                    return 1;
                case Opcode.SIPUSH:
                    return 2;
                case Opcode.POP:
                    return -1;
                case Opcode.DUP:
                    return 1;
                case Opcode.CLOAD:
                    return 1;
                case Opcode.LOAD_LOCAL:
                    return 1;
                case Opcode.STORE_LOCAL:
                    return -1;
                case Opcode.LOAD_INT8_BIT:
                case Opcode.LOAD_INT16_BIT:
                case Opcode.LOAD_INT32_BIT:
                case Opcode.LOAD_INT64_BIT:
                    return -1;
                case Opcode.LOAD_INT8:
                case Opcode.LOAD_INT16:
                case Opcode.LOAD_INT32:
                case Opcode.LOAD_INT64:
                case Opcode.LOAD_FLOAT:
                case Opcode.LOAD_DOUBLE:
                    return 0;
                case Opcode.STORE_INT8_BIT:
                case Opcode.STORE_INT16_BIT:
                case Opcode.STORE_INT32_BIT:
                case Opcode.STORE_INT64_BIT:
                    return -3;
                case Opcode.STORE_INT8:
                case Opcode.STORE_INT16:
                case Opcode.STORE_INT32:
                case Opcode.STORE_INT64:
                case Opcode.STORE_FLOAT:
                case Opcode.STORE_DOUBLE:
                    return -2;
                case Opcode.PBINC:
                case Opcode.PSINC:
                    return 0;
                case Opcode.PADD:
                case Opcode.IADD:
                case Opcode.LADD:
                case Opcode.FADD:
                case Opcode.DADD:
                case Opcode.ISUB:
                case Opcode.LSUB:
                case Opcode.FSUB:
                case Opcode.DSUB:
                case Opcode.IMUL:
                case Opcode.LMUL:
                case Opcode.FMUL:
                case Opcode.DMUL:
                case Opcode.IDIV:
                case Opcode.LDIV:
                case Opcode.FDIV:
                case Opcode.DDIV:
                case Opcode.IMOD:
                case Opcode.FMOD:
                case Opcode.ICMP:
                case Opcode.LMOD:
                case Opcode.DMOD:
                case Opcode.LCMP:
                case Opcode.FCMP:
                case Opcode.DCMP:
                case Opcode.IAND:
                case Opcode.LAND:
                case Opcode.IOR:
                case Opcode.LOR:
                case Opcode.LSHL:
                case Opcode.LSHR:
                case Opcode.ISHL:
                case Opcode.ISHR:
                    return -1;
                case Opcode.INOT:
                case Opcode.BNOT:
                case Opcode.LNOT:
                    return 0;
                case Opcode.IXOR:
                case Opcode.LXOR:
                    return -1;
                case Opcode.INEG:
                case Opcode.LNEG:
                case Opcode.FNEG:
                case Opcode.DNEG:
                case Opcode.I2B:
                case Opcode.I2S:
                case Opcode.B2I:
                case Opcode.S2I:
                case Opcode.I2L:
                case Opcode.I2F:
                case Opcode.I2D:
                case Opcode.L2I:
                case Opcode.L2F:
                case Opcode.L2D:
                case Opcode.F2I:
                case Opcode.F2L:
                case Opcode.F2D:
                case Opcode.D2I:
                case Opcode.D2L:
                case Opcode.D2F:
                    return 0;
                case Opcode.CALL:
                    var index = BitConverter.ToUInt16(codes_.GetRange(pos + 1, 2).ToArray(), 0);
                    var name = pool_[(int)index];
                    if (name is MacroAssembler.RoutineConst)
                    {
                        return 1;
                    }
                    else if (name is MacroAssembler.FunctionConst)
                    {
                        var func_name = (name as MacroAssembler.FunctionConst).value;
                        var tup = Utils.GetFunctionInfo(func_name);

                        if (func_name == "SYSLOAD")
                        {
                            return 0;

                        }
                        else
                        {
                            return 1 - tup.Item1.Count;
                        }

                    }
                    else
                    {

                        Debug.Assert(false, $"{name.ToString()}:{name.GetType().ToString()}");
                    }
                    return 0;
                case Opcode.RET:
                    return 0;
                case Opcode.JEQ:
                case Opcode.JNE:
                case Opcode.JGE:
                case Opcode.JGT:
                case Opcode.JLE:
                case Opcode.JLT:
                    return -1;
                case Opcode.JMP:
                    return 0;
                case Opcode.THROW:
                    return -2;
                case Opcode.DUP_X1:
                    return 1;
                case Opcode.SWAP:
                    return 0;
                case Opcode.CONST_PNULL:
                case Opcode.CONST_I0:
                case Opcode.CONST_I1:
                case Opcode.CONST_L0:
                case Opcode.CONST_L1:
                case Opcode.CONST_F0:
                case Opcode.CONST_F1:
                case Opcode.CONST_D0:
                case Opcode.CONST_D1:
                case Opcode.CONST_M1:
                    return 1;
                case Opcode.CHECK:
                    return -1;
                case Opcode.SWAP_X1:
                    return 0;
                case Opcode.ECHECK:
                    return 0;
                default:
                    Debug.Assert(false);
                    return 0;
            }
        }

        private Queue<BasicBlock> work_list_ = new Queue<BasicBlock>();

        private List<byte> codes_;
        private List<BasicBlock> blocks_;
        private List<SlotDataType> locals_;
        private List<SlotDataType> consts_;
        private List<MacroAssembler.IConst> pool_;
        public int MaxStackSize { get; private set; }

    }

    public class CCodeGeneratorContext
    {
        public string ProgramName { get; }
        public HashSet<string> ProgramTags { get; }

        public CCodeGeneratorContext(string name, HashSet<string> programTags)
        {
            ProgramName = name;
            ProgramTags = programTags;
        }
      
    }

    public class CCodeGenerator
    {
        public CCodeGenerator(CCodeGeneratorContext context, Function func, List<MacroAssembler.IConst> consts, OutputStream writer)
        {
            this._context = context;
            this._func = func;
            this._consts = consts;
            this._output = writer;
        }

        public void GenCode(string name)
        {
            EmitTags();

            var builder = new CBasicBlockBuilder(_func, _consts);

            builder.MarkBasicBlock();
            builder.GenerateStackInfo();

            EmitPrologue(name);
            EmitStack(builder.MaxStackSize);
            EmitLocals(_func.LocalsSize);
            EmitConsts();

            BasicBlock block = null;

            foreach (var tuple in Utils.GetByteCode(_func.Codes))
            {
                var code = tuple.Item1;
                var args = tuple.Item2;
                var pos = tuple.Item3;
                var b = builder.GetBlockAt(pos);
                if (b != null)
                {
                    Debug.Assert(b.BeginStackSize != BasicBlock.StackSizeDefault, $"{b.BeginStackSize}");
                    EmitBlockBegin(b.SeqNo);
                    block = b;
                    b.CurrStackSize = b.BeginStackSize;
                }

                if (block.IsReachable)
                {
                    ProcessByteCode(builder, block, pos, code, args);
                }

                pos += (1 + Opcode.ARG_SIZE[code]);

            }

            EmitEpilogue(name);

        }

        private static string ConvertModuleTagName(string name)
        {
            var tmp = Encoding.Default.GetBytes(name);
            var hex = BitConverter.ToString(tmp);
            return hex.Replace("-", "");

        }

        private string TagName(string name)
        {
            if (!_context.ProgramTags.Contains(name) && name.Contains(":"))
            {
                return $"_I{ConvertModuleTagName(name)}";
            } else if (name.StartsWith("\\"))
            {
                var fullname = name.Substring(1);
                var splitName = fullname.Split('.');
                Debug.Assert(splitName.Length == 2, name.ToString());
                return $"_T{splitName[0].Count()}{splitName[0]}{splitName[1].Length}{splitName[1].ToString()}";
            }
            else if (!_context.ProgramTags.Contains(name))
            {

                return $"_T{name}";

            }
            else
            {
                return $"_T{_context.ProgramName.Length}{_context.ProgramName}{name.Length}{name.ToString()}";
            }
        }
        private void EmitTags()
        {
            foreach (var c in _consts)
            {
                if (c.type() == MacroAssembler.ConstantPoolType.TAG)
                {
                    _output.WriteLine($"extern char {TagName(c.ToString().ToUpper())};");
                }
            }

        }

        private void EmitConsts()
        {
            if (_consts.Count == 0)
                return;
            _output.WriteLine("static const union Operand consts[] = {");
            foreach (var c in _consts)
            {
                switch (c.type())
                {
                    case MacroAssembler.ConstantPoolType.TAG:
                        _output.WriteLine($"{{ .pvalue = &{TagName(c.ToString().ToUpper())}  }},");
                        break;
                    case MacroAssembler.ConstantPoolType.INTEGER:
                        _output.WriteLine($"{{ .ivalue = {c.ToCString()}  }},");
                        break;
                    case MacroAssembler.ConstantPoolType.INTEGER64:
                        _output.WriteLine($"{{ .lvalue = {c.ToCString()}  }},");
                        break;
                    case MacroAssembler.ConstantPoolType.FLOAT:
                        _output.WriteLine($"{{ .fvalue = {c.ToCString()}  }},");
                        break;
                    case MacroAssembler.ConstantPoolType.STRING:
                        _output.WriteLine($"{{ .pvalue = \"{c.ToCString().ToUpper()}\"  }},");
                        break;
                    case MacroAssembler.ConstantPoolType.FUNCTION:
                        _output.WriteLine($"{{ .pvalue = 0 }},");
                        break;
                    case MacroAssembler.ConstantPoolType.ROUTINE:
                        _output.WriteLine($"{{ .pvalue = 0 }},");
                        break;
                    case MacroAssembler.ConstantPoolType.TASK:
                    default:
                        Debug.Assert(false, c.type().ToString());
                        break;
                }

            }
            _output.WriteLine("};");

        }

        void EmitPrologue(string name)
        {
            if (_context == null)
            {
                //这是AOI
                List<string> parameters = new List<string>();
                for (int i = 0; i < _func.ArgsSize; ++i)
                {
                    if (_func.ArgInfos[i].IsRef)
                    {
                        parameters.Add($"void *arg{i}");
                    }
                    else
                    {
                        //先強制都是DINT
                        Debug.Assert(_func.ArgInfos[i].Type == DINT.Inst);
                        parameters.Add($"int32_t arg{i}");
                    }
                }

                _output.WriteLine($"int32_t {EncodeName("_A", name).ToUpper()}({string.Join(",", parameters)})");

            }
            else
            {
                _output.WriteLine($"int32_t {EncodeName("_F", _context.ProgramName, name).ToUpper()}()");
            }

            _output.WriteLine("{");
            _output.WriteLine("ivm_builtin_stack_check();");


        }

        void EmitStack(int maxStackSize)
        {
            if (maxStackSize != 0)
            _output.WriteLine($"union Operand stack[{maxStackSize}];");
        }

        void EmitLocals(int num)
        {
            if (num != 0)
            {
                _output.WriteLine($"union Operand locals[{num}];");
            }

            if (_context == null)
            {
                for (int i = 0; i < _func.ArgsSize; ++i)
                {
                    if (_func.ArgInfos[i].IsRef)
                    {
                        _output.WriteLine($"locals[{i}].pvalue = arg{i};");
                    }
                    else
                    {
                        //先強制都是DINT
                        Debug.Assert(_func.ArgInfos[i].Type == DINT.Inst);
                        _output.WriteLine($"locals[{i}].ivalue = arg{i};");
                    }
                }
            }
        }

        void EmitEpilogue(string name)
        {
            _output.WriteLine("}");
            if (_context == null)
            {
                _output.WriteLine($"EXPORT_SYMBOL({EncodeName("_A", name).ToUpper()});");
            }
            else
            {

                _output.WriteLine($"EXPORT_SYMBOL({EncodeName("_F", _context.ProgramName, name).ToUpper()});");
            }


        }

        void EmitBlockBegin(int num)
        {
            _output.WriteLine($"L{num}: ;");
        }

        private void ProcessByteCode(CBasicBlockBuilder builder, BasicBlock block, int pos, byte code, List<byte> args)
        {
            uint index = 0;
            Int32 offset = 0;
            switch (code)
            {
                case Opcode.NOP:
                    break;
                case Opcode.BIPUSH:
                    Debug.Assert(args.Count == 1, args.Count.ToString());
                    _output.WriteLine($"stack[{block.CurrStackSize}].ivalue = {args[0]};");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.SIPUSH:
                    Debug.Assert(args.Count==2, args.Count.ToString());
                    _output.WriteLine($"stack[{block.CurrStackSize}].ivalue = {BitConverter.ToInt16(args.ToArray(), 0)};");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.POP:
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.DUP:
                    _output.WriteLine($"stack[{block.CurrStackSize}] = stack[{block.CurrStackSize - 1}];");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.CLOAD:
                    Debug.Assert(args.Count == 4, $"{args.Count}");
                    index = BitConverter.ToUInt32(args.ToArray(), 0);
                    _output.WriteLine($"stack[{block.CurrStackSize}] = consts[{index}];");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.LOAD_LOCAL:
                    Debug.Assert(args.Count == 2, $"{args.Count}");
                    index = BitConverter.ToUInt16(args.ToArray(), 0);
                    _output.WriteLine($"stack[{block.CurrStackSize}] = locals[{index}];");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.STORE_LOCAL:
                    Debug.Assert(args.Count==2, $"{args.Count}");
                    index = BitConverter.ToUInt16(args.ToArray(), 0);
                    _output.WriteLine($"locals[{index}]=stack[{block.CurrStackSize - 1}];");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LOAD_INT8_BIT:
                    _output.WriteLine("{");
                    _output.WriteLine($"int8_t *ptr = stack[{block.CurrStackSize - 2}].pvalue;");
                    _output.WriteLine($"uint8_t offset = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = ivm_load_int8_bit(ptr, offset);");           
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LOAD_INT16_BIT:
                    _output.WriteLine("{");
                    _output.WriteLine($"int16_t *ptr = stack[{block.CurrStackSize - 2}].pvalue;");
                    _output.WriteLine($"uint8_t offset = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = ivm_load_int16_bit(ptr, offset);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LOAD_INT32_BIT:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t *ptr = stack[{block.CurrStackSize - 2}].pvalue;");
                    _output.WriteLine($"uint8_t offset = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = ivm_load_int32_bit(ptr, offset);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LOAD_INT64_BIT:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t *ptr = stack[{block.CurrStackSize - 2}].pvalue;");
                    _output.WriteLine($"uint8_t offset = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = ivm_load_int64_bit(ptr, offset);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LOAD_INT8:
                    _output.WriteLine("{");
                    _output.WriteLine($"int8_t *ptr = stack[{block.CurrStackSize - 1}].pvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].ivalue = ivm_load_int8(ptr);");
                    _output.WriteLine("}");
                    break;
                case Opcode.LOAD_INT16:
                    _output.WriteLine("{");
                    _output.WriteLine($"int16_t *ptr = stack[{block.CurrStackSize - 1}].pvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].ivalue = ivm_load_int16(ptr);");
                    _output.WriteLine("}");
                    break;
                case Opcode.LOAD_INT32:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t *ptr = stack[{block.CurrStackSize - 1}].pvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].ivalue =  ivm_load_int32(ptr);");
                    _output.WriteLine("}");
                    break;
                case Opcode.LOAD_INT64:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t *ptr = stack[{block.CurrStackSize - 1}].pvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].lvalue =ivm_load_int64(ptr);");
                    _output.WriteLine("}");
                    break;
                case Opcode.LOAD_FLOAT:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t *ptr = stack[{block.CurrStackSize - 1}].pvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].fvalue = ivm_load_float((float*)ptr);");
                    _output.WriteLine("}");
                    break;
                case Opcode.LOAD_DOUBLE:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t *ptr = stack[{block.CurrStackSize - 1}].pvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].dvalue = ivm_load_double((double*)ptr);");
                    _output.WriteLine("}");
                    break;
                case Opcode.STORE_INT8_BIT:
                    _output.WriteLine("{");
                    _output.WriteLine($"int8_t *ptr = stack[{block.CurrStackSize - 3}].pvalue;");
                    _output.WriteLine($"int32_t offset = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"int32_t value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"ivm_store_int8_bit(ptr, offset, value);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 3;
                    break;
                case Opcode.STORE_INT16_BIT:
                    _output.WriteLine("{");
                    _output.WriteLine($"int16_t *ptr = stack[{block.CurrStackSize - 3}].pvalue;");
                    _output.WriteLine($"int32_t offset = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"int32_t value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"ivm_store_int16_bit(ptr, offset, value);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 3;
                    break;
                case Opcode.STORE_INT32_BIT:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t *ptr = stack[{block.CurrStackSize - 3}].pvalue;");
                    _output.WriteLine($"int32_t offset = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"int32_t value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"ivm_store_int32_bit(ptr, offset, value);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 3;
                    break;
                case Opcode.STORE_INT64_BIT:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t *ptr = stack[{block.CurrStackSize - 3}].pvalue;");
                    _output.WriteLine($"int32_t offset = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"int32_t value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"ivm_store_int64_bit(ptr, offset, value);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 3;
                    break;
                case Opcode.STORE_INT8:
                    _output.WriteLine("{");
                    _output.WriteLine($"int8_t  value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int8_t *ptr = stack[{block.CurrStackSize - 2}].pvalue;");
                    _output.WriteLine($"ivm_store_int8(ptr, value);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 2;
                    break;
                case Opcode.STORE_INT16:
                    _output.WriteLine("{");
                    _output.WriteLine($"int16_t  value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int16_t *ptr = stack[{block.CurrStackSize - 2}].pvalue;");
                    _output.WriteLine($"ivm_store_int16(ptr, value);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 2;
                    break;
                case Opcode.STORE_INT32:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t  value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int32_t *ptr = stack[{block.CurrStackSize - 2}].pvalue;");
                    _output.WriteLine($"ivm_store_int32(ptr, value);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 2;
                    break;
                case Opcode.STORE_INT64:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t value = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"int64_t *ptr = stack[{block.CurrStackSize - 2}].pvalue;");
                    _output.WriteLine($"ivm_store_int64(ptr, value);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 2;
                    break;
                case Opcode.STORE_FLOAT:
                    _output.WriteLine("{");
                    _output.WriteLine($"float value = stack[{block.CurrStackSize - 1}].fvalue;");
                    _output.WriteLine($"float *ptr = stack[{block.CurrStackSize - 2}].pvalue;");
                    _output.WriteLine($"ivm_store_float(ptr, value);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 2;
                    break;
                case Opcode.STORE_DOUBLE:
                    _output.WriteLine("{");
                    _output.WriteLine($"double value = stack[{block.CurrStackSize - 1}].dvalue;");
                    _output.WriteLine($"double *ptr = stack[{block.CurrStackSize - 2}].pvalue;");
                    _output.WriteLine($"ivm_store_double(ptr, value);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 2;
                    break;
                case Opcode.PBINC:
                    Debug.Assert(args.Count == 1, args.Count.ToString());
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].pvalue = ivem_pbinc(stack[{block.CurrStackSize - 1}].pvalue, {args[0]});");
                    break;
                case Opcode.PSINC:
                    Debug.Assert(args.Count == 2, args.Count.ToString());
                    offset = BitConverter.ToInt16(args.ToArray(), 0);
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].pvalue = ivem_psinc(stack[{block.CurrStackSize - 1}].pvalue, {offset});");
                    break;
                case Opcode.PADD:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t rhs = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].pvalue += rhs;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.IADD:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t lhs = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"int32_t rhs = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int32_t res = ivm_iadd(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.ISUB:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t lhs = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"int32_t rhs = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int32_t res = ivm_isub(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.IMUL:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t lhs = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"int32_t rhs = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int32_t res = ivm_imul(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.IDIV:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t lhs = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"int32_t rhs = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int32_t res = ivm_idiv(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.IMOD:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t lhs = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"int32_t rhs = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int32_t res = ivm_imod(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.IOR:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t lhs = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"int32_t rhs = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int32_t res = ivm_ior(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.IXOR:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t lhs = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"int32_t rhs = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int32_t res = ivm_ixor(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.IAND:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t lhs = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"int32_t rhs = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int32_t res = ivm_iand(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LADD:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t lhs = stack[{block.CurrStackSize - 2}].lvalue;");
                    _output.WriteLine($"int64_t rhs = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"int64_t res = ivm_ladd(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].lvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LSUB:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t lhs = stack[{block.CurrStackSize - 2}].lvalue;");
                    _output.WriteLine($"int64_t rhs = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"int64_t res = ivm_lsub(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].lvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LMUL:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t lhs = stack[{block.CurrStackSize - 2}].lvalue;");
                    _output.WriteLine($"int64_t rhs = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"int64_t res = ivm_lmul(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].lvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LDIV:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t lhs = stack[{block.CurrStackSize - 2}].lvalue;");
                    _output.WriteLine($"int64_t rhs = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"int64_t res = ivm_ldiv(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].lvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LMOD:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t lhs = stack[{block.CurrStackSize - 2}].lvalue;");
                    _output.WriteLine($"int64_t rhs = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"int64_t res = ivm_lmod(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].lvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LOR:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t lhs = stack[{block.CurrStackSize - 2}].lvalue;");
                    _output.WriteLine($"int64_t rhs = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"int64_t res = ivm_lor(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].lvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LXOR:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t lhs = stack[{block.CurrStackSize - 2}].lvalue;");
                    _output.WriteLine($"int64_t rhs = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"int64_t res = ivm_lxor(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].lvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LAND:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t lhs = stack[{block.CurrStackSize - 2}].lvalue;");
                    _output.WriteLine($"int64_t rhs = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"int64_t res = ivm_land(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].lvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.FADD:
                    _output.WriteLine("{");
                    _output.WriteLine($"float lhs = stack[{block.CurrStackSize - 2}].fvalue;");
                    _output.WriteLine($"float rhs = stack[{block.CurrStackSize - 1}].fvalue;");
                    _output.WriteLine($"float res = ivm_fadd(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].fvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.FSUB:
                    _output.WriteLine("{");
                    _output.WriteLine($"float lhs = stack[{block.CurrStackSize - 2}].fvalue;");
                    _output.WriteLine($"float rhs = stack[{block.CurrStackSize - 1}].fvalue;");
                    _output.WriteLine($"float res = ivm_fsub(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].fvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.FMUL:
                    _output.WriteLine("{");
                    _output.WriteLine($"float lhs = stack[{block.CurrStackSize - 2}].fvalue;");
                    _output.WriteLine($"float rhs = stack[{block.CurrStackSize - 1}].fvalue;");
                    _output.WriteLine($"float res = ivm_fmul(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].fvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.FDIV:
                    _output.WriteLine("{");
                    _output.WriteLine($"float lhs = stack[{block.CurrStackSize - 2}].fvalue;");
                    _output.WriteLine($"float rhs = stack[{block.CurrStackSize - 1}].fvalue;");
                    _output.WriteLine($"float res = ivm_fdiv(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].fvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.FMOD:
                    _output.WriteLine("{");
                    _output.WriteLine($"float lhs = stack[{block.CurrStackSize - 2}].fvalue;");
                    _output.WriteLine($"float rhs = stack[{block.CurrStackSize - 1}].fvalue;");
                    _output.WriteLine($"float res = ivm_fmod(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].fvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.DADD:
                    _output.WriteLine("{");
                    _output.WriteLine($"double lhs = stack[{block.CurrStackSize - 2}].dvalue;");
                    _output.WriteLine($"double rhs = stack[{block.CurrStackSize - 1}].dvalue;");
                    _output.WriteLine($"double res = ivm_dadd(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].dvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.DSUB:
                    _output.WriteLine("{");
                    _output.WriteLine($"double lhs = stack[{block.CurrStackSize - 2}].dvalue;");
                    _output.WriteLine($"double rhs = stack[{block.CurrStackSize - 1}].dvalue;");
                    _output.WriteLine($"double res = ivm_dsub(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].dvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.DMUL:
                    _output.WriteLine("{");
                    _output.WriteLine($"double lhs = stack[{block.CurrStackSize - 2}].dvalue;");
                    _output.WriteLine($"double rhs = stack[{block.CurrStackSize - 1}].dvalue;");
                    _output.WriteLine($"double res = ivm_dmul(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].dvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.DDIV:
                    _output.WriteLine("{");
                    _output.WriteLine($"double lhs = stack[{block.CurrStackSize - 2}].dvalue;");
                    _output.WriteLine($"double rhs = stack[{block.CurrStackSize - 1}].dvalue;");
                    _output.WriteLine($"double res = ivm_ddiv(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].dvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.DMOD:
                    _output.WriteLine("{");
                    _output.WriteLine($"double lhs = stack[{block.CurrStackSize - 2}].dvalue;");
                    _output.WriteLine($"double rhs = stack[{block.CurrStackSize - 1}].dvalue;");
                    _output.WriteLine($"double res = ivm_dmod(lhs, rhs);");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].dvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.ISHL:
                    _output.WriteLine("{");
                    _output.WriteLine($"uint32_t lhs = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"uint32_t rhs = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int32_t res = lhs << rhs;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.ISHR:
                    _output.WriteLine("{");
                    _output.WriteLine($"uint32_t lhs = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"uint32_t rhs = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int32_t res = lhs >> rhs;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LSHL:
                    _output.WriteLine("{");
                    _output.WriteLine($"uint64_t lhs = stack[{block.CurrStackSize - 2}].lvalue;");
                    _output.WriteLine($"uint32_t rhs = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int64_t res = lhs << rhs;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].lvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LSHR:
                    _output.WriteLine("{");
                    _output.WriteLine($"uint64_t lhs = stack[{block.CurrStackSize - 2}].lvalue;");
                    _output.WriteLine($"uint32_t rhs = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"int64_t res = lhs >> rhs;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].lvalue = res;");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.ICMP:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t lhs = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"int32_t rhs = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = ivm_icmp(lhs, rhs);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.LCMP:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t lhs = stack[{block.CurrStackSize - 2}].lvalue;");
                    _output.WriteLine($"int64_t rhs = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = ivm_lcmp(lhs, rhs);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.FCMP:
                    _output.WriteLine("{");
                    _output.WriteLine($"float lhs = stack[{block.CurrStackSize - 2}].fvalue;");
                    _output.WriteLine($"float rhs = stack[{block.CurrStackSize - 1}].fvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].ivalue = ivm_fcmp(lhs, rhs);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.DCMP:
                    _output.WriteLine("{");
                    _output.WriteLine($"double lhs = stack[{block.CurrStackSize - 2}].dvalue;");
                    _output.WriteLine($"double rhs = stack[{block.CurrStackSize - 1}].dvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}].dvalue = ivm_dcmp(lhs, rhs);");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.INOT:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].ivalue = ivm_inot(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.BNOT:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].ivalue = ivm_bnot(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.INEG:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].ivalue = ivm_ineg(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.LNOT:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t value = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].lvalue = ivm_lnot(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.LNEG:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t value = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].lvalue = ivm_lneg(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.FNEG:
                    _output.WriteLine("{");
                    _output.WriteLine($"float value = stack[{block.CurrStackSize - 1}].fvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].fvalue = ivm_fneg(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.DNEG:
                    _output.WriteLine("{");
                    _output.WriteLine($"double value = stack[{block.CurrStackSize - 1}].dvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].dvalue = ivm_dneg(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.B2I:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].ivalue = ivm_b2i(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.S2I:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].ivalue = ivm_s2i(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.I2B:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].ivalue = ivm_i2b(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.I2S:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].ivalue = ivm_i2s(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.I2L:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].lvalue = ivm_i2l(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.I2F:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].fvalue = ivm_i2f(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.I2D:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t value = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].dvalue = ivm_i2d(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.L2I:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t value = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].ivalue = ivm_l2i(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.L2F:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t value = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].fvalue = ivm_l2f(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.L2D:
                    _output.WriteLine("{");
                    _output.WriteLine($"int64_t value = stack[{block.CurrStackSize - 1}].lvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].dvalue = ivm_l2d(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.F2I:
                    _output.WriteLine("{");
                    _output.WriteLine($"float value = stack[{block.CurrStackSize - 1}].fvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].ivalue = ivm_f2i(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.F2L:
                    _output.WriteLine("{");
                    _output.WriteLine($"float value = stack[{block.CurrStackSize - 1}].fvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].lvalue = ivm_f2l(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.F2D:
                    _output.WriteLine("{");
                    _output.WriteLine($"float value = stack[{block.CurrStackSize - 1}].fvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].dvalue = ivm_f2d(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.D2I:
                    _output.WriteLine("{");
                    _output.WriteLine($"double value = stack[{block.CurrStackSize - 1}].dvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].ivalue = ivm_d2i(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.D2L:
                    _output.WriteLine("{");
                    _output.WriteLine($"double value = stack[{block.CurrStackSize - 1}].dvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].lvalue = ivm_d2l(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.D2F:
                    _output.WriteLine("{");
                    _output.WriteLine($"double value = stack[{block.CurrStackSize - 1}].dvalue;");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}].fvalue = ivm_d2f(value);");
                    _output.WriteLine("}");
                    break;
                case Opcode.CALL:
                    BitConverter.ToUInt16(args.ToArray(), 0);
                    index = BitConverter.ToUInt16(args.ToArray(), 0);
                    var name = _consts[(int)index];
                    if (name is MacroAssembler.RoutineConst)
                    {

                        _output.WriteLine($"int32_t {EncodeName("_F", _context.ProgramName, name.ToString().Replace(".", string.Empty).ToUpper())}(void);");
                        _output.Write($"ivm_builtin_push(0, \"{name.ToString()}\", 0);");
                        _output.WriteLine($"{EncodeName("_F", _context.ProgramName, name.ToString().Replace(".", string.Empty).ToUpper())}();");
                        _output.Write($"ivm_builtin_pop();");
                        block.CurrStackSize += 1;

                    }
                    else if (name is MacroAssembler.FunctionConst)
                    {
                        var orig_name = (name as MacroAssembler.FunctionConst).value;

                        var func_name = orig_name;
                        var tup = Utils.GetFunctionInfo(func_name);
                        if (func_name == "SYSLOAD")
                        {
                            _output.WriteLine("extern void * SYSLOAD(int32_t);");
                            _output.WriteLine(
                                $"stack[{block.CurrStackSize - 1}].pvalue = SYSLOAD(stack[{block.CurrStackSize - 1}].ivalue);");
                        }
                        else 
                        {
                            //aoi怎么办，名字怎么处理
                            if (func_name.Contains("."))
                            {
                                func_name = EncodeName("_A",func_name.Replace(".", string.Empty).ToUpper());
                            }
                            GenDecl(func_name, tup);
                            _output.Write($"ivm_builtin_push(0, \"{orig_name}\", 0);");
                            GenCall(func_name, tup, block.CurrStackSize);
                            _output.Write($"ivm_builtin_pop();");

                            block.CurrStackSize -= (tup.Item1.Count - 1);
                        }

                    }
                    else
                    {

                        Debug.Assert(false, $"{name.ToString()}:{name.GetType().ToString()}");
                    }
                    break;
                case Opcode.RET:
                    _output.WriteLine($"return {args[0]};");
                    break;
                case Opcode.JEQ:
                    Debug.Assert(args.Count == 4, args.Count.ToString());
                    offset = BitConverter.ToInt32(args.ToArray(), 0);
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t  cond = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"if(cond == 0) {{");
                    _output.WriteLine($"goto L{builder.GetBlockAt(pos + offset).SeqNo};");
                    _output.WriteLine("}");
                    _output.WriteLine("}");
                    block.CurrStackSize--;
                    break;
                case Opcode.JNE:
                    Debug.Assert(args.Count == 4, args.Count.ToString());
                    offset = BitConverter.ToInt32(args.ToArray(), 0);
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t  cond = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"if(cond != 0) {{");
                    _output.WriteLine($"goto L{builder.GetBlockAt(pos + offset).SeqNo};");
                    _output.WriteLine("}");
                    _output.WriteLine("}");
                    block.CurrStackSize--;
                    break;
                case Opcode.JGE:
                    Debug.Assert(args.Count == 4, args.Count.ToString());
                    offset = BitConverter.ToInt32(args.ToArray(), 0);
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t  cond = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"if(cond >= 0) {{");
                    _output.WriteLine($"goto L{builder.GetBlockAt(pos + offset).SeqNo};");
                    _output.WriteLine("}");
                    _output.WriteLine("}");
                    block.CurrStackSize--;
                    break;
                case Opcode.JGT:
                    Debug.Assert(args.Count == 4, args.Count.ToString());
                    offset = BitConverter.ToInt32(args.ToArray(), 0);
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t  cond = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"if(cond > 0) {{");
                    _output.WriteLine($"goto L{builder.GetBlockAt(pos + offset).SeqNo};");
                    _output.WriteLine("}");
                    _output.WriteLine("}");
                    block.CurrStackSize--;
                    break;
                case Opcode.JLE:
                    Debug.Assert(args.Count == 4, args.Count.ToString());
                    offset = BitConverter.ToInt32(args.ToArray(), 0);
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t  cond = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"if(cond <= 0) {{");
                    _output.WriteLine($"goto L{builder.GetBlockAt(pos + offset).SeqNo};");
                    _output.WriteLine("}");
                    _output.WriteLine("}");
                    block.CurrStackSize--;
                    break;
                case Opcode.JLT:
                    Debug.Assert(args.Count == 4, args.Count.ToString());
                    offset = BitConverter.ToInt32(args.ToArray(), 0);
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t  cond = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine($"if(cond < 0) {{");
                    _output.WriteLine($"goto L{builder.GetBlockAt(pos + offset).SeqNo};");
                    _output.WriteLine("}");
                    _output.WriteLine("}");
                    block.CurrStackSize--;
                    break;
                case Opcode.JMP:
                    Debug.Assert(args.Count == 4, args.Count.ToString());
                    offset = BitConverter.ToInt32(args.ToArray(), 0);
                    _output.WriteLine("{");
                    _output.WriteLine($"goto L{builder.GetBlockAt(pos + offset).SeqNo};");
                    _output.WriteLine("}");
                    block.CurrStackSize--;
                    break;
 
                case Opcode.THROW:
                    _output.WriteLine("{");
                    _output.WriteLine($"int32_t code = stack[{block.CurrStackSize - 2}].ivalue;");
                    _output.WriteLine($"int32_t type = stack[{block.CurrStackSize - 1}].ivalue;");
                    _output.WriteLine("if (ivm_throw(type, code) != 0) { return 0; }");
                    _output.WriteLine("}");
                    block.CurrStackSize -= 2;
                    break;
                case Opcode.DUP_X1:
                    _output.WriteLine("{");
                    _output.WriteLine($"stack[{block.CurrStackSize}] = stack[{block.CurrStackSize - 1}];");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}] = stack[{block.CurrStackSize - 2}];");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}] = stack[{block.CurrStackSize}];");
                    _output.WriteLine("}");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.SWAP:
                    _output.WriteLine("{");
                    _output.WriteLine($"union Operand tmp = stack[{block.CurrStackSize - 1}];");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}] = stack[{block.CurrStackSize - 2}];");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}] = tmp;");
                    _output.WriteLine("}");
                    break;
                case Opcode.SWAP_X1:
                    _output.WriteLine("{");
                    _output.WriteLine($"union Operand tmp = stack[{block.CurrStackSize - 3}];");
                    _output.WriteLine($"stack[{block.CurrStackSize - 3}] = stack[{block.CurrStackSize - 2}];");
                    _output.WriteLine($"stack[{block.CurrStackSize - 2}] = stack[{block.CurrStackSize - 1}];");
                    _output.WriteLine($"stack[{block.CurrStackSize - 1}] = tmp;");
                    _output.WriteLine("}");
                    break;
                case Opcode.CONST_PNULL:
                    _output.WriteLine($"stack[{block.CurrStackSize}].pvalue = 0;");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.CONST_I0:
                    _output.WriteLine($"stack[{block.CurrStackSize}].ivalue = 0;");
                    block.CurrStackSize += 1;
                    break;
 
                case Opcode.CONST_I1:
                    _output.WriteLine($"stack[{block.CurrStackSize}].ivalue = 1;");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.CONST_M1:
                    _output.WriteLine($"stack[{block.CurrStackSize}].ivalue = -1;");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.CONST_L0:
                    _output.WriteLine($"stack[{block.CurrStackSize}].lvalue = 0;");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.CONST_L1:
                    _output.WriteLine($"stack[{block.CurrStackSize}].lvalue = 1;");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.CONST_F0:
                    _output.WriteLine($"stack[{block.CurrStackSize}].fvalue = 0.0;");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.CONST_F1:
                    _output.WriteLine($"stack[{block.CurrStackSize}].fvalue = 1.0;");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.CONST_D0:
                    _output.WriteLine($"stack[{block.CurrStackSize}].dvalue = 0.0;");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.CONST_D1:
                    _output.WriteLine($"stack[{block.CurrStackSize}].dvalue = 1.0;");
                    block.CurrStackSize += 1;
                    break;
                case Opcode.CHECK:
                    _output.WriteLine($"ivm_check(stack[{block.CurrStackSize-1}].ivalue);");
                    block.CurrStackSize -= 1;
                    break;
                case Opcode.ECHECK:
                    _output.WriteLine("if (SYSSTATUS(1) != 0){ return 0; }");
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private string EncodeName(string prefix, string scope, string name)
        {
            return $"{prefix}{scope.Length}{scope}{name.Length}{name}".ToUpper();
        }

        private string EncodeName(string prefix, string name)
        {
            return $"{prefix}{name.Length}{name}".ToUpper();
        }

        private string TypeName(SlotDataType type)
        {
            switch (type)
            {
                case SlotDataType.INT32:
                    return "int32_t";
                case SlotDataType.INT64:
                    return "int64_t";
                case SlotDataType.FLOAT:
                    return "float";
                case SlotDataType.DOUBLE:
                    return "double";
                case SlotDataType.POINTER:
                    return "void*";
                case SlotDataType.UNKNOWN:
                    Debug.Assert(false, type.ToString());
                    return "int32_t";
            }
            Debug.Assert(false, type.ToString());
            return "int32_t";
        }

        private string MemberName(SlotDataType type)
        {
            switch (type)
            {
                case SlotDataType.INT32:
                    return "ivalue";
                case SlotDataType.INT64:
                    return "lvalue";
                case SlotDataType.FLOAT:
                    return "fvalue";
                case SlotDataType.DOUBLE:
                    return "dvalue";
                case SlotDataType.POINTER:
                    return "pvalue";
            }
            Debug.Assert(false, type.ToString());
            return "int32_t";
        }
        private void GenDecl(string name, Tuple<List<SlotDataType>, SlotDataType> proto)
        {
            List<string> parameters = new List<string>();
            foreach (var tp in proto.Item1)
            {
                parameters.Add(TypeName(tp));
            }
            _output.WriteLine($"{TypeName(proto.Item2)} {name.ToUpper()}({string.Join(",", parameters)});");
        }

        private void GenCall(string name, Tuple<List<SlotDataType>, SlotDataType> proto, int stackSize)
        {
            List<string> parameters = new List<string>();
            for (int i = 0;i < proto.Item1.Count; ++i)
            {
                parameters.Add($"stack[{stackSize  - proto.Item1.Count + i}].{MemberName(proto.Item1[i])}");
            }
            _output.WriteLine($"stack[{stackSize - proto.Item1.Count}].{MemberName(proto.Item2)} = {name.ToUpper()}({string.Join(",", parameters)});");
        }

        private readonly OutputStream _output;
        private readonly Function _func;
        private CCodeGeneratorContext _context;
        private readonly List<MacroAssembler.IConst> _consts;
    }

    public class OutputStream
    {
        public OutputStream(string path)
        {
            _file = new StreamWriter(path);
        }
        public void Write(string str)
        {
            _file.Write(str);
        }

        public void WriteLine(string str)
        {
            _file.WriteLine(str);
        }

        public void Close()
        {
            _file.Close();
        }
        private readonly StreamWriter _file;
    }



}
