using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.DeviceProperties.Common
{
    public class DeviceOptionPanel : ViewModelBase, IOptionPanel
    {
        private readonly Controller _controller;
        private readonly LanguageManager _languageManager;
        private bool _isDirty;

        public DeviceOptionPanel(UserControl control)
        {
            Contract.Assert(control != null);
            control.DataContext = this;
            Control = control;

            _controller = Controller.GetInstance();
            _languageManager = LanguageManager.GetInstance();

            LanguageManager.GetInstance().SetLanguage(control);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(_languageManager, "LanguageChanged", LanguageChanged);
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
        }
        
        public object Control { get; }
        public virtual Visibility Visibility { get; } = Visibility.Visible;

        public virtual bool IsOnline => _controller.IsOnline;

        public void LanguageChanged(object sender, EventArgs e)
        {
            if (Control is UserControl)
                LanguageManager.GetInstance().SetLanguage(Control as UserControl);
        }

        public override void Cleanup()
        {
            base.Cleanup();
            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(_languageManager, "LanguageChanged", LanguageChanged);
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public virtual void Show()
        {

        }

        public virtual void Hide()
        {

        }

        public virtual int CheckValid()
        {
            return 0;
        }

        /// <summary>
        /// final to set IsDirty
        /// </summary>
        public virtual void CheckDirty()
        {
        }

        public virtual void LoadOptions()
        {

        }

        public virtual bool SaveOptions()
        {
            return true;
        }

        public virtual void Refresh()
        {
            RaisePropertyChanged(nameof(IsOnline));
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

        public virtual void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                Refresh();
            });
        }
    }
}
