using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.Common
{
    [SuppressMessage("ReSharper", "UseObjectOrCollectionInitializer")]
    public class TimeSetting
    {
        public int Priority1 { get; set; }
        public int Priority2 { get; set; }
        public bool PTPEnable { get; set; }

        public JObject ConvertToJObject()
        {
            var res = new JObject();
            res["Priority1"] = Priority1;
            res["Priority2"] = Priority2;
            res["PTPEnable"] = PTPEnable;
            return res;
        }
    }
}