using System.Collections.Generic;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.Interfaces.Instruction
{
    public class InstructionInfo
    {
        public string Name { get; set; }
        public string Mnemonic { get; set; }
        public string Description { get; set; }
        public DataTypeInfo ReturnType { get; set; }
        public IEnumerable<InstructionParameterInfo> Parameters { get; set; }

        public static DataTypeInfo VoidTypeInfo = new DataTypeInfo() {DataType = null, Dim1 = 0, Dim2 = 0, Dim3 = 0};
    }
}
