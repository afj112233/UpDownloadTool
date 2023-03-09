using System.Collections.Generic;
using System.Linq;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.SimpleServices.Instruction
{
    public partial class RLLInstructionCollection
    {

        private readonly Controller _parentController;
        private readonly Dictionary<string, IXInstruction> _instrs = new Dictionary<string, IXInstruction>();

        public RLLInstructionCollection(Controller ctrl)
        {
            _parentController = ctrl;

            Init();
        }

        public IXInstruction FindInstruction(string name)
        {
            name = name.ToUpper();
            return _instrs.ContainsKey(name) ? _instrs[name] : null;
        }

        public void RemoveInstruction(IXInstruction instruction)
        {
            if (_instrs.ContainsValue(instruction))
                _instrs.Remove(instruction.Name);
        }

        public void AddInstruction(IXInstruction instruction)
        {
            if (!_instrs.ContainsValue(instruction))
                _instrs.Add(instruction.Name, instruction);
        }

        private List<string> _instructions;
        public List<string> Instructions
        {
            get
            {
                if (_instructions == null)
                    _instructions = _instrs.Keys.ToList();

                return _instructions;
            }
        }
    }

}
