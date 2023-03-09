using ICSStudio.Interfaces.Common;

namespace ICSStudio.SimpleServices.Chart
{
    public class Stop : IChart
    {
        //public Stop(IRoutine routine)
        //{
        //    ParentRoutine = routine;
        //}
        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Operand { get; set; }
        public bool HideDesc { get; set; }
        public double DescX { get; set; }
        public double DescY { get; set; }
        public double DescWidth { get; set; }
        public IRoutine ParentRoutine { get; }
    }
}
