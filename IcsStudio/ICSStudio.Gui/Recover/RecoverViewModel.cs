using System.IO;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ICSStudio.Gui.Recover
{
    public class RecoverViewModel : ViewModelBase
    {
        private bool? _dialogResult;
        private readonly string _filePath;

        public RecoverViewModel(string filePath)
        {
            _filePath = filePath;
            OKCommand = new RelayCommand(ExecuteOKCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ClickCommand = new RelayCommand<RoutedEventArgs>(ExecuteClickCommand);
        }

        public bool IsDiscard { set; get; }

        private bool _isRecover = true;

        public bool IsRecover
        {
            get { return _isRecover; }
            set { Set(ref _isRecover, value); }
        }

        private bool _isSave;
        public bool IsSave
        {
            get { return _isSave; }
            set { Set(ref _isSave, value); }
        }

        private bool _isContinue;

        public bool IsContinue
        {
            set { Set(ref _isContinue, value); }
            get { return _isContinue; }
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public RelayCommand OKCommand { get; }

        private void ExecuteOKCommand()
        {
            if (IsRecover)
            {
                File.Delete($"{_filePath}");
                File.Copy($"{_filePath}.Recovery", _filePath);
                File.Delete($"{_filePath}.Recovery");
                DialogResult = true;
                return;
            }

            if (IsSave)
            {
                System.Windows.Forms.SaveFileDialog saveDlg = new System.Windows.Forms.SaveFileDialog()
                {
                    Title = "Export file",
                    Filter = "json文件(*.json)|*.json"
                };

                if (saveDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    File.Copy($"{_filePath}.Recovery", saveDlg.FileName);
                    File.Delete($"{_filePath}.Recovery");
                }

                DialogResult = false;
                return;
            }

            if (IsDiscard)
            {
                File.Delete($"{_filePath}.Recovery");
            }

            DialogResult = true;
        }

        public RelayCommand CancelCommand { get; }

        private void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        public RelayCommand<RoutedEventArgs> ClickCommand { get; }

        private void ExecuteClickCommand(RoutedEventArgs args)
        {
            var source = args.Source as TextBlock;
            string index = source?.Tag?.ToString();

            if (index == "1")
                IsRecover = true;

            if (index == "2")
                IsSave = true;

            if (index == "3")
                IsContinue = true;
        }
    }
}
