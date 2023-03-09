using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ICSStudio.AvalonEdit.Rendering;

namespace ICSStudio.AvalonEdit.Variable
{
    public partial class VariableInfo
    {
        private Visibility _bottomEditTextBoxVisibility=Visibility.Collapsed;
        private Visibility _topEditTextBoxVisibility=Visibility.Collapsed;
        private Dictionary<TextView,List<InLineTextBox>> _inlineDictionary=new Dictionary<TextView, List<InLineTextBox>>();
        internal InLineTextBox GetInlineTextBox(TextView textView)
        {
            if (!_inlineDictionary.ContainsKey(textView))
            {
                var list=new List<InLineTextBox>();
                var displayTextBox = new InLineTextBox(false) {Name = textView.Name};
                displayTextBox.Background = new SolidColorBrush(Color.FromArgb(95, 204, 255, 204));
                displayTextBox.IsReadOnly = true;
                displayTextBox.TextPosition = TextLocation;
                displayTextBox.LineOffset = this.LineOffset;
                displayTextBox.Offset = this.Offset;
                displayTextBox.DataContext = this;
                displayTextBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                displayTextBox.VerticalContentAlignment = VerticalAlignment.Center;
                displayTextBox.Padding = new Thickness(1, -1, 0, 0);
                var binding = new Binding("Value");
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                var validate = new ValidateVariable();
                validate.VariableInfo = this;
                binding.ValidationRules.Add(validate);
                displayTextBox.SetBinding(TextBox.TextProperty, binding);
                bool isEnabled = !(this.Value == "{...}" || this.Value == "??");
                if (this.IsEnabled != isEnabled) this.IsEnabled = isEnabled;
                
                binding = new Binding("IsEnabled");
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                displayTextBox.SetBinding(UIElement.IsEnabledProperty, binding);

                list.Add(displayTextBox);
                _inlineDictionary.Add(textView,list);
                return displayTextBox;
            }
            else
            {
                var list = _inlineDictionary[textView];
                Debug.Assert(list.Count>0);
                return list[0];
            }
        }

        internal void SetInlineVisibility(Visibility visibility,bool isTop,TextView textView)
        {
            if (isTop)
            {
                TopEditTextBoxVisibility = visibility;
            }
            else
            {
                BottomEditTextBoxVisibility = visibility;
            }
            //显示样式未能正常恢复
            if (textView!=null&&visibility != Visibility.Visible)
            {
                var inLineTextBox = GetInlineTextBox(textView);
                inLineTextBox.RecoverState(true);
            }
        }

        internal InLineTextBox GetEditTextBox(TextView textView)
        {
            var list = _inlineDictionary[textView];
            Debug.Assert(list!=null);
            if (list.Count<2)
            {
                var editTextBox = new InLineTextBox(true) {Name = textView.Name};
                editTextBox.Background= new SolidColorBrush(Colors.White);
                editTextBox.BorderThickness=new Thickness(1);
                editTextBox.AllowDrop = false;
                editTextBox.TextPosition = TextLocation;
                editTextBox.LineOffset = this.LineOffset;
                editTextBox.Offset = this.Offset;
                editTextBox.DataContext = this;
                editTextBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                editTextBox.VerticalContentAlignment = VerticalAlignment.Center;
                editTextBox.Padding = new Thickness(1, -1, 0, 0);
                var binding = new Binding("EditValue");
                binding.Mode = BindingMode.TwoWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
                var validate = new ValidateVariable();
                validate.VariableInfo = this;
                binding.ValidationRules.Add(validate);
                editTextBox.SetBinding(TextBox.TextProperty, binding);

                if (textView.Name == "Top")
                {
                    binding = new Binding("TopEditTextBoxVisibility");
                }
                else
                {
                    binding = new Binding("BottomEditTextBoxVisibility");
                }
                editTextBox.SetBinding(UIElement.VisibilityProperty, binding);
                list.Add(editTextBox);
                return editTextBox;
            }
            else
            {
                Debug.Assert(list.Count > 1);
                return list[1];
            }
        }

        internal Visibility GetInlineVisibility(bool isTop)
        {
            if (isTop) return TopEditTextBoxVisibility;
            return BottomEditTextBoxVisibility;
        }
        
        public Visibility TopEditTextBoxVisibility
        {
            set
            {
                if (_topEditTextBoxVisibility != value)
                {
                    _topEditTextBoxVisibility = value;
                    OnPropertyChanged();
                }
            }
            get { return _topEditTextBoxVisibility; }
        }

        public Visibility BottomEditTextBoxVisibility
        {
            set
            {
                if (_bottomEditTextBoxVisibility != value)
                {
                    _bottomEditTextBoxVisibility = value;
                    OnPropertyChanged();
                }
            }
            get { return _bottomEditTextBoxVisibility; }
        }

        internal void AbandonInline(TextView textView)
        {
            if (_inlineDictionary.ContainsKey(textView))
            {
                _inlineDictionary.Remove(textView);
            }
        }

        public double MinWidth { set; get; } = 0;
    }


    public class ValidateVariable : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (VariableInfo == null || (VariableInfo.TopEditTextBoxVisibility != Visibility.Visible &&
                                         VariableInfo.BottomEditTextBoxVisibility != Visibility.Visible))
            {
                return new ValidationResult(true, "");
            }

            VariableInfo.Verify((string)value);
            VariableInfo.IsValueChanged = true;
            return VariableInfo.IsCorrect ? new ValidationResult(true, "") : new ValidationResult(false, "");
        }

        public VariableInfo VariableInfo { set; get; }
    }
}
