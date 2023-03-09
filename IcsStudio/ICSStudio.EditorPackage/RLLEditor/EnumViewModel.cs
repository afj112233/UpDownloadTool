using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace ICSStudio.EditorPackage.RLLEditor
{
    internal class EnumViewModel:ViewModelBase
    {
        private string _name;
        private string _selectedEnum;
        public void Reset(List<string> enums, string @enum)
        {
            Name = @enum;
            NameList.Clear();
            Enums.Clear();
            foreach (var name in enums)
            {
                NameList.Add(name);
                Enums.Add(name);
            }

            SelectedEnum = @enum;
        }

        public ObservableCollection<string> NameList { set; get; }=new ObservableCollection<string>();

        public string Name
        {
            set
            {
                if (_name != value)
                {
                    Set(ref _name, value);
                    SelectedEnum = _name;
                }
            }
            get { return _name; }
        }

        public string SelectedEnum
        {
            set
            {
                if (SelectedEnum != value)
                {
                    Set(ref _selectedEnum, value);
                    Name = value;
                }
            }
            get { return _selectedEnum; }
        }

        public ObservableCollection<string> Enums { set; get; }=new ObservableCollection<string>();
    }
}
