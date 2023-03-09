namespace ICSStudio.Dialogs.ConfigDialogs
{
    internal class AddCommand : ICommand
    {
        private readonly CamEditorViewModel _viewModel;
        private readonly CamPoint _point;
        private readonly int _index;

        public AddCommand(CamEditorViewModel viewModel, int index, CamPoint point)
        {
            _viewModel = viewModel;
            _point = point;
            _index = index;
        }

        public void Execute()
        {
            _viewModel.CamPoints.Insert(_index, _point);

            _viewModel.UpdateView();
        }

        public void UnExecute()
        {
            _viewModel.CamPoints.RemoveAt(_index);

            _viewModel.UpdateView();
        }
    }
}
