   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761f?Eclr-namespace:ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel7ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel  5      Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls 5      3    -ICSStudio.UIServicesPackage.View.ManualAdjust   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      WlocalEclr-namespace:ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5        ? 	Resources  5	       ?? MergedDictionaries ?? Source?? 5
   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %    5      .??5      ?  +? FocusedElement 6   ??  ?? ElementName$ S1?? ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6     5   
   3  -ICSStudio.Gui.Controls.SingleUpDownExtensions ?? 
TargetType ?? Property	 ?? Value??%Q   '        ?? 5       "  6   .?5!      ??  ? ??$	 5q?6+   5"      5#      .+?5%   
   ?? $??0??5&      $??0??5'         Maximum Speed:#???   5(        5)   
   -   5        $??S1??5*      $??0??5+      $??1??5,      $??Center=?5-      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
   	AllowSpin
 .    Minimum$ 0X?5/         ShowButtonSpinner .    Value 52      ?? *Speed+?? 53   
   $??0??54      $??2??55      ??56      ?? *PositionUnits+ ?? StringFormat$ {0}/s???? 57   
   $??0??58      $??3??59      $??5q?5:       	 Reset#???  5<      ??5;      ?? *ResetCommand+5<      ?? 5=   
   $??1??5>      $??0??5?        Maximum Acceleration:#???  5@        5A   
   -   5        $??1??5B      $??1??5C      $??Center=?5D      
 . $ 0X?5F       .  5I      ?? *Accel+?? 5J   
   $??1??5K      $??2??5L      ??5M      ?? *PositionUnits+$ {0}/s^2???? 5N   
   $??2??5O      $??0??5P        Maximum Deceleration:#???  5Q        5R   
   -   5        $??2??5S      $??1??5T      $??Center=?5U      
 . $ 0X?5W       .  5Z      ?? *Decel+?? 5[   
   $??2??5\      $??2??5]      ??5^      ?? *PositionUnits+$ {0}/s^2???? 5_   
   $??3??5`      $??0??5a        Maximum Acceleration Jerk:#???  5b        5c   
   $??3??5d      $??1??5e      $??Center=?5f      
 . ?  ?Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	Increment$ 0.00005X?5h      $ 0X?5i       .  5k      ?? *	AccelJerk+?? 5l   
   $??3??5m      $??2??5n      ??5o      ?? *PositionUnits+$ {0}/s^3???? 5p   
   $??3??5q      $??3??5r      ??5s      ?? *AJText+?? 5t   
   $??4??5u      $??0??5v        Maximum Deceleration Jerk:#???  5w        5x   
   $??4??5y      $??1??5z      $	??100??5{      $??Center=?5|      
 . $ 0.00005X?5~      $ 0X?5       .  5?      ?? *	DecelJerk+?? 5?   
   $??4??5?      $??2??5?      ??5?      ?? *PositionUnits+$ {0}/s^3???? 5?   
   $??4??5?      $??3??5?      ??5?      ?? *DJText+5?      5?      