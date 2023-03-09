using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.SimpleServices.SourceProtection;
using ICSStudio.MultiLanguage;

namespace ICSStudio.ToolsPackage.SourceProtection
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class SourceKeyCfgViewModel : ViewModelBase
    {
        private readonly SourceKeyProvider _provider;

        private bool? _dialogResult;


        public SourceKeyCfgViewModel(SourceKeyProvider provider)
        {
            _provider = provider;

            CloseCommand = new RelayCommand(ExecuteClose);

            SpecifyCommand = new RelayCommand(ExecuteSpecify);
            ViewCommand = new RelayCommand(ExecuteView, CanExecuteView);
            ClearCommand = new RelayCommand(ExecuteClear, CanExecuteClear);
        }

        public string SourceKeyFileName
        {
            get
            {
                string keyFile = _provider.SourceKeyFile;

                if (IsSourceKeyFileValid(keyFile))
                    return keyFile;

                return LanguageManager.GetInstance().ConvertSpecifier("DoesNotExist");
            }
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        #region Command

        public RelayCommand CloseCommand { get; }
        public RelayCommand SpecifyCommand { get; }
        public RelayCommand ViewCommand { get; }
        public RelayCommand ClearCommand { get; }

        private void ExecuteClose()
        {
            DialogResult = false;
        }

        private bool CanExecuteClear()
        {
            if (IsSourceKeyFileValid(_provider.SourceKeyFile))
                return true;

            return false;
        }

        private void ExecuteClear()
        {
            if (IsSourceKeyFileValid(_provider.SourceKeyFile))
            {
                string message = "The Source Key File(sk.dat) exists.\nDelete Source Key File?";

                if (MessageBox.Show(message, "ICS Studio", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        File.Delete(_provider.SourceKeyFile);
                    }
                    catch (Exception)
                    {
                        //ignore
                    }

                    _provider.SourceKeyFolder = string.Empty;
                    _provider.SaveConfig();

                    RaisePropertyChanged("SourceKeyFileName");
                    ViewCommand.RaiseCanExecuteChanged();
                    ClearCommand.RaiseCanExecuteChanged();

                }

            }
        }

        private bool CanExecuteView()
        {
            if (IsSourceKeyFileValid(_provider.SourceKeyFile))
                return true;

            return false;
        }

        private void ExecuteView()
        {
            if (IsSourceKeyFileValid(_provider.SourceKeyFile))
                System.Diagnostics.Process.Start(_provider.SourceKeyFile);
        }

        private void ExecuteSpecify()
        {
            SourceKeyFileLocationDialog dialog = new SourceKeyFileLocationDialog
            {
                Owner = Application.Current.MainWindow
            };
            SourceKeyFileLocationViewModel viewModel
                = new SourceKeyFileLocationViewModel(_provider.SourceKeyFile);
            dialog.DataContext = viewModel;

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                _provider.SourceKeyFolder = viewModel.Location;
                _provider.SaveConfig();

                RaisePropertyChanged("SourceKeyFileName");
                ViewCommand.RaiseCanExecuteChanged();
                ClearCommand.RaiseCanExecuteChanged();
            }
        }


        #endregion

        private bool IsSourceKeyFileValid(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            if (!File.Exists(fileName))
                return false;

            return true;
        }

    }
}
