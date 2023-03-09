using System.Runtime.Serialization;

namespace ICSStudio.SimpleServices.Common
{
    public enum InitialStateType : byte
    {
        Abort,
        Complete,
        Idle,
        Stopped
    }

    public enum LossOfCommCmdType : byte
    {
        [EnumMember(Value = "<none>")] None,
        Abort,
        Hold,
        Stop
    }

    public enum ExternalRequestActionType : byte
    {
        [EnumMember(Value = "<none>")] None,
        Clear
    }

    public enum CompleteStateIfNotImplType : byte
    {
        NoAction,
        StateComplete,
    }

    public enum ValuesToUseOnStartType : byte
    {
        [EnumMember(Value = "Use Initial Values")]
        UseInitialValues,

        [EnumMember(Value = "Use Current Values")]
        UseCurrentValues,
    }

    public enum ValuesToUseOnResetType : byte
    {
        [EnumMember(Value = "Restore Its Initial Values")]
        RestoreItsInitialValues,

        [EnumMember(Value = "Maintain Current Values")]
        MaintainCurrentValues,
    }

    public class ProgramProperties
    {
        public byte InitialStepIndex = 0;
        public InitialStateType InitialState = 0;
        public CompleteStateIfNotImplType CompleteStateIfNotImpl = 0;
        public LossOfCommCmdType LossOfCommCmd = 0;
        public ExternalRequestActionType ExternalRequestAction;
        public bool UseAsFolder = false;
        public bool AutoValueAssignStepToPhase = false;
        public bool AutoValueAssignPhaseToStepOnComplete = false;
        public bool AutoValueAssignPhaseToStepOnStopped = false;
        public bool AutoValueAssignPhaseToStepOnAborted = false;
        public string Revision;
        public int UnitID = 1;
        public bool RetainSequenceIDOnReset = false;
        public bool GenerateSequenceEvents = false;
        public ValuesToUseOnStartType ValuesToUseOnStart = 0;
        public ValuesToUseOnResetType ValuesToUseOnReset = 0;
        public string RevisionExtension;
        public string RevisionNote;
    }
}
