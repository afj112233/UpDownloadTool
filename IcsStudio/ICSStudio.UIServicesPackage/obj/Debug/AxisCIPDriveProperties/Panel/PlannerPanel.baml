   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5       ICSStudio.UIServicesPackageK   EICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel.PlannerPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5        ? 	Resources  5       ?? MergedDictionaries ?? Source3   -ICSStudio.Gui.Controls.SingleUpDownExtensions ?? 
TargetType ?? Property ?? Value?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %S   '        ?? 5      "  6   .?5      ??  ? ??$	 0,4q?6+   5      5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      $	??175 ?6   ?? 5      $	??120 ?6   ?? 5       $	??120 ?6   ?? 5!      $	??170 ?6   ?? 5"      $
??Auto ?6   .+?5%   
   ?? $??0??5&      $??0??5'      $??Center=?5(         Maximum Speed:#???   5)      ?? 5*   
   $??1??5+      $??0??5,      $??Center=?5-       ( $CIPTag Planner Maximum Acceleration:#???  5.      ?? 5/   
   $??2??50      $??0??51      $??Center=?52        Maximum Deceleration:#???  53      ?? 54   
   $??3??55      $??0??56      $??Center=?57        Maximum Acceleration Jerk:#???  58      ?? 59   
   $??4??5:      $??0??5;      $??Center=?5<        Maximum Deceleration Jerk:#???  5=        5?   
   $??0??5@      $??1??5A      $??Center=?5B      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .  	   Minimum$	 0X?5E      
   ShowButtonSpinner
 .  ,  &Xceed.Wpf.Toolkit.Primitives.InputBase   TextAlignment$
 Left=?5G      {?5D      ?? *IsPlannerEnabled+   Value 5H      ?? *MaximumSpeed+  5I   
   $??1??5J      $??1??5K      $??Center=?5L       .  $	 0X?5O      
 .  $
 Left=?5Q      {?5N      ?? *IsPlannerEnabled+ 5R      ?? *MaximumAcceleration+  5T   
   $??2??5U      $??1??5V      $??Center=?5W       .  $	 0X?5Z      
 .  $
 Left=?5\      {?5Y      ?? *IsPlannerEnabled+ 5]      ?? *MaximumDeceleration+  5^   
   $??3??5_      $??1??5`      $??Center=?5a       .  $	 0X?5d      
 .  $
 Left=?5f      {?5c      ?? *IsPlannerEnabled+ 5g      ?? *MaximumAccelerationJerk+  5h   
   $??4??5i      $??1??5j      $??Center=?5k       .  $	 0X?5n      
 .  $
 Left=?5p      {?5m      ?? *IsPlannerEnabled+ 5q      ?? *MaximumDecelerationJerk+?? 5t   
   $??0??5u      $??2??5v      $??Center=?5w      ??5x      ?? *PositionUnits+ ?? StringFormat$ {0}/s???? 5y   
   $??1??5z      $??2??5{      $??Center=?5|      ??5}      ?? *PositionUnits+$ {0}/s^2???? 5~   
   $??2??5      $??2??5?      $??Center=?5?      ??5?      ?? *PositionUnits+$ {0}/s^2???? 5?   
   $??3??5?      $??2??5?      $??Center=?5?      ??5?      ?? *PositionUnits+$ {0}/s^3???? 5?   
   $??4??5?      $??2??5?      $??Center=?5?      ??5?      ?? *PositionUnits+$ {0}/s^3???? 5?   
   $??0??5?      $??3??5?      $
??Left=?5?      $??Center=?5?        Parameters...#???  5?      ??5?      ?? *ParametersCommand+5?      ?? 5?   
   $??1??5?      $??3??5?      $
??Left=?5?      $??Center=?5?        AccelCalculate...#???  5?      ??5?      ?? *CalculateAccelerationCommand+5?      ?? 5?   
   $??3??5?      $??3??5?      $??Center=?5?      ??5?      ?? *AccelPercentTime+$ {0} of Max Accel Time???? 5?   
   $??4??5?      $??3??5?      $??Center=?5?      ??5?      ?? *DecelPercentTime+$ {0} of Max Decel Time???? 5?   
   $??3??5?      $??4??5?      $
??Left=?5?      $??Center=?5?      #???  5?      ??5?      ?? *AccelJerkCalculateCommand+{?5?      ?? *IsPlannerEnabled+?? 5?   
   $??4??5?      $??4??5?      $
??Left=?5?      $??Center=?5?        DecelCalculate...#???  5?      ??5?      ?? *DecelJerkCalculateCommand+{?5?      ?? *IsPlannerEnabled+5?      5?      