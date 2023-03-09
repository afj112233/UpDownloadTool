//------------------------------------------------------------------------------
// <copyright file="ControllerOrganizer.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using ICSStudio.MultiLanguage;
using ICSStudio.OrganizerPackage.View;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.OrganizerPackage
{
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("9aca9868-75b6-4bd2-85e3-34c6e221943b")]
    public sealed class ControllerOrganizer : ToolWindowPane
    {
        private readonly ControllerOrganizerView _controllerOrganizerView;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerOrganizer"/> class.
        /// </summary>
        public ControllerOrganizer() : base(null)
        {
            this.Caption = LanguageManager.GetInstance().ConvertSpecifier("Controller Organizer");
            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
            _controllerOrganizerView = new ControllerOrganizerView();

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            Content = _controllerOrganizerView;
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            this.Caption = LanguageManager.GetInstance().ConvertSpecifier("Controller Organizer");
        }

        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();

            ThreadHelper.ThrowIfNotOnUIThread();
            var track = (ITrackSelection) GetService(typeof(STrackSelection));

            // (gjc):Important!!!
            // Initialize with an empty selection
            // Failure to do this would result in our later calls to 
            // OnSelectChange to be ignored (unless focus is lost
            // and regained).
            var selectionContainer = new SelectionContainer
            {
                SelectableObjects = null,
                SelectedObjects = null
            };

            track.OnSelectChange(selectionContainer);

            _controllerOrganizerView.Initialize(Package as Package, track);
        }
    }
}
