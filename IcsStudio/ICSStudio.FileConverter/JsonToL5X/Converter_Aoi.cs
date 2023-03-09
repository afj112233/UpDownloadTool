using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using ICSStudio.FileConverter.JsonToL5X.Model;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;
using RoutineCollection = ICSStudio.FileConverter.JsonToL5X.Model.RoutineCollection;

namespace ICSStudio.FileConverter.JsonToL5X
{
    public static partial class Converter
    {
        private static AOIDefinitionType ToAOIDefinitionType(AoiDefinition aoiDefinition)
        {
            AOIDefinitionType aoiDefinitionType = new AOIDefinitionType();

            Contract.Assert(aoiDefinition != null);

            aoiDefinitionType.Name = aoiDefinition.Name;
            aoiDefinitionType.Revision = aoiDefinition.Revision;

            aoiDefinitionType.ExecutePrescan = aoiDefinition.ExecutePrescan ? BoolEnum.@true : BoolEnum.@false;
            aoiDefinitionType.ExecutePostscan = aoiDefinition.ExecutePostscan ? BoolEnum.@true : BoolEnum.@false;
            aoiDefinitionType.ExecuteEnableInFalse =
                aoiDefinition.ExecuteEnableInFalse ? BoolEnum.@true : BoolEnum.@false;

            DateTime createDate = aoiDefinition.CreatedDate;
            DateTime createDateUtc = createDate.ToUniversalTime();
            aoiDefinitionType.CreatedDate = createDateUtc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            aoiDefinitionType.CreatedBy = aoiDefinition.CreatedBy;

            DateTime editedDate = aoiDefinition.EditedDate;
            aoiDefinitionType.EditedDate = editedDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            aoiDefinitionType.EditedBy = aoiDefinition.EditedBy;

            aoiDefinitionType.SoftwareRevision = "v32.01";

            // Parameters
            aoiDefinitionType.Parameters = ToAoiParameters(aoiDefinition);

            // LocalTags
            aoiDefinitionType.LocalTags = ToAoiLocalTags(aoiDefinition);

            // Routines
            aoiDefinitionType.Routines = ToAoiRoutines(aoiDefinition);

            return aoiDefinitionType;
        }

        private static RoutineCollection ToAoiRoutines(AoiDefinition aoiDefinition)
        {

            string[] routineNames = new[] { "EnableInFalse", "Logic", "Postscan", "Prescan" };

            List<object> routineTypes = new List<object>();

            foreach (string routineName in routineNames)
            {
                IRoutine routine = aoiDefinition.Routines[routineName];
                if (routine != null)
                {
                    Model.RoutineType routineType = ToRoutineType(routine);
                    routineTypes.Add(routineType);
                }
            }

            if (routineTypes.Count > 0)
            {
                RoutineCollection routineCollection = new RoutineCollection
                {
                    Items = routineTypes.ToArray()
                };
                return routineCollection;
            }

            return null;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static AOILocalTagType[] ToAoiLocalTags(AoiDefinition aoiDefinition)
        {
            var localTags = aoiDefinition.GetLocalTags();

            List<AOILocalTagType> localTagTypes = new List<AOILocalTagType>();

            if (localTags.Count > 0)
            {
                foreach (var tag in localTags.OfType<Tag>())
                {
                    AOILocalTagType localTagType = new AOILocalTagType();

                    localTagType.Name = tag.Name;
                    localTagType.DataType = tag.DataTypeInfo.DataType.Name;
                    localTagType.Dimensions = GetDimensions(tag.DataTypeInfo, " ");
                    localTagType.Radix = ToRadix(tag.DisplayStyle);
                    if (tag.DataTypeInfo.DataType.IsAtomic)
                    {
                        localTagType.RadixSpecified = true;
                    }

                    localTagType.ExternalAccess = ToExternalAccess(tag.ExternalAccess);

                    List<object> items = new List<object>();

                    DescriptionType descriptionType = ToDescription(tag.Description);
                    if (descriptionType != null)
                        items.Add(descriptionType);

                    Data l5kData = ToL5KData(tag);
                    if (l5kData != null)
                        items.Add(l5kData);

                    Data decoratedData = ToDecoratedData(tag);
                    if (decoratedData != null)
                        items.Add(decoratedData);

                    if (items.Count > 0)
                        localTagType.Items = items.ToArray();

                    localTagTypes.Add(localTagType);
                }

                return localTagTypes.ToArray();
            }

            //TODO(gjc): add code here
            return null;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static AOIParameterType[] ToAoiParameters(AoiDefinition aoiDefinition)
        {
            var parameterTags = aoiDefinition.GetParameterTags();

            List<AOIParameterType> parameterTypes = new List<AOIParameterType>();

            if (parameterTags.Count > 0)
            {
                foreach (var tag in parameterTags.OfType<Tag>())
                {
                    AOIParameterType aoiParameterType = new AOIParameterType();

                    Usage usage = tag.Usage;
                    ExternalAccess externalAccess = tag.ExternalAccess;
                    bool isRequired = tag.IsRequired;
                    bool isVisible = tag.IsVisible;

                    // correct aoi error
                    if (string.Equals(tag.Name, "EnableIn", StringComparison.OrdinalIgnoreCase))
                    {
                        usage = Usage.Input;
                        externalAccess = ExternalAccess.ReadOnly;
                    }

                    if (string.Equals(tag.Name, "EnableOut", StringComparison.OrdinalIgnoreCase))
                    {
                        usage = Usage.Output;
                        externalAccess = ExternalAccess.ReadOnly;
                    }

                    if (usage == Usage.InOut)
                    {
                        isRequired = true;
                        isVisible = true;
                    }


                    aoiParameterType.Name = tag.Name;
                    aoiParameterType.TagType = ToTagTypeEnum(tag.TagType);
                    aoiParameterType.DataType = tag.DataTypeInfo.DataType.Name;
                    aoiParameterType.Dimensions = GetDimensions(tag.DataTypeInfo, " ");
                    aoiParameterType.Usage = ToTagUsageEnum(usage);
                    aoiParameterType.Radix = ToRadix(tag.DisplayStyle);
                    if (tag.DataTypeInfo.DataType.IsAtomic)
                    {
                        aoiParameterType.RadixSpecified = true;
                    }

                    aoiParameterType.Required = isRequired ? BoolEnum.@true : BoolEnum.@false;
                    aoiParameterType.Visible = isVisible ? BoolEnum.@true : BoolEnum.@false;

                    aoiParameterType.Constant = tag.IsConstant ? BoolEnum.@true : BoolEnum.@false;
                    if (usage == Usage.InOut
                        && !tag.DataTypeInfo.DataType.IsMotionGroupType
                        && !tag.DataTypeInfo.DataType.IsAxisType)
                    {
                        if ((!string.Equals(tag.Name, "EnableIn", StringComparison.OrdinalIgnoreCase))
                            && (!string.Equals(tag.Name, "EnableOut", StringComparison.OrdinalIgnoreCase)))
                        {
                            aoiParameterType.ConstantSpecified = true;
                        }
                    }

                    if (externalAccess == ExternalAccess.ReadWrite
                        || externalAccess == ExternalAccess.ReadOnly
                        || externalAccess == ExternalAccess.None)
                    {
                        aoiParameterType.ExternalAccess = ToExternalAccess(externalAccess);

                        aoiParameterType.ExternalAccessSpecified = true;
                    }

                    List<object> items = new List<object>();

                    DescriptionType descriptionType = ToDescription(tag.Description);
                    if (descriptionType != null)
                        items.Add(descriptionType);

                    if ((!string.Equals(tag.Name, "EnableIn", StringComparison.OrdinalIgnoreCase))
                        && (!string.Equals(tag.Name, "EnableOut", StringComparison.OrdinalIgnoreCase)))
                    {
                        Data l5kData = ToL5KData(tag);
                        if (l5kData != null)
                            items.Add(l5kData);

                        Data decoratedData = ToDecoratedData(tag);
                        if (decoratedData != null)
                            items.Add(decoratedData);
                    }

                    if (items.Count > 0)
                        aoiParameterType.Items = items.ToArray();

                    parameterTypes.Add(aoiParameterType);
                }

                return parameterTypes.ToArray();
            }

            return null;
        }

        private static EncodedAOIDefinitionType ToEncodedAOIDefinitionType(AoiDefinition aoiDefinition)
        {
            //TODO(gjc): add code here later
            return null;
        }
    }
}
