using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ICSStudio.Components.Converters;
using ICSStudio.Components.Model;

namespace ICSStudio.Components.Controls
{
    /// <summary>
    /// TagSelectorAutoCompleteControl.xaml 的交互逻辑
    /// </summary>
    public partial class TagSelectorAutoCompleteControl : IEditableControl
    {
        public static readonly DependencyProperty TagNameProperty =
            DependencyProperty.Register(nameof(TagName), typeof(string), typeof(TagSelectorAutoCompleteControl));

        public static readonly DependencyProperty UsageProperty =
            DependencyProperty.Register(nameof(Usage), typeof(string), typeof(TagSelectorAutoCompleteControl));

        public static readonly DependencyProperty BrowsingParametersProperty =
            DependencyProperty.Register(nameof(BrowsingParameters), typeof(bool),
                typeof(TagSelectorAutoCompleteControl));

        public static readonly DependencyProperty BrowsingModulesProperty =
            DependencyProperty.Register(nameof(BrowsingModules), typeof(bool), typeof(TagSelectorAutoCompleteControl),
                new PropertyMetadata(false));

        public static readonly DependencyProperty ForConnectionProperty =
            DependencyProperty.Register(nameof(ForConnection), typeof(bool), typeof(TagSelectorAutoCompleteControl));

        public static readonly DependencyProperty IsFocusWithinProperty =
            DependencyProperty.Register(nameof(IsFocusWithin), typeof(bool), typeof(TagSelectorAutoCompleteControl),
                new FrameworkPropertyMetadata());

        public static readonly DependencyProperty IsInvisibleProperty = DependencyProperty.Register(nameof(IsInvisible),
            typeof(bool), typeof(TagSelectorAutoCompleteControl),
            new FrameworkPropertyMetadata(false,
                HandleIsInvisibleChanged));

        public static readonly DependencyProperty UpdateTagNameImmediatelyProperty =
            DependencyProperty.Register(nameof(UpdateTagNameImmediately), typeof(bool),
                typeof(TagSelectorAutoCompleteControl),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty OverflowTooltipProperty =
            DependencyProperty.Register(nameof(OverflowTooltip), typeof(string), typeof(TagSelectorAutoCompleteControl),
                new FrameworkPropertyMetadata(string.Empty));

        private static readonly RoutedEvent EditCommittedEvent =
            EditableControlEvents.EditCommittedEvent.AddOwner(typeof(TagSelectorAutoCompleteControl));

        private static readonly RoutedEvent EditCanceledEvent =
            EditableControlEvents.EditCanceledEvent.AddOwner(typeof(TagSelectorAutoCompleteControl));

        // ReSharper disable once InconsistentNaming
        private static readonly RoutedCommand _launchTagBrowser = new RoutedCommand();

        public static readonly DependencyProperty ProgramScopeProperty =
            TagAutoCompleteControl.ProgramScopeProperty.AddOwner(typeof(TagSelectorAutoCompleteControl),
                new FrameworkPropertyMetadata()
                {
                    Inherits = true
                });

        public static readonly DependencyProperty IncludeControllerScopeProperty =
            TagAutoCompleteControl.IncludeControllerScopeProperty.AddOwner(typeof(TagSelectorAutoCompleteControl),
                new FrameworkPropertyMetadata()
                {
                    Inherits = true
                });

        public static readonly DependencyProperty ForceFullScopeProperty =
            TagAutoCompleteControl.ForceFullScopeProperty.AddOwner(typeof(TagSelectorAutoCompleteControl),
                new FrameworkPropertyMetadata()
                {
                    Inherits = true
                });

        private string _storedTagName;
        private bool _isEditing;

        static TagSelectorAutoCompleteControl()
        {
            TagAutoCompleteControl.ProgramScopeProperty.OverrideMetadata(typeof(TagAutoCompleteControl),
                new FrameworkPropertyMetadata()
                {
                    Inherits = true
                });
            TagAutoCompleteControl.IncludeControllerScopeProperty.OverrideMetadata(typeof(TagAutoCompleteControl),
                new FrameworkPropertyMetadata()
                {
                    Inherits = true
                });
            TagAutoCompleteControl.ForceFullScopeProperty.OverrideMetadata(typeof(TagAutoCompleteControl),
                new FrameworkPropertyMetadata()
                {
                    Inherits = true
                });
        }

        public TagSelectorAutoCompleteControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler EditCommitted
        {
            add { AddHandler(EditCommittedEvent, value); }
            remove { RemoveHandler(EditCommittedEvent, value); }
        }

        public event RoutedEventHandler EditCanceled
        {
            add { AddHandler(EditCanceledEvent, value); }
            remove { RemoveHandler(EditCanceledEvent, value); }
        }

        public event EventHandler<TextChangedEventArgs> TagNameChanged;

        public static RoutedCommand LaunchTagBrowser => _launchTagBrowser;

        public bool IsFocusWithin
        {
            get { return (bool) GetValue(IsFocusWithinProperty); }
            private set { SetValue(IsFocusWithinProperty, value); }
        }

        public bool IsInvisible
        {
            get { return (bool) GetValue(IsInvisibleProperty); }
            set { SetValue(IsInvisibleProperty, value); }
        }

        public bool UpdateTagNameImmediately
        {
            get { return (bool) GetValue(UpdateTagNameImmediatelyProperty); }
            set
            {
                UpdateTagNameBindingTrigger(value);
                SetValue(UpdateTagNameImmediatelyProperty, value);
            }
        }

        public string OverflowTooltip
        {
            get { return GetValue(OverflowTooltipProperty).ToString(); }
            set { SetValue(OverflowTooltipProperty, value); }
        }

        public bool IsEditing
        {
            get { return _isEditing; }
            private set { _isEditing = value; }
        }

        public string TagName
        {
            get { return (string) GetValue(TagNameProperty); }
            set { SetValue(TagNameProperty, value); }
        }

        public string ProgramScope
        {
            get { return (string) GetValue(ProgramScopeProperty); }
            set { SetValue(ProgramScopeProperty, value); }
        }

        public string Usage
        {
            get { return (string) GetValue(UsageProperty); }
            set { SetValue(UsageProperty, value); }
        }

        public bool BrowsingParameters
        {
            get { return (bool) GetValue(BrowsingParametersProperty); }
            set { SetValue(BrowsingParametersProperty, value); }
        }

        public bool BrowsingModules
        {
            get { return (bool) GetValue(BrowsingModulesProperty); }
            set { SetValue(BrowsingModulesProperty, value); }
        }

        public bool ForConnection
        {
            get { return (bool) GetValue(ForConnectionProperty); }
            set { SetValue(ForConnectionProperty, value); }
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

        public TagPathValidationType Validation
        {
            get { return TagNameBox.Validation; }
            set { TagNameBox.Validation = value; }
        }

        public bool FocusAndSelectContents()
        {
            if (!TagNameBox.Focus())
                return false;
            TagNameBox.SelectAll();
            return true;
        }

        public bool FocusAndExpandContents()
        {
            if (!TagNameBox.Focus())
                return false;
            LaunchTagBrowserDialog();
            return true;
        }

        public void BeginEdit()
        {
            _storedTagName = TagName;
            IsEditing = true;
        }

        public void CancelEdit()
        {
            IsEditing = false;
            if (TagNameBox.Text == _storedTagName)
                return;

            GetBindingExpression(TagNameProperty)?.UpdateTarget();
            TagNameBox.Text = TagName;
            OnEditCanceled();
        }

        public void CommitEdit(bool forceUpdateSource)
        {
            IsEditing = false;
            if (!forceUpdateSource && TagNameBox.Text == _storedTagName)
                return;
            GetBindingExpression(TagNameProperty)?.UpdateSource();
            OnEditCommitted();
        }

        protected virtual void OnTagNameChanged(TextChangedEventArgs e)
        {
            EventHandler<TextChangedEventArgs> tagNameChanged = TagNameChanged;
            tagNameChanged?.Invoke(this, e);
        }

        protected virtual void OnEditCanceled()
        {
            RaiseEvent(new RoutedEventArgs(EditCanceledEvent, this));
        }

        protected virtual void OnEditCommitted()
        {
            RaiseEvent(new RoutedEventArgs(EditCommittedEvent, this));
        }

        private static void HandleIsInvisibleChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement stateGroupsRoot = sender as FrameworkElement;
            if (stateGroupsRoot == null)
                return;

            if ((bool) e.NewValue)
                VisualStateManager.GoToElementState(stateGroupsRoot, "Invisible", false);
            else
                VisualStateManager.GoToElementState(stateGroupsRoot, "Visible", false);
        }

        private void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;
            OnTagNameChanged(e);
            OverflowTooltip = TagNameBox.Tag.ToString();
        }

        private void TextBoxSelectAll(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            TagNameBox.SelectAll();
        }

        private void HandleTagBrowserButtonClick(object sender, RoutedEventArgs e)
        {
            LaunchTagBrowserDialog();
        }

        private void ExecuteLaunchTagBrowser(object sender, ExecutedRoutedEventArgs e)
        {
            LaunchTagBrowserDialog();
        }

        private void HandleRootLoaded(object sender, RoutedEventArgs e)
        {
            MultiBinding multiBinding = new MultiBinding()
            {
                Converter = new BooleanOrMultiConverter()
            };
            multiBinding.Bindings.Add(new Binding("IsFocused")
            {
                Source = TagNameBox
            });
            multiBinding.Bindings.Add(new Binding("IsFocused")
            {
                Source = TagBrowserButton
            });
            SetBinding(IsFocusWithinProperty, multiBinding);
            TagNameBox.Foreground = IsInvisible ? Brushes.Transparent : SystemColors.ControlTextBrush;
            UpdateTagNameBindingTrigger(UpdateTagNameImmediately);
            OverflowTooltip = new ClippedTextBoxTooltipConverter().Convert(new object[]
            {
                TagNameBox.Text,
                TagNameBox
            }, null, null, null).ToString();
        }

        private void HandleRootGotFocus(object sender, RoutedEventArgs e)
        {
            TagNameBox.Focus();
            e.Handled = true;
        }

        private void LaunchTagBrowserDialog()
        {
            throw new NotImplementedException();
            //if (this.BrowsingModules)
            //{
            //    ModuleBrowserDialogWrapper browserDialogWrapper = new ModuleBrowserDialogWrapper();
            //    if (!browserDialogWrapper.ShowDialog(this.TagName))
            //        return;
            //    this.TagName = browserDialogWrapper.Selection;
            //}
            //else
            //{
            //    Window window = Window.GetWindow((DependencyObject) this);
            //    IntPtr parentHwnd = (IntPtr) (void*) null;
            //    if (window != null)
            //        parentHwnd = new WindowInteropHelper(window).Handle;
            //    TagBrowserDialogWrapper browserDialogWrapper = new TagBrowserDialogWrapper();
            //    if (string.IsNullOrEmpty(this.ProgramScope))
            //    {
            //        if (!browserDialogWrapper.ShowDialog(this.TagName, parentHwnd))
            //            return;
            //        this.TagName = browserDialogWrapper.Selection;
            //    }
            //    else
            //    {
            //        if (!browserDialogWrapper.ShowDialog(this.TagName, this.ProgramScope, this.BrowsingParameters,
            //            this.ForConnection, this.Usage, parentHwnd))
            //            return;
            //        this.TagName = browserDialogWrapper.Selection;
            //    }
            //}
        }

        private void UpdateTagNameBindingTrigger(bool updateImmediately)
        {
            Binding parentBinding = BindingOperations
                .GetBindingExpression(TagNameBox, TextBox.TextProperty)
                ?.ParentBinding;

            Contract.Assert(parentBinding != null);

            Binding binding = new Binding
            {
                ElementName = parentBinding.ElementName,
                Path = parentBinding.Path,
                Mode = parentBinding.Mode
            };

            if (updateImmediately)
            {
                if (parentBinding.UpdateSourceTrigger != UpdateSourceTrigger.PropertyChanged)
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            }
            else if (parentBinding.UpdateSourceTrigger != UpdateSourceTrigger.Default)
                binding.UpdateSourceTrigger = UpdateSourceTrigger.Default;

            TagNameBox.SetBinding(TextBox.TextProperty, binding);
        }
    }
}
