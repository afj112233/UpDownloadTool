using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using ICSStudio.EditorPackage.MonitorEditTags.Commands;
using ICSStudio.EditorPackage.MonitorEditTags.Models;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Notification;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.UI;
using ICSStudio.Utils;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json.Linq;

namespace ICSStudio.EditorPackage.MonitorEditTags
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    internal partial class EditTagsViewModel
    {
        public RelayCommand<IList> SelectionChangedCommand { get; }

        public RelayCommand<string> SortCommand { get; }
        public RelayCommand<string> SortIncludeTagMembersCommand { get; }

        public RelayCommand<TagItem> MonitorCommand { get; }
        public RelayCommand CopyCommand { get; }
        public RelayCommand CutCommand { get; }
        public RelayCommand PasteCommand { get; }
        public RelayCommand DeleteCommand { get; }

        private void OnSelectionChanged(IList selectedItems)
        {
            List<EditTagItem> list = new List<EditTagItem>();

            foreach (var item in selectedItems)
            {
                EditTagItem editTagItem = item as EditTagItem;
                list.Add(editTagItem);
            }

            SelectedEditTagItems = list;
        }

        #region Sort

        private void ExecuteSort(string columnHeader)
        {
            var selectedItem = SelectedEditTagItem;

            _descending = !_descending;

            Sort(_descending, _includeTagMembersInSorting);

            if (selectedItem != null)
                SelectedEditTagItem = selectedItem;
        }

        private void ExecuteSortIncludeTagMembers(string columnHeader)
        {
            var selectedItem = SelectedEditTagItem;

            _includeTagMembersInSorting = !_includeTagMembersInSorting;

            Sort(_descending, _includeTagMembersInSorting);

            RaisePropertyChanged("IncludeTagMembersInSorting");

            if (selectedItem != null)
                SelectedEditTagItem = selectedItem;
        }

        #endregion

        private bool CanExecuteDelete()
        {
            if (SelectedEditTagItems == null)
                return false;

            if (SelectedEditTagItems.Count == 0)
                return false;

            List<Tag> selectedTags = new List<Tag>();
            foreach (EditTagItem item in SelectedEditTagItems)
            {
                if (item?.Tag != null)
                {
                    if (!selectedTags.Contains(item.Tag))
                        selectedTags.Add(item.Tag);
                }
            }

            if (selectedTags.Count == 0)
                return false;

            foreach (var tag in selectedTags)
            {
                if (CanDeleteTag(tag))
                    return true;
            }

            return false;
        }

        private bool CanDeleteTag(Tag tag)
        {
            if (Controller.IsOnline)
            {
                //TODO(gjc): edit later
                return false;
                //

                //if (IsInAOI)
                //    return false;

                //var commandService =
                //    Package.GetGlobalService(typeof(SCommandService)) as ICommandService;

                //if (commandService == null)
                //    return false;

                //if (commandService.IsReference(tag))
                //    return false;
            }

            if (tag != null)
            {
                if (tag.DataTypeInfo.DataType.IsIOType)
                    return false;
                
                if (Scope is AoiDefinition)
                {
                    if (tag.Name.Equals("EnableIn", StringComparison.OrdinalIgnoreCase) ||
                        tag.Name.Equals("EnableOut", StringComparison.OrdinalIgnoreCase))
                        return false;
                }


                return true;
            }

            return true;
        }

        private void ExecuteDelete()
        {
            //1. get delete tag list
            List<Tag> tagList = new List<Tag>();
            List<bool> isElementList = new List<bool>();

            foreach (EditTagItem item in SelectedEditTagItems)
            {
                var tag = item?.Tag;
                if (tag != null && CanDeleteTag(tag))
                {
                    bool isElement = item.ParentItem != null;

                    if (tagList.Contains(tag))
                    {
                        if (!isElement)
                        {
                            int index = tagList.IndexOf(tag);

                            isElementList[index] = false;
                        }

                    }
                    else
                    {
                        tagList.Add(tag);
                        isElementList.Add(isElement);
                    }
                }
            }

            for (int i = 0; i < isElementList.Count; i++)
            {
                if (isElementList[i])
                {
                    string message = "Cannot delete elements of a tag!\n";
                    message += $"Delete Tag {tagList[i].Name}?";

                    if (MessageBox.Show(message, "ICS Studio", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                        MessageBoxResult.Yes)
                    {
                        isElementList[i] = false;
                    }
                }
            }
            //2. delete
            try
            {
                var studioUIService =
                    Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
                for (int i = 0; i < isElementList.Count; i++)
                {
                    if (!isElementList[i])
                    {
                        studioUIService?.DeleteTag(tagList[i]);
                    }
                }
                //3. update
                SelectedEditTagItem = null;

                //var aoiDefinition = Scope as AoiDefinition;
                //aoiDefinition?.Reset();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }




        }

        private void ExecuteCopy()
        {
            if (CanExecuteCopy())
            {
                Clipboard.Clear();

                List<Tag> tags = new List<Tag>();
                foreach (var item in SelectedEditTagItems)
                {
                    if (item != null && item.ParentItem == null)
                    {
                        tags.Add(item.Tag);
                    }
                }

                var data = CreateCopyDataObject(tags);

                try
                {
                    Clipboard.SetDataObject(data, true);
                }
                catch (ExternalException)
                {
                    // Apparently this exception sometimes happens randomly.
                    // The MS controls just ignore it, so we'll do the same.
                }

            }
        }

        private bool CanExecuteCopy()
        {
            if (SelectedEditTagItems == null)
                return false;

            if (SelectedEditTagItems.Count == 0)
                return false;

            foreach (var item in SelectedEditTagItems)
            {
                if (item != null && item.Tag != null && item.ParentItem == null)
                    return true;
            }

            return false;
        }

        private void ExecuteCut()
        {
            if (CanExecuteCut())
            {
                Clipboard.Clear();

                //1. get cut tag list
                List<Tag> tags = new List<Tag>();
                foreach (var item in SelectedEditTagItems)
                {
                    if (item != null && item.ParentItem == null && item.Tag != null)
                    {
                        if (CanDeleteTag(item.Tag))
                            tags.Add(item.Tag);
                    }
                }

                //2. copy
                var data = CreateCopyDataObject(tags);
                try
                {
                    Clipboard.SetDataObject(data, true);
                }
                catch (ExternalException)
                {
                    // Apparently this exception sometimes happens randomly.
                    // The MS controls just ignore it, so we'll do the same.
                }

                //3. delete
                try
                {
                    var studioUIService =
                        Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;

                    foreach (var tag in tags)
                    {
                        studioUIService?.DeleteTag(tag);
                    }

                }
                catch (Exception)
                {
                    // ignor
                }

            }
        }

        private bool CanExecuteCut()
        {
            if (Controller.IsOnline)
                return false;

            if (SelectedEditTagItems == null)
                return false;

            if (SelectedEditTagItems.Count == 0)
                return false;

            foreach (EditTagItem item in SelectedEditTagItems)
            {
                if (item != null)
                {
                    if (item.ParentItem == null && item.Tag != null)
                    {
                        if (CanDeleteTag(item.Tag))
                            return true;
                    }
                }
            }

            return false;
        }

        private void ExecutePaste()
        {
            if (CanExecutePaste())
            {
                //1.get clip data
                JArray tagArray = null;
                try
                {
                    var data = Clipboard.GetDataObject();
                    var jsonData = data?.GetData(MonitorEditTagsCommands.CopyTagsFormat) as string;

                    if (jsonData != null)
                        tagArray = JArray.Parse(jsonData);
                }
                catch (ExternalException)
                {
                    // ignor
                }

                //2. create tags
                if (tagArray != null)
                {
                    foreach (var tagObject in tagArray.OfType<JObject>())
                    {
                        if (CanCreateTag(tagObject))
                        {
                            CreateTag(tagObject);
                        }
                    }
                }

            }
        }

        private bool CanExecutePaste()
        {
            if (Controller.IsOnline)
                return false;

            if (SelectedEditTagItem == null)
            {
                try
                {
                    var data = Clipboard.GetDataObject();
                    if (data != null)
                    {
                        if (data.GetDataPresent(MonitorEditTagsCommands.CopyTagsFormat))
                            return true;
                    }

                }
                catch (ExternalException)
                {
                    // ignor
                }
            }

            return false;
        }

        private DataObject CreateCopyDataObject(List<Tag> tags)
        {
            DataObject data = new DataObject();

            JArray tagArray = new JArray();

            foreach (var tag in tags)
            {
                tagArray.Add(tag.ConvertToJObject());
            }

            data.SetData(MonitorEditTagsCommands.CopyTagsFormat, tagArray.ToString());

            return data;
        }

        private bool CanCreateTag(JObject config)
        {
            JObjectExtensions jsonTag = new JObjectExtensions(config);

            if (jsonTag["DataType"] != null)
            {
                var dataTypeName = (string)jsonTag["DataType"];
                dataTypeName = ((Controller)Controller).GetFinalName(typeof(IDataType), dataTypeName);
                var dataType = Controller.DataTypes[dataTypeName];

                if (dataType == null)
                    return false;

                if (dataType.IsIOType)
                    return false;

                return true;
            }

            return false;
        }

        private void CreateTag(JObject tagObject)
        {
            TagCollection tagCollection = Scope.Tags as TagCollection;
            if (tagCollection == null)
                return;

            try
            {
                Tag newTag = TagsFactory.CreateTag(tagCollection, tagObject);

                //1. get new name
                List<string> tagNames = tagCollection.Select(i => i.Name).ToList();
                newTag.Name = GetNewTagName(newTag.Name, tagNames);

                //TODO(gjc): add code here

                tagCollection.AddTag(newTag, tagCollection.ParentProgram is AoiDefinition, false);
                Notifications.Publish(new MessageData() { Object = newTag, Type = MessageData.MessageType.AddTag });
            }
            catch (Exception)
            {
                // ignore
            }

        }

        private string GetNewTagName(string name, List<string> tagNames)
        {
            int index = 0;
            string newName = name;

            while (true)
            {
                if (newName.Length > 40)
                {
                    if (index == 0)
                    {
                        newName = name.Remove(40);
                    }
                    else
                    {
                        var overflowLength = newName.Length - 40 + index.ToString().Length;
                        newName = name.Remove(newName.Length - overflowLength) + index;
                    }
                }

                if (!tagNames.Contains(newName, StringComparer.OrdinalIgnoreCase))
                    return newName;

                index++;

                newName = $"{name}{index}";
            }

        }
    }
}
