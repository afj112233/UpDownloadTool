using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.CreateHistoryEntry;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using Microsoft.VisualStudio.Shell;
using DateTime = System.DateTime;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    class SignatureViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _canAddToHistoryCommand;
        private bool _canClearCommand;
        private bool _isDirty;
        private DateTime _timestamp;
        private Visibility _generateVisibility;
        private Visibility _removeVisibility;
        private readonly AoiDefinition _aoiDefinition;
        private readonly string _user;
        private bool _copyEnable=false;
        private AddOnInstructionVM _vm;
        private string _timestampKey;
        private string _generatingText;
        public SignatureViewModel(Signature panel, IAoiDefinition aoiDefinition, AddOnInstructionVM vm)
        {
            _vm = vm;
            Control = panel;
            panel.DataContext = this;
            _aoiDefinition = aoiDefinition as AoiDefinition;
            _user = Environment.MachineName+ @"\" + Environment.UserName;
            HistoryRows = new ObservableCollection<HistoryRow>();
            GenerateVisibility = Visibility.Visible;
            RemoveVisibility = Visibility.Collapsed;
            GenerateCommand = new RelayCommand(ExecuteGenerateCommand);
            RemoveCommand = new RelayCommand(ExecuteRemoveCommand);
            _canClearCommand = false;
            _canAddToHistoryCommand = false;
            AddToHistoryCommand = new RelayCommand(ExecuteAddToHistoryCommand, CanAddToHistoryCommand);
            ClearCommand = new RelayCommand(ExecuteClearCommand, CanClearCommand);
            SetDataGrid();
            if (_aoiDefinition.IsSealed)
            {
                GenerateVisibility = Visibility.Collapsed;
                RemoveVisibility = Visibility.Visible;
                _canAddToHistoryCommand = true;
                _timestamp = _aoiDefinition.EditedDate;
                _copyEnable = _aoiDefinition.IsSealed;
            }

            if (HistoryRows.Count > 0 && !_aoiDefinition.IsSealed)
            {
                _canClearCommand = true;
            }

            IsAllEnabled = !aoiDefinition.ParentController.IsOnline;

            Controller controller = (Controller)_aoiDefinition.ParentController;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            _generatingText = LanguageManager.GetInstance().ConvertSpecifier("GeneratingASignature");
            _timestampKey = LanguageManager.GetInstance().ConvertSpecifier("Timestamp");
            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(),"LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            _timestampKey = LanguageManager.GetInstance().ConvertSpecifier("Timestamp");
            Timestamp = _timestampKey + $":   {(_timestamp == default(DateTime) ? "" : _timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}";
            _generatingText = LanguageManager.GetInstance().ConvertSpecifier("GeneratingASignature");
        }

        public override void Cleanup()
        {
            Controller controller = (Controller)_aoiDefinition.ParentController;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                IsAllEnabled = !_aoiDefinition.ParentController.IsOnline;
                RaisePropertyChanged("IsAllEnabled");
                IsDirty = false;
            });
        }

        public bool IsAllEnabled { set; get; }

        public void SetDataGrid()
        {
            foreach (History item in _aoiDefinition.History)
            {
                HistoryRow historyRow = new HistoryRow();
                historyRow.Description = item.Description;
                historyRow.SignatureID = item.SignatureID;
                historyRow.Timestamp = item.Timestamp;
                historyRow.User = item.User;
                HistoryRows.Add(historyRow);
            }
        }

        public ObservableCollection<HistoryRow> HistoryRows { set; get; }

        public string ID => $"ID:                 {_aoiDefinition.SignatureID}";

        private string _timestampValue;

        public string Timestamp
        {
            get
            {
                return _timestampValue;
            }
            set
            {
                _timestampValue = _timestampKey + $":   {(_timestamp == default(DateTime) ? "" : _timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"))}"; 
                RaisePropertyChanged(Timestamp);
            }
        }

        public RelayCommand GenerateCommand { set; get; }

        public void ExecuteGenerateCommand()
        {
            if (MessageBox.Show(
                    _generatingText,
                    "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Exclamation) == MessageBoxResult.OK)
            {
                _aoiDefinition.IsSealed = true;
                if (_aoiDefinition.IsSealed)
                {
                    GenerateVisibility = Visibility.Collapsed;
                    RemoveVisibility = Visibility.Visible;
                    _canAddToHistoryCommand = true;
                    AddToHistoryCommand.RaiseCanExecuteChanged();
                    _aoiDefinition.SignatureID = MD5Encrypt(_aoiDefinition.Name);
                    _timestamp = DateTime.Now;

                    _aoiDefinition.EditedBy = _user;
                    _aoiDefinition.EditedDate = _timestamp;
                    CopyEnable = _aoiDefinition.IsSealed;
                    RaisePropertyChanged("ID");
                    RaisePropertyChanged("Timestamp");
                }
            }
        }

        public bool CopyEnable
        {
            set { Set(ref _copyEnable , value); }
            get { return _copyEnable; }
        }

        public Visibility GenerateVisibility
        {
            set { Set(ref _generateVisibility, value); }
            get { return _generateVisibility; }
        }

        public RelayCommand RemoveCommand { set; get; }

        public void ExecuteRemoveCommand()
        {
            if (MessageBox.Show(
                    "Removing the signature will unseal the instruction and allow modifications.\rRemove signature?",
                    "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Exclamation) == MessageBoxResult.OK)
            {
                GenerateVisibility = Visibility.Visible;
                RemoveVisibility = Visibility.Collapsed;
                _canAddToHistoryCommand = false;
                AddToHistoryCommand.RaiseCanExecuteChanged();
                _aoiDefinition.SignatureID = "";
               _timestamp = default(DateTime);

                _aoiDefinition.IsSealed = false;
                if (HistoryRows.Count > 0)
                {
                    _canClearCommand = true;
                    ClearCommand.RaiseCanExecuteChanged();
                }

                RaisePropertyChanged("ID");
                RaisePropertyChanged("Timestamp");
            }
        }

        public Visibility RemoveVisibility
        {
            set { Set(ref _removeVisibility, value); }
            get { return _removeVisibility; }
        }

        public RelayCommand AddToHistoryCommand { set; get; }

        public bool CanAddToHistoryCommand()
        {
            return _canAddToHistoryCommand;
        }

        public void ExecuteAddToHistoryCommand()
        {
            var dialog = new CreateHistoryEntryDialog();
            var viewModel = new CreateHistoryEntryDialogViewModel();
            dialog.DataContext = viewModel;
            dialog.Width = 350;
            dialog.Height = 167;
            dialog.Owner = Application.Current.MainWindow;
            if (dialog.ShowDialog().Value)
            {
                if (HistoryRows.Count == 6)
                {
                    HistoryRows.RemoveAt(5);
                    _aoiDefinition.History.RemoveAt(5);
                }

                HistoryRow historyRow = new HistoryRow();
                historyRow.Description = viewModel.Description;
                historyRow.SignatureID = _aoiDefinition.SignatureID;
                var time = DateTime.Now;
                historyRow.Timestamp = time.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                historyRow.User = _user;
                HistoryRows.Insert(0, historyRow);
                History historyTuple =
                    new History()
                    {
                        User = _user, SignatureID = _aoiDefinition.SignatureID, Timestamp = historyRow.Timestamp,
                        Description = historyRow.Description
                    };
                (_aoiDefinition.History as List<History>)?.Insert(0, historyTuple);
                _aoiDefinition.EditedBy = _user;
                _aoiDefinition.EditedDate = time;
                //_aoiDefinition.Reset();
            }

        }

        public RelayCommand ClearCommand { set; get; }

        public bool CanClearCommand()
        {
            return _canClearCommand;
        }

        public void ExecuteClearCommand()
        {
            if (MessageBox.Show("Existing signature history will be deleted.\rDelete signature history?",
                    "ICS Studio",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Exclamation) == MessageBoxResult.OK)
            {
                if (_vm.Check(true))
                {
                    HistoryRows.Clear();
                    _aoiDefinition.History.Clear();
                    _canClearCommand = false;
                    ClearCommand.RaiseCanExecuteChanged();
                    //_aoiDefinition.Reset();
                    RaisePropertyChanged("HistoryRows");
                }
            }
        }

        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {

        }

        public bool SaveOptions()
        {
            return true;
        }

        public bool IsDirty
        {
            set
            {
                Set(ref _isDirty, value);
                IsDirtyChanged?.Invoke(this, new EventArgs());
            }
            get { return _isDirty; }
        }

        public event EventHandler IsDirtyChanged;

        private string MD5Encrypt(string password)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes;
            hashedDataBytes = md5Hasher.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(password));
            StringBuilder tmp = new StringBuilder();
            foreach (byte i in hashedDataBytes)
            {
                tmp.Append(i.ToString("X2"));
            }

            return tmp.ToString().Substring(11, 8);
        }
    }

    public class HistoryRow
    {
        public string User { set; get; }
        public string SignatureID { set; get; }
        public string Timestamp { set; get; }
        public string Description { set; get; }
    }
}
