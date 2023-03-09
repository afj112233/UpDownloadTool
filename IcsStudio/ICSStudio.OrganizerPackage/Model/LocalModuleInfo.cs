using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSStudio.SimpleServices.DeviceModule;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.Model
{
    internal class LocalModuleInfo : BaseSimpleInfo
    {
        private readonly LocalModule _localModule;

        public LocalModuleInfo(LocalModule localModule, ObservableCollection<SimpleInfo> infoSource)
            : base(infoSource)
        {
            _localModule = localModule;

            if (_localModule != null)
            {
                CreateInfoItems();

                if (_localModule.ParentController != null)
                {
                    PropertyChangedEventManager.AddHandler(_localModule.ParentController, OnControllerPropertyChanged,
                        string.Empty);
                }

            }
        }

        private void OnControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.PropertyName == "Description")
                {
                    SetSimpleInfo("Description", _localModule.ParentController?.Description);
                }
            });
        }

        private void CreateInfoItems()
        {
            if (InfoSource != null)
            {
                InfoSource.Add(new SimpleInfo
                    { Name = "Description", Value = _localModule.ParentController?.Description });
                InfoSource.Add(new SimpleInfo { Name = "Major Fault", Value = "" });
                InfoSource.Add(new SimpleInfo { Name = "Minor Fault", Value = "" });
            }
        }
    }
}
