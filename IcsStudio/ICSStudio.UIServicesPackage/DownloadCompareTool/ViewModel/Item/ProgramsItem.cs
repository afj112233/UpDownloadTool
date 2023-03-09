using System.Collections.Generic;
using System.Linq;
using ICSStudio.DownloadChange;

namespace ICSStudio.UIServicesPackage.DownloadCompareTool.ViewModel.Item
{
    internal class ProgramsItem : CompareItem
    {
        private readonly List<IDiffItem> _programs;

        public ProgramsItem(List<IDiffItem> programs)
        {
            _programs = programs;

            Title = "Programs";

            BuildChildren();
        }

        private void BuildChildren()
        {
            Children = new List<CompareItem>();

            if (_programs != null && _programs.Count > 0)
            {
                foreach (var program in _programs.OrderBy(x => x.ChangeType))
                {
                    if (program.ChangeType == ItemChangeType.Unchanged)
                        continue;

                    if (program.ChangeType == ItemChangeType.Deleted)
                    {
                        string programName = program.OldValue["Name"]?.ToString();
                        var deletedProgram = new CompareItem() { Title = programName, DiffItem = program };
                        Children.Add(deletedProgram);
                    }
                    else if (program.ChangeType == ItemChangeType.Added)
                    {
                        string programName = program.NewValue["Name"]?.ToString();
                        var addedProgram = new CompareItem() { Title = programName, DiffItem = program };
                        Children.Add(addedProgram);
                    }
                    else if (program.ChangeType == ItemChangeType.Modified)
                    {
                        var modifiedProgram = new ModifiedProgramItem(program as IProgramDiffItem);
                        Children.Add(modifiedProgram);
                    }

                    ItemType = CompareItemType.Modified;
                }
            }
        }
    }
}
