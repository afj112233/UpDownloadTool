namespace ICSStudio.Database.Table.Motion
{
    public class FeedbackDeviceView
    {
        // ReSharper disable once InconsistentNaming
        public int ID { get; set; }
        // ReSharper disable once InconsistentNaming
        public int FeedbackTypeID { get; set; }
        // ReSharper disable once InconsistentNaming
        public int SercosID { get; set; }
        // ReSharper disable once InconsistentNaming
        public int CipID { get; set; }
        // ReSharper disable once InconsistentNaming
        public int CipStartupTypeID { get; set; }
        public int SingleTurnResolution { get; set; }
        public int MultiTurnResolution { get; set; }
        public float DeviceLength { get; set; }
        public int DataLength { get; set; }
        public string Name { get; set; }
        // ReSharper disable once InconsistentNaming
        public int LDTType { get; set; }
        // ReSharper disable once InconsistentNaming
        public int LDTRecirculations { get; set; }
        public int Cycles { get; set; }
        public int Mode { get; set; }
        public bool RotarySupport { get; set; }
        public bool LinearSupport { get; set; }
        public bool HallSupport { get; set; }
    }
}
