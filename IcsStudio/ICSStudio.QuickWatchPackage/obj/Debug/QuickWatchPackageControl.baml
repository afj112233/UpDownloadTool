   M S B A M L   `   `   `  ????g  cICSStudio.QuickWatchPackage, Version=1.0.10.22348, Culture=neutral, PublicKeyToken=9b683e786b54e86dS.clr-namespace:ICSStudio.QuickWatchPackage.View ICSStudio.QuickWatchPackage.View  5      :    4ICSStudio.QuickWatchPackage.QuickWatchPackageControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      ?view.clr-namespace:ICSStudio.QuickWatchPackage.View     5      -   5        {?5
      ?? *	IsEnabled+.??5      ?   ? 	Resources  5   
    ?? 
TargetType ?? Property ?? Value&   System.Windows.Controls.DataGrid??%D  '??       '?? G     '?? ?     '  ?     ?? 5      " ??6   .?5      ??  ? ??$ 5q?6+   5      ?? 5      " ??6   .?5      ??  ? ??$ 5q?6+   5      ?? 5      " ??6   .?5      ??  ? ??$ 5q?6+   5      ?? 5      "  6   .?5      ??  ? ??$ 5q?6+   5      5       ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5       .+?5"   
   ??  ?? Orientation$ 
Horizontal=?6   .+?5#      ?? $??0??5$      $	??300??5%      $??25??5&      $??5q?5'       ?? DisplayMemberPath$
 Name??5(      ??5)      ?? *WatchListCollectionView+ ?? SelectedItem 5*      ?? *SelectedItem+ ?? 
GroupStyle 5+      (  "System.Windows.Controls.GroupStyle  5,      	   HeaderTemplate	 5-      ?? 5.      .R?5/   "   ? $??1??6'   
???   ?62   50      52      54      ?? 55      -   5        $??1??56      $	??190??57      $??25??58      $??5q?59      ??5;      ?? *
ListName+
 ?? UpdateSourceTrigger$
 	LostFocus=?y?5<      ?? *RemoveCommandVisibility+.??6@     5=         	HelpBrush j? 
AlignmentX ?? Opacity j? Stretch j? TileMode !? Visual  EnterQuickWatchListName??%?   &	        !? 5>      $
 Left=?5@      $	 0.3Q?5A      $
 None=?5B      $
 None=?5C       5D      ?? 5E      $??Italic$?6(   #???  6;   5G      5H      .??6%   ??5I      ?? 5J      " ??6    ?? Triggers 5K      P? 5L       P? Property ? ?? P? Value 66   Z? . ?5M   "   ??  ? ??# [  6?   5N      P? 5O       ? ??$  ??66   . ?5P   "   ??  ? ??# [  6?   5Q      5S      .??5T   !   5U      ?? 5W      $??2??5X      $??25??5Y      $??25??5Z      $??5q?5[      ??5\      ?? *RefreshCommand+y?5]      ?? *RefreshCommandVisibility+??5^      ?? 5_       ?? BitmapScalingMode$
 Fant=?6   $??Resources/refresh.png??6C   5a      ?? 5b      $??2??5c      $??25??5d      $??25??5e      $??5q?5f      ??5g      ?? *RemoveCommand+y?5h      ?? *RemoveCommandVisibility+??5i      ?? 5j      $
 Fant=?6   $??Resources/delete.png??6C   5l      5m      5   /ICSStudio.QuickWatchPackage.View.MonitorControl  5o   
   -   5          Name$ MonitorControl??5p      $??1??5q      $??3??5r       ? DataContext 5s      ?? *MonitorTagsViewModel+5t      5u      