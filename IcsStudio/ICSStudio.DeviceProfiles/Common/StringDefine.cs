using System.Collections.Generic;

namespace ICSStudio.DeviceProfiles.Common
{
    public class StringDefine
    {
        public string ID { get; set; }
        public List<StringDefineItem> Strings { get; set; }

        public string GetString(int stringID, int lcid)
        {
            foreach (var item in Strings)
                if (stringID == item.ID)
                    foreach (var description in item.Descriptions)
                        if (description.LCID == lcid)
                            return description.Text;

            return string.Empty;
        }
    }

    public class StringDefineItem
    {
        public int ID { get; set; }
        public List<Description> Descriptions { get; set; }
    }
}
