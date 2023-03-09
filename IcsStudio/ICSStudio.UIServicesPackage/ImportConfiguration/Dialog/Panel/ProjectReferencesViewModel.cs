using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Dialog.Panel
{
    public class ProjectReferencesViewModel : ViewModelBase, IOptionPanel
    {
        public ProjectReferencesViewModel(ProjectReferences panel)
        {
            Control = panel;
            panel.DataContext = this;
        }

        public object Owner { get; set; }
        public object Control { get; }
        public void LoadOptions()
        {
            
        }

        public bool SaveOptions()
        {
            return true;
        }
    }
}
