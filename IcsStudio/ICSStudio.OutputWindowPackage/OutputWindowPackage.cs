//------------------------------------------------------------------------------
// <copyright file="OutputWindowPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------


using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ICSStudio.OutputWindowPackage.Services;
using ICSStudio.UIInterfaces.OutputWindow;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.OutputWindowPackage
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// https://docs.microsoft.com/en-us/visualstudio/extensibility/how-to-provide-an-asynchronous-visual-studio-service?view=vs-2015
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
   
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideService(typeof(SOutputService), IsAsyncQueryable = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class OutputWindowPackage : AsyncPackage
    {
        /// <summary>
        /// OutputWindowPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "3710aabe-9a88-4353-bc6a-45b01389a916";

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputWindowPackage"/> class.
        /// </summary>
        // ReSharper disable once EmptyConstructor
        public OutputWindowPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        //protected override void Initialize()
        //{
        //    base.Initialize();
        //}

        protected override async Task InitializeAsync(CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);
            
            // When initialized asynchronously, we *may* be on a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            // Otherwise, remove the switch to the UI thread if you don't need it.
            // await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            // add command initialize

            AddService(typeof(SOutputService), CreateOutputServiceAsync, true);
        }


        private async Task<object> CreateOutputServiceAsync(IAsyncServiceContainer container,
            CancellationToken cancellationToken, Type serviceType)
        {
            if (serviceType == typeof(SOutputService))
            {
                OutputService service = new OutputService(this);
                await service.InitializeAsync(cancellationToken);
                return service;
            }

            return null;
        }

        #endregion
    }
}
