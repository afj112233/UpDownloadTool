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
using ICSStudio.Interfaces.Common;

namespace ICSStudio.EditorPackage.Reference.Extend
{
    /// <summary>
    /// ScopeTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class ScopeTreeView : UserControl
    {
        private AdornerLayer _layer;
        private Adorner _adorner;
        private ScopeTreeViewVM _scopeTreeViewVm;
        public ScopeTreeView(AdornerLayer layer,Adorner adorner,SelectionTextType type,IController controller)
        {
            InitializeComponent();
            _layer = layer;
            _adorner = adorner;
            _scopeTreeViewVm=new ScopeTreeViewVM(type,controller);
            DataContext=_scopeTreeViewVm;
        }

        public void Update(SelectionTextType type)
        {
            _scopeTreeViewVm.Update(type);
        }
        
        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            var viewItem = (TreeViewItem) sender;
            var scopeItem = (ScopeItem)viewItem.DataContext;
            if (scopeItem.CanGetValue)
            {
                _layer.Remove(_adorner);

                (_adorner as ScopeAdorner)?.DoScopeChanged(scopeItem.Name,scopeItem.IsController);
                //close
            }
        }
    }
}
