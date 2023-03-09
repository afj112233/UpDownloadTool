using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.Interfaces.Common
{
    public class TagDisplayInfo
    {
        public string DataTypeName;
        public int DisplayStyle;
        public int FirstDim;
        public int SecondDim;
        public int ThirdDim;
        public bool IsArray;
        public bool IsStructure;
        public bool IsPublished;
        public bool IsVerified;
        public bool IsForcible;
        public bool IsIO;
        public bool IsSequencing;
        public bool IsHistorized;
        public string ComponentSpecifer;
        public string ExtendedComponentSpecifer;
        public string Description;
        public Usage ParameterType;
        public ExternalAccess ExternalAccess;
    }
}
