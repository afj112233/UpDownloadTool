using System;
using System.Threading.Tasks;
using System.Timers;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using NLog;

namespace ICSStudio.SimpleServices.Common
{
    internal class ControllerStateManager : IDisposable
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private Timer _stateRefreshTimer;

        public ControllerStateManager(Controller controller)
        {
            Controller = controller;

            _stateRefreshTimer = new Timer(500);
            _stateRefreshTimer.Elapsed += OnElapsed;
            _stateRefreshTimer.AutoReset = false;
        }

        public Controller Controller { get; }

        public void Dispose()
        {
            _stateRefreshTimer.Stop();
            _stateRefreshTimer.Elapsed -= OnElapsed;

            _stateRefreshTimer = null;
        }

        public async Task Update()
        {
            await UpdateState();

            _stateRefreshTimer.Start();
        }

        public async Task ClearFaults()
        {
            if (!Controller.IsConnected)
                return;

            if (Controller.OperationMode != ControllerOperationMode.OperationModeFaulted)
                return;

            try
            {
                CIPController cipController = new CIPController(0, Controller.CipMessager);

                int result = await cipController.WriterLockRetry();
                if (result == 0)
                {
                    await cipController.ClearFaults();
                    await cipController.WriterUnLock();
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }

        public async Task ChangeOperationMode(ControllerOperationMode mode)
        {
            if (!Controller.IsConnected)
                return;

            if (Controller.KeySwitchPosition != ControllerKeySwitch.RemoteKeySwitch)
                return;

            var oldMode = Controller.OperationMode;
            if (oldMode == ControllerOperationMode.OperationModeNull
                || oldMode == ControllerOperationMode.OperationModeFaulted
                || oldMode == mode)
                return;

            try
            {
                CIPController cipController = new CIPController(0, Controller.CipMessager);

                int state;
                // RUN=0,PROGRAM=1,TEST=2,FAULT=3
                switch (mode)
                {
                    case ControllerOperationMode.OperationModeRun:
                        state = 0;
                        break;
                    case ControllerOperationMode.OperationModeProgram:
                        state = 1;
                        break;
                    case ControllerOperationMode.OperationModeDebug:
                        state = 2;
                        break;
                    default:
                        state = 1;
                        break;
                }

                //Debug.WriteLine($"Set state:{state}");

                int result = await cipController.WriterLockRetry();
                if (result == 0)
                {
                    await cipController.SetState(state);
                    await cipController.WriterUnLock();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private async void OnElapsed(object sender, ElapsedEventArgs e)
        {
            await UpdateState();

            if (Controller.IsConnected)
                _stateRefreshTimer.Start();
            else
            {
                Controller.KeySwitchPosition = ControllerKeySwitch.NullKeySwitch;
                Controller.OperationMode = ControllerOperationMode.OperationModeNull;
            }
        }

        private async Task UpdateState()
        {
            try
            {
                if (Controller.IsConnected)
                {
                    CIPController cipController = new CIPController(0, Controller.CipMessager);
                    CtrlStatus ctrlStatus = await cipController.GetStatus();

                    //Debug.WriteLine($"Mode:{ctrlStatus.Mode}, State:{ctrlStatus.State}");

                    // PROGRAM=0,REM=1,RUN=2
                    switch (ctrlStatus.Mode)
                    {
                        case 0:
                            Controller.KeySwitchPosition = ControllerKeySwitch.ProgramKeySwitch;
                            break;
                        case 1:
                            Controller.KeySwitchPosition = ControllerKeySwitch.RemoteKeySwitch;
                            break;
                        case 2:
                            Controller.KeySwitchPosition = ControllerKeySwitch.RunKeySwitch;
                            break;
                        default:
                            Controller.KeySwitchPosition = ControllerKeySwitch.NullKeySwitch;
                            break;
                    }

                    // RUN=0,PROGRAM=1,TEST=2,FAULT=3
                    switch (ctrlStatus.State)
                    {
                        case 0:
                            Controller.OperationMode = ControllerOperationMode.OperationModeRun;
                            break;
                        case 1:
                            Controller.OperationMode = ControllerOperationMode.OperationModeProgram;
                            break;
                        case 2:
                            Controller.OperationMode = ControllerOperationMode.OperationModeDebug;
                            break;
                        case 3:
                            Controller.OperationMode = ControllerOperationMode.OperationModeFaulted;
                            break;
                        default:
                            Controller.OperationMode = ControllerOperationMode.OperationModeNull;
                            break;
                    }
                }
                else
                {
                    Controller.KeySwitchPosition = ControllerKeySwitch.NullKeySwitch;
                    Controller.OperationMode = ControllerOperationMode.OperationModeNull;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

    }
}
