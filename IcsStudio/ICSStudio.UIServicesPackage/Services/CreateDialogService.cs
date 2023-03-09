using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using GalaSoft.MvvmLight;
using ICSStudio.Dialogs.NewTag;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIServicesPackage.AddOnInstruction;
using ICSStudio.UIServicesPackage.AxisCIPDriveProperties;
using ICSStudio.UIServicesPackage.AxisVirtualProperties;
using ICSStudio.UIServicesPackage.ManualTune;
using ICSStudio.UIServicesPackage.MotionDirectCommands;
using ICSStudio.UIServicesPackage.MotionGroupProperties;
using ICSStudio.UIServicesPackage.PLCProperties;
using ICSStudio.UIServicesPackage.ProgramProperties;
using ICSStudio.UIServicesPackage.RoutineProperties;
using ICSStudio.UIServicesPackage.RSTrendXProperties;
using ICSStudio.UIServicesPackage.SelectModuleType;
using ICSStudio.UIServicesPackage.TagProperties;
using ICSStudio.UIServicesPackage.TaskProperties;
using ICSStudio.UIServicesPackage.View;
using ICSStudio.UIServicesPackage.ViewModel;
using Microsoft.VisualStudio.Shell;
using NewTagDialog = ICSStudio.Dialogs.NewTag.NewTagDialog;

namespace ICSStudio.UIServicesPackage.Services
{
    public class CreateDialogService : ICreateDialogService, SCreateDialogService
    {
        private readonly Package _package;
        private readonly List<Tuple<string, Window>> _openedWindows;
        private int _motionDirectCommandsIndex = 1;

        public CreateDialogService(Package package)
        {
            _package = package;
            _openedWindows = new List<Tuple<string, Window>>();
        }

        // ReSharper disable once UnusedMember.Local
        private IServiceProvider ServiceProvider => _package;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CloseAllDialogs()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var allDialogs = _openedWindows.Select(x => x.Item2).ToList();
                foreach (var dialog in allDialogs)
                {
                    dialog.Close();
                }

                _motionDirectCommandsIndex = 1;
            });
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int ApplyAllDialogs()
        {
            var allDialogs = _openedWindows.Select(x => x.Item2).ToList();
            foreach (var dialog in allDialogs)
            {
                ICanApply canApply = dialog.DataContext as ICanApply;

                if (canApply != null)
                {
                    int result = canApply.Apply();
                    if (result < 0)
                    {
                        dialog.Activate();
                        return result;
                    }
                }
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CloseDialog(List<string> windowId)
        {
            if (windowId == null) return;
            var allDialogs = _openedWindows.Select(x =>
            {
                if (windowId.Contains(x.Item1)) return x.Item2;
                return null;
            }).ToList();
            foreach (var dialog in allDialogs)
            {
                dialog?.Close();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateNewTaskDialog()
        {
            return new NewTaskDialog();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateNewTagDialog(IDataType dataType,
            ITagCollection parentCollection,
            Usage usage = Usage.NullParameterType,
            ITag assignedGroup = null, string name = "")
        {
            var targetDataType = "";
            var expectDataType = dataType as ExpectType;
            if (expectDataType != null)
            {
                targetDataType = expectDataType.ExpectTypes.Contains(DINT.Inst)
                    ? DINT.Inst.Name
                    : expectDataType.ExpectTypes[0].Name;
            }
            else
            {
                if (dataType is ExceptType)
                    targetDataType = "DINT";
                else
                {
                    targetDataType = dataType?.Name;
                    var arrayNormal = dataType as ArrayTypeNormal;
                    if (arrayNormal != null)
                        targetDataType = arrayNormal.Type.Name;

                    var arrayOne = dataType as ArrayTypeDimOne;
                    if (arrayOne != null)
                        targetDataType = arrayOne.type.Name;

                    var array = dataType as ArrayType;
                    if (array != null)
                    {
                        targetDataType = array.ToString();
                    }
                }
            }

            var viewModel = new NewTagViewModel(targetDataType, parentCollection, usage, assignedGroup, name);
            return new NewTagDialog(viewModel);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateNewAoiTagDialog(IDataType dataType,
            IAoiDefinition aoiDefinition,
            Usage usage, bool canRefreshDataType, string name = "")
        {
            var targetDataType = "";
            var expectDataType = dataType as ExpectType;
            if (expectDataType != null)
            {
                targetDataType = expectDataType.ExpectTypes[0].Name;
            }
            else
            {
                if (dataType is ExceptType)
                    targetDataType = "Dint";
                else
                    targetDataType = dataType?.Name;
            }

            var viewModel = new NewAoiTagViewModel(targetDataType, aoiDefinition, usage, canRefreshDataType, name);
            return new NewAoiTagDialog(viewModel);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateRoutineDialog(IProgramModule program, string name = default(string))
        {
            return new NewRoutineDialog(program,name);
        }

        public Window CreatePhaseStateDialog(IProgramModule program)
        {
            return new NewPhaseStateDialog(program);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateSelectModuleTypeDialog(IController controller, IDeviceModule parentModule,
            PortType portType)
        {
            return new SelectModuleTypeDialog(controller, parentModule, portType);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateMotionGroupProperties(ITag motionGroup)
        {
            string windowId = $"MotionGroupProperties{motionGroup.Uid}";

            var result = _openedWindows.Find(x => x.Item1.Equals(windowId));
            if (result != null)
            {
                return result.Item2;
            }

            var viewModel = new MotionGroupPropertiesViewModel(motionGroup);
            TabbedOptionsDialog dialog = new TabbedOptionsDialog(viewModel);
            viewModel.CloseAction = dialog.Close;

            dialog.Closed += WindowClosed;
            _openedWindows.Add(new Tuple<string, Window>(windowId, dialog));
            dialog.Width = 480;
            return dialog;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateAxisCIPDriveProperties(ITag axisTag)
        {
            string windowId = $"AxisCIPDriveProperties{axisTag.Uid}";

            var result = _openedWindows.Find(x => x.Item1.Equals(windowId));
            if (result != null)
            {
                return result.Item2;
            }

            var viewModel = new AxisCIPDrivePropertiesViewModel(axisTag);
            var dialog = new AxisCIPDrivePropertiesDialog { DataContext = viewModel };
            viewModel.CloseAction = dialog.Close;

            dialog.Closed += WindowClosed;
            _openedWindows.Add(new Tuple<string, Window>(windowId, dialog));

            return dialog;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateAxisScheduleDialog(ITag motionGroup)
        {
            string windowId = $"AxisSchedule{motionGroup.Uid}";
            var result = _openedWindows.Find(x => x.Item1.Equals(windowId));
            if (result != null)
            {
                return result.Item2;
            }

            var viewModel = new AxisScheduleViewModel(motionGroup);
            var dialog = new AxisScheduleDialog { DataContext = viewModel };
            viewModel.CloseAction = dialog.Close;

            dialog.Closed += WindowClosed;
            _openedWindows.Add(new Tuple<string, Window>(windowId, dialog));

            return dialog;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateMotionDirectCommandsDialog(ITag motion)
        {
            var viewModel = new MotionDirectCommandsViewModel(motion, _motionDirectCommandsIndex);
            var dialog = new MotionDirectCommandsDialog { DataContext = viewModel };
            viewModel.CloseAction = dialog.Close;

            dialog.Closed += WindowClosed;
            var windowId = $"MotionDirectCommands:{_motionDirectCommandsIndex}";
            _openedWindows.Add(new Tuple<string, Window>(windowId, dialog));

            _motionDirectCommandsIndex++;
            return dialog;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateManualTuneDialog(ITag axisTag)
        {
            string windowId = $"ManualTune{axisTag.Uid}";

            var result = _openedWindows.Find(x => x.Item1.Equals(windowId));
            if (result != null)
                return result.Item2;

            var viewModel = new ManualTuneViewModel(axisTag);
            var dialog = new ManualTuneDialog { DataContext = viewModel };
            viewModel.CloseAction = dialog.Close;

            dialog.Closed += WindowClosed;
            _openedWindows.Add(new Tuple<string, Window>(windowId, dialog));

            return dialog;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateTaskProperties(ITask task)
        {
            string windowId = $"TaskProperties{task.Uid}";

            var result = _openedWindows.Find(x => x.Item1.Equals(windowId));
            if (result != null)
            {
                return result.Item2;
            }

            var viewModel = new TaskPropertiesViewModel(task);
            TabbedOptionsDialog dialog = new TabbedOptionsDialog(viewModel);
            viewModel.CloseAction = dialog.Close;

            dialog.Closed += WindowClosed;
            _openedWindows.Add(new Tuple<string, Window>(windowId, dialog));

            return dialog;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateProgramProperties(IProgram program, bool isShowParameterTab)
        {
            string windowId = $"ProgramProperties{program.Uid}";

            var result = _openedWindows.Find(x => x.Item1.Equals(windowId));
            if (result != null)
            {
                return result.Item2;
            }

            var viewModel = new ProgramPropertiesViewModel(program, isShowParameterTab);
            TabbedOptionsDialog dialog = new TabbedOptionsDialog(viewModel);
            dialog.Width = 615;
            dialog.Height = 430;
            if (program.Type == ProgramType.Phase)
            {
                dialog.MinHeight = 510;
                dialog.MinWidth = 550;
            }
            else if (program.Type == ProgramType.Sequence)
            {
                dialog.MinHeight = 470;
                dialog.MinWidth = 530;
            }

            viewModel.CloseAction = dialog.Close;

            dialog.Closed += WindowClosed;
            dialog.ResizeMode = ResizeMode.CanResizeWithGrip;
            _openedWindows.Add(new Tuple<string, Window>(windowId, dialog));

            return dialog;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateAxisVirtualProperties(ITag axisTag)
        {
            string windowId = $"AxisVirtualProperties{axisTag.Uid}";

            var result = _openedWindows.Find(x => x.Item1.Equals(windowId));
            if (result != null)
            {
                return result.Item2;
            }

            var viewModel = new AxisVirtualPropertiesViewModel(axisTag);
            TabbedOptionsDialog dialog = new TabbedOptionsDialog(viewModel);
            dialog.Width = 680;
            dialog.Height = 380;
            viewModel.CloseAction = dialog.Close;

            dialog.Closed += WindowClosed;
            _openedWindows.Add(new Tuple<string, Window>(windowId, dialog));

            return dialog;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateManualAdjustDialog(ITag tag, string positionUnits)
        {
            string windowId = $"Manual Adjust{tag.Uid}";

            var result = _openedWindows.Find(x => x.Item1.Equals(windowId));
            if (result != null)
            {
                return result.Item2;
            }

            var viewModel = new ManualAdjustVM(tag, positionUnits);
            TabbedOptionsDialog dialog = new TabbedOptionsDialog(viewModel);
            viewModel.CloseAction = dialog.Close;
            dialog.Width = 500;
            dialog.Closed += WindowClosed;

            _openedWindows.Add(new Tuple<string, Window>(windowId, dialog));

            return dialog;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateRoutineProperties(IRoutine routine)
        {
            string windowId = $"RoutineProperties{routine.Uid}";

            var result = _openedWindows.Find(x => x.Item1.Equals(windowId));
            if (result != null)
            {
                return result.Item2;
            }

            var viewModel = new RoutinePropertiesVM(routine);
            TabbedOptionsDialog dialog = new TabbedOptionsDialog(viewModel);

            viewModel.CloseAction = dialog.Close;
            dialog.Width = 400;
            dialog.Height = 300;
            dialog.Closed += WindowClosed;
            _openedWindows.Add(new Tuple<string, Window>(windowId, dialog));

            return dialog;
        }

        public Window CreateAddOnInstruction()
        {
            return new NewAddOnInstructionDialog();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window AddOnInstructionProperties(IAoiDefinition aoiDefinition)
        {
            string windowId = $"AddOnInstruction{aoiDefinition.Uid}";

            var result = _openedWindows.Find(x => x.Item1.Equals(windowId));
            if (result != null)
            {
                return result.Item2;
            }

            var viewModel = new AddOnInstructionVM(aoiDefinition);
            AddOnInstructionOptionsDialog dialog = new AddOnInstructionOptionsDialog(viewModel);
            viewModel.CloseAction = dialog.Close;
            dialog.MinWidth = 600;
            dialog.MinHeight = 500;
            dialog.Closed += WindowClosed;

            _openedWindows.Add(new Tuple<string, Window>(windowId, dialog));

            return dialog;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Window CreateTagProperties(ITag tag)
        {
            string windowId = $"TagProperties{tag.Uid}";

            var result = _openedWindows.Find(x => x.Item1.Equals(windowId));
            if (result != null)
            {
                return result.Item2;
            }

            var viewModel = new TagPropertiesViewModel(tag);
            TabbedOptionsDialog dialog = new TabbedOptionsDialog(viewModel);
            viewModel.CloseAction = dialog.Close;
            dialog.MinWidth = 410;
            dialog.MinHeight = 570;
            dialog.Closed += WindowClosed;

            _openedWindows.Add(new Tuple<string, Window>(windowId, dialog));

            return dialog;
        }

        public Window CreateControllerProperties(IController controller)
        {
            string windowId = $"ControllerProperties";

            var result = _openedWindows.Find(x => x.Item1.Equals(windowId));
            if (result != null) return result.Item2;

            var viewModel = new PLCPropertiesViewModel(controller);
            TabbedOptionsDialog dialog = new TabbedOptionsDialog(viewModel);
            viewModel.CloseAction = dialog.Close;
            dialog.Width = 630;
            dialog.MinHeight = 580;
            dialog.Closed += WindowClosed;
            _openedWindows.Add(new Tuple<string, Window>(windowId, dialog));

            return dialog;
        }

        public Window CreateControllerProperties(IController controller, int selectedIndex)
        {
            string windowId = $"ControllerProperties";

            var result = _openedWindows.Find(x => x.Item1.Equals(windowId));

            TabbedOptionsDialog dialog = result?.Item2 as TabbedOptionsDialog;

            PLCPropertiesViewModel viewModel = dialog?.DataContext as PLCPropertiesViewModel;

            if (viewModel == null)
            {
                viewModel = new PLCPropertiesViewModel(controller);
                dialog = new TabbedOptionsDialog(viewModel);
                viewModel.CloseAction = dialog.Close;
                dialog.Width = 630;
                dialog.MinHeight = 580;
                dialog.Closed += WindowClosed;
                _openedWindows.Add(new Tuple<string, Window>(windowId, dialog));
            }
            
            int count = viewModel.TabbedOptions.Items.Count;
            if (selectedIndex >= 0 && selectedIndex < count)
            {
                viewModel.TabbedOptions.SelectedIndex = selectedIndex;
            }
            
            return dialog;
        }

        public Window CreateNewTrendDialog(IController controller)
        {
            var dialog = new NewTrend();
            var vm = new NewTrendViewModel(dialog, controller);
            dialog.DataContext = vm;
            return dialog;
        }

        public Window CreateRSTrendXProperties(ITrend trend, int kind)
        {
            var vm = new RSTrendXPropertiesVM((TrendObject)trend, kind);
            var dialog = new TabbedOptionsDialog(vm);
            dialog.Width = 742;
            dialog.Height = 644;
            dialog.Closed += WindowClosed;
            vm.CloseAction = dialog.Close;
            return dialog;
        }

        public Window CreateGraphTitle(ITrend trend)
        {
            var dialog = new GraphTitle();
            var vm = new GraphTitleViewModel(trend);
            dialog.DataContext = vm;
            return dialog;
        }

        public Window CreateProgramDialog(ProgramType type, ITask task)
        {
            return new NewProgramDialog(type, task);
        }

        public List<Window> GetAllWindows()
        {
            return _openedWindows.Select(p => p.Item2).ToList();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            Window window = sender as Window;
            if (window != null)
            {
                var result = _openedWindows.Find(x => x.Item2 == window);
                if (result != null)
                {
                    _openedWindows.Remove(result);
                }

                // ReSharper disable once UseObjectOrCollectionInitializer
                var helper = new WindowInteropHelper(window);
                helper.Owner = IntPtr.Zero;

                // cleanup
                var viewModel = window.DataContext as ViewModelBase;
                if (viewModel != null)
                {
                    viewModel.Cleanup();
                }

                // activate
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow != null && !mainWindow.IsActive)
                {
                    mainWindow.Activate();
                }
            }

        }
    }
}
