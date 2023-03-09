using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ICSStudio.DownloadChange
{
    public partial class ProjectDiffBuilder
    {

        private IDiffItem DiffControllerProperties(JObject oldProperties, JObject newProperties)
        {
            var result = _diffPath.Diff(oldProperties, newProperties);

            ItemChangeType changeType = ItemChangeType.Unchanged;
            if (result != null)
                changeType = ItemChangeType.Modified;

            DiffItem diffItem = new DiffItem(changeType, oldProperties, newProperties);

            return diffItem;
        }

        private List<IDiffItem> DiffControllerTags(JArray oldTags, JArray newTags)
        {
            Dictionary<string, CompareTuple> dictionary =
                new Dictionary<string, CompareTuple>();

            foreach (var oldTag in oldTags.OfType<JObject>())
            {
                if (oldTag.ContainsKey("Name"))
                {
                    string tagName = oldTag.GetValue("Name")?.ToString().ToLower();
                    if (!string.IsNullOrEmpty(tagName))
                        dictionary.Add(tagName, new CompareTuple { OldValue = oldTag });
                }
            }

            foreach (var newTag in newTags.OfType<JObject>())
            {
                if (newTag.ContainsKey("Name"))
                {
                    string tagName = newTag.GetValue("Name")?.ToString().ToLower();
                    if (!string.IsNullOrEmpty(tagName))
                    {
                        if (dictionary.ContainsKey(tagName))
                        {
                            var compareTuple = dictionary[tagName];
                            compareTuple.NewValue = newTag;
                        }
                        else
                        {
                            dictionary.Add(tagName, new CompareTuple { NewValue = newTag });
                        }
                    }

                }
            }

            List<IDiffItem> diffItems = new List<IDiffItem>();

            foreach (KeyValuePair<string, CompareTuple> keyValuePair in dictionary)
            {
                CompareTuple compareTuple = keyValuePair.Value;

                ItemChangeType changeType;

                if (compareTuple.OldValue != null && compareTuple.NewValue != null)
                {
                    changeType = IsTagEqual(compareTuple.OldValue, compareTuple.NewValue)
                        ? ItemChangeType.Unchanged
                        : ItemChangeType.Modified;
                }
                else if (compareTuple.OldValue != null && compareTuple.NewValue == null)
                {
                    changeType = ItemChangeType.Deleted;
                }
                else if (compareTuple.OldValue == null && compareTuple.NewValue != null)
                {
                    changeType = ItemChangeType.Added;
                }
                else
                {
                    throw new NotImplementedException("Not run here!");
                }

                DiffItem diffItem = new DiffItem(changeType, compareTuple.OldValue, compareTuple.NewValue);
                diffItems.Add(diffItem);

            }

            return diffItems;
        }

        private bool IsTagEqual(JToken oldValue, JToken newValue)
        {
            var result = _diffPath.Diff(oldValue, newValue) as JObject;

            if (result == null)
                return true;

            if (result.ContainsKey("Name"))
            {
                JArray nameArray = result["Name"] as JArray;
                if (nameArray != null && nameArray.Count == 2)
                {
                    if (string.Equals(nameArray[0].ToString(),
                            nameArray[1].ToString(),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        result.Remove("Name");
                    }
                }
            }

            if (result.Count == 0)
                return true;

            //TODO(gjc):remove later
            string tagName = oldValue["Name"]?.ToString();
            Logger.Info($"{tagName} diff: {result}");
            //end

            //TODO(gjc): add code here

            return false;
        }

        private List<IDiffItem> DiffDataTypes(JArray oldDataTypes, JArray newDataTypes)
        {
            Dictionary<string, CompareTuple> dictionary =
                new Dictionary<string, CompareTuple>();

            foreach (var oldTag in oldDataTypes.OfType<JObject>())
            {
                if (oldTag.ContainsKey("Name"))
                {
                    string tagName = oldTag.GetValue("Name")?.ToString().ToLower();

                    if (!string.IsNullOrEmpty(tagName))
                        dictionary.Add(tagName, new CompareTuple { OldValue = oldTag });
                }
            }

            foreach (var newTag in newDataTypes.OfType<JObject>())
            {
                if (newTag.ContainsKey("Name"))
                {
                    string tagName = newTag.GetValue("Name")?.ToString().ToLower();

                    if (!string.IsNullOrEmpty(tagName))
                    {
                        if (dictionary.ContainsKey(tagName))
                        {
                            var compareTuple = dictionary[tagName];
                            compareTuple.NewValue = newTag;
                        }
                        else
                        {
                            dictionary.Add(tagName, new CompareTuple { NewValue = newTag });
                        }
                    }

                }
            }

            List<IDiffItem> diffItems = new List<IDiffItem>();

            foreach (KeyValuePair<string, CompareTuple> keyValuePair in dictionary)
            {
                CompareTuple compareTuple = keyValuePair.Value;

                ItemChangeType changeType;

                if (compareTuple.OldValue != null && compareTuple.NewValue != null)
                {
                    if (IsDataTypeEqual(compareTuple.OldValue, compareTuple.NewValue))
                    {
                        changeType = ItemChangeType.Unchanged;
                    }
                    else
                    {
                        changeType = ItemChangeType.Modified;
                    }
                }
                else if (compareTuple.OldValue != null && compareTuple.NewValue == null)
                {
                    changeType = ItemChangeType.Deleted;
                }
                else if (compareTuple.OldValue == null && compareTuple.NewValue != null)
                {
                    changeType = ItemChangeType.Added;
                }
                else
                {
                    throw new NotImplementedException("Not run here!");
                }

                DiffItem diffItem = new DiffItem(changeType, compareTuple.OldValue, compareTuple.NewValue);
                diffItems.Add(diffItem);

            }

            return diffItems;

        }

        private bool IsDataTypeEqual(JToken oldValue, JToken newValue)
        {
            var result = _diffPath.Diff(oldValue, newValue);

            if (result == null)
                return true;

            // log
            string dataType = oldValue["Name"]?.ToString();

            Logger.Info($"{dataType} diff: {result}");

            //TODO(gjc): add code here

            return false;
        }

        private List<IDiffItem> DiffAOIDefinitions(JArray oldAois, JArray newAois)
        {
            Dictionary<string, CompareTuple> dictionary =
                new Dictionary<string, CompareTuple>();

            foreach (var oldTag in oldAois.OfType<JObject>())
            {
                if (oldTag.ContainsKey("Name"))
                {
                    string tagName = oldTag.GetValue("Name")?.ToString().ToLower();

                    if (!string.IsNullOrEmpty(tagName))
                        dictionary.Add(tagName, new CompareTuple { OldValue = oldTag });
                }
            }

            foreach (var newTag in newAois.OfType<JObject>())
            {
                if (newTag.ContainsKey("Name"))
                {
                    string tagName = newTag.GetValue("Name")?.ToString().ToLower();

                    if (!string.IsNullOrEmpty(tagName))
                    {
                        if (dictionary.ContainsKey(tagName))
                        {
                            var compareTuple = dictionary[tagName];
                            compareTuple.NewValue = newTag;
                        }
                        else
                        {
                            dictionary.Add(tagName, new CompareTuple { NewValue = newTag });
                        }
                    }

                }
            }

            List<IDiffItem> diffItems = new List<IDiffItem>();

            foreach (KeyValuePair<string, CompareTuple> keyValuePair in dictionary)
            {
                CompareTuple compareTuple = keyValuePair.Value;

                ItemChangeType changeType;

                if (compareTuple.OldValue != null && compareTuple.NewValue != null)
                {
                    if (IsAOIDefinitionEqual(compareTuple.OldValue, compareTuple.NewValue))
                    {
                        changeType = ItemChangeType.Unchanged;
                    }
                    else
                    {
                        changeType = ItemChangeType.Modified;
                    }
                }
                else if (compareTuple.OldValue != null && compareTuple.NewValue == null)
                {
                    changeType = ItemChangeType.Deleted;
                }
                else if (compareTuple.OldValue == null && compareTuple.NewValue != null)
                {
                    changeType = ItemChangeType.Added;
                }
                else
                {
                    throw new NotImplementedException("Not run here!");
                }

                DiffItem diffItem = new DiffItem(changeType, compareTuple.OldValue, compareTuple.NewValue);
                diffItems.Add(diffItem);

            }

            return diffItems;
        }

        private bool IsAOIDefinitionEqual(JToken oldValue, JToken newValue)
        {
            var result = _diffPath.Diff(oldValue, newValue);

            if (result == null)
                return true;

            //TODO(gjc): add code here

            return false;
        }

        private List<IDiffItem> DiffTasks(JArray oldTasks, JArray newTasks)
        {
            Dictionary<string, CompareTuple> dictionary =
                new Dictionary<string, CompareTuple>();

            foreach (var oldTag in oldTasks.OfType<JObject>())
            {
                if (oldTag.ContainsKey("Name"))
                {
                    string tagName = oldTag.GetValue("Name")?.ToString().ToLower();

                    if (!string.IsNullOrEmpty(tagName))
                        dictionary.Add(tagName, new CompareTuple { OldValue = oldTag });
                }
            }

            foreach (var newTag in newTasks.OfType<JObject>())
            {
                if (newTag.ContainsKey("Name"))
                {
                    string tagName = newTag.GetValue("Name")?.ToString().ToLower();

                    if (!string.IsNullOrEmpty(tagName))
                    {
                        if (dictionary.ContainsKey(tagName))
                        {
                            var compareTuple = dictionary[tagName];
                            compareTuple.NewValue = newTag;
                        }
                        else
                        {
                            dictionary.Add(tagName, new CompareTuple { NewValue = newTag });
                        }
                    }

                }
            }

            List<IDiffItem> diffItems = new List<IDiffItem>();

            foreach (KeyValuePair<string, CompareTuple> keyValuePair in dictionary)
            {
                CompareTuple compareTuple = keyValuePair.Value;

                ItemChangeType changeType;

                if (compareTuple.OldValue != null && compareTuple.NewValue != null)
                {
                    if (IsTaskEqual(compareTuple.OldValue, compareTuple.NewValue))
                    {
                        changeType = ItemChangeType.Unchanged;
                    }
                    else
                    {
                        changeType = ItemChangeType.Modified;
                    }
                }
                else if (compareTuple.OldValue != null && compareTuple.NewValue == null)
                {
                    changeType = ItemChangeType.Deleted;
                }
                else if (compareTuple.OldValue == null && compareTuple.NewValue != null)
                {
                    changeType = ItemChangeType.Added;
                }
                else
                {
                    throw new NotImplementedException("Not run here!");
                }

                DiffItem diffItem = new DiffItem(changeType, compareTuple.OldValue, compareTuple.NewValue);
                diffItems.Add(diffItem);

            }

            return diffItems;
        }

        private bool IsTaskEqual(JToken oldValue, JToken newValue)
        {
            var result = _diffPath.Diff(oldValue, newValue);

            if (result == null)
                return true;


            //TODO(gjc): add code here

            return false;
        }

        private List<IDiffItem> DiffPrograms(JArray oldPrograms, JArray newPrograms)
        {
            Dictionary<string, CompareTuple> dictionary =
                new Dictionary<string, CompareTuple>();

            foreach (var oldTag in oldPrograms.OfType<JObject>())
            {
                if (oldTag.ContainsKey("Name"))
                {
                    string tagName = oldTag.GetValue("Name")?.ToString().ToLower();

                    if (!string.IsNullOrEmpty(tagName))
                        dictionary.Add(tagName, new CompareTuple { OldValue = oldTag });
                }
            }

            foreach (var newTag in newPrograms.OfType<JObject>())
            {
                if (newTag.ContainsKey("Name"))
                {
                    string tagName = newTag.GetValue("Name")?.ToString().ToLower();
                    if (!string.IsNullOrEmpty(tagName))
                    {
                        if (dictionary.ContainsKey(tagName))
                        {
                            var compareTuple = dictionary[tagName];
                            compareTuple.NewValue = newTag;
                        }
                        else
                        {
                            dictionary.Add(tagName, new CompareTuple { NewValue = newTag });
                        }
                    }

                }
            }

            List<IDiffItem> diffItems = new List<IDiffItem>();

            foreach (KeyValuePair<string, CompareTuple> keyValuePair in dictionary)
            {
                CompareTuple compareTuple = keyValuePair.Value;

                ItemChangeType changeType;

                if (compareTuple.OldValue != null && compareTuple.NewValue != null)
                {
                    ProgramDiffItem programDiffItem =
                        new ProgramDiffItem(compareTuple.OldValue, compareTuple.NewValue);

                    programDiffItem.BuildDiffModel();

                    diffItems.Add(programDiffItem);
                    continue;
                }

                if (compareTuple.OldValue != null && compareTuple.NewValue == null)
                {
                    changeType = ItemChangeType.Deleted;
                }
                else if (compareTuple.OldValue == null && compareTuple.NewValue != null)
                {
                    changeType = ItemChangeType.Added;
                }
                else
                {
                    throw new NotImplementedException("Not run here!");
                }

                DiffItem diffItem = new DiffItem(changeType, compareTuple.OldValue, compareTuple.NewValue);
                diffItems.Add(diffItem);

            }

            return diffItems;
        }

        private bool IsProgramEqual(JToken oldValue, JToken newValue)
        {
            var result = _diffPath.Diff(oldValue, newValue);

            if (result == null)
                return true;

            //TODO(gjc): add code here

            return false;
        }

        private List<IDiffItem> DiffModules(JArray oldModules, JArray newModules)
        {
            Dictionary<string, CompareTuple> dictionary =
                new Dictionary<string, CompareTuple>();

            foreach (var oldTag in oldModules.OfType<JObject>())
            {
                if (oldTag.ContainsKey("Name"))
                {
                    string tagName = oldTag.GetValue("Name")?.ToString().ToLower();

                    if (!string.IsNullOrEmpty(tagName))
                        dictionary.Add(tagName, new CompareTuple { OldValue = oldTag });
                }
            }

            foreach (var newTag in newModules.OfType<JObject>())
            {
                if (newTag.ContainsKey("Name"))
                {
                    string tagName = newTag.GetValue("Name")?.ToString().ToLower();

                    if (!string.IsNullOrEmpty(tagName))
                    {
                        if (dictionary.ContainsKey(tagName))
                        {
                            var compareTuple = dictionary[tagName];
                            compareTuple.NewValue = newTag;
                        }
                        else
                        {
                            dictionary.Add(tagName, new CompareTuple { NewValue = newTag });
                        }
                    }

                }
            }

            List<IDiffItem> diffItems = new List<IDiffItem>();

            foreach (KeyValuePair<string, CompareTuple> keyValuePair in dictionary)
            {
                CompareTuple compareTuple = keyValuePair.Value;

                ItemChangeType changeType;

                if (compareTuple.OldValue != null && compareTuple.NewValue != null)
                {
                    if (IsModuleEqual(compareTuple.OldValue, compareTuple.NewValue))
                    {
                        changeType = ItemChangeType.Unchanged;
                    }
                    else
                    {
                        changeType = ItemChangeType.Modified;
                    }
                }
                else if (compareTuple.OldValue != null && compareTuple.NewValue == null)
                {
                    changeType = ItemChangeType.Deleted;
                }
                else if (compareTuple.OldValue == null && compareTuple.NewValue != null)
                {
                    changeType = ItemChangeType.Added;
                }
                else
                {
                    throw new NotImplementedException("Not run here!");
                }

                DiffItem diffItem = new DiffItem(changeType, compareTuple.OldValue, compareTuple.NewValue);
                diffItems.Add(diffItem);

            }

            return diffItems;
        }

        private bool IsModuleEqual(JToken oldValue, JToken newValue)
        {
            var result = _diffPath.Diff(oldValue, newValue);

            if (result == null)
                return true;

            //TODO(gjc): add code here

            return false;
        }
    }
}
