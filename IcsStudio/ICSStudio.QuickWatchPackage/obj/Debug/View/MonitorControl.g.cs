﻿#pragma checksum "..\..\..\View\MonitorControl.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "B73A87C9E4C4B697C920496EF91D40C7B4FCAE6E"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using GalaSoft.MvvmLight.Command;
using ICSStudio.Components.Controls;
using ICSStudio.Gui.Converters;
using ICSStudio.Gui.View;
using ICSStudio.QuickWatchPackage.View;
using ICSStudio.QuickWatchPackage.View.UI;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace ICSStudio.QuickWatchPackage.View {
    
    
    /// <summary>
    /// MonitorControl
    /// </summary>
    public partial class MonitorControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 307 "..\..\..\View\MonitorControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid MainDataGrid;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ICSStudio.QuickWatchPackage;component/view/monitorcontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\View\MonitorControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.MainDataGrid = ((System.Windows.Controls.DataGrid)(target));
            
            #line 310 "..\..\..\View\MonitorControl.xaml"
            this.MainDataGrid.AddingNewItem += new System.EventHandler<System.Windows.Controls.AddingNewItemEventArgs>(this.DataGrid_AddingNewItem);
            
            #line default
            #line hidden
            
            #line 312 "..\..\..\View\MonitorControl.xaml"
            this.MainDataGrid.BeginningEdit += new System.EventHandler<System.Windows.Controls.DataGridBeginningEditEventArgs>(this.DataGrid_OnBeginningEdit);
            
            #line default
            #line hidden
            
            #line 316 "..\..\..\View\MonitorControl.xaml"
            this.MainDataGrid.CellEditEnding += new System.EventHandler<System.Windows.Controls.DataGridCellEditEndingEventArgs>(this.MainDataGrid_OnCellEditEnding);
            
            #line default
            #line hidden
            
            #line 317 "..\..\..\View\MonitorControl.xaml"
            this.MainDataGrid.CurrentCellChanged += new System.EventHandler<System.EventArgs>(this.MainDataGrid_OnCurrentCellChanged);
            
            #line default
            #line hidden
            
            #line 318 "..\..\..\View\MonitorControl.xaml"
            this.MainDataGrid.InitializingNewItem += new System.Windows.Controls.InitializingNewItemEventHandler(this.DataGrid_OnInitializingNewItem);
            
            #line default
            #line hidden
            
            #line 321 "..\..\..\View\MonitorControl.xaml"
            this.MainDataGrid.LoadingRow += new System.EventHandler<System.Windows.Controls.DataGridRowEventArgs>(this.DataGrid_OnLoadingRow);
            
            #line default
            #line hidden
            
            #line 322 "..\..\..\View\MonitorControl.xaml"
            this.MainDataGrid.RowEditEnding += new System.EventHandler<System.Windows.Controls.DataGridRowEditEndingEventArgs>(this.DataGrid_OnRowEditEnding);
            
            #line default
            #line hidden
            
            #line 323 "..\..\..\View\MonitorControl.xaml"
            this.MainDataGrid.AddHandler(System.Windows.Controls.ScrollViewer.ScrollChangedEvent, new System.Windows.Controls.ScrollChangedEventHandler(this.MainDataGrid_OnScrollChanged));
            
            #line default
            #line hidden
            
            #line 326 "..\..\..\View\MonitorControl.xaml"
            this.MainDataGrid.UnloadingRow += new System.EventHandler<System.Windows.Controls.DataGridRowEventArgs>(this.DataGrid_OnUnloadingRow);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 2:
            
            #line 380 "..\..\..\View\MonitorControl.xaml"
            ((ICSStudio.Components.Controls.FastAutoCompleteTextBox)(target)).TextChanged += new System.Windows.RoutedEventHandler(this.AutoCompleteBox_OnTextChanged);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

