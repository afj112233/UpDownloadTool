using System.Collections.ObjectModel;
using System.ComponentModel;
using ICSStudio.Interfaces.DataType;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.Model
{
    internal class DataTypeInfo : BaseSimpleInfo
    {
        private readonly IDataType _dataType;

        public DataTypeInfo(IDataType dataType, ObservableCollection<SimpleInfo> infoSource)
            : base(infoSource)
        {
            _dataType = dataType;

            if (_dataType != null)
            {
                CreateInfoItems();

                PropertyChangedEventManager.AddHandler(_dataType,
                    OnDataTypePropertyChanged, string.Empty);

            }
        }

        private void OnDataTypePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.PropertyName == "Description")
                {
                    SetSimpleInfo("Description", _dataType.Description);
                }

                if (e.PropertyName == "ByteSize")
                {
                    SetSimpleInfo("Size", $"{_dataType.ByteSize} bytes");
                }
            });
        }

        private void CreateInfoItems()
        {
            if (InfoSource != null)
            {
                InfoSource.Add(new SimpleInfo { Name = "Description", Value = _dataType.Description });
                InfoSource.Add(
                    new SimpleInfo { Name = "Size", Value = $"{_dataType.ByteSize} bytes", Key = "DataSize" });
            }
        }
    }
}
