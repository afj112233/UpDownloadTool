using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Diagram.Flowchart;
using ICSStudio.SimpleServices.Chart;

namespace ICSStudio.Diagrams.Chart
{
    public class StopViewModel
    {
        private readonly SimpleServices.Chart.Stop _content;
        public StopViewModel(FlowNode node)
        {
            if (node.Content == null)
            {
                node.Content=new SimpleServices.Chart.Stop() {Operand = "Stop_001"};
            }
            _content = node.Content as SimpleServices.Chart.Stop;
            if (_content == null)
            {
                Name = "Stop_001";
            }
            else
            {
                Name = _content.Operand;
            }
        }

        public SimpleServices.Chart.Stop Content => _content;

        public string Name { set; get; }
    }
}
