using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.UI;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    public class ChangeTypeViewModel : ViewModelBase
    {
        private AoiDefinition _aoiDefinition;
        public ChangeTypeViewModel(RoutineType type,AoiDefinition aoiDefinition)
        {
            _aoiDefinition = aoiDefinition;
            OkCommand=new RelayCommand(ExecuteOkCommand);
            CancelCommand=new RelayCommand(ExecuteCancelCommand);
            switch (type)
            {
                case RoutineType.RLL:
                    OldType = "Ladder Diagram";
                    break;
                case RoutineType.FBD:
                    OldType = "Function Block Diagram";
                    break;
                case RoutineType.ST:
                    OldType = "Structured Text";
                    break;
            }
            List<RoutineType> temp=new List<RoutineType>()
            {
                RoutineType.RLL,RoutineType.FBD,RoutineType.ST
            };
            temp.Remove(type);
            TypeList = temp.Select(x =>
            {
                string name = "";
                switch (x)
                {
                    case RoutineType.RLL:
                        name = "Ladder Diagram";
                        break;
                    case RoutineType.FBD:
                        name = "Function Block Diagram";
                        break;
                    case RoutineType.ST:
                        name = "Structured Text";
                        break;
                }

                return new {DisplayName = name, Value = x};
            }).ToList();
            Type = temp[0];
        }
        public bool? DialogResult { set; get; }
        public string OldType { set; get; }
        public IList TypeList { set; get; }
        public RoutineType Type { set; get; }

        public RelayCommand OkCommand { set; get; }

        public void ExecuteOkCommand()
        {
            var createEditorService = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            foreach (var item in _aoiDefinition.Routines) createEditorService?.CloseSTEditor(item);

            IRoutine logic = null;
            switch (Type)
            {
                case RoutineType.RLL:
                    logic = new RLLRoutine(_aoiDefinition.ParentController);
                    break;
                case RoutineType.FBD:
                    logic = new FBDRoutine(_aoiDefinition.ParentController, null);
                    break;
                case RoutineType.ST:
                    logic = new STRoutine(_aoiDefinition.ParentController);
                    break;
            }

            logic.Name = "Logic";
            logic.ParentCollection = _aoiDefinition.Routines;
            (_aoiDefinition.Routines as RoutineCollection)?.ReplaceLogic(logic);
            IStudioUIService studioUIService =
                Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
            studioUIService?.Reset();
            DialogResult = true;
            RaisePropertyChanged("DialogResult");
        }

        public RelayCommand CancelCommand { set; get; }

        public void ExecuteCancelCommand()
        {
            DialogResult = false;
            RaisePropertyChanged("DialogResult");
        }
    }
}
