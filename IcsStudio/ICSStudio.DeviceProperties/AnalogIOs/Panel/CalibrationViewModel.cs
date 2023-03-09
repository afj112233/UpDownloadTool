using System.Collections.ObjectModel;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DeviceProfiles.DIOModule;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.AnalogIOs.Panel
{
    public class CalibrationViewModel : DeviceOptionPanel
    {
        public CalibrationViewModel(UserControl panel, ModifiedAnalogIO modifiedAnalogIO) : base(panel)
        {
            ModifiedAnalogIO = modifiedAnalogIO;

            CreateCalibrateChannels();

            StartCommand = new RelayCommand(ExecuteStart, CanExecuteStart);
        }

        public ModifiedAnalogIO ModifiedAnalogIO { get; }

        public AnalogIO OriginalAnalogIO => ModifiedAnalogIO?.OriginalAnalogIO;

        public DIOModuleProfiles Profiles => OriginalAnalogIO?.Profiles;

        public ObservableCollection<CalibrateChannelItem> CalibrateChannels { get; private set; }

        public RelayCommand StartCommand { get; }

        private void CreateCalibrateChannels()
        {
            CalibrateChannels = new ObservableCollection<CalibrateChannelItem>();

            var module = Profiles.GetModule(ModifiedAnalogIO.Major);

            if (module != null)
            {
                for (var i = 0; i < module.NumberOfInputs; i++)
                {
                    var item = new CalibrateChannelItem(i, false, "4 to 20 mA");
                    CalibrateChannels.Add(item);
                }
                
                for (var i = 0; i < module.NumberOfOutputs; i++)
                {
                    var item = new CalibrateChannelItem(i, false, "4 to 20 mA");
                    CalibrateChannels.Add(item);
                }
            }

        }

        private bool CanExecuteStart()
        {
            return false;
        }

        private void ExecuteStart()
        {
            //TODO(gjc): add code here
        }
    }

    public class CalibrateChannelItem : ObservableObject
    {
        public CalibrateChannelItem(int index, bool isCalibrated, string range)
        {
            ChannelIndex = index;
            IsCalibrated = isCalibrated;
            CalibrationRange = range;
        }

        public int ChannelIndex { get; }
        public bool IsCalibrated { get; set; }
        public string CalibrationRange { get; set; }
    }
}
