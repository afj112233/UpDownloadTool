using System.Windows;

namespace ICSStudio.Gui.Controls
{
    /// <summary>
    ///     StatusBitControl.xaml 的交互逻辑
    /// </summary>
    public partial class StatusBitControl
    {
        public static readonly DependencyProperty IsSetProperty = DependencyProperty.Register("IsSet", typeof(bool),
            typeof(StatusBitControl), new UIPropertyMetadata(false, OnIsSetChanged));


        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string),
            typeof(StatusBitControl), new UIPropertyMetadata(string.Empty, OnTextChanged));

        public StatusBitControl()
        {
            InitializeComponent();

            IsEnabledChanged += OnIsEnabledChanged;
        }

        public bool IsSet
        {
            get { return (bool) GetValue(IsSetProperty); }
            set { SetValue(IsSetProperty, value); }
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var statusBitControl = (StatusBitControl) d;

            statusBitControl?.OnTextChanged((string) args.OldValue, (string) args.NewValue);
        }

        private static void OnIsSetChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var statusBitControl = (StatusBitControl) d;

            statusBitControl?.OnIsSetChanged((bool) args.OldValue, (bool) args.NewValue);
        }

        // ReSharper disable once UnusedParameter.Local
        private void OnIsSetChanged(bool oldValue, bool newValue)
        {
            if (newValue && IsEnabled)
                SetEllipse.Visibility = Visibility.Visible;
            else
                SetEllipse.Visibility = Visibility.Collapsed;
        }

        // ReSharper disable once UnusedParameter.Local
        private void OnTextChanged(string oldValue, string newValue)
        {
            NameLabel.Content = newValue;
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsEnabled)
            {
                EnableEllipse.Visibility = Visibility.Visible;
                DisableEllipse.Visibility = Visibility.Collapsed;
                SetEllipse.Visibility = IsSet ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                EnableEllipse.Visibility = Visibility.Collapsed;
                DisableEllipse.Visibility = Visibility.Visible;
                SetEllipse.Visibility = Visibility.Collapsed;
            }
        }
    }
}