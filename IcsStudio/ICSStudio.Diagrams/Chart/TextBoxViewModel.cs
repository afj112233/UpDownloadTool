using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ICSStudio.Diagram.Flowchart;
using ICSStudio.SimpleServices.Chart;

namespace ICSStudio.Diagrams.Chart
{
    class TextBoxViewModel
    {
        private SimpleServices.Chart.TextBox _content;
        private string _text;

        public TextBoxViewModel(FlowNode node)
        {
            if(node.Content==null)
                node.Content=new SimpleServices.Chart.TextBox() ;
            _content = node.Content as SimpleServices.Chart.TextBox;
            if (_content == null)
            {
                Text = @"this is test.
this is test.this is test.
this is test.
this is test.";
            }
            else
            {
                Text = _content.Text;
            }
        }

        public SimpleServices.Chart.TextBox Content => _content;

        public string Text
        {
            set
            {
                _text = value;
                _content.Text = value;
            }
            get { return _text; }
        }
    }
}
