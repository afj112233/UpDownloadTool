using System.ComponentModel;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.MultiLanguage;
using ICSStudio.UIInterfaces.Project;

namespace ICSStudio.OrganizerPackage.ViewModel.Items
{
    public class AddOnInstructionItem : OrganizerItem
    {
        private readonly IAoiDefinition _definition;

        public AddOnInstructionItem(IAoiDefinition definition)
        {
            _definition = definition;

            Name = _definition.Name;
            Kind = ProjectItemType.AddOnInstruction;
            AssociatedObject = _definition;
            IsExpanded = false;

            // sub item
            var programTagsItem = new OrganizerItem
            {
                Name = "Parameters And Local Tag",
                Kind = ProjectItemType.ProgramTags,
                AssociatedObject = definition
            };
            this.ProjectItems.Add(programTagsItem);

            foreach (var routineItem in definition.Routines)
            {
                var routine = new OrganizerItem
                {
                    Name = routineItem.Name,
                    Kind = ProjectItemType.Routine,
                    AssociatedObject = routineItem
                };
                this.ProjectItems.Add(routine);
            }

            PropertyChangedEventManager.AddHandler(_definition, OnPropertyChanged, "");
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                Name = _definition.Name; 

                // sort
                SortByName();
            }
        }

        protected override void DisplayNameConvert()
        {
            DisplayName = Name.Equals("Parameters And Local Tag") ? LanguageManager.GetInstance().ConvertSpecifier(Name) : Name;
        }
    }
}
