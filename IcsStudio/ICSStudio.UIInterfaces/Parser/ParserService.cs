using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;
using ICSStudio.UIInterfaces.Project;

namespace ICSStudio.UIInterfaces.Parser
{
    [Guid("7CAF52BA-78A7-4408-BB0B-04674DA78850")]
    [ComVisible(true)]
    public interface IParserService
    {
        IUnresolvedRoutine Parse(List<string> codeText, IRoutine routine, IProject parentProject = null,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IUnresolvedRoutine> ParseAsync(List<string> codeText, IRoutine routine, IProject parentProject = null,
            CancellationToken cancellationToken = default(CancellationToken));

        IUnresolvedRoutine GetCachedParseInformation(IRoutine routine);

        void Setup(IProject project);
        void Reset();

        event EventHandler<ParseInformationEventArgs> ParseInformationUpdated;
    }

    [Guid("440278A1-F14E-483D-9B2F-86680D754572")]
    public interface SParserService
    {
    }
}