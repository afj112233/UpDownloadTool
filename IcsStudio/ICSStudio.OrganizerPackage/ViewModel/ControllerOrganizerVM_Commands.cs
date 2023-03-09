using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using ICSStudio.MultiLanguage;
using ICSStudio.OrganizerPackage.Commands;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.OrganizerPackage.ViewModel
{
    public partial class ControllerOrganizerVM
    {
        public RelayCommand CopyCommand { get; }
        public RelayCommand CutCommand { get; }
        public RelayCommand PasteCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand CrossReferenceCommand { get; }

        public RelayCommand CollapseAllCommand { get; }
        public RelayCommand ExpandAllCommand { get; }
        public RelayCommand HideControllerOrganizerCommand { get; }

        private ClipboardCommand Clipboard => ClipboardCommand.Instance;

        public RelayCommand<RoutedPropertyChangedEventArgs<object>> SelectedItemChangedCommand { get; }

        private bool CanExecuteCopy()
        {
            return SelectedItems.Count > 0 && Clipboard.CanCopy(SelectedItems[0]);
        }

        private bool CanExecuteCut()
        {
            return SelectedItems.Count > 0 && Clipboard.CanCut(SelectedItems[0]);
        }

        private bool CanExecutePaste()
        {
            if (SelectedItems[0].Kind == UIInterfaces.Project.ProjectItemType.Tasks 
                && SelectedItems[0].ProjectItems.Count >= 17)
            {
                return false;
            }

            if (SelectedItems[0].Kind == UIInterfaces.Project.ProjectItemType.Task 
                && SelectedItems[0].ProjectItems.Count >= 1000)
            {
                return false;
            }

            return SelectedItems.Count > 0 && Clipboard.CanPaste(SelectedItems[0]);
        }

        private bool CanExecuteDelete()
        {
            return CanExecuteCut();
        }

        private void ExecuteCopy()
        {
            Clipboard.Copy();
        }

        private void ExecuteCut()
        {
            Clipboard.Cut();
        }

        private void ExecuteDelete()
        {
            Clipboard.Delete();
        }

        private void ExecutePaste()
        {
            Clipboard.Paste();
        }

        private void ExecuteCrossReference()
        {
            //TODO(zyl):cross reference
        }

        private void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> args)
        {
            //Debug.WriteLine(SelectedItems.Count);
            //Console.WriteLine((args.NewValue as OrganizerItem).Kind.ToString());
            //OrganizerItem newItem = args.NewValue as OrganizerItem;

            //_selectedItem = newItem;

            ////int count = SelectedItems.Count;
            ////Debug.WriteLine($"select item count: {count}");

            //if (newItem != null)
            //{
            //    SetSimpleInfoTreeView(newItem);
            //}
        }

        public void ExecuteHideControllerOrganizer()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Package package = _provider as Package;
            if (package != null)
            {
                var window = package.FindToolWindow(typeof(ControllerOrganizer), 0, false);
                IVsWindowFrame windowFrame = (IVsWindowFrame) window.Frame;
                windowFrame?.CloseFrame((int) __FRAMECLOSE.FRAMECLOSE_SaveIfDirty);
            }
        }

        private void ExecuteExpandAll()
        {
            foreach (var item in _root.ProjectItems)
            {
                ExpandAllOrganizerItem((OrganizerItem) item);
            }
        }

        private void ExpandAllOrganizerItem(OrganizerItem items)
        {
            if (items != null)
            {
                items.IsExpanded = true;
                foreach (OrganizerItem item in items.ProjectItems)
                {
                    ExpandAllOrganizerItem(item);
                }
            }
        }

        private void ExecuteCollapseAll()
        {
            foreach (var item in _root.ProjectItems)
            {
                CollapseOrganizerItem((OrganizerItem) item);
            }
            SelectedItems.Clear();
        }

        private void CollapseOrganizerItem(OrganizerItem items)
        {
            if (items != null)
            {
                items.IsExpanded = false;
                foreach (OrganizerItem item in items.ProjectItems)
                {
                    CollapseOrganizerItem(item);
                }
            }
        }

        private void OnSelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SelectionContainer selectionContainer = new SelectionContainer();
            ArrayList items = new ArrayList(SelectedItems.ToList());

            selectionContainer.SelectableObjects = items;
            selectionContainer.SelectedObjects = items;

            ThreadHelper.ThrowIfNotOnUIThread();
            _track.OnSelectChange(selectionContainer);

            if (SelectedItems.Count > 0)
            {
                SetSimpleInfoTreeView(SelectedItems[0]);
            }
        }
        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            if (SelectedItems.Count > 0)
            {
                SetSimpleInfoTreeView(SelectedItems[0]);
            }
        }
    }
}
