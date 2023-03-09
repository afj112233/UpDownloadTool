using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.UIInterfaces.Project;
using Newtonsoft.Json.Linq;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Dialog.Panel
{
    public class PropertiesViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;

        public PropertiesViewModel(Properties panel, ProjectItemType type, JObject config)
        {
            Control = panel;
            panel.DataContext = this;
            if (type == ProjectItemType.UserDefined || type == ProjectItemType.String)
            {
                var importDataType = Controller.GetInstance().DataTypes[config["Name"]?.ToString()];
                Debug.Assert(importDataType != null);
                var name = new PropertiesItem();
                name.Title = "Name:";
                name.Value = importDataType.Name;
                Items.Add(name);

                var description = new PropertiesItem();
                description.Title = "Description:";
                description.Value = importDataType.Description;
                Items.Add(description);

                var size = new PropertiesItem();
                size.Title = "Data Type Size(bytes):";
                size.Value = importDataType.ByteSize.ToString();
                Items.Add(size);

                if (importDataType.FamilyType == FamilyType.StringFamily)
                {
                    var max = new PropertiesItem();
                    max.Title = "Maximum number of characters:";
                    max.Value = (importDataType as UserDefinedDataType)?.TypeMembers["DATA"].DataTypeInfo.Dim1
                        .ToString();
                    Items.Add(max);
                }
            }
            else if (type == ProjectItemType.AddOnInstructions || type == ProjectItemType.AddOnDefined)
            {
                var importAoi =
                    ((AoiDefinitionCollection) Controller.GetInstance().AOIDefinitionCollection).Find(config["Name"]
                        ?.ToString(), true);
                var name = new PropertiesItem();
                name.Title = "Name:";
                name.Value = config["FinalName"]?.ToString();
                Items.Add(name);

                var description = new PropertiesItem();
                Items.Add(description);
                description.Title = "Description:";
                description.Value = config["Description"]?.ToString();

                var revision = new PropertiesItem();
                Items.Add(revision);
                revision.Title = "Revision:";
                revision.Value = config["Revision"]?.ToString();

                var revisionNote = new PropertiesItem();
                Items.Add(revisionNote);
                revisionNote.Title = "Revision Note:";
                revisionNote.Value = config["RevisionNote"]?.ToString();

                var vendor = new PropertiesItem();
                Items.Add(vendor);
                vendor.Title = "Vendor:";
                vendor.Value = config["Vendor"]?.ToString();

                var createDate = new PropertiesItem();
                Items.Add(createDate);
                createDate.Title = "Created Date:";
                createDate.Value = config["CreatedDate"]?.ToString();

                var createBy = new PropertiesItem();
                Items.Add(createBy);
                createBy.Title = "Created By:";
                createBy.Value = config["CreatedBy"]?.ToString();

                var editedDate = new PropertiesItem();
                Items.Add(editedDate);
                editedDate.Title = "Edited Date:";
                editedDate.Value = config["EditedDate"]?.ToString();

                var editedBy = new PropertiesItem();
                Items.Add(editedBy);
                editedBy.Title = "Edited By:";
                editedBy.Value = config["EditedBy"]?.ToString();

                var executePrescan = new PropertiesItem();
                Items.Add(executePrescan);
                executePrescan.Title = "Execute Prescan:";
                executePrescan.Value = importAoi.Routines["Prescan"] != null ? "Yes" : "No";

                var executePostscan = new PropertiesItem();
                Items.Add(executePostscan);
                executePostscan.Title = "Execute Postscan:";
                executePostscan.Value = importAoi.Routines["Postscan"] != null ? "Yes" : "No";

                var executeInFalse = new PropertiesItem();
                Items.Add(executeInFalse);
                executeInFalse.Title = "Execute Enable In False:";
                executeInFalse.Value = importAoi.Routines["EnableInFalse"] != null ? "Yes" : "No";

                var softwareRevision = new PropertiesItem();
                Items.Add(softwareRevision);
                softwareRevision.Title = "Software Revision:";
                softwareRevision.Value = "TODO";

                var helpText = new PropertiesItem();
                Items.Add(helpText);
                helpText.Title = "Additional Help Text:";
                helpText.Value = config["AdditionalHelpText"]?.ToString();
            }
            else if (type == ProjectItemType.Program)
            {
                var name = new PropertiesItem();
                Items.Add(name);
                name.Title = "Name:";
                name.Value = config["FinalName"]?.ToString();

                var description = new PropertiesItem();
                Items.Add(description);
                description.Title = "Description:";
                description.Value = config["Description"]?.ToString();

                var schedule = new PropertiesItem();
                Items.Add(schedule);
                schedule.Title = "Schedule In:";
                schedule.Value = config["ScheduleIn"]?.ToString();

                var main = new PropertiesItem();
                Items.Add(main);
                main.Title = "Main Routine:";
                main.Value = config["MainRoutineName"]?.ToString();

                var fault = new PropertiesItem();
                Items.Add(fault);
                fault.Title = "Fault Routine:";
                fault.Value = config["FaultRoutineName"]?.ToString();

                var inhibit = new PropertiesItem();
                Items.Add(inhibit);
                inhibit.Title = "Inhibited:";
                //inhibit.Value = ((bool) config["Inhibit"]) ? "Yes" : "No";
                inhibit.Value = ((bool) config["Inhibited"]) ? "Yes" : "No";

                var synchronize = new PropertiesItem();
                Items.Add(synchronize);
                synchronize.Title = "Synchronize Redundancy Data After Execution:";
                synchronize.Value = "TODO";

                var testEdit = new PropertiesItem();
                Items.Add(testEdit);
                testEdit.Title = "Test Edits:";
                testEdit.Value = "TODO";

                var useAsFolder = new PropertiesItem();
                Items.Add(useAsFolder);
                useAsFolder.Title = "Use As Folder:";
                useAsFolder.Value = ((bool) config["UseAsFolder"]) ? "Yes" : "No";
            }
            else if (type == ProjectItemType.Routine)
            {
                var name = new PropertiesItem();
                Items.Add(name);
                name.Title = "Name:";
                name.Value = config["FinalName"]?.ToString();

                var type1 = new PropertiesItem();
                Items.Add(type1);
                type1.Title = "Type:";
                type1.Value = ((RoutineType) (byte) config["Type"]).ToString();

                var description = new PropertiesItem();
                Items.Add(description);
                description.Title = "Description:";
                description.Value = config["Description"]?.ToString();

                var numberOfLine = new PropertiesItem();
                Items.Add(numberOfLine);
                numberOfLine.Title = "Number of Lines:";
                numberOfLine.Value = config["CodeText"].Count().ToString();

            }
            else if (type == ProjectItemType.ModuleDefined || type == ProjectItemType.Ethernet)
            {
                var name = new PropertiesItem();
                name.Title = "Name:";
                name.Value = config["FinalName"]?.ToString();
                Items.Add(name);

                var catalog = new PropertiesItem();
                Items.Add(catalog);
                catalog.Title = "Catalog Number:";
                catalog.Value = config["CatalogNumber"]?.ToString();

                var vendor = new PropertiesItem();
                Items.Add(vendor);
                vendor.Title = "Vendor:";
                vendor.Value = config["Vendor"]?.ToString();

                var productType = new PropertiesItem();
                Items.Add(productType);
                productType.Title = "Product Type:";
                productType.Value = config["ProductType"]?.ToString();

                var productCode = new PropertiesItem();
                Items.Add(productCode);
                productCode.Title = "Product Code:";
                productCode.Value = config["ProductCode"]?.ToString();

                var major = new PropertiesItem();
                Items.Add(major);
                major.Title = "Major:";
                major.Value = config["Major"]?.ToString();

                var minor = new PropertiesItem();
                Items.Add(minor);
                minor.Title = "Minor:";
                minor.Value = config["Minor"]?.ToString();

                var parentModule = new PropertiesItem();
                Items.Add(parentModule);
                parentModule.Title = "Parent Module:";
                parentModule.Value = config["ParentModule"]?.ToString();

                var parentModPortId = new PropertiesItem();
                Items.Add(parentModPortId);
                parentModPortId.Title = "Parent Mod Port Id:";
                parentModPortId.Value = config["ParentModPortId"]?.ToString();

                var inhibited = new PropertiesItem();
                Items.Add(inhibited);
                inhibited.Title = "Inhibited:";
                inhibited.Value = ((bool) config["Inhibited"]) ? "Yes" : "No";

                var majorFault = new PropertiesItem();
                Items.Add(majorFault);
                majorFault.Title = "Major Fault:";
                majorFault.Value = (bool) config["MajorFault"] ? "Yes" : "No";

                var description = new PropertiesItem();
                Items.Add(description);
                description.Title = "Description:";
                description.Value = config["Description"]?.ToString();

                var extendedProperties = new PropertiesItem();
                Items.Add(extendedProperties);
                extendedProperties.Title = "Extended Properties:";
                extendedProperties.Value = "TODO";
            }
            else if (type == ProjectItemType.ControllerTags || type == ProjectItemType.AddOnDefinedTags ||
                     type == ProjectItemType.ProgramTags)
            {
                var name = new PropertiesItem();
                Items.Add(name);
                name.Title = "Name:";
                name.Value = config["FinalName"]?.ToString();

                var dataType = new PropertiesItem();
                Items.Add(dataType);
                dataType.Title = "Data Type:";
                dataType.Value = config["DataType"]?.ToString();

                var externalAccess = new PropertiesItem();
                Items.Add(externalAccess);
                externalAccess.Title = "External Access:";
                externalAccess.Value = ((ExternalAccess) (byte) config["ExternalAccess"]).ToString();

                var description = new PropertiesItem();
                Items.Add(description);
                description.Title = "Description:";
                description.Value = config["Description"]?.ToString();

                if (type == ProjectItemType.ProgramTags)
                {
                    var scope = new PropertiesItem();
                    Items.Add(scope);
                    scope.Title = "Scope:";
                    scope.Value = $"{((JObject) config.Parent.Parent.Parent)["FinalName"]}(program)";

                    var usage = new PropertiesItem();
                    Items.Add(usage);
                    usage.Title = "Usage:";
                    usage.Value = ((Usage) (byte) config["Usage"]).ToString();

                    var constant = new PropertiesItem();
                    Items.Add(constant);
                    constant.Title = "Constant:";
                    constant.Value = (bool) config["Constant"] ? "Yes" : "No";
                }
            }
        }

        public List<PropertiesItem> Items { get; } = new List<PropertiesItem>();

        public string Title { set; get; }
        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {

        }

        public bool SaveOptions()
        {
            return true;
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                Set(ref _isDirty, value);
                IsDirtyChanged?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler IsDirtyChanged;
    }

    public class PropertiesItem
    {
        private string _title;

        public string Title
        {
            set { _title = value; }
            get{ return LanguageManager.GetInstance().ConvertSpecifier(_title); }
        }

        public string Value { set; get; }
    }
}
