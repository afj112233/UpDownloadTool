using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using ICSStudio.SimpleServices.Annotations;

namespace ICSStudio.Dialogs.BrowseString.RichTextBoxExtend
{
    public class Message : INotifyPropertyChanged
    {
        private string _text;
        private bool _isClose;
        private Popup _container;
        public Message(Popup container,string text = "", string itemName = "")
        {
            _container = container;
            Text = text;
            ItemName = itemName;
        }

        public bool ShowButton { set; get; } = true;

        public void SendMessage(string text)
        {
            Text = text;
            OnPropertyChanged("Text");
        }

        public string Text
        {
            set { _text = value; }
            get
            {
                var t = _text;
                if (t.StartsWith("'") && t.EndsWith("'"))
                    t = t.Substring(0, t.Length - 1).Substring(1);
                return t;
            }
        }

        public bool IsClose
        {
            set
            {
                _isClose = value;
                if (_container != null)
                    _container.IsOpen = false;
            }
            get { return _isClose; }
        }

        public Popup Container => _container;

        public string ItemName { set; get; }
        public int Len { set; get; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
