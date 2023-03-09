//------------------------------------------------------------------------------
// <copyright file="ControllerOverviewControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using ICSStudio.MultiLanguage;

namespace ICSStudio.ControllerOverviewPackage
{
    /// <summary>
    /// Interaction logic for ControllerOverviewControl.
    /// </summary>
    public partial class ControllerOverviewControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerOverviewControl"/> class.
        /// </summary>
        public ControllerOverviewControl()
        {
            this.InitializeComponent();

            DataContext = new ControllerOverviewViewModel();

            LanguageManager.GetInstance().SetLanguage(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            LanguageManager.GetInstance().SetLanguage(this);
        }


        private void ForceButton_OnClick(object sender, RoutedEventArgs e)
        {
            ForceMenu.IsOpen = true;
        }

        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            EditMenu.IsOpen = true;
        }
    }
}