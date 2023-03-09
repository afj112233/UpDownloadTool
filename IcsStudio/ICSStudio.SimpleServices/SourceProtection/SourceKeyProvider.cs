using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSStudio.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.SourceProtection
{
    public class SourceKeyProvider
    {
        public SourceKeyProvider()
        {
            KeyDictionary = new Dictionary<string, string>();

            LoadConfig();

            LoadSourceKeys();
        }

        public string SourceKeyFolder { get; set; }
        public Dictionary<string, string> KeyDictionary { get; }

        public string SourceKeyFile
        {
            get
            {
                if (string.IsNullOrEmpty(SourceKeyFolder))
                    return string.Empty;

                string fileName = "sk.dat";
                string fullName;

                if (SourceKeyFolder.EndsWith("\\"))
                    fullName = SourceKeyFolder + fileName;
                else
                    fullName = SourceKeyFolder + "\\" + fileName;

                return fullName;
            }
        }

        public void LoadSourceKeys()
        {
            KeyDictionary.Clear();

            try
            {
                var lines = File.ReadAllLines(SourceKeyFile, Encoding.UTF8);

                string lastSourceKey = string.Empty;

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];

                    if (IsSourceKey(line))
                    {
                        if (!string.IsNullOrEmpty(lastSourceKey))
                        {
                            //上一行是无名称key，先存储
                            KeyDictionary.Add(lastSourceKey, string.Empty);
                        }
                        lastSourceKey = line;
                        if (i == lines.Length - 1)
                        {
                            //最后一行
                            KeyDictionary.Add(lastSourceKey, string.Empty);
                        }
                    }
                    else if (IsSourceKeyName(line))
                    {
                        if (!string.IsNullOrEmpty(lastSourceKey))
                        {
                            string sourceKeyName = line.Trim();
                            KeyDictionary.Add(lastSourceKey, sourceKeyName);
                            lastSourceKey = string.Empty;
                        }
                    }
                    //else
                    //{
                    //    if (!string.IsNullOrEmpty(lastSourceKey))
                    //    {
                    //        KeyDictionary.Add(lastSourceKey, string.Empty);
                    //        lastSourceKey = string.Empty;
                    //    }
                    //}

                }

            }
            catch (Exception)
            {
                //ignore
            }
        }

        public void SaveSourceKeys()
        {
            List<string> lines = new List<string>
            {
                "\t<ICS STUDIO UTF-8 ENCODED SOURCE KEY FILE - FIRST LINE MUST NOT BE A SOURCE KEY!>"
            };

            foreach (var pair in KeyDictionary)
            {
                lines.Add(pair.Key);

                if (!string.IsNullOrEmpty(pair.Value))
                    lines.Add($"\t{pair.Value}");
            }

            try
            {
                File.WriteAllLines(SourceKeyFile, lines);
            }
            catch (Exception)
            {
                //ignore
            }

        }

        public void LoadConfig()
        {
            try
            {
                if(File.Exists(ConfigFile))
                using (StreamReader file = File.OpenText(ConfigFile))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    var json = (JObject)JToken.ReadFrom(reader);

                    SourceKeyFolder = (string)json["SourceKeyFolder"];
                }
            }
            catch (Exception)
            {
                //ignor
            }
        }

        public void SaveConfig()
        {
            try
            {
                using (var sw = File.CreateText(ConfigFile))
                using (var jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Formatting.Indented;

                    JObject config = new JObject { { "SourceKeyFolder", SourceKeyFolder } };
                    config.WriteTo(jw);
                }
            }
            catch (Exception)
            {
                //ignor
            }
        }

        private string ConfigFile => AssemblyUtils.AssemblyDirectory + "\\" + "SourceKeyProvider.cfg";

        private bool IsSourceKeyName(string line)
        {
            if (IsSourceKey(line))
                return false;

            string trimLine = line.Trim();

            if (string.IsNullOrEmpty(trimLine))
                return false;

            if (trimLine.Length > 40)
                return false;

            return true;
        }

        private bool IsSourceKey(string line)
        {
            if (string.IsNullOrEmpty(line))
                return false;

            string trimLine = line.Trim();
            if (string.Equals(line, trimLine) && trimLine.Length <= 40)
                return true;

            return false;
        }
    }
}
