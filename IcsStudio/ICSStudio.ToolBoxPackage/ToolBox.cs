//------------------------------------------------------------------------------
// <copyright file="ToolBox.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;

namespace ICSStudio.ToolBoxPackage
{
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using MultiLanguage;

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
    [Guid("a6937b6b-1d17-495b-a533-de3a1c444658")]
    public sealed class ToolBox : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolBox"/> class.
        /// </summary>
        public ToolBox() : base(null)
        {
            this.Caption = LanguageManager.GetInstance().ConvertSpecifier("ToolBox");
            LanguageManager.GetInstance().LanguageChanged+= OnLanguageChanged;
            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new ToolBoxControl();
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            this.Caption = LanguageManager.GetInstance().ConvertSpecifier("ToolBox");
        }
    }
}
