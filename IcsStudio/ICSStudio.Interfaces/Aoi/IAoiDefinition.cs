using System.Collections.Generic;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.Interfaces.Aoi
{
    public interface IAoiDefinition : IProgramModule, ITrackedComponent, ISourceProtected
    {
        IAoiDefinitionCollection ParentCollection { get; }

        bool IsSealed { get; }

        bool IsEncrypted { get; }

        IEnumerable<IAoiInvocationContext> GetInvocationContexts();
    }
}
