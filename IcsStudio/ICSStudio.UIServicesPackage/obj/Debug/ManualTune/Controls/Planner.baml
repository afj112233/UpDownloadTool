   M S B A M L   `   `   `  ????  ICSStudio.UIServicesPackage=    7ICSStudio.UIServicesPackage.ManualTune.Controls.Planner   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5        ? 	Resources  5	         	LeftImage ?? 	UriSource$  Xceed.Wpf.Toolkit.SingleUpDown ?? 
TargetType?? 5
   
   %?   &	        '  i     ?? 5      $Q Kpack://application:,,,/ICSStudio.Gui;component/Resources/image/left_6_5.png??5      ?? 5      "  6   .?5      -   5        5      5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $	??100 ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $	??100 ?6   ?? 5      $
??Auto ?6   .+?5!   
   ?? $??0??6   $??0??6     Planner Maximum Speed:#???  6-   6b   ?? 5"   
   $??1??6   $??0??6    ! Planner Maximum Acceleration:#???  6-   6i   ?? 5#   
   $??2??6   $??0??6    ! Planner Maximum Deceleration:#???  6-   6i     5%   
   -   5        $??0??5%      $??1??6)   $??Center=?69   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Minimum$ 0X?5&      ?  ?Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   FormatString$ ######0.0######??5*         Value 5'      ?? *MaximumSpeed+ ?? UpdateSourceTrigger$ 	LostFocus=?{?5)      ?? *EditEnabled+5*        5+   
   -   5        $??1??5+      $??1??6)   $??Center=?69   $ 0X?5,      $ ######0.0######??50       5-      ?? *MaximumAcceleration+$ 	LostFocus=?{?5/      ?? *EditEnabled+50        51   
   -   5        $??2??51      $??1??6)   $??Center=?69   $ 0X?52      $ ######0.0######??56       53      ?? *MaximumDeceleration+$ 	LostFocus=?{?55      ?? *EditEnabled+56      ?? 59   
   $??0??6   $??2??6"   	 ?? Orientation$	 
Horizontal=?5:      $??Center=?6.   .+?5;      ?? #??[  6   $??6??68   $??5??6B   ?? 5<      
 ?? ContentStringFormat$
 {0}/s??5=      ??5<      ?? *PositionUnits+5=      5>      ?? 5?   
   $??1??6   $??2??6"   $	 
Horizontal=?5@      $??Center=?6.   .+?5A      ?? #??[  6   $??6??68   $??5??6B   ?? 5B      $
 {0}/s^2??5C      ??5B      ?? *PositionUnits+5C      5D      ?? 5E   
   $??2??6   $??2??6"   $	 
Horizontal=?5F      $??Center=?6.   .+?5G      ?? #??[  6   $??6??68   $??5??6B   ?? 5H      $
 {0}/s^2??5I      ??5H      ?? *PositionUnits+5I      5J      ?? 5L   
   $??0??6   $??3??6    & "Planner Maximum Acceleration Jerk:#???  6-   6n   ?? 5M   
   $??1??6   $??3??6    & "Planner Maximum Deceleration Jerk:#???  6-   6n     5O   
   -   5        $??0??5O      $??4??6)   $??Center=?69   $ 0X?5P      $ ######0.0######??5T       5Q      ?? *MaximumAccelerationJerk+$ 	LostFocus=?{?5S      ?? *EditEnabled+5T        5U   
   -   5        $??1??5U      $??4??6)   $??Center=?69   $ 0X?5V      $ ######0.0######??5Z       5W      ?? *MaximumDecelerationJerk+$ 	LostFocus=?{?5Y      ?? *EditEnabled+5Z      ?? 5\   
   $??0??6   $??5??6"   $	 
Horizontal=?5]      $??Center=?6.   .+?5^      ?? #??[  6   $??6??68   $??5??6B   ?? 5_      $
 {0}/s^3??5`      ??5_      ?? *PositionUnits+5`      5a      ?? 5b   
   $??1??6   $??5??6"   $	 
Horizontal=?5c      $??Center=?6.   .+?5d      ?? #??[  6   $??6??68   $??5??6B   ?? 5e      $
 {0}/s^3??5f      ??5e      ?? *PositionUnits+5f      5g      5i      5j      