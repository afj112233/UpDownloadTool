using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Gui.Dialogs;

namespace ICSStudio.UIServicesPackage.ManualTune.Panel
{
    interface IMotionGeneratorPanel : IOptionPanel
    {
        void Show();
        bool CanExecute();
        Cip.Objects.MotionGeneratorCommand MotionGeneratorCommand { get; }
        IMessageRouterRequest GetQueryCommandRequest();
        IMessageRouterRequest GetExecuteCommandRequest();
    }
}
