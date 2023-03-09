using System.Windows;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    internal interface IAxisCIPDrivePanel : IOptionPanel, ICanBeDirty
    {
        Visibility Visibility { get; }
        void Show();

        void CheckDirty();
        int CheckValid();
    }
}