namespace ICSStudio.Gui.Controls
{
    public class UpdateCommandParameterInfo
    {
        public double Max { get; set; }
        public double Min { get; set; }
        public double Step { get; set; }
        public SpinStrategy Strategy { get; set; }

        public bool Direction { get; set; }
        public string CurrVal { get; set; }
        public double DefaultVal { get; set; }
    }
}
