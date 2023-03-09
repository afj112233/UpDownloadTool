//------------------------------------------------------------------------------
// <copyright file="ToolBoxControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell;
using ICSStudio.MultiLanguage;

namespace ICSStudio.ToolBoxPackage
{

    /// <summary>
    /// Interaction logic for ToolBoxControl.
    /// </summary>
    public partial class ToolBoxControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolBoxControl"/> class.
        /// </summary>
        public ToolBoxControl()
        {
            InitializeComponent();

            var viewModel = new ToolBoxControlVM();
            DataContext = viewModel;
            LanguageManager.GetInstance().SetLanguage(this);
        }

        private void TreeViewItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ToolBoxItem item = ((TreeViewItem)sender).Header as ToolBoxItem;
            if (item != null && item.Kind != ToolBoxItemType.Category)
            {
                string mnemonic = item.DisplayName;
                EditorPackage.RLLEditor.RLLEditorControl.InsertElement(mnemonic);
            }
        }
    }
}