using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    public class BoolValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string errorContent;

            if (value == null)
            {
                errorContent = "value is null!";
                MessageBox.Show(errorContent);
                return new ValidationResult(false, errorContent);
            }

            var input = value.ToString();
            if (input.Equals("0")
                || input.Equals("1")
                || input.Equals("2#0")
                || input.Equals("2#1")
                || input.Equals("8#0")
                || input.Equals("8#1")
                || input.Equals("16#0")
                || input.Equals("16#1"))
                return ValidationResult.ValidResult;

            errorContent = "Value string invalid.";
            MessageBox.Show(errorContent);
            return new ValidationResult(false, errorContent);
        }
    }
}