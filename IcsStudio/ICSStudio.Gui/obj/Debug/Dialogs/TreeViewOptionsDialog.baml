   M S B A M L   `   `   `  ????  ICSStudio.Gui1    +ICSStudio.Gui.Dialogs.TreeViewOptionsDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -   5        $	??414??5   	   $	??552??6   $	??260??5   	   $	??480??6     ? ShowInTaskbar  .   ? WindowStartupLocation$ CenterOwner=?5
   	    ? Title 5   	   ?? *Title+.??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $??1* ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5   
   ?? $??0??6      Categories:#???   6!   ? 5   
   $??1??6    ? ColumnDefinitions 5      ?? 5      $	??200 ?5      ?? 5      $??1* ?5      .+?5      R? -   5        $??TreeView??5   '   $??0??6   $??0,0,2,0q?67   $??Stretch=?5!      ??5       ?? *OptionPanelNodes+??5"      ?? 5#       ?? 
TargetType" Q?6   .?5$      ??  ?? Property ?@Q?
IsExpanded ?? Value 67   ?? *
IsExpanded+ ?? Mode$ TwoWay=??? 5%       ?@Q?
IsSelected 67   ?? *
IsSelected+$ TwoWay=?5&      ??5(      ?? 5*      	 ?? ItemsSource	 6/   ?? *
Children+.R?5+      ?? 
 ?? Orientation$
 
Horizontal=?6%   $??0 0 0 0q?6>   .+?5,      ?? ??6(   ?? *Title+5-      5.      51      ?? 52      -   5        $??OptionPanelScrollViewer??52   +   $??1??6   $??Disabled=?53      $
??Auto=?54      .??55      ]? .+?56      ?? $	??Top=?6    $
??Bold"?57        ?? Padding$	 4,0q?58       ??6.   ?? *OptionPanelTitle+??59      ?? 5:       ?? 
StartPoint$
 0, 0&?62    ?? EndPoint$
 0, 1&?6D   ??5<   &   ?  ? Offset$	 0.0Q?63    ? Color# ? ??5=   3   ? 5>   &   $	 1.0Q?63   # ? ??5?   3   5A      5C      ?? 5D      ??6'   ?? *OptionPanelContent+5E      5G      5H      ?? 5J   
   $??2??6   ?? 5L   
   $??3??6   ??6!   ?? *State+y?68   ?? *StateVisibility+]? 5N   
   $??4??6    ? 	Resources 5O        ButtonStyle??%|   &	       ?? 5P      " ??6   .?5Q      ??  ? ??$ 4,4,12,4q?6/   ?? 5R       ? ??$ 74??61   5S      5T      .+?5U      ?? $??Center=?67   #??[ 5V      ??5U      ?? *ExpansionName+y?5V   :   ?? *ExpansionVisibility+??5W      ?? *ExpansionCommand+?? 5Y        Help#???  6   $??Right=?66   $??Right=?6M   #??[ 5Z      ??5[      ?? *HelpCommand+?? 5\       	 Apply#???  6   $??Right=?67   $??Right=?6N   #??[ 5]      ??5^      ?? *ApplyCommand+?? 5_       
 Cancel#???  6   $??Right=?68   $??Right=?6O    ?? IsCancel . #??[ 5`      ??5a      ?? *CancelCommand+?? 5b        OK#???  6   $??Right=?64   $??Right=?6K    ?? 	IsDefault . #??[ 5c      ??5d      ?? *	OkCommand+5f      5h      5i      