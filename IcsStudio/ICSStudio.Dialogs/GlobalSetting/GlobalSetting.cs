using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSStudio.Dialogs.GlobalSetting
{
    public class GlobalSetting
    {
        private static GlobalSetting _globalSetting;

        private static readonly string ConfigPath =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ICSStudio\\ICSConfig.json";

        private bool _isGetConfig = false;

        public static GlobalSetting GetInstance()
        {
            return _globalSetting ?? (_globalSetting = new GlobalSetting());
        }

        private GlobalSetting()
        {
            Controller.GetInstance().Loaded += GlobalSetting_Loaded;
        }

        private void GlobalSetting_Loaded(object sender, EventArgs e)
        {
            Clear();
        }

        public RoutineSetting RoutineSetting { get; private set; } = new RoutineSetting();
        public TagSetting TagSetting { get; } = new TagSetting();
        public MonitorTagSetting MonitorTagSetting { get; private set; } = new MonitorTagSetting();
        public DownloadSetting DownloadSetting { get; private set; } = new DownloadSetting();

        public FindInRoutineSetting FindInRoutineSetting { get; private set; } = new FindInRoutineSetting();

        public ShortcutSetting ShortcutSetting { get; private set; } = new ShortcutSetting();

        public NewAoiDialogSetting NewAoiDialogSetting { get; private set; } = new NewAoiDialogSetting();

        public Visibility ProgramInstructionVisibilitySetting { get; set; }

        public Double ProgramInstructionWidthSetting { get; set; }

        public void Clear()
        {
            TagSetting.Scope = null;
        }

        public void SaveConfig()
        {
            try
            {
                var config = new JObject
                {
                    { nameof(RoutineSetting), JsonConvert.SerializeObject(RoutineSetting) },
                    { nameof(MonitorTagSetting), JsonConvert.SerializeObject(MonitorTagSetting) },
                    { nameof(DownloadSetting), JsonConvert.SerializeObject(DownloadSetting) },
                    { nameof(FindInRoutineSetting), JsonConvert.SerializeObject(FindInRoutineSetting) },
                    { nameof(ShortcutSetting), JsonConvert.SerializeObject(ShortcutSetting) },
                    { nameof(NewAoiDialogSetting), JsonConvert.SerializeObject(NewAoiDialogSetting) },
                    { nameof(ProgramInstructionVisibilitySetting),JsonConvert.SerializeObject(ProgramInstructionVisibilitySetting)},
                    { nameof(ProgramInstructionWidthSetting),JsonConvert.SerializeObject(ProgramInstructionWidthSetting)},
                };

                if (!Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ICSStudio"))
                    Directory.CreateDirectory(
                        $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ICSStudio");
                using (var sw = File.CreateText(ConfigPath))
                using (var jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Formatting.Indented;

                    config.WriteTo(jw);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

        }

        public void ReadConfig()
        {
            if (_isGetConfig) return;
            _isGetConfig = true;
            if (File.Exists(ConfigPath))
                try
                {
                    using (StreamReader file = File.OpenText(ConfigPath))
                    using (JsonTextReader reader = new JsonTextReader(file))
                    {
                        var config = JToken.ReadFrom(reader);
                        var routineSetting = config[nameof(RoutineSetting)];
                        if (routineSetting != null)
                        {
                            RoutineSetting = JsonConvert.DeserializeObject<RoutineSetting>(routineSetting.ToString());
                        }

                        var monitorTagSetting = config[nameof(MonitorTagSetting)];
                        if (monitorTagSetting != null)
                        {
                            MonitorTagSetting =
                                JsonConvert.DeserializeObject<MonitorTagSetting>(monitorTagSetting.ToString());
                          
                            if (!MonitorTagSetting.FilterTypeHistory.Contains("Configure...")
                                || !MonitorTagSetting.FilterTypeHistory.Contains("All Variables"))
                            {
                                MonitorTagSetting.FilterTypeHistory.Insert(0, "Configure...");
                                MonitorTagSetting.FilterTypeHistory.Insert(0, "All Variables");
                            }
                        }

                        var downloadSetting = config[nameof(DownloadSetting)];
                        if (downloadSetting != null)
                        {
                            DownloadSetting =
                                JsonConvert.DeserializeObject<DownloadSetting>(downloadSetting.ToString());
                        }

                        var findInRoutinesSetting = config[nameof(FindInRoutineSetting)];
                        if (findInRoutinesSetting != null)
                        {
                            FindInRoutineSetting =
                                JsonConvert.DeserializeObject<FindInRoutineSetting>(findInRoutinesSetting.ToString());
                        }

                        var shortcutSetting = config[nameof(ShortcutSetting)];
                        if (shortcutSetting != null)
                        {
                            ShortcutSetting =
                                JsonConvert.DeserializeObject<ShortcutSetting>(shortcutSetting.ToString());
                        }

                        var newAoiDialogSetting = config[nameof(NewAoiDialogSetting)];
                        if (newAoiDialogSetting != null)
                        {
                            NewAoiDialogSetting =
                                JsonConvert.DeserializeObject<NewAoiDialogSetting>(newAoiDialogSetting.ToString());
                        }

                        var programInstructionSetting = config[nameof(ProgramInstructionVisibilitySetting)];
                        if (programInstructionSetting != null)
                        {
                            ProgramInstructionVisibilitySetting =
                                JsonConvert.DeserializeObject<Visibility>(programInstructionSetting.ToString());
                        }

                        var programInstructionWidthSetting = config[nameof(ProgramInstructionWidthSetting)];
                        if (programInstructionWidthSetting != null)
                        {
                            ProgramInstructionWidthSetting =
                                JsonConvert.DeserializeObject<Double>(programInstructionWidthSetting.ToString());
                        }

                    }
                }
                catch (Exception e)
                {
                    Debug.Assert(false, e.StackTrace);
                }
        }
    }

    public class ShortcutSetting
    {
        [JsonIgnore]
        public Dictionary<string, string> ICSStudioCommandList { get; } = new Dictionary<string, string>()
        {
            //File菜单
            {"File.Save", "::Ctrl+S"},
            {"File.New","::Ctrl+N"},
            {"File.Open","::Ctrl+O"},

            //Edit菜单
            {"Edit.Cut","::Ctrl+X"},
            {"Edit.Copy","::Ctrl+C"},
            {"Edit.Paste","::Ctrl+V"},
            {"Edit.PasteWithConfiguration","::Ctrl+Shift+V"},
            {"Edit.Properties","::Alt+Enter"},

            //View菜单
            {"View.ControllerOrganizer","::Alt+0"},
            {"View.Errors","::Alt+1"},
            {"View.SearchResult","::Alt+2"},
            {"View.QuickWatch","::Alt+3"},

            //Search菜单
            {"Search.Find", "::Ctrl+F"},
            {"Search.Replace","::Ctrl+H"},
            {"Search.GoTo","::Ctrl+G"},
            {"Search.BrowseLogic","::Ctrl+L"},
            {"Search.CrossReference","::Ctrl+E"},
            {"Search.FindNext","::F3"},
            {"Search.FindPrevious","::Shift+F3"},
            {"Search.NextResult","::F4"},
            {"Search.PreviousResult","::Shift+F4"},
        };
        public bool IsSetting { set; get; }
    }

    public class FindInRoutineSetting
    {
        public enum FindWhereType
        {
            [EnumMember(Value = "Current Routine")]
            Current,

            [EnumMember(Value = "All Routines in Current Program / Equipment Phase")]
            InCurrentProgram,

            [EnumMember(Value = "All Routines in Current Task")]
            InCurrentTask,
            [EnumMember(Value = "All Routines")]
            All
        }

        public FindWhereType FindWhere { set; get; } = FindWhereType.All;
    }

    public class RoutineSetting
    {
        public RoutineType CurrentRoutineType { set; get; }

    }

    public class TagSetting
    {
        public ITagCollectionContainer Scope { set; get; }
    }

    public class MonitorTagSetting
    {
        public MonitorTagSetting()
        {
            if (!FilterTypeHistory.Contains("Configure...") || !FilterTypeHistory.Contains("All Variables"))
            {
                FilterTypeHistory.Insert(0, "Configure...");
                FilterTypeHistory.Insert(0, "All Variables");
            }
        }

        public int FilterNameType { set; get; } = 0;

        public ObservableCollection<string> FilterNameHistory { get; } = new ObservableCollection<string>();

        public ObservableCollection<string> FilterTypeHistory { get; } = new ObservableCollection<string>();

        public void CheckFilterType()
        {
            var dataTypeCollection = Controller.GetInstance().DataTypes;
            var usageList = new List<string>()
            {
                "Input", "Output", "InOut", "Local", "Produced", "Consumed", "Alias"
            };
            for (int i = FilterTypeHistory.Count - 1; i > 1; i--)
            {
                var filterType = FilterTypeHistory[i].Split(',');
                bool isCorrect = true;
                for (int j = 0; j < filterType.Length; j++)
                {
                    var type = filterType[j];
                    if (j == 0)
                    {
                        if (usageList.Contains(type))
                            continue;
                    }

                    if (dataTypeCollection[type] == null)
                    {
                        isCorrect = false;
                        break;
                    }
                }

                if (!isCorrect)
                {
                    FilterTypeHistory.RemoveAt(i);
                }
            }
        }
    }

    public class DownloadSetting
    {
        public bool IsPreserve { set; get; } = true;

        public bool IsRestore { set; get; }
    }

    public class NewAoiDialogSetting
    {
        public bool IsOpenLogic { set; get; }
        public bool IsOpenDefinition { set; get; }
    }
}
