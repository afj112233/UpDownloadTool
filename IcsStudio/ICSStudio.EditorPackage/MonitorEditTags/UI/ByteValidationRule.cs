using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Exceptions;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    public class ByteValidationRule : IntegerValidationRule
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
                    if (bytes.Count == 1)
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

                    var unused = Convert.ToByte(input, 2);
                }
                else if (input.StartsWith("8#"))
                {
                    input = input.Substring(2, input.Length - 2);
                    input = input.Replace("_", "");

                    var unused = Convert.ToByte(input, 8);
                }
                else if (input.StartsWith("16#"))
                {
                    input = input.Substring(3, input.Length - 3);
                    input = input.Replace("_", "");

                    var unused = Convert.ToByte(input, 16);
                }

                else if (Wrapper.DisplayStyle == DisplayStyle.Binary)
                {
                    var unusedBinary = Convert.ToByte(input, 2);
                }

                else if (Wrapper.DisplayStyle == DisplayStyle.Octal)
                {
                    var unusedOctal = Convert.ToByte(input, 8);
                }

                else if (Wrapper.DisplayStyle == DisplayStyle.Hex)
                {
                    var unusedHex = Convert.ToByte(input, 16);
                }

                else
                {
                    var unused = byte.Parse(input);
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