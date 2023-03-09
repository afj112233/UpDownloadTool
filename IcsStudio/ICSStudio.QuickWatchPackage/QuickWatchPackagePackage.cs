//------------------------------------------------------------------------------
// <copyright file="QuickWatchPackagePackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using ICSStudio.UIInterfaces.QuickWatch;
using Microsoft.VisualStudio.Shell.Interop;
using Type = System.Type;

namespace ICSStudio.QuickWatchPackage
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
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(QuickWatchPackage))]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    [ProvideAutoLoad(UIContextGuids.NoSolution)]
    [Guid(QuickWatchPackagePackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class QuickWatchPackagePackage : Package, SQuickWatchService, IQuickWatchService
    {
        /// <summary>
        /// QuickWatchPackagePackage GUID string.
        /// </summary>
        public const string PackageGuidString = "7916416d-a7a9-4290-9f75-8c36cffe4474";

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickWatchPackage"/> class.
        /// </summary>
        public QuickWatchPackagePackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
            IServiceContainer serviceContainer = this;
            ServiceCreatorCallback creationCallback = CreateService;
            serviceContainer.AddService(typeof(SQuickWatchService),
                creationCallback, true);
        }

        private object CreateService(IServiceContainer container, Type serviceType)
        {
            if (container != this) return null;

            if (typeof(SQuickWatchService) == serviceType) return this;

            return null;
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            QuickWatchPackageCommand.Initialize(this);
            base.Initialize();
        }

        #endregion

        public void UnlockQuickWatch()
        {
            var control = QuickWatchPackageCommand.Instance.GetPane().Content as QuickWatchPackageControl;
            control?.UnlockQuickWatch();
        }

        public void LockQuickWatch()
        {
            var control = QuickWatchPackageCommand.Instance.GetPane().Content as QuickWatchPackageControl;
            control?.Clean();
            control?.LockQuickWatch();
        }

        public void AddMonitorRoutine(IRoutine routine)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    var control = QuickWatchPackageCommand.Instance.GetPane().Content as QuickWatchPackageControl;
                    control?.AddMonitorRoutine(routine);
                });
        }

        public void RemoveMonitorRoutine(IRoutine routine)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    var control = QuickWatchPackageCommand.Instance.GetPane().Content as QuickWatchPackageControl;
                    control?.RemoveMonitorRoutine(routine);
                });
        }

        public void AddScope(ITagCollectionContainer container)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    var control = QuickWatchPackageCommand.Instance.GetPane().Content as QuickWatchPackageControl;
                    control?.AddScope(container);
                    control?.AddExplicitScope(container);
                });
        }

        public void RemoveScope(ITagCollectionContainer container)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(
                async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    var control = QuickWatchPackageCommand.Instance.GetPane().Content as QuickWatchPackageControl;
                    control?.RemoveScope(container);
                });
        }

        public void AddExplicitScope(ITagCollectionContainer container)
        {
            var control = QuickWatchPackageCommand.Instance.GetPane().Content as QuickWatchPackageControl;
            control?.AddExplicitScope(container);
        }

        public void RemoveExplicitScope(ITagCollectionContainer container)
        {
            var control = QuickWatchPackageCommand.Instance.GetPane().Content as QuickWatchPackageControl;
            control?.RemoveExplicitScope(container);
        }

        public void SetAoiMonitor(IRoutine routine, AoiDataReference reference)
        {
            var control = QuickWatchPackageCommand.Instance.GetPane().Content as QuickWatchPackageControl;
            control?.SetAoiMonitor(routine, reference);

        }

        public void Clean()
        {
            var control = QuickWatchPackageCommand.Instance.GetPane().Content as QuickWatchPackageControl;
            control?.Clean();
        }

        public void Reset()
        {
            Clean();

            var projectInfoService =
                Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;

            var project = projectInfoService?.CurrentProject;

            if (project != null)
            {
                UnlockQuickWatch();
            }
            else
            {
                LockQuickWatch();
            }

        }

        public void Hide()
        {
            var pane = QuickWatchPackageCommand.Instance.GetPane();
            ((IVsWindowFrame) pane.Frame).Hide();
        }

        public bool IsVisible()
        {
            var pane = QuickWatchPackageCommand.Instance.GetPane();
            int result;
            ((IVsWindowFrame) pane.Frame).IsOnScreen(out result);
            return result==1;
        }
        
        public void Show()
        {
            QuickWatchPackageCommand.Instance.Show(this,new EventArgs());
        }
    }
}
