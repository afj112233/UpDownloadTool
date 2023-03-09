using System.Collections.Generic;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.SimpleServices.Chart
{
    public class Branch : IChart
    {
        //public Branch(IRoutine routine)
        //{
        //    ParentRoutine = routine;
        //}
        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public BranchType BranchType { get; set; }
        public BranchFlowType BranchFlow { get; set; }
        public string Priority { get; set; }
        public List<int> Legs { get; } = new List<int>();

        public void AddLeg(int id)
        {
            if (!Legs.Contains(id))
                Legs.Add(id);
        }

        public void DelLeg(int id)
        {
            if (Legs.Contains(id))
                Legs.Remove(id);
        }

        public IRoutine ParentRoutine { get; }
    }

    public enum BranchType
    {
        Simultaneous,
        Selection
    }

    public enum BranchFlowType
    {
        Diverge,
        Converge
    }
}
