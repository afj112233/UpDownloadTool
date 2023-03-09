using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Diagrams.Controls;
using ICSStudio.Diagram.Flowchart;

namespace ICSStudio.Diagrams.Flowchart.Model
{
    public class AttachLink
    {
        public FlowNode Source { get; private set; }
        public FlowNode Target { get; private set; }

        public AttachLink(FlowNode source,FlowNode target)
        {
            if (source.Kind == NodeKinds.TextBox)
            {
                Source = source;
                Target = target;
            }else if (target.Kind == NodeKinds.TextBox)
            {
                Source = target;
                Target = source;
            }
            else
            {
                throw new Exception("error AttachLink initial.");
            }
            
        }
    }
}
