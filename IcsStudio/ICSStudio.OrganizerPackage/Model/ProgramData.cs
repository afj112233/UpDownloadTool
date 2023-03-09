using System.Collections.Generic;
using GalaSoft.MvvmLight;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.OrganizerPackage.Model
{
    public class ProgramData : ViewModelBase
    {
        private string _description;
        private string _scanTimesMax;
        private string _scanTimesLast;
        private List<string> _configurationData;

        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        public string ScanTimesMax
        {
            get { return _scanTimesMax; }
            set { Set(ref _scanTimesMax, value); }
        }

        public string ScanTimesLast
        {
            get { return _scanTimesLast; }
            set { Set(ref _scanTimesLast, value); }
        }

        public List<string> ConfigurationData
        {
            get { return _configurationData; }
            set { Set(ref _configurationData, value); }
        }


        public ProgramData(IProgram program)
        {
            Description = program.Description;
            ScanTimesMax = program.MaxScanTime.ToString();
            ScanTimesLast = program.LastScanTime.ToString();
            ConfigurationData = new List<string>
            {
                program.MainRoutineName == string.Empty ? "none" : program.MainRoutineName,
                program.Inhibited ? "Yes" : "No",
                program.FaultRoutineName == string.Empty ? "none" : program.FaultRoutineName,
                "Synchronize redundancy data after execution : ",
                "Yes"
            };
        }
    }
}
