using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSStudio.Dialogs.BrowseString.RichTextBoxExtend;
using ICSStudio.Interfaces.DataType;

namespace ICSStudio.Dialogs.BrowseString
{
    /// <summary>
    /// BrowseStringDialog.xaml 的交互逻辑
    /// </summary>
    public partial class BrowseStringDialog : Window
    {
        public BrowseStringDialog(IDataType dataType, Message message)
        {
            InitializeComponent();
            var browse=new BrowseString(dataType,message);
            Grid.Children.Add(browse);
            DataContext = browse.DataContext;
        }

        private void DisableMaxmizebox(bool isDisable)
        {
            int GWL_STYLE = -16;
            int WS_MAXIMIZEBOX = 0x00010000;
            int WS_MINIMIZEBOX = 0x00020000;
            int SWP_NOSIZE = 0x0001;
            int SWP_NOMOVE = 0x0002;
            int SWP_FRAMECHANGED = 0x0020;
            IntPtr handle = new WindowInteropHelper(this).Handle;
            int nStyle = GetWindowLong(handle, GWL_STYLE);
            if (isDisable)
            {
                nStyle &= ~(WS_MAXIMIZEBOX);
                nStyle &= ~(WS_MINIMIZEBOX);
            }
            else
            {
                nStyle |= WS_MAXIMIZEBOX;
                nStyle |= WS_MINIMIZEBOX;
            }

            SetWindowLong(handle, GWL_STYLE, nStyle);
            SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_FRAMECHANGED);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DisableMaxmizebox(true);
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hMenu, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy,
            int uFlags);
    }
}
