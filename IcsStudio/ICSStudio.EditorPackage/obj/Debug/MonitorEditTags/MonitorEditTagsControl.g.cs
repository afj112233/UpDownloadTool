#pragma checksum "..\..\..\MonitorEditTags\MonitorEditTagsControl.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "211AC41AFD1E2C6C97EF91C2FF6A2FCD4F74360C"
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
using ICSStudio.EditorPackage.MonitorEditTags.Controls;
using ICSStudio.EditorPackage.MonitorEditTags.UI;
using ICSStudio.Gui.Converters;
using ICSStudio.Gui.View;
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
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Chromes;
using Xceed.Wpf.Toolkit.Converters;
using Xceed.Wpf.Toolkit.Core;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Core.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Mag.Converters;
using Xceed.Wpf.Toolkit.Panels;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Commands;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.Zoombox;


namespace ICSStudio.EditorPackage.MonitorEditTags {
    
    
    /// <summary>
    /// MonitorEditTagsControl
    /// </summary>
    public partial class MonitorEditTagsControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 302 "..\..\..\MonitorEditTags\MonitorEditTagsControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox CmbScope;
        
        #line default
        #line hidden
        
        
        #line 454 "..\..\..\MonitorEditTags\MonitorEditTagsControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl TabControl;
        
        #line default
        #line hidden
        
        
        #line 460 "..\..\..\MonitorEditTags\MonitorEditTagsControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ICSStudio.EditorPackage.MonitorEditTags.Controls.MonitorTagsControl MonitorTagsControl;
        
        #line default
        #line hidden
        
        
        #line 464 "..\..\..\MonitorEditTags\MonitorEditTagsControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ICSStudio.EditorPackage.MonitorEditTags.Controls.EditTagsControl EditTagsControl;
        
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
            System.Uri resourceLocater = new System.Uri("/ICSStudio.EditorPackage;component/monitoredittags/monitoredittagscontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MonitorEditTags\MonitorEditTagsControl.xaml"
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
            this.CmbScope = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 2:
            
            #line 396 "..\..\..\MonitorEditTags\MonitorEditTagsControl.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonBase_OnClick);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 398 "..\..\..\MonitorEditTags\MonitorEditTagsControl.xaml"
            ((System.Windows.Controls.ContextMenu)(target)).Opened += new System.Windows.RoutedEventHandler(this.ContextMenu_OnOpened);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 400 "..\..\..\MonitorEditTags\MonitorEditTagsControl.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MenuItem_OnClick);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 404 "..\..\..\MonitorEditTags\MonitorEditTagsControl.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MenuItem_OnClick);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 408 "..\..\..\MonitorEditTags\MonitorEditTagsControl.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MenuItem_OnClick);
            
            #line default
            #line hidden
            return;
            case 7:
            this.TabControl = ((System.Windows.Controls.TabControl)(target));
            return;
            case 8:
            this.MonitorTagsControl = ((ICSStudio.EditorPackage.MonitorEditTags.Controls.MonitorTagsControl)(target));
            return;
            case 9:
            this.EditTagsControl = ((ICSStudio.EditorPackage.MonitorEditTags.Controls.EditTagsControl)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

