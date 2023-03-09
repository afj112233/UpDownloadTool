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
using Newtonsoft.Json.Linq;

namespace ICSStudio.ToolsPackage.Import
{
    /// <summary>
    /// ImportingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ImportingDialog : Window
    {
        private readonly ImportDialogVM _importDialogVM;
        private readonly List<JObject> _tags;
        public ImportingDialog(List<JObject> tags)
        {
            InitializeComponent();
            Loaded += ImportingDialog_Loaded;
            _importDialogVM=new ImportDialogVM();
            DataContext = _importDialogVM;
            _tags = tags;
        }

        private void ImportingDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            _importDialogVM.Start(_tags);
        }

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
