using System.ComponentModel;
using System.Diagnostics.Contracts;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Project;

namespace ICSStudio.OrganizerPackage.ViewModel.Items
{
    internal class AddOnDefinedItem : OrganizerItem
    {
        private readonly AoiDefinition _aoiDefinition;

        public AddOnDefinedItem(IAoiDefinition definition)
        {
            _aoiDefinition = definition as AoiDefinition;
            Contract.Assert(_aoiDefinition != null);

            Name = _aoiDefinition.datatype.Name;
            Kind = ProjectItemType.AddOnDefined;
            AssociatedObject = definition;
            PropertyChangedEventManager.AddHandler(_aoiDefinition.datatype, OnPropertyChanged, "");

        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                Name = _aoiDefinition.datatype.Name;

                // sort
                SortByName();
            }
        }
        protected override void DisplayNameConvert()
        {
            DisplayName = Name;
        }
    }
}
