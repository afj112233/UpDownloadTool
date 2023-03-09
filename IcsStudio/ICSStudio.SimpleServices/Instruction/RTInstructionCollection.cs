using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Common;
using System;
using System.Diagnostics;

namespace ICSStudio.SimpleServices.Instruction
{
    public class RTInstructionInfo {
        public string name { get; }
        public IDataType return_type { get; }
        public List<IDataType> param_types { get; }
        public List<bool> is_ref { get; }

        public RTInstructionInfo(string name, IDataType return_type, List<IDataType> param_types, List<bool> is_ref)
        {
            this.name = name;
            this.return_type = return_type;
            this.param_types = param_types;
            this.is_ref = is_ref;
        }
    }

    public partial class RTInstructionCollection
    {
        private readonly Dictionary<string, RTInstructionInfo> _instrs = new Dictionary<string, RTInstructionInfo>();

        private RTInstructionCollection()
        {
            Init();
        }

        public static readonly RTInstructionCollection Inst = new RTInstructionCollection();

        public RTInstructionInfo FindInstruction(string name)
        {
            return _instrs[name.ToUpper()];
        }

        public void Add(string name, RTInstructionInfo info)
        {
            name = name.ToUpper();
            Debug.Assert(!_instrs.ContainsKey(name));
            _instrs[name] = info;
        }

        public void Remove(string name)
        {
            name = name.ToUpper();
            Debug.Assert(_instrs.ContainsKey(name));
            _instrs.Remove(name);
        }
    }
}