﻿#pragma checksum "..\..\..\View\StxEditorView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "A340897EBAE01BDE6374BC7D0B15DBA17F316F92"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using ICSStudio.AvalonEdit;
using ICSStudio.Gui.View;
using ICSStudio.StxEditor.View;
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


namespace ICSStudio.StxEditor.View {
    
    
    /// <summary>
    /// StxEditorView
    /// </summary>
    public partial class StxEditorView : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 104 "..\..\..\View\StxEditorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock DragTextBlock;
        
        #line default
        #line hidden
        
        
        #line 118 "..\..\..\View\StxEditorView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ICSStudio.AvalonEdit.TextEditor Editor;
        
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
            System.Uri resourceLocater = new System.Uri("/ICSStudio.StxEditor;component/view/stxeditorview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\View\StxEditorView.xaml"
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
            this.DragTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.Editor = ((ICSStudio.AvalonEdit.TextEditor)(target));
            
            #line 122 "..\..\..\View\StxEditorView.xaml"
            this.Editor.Loaded += new System.Windows.RoutedEventHandler(this.Editor_Loaded);
            
            #line default
            #line hidden
            
            #line 123 "..\..\..\View\StxEditorView.xaml"
            this.Editor.MouseHover += new System.Windows.Input.MouseEventHandler(this.Editor_MouseHover);
            
            #line default
            #line hidden
            
            #line 124 "..\..\..\View\StxEditorView.xaml"
            this.Editor.MouseHoverStopped += new System.Windows.Input.MouseEventHandler(this.Editor_MouseHoverStopped);
            
            #line default
            #line hidden
            
            #line 125 "..\..\..\View\StxEditorView.xaml"
            this.Editor.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.Editor_OnPreviewMouseWheel);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

