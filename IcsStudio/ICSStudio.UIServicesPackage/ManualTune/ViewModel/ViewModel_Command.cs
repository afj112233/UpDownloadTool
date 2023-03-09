using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Cip.Other;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Error;
using ICSStudio.UIServicesPackage.ManualTune.Panel;
using ICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionState;
using Microsoft.VisualStudio.Threading;
using MSFViewModel = ICSStudio.UIServicesPackage.ManualTune.Panel.MotionState.MSFViewModel;
using Package = Microsoft.VisualStudio.Shell.Package;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;

namespace ICSStudio.UIServicesPackage.ManualTune.ViewModel
{
    public partial class MotionGeneratorViewModel
    {

        public void ExecuteExecuteCommand()
        {
            if (!_controller.IsOnline) return;
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate { await ExecuteExecuteCommandAsync(); });
        }

        public void ExecuteDisableAxisCommand()
        {
            if (!_controller.IsOnline) return;
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate { await ExecuteDisableCommandAsync(); });
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

            var panel = _activeNode?.OptionPanelDescriptor.OptionPanel as IMotionGeneratorPanel;
            if (panel != null)
                return panel.CanExecute();

            return false;
        }

        private bool CanExecuteDisableCommand()
        {

            if (!_controller.IsOnline)
                return false;

            if (_controller.IsOnline)
            {
                if (_controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch ||
                    _controller.KeySwitchPosition == ControllerKeySwitch.ProgramKeySwitch)
                    return false;
            }

            var panel = _activeNode?.OptionPanelDescriptor.OptionPanel as IMotionGeneratorPanel;
            if (panel != null)
                return panel.CanExecute();

            return false;
        }

        private async Task ExecuteExecuteCommandAsync()
        {
            await TaskScheduler.Default;
            var panel = _activeNode?.OptionPanelDescriptor.OptionPanel as IMotionGeneratorPanel;
            var command = panel.MotionGeneratorCommand;
            var queryCommandRequest = panel.GetQueryCommandRequest();
            var executeCommandRequest = panel.GetExecuteCommandRequest();

            await ExecuteExecuteCommandAsync(queryCommandRequest, executeCommandRequest, command);
        }

        private async Task ExecuteDisableCommandAsync()
        {
            await TaskScheduler.Default;
            var command = MotionGeneratorCommand.MSF;
            //var queryCommandRequest = panel?.GetQueryCommandRequest();
            //var executeCommandRequest = panel?.GetExecuteCommandRequest();

            Controller controller = _controller as Controller;
            if (controller == null)
                return;

            int instanceId = controller.GetTagId(AxisTag);
            var panel = _activeNode?.OptionPanelDescriptor.OptionPanel as IMotionGeneratorPanel;
            //var queryCommandRequest = MotionGeneratorHelper.QueryCommandRequest(
            //(ushort)CipObjectClassCode.MotionGroup, instanceId, command);
            var queryCommandRequest = panel.GetQueryCommandRequest();
            var executeCommandRequest = MotionGeneratorHelper.MSF(instanceId);

            await ExecuteExecuteCommandAsync(queryCommandRequest, executeCommandRequest, command);
        }

        private async Task ExecuteExecuteCommandAsync(
            IMessageRouterRequest queryCommandRequest,
            IMessageRouterRequest executeCommandRequest,
            MotionGeneratorCommand command)
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
                    MessageBox.Show("1");
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

        private void SendRunningLog(MotionGeneratorCommand command, CipMotionInstruction motionInstruction)
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

        private void SendRunningLog(MotionGeneratorCommand command, string result)
        {
            string message = $"{Title}, {command.ToString().PadRight(5)}, {result}";

            Logger.Trace(message);

            var outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
            outputService?.AddMessages(message, null);

            UpdateExecutionError(!result.Contains("No Error"));
        }
    }
}
