//------------------------------------------------------------------------------
// <copyright file="ControllerOverview.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using ICSStudio.MultiLanguage;

namespace ICSStudio.ControllerOverviewPackage
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
    [Guid("6eb49a1e-25b9-4e94-adac-5f8adcc57526")]
    public class ControllerOverview : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerOverview"/> class.
        /// </summary>
        public ControllerOverview() : base(null)
        {
            this.Caption = LanguageManager.GetInstance().ConvertSpecifier("Controller Overview");
            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            base.Content = new ControllerOverviewControl();
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            this.Caption = LanguageManager.GetInstance().ConvertSpecifier("Controller Overview");
        }
    }
}
