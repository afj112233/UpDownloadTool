using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Exceptions;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    public class Int16ValidationRule : IntegerValidationRule
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

            if (Wrapper.DisplayStyle == DisplayStyle.Ascii)
            {
                try
                {
                    var bytes = ValueConverter.ToBytes(input);
                    if (bytes.Count > 0 && bytes.Count <= 2)
                        return ValidationResult.ValidResult;

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

            try
            {
                if (input.StartsWith("2#"))
                {
                    input = input.Substring(2, input.Length - 2);
                    input = input.Replace("_", "");

                    var unused = Convert.ToInt16(input, 2);
                }
                else if (input.StartsWith("8#"))
                {
                    input = input.Substring(2, input.Length - 2);
                    input = input.Replace("_", "");

                    var unused = Convert.ToInt16(input, 8);
                }
                else if (input.StartsWith("16#"))
                {
                    input = input.Substring(3, input.Length - 3);
                    input = input.Replace("_", "");

                    var unused = Convert.ToInt16(input, 16);
                }

                else if (Wrapper.DisplayStyle == DisplayStyle.Binary)
                {
                    var unusedBinary = Convert.ToInt16(input, 2);
                }

                else if (Wrapper.DisplayStyle == DisplayStyle.Octal)
                {
                    var unusedOctal = Convert.ToInt16(input, 8);
                }

                else if (Wrapper.DisplayStyle == DisplayStyle.Hex)
                {
                    var unusedHex = Convert.ToInt16(input, 16);
                }

                else
                {
                    var unused = short.Parse(input);
                }
            }
            catch (Exception)
            {
                errorContent = "String invalid.";
                MessageBox.Show(errorContent);
                return new ValidationResult(false, errorContent);
            }

            return ValidationResult.ValidResult;
        }
    }
}