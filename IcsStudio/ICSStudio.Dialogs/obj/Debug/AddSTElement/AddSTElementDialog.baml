   M S B A M L   `   `   `  ????d  `DotNetProjects.Input.Toolkit, Version=6.1.91.0, Culture=neutral, PublicKeyToken=79778c5f2eed289bgKclr-namespace:System.Windows.Controls;assembly=DotNetProjects.Input.ToolkitSystem.Windows.Controls  5      ^ ZICSStudio.Dialogs, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=8558c3dcb301e1d4?$clr-namespace:ICSStudio.Dialogs.GoToICSStudio.Dialogs.GoTo 5      Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui 5      N7clr-namespace:ICSStudio.Gui.View;assembly=ICSStudio.GuiICSStudio.Gui.View 5      7   1ICSStudio.Dialogs.AddSTElement.AddSTElementDialog   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      `controlsKclr-namespace:System.Windows.Controls;assembly=DotNetProjects.Input.Toolkit     5      :	converter$clr-namespace:ICSStudio.Dialogs.GoTo     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     5      V RMaterialDesignThemes.Wpf, Version=3.1.3.1513, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5	      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5
      Bview7clr-namespace:ICSStudio.Gui.View;assembly=ICSStudio.Gui  5      -   5           Add Structured Text Element  ? Title#  ?   5      $	??360??5      $	??360??5      $	??360??5      $	??360??5      ???	#FFF0F0F0 ? 
ResizeMode$ CanResizeWithGrip=?5       ? ShowInTaskbar .   ? WindowStartupLocation$ CenterOwner=?5         ICSStudio.Gui.DialogCloser   DialogResult 5      ?? *DialogResult+ ? 	Resources 5      %  ICSStudio.Gui.View.BindingProxy 	 Proxy   Data.  (ICSStudio.Dialogs.GoTo.LanguageConverter  LanguageConverter ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %C   &	       &	        5       6.   ??   5      6*   5!      .??5#      ?  5$   
   	 ?? 
TargetType
 ?? Property ?? Value??%S   'R?       ?? 5%      "	 R?6   .?5&      ?? 
 ? ??$	 2.5q?6+   5'      5(       ? RowDefinitions 5)   
   ?? 5*      $??25 ?6   ?? 5+      $??20 ?6   ?? 5,      ?? 5-      $??70 ?6    ? ColumnDefinitions 5/   
   ?? 50      $
??Auto ?6   ?? 51      .+?53   
   ?? $??0??54      $??0??55        ST Element:#???  56      -   'System.Windows.Controls.AutoCompleteBox  57   
   $??0??58      $??1??59      $??3q?5:         IsTextCompletionEnabled .    MaxDropDownHeight$	 160Q?5=         ItemsSource 5<      ?? *AllST+   Text 5>      ?? *Name+ ?? Mode$ TwoWay=? ?? UpdateSourceTrigger$ PropertyChanged=? ?? Delay$ 30??R? 5?   
   $??2??5@      $??0??5A      $??2??5B      ??5C      ?? *ListViewTree+y?5D      ?? *TreeViewVisibility+??5E      ?? 5F      "	 Q?6   .?5G      ?? 
 ?@Q?
IsExpanded 63   ?? *
IsExpanded+$ TwoWay=??? 5H      
 ?@Q?
IsSelected 63   ?? *
IsSelected+$ TwoWay=?-   5        5K      ??5N      ?? 5O       ?? ItemsSource 6+   ??  ?? Path$ Items?.R?5P      ? .+?5Q      ??  ?? Orientation$ 
Horizontal=?6%   .+?5R      '  !MaterialDesignThemes.Wpf.PackIcon  $??2q?66      Kind 6A   ?? *
IconKind+?? 5S      $??2q?6(   ??63   ?? *Name+ ?? 	Converter# [ 5T      ?? 5U      $
??Left=?5X      $??WordEllipsis=?5Z      ??5V      ?? *Data.DescriptionTextTreeWidth+ ?? Source# [ ??5W      ?? *Data.DescriptionMargin+# [ ??5Y      ?? *Description+# [ 5Z      5[      5\      5^      R? 5_   
   $??2??5`      $??0??5a      $??2??5b      ??5c      ?? *
ListView+y?5d      ?? *ListViewVisibility+??5e      ?? 5f      "	 Q?6   .?5g      ?? 
 ?@Q?
IsSelected 63   ?? *
IsSelected+$ TwoWay=?-   5        5j      ??5l      ?? 5m      .R?5n      ? .+?5o      ?? $ 
Horizontal=?6%   .+?5p        $??2q?66    6A   ?? *
IconKind+?? 5q      $??2q?6(   ??63   ?? *Name+5r      ?? 5s      $
??Left=?5v      $??WordEllipsis=?5x      ??5t      ?? *Data.DescriptionTextWidth+# [ ??5u      ?? *Data.DescriptionListMargin+# [ ??5w      ?? *Description+5x      5y      5z      5|      ?? 5}   
   $??1??5~      $??2??5      $??0??5?      $??2??5?       K? ZIndex$ -1??5?       ?? View 5?      ?? 5?      P?5?      ?? 5?      #??? ??6w    ?? Width 6)   ?? *	NameWidth+$ PropertyChanged=?$ TwoWay=?6w   ?? 5?        Description#???  6L    6)   ?? *DescriptionWidth+6L   5?      5?      ?? 5?   
   $??3??5?      $??0??5?      $??2??5?       $  Show Language Elements By Groups#???  5?       d? 	IsChecked 5?      ?? *ShowTreeView+?? 5?   
   $??3??5?      $??0??5?      $??2??5?      $
??auto??5?      $??Right=?5?      $??Bottom=?5?      .+?5?      ??   OK#???  5?       ?? 	IsDefault . ??5?      ?? *	OkCommand+5?      ?? 5?       
 Cancel#???  5?        ?? IsCancel  . ??5?      ?? *CancelCommand+5?      ?? 5?        Help#???  6   5?      ?? 5?   
   $??3??5?      $??0??5?      $??2??5?      $??20??5?      $
??Left=?5?      $??Bottom=?5?       	 New UD Function Block...#??? 	 5?      ??5?      ?? *NewAOICommand+5?      5?      5?      