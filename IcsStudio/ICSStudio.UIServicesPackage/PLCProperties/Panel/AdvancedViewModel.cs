using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.PLCProperties.Panel
{
    class AdvancedViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private IController _controller;
        private bool _isDirty;
        private bool _enable;
        private int _timeSlice;
        private string _faultHandler;
        private string _powerupHandler;
        private string _sn;
        private bool _matchProjectToController;

        public AdvancedViewModel(Advanced panel,IController controller)
        {
            Control = panel;
            panel.DataContext = this;
            _controller = controller;
            IsCheck1 = true;
            ProgramCollection.Add("<none>");
            PowerupHandler = "<none>";
            FaultHandler = "<none>";
            if (!string.IsNullOrEmpty(controller.PowerLossProgram))
            {
                ProgramCollection.Add(controller.PowerLossProgram);
                PowerupHandler = controller.PowerLossProgram;
            }

            if (!string.IsNullOrEmpty(controller.MajorFaultProgram))
            {
                ProgramCollection.Add(controller.MajorFaultProgram);
                FaultHandler = controller.MajorFaultProgram;
            }

            foreach (var p in controller.Programs.Where(p=>p.ParentTask==null))
            {
                if (!ProgramCollection.Contains(p.Name))
                    ProgramCollection.Add(p.Name);
            }

            MatchProjectToController = controller.MatchProjectToController;
            {
                if (controller.TimeSlice < 10)
                    TimeSlice = 10;
                else if(controller.TimeSlice>90)
                {
                    TimeSlice = 90;
                }
                else
                {
                    TimeSlice = controller.TimeSlice;
                }
            }
            //SN = FormatOp.RemoveFormat(controller.ProjectSN, false);
            SN = controller.ProjectSN.ToString("x8");
            Enable = !controller.IsOnline;
            controller.Programs.CollectionChanged += Programs_CollectionChanged;
            IsDirty = false;
        }

        public override void Cleanup()
        {
            _controller.Programs.CollectionChanged -= Programs_CollectionChanged;
        }

        private void Programs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (IProgram item in e.NewItems)
                {
                    ProgramCollection.Add(item.Name);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (IProgram item in e.OldItems)
                {
                    ProgramCollection.Remove(item.Name);
                }
            }
        }

        public bool Enable
        {
            set { Set(ref _enable , value); }
            get { return _enable; }
        }

        public int TimeSlice
        {
            set
            {
                //if (value < 10 || value > 90)
                //{
                //    MessageBox.Show(
                //        "Failed to modify the controller properties.\nEnter system overhead time slice in the range (10 - 90)");
                //    return;
                //}
                _timeSlice = value;
                IsDirty = true;
            }
            get { return _timeSlice; }
        }

        public string SN
        {
            set
            {
                Set(ref _sn , value);
                IsDirty = true;
            }
            get { return _sn; }
        }

        public ObservableCollection<string> ProgramCollection { set; get; }=new ObservableCollection<string>();

        public string FaultHandler
        {
            set
            {
                _faultHandler = value;
                IsDirty = true;
            }
            get { return _faultHandler; }
        }
        
        public string PowerupHandler
        {
            set
            {
                _powerupHandler = value;
                IsDirty = true;
            }
            get { return _powerupHandler; }
        }

        public bool IsCheck1 { set; get; }

        public bool IsCheck2 { set; get; }

        public bool MatchProjectToController
        {
            set
            {
                Set(ref _matchProjectToController , value);
                IsDirty = true;
            }
            get { return _matchProjectToController; }
        }

        public bool Verify()
        {
            if (FaultHandler.Equals(PowerupHandler) && !"<none>".Equals(FaultHandler) && !"<none>".Equals(PowerupHandler))
            {
                MessageBox.Show(
                    "Failed to modify the controller properties.\nProgram cannot be both Controller Fault and Power-Up handle.",
                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            if (TimeSlice < 10 || TimeSlice > 90)
            {
                MessageBox.Show(
                    "Failed to modify the controller properties.\nEnter system overhead time slice in the range (10 - 90)");
                return false;
            }

            return true;
        }

        public void Save()
        {
            var controller = _controller as Controller;
            controller.MajorFaultProgram = FaultHandler.Equals("<none>") ? String.Empty : FaultHandler;
            controller.PowerLossProgram = PowerupHandler.Equals("<none>") ? String.Empty : PowerupHandler;
            controller.TimeSlice = TimeSlice;
            //var sn = SN.PadLeft(8, '0');
            //SN = sn;
            //sn = sn.Insert(4, "_");
            //controller.ProjectSN = $"16#{sn}";
            controller.ProjectSN = Convert.ToInt32(SN, 16);
            controller.MatchProjectToController = MatchProjectToController;
            IsDirty = false;
        }

        public object Owner { get; set; }
        public object Control { get; }
        public void LoadOptions()
        {
            
        }

        public bool SaveOptions()
        {
            return true;
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                IsDirtyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsDirtyChanged;

        public void Refresh()
        {
            Enable = !_controller.IsOnline;
        }
    }
}
