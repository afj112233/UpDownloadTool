   M S B A M L   `   `   `  ????  ICSStudio.UIServicesPackageE    ?ICSStudio.UIServicesPackage.ManualTune.Controls.MotionGenerator   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      .??5	      ?   ? RowDefinitions  5
   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   $	??200??6*   ?? 5      $??20 ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5   
   ]? $??0??6   .+?5      ??    Motion Generator#???   6   $
??Bold"?6B   ?? 5      $??Right=?6     More Commands...#???  61   ??6^   ?? *MoreCommands+5      5      ? 5   
   $??1??6     5       ?? 5!      $
??Auto ?6    ?? 5"      $??* ?6     ? ColumnDefinitions 5$      ?? 5%      $	??135 ?6#   ?? 5&      $??* ?6#   .+?5)      ?? $??0??6   $??0??6%     	Commands:#???  65   6Z   ?? 5+      $??0??6   $??1??6%   $
??Bold"?5,       ?? Padding$	 4,0q?6*   ??5-      ??  ?? ElementName$ CommandsTreeView?? ?? Path$ SelectedItem.Tag???5.      ?? 5/       ?? 
StartPoint$
 0, 0&?6*    ?? EndPoint$
 0, 1&?6<   ??51      ?  ? Offset$	 0.0Q?6+    ? Color# ? ??52   +   ? 53      $	 1.0Q?6+   # ? ??54   +   56      58      R? 5:      -   5        	 R?Name$	 CommandsTreeView??5;      $??1??5:      $??0??6$   $??0,0,2,0q?64   $??Stretch=?5<      .@?5>      Q? $	??MSO??6     Motion Servo On

 ? Tag#
 ?  6,    Q? 
IsSelected . 6T   Q? 5?      $	??MSF??6     Motion Servo Off#
 ?  6,   Q? 5@      $	??MAH??6     Motion Axis Home#
 ?  6,   Q? 5A      $	??MAJ??6     Motion Axis Jog#
 ?  6,   Q? 5B      $	??MAM??6     Motion Axis Move#
 ?  6,   Q? 5C      $	??MAS??6     Motion Axis Stop#
 ?  6,   Q? 5D      $	??MDS??6    	 Motion Drive Start#
 ? 	 6,   Q? 5E      $
??MAFR??6    
 Motion Axis Fault Reset#
 ? 
 6-   5F      1  +Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid  5H      $??1??6    $??1??6-      ShowSearchBox .     ShowSortOptions .     	ShowTitle .    ShowSummary .  $	??220??5K       $	??220??6-      SelectedObject 5L       ?? *SelectedParam+y?5M       ?? *ParamVisibility+5O      ? 5R   
   $??3??6     5S      ?? 5T      $
??Auto ?6    ?? 5U      $
??Auto ?6    ?? 5V      $
??Auto ?6     5X      ?? 5Y      $??90 ?6#   ?? 5Z      $??90 ?6#   ?? 5[      $??90 ?6#   ?? 5\      $	??120 ?6#   .+?5_      ?? $??0??6   $??0??6%     MotionConsole Axis State:#???  65   ?? 5`      $??1??6   $??0??6%     Axis Fault:#???  65   ?? 5a      $??2??6   $??0??6%     Start Inhibited:#???  65   ?? 5c      $??0??6   $??1??6%   $??2??65   ??6I   ?? *	AxisState+?? 5d      $??1??6   $??1??6%   $??3??6P   ??65   ?? *	AxisFault+6P   5g      ?? 5i   
   $??4??6    ?? Orientation$ 
Horizontal=?6"   .+?5j      ??   Execute#???  6   ??69   ?? *ExecuteCommand+?? 5k        Disable Axis#???  6   ??6>   ?? *DisableAxisCommand+5l      5o      5p      