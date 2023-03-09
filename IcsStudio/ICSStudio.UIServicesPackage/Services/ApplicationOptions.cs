namespace ICSStudio.UIServicesPackage.Services
{
    public class ApplicationOptions
    {
        public ApplicationOptions()
        {
            EnableAutomaticProjectBackup = true;
            NumberOfBackups = 3;
        }

        public bool EnableAutomaticProjectBackup { get; set; }
        public int NumberOfBackups { get; set; }
    }
}
