﻿#pragma checksum "..\..\..\..\AddOnInstruction\Panel\LocalTags.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "DA5605D8281E3157F418E490EC3E618162AD8DE5"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using AttachedCommandBehavior;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Components.Controls;
using ICSStudio.Gui.View;
using ICSStudio.UIServicesPackage.AddOnInstruction;
using ICSStudio.UIServicesPackage.AddOnInstruction.Panel;
using ICSStudio.UIServicesPackage.AddOnInstruction.Panel.Validate;
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


namespace ICSStudio.UIServicesPackage.AddOnInstruction {
    
    
    /// <summary>
    /// LocalTags
    /// </summary>
    public partial class LocalTags : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 40 "..\..\..\..\AddOnInstruction\Panel\LocalTags.xaml"
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
            System.Uri resourceLocater = new System.Uri("/ICSStudio.UIServicesPackage;component/addoninstruction/panel/localtags.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\AddOnInstruction\Panel\LocalTags.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
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
            
            #line 44 "..\..\..\..\AddOnInstruction\Panel\LocalTags.xaml"
            this.MainDataGrid.BeginningEdit += new System.EventHandler<System.Windows.Controls.DataGridBeginningEditEventArgs>(this.MainDataGrid_OnBeginningEdit);
            
            #line default
            #line hidden
            
            #line 48 "..\..\..\..\AddOnInstruction\Panel\LocalTags.xaml"
            this.MainDataGrid.CellEditEnding += new System.EventHandler<System.Windows.Controls.DataGridCellEditEndingEventArgs>(this.DataGrid_OnCellEditEnding);
            
            #line default
            #line hidden
            
            #line 49 "..\..\..\..\AddOnInstruction\Panel\LocalTags.xaml"
            this.MainDataGrid.CurrentCellChanged += new System.EventHandler<System.EventArgs>(this.DataGrid_OnCurrentCellChanged);
            
            #line default
            #line hidden
            
            #line 50 "..\..\..\..\AddOnInstruction\Panel\LocalTags.xaml"
            this.MainDataGrid.PreviewKeyUp += new System.Windows.Input.KeyEventHandler(this.MainDataGrid_OnPreviewKeyUp);
            
            #line default
            #line hidden
            
            #line 53 "..\..\..\..\AddOnInstruction\Panel\LocalTags.xaml"
            this.MainDataGrid.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.MainDataGrid_OnSelectionChanged);
            
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
            
            #line 260 "..\..\..\..\AddOnInstruction\Panel\LocalTags.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AutoButton_OnClick);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

