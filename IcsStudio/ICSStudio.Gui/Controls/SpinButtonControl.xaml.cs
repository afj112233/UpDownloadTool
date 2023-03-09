using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;

namespace ICSStudio.Gui.Controls
{
    public enum SpinStrategy
    {
        Rollover,
        StopAtBoundaries
    }

    /// <summary>
    ///     SpinButtonControl.xaml 的交互逻辑
    /// </summary>
    public partial class SpinButtonControl
    {
        public static readonly DependencyProperty SpinBehaviorProperty;
        public static readonly DependencyProperty IsEditableProperty;
        public static readonly DependencyProperty MaxLengthProperty;
        public static readonly DependencyProperty BackColorProperty;
        public static readonly DependencyProperty BorderColorProperty;
        public static readonly DependencyProperty ButtonBackgroundBrushProperty;
        public static readonly DependencyProperty ButtonBorderBrushProperty;
        public static readonly DependencyProperty ButtonArrowBrushProperty;
        public static readonly DependencyProperty ForeColorProperty;
        public static readonly DependencyProperty StepProperty;
        public static readonly DependencyProperty MinValueProperty;
        public static readonly DependencyProperty MaxValueProperty;
        public static readonly DependencyProperty ReadOnlyCaretVisibleProperty;
        public static readonly DependencyProperty SelectAllTextProperty;
        public static readonly DependencyProperty ValueProperty;

        static SpinButtonControl()
        {
            SpinBehaviorProperty = DependencyProperty.Register("SpinBehavior", typeof(SpinStrategy),
                typeof(SpinButtonControl), new UIPropertyMetadata(SpinStrategy.Rollover));
            IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(SpinButtonControl),
                new UIPropertyMetadata(true));
            MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(SpinButtonControl),
                new UIPropertyMetadata(5));
            BackColorProperty = DependencyProperty.Register("BackColor", typeof(Brush), typeof(SpinButtonControl),
                new UIPropertyMetadata(SystemColors.WindowBrush));
            BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(Brush), typeof(SpinButtonControl),
                new UIPropertyMetadata(Brushes.LightGray));
            ButtonBackgroundBrushProperty =
                DependencyProperty.Register("ButtonBackgroundBrush", typeof(Brush), typeof(SpinButtonControl));
            ButtonBorderBrushProperty =
                DependencyProperty.Register("ButtonBorderBrush", typeof(Brush), typeof(SpinButtonControl));
            ButtonArrowBrushProperty = DependencyProperty.Register("ButtonArrowColor", typeof(Brush),
                typeof(SpinButtonControl), new UIPropertyMetadata(null));
            ForeColorProperty = DependencyProperty.Register("ForeColor", typeof(Brush), typeof(SpinButtonControl),
                new UIPropertyMetadata(SystemColors.WindowTextBrush));
            StepProperty = DependencyProperty.Register("Step", typeof(double), typeof(SpinButtonControl),
                new UIPropertyMetadata(1.0));
            MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(SpinButtonControl),
                new UIPropertyMetadata(0.0));
            MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(SpinButtonControl),
                new UIPropertyMetadata((double) ushort.MaxValue));
            ReadOnlyCaretVisibleProperty = DependencyProperty.Register("ReadOnlyCaretVisible", typeof(bool),
                typeof(SpinButtonControl), new UIPropertyMetadata(true));
            SelectAllTextProperty = DependencyProperty.Register("SelectAllText", typeof(bool),
                typeof(SpinButtonControl),
                new UIPropertyMetadata(false, OnSelectAllTextChanged));
            ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(SpinButtonControl),
                new UIPropertyMetadata("0", OnValueChanged));
        }

        public SpinButtonControl()
        {
            InitializeComponent();

            UpdateValueCommand = new RelayCommand<UpdateCommandParameterInfo>(ExecuteUpdateValue);
        }

        public Brush ButtonBackgroundBrush
        {
            get { return (Brush) GetValue(ButtonBackgroundBrushProperty); }
            set { SetValue(ButtonBackgroundBrushProperty, value); }
        }

        public Brush ButtonBorderBrush
        {
            get { return (Brush) GetValue(ButtonBorderBrushProperty); }
            set { SetValue(ButtonBorderBrushProperty, value); }
        }

        public Brush ButtonArrowBrush
        {
            get { return (Brush) GetValue(ButtonArrowBrushProperty); }
            set { SetValue(ButtonArrowBrushProperty, value); }
        }

        public Brush ForeColor
        {
            get { return (Brush) GetValue(ForeColorProperty); }
            set { SetValue(ForeColorProperty, value); }
        }

        public Brush BackColor
        {
            get { return (Brush) GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }

        public Brush BorderColor
        {
            get { return (Brush) GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public double Step
        {
            get { return (double) GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public double MinValue
        {
            get { return (double) GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public double MaxValue
        {
            get { return (double) GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }


        public bool SelectAllText
        {
            get { return (bool) GetValue(SelectAllTextProperty); }
            set { SetValue(SelectAllTextProperty, value); }
        }

        public string Value
        {
            get { return (string) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public int MaxLength
        {
            get { return (int) GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        public bool IsEditable
        {
            get { return (bool) GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        public SpinStrategy SpinBehavior
        {
            get { return (SpinStrategy) GetValue(SpinBehaviorProperty); }
            set { SetValue(SpinBehaviorProperty, value); }
        }

        public bool ReadOnlyCaretVisible
        {
            get { return DisplayTextBox.IsReadOnlyCaretVisible; }
            set { DisplayTextBox.IsReadOnlyCaretVisible = value; }
        }

        public RelayCommand<UpdateCommandParameterInfo> UpdateValueCommand { get; }
        public double DefaultValue { get; set; }

        private void ExecuteUpdateValue(UpdateCommandParameterInfo parameter)
        {
            //throw new NotImplementedException();
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 1021
            //throw new NotImplementedException();
        }

        private static void OnSelectAllTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // 1432
            //throw new NotImplementedException();
        }
    }
}