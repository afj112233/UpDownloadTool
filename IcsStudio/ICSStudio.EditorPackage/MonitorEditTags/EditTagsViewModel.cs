using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.EditorPackage.MonitorEditTags.Models;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Editor;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.EditorPackage.MonitorEditTags
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal partial class EditTagsViewModel : ViewModelBase
    {
        private EditTagItem _selectedItem;

        private bool _descending;
        private bool _includeTagMembersInSorting;
        private readonly object _collectionLock = new object();

        public EditTagsViewModel(IController controller)
        {
            Controller = controller;
            EditTagCollection = new EditTagCollection();
            var defaultView = CollectionViewSource.GetDefaultView(EditTagCollection) as IEditableCollectionView;
            Contract.Assert(defaultView != null);
            Contract.Assert(defaultView.CanAddNew);

            //TODO(gjc): need test later for 
            BindingOperations.EnableCollectionSynchronization(EditTagCollection, _collectionLock);

            SortCommand = new RelayCommand<string>(ExecuteSort);
            SortIncludeTagMembersCommand = new RelayCommand<string>(ExecuteSortIncludeTagMembers);

            SelectionChangedCommand = new RelayCommand<IList>(OnSelectionChanged);
            MonitorCommand = new RelayCommand<TagItem>(ExecuteMonitor);

            CopyCommand = new RelayCommand(ExecuteCopy, CanExecuteCopy);
            CutCommand = new RelayCommand(ExecuteCut, CanExecuteCut);
            PasteCommand = new RelayCommand(ExecutePaste, CanExecutePaste);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            UpdateAllDataTypeNames();
        }

        public override void Cleanup()
        {
            EditTagCollection.Update(null);

            base.Cleanup();
        }

        public IController Controller { get; }
        public ITagCollectionContainer Scope => EditTagCollection?.Scope;
        public bool IsInAOI => Scope?.Tags?.ParentProgram is AoiDefinition;

        public EditTagCollection EditTagCollection { get; set; }

        public EditTagItem SelectedEditTagItem
        {
            get { return _selectedItem; }
            set { Set(ref _selectedItem, value); }
        }

        public List<EditTagItem> SelectedEditTagItems { get; private set; }

        public bool IncludeTagMembersInSorting => _includeTagMembersInSorting;
        public ObservableCollection<string> AllDataTypeNames { get; private set; }

        public Visibility UsageVisibility
        {
            get
            {
                if (Scope == Controller)
                    return Visibility.Collapsed;

                return Visibility.Visible;
            }
        }

        public bool CanAddRows
        {
            get
            {
                if (!Controller.IsOnline)
                    return true;

                if (Controller.IsOnline && !IsInAOI)
                    return true;

                return false;
            }
        }

        public bool IsDataTypeReadOnly =>
            //TODO(gjc): add code here
            //return Controller.IsOnline;
            false;

        public void Update(ITagCollectionContainer scope)
        {
            EditTagCollection.Update(scope);

            RaisePropertyChanged("UsageVisibility");
            RaisePropertyChanged("CanAddRows");
        }

        public void UpdateUI()
        {
            RaisePropertyChanged("IsDataTypeReadOnly");
            RaisePropertyChanged("CanAddRows");
        }

        public bool SetFocusName(string focusName)
        {
            SelectedEditTagItem = EditTagCollection.GetTagItemAndExpandByName(focusName);
            return SelectedEditTagItem != null;
        }

        public void Sort(bool descending = false, bool includeMember = false)
        {
            //TODO(gjc): edit here later, very slow!!!
            var defaultView = CollectionViewSource.GetDefaultView(EditTagCollection) as ListCollectionView;
            if (defaultView != null)
            {
                if (defaultView.IsAddingNew || defaultView.IsEditingItem)
                    return;

                using (defaultView.DeferRefresh())
                {
                    defaultView.CustomSort = new TagItemComparer(descending, includeMember);
                }

            }
        }

        private void ExecuteMonitor(TagItem tagItem)
        {
            var createEditorService =
                Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

            createEditorService?.CreateMonitorEditTags(Controller,
                Scope, tagItem != null ? tagItem.Name : string.Empty);
        }

        private void UpdateAllDataTypeNames()
        {
            var dataTypeNameList = new List<string>();
            var disableDataTypes = new DisableDataTypes();

            foreach (var dataType in Controller.DataTypes)
            {
                if (dataType.Name.StartsWith("BOOL:", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (dataType.Name.Contains("$"))
                    continue;

                if(disableDataTypes.DisableData.Contains(dataType.Name))
                    continue;

                dataTypeNameList.Add(dataType.Name);
            }

            dataTypeNameList.Sort((x, y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase));

            AllDataTypeNames = new ObservableCollection<string>(dataTypeNameList);
        }

    }
}