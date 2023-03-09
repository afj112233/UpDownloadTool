//------------------------------------------------------------------------------
// <copyright file="ErrorWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using ICSStudio.ErrorOutputPackage.View;
using ICSStudio.Interfaces.Common;
using ICSStudio.UIInterfaces.Error;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace ICSStudio.ErrorOutputPackage
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ErrorWindowControl.
    /// </summary>
    public partial class ErrorWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorWindowControl"/> class.
        /// </summary>
        public ErrorWindowControl(bool isShowTopBar)
        {
            this.InitializeComponent();
            errorList.Content = new ICSStudio.ErrorOutputPackage.View.ErrorListControl(isShowTopBar);
        }

        public void AddFound(string mes,OrderType orderType, OnlineEditType onlineEditType, object original, int? line, int? offset, int? len = null)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                ((ErrorListControl)errorList.Content)?.AddError(mes, orderType, onlineEditType, line, offset, original,len);
            });
        }

        public void AddInfo(string mes, object original)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                ((ErrorListControl)errorList.Content)?.AddInformation(mes, original);
            });
           
        }

        public void Clean()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                ((ErrorListControl)errorList.Content)?.ClearAll();
            });
        }
    }
}