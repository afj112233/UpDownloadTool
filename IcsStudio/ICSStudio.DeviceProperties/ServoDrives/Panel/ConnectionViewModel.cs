using System.Windows.Controls;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.ServoDrives.Panel
{
    public class ConnectionViewModel : DeviceOptionPanel
    {
        public ConnectionViewModel(UserControl panel, ModifiedMotionDrive motionDrive) : base(panel)
        {
            ModifiedMotionDrive = motionDrive;
        }

        public ModifiedMotionDrive ModifiedMotionDrive { get; }
        public CIPMotionDrive OriginalMotionDrive => ModifiedMotionDrive.OriginalMotionDrive;

        public bool Inhibited
        {
            get { return ModifiedMotionDrive.Inhibited; }
            set
            {
                if (ModifiedMotionDrive.Inhibited != value)
                {
                    ModifiedMotionDrive.Inhibited = value;

                    RaisePropertyChanged();

                    CheckDirty();
                }
            }
        }

        public bool IsInhibitedEnabled => !IsOnline;

        public bool IsMajorOnControllerEnabled => !IsOnline;

        public bool MajorFault
        {
            get { return ModifiedMotionDrive.MajorFault; }
            set
            {
                if (ModifiedMotionDrive.MajorFault != value)
                {
                    ModifiedMotionDrive.MajorFault = value;
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
        }

        public override void CheckDirty()
        {
            if (ModifiedMotionDrive.Inhibited != OriginalMotionDrive.Inhibited)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedMotionDrive.MajorFault != OriginalMotionDrive.MajorFault)
            {
                IsDirty = true;
                return;
            }

            IsDirty = false;
        }

        public override bool SaveOptions()
        {
            OriginalMotionDrive.Inhibited = ModifiedMotionDrive.Inhibited;
            OriginalMotionDrive.MajorFault = ModifiedMotionDrive.MajorFault;

            return true;
        }

        public override void Show()
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            RaisePropertyChanged("IsInhibitedEnabled");
        }

        public override void Refresh()
        {
            base.Refresh();
            RaisePropertyChanged(nameof(IsInhibitedEnabled));
            RaisePropertyChanged(nameof(IsMajorOnControllerEnabled));
        }
    }
}
