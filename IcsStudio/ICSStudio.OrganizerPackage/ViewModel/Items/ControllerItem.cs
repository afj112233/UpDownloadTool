using System.ComponentModel;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.UIInterfaces.Project;

namespace ICSStudio.OrganizerPackage.ViewModel.Items
{
    internal class ControllerItem : OrganizerItem
    {
        private readonly IController _controller;

        public ControllerItem(IController controller)
        {
            _controller = controller;
            Name = "Controller";
            Kind = ProjectItemType.ControllerModel;
            AssociatedObject = _controller;

            PropertyChangedEventManager.AddHandler(_controller, OnPropertyChanged, "");
        }

        protected override void DisplayNameConvert()
        {
            DisplayName = LanguageManager.GetInstance().ConvertSpecifier(Name) + " " + _controller.Name;
        }


        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                DisplayNameConvert();
            }
        }
    }
}
