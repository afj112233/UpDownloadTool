using System.Windows;

namespace ICSStudio.Dialogs
{
    /// <summary>
    /// CopyableMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class CopyableMessageBox
    {
        public static void Show(string message, string caption)
        {
            CopyableMessageBox messageBox = new CopyableMessageBox();

            messageBox.Title = caption;
            messageBox.Message = message;

            messageBox.Owner = Application.Current.MainWindow;

            messageBox.ShowDialog();
        }

        public CopyableMessageBox()
        {
            InitializeComponent();
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;

                TextBox.Text = _message;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
