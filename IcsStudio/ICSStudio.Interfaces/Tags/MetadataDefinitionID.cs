using System.Diagnostics.CodeAnalysis;

namespace ICSStudio.Interfaces.Tags
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public enum MetadataDefinitionID : byte
    {
        DefID_Invalid,
        DefID_Comment,
        DefID_MinLimit,
        DefID_MaxLimit,
        DefID_AssocTag,
        DefID_EngUnit,
        DefID_State0,
        DefID_State1,
        DefID_MMD_LocalName,
        DefID_MMD_EnglishName,
        DefID_MMD_LangSwitch,
        DefID_MMD_PassThrough,
        DefID_MMD_PassThroughConcat,
        DefID_MMD_CopyToInstance,
        DefID_MMD_MirrorValFromBase,
        DefID_MMD_AppliesToComps,
        DefID_MMD_AppliesToDataTypes,
        DefID_MMD_ValueType,
        DefID_MMD_DefaultValue,
        DefID_MMD_MinLogixVersion,
        DefID_MMD_EditableInRSL5K,
        DefID_MMD_ProgAccess,
        DefID_MMD_LocalizedDesc,
        DefID_MMD_DisplayGroup,
        DefID_MMD_DisplaySubGroup,
        DefID_CustomProperties,
        DefID_PermissionSet,
        DefID_PrimaryActionSet,
        DefID_Historize,
        DefID_Alarm,
        DefID_AlarmDefinition,
        DefID_ForceDownloadComment,
        DefID_ForceDownloadNoLangSwitchComment,
        DefID_AlarmSet,
        DefID_ShortName,
        DefID_SystemText,
        DefID_SystemNumeric,
    }
}
