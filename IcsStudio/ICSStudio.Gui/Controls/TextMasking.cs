using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace ICSStudio.Gui.Controls
{
    public static class TextMasking
    {
        private static readonly DependencyPropertyKey MaskExpressionPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("MaskExpression", typeof(Regex), typeof(TextMasking),
                new FrameworkPropertyMetadata());

        public static readonly DependencyProperty MaskExpressionProperty = MaskExpressionPropertyKey.DependencyProperty;

        public static Regex GetMaskExpression(TextBox textBox)
        {
            if (textBox == null)
                throw new ArgumentNullException(nameof(textBox));

            return textBox.GetValue(MaskExpressionProperty) as Regex;
        }


        public static readonly DependencyProperty MaskProperty = DependencyProperty.RegisterAttached("Mask",
            typeof(string), typeof(TextMasking),
            new FrameworkPropertyMetadata(OnMaskChanged));

        public static string GetMask(TextBox textBox)
        {
            if (textBox == null)
                throw new ArgumentNullException(nameof(textBox));

            return textBox.GetValue(MaskProperty) as string;
        }

        public static void SetMask(TextBox textBox, string mask)
        {
            if (textBox == null)
                throw new ArgumentNullException(nameof(textBox));

            textBox.SetValue(MaskProperty, mask);
        }

        private static void OnMaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //106
            //TODO(gjc): add code here
        }

        public static readonly DependencyProperty ShouldReplaceSpaceProperty =
            DependencyProperty.RegisterAttached("ShouldReplaceSpace", typeof(bool), typeof(TextMasking),
                new FrameworkPropertyMetadata(true));

        public static bool GetShouldReplaceSpace(TextBox textBox)
        {
            if (textBox == null)
                throw new ArgumentNullException(nameof(textBox));

            return (bool) textBox.GetValue(ShouldReplaceSpaceProperty);
        }

        public static void SetShouldReplaceSpace(TextBox textBox, bool shouldReplaceSpace)
        {
            if (textBox == null)
                throw new ArgumentNullException(nameof(textBox));

            textBox.SetValue(ShouldReplaceSpaceProperty, shouldReplaceSpace);
        }

        public static readonly DependencyProperty ReplaceSpaceCharProperty =
            DependencyProperty.RegisterAttached("ReplaceSpaceChar", typeof(char), typeof(TextMasking),
                new FrameworkPropertyMetadata('_'));


    }
}
