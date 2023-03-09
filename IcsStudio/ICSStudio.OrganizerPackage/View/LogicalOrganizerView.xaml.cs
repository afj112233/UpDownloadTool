//------------------------------------------------------------------------------
// <copyright file="LogicalOrganizerView.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;

namespace ICSStudio.OrganizerPackage.View
{
    /// <summary>
    /// Interaction logic for LogicalOrganizerView.
    /// </summary>
    public partial class LogicalOrganizerView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalOrganizerView"/> class.
        /// </summary>
        public LogicalOrganizerView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(CultureInfo.CurrentUICulture, "Invoked '{0}'", ToString()),
                "LogicalOrganizer");
        }
    }
}