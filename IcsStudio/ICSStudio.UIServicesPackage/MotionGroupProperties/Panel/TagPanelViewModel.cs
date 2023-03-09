using System;
using System.Windows;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.MotionGroupProperties.Panel
{
    class TagPanelViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;
        private string _tagName;
        private string _description;

        public TagPanelViewModel(TagPanel panel, ITag motionGroup)
        {
            Control = panel;
            panel.DataContext = this;

            MotionGroup = motionGroup;

            TagName = motionGroup.Name;
            Description = motionGroup.Description;

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                motionGroup.ParentController as Controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public string TagName
        {
            set
            {
                _tagName = value;
                Compare();
            }
            get { return _tagName; }
        }

        public string Description
        {
            set
            {
                _description = value;
                Compare();
            }
            get { return _description; }
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
                if (_isDirty != value)
                {
                    _isDirty = value;
                    IsDirtyChanged?.Invoke(this, EventArgs.Empty);
                }

                Set(ref _isDirty, value);
            }
        }

        public bool IsEnabled => !MotionGroup.ParentController.IsOnline;

        public event EventHandler IsDirtyChanged;

        public ITag MotionGroup { get; }

        public void Compare()
        {
            if (TagName == MotionGroup.Name &&
                string.IsNullOrEmpty(Description) == string.IsNullOrEmpty(MotionGroup.Description) &&
                Description == MotionGroup.Description)
                IsDirty = false;
            else
                IsDirty = true;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                MotionGroup.ParentController as Controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                RaisePropertyChanged(nameof(IsEnabled));
            });
        }
    }
}
