using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.Instruction.Instructions;
using ICSStudio.SimpleServices.PredefinedType;
using System.Diagnostics;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.SimpleServices.Compiler
{
    enum SlotDataType
    {
        UNKNOWN,
        INT32,
        INT64,
        FLOAT,
        DOUBLE,
        POINTER,
    }
    class Utils
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
                yield return Tuple.Create(code, codes.GetRange(index + 1, arg_size), index);
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
            if (instr == null)
            {
                Controller.GetInstance().Log($"GetFunctionInfo({name})");
            }
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
}
