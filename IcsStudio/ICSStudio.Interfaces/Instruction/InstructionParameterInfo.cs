using ICSStudio.Interfaces.Tags;

namespace ICSStudio.Interfaces.Instruction
{
    public class InstructionParameterInfo
    {
        public DataTypeInfo DataTypeInfo { get; set; }
        public bool IsRef { get; set; }
        public string Name { get; set; }
        public ParameterAccessType AccessType { get; set; }
    }
}
