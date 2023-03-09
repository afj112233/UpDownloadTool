using GalaSoft.MvvmLight;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    internal class ParameterConnectionsViewModel : ViewModelBase
    {

        public ParameterConnectionsViewModel(ITag tag)
        {
            Title = "Connection Configutation - \\" + tag.ParentCollection.ParentProgram.Name + "." + tag.Name;
        }


        public string Title { get; }

        //public RelayCommand CancelCommand { get; }
    }
}
