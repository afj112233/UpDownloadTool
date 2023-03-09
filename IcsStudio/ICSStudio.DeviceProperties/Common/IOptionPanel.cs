using System.Windows;
using ICSStudio.Gui.Utils;

namespace ICSStudio.DeviceProperties.Common
{
    internal interface IOptionPanel : ICanBeDirty
    {
        object Control { get; }
        Visibility Visibility { get; }

        void LoadOptions();
        bool SaveOptions();

        void Show();
        void Hide();

        int CheckValid();
        void CheckDirty();
    }
}
