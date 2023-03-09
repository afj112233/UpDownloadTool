using System;
using System.Windows;
using ICSStudio.Dialogs.BrowseString.RichTextBoxExtend;
using ICSStudio.Interfaces.DataType;

namespace ICSStudio.Dialogs.BrowseString
{
    /// <summary>
    /// BrowseString.xaml 的交互逻辑
    /// </summary>
    public partial class BrowseString
    {
        private Message _message;
        public BrowseString(IDataType dataType, Message message)
        {
            InitializeComponent();
            _message = message;
            DataContext = new BrowseStringViewModel(dataType, message);
            Loaded += BrowseString_Loaded;
        }

        private void BrowseString_Loaded(object sender, RoutedEventArgs e)
        {
            AsciiRichTextBox.Focus();
        }

        public Message Message => _message;

        public bool IsError
        {
            get { return ((BrowseStringViewModel) DataContext).IsError || ((BrowseStringViewModel)DataContext).IsOverflow; }
        }

        public string Text
        {
            get { return _message.Text; }
        }

        public int Count
        {
            get { return ((BrowseStringViewModel) DataContext).Count; }
        }
        
        public void ForceApplyTextAlignment()
        {
            AsciiRichTextBox.Document.TextAlignment = TextAlignment.Left;
        }
    }
}
