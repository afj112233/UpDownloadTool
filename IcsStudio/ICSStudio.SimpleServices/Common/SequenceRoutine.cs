using System;
using System.Collections.Generic;
using ICSStudio.Interfaces.Common;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.Common
{
    public class SequenceRoutine : BaseComponent, IRoutine
    {
        private bool _isAbandoned;

        private bool _isEncrypted;

        public SequenceRoutine(IController controller)
        {
            ParentController = controller;
        }

        public override IController ParentController { get; }
        public uint TrackingGroups { get; }
        public bool IsSourceEditable { get; }
        public bool IsSourceViewable { get; }
        public bool IsSourceCopyable { get; }
        public IRoutineCollection ParentCollection { get; set; }
        public bool IsMainRoutine { get; set; }
        public bool IsFaultRoutine { get; set; }
        public RoutineType Type { get; } = RoutineType.Sequence;
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
            //TODO(gjc): add code here
        }

        public JObject ConvertToJObject(bool useCode)
        {
            //TODO(gjc): add code here

            var routine = new JObject
            {
                { "Name", Name },
                { "Type", (int)Type },
                { "Description", Description }
            };


            return routine;
        }

        public bool IsCompiling { get; set; }
        public List<IRoutine> GetJmpRoutines()
        {
            return new List<IRoutine>();
        }
    }
}
