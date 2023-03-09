using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ICSStudio.DownloadChange
{
    internal class ProgramDiffItem : IProgramDiffItem
    {
        private readonly JsonDiffPatch.JsonDiffPatch _diffPath;

        public ProgramDiffItem(JToken oldValue, JToken newValue)
        {
            Debug.Assert(oldValue != null);
            Debug.Assert(newValue != null);

            _diffPath = new JsonDiffPatch.JsonDiffPatch();

            OldValue = InfoExtractor.TrimProgram(oldValue as JObject);
            NewValue = InfoExtractor.TrimProgram(newValue as JObject);
        }

        public ItemChangeType ChangeType { get; private set; }
        public JToken OldValue { get; }
        public JToken NewValue { get; }

        public IDiffItem Properties { get; private set; }

        public List<IDiffItem> Tags { get; private set; }

        public List<IDiffItem> Routines { get; private set; }

        public void BuildDiffModel()
        {
            JObject oldProperties = InfoExtractor.ExtractProgramProperties(OldValue as JObject);
            JObject newProperties = InfoExtractor.ExtractProgramProperties(NewValue as JObject);

            Properties = IsPropertiesEqual(oldProperties, newProperties)
                ? new DiffItem(ItemChangeType.Unchanged, oldProperties, newProperties)
                : new DiffItem(ItemChangeType.Modified, oldProperties, newProperties);

            //
            Tags = DiffTags(InfoExtractor.ExtractTags(OldValue as JObject),
                InfoExtractor.ExtractTags(NewValue as JObject));

            //
            Routines = DiffRoutines(InfoExtractor.ExtractRoutines(OldValue as JObject),
                InfoExtractor.ExtractRoutines(NewValue as JObject));

            // update change type
            ChangeType = ItemChangeType.Unchanged;

            if (ChangeType == ItemChangeType.Unchanged)
            {
                if (Properties.ChangeType != ItemChangeType.Unchanged)
                {
                    ChangeType = ItemChangeType.Modified;
                }
            }


            if (ChangeType == ItemChangeType.Unchanged)
            {
                foreach (var item in Tags)
                {
                    if (item.ChangeType != ItemChangeType.Unchanged)
                    {
                        ChangeType = ItemChangeType.Modified;
                        break;
                    }
                }
            }

            if (ChangeType == ItemChangeType.Unchanged)
            {
                foreach (var item in Routines)
                {
                    if (item.ChangeType != ItemChangeType.Unchanged)
                    {
                        ChangeType = ItemChangeType.Modified;
                        break;
                    }
                }
            }
        }

        private List<IDiffItem> DiffRoutines(JArray oldRoutines, JArray newRoutines)
        {
            Dictionary<string, CompareTuple> dictionary =
                new Dictionary<string, CompareTuple>();

            foreach (var oldTag in oldRoutines.OfType<JObject>())
            {
                if (oldTag.ContainsKey("Name"))
                {
                    string tagName = oldTag.GetValue("Name")?.ToString().ToLower();
                    if (!string.IsNullOrEmpty(tagName))
                        dictionary.Add(tagName, new CompareTuple { OldValue = oldTag });
                }
            }

            foreach (var newTag in newRoutines.OfType<JObject>())
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
                    changeType = IsRoutineEqual(compareTuple.OldValue, compareTuple.NewValue)
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

        private List<IDiffItem> DiffTags(JArray oldTags, JArray newTags)
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

        private bool IsPropertiesEqual(JToken oldValue, JToken newValue)
        {
            var result = _diffPath.Diff(oldValue, newValue);

            if (result == null)
                return true;

            //TODO(gjc): add code here

            return false;
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

            //TODO(gjc): add code here

            return false;
        }

        private bool IsRoutineEqual(JToken oldValue, JToken newValue)
        {
            var result = _diffPath.Diff(oldValue, newValue);

            if (result == null)
                return true;

            //TODO(gjc): add code here

            return false;
        }

        public void AddSummary(StringBuilder builder)
        {
            builder.AppendLine($"\t{OldValue["Name"]}");

            builder.AppendLine($"\t\tProperties: {Properties.ChangeType}");
            builder.AppendLine($"\t\tProgram Tags");

            foreach (var diffItem in Tags.OrderBy(x => x.ChangeType))
            {
                if (diffItem.ChangeType == ItemChangeType.Unchanged)
                    continue;

                builder.Append($"\t\t\t");
                if (diffItem.ChangeType == ItemChangeType.Deleted
                    || diffItem.ChangeType == ItemChangeType.Modified)
                {
                    builder.Append($"{diffItem.OldValue["Name"]}:\t{diffItem.ChangeType}");
                }

                if (diffItem.ChangeType == ItemChangeType.Added)
                {
                    builder.Append($"{diffItem.NewValue["Name"]}:\t{diffItem.ChangeType}");
                }

                builder.AppendLine();
            }

            builder.AppendLine($"\t\tProgram Routines");

            foreach (var diffItem in Routines.OrderBy(x => x.ChangeType))
            {
                if (diffItem.ChangeType == ItemChangeType.Unchanged)
                    continue;

                builder.Append($"\t\t\t");
                if (diffItem.ChangeType == ItemChangeType.Deleted
                    || diffItem.ChangeType == ItemChangeType.Modified)
                {
                    builder.Append($"{diffItem.OldValue["Name"]}:\t{diffItem.ChangeType}");
                }

                if (diffItem.ChangeType == ItemChangeType.Added)
                {
                    builder.Append($"{diffItem.NewValue["Name"]}:\t{diffItem.ChangeType}");
                }

                builder.AppendLine();
            }
        }
    }
}
