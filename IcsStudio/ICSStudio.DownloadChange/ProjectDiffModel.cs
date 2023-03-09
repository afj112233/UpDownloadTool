using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSStudio.DownloadChange
{
    public class ProjectDiffModel
    {
        public IDiffItem ControllerProperties { get; set; }

        public List<IDiffItem> ControllerTags { get; set; }

        public List<IDiffItem> DataTypes { get; set; }

        public List<IDiffItem> AOIDefinitions { get; set; }

        public List<IDiffItem> Tasks { get; set; }

        public List<IDiffItem> Programs { get; set; }

        public List<IDiffItem> Modules { get; set; }

        public string Summary
        {
            get
            {
                StringBuilder builder = new StringBuilder();

                AddControllerSummary(builder);

                AddDataTypesSummary(builder);

                AddAOIDefinitionsSummary(builder);

                AddTasksSummary(builder);

                AddProgramsSummary(builder);

                AddModulesSummary(builder);

                return builder.ToString();
            }
        }

        private void AddModulesSummary(StringBuilder builder)
        {
            if (Modules != null && Modules.Count > 0)
            {
                builder.AppendLine("Modules");
                foreach (var diffItem in Modules.OrderBy(x => x.ChangeType))
                {
                    if (diffItem.ChangeType == ItemChangeType.Unchanged)
                        continue;

                    builder.Append("\t");

                    if (diffItem.ChangeType == ItemChangeType.Deleted
                        || diffItem.ChangeType == ItemChangeType.Modified)
                    {
                        builder.Append($"{diffItem.OldValue["CatalogNumber"]} {diffItem.OldValue["Name"]}: {diffItem.ChangeType}");
                    }

                    if (diffItem.ChangeType == ItemChangeType.Added)
                    {
                        builder.Append($"{diffItem.NewValue["CatalogNumber"]} {diffItem.NewValue["Name"]}: {diffItem.ChangeType}");
                    }

                    builder.AppendLine();
                }

                builder.AppendLine();
            }
        }

        private void AddProgramsSummary(StringBuilder builder)
        {
            if (Programs != null && Programs.Count > 0)
            {
                builder.AppendLine("Programs");

                foreach (var diffItem in Programs)
                {
                    if (diffItem.ChangeType == ItemChangeType.Unchanged)
                        continue;

                    if (diffItem.ChangeType == ItemChangeType.Deleted)
                    {
                        builder.Append($"\t{diffItem.OldValue["Name"]}: {diffItem.ChangeType}");
                    }

                    if (diffItem.ChangeType == ItemChangeType.Added)
                    {
                        builder.Append($"\t{diffItem.NewValue["Name"]}: {diffItem.ChangeType}");
                    }

                    if (diffItem.ChangeType == ItemChangeType.Modified)
                    {
                        ProgramDiffItem programDiffItem = diffItem as ProgramDiffItem;
                        if (programDiffItem != null)
                        {
                            programDiffItem.AddSummary(builder);
                        }
                    }

                    builder.AppendLine();
                }

                builder.AppendLine();
            }
        }

        private void AddTasksSummary(StringBuilder builder)
        {
            if (Tasks != null && Tasks.Count > 0)
            {
                builder.AppendLine("Tasks");

                foreach (var diffItem in Tasks)
                {
                    if (diffItem.ChangeType == ItemChangeType.Unchanged)
                        continue;

                    builder.Append("\t");

                    if (diffItem.ChangeType == ItemChangeType.Deleted
                        || diffItem.ChangeType == ItemChangeType.Modified)
                    {
                        builder.Append($"{diffItem.OldValue["Name"]}: {diffItem.ChangeType}");
                    }

                    if (diffItem.ChangeType == ItemChangeType.Added)
                    {
                        builder.Append($"{diffItem.NewValue["Name"]}: {diffItem.ChangeType}");
                    }

                    builder.AppendLine();
                }

                builder.AppendLine();
            }
        }

        private void AddAOIDefinitionsSummary(StringBuilder builder)
        {
            if (AOIDefinitions != null && AOIDefinitions.Count > 0)
            {
                builder.AppendLine("AOIDefinitions");

                foreach (var diffItem in AOIDefinitions)
                {
                    if (diffItem.ChangeType == ItemChangeType.Unchanged)
                        continue;

                    builder.Append("\t");

                    if (diffItem.ChangeType == ItemChangeType.Deleted
                        || diffItem.ChangeType == ItemChangeType.Modified)
                    {
                        builder.Append($"{diffItem.OldValue["Name"]}: {diffItem.ChangeType}");
                    }

                    if (diffItem.ChangeType == ItemChangeType.Added)
                    {
                        builder.Append($"{diffItem.NewValue["Name"]}: {diffItem.ChangeType}");
                    }

                    builder.AppendLine();
                }

                builder.AppendLine();
            }
        }

        private void AddDataTypesSummary(StringBuilder builder)
        {
            if (DataTypes != null && DataTypes.Count > 0)
            {
                builder.AppendLine("DataTypes");

                foreach (var diffItem in DataTypes)
                {
                    if (diffItem.ChangeType == ItemChangeType.Unchanged)
                        continue;

                    builder.Append("\t");

                    if (diffItem.ChangeType == ItemChangeType.Deleted
                        || diffItem.ChangeType == ItemChangeType.Modified)
                    {
                        builder.Append($"{diffItem.OldValue["Name"]}: {diffItem.ChangeType}");
                    }

                    if (diffItem.ChangeType == ItemChangeType.Added)
                    {
                        builder.Append($"{diffItem.NewValue["Name"]}: {diffItem.ChangeType}");
                    }

                    builder.AppendLine();
                }

                builder.AppendLine();
            }
        }

        private void AddControllerSummary(StringBuilder builder)
        {
            builder.AppendLine("Controller");

            builder.AppendLine($"\tProperties: {ControllerProperties.ChangeType}");
            builder.AppendLine($"\tController Tags");

            foreach (var diffItem in ControllerTags.OrderBy(x => x.ChangeType))
            {
                if (diffItem.ChangeType == ItemChangeType.Unchanged)
                    continue;

                builder.Append($"\t\t");

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

            builder.AppendLine();
        }
    }
}
