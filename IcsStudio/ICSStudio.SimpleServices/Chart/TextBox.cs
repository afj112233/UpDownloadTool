using ICSStudio.Interfaces.Common;

namespace ICSStudio.SimpleServices.Chart
{
    public class TextBox : IChart
    {
        //public TextBox(IRoutine routine)
        //{
        //    ParentRoutine = routine;
        //}
        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public string Text { set; get; }
        public IRoutine ParentRoutine { get; }
    }
}
