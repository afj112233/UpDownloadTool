namespace ICSStudio.Database.Table.Motion
{
    public class Motor
    {
        // ReSharper disable once InconsistentNaming
        public int ID { get; set; }
        // ReSharper disable once InconsistentNaming
        public int MotorID { get; set; } // no used
        // ReSharper disable once InconsistentNaming
        public int MotorTypeID { get; set; }
        // ReSharper disable once InconsistentNaming
        public int FeedbackDeviceID { get; set; }
        public double MotorUnits { get; set; }

        // ReSharper disable once InconsistentNaming
        public int OldFormatMotorID { get; set; }

        public int MotorPolarity { get; set; }
        public double RatedVoltage { get; set; }
        public double RatedCurrent { get; set; }
        public double PeakCurrent { get; set; }
        public double RatedPower { get; set; }
        public double OverloadLimit { get; set; }
        public bool IntegralThermostat { get; set; }
        public int Cthwa { get; set; }
        public double Rthwa { get; set; }
        // ReSharper disable once InconsistentNaming
        public int LoadID { get; set; }
        // ReSharper disable once InconsistentNaming
        public int LoadTypeID { get; set; }
        public double MtrMaxWindingTemp { get; set; }
        public string CatalogNumber { get; set; }
        public byte[] BlobData { get; set; }
    }
}
