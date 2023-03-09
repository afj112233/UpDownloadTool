using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.Command;
using ICSStudio.EditorPackage.MonitorEditTags.Commands;
using ICSStudio.EditorPackage.MonitorEditTags.Models;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.UIInterfaces.Dialog;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json.Linq;

namespace ICSStudio.EditorPackage.MonitorEditTags
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal partial class MonitorTagsViewModel
    {
        public RelayCommand<IList> SelectionChangedCommand { get; }

        public RelayCommand<string> SortCommand { get; }
        public RelayCommand<string> SortIncludeTagMembersCommand { get; }

        public RelayCommand NewTagCommand { get; }
        public RelayCommand CopyCommand { get; }
        public RelayCommand CutCommand { get; }
        public RelayCommand PasteCommand { get; }
        public RelayCommand DeleteCommand { get; }

        private void OnSelectionChanged(IList selectedItems)
        {
            SelectedMonitorTagItems = selectedItems.Cast<MonitorTagItem>().ToList();
        }

        #region Sort

        private void ExecuteSort(string columnHeader)
        {
            var selectedItem = SelectedMonitorTagItem;

            _descending = !_descending;

            Sort(_descending, _includeTagMembersInSorting);

            if (selectedItem != null)
                SelectedMonitorTagItem = selectedItem;
        }

        private void ExecuteSortIncludeTagMembers(string columnHeader)
        {
            var selectedItem = SelectedMonitorTagItem;

            _includeTagMembersInSorting = !_includeTagMembersInSorting;

            Sort(_descending, _includeTagMembersInSorting);

            RaisePropertyChanged("IncludeTagMembersInSorting");

            if (selectedItem != null)
                SelectedMonitorTagItem = selectedItem;
        }

        private void Sort(bool descending = false, bool includeMember = false)
        {
            //TODO(gjc): edit here later, very slow!!!
            var defaultView = CollectionViewSource.GetDefaultView(MonitorTagCollection) as ListCollectionView;
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

        #endregion

        private bool CanExecuteNewTag()
        {
            return true;
        }

        private void ExecuteNewTag()
        {
            ICreateDialogService createDialogService =
                Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;

            if (createDialogService != null)
            {
                if (IsInAOI)
                {
                    var dialog = createDialogService.CreateNewAoiTagDialog(
                        DINT.Inst,
                        Scope?.Tags?.ParentProgram as AoiDefinition,
                        Usage.Local,
                    true);

                    dialog.Owner = Application.Current.MainWindow;
                    dialog.ShowDialog();
                }
                else
                {
                    var dialog = createDialogService.CreateNewTagDialog(
                        DINT.Inst,
                        Scope?.Tags);

                    dialog.Owner = Application.Current.MainWindow;
                    dialog.ShowDialog();
                }
            }
        }

        private bool CanExecuteCopy()
        {
            if (SelectedMonitorTagItems == null)
                return false;

            if (SelectedMonitorTagItems.Count == 0)
                return false;

            foreach (var item in SelectedMonitorTagItems)
            {
                if (item.ParentItem == null)
                    return true;
            }

            return false;
        }

        private void ExecuteCopy()
        {
            if (CanExecuteCopy())
            {
                Clipboard.Clear();

                var data = CreateCopyDataObject();

                try
                {
                    Clipboard.SetDataObject(data, true);
                }
                catch (ExternalException)
                {
                    // Apparently this exception sometimes happens randomly.
                    // The MS controls just ignore it, so we'll do the same.
                }

            }
        }

        private DataObject CreateCopyDataObject()
        {
            DataObject data = new DataObject();

            JArray tagArray = new JArray();

            foreach (var item in SelectedMonitorTagItems)
            {
                if (item.ParentItem == null)
                {
                    tagArray.Add(item.Tag.ConvertToJObject());
                }
            }

            data.SetData(MonitorEditTagsCommands.CopyTagsFormat, tagArray.ToString());

            return data;
        }

        private void ExecuteCut()
        {
            // do nothing
        }

        private bool CanExecuteCut()
        {
            return false;
        }

        private void ExecutePaste()
        {
            // do nothing
        }

        private bool CanExecutePaste()
        {
            return false;
        }

        private void ExecuteDelete()
        {
            // do nothing
        }

        private bool CanExecuteDelete()
        {
            return false;
        }
    }
}
