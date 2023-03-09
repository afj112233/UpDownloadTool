using System;
using System.Data;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.MultiLanguage;

namespace ICSStudio.ToolBoxPackage
{
    public class ToolBoxControlVM : ViewModelBase
    {
        private readonly ToolBoxItem _root;

        public ToolBoxItems Items => _root.Children;

        private Visibility _visible = Visibility.Visible;

        public Visibility Visible
        {
            get { return _visible; }
            set { Set(ref _visible, value); }
        }

        public ToolBoxControlVM()
        {
            _root = new ToolBoxItem { Name = "Root" };
            LanguageManager.GetInstance().LanguageChanged+= OnLanguageChanged;
            InitItems();
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            foreach (var i in _root.Children)
            {
                if (i.DisplayName=="位操作"||i.DisplayName =="Bit")
                    i.DisplayName = LanguageManager.GetInstance().ConvertSpecifier("ToolBit");
                if (i.DisplayName == "运动状态" || i.DisplayName == "Motion State")
                    i.DisplayName = LanguageManager.GetInstance().ConvertSpecifier("ToolMotion State");
                if (i.DisplayName == "运动位移" || i.DisplayName == "Motion Move")
                    i.DisplayName = LanguageManager.GetInstance().ConvertSpecifier("ToolMotion Move");
                else
                    i.DisplayName = LanguageManager.GetInstance().ConvertSpecifier(i.DisplayName);
            }
        }

        private void InitItems()
        {
            string baseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            string configPath = baseDirectoryPath + "RLL";
            //从 D:\\icon\\icsstudio\\data\\RLL获取所有Instruction
            if (!Directory.Exists(configPath))
                return;
            ToolBoxItem rItem = new ToolBoxItem { DisplayName = "Rung", Kind = ToolBoxItemType.Rung };
            _root.Children.Add(rItem);
            rItem = new ToolBoxItem { DisplayName = "Branch", Kind = ToolBoxItemType.Rung };
            _root.Children.Add(rItem);
            rItem = new ToolBoxItem { DisplayName = "Branch Level", Kind = ToolBoxItemType.Rung };
            _root.Children.Add(rItem);

            DirectoryInfo theFolder = new DirectoryInfo(configPath);
            foreach (DirectoryInfo directory in theFolder.GetDirectories())
            {
                DirectoryInfo folder = new DirectoryInfo(directory.FullName);
                string group = Path.GetFileNameWithoutExtension(folder.FullName);
                ToolBoxItem cate = new ToolBoxItem { DisplayName = group, Kind = ToolBoxItemType.Category, ToolTip = $"Element Group: {group}", IsExpanded = false };
                foreach (var file in folder.GetFiles())
                {
                    string inst = Path.GetFileNameWithoutExtension(file.FullName);
                    ToolBoxItem item = new ToolBoxItem { DisplayName = inst, Kind = ToolBoxItemType.Instruction };
                    using (DataSet ds = new DataSet())
                    {
                        ds.ReadXml(file.FullName, XmlReadMode.InferSchema);
                        DataRow row = ds.Tables["BuiltInInstructionDefinition"].Rows[0];
                        string description = row["Description"].ToString();
                        item.ToolTip = $"{inst}\r\n{description}";
                    }

                    cate.Children.Add(item);
                }
                _root.Children.Add(cate);
            }
        }
    }
}
