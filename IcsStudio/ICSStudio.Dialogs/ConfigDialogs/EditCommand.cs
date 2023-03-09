namespace ICSStudio.Dialogs.ConfigDialogs
{
    internal class EditCommand : ICommand
    {
        private CamEditorViewModel _camEditorViewModel;
        private CamPoint _orginalCamPoint;
        private CamPoint _camPoint;
        private int _index;

        public EditCommand(CamEditorViewModel camEditorViewModel,int index,
            CamPoint orginalCamPoint, CamPoint camPoint)
        {
            _camEditorViewModel = camEditorViewModel;
            _orginalCamPoint = orginalCamPoint;
            _camPoint = camPoint;
            _index = index;
        }

        public void Execute()
        {
            _camEditorViewModel.CamPoints[_index].ChangedCamPoint(_camPoint);
                   
            _camEditorViewModel.UpdatePlotModel();
        }

        public void UnExecute()
        {
            _camEditorViewModel.CamPoints[_index].ChangedCamPoint(_orginalCamPoint);
            
            _camEditorViewModel.UpdatePlotModel();
        }
    }
}
