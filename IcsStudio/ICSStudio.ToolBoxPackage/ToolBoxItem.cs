using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ICSStudio.ToolBoxPackage
{
    public class ToolBoxItem : ObservableObject
    {
        private bool _isExpanded;
        private string _displayName;
        private string _toolTip;
        private string _iconKind;
        private ToolBoxItemType _kind;
        public string Name { get; set; }
        public ToolBoxItems Parent { get; set; }
        public ToolBoxItems Children { get; set; }

        public ToolBoxItem()
        {
            IsExpanded = true;
            Children = new ToolBoxItems(this);
            MouseDown = new RelayCommand<RoutedEventArgs>(ExecuteMouseDown);
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                Set(ref _isExpanded, value);
                //UpdateIconKind();
            }
        }

        [ReadOnly(true)]
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                Set(ref _displayName, value);
                ToolTip = _displayName;
            }
        }

        public string ToolTip
        {
            get { return _toolTip; }
            set { Set(ref _toolTip, value); }
        }

        public string IconKind
        {
            get { return _iconKind; }
            set { Set(ref _iconKind, value); }
        }

        public ToolBoxItemType Kind
        {
            get { return _kind; }
            set { Set(ref _kind, value); }
        }

        //void UpdateIconKind()
        //{
        //    //if(Kind== ToolBoxItemType.Category)
        //}

        public RelayCommand<RoutedEventArgs> MouseDown { get; }

        private void ExecuteMouseDown(RoutedEventArgs args)
        {
            var source = args.Source as TextBlock;
            if (source == null)
                return;

            if (Kind == ToolBoxItemType.Category)
                return;

            var dragData = new DataObject("ICSToolBoxItem", source.Text);
            DragDrop.DoDragDrop(source, dragData, DragDropEffects.All);

            EditorPackage.RLLEditor.RLLEditorControl.ClearDropPoints();
        }
    }

    public enum ToolBoxItemType
    {
        Rung,
        Category,
        Instruction
    }
}
