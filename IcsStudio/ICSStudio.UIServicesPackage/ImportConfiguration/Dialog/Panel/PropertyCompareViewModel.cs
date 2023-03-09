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
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Project;
using Newtonsoft.Json.Linq;
using ICSStudio.MultiLanguage;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Dialog.Panel
{
    public class PropertyCompareViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;

        public PropertyCompareViewModel(PropertyCompare panel, ProjectItemType type, JObject config, IBaseComponent exist)
        {
            Control = panel;
            panel.DataContext = this;
            if (type == ProjectItemType.UserDefined || type == ProjectItemType.Strings)
            {
                var existDataType = exist as UserDefinedDataType;
                var importDataType = (UserDefinedDataType)(Controller.GetInstance().DataTypes)[config["FinalName"]?.ToString()];
                var name = new PropertiesCompareItem();
                name.Title = "Name:";
                name.Value = config["FinalName"]?.ToString();
                name.ExistValue = existDataType.Name;
                Items.Add(name);

                var description = new PropertiesCompareItem();
                description.Title = "Description:";
                description.Value = config["Description"]?.ToString();
                description.ExistValue = existDataType.Description;
                Items.Add(description);

                var size = new PropertiesCompareItem();
                size.Title = "Data Type Size(bytes):";
                size.Value = importDataType.ByteSize.ToString();
                size.ExistValue = existDataType.ByteSize.ToString();
                Items.Add(size);

                var engineeringUnit=new PropertiesCompareItem();
                engineeringUnit.Title = "Engineering Unit:";
                engineeringUnit.Value = importDataType.EngineeringUnit;
                engineeringUnit.ExistValue = existDataType.EngineeringUnit;
                Items.Add(engineeringUnit);

                if ("1".Equals(config["Family"]?.ToString()) || existDataType.FamilyType == FamilyType.StringFamily)
                {
                    var isString = new PropertiesCompareItem(false);
                    isString.Title = "Data Type is a string:";
                    isString.Value = "1".Equals(config["Family"]?.ToString()) ? "Yes" : "No";
                    isString.Value = existDataType.FamilyType == FamilyType.StringFamily ? "Yes" : "No";
                    Items.Add(isString);
                }

                if ("1".Equals(config["Family"]?.ToString()) && existDataType.FamilyType == FamilyType.StringFamily)
                {
                    var max = new PropertiesCompareItem();
                    max.Title = "Maximum number of characters:";
                    max.Value = importDataType.TypeMembers["DATA"].DataTypeInfo.Dim1.ToString();
                    max.ExistValue = existDataType.TypeMembers["DATA"].DataTypeInfo.Dim1.ToString();
                    Items.Add(max);
                }
                else
                {
                    var structureIsDiff=new PropertiesCompareItem(false);
                    structureIsDiff.Title = "Structure is different:";
                    structureIsDiff.Value = CompareDataTypeStruct(importDataType, existDataType)?"Yes":"No";
                    Items.Add(structureIsDiff);

                    var memberCommentsDiff=new PropertiesCompareItem(false);
                    memberCommentsDiff.Title = "Member comments are different:";
                    memberCommentsDiff.Value = "<Unknown>";
                    Items.Add(memberCommentsDiff);

                    var memberExtendedPropertiesDiff=new PropertiesCompareItem(false);
                    memberExtendedPropertiesDiff.Title = "Member extended properties are different:";
                    memberExtendedPropertiesDiff.Value = "<Unknown>";
                    Items.Add(memberExtendedPropertiesDiff);

                    var memberStylesDiff=new PropertiesCompareItem(false);
                    memberStylesDiff.Title = "Member style are different:";
                    memberStylesDiff.Value = "<Unknown>";
                    Items.Add(memberStylesDiff);

                    var memberExternalAccessDiff=new PropertiesCompareItem(false);
                    memberExternalAccessDiff.Title = "Member external access are different:";
                    memberExternalAccessDiff.Value = "<Unknown>";
                    Items.Add(memberExternalAccessDiff);
                }
            }
            else if (type == ProjectItemType.AddOnInstructions||type==ProjectItemType.AddOnDefined)
            {
                var existAoi = exist as AoiDefinition;
                var existAoiConfig = existAoi.GetConfig();
                var importAoi =
                    ((AoiDefinitionCollection) Controller.GetInstance().AOIDefinitionCollection).Find(config["FinalName"]
                        ?.ToString(),true);
                var name=new PropertiesCompareItem();
                Items.Add(name);
                name.Title = "Name:";
                name.Value = config["Name"]?.ToString();
                name.ExistValue = existAoiConfig["Name"]?.ToString();

                var description=new PropertiesCompareItem();
                Items.Add(description);
                description.Title = "Description";
                description.Value = config["Description"]?.ToString();
                description.ExistValue = existAoiConfig["Description"]?.ToString();

                var revision=new PropertiesCompareItem();
                Items.Add(revision);
                revision.Title = "Revision:";
                revision.Value = config["Revision"]?.ToString();
                revision.ExistValue= existAoiConfig["Revision"]?.ToString();

                var vendor=new PropertiesCompareItem();
                Items.Add(vendor);
                vendor.Title = "Vendor:";
                vendor.Value = config["Vendor"]?.ToString();
                vendor.ExistValue = existAoiConfig["Vendor"]?.ToString();

                var createDate=new PropertiesCompareItem();
                Items.Add(createDate);
                createDate.Title = "Create Date:";
                createDate.Value = config["CreateDate"]?.ToString();
                createDate.ExistValue = existAoiConfig["CreateDate"]?.ToString();

                var createBy=new PropertiesCompareItem();
                Items.Add(createBy);
                createBy.Title = "Create By:";
                createBy.Value = config["CreateBy"]?.ToString();
                createBy.ExistValue = existAoiConfig["CreateBy"]?.ToString();

                var editorDate=new PropertiesCompareItem();
                Items.Add(editorDate);
                editorDate.Title = "Editor Date:";
                editorDate.Value = config["EditorDate"]?.ToString();
                editorDate.ExistValue = existAoiConfig["EditorDate"]?.ToString();

                var editorBy=new PropertiesCompareItem();
                Items.Add(editorBy);
                editorBy.Title = "Editor By:";
                editorBy.Value = config["EditorBy"]?.ToString();
                editorBy.ExistValue = existAoiConfig["EditorDate"]?.ToString();

                var executePrescan=new PropertiesCompareItem();
                Items.Add(executePrescan);
                executePrescan.Title = "Execute Prescan:";
                executePrescan.Value = importAoi.Routines["Prescan"]!=null?"Yes":"No";
                executePrescan.ExistValue = existAoi.Routines["Prescan"] != null ? "Yes" : "No";

                var executePostscan=new PropertiesCompareItem();
                Items.Add(executePostscan);
                executePostscan.Title = "Execute Postscan:";
                executePostscan.Value = importAoi.Routines["Postscan"] != null ? "Yes" : "No";
                executePostscan.ExistValue = existAoi.Routines["Postscan"] != null ? "Yes" : "No";

                var executeEnableInFalse=new PropertiesCompareItem();
                Items.Add(executeEnableInFalse);
                executeEnableInFalse.Title = "Execute Enable In False";
                executeEnableInFalse.Value = importAoi.Routines["EnableInFalse"] != null ? "Yes" : "No";
                executeEnableInFalse.ExistValue = existAoi.Routines["EnableInFalse"] != null ? "Yes" : "No";

                var softwareRevision=new PropertiesCompareItem();
                Items.Add(softwareRevision);
                softwareRevision.Title = "Software Revision";
                softwareRevision.Value = "TODO";
                softwareRevision.ExistValue = "TODO";

                var additionalHelpText=new PropertiesCompareItem();
                Items.Add(additionalHelpText);
                additionalHelpText.Title = "Additional Help Text:";
                additionalHelpText.Value = config["AdditionalHelpText"]?.ToString();
                additionalHelpText.ExistValue =existAoiConfig["AdditionalHelpText"]?.ToString();

                var logicDiff=new PropertiesCompareItem(false);
                Items.Add(logicDiff);
                logicDiff.Title = "Logic is different:";
                logicDiff.Value = RoutineIsDiff(importAoi.Routines["Logic"],importAoi.Routines["Logic"])?"Yes":"No";

                var numberOfParam=new PropertiesCompareItem();
                Items.Add(numberOfParam);
                numberOfParam.Title = "Number of Parameters:";
                numberOfParam.Value = (config["Parameters"] as JArray).Count.ToString();
                numberOfParam.ExistValue = (existAoiConfig["Parameters"] as JArray).Count.ToString();

                int newCount = 0, modifiedCount = 0, delCount = 0;
                int newCountLocal = 0, modifiedCountLocal = 0, delCountLocal = 0;
                var existParam=new List<ITag>();
                var existLocal=new List<ITag>();
                foreach (var existAoiTag in existAoi.Tags)
                {
                    if (existAoiTag.Usage == Usage.Local)
                    {
                        existLocal.Add(existAoiTag);
                    }
                    else
                    {
                        existParam.Add(existAoiTag);
                    }
                }
                foreach (var tag in importAoi.Tags)
                {
                    var existTag = existAoi.Tags[tag.Name];
                    if (tag.Usage == Usage.Local)
                    {
                        if (existTag == null)
                        {
                            newCountLocal++;
                        }
                        else
                        {
                            if (TagIsDiff(existTag, tag))
                                modifiedCountLocal++;
                            existLocal.Remove(existTag);
                        }
                    }
                    else
                    {
                        if (existTag == null)
                        {
                            newCount++;
                        }
                        else
                        {
                            if(TagIsDiff(existTag,tag))
                                modifiedCount++;
                            existParam.Remove(existTag);
                        }
                    }
                }

                delCount = existParam.Count;
                delCountLocal = existLocal.Count;
                var newParam=new PropertiesCompareItem(false);
                Items.Add(newParam);
                newParam.Title = "New Parameters:";
                newParam.Value = newCount.ToString();

                var modifiedParam=new PropertiesCompareItem(false);
                Items.Add(modifiedParam);
                modifiedParam.Title = "Modified Parameters:";
                modifiedParam.Value = modifiedCount.ToString();

                var delParam=new PropertiesCompareItem(false);
                Items.Add(delParam);
                delParam.Title = "Deleted Parameters:";
                delParam.Value = delCount.ToString();

                var numberOfLocalTags=new PropertiesCompareItem();
                Items.Add(numberOfLocalTags);
                numberOfLocalTags.Title = "Number of LocalTags:";
                numberOfLocalTags.Value = (config["LocalTags"] as JArray).Count.ToString();
                numberOfLocalTags.ExistValue = (existAoiConfig["LocalTags"] as JArray).Count.ToString();
                
                var newLocal=new PropertiesCompareItem(false);
                Items.Add(newLocal);
                newLocal.Title = "New LocalTags:";
                newLocal.Value = newCountLocal.ToString();

                var modifiedLocal=new PropertiesCompareItem(false);
                Items.Add(modifiedLocal);
                modifiedLocal.Title = "Modified LocalTags:";
                modifiedLocal.Value = modifiedCountLocal.ToString();

                var delLocal=new PropertiesCompareItem(false);
                Items.Add(delLocal);
                delLocal.Title = "Deleted LocalTags:";
                delLocal.Value = delCountLocal.ToString();
            }else if (type == ProjectItemType.Routine)
            {
                var existRoutine = exist as IRoutine;
                if (existRoutine == null && exist is Program)
                    existRoutine = (exist as Program)?.Routines[config["FinalName"]?.ToString()];

                var name =new PropertiesCompareItem();
                Items.Add(name);
                name.Title = "Name:";
                name.Value = config["FinalName"]?.ToString();
                
                name.ExistValue = existRoutine?.Name;

                var type1=new PropertiesCompareItem();
                Items.Add(type1);
                type1.Title = "Type:";
                type1.Value = ((RoutineType) (byte) config["Type"]).ToString();
                type1.ExistValue = existRoutine?.Type.ToString();

                var numberOfLines=new PropertiesCompareItem();
                Items.Add(numberOfLines);
                numberOfLines.Title = "Number of Lines:";
                numberOfLines.Value = (config["CodeText"] as JArray)?.Count.ToString();
                numberOfLines.ExistValue = (existRoutine as STRoutine)?.CodeText.Count.ToString()??"0";

                var logicDiff=new PropertiesCompareItem(false);
                Items.Add(logicDiff);
                logicDiff.Title = "Logic is different:";
                logicDiff.Value = "TODO";

                var hasPendingEdit=new PropertiesCompareItem();
                Items.Add(hasPendingEdit);
                hasPendingEdit.Title = "Existing routine has pending edits:";
                hasPendingEdit.Value = config["PendingCodeText"]!=null?"Yes":"No";
                hasPendingEdit.ExistValue = existRoutine.PendingEditsExist?"Yes":"No";
            }else if (type == ProjectItemType.Program)
            {
                var existProgram = exist as Program;

                var name = new PropertiesCompareItem();
                Items.Add(name);
                name.Title = "Name:";
                name.Value = config["FinalName"]?.ToString();
                name.ExistValue = existProgram.Name;

                var description = new PropertiesCompareItem();
                Items.Add(description);
                description.Title = "Description:";
                description.Value = config["Description"]?.ToString();
                description.ExistValue = existProgram.Description;

                var schedule = new PropertiesCompareItem();
                Items.Add(schedule);
                schedule.Title = "Schedule In:";
                schedule.Value = config["ScheduleIn"]?.ToString();
                schedule.ExistValue = existProgram.ParentTask?.Name;

                var main = new PropertiesCompareItem();
                Items.Add(main);
                main.Title = "Main Routine:";
                main.Value = config["MainRoutineName"]?.ToString();
                main.ExistValue = existProgram.MainRoutineName;

                var fault = new PropertiesCompareItem();
                Items.Add(fault);
                fault.Title = "Fault Routine:";
                fault.Value = config["FaultRoutineName"]?.ToString();
                fault.ExistValue = existProgram.FaultRoutineName;

                var inhibit = new PropertiesCompareItem();
                Items.Add(inhibit);
                inhibit.Title = "Inhibited:";
                inhibit.Value = ((bool)config["Inhibited"]) ? "Yes" : "No";
                inhibit.ExistValue = existProgram.Inhibited ? "Yes" : "No";

                var synchronize = new PropertiesCompareItem();
                Items.Add(synchronize);
                synchronize.Title = "Synchronize Redundancy Data After Execution:";
                synchronize.Value = "TODO";

                var testEdit = new PropertiesCompareItem();
                Items.Add(testEdit);
                testEdit.Title = "Test Edits:";
                testEdit.Value = "TODO";

                var useAsFolder = new PropertiesCompareItem();
                Items.Add(useAsFolder);
                useAsFolder.Title = "Use As Folder:";
                useAsFolder.Value = ((bool)config["UseAsFolder"])?"Yes":"No";
                useAsFolder.ExistValue = existProgram.ProgramProperties.UseAsFolder?"Yes":"No";

                int newCount = 0, modifiedCount = 0,count=0;
                int newCountLocal = 0, modifiedCountLocal = 0,countLocal=0;

                var existParam = new List<ITag>();
                var existLocal = new List<ITag>();
                foreach (var existAoiTag in existProgram.Tags)
                {
                    if (existAoiTag.Usage == Usage.Local)
                    {
                        existLocal.Add(existAoiTag);
                    }
                    else
                    {
                        existParam.Add(existAoiTag);
                    }
                }
                foreach (var importTag in config["Tags"])
                {
                    var existTag = existProgram.Tags[importTag["Name"]?.ToString()];
                    if ((byte)importTag["Usage"] == (byte)Usage.Local)
                    {
                        countLocal++;
                        if (existTag == null)
                        {
                            newCountLocal++;
                        }
                        else
                        {
                            if (TagIsDiff(existTag, existTag))
                                modifiedCountLocal++;
                            existLocal.Remove(existTag);
                        }
                    }
                    else
                    {
                        count++;
                        if (existTag == null)
                        {
                            newCount++;
                        }
                        else
                        {
                            if (TagIsDiff(existTag, existTag))
                                modifiedCount++;
                            existParam.Remove(existTag);
                        }
                    }
                }
                
                var numberOfTags=new PropertiesCompareItem();
                Items.Add(numberOfTags);
                numberOfTags.Title = "Number of Tags:";
                numberOfTags.Value = countLocal.ToString();
                numberOfTags.ExistValue = existLocal.Count.ToString();

                var newTags=new PropertiesCompareItem(false);
                Items.Add(newTags);
                newTags.Title = "New Tags:";
                newTags.Value = newCountLocal.ToString();

                var modifiedTags=new PropertiesCompareItem(false);
                Items.Add(modifiedTags);
                modifiedTags.Title = "Modified Tags:";
                modifiedTags.Value = modifiedCountLocal.ToString();

                var deletedTags=new PropertiesCompareItem(false);
                Items.Add(deletedTags);
                deletedTags.Title = "Deleted Tags:";
                deletedTags.Value = existLocal.Count.ToString();

                var numberOfParam=new PropertiesCompareItem();
                Items.Add(numberOfParam);
                numberOfParam.Title = "Number Of Parameters:";
                numberOfParam.Value = count.ToString();
                numberOfParam.Value = existParam.Count.ToString();

                var newParam=new PropertiesCompareItem();
                Items.Add(newParam);
                newParam.Title = "New Parameters:";
                newParam.Value = newCount.ToString();

                var modifiedParam=new PropertiesCompareItem();
                Items.Add(modifiedParam);
                modifiedParam.Title = "Modified Parameters:";
                modifiedParam.Value = modifiedCount.ToString();

                var deletedParam=new PropertiesCompareItem();
                Items.Add(deletedParam);
                deletedParam.Title = "Deleted Parameters:";
                deletedParam.Value = existParam.Count.ToString();

                var numberOfRoutine=new PropertiesCompareItem();
                Items.Add(numberOfRoutine);
                numberOfRoutine.Title = "Number of Routine:";
                numberOfRoutine.Value = (config["Routines"] as JArray)?.Count.ToString();
                numberOfRoutine.ExistValue = existProgram.Routines.Count.ToString();

                var existRoutines = existProgram.Routines.ToList();
                int newRoutineCount = 0, modifiedRoutineCount = 0;
                foreach (JObject routine in config["Routines"])
                {
                    var existRoutine = existProgram.Routines[routine["Name"]?.ToString()];
                    if (existRoutine == null)
                    {
                        newRoutineCount++;
                    }
                    else
                    {
                        existRoutines.Remove(existRoutine);
                        if (RoutineIsDiff(routine, existRoutine))
                        {
                            modifiedRoutineCount++;
                        }
                    }
                }

                var newRoutine=new PropertiesCompareItem();
                Items.Add(newRoutine);
                newRoutine.Title = "New Routines:";
                newRoutine.Value = newRoutineCount.ToString();

                var modifiedRoutine=new PropertiesCompareItem();
                Items.Add(modifiedRoutine);
                modifiedRoutine.Title = "Modified Routines:";
                modifiedRoutine.Value = modifiedRoutineCount.ToString();

                var deletedRoutine=new PropertiesCompareItem();
                Items.Add(deletedRoutine);
                deletedRoutine.Title = "Deleted Routines:";
                deletedRoutine.Value = existRoutines.Count.ToString();
            }else if (type == ProjectItemType.ModuleDefined || type == ProjectItemType.Ethernet)
            {
                var existDevice = exist as DeviceModule;
                Debug.Assert(existDevice!=null,"device");
                var name = new PropertiesCompareItem();
                name.Title = "Name:";
                name.Value = config["FinalName"]?.ToString();
                name.ExistValue = config["FinalName"]?.ToString();
                Items.Add(name);

                var catalog = new PropertiesCompareItem();
                Items.Add(catalog);
                catalog.Title = "Catalog Number:";
                catalog.Value = config["CatalogNumber"]?.ToString();
                catalog.ExistValue = existDevice.CatalogNumber;

                var vendor = new PropertiesCompareItem();
                Items.Add(vendor);
                vendor.Title = "Vendor:";
                vendor.Value = config["Vendor"]?.ToString();
                vendor.ExistValue = existDevice.Vendor.ToString();

                var productType = new PropertiesCompareItem();
                Items.Add(productType);
                productType.Title = "Product Type:";
                productType.Value = config["ProductType"]?.ToString();
                productType.ExistValue = existDevice.ProductType.ToString();

                var productCode = new PropertiesCompareItem();
                Items.Add(productCode);
                productCode.Title = "Product Code:";
                productCode.Value = config["ProductCode"]?.ToString();
                productCode.ExistValue = existDevice.ProductCode.ToString();

                var major = new PropertiesCompareItem();
                Items.Add(major);
                major.Title = "Major:";
                major.Value = config["Major"]?.ToString();
                major.ExistValue = existDevice.Major.ToString();

                var minor = new PropertiesCompareItem();
                Items.Add(minor);
                minor.Title = "Minor:";
                minor.Value = config["Minor"]?.ToString();
                minor.ExistValue = existDevice.Minor.ToString();

                var parentModule = new PropertiesCompareItem();
                Items.Add(parentModule);
                parentModule.Title = "Parent Module:";
                parentModule.Value = config["ParentModule"]?.ToString();
                parentModule.ExistValue = existDevice.ParentModuleName;

                var parentModPortId = new PropertiesCompareItem();
                Items.Add(parentModPortId);
                parentModPortId.Title = "Parent Mod Port Id:";
                parentModPortId.Value = config["ParentModPortId"]?.ToString();
                parentModPortId.ExistValue = existDevice.ParentModPortId.ToString();

                var inhibited = new PropertiesCompareItem();
                Items.Add(inhibited);
                inhibited.Title = "Inhibited:";
                inhibited.Value = ((bool)config["Inhibited"]) ? "Yes" : "No";
                inhibited.ExistValue = existDevice.Inhibited ? "Yes" : "No";

                var majorFault = new PropertiesCompareItem();
                Items.Add(majorFault);
                majorFault.Title = "Major Fault:";
                majorFault.Value = (bool)config["MajorFault"] ? "Yes" : "No";
                majorFault.ExistValue = existDevice.MajorFault ? "Yes" : "No";

                var description = new PropertiesCompareItem();
                Items.Add(description);
                description.Title = "Description:";
                description.Value = config["Description"]?.ToString();
                description.ExistValue = existDevice.Description;

                var extendedProperties = new PropertiesCompareItem();
                Items.Add(extendedProperties);
                extendedProperties.Title = "Extended Properties:";
                extendedProperties.Value = "TODO";
                extendedProperties.ExistValue = "TODO";
            }
            else if (type == ProjectItemType.ControllerTags || type == ProjectItemType.AddOnDefinedTags ||
                     type == ProjectItemType.ProgramTags)
            {

                Tag existTag = exist as Tag;
                Debug.Assert(existTag!=null,$"tag name:{config["Name"]}");
                var name = new PropertiesCompareItem();
                Items.Add(name);
                name.Title = "Name:";
                name.Value = config["FinalName"]?.ToString();
                name.ExistValue = existTag.Name;

                var dataType = new PropertiesCompareItem();
                Items.Add(dataType);
                dataType.Title = "Data Type:";
                dataType.Value = config["DataType"]?.ToString();
                dataType.ExistValue = existTag.DataTypeInfo.ToString();

                if (type !=  ProjectItemType.AddOnDefinedTags)
                {
                    var scope = new PropertiesCompareItem(false);
                    Items.Add(scope);
                    scope.Title = "Scope:";
                    scope.Value = "TODO";
                    scope.ExistValue = existTag.ParentCollection.ParentProgram == null
                        ? $"{Controller.GetInstance().Name}(controller)"
                        : $"{existTag.ParentCollection.ParentProgram.Name}(program)";
                }

                var externalAccess = new PropertiesCompareItem();
                Items.Add(externalAccess);
                externalAccess.Title = "External Access:";
                externalAccess.Value = ((ExternalAccess)(byte)config["ExternalAccess"]).ToString();
                externalAccess.ExistValue = existTag.ExternalAccess.ToString();

                var description = new PropertiesCompareItem();
                Items.Add(description);
                description.Title = "Description:";
                description.Value = config["Description"]?.ToString();
                description.ExistValue = exist.Description;

                var constant=new PropertiesCompareItem();
                Items.Add(constant);
                constant.Title = "Constant:";
                constant.Value = ((bool) config["Constant"]) ? "Yes" : "No";
                constant.ExistValue = existTag.IsConstant ? "Yes" : "No";

                var structureDiff=new PropertiesCompareItem(false);
                Items.Add(structureDiff);
                structureDiff.Title = "Structure is different:";
                structureDiff.Value = "TODO";

                var dataDiff=new PropertiesCompareItem(false);
                Items.Add(dataDiff);
                dataDiff.Title = "Data is different:";
                dataDiff.Value = "TODO";

                var commentsDiff=new PropertiesCompareItem(false);
                Items.Add(commentsDiff);
                commentsDiff.Title = "Comments are different:";
                commentsDiff.Value = "TODO";

                var extendedDiff=new PropertiesCompareItem(false);
                Items.Add(extendedDiff);
                extendedDiff.Title = "Extended properties are different:";
                extendedDiff.Value = "TODO";

                var alarmDiff=new PropertiesCompareItem(false);
                Items.Add(alarmDiff);
                alarmDiff.Title = "Alarm conditions are different:";
                alarmDiff.Value = "TODO";
            }
        }

        public List<PropertiesCompareItem> Items { get; } = new List<PropertiesCompareItem>();

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

        private bool TagIsDiff(JObject a, ITag b)
        {
            if (string.Compare(a["Description"]?.ToString(), b.Description) != 0) return true;
            if (string.CompareOrdinal(a["AliasFor"]?.ToString(), b.AliasBaseSpecifier) != 0) return true;
            if (string.CompareOrdinal(a["DataType"]?.ToString(), b.DataTypeInfo.ToString())!=0) return true;
            return false;
        }
        private bool TagIsDiff(ITag a, ITag b)
        {
            if (string.Compare(a.Description, b.Description) != 0) return true;
            if (string.CompareOrdinal(a.AliasSpecifier, b.AliasSpecifier) != 0) return true;
            if (string.CompareOrdinal(a.DataTypeInfo.ToString(), b.DataTypeInfo.ToString()) != 0) return true;
            return false;
        }

        private bool RoutineIsDiff(JObject a, IRoutine b)
        {
            var bRoutine = b as STRoutine;
            if (bRoutine == null) return true;
            if (a["CodeText"].Count() != bRoutine.CodeText.Count) return true;
            for (int i = 0; i < a["CodeText"].Count(); i++)
            {
                var aLine = a["CodeText"][i]?.ToString();
                var bLine = bRoutine.CodeText[i];
                if (string.CompareOrdinal(aLine,bLine)!=0) return true;
            }
            return false;
            //TODO(zyl):add other routine
        }

        private bool RoutineIsDiff(IRoutine a, IRoutine b)
        {
            var bRoutine = b as STRoutine;
            var aRoutine = a as STRoutine;
            if (aRoutine==null||bRoutine == null) return true;
            if (aRoutine.CodeText.Count() != bRoutine.CodeText.Count) return true;
            for (int i = 0; i < aRoutine.CodeText.Count; i++)
            {
                var aLine = aRoutine.CodeText[i];
                var bLine = bRoutine.CodeText[i];
                if (string.CompareOrdinal(aLine, bLine) != 0) return true;
            }
            return false;
            //TODO(zyl):add other routine
        }

        private bool CompareDataTypeStruct(UserDefinedDataType a, UserDefinedDataType b)
        {
            if (a.TypeMembers.Count != b.TypeMembers.Count) return false;
            foreach (var memberA in a.TypeMembers)
            {
                var memberB = b.TypeMembers[memberA.Name];
                if (memberB == null) return false;
                if (!memberA.DataTypeInfo.Equals(memberB.DataTypeInfo)) return false;
                if (memberA.DataTypeInfo.DataType is UserDefinedDataType && !CompareDataTypeStruct(
                        (UserDefinedDataType)memberA.DataTypeInfo.DataType,
                        (UserDefinedDataType)memberB.DataTypeInfo.DataType)) return false;
            }
            return true;
        }
    }

    public class PropertiesCompareItem
    {
        private readonly bool _showDiff;
        private string _title;

        public PropertiesCompareItem(bool showDiff = true)
        {
            _showDiff = showDiff;
        }

        public string Title
        {
            set { _title = value; }
            get
            {
                return LanguageManager.GetInstance().ConvertSpecifier(_title);
            }
        }

        public string Value { set; get; }

        public string ExistValue { set; get; }

        public bool IsDifferent => _showDiff && string.CompareOrdinal(Value??"",ExistValue??"")!=0 ;
    }
}
