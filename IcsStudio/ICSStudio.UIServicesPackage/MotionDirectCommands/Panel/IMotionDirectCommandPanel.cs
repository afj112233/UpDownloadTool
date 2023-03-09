using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Gui.Dialogs;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands.Panel
{
    interface IMotionDirectCommandPanel : IOptionPanel
    {
        void Show();
        bool CanExecute();

        Cip.Objects.MotionDirectCommand MotionDirectCommand { get; }
        IMessageRouterRequest GetQueryCommandRequest();
        IMessageRouterRequest GetExecuteCommandRequest();
    }
}
