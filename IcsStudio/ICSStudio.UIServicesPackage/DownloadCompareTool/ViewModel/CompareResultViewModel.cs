using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.DownloadChange;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIServicesPackage.DownloadCompareTool.ViewModel.Item;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSStudio.UIServicesPackage.DownloadCompareTool.ViewModel
{
    internal class CompareResultViewModel : ViewModelBase
    {
        private readonly Controller _controller;

        private bool? _dialogResult;

        private readonly DownloadChangeHelper _helper;

        private bool _isDownloadAxisParameters;

        public CompareResultViewModel(Controller controller, ProjectDiffModel diffModel)
        {
            _controller = controller;

            DiffModel = diffModel;

            _helper = new DownloadChangeHelper(diffModel);

            ShowResultCommand = new RelayCommand<CompareItem>(ShowCompareResult);
            DownloadCommand = new RelayCommand(ExecuteDownload, CanExecuteDownload);
            CancelCommand = new RelayCommand(ExecuteCancel);

            BuildItemsSource();

            if (_controller?.CipMessager != null)
            {
                WeakEventManager<ICipMessager, EventArgs>.AddHandler(
                    _controller.CipMessager, "Disconnected", OnDisconnected);
            }
        }

        public ProjectDiffModel DiffModel { get; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public bool IsDownloadAxisParameters
        {
            get { return _isDownloadAxisParameters; }
            set { Set(ref _isDownloadAxisParameters, value); }
        }

        public ObservableCollection<CompareItem> ItemsSource { get; set; }

        public RelayCommand<CompareItem> ShowResultCommand { get; }
        public RelayCommand DownloadCommand { get; }
        public RelayCommand CancelCommand { get; }

        private string _oldText;

        public string OldText
        {
            get { return _oldText; }
            set { Set(ref _oldText, value); }
        }

        private string _newText;

        public string NewText
        {
            get { return _newText; }
            set { Set(ref _newText, value); }
        }

        private bool CanExecuteDownload()
        {
            return _controller.IsConnected && _helper.CanDownload;
        }

        private void ExecuteDownload()
        {
            DialogResult = true;
        }

        private void ExecuteCancel()
        {
            DialogResult = false;
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                DownloadCommand.RaiseCanExecuteChanged();
            });
        }

        private void BuildItemsSource()
        {
            var itemsSource = new ObservableCollection<CompareItem>();

            itemsSource.Add(new ControllerItem(DiffModel));

            itemsSource.Add(new DataTypesItem(DiffModel.DataTypes));

            itemsSource.Add(new AOIDefinitionsItem(DiffModel.AOIDefinitions));

            itemsSource.Add(new TasksItem(DiffModel.Tasks));

            //Programs
            itemsSource.Add(new ProgramsItem(DiffModel.Programs));

            itemsSource.Add(new ModulesItem(DiffModel.Modules));


            ItemsSource = itemsSource;
        }

        private void ShowCompareResult(CompareItem compareItem)
        {
            var item = compareItem.DiffItem;
            if (item == null) return;
            try
            {
                var oldValue = item.OldValue.DeepClone() as JObject;
                var newValue = item.NewValue.DeepClone() as JObject;
                SortJObject(oldValue);
                SortJObject(newValue);

                OldText = Convert.ToString(oldValue);
                var tempNewText = Convert.ToString(newValue);
                if (!string.IsNullOrEmpty(tempNewText) && compareItem.OriginalType == CompareOriginalType.Tag)
                {
                    if (tempNewText.Contains(".0,\r\n")) tempNewText = tempNewText.Replace(".0,\r\n", ",\r\n");
                    if (tempNewText.Contains(".0\r\n")) tempNewText = tempNewText.Replace(".0\r\n", "\r\n");
                }

                NewText = tempNewText;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Controller.GetInstance().Log($"Show compare result failed:[{compareItem.Title}] - {e.Message}");
                OldText = item.OldValue?.ToString() ?? string.Empty;
                NewText = item.NewValue?.ToString() ?? string.Empty;
            }
        }

        private void SortJObject(JObject jObj)
        {
            if (jObj == null) return;
            var props = jObj.Properties().ToList();
            foreach (var prop in props) prop.Remove();

            foreach (var prop in props.OrderBy(p => p.Name))
            {
                jObj.Add(prop);
                if (prop.Value is JObject)
                    SortJObject((JObject)prop.Value);
                if (prop.Value is JArray)
                {
                    var iCount = prop.Value.Count();
                    for (var iIterator = 0; iIterator < iCount; iIterator++)
                        if (prop.Value[iIterator] is JObject)
                            SortJObject((JObject)prop.Value[iIterator]);
                }
            }
        }
    }
}
