using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSStudio.Gui.View;
using ICSStudio.MultiLanguage;
using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.EditorPackage.DataTypes
{
    /// <summary>
    /// NewDataType.xaml 的交互逻辑
    /// </summary>
    public partial class NewDataType : UserControl
    {
        private bool _isInit;

        public NewDataType()
        {
            InitializeComponent();
            Loaded += NewDataType_Loaded;
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void NewDataType_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
            {
                var mainDock = VisualTreeHelpers.FindVisualParentByName(this, "MainDockTarget");
                var panel = VisualTreeHelpers.GetChildObject(mainDock, "PART_TabPanel") as Panel;
                if (panel != null)
                {
                    FrameworkElement item = null;
                    foreach (var child in panel.Children)
                    {
                        var tabItem = child as TabItem;
                        if (tabItem?.IsSelected ?? false)
                            item = tabItem;
                    }

                    if (item != null)
                    {
                        var button = VisualTreeHelpers.GetChildObject(item, "HideButton") as Button;
                        if (button != null)
                            button.PreviewMouseLeftButtonUp += Button_PreviewMouseLeftButtonUp; ;
                    }
                    _isInit = true;
                }
            }
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var vm = DataContext as NewDataTypeViewModel;
            if (vm == null) return;

            if (vm.IsDirty)
            {
                var message = "Would you like to apply modifications?";
                var result = MessageBox.Show(message, "ICS Studio", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        {
                            if (vm.OkCommand.CanExecute(null))
                            {
                                vm.OkCommand.Execute(null);
                            }

                            if (!vm.IsDirty)
                            {
                                ((Button)sender).PreviewMouseLeftButtonUp -= Button_PreviewMouseLeftButtonUp;
                            }

                            break;
                        }
                    case MessageBoxResult.No:
                        {
                            ((Button)sender).PreviewMouseLeftButtonUp -= Button_PreviewMouseLeftButtonUp;
                            vm.CloseAction?.Invoke();

                            break;
                        }
                    case MessageBoxResult.Cancel:
                        break;
                }
            }
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void DataGrid_OnPreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            var ss = VisualTreeHelpers.FindFirstVisualChildOfType<TextBox>(e.EditingElement);
            Keyboard.Focus((TextBox)ss);
        }

        private void TextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                var textBox = ((TextBox)sender);
                var caretIndex = textBox.CaretIndex;
                textBox.Text = ((TextBox)sender).Text.Insert(caretIndex, "_");
                textBox.CaretIndex = caretIndex + 1;
                e.Handled = true;
            }
        }

        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right)
            {
                return;
            }
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                e.Handled = true;
            }
            else if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Tab || e.Key == Key.Enter))
            {
                e.Handled = true;
            }
        }
    }

    public class BoolToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (bool)value) return new Thickness(1);
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
