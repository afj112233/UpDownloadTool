using System.Collections.Generic;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Utils;
using Newtonsoft.Json.Linq;

namespace ICSStudio.DeviceProfiles.Common
{
    public class DataTypeDefinition
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public List<DataTypeMemberDefinition> Members { get; set; }

        public JObject ToJObject()
        {
            var members = new JArray();

            foreach (var memberDefinition in Members)
            {
                var member = new JObject();

                member.Add("Name", memberDefinition.Name);
                member.Add("DataType", memberDefinition.DataType);

                if (memberDefinition.Hidden)
                    member.Add("Hidden", true);

                member.Add("Radix", (int) memberDefinition.Radix);

                var dim = 0;
                if (memberDefinition.Dimension != null)
                    dim = int.Parse(memberDefinition.Dimension);

                if (dim > 0)
                    member.Add("Dimension", dim);

                if (memberDefinition.DataType == "BIT")
                {
                    member.Add("BitNumber", memberDefinition.BitNumber);
                    member.Add("Target", memberDefinition.Target);
                }

                members.Add(member);
            }

            return new JObject
            {
                {"Name", Name},
                {"Class", (int) EnumUtils.Parse<DataTypeClass>(Class)},
                {"Members", members}
            };
        }


        public string GetEnumByMember(string memberName)
        {
            foreach (var member in Members)
            {
                if (member.Name.Equals(memberName))
                    return member.Enum;
            }

            return string.Empty;
        }
    }

    public class DataTypeMemberDefinition
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool Hidden { get; set; }
        public DisplayStyle Radix { get; set; }

        public string Dimension { get; set; }

        public int BitNumber { get; set; }
        public int LowBit { get; set; }
        public int HiBit { get; set; }
        public string Enum { get; set; }
        public string Target { get; set; }

        public string Min { get; set; }
        public string Max { get; set; }
    }
}
