using ICSStudio.Interfaces.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Parser;
using Microsoft.VisualStudio.Services.Settings.Telemetry;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.Model
{
    class FindLocation
    {
        public class Location
        {
            public string FindResult;
            public string Container;
            public string Routine;
        }
        public string FindResult;
        public string Routine;
        public string Container;
        public List<int> Line = new List<int>();
        public FindLocation()
        {

        }
        public Location Find(Tag tag)
        {
            var name = tag.Name;
            var controller = Controller.GetInstance();
            if (string.IsNullOrEmpty(name)) return new Location(){FindResult = FindResult,Container = Container,Routine = Routine};
            if (ObtainValue.HasVariableInDim(name)) return new Location() { FindResult = FindResult, Container = Container, Routine = Routine };

            var filter = new List<string>() { name };

            if (tag.ParentCollection.ParentProgram is AoiDefinition)
            {
                var aoi = tag.ParentCollection.ParentProgram;
                foreach (var routine in aoi.Routines)
                {
                    var st = routine as STRoutine;
                    if (st != null)
                    {
                        SearchAndAddLogicItem(st, tag, filter);
                    }
                }
                return new Location() { FindResult = FindResult, Container = Container, Routine = Routine };
            }

            var dotIndex = name.LastIndexOf(".", StringComparison.Ordinal);
            if (dotIndex > -1)
            {
                var parentMember = GetMemberParent(tag, name);
                filter.Add(parentMember);
                if (parentMember.EndsWith("]"))
                {
                    var index = parentMember.LastIndexOf("[", StringComparison.Ordinal);
                    if (index > -1)
                        filter.Add(parentMember.Substring(0, index));
                }

                var children = GetMemberChildren(tag, name);
                if (children != null)
                {
                    filter.AddRange(children);
                }

                if (tag.DataTypeInfo.DataType.IsAxisType)
                {
                    if (!filter.Contains(tag.Name))
                        filter.Add(tag.Name);
                }
            }
            else
            {
                if (name.EndsWith("]"))
                {
                    var index = name.LastIndexOf("[", StringComparison.Ordinal);
                    if (index > -1)
                        filter.Add(name.Substring(0, index));
                }
            }

            var count = filter.Count;
            if (!string.IsNullOrEmpty(tag.ParentCollection.ParentProgram?.Name))
                for (int i = 0; i < count; i++)
                {
                    filter.Add($"\\{tag.ParentCollection.ParentProgram?.Name}.{filter[i]}");
                }

            foreach (var program in controller.Programs)
            {
                foreach (var r in program.Routines)
                {
                    var st = r as STRoutine;
                    if (st != null)
                    {
                        SearchAndAddLogicItem(st, tag, filter);
                    }

                    RLLRoutine rll = r as RLLRoutine;
                    if (rll != null)
                    {
                        SearchAndAddLogicItem(rll, tag, filter);
                    }
                }
            }
            return new Location() { FindResult = FindResult, Container = Container, Routine = Routine };
        }

        private string GetMemberParent(ITag tag, string name)
        {
            return ObtainValue.GetParentName(tag.DataTypeInfo.DataType, name.Split('.').ToList(), 0, "");
        }

        private List<string> GetMemberChildren(ITag tag, string name)
        {
            return ObtainValue.GetChildrenName(tag.DataTypeInfo.DataType, name.Split('.').ToList(), 0, "");
        }
        private void SearchAndAddLogicItem(STRoutine st, Tag tag, List<string> filter)
        {
            {
                var variableInfos = st.GetCurrentVariableInfos(OnlineEditType.Original).ToList().Where(v =>
                    v.Tag == tag && IsMatch(v, filter)).ToList();

                foreach (var variableInfo in variableInfos)
                {
                    //var description = Tag.GetChildDescription(tag.Description, tag.DataTypeInfo, tag.ChildDescription,
                    //    Tag.GetOperand(variableInfo.Name));
                    var info = variableInfo.GetLocation();
                    if (Line.Contains(info.Item1)) continue;
                    Line.Add(info.Item1);
                    FindResult += "#" + (info.Item1) + " ";
                    Routine = st.Name;
                    Container = st.ParentCollection.ParentProgram.Name;
                }
            }

            if (st.PendingCodeText != null)
            {
                var variableInfos = st.GetCurrentVariableInfos(OnlineEditType.Pending).ToList().Where(v =>
                    v.Tag == tag && IsMatch(v, filter)).ToList();
                foreach (var variableInfo in variableInfos)
                {
                    //var description = Tag.GetChildDescription(tag.Description, tag.DataTypeInfo, tag.ChildDescription,
                    //    Tag.GetOperand(variableInfo.Name));
                    var info = variableInfo.GetLocation();
                    if (Line.Contains(info.Item1)) continue;
                    Line.Add(info.Item1);
                    FindResult += "#" + (info.Item1) + " ";
                    Routine = st.Name;
                    Container = st.ParentCollection.ParentProgram.Name;
                }
            }

            if (st.TestCodeText != null)
            {
                var variableInfos = st.GetCurrentVariableInfos(OnlineEditType.Test).ToList().Where(v =>
                    v.Tag == tag && IsMatch(v, filter)).ToList();
                foreach (var variableInfo in variableInfos)
                {
                    //var description = Tag.GetChildDescription(tag.Description, tag.DataTypeInfo, tag.ChildDescription,
                    //    Tag.GetOperand(variableInfo.Name));
                    var info = variableInfo.GetLocation();
                    if (Line.Contains(info.Item1)) continue;
                    Line.Add(info.Item1);
                    FindResult += "#" + (info.Item1) + " ";
                    Routine = st.Name;
                    Container = st.ParentCollection.ParentProgram.Name;
                }
            }
        }
        private void SearchAndAddLogicItem(RLLRoutine rll, Tag tag, List<string> filter)
        {
            var parserService =
                Package.GetGlobalService(typeof(SParserService)) as IParserService;

            var parseInformation = parserService?.GetCachedParseInformation(rll);

            if (parseInformation == null)
                return;

            foreach (var parameter in parseInformation.Parameters)
            {
                if (parameter.Tag == tag)
                {
                    if (Line.Contains(parameter.Row)) continue;
                    Line.Add(parameter.Row);
                    FindResult += "#" + (parameter.Row) + " ";
                    Routine = rll.ParentCollection.ParentProgram.Name;
                    Container = rll.Name;
                }
            }

        }

        private bool IsMatch(IVariableInfo variable, List<string> filterName)
        {
            if (string.IsNullOrEmpty(variable.Name)) return false;
            var crossReferenceRegex = variable.GetCrossReferenceRegex();
            var crossReferenceParentRegex = variable.GetCrossReferenceParentRegex();
            var regex = new Regex(crossReferenceRegex, RegexOptions.IgnoreCase);
            var regexParent = string.IsNullOrEmpty(crossReferenceParentRegex)
                ? null
                : new Regex($"^{crossReferenceParentRegex}$", RegexOptions.IgnoreCase);
            for (int i = 0; i < filterName.Count; i++)
            {
                var filter = filterName[i];
                if (i == 0)
                {
                    if (regex.IsMatch(filter) || IsMember(variable.Name, filter)) return true;
                    if (regexParent?.IsMatch(filter) ?? false) return true;
                }
                else
                {
                    if (regex.IsMatch(filter)) return true;
                }
            }

            return false;
        }

        private bool IsMember(string target, string item)
        {
            if (target.StartsWith(item, StringComparison.OrdinalIgnoreCase))
            {
                Regex regex = new Regex($@"{item.Replace(".", "\\.")}(?!\w)", RegexOptions.IgnoreCase);
                return regex.IsMatch(target);
            }

            return false;
        }

        public class Port
        {
            public string Name;
            public bool IsEnabled;
        }

        public List<Port> GetPorts(IController controller)
        {
            List<Port> Ports = new List<Port>();
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {

                var portConfiguration = new Cip.Objects.PortConfiguration(((Controller)controller).CipMessager);
                Ports.Clear();
                await portConfiguration.GetPortInfo();
                int index = 1;
                foreach (var port in portConfiguration.Ports)
                {
                    if (!(port.InterfaceState == CIPEthernetLinkObject.State.Disable ||
                          port.InterfaceState == CIPEthernetLinkObject.State.Enable)) continue;
                    var info = new Port
                    {
                        Name = "Port"+index,
                        IsEnabled = port.InterfaceState == CIPEthernetLinkObject.State.Enable
                    };
                    Ports.Add(info);
                    index++;
                }
            });
            return Ports;
        }
    }
}
