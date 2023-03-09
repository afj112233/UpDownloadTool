namespace ICSStudio.Interfaces.Common
{
    public interface IBaseComponent : IBaseObject
    {
        string Name { get; set; }

        string Description { get; set; }

        int InstanceNumber { get; }

        bool IsSafety { get; }

        bool IsTypeLess { get; }

        bool IsDescriptionDefaultLocale();

        Language[] GetDescriptionTranslations();
    }
}
