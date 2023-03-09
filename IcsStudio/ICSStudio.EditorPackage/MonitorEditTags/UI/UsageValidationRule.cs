using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows.Controls;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.EditorPackage.MonitorEditTags.UI
{
    internal class UsageValidationRule : ValidationRule
    {
        public ValidationParam Param { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var tagItem = Param?.TagItem;
            var scope = Param?.Scope;
            var controller = Param?.Controller;

            Contract.Assert(tagItem != null);
            Contract.Assert(scope != null);
            Contract.Assert(controller != null);

            // create new item
            if (tagItem.Tag == null)
            {
                return ValidationResult.ValidResult;
            }

            // aoi
            bool isInAoi = scope is AoiDefinition;

            if (value != null)
            {
                Usage newUsage = (Usage) value;

                if (isInAoi && tagItem.Usage != newUsage)
                {
                    var dataTypeInfo = tagItem.DataTypeInfo;

                    if (dataTypeInfo.DataType.IsAxisType ||
                        dataTypeInfo.DataType.IsMotionGroupType ||
                        dataTypeInfo.DataType.IsCoordinateSystemType ||
                        dataTypeInfo.DataType.IsMessageType() ||
                        dataTypeInfo.DataType.IsAlarmAnalogType() ||
                        dataTypeInfo.DataType.IsAlarmDigitalType())
                    {
                        if (newUsage != Usage.InOut)
                        {
                            return new ValidationResult(false,
                                "Tag can only be created as InOut parameter in Add-On Instruction.");
                        }
                    }

                    if (newUsage == Usage.Input || newUsage == Usage.Output)
                    {
                        if (!dataTypeInfo.DataType.IsAtomic)
                            return new ValidationResult(false,
                                "Input or output parameter must be of supported elementary data type.");

                        if (dataTypeInfo.Dim1 > 0)
                            return new ValidationResult(false,
                                "Invalid array. Input or output parameter must be of supported elementary data type with no dimensions.");
                    }

                    if (newUsage == Usage.Local)
                    {
                        if (dataTypeInfo.Dim2 > 0)
                            return new ValidationResult(false,
                                "Number, size, or format of dimensions specified for this tag or type is invalid.");
                    }
                }

            }

            return ValidationResult.ValidResult;
        }
    }
}
