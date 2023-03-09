using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Editor;
using Clipboard = System.Windows.Clipboard;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;
using Type = ICSStudio.UIInterfaces.Editor.Type;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.Panel
{
    internal class AoiContextMenu
    {
        public static ContextMenu GetParameterContextMenu(BaseTagRow baseTagRow,ParametersViewModel vm, EventHandler<PropertyChangedEventArgs> eventHandler)
        {
            var contextMenu=new ContextMenu();
            var insertMenu=new Insert(baseTagRow,vm);
            contextMenu.Items.Add(insertMenu);
            var separator = new Separator();
            contextMenu.Items.Add(separator);
            var goToCross=new GoToCrossReference(baseTagRow);
            contextMenu.Items.Add(goToCross);
            var goTo=new GoTo();
            contextMenu.Items.Add(goTo);
            var separator2=new Separator();
            contextMenu.Items.Add(separator2);
            var cut=new Cut(baseTagRow,eventHandler);
            contextMenu.Items.Add(cut);
            var copy=new Copy(baseTagRow);
            contextMenu.Items.Add(copy);
            var paste=new Paste(baseTagRow,eventHandler);
            contextMenu.Items.Add(paste);
            var paste2=new PastePassThrough();
            contextMenu.Items.Add(paste2);
            var separator3=new Separator();
            contextMenu.Items.Add(separator3);
            var delete=new Delete(baseTagRow,eventHandler);
            contextMenu.Items.Add(delete);
            var separator4=new Separator();
            contextMenu.Items.Add(separator4);
            var expand=new Expand(baseTagRow);
            contextMenu.Items.Add(expand);
            var collapse=new Collapse(baseTagRow);
            contextMenu.Items.Add(collapse);
            var separator5=new Separator();
            contextMenu.Items.Add(separator5);
            var externalAccess=new SetExternalAccess(baseTagRow);
            contextMenu.Items.Add(externalAccess);
            return contextMenu;
        }
        
        public static ContextMenu GetEmptyContextMenu(BaseTagRow baseTagRow, EventHandler<PropertyChangedEventArgs> eventHandler)
        {
            var contextMenu = new ContextMenu();
            var goTo = new GoTo();
            contextMenu.Items.Add(goTo);
            var separator = new Separator();
            contextMenu.Items.Add(separator);
            var cut = new Cut(baseTagRow,eventHandler);
            contextMenu.Items.Add(cut);
            var copy = new Copy(baseTagRow);
            contextMenu.Items.Add(copy);
            var paste = new Paste(baseTagRow,eventHandler);
            contextMenu.Items.Add(paste);
            return contextMenu;
        }

        public static ContextMenu GetMultiContextMenu(List<BaseTagRow> baseTagRows, EventHandler<PropertyChangedEventArgs> eventHandler)
        {
            var contextMenu = new ContextMenu();
            var cut = new Cut(baseTagRows,eventHandler);
            contextMenu.Items.Add(cut);
            var copy = new Copy(baseTagRows);
            contextMenu.Items.Add(copy);
            var paste = new Paste(eventHandler);
            contextMenu.Items.Add(paste);
            var separator = new Separator();
            contextMenu.Items.Add(separator);
            var delete=new Delete(baseTagRows, eventHandler);
            contextMenu.Items.Add(delete);
            return contextMenu;
        }

        public static ContextMenu GetLocalContextMenu(BaseTagRow baseTagRow,EventHandler<PropertyChangedEventArgs> eventHandler)
        {
            var contextMenu=new ContextMenu();
            var newParam=new NewParameter(baseTagRow);
            contextMenu.Items.Add(newParam);
            var dataTypeInfo = Controller.GetInstance().DataTypes.ParseDataTypeInfo(baseTagRow.DataType);
            var editDataType=new EditDataType(dataTypeInfo.DataType?.ToString());
            contextMenu.Items.Add(editDataType);
            var separator = new Separator();
            contextMenu.Items.Add(separator);
            var goToCross = new GoToCrossReference(baseTagRow);
            contextMenu.Items.Add(goToCross);
            var goTo = new GoTo();
            contextMenu.Items.Add(goTo);
            var separator2 = new Separator();
            contextMenu.Items.Add(separator2);
            var cut = new Cut(baseTagRow, eventHandler);
            contextMenu.Items.Add(cut);
            var copy = new Copy(baseTagRow);
            contextMenu.Items.Add(copy);
            var paste = new Paste(baseTagRow, eventHandler);
            contextMenu.Items.Add(paste);
            var paste2 = new PastePassThrough();
            contextMenu.Items.Add(paste2);
            var separator3 = new Separator();
            contextMenu.Items.Add(separator3);
            var delete = new Delete(baseTagRow, eventHandler);
            contextMenu.Items.Add(delete);
            var separator4 = new Separator();
            contextMenu.Items.Add(separator4);
            var expand = new Expand(baseTagRow);
            contextMenu.Items.Add(expand);
            var collapse = new Collapse(baseTagRow);
            contextMenu.Items.Add(collapse);
            var separator5 = new Separator();
            contextMenu.Items.Add(separator5);
            var externalAccess = new SetExternalAccess(baseTagRow);
            contextMenu.Items.Add(externalAccess);
            return contextMenu;
        }

        public class Insert : MenuItem
        {
            private int _index;
            private ParametersViewModel _vm;
            public Insert(BaseTagRow row,ParametersViewModel vm)
            {
                Header = "Insert Parameter";
                var index = row.Parent.Child.IndexOf(row);
                _index = index;
                _vm = vm;
                IsEnabled = index > 1;
                if (row.ParentAddOnInstruction.IsSealed)
                    IsEnabled = false;
                Click += InsertMenuItem_Click;
            }

            private void InsertMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                _vm.BlankRowFactory(_index);
            }
        }

        public class GoToCrossReference : MenuItem
        {
            private BaseTagRow _baseTagRow;
            public GoToCrossReference(BaseTagRow baseTagRow)
            {
                _baseTagRow = baseTagRow;
                Header = $"Go to Cross Reference for '{baseTagRow.Name}'";
                Click += GoToCrossReference_Click;
            }

            private void GoToCrossReference_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                ICreateEditorService createDialogService =
                    Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SCreateEditorService)) as
                        ICreateEditorService;

                createDialogService?.CreateCrossReference(Type.Tag, _baseTagRow.Tag.ParentCollection.ParentProgram,
                    _baseTagRow.Name);
            }
        }

        public class GoTo : MenuItem
        {
            public GoTo()
            {
                Header = "Go To...";
                IsEnabled = false;
            }
        }

        public class Cut : MenuItem
        {
            private BaseTagRow _baseTagRow;
            private EventHandler<PropertyChangedEventArgs> _eventHandler;
            public Cut(BaseTagRow baseTagRow, EventHandler<PropertyChangedEventArgs> eventHandler)
            {
                _eventHandler = eventHandler;
                _baseTagRow = baseTagRow;
                Header = "Cut";
                IsEnabled = !string.IsNullOrEmpty(baseTagRow.Name) && baseTagRow.NameEnabled;
                if (baseTagRow.ParentAddOnInstruction.IsSealed||Controller.GetInstance().IsOnline)
                    IsEnabled = false;
                Click += Cut_Click;
            }

            private List<BaseTagRow> _baseTagRows;
            public Cut(List<BaseTagRow> baseTagRows, EventHandler<PropertyChangedEventArgs> eventHandler)
            {
                _eventHandler = eventHandler;
                _baseTagRows = baseTagRows;
                Header = "Cut";
                if (baseTagRows[0].ParentAddOnInstruction.IsSealed || Controller.GetInstance().IsOnline||baseTagRows.Any(b=>b.IsBlank()))
                    IsEnabled = false;
                Click += Cut_Click1;
            }

            private void Cut_Click1(object sender, RoutedEventArgs e)
            {
               ClipboardOperation.Copy(_baseTagRows,true,_eventHandler);
            }

            private void Cut_Click(object sender, System.Windows.RoutedEventArgs e)
            {
                ClipboardOperation.Copy(_baseTagRow,true,_eventHandler);
            }
        }

        internal class ClipboardOperation
        {
            public static void Copy(BaseTagRow baseTagRow,bool del, EventHandler<PropertyChangedEventArgs> eventHandler)
            {
                if (baseTagRow.Usage == Usage.Local)
                {
                    var data =
                        $"{baseTagRow.Name}\t{baseTagRow.DataType}\t{baseTagRow.Default}\t{baseTagRow.Style?.ToString()}\t{baseTagRow.ExternalAccess}";
                    Clipboard.SetData("aoi.local",data);
                    //Clipboard.SetText(data);
                }
                else
                {
                    var data =
                        $"{baseTagRow.Name}\t{baseTagRow.Usage.ToString()}\t{baseTagRow.DataType}\t{baseTagRow.Default}\t{baseTagRow.Style?.ToString()}\t{(baseTagRow.Req ? "1" : "0")}\t{(baseTagRow.Vis ? "1" : "0")}\t{baseTagRow.Description}\t{(baseTagRow.Constant ? "1" : "0")}";
                    Clipboard.SetData("aoi.parameter",data);
                    //Clipboard.SetText(data);
                }
                if(del)
                {
                    PropertyChangedEventManager.RemoveHandler(baseTagRow, baseTagRow.GetBaseTagRow().OnChildPropertyChanged, String.Empty);
                    PropertyChangedEventManager.RemoveHandler(baseTagRow, eventHandler, String.Empty);
                    baseTagRow.Parent.Child.RemoveTagItem(baseTagRow);
                }
            }

            public static void Copy(List<BaseTagRow> baseTagRows, bool del,EventHandler<PropertyChangedEventArgs> eventHandler)
            {
                var info = "";
                foreach (var baseTagRow in baseTagRows)
                {
                    if (baseTagRow.Usage == Usage.Local)
                        info +=
                            $"\r{baseTagRow.Name}\t{baseTagRow.DataType}\t{baseTagRow.Default}\t{baseTagRow.Style?.ToString()}\t{baseTagRow.ExternalAccess}";
                    else
                        info +=
                            $"\r{baseTagRow.Name}\t{baseTagRow.Usage.ToString()}\t{baseTagRow.DataType}\t{baseTagRow.Default}\t{baseTagRow.Style?.ToString()}\t{(baseTagRow.Req ? "1" : "0")}\t{(baseTagRow.Vis ? "1" : "0")}\t{baseTagRow.Description}\t{(byte)(baseTagRow.ExternalAccess ?? 0)}";
                    if (del)
                    {
                        PropertyChangedEventManager.RemoveHandler(baseTagRow, baseTagRow.GetBaseTagRow().OnChildPropertyChanged, String.Empty);
                        PropertyChangedEventManager.RemoveHandler(baseTagRow, eventHandler, String.Empty);
                        baseTagRow.Parent.Child.RemoveTagItem(baseTagRow);
                    }
                }

                if (info.Any())
                {
                    info = info.Substring(1);
                }
                Clipboard.SetData(baseTagRows[0].Usage == Usage.Local ? "aoi.locals" : "aoi.parameters", info);
                //Clipboard.SetText(info);
            }

            public static void Paste(BaseTagRow baseTagRow, EventHandler<PropertyChangedEventArgs> onPropertyChanged)
            {
                var parent = baseTagRow.Parent;
                var offset = parent.Child.IndexOf(baseTagRow);
                var data = Clipboard.GetData("aoi.local");
                if (data != null)
                {
                    ParseLocal(parent, offset,(string)data, onPropertyChanged);
                    return;
                }

                data = Clipboard.GetData("aoi.locals");
                if (data != null)
                {
                    var d = ((string) data).Split('\r');
                    foreach (var s in d)
                    {
                        ParseLocal(parent, offset, s, onPropertyChanged);
                    }
                    return;
                }
                data = Clipboard.GetData("aoi.parameter");
                if (data != null)
                {
                    ParseParameter(parent, offset, (string)data, onPropertyChanged);
                    return;
                }
                data = Clipboard.GetData("aoi.parameters");
                if (data != null)
                {
                    var d = ((string)data).Split('\r');
                    foreach (var s in d)
                    {
                        ParseParameter(parent, offset, s, onPropertyChanged);
                    }
                }
            }
            
            public static void Delete(BaseTagRow baseTagRow, EventHandler<PropertyChangedEventArgs> eventHandler)
            {
                PropertyChangedEventManager.RemoveHandler(baseTagRow, baseTagRow.GetBaseTagRow().OnChildPropertyChanged, String.Empty);
                PropertyChangedEventManager.RemoveHandler(baseTagRow, eventHandler, String.Empty);
                baseTagRow.Parent.ClearBaseChild(baseTagRow);
                baseTagRow.Parent.Child.RemoveTagItem(baseTagRow);
            }

            public static void Delete(IList baseTagRows, EventHandler<PropertyChangedEventArgs> eventHandler)
            {
                var delList=new List<BaseTagRow>();
                foreach (BaseTagRow baseTagRow in baseTagRows)
                {
                    delList.Add(baseTagRow);
                }
                foreach (var baseTagRow in delList)
                {
                    if (baseTagRow.IsBlank() || !baseTagRow.IsMember) continue;
                    Delete(baseTagRow,eventHandler);
                }
            }

            private static void ParseParameter(BaseTagRow parent,int offset,string data,EventHandler<PropertyChangedEventArgs> onPropertyChanged)
            {
                var properties = ((string)data).Split('\t');
                int index = 0;
                var name = properties[index++];
                Usage? usage = null;
                var p = properties[index++];
                if (!string.IsNullOrEmpty(p))
                    usage = (Usage)Enum.Parse(typeof(Usage), p);
                var dataType = properties[index++];
                var defaultV = properties[index++];
                DisplayStyle? style = null;
                p = properties[index++];
                if (!string.IsNullOrEmpty(p))
                    style = (DisplayStyle)Enum.Parse(typeof(DisplayStyle),
                        p);
                var req = properties[index++] == "1";
                var vis = properties[index++] == "1";
                var constant = properties.Last() == "1";
                var description = string.Join("\t", properties, index, properties.Length - index - 1);
                if (parent is ParametersRow)
                {
                    var parameter = new ParametersRow(parent.ParentAddOnInstruction, new Tag(parent.ParentAddOnInstruction.Tags as TagCollection), parent.IsOnline, parent);
                    parameter.Name = name;
                    parameter.Description = description;
                    parameter.Style = style;
                    parameter.IsShowMessage = false;
                    parameter.DataType = dataType;
                    parameter.Usage = usage;
                    parameter.Req = req;
                    parameter.Vis = vis;
                    parameter.Constant = constant;
                    parameter.Default = defaultV;
                    parameter.IsMember = true;
                    parameter.IsShowMessage = true;
                    PropertyChangedEventManager.AddHandler(parameter, parameter.GetBaseTagRow().OnChildPropertyChanged, String.Empty);
                    PropertyChangedEventManager.AddHandler(parameter, onPropertyChanged, String.Empty);
                    parent.Child.Insert(offset, parameter);
                }
                else
                {
                    var local = new LocalTagRow(parent.ParentAddOnInstruction, new Tag(parent.ParentAddOnInstruction.Tags as TagCollection), parent);
                    local.Name = name;
                    local.Usage = usage;
                    local.DataType = dataType;
                    local.Style = style;
                    local.Default = defaultV;
                    local.ExternalAccess = ExternalAccess.ReadWrite;
                    local.IsMember = true;
                    PropertyChangedEventManager.AddHandler(local, local.GetBaseTagRow().OnChildPropertyChanged, String.Empty);
                    PropertyChangedEventManager.AddHandler(local, onPropertyChanged, String.Empty);
                    parent.Child.Insert(offset, local);
                }
            }

            private static void ParseLocal(BaseTagRow parent, int offset, string data, EventHandler<PropertyChangedEventArgs> onPropertyChanged)
            {
                var properties = ((string)data).Split('\t');
                int index = 0;
                var name = properties[index++];

                var dataType = properties[index++];
                var defaultV = properties[index++];
                DisplayStyle? style = null;
                var p = properties[index++];
                if (!string.IsNullOrEmpty(p))
                    style = (DisplayStyle)Enum.Parse(typeof(DisplayStyle), p);
                var description = string.Join("\t", properties, index, properties.Length - index - 1);
                var externalAccess = (ExternalAccess)Enum.Parse(typeof(ExternalAccess), properties.Last());
                if (parent is LocalTagRow)
                {
                    var local = new LocalTagRow(parent.ParentAddOnInstruction, new Tag(parent.ParentAddOnInstruction.Tags as TagCollection), parent);
                    local.IsMember = true;
                    local.Name = name;
                    local.Usage = Usage.Local;
                    local.DataType = dataType;
                    local.Style = style;
                    local.Default = defaultV;
                    local.ExternalAccess = externalAccess;
                    PropertyChangedEventManager.AddHandler(local, local.GetBaseTagRow().OnChildPropertyChanged, String.Empty);
                    PropertyChangedEventManager.AddHandler(local, onPropertyChanged, String.Empty);
                    parent.Child.Insert(offset, local);
                }
                else
                {
                    var parameter = new ParametersRow(parent.ParentAddOnInstruction, new Tag(parent.ParentAddOnInstruction.Tags as TagCollection), parent.IsOnline, parent);
                    parameter.IsShowMessage = false;
                    parameter.Name = name;
                    parameter.IsMember = true;
                    parameter.Description = description;
                    parameter.Style = style;
                    parameter.DataType = dataType;
                    if (parameter.DataTypeL is CompositiveType)
                    {
                        parameter.Usage = Usage.InOut;
                    }
                    else
                    {
                        parameter.Usage = Usage.Input;
                    }
                    parameter.Default = defaultV;
                    parameter.IsShowMessage = true;
                    PropertyChangedEventManager.AddHandler(parameter, parameter.GetBaseTagRow().OnChildPropertyChanged, String.Empty);
                    PropertyChangedEventManager.AddHandler(parameter, onPropertyChanged, String.Empty);
                    parent.Child.Insert(offset, parameter);
                }
            }
        }
        
        public class Copy : MenuItem
        {
            private BaseTagRow _baseTagRow;
            public Copy(BaseTagRow baseTagRow)
            {
                Header = "Copy";
                if (string.IsNullOrEmpty(baseTagRow.Name))
                    IsEnabled = false;
                _baseTagRow = baseTagRow;
                Click += Copy_Click;
            }

            private List<BaseTagRow> _baseTagRows;
            public Copy(List<BaseTagRow> baseTagRows)
            {
                Header = "Copy";
                _baseTagRows = baseTagRows;
                Click += Copy_Click1;
            }

            private void Copy_Click1(object sender, RoutedEventArgs e)
            {
                ClipboardOperation.Copy(_baseTagRows, false,null);
            }

            private void Copy_Click(object sender, RoutedEventArgs e)
            {
               ClipboardOperation.Copy(_baseTagRow,false,null);
            }
        }

        public class Paste : MenuItem
        {
            private BaseTagRow _baseTagRow;
            private EventHandler<PropertyChangedEventArgs> _onPropertyChanged;
            public Paste(BaseTagRow baseTagRow, EventHandler<PropertyChangedEventArgs> onPropertyChanged)
            {
                _onPropertyChanged = onPropertyChanged;
                Header = "Paste";
                _baseTagRow = baseTagRow;
                IsEnabled = baseTagRow.IsBlank()&&CanPaste();
                if (baseTagRow.ParentAddOnInstruction.IsSealed || Controller.GetInstance().IsOnline)
                    IsEnabled = false;
                Click += Paste_Click;
            }

            public Paste(EventHandler<PropertyChangedEventArgs> onPropertyChanged)
            {
                _onPropertyChanged = onPropertyChanged;
                Header = "Paste";
                IsEnabled = false;
            }

            private void Paste_Click(object sender, RoutedEventArgs e)
            {
               ClipboardOperation.Paste(_baseTagRow, _onPropertyChanged);
            }

            public static bool CanPaste()
            {
                var data = Clipboard.GetData("aoi.local");
                if (data != null) return true;
                data = Clipboard.GetData("aoi.locals");
                if (data != null) return true;
                data = Clipboard.GetData("aoi.parameter");
                if (data != null) return true;
                data = Clipboard.GetData("aoi.parameters");
                if (data != null) return true;
                return false;
            }
        }

        public class PastePassThrough : MenuItem
        {
            public PastePassThrough()
            {
                Header = "Paste Pass-Through";
                IsEnabled = false;
            }
        }

        public class Delete : MenuItem
        {
            private BaseTagRow _baseTagRow;
            private EventHandler<PropertyChangedEventArgs> _eventHandler;
            public Delete(BaseTagRow baseTagRow, EventHandler<PropertyChangedEventArgs> onPropertyChanged)
            {
                _eventHandler = onPropertyChanged;
                _baseTagRow = baseTagRow;
                Header = "Delete";
                IsEnabled = baseTagRow.NameEnabled;
                if (baseTagRow.IsBlank()||baseTagRow.ParentAddOnInstruction.IsSealed||!(baseTagRow.IsMember)|| Controller.GetInstance().IsOnline)
                    IsEnabled = false;
                Click += Delete_Click;
            }

            private List<BaseTagRow> _baseTagRows;
            public Delete(List<BaseTagRow> baseTagRows, EventHandler<PropertyChangedEventArgs> onPropertyChanged)
            {
                _eventHandler = onPropertyChanged;
                _baseTagRows = baseTagRows;
                Header = "Delete";
                if (baseTagRows[0].ParentAddOnInstruction.IsSealed || Controller.GetInstance().IsOnline)
                    IsEnabled = false;
                Click += Delete_Click;
            }

            private void Delete_Click(object sender, RoutedEventArgs e)
            {
                if (_baseTagRows != null)
                {
                    ClipboardOperation.Delete(_baseTagRows, _eventHandler);
                }
                else
                {
                    ClipboardOperation.Delete(_baseTagRow, _eventHandler);
                }
            }
        }

        public class Expand : MenuItem
        {
            private BaseTagRow _baseTagRow;
            public Expand(BaseTagRow baseTagRow)
            {
                _baseTagRow = baseTagRow;
                Header = $"Expand All \"{baseTagRow.Name}\" Members";
                IsEnabled = _baseTagRow.ExpanderVis == Visibility.Visible;
                Click += Expand_Click;
            }

            private void Expand_Click(object sender, RoutedEventArgs e)
            {
                _baseTagRow.ExpanderCommand.Execute(null);
            }
        }

        public class Collapse : MenuItem
        {
            private BaseTagRow _baseTagRow;
            public Collapse(BaseTagRow baseTagRow)
            {
                _baseTagRow = baseTagRow;
                Header = $"Collapse All \"{_baseTagRow.Name}\" Members";
                IsEnabled = _baseTagRow.ExpanderCloseVis == Visibility.Visible;
                Click += Collapse_Click;
            }

            private void Collapse_Click(object sender, RoutedEventArgs e)
            {
                _baseTagRow.ExpanderCloseCommand.Execute(null);
            }
        }

        public class SetExternalAccess : MenuItem
        {
            public SetExternalAccess(BaseTagRow baseTagRow)
            {
                Header = $"Set External Access for \"{baseTagRow.Name}\"";
                var item1=new ReadWrite(baseTagRow);
                var item2=new Read(baseTagRow);
                var item3=new None(baseTagRow);
                item1.SetGroupItems(item2,item3);
                item2.SetGroupItems(item1,item3);
                item3.SetGroupItems(item1,item2);
                Items.Add(item1);
                Items.Add(item2);
                Items.Add(item3);
            }
        }

        public class ReadWrite : MenuItem
        {
            private BaseTagRow _baseTagRow;
            private MenuItem _b, _c;
            public ReadWrite(BaseTagRow baseTagRow)
            {
                _baseTagRow = baseTagRow;
                Header = "Read/Write";
                if (_baseTagRow.Usage == Usage.InOut)
                {
                    IsEnabled = false;
                }
                else
                {
                    if (_baseTagRow.NameEnabled)
                        IsEnabled = baseTagRow.ExternalAccess != ExternalAccess.ReadWrite;
                    else
                        IsEnabled = false;
                }
                Click += ReadWrite_Click;
            }

            public void SetGroupItems(MenuItem b, MenuItem c)
            {
                _b = b;
                _c = c;
            }

            private void ReadWrite_Click(object sender, RoutedEventArgs e)
            {
                _baseTagRow.ExternalAccess = ExternalAccess.ReadWrite;
                IsEnabled = false;
                _b.IsEnabled = true;
                _c.IsEnabled = true;
            }
        }

        public class Read : MenuItem
        {
            private BaseTagRow _baseTagRow;
            private MenuItem _b, _c;
            public Read(BaseTagRow baseTagRow)
            {
                Header = "Read Only";
                _baseTagRow = baseTagRow;
                if (_baseTagRow.Usage == Usage.InOut)
                {
                    IsEnabled = false;
                }
                else
                {
                    if (_baseTagRow.NameEnabled)
                        IsEnabled = baseTagRow.ExternalAccess != ExternalAccess.ReadOnly;
                    else
                        IsEnabled = false;
                }
                Click += Read_Click;
            }

            public void SetGroupItems(MenuItem b, MenuItem c)
            {
                _b = b;
                _c = c;
            }

            private void Read_Click(object sender, RoutedEventArgs e)
            {
                _baseTagRow.ExternalAccess = ExternalAccess.ReadOnly;
                IsEnabled = false;
                _b.IsEnabled = true;
                _c.IsEnabled = true;
            }
        }

        public class None : MenuItem
        {
            private BaseTagRow _baseTagRow;
            private MenuItem _b, _c;
            public None(BaseTagRow baseTagRow)
            {
                Header = "None";
                _baseTagRow = baseTagRow;
                if (_baseTagRow.Usage == Usage.InOut)
                    IsEnabled = false;
                else
                {
                    if (_baseTagRow.NameEnabled)
                        IsEnabled = baseTagRow.ExternalAccess != ExternalAccess.None;
                    else
                        IsEnabled = false;
                }
                Click += None_Click; ;
            }

            public void SetGroupItems(MenuItem b, MenuItem c)
            {
                _b = b;
                _c = c;
            }


            private void None_Click(object sender, RoutedEventArgs e)
            {
                _baseTagRow.ExternalAccess = ExternalAccess.None;
                IsEnabled = false;
                _b.IsEnabled = true;
                _c.IsEnabled = true;
            }
        }

        public class NewParameter : MenuItem
        {
            public NewParameter(BaseTagRow baseTagRow)
            {
                Header = $"New Parameter which alias \"{baseTagRow.Name}\"";
                IsEnabled = false;
            }
        }

        public class EditDataType:MenuItem
        {
            public EditDataType(string dataType)
            {
                Header = $"Edit \"{dataType}\" Data Type";
            }
        }
    }
}
