using System;
using System.Collections.Generic;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    internal class DeleteCommand : ICommand
    {
        private readonly CamEditorViewModel _viewModel;
        private readonly List<Tuple<int, CamPoint>> _deleteList;

        public DeleteCommand(CamEditorViewModel viewModel, List<Tuple<int, CamPoint>> deleteList)
        {
            _viewModel = viewModel;
            _deleteList = deleteList;
        }

        public void Execute()
        {
            foreach (var point in _deleteList)
            {
                _viewModel.CamPoints.RemoveAt(point.Item1);
            }

            _viewModel.UpdateView();
        }

        public void UnExecute()
        {
            foreach (var point in _deleteList)
            {
                _viewModel.CamPoints.Insert(point.Item1, point.Item2);
            }

            _viewModel.UpdateView();
        }
    }
}
