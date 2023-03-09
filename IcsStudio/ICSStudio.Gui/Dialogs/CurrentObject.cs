using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ICSStudio.Gui.Dialogs
{
    public class CurrentObject
    {
        private static CurrentObject _instance;
        private object _current;
        private object _currentControl;

        public static CurrentObject GetInstance()
        {
            return _instance ?? (_instance = new CurrentObject());
        }

        public object Current
        {
            set
            {
                if(_current!=value)
                {
                    Previous = _current;
                    _current = value;
                    CurrentObjectChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            get { return _current; }
        }

        public object Previous { private set; get; }

        public object CurrentControl
        {
            set
            {
                if(_currentControl!=value)
                {
                    PreviousControl = _currentControl;
                    _currentControl = value;
                }
            }
            get { return _currentControl; }
        }

        public object PreviousControl {private set; get; }

        public event EventHandler CurrentObjectChanged;
    }
}
