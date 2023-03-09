using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.QuickWatchPackage.View.UI
{
    internal class DataTypeValidationRule : ValidationRule
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

            // aoi
            bool isInAoi = scope is AoiDefinition;

            if (value != null)
            {
                var dataType = value.ToString();
                if (tagItem.Tag == null || !string.Equals(tagItem.Tag.DataTypeInfo.ToString(), dataType))
                {
                    string typeName;
                    int dim1, dim2, dim3;
                    int errorCode;

                    var isValid = controller.DataTypes.ParseDataType(
                        dataType, out typeName,
                        out dim1, out dim2, out dim3, out errorCode);

                    if (errorCode == -5)
                        return new ValidationResult(false,
                            "Number,size,or format of dimensions specified for this tag or type is invalid.");

                    if (!isValid)
                        return new ValidationResult(false, "Data type could not be found.");

                    var foundDataType = controller.DataTypes[typeName];



                    if (dim1 > 0 && !foundDataType.SupportsOneDimensionalArray)
                        return new ValidationResult(false, "Cannot create arrays of this data type.");

                    if (dim2 > 0 && !foundDataType.SupportsMultiDimensionalArrays)
                        return new ValidationResult(false, "Cannot create multi dimensional arrays of this data type.");

                    // scope
                    if (isInAoi)
                    {
                        if (IsRecursion(((AoiDefinition) scope).datatype, foundDataType))
                        {
                            return new ValidationResult(false,
                                "Tag cannot be created with data type that directly or indirectly references itself. Recursion is not allowed.");
                        }

                        //TODO(gjc): add MODULE type later
                        if (foundDataType.IsAxisType ||
                            foundDataType.IsMotionGroupType ||
                            foundDataType.IsCoordinateSystemType ||
                            foundDataType.IsMessageType() ||
                            foundDataType.IsAlarmAnalogType() ||
                            foundDataType.IsAlarmDigitalType())
                        {
                            if (tagItem.Usage != Usage.InOut)
                            {
                                return new ValidationResult(false,
                                    "Tag can only be created as InOut parameter in Add-On Instruction.");
                            }
                        }


                        if (tagItem.Usage == Usage.Input || tagItem.Usage == Usage.Output)
                        {
                            if (!foundDataType.IsAtomic)
                                return new ValidationResult(false,
                                    "Input or output parameter must be of supported elementary data type.");

                            if (dim1 > 0)
                                return new ValidationResult(false,
                                    "Invalid array. Input or output parameter must be of supported elementary data type with no dimensions.");
                        }

                        if (tagItem.Usage == Usage.Local && dim2 > 0)
                            return new ValidationResult(false,
                                "Number, size, or format of dimensions specified for this tag or type is invalid.");

                    }
                    else
                    {
                        if ((foundDataType.IsAxisType
                             || foundDataType.IsMotionGroupType
                             || foundDataType.IsCoordinateSystemType)
                            && scope != controller)
                            return new ValidationResult(false, "Tag can only be created at the controller scope.");
                    }



                    // check size, 2MBytes
                    var totalDimension = Math.Max(dim1, 1) * Math.Max(dim2, 1) * Math.Max(dim3, 1);
                    int maxDimension = 2 * 1024 * 1024 / foundDataType.ByteSize;
                    if (totalDimension > maxDimension)
                        return new ValidationResult(false, "The array size exceeds 2MBytes.");

                    // max number
                    if (foundDataType.IsMotionGroupType)
                        if (controller.Tags.Count(tag =>
                            tag.DataTypeInfo.DataType.IsMotionGroupType) >= 1)
                            return new ValidationResult(false,
                                "The maximum number of motion group tags (1) has been reached.");

                    if (foundDataType.IsCoordinateSystemType)
                        if (controller.Tags.Count(tag =>
                            tag.DataTypeInfo.DataType.IsCoordinateSystemType) >= 32)
                            return new ValidationResult(false,
                                "The maximum number of coordinate system tags (32) has been reached.");
                }
            }

            return ValidationResult.ValidResult;
        }

        private bool IsRecursion(IDataType parentType, IDataType memberType)
        {
            if (parentType == memberType)
                return true;

            CompositiveType compositiveType = memberType as CompositiveType;
            if (compositiveType != null)
            {
                foreach (var member in compositiveType.TypeMembers)
                {
                    if (IsRecursion(parentType, member.DataTypeInfo.DataType))
                        return true;
                }
            }

            return false;
        }
    }

    //TODO(gjc): remove later
    internal static class DataTypeExtension
    {
        public static bool IsMessageType(this IDataType dataType)
        {
            if (string.Equals(dataType?.Name, "MESSAGE", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public static bool IsAlarmAnalogType(this IDataType dataType)
        {
            if (string.Equals(dataType?.Name, "ALARM_ANALOG", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public static bool IsAlarmDigitalType(this IDataType dataType)
        {
            if (string.Equals(dataType?.Name, "ALARM_DIGITAL", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}