using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using ICSStudio.Diagram.Flowchart;
using ICSStudio.SimpleServices.Chart;

namespace ICSStudio.Diagram.Chart
{
    public class TransitionViewModel : ViewModelBase
    {
        private string _name;
        private double _width;
        private double _hight;
        private string _st;
        private SimpleServices.Chart.Transition _content;
        public TransitionViewModel(FlowNode node)
        {
            if (node.Content == null)
            {
                node.Content=new SimpleServices.Chart.Transition() {Operand = "Step_001"};
            }
            _content = node.Content as SimpleServices.Chart.Transition;
            _st = "";
            Name = _content.Operand;
            int length = (int)Math.Ceiling((double)(Math.Max(0, Name.Length - "Step_001".Length) / 3));
            double width = length;
            foreach (var code in _content.CodeText)
            {
                if(ST=="")
                    ST += code;
                else
                {
                    ST += "\r\n" + code;
                    width = Math.Max(width,
                        (int)Math.Ceiling((double)(Math.Max(0, code.Length - "Step_001".Length) / 3)));
                }
            }
            //double width = 100 + length * 20;
            
            length = Math.Max(0, _content.CodeText.Count - 1);
            Hight = 80 + 10 * length;
            Width = 110 + width * 20;
        }

        public SimpleServices.Chart.Transition Content => _content;

        public string Name
        {
            set { Set( ref _name, value); }
            get { return _name; }
        }

        public double Width
        {
            set { Set( ref _width, value); }
            get { return _width; }
        }

        public double Hight
        {
            set { Set( ref _hight, value); }
            get { return _hight; }
        }

        public string ST
        {
            set
            {
                Set(ref _st, value); 
            }
            get { return _st; }
        }
    }
}
