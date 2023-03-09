using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSStudio.Interfaces.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.Model
{
    internal class ControllerInfo : BaseSimpleInfo
    {
        private readonly IController _controller;

        public ControllerInfo(IController controller, ObservableCollection<SimpleInfo> infoSource)
            : base(infoSource)
        {
            _controller = controller;

            CreateDefaultInfoItems();
        }

        private void CreateDefaultInfoItems()
        {
            if (_controller != null)
            {
                var localModule = _controller.DeviceModules["Local"];

                InfoSource.Add(new SimpleInfo { Name = "Motor Catalog", Value = localModule.CatalogNumber });
                InfoSource.Add(new SimpleInfo { Name = "Description", Value = _controller.Description });
                InfoSource.Add(new SimpleInfo { Name = "Slot", Value = "", Key = "ControllerSlot" });
                InfoSource.Add(new SimpleInfo { Name = "Major Fault", Value = "" });
                InfoSource.Add(new SimpleInfo { Name = "Minor Fault", Value = "" });

                PropertyChangedEventManager.AddHandler(_controller,
                    OnControllerPropertyChanged, string.Empty);
            }
        }

        private void OnControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.PropertyName == "Description")
                {
                    SetSimpleInfo("Description", _controller.Description);
                }
            });
        }
    }
}
