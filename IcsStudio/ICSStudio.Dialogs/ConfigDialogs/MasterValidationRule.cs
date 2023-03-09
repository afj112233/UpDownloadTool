using System.Globalization;
using System.Windows.Controls;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    public class MasterValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return ValidationResult.ValidResult;
        }
    }
}
