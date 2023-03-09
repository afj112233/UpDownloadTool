using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    internal class ParserServiceEntry
    {
        private readonly ParserService _parserService;
        private readonly IRoutine _routine;

        private readonly object _runningAsyncParseLock = new object();

        private readonly ReaderWriterLockSlim _rwLock
            = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private IUnresolvedRoutine _currentUnresolvedRoutine;

        public ParserServiceEntry(ParserService parserService, IRoutine routine)
        {
            //TODO(gjc): remove later
            Debug.Assert(routine is RLLRoutine);

            _parserService = parserService;
            _routine = routine;
        }

        public IUnresolvedRoutine CurrentUnresolvedRoutine
        {
            get
            {
                _rwLock.EnterReadLock();

                try
                {
                    return _currentUnresolvedRoutine;

                }
                finally
                {
                    _rwLock.ExitReadLock();
                }
            }
        }

        public IUnresolvedRoutine Parse(
            List<string> codeText, IProject project,
            CancellationToken cancellationToken)
        {
            IUnresolvedRoutine result;

            _rwLock.EnterUpgradeableReadLock();

            try
            {
                // parse
                Debug.Assert(_routine is RLLRoutine);

                RLLInfoExtractor rllInfoExtractor = new RLLInfoExtractor((RLLRoutine)_routine);

                IUnresolvedRoutine newUnresolvedRoutine = rllInfoExtractor.Parse(codeText);
                
                // update result
                _rwLock.EnterWriteLock();

                try
                {
                    IUnresolvedRoutine oldUnresolvedRoutine = _currentUnresolvedRoutine;
                    _currentUnresolvedRoutine = newUnresolvedRoutine;

                    var args = new ParseInformationEventArgs(oldUnresolvedRoutine, newUnresolvedRoutine);
                    _parserService.RaiseParseInformationUpdated(args);

                    result = newUnresolvedRoutine;
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                }
            }
            finally
            {
                _rwLock.ExitUpgradeableReadLock();
            }

            return result;
        }

        public Task<IUnresolvedRoutine> ParseAsync(
            List<string> codeText, IProject project,
            CancellationToken cancellationToken)
        {
            Task<IUnresolvedRoutine> task;

            lock (_runningAsyncParseLock)
            {
                //TODO(gjc): check running parse task later

                //

                var joinableTask = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                    await DoParseAsync(codeText, project, cancellationToken));

                task = joinableTask.Task;
            }

            return task;
        }

        [SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
        private async Task<IUnresolvedRoutine> DoParseAsync(List<string> codeText, IProject project,
            CancellationToken cancellationToken)
        {
            await TaskScheduler.Default;

            try
            {
                return Parse(codeText, project, cancellationToken);
            }
            finally
            {
                lock (_runningAsyncParseLock)
                {
                    //TODO(gjc): add code later
                }
            }
        }
    }
}
