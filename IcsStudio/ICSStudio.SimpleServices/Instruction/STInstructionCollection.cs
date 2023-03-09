using System;
using System.Collections.Generic;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.SimpleServices.Instruction
{

    public partial class STInstructionCollection
    {

        private readonly Dictionary<string, IXInstruction> _instrs = new Dictionary<string, IXInstruction>(StringComparer.OrdinalIgnoreCase);

        public STInstructionCollection(Controller ctrl)
        {
            Init();
        }

        public IXInstruction FindInstruction(string name)
        {
            return _instrs.ContainsKey(name) ? _instrs[name] : null;
        }

        public void RemoveInstruction(IXInstruction instruction)
        {
            if (_instrs.ContainsValue(instruction))
                _instrs.Remove(instruction.Name);
        }

        public void AddInstruction(IXInstruction instruction)
        {
            if(!_instrs.ContainsValue(instruction))
                _instrs.Add(instruction.Name,instruction);
        }
    }

}
