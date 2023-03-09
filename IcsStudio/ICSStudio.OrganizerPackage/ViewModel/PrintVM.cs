using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using ICSStudio.Descriptor;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using ICSStudio.Interfaces.DataType;
using ICSStudio.EditorPackage.DataTypes;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.OrganizerPackage.Model;
using ICSStudio.OrganizerPackage.ViewModel.Items;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.OrganizerPackage.ViewModel
{
    /// <summary>
    /// 主线程调度管理
    /// </summary>
    public static class DispatcherHelper
    {
        public static Dispatcher Dispatcher => Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
    }

    class PrintVM : ViewModelBase
    {
        private readonly OleMenuCommand _oleMenuCommand;
        private readonly IProjectItem _projectItem;
        private delegate void DoPrintMethod(PrintDialog pePrintDialog, DocumentPaginator document);

        public PrintVM(OleMenuCommand menuCommand, IProjectItem selectedProjectItem)
        {
            _oleMenuCommand = menuCommand;
            _projectItem = selectedProjectItem;
            Print();
        }

        public void Print()
        {
            var projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
            var project = projectInfoService?.CurrentProject;
            var routine = _projectItem.AssociatedObject as IRoutine;
            var controller = project?.Controller;
            if(controller == null)
                return;

            try
            {
                CTask task;
                IProgram program;
                IDataType dataType;
                IAoiDefinition aoi;
                switch (_oleMenuCommand.CommandID.ID)
                {
                    case PackageIds.printTaskProperties:
                        task = _projectItem.AssociatedObject as CTask;
                        PrintTask(task, controller);
                        break;
                    case PackageIds.printProgramProperties:
                        program = _projectItem.AssociatedObject as IProgram;
                        PrintProgram(program, controller);
                        break;
                    case PackageIds.printRoutine:
                        PrintRoutine(routine, controller);
                        break;
                    case PackageIds.printRoutineProperties:
                        PrintRoutineProperty(routine, controller);
                        break;
                    case PackageIds.printDataType:
                        dataType = _projectItem.AssociatedObject as IDataType;
                        PrintDataType(dataType, _projectItem);
                        break;
                    case PackageIds.printAdd_OnInstruction:
                        aoi = _projectItem.AssociatedObject as IAoiDefinition;
                        PrintAOI(aoi,_projectItem);
                        break;
                    case PackageIds.printModuleProperties:
                        PrintModuleProperties( controller,_projectItem);
                        break;
                    case PackageIds.printControllerOrganizer:
                        PrintControllerOrganizer(controller);
                        break;
                    case PackageIds.printControllerProperties:
                        PrintControllerProperties(controller);
                        break;
                    case PackageIds.printTags:
                        PrintTags(controller,_projectItem);
                        break;
                    case PackageIds.printTaskProperties2:
                        task = _projectItem.AssociatedObject as CTask;
                        PrintTask(task, controller);
                        break;
                    case PackageIds.printProgramProperties2:
                        program = _projectItem.AssociatedObject as IProgram;
                        PrintProgram(program, controller);
                        break;
                    case PackageIds.printRoutine2:
                        PrintRoutine(routine, controller);
                        break;
                    case PackageIds.printRoutineProperties2:
                        PrintRoutineProperty(routine, controller);
                        break;
                    case PackageIds.printDataType2:
                        dataType = _projectItem.AssociatedObject as IDataType;
                        PrintDataType(dataType, _projectItem);
                        break;
                    case PackageIds.printAdd_OnInstruction2:
                        aoi = _projectItem.AssociatedObject as IAoiDefinition;
                        PrintAOI(aoi, _projectItem);
                        break;
                    case PackageIds.printModuleProperties2:
                        PrintModuleProperties(controller, _projectItem);
                        break;
                    case PackageIds.printControllerOrganizer2:
                        PrintControllerOrganizer(controller);
                        break;
                    case PackageIds.printControllerProperties2:
                        PrintControllerProperties(controller);
                        break;
                    case PackageIds.printTags2:
                        PrintTags(controller, _projectItem);
                        break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private static void DoPrint(PrintDialog printDialog, DocumentPaginator document)
        {
            printDialog.PrintDocument(document, "ICSStudio");
        }

        public void PrintTask(CTask task, IController controller)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() != true) return;
            var uri = new Uri(@"pack://application:,,,/ICSStudio.Gui;component/Resources/styles/TaskTemplate.xaml",
                UriKind.RelativeOrAbsolute);
            var info = Application.GetResourceStream(uri);

            if (info == null) return;
            var doc = XamlReader.Load(info.Stream) as FlowDocument;

            var data = new TaskData(task, controller);
            if (doc == null) return;
            doc.DataContext = data;

#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            DispatcherHelper.Dispatcher.BeginInvoke(new DoPrintMethod(DoPrint),
                DispatcherPriority.ApplicationIdle, printDialog,
                ((IDocumentPaginatorSource) doc).DocumentPaginator);

#pragma warning restore VSTHRD001 // 避免旧线程切换 API
        }

        public void PrintProgram(IProgram program, IController controller)
        {
            var printDialog = new PrintDialog();

            if (printDialog.ShowDialog() != true) return;
            var uri = new Uri(
                @"pack://application:,,,/ICSStudio.Gui;component/Resources/styles/ProgramTemplate.xaml",
                UriKind.RelativeOrAbsolute);
            var info = Application.GetResourceStream(uri);

            if (info == null) return;
            var doc = XamlReader.Load(info.Stream) as FlowDocument;

            var data = new ProgramData(program);
            if (doc == null) return;
            doc.DataContext = data;

#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            DispatcherHelper.Dispatcher.BeginInvoke(new DoPrintMethod(DoPrint),
                DispatcherPriority.ApplicationIdle, printDialog,
                ((IDocumentPaginatorSource) doc).DocumentPaginator);

#pragma warning restore VSTHRD001 // 避免旧线程切换 API
        }

        public void PrintRoutineProperty(IRoutine routine, IController controller)
        {
            var printDialog = new PrintDialog();

            if (printDialog.ShowDialog() != true) return;
            var uri = new Uri(
                @"pack://application:,,,/ICSStudio.Gui;component/Resources/styles/RoutinePropertyTemplate.xaml",
                UriKind.RelativeOrAbsolute);
            var info = Application.GetResourceStream(uri);

            if (info == null) return;
            var doc = XamlReader.Load(info.Stream) as FlowDocument;

            var data = new RoutinePropertyData(routine, controller);
            if (doc == null) return;
            doc.DataContext = data;

#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            DispatcherHelper.Dispatcher.BeginInvoke(new DoPrintMethod(DoPrint),
                DispatcherPriority.ApplicationIdle, printDialog,
                ((IDocumentPaginatorSource) doc).DocumentPaginator);

#pragma warning restore VSTHRD001 // 避免旧线程切换 API
        }

        public void PrintRoutine(IRoutine routine, IController controller)
        {
            var printDialog = new PrintDialog();

            if (printDialog.ShowDialog() != true) return;
            var a = routine as STRoutine;
            var doc = new FlowDocument {ColumnWidth = 800};
            var table = new Table {FontFamily = new FontFamily("Times New Roman"), FontSize = 10};
            var num = 1;
            var tm = new TableColumn() {Width = new GridLength(20)};
            table.Columns.Add(tm);
            tm = new TableColumn() {Width = new GridLength(780)};
            table.Columns.Add(tm);
            var trg = new TableRowGroup();
            if (a != null)
                foreach (var i in a.CodeText)
                {
                    var tr = new TableRow();
                    var tc = new TableCell(new Paragraph(new Run(num.ToString())))
                    {
                        Foreground = new SolidColorBrush(Colors.DeepSkyBlue)
                    };
                    tr.Cells.Add(tc);
                    tc = new TableCell(new Paragraph(new Run(i)));
                    tr.Cells.Add(tc);
                    trg.Rows.Add(tr);
                    num++;
                }

            table.RowGroups.Add(trg);
            doc.Blocks.Add(table);

#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            DispatcherHelper.Dispatcher.BeginInvoke(new DoPrintMethod(DoPrint),
                DispatcherPriority.ApplicationIdle, printDialog,
                ((IDocumentPaginatorSource) doc).DocumentPaginator);

#pragma warning restore VSTHRD001 // 避免旧线程切换 API


        }

        public void PrintDataType(IDataType dataType, IProjectItem selectedProjectItem)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() != true) return;
            var doc = new FlowDocument {ColumnWidth = 800};
            var table = new Table {FontFamily = new FontFamily("Times New Roman"), FontSize = 10};
            Paragraph p = new Paragraph(new Run("Data type Name: "+dataType.Name));
            doc.Blocks.Add(p);
            p = new Paragraph(new Run("Description: " + dataType.Description));
            doc.Blocks.Add(p);
            p = new Paragraph(new Run("Size: " + (dataType.BitSize/8) + "byte(s)"));
            doc.Blocks.Add(p);
            for (var i = 0; i < 4; i++)
            {
                var tm = new TableColumn() { Width = new GridLength(200) };
                table.Columns.Add(tm);
            }
            var trg = new TableRowGroup();
            var tr = NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Name", Type = 1},
                new RowInfo {Message = "Value", Type = 1},
                new RowInfo {Message = "Data Type", Type = 1},
                new RowInfo {Message = "Style", Type = 1},
            });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);

            var viewmodel = new NewDataTypeViewModel(new NewDataType(), dataType);
            foreach (var i in viewmodel.DataGrid)
            {
                if(string.IsNullOrEmpty(i.Name)) continue;
                trg.Rows.Add(NewRow(new List<RowInfo>
                {
                    new RowInfo {Message = i.Name, Type = 1,Length = 2},
                    new RowInfo {Message = i.DataTypeInfo.ToString()},
                    new RowInfo {Message = i.DisplayStyle.ToString()},
                }));
                if (i.Description != string.Empty)
                {
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo {Message = i.Description,Length = 4},
                    }));
                }
                trg.Rows.Add(NewRow(new List<RowInfo>
                {
                    new RowInfo {Message = "External Access: "},
                    new RowInfo {Message = i.ExternalAccess.ToString()},
                }));
            }
            table.RowGroups.Add(trg);
            doc.Blocks.Add(table);
                

#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            DispatcherHelper.Dispatcher.BeginInvoke(new DoPrintMethod(DoPrint),
                DispatcherPriority.ApplicationIdle, printDialog,
                ((IDocumentPaginatorSource) doc).DocumentPaginator);

#pragma warning restore VSTHRD001 // 避免旧线程切换 API
        }

        public void PrintAOI(IAoiDefinition aoi, IProjectItem selectedProjectItem)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() != true) return;
            var parameters = aoi.Tags;
            var doc = new FlowDocument { ColumnWidth = 800 ,PagePadding = new Thickness(0,0,0,0)};
            var table = new Table { FontFamily = new FontFamily("Times New Roman"), FontSize = 10 };
            var p = new Paragraph(new Bold(new Run(aoi.Name+" v1.0")));
            doc.Blocks.Add(p);
            p = new Paragraph(new Bold(new Run("Available Languages")))
            {
                Background = new SolidColorBrush(Colors.DarkGray)
            };
            doc.Blocks.Add(p);
            p = new Paragraph(new Run("Relay Ladder "));
            doc.Blocks.Add(p);
            p = new Paragraph(new Run("Function Block"));
            doc.Blocks.Add(p);
            p = new Paragraph(new Run("Structured Text"));
            doc.Blocks.Add(p);
            var st = parameters.Where(i => i.Usage != Usage.Local).Aggregate(string.Empty, (current1, i) => aoi.Routines.OfType<STRoutine>().Where(routine => routine.CodeText.Any(t => t.Contains(i.Name))).Aggregate(current1, (current, routine) => current + (i.Name + ", ")));
            if (st != string.Empty)
            {
                st = st.Remove(st.Length - 2,2);
            }

            p = new Paragraph(new Run($"{aoi.Name}({st});"));
            doc.Blocks.Add(p);
            p = new Paragraph(new Bold(new Run("Parameters")))
            {
                Background = new SolidColorBrush(Colors.DarkGray)
            };
            doc.Blocks.Add(p);
            var tm = new TableColumn(){Width = new GridLength(50)};
            table.Columns.Add(tm);
            tm = new TableColumn() { Width = new GridLength(120) };
            table.Columns.Add(tm);
            tm = new TableColumn() { Width = new GridLength(80) };
            table.Columns.Add(tm);
            tm = new TableColumn() { Width = new GridLength(40) };
            table.Columns.Add(tm);
            tm = new TableColumn() { Width = new GridLength(110) };
            table.Columns.Add(tm);
            tm = new TableColumn() { Width = new GridLength(400) };
            table.Columns.Add(tm);
            var trg = new TableRowGroup();
            trg.Rows.Add(NewRow(new List<RowInfo>()
            {
                new RowInfo(){Message = "Required",Type = 1},
                new RowInfo(){Message = "Name",Type = 1},
                new RowInfo(){Message = "Data Type",Type = 1},
                new RowInfo(){Message = "Usage",Type = 1},
                new RowInfo(){Message = "Description",Type = 1},
            }));
            var tr = new TableRow();
            var tc = new TableCell(){ColumnSpan = 5};
            p = new Paragraph();
            var l = new Line()
            {
                X1 = 0,
                X2 = 400,
                Y1 = 0,
                Y2 = 0,
                Stroke = new SolidColorBrush(Colors.Black),
            };
            p.Inlines.Add(l);
            tc.Blocks.Add(p);
            tr.Cells.Add(tc);
            trg.Rows.Add(tr);

            trg.Rows.Add(NewRow(new List<RowInfo>()
            {
                new RowInfo(){Message = "X"},
                new RowInfo(){Message = aoi.Name},
                new RowInfo(){Message = aoi.Name},
                new RowInfo(){Message = "InOut"},
                new RowInfo(){Message = aoi.Description},
            }));

            foreach (var i in parameters)
            {
                if(i.Usage == Usage.Local) continue;
                trg.Rows.Add(NewRow(new List<RowInfo>()
                {
                    new RowInfo(){Message = i.IsRequired?"X":""},
                    new RowInfo(){Message = i.Name},
                    new RowInfo(){Message = i.DataTypeInfo.ToString()},
                    new RowInfo(){Message = i.Usage.ToString()},
                    new RowInfo(){Message = (i.Name != "EnableIn" && i.Name != "EnableOut")?aoi.Description:""},
                }));
            }
            table.RowGroups.Add(trg);
            doc.Blocks.Add(table);

            p = new Paragraph(new Bold(new Run("Extended Description")))
            {
                Background = new SolidColorBrush(Colors.DarkGray)
            };
            doc.Blocks.Add(p);

            p = new Paragraph(new Bold(new Run("Execution")))
            {
                Background = new SolidColorBrush(Colors.DarkGray)
            };
            doc.Blocks.Add(p);
            table = new Table { FontFamily = new FontFamily("Times New Roman"), FontSize = 10 };
            trg = new TableRowGroup();
            tm = new TableColumn() { Width = new GridLength(100) };
            table.Columns.Add(tm);
            tm = new TableColumn() { Width = new GridLength(100) };
            table.Columns.Add(tm);
            tm = new TableColumn() { Width = new GridLength(600) };
            table.Columns.Add(tm);
            trg.Rows.Add(NewRow(new List<RowInfo>()
            {
                new RowInfo(){Message = "Condition",Type = 1},
                new RowInfo(){Message = "Description",Type = 1},
            }));
            tr = new TableRow();
            tc = new TableCell { ColumnSpan = 2 };
            p = new Paragraph();
            var line = new Line
            {
                X1 = 0,
                X2 = 200,
                Y1 = 0,
                Y2 = 0,
                Stroke = new SolidColorBrush(Colors.Black),
            };
            p.Inlines.Add(line);
            tc.Blocks.Add(p);
            tr.Cells.Add(tc);
            trg.Rows.Add(tr);
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo(){Message = "EnableIn is true"},
            }));
            table.RowGroups.Add(trg);
            doc.Blocks.Add(table);

            p = new Paragraph(new Bold(new Run("Revision v1.0 Notes")))
            {
                Background = new SolidColorBrush(Colors.DarkGray)
            };
            doc.Blocks.Add(p);
            
            table = new Table { FontFamily = new FontFamily("Times New Roman"), FontSize = 10 };
            trg = new TableRowGroup();
            tm = new TableColumn() { Width = new GridLength(200) };
            table.Columns.Add(tm);
            tm = new TableColumn() { Width = new GridLength(200) };
            table.Columns.Add(tm);
            tm = new TableColumn() { Width = new GridLength(200) };
            table.Columns.Add(tm);
            tm = new TableColumn() { Width = new GridLength(200) };
            table.Columns.Add(tm);
            tr = NewRow(new List<RowInfo>()
            {
                new RowInfo() {Message = "Name", Type = 1},
                new RowInfo() {Message = "Default", Type = 1},
                new RowInfo() {Message = "Data Type", Type = 1},
                new RowInfo() {Message = "Scope", Type = 1},
            });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            foreach (var i in parameters)
            {
                if(i.Usage == Usage.Local) continue;
                if(i.Name == "EnableIn") continue;
                if(i.Name == "EnableOut") continue;

                var a = new FindLocation().Find(i as Tag);

                if (a.FindResult == string.Empty)
                {
                    continue;
                }
                var scope = i?.IsControllerScoped ?? true
                    ? "Controller"
                    : i.ParentCollection?.ParentProgram?.Name;
                trg.Rows.Add(NewRow(new List<RowInfo>
                {
                    new RowInfo() {Message = "  "+i.Name,Type = 1},
                    new RowInfo() {Message = "0"},
                    new RowInfo() {Message = i.DataTypeInfo.ToString()},
                    new RowInfo() {Message = scope},
                }));
                if (i.Description != string.Empty)
                {
                    trg.Rows.Add(NewRow(new List<RowInfo>(){new RowInfo(){Message = i.Description,Length = 4}}));
                }
                trg.Rows.Add(NewRow(new List<RowInfo>()
                {
                    new RowInfo() {Message = "    Usage"},
                    new RowInfo() {Message = i.Usage.ToString()},
                }));
                trg.Rows.Add(NewRow(new List<RowInfo>()
                {
                    new RowInfo() {Message = "    Required:"},
                    new RowInfo() {Message = i.IsRequired?"Yes":"No"},
                }));
                trg.Rows.Add(NewRow(new List<RowInfo>()
                {
                    new RowInfo() {Message = "    Visible:"},
                    new RowInfo() {Message = i.IsVisible?"Yes":"No"},
                }));
                trg.Rows.Add(NewRow(new List<RowInfo>()
                {
                    new RowInfo() {Message = "    Constant:"},
                    new RowInfo() {Message = i.IsConstant?"Yes":"No"},
                }));
                trg.Rows.Add(NewRow(new List<RowInfo>()
                {
                    new RowInfo() {Message = "    External Access:"},
                    new RowInfo() {Message = i.ExternalAccess.ToString()},
                }));
                if (!string.IsNullOrEmpty(i.Description))
                {
                    trg.Rows.Add(NewRow(new List<RowInfo>()
                    {
                        new RowInfo {Message = i.Description,Length = 4}
                    }));
                }
                if (!string.IsNullOrWhiteSpace(a.FindResult))
                {
                    trg.Rows.Add(NewRow(new List<RowInfo> { new RowInfo() { Message = "    "+i.Name + " - " + a.Container+"/"+a.Routine+" - " + a.FindResult+"\n",Type = 2, Length = 4 } }));
                }
                var find = new FindLocation().Find(i as Tag);
                trg.Rows.Add(new TableRow());
            }
            table.RowGroups.Add(trg);
            doc.Blocks.Add(table);

            var routines = aoi.Routines;
            foreach (var routine in routines)
            {
                table = new Table { FontFamily = new FontFamily("Times New Roman"), FontSize = 10 };
                trg = new TableRowGroup();
                tm = new TableColumn() { Width = new GridLength(20) };
                table.Columns.Add(tm);
                tm = new TableColumn() { Width = new GridLength(780) };
                table.Columns.Add(tm);
                var a = routine as STRoutine;
                var num = 1;
                if (a != null)
                    foreach (var i in a.CodeText)
                    {
                        tr = new TableRow();
                        tc = new TableCell(new Paragraph(new Run(num.ToString())))
                        {
                            Foreground = new SolidColorBrush(Colors.DeepSkyBlue)
                        };
                        tr.Cells.Add(tc);
                        tc = new TableCell(new Paragraph(new Run(i)));
                        tr.Cells.Add(tc);
                        trg.Rows.Add(tr);
                        num++;
                    }
                table.RowGroups.Add(trg);
                doc.Blocks.Add(table);
            }

#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            DispatcherHelper.Dispatcher.BeginInvoke(new DoPrintMethod(DoPrint),
                DispatcherPriority.ApplicationIdle, printDialog,
                ((IDocumentPaginatorSource)doc).DocumentPaginator);

#pragma warning restore VSTHRD001 // 避免旧线程切换 API
        }

        public void PrintModuleProperties(IController controller, IProjectItem selectedProjectItem)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true) return;
            var doc = new FlowDocument { ColumnWidth = 800 ,PagePadding = new Thickness(0,0,0,0),FontFamily = new FontFamily("Times New Roman"),FontSize = 10};

            var table = new Table();
            for (var j = 0; j < 4; j++)
            {
                var tm = new TableColumn() { Width = new GridLength(200) };
                table.Columns.Add(tm);
            }
            //Paragraph p;
            TableRowGroup trg;
            TableRow tr;
            ModuleDescriptor md;
            switch (selectedProjectItem.Kind)
            {
                case ProjectItemType.LocalModule:
                    var localModule = selectedProjectItem.AssociatedObject as LocalModule;
                    if (localModule != null)
                    {
                        trg = new TableRowGroup();
                        tr =NewRow(new List<RowInfo>()
                        {
                            new RowInfo() {Message = localModule.DisplayText + "：Local Modules", Length = 4, Type = 1}
                        });
                        tr.Background = new SolidColorBrush(Colors.DarkGray);
                        trg.Rows.Add(tr);
                        table.RowGroups.Add(trg);
                        var k = localModule as DeviceModule;
                            md = new ModuleDescriptor(k);
                            trg = new TableRowGroup();
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "  "+k.ParentModuleName + ":" + k.DisplayText, Length = 4, Type = 1}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Type:"},
                                new RowInfo() {Message = k.CatalogNumber + k.Description},
                                new RowInfo() {Message = "Parent:"},
                                new RowInfo() {Message = k.ParentModuleName}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Vendor:"},
                                new RowInfo() {Message = "I-CON"},
                                new RowInfo() {Message = "Vendor ID:"},
                                new RowInfo() {Message = k.Vendor.ToString()}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Slot:"},
                                new RowInfo() {Message = "0"},
                                new RowInfo() {Message = "Electronic Keying:"},
                                new RowInfo() {Message = k.EKey.ToString()}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Revision:"},
                                new RowInfo() {Message = k.Major + "." + k.Minor.ToString("D3")},
                                new RowInfo() {Message = "Status:"},
                                new RowInfo() {Message = md.Status}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Module Fault:"},
                                new RowInfo() {Message = k.ParentController.IsOnline ? "Online" : "Offline"},
                                new RowInfo() {Message = "Inhibit Flag"},
                                new RowInfo() {Message = k.Inhibited ? "On\n\n" : "Off\n\n"}
                            }));
                            table.RowGroups.Add(trg);
                            doc.Blocks.Add(table);
                    }

                    break;
                case ProjectItemType.Bus:
                    table = new Table();
                    trg = new TableRowGroup();
                    tr = NewRow(new List<RowInfo>()
                    {
                        new RowInfo()
                            {Message = (selectedProjectItem.AssociatedObject as DeviceModule)?.DisplayText+ " ：Local Modules", Length = 4, Type = 1}
                    });
                    tr.Background = new SolidColorBrush(Colors.DarkGray);
                    trg.Rows.Add(tr);
                    table.RowGroups.Add(trg);
                    foreach (var i in controller.DeviceModules)
                    {
                        var j = i as DeviceModule;
                        
                        if (j.GetFirstPort(PortType.PointIO) != null)
                        {
                            md = new ModuleDescriptor(j);
                            trg = new TableRowGroup();
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo()
                                    {Message = "  " + j.ParentModuleName + ":" + j.DisplayText, Length = 4, Type = 1}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Type:"},
                                new RowInfo() {Message = j.CatalogNumber+j.Description},
                                new RowInfo() {Message = "Parent:"},
                                new RowInfo() {Message = j.ParentModuleName}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Vendor:"},
                                new RowInfo() {Message = "I-CON"},
                                new RowInfo() {Message = "Vendor ID:"},
                                new RowInfo() {Message = j.Vendor.ToString()}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Slot:"},
                                new RowInfo() {Message = "0"},
                                new RowInfo() {Message = "Electronic Keying:"},
                                new RowInfo() {Message = j.EKey.ToString()}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Revision:"},
                                new RowInfo() {Message = j.Major + "." + j.Minor.ToString("D3")},
                                new RowInfo() {Message = "Status:"},
                                new RowInfo() {Message = md.Status}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Module Fault:"},
                                new RowInfo() {Message = j.ParentController.IsOnline ? "Online" : "Offline"},
                                new RowInfo() {Message = "Inhibit Flag"},
                                new RowInfo() {Message = j.Inhibited ? "On\r\n" : "Off\r\n"}
                            }));
                            table.RowGroups.Add(trg);
                        }
                    }
                    doc.Blocks.Add(table);
                    break;
                case ProjectItemType.DeviceModule:
                    DeviceModule deviceModule = selectedProjectItem.AssociatedObject as DeviceModule;
                    if (deviceModule == null) break;
                    var pointIOPort = deviceModule?.GetFirstPort(PortType.PointIO);
                    var slot = pointIOPort?.Bus?.Size - 1 ?? 0;
                    md = new ModuleDescriptor(deviceModule);

                    trg = new TableRowGroup();
                    tr = NewRow(new List<RowInfo>()
                    {
                        new RowInfo() {Message = deviceModule.ParentModule.DisplayText + "：Local Modules", Length = 4, Type = 1}
                    });
                    tr.Background = new SolidColorBrush(Colors.DarkGray);
                    trg.Rows.Add(tr);
                    trg.Rows.Add(NewRow(new List<RowInfo>()
                    {
                        new RowInfo() {Message = "  "+deviceModule.ParentModuleName + ":" + deviceModule.DisplayText, Length = 4, Type = 1}
                    }));
                    trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Type:"},
                                new RowInfo() {Message = deviceModule.CatalogNumber+deviceModule.Description},
                                new RowInfo() {Message = "Parent:"},
                                new RowInfo() {Message = deviceModule.ParentModuleName}
                            }));
                    trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Vendor:"},
                                new RowInfo() {Message = "I-CON"},
                                new RowInfo() {Message = "Vendor ID:"},
                                new RowInfo() {Message = deviceModule.Vendor.ToString()}
                            }));
                    trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Slot:"},
                                new RowInfo() {Message = slot.ToString()},
                                new RowInfo() {Message = "Electronic Keying:"},
                                new RowInfo() {Message = deviceModule.EKey.ToString()}
                            }));
                    trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Revision:"},
                                new RowInfo() {Message = deviceModule.Major + "." + deviceModule.Minor.ToString("D3")},
                                new RowInfo() {Message = "Status:"},
                                new RowInfo() {Message = md.Status}
                            }));
                    trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Module Fault:"},
                                new RowInfo() {Message = deviceModule.ParentController.IsOnline ? "Online" : "Offline"},
                                new RowInfo() {Message = "Inhibit Flag"},
                                new RowInfo() {Message = deviceModule.Inhibited ? "On\r\n" : "Off\r\n"}
                            }));
                    table.RowGroups.Add(trg);
                    doc.Blocks.Add(table);
                    break;
                case ProjectItemType.Ethernet:
                    foreach (var i in controller.DeviceModules)
                    {
                        var j = i as DeviceModule;
                        if (j.GetFirstPort(PortType.Ethernet) != null)
                        {
                            md = new ModuleDescriptor(j);
                            table = new Table();
                            trg = new TableRowGroup();
                            tr = NewRow(new List<RowInfo>()
                            {
                                new RowInfo()
                                    {Message = "Ethernet ：Local Modules", Length = 4, Type = 1}
                            });
                            tr.Background = new SolidColorBrush(Colors.DarkGray);
                            trg.Rows.Add(tr);
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo()
                                    {Message = "  " + j.ParentModuleName + ":" + j.DisplayText, Length = 4, Type = 1}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Type:"},
                                new RowInfo() {Message = j.CatalogNumber+j.Description},
                                new RowInfo() {Message = "Parent:"},
                                new RowInfo() {Message = j.ParentModuleName}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Vendor:"},
                                new RowInfo() {Message = "I-CON"},
                                new RowInfo() {Message = "Vendor ID:"},
                                new RowInfo() {Message = j.Vendor.ToString()}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Slot:"},
                                new RowInfo() {Message = "0"},
                                new RowInfo() {Message = "Electronic Keying:"},
                                new RowInfo() {Message = j.EKey.ToString()}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Revision:"},
                                new RowInfo() {Message = j.Major + "." + j.Minor.ToString("D3")},
                                new RowInfo() {Message = "Status:"},
                                new RowInfo() {Message = md.Status}
                            }));
                            trg.Rows.Add(NewRow(new List<RowInfo>()
                            {
                                new RowInfo() {Message = "      Module Fault:"},
                                new RowInfo() {Message = j.ParentController.IsOnline ? "Online" : "Offline"},
                                new RowInfo() {Message = "Inhibit Flag"},
                                new RowInfo() {Message = j.Inhibited ? "On\r\n" : "Off\r\n"}
                            }));
                            table.RowGroups.Add(trg);
                            doc.Blocks.Add(table);
                        }
                    }
                    break;
            }



#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            DispatcherHelper.Dispatcher.BeginInvoke(new DoPrintMethod(DoPrint),
                    DispatcherPriority.ApplicationIdle, printDialog,
                    ((IDocumentPaginatorSource)doc).DocumentPaginator);

#pragma warning restore VSTHRD001 // 避免旧线程切换 API




        }

        public void PrintControllerOrganizer( IController controller)
        {

            var printDialog = new PrintDialog();
            
            if (printDialog.ShowDialog() != true) return;
            var doc = new FlowDocument { ColumnWidth = 800 };
            var table = new Table { FontFamily = new FontFamily("Times New Roman"), FontSize = 10 };
            var trg = new TableRowGroup();
            trg.Rows.Add(NewRow(new List<RowInfo> { new RowInfo { Message = "  " + controller.Name, Type = 1 } }));
            trg.Rows.Add(NewRow(new List<RowInfo> { new RowInfo { Message = "  Controller Fault Handler", Type = 1 } }));
            trg.Rows.Add(NewRow(new List<RowInfo> { new RowInfo { Message = "  Power-Up Handler", Type = 1 } }));
            var tr = NewRow(new List<RowInfo> { new RowInfo { Message = "Tasks", Type = 1 } });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            foreach (var i in controller.Tasks)
            {
                trg.Rows.Add(NewRow(new List<RowInfo> { new RowInfo { Message = "  " + i.Name, Type = 1 } }));
                foreach (var j in controller.Programs)
                {
                    if (j.ParentTask != i) continue;
                    trg.Rows.Add(NewRow(new List<RowInfo> { new RowInfo { Message = "    " + j.Name, Type = 1 } }));
                    foreach (var k in j.Routines)
                    {
                        if (string.IsNullOrEmpty(k.Description))
                        {
                            trg.Rows.Add(
                                NewRow(new List<RowInfo> {new RowInfo {Message = "      " + k.Name, Type = 1}}));
                        }
                        else
                        {
                            trg.Rows.Add(NewRow(new List<RowInfo>
                            {
                                new RowInfo {Message = "      " + k.Name, Type = 1}}));
                            trg.Rows.Add(NewRow(new List<RowInfo>
                            {
                                new RowInfo {Message = "        " + k.Description, Type = 1}}));
                        }
                    }
                }
            }
            trg.Rows.Add(NewRow(new List<RowInfo>{ new RowInfo { Message = "  Unscheduled" ,Type = 1} }));
            foreach (var j in controller.Programs)
            {
                var check = controller.Tasks.Any(i => j.ParentTask == i);
                if(check) continue;
                trg.Rows.Add(NewRow(new List<RowInfo> { new RowInfo { Message = "    " + j.Name, Type = 1 } }));
                foreach (var k in j.Routines)
                {
                    if (string.IsNullOrEmpty(k.Description))
                    {
                        trg.Rows.Add(
                            NewRow(new List<RowInfo> { new RowInfo { Message = "      " + k.Name, Type = 1 } }));
                    }
                    else
                    {
                        trg.Rows.Add(NewRow(new List<RowInfo>
                        {
                            new RowInfo {Message = "      " + k.Name, Type = 1}}));
                        trg.Rows.Add(NewRow(new List<RowInfo>
                        {
                            new RowInfo {Message = "        " + k.Description, Type = 1}}));
                    }
                }
            }

            tr = NewRow(new List<RowInfo> {new RowInfo {Message = "Motion Groups", Type = 1}});
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            List<string> ungroupedNameList = new List<string>();
            List<string> ungroupedDList = new List<string>();
            foreach (var i in controller.Tags)
            {
                if (!i.IsAlias && !i.DataTypeInfo.DataType.IsMotionGroupType) continue;
                trg.Rows.Add(
                    NewRow(new List<RowInfo> { new RowInfo { Message = "  " + i.Name, Type = 1 } }));
                var a = new MotionGroupItem(i);
                foreach (var tag in controller.Tags)
                {
                    Tag axis = controller.Tags[tag.Name] as Tag;
                    AxisCIPDrive axisCIPDrive = axis?.DataWrapper as AxisCIPDrive;
                    if (axisCIPDrive != null)
                    {

                        if (axisCIPDrive.AssignedGroup == a.AssociatedObject)
                        {
                            if (string.IsNullOrEmpty(tag.Description))
                            {
                                trg.Rows.Add(
                                    NewRow(new List<RowInfo> { new RowInfo { Message = "    " + tag.Name, Type = 1 } }));
                            }
                            else
                            {
                                trg.Rows.Add(NewRow(new List<RowInfo>
                                {
                                    new RowInfo {Message = "    " + tag.Name, Type = 1}}));
                                trg.Rows.Add(NewRow(new List<RowInfo>
                                {
                                    new RowInfo {Message = "      " + tag.Description, Type = 1}}));
                            }
                        }
                        else
                        {
                            ungroupedNameList.Add(tag.Name);
                            ungroupedDList.Add(tag.Description);
                        }
                    }

                    AxisVirtual axisVirtual = axis?.DataWrapper as AxisVirtual;
                    if (axisVirtual != null)
                    {

                        if (axisVirtual.AssignedGroup == a.AssociatedObject)
                        {
                            if (string.IsNullOrEmpty(tag.Description))
                            {
                                trg.Rows.Add(
                                    NewRow(new List<RowInfo> { new RowInfo { Message = "    " + tag.Name, Type = 1 } }));
                            }
                            else
                            {
                                trg.Rows.Add(NewRow(new List<RowInfo>
                                {
                                    new RowInfo {Message = "    " + tag.Name, Type = 1}}));
                                trg.Rows.Add(NewRow(new List<RowInfo>
                                {
                                    new RowInfo {Message = "      " + tag.Description, Type = 1}}));
                            }
                        }
                        else
                        {
                            ungroupedNameList.Add(tag.Name);
                            ungroupedDList.Add(tag.Description);
                        }
                    }
                }
            }
            trg.Rows.Add(NewRow(new List<RowInfo> { new RowInfo { Message = "  Ungrouped Axes", Type = 1 } }));
            for (var i = 0; i < ungroupedNameList.Count; i++)
            {
                if (string.IsNullOrEmpty(ungroupedDList[i]))
                {
                    trg.Rows.Add(
                        NewRow(new List<RowInfo> { new RowInfo { Message = "    " + ungroupedNameList[i], Type = 1 } }));
                }
                else
                {
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo {Message = "    " + ungroupedNameList[i], Type = 1}}));
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo {Message = "      " + ungroupedDList[i], Type = 1}}));
                }
            }
            tr = NewRow(new List<RowInfo> { new RowInfo { Message = "UD Function Block", Type = 1 } });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            foreach (var i in controller.AOIDefinitionCollection)
            {
                trg.Rows.Add(
                    NewRow(new List<RowInfo> { new RowInfo { Message = "  " + i.Name, Type = 1 } }));
                foreach (var k in i.Routines)
                {
                    if (string.IsNullOrEmpty(k.Description))
                    {
                        trg.Rows.Add(
                            NewRow(new List<RowInfo> { new RowInfo { Message = "    " + k.Name, Type = 1 } }));
                    }
                    else
                    {
                        trg.Rows.Add(NewRow(new List<RowInfo>
                        {
                            new RowInfo {Message = "    " + k.Name, Type = 1}}));
                        trg.Rows.Add(NewRow(new List<RowInfo>
                        {
                            new RowInfo {Message = "      " + k.Description, Type = 1}}));
                    }
                }
            }
            tr = NewRow(new List<RowInfo> { new RowInfo { Message = "Data Types", Type = 1 } });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            trg.Rows.Add(NewRow(new List<RowInfo> { new RowInfo { Message = "  User-Defined", Type = 1 } }));
            foreach (var i in controller.DataTypes)
            {
                if (!(i is UserDefinedDataType)) continue;
                if (string.IsNullOrEmpty(i.Description))
                {
                    trg.Rows.Add(
                        NewRow(new List<RowInfo> { new RowInfo { Message = "    " + i.Name, Type = 1 } }));
                }
                else
                {
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo {Message = "    " + i.Name, Type = 1}}));
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo {Message = "      " + i.Description, Type = 1}}));
                }
            }
            trg.Rows.Add(NewRow(new List<RowInfo> { new RowInfo { Message = "  Strings", Type = 1 } }));
            foreach (var i in controller.DataTypes)
            {
                if (!(i.IsStringType) || (i.IsPredefinedType)) continue;
                if (string.IsNullOrEmpty(i.Description))
                {
                    trg.Rows.Add(
                        NewRow(new List<RowInfo> { new RowInfo { Message = "    " + i.Name, Type = 1 } }));
                }
                else
                {
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo {Message = "    " + i.Name, Type = 1}}));
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo {Message = "      " + i.Description, Type = 1}}));
                }
            }
            trg.Rows.Add(NewRow(new List<RowInfo> { new RowInfo { Message = "  Add-On-Defined", Type = 1 } }));
            foreach (var i in controller.DataTypes)
            {
                if (!(i is AOIDataType)) continue;
                if (string.IsNullOrEmpty(i.Description))
                {
                    trg.Rows.Add(
                        NewRow(new List<RowInfo> { new RowInfo { Message = "    " + i.Name, Type = 1 } }));
                }
                else
                {
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo {Message = "    " + i.Name, Type = 1}}));
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo {Message = "      " + i.Description, Type = 1}}));
                }
            }
            trg.Rows.Add(NewRow(new List<RowInfo> { new RowInfo { Message = "  Module-Defined", Type = 1 } }));
            foreach (var i in controller.DataTypes)
            {
                if (!(i is ModuleDefinedDataType)) continue;
                if (string.IsNullOrEmpty(i.Description))
                {
                    trg.Rows.Add(
                        NewRow(new List<RowInfo> { new RowInfo { Message = "    " + i.Name, Type = 1 } }));
                }
                else
                {
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo {Message = "    " + i.Name, Type = 1}}));
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo {Message = "      " + i.Description, Type = 1}}));
                }
            }
            tr = NewRow(new List<RowInfo> { new RowInfo { Message = "Trends", Type = 1 } });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            foreach (var i in controller.Trends)
            {
                trg.Rows.Add(
                    NewRow(new List<RowInfo> { new RowInfo { Message = "    " + i.Name, Type = 1 } }));
            }
            tr = NewRow(new List<RowInfo> { new RowInfo { Message = "I/O Configuration", Type = 1 } });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            foreach (var i in controller.DeviceModules)
            {
                if (i.ParentController == controller)
                {
                    trg.Rows.Add(
                        NewRow(new List<RowInfo> { new RowInfo { Message = "    " + i.DisplayText, Type = 1 } }));
                }
            }
            table.RowGroups.Add(trg);
            doc.Blocks.Add(table);

#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            DispatcherHelper.Dispatcher.BeginInvoke(new DoPrintMethod(DoPrint),
                DispatcherPriority.ApplicationIdle, printDialog,
                ((IDocumentPaginatorSource)doc).DocumentPaginator);

#pragma warning restore VSTHRD001 // 避免旧线程切换 API


        }

        public void PrintControllerProperties(IController controller)
        {

            var printDialog = new PrintDialog();
            var a = controller as Controller;
            if (printDialog.ShowDialog() != true) return;
            LocalModule localModule = controller.DeviceModules["Local"] as LocalModule; 
            var doc = new FlowDocument { ColumnWidth = 800 ,PagePadding = new Thickness(0,0,0,0)};
            var table = new Table { FontFamily = new FontFamily("Times New Roman"), FontSize = 10 };
            var trg = new TableRowGroup();
            for (var i = 0; i < 4; i++)
            {
                var tb= new TableColumn(){Width = new GridLength(200)};
                table.Columns.Add(tb);
            }

            var tr = NewRow(new List<RowInfo> { new RowInfo { Message = "General", Type = 1,Length = 4} });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Vendor:"},
                new RowInfo {Message = "Rockwell Automation/Allen-Bradley"},
                new RowInfo {Message = "Mode:"},
                new RowInfo {Message = controller.IsOnline?"Online":"Offline"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Revision:"},
                new RowInfo {Message = "32"},
                new RowInfo {Message = "Key Switch Position:"},
                new RowInfo {Message = controller.IsOnline?"Online":"Offline"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Chassis Type: "},
                new RowInfo {Message = localModule?.DisplayText},
                new RowInfo {Message = "Created:"},
                new RowInfo {Message = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Slot: "},
                new RowInfo {Message = "0"},
                new RowInfo {Message = "Edited:"},
                new RowInfo {Message = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},
            }));
            tr = NewRow(new List<RowInfo> { new RowInfo { Message = "Date/Time", Type = 1, Length = 4 } });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Data and Time: "},
                new RowInfo {Message = controller.IsOnline?new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("yyyy/MM/dd HH:mm:ss"):"<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Time Zone:"},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Daylight Saving (+00:00): "},
                new RowInfo {Message = controller.IsOnline?"No":"<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Enable Time Synchronization: "},
                new RowInfo {Message = a != null && a.TimeSetting.PTPEnable ? "true" : "false"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Is the system time master: "},
                new RowInfo {Message = controller.IsOnline?"Off":"<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Is a synchronized time slave: "},
                new RowInfo {Message = controller.IsOnline?"Off":"<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Duplicate CST Master Detected: "},
                new RowInfo {Message = controller.IsOnline?"Off":"<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "CST Mastership disabled:  "},
                new RowInfo {Message = controller.IsOnline?"Off":"<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "No CST Master: "},
                new RowInfo {Message = controller.IsOnline?"Off":"<offline>"},
            }));
            tr = NewRow(new List<RowInfo> { new RowInfo { Message = "Advanced Time Sync", Type = 1, Length = 4 } });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "CIP Sync Time Synchronization: "},
                new RowInfo {Message = "Enabled"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "CST Mastership disabled:  "},
                new RowInfo {Message = "<offline>\n"},
            }));

            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Grandmaster Clock",Type = 1},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Identity:  "},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Class: "},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Accuracy: "},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Variance:"},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Source: "},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Priority 1: "},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Priority 2: "},
                new RowInfo {Message = "<offline>\n"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Local Clock",Type = 1},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Synchronization Status:"},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Offset from Master: "},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Backplane State: "},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Ethernet State: "},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Identity:"},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Class:"},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Accuracy:"},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Variance:"},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Source:"},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Priority 1:"},
                new RowInfo {Message = "128 (Master Override)"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Priority 2:"},
                new RowInfo {Message = "128 (Tie Breaker)"},
            }));

            tr = NewRow(new List<RowInfo> { new RowInfo { Message = "Advanced", Type = 1, Length = 4 } });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            if (a != null)
            {
                trg.Rows.Add(NewRow(new List<RowInfo>
                {
                    new RowInfo {Message = "Controller Fault Handler: "},
                    new RowInfo
                    {
                        Message = !string.IsNullOrEmpty(controller.MajorFaultProgram) ? "<none>" : a.MajorFaultProgram
                    },
                    new RowInfo {Message = "Serial Number: "},
                    new RowInfo {Message = a.ProjectSN.ToString()},
                }));
                trg.Rows.Add(NewRow(new List<RowInfo>
                {
                    new RowInfo {Message = "Power-Up Handler:"},
                    new RowInfo {Message = !string.IsNullOrEmpty(controller.PowerLossProgram) ? "<none>" : a.PowerLossProgram},
                    new RowInfo {Message = "Allow Consumed Tags to Use RPI Provided by Producer: "},
                    new RowInfo {Message = "No"},
                }));
            }

            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Match Project To Controller: "},
                new RowInfo {Message = controller.MatchProjectToController?"Yes":"No"},
                new RowInfo {Message = "Report Overflow Faults: "},
                new RowInfo {Message = "No"},
            }));

            tr = NewRow(new List<RowInfo> { new RowInfo { Message = "SFC Execution", Type = 1, Length = 4 } });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Execution Control: "},
                new RowInfo {Message = "Execute current active steps only"},
                new RowInfo {Message = "Last Scan of Active Step: "},
                new RowInfo {Message = "Don't scan"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Restart Position: "},
                new RowInfo {Message = "Restart at most recently executed step"},
            }));

            tr = NewRow(new List<RowInfo> { new RowInfo { Message = "Nonvolatile Memory", Type = 1, Length = 4 } });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = controller.IsOnline?"Empty":"<offline>"},
            }));

            tr = NewRow(new List<RowInfo> { new RowInfo { Message = "Memory", Type = 1, Length = 4 } });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "<Capacity:>",Type = 1},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Total:"},
                new RowInfo {Message = "20,971,520 blocks"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Available: "},
                new RowInfo {Message = "17,794,676 blocks"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Used:"},
                new RowInfo {Message = "3,176,844 blocks"},
            }));

            tr = NewRow(new List<RowInfo> { new RowInfo { Message = "Security:", Type = 1, Length = 4 } });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Primary Security Authority: "},
                new RowInfo {Message = "No Protection"},
                new RowInfo {Message = "Restrict Communications Except Through Selected Slots:"},
                new RowInfo {Message = "No"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Use only the selected Security Authority for Authentication and Authorization:"},
                new RowInfo {Message = "No"},
                new RowInfo {Message = "Selected Slots:"},
                new RowInfo {Message = ""},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Secondary Security Authority:"},
                new RowInfo {Message = "No Protection"},
                new RowInfo {Message = "Changes To Detect: "},
                new RowInfo {Message = "16#ffff_ffff_ffff_ffff"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Use only the selected Security Authority for Authentication and Authorization:"},
                new RowInfo {Message = "No"},
                new RowInfo {Message = "Audit Value: "},
                new RowInfo {Message = "<offline>"},
            }));
            trg.Rows.Add(NewRow(new List<RowInfo>
            {
                new RowInfo {Message = "Permission Set:"},
                new RowInfo {Message = ""},
            }));

            tr = NewRow(new List<RowInfo> { new RowInfo { Message = "Port Configuration", Type = 1, Length = 4 } });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            var n = 1;
            foreach (var i in new FindLocation().GetPorts(controller))
            {
                trg.Rows.Add(NewRow(new List<RowInfo>
                {
                    new RowInfo {Message = "Port"+n},
                }));
                trg.Rows.Add(NewRow(new List<RowInfo>
                {
                    new RowInfo {Message = "Enable:"},
                    new RowInfo {Message = "Yes"},
                }));
                n++;
            }

            if (controller.IsOnline)
            {
                tr = NewRow(new List<RowInfo> { new RowInfo { Message = "Advanced Network", Type = 1, Length = 4 } });
                tr.Background = new SolidColorBrush(Colors.DarkGray);
                trg.Rows.Add(tr);
                trg.Rows.Add(NewRow(new List<RowInfo>
                {
                    new RowInfo {Message = "Network Technology:"},
                    new RowInfo {Message = "Linear/Star"},
                }));
                trg.Rows.Add(NewRow(new List<RowInfo>
                {
                    new RowInfo {Message = "Supervisor Mode:"},
                    new RowInfo {Message = "Disabled"},
                }));
            }

            table.RowGroups.Add(trg);
            doc.Blocks.Add(table);

#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            DispatcherHelper.Dispatcher.BeginInvoke(new DoPrintMethod(DoPrint),
                DispatcherPriority.ApplicationIdle, printDialog,
                ((IDocumentPaginatorSource)doc).DocumentPaginator);

#pragma warning restore VSTHRD001 // 避免旧线程切换 API


        }

        public void PrintTags(IController controller, IProjectItem selectProjectItem)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true) return;
            var doc = new FlowDocument { ColumnWidth = 800, PagePadding = new Thickness(0, 0, 0, 0) };
            var table = new Table { FontFamily = new FontFamily("Times New Roman"), FontSize = 10 };
            var trg = new TableRowGroup();
            var tr = NewRow(new List<RowInfo>()
            {
                new RowInfo() {Message = "Name", Type = 1},
                new RowInfo() {Message = "Value", Type = 1},
                new RowInfo() {Message = "Data Type", Type = 1},
                new RowInfo() {Message = "Scope", Type = 1},
            });
            tr.Background = new SolidColorBrush(Colors.DarkGray);
            trg.Rows.Add(tr);
            for (int i = 0; i < 4; i++)
            {
                table.Columns.Add(new TableColumn(){Width = new GridLength(200)});
            }

            IProgram program = null;
            IAoiDefinition aoi = null;
            IRoutine iRoutine = null;
            ITag motionGroup = null;
            if (selectProjectItem.Kind == ProjectItemType.ProgramTags)
            {
                var a = selectProjectItem.AssociatedObject as IProgram;
                foreach (var i in controller.Programs)
                {
                    if (a != i) continue;
                    if (i != null)
                    {
                        program = i;
                    }

                    break;
                }
                var b = selectProjectItem.AssociatedObject as IAoiDefinition;
                foreach (var i in controller.AOIDefinitionCollection)
                {
                    if (b != i) continue;
                    if (i != null)
                    {
                        aoi = i;
                    }

                    break;
                }
            }
            else if (selectProjectItem.Kind == ProjectItemType.Routine)
            {
                iRoutine = selectProjectItem.AssociatedObject as IRoutine;
            }
            else if (selectProjectItem.Kind == ProjectItemType.MotionGroup)
            {
                motionGroup = selectProjectItem.AssociatedObject as ITag;
            }
            if (program != null)
            {
                foreach (var i in program.Tags)
                {
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo() {Message = i.Name, Type = 1},
                        new RowInfo() {Message = "0"},
                        new RowInfo() {Message = i.DataTypeInfo.ToString(), Type = 1},
                        new RowInfo() {Message = program.Name},
                    }));
                    if (!string.IsNullOrEmpty(i.Description))
                    {
                        trg.Rows.Add(NewRow(new List<RowInfo>
                        {
                            new RowInfo() {Message = i.Description,Length = 4},
                        }));
                    }
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo() {Message = "Constant"},
                        new RowInfo() {Message = i.IsConstant?"Yes":"No"},
                    }));
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo() {Message = "External Access:"},
                        new RowInfo() {Message = i.ExternalAccess.ToString()},
                    }));
                    if (!string.IsNullOrEmpty(i.Description))
                    {
                        trg.Rows.Add(NewRow(new List<RowInfo>()
                        {
                            new RowInfo() {Message = i.Description, Length = 4}
                        }));
                    }
                    var a = new FindLocation().Find(i as Tag);
                    if (!string.IsNullOrWhiteSpace(a.FindResult))
                        trg.Rows.Add(NewRow(new List<RowInfo>
                        {
                            new RowInfo() {Message = $" {i.Name} - {a.Container}/{a.Routine} {a.FindResult}\n",Type = 2,Length = 4},
                        }));
                }
            }

            if (aoi != null)
            {
                foreach (var i in aoi.Tags)
                {
                    if (i.Name == "EnableIn" || i.Name == "EnableOut") continue;
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo() {Message = " "+i.Name, Type = 1},
                        new RowInfo() {Message = "0"},
                        new RowInfo() {Message = i.DataTypeInfo.ToString(), Type = 1},
                        new RowInfo() {Message = aoi.Name},
                    }));
                    if (!string.IsNullOrEmpty(i.Description))
                    {
                        trg.Rows.Add(NewRow(new List<RowInfo>
                        {
                            new RowInfo() {Message = i.Description,Length = 4},
                        }));
                    }
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo() {Message = " Constant"},
                        new RowInfo() {Message = i.IsConstant?"Yes":"No"},
                    }));
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo() {Message = " External Access:"},
                        new RowInfo() {Message = i.ExternalAccess.ToString()},
                    }));
                    if (string.IsNullOrEmpty(i.Description))
                    {
                        trg.Rows.Add(NewRow(new List<RowInfo>()
                        {
                            new RowInfo() {Message = i.Description, Length = 4}
                        }));
                    }
                    var a = new FindLocation().Find(i as Tag);
                    if (!string.IsNullOrWhiteSpace(a.FindResult))
                        trg.Rows.Add(NewRow(new List<RowInfo>
                        {
                            new RowInfo() {Message = $" {i.Name} - {a.Container}/{a.Routine} {a.FindResult}\n",Type = 2,Length = 4},
                        }));
                }
            }

            if (iRoutine != null)
            {
                var routine = iRoutine as STRoutine;

                if (routine != null)
                    foreach (var i in routine.GetAllReferenceTags())
                    {
                        var scope = i?.IsControllerScoped ?? true
                            ? "Controller"
                            : i.ParentCollection?.ParentProgram?.Name;
                        trg.Rows.Add(NewRow(new List<RowInfo>
                        {
                            new RowInfo() {Message = " "+i.Name, Type = 1},
                            new RowInfo() {Message = "0"},
                            new RowInfo() {Message = i.DataTypeInfo.ToString(), Type = 1},
                            new RowInfo() {Message = scope},
                        }));
                        if(!string.IsNullOrWhiteSpace(i.Description))
                            trg.Rows.Add(NewRow(new List<RowInfo>
                            {
                                new RowInfo() {Message = " "+i.Description,Length = 4},
                            }));
                        trg.Rows.Add(NewRow(new List<RowInfo>
                        {
                            new RowInfo() {Message = " Constant"},
                            new RowInfo() {Message = i.IsConstant?"Yes":"No"},
                        }));
                        trg.Rows.Add(NewRow(new List<RowInfo>
                        {
                            new RowInfo() {Message = " External Access"},
                            new RowInfo() {Message = i.ExternalAccess.ToString()},
                        }));
                        var a = new FindLocation().Find(i as Tag);
                        if (!string.IsNullOrWhiteSpace(a.FindResult))
                            trg.Rows.Add(NewRow(new List<RowInfo>
                            {
                                new RowInfo() {Message = $" {i.Name} - {a.Container}/{a.Routine} {a.FindResult}\n",Type = 2,Length = 4},
                            }));
                    }
            }

            if (motionGroup != null)
            {
                trg.Rows.Add(NewRow(new List<RowInfo>
                {
                    new RowInfo() {Message = " "+motionGroup.Name, Type = 1,Length = 2},
                    new RowInfo() {Message = motionGroup.DataTypeInfo.ToString(),Type = 1},
                    new RowInfo() {Message = controller.Name, Type = 1},
                }));
                trg.Rows.Add(NewRow(new List<RowInfo>
                {
                    new RowInfo() {Message = "  External Access"},
                    new RowInfo() {Message = motionGroup.ExternalAccess.ToString()},
                }));
                trg.Rows.Add(NewRow(new List<RowInfo>
                {
                    new RowInfo() {Message = " "+motionGroup.Name+".GroupSynced",Type = 1,Length = 4},
                }));
                var a = new FindLocation().Find(motionGroup as Tag);
                if (!string.IsNullOrWhiteSpace(a.FindResult))
                    trg.Rows.Add(NewRow(new List<RowInfo>
                    {
                        new RowInfo() {Message = $"    {motionGroup.Name}.GroupSynced - {a.Container}/{a.Routine} - {a.FindResult}\n",Type = 2,Length = 4},
                    }));
            }

            table.RowGroups.Add(trg);
            doc.Blocks.Add(table);

#pragma warning disable VSTHRD001 // 避免旧线程切换 API
            DispatcherHelper.Dispatcher.BeginInvoke(new DoPrintMethod(DoPrint),
                DispatcherPriority.ApplicationIdle, printDialog,
                ((IDocumentPaginatorSource)doc).DocumentPaginator);

#pragma warning restore VSTHRD001 // 避免旧线程切换 API
        }

        public TableRow NewRow(List<RowInfo> rowInfos)
        {
            var newRow = new TableRow();
            foreach (var i in rowInfos)
            {
                TableCell newCell = new TableCell();
                switch (i.Type) //type 0不加任何修饰，1为粗体，2为斜体，3为粗体+斜体,length为占用column数量
                {
                    case 0:
                        newCell = new TableCell(new Paragraph(new Run(i.Message))) { ColumnSpan = i.Length };
                        break;
                    case 1:
                        newCell = new TableCell(new Paragraph(new Bold(new Run(i.Message)))) { ColumnSpan = i.Length };
                        break;
                    case 2:
                        newCell = new TableCell(new Paragraph(new Italic(new Run(i.Message)))) { ColumnSpan = i.Length };
                        break;
                    case 3:
                        newCell = new TableCell(new Paragraph(new Italic(new Bold(new Run(i.Message))))) { ColumnSpan = i.Length };
                        break;
                }
                newRow.Cells.Add(newCell);
            }

            return newRow;
        }
    }
}
