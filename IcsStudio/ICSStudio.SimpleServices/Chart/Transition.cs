using System.Collections.Generic;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.SimpleServices.Chart
{
    public class Transition : IChart
    {
        public Transition()
        {
            //ParentRoutine = routine;
            STContent = new STContent();
        }

        public STContent STContent { get; set; }
        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Operand { get; set; }
        public bool HideDesc { get; set; }
        public double DescX { get; set; }
        public double DescY { get; set; }
        public double DescWidth { get; set; }
        public List<string> CodeText { get; } = new List<string>();

        public IRoutine ParentRoutine { get; }
    }
}
