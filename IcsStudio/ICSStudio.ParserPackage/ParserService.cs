using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Parser;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace ICSStudio.ParserPackage
{
    internal class ParserService : IParserService, SParserService
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly Package _package;

        public ParserService(Package package)
        {
            _package = package;
        }

        public IUnresolvedRoutine Parse(List<string> codeText, IRoutine routine, IProject parentProject = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetRoutineEntry(routine, true).Parse(codeText, parentProject, cancellationToken);
        }

        public Task<IUnresolvedRoutine> ParseAsync(
            List<string> codeText, IRoutine routine,
            IProject parentProject = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetRoutineEntry(routine, true).ParseAsync(codeText, parentProject, cancellationToken);
        }

        public IUnresolvedRoutine GetCachedParseInformation(IRoutine routine)
        {
            var routineEntry = GetRoutineEntry(routine, false);

            return routineEntry?.CurrentUnresolvedRoutine;
        }

        public void Setup(IProject project)
        {
            if (project == null)
                return;
            if (project.IsEmpty)
                return;
            if (project.Controller == null)
                return;

            foreach (var program in project.Controller.Programs)
            {
                foreach (var routine in program.Routines)
                {
                    ParseAsync(routine);
                }
            }

            foreach (var aoiDefinition in project.Controller.AOIDefinitionCollection)
            {
                foreach (var routine in aoiDefinition.Routines)
                {
                    ParseAsync(routine);
                }
            }

        }

        private void ParseAsync(IRoutine routine)
        {
            RLLRoutine rllRoutine = routine as RLLRoutine;
            if (rllRoutine != null)
            {
                ParseAsync(rllRoutine.CodeText, rllRoutine);
            }

            //TODO(gjc): add other routine later
        }

        public void Reset()
        {
            lock (_routineEntryDict)
            {
                _routineEntryDict.Clear();
            }
        }

        public event EventHandler<ParseInformationEventArgs> ParseInformationUpdated = delegate { };

        internal void RaiseParseInformationUpdated(ParseInformationEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await TaskScheduler.Default;

                ParseInformationUpdated.Invoke(null, e);
            });
        }

        #region Entry Manage

        readonly Dictionary<IRoutine, ParserServiceEntry> _routineEntryDict
            = new Dictionary<IRoutine, ParserServiceEntry>();

        private ParserServiceEntry GetRoutineEntry(IRoutine routine, bool createIfMissing)
        {
            if (routine == null)
                throw new ArgumentNullException(nameof(routine));

            ParserServiceEntry parserServiceEntry;
            lock (_routineEntryDict)
            {
                if (!_routineEntryDict.TryGetValue(routine, out parserServiceEntry))
                {
                    if (!createIfMissing)
                        return null;

                    parserServiceEntry = new ParserServiceEntry(this, routine);
                    _routineEntryDict.Add(routine, parserServiceEntry);
                }
            }

            return parserServiceEntry;
        }

        #endregion


    }
}
