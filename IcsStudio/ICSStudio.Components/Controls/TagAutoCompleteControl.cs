using System.Windows;
using System.Windows.Input;
using ICSStudio.Components.Model;

namespace ICSStudio.Components.Controls
{
    public class TagAutoCompleteControl : ValidateNameControl
    {
        public static readonly DependencyProperty ProgramScopeProperty =
            DependencyProperty.Register(nameof(ProgramScope), typeof(string), typeof(TagAutoCompleteControl));

        public static readonly DependencyProperty IncludeControllerScopeProperty =
            DependencyProperty.Register(nameof(IncludeControllerScope), typeof(bool), typeof(TagAutoCompleteControl));

        public static readonly DependencyProperty ForceFullScopeProperty =
            DependencyProperty.Register(nameof(ForceFullScope), typeof(bool), typeof(TagAutoCompleteControl));

        public static readonly DependencyProperty ExpectingModuleProperty =
            DependencyProperty.Register(nameof(ExpectingModule), typeof(bool), typeof(TagAutoCompleteControl),
                new PropertyMetadata(false));

        private ComponentLookupHelper _lookupHelper;

        public string ProgramScope
        {
            get { return (string) GetValue(ProgramScopeProperty); }
            set { SetValue(ProgramScopeProperty, value); }
        }

        public bool IncludeControllerScope
        {
            get { return (bool) GetValue(IncludeControllerScopeProperty); }
            set { SetValue(IncludeControllerScopeProperty, value); }
        }

        public bool ForceFullScope
        {
            get { return (bool) GetValue(ForceFullScopeProperty); }
            set { SetValue(ForceFullScopeProperty, value); }
        }

        public bool ExpectingModule
        {
            get { return (bool) GetValue(ExpectingModuleProperty); }
            set { SetValue(ExpectingModuleProperty, value); }
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            if (!e.Handled && (!string.IsNullOrEmpty(e.Text) && ValidateText(e.Text)))
                e.Handled = AutoComplete(e.Text);
            base.OnTextInput(e);
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (_lookupHelper != null)
            {
                _lookupHelper.Dispose();
                _lookupHelper = null;
            }

            base.OnLostKeyboardFocus(e);
        }

        private bool AutoComplete(string newText)
        {
            if (newText.Length > 1 || Validation == TagPathValidationType.ControllerScopedOperand)
                return false;
            string specifier = (SelectionLength != 0
                ? Text.Remove(SelectionStart, SelectionLength)
                : Text) + newText;
            if (_lookupHelper == null)
                _lookupHelper = new ComponentLookupHelper();
            string str = !ExpectingModule
                ? (!specifier.StartsWith("\\")
                    ? (string.IsNullOrEmpty(ProgramScope) || ForceFullScope
                        ? _lookupHelper.CompleteTagSpecifier(specifier)
                        : _lookupHelper.CompleteTagSpecifier(specifier, ProgramScope,
                            IncludeControllerScope))
                    : _lookupHelper.CompleteFullyScopedSpecifier(specifier))
                : _lookupHelper.CompleteModuleSpecifier(specifier);
            if (string.IsNullOrEmpty(str))
                return false;
            int start = CaretIndex + 1;
            Text = str;
            CaretIndex = str.Length - 1;
            Select(start, str.Length - start);
            return true;
        }
    }
}
