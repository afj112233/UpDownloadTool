   M S B A M L   `   `   `  ???? d  `DotNetProjects.Input.Toolkit, Version=6.1.91.0, Culture=neutral, PublicKeyToken=79778c5f2eed289bgKclr-namespace:System.Windows.Controls;assembly=DotNetProjects.Input.ToolkitSystem.Windows.Controls  ^ ZICSStudio.Dialogs, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=8558c3dcb301e1d4?$clr-namespace:ICSStudio.Dialogs.GoToICSStudio.Dialogs.GoTo Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui N7clr-namespace:ICSStudio.Gui.View;assembly=ICSStudio.GuiICSStudio.Gui.View 7   1ICSStudio.Dialogs.AddSTElement.AddSTElementDialog   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     `controlsKclr-namespace:System.Windows.Controls;assembly=DotNetProjects.Input.Toolkit     :	converter$clr-namespace:ICSStudio.Dialogs.GoTo     @d2http://schemas.microsoft.com/expression/blend/2008     Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     V RMaterialDesignThemes.Wpf, Version=3.1.3.1513, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  Bview7clr-namespace:ICSStudio.Gui.View;assembly=ICSStudio.Gui  -      Add Structured Text Element  ? Title#  ?   $	??360??$	??360??$	??360??$	??360?????	#FFF0F0F0 ? 
ResizeMode$ CanResizeWithGrip=? ? ShowInTaskbar .   ? WindowStartupLocation$ CenterOwner=?   ICSStudio.Gui.DialogCloser   DialogResult ?? *DialogResult+ ? 	Resources %  ICSStudio.Gui.View.BindingProxy 	 Proxy   Data.  (ICSStudio.Dialogs.GoTo.LanguageConverter  LanguageConverter ?? MergedDictionaries ?? Source??  ?? $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??%'   &	       &	         ??   .???  	 ?? 
TargetType
 ?? Property ?? Value??%.   'R?       ?? "	 R?.??? 
 ? ??$	 2.5q? ? RowDefinitions ?? $??25 ??? $??20 ??? ?? $??70 ? ? ColumnDefinitions ?? $
??Auto ??? .+??? $??0??$??0??  ST Element:#???  -   'System.Windows.Controls.AutoCompleteBox  $??0??$??1??$??3q?   IsTextCompletionEnabled .    MaxDropDownHeight$	 160Q?   ItemsSource ?? *AllST+   Text ?? *Name+ ?? Mode$ TwoWay=? ?? UpdateSourceTrigger$ PropertyChanged=? ?? Delay$ 30??R? $??2??$??0??$??2?????? *ListViewTree+y??? *TreeViewVisibility+???? "	 Q?.??? 
 ?@Q?
IsExpanded ?? *
IsExpanded+$ TwoWay=??? 
 ?@Q?
IsSelected ?? *
IsSelected+$ TwoWay=?-   ????  ?? ItemsSource ??  ?? Path$ Items?.R?? .+???  ?? Orientation$ 
Horizontal=?.+?'  !MaterialDesignThemes.Wpf.PackIcon  $??2q?   Kind ?? *
IconKind+?? $??2q????? *Name+ ?? 	Converter# [ ?? $
??Left=?$??WordEllipsis=????? *Data.DescriptionTextTreeWidth+ ?? Source# [ ???? *Data.DescriptionMargin+# [ ???? *Description+# [ R? $??2??$??0??$??2?????? *
ListView+y??? *ListViewVisibility+???? "	 Q?.??? 
 ?@Q?
IsSelected ?? *
IsSelected+$ TwoWay=?-   ???? .R?? .+??? $ 
Horizontal=?.+?  $??2q? ?? *
IconKind+?? $??2q????? *Name+?? $
??Left=?$??WordEllipsis=????? *Data.DescriptionTextWidth+# [ ???? *Data.DescriptionListMargin+# [ ???? *Description+?? $??1??$??2??$??0??$??2?? K? ZIndex$ -1?? ?? View ?? P??? #??? ?? ?? Width ?? *	NameWidth+$ PropertyChanged=?$ TwoWay=???   Description#???   ?? *DescriptionWidth+?? $??3??$??0??$??2?? $  Show Language Elements By Groups#???   d? 	IsChecked ?? *ShowTreeView+?? $??3??$??0??$??2??$
??auto??$??Right=?$??Bottom=?.+???   OK#???   ?? 	IsDefault . ???? *	OkCommand+??  
 Cancel#???    ?? IsCancel  . ???? *CancelCommand+??   Help#???  ?? $??3??$??0??$??2??$??20??$
??Left=?$??Bottom=? 	 New UD Function Block...#??? 	 ???? *NewAOICommand+