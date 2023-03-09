using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Exceptions;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    public class StringValidationRule : ValidationRule
    {
        public ValidationParams Params { get; set; }

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

            try
            {
                var bytes = ValueConverter.ToBytes(input);

                ICompositeField field = Params.Data as ICompositeField;
                if (field != null)
                {
                    if (field.fields.Count == 2)
                    {
                        var lenField = (Int32Field) field.fields[0].Item1;
                        var arrayField = (ArrayField) field.fields[1].Item1;

                        if (lenField != null && arrayField != null)
                        {
                            int maxCount = arrayField.Size();

                            if (bytes.Count >= 0 && bytes.Count <= maxCount)
                                return ValidationResult.ValidResult;
                        }
                    }
                }

                throw new InvalidSizeException();
            }
            catch (Exception ex)
            {
                errorContent = "String invalid.";

                if (ex is InvalidSizeException)
                    errorContent = "Invalid size.";
                if (ex is InvalidCharacterCombinationException)
                    errorContent = "String literal contains an invalid character combination.";
                if (ex is ExtraCharactersException)
                    errorContent = "Extra Characters.";
                if (ex is InvalidCharacterException)
                    errorContent = "Only ASCII characters are supported.";
                if (ex is MissCharacterException)
                    errorContent = "Miss Character.";

                MessageBox.Show(errorContent);
                return new ValidationResult(false, errorContent);
            }

        }
    }

    public class ValidationParams : DependencyObject
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data", typeof(object), typeof(ValidationParams), new FrameworkPropertyMetadata(null));

        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
    }
}