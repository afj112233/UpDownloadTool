using System.Collections.Generic;
using System.Collections.ObjectModel;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.Interfaces.Instruction
{
    public interface ISupportedInstructions
    {
        ReadOnlyCollection<string> SupportedBuiltInInstructionMnemonics { get; }
        InstructionInfo GetInstructionInfo(string instructionMnemonic);

        List<InstructionInfo> GetCandidateInstructions(string name);
        InstructionInfo GetFirstMatchInstructionInfo(string name, DataTypeInfo[] parameters);
    }
}
