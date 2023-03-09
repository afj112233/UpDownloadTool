using System.Collections.Generic;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIServicesPackage.ImportConfiguration.Dialog.Panel;
using Newtonsoft.Json.Linq;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Dialog
{
    public class ImportDialogVM:ImportPropertiesDialogViewModel
    {
        private readonly List<IOptionPanelDescriptor> _optionPanelDescriptors;
        private readonly bool _canExecuteCommand ;
        public ImportDialogVM(ProjectItemType type,string operation,JObject config,IBaseComponent exist)
        {
            CurrentOperation = operation;
            _canExecuteCommand = type != ProjectItemType.ProgramTags;
            if (type == ProjectItemType.AddOnDefined)
            {
                Title = LanguageManager.GetInstance().ConvertSpecifier("Add-On Instructions");
            }
            else if (type == ProjectItemType.UserDefined)
            {
                Title = LanguageManager.GetInstance().ConvertSpecifier("Data Type");
            }
            else if (type == ProjectItemType.ControllerTags||type==ProjectItemType.AddOnDefinedTags||type==ProjectItemType.ProgramTags)
            {
                Title = LanguageManager.GetInstance().ConvertSpecifier("Tag");
            }
            else if (type == ProjectItemType.Program)
            {
                Title = LanguageManager.GetInstance().ConvertSpecifier("Program");
            }
            else if (type == ProjectItemType.Routine)
            {
                Title = LanguageManager.GetInstance().ConvertSpecifier("Routine");
            }
            else if (type == ProjectItemType.ModuleDefined)
            {
                Title = LanguageManager.GetInstance().ConvertSpecifier("Module");
            }

            Title += exist!=null? $"{LanguageManager.GetInstance().ConvertSpecifier("Name") + LanguageManager.GetInstance().ConvertSpecifier("Collision")} - {config["Name"]}"
                :$"{LanguageManager.GetInstance().ConvertSpecifier("Properties")} - {config["Name"]}";

            if (CurrentOperation.Equals("Create") || CurrentOperation.Equals("Discard"))
            {
                Operation1 = "Create";
                Operation2 = "Discard";
                if (type == ProjectItemType.UserDefined || type == ProjectItemType.Strings)
                {
                    Title = $"{LanguageManager.GetInstance().ConvertSpecifier("Data Type")+ LanguageManager.GetInstance().ConvertSpecifier("Properties")} - {config["Name"]}";
                }

                _optionPanelDescriptors = new List<IOptionPanelDescriptor>
                {
                    new DefaultOptionPanelDescriptor("1", "General", "General",
                        new PropertiesViewModel(new ICSStudio.UIServicesPackage.ImportConfiguration.Dialog.Panel.Properties(), type, config),null)
                };
                if (type == ProjectItemType.ControllerTags || type == ProjectItemType.AddOnDefinedTags ||
                    type == ProjectItemType.ProgramTags)
                {
                    _optionPanelDescriptors.Add(new DefaultOptionPanelDescriptor("2", "Data", "Data",
                        new DataViewModel(new Data(), config), null));
                }
                TabbedOptions.AddOptionPanels(_optionPanelDescriptors);
            }
            else
            {
                Operation1 = "Overwrite";
                Operation2 = "Use Existing";
                if (type == ProjectItemType.UserDefined || type == ProjectItemType.Strings)
                {
                    Title = $"{LanguageManager.GetInstance().ConvertSpecifier("Data Type")}{LanguageManager.GetInstance().ConvertSpecifier("Collision")} - {config["Name"]}";
                }

                if (type == ProjectItemType.ControllerTags || type == ProjectItemType.AddOnDefinedTags ||
                    type == ProjectItemType.ProgramTags)
                {
                    _optionPanelDescriptors = new List<IOptionPanelDescriptor>
                    {
                        new DefaultOptionPanelDescriptor("1", "Property Compare", "Property Compare",
                            new PropertyCompareViewModel(new PropertyCompare(), type, config,exist),null),
                        new DefaultOptionPanelDescriptor("2", "Data Compare", "Data Compare",
                            new DataCompareViewModel(new DataCompare(), config,exist as Tag), null),
                        //new DefaultOptionPanelDescriptor("3", "Project References", "Project References",
                        //    new ProjectReferencesViewModel(new ProjectReferences()),null)
                    };
                }
                else
                {
                    _optionPanelDescriptors = new List<IOptionPanelDescriptor>
                    {
                        new DefaultOptionPanelDescriptor("1", "Property Compare", "Property Compare",
                            new PropertyCompareViewModel(new PropertyCompare(), type, config,exist),null),
                        //new DefaultOptionPanelDescriptor("2", "Project References", "Project References",
                        //    new ProjectReferencesViewModel(new ProjectReferences()),null)
                    };
                }
                
                TabbedOptions.AddOptionPanels(_optionPanelDescriptors);
            }
        }

        protected override void ExecuteOperation1Command()
        {
            if (CurrentOperation.Equals("Create") || CurrentOperation.Equals("Discard"))
            {
                CurrentOperation = "Create";
            }
            else
            {
                CurrentOperation = "Overwrite";
            }
            CloseAction?.Invoke();
        }
        
        protected override bool CanExecuteOperation1Command()
        {
            return _canExecuteCommand;
        }


        protected override bool CanExecuteOperation2Command()
        {
            return _canExecuteCommand;
        }

        protected override void ExecuteOperation2Command()
        {
            if (CurrentOperation.Equals("Create") || CurrentOperation.Equals("Discard"))
            {
                CurrentOperation = "Discard";
            }
            else
            {
                CurrentOperation = "Use Existing";
            }
            CloseAction?.Invoke();
        }
    }
}
