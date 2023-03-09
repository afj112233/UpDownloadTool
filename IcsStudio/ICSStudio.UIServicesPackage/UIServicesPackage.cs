//------------------------------------------------------------------------------
// <copyright file="UIServicesPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft;
using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using ICSStudio.MultiLanguage;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIInterfaces.UI;
using ICSStudio.UIInterfaces.Command;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.Version;
using ICSStudio.UIServicesPackage.Services;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Type = System.Type;


namespace ICSStudio.UIServicesPackage
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
#pragma warning disable VSSDK004 // Use BackgroundLoad flag in ProvideAutoLoad attribute for asynchronous auto load.
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    [ProvideAutoLoad(UIContextGuids.NoSolution)]
#pragma warning restore VSSDK004 // Use BackgroundLoad flag in ProvideAutoLoad attribute for asynchronous auto load.
    [Guid(PackageGuidString)]
    [ProvideService(typeof(SCreateDialogService))]
    [ProvideService(typeof(SProjectInfoService))]
    [ProvideService(typeof(SStudioUIService))]
    [ProvideService(typeof(SCommandService))]
    [ProvideService(typeof(SVersionService))]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class UIServicesPackage : Package
    {
        /// <summary>
        /// UIServicesPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "77c2d22d-9e30-449b-a71a-368ecbe33cef";

        /// <summary>
        /// Initializes a new instance of the <see cref="UIServicesPackage"/> class.
        /// </summary>
        public UIServicesPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.

            IServiceContainer serviceContainer = this;
            ServiceCreatorCallback creationCallback = CreateService;
            serviceContainer.AddService(typeof(SCreateDialogService),
                creationCallback, true);

            serviceContainer.AddService(typeof(SProjectInfoService),
                creationCallback, true);

            serviceContainer.AddService(typeof(SStudioUIService),
                creationCallback, true);

            serviceContainer.AddService(typeof(SCommandService),
                creationCallback, true);

            serviceContainer.AddService(typeof(SVersionService), 
                creationCallback, true);
        }

        #region Package Members

        // ReSharper disable once RedundantOverriddenMember
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        #endregion

        private object CreateService(IServiceContainer container, Type serviceType)
        {
            if (container != this)
            {
                return null;
            }

            if (typeof(SCreateDialogService) == serviceType)
            {
                return new CreateDialogService(this);
            }

            if (typeof(SProjectInfoService) == serviceType)
            {
                return new ProjectInfoService(this);
            }

            if (typeof(SStudioUIService) == serviceType)
            {
                return new StudioUIService(this);
            }

            if (typeof(SCommandService) == serviceType)
            {
                return new CommandService(this);
            }

            if (typeof(SVersionService) == serviceType)
            {
                return new VersionService(this);
            }

            return null;
        }

        protected override int QueryClose(out bool pfCanClose)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var createEditorService = GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            if (createEditorService != null)
                if (createEditorService.IsThereATrendRunning())
                {
                    var message =
                        "One or more trends are collecting data samples. Closing ICS Studio will stop " +
                        "the trend operation(s), and cause trend data to be lost." +
                        "\n\n\nContinue closing ICS Studio?";
                    var result = MessageBox.Show(message, "ICS Studio", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);
                    if (result == MessageBoxResult.No)
                    {
                        pfCanClose = false;
                        return VSConstants.S_OK;
                    }
                }

            var projectInfoService = GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            Assumes.Present(projectInfoService);
            var project = projectInfoService?.CurrentProject;
            var controller = projectInfoService?.Controller;
            if (project?.Controller != null)
                project.GoOffline();
            else
                controller?.GoOffline();

            if (project != null
                && project.IsDirty
                && !string.IsNullOrEmpty(project.Controller?.ProjectLocaleName))
            {
                string fileName = Path.GetFileName(project.Controller.ProjectLocaleName);
                
                string message = LanguageManager.GetInstance().ConvertSpecifier("Project file of") + fileName + LanguageManager.GetInstance().ConvertSpecifier("has been changed.Save the changes ?");

                var result = MessageBox.Show(message, "ICS Studio", MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Cancel)
                {
                    pfCanClose = false;
                    return VSConstants.S_OK;
                }

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        int saveResult = projectInfoService.Save(false);
                        if (saveResult < 0)
                        {
                            pfCanClose = false;
                            return VSConstants.S_OK;
                        }
                    }
                    catch (Exception)
                    {
                        pfCanClose = false;
                        return VSConstants.S_OK;
                    }

                }

            }

            pfCanClose = true;
            return VSConstants.S_OK;
        }
    }
}
