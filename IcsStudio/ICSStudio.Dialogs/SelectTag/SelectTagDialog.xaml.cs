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
using ICSStudio.Dialogs.Filter;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.Dialogs.SelectTag
{
    /// <summary>
    /// SelectTagDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectTagDialog
    {
        private SelectTagDialogVM _vm;
        public SelectTagDialog(FilterViewModel filterViewModel)
        {
            InitializeComponent();
            _vm = new SelectTagDialogVM(filterViewModel);
            _vm.FilterViewModel.SizeUpdateEvent += FilterViewModel_SizeUpdateEvent;
            DataContext = _vm;
        }

        public void Clean()
        {
            _vm.FilterViewModel.SizeUpdateEvent -= FilterViewModel_SizeUpdateEvent;
        }

        private void FilterViewModel_SizeUpdateEvent(object sender, EventArgs e)
        {
            var sizeChangedArg = (WidthChangedArg)e;

            Width += sizeChangedArg.ChangedSize;
        }

        public string Selection => FilterViewModel.Name;

        public FilterViewModel FilterViewModel => _vm.FilterViewModel;
    }
}
