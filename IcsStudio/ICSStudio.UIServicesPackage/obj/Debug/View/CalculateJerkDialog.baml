   M S B A M L   `   `   `  ????  ICSStudio.UIServicesPackage:    4ICSStudio.UIServicesPackage.View.CalculateJerkDialog   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      $	??425??5      $	??190??5	        ? 
ResizeMode$  NoResize=?5       ? ShowInTaskbar .   ? WindowStartupLocation$ CenterOwner=?5       ? Title 5      ?? *Title+ ? 	Resources 5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $??20 ?6   ?? 5      $
??Auto ?6   ?? 5      $??20 ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5       $
??Auto ?6   ?? 5!      $??* ?6   .+?5$   
   ?? $??0??6   $??0??6!   	% of Time6<   ?? 5&   
   $??0??6   $??1??6"   .+?5'      ?? 	 ?? AutoToolTipPlacement$	 TopLeft=?5(      
 ?? IsMoveToPointEnabled
 .  ?? IsSnapToTickEnabled .  ? Maximum$	 100Q?5+       ? Minimum$ 0Q?5,       ?? TickFrequency$ 1Q?5-       ?? TickPlacement$ BottomRight=?5.       ? Value 5/      ?? *PercentTime+A? 50      $??1q?51       A? Columns$ 3??52       A? Rows$ 1??53      .+?54      ??  ?? TextAlignment$
 Left=?6   0%65   ?? 55      $ Center=?6   50%68   ?? 56      $ Right=?6   100%68   57      58      ]? 5:   
   $??2??5;      $??0??5<      $??2??5=      .+?5>      ?? ??6   ?? *Label+ ?? StringFormat$
 {0}:???? 5?      {?.  ??6(   ?? *Jerk+$ f1???? 5@      ??6   ?? *PositionUnits+$ {0}/s^3??5A      ]? 5C   
   $??4??5D      $??0??5E      $??2??5F      $??Right=?5G      .+?5H      ?? -   5           OK#???   5J       ?? 	IsDefault . 5K      ?? 5L      -   5         
 Cancel#???  5N       ?? IsCancel . 5O      ?? 5P        Help#???  6   5Q      5S      5T      