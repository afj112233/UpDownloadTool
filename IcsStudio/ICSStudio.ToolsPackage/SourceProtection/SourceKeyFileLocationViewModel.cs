using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MessageBox = System.Windows.MessageBox;
using ICSStudio.MultiLanguage;

namespace ICSStudio.ToolsPackage.SourceProtection
{
    public class SourceKeyFileLocationViewModel : ViewModelBase
    {
        private bool? _dialogResult;
        private string _location;

        public SourceKeyFileLocationViewModel(string sourceKeyFile)
        {
            _location = GetDirectoryName(sourceKeyFile);

            BrowseCommand = new RelayCommand(ExecuteBrowse);

            CancelCommand = new RelayCommand(ExecuteCancel);
            OkCommand = new RelayCommand(ExecuteOk, CanExecuteOk);
        }

        private string GetDirectoryName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            if (!File.Exists(fileName))
                return string.Empty;

            return Path.GetDirectoryName(fileName);
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public string Location
        {
            get { return _location; }
            set
            {
                Set(ref _location, value);

                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand BrowseCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand OkCommand { get; }


        private void ExecuteBrowse()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                SelectedPath = Location,
                ShowNewFolderButton = false,
                Description = @LanguageManager.GetInstance().ConvertSpecifier("Browse Source Key")
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Location = dialog.SelectedPath;
            }

        }

        private void ExecuteOk()
        {
            string fullName;
            if (_location.EndsWith("\\"))
                fullName = _location + "sk.dat";
            else
                fullName = _location + "\\" + "sk.dat";

            if (!File.Exists(fullName))
            {
                string message = "Source key file(sk.dat) does not exits.\nCreate a new file?";

                if (MessageBox.Show(message, "ICS Studio", MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.No)
                    return;

                if (!Directory.Exists(_location))
                {
                    message = $"The directory '{_location}' does not exit.\nCreate new directory?";
                    if (MessageBox.Show(message, "ICS Studio", MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.No)
                        return;

                    try
                    {
                        Directory.CreateDirectory(_location);
                    }
                    catch (Exception)
                    {
                        //ignore
                    }

                }

                if (CreateEmptyFile(fullName) == 0)
                {
                    DialogResult = true;
                }
                else
                {
                    message = $"Failed to create the source key file.\n{fullName} contains an incorrect path.";
                    MessageBox.Show(message, "ICS Studio", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);
                }

            }
            else
            {
                DialogResult = true;
            }
        }

        private int CreateEmptyFile(string fileName)
        {
            List<string> lines = new List<string>
            {
                "\t<ICS STUDIO UTF-8 ENCODED SOURCE KEY FILE - FIRST LINE MUST NOT BE A SOURCE KEY!>"
            };

            try
            {
                File.WriteAllLines(fileName, lines);
                return 0;
            }
            catch (Exception)
            {
                //ignore
            }

            return -1;
        }

        private void ExecuteCancel()
        {
            DialogResult = false;
        }

        private bool CanExecuteOk()
        {
            if (string.IsNullOrEmpty(Location))
                return false;

            return true;
        }
    }
}
