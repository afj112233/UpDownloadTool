using System;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using ICSStudio.Cip.Objects;
using NLog;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    public class DefaultViewModel : ViewModelBase, IAxisCIPDrivePanel
    {
        protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        protected AxisCIPDrivePropertiesViewModel ParentViewModel;
        private bool _isDirty;

        public DefaultViewModel(UserControl panel,
            AxisCIPDrivePropertiesViewModel parentViewModel)
        {
            Control = panel;
            panel.DataContext = this;

            ParentViewModel = parentViewModel;
            CompareProperties = new string[0];
            PeriodicRefreshProperties = new string[0];
        }

        protected CIPAxis OriginalCIPAxis => ParentViewModel.OriginalAxisCIPDrive.CIPAxis;
        protected CIPAxis ModifiedCIPAxis => ParentViewModel.ModifiedAxisCIPDrive.CIPAxis;

        public string[] CompareProperties { get; protected set; }
        public string[] PeriodicRefreshProperties { get; protected set; }

        public object Owner { get; set; }
        public object Control { get; }

        public virtual void LoadOptions()
        {
        }

        public virtual bool SaveOptions()
        {
            return true;
        }

        public virtual Visibility Visibility => Visibility.Visible;

        public virtual void Show()
        {
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    IsDirtyChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler IsDirtyChanged;

        protected virtual bool PropertiesChanged()
        {
            if (!CipAttributeHelper.EqualByAttributeNames(
                OriginalCIPAxis,
                ModifiedCIPAxis,
                CompareProperties))
                return true;

            return false;
        }

        public void CheckDirty()
        {
            IsDirty = PropertiesChanged();
        }

        public virtual int CheckValid()
        {
            return 0;
        }
    }
}