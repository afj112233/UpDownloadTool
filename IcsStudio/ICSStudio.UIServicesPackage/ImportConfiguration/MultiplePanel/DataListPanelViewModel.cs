using GalaSoft.MvvmLight;
using ICSStudio.UIServicesPackage.ImportConfiguration.Panel.Validate;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.MultiplePanel
{
    public class DataListPanelViewModel : ViewModelBase, IVerify
    {
        public string Error { get; set; }
    }
}
