using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ICSStudio.Components.Language
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class LanguageVM : INotifyPropertyChanged
    {
        private string _name;
        private bool _isDefault;
        private string _translation;
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(nameof (Name));
            }
        }

        public bool IsDefault
        {
            get
            {
                return _isDefault;
            }
            set
            {
                _isDefault = value;
                OnPropertyChanged(nameof (IsDefault));
                OnPropertyChanged("Image");
            }
        }

        public string Translation
        {
            get
            {
                return this._translation;
            }
            set
            {
                this._translation = value;
                this.OnPropertyChanged(nameof (Translation));
            }
        }
    }
}
