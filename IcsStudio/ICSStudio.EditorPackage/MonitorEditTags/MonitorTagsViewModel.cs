using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.EditorPackage.MonitorEditTags.Models;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Notification;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Notification;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.EditorPackage.MonitorEditTags
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    [SuppressMessage("ReSharper", "NotResolvedInText")]
    internal partial class MonitorTagsViewModel : ViewModelBase, IConsumer
    {
        private MonitorTagItem _selectedItem;

        private bool _descending;
        private bool _includeTagMembersInSorting;
        private readonly object _collectionLock = new object();

        public MonitorTagsViewModel(IController controller)
        {
            Controller = controller;
            MonitorTagCollection = new MonitorTagCollection();

            OldSelectItem = null;

            //TODO(gjc): need test later for 
            BindingOperations.EnableCollectionSynchronization(MonitorTagCollection, _collectionLock);

            SelectionChangedCommand = new RelayCommand<IList>(OnSelectionChanged);

            SortCommand = new RelayCommand<string>(ExecuteSort);
            SortIncludeTagMembersCommand = new RelayCommand<string>(ExecuteSortIncludeTagMembers);

            NewTagCommand = new RelayCommand(ExecuteNewTag, CanExecuteNewTag);
            CopyCommand = new RelayCommand(ExecuteCopy, CanExecuteCopy);
            CutCommand = new RelayCommand(ExecuteCut, CanExecuteCut);
            PasteCommand = new RelayCommand(ExecutePaste, CanExecutePaste);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);

            Notifications.ConnectConsumer(this);

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(ValueColumnName));
        }

        public IController Controller { get; }
        public ITagCollectionContainer Scope => MonitorTagCollection?.Scope;
        public MonitorTagCollection MonitorTagCollection { get; set; }

        public TagItem OldSelectItem { get; set; }

        public MonitorTagItem SelectedMonitorTagItem
        {
            get { return _selectedItem; }
            set
            {
                if (value != null)
                {
                    OldSelectItem = _selectedItem;
                }

                Set(ref _selectedItem, value);
            }
        }

        public List<MonitorTagItem> SelectedMonitorTagItems { get; private set; }

        public bool IsInAOI => Scope?.Tags?.ParentProgram is AoiDefinition;

        public string ValueColumnName
        {
            get
            {
                if (IsInAOI)
                {
                    if (DataContext?.ReferenceAoi == null)
                        return LanguageManager.GetInstance().ConvertSpecifier("Default");
                }


                return LanguageManager.GetInstance().ConvertSpecifier("Value");
            }
        }

        public Visibility UsageVisibility
        {
            get
            {
                if (Scope == Controller)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public bool IncludeTagMembersInSorting => _includeTagMembersInSorting;

        public override void Cleanup()
        {
            Notifications.DisconnectConsumer(this);

            MonitorTagCollection.Update(null);

            BindingOperations.DisableCollectionSynchronization(MonitorTagCollection);

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);

            base.Cleanup();
        }

        public void Update(ITagCollectionContainer scope)
        {
            MonitorTagCollection.Update(scope);

            RaisePropertyChanged("ValueColumnName");
            RaisePropertyChanged("UsageVisibility");
        }

        public bool SetFocusName(string focusName)
        {
            SelectedMonitorTagItem = MonitorTagCollection.GetTagItemAndExpandByName(focusName);
            return SelectedMonitorTagItem != null;
        }

        public void Consume(MessageData message)
        {
            if (message.Type == MessageData.MessageType.PullFinished
                || message.Type == MessageData.MessageType.Restored)
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(
                    async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        foreach (var item in MonitorTagCollection)
                        {
                            item.OnPropertyChanged("Value");
                        }
                    });
            }
        }

        public void UpdateDataContext(AoiDataReference dataContext)
        {
            DataContext = dataContext;
            MonitorTagCollection.UpdateDataContext(dataContext);
            RaisePropertyChanged("ValueColumnName");
            RaisePropertyChanged("CanConfig");
        }

        public AoiDataReference DataContext { get; private set; }
    }
}