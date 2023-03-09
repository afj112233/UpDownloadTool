using ICSStudio.Interfaces.Common;

namespace ICSStudio.SimpleServices.Chart
{
    public class Step : IChart
    {
        //public Step(IRoutine routine)
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
        public bool InitialStep { get; set; }
        public bool PresetUsesExpr { get; set; }
        public bool LimitHighUsesExpr { get; set; }
        public bool LimitLowUsesExpr { get; set; }
        public bool ShowActions { get; set; }
        public IRoutine ParentRoutine { get; }
    }
}
