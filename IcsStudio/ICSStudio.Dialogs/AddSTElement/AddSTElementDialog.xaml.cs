using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;

namespace ICSStudio.Dialogs.AddSTElement
{
    /// <summary>
    ///     AddSTElementDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddSTElementDialog : Window
    {
        private readonly AddSTElementViewModel _addSTElementViewModel;

        public AddSTElementDialog(IController controller, string name = "")
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);

            _addSTElementViewModel = new AddSTElementViewModel(name, controller);
            DataContext = _addSTElementViewModel;
        }

        public string SelectedElement => _addSTElementViewModel.SelectedElement?.Name;

        private void AddSTElementDialog_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Math.Abs(e.PreviousSize.Width) < double.Epsilon) return;
            var vector = e.NewSize.Width - e.PreviousSize.Width;
            _addSTElementViewModel.DescriptionWidth += vector;
        }

        //private void EventSetter_OnHandler(object sender, RoutedEventArgs e)
        //{
        //    var item = (TreeViewItem) sender;
        //    var treeView = FindVisualParent<TreeView>(item);
        //    if (treeView.Count > 0)
        //    {
        //        treeView[0].ScrollToCenterOfView(item);
        //        item.Focus();
        //    }
        //}

        //private static List<T> FindVisualParent<T>(DependencyObject obj) where T : DependencyObject
        //{
        //    try
        //    {
        //        List<T> TList = new List<T> { };
        //        DependencyObject parent = VisualTreeHelper.GetParent(obj);
        //        if (parent != null && parent is T)
        //        {
        //            TList.Add((T)parent);
        //            List<T> parentOfParent = FindVisualParent<T>(parent);
        //            if (parentOfParent != null)
        //            {
        //                TList.AddRange(parentOfParent);
        //            }
        //        }
        //        else if (parent != null)
        //        {
        //            List<T> parentOfParent = FindVisualParent<T>(parent);
        //            if (parentOfParent != null)
        //            {
        //                TList.AddRange(parentOfParent);
        //            }
        //        }
        //        return TList;
        //    }
        //    catch (Exception ee)
        //    {
        //        MessageBox.Show(ee.Message);
        //        return null;
        //    }
        //}

        private void EventSetter_OnHandler(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;
            item?.BringIntoView();
        }

        private void EventSetter_OnHandler2(object sender, MouseButtonEventArgs e)
        {
            _addSTElementViewModel.ExecuteOkCommand();
        }
    }
}