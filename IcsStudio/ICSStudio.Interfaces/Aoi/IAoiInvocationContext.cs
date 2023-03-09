using ICSStudio.Interfaces.Tags;

namespace ICSStudio.Interfaces.Aoi
{
    public interface IAoiInvocationContext : ITagDataContext
    {
        int InvokerUid { get; }

        int Location { get; }

        bool IsDefinitionContext { get; }
    }
}
