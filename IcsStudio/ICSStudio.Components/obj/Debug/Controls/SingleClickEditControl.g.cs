#pragma checksum "..\..\..\Controls\SingleClickEditControl.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "DA04CE829BA718BA84DA994E2B4E4C611C2FD878"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using ICSStudio.Gui.Converters;
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
    /// SingleClickEditControl
    /// </summary>
    public partial class SingleClickEditControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\Controls\SingleClickEditControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ICSStudio.Components.Controls.SingleClickEditControl Root;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\Controls\SingleClickEditControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LayoutGrid;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\..\Controls\SingleClickEditControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel EditableControlPlaceholder;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\Controls\SingleClickEditControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Rectangle ReadOnlyBackgroundFill;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\Controls\SingleClickEditControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ReadOnlyTextBlock;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\Controls\SingleClickEditControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.VisualStateGroup CommonStates;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\Controls\SingleClickEditControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.VisualState Normal;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\..\Controls\SingleClickEditControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.VisualState Parent_Cell_Focused;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\Controls\SingleClickEditControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.VisualState Unfocused_MouseOver;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\Controls\SingleClickEditControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.VisualState Focused_MouseOver;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\..\Controls\SingleClickEditControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.VisualState Focused_MouseNotOver;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\..\Controls\SingleClickEditControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.VisualState ReadOnly_RowUnselected;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\..\Controls\SingleClickEditControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.VisualState ReadOnly_RowSelected_FocusNotInGrid;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\..\Controls\SingleClickEditControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.VisualState ReadOnly_RowSelected_FocusInGrid;
        
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
            System.Uri resourceLocater = new System.Uri("/ICSStudio.Components;component/controls/singleclickeditcontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\SingleClickEditControl.xaml"
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
            this.Root = ((ICSStudio.Components.Controls.SingleClickEditControl)(target));
            
            #line 9 "..\..\..\Controls\SingleClickEditControl.xaml"
            this.Root.Loaded += new System.Windows.RoutedEventHandler(this.HandleLoaded);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\Controls\SingleClickEditControl.xaml"
            this.Root.Unloaded += new System.Windows.RoutedEventHandler(this.HandleUnloaded);
            
            #line default
            #line hidden
            
            #line 10 "..\..\..\Controls\SingleClickEditControl.xaml"
            this.Root.MouseEnter += new System.Windows.Input.MouseEventHandler(this.HandleMouseEnter);
            
            #line default
            #line hidden
            
            #line 10 "..\..\..\Controls\SingleClickEditControl.xaml"
            this.Root.MouseLeave += new System.Windows.Input.MouseEventHandler(this.HandleMouseLeave);
            
            #line default
            #line hidden
            
            #line 11 "..\..\..\Controls\SingleClickEditControl.xaml"
            this.Root.GotFocus += new System.Windows.RoutedEventHandler(this.HandleGotFocus);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 47 "..\..\..\Controls\SingleClickEditControl.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CanExecuteDelete);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 48 "..\..\..\Controls\SingleClickEditControl.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ExecuteCommitEdit);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 49 "..\..\..\Controls\SingleClickEditControl.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ExecuteCancelEdit);
            
            #line default
            #line hidden
            return;
            case 5:
            this.LayoutGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.EditableControlPlaceholder = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 7:
            this.ReadOnlyBackgroundFill = ((System.Windows.Shapes.Rectangle)(target));
            return;
            case 8:
            this.ReadOnlyTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 9:
            this.CommonStates = ((System.Windows.VisualStateGroup)(target));
            return;
            case 10:
            this.Normal = ((System.Windows.VisualState)(target));
            return;
            case 11:
            this.Parent_Cell_Focused = ((System.Windows.VisualState)(target));
            return;
            case 12:
            this.Unfocused_MouseOver = ((System.Windows.VisualState)(target));
            return;
            case 13:
            this.Focused_MouseOver = ((System.Windows.VisualState)(target));
            return;
            case 14:
            this.Focused_MouseNotOver = ((System.Windows.VisualState)(target));
            return;
            case 15:
            this.ReadOnly_RowUnselected = ((System.Windows.VisualState)(target));
            return;
            case 16:
            this.ReadOnly_RowSelected_FocusNotInGrid = ((System.Windows.VisualState)(target));
            return;
            case 17:
            this.ReadOnly_RowSelected_FocusInGrid = ((System.Windows.VisualState)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

