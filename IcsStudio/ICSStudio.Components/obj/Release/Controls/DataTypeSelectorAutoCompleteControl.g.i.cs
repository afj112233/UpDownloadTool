﻿#pragma checksum "..\..\..\Controls\DataTypeSelectorAutoCompleteControl.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "F8A098A4938C4BA478FD5515B4587CAB392E4185"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using ICSStudio.Components.Controls;
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


namespace ICSStudio.Components.Controls {
    
    
    /// <summary>
    /// DataTypeSelectorAutoCompleteControl
    /// </summary>
    public partial class DataTypeSelectorAutoCompleteControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\Controls\DataTypeSelectorAutoCompleteControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ICSStudio.Components.Controls.DataTypeSelectorAutoCompleteControl Root;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\Controls\DataTypeSelectorAutoCompleteControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ICSStudio.Components.Controls.DataTypeAutoCompleteControl DataTypeAutoCompleteBox;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\Controls\DataTypeSelectorAutoCompleteControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button DataTypeSelectorButton;
        
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
            System.Uri resourceLocater = new System.Uri("/ICSStudio.Components;component/controls/datatypeselectorautocompletecontrol.xaml" +
                    "", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\DataTypeSelectorAutoCompleteControl.xaml"
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
            this.Root = ((ICSStudio.Components.Controls.DataTypeSelectorAutoCompleteControl)(target));
            
            #line 9 "..\..\..\Controls\DataTypeSelectorAutoCompleteControl.xaml"
            this.Root.Loaded += new System.Windows.RoutedEventHandler(this.HandleRootLoaded);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\Controls\DataTypeSelectorAutoCompleteControl.xaml"
            this.Root.GotFocus += new System.Windows.RoutedEventHandler(this.HandleRootGotFocus);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 14 "..\..\..\Controls\DataTypeSelectorAutoCompleteControl.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ExecuteLaunchDataTypeSelector);
            
            #line default
            #line hidden
            return;
            case 3:
            this.DataTypeAutoCompleteBox = ((ICSStudio.Components.Controls.DataTypeAutoCompleteControl)(target));
            return;
            case 4:
            this.DataTypeSelectorButton = ((System.Windows.Controls.Button)(target));
            
            #line 33 "..\..\..\Controls\DataTypeSelectorAutoCompleteControl.xaml"
            this.DataTypeSelectorButton.Click += new System.Windows.RoutedEventHandler(this.HandleDataTypeSelectorButtonClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

