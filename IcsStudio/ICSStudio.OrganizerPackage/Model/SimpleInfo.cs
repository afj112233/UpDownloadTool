using System.ComponentModel;
using System.Runtime.CompilerServices;
using ICSStudio.Gui.Annotations;
using ICSStudio.MultiLanguage;

namespace ICSStudio.OrganizerPackage.Model
{
    public class SimpleInfo : INotifyPropertyChanged
    {
        private string _value;

        public string Name { set; get; }

        public string DisplayName
        {
            get
            {
                string displayName = string.Empty;

                if (!string.IsNullOrEmpty(Key))
                {
                    displayName = LanguageManager.GetInstance().ConvertSpecifier(Key);
                }

                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = LanguageManager.GetInstance().ConvertSpecifier(Name);
                }

                if (string.IsNullOrEmpty(displayName))
                    return Name;

                return displayName;
            }
        }

        public string Key { get; set; }

        public string Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
