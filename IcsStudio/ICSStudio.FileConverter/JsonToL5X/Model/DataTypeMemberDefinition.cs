using ICSStudio.Interfaces.DataType;

namespace ICSStudio.FileConverter.JsonToL5X.Model
{
    public class DataTypeMemberDefinition
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool Hidden { get; set; }
        public DisplayStyle Radix { get; set; }

        public string Dimension { get; set; }

        public ExternalAccess ExternalAccess { get; set; }

        public int BitNumber { get; set; }
        public int LowBit { get; set; }
        public int HiBit { get; set; }
        public string Enum { get; set; }
        public string Target { get; set; }

        public string Min { get; set; }
        public string Max { get; set; }

        public string Description { get; set; }
    }
}
