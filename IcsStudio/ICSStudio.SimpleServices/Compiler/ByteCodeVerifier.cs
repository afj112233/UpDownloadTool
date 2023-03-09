using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using Antlr4.Runtime;
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
    internal enum SlotDataType
    {
        UNKNOWN,
        INT32,
        INT64,
        FLOAT,
        DOUBLE,
        POINTER,
    }
    */

    internal class BlockBegin
    {
        public BlockBegin(int pos)
        {
            pos_ = pos;
        }

        public int pos
        {
            get { return pos_; }
        }

        public void AddSuccessor(BlockBegin succ)
        {
            successor_.Add(succ);
        }

        private List<BlockBegin> successor_ = new List<BlockBegin>();
        private int pos_;

        public Stack<SlotDataType> stack { get; set; } = new Stack<SlotDataType>();
        public Stack<SlotDataType> pre_stack { get; set; }
        public bool is_visited { set; get; } = false;
    }
    /*
    internal class Utils
    {
        public static IEnumerable<Tuple<byte, List<byte>, int>> GetByteCode(List<byte> codes)
        {
            int index = 0;
            while (true)
            {
                if (index >= codes.Count)
                {
                    break;
                }
                var code = codes[index];
                var arg_size = Opcode.ARG_SIZE[code];
                yield return Tuple.Create(code, codes.GetRange(index + 1,arg_size), index);
                index += 1 + arg_size;
            }
        }

        private static SlotDataType ToSlotDataType(IDataType tp)
        {
            if (tp.IsInteger && !(tp is LINT))
            {
                return SlotDataType.INT32;
            }

            if (tp is LINT)
            {
                return SlotDataType.INT64;
            }

            if (tp is REAL)
            {
                return SlotDataType.FLOAT;
            }

            if (tp is LREAL)
            {
                return SlotDataType.DOUBLE;
            }

            Debug.Assert(false, tp.ToString());
            return SlotDataType.UNKNOWN;

        }

        public static Tuple<List<SlotDataType>, SlotDataType> GetFunctionInfo(string name)
        {
            var instr = RTInstructionCollection.Inst.FindInstruction(name);
            Debug.Assert(instr != null, name);
            var args = new List<SlotDataType>();

            Debug.Assert(instr.is_ref.Count == instr.param_types.Count, $"{instr.is_ref.Count}:{instr.param_types.Count}");
            for (int i = 0; i < instr.is_ref.Count; ++i)
            {
                if (instr.is_ref[i])
                {
                    args.Add(SlotDataType.POINTER);
                }
                else
                {
                   args.Add(ToSlotDataType(instr.param_types[i]));
                }
            }

            return Tuple.Create(args, ToSlotDataType(instr.return_type));

        }
    }
    */

    class BasicBlockBuilder
    {

        public BasicBlockBuilder(Function func, List<MacroAssembler.IConst> consts, List<bool> isDINT = null)
        {
            Initialize(func.Codes, func.ArgsSize, func.LocalsSize, func.SafePoints, consts, isDINT);
        }

        private void Initialize(List<byte> codes, int args_size, int locals_size, List<int> safe_points, List<MacroAssembler.IConst> consts, List<bool> isDINT)
        {
            codes_ = codes;
            locals_ = Enumerable.Repeat(SlotDataType.UNKNOWN, locals_size).ToList();
            consts_ = ConvertConstSlotType(consts);
            
            pool_ = consts;

            Debug.Assert(args_size <= locals_size, $"{args_size}:{locals_size} :{isDINT?.Count}");
            for (int i = 0; i < args_size; ++i)
            {
                locals_[i] = isDINT == null || !isDINT[i] ? SlotDataType.POINTER : SlotDataType.INT32;
            }
            Debug.Assert(safe_points.Count % 3 == 0, safe_points.ToString());

            for (int i = 0; i < safe_points.Count/3; ++i)
            {
                var begin = safe_points[3 * i + 0];
                var end = safe_points[3 * i + 1];
                var stack_size = safe_points[3 * i + 2];
                if (safe_points_.ContainsKey(begin))
                {
                    Debug.Assert(safe_points_[begin] == stack_size, $"{safe_points_[begin]}:{stack_size}");
                }
                else
                {
                    safe_points_[begin] = stack_size;
                }

                if (safe_points_.ContainsKey(end))
                {
                    Debug.Assert(safe_points_[end] == stack_size, $"{safe_points_[end]}:{stack_size}");

                }
                else
                {
                    safe_points_[end] = stack_size;
                }
            }


            blocks_ = new List<BlockBegin>(codes.Count);
            for (int i = 0; i < codes.Count; ++i)
            {
                blocks_.Add(null);
            }
        }

        public void BuildBlocks()
        {
            Debug.Assert(codes_.Count >= 2, $"{codes_.Count}");
            Debug.Assert(codes_[codes_.Count - 2] == Opcode.RET, $"{codes_[codes_.Count - 2]}");
            MarkBasicBlock();
            blocks_[0].pre_stack = new Stack<SlotDataType>();
            work_list_.Enqueue(blocks_[0]);
            IteratorAllBlocks();
        }

        void IteratorAllBlocks()
        {
            while (work_list_.Count != 0)
            {
                var block = work_list_.Dequeue();
                if (block.is_visited)
                {
                    continue;
                }
                IteratorByteCodesForBlock(block);
                block.is_visited = true;

            }
        }

        private static void AssertTopAndPop(Stack<SlotDataType> stack, SlotDataType type)
        {
            var tmp = stack.Pop();
            if (tmp != type)
            {
                Debug.WriteLine($"fffffff:{tmp}");
                while (stack.Count != 0)
                {
                    Debug.WriteLine($"xxxxxxxx:{stack.Pop()}");
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
        void IteratorByteCodesForBlock(BlockBegin block)
        {
            var pos = block.pos;
            while (pos != codes_.Count && (blocks_[pos] == null || blocks_[pos] == block))
            {

                var code = codes_[pos];
               //Console.WriteLine($"{pos}:{(Opcode.Code) code}:{block.stack.Count}");
               
                if (safe_points_.ContainsKey(pos))
                {
                    Debug.Assert(safe_points_[pos] == block.stack.Count, $"{pos},real:{block.stack.Count},acquire:{safe_points_[pos]}");
                    safe_points_.Remove(pos);
                }
                switch (code)
                {
                    case Opcode.NOP:
                        break;
                    case Opcode.BIPUSH:
                    case Opcode.SIPUSH:
                        block.stack.Push(SlotDataType.INT32);
                        break;
                    case Opcode.POP:
                        block.stack.Pop();
                        break;
                    case Opcode.DUP:
                        block.stack.Push(block.stack.Peek());
                        break;
                    case Opcode.CLOAD:
                        var index = BitConverter.ToUInt32(codes_.GetRange(pos + 1, 4).ToArray(), 0);
                        block.stack.Push(consts_[(int)index]);
                        break;
                    case Opcode.LOAD_LOCAL:
                        index = BitConverter.ToUInt16(codes_.GetRange(pos + 1, 2).ToArray(), 0);
                        var tp = locals_[(int) index];
                        Debug.Assert(tp!= SlotDataType.UNKNOWN, tp.ToString());
                        block.stack.Push(tp);
                        break;

                    case Opcode.STORE_LOCAL:
                        index = BitConverter.ToUInt16(codes_.GetRange(pos + 1, 2).ToArray(), 0);
                        tp = locals_[(int) index];
                        var top_tp = block.stack.Peek();
                        Debug.Assert(tp == SlotDataType.UNKNOWN || tp == top_tp, $"{tp.ToString()}:{top_tp.ToString()}");
                        locals_[(int)index] = top_tp;
                        block.stack.Pop();
                        break;
                    case Opcode.LOAD_INT8_BIT:
                    case Opcode.LOAD_INT16_BIT:
                    case Opcode.LOAD_INT32_BIT:
                    case Opcode.LOAD_INT64_BIT:
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        AssertTopAndPop(block.stack, SlotDataType.POINTER);
                        block.stack.Push(SlotDataType.INT32);
                        break;
                    case Opcode.LOAD_INT8:
                    case Opcode.LOAD_INT16:
                    case Opcode.LOAD_INT32:
                        tp = block.stack.Pop();
                        Debug.Assert(tp == SlotDataType.POINTER, tp.ToString());
                        block.stack.Push(SlotDataType.INT32);
                        break;
                    case Opcode.LOAD_INT64:
                        tp = block.stack.Pop();
                        Debug.Assert(tp == SlotDataType.POINTER, tp.ToString());
                        block.stack.Push(SlotDataType.INT64);
                        break;
                    case Opcode.LOAD_FLOAT:
                        tp = block.stack.Pop();
                        Debug.Assert(tp == SlotDataType.POINTER, tp.ToString());
                        block.stack.Push(SlotDataType.FLOAT);
                        break;
                    case Opcode.LOAD_DOUBLE:
                        tp = block.stack.Pop();
                        Debug.Assert(tp == SlotDataType.POINTER, tp.ToString());
                        block.stack.Push(SlotDataType.DOUBLE);
                        break;
                    case Opcode.STORE_INT8_BIT:
                    case Opcode.STORE_INT16_BIT:
                    case Opcode.STORE_INT32_BIT:
                    case Opcode.STORE_INT64_BIT:
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        AssertTopAndPop(block.stack, SlotDataType.POINTER);
                        break;
                    case Opcode.STORE_INT8:
                    case Opcode.STORE_INT16:
                    case Opcode.STORE_INT32:
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        AssertTopAndPop(block.stack, SlotDataType.POINTER);
                        break;
                    case Opcode.STORE_INT64:
                        AssertTopAndPop(block.stack, SlotDataType.INT64);
                        AssertTopAndPop(block.stack, SlotDataType.POINTER);
                        break;
                    case Opcode.STORE_FLOAT:
                        AssertTopAndPop(block.stack, SlotDataType.FLOAT);
                        AssertTopAndPop(block.stack, SlotDataType.POINTER);
                        break;
                    case Opcode.STORE_DOUBLE:
                        AssertTopAndPop(block.stack, SlotDataType.DOUBLE);
                        AssertTopAndPop(block.stack, SlotDataType.POINTER);
                        break;
                    case Opcode.PBINC:
                    case Opcode.PSINC:           
                        AssertTop(block.stack, SlotDataType.POINTER);
                        break;
                    case Opcode.PADD:
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        AssertTop(block.stack, SlotDataType.POINTER);
                        break;
                    case Opcode.IADD:
                    case Opcode.ISUB:
                    case Opcode.IMUL:
                    case Opcode.IDIV:
                    case Opcode.IMOD:
                    case Opcode.IOR:
                    case Opcode.IXOR:
                    case Opcode.IAND:
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        AssertTop(block.stack, SlotDataType.INT32);
                        break;
                    case Opcode.LADD:
                    case Opcode.LSUB:
                    case Opcode.LMUL:
                    case Opcode.LDIV:
                    case Opcode.LMOD:
                    case Opcode.LOR:
                    case Opcode.LXOR:
                    case Opcode.LAND:
                        AssertTopAndPop(block.stack, SlotDataType.INT64);
                        AssertTop(block.stack, SlotDataType.INT64);
                        break;
                    case Opcode.FADD:
                    case Opcode.FSUB:
                    case Opcode.FMUL:
                    case Opcode.FDIV:
                    case Opcode.FMOD:
                        AssertTopAndPop(block.stack, SlotDataType.FLOAT);
                        AssertTop(block.stack, SlotDataType.FLOAT);
                        break;
                    case Opcode.DADD:
                    case Opcode.DSUB:
                    case Opcode.DMUL:
                    case Opcode.DDIV:
                    case Opcode.DMOD:
                        AssertTopAndPop(block.stack, SlotDataType.DOUBLE);
                        AssertTop(block.stack, SlotDataType.DOUBLE);
                        break;
                    case Opcode.ISHL:
                    case Opcode.ISHR:
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        AssertTop(block.stack, SlotDataType.INT32);
                        break;
                    case Opcode.LSHL:
                    case Opcode.LSHR:
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        AssertTop(block.stack, SlotDataType.INT64);
                        break;
                    case Opcode.ICMP:
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        block.stack.Push(SlotDataType.INT32);
                        break;
                    case Opcode.LCMP:
                        AssertTopAndPop(block.stack, SlotDataType.INT64);
                        AssertTopAndPop(block.stack, SlotDataType.INT64);
                        block.stack.Push(SlotDataType.INT32);
                        break;
                    case Opcode.FCMP:
                        AssertTopAndPop(block.stack, SlotDataType.FLOAT);
                        AssertTopAndPop(block.stack, SlotDataType.FLOAT);
                        block.stack.Push(SlotDataType.INT32);
                        break;
                    case Opcode.DCMP:
                        AssertTopAndPop(block.stack, SlotDataType.DOUBLE);
                        AssertTopAndPop(block.stack, SlotDataType.DOUBLE);
                        block.stack.Push(SlotDataType.INT32);
                        break;
                    case Opcode.INOT:
                    case Opcode.BNOT:
                    case Opcode.INEG:
                        AssertTop(block.stack, SlotDataType.INT32);
                        break;
                    case Opcode.LNOT:
                    case Opcode.LNEG:
                        AssertTop(block.stack, SlotDataType.INT64);
                        break;
                    case Opcode.FNEG:
                        AssertTop(block.stack, SlotDataType.FLOAT);
                        break;
                    case Opcode.DNEG:
                        AssertTop(block.stack, SlotDataType.DOUBLE);
                        break;
                    case Opcode.B2I:
                    case Opcode.S2I:
                    case Opcode.I2B:
                    case Opcode.I2S:
                        AssertTop(block.stack, SlotDataType.INT32);
                        break;
                    case Opcode.I2L:
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        block.stack.Push(SlotDataType.INT64);
                        break;
                    case Opcode.I2F:
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        block.stack.Push(SlotDataType.FLOAT);
                        break;
                    case Opcode.I2D:
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        block.stack.Push(SlotDataType.DOUBLE);
                        break;
                    case Opcode.L2I:
                        AssertTopAndPop(block.stack, SlotDataType.INT64);
                        block.stack.Push(SlotDataType.INT32);
                        break;
                    case Opcode.L2F:
                        AssertTopAndPop(block.stack, SlotDataType.INT64);
                        block.stack.Push(SlotDataType.FLOAT);
                        break;
                    case Opcode.L2D:
                        AssertTopAndPop(block.stack, SlotDataType.INT64);
                        block.stack.Push(SlotDataType.DOUBLE);
                        break;
                    case Opcode.F2I:
                        AssertTopAndPop(block.stack, SlotDataType.FLOAT);
                        block.stack.Push(SlotDataType.INT32);
                        break;
                    case Opcode.F2L:
                        AssertTopAndPop(block.stack, SlotDataType.FLOAT);
                        block.stack.Push(SlotDataType.INT64);
                        break;
                    case Opcode.F2D:
                        AssertTopAndPop(block.stack, SlotDataType.FLOAT);
                        block.stack.Push(SlotDataType.DOUBLE);
                        break;
                    case Opcode.D2I:
                        AssertTopAndPop(block.stack, SlotDataType.DOUBLE);
                        block.stack.Push(SlotDataType.INT32);
                        break;
                    case Opcode.D2L:
                        AssertTopAndPop(block.stack, SlotDataType.DOUBLE);
                        block.stack.Push(SlotDataType.INT64);
                        break;
                    case Opcode.D2F:
                        AssertTopAndPop(block.stack, SlotDataType.DOUBLE);
                        block.stack.Push(SlotDataType.FLOAT);
                        break;
                    case Opcode.CALL:
                        index = BitConverter.ToUInt16(codes_.GetRange(pos + 1, 2).ToArray(), 0);
                        var name = pool_[(int) index];
                        if (name is MacroAssembler.RoutineConst)
                        {
                            block.stack.Push(SlotDataType.INT32);
                        } else if (name is MacroAssembler.FunctionConst)
                        {
                            var func_name = (name as MacroAssembler.FunctionConst).value;
                            var tup = Utils.GetFunctionInfo(func_name);
                            for (int i = 0; i < tup.Item1.Count; ++i)
                            {
                                AssertTopAndPop(block.stack, tup.Item1[tup.Item1.Count - i - 1]);
                            }

                            if (func_name == "SYSLOAD")
                            {
                                block.stack.Push(SlotDataType.POINTER);

                            }
                            else
                            {
                                block.stack.Push(tup.Item2);

                            }

                        }
                        else
                        {

                            Debug.Assert(false, $"{name.ToString()}:{name.GetType().ToString()}");
                        }

                        break;
                    case Opcode.RET:
                        break;
                    case Opcode.JEQ:
                    case Opcode.JNE:
                    case Opcode.JGE:
                    case Opcode.JGT:
                    case Opcode.JLE:
                    case Opcode.JLT:
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        //AddBlockToWorkList(blocks_[pos + 1 + 4], block);
                        AddBlockToWorkList(blocks_[Dest(pos)], block);
                        break;
                    case Opcode.JMP:
                        AddBlockToWorkList(blocks_[Dest(pos)], block);
                        break;
                    case Opcode.THROW:
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        break;
                    case Opcode.DUP_X1:
                        top_tp = block.stack.Pop();
                        tp = block.stack.Pop();
                        block.stack.Push(top_tp);
                        block.stack.Push(tp);
                        block.stack.Push(top_tp);
                        break;
                    case Opcode.SWAP:
                        top_tp = block.stack.Pop();
                        tp = block.stack.Pop();
                        block.stack.Push(top_tp);
                        block.stack.Push(tp);
                        break;
                    case Opcode.SWAP_X1:
                        top_tp = block.stack.Pop();
                        tp = block.stack.Pop();
                        var tmp = block.stack.Pop();
                        block.stack.Push(tp);
                        block.stack.Push(top_tp);
                        block.stack.Push(tmp);
                        break;
                    case Opcode.CONST_PNULL:
                        block.stack.Push(SlotDataType.POINTER);
                        break;
                    case Opcode.CONST_I0:
                    case Opcode.CONST_I1:
                    case Opcode.CONST_M1:
                        block.stack.Push(SlotDataType.INT32);
                        break;
                    case Opcode.CONST_L0:
                    case Opcode.CONST_L1:
                        block.stack.Push(SlotDataType.INT64);
                        break;
                    case Opcode.CONST_F0:
                    case Opcode.CONST_F1:
                        block.stack.Push(SlotDataType.FLOAT);
                        break;
                    case Opcode.CONST_D0:
                    case Opcode.CONST_D1:
                        block.stack.Push(SlotDataType.DOUBLE);
                        break;
                    case Opcode.CHECK:
                        AssertTopAndPop(block.stack, SlotDataType.INT32);
                        break;
                    case Opcode.ECHECK:
                        break;

                    default:
                        throw new NotImplementedException();
                }

                pos += (1+ Opcode.ARG_SIZE[code]);
                if (pos <= blocks_.Count - 1 && blocks_[pos] != null && code != Opcode.JMP && code != Opcode.RET)
                {
                    AddBlockToWorkList(blocks_[pos], block);
                }
            }
        }

        void MarkBasicBlock()
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
                            MakeBlockAt(pos+2);
                        }

                        break;
                }
            }
        }

        private void HandlePredessor(BlockBegin block, BlockBegin current)
        {
            Debug.Assert(block.pre_stack.SequenceEqual(current.stack));
        }

        private void AddBlockToWorkList(BlockBegin block, BlockBegin current)
        {
            if (block.is_visited)
            {
                HandlePredessor(block, current);
                return;
            }

            block.pre_stack = new Stack<SlotDataType>(current.stack.Reverse());
            block.stack = new Stack<SlotDataType>(current.stack.Reverse());
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
                    return BitConverter.ToInt32(new byte[] { args[0], args[1], args[2], args[3] }, 0);
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

            var offset = BitConverter.ToInt32(new byte[] {codes_[pos+1], codes_[pos+2], codes_[pos+3], codes_[pos+4]}, 0);
            return pos + offset;
        }

        private BlockBegin MakeBlockAt(int pos)
        {
            if (blocks_[pos] == null)
            {
                blocks_[pos] = new BlockBegin(pos);
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

        private Queue<BlockBegin> work_list_ = new Queue<BlockBegin>();

        private List<byte> codes_;
        private List<BlockBegin> blocks_;
        private List<SlotDataType> locals_;
        private List<SlotDataType> consts_;
        private List<MacroAssembler.IConst> pool_;
        private Dictionary<int, int> safe_points_ = new Dictionary<int, int>();


    }

    class ByteCodeVerifier
    {
    }
}
