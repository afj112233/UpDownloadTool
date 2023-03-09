using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using GalaSoft.MvvmLight;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;


namespace ICSStudio.OrganizerPackage.Model
{
    public class RoutinePropertyData : ViewModelBase
    {
        private string _type;
        private string _numberOfLines;
        private string _inWhere;
        private string _inWho;

        public string Type
        {
            get { return _type; }
            set { Set(ref _type, value); }
        }

        public string NumberOfLines
        {
            get { return _numberOfLines; }
            set { Set(ref _numberOfLines, value); }
        }

        public string InWhere
        {
            get { return _inWhere; }
            set { Set(ref _inWhere, value); }
        }

        public string InWho
        {
            get { return _inWho; }
            set { Set(ref _inWho, value); }
        }



        public RoutinePropertyData(IRoutine routine, IController controller)
        {
            Type = routine.Type.ToString();
            if (Type == "ST")
                Type = "Structured Text";
            NumberOfLines = "Original "+(routine as STRoutine)?.CodeText.Count;
            var a = routine.ParentCollection.ParentProgram;
            InWhere = controller.Programs.Contains(a) ? "In Program:" : "In Instruction:";
            InWho = routine.ParentCollection.ParentProgram.Name;
        }
    }
}
