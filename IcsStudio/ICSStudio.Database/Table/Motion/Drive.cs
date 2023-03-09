namespace ICSStudio.Database.Table.Motion
{
    public class Drive
    {
        // ReSharper disable once InconsistentNaming
        public int ID { get; set; }
        public float PeakCurrent { get; set; }
        public float BaseTimeConstant { get; set; }
        public int MaxOutputFrequency { get; set; }
        public int DriveId { get; set; }
        public float BusOvervoltageOperationalLimit { get; set; }
        public int DriveTypeID { get; set; }

        public int ConverterACInputVoltage { get; set; }
        public int ConverterACInputPhasing { get; set; }
    }
}
