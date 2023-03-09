using System.ComponentModel;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.UIInterfaces.Project;

namespace ICSStudio.OrganizerPackage.ViewModel.Items
{
    internal class UserDefinedItem : OrganizerItem
    {
        private readonly UserDefinedDataType _dataType;

        public UserDefinedItem(UserDefinedDataType dataType)
        {
            _dataType = dataType;
            Name = _dataType.Name;
            Kind = ProjectItemType.UserDefined;
            AssociatedObject = _dataType;

            PropertyChangedEventManager.AddHandler(_dataType, OnPropertyChanged, "");
        }

        public override void Cleanup()
        {
            PropertyChangedEventManager.RemoveHandler(_dataType, OnPropertyChanged, "");
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                Name = _dataType.Name;
                SortByName();
            }
        }

        protected override void DisplayNameConvert()
        {
            DisplayName = Name;
        }
    }

    internal class StringTypeItem : UserDefinedItem
    {
        public StringTypeItem(UserDefinedDataType dataType) : base(dataType)
        {
            Kind = ProjectItemType.String;
        }
    }

    


}
