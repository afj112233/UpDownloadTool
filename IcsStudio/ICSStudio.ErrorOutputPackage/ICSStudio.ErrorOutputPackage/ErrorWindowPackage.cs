//------------------------------------------------------------------------------
// <copyright file="ErrorWindowPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using ICSStudio.ErrorOutputPackage.View;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Error;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;
using Type = System.Type;

namespace ICSStudio.ErrorOutputPackage
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    [ProvideAutoLoad(UIContextGuids.NoSolution)]
    [ProvideAutoLoad(PackageGuidString, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ErrorWindow), Style = VsDockStyle.Linked,
        Window = EnvDTE.Constants.vsWindowKindOutput, Orientation = ToolWindowOrientation.Right)]
    [ProvideToolWindowVisibility(typeof(ErrorWindow), VSConstants.UICONTEXT.NoSolution_string)]
    [Guid(ErrorWindowPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class ErrorWindowPackage : Package, SErrorOutputService, IErrorOutputService
    {
        private Task _currentTask = null;
        private object _syncRoot = new object();

        /// <summary>
        /// ErrorWindowPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "0a6faef5-9905-45a6-ba94-9a09f56eb33b";

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorWindow"/> class.
        /// </summary>
        public ErrorWindowPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
            IServiceContainer serviceContainer = this;
            ServiceCreatorCallback creationCallback = CreateService;
            serviceContainer.AddService(typeof(SErrorOutputService),
                creationCallback, true);
        }

        private object CreateService(IServiceContainer container, Type serviceType)
        {
            if (container != this) return null;

            if (typeof(SErrorOutputService) == serviceType) return this;

            return null;
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            ErrorWindowCommand.Initialize(this);
            base.Initialize();
        }

        #endregion

        public bool CanCleanUp { get; set; } = true;

        public void AddErrors(string description, OrderType orderType, OnlineEditType onlineEditType, int? line,
            int? offset, object original = null)
        {
            lock (_syncRoot)
            {
                if (_currentTask != null)
                {
                    _currentTask = _currentTask.ContinueWith(async t =>
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        Controller controller = Controller.GetInstance();
                        if (controller != null && onlineEditType == OnlineEditType.Original)
                        {
                            controller.IsPass = false;
                        }

                        var routine = original as IRoutine;
                        if (routine != null)
                        {
                            routine.IsError = true;
                        }

                        ToolWindowPane window = ErrorWindowCommand.Instance.GetPane();
                        IVsWindowFrame windowFrame = (IVsWindowFrame) window?.Frame;
                        if (windowFrame != null)
                            ErrorHandler.ThrowOnFailure(windowFrame.Show());

                        var control = window?.Content as ErrorWindowControl;
                        ((ErrorListControl) control?.errorList.Content)?.AddError(description, orderType,
                            onlineEditType,
                            line, offset, original);
                    });
                }
                else
                {
                    _currentTask = ThreadHelper.JoinableTaskFactory.RunAsync(
                        async delegate
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                            Controller controller = Controller.GetInstance();
                            if (controller != null && onlineEditType == OnlineEditType.Original)
                            {
                                controller.IsPass = false;
                            }

                            var routine = original as IRoutine;
                            if (routine != null)
                            {
                                routine.IsError = true;
                            }

                            ToolWindowPane window = ErrorWindowCommand.Instance.GetPane();
                            IVsWindowFrame windowFrame = (IVsWindowFrame) window?.Frame;
                            if (windowFrame != null)
                                ErrorHandler.ThrowOnFailure(windowFrame.Show());

                            var control = window?.Content as ErrorWindowControl;
                            ((ErrorListControl) control?.errorList.Content)?.AddError(description, orderType,
                                onlineEditType,
                                line, offset, original);
                        }).Task;
                }
            }
        }

        public void AddWarnings(string description, object original = null, int? line = null, int? offset = null,
            Destination destination = Destination.None)
        {
            lock (_syncRoot)
            {
                if (_currentTask == null)
                {
                    _currentTask = ThreadHelper.JoinableTaskFactory.RunAsync(
                        async delegate
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                            ToolWindowPane window = ErrorWindowCommand.Instance.GetPane();
                            IVsWindowFrame windowFrame = (IVsWindowFrame) window?.Frame;
                            if (windowFrame != null)
                                ErrorHandler.ThrowOnFailure(windowFrame.Show());

                            var control = window?.Content as ErrorWindowControl;
                            ((ErrorListControl) control?.errorList.Content)?.AddWarning(description, original, line,
                                offset, destination);
                        }).Task;
                }
                else
                {
                    _currentTask = _currentTask.ContinueWith(async t =>
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        ToolWindowPane window = ErrorWindowCommand.Instance.GetPane();
                        IVsWindowFrame windowFrame = (IVsWindowFrame) window?.Frame;
                        if (windowFrame != null)
                            ErrorHandler.ThrowOnFailure(windowFrame.Show());

                        var control = window?.Content as ErrorWindowControl;
                        ((ErrorListControl) control?.errorList.Content)?.AddWarning(description, original, line,
                            offset, destination);
                    });
                }
            }
        }

        public void AddMessages(string description, object original = null)
        {
            lock (_syncRoot)
            {
                if (_currentTask == null)
                {
                    _currentTask = ThreadHelper.JoinableTaskFactory.RunAsync(
                        async delegate
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                            var control = ErrorWindowCommand.Instance.GetPane().Content as ErrorWindowControl;
                            ((ErrorListControl) control?.errorList.Content)?.AddInformation(description, original);
                        }).Task;
                }
                else
                {
                    _currentTask = _currentTask.ContinueWith(async t =>
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        var control = ErrorWindowCommand.Instance.GetPane().Content as ErrorWindowControl;
                        ((ErrorListControl) control?.errorList.Content)?.AddInformation(description, original);
                    });
                }
            }
        }

        public void RemoveError(object original)
        {
            lock (_syncRoot)
            {
                _currentTask?.ConfigureAwait(false).GetAwaiter().GetResult();
                _currentTask = ThreadHelper.JoinableTaskFactory.RunAsync(
                    async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        var control = ErrorWindowCommand.Instance.GetPane().Content as ErrorWindowControl;
                        ((ErrorListControl) control?.errorList.Content)?.Remove(ErrorListLevel.Error, original);
                    }).Task;
            }
        }

        public void RemoveError(IRoutine original, OnlineEditType onlineEditType)
        {
            lock (_syncRoot)
            {
                _currentTask?.ConfigureAwait(false).GetAwaiter().GetResult();
                _currentTask = ThreadHelper.JoinableTaskFactory.RunAsync(
                    async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        var control = ErrorWindowCommand.Instance.GetPane().Content as ErrorWindowControl;
                        ((ErrorListControl) control?.errorList.Content)?.Remove(ErrorListLevel.Error, original,
                            onlineEditType);
                    }).Task;
            }
        }

        public void RemoveWarning(object original)
        {
            lock (_syncRoot)
            {
                _currentTask?.ConfigureAwait(false).GetAwaiter().GetResult();
                _currentTask = ThreadHelper.JoinableTaskFactory.RunAsync(
                    async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        var control = ErrorWindowCommand.Instance.GetPane().Content as ErrorWindowControl;
                        ((ErrorListControl) control?.errorList.Content)?.Remove(ErrorListLevel.Warning, original);
                    }).Task;
            }
        }

        public void RemoveMessage(object original)
        {
            lock (_syncRoot)
            {
                _currentTask?.ConfigureAwait(false).GetAwaiter().GetResult();
                _currentTask = ThreadHelper.JoinableTaskFactory.RunAsync(
                    async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        var control = ErrorWindowCommand.Instance.GetPane().Content as ErrorWindowControl;
                        ((ErrorListControl) control?.errorList.Content)?.Remove(ErrorListLevel.Information, original);
                    }).Task;
            }
        }

        public void RemoveImportError()
        {
            lock (_syncRoot)
            {
                _currentTask?.ConfigureAwait(false).GetAwaiter().GetResult();
                _currentTask = ThreadHelper.JoinableTaskFactory.RunAsync(
                    async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        var control = ErrorWindowCommand.Instance.GetPane().Content as ErrorWindowControl;
                        ((ErrorListControl) control?.errorList.Content)?.RemoveImportError();
                    }).Task;
            }
        }

        public List<IRoutine> GetErrorRoutines()
        {
            var control = ErrorWindowCommand.Instance.GetPane().Content as ErrorWindowControl;
            return ((ErrorListControl) control?.errorList.Content)?.GetErrorRoutines();
        }

        public void Summary()
        {
            lock (_syncRoot)
            {
                _currentTask?.ConfigureAwait(false).GetAwaiter().GetResult();
                if (_currentTask == null)
                {
                    _currentTask = ThreadHelper.JoinableTaskFactory.RunAsync(
                        async delegate
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                            var control = ErrorWindowCommand.Instance.GetPane().Content as ErrorWindowControl;
                            ((ErrorListControl) control?.errorList.Content)?.Summary();
                        }).Task;
                }
                else
                {
                    _currentTask = _currentTask.ContinueWith(async t =>
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                        var control = ErrorWindowCommand.Instance.GetPane().Content as ErrorWindowControl;
                        ((ErrorListControl) control?.errorList.Content)?.Summary();
                    });
                }
            }
        }

        public void Cleanup()
        {
            lock (_syncRoot)
            {
                if (CanCleanUp)
                {
                    _currentTask?.ConfigureAwait(false).GetAwaiter().GetResult();

                    ThreadHelper.JoinableTaskFactory.RunAsync(
                        async delegate
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                            var control = ErrorWindowCommand.Instance.GetPane().Content as ErrorWindowControl;
                            ((ErrorListControl) control?.errorList.Content)?.ClearAll();
                        });
                }
            }
        }
    }
}
