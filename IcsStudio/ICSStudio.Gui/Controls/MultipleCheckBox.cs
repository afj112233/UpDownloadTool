using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSStudio.Gui.Controls
{
    public class MultipleCheckBox : Control
    {
        static MultipleCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultipleCheckBox),
                new FrameworkPropertyMetadata(typeof(MultipleCheckBox)));
        }

        public MultipleCheckBox()
        {
            this.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(MouseClick));
            Width = 20;
            Height = 20;
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(MultipleCheckBox), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private void MouseClick(object sender, EventArgs e)
        {
            if (CheckType == CheckType.All)
            {
                CheckType = CheckType.Null;
            }
            else if (CheckType == CheckType.Null)
            {
                CheckType = CanChooseAll ? CheckType.All : CheckType.Half;
            }
            else
            {
                CheckType = CanChooseAll ? CheckType.All : CheckType.Null;
            }

            Command?.Execute(null);
        }

        public static readonly DependencyProperty CanChooseAllProperty = DependencyProperty.Register(
            "CanChooseAll", typeof(bool), typeof(MultipleCheckBox), new PropertyMetadata(default(bool)));

        public bool CanChooseAll
        {
            get { return (bool)GetValue(CanChooseAllProperty); }
            set { SetValue(CanChooseAllProperty, value); }
        }

        public static readonly DependencyProperty CheckTypeProperty = DependencyProperty.Register(
            "CheckType", typeof(CheckType), typeof(MultipleCheckBox), new PropertyMetadata(default(CheckType)));

        public CheckType CheckType
        {
            get { return (CheckType)GetValue(CheckTypeProperty); }
            set { SetValue(CheckTypeProperty, value); }
        }

        public static readonly DependencyProperty VisibilityCrossProperty = DependencyProperty.Register(
            "VisibilityCross", typeof(Visibility), typeof(MultipleCheckBox), new PropertyMetadata(default(Visibility)));

        public Visibility VisibilityCross
        {
            get { return (Visibility)GetValue(VisibilityCrossProperty); }
            set { SetValue(VisibilityCrossProperty, value); }
        }

        public static readonly DependencyProperty VisibilityHookProperty = DependencyProperty.Register(
            "VisibilityHook", typeof(Visibility), typeof(MultipleCheckBox), new PropertyMetadata(default(Visibility)));

        public Visibility VisibilityHook
        {
            get { return (Visibility)GetValue(VisibilityHookProperty); }
            set { SetValue(VisibilityHookProperty, value); }
        }
    }


    public enum CheckType
    {
        Null,
        Half,
        All
    }
}
