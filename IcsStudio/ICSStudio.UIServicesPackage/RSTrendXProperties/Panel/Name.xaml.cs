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
using System.Windows.Threading;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.RSTrendXProperties.Panel
{
    /// <summary>
    /// Name.xaml 的交互逻辑
    /// </summary>
    public partial class Name : UserControl
    { 
        public Name()
        {
            InitializeComponent();
            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(),"LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void Name_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
#pragma warning disable VSTHRD001 // 避免旧线程切换 API
                Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)(() => ((NameViewModel)DataContext).Save()));
#pragma warning restore VSTHRD001 // 避免旧线程切换 API
            }
        }
    }
}
