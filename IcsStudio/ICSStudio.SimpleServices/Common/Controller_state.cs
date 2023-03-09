using System;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.SimpleServices.Common
{
    public partial class Controller
    {
        private ControllerOperationMode _operationMode;
        private readonly object _operationModeLock = new object();

        private ControllerKeySwitch _keySwitchPosition;
        private readonly object _keySwitchPositionLock = new object();

        public ControllerOperationMode OperationMode
        {
            get
            {
                lock (_operationModeLock)
                {
                    return _operationMode;
                }
            }
            set
            {
                lock (_operationModeLock)
                {
                    if (_operationMode == value)
                        return;

                    _operationMode = value;

                    OperationModeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public ControllerKeySwitch KeySwitchPosition
        {
            get
            {
                lock (_keySwitchPositionLock)
                {
                    return _keySwitchPosition;
                }
            }
            internal set
            {
                lock (_keySwitchPositionLock)
                {
                    if (_keySwitchPosition == value)
                        return;

                    _keySwitchPosition = value;

                    KeySwitchChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public ControllerLockState LockState { get; }


        public event EventHandler KeySwitchChanged;
        public event EventHandler OperationModeChanged;


        public async Task UpdateState()
        {
            await _stateManager.Update();
        }

        public async Task ClearFaults()
        {
            await _stateManager.ClearFaults();
        }

        public async Task ChangeOperationMode(ControllerOperationMode mode)
        {
            if (mode == ControllerOperationMode.OperationModeFaulted ||
                mode == ControllerOperationMode.OperationModeNull)
                throw new ArgumentException(@"Not support mode", nameof(mode));

            await _stateManager.ChangeOperationMode(mode);
        }
    }
}
