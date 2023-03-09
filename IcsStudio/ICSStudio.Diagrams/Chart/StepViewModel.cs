using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ICSStudio.Diagram.Flowchart;
using ICSStudio.Diagrams.Exceptions;
using ICSStudio.SimpleServices.Chart;

namespace ICSStudio.Diagrams.Chart
{
    public class StepViewModel:ViewModelBase
    {
        private readonly SimpleServices.Chart.Step _content;

        public StepViewModel(FlowNode node)
        {
            if (node.Content == null)
            {
                node.Content = new SimpleServices.Chart.Step() {Operand = "Step_001"};
            }
            _content = node.Content as SimpleServices.Chart.Step;
            if (_content!=null)
            {
                Name = _content.Operand;
            }
        }

        public SimpleServices.Chart.Step Content => _content;

        public string Name { set; get; } 
    }
}
