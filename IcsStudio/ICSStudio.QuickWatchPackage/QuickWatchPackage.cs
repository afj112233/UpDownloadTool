//------------------------------------------------------------------------------
// <copyright file="QuickWatchPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Windows;
using ICSStudio.MultiLanguage;

namespace ICSStudio.QuickWatchPackage
{
    using System;
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
    [Guid("346d2c50-4fb6-414c-b76f-f24d07a6d991")]
    public class QuickWatchPackage : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuickWatchPackage"/> class.
        /// </summary>
        public QuickWatchPackage() : base(null)
        {
            this.Caption = LanguageManager.GetInstance().ConvertSpecifier("Watch");
          
            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new QuickWatchPackageControl();
            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            this.Caption = LanguageManager.GetInstance().ConvertSpecifier("Watch");
        }
    }
}
