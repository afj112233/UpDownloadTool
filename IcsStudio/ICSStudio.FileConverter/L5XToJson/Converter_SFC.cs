using System.Diagnostics;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace ICSStudio.FileConverter.L5XToJson
{
    public partial class Converter
    {
        private static JArray ToSFCContent(XmlNode xmlNode)
        {
            if (xmlNode.HasChildNodes)
            {
                var jarray = new JArray();
                foreach (XmlElement childNode in xmlNode.ChildNodes)
                {
                    if (childNode.Name.Equals("Step"))
                    {
                        jarray.Add(ToStep(childNode));
                    }
                    else if (childNode.Name.Equals("Transition"))
                    {
                        jarray.Add(ToTransition(childNode));
                    }
                    else if (childNode.Name.Equals("Branch"))
                    {
                        jarray.Add(ToBranch(childNode));
                    }
                    else if (childNode.Name.Equals("Stop"))
                    {
                        jarray.Add(ToStop(childNode));
                    }
                    else if (childNode.Name.Equals("DirectedLink"))
                    {
                        jarray.Add(ToDirectedLink(childNode));
                    }
                    else if (childNode.Name.Equals("TextBox"))
                    {
                        jarray.Add(ToTextBox(childNode));
                    }
                    else if (childNode.Name.Equals("Attachment"))
                    {
                        jarray.Add(ToAttachment(childNode));
                    }
                }

                return jarray;
            }

            return null;
        }

        private static JObject ToStep(XmlElement node)
        {
            var step = new JObject();
            step["Name"] = "Step";
            step["ID"] = node.Attributes["ID"].Value;
            step["X"] = node.Attributes["X"].Value;
            step["Y"] = node.Attributes["Y"].Value;
            step["Operand"] = node.Attributes["Operand"].Value;
            step["HideDesc"] = node.Attributes["HideDesc"].Value;
            step["DescX"] = node.Attributes["DescX"].Value;
            step["DescY"] = node.Attributes["DescY"].Value;
            step["DescWidth"] = node.Attributes["DescWidth"].Value;
            step["InitialStep"] = node.Attributes["InitialStep"].Value;
            step["PresetUsesExpr"] = node.Attributes["PresetUsesExpr"].Value;
            step["LimitHighUsesExpr"] = node.Attributes["LimitHighUsesExpr"].Value;
            step["LimitLowUsesExpr"] = node.Attributes["LimitLowUsesExpr"].Value;
            step["ShowActions"] = node.Attributes["ShowActions"].Value;
            return step;
        }

        private static JObject ToTransition(XmlElement node)
        {
            var transition = new JObject();
            transition["Name"] = "Transition";
            transition["ID"] = node.Attributes["ID"].Value;
            transition["X"] = node.Attributes["X"].Value;
            transition["Y"] = node.Attributes["Y"].Value;
            transition["Operand"] = node.Attributes["Operand"].Value;
            transition["HideDesc"] = node.Attributes["HideDesc"].Value;
            transition["DescX"] = node.Attributes["DescX"].Value;
            transition["DescY"] = node.Attributes["DescY"].Value;
            transition["DescWidth"] = node.Attributes["DescWidth"].Value;
            var lines = node.GetElementsByTagName("Line");
            JArray codeText = new JArray();
            foreach (var line in lines)
            {
                codeText.Add(((XmlElement) line).InnerText);
            }

            transition["STContent"] = codeText;
            return transition;
        }

        private static JObject ToBranch(XmlElement node)
        {
            var branch = new JObject();

            branch["Name"] = "Branch";
            branch["ID"] = node.Attributes["ID"].Value;
            branch["Y"] = node.Attributes["Y"].Value;
            branch["BranchType"] = node.Attributes["BranchType"].Value;
            branch["BranchFlow"] = node.Attributes["BranchFlow"].Value;

            var legs = node.ChildNodes;
            Debug.Assert(legs.Count >= 2, node.InnerXml);

            var jArray = new JArray();
            foreach (XmlNode leg in legs)
            {
                var legContent = new JObject();
                // ReSharper disable once PossibleNullReferenceException
                legContent["ID"] = leg.Attributes["ID"].Value;
                jArray.Add(legContent);
            }

            branch["Legs"] = jArray;
            return branch;
        }

        private static JObject ToStop(XmlElement node)
        {
            var stop = new JObject();
            stop["Name"] = "Stop";
            stop["ID"] = node.Attributes["ID"].Value;
            stop["X"] = node.Attributes["X"].Value;
            stop["Y"] = node.Attributes["Y"].Value;
            stop["Operand"] = node.Attributes["Operand"].Value;
            stop["HideDesc"] = node.Attributes["HideDesc"].Value;
            stop["DescX"] = node.Attributes["DescX"].Value;
            stop["DescY"] = node.Attributes["DescY"].Value;
            stop["DescWidth"] = node.Attributes["DescWidth"].Value;
            return stop;
        }

        private static JObject ToDirectedLink(XmlElement node)
        {
            var link = new JObject();
            link["Name"] = "DirectedLink";
            link["FromID"] = node.Attributes["FromID"].Value;
            link["ToID"] = node.Attributes["ToID"].Value;
            link["Show"] = node.Attributes["Show"].Value;
            return link;
        }

        private static JObject ToTextBox(XmlElement node)
        {
            var textBox = new JObject();
            textBox["Name"] = "TextBox";
            textBox["ID"] = node.Attributes["ID"].Value;
            textBox["X"] = node.Attributes["X"].Value;
            textBox["Y"] = node.Attributes["Y"].Value;
            textBox["Width"] = node.Attributes["Width"].Value;
            var text = node.FirstChild;
            if (text != null)
            {
                textBox["Text"] = text.InnerText;
            }

            return textBox;
        }

        private static JObject ToAttachment(XmlElement node)
        {
            var attachment = new JObject();
            attachment["Name"] = "Attachment";
            attachment["FromID"] = node.Attributes["FromID"].Value;
            attachment["ToID"] = node.Attributes["ToID"].Value;
            return attachment;
        }
    }
}
