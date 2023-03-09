using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    class NewScanModeRoutineViewModel : ViewModelBase
    {
        private readonly string _mode;
        private readonly string _name;
        private readonly IAoiDefinition _aoiDefinition;
        private bool? _dialogResult;

        public NewScanModeRoutineViewModel(string mode, IAoiDefinition aoiDefinition)
        {
            _mode = mode;
            _name = aoiDefinition.Name;
            _aoiDefinition = aoiDefinition;
            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            List<RoutineType> list = new List<RoutineType>();
            list.Add(RoutineType.RLL);
            list.Add(RoutineType.FBD);
            list.Add(RoutineType.ST);
            TypeList = list.Select(x =>
            {
                string displayName = "";
                switch (x)
                {
                    case RoutineType.RLL:
                        displayName = "Ladder Diagram";
                        break;
                    case RoutineType.FBD:
                        displayName = "Function Block Diagram";
                        break;
                    case RoutineType.ST:
                        displayName = "Structured Text";
                        break;
                }

                return new {DisplayName = displayName, Value = x};
            }).ToList();
            SelectedType = RoutineType.RLL;
        }

        public string Mode => _mode;

        public string Name => _name;

        public string Description { set; get; }

        public RelayCommand OkCommand { set; get; }

        public void ExecuteOkCommand()
        {
            switch (SelectedType)
            {
                case RoutineType.RLL:
                    RLLRoutine rllRoutine = new RLLRoutine(_aoiDefinition.ParentController)
                        {Name = Mode, Description = Description};
                    (_aoiDefinition.Routines as RoutineCollection).AddRoutine(rllRoutine);
                    break;
                case RoutineType.FBD:
                    FBDRoutine fbdRoutine = new FBDRoutine(_aoiDefinition.ParentController, null)
                        {Name = Mode, Description = Description};
                    (_aoiDefinition.Routines as RoutineCollection).AddRoutine(fbdRoutine);
                    break;
                case RoutineType.ST:
                    STRoutine stRoutine = new STRoutine(_aoiDefinition.ParentController)
                        {Name = Mode, Description = Description};
                    (_aoiDefinition.Routines as RoutineCollection).AddRoutine(stRoutine);
                    break;
            }

            //(_aoiDefinition as AoiDefinition)?.Reset();
            DialogResult = true;

            //IStudioUIService studioUIService =
            //    Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
            //studioUIService?.UpdateUI();
        }

        public RelayCommand CancelCommand { set; get; }

        public void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        public IList TypeList { set; get; }

        public RoutineType SelectedType { set; get; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }
    }
}
