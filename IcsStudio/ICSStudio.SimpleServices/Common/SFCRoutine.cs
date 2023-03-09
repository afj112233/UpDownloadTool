using System.Collections.Generic;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Chart;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.Common
{
    public class SFCRoutine : BaseComponent, IRoutine
    {
        private bool _isAbandoned;

        private bool _isMainRoutine;
        private bool _isFaultRoutine;

        private bool _isEncrypted;

        public SFCRoutine(IController controller)
        {
            ParentController = controller;
            Contents = new List<IContent>();
        }

        public SheetSize SheetSize { get; set; }
        public Orientation SheetOrientation { get; set; }
        public string StepName { set; get; }
        public string TransitionName { set; get; }
        public string ActionName { set; get; }
        public string StopName { set; get; }

        public List<IContent> Contents { get; }
        public override IController ParentController { get; }
        public uint TrackingGroups { get; }
        public bool IsSourceEditable { get; }
        public bool IsSourceViewable { get; }
        public bool IsSourceCopyable { get; }
        public IRoutineCollection ParentCollection { get; set; }

        public bool IsMainRoutine
        {
            get { return _isMainRoutine; }
            set
            {
                if (_isMainRoutine != value)
                {
                    _isMainRoutine = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsFaultRoutine
        {
            get { return _isFaultRoutine; }
            set
            {
                if (_isFaultRoutine != value)
                {
                    _isFaultRoutine = value;
                    RaisePropertyChanged();
                }
            }
        }

        public RoutineType Type { get; } = RoutineType.SFC;
        public bool IsAoiObject { get; }
        public bool ControllerEditsExist { get; }
        public bool PendingEditsExist { get; }
        public bool HasEditAccess { get; }
        public bool HasEditContentAccess { get; }

        public bool IsEncrypted
        {
            get { return _isEncrypted; }
            set
            {
                _isEncrypted = value;
                RaisePropertyChanged();
            }
        }

        public IRoutineCode Routine { get; } = new RoutineCode();
        public bool IsError { get; set; }


        public bool IsAbandoned
        {
            get { return _isAbandoned; }
            set
            {
                _isAbandoned = value;
                RaisePropertyChanged();
            }
        }

        public override bool IsDeleted
        {
            get
            {
                if (ParentCollection == null)
                    return true;

                if (ParentCollection.ParentProgram == null)
                    return true;

                if (ParentCollection.ParentProgram.IsDeleted)
                    return true;

                return false;
            }
        }

        public void GenCode(IProgramModule program)
        {
            //throw new NotImplementedException();
        }

        public JObject ConvertToJObject(bool useCode)
        {
            //throw new NotImplementedException();
            return null;
        }

        public bool IsCompiling { get; set; }
        public List<IRoutine> GetJmpRoutines()
        {
            return new List<IRoutine>();
        }
    }
}
