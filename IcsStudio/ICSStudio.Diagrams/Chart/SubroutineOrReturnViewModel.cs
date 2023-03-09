using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace ICSStudio.Diagrams.Chart
{
    public class SubroutineOrReturnViewModel:ViewModelBase
    {
        private double _width;
        //private double _height;
        //private double _dataGridWidth;
        //private double _colWidth;

        public SubroutineOrReturnViewModel()
        {
            Properties=new ObservableCollection<Properties>() { };
            //DataGridWidth = Double.NaN;
            Properties.Add(new Properties() { Type = "Input Par", Value = "22" });
            Properties.Add(new Properties() { Type = "Input Par", Value = "55" });
            Properties.Add(new Properties() { Type = "Return Par", Value = "33" });
            Properties.Add(new Properties() { Type = "Return Par", Value = "4" });
        }
        

        public string Name { set; get; }

        public double Width
        {
            set
            {
                Set(ref _width, value);
            }
            get { return _width; }
        }

        //public double ColWidth
        //{
        //    set { Set(ref _colWidth, value); }
        //    get { return _colWidth; }
        //}

        //public double Height
        //{
        //    set
        //    {
        //        Set(ref _height, value);
        //    }
        //    get { return _height; }
        //}

        public ObservableCollection<Properties> Properties { set; get; }

        //public double DataGridWidth
        //{
        //    set { Set(ref _dataGridWidth, value); }
        //    get { return _dataGridWidth; }
        //}
    }

    public class Properties:INotifyPropertyChanged
    {
        private string _value;
        public string Type { set; get; }

        public string Value
        {
            set
            {
                _value = value;
                OnPropertyChanged();
            }
            get { return _value; }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}
