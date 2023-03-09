using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ICSStudio.SimpleServices.CodeAnalysis
{
    public class InstructionDefinition
    {
        private static Dictionary<string, string> _builtInInstructionDictionary;

        public static Dictionary<string, string> BuiltInInstructionDictionary
            => _builtInInstructionDictionary ?? (_builtInInstructionDictionary = Create());

        private static readonly string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;

        private static Dictionary<string, string> Create()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string configPath = BaseDirectoryPath + "RLL";
            DirectoryInfo theFolder = new DirectoryInfo(configPath);
            foreach (DirectoryInfo directory in theFolder.GetDirectories())
            {
                DirectoryInfo folder = new DirectoryInfo(directory.FullName);
                foreach (var file in folder.GetFiles())
                {
                    string inst = Path.GetFileNameWithoutExtension(file.FullName);
                    dictionary.Add(inst, file.FullName);
                }
            }
            return dictionary;
        }

        public static InstructionDefinition GetDefinition(string mnemonic)
        {
            string keyName = mnemonic.ToUpper();

            if (BuiltInInstructionDictionary.ContainsKey(keyName))
            {
                var path = BuiltInInstructionDictionary[keyName];

                if (path != null)
                {
                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(stream);
                        XmlNode root = doc.FirstChild;
                        if (root == null)
                            return null;

                        InstructionDefinition def = new InstructionDefinition();
                        if (root.Attributes != null)
                        {
                            def.Mnemonic = root.Attributes["Mnemonic"].Value;
                            def.Label = root.Attributes["Label"].Value;
                            def.Type = root.Attributes["Type"].ToString();
                            def.DictatingRungState = bool.Parse(root.Attributes["DictatingRungState"].Value);

                            foreach (XmlNode node in root.ChildNodes)
                            {
                                if (node.Name == "Description")
                                {
                                    def.Description = node.InnerText;
                                }

                                if (node.Name == "Parameters")
                                {
                                    var parameters = new List<InstructionParameter>();
                                    foreach (XmlNode child in node.ChildNodes)
                                    {
                                        if (child.Name == "Parameter")
                                        {
                                            InstructionParameter parameter = new InstructionParameter
                                            {
                                                Label = child.Attributes["Label"].Value,
                                                Type = child.Attributes["Type"].Value,
                                                DataType = child.Attributes["DataType"].Value,
                                                Visible = bool.Parse(child.Attributes["Visible"].Value),
                                                HasConfig = bool.Parse(child.Attributes["HasConfig"].Value)
                                            };

                                            if (child.Attributes["IsEnum"] != null)
                                                parameter.IsEnum = bool.Parse(child.Attributes["IsEnum"].Value);

                                            foreach (XmlNode child2Nd in child.ChildNodes)
                                            {
                                                if (child2Nd.Name == "Description")
                                                    parameter.Description = child2Nd.InnerText;
                                                if (child2Nd.Name == "DataTypes")
                                                {
                                                    var datatypes = new List<string>();
                                                    foreach (XmlNode child3Rd in child2Nd.ChildNodes)
                                                    {
                                                        if (child3Rd.Name == "DataType")
                                                        {
                                                            datatypes.Add(child3Rd.Attributes["Name"].Value);
                                                        }
                                                    }

                                                    parameter.AcceptTypes = datatypes;
                                                }

                                                if (child2Nd.Name == "Formats")
                                                {
                                                    var formats = new List<FormatType>();
                                                    foreach (XmlNode child3Rd in child2Nd.ChildNodes)
                                                    {
                                                        if (child3Rd.Name == "Format")
                                                        {
                                                            string formatStr = child3Rd.Attributes["Name"].Value;
                                                            FormatType type = ParseFormat(formatStr);
                                                            formats.Add(type);
                                                        }
                                                    }

                                                    parameter.Formats = formats;
                                                }
                                            }

                                            parameters.Add(parameter);
                                        }
                                    }

                                    def.Parameters = parameters;
                                }

                                if (node.Name == "Options")
                                {
                                    foreach (XmlNode child in node.ChildNodes)
                                    {
                                        if (child.Name == "Option")
                                        {
                                            if (child.Attributes["Name"].Value == "RLLViewInstructionIOFormat")
                                                def.InstructionLayout = child.Attributes["Value"].Value;
                                            if (child.Attributes["Name"].Value == "RLLViewInstructionDisplayBoolValue")
                                                def.DisplayBoolValue = bool.Parse(child.Attributes["Value"].Value);
                                        }
                                    }
                                }

                                if (node.Name == "BitLegs")
                                {
                                    var bitlegs = new List<string>();

                                    foreach (XmlNode child in node.ChildNodes)
                                    {
                                        bitlegs.Add(child.Attributes["Name"].Value);
                                    }

                                    def.BitLegs = bitlegs;
                                }
                            }
                        }

                        return def;
                    }
                }
            }
            return null;
        }

        static FormatType ParseFormat(string formatStr)
        {
            switch (formatStr)
            {
                case "tag":
                    return FormatType.Tag;
                case "immediate":
                    return FormatType.Immediate;
                case "Phase":
                    return FormatType.Phase;
                case "tag array":
                    return FormatType.TagArray;
                case "sequence":
                    return FormatType.Sequence;
                case "expression":
                    return FormatType.Expression;
                case "label":
                    return FormatType.Label;
                case "routine":
                    return FormatType.Routine;
                default:
                    return FormatType.None;
            }
        }

        public string Mnemonic { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        // contact,coil,box,tinybox,complex,expcomplex,
        public string Type { get; set; }

        public bool DictatingRungState { get; set; }
        public bool DrawSafetyWatermark { get; set; }

        // Classic,All
        public string InstructionLayout { get; set; }
        public bool DisplayBoolValue { get; set; }

        public List<InstructionParameter> Parameters { get; set; }
        public List<string> BitLegs { get; set; }
    }

    public class InstructionParameter
    {
        public string Label { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public List<string> AcceptTypes { get; set; }
        public List<FormatType> Formats { get; set; }
        public bool Visible { get; set; }
        public bool HasConfig { get; set; }
        public bool IsEnum { get; set; }
    }

    public enum FormatType
    {
        Tag,
        Immediate,
        Phase,
        TagArray,
        Sequence,
        Expression,
        Label,
        Routine,
        None
    }
}
