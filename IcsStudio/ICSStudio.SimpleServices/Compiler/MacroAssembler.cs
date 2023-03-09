using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.Compiler
{
    public class ConstPool
    {
        public Dictionary<string, int> str_index = new Dictionary<string, int>();
        public Dictionary<Int64, int> int_index = new Dictionary<Int64, int>();
        public Dictionary<string, int> tag_index = new Dictionary<string, int>();
        public List<byte[]> consts_pool = new List<byte[]>();
    }

    public class MacroAssembler
    {
        Dictionary<string, int> str_index = new Dictionary<string, int>();
        Dictionary<Int64, int> int_index = new Dictionary<Int64, int>();
        Dictionary<string, int> tag_index = new Dictionary<string, int>();

        //private ConstPool pool;

        public MacroAssembler(ConstPool pool)
        {
            this.consts_pool = pool.consts_pool;
            this.str_index = pool.str_index;
            this.int_index = pool.int_index;
            this.tag_index = pool.tag_index;
        }

        public enum ConstantPoolType : byte
        {
            INTEGER = 0,
            FLOAT = 1,
            TAG = 2,
            FUNCTION = 3,
            STRING = 4,
            ROUTINE = 5,
            TASK = 6,
            INTEGER64,
        }


        public interface IConst
        {
            ConstantPoolType type();
            string ToCString();
        }

        public  class IntegerConst : IConst
        {
            public IntegerConst(int value)
            {
                this.value = value;

            }

            public override string ToString()
            {
                return value.ToString();
            }

            public string ToCString()
            {
                return value.ToString();
            }

            public ConstantPoolType type()
            {
                return ConstantPoolType.INTEGER;
            }


            public int value { get; private set; }
        }

        public class FloatConst : IConst
        {
            public FloatConst(float value)
            {
                this.value = value;
            }

            public override string ToString()
            {
                return value.ToString();
            }

            public string ToCString()
            {
                if (Single.IsPositiveInfinity(value))
                {
                    return "1.0f/0.0";
                } else if (Single.IsNegativeInfinity(value))
                {
                    return "-1.0f/0.0f ";
                }
                else if (float.IsNaN(value))
                {
                    return "0.0/0.0";
                }
                return value.ToString();
            }
            public ConstantPoolType type()
            {
                return ConstantPoolType.FLOAT;
            }
            public float value { get; }
        }


        public class Integer64Const : IConst
        {
            public Integer64Const(Int64 value)
            {
                this.value = value;

            }

            public override string ToString()
            {
                return value.ToString();
            }

            public string ToCString()
            {
                return value.ToString();
            }
            public ConstantPoolType type()
            {
                return ConstantPoolType.INTEGER64;
            }


            public Int64 value { get; private set; }
        }
        public abstract class IStringConst : IConst
        {
            public IStringConst(string value)
            {
                this.value = value;
            }

            public override string ToString()
            {
                return value.ToString();
            }

            public virtual string ToCString()
            {
                return value.ToString();
            }

            public abstract ConstantPoolType type();

            public string value { get; }
        }

        public class TagConst : IStringConst
        {
            public TagConst(string value) : base(value)
            {
            }

            public override ConstantPoolType type()
            {
                return ConstantPoolType.TAG;
            }

            public override string ToCString()
            {
                return $"T{base.ToCString()}";
            }
        }

        public class FunctionConst : IStringConst
        {
            public FunctionConst(string value) : base(value)
            {
            }

            public override ConstantPoolType type()
            {
                return ConstantPoolType.FUNCTION;
            }

            public override string ToCString()
            {
                return $"F{base.ToCString()}";
            }
        }

        public class StringConst : IStringConst
        {
            public StringConst(string value) : base(value)
            {
            }

            public override ConstantPoolType type()
            {
                return ConstantPoolType.STRING;
            }
        }

        public class RoutineConst : IStringConst
        {
            public RoutineConst(string value) : base(value)
            {
            }

            public override ConstantPoolType type()
            {
                return ConstantPoolType.ROUTINE;
            }

            public override string ToCString()
            {
                return $"F{base.ToCString()}";
            }
        }

        public class TaskConst : IStringConst
        {
            public  TaskConst(string value) : base(value)
            {
            }

            public override ConstantPoolType type()
            {
                return ConstantPoolType.TASK;
            }
        }

        private static List<Tuple<ConstantPoolType, int, int>> ParsePosition(List<byte> pool)
        {
            var res = new List<Tuple<ConstantPoolType, int, int>>();
            int index = 0;
            while (index < pool.Count)
            {
                var type = (ConstantPoolType)pool[index];
                switch (type)
                {
                    case ConstantPoolType.INTEGER:
                    case ConstantPoolType.FLOAT:
                        res.Add(new Tuple<ConstantPoolType, int, int>(type, index, 5));
                        index += 5;
                        break;
                    case ConstantPoolType.INTEGER64:
                        res.Add(new Tuple<ConstantPoolType, int, int>(type, index, 9));
                        index += 9;
                        break;
                    case ConstantPoolType.TAG:
                        res.Add(new Tuple<ConstantPoolType, int, int>(type, index, 7));
                        index += 7;
                        break;
                    case ConstantPoolType.FUNCTION:
                    case ConstantPoolType.ROUTINE:
                        res.Add(new Tuple<ConstantPoolType, int, int>(type, index, 4));
                        index += 4;
                        break;
                    case ConstantPoolType.STRING:
                        var len = BitConverter.ToUInt16(pool.GetRange(index + 1, 2).ToArray(), 0);
                        res.Add(new Tuple<ConstantPoolType, int, int>(type, index, 3 + len));
                        index += 3 + len;
                        break;
                    default:
                        Debug.Assert(false, type.ToString());
                        break;
                }

            }
            return res;

        }

        private static string GetString(List<byte> pool, Tuple<ConstantPoolType, int, int> tup)
        {
            var offset = tup.Item2 + 3;
            var size = tup.Item3 - 3;
            return System.Text.Encoding.Default.GetString((pool.GetRange(offset, size).ToArray()));
        }

        private static string GetName(List<byte> pool, List<Tuple<ConstantPoolType, int, int>> poses,
            Tuple<ConstantPoolType, int, int> tup)
        {
            var offset = tup.Item2 + 1;
            var index = BitConverter.ToUInt16(pool.GetRange(offset, 2).ToArray(), 0);
            return GetString(pool, poses[index]);
        }
        public static List<IConst> ParseConstses(List<byte> pool)
        {
            var res = new List<IConst>();

            var poses = ParsePosition(pool);

            foreach (var tup in poses)
            {                
                var type = tup.Item1;
                switch (type)
                {
                    case ConstantPoolType.INTEGER:
                        res.Add(new IntegerConst(BitConverter.ToInt32(pool.GetRange(tup.Item2 + 1, 4).ToArray(), 0)));
                        break;
                    case ConstantPoolType.FLOAT:
                        res.Add(new FloatConst(BitConverter.ToSingle(pool.GetRange(tup.Item2 + 1, 4).ToArray(), 0)));
                        break;
                    case ConstantPoolType.INTEGER64:
                        res.Add(new Integer64Const(BitConverter.ToInt64(pool.GetRange(tup.Item2 + 1, 8).ToArray(), 0)));
                        break;
                    case ConstantPoolType.TAG:
                        res.Add(new TagConst(GetName(pool, poses, tup)));
                        break;
                    case ConstantPoolType.FUNCTION:
                        res.Add(new FunctionConst(GetName(pool, poses, tup)));
                        break;
                    case ConstantPoolType.ROUTINE:
                        res.Add(new RoutineConst(GetName(pool, poses, tup)));
                        break;
                    case ConstantPoolType.STRING:
                        var offset = tup.Item2 + 3;
                        var size = tup.Item3 - 3;
                        res.Add(new StringConst(System.Text.Encoding.Default.GetString((pool.GetRange(offset, size).ToArray()))));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return res;
        }

        private static byte[] MakeCPTag(UInt16 index, UInt32 size_index)
        {
            var res = new List<byte>();
            res.Add((byte)ConstantPoolType.TAG);
            res.AddRange(BitConverter.GetBytes(index));
            res.AddRange(BitConverter.GetBytes(size_index));
            return res.ToArray();
        }

        private static byte[] MakeCPFloat(float value)
        {
            var res = new List<byte>();
            res.Add((byte)ConstantPoolType.FLOAT);
            res.AddRange(BitConverter.GetBytes(value));
            return res.ToArray();
        }

        private static byte[] MakeCPInteger(Int32 value)
        {
            var res = new List<byte>();
            res.Add((byte)ConstantPoolType.INTEGER);
            res.AddRange(BitConverter.GetBytes(value));
            return res.ToArray();
        }

        private static byte[] MakeCPInteger64(Int64 value)
        {
            var res = new List<byte>();
            res.Add((byte)ConstantPoolType.INTEGER64);
            res.AddRange(BitConverter.GetBytes(value));
            return res.ToArray();
        }

        private static byte[] MakeCPFunc(UInt16 value, int args_size)
        {
            Debug.Assert(args_size < 256);
            var res = new List<byte>();
            res.Add((byte)ConstantPoolType.FUNCTION);
            res.AddRange(BitConverter.GetBytes(value));
            res.Add((byte)args_size);
            return res.ToArray();
        }

        private static byte[] MakeCPRoutine(UInt16 value, int args_size)
        {
            Debug.Assert(args_size < 256);
            var res = new List<byte>();
            res.Add((byte)ConstantPoolType.ROUTINE);
            res.AddRange(BitConverter.GetBytes(value));
            res.Add((byte)args_size);
            return res.ToArray();
        }

        private static byte[] MakeCPStr(string str)
        {
            var res = new List<byte>();
            res.Add((byte)ConstantPoolType.STRING);
            var bytes = System.Text.Encoding.ASCII.GetBytes(str);
            res.AddRange(BitConverter.GetBytes((UInt16)bytes.Count()));
            res.AddRange(bytes);
            return res.ToArray();
        }

        private void Emit(params byte[] codes)
        {
            foreach (byte code in codes)
            {
                this.codes.Add(code);
            }
        }

        public class Label
        {
            public bool _is_bind { get; private set; }= false;
            private int _pos = 0;
            private List<int> _origin = new List<int>();

            public int Link(int pos)
            {
                if (!_is_bind)
                {
                    _origin.Add(pos);
                    return pos;
                }

                return _pos;
            }

            public List<int> Bind(int pos)
            {
                Debug.Assert(!_is_bind);
                _is_bind = true;
                _pos = pos;
                return _origin;
            }

            public int pos  => _pos;

        }

        public void Bind(Label label, int offset = 0)
        {
            var pos = PC + offset;
            var origins = label.Bind(pos);
            foreach (var origin in origins)
            {
                PatchBranch(origin, pos);
            }
        }

        public int PC
        {
            get { return codes.Count; }
        }

        private UInt16 FindStr(string name)
        {
            if (str_index.ContainsKey(name))
            {

                return (UInt16)str_index[name];
            }
            consts_pool.Add(MakeCPStr(name));

            Debug.Assert(consts_pool.Count <= UInt16.MaxValue);
            var index = (UInt16) (consts_pool.Count - 1);
            str_index[name] = index;
            return  index;
        }

        private UInt16 FindTag(string name, int size)
        {
            if (tag_index.ContainsKey(name))
            {
                return (UInt16)tag_index[name];
            }

            var name_index = FindStr(name);
            consts_pool.Add(MakeCPTag(name_index, (UInt32)size));
            Debug.Assert(consts_pool.Count <= UInt16.MaxValue);
            var index = (UInt16)(consts_pool.Count - 1);
            tag_index[name] = index;
            return index;
        }

        private UInt16 AddCPInteger(int value)
        {
            if (int_index.ContainsKey(value))
            {
                return (UInt16)int_index[value];
            }
            consts_pool.Add(MakeCPInteger(value));
            var index = (UInt16) (consts_pool.Count - 1);
            int_index[value] = index;
            return index;
        }

        private UInt16 AddCPInteger64(Int64 value)
        {
            if (int_index.ContainsKey(value))
            {
                return (UInt16)int_index[value];
            }
            consts_pool.Add(MakeCPInteger64(value));
            var index = (UInt16)(consts_pool.Count - 1);
            int_index[value] = index;
            return index;
        }

        public UInt16 AddCPFloat(float value)
        {
            int index = consts_pool.Count;
            consts_pool.Add(MakeCPFloat(value));
            return Convert.ToUInt16(index);
        }

        private UInt16 AddCPTag(string name, UInt32 size)
        {
            var index = FindStr(name);
            consts_pool.Add(MakeCPTag(Convert.ToUInt16(index), size));
            return Convert.ToUInt16(consts_pool.Count - 1);
        }

        private UInt16 AddCPFunc(string name, int args_size)
        {
            var index = FindStr(name);
            consts_pool.Add(MakeCPFunc(Convert.ToUInt16(index), args_size));
            return Convert.ToUInt16(consts_pool.Count - 1);
        }

        private UInt16 AddCPRoutine(string name, int args_size)
        {
            var index = FindStr(name);
            consts_pool.Add(MakeCPRoutine(Convert.ToUInt16(index), args_size));
            return Convert.ToUInt16(consts_pool.Count - 1);
        }

        public void PatchBranch(int origin, int pos)
        {
            byte opcode = codes[origin ];
            Debug.Assert(opcode == Opcode.JMP ||
                         opcode == Opcode.JEQ ||
                         opcode == Opcode.JNE ||
                         opcode == Opcode.JGE ||
                         opcode == Opcode.JGT ||
                         opcode == Opcode.JLE ||
                         opcode == Opcode.JLT);
            Debug.Assert(Math.Abs(pos - origin) < 1<<30);
            Int32 offset = pos - origin;
            var bytes = BitConverter.GetBytes(offset);
            codes[origin + 1] = bytes[0];
            codes[origin + 2] = bytes[1];
            codes[origin + 3] = bytes[2];
            codes[origin + 4] = bytes[3];
        }

        public void Nop()
        {
            Emit(Opcode.NOP);
        }

        public void BiPush(byte value)
        {
            stack_size += 1;
            Emit(Opcode.BIPUSH, value);
        }

        public void SiPush(Int16 value)
        {
            stack_size += 1;
            var tmp = BitConverter.GetBytes(value);
            Emit(Opcode.SIPUSH, tmp[0], tmp[1]);
        }

        public void Pop()
        {
            Debug.Assert(stack_size >= 1, stack_size.ToString());
            stack_size -= 1;
            Emit(Opcode.POP);
        }

        public void Dup()
        {
            stack_size += 1;
            Emit(Opcode.DUP);
        }

        public void Swap()
        {
            Emit(Opcode.SWAP);
        }

        public void SwapX1()
        {
            Emit(Opcode.SWAP_X1);
        }

        public void Throw()
        {
            stack_size -= 2;
            Emit(Opcode.THROW);
        }

        public void DupX1()
        {
            stack_size += 1;
            Emit(Opcode.DUP_X1);
        }

        public void ConstPnull()
        {
            stack_size += 1;
            Emit(Opcode.CONST_PNULL);
        }

        public void ConstI0()
        {
            stack_size += 1;
            Emit(Opcode.CONST_I0);
        }

        public void ConstI1()
        {
            stack_size += 1;
            Emit(Opcode.CONST_I1);
        }

        public void ConstL0()
        {
            stack_size += 1;
            Emit(Opcode.CONST_L0);
        }

        public void ConstL1()
        {
            stack_size += 1;
            Emit(Opcode.CONST_L1);
        }

        public void ConstM1()
        {
            stack_size += 1;
            Emit(Opcode.CONST_M1);
        }

        public void ConstF0()
        {
            stack_size += 1;
            Emit(Opcode.CONST_F0);
        }

        public void ConstF1()
        {
            stack_size += 1;
            Emit(Opcode.CONST_F1);
        }

        public void ConstD0()
        {
            stack_size += 1;
            Emit(Opcode.CONST_D0);
        }

        public void ConstD1()
        {
            stack_size += 1;
            Emit(Opcode.CONST_D1);
        }

        /*
        public void LoadLocalAddr(UInt16 index)
        {
           
            stack_size += 1;
            var tmp = BitConverter.GetBytes(index);
                Emit(Opcode.LOAD_LOCAL_ADDR, tmp[0], tmp[1]);

        }
        */

        public void CLoad(UInt32 index)
        {
            stack_size += 1;
            var tmp = BitConverter.GetBytes(index);
            Emit(Opcode.CLOAD, tmp[0], tmp[1], tmp[2], tmp[3]);
        }

        public void LoadLocal(UInt16 index)
        {
            stack_size += 1;
            /*
            if (index <= 255)
            {
                Emit(Opcode.LOAD_LOCAL, (byte)index);
            }
            else
            {
            */
                var tmp = BitConverter.GetBytes(index);
                Emit(Opcode.LOAD_LOCAL, tmp[0], tmp[1]);
            /*
            }
            */
        }

        public void StoreLocal(UInt16 index)
        {
            stack_size -= 1;
            /*
            if (index <= 255)
            {
                Emit(Opcode.STORE_LOCAL, (byte)index);
            }
            else
            {
            */
                var tmp = BitConverter.GetBytes(index);
                Emit(Opcode.STORE_LOCAL, tmp[0], tmp[1]);
            /*
            }
            */
        }

        public void LoadInt8Bit()
        {
            Emit(Opcode.LOAD_INT8_BIT);
            stack_size -= 1;
        }
        public void LoadInt16Bit()
        {
            Emit(Opcode.LOAD_INT16_BIT);
            stack_size -= 1;
        }

        public void LoadInt32Bit()
        {
            Emit(Opcode.LOAD_INT32_BIT);
            stack_size -= 1;
        }

        public void LoadInt64Bit()
        {
            Emit(Opcode.LOAD_INT64_BIT);
            stack_size -= 1;
        }

        public void LoadInt8()
        {
            Emit(Opcode.LOAD_INT8);
        }

        public void LoadInt16()
        {
            Emit(Opcode.LOAD_INT16);
        }

        public void LoadInt32()
        {
            Emit(Opcode.LOAD_INT32);
        }

        public void LoadInt64()
        {
            Emit(Opcode.LOAD_INT64);
        }

        public void LoadFloat()
        {
            Emit(Opcode.LOAD_FLOAT);
        }

        public void StoreInt8Bit()
        {
            stack_size -= 3;
            Emit(Opcode.STORE_INT8_BIT);
        }

        public void StoreInt16Bit()
        {
            stack_size -= 3;
            Emit(Opcode.STORE_INT16_BIT);
        }

        public void StoreInt32Bit()
        {
            stack_size -= 3;
            Emit(Opcode.STORE_INT32_BIT);
        }

        public void StoreInt64Bit()
        {
            stack_size -= 3;
            Emit(Opcode.STORE_INT64_BIT);
        }

        public void StoreInt8()
        {
            stack_size -= 2;
            Emit(Opcode.STORE_INT8);
        }

        public void StoreInt16()
        {
            stack_size -= 2;
            Emit(Opcode.STORE_INT16);
        }

        public void StoreInt32()
        {
            stack_size -= 2;
            Emit(Opcode.STORE_INT32);
        }

        public void StoreInt64()
        {
            stack_size -= 2;
            Emit(Opcode.STORE_INT64);
        }

        public void StoreFloat()
        {
            stack_size -= 2;
            Emit(Opcode.STORE_FLOAT);
        }
        public void StoreDouble()
        {
            stack_size -= 2;
            Emit(Opcode.STORE_DOUBLE);
        }

        public void PBInc(byte offset)
        {
            Emit(Opcode.PBINC, offset);
        }

        public void PSINC(Int16 offset)
        {
            var tmp = BitConverter.GetBytes(offset);
            Emit(Opcode.PSINC, tmp[0], tmp[1]);
        }

        public void PAdd()
        {
            stack_size -= 1;
            Emit(Opcode.PADD);
        }

        public void IAdd()
        {
            stack_size -= 1;
            Emit(Opcode.IADD);
        }

        public void LAdd()
        {
            stack_size -= 1;
            Emit(Opcode.LADD);
        }

        public void FAdd()
        {
            stack_size -= 1;
            Emit(Opcode.FADD);
        }

        public void DAdd()
        {
            stack_size -= 1;
            Emit(Opcode.DADD);
        }

        public void ISub()
        {
            stack_size -= 1;
            Emit(Opcode.ISUB);
        }

        public void LSub()
        {
            stack_size -= 1;
            Emit(Opcode.LSUB);
        }

        public void FSub()
        {
            stack_size -= 1;
            Emit(Opcode.FSUB);
        }

        public void DSub()
        {
            stack_size -= 1;
            Emit(Opcode.DSUB);
        }

        public void IMul()
        {
            stack_size -= 1;
            Emit(Opcode.IMUL);
        }

        public void LMul()
        {
            stack_size -= 1;
            Emit(Opcode.LMUL);
        }

        public void FMul()
        {
            stack_size -= 1;
            Emit(Opcode.FMUL);
        }

        public void DMul()
        {
            stack_size -= 1;
            Emit(Opcode.DMUL);
        }

        public void IDiv()
        {
            stack_size -= 1;
            Emit(Opcode.IDIV);
        }

        public void LDiv()
        {
            stack_size -= 1;
            Emit(Opcode.LDIV);
        }

        public void FDiv()
        {
            stack_size -= 1;
            Emit(Opcode.FDIV);
        }

        public void DDiv()
        {
            stack_size -= 1;
            Emit(Opcode.DDIV);
        }

        public void IMod()
        {
            stack_size -= 1;
            Emit(Opcode.IMOD);
        }

        public void LMod()
        {
            stack_size -= 1;
            Emit(Opcode.LMOD);
        }

        public void FMod()
        {
            stack_size -= 1;
            Emit(Opcode.FMOD);
        }

        public void DMod()
        {
            stack_size -= 1;
            Emit(Opcode.DMOD);
        }


        public void IAnd()
        {
            stack_size -= 1;
            Emit(Opcode.IAND);
        }

        public void LAnd()
        {
            stack_size -= 1;
            Emit(Opcode.LAND);
        }

        public void IOr()
        {
            stack_size -= 1;
            Emit(Opcode.IOR);
        }

        public void LOr()
        {
            stack_size -= 1;
            Emit(Opcode.LOR);
        }

        public void IXor()
        {
            stack_size -= 1;
            Emit(Opcode.IXOR);
        }

        public void LXor()
        {
            stack_size -= 1;
            Emit(Opcode.LXOR);
        }

        public void IShiftL()
        {
            stack_size -= 1;
            Emit(Opcode.ISHL);
        }
        public void IShiftR()
        {
            stack_size -= 1;
            Emit(Opcode.ISHR);
        }

        public void LShiftL()
        {
            stack_size -= 1;
            Emit(Opcode.LSHL);
        }
        public void LShiftR()
        {
            stack_size -= 1;
            Emit(Opcode.LSHR);
        }

        public void INot()
        {
            Emit(Opcode.INOT);
        }

        public void LNot()
        {
            Emit(Opcode.LNOT);
        }

        public void BNot()
        {
            Emit(Opcode.BNOT);
        }

        public void ICmp()
        {
            stack_size -= 1;
            Emit(Opcode.ICMP);
        }

        public void LCmp()
        {
            stack_size -= 1;
            Emit(Opcode.LCMP);
        }

        public void FCmp()
        {
            stack_size -= 1;
            Emit(Opcode.FCMP);
        }

        public void DCmp()
        {
            stack_size -= 1;
            Emit(Opcode.DCMP);
        }

        public void INeg()
        {
            Emit(Opcode.INEG);
        }

        public void LNeg()
        {
            Emit(Opcode.LNEG);
        }

        public void FNeg()
        {
            Emit(Opcode.FNEG);
        }

        public void DNeg()
        {
            Emit(Opcode.DNEG);
        }

        public void I2L()
        {
            Emit(Opcode.I2L);
        }

        public void I2F()
        {
            Emit(Opcode.I2F);
        }

        public void I2D()
        {
            Emit(Opcode.I2D);
        }

        public void L2I()
        {
            Emit(Opcode.L2I);
        }

        public void L2F()
        {
            Emit(Opcode.L2F);
        }

        public void L2D()
        {
            Emit(Opcode.L2D);
        }


        public void I2B()
        {
            Emit(Opcode.I2B);
        }

        public void I2S()
        {
            Emit(Opcode.I2S);
        }

        public void F2I()
        {
            Emit(Opcode.F2I);
        }

        public void F2L()
        {
            Emit(Opcode.F2L);
        }

        public void F2D()
        {
            Emit(Opcode.F2D);
        }

        public void D2I()
        {
            Emit(Opcode.D2I);
        }

        public void D2L()
        {
            Emit(Opcode.D2L);
        }

        public void D2F()
        {
            Emit(Opcode.D2F);
        }

        public void Call(UInt16 index, int args_size)
        {
            stack_size -= args_size;
            stack_size += 1;
            var tmp = BitConverter.GetBytes(index);
            Emit(Opcode.CALL, tmp[0], tmp[1]);
        }

        public void Ret(byte value = 0)
        {
            stack_size += 1;
            Emit(Opcode.RET, value);
        }

        public void Jmp(Int32 offset)
        {
            var tmp = BitConverter.GetBytes(offset);
            Emit( Opcode.JMP, tmp[0], tmp[1], tmp[2], tmp[3]);
        }

        public void Jeq(Int32 offset)
        {
            stack_size -= 1;
            var tmp = BitConverter.GetBytes(offset);
            Emit(Opcode.JEQ, tmp[0], tmp[1], tmp[2], tmp[3]);
        }

        public void Jne(Int32 offset)
        {
            stack_size -= 1;

            var tmp = BitConverter.GetBytes(offset);
            Emit( Opcode.JNE, tmp[0], tmp[1], tmp[2], tmp[3]);
        }

        public void Jge(Int32 offset)
        {
            stack_size -= 1;

            var tmp = BitConverter.GetBytes(offset);
            Emit( Opcode.JGE, tmp[0], tmp[1], tmp[2], tmp[3]);
        }

        public void Jgt(Int32 offset)
        {
            stack_size -= 1;

            var tmp = BitConverter.GetBytes(offset);
            Emit(Opcode.JGT, tmp[0], tmp[1], tmp[2], tmp[3]);
        }

        public void Jle(Int32 offset)
        {
            stack_size -= 1;

            var tmp = BitConverter.GetBytes(offset);
            Emit(Opcode.JLE, tmp[0], tmp[1], tmp[2], tmp[3]);
        }

        public void Jlt(Int32 offset)
        {
            stack_size -= 1;

            var tmp = BitConverter.GetBytes(offset);
            Emit(Opcode.JLT, tmp[0], tmp[1], tmp[2], tmp[3]);
        }

        public void Wide()
        {
            throw new NotImplementedException();
        }

        public void breakpoint()
        {
            Emit(Opcode.BREAKPOINT);
        }

        public void ECheck()
        {
            Emit(Opcode.ECHECK);
        }

        //Macro Instruction
        public void CLoadInteger(BigInteger value)
        {
            if (value == 0)
            {
                ConstI0();
            } else if (value == 1)
            {
                ConstI1();
            } else if (value == -1)
            {
                ConstM1();
            }
            else if (value >= Int32.MinValue && value <= Int32.MaxValue)
            {
                var index = AddCPInteger((Int32)value);
                CLoad(index);
            } else if (value >= Int64.MinValue && value <= Int64.MaxValue)
            {
                var index = AddCPInteger64((Int64)value);
                CLoad(index);
            }
            else
            {
                Debug.Assert(false, value.ToString());
            }
        }

        public void CLoadString(string value)
        {
            var index = FindStr(value);
            CLoad(index);
        }

        public void CLoadFloat(float value)
        {
            if (value == 0.0)
            {
                ConstF0();
            }
            else
            {
                var index = AddCPFloat(value);
                CLoad(index);
            }
        }

        public void CLoadTag(string name, UInt32 size = 0)
        {
            Debug.Assert(size != 0, name);
            var index = FindTag(name, (int)size);
            CLoad(index);
        }

        public void JmpL(Label label)
        {
            var curr = PC;
            var pos = label.Link(curr);
            Debug.Assert(Math.Abs(pos - curr) < 1 << 30);
            Jmp(pos - curr);
        }

        public void JeqL(Label label)
        {
            var curr = PC;
            var pos = label.Link(curr);
            Debug.Assert(Math.Abs(pos - curr) < 1 << 30);
            Jeq(pos - curr);
        }

        public void JneL(Label label)
        {
            var curr = PC;
            var pos = label.Link(curr);
            Debug.Assert(Math.Abs(pos - curr) < 1 << 30);
            Jne(pos - curr);
        }

        public void JgeL(Label label)
        {
            var curr = PC;
            var pos = label.Link(curr);
            Debug.Assert(Math.Abs(pos - curr) < 1 << 30);
            Jge(pos - curr);
        }

        public void JgtL(Label label)
        {
            var curr = PC;
            var pos = label.Link(curr);
            Debug.Assert(Math.Abs(pos - curr) < 1 << 30);
            Jgt(pos - curr);
        }

        public void JleL(Label label)
        {
            var curr = PC;
            var pos = label.Link(curr);
            Debug.Assert(Math.Abs(pos - curr) < 1 << 30);
            Jle(pos - curr);
        }

        public void JltL(Label label)
        {
            var curr = PC;
            var pos = label.Link(curr);
            Debug.Assert(Math.Abs(pos - curr) < 1 << 30);
            Jlt(pos - curr);
        }

        public void Eq(IDataType type)
        {
            Cmp(type);
            var else_label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();

            JeqL(else_label);
            ConstI0();
            JmpL(exit_label);
            Bind(else_label);
            ConstI1();
            Bind(exit_label);

            stack_size -= 1;
        }

        public void Ne(IDataType type)
        {
            Cmp(type);
            var else_label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();

            JneL(else_label);
            ConstI0();
            JmpL(exit_label);
            Bind(else_label);
            ConstI1();
            Bind(exit_label);

            stack_size -= 1;
        }

        public void Ge(IDataType type)
        {
            Cmp(type);
            var else_label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();

            JgeL(else_label);
            ConstI0();
            JmpL(exit_label);
            Bind(else_label);
            ConstI1();
            Bind(exit_label);

            stack_size -= 1;
        }

        public void Gt(IDataType type)
        {
            Cmp(type);
            var else_label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();

            JgtL(else_label);
            ConstI0();
            JmpL(exit_label);
            Bind(else_label);
            ConstI1();
            Bind(exit_label);

            stack_size -= 1;
        }

        public void Le(IDataType type)
        {
            Cmp(type);
            var else_label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();

            JleL(else_label);
            ConstI0();
            JmpL(exit_label);
            Bind(else_label);
            ConstI1();
            Bind(exit_label);

            stack_size -= 1;
        }

        public void Lt(IDataType type)
        {
            Cmp(type);
            var else_label = new MacroAssembler.Label();
            var exit_label = new MacroAssembler.Label();

            JltL(else_label);
            ConstI0();
            JmpL(exit_label);
            Bind(else_label);
            ConstI1();
            Bind(exit_label);

            stack_size -= 1;
        }

        public void Neg(IDataType type)
        {
            if (type is REAL)
            {
                FNeg();
            } else if (type is DINT)
            {
                INeg();
            } else if (type is LINT)
            {
                LNeg();
            } else if (type is LREAL)
            {
                DNeg();
            } else
            {
                Debug.Assert(false);
            }
        }

        public void Neg(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.DINT:
                    INeg();
                    break;
                case PrimitiveType.LINT:
                    LNeg();
                    break;
                case PrimitiveType.REAL:
                    FNeg();
                    break;
                case PrimitiveType.LREAL:
                    DNeg();
                    break;
            }
        }

        public void And(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.DINT:
                    IAnd();
                    break;
                case PrimitiveType.LINT:
                    LAnd();
                    break;
                default:
                    Debug.Assert(false);
                    break;                
            }
        }

        public void ShiftL(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.DINT:
                    IShiftL();
                    break;
                case PrimitiveType.LINT:
                    LShiftL();
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        public void ShiftR(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.DINT:
                    IShiftR();
                    break;
                case PrimitiveType.LINT:
                    LShiftR();
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        public void Inv(IDataType type)
        {
            if (type is DINT)
            {
                INot();
            } else if (type is LINT)
            {
                LNot();
            } else if (type is BOOL)
            {
                BNot();
            }
            else
            {
                Debug.Assert(false);
            }
        }

        public void StoreBit(PrimitiveType type, int offset)
        {
            CLoadInteger(offset);
            Swap();
            switch (type)
            {
                case PrimitiveType.SINT:
                    StoreInt8Bit();
                    break;
                case PrimitiveType.INT:
                    StoreInt16Bit();
                    break;
                case PrimitiveType.DINT:
                    StoreInt32Bit();
                    break;
                case PrimitiveType.LINT:
                    StoreInt64Bit();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Store(IDataType name_type, IDataType value_type)
        {

            if (name_type is SINT)
            {
                ToDINT(value_type);
                StoreInt8();
            }
            else if (name_type is INT)
            {
                ToDINT(value_type);
                StoreInt16();
            }
            else if (name_type is DINT)
            {
                ToDINT(value_type);
                StoreInt32();
            }
            else if (name_type is LINT)
            {
                ToLINT(value_type);
                StoreInt64();
            }
            else if (name_type is REAL)
            {
                ToREAL(value_type);
                StoreFloat();
            }
            else if (name_type is LREAL)
            {
                ToLREAL(value_type);
                StoreDouble();
            }
            else if (name_type.IsBool)
            {

                var type = (name_type as BOOL).RefDataType;
                if (type is SINT)
                {
                    StoreInt8Bit();
                }
                else if (type is INT)
                {
                    StoreInt16Bit();
                }
                else if (type is DINT)
                {
                    StoreInt32Bit();
                }
                else if (type is LINT)
                {
                    StoreInt64Bit();
                }
                else
                {
                    Debug.Assert(false);
                    StoreInt32Bit();
                }

            }
            else
            {
                Debug.Assert(false, name_type.ToString());
            }
        }

        public void Load(IDataType type)
        {
            if (type is SINT)
            {
                LoadInt8();
            }
            else if (type is INT)
            {
                LoadInt16();
            }
            else if (type is DINT)
            {
                LoadInt32();
            }
            else if (type is LINT)
            {
               LoadInt64();
            }
            else if (type is REAL)
            {
                LoadFloat();
            }
            else if (type is BOOL)
            {
                var t = (type as BOOL).RefDataType;
                if (t is SINT)
                {
                    LoadInt8Bit();
                }
                else if (t is INT)
                {
                    LoadInt16Bit();
                }
                else if (t is DINT)
                {
                    LoadInt32Bit();
                }
                else if (t is LINT)
                {
                    LoadInt64Bit();
                }
                else
                {
                    Debug.Assert(false);
                    LoadInt32Bit();
                }
            }
            else
            {
                Debug.Assert(false, type.ToString());
                //USE ADDRESS DIRECTLY
            }
        }

        /*
        public void LoadBit(IDataType type, byte offset)
        {
            if (type is SINT)
            {
                LoadInt8Bit(offset);
            } else if (type is INT)
            {
                LoadInt16Bit(offset);
            } else if (type is DINT)
            {
                LoadInt32Bit(offset);
            } else if (type is LINT)
            {
                LoadInt64Bit(offset);
            }
            else
            {
                Debug.Assert(false);
            }
          
        }

        public void LoadBit(PrimitiveType type, int offset)
        {
            CLoadInteger(offset);
            switch (type)
            {
                case PrimitiveType.SINT:
                    LoadInt8Bit();
                    break;
                case PrimitiveType.INT:
                    LoadInt16Bit();
                    break;
                case PrimitiveType.DINT:
                    LoadInt32Bit();
                    break;
                case PrimitiveType.LINT:
                    LoadInt64Bit();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
          */

        public enum PrimitiveType
        {
            SINT,
            INT,      
            DINT,
            LINT,
            REAL,
            LREAL,
        }

        private void ToLREAL(IDataType from)
        {
            if (from is LINT)
            {
                L2D();
            } else if (from.IsInteger)
            {
                I2D();
            } else if (from is REAL)
            {
                F2D();
            }
            else
            {
                Debug.Assert(from is LREAL, from.ToString());
            }
        }

        private void ToREAL(IDataType from)
        {
            if (from is LINT)
            {
                L2F();
            } else if (from.IsInteger)
            {
                I2F();
            } else if (from is LREAL)
            {
                D2F();
            }
            else
            {
                Debug.Assert(from is REAL, from.ToString());
            }
        }


        private void ToLINT(IDataType from)
        {
            if (from is LINT)
            {
                return;
            } else if (from.IsInteger)
            {
                I2L();
            } else if (from is REAL)
            {
                F2L();
            } else if (from is LREAL)
            {
                D2L();
            }
            else
            {
                Debug.Assert(from is LINT, from.ToString());
            }
        }

        private void ToDINT(IDataType from)
        {
            if (from is LINT)
            {
                L2I();
            }
            else if (from.IsInteger)
            {

            }
            else if (from is REAL)
            {
                F2I();
            }
            else if (from is LREAL)
            {
                D2I();
            }
            else
            {
                Debug.Assert(false, from.ToString());
            }
        }


        public void ArithConv(IDataType from, IDataType to)
        {
            if (to is LREAL)
            {
                ToLREAL(from);
                return;
            } else if (to is REAL)
            {
                ToREAL(from);
                return;
            } else if (to is LINT)
            {
                ToLINT(from);
                return;
            } else if (to.IsInteger)
            {
                ToDINT(from);
                return;
            }

            Debug.Assert(false);

            /*
            if (from.IsInteger && to.IsInteger)
                return;
            if (from.IsReal && to.IsReal)
                return;
            if (from.IsReal && to.IsInteger)
            {
                F2I();
                return;
            }

            if (from.IsInteger && to.IsReal)
            {
                I2F();
                return;
            }
            */

        }

        public void RelConv(IDataType from, IDataType to)
        {
             ArithConv(from, to);
            /*
            if (from.IsInteger && to.IsInteger)
            {
                return;
            }

            if (from.IsReal && to.IsReal)
            {
                return;
            }

            if (from.IsReal && to.IsInteger)
            {
                F2I();
                return;
            }

            if (from.IsInteger && to.IsReal)
            {
                I2F();
                return;
            }
            */

        }

        public void LogicConv(IDataType from, IDataType to)
        {

            if (to is DINT)
            {
                if (from is LREAL)
                {
                    D2I();
                } else if (from is REAL)
                {
                    F2I();
                }
                else if (from is SINT)
                {
                    I2B();
                } else if (from is INT)
                {
                    I2S();
                } else if (from is LINT)
                {
                    L2I();
                }
                else
                {
                    Debug.Assert(from is DINT);
                }
            }
            else
            {
                Debug.Assert(from.IsBool && to.IsBool, $"{from} to {to}");
            }

        }

        private void ToDINT(PrimitiveType tp)
        {
            switch (tp)
            {
                case PrimitiveType.DINT:
                    break;
                case PrimitiveType.LINT:
                    L2I();
                    break;
                case PrimitiveType.REAL:
                    F2I();
                    break;
                case PrimitiveType.LREAL:
                    D2I();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ToLINT(PrimitiveType tp)
        {
            switch (tp)
            {
                case PrimitiveType.DINT:
                    I2L();
                    break;
                case PrimitiveType.LINT:
                    break;
                case PrimitiveType.REAL:
                    F2L();
                    break;
                case PrimitiveType.LREAL:
                    D2L();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ToREAL(PrimitiveType tp)
        {
            switch (tp)
            {
                case PrimitiveType.DINT:
                    I2F();
                    break;
                case PrimitiveType.LINT:
                    L2F();
                    break;
                case PrimitiveType.REAL:
                    break;
                case PrimitiveType.LREAL:
                    D2F();
                    break;
                default:
                    throw new NotImplementedException();

            }
        }

        private void ToLREAL(PrimitiveType tp)
        {
            switch (tp)
            {
                case PrimitiveType.DINT:
                    I2D();
                    break;
                case PrimitiveType.LINT:
                    L2D();
                    break;
                case PrimitiveType.REAL:
                    F2D();
                    break;
                case PrimitiveType.LREAL:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void TypeConv(PrimitiveType from, PrimitiveType to)
        {
            switch (to)
            {
                case PrimitiveType.DINT:
                    ToDINT(from);
                    break;
                case PrimitiveType.LINT:
                    ToLINT(from);
                    break;
                case PrimitiveType.REAL:
                    ToREAL(from);
                    break;
                case PrimitiveType.LREAL:
                    ToLREAL(from);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Add(IDataType type)
        {
            if (type is REAL)
            {
                FAdd();
            } else if (type is DINT)
            {
                IAdd();
            } else if (type is LINT)
            {
                LAdd();
            } else if (type is LREAL)
            {
                DAdd();
            }
            else
            {
                Debug.Assert(false);
            }
        }

        public void Add(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.DINT:
                    IAdd();
                    break;
                case PrimitiveType.LINT:
                    LAdd();
                    break;
                case PrimitiveType.REAL:
                    FAdd();
                    break;
                case PrimitiveType.LREAL:
                    DAdd();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Or(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.DINT:
                    IOr();
                    break;
                case PrimitiveType.LINT:
                    LOr();
                    break;
                default:
                    Debug.Assert(false, type.ToString());
                    break;
            }
        }

        public void Xor(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.DINT:
                    IXor();
                    break;
                case PrimitiveType.LINT:
                    LXor();
                    break;
                default:
                    Debug.Assert(false, type.ToString());
                    break;
            }
        }

        public void Sub(IDataType type)
        {
            if (type is REAL)
            {
                FSub();
            } else if (type is DINT)
            {
                ISub();
            } else if (type is LINT)
            {
                LSub();
            } else if (type is LREAL)
            {
                DSub();
            }
            else
            {
                Debug.Assert(false);
            }
        }

        public void Sub(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.DINT:
                    ISub();
                    break;
                case PrimitiveType.LINT:
                    LSub();
                    break;
                case PrimitiveType.REAL:
                    FSub();
                    break;
                case PrimitiveType.LREAL:
                    DSub();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Mul(IDataType type)
        {
            if (type is REAL)
            {
                FMul();
            } else if (type is DINT)
            {
                IMul();
            } else if (type is LINT)
            {
                LMul();
            } else if (type is LREAL)
            {
                DMul();
            }
            else
            {
                Debug.Assert(false);
            }
        }

        public void Mul(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.DINT:
                    IMul();
                    break;
                case PrimitiveType.LINT:
                    LMul();
                    break;
                case PrimitiveType.REAL:
                    FMul();
                    break;
                case PrimitiveType.LREAL:
                    DMul();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Div(IDataType type)
        {
            if (type is REAL)
            {
                FDiv();
            } else if (type is DINT)
            {
                IDiv();
            } else if (type is LINT)
            {
                LDiv();
            } else if (type is LREAL)
            {
                DDiv();
            } else
            {
                Debug.Assert(false);
            }
        }

        public void Div(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.DINT:
                    IDiv();
                    break;
                case PrimitiveType.LINT:
                    LDiv();
                    break;
                case PrimitiveType.REAL:
                    FDiv();
                    break;
                case PrimitiveType.LREAL:
                    DDiv();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Mod(IDataType type)
        {
            if (type is REAL)
            {
                FMod();
            } else if (type is DINT)
            {
                IMod();
            } else if (type is LINT)
            {
                LMod();
            } else if (type is LREAL)
            {
                DMod();
            } else
            {
                Debug.Assert(false);
            }
        }

        public void Mod(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.DINT:
                    IMod();
                    break;
                case PrimitiveType.LINT:
                    LMod();
                    break;
                case PrimitiveType.REAL:
                    FMod();
                    break;
                case PrimitiveType.LREAL:
                    DMod();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Cmp(IDataType type)
        {
            if (type is REAL)
            {
                FCmp();
            }
            else if (type is DINT || type is BOOL)
            {
                ICmp();
            } else if (type is LINT)
            {
                LCmp();
            } else if (type is LREAL)
            {
                DCmp();
            }
            else
            {
                Debug.Assert(false, type.ToString());
            }
        }

        public void Abs(PrimitiveType type)
        {
            Dup();
            switch (type)
            {
                case PrimitiveType.DINT:                    
                    ConstI0();
                    ICmp();
                    break;
                case PrimitiveType.LINT:
                    ConstL0();
                    LCmp();
                    break;
                case PrimitiveType.REAL:
                    ConstF0();
                    FCmp();
                    break;
                case PrimitiveType.LREAL:
                    ConstD0();
                    DCmp();
                    break;
                default:
                    throw new NotImplementedException();
            }

            var exit = new Label();
            JgtL(exit);
            Neg(type);
            Bind(exit);
        }

        /*

        public void Min(PrimitiveType type)
        {
            var exit = new Label();
            Cmp(type);

            Bind(exit);
            

        }
        */

        

        public void Cmp(PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.DINT:
                    ICmp();
                    break;
                case PrimitiveType.LINT:
                    LCmp();
                    break;
                case PrimitiveType.REAL:
                    FCmp();
                    break;
                case PrimitiveType.LREAL:
                    DCmp();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void CallToDint(IDataType type)
        {
            if (type.IsReal)
            {
                F2I();
            }
            else
            {
                Debug.Assert(type.IsInteger);
            }
        }

        private void CallToReal(IDataType type)
        {
            if (type.IsInteger)
            {
                I2F();
            }
            else
            {
                Debug.Assert(type.IsReal, type.ToString());
            }
        }

        public void CallTypeConv(IDataType from, IDataType to)
        {
            if (to is LINT)
            {
                ToLINT(from);
            } else if (to.IsInteger)
            {
                ToDINT(from);
            } else if (to is LREAL)
            {
                ToLREAL(from);
            } else if (to is REAL)
            {
                ToREAL(from);
            }

            return;
            /*
            if (to.IsInteger)
            {
                CallToDint(from);
            }
            else if (to.IsReal)
            {
                CallToReal(from);
            }
            else if (to.IsBool)
            {
                Debug.Assert(from.IsInteger || from.IsBool);
            } else if (from is STRING && to is STRING)
            {
                //FIXME Handle this
            }else
            {
                Debug.Assert(false, $"{from} to {to}");
            }
            */
        }

        public void CallName(string name, int argsSize)
        {
            var index = AddCPFunc(name, argsSize);
            Call(index, argsSize);
        }

        public void CallRoutine(string name, int argsSize)
        {
            var index = AddCPRoutine(name, argsSize);
            Call(index, argsSize);
            ECheck();
        }

        public void RawCallRoutine(string name, int argsSize)
        {
            var index = AddCPRoutine(name, argsSize);
            Call(index, argsSize);
        }


        public void If(Action cond, Action lhs, Action rhs)
        {
            cond();
            var else_label = new Label();
            var exit_label = new Label();
            JeqL(else_label);
            lhs();
            JmpL(exit_label);
            Bind(else_label);
            rhs();
            Bind(exit_label);
        }

        private List<byte[]> consts_pool;
        public List<byte> codes = new List<byte>();
        public int stack_size {
            get {
                Debug.Assert(_stack_size >= 0);
                return _stack_size;
            }
            set
            {
                Debug.Assert(value >= 0, value.ToString());
                _stack_size = value;
            }

        }
        private int _stack_size = 0;
    }
}
