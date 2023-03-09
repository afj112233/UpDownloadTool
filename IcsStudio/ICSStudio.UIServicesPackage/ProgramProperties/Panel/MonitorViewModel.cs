using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.ProgramProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    class MonitorViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly Program _program;

        private bool _isDirty;

        private readonly DispatcherTimer _timer;

        public MonitorViewModel(Monitor panel, IProgram program)
        {
            Control = panel;
            panel.DataContext = this;

            _program = program as Program;
            Contract.Assert(_program != null);

            ResetCommand = new RelayCommand(ExecuteResetCommand, CanExecuteResetCommand);

            Controller controller = program.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            _timer.Tick += OnTick;
            if (IsOnline)
                _timer.Start();
        }

        public override void Cleanup()
        {
            _timer.Stop();
            base.Cleanup();
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                Controller.GetInstance(), "IsOnlineChanged", OnIsOnlineChanged);
        }

        private void OnTick(object sender, EventArgs e)
        {
            RaisePropertyChanged("MaxScanTime");
            RaisePropertyChanged("LastScanTime");
        }

        public RelayCommand ResetCommand { get; }

        private void ExecuteResetCommand()
        {
            _program.ResetMaxScanTime();
        }

        private bool CanExecuteResetCommand()
        {
            return IsOnline;
        }

        public bool IsOnline => _program.ParentController.IsOnline;

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                RaisePropertyChanged("IsOnline");
                RaisePropertyChanged("MaxScanTime");
                RaisePropertyChanged("LastScanTime");
                ResetCommand.RaiseCanExecuteChanged();

                if (IsOnline)
                    _timer.Start();
                else
                    _timer.Stop();

            });
        }

        public void Compare()
        {
        }

        public Visibility Type2Visibility
        {
            get
            {
                if (_program.Type == ProgramType.Phase || _program.Type == ProgramType.Sequence)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public Visibility Type3Visibility
        {
            get
            {
                if (_program.Type == ProgramType.Sequence)
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public string MaxScanTime => IsOnline ? _program.MaxScanTime.ToString() : string.Empty;

        public string LastScanTime => IsOnline ? _program.LastScanTime.ToString() : string.Empty;

        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {
            //throw new NotImplementedException();
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
                if (_isDirty != value)
                {
                    _isDirty = value;
                    IsDirtyChanged?.Invoke(this, EventArgs.Empty);
                }

                Set(ref _isDirty, value);
            }
        }

        public event EventHandler IsDirtyChanged;
    }
}
