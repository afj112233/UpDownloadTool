using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ICSStudio.Diagrams.Chart.ItemControl
{
    class DialogInputViewModel:ViewModelBase
    {
        private double _width;
        private string _content;
        private Visibility _visibility;
        //private double _height;
        private Properties _properties;

        public DialogInputViewModel(Properties properties)
        {
            _properties = properties;
            Content = properties.Value;
            //Height = 20;
            Visibility = Visibility.Collapsed;
            Command = new RelayCommand(ExecuteCommand);
        }

        //public double Height
        //{
        //    set { Set(ref _height, value); }
        //    get { return _height; }
        //}

        public RelayCommand Command { set; get; }

        public void ExecuteCommand()
        {
            if (Visibility == Visibility.Collapsed) Visibility = Visibility.Visible;
            else Visibility = Visibility.Collapsed;
        }

        //public void UpdateWidth()
        //{
        //    Width = Content.Length * 10;
        //}

        public Visibility Visibility
        {
            set { Set(ref _visibility, value); }
            get { return _visibility; }
        }

        public double Width
        {
            set { Set(ref _width, value); }
            get { return _width; }
        }

        public string Content
        {
            set
            {
                Set(ref _content, value);
                //UpdateWidth();
                _properties.Value = value;
            }
            get { return _content; }
        }
    }
}
