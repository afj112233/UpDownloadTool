using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    public class SingleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string errorContent;

            if (value == null)
            {
                errorContent = "Value is null!";
                MessageBox.Show(errorContent);
                return new ValidationResult(false, errorContent);
            }

            var input = value.ToString();

            if (string.Equals(input, "1.$")
                || string.Equals(input, "-1.$")
                || string.Equals(input, "1.#QNAN")
                || string.Equals(input, "-1.#QNAN"))
                return ValidationResult.ValidResult;


            float result;
            if (float.TryParse(input, out result))
                return ValidationResult.ValidResult;

            errorContent = "Value string invalid.";
            MessageBox.Show(errorContent);
            return new ValidationResult(false, errorContent);
        }
    }
}