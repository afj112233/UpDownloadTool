﻿#pragma checksum "..\..\..\Controls\TranslationViewerCtrl.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "BE6C863B8F01302FA37053510212541654FF15B5"
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
    /// TranslationViewerCtrl
    /// </summary>
    public partial class TranslationViewerCtrl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\Controls\TranslationViewerCtrl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ICSStudio.Components.Controls.PopupWithUIA TranslationViewer;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\Controls\TranslationViewerCtrl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel TranslationViewerPanel;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\Controls\TranslationViewerCtrl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox LanguageCombobox;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\Controls\TranslationViewerCtrl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button PasteButton;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Controls\TranslationViewerCtrl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button HelpButton;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\Controls\TranslationViewerCtrl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CloseButton;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\Controls\TranslationViewerCtrl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TranslatedText;
        
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
            System.Uri resourceLocater = new System.Uri("/ICSStudio.Components;component/controls/translationviewerctrl.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\TranslationViewerCtrl.xaml"
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
            this.TranslationViewer = ((ICSStudio.Components.Controls.PopupWithUIA)(target));
            return;
            case 2:
            this.TranslationViewerPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 3:
            this.LanguageCombobox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 4:
            this.PasteButton = ((System.Windows.Controls.Button)(target));
            
            #line 31 "..\..\..\Controls\TranslationViewerCtrl.xaml"
            this.PasteButton.Click += new System.Windows.RoutedEventHandler(this.PasteButton_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.HelpButton = ((System.Windows.Controls.Button)(target));
            return;
            case 6:
            this.CloseButton = ((System.Windows.Controls.Button)(target));
            
            #line 37 "..\..\..\Controls\TranslationViewerCtrl.xaml"
            this.CloseButton.Click += new System.Windows.RoutedEventHandler(this.CloseButton_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.TranslatedText = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

