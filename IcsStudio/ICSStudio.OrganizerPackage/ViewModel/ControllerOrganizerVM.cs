using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Notification;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.OrganizerPackage.Model;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIInterfaces.UI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.OrganizerPackage.ViewModel
{
    public partial class ControllerOrganizerVM : ViewModelBase, IConsumer
    {
        private readonly OrganizerItem _root;

        private IServiceProvider _provider;
        private ITrackSelection _track;

        public ControllerOrganizerVM()
        {
            _root = new OrganizerItem {Name = "Root"};
            _selectedItems = new ObservableCollection<OrganizerItem>();
            _selectedItems.CollectionChanged += OnSelectedItemsChanged;
            CopyCommand = new RelayCommand(ExecuteCopy, CanExecuteCopy);
            CutCommand = new RelayCommand(ExecuteCut, CanExecuteCut);
            PasteCommand = new RelayCommand(ExecutePaste, CanExecutePaste);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            CrossReferenceCommand = new RelayCommand(ExecuteCrossReference);
            ExpandAllCommand = new RelayCommand(ExecuteExpandAll);
            CollapseAllCommand = new RelayCommand(ExecuteCollapseAll);
            HideControllerOrganizerCommand = new RelayCommand(ExecuteHideControllerOrganizer);

            SelectedItemChangedCommand =
                new RelayCommand<RoutedPropertyChangedEventArgs<object>>(OnSelectedItemChanged);

            DataGridInfo = new ObservableCollection<SimpleInfo>();
            ItemTreeViewInfo = new ObservableCollection<OrganizerItemInfo>();

            Notifications.ConnectConsumer(this);

            RebuildItems();

            IStudioUIService studioUIService =
                Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
            if (studioUIService != null)
            {
                studioUIService.OnReset += OnReset;
                studioUIService.OnDetachController += OnDetachController;
            }

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(),
                "LanguageChanged", LanguageChangedHandler);
        }

        public override void Cleanup()
        {
            Notifications.DisconnectConsumer(this);

            IStudioUIService studioUIService =
                Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
            if (studioUIService != null)
                studioUIService.OnReset -= OnReset;

            base.Cleanup();
        }

        public void Initialize(IServiceProvider provider, ITrackSelection track)
        {
            _provider = provider;
            _track = track;
        }

        private void OnReset(object sender, EventArgs e)
        {
            CleanupProjectItems();
            SelectedItems.Clear();

            ItemTreeViewInfoVisibility = Visibility.Hidden;
            DataGridInfo.Clear();
            ItemTreeViewInfo.Clear();

            RebuildItems();
        }

        private void OnDetachController(object sender, EventArgs e)
        {
            RemoveHandlers(_controller);

            if (_currentSimpleInfo != null)
            {
                _currentSimpleInfo.Clear();
                _currentSimpleInfo = null;
            }

            ItemTreeViewInfoVisibility = Visibility.Hidden;
            DataGridInfo.Clear();
            ItemTreeViewInfo.Clear();
        }

        public void Consume(MessageData message)
        {
            if (message.Type == MessageData.MessageType.DelTag)
            {
                if ((_controller?.IsLoading ?? true) || !_controller.CanVerify) return;
                var delTags = message.Object as List<ITag>;
                if (delTags != null)
                {
                    IProjectInfoService projectInfoService =
                        Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                    projectInfoService?.VerifyReference(delTags);
                }

                return;
            }

            if (message.Type == MessageData.MessageType.AddTag)
            {
                if ((_controller?.IsLoading ?? true) || !_controller.CanVerify) return;
                var createEditorService =
                    Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                createEditorService?.ParseErrorRoutine();
                return;
            }

            if (message.Type == MessageData.MessageType.Verify)
            {
                if ((_controller?.IsLoading ?? true) || !_controller.CanVerify) return;
                IProjectInfoService projectInfoService =
                    Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                if (message.Object == null)
                {
                    projectInfoService?.Verify(_controller);
                    return;
                }

                var routine = message.Object as IRoutine;
                if (routine != null)
                {
                    var createEditorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                    routine.IsError = true;
                    createEditorService?.ParseRoutine(routine, true,false,message.IsForce);

                    //projectInfoService?.Verify(routine);
                }

                var program = message.Object as IProgram;
                if (program != null)
                {
                   projectInfoService?.VerifyReferenceProgram(program);
                }
                return;
            }

            if (message.Type == MessageData.MessageType.PullFinished
                || message.Type == MessageData.MessageType.Restored)
            {
                //ThreadHelper.JoinableTaskFactory.RunAsync(
                //    async delegate
                //    {
                //        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                //        if (SelectedItems.Count > 0)
                //        {
                //            try
                //            {
                //                //SetSimpleInfoTreeView(SelectedItems[0]);
                //            }
                //            catch (Exception exception)
                //            {
                //                Debug.WriteLine(exception);
                //            }

                //        }
                //    });
            }
        }
    }
}
