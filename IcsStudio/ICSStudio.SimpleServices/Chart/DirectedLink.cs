using ICSStudio.Interfaces.Common;

namespace ICSStudio.SimpleServices.Chart
{
    public class DirectedLink : ILink
    {
        //public DirectedLink(IRoutine routine)
        //{
        //    ParentRoutine = routine;
        //}
        public int FromID { get; set; }
        public int ToID { get; set; }
        public bool Show { get; set; }
        public IRoutine ParentRoutine { get; }
    }
}
