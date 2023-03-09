using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.ImportConfiguration.Panel.Validate;
using Newtonsoft.Json.Linq;

namespace ICSStudio.UIServicesPackage.ImportConfiguration.Panel.Rung
{
    public class RungViewModel: ViewModelBase, IVerify
    {
        private Operation _selectedOperation;
        private readonly int _startIndex;
        private readonly int _endIndex;
        private readonly JToken _contextRoutine;
        private readonly bool _isAfterLastRung;

        public enum Operation
        {
            Discard,
            Create,
            Overwrite
        }
        public RungViewModel(JToken contextProgram, RLLRoutine rllRoutine, int startIndex,int endIndex)
        {
            _contextRoutine = contextProgram["Routines"]?[0];

            var rungs = rllRoutine.CloneRungs();
            if (startIndex == endIndex && startIndex == rungs.Count) _isAfterLastRung = true;
            
            _startIndex = startIndex;
            _endIndex = endIndex;

            if (startIndex == rungs.Count) startIndex--;
            if (endIndex == rungs.Count) endIndex--;
            _contextRoutine["StartIndex"] = startIndex;
            _contextRoutine["EndIndex"] = endIndex;

            RoutineName = rllRoutine.Name;
            RoutineDescription = rllRoutine.Description;
            RoutineType = rllRoutine.Type.ToString();
            InProgram = rllRoutine.ParentCollection.ParentProgram.Name;
            ImportedRungs = _contextRoutine?["Rungs"]?.ToList().Count.ToString() ?? "0";

            Operations = new List<Operation>() { Operation.Create ,Operation.Discard};
            if (!_isAfterLastRung) Operations.Add(Operation.Overwrite);
            SelectedOperation = startIndex == endIndex ? Operation.Create : Operation.Overwrite;
        }

        public string ImportedRungs { get; }

        public List<Operation> Operations { get; }

        public Operation SelectedOperation
        {
            get { return _selectedOperation; }
            set
            {
                _selectedOperation = value;
                RaisePropertyChanged(nameof(OperationTip));
                _contextRoutine["Operation"] = value.ToString();
            }
        }

        public string OperationTip
        {
            get
            {
                switch (SelectedOperation)
                {
                    case Operation.Create:
                        return _isAfterLastRung? "after last Rung": "after Rung " + _endIndex;
                    case Operation.Overwrite:
                        var count = _endIndex - _startIndex + 1;
                        var unit = count > 1 ? "Rungs" : "Rung";
                        return _startIndex == _endIndex ? $"Rung {_endIndex} (1Rung)" : $"Rungs {_startIndex} through {_endIndex} ({count}{unit})";
                    default:
                        return string.Empty;
                }
            }
        }

        public string RoutineName { get;}

        public string RoutineDescription { get; }

        public string RoutineType { get; }

        public string InProgram { get; }
        public string Error { get; set; }
    }
}
