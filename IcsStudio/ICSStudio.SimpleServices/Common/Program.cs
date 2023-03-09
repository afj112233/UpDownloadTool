using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.Utils;
using Newtonsoft.Json.Linq;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.Online;

namespace ICSStudio.SimpleServices.Common
{
    public class Program : ProgramModule, IProgram
    {
        private readonly ProgramCollection _childCollection;
        private ITask _parentTask;
        private TestEditsModeType _testEditsMode;
        private bool _updateAllRoutine;
        private bool _executeFinalizeCode;

        private bool _inhibited;
        private long _maxScanTime;
        private long _lastScanTime;
        private bool _updateRoutineRunStatus;

        public Program(IController controller) : base(controller)
        {
            _childCollection = new ProgramCollection(controller);
            ProgramProperties = new ProgramProperties();
        }

        public IProgramCollection ParentCollection { get; set; }

        public IProgramCollection ChildCollection => _childCollection;

        public ITask ParentTask
        {
            get { return _parentTask; }
            set
            {
                if (_parentTask != value)
                {
                    _parentTask = value;
                    RaisePropertyChanged();
                }
            }
        }

        public override bool IsDeleted
        {
            get
            {
                if (ParentCollection == null)
                    return true;

                return false;
            }
        }

        public TestEditsModeType TestEditsMode
        {
            get { return _testEditsMode; }
            set
            {
                if (_testEditsMode != value)
                {
                    _testEditsMode = value;
                    if (_testEditsMode == TestEditsModeType.Null && HasTest)
                    {
                        _testEditsMode = TestEditsModeType.UnTest;
                    }

                    RaisePropertyChanged();
                }
            }
        }

        public bool HasPending => Routines.FirstOrDefault(r => r.PendingEditsExist) != null;

        public bool HasTest => Routines.FirstOrDefault(r => (r as STRoutine)?.TestCodeText?.Count > 0) != null;

        public void Override(JObject config, IProgramCollection parentCollection)
        {
            JObjectExtensions jsonProgram = new JObjectExtensions(config);

            try
            {
                if(Name != (string)jsonProgram["Name"])
                    Name = (string)jsonProgram["Name"];
                //maybe some place monitor the "Name", and generate errors
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            Inhibited = (bool) jsonProgram["Inhibited"];

            string mainRoutineName = (string) jsonProgram["MainRoutineName"];
            string faultRoutineName = (string) jsonProgram["FaultRoutineName"];

            if (jsonProgram["Description"] != null)
            {
                Description = (string) jsonProgram["Description"];
            }

            if (jsonProgram["Type"] != null)
            {
                Type = (ProgramType) ((int) jsonProgram["Type"]);
            }
            else
            {
                Type = ProgramType.Normal;
            }

            if (jsonProgram["InitialStepIndex"] != null)
            {
                ProgramProperties.InitialStepIndex = (byte) jsonProgram["InitialStepIndex"];
            }

            if (jsonProgram["InitialState"] != null)
            {
                ProgramProperties.InitialState = (InitialStateType) (int) jsonProgram["InitialState"];
            }

            if (jsonProgram["CompleteStateIfNotImpl"] != null)
            {
                ProgramProperties.CompleteStateIfNotImpl =
                    (CompleteStateIfNotImplType) (int) jsonProgram["CompleteStateIfNotImpl"];
            }

            if (jsonProgram["LossOfCommCmd"] != null)
            {
                ProgramProperties.LossOfCommCmd = (LossOfCommCmdType) (int) jsonProgram["LossOfCommCmd"];
            }

            if (jsonProgram["ExternalRequestAction"] != null)
            {
                ProgramProperties.ExternalRequestAction =
                    (ExternalRequestActionType) (int) jsonProgram["ExternalRequestAction"];
            }

            if (jsonProgram["UseAsFolder"] != null)
            {
                ProgramProperties.UseAsFolder = (bool) jsonProgram["UseAsFolder"];
            }

            if (jsonProgram["AutoValueAssignStepToPhase"] != null)
            {
                ProgramProperties.AutoValueAssignStepToPhase = (bool) jsonProgram["AutoValueAssignStepToPhase"];
            }

            if (jsonProgram["AutoValueAssignPhaseToStepOnComplete"] != null)
            {
                ProgramProperties.AutoValueAssignPhaseToStepOnComplete =
                    (bool) jsonProgram["AutoValueAssignPhaseToStepOnComplete"];
            }

            if (jsonProgram["AutoValueAssignPhaseToStepOnStopped"] != null)
            {
                ProgramProperties.AutoValueAssignPhaseToStepOnStopped =
                    (bool) jsonProgram["AutoValueAssignPhaseToStepOnStopped"];
            }

            if (jsonProgram["AutoValueAssignPhaseToStepOnAborted"] != null)
            {
                ProgramProperties.AutoValueAssignPhaseToStepOnAborted =
                    (bool) jsonProgram["AutoValueAssignPhaseToStepOnAborted"];
            }

            var controller = Controller.GetInstance();

            // Routines
            ((RoutineCollection) Routines).RemoveAll();
            foreach (var routine in jsonProgram["Routines"])
            {
                AddRoutine(controller.CreateRoutine(routine as JObject));
            }

            foreach (var routine in Routines)
            {
                if (string.Equals(mainRoutineName, routine.Name, StringComparison.OrdinalIgnoreCase))
                    routine.IsMainRoutine = true;

                if (string.Equals(faultRoutineName, routine.Name, StringComparison.OrdinalIgnoreCase))
                    routine.IsFaultRoutine = true;
            }

            // Tags
            ((TagCollection) Tags).Clear();
            foreach (var tag in jsonProgram["Tags"])
            {
                AddTag(tag);
            }

            if (!parentCollection.Contains(this))
                ((ProgramCollection) parentCollection).AddProgram(this);
            CheckTestStatus();
        }

        public void CheckTestStatus()
        {
            if (HasTest)
            {
                if (TestEditsMode == TestEditsModeType.Null) TestEditsMode = TestEditsModeType.UnTest;
            }
            else
            {
                TestEditsMode = TestEditsModeType.Null;
            }
        }

        public void CheckPendingStatus()
        {
            RaisePropertyChanged("HasPending");
        }

        public void CancelPending()
        {
            foreach (var routine in Routines)
            {
                var stRoutine = routine as STRoutine;
                stRoutine?.CancelPending();
                //TODO(ZYL):Add other routines
            }

            RaisePropertyChanged("HasPending");
        }

        public void CancelAccepted()
        {
            foreach (var routine in Routines)
            {
                var stRoutine = routine as STRoutine;
                stRoutine?.CancelTest();
                //TODO(ZYL):Add other routines
            }

            TestEditsMode = TestEditsModeType.Null;
        }

        public void AssembledAccepted()
        {
            foreach (var routine in Routines)
            {
                var stRoutine = routine as STRoutine;
                if (stRoutine != null && stRoutine.TestCodeText.Count > 0)
                {
                    var tmp = new string[stRoutine.TestCodeText.Count];
                    stRoutine.TestCodeText.CopyTo(tmp);
                    stRoutine.CodeText = tmp.ToList();
                    stRoutine.TestCodeText.Clear();
                    //TODO(ZYL):Add other routines
                }

                TestEditsMode = TestEditsModeType.Null;
            }
        }

        public bool ExecuteFinalizeCode
        {
            set
            {
                _executeFinalizeCode = value;
                //if (_executeFinalizeCode)
                //{
                //    FinalizeRoutineCode();
                //}
                RaisePropertyChanged();
            }
            get { return _executeFinalizeCode; }
        }

        //private async void FinalizeRoutineCode()
        //{
        //    var stopwatch = new Stopwatch();
        //    stopwatch.Start();
        //    var ctrl = (Controller) ParentController;
        //    OnlineEditHelper onlineEditHelper=new OnlineEditHelper(ctrl.CipMessager);
        //    foreach (var routine in Routines)
        //    {
        //        var stRoutine = routine as STRoutine;
        //        if (stRoutine != null)
        //        {
        //            if (stRoutine.TestCodeText!=null)
        //            {
        //               stRoutine.ApplyTest();
        //                continue;
        //            }

        //            if (stRoutine.PendingEditsExist)
        //            {
        //                stRoutine.ApplyPending();
        //            }

        //            //TODO(ZYL):Add other routines
        //        }
        //    }
        //    if (!Routines.Any(r => r.IsError))
        //    {
        //        foreach (var routine in Routines)
        //        {
        //            var st = routine as STRoutine;
        //            if (st != null)
        //                await onlineEditHelper.ReplaceRoutine(st);
        //        }
        //        await onlineEditHelper.UpdateProgram(this);
        //        if (TestEditsMode == TestEditsModeType.Null)
        //            RaisePropertyChanged("TestEditsMode");
        //        else
        //            TestEditsMode = TestEditsModeType.Null;
        //    }
        //    stopwatch.Stop();
        //    Controller.GetInstance().Log($"download end :{stopwatch.ElapsedMilliseconds}");
        //}

        public async Task AcceptPending()
        {
            var onlineEditHelper = new OnlineEditHelper(((Controller) ParentController).CipMessager);
            if (ParentController.OperationMode == ControllerOperationMode.OperationModeProgram)
            {
                foreach (var routine in Routines)
                {
                    if(routine.IsError)continue;
                    var st = routine as STRoutine;
                    if (st?.PendingCodeText!=null)
                    {
                        var tmp = new string[st.PendingCodeText.Count];
                        st.PendingCodeText.CopyTo(tmp);
                        st.CodeText = tmp.ToList();
                        st.PendingCodeText = null;
                        st.TestCodeText = null;
                        await onlineEditHelper.ReplaceRoutine(st);
                    }
                }
            }
            else
            {
                foreach (var routine in Routines)
                {
                    if (routine.IsError) continue;
                    var st = routine as STRoutine;
                    if (st?.PendingCodeText != null)
                    {
                        var tmp = new string[st.PendingCodeText.Count];
                        st.PendingCodeText.CopyTo(tmp);
                        st.TestCodeText = tmp.ToList();
                        st.PendingCodeText = null;
                        await onlineEditHelper.ReplaceRoutine(st);
                    }
                }
            }

            CheckPendingStatus();
            UpdateAllRoutine = true;
        }

        public bool UpdateAllRoutine
        {
            set
            {
                _updateAllRoutine = value;
                RaisePropertyChanged();
            }
            get { return _updateAllRoutine; }
        }

        public bool Inhibited
        {
            get { return _inhibited; }
            set
            {
                if (_inhibited != value)
                {
                    _inhibited = value;
                    RaisePropertyChanged();
                    UpdateRoutineRunStatus = true;
                }
            }
        }

        public bool CanSchedule { get; set; }
        public bool SynchronizeData { get; set; }

        public long MaxScanTime
        {
            get { return _maxScanTime; }
            set
            {
                if (_maxScanTime != value)
                {
                    _maxScanTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        public long LastScanTime
        {
            get { return _lastScanTime; }
            set
            {
                if (_lastScanTime != value)
                {
                    _lastScanTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool UpdateRoutineRunStatus
        {
            set
            {
                _updateRoutineRunStatus = value; 
                RaisePropertyChanged();
            }
            get { return _updateRoutineRunStatus; }
        }

        public IProgram AlternateParentComponent { get; set; }
        public ProgramProperties ProgramProperties { get; set; }

        public void ResetMaxScanTime()
        {
            try
            {
                Controller controller = ParentController as Controller;
                controller?.ResetMaxScanTime(this).GetAwaiter();
            }
            catch (Exception)
            {
                // ignore
            }
        }

        public bool CheckClientEditAccess()
        {
            throw new NotImplementedException();
        }

        public JObject ConvertToJObject(bool useCode, bool needNativeCode = true)
        {
            JObject program = new JObject
            {
                {"Inhibited", Inhibited},
                {"Name", Name},
            };
            if (!string.IsNullOrEmpty(Description))
            {
                program["Description"] = Description;
            }

            if (MainRoutineName != null)
            {
                program["MainRoutineName"] = MainRoutineName;
            }

            if (FaultRoutineName != null)
            {
                program["FaultRoutineName"] = FaultRoutineName;
            }

            program["Type"] = (int) Type;
            program["UseAsFolder"] = ProgramProperties.UseAsFolder;

            if (Type == ProgramType.Phase)
            {
                program["InitialStepIndex"] = ProgramProperties.InitialStepIndex;
                program["InitialState"] = (int) ProgramProperties.InitialState;
                program["CompleteStateIfNotImpl"] = (int) ProgramProperties.CompleteStateIfNotImpl;
                program["LossOfCommCmd"] = (int) ProgramProperties.LossOfCommCmd;
                program["ExternalRequestAction"] = (int) ProgramProperties.ExternalRequestAction;
                program["AutoValueAssignStepToPhase"] = ProgramProperties.AutoValueAssignStepToPhase;
                program["AutoValueAssignPhaseToStepOnComplete"] =
                    ProgramProperties.AutoValueAssignPhaseToStepOnComplete;
                program["AutoValueAssignPhaseToStepOnStopped"] = ProgramProperties.AutoValueAssignPhaseToStepOnStopped;
                program["AutoValueAssignPhaseToStepOnAborted"] = ProgramProperties.AutoValueAssignPhaseToStepOnAborted;
            }

            // Routines
            JArray routines = new JArray();
            foreach (var routine in Routines)
            {
                if (routine != null)
                    routines.Add(routine.ConvertToJObject(useCode));
            }

            program.Add("Routines", routines);

            // Tags
            JArray tags = new JArray();
            foreach (var t in Tags)
            {
                var tag = t as Tag;
                if (tag != null)
                    tags.Add(tag.ConvertToJObject());
            }

            program.Add("Tags", tags);

            if (NativeCode != null && needNativeCode)
                program.Add("NativeCode", System.Convert.ToBase64String(NativeCode));// JArray.FromObject(new List<byte>(NativeCode)));

            return program;
        }

        private byte[] NativeCode { get; set; }


        public void GenCode()
        {
            foreach (var routine in Routines)
            {
                routine.GenCode(this);
            }
        }

        private CCodeGeneratorContext GenCCodeGeneratorContext()
        {
            HashSet<string> tags = new HashSet<string>();
            foreach (var tag in Tags)
            {
                tags.Add(tag.Name.ToUpper());
            }
            return new CCodeGeneratorContext(Name, tags);
        }

        public void GenNativeCode(OutputStream writer)
        {
            var context = GenCCodeGeneratorContext();
            foreach (var routine in Routines)
            {
                var Routine = (routine.Routine as RoutineCode).Code;
                {
                    var consts = MacroAssembler.ParseConstses(Routine.Pool);

                    var codegen = new CCodeGenerator(context, Routine.Logic, consts, writer);
                    codegen.GenCode(routine.Name + "LOGIC");
                }

                {
                    var consts = MacroAssembler.ParseConstses(Routine.Pool);

                    var codegen = new CCodeGenerator(context, Routine.Prescan, consts, writer);
                    codegen.GenCode(routine.Name + "PRESCAN");
                }
            }


        }

        public void GenSepNativeCode(Controller controller)
        {
            NativeCode = controller.GenerateNativeCodeForAction(GenNativeCode);
        }

        //TODO(gjc): for temp test
        public JObject CreateProgramUpdateInfo()
        {
            JObject updateInfo = new JObject();

            if (NativeCode != null)
                updateInfo.Add("NativeCode", System.Convert.ToBase64String(NativeCode));

            return updateInfo;
        }

        protected override void DisposeAction()
        {
            Routines.Dispose();
            Tags.Dispose();
        }
    }
}