//------------------------------------------------------------------------------
// <copyright file="LogicalOrganizer.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

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
    [Guid("d24ee47f-6b4c-444a-ae9b-1f83895dec42")]
    public sealed class LogicalOrganizer : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalOrganizer"/> class.
        /// </summary>
        public LogicalOrganizer() : base(null)
        {
            Caption = "Logical Organizer";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            Content = new View.LogicalOrganizerView();
        }
    }
}
