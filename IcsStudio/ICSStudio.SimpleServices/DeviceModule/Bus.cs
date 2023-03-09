using System.ComponentModel;
using System.Runtime.CompilerServices;
using ICSStudio.Gui.Annotations;

namespace ICSStudio.SimpleServices.DeviceModule
{
    public class Bus : INotifyPropertyChanged
    {
        private int _size;

        public Bus()
        {
            _size = -1;
        }

        public int Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
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
