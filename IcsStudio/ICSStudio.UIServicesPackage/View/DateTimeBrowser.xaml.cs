using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSStudio.UIServicesPackage.ViewModel;

namespace ICSStudio.UIServicesPackage.View
{
    /// <summary>
    /// DateTimeBrowser.xaml 的交互逻辑
    /// </summary>
    public partial class DateTimeBrowser : Window
    {
        private DateTimeBrowserViewModel _browserViewModel;
        public DateTimeBrowser(DateTime dateTime)
        {
            InitializeComponent();
            _browserViewModel= new DateTimeBrowserViewModel(dateTime);
            DataContext = _browserViewModel;
        }

        public DateTime? GetTime()
        {
            return _browserViewModel.GetTime();
        }
    }
}
