using System.Diagnostics;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace ICSStudio.FileConverter.L5XToJson
{
    public partial class Converter
    {
        private static JArray ToTrends(XmlElement node)
        {
            if (node == null) return null;

            var res = new JArray();
            var list = node.GetElementsByTagName("Trend");
            foreach (XmlElement t in list)
            {
                res.Add(ToTrend(t));
            }

            return res;
        }

        private static JObject ToTrend(XmlElement node)
        {
            var res = new JObject();

            res.AddOrIgnore(node, "Name");
            res.AddOrIgnore<int>(node, "SamplePeriod");
            res.AddOrIgnore<int>(node, "NumberOfCaptures");
            res.AddOrIgnore(node, "CaptureSizeType");
            res.AddOrIgnore<int>(node, "CaptureSize");
            res.AddOrIgnore(node, "Use");
            ParseTriggerType(res, node, "StartTriggerType");
            ParseTriggerType(res, node, "StopTriggerType");

            res.AddOrIgnore(node, "TrendxVersion");

            var pens = node.SelectSingleNode("Pens") as XmlElement;
            if (pens != null)
                res.Add("Pens", ToPens(pens));

            return res;
        }

        private static JArray ToPens(XmlElement node)
        {

            var res = new JArray();
            if (node == null)
            {
                return res;
            }

            var list = node.GetElementsByTagName("Pen");
            foreach (XmlElement t in list)
            {
                res.Add(ToPen(t));
            }

            return res;
        }

        private static JObject ToPen(XmlElement node)
        {
            var res = new JObject();

            res.AddOrIgnore(node, "Name");
            res.AddOrIgnore(node, "Color");
            res.AddOrIgnore<bool>(node, "Visible");
            res.AddOrIgnore<int>(node, "Style");
            res.AddOrIgnore(node, "Type");
            res.AddOrIgnore<int>(node, "Width");
            res.AddOrIgnore<int>(node, "Marker");

            res.AddOrIgnore<float>(node, "Min");
            res.AddOrIgnore<float>(node, "Max");
            res.AddOrIgnore(node, "EngUnits");
            return res;
        }


        private static void ParseTriggerType(JObject res, XmlElement node, string key)
        {
            int type = 0;
            var value = node.Attributes[key].Value;
            if (value == "No Trigger")
            {
                type = 0;
            }
            else
            {
                Debug.Assert(false);
            }

            res.Add(key, type);
        }
    }
}
