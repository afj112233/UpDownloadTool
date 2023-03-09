using System.Threading.Tasks;
using System.Windows;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Error;
using ICSStudio.UIServicesPackage.MotionDirectCommands.Panel;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Package = Microsoft.VisualStudio.Shell.Package;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;

namespace ICSStudio.UIServicesPackage.MotionDirectCommands
{
    internal partial class MotionDirectCommandsViewModel
    {
        private void ExecuteCloseCommand()
        {
            CloseAction?.Invoke();
        }

        private void ExecuteHelpCommand()
        {
            //TODO(gjc): add code here
        }

        private bool CanExecuteMotionGroupShutdownCommand()
        {
            if (!_controller.IsOnline)
                return false;

            if (_controller.IsOnline)
            {
                if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch ||
                    _controller.KeySwitchPosition == ControllerKeySwitch.ProgramKeySwitch)
                    return false;

                if (AllMotionGroups.Count > 0)
                    return true;
            }

            return false;
        }

        private void ExecuteMotionGroupShutdownCommand()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate { await ExecuteMotionGroupShutdownCommandAsync(); });
        }

        private async Task ExecuteMotionGroupShutdownCommandAsync()
        {
            await TaskScheduler.Default;

            if (!CanExecuteMotionGroupShutdownCommand())
                return;

            Controller controller = _controller as Controller;
            if (controller == null)
                return;

            int instanceId = controller.GetTagId(AllMotionGroups[0]);

            var command = MotionDirectCommand.MGSD;
            var queryCommandRequest = MotionDirectCommandHelper.QueryCommandRequest(
                (ushort)CipObjectClassCode.MotionGroup, instanceId, command);
            var executeCommandRequest = MotionDirectCommandHelper.MGSD(instanceId);

            await ExecuteExecuteCommandAsync(queryCommandRequest, executeCommandRequest, command);
        }

        private bool CanExecuteExecuteCommand()
        {

            if (!_controller.IsOnline)
                return false;

            if (_controller.IsOnline)
            {
                if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch ||
                    _controller.KeySwitchPosition == ControllerKeySwitch.ProgramKeySwitch)
                    return false;
            }

            var panel = _activeNode?.OptionPanelDescriptor.OptionPanel as IMotionDirectCommandPanel;
            if (panel != null)
                return panel.CanExecute();

            return false;
        }

        private void ExecuteExecuteCommand()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate { await ExecuteExecuteCommandAsync(); });
        }

        private async Task ExecuteExecuteCommandAsync()
        {
            await TaskScheduler.Default;

            var panel = _activeNode?.OptionPanelDescriptor.OptionPanel as IMotionDirectCommandPanel;
            if (panel == null)
                return;

            var command = panel.MotionDirectCommand;
            var queryCommandRequest = panel.GetQueryCommandRequest();
            var executeCommandRequest = panel.GetExecuteCommandRequest();

            await ExecuteExecuteCommandAsync(queryCommandRequest, executeCommandRequest, command);
        }

        private void ExecuteAxisPropertiesCommand(ITag axis)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (axis != null)
            {
                ICreateDialogService createDialogService =
                    Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;

                var uiShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));

                if (createDialogService != null && uiShell != null)
                {
                    var dataType = axis.DataTypeInfo.DataType;
                    Window window = null;
                    if (dataType is AXIS_CIP_DRIVE)
                    {
                        window =
                            createDialogService.CreateAxisCIPDriveProperties(axis);
                    }
                    else if (dataType is AXIS_VIRTUAL)
                    {
                        window = createDialogService.CreateAxisVirtualProperties(axis);
                    }

                    window?.Show(uiShell);
                }
            }
        }

        private void ExecuteMotionGroupPropertiesCommand(ITag motionGroup)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (motionGroup != null)
            {
                ICreateDialogService createDialogService =
                    Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;

                var uiShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));

                if (createDialogService != null && uiShell != null)
                {
                    var window =
                        createDialogService.CreateMotionGroupProperties(motionGroup);
                    window.Show(uiShell);
                }
            }
        }

        private async Task ExecuteExecuteCommandAsync(
            IMessageRouterRequest queryCommandRequest,
            IMessageRouterRequest executeCommandRequest,
            MotionDirectCommand command)
        {
            if (!_controller.IsOnline)
                return;

            Controller controller = _controller as Controller;
            var messager = controller?.CipMessager;
            if (queryCommandRequest != null && executeCommandRequest != null && messager != null)
            {
                // query
                var motionInstruction = await SendMotionDirectCommandAsync(messager, queryCommandRequest);
                if (motionInstruction != null)
                {
                    // execute
                    motionInstruction = await SendMotionDirectCommandAsync(messager, executeCommandRequest);
                    if (motionInstruction != null)
                    {
                        if (motionInstruction.ER || motionInstruction.DN)
                        {
                            SendRunningLog(command, motionInstruction);
                        }
                        else
                        {
                            while (true)
                            {
                                // query
                                motionInstruction = await SendMotionDirectCommandAsync(messager, queryCommandRequest);
                                if (motionInstruction != null)
                                {
                                    if (motionInstruction.ER || motionInstruction.DN)
                                    {
                                        SendRunningLog(command, motionInstruction);
                                        break;
                                    }
                                }
                                else
                                {
                                    SendRunningLog(command, "Execute failed!");
                                    break;
                                }
                            }
                        }

                    }
                    else
                    {
                        SendRunningLog(command, "Execute failed!");
                    }
                }
                else
                {
                    SendRunningLog(command, "Execute failed!");
                }

            }
        }

        private async Task<CipMotionInstruction> SendMotionDirectCommandAsync(ICipMessager messager,
            IMessageRouterRequest request)
        {
            if (messager == null || request == null)
                return null;

            var motionInstruction = new CipMotionInstruction();

            var response = await messager.SendUnitData(request);
            if ((response != null) && (response.GeneralStatus == (byte)CipGeneralStatusCode.Success))
            {
                if (motionInstruction.Parse(response.ResponseData) == 0)
                {
                    return motionInstruction;
                }

                Logger.Error("motion instruction parse failed!" + response.ResponseData);

                return null;
            }

            Logger.Error("SendMotionDirectCommand failed!");

            return null;
        }

        private void SendRunningLog(MotionDirectCommand command, CipMotionInstruction motionInstruction)
        {
            var errorCode = CommandResultDescriptor.GetErrorCodeString(motionInstruction.ErrorCode);
            // ReSharper disable once UnusedVariable
            var extendedErrorCode =
                CommandResultDescriptor.GetExtendedErrorCodeString(
                    motionInstruction.ExtendedErrorCode);

            string result =
                $"(16#{motionInstruction.ErrorCode:x04}), {errorCode}";

            SendRunningLog(command, result);
        }

        private void SendRunningLog(MotionDirectCommand command, string result)
        {
            string message = $"{Title}, {command.ToString().PadRight(5)}, {result}";

            Logger.Trace(message);

            var outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
            outputService?.AddMessages(message, null);

            UpdateExecutionError(!result.Contains("No Error"));
        }
    }
}
