namespace ICSStudio.Database.Table.Motion
{
    public class MotorSearchView
    {
        // ReSharper disable once InconsistentNaming
        public int ID { get; set; }
        public string CatalogNumber { get; set; }
        // ReSharper disable once InconsistentNaming
        public int CipID { get; set; }
        // ReSharper disable once InconsistentNaming
        public int DriveTypeID { get; set; }
        public float RatedVoltage { get; set; }
        // ReSharper disable once InconsistentNaming
        public int MotorFamilyID { get; set; }
    }
}
