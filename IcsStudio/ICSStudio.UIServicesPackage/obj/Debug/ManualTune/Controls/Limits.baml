   M S B A M L   `   `   `  ????  ICSStudio.UIServicesPackage<    6ICSStudio.UIServicesPackage.ManualTune.Controls.Limits   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5        ? 	Resources  5	         	LeftImage ?? 	UriSource$  Xceed.Wpf.Toolkit.SingleUpDown ?? 
TargetType?? 5
   
   %?   &	        '  i     ?? 5      $Q Kpack://application:,,,/ICSStudio.Gui;component/Resources/image/left_6_5.png??5      ?? 5      "  6   .?5      -   5        5      5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $	??100 ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $	??100 ?6   ?? 5      $
??Auto ?6   .+?5"   
   ?? $??0??6   $??0??6    & "Limits Peak Torque Limit Positive:#???  6-   6n   ?? 5#   
   $??1??6   $??0??6    & "Limits Peak Torque Limit Negative:#???  6-   6n   ?? 5$   
   $??2??6   $??0??6     Limits Acceleration Limit:#???  6-   6f   ?? 5%   
   $??3??6   $??0??6     Limits Deceleration Limit:#???  6-   6f     5'   
   -   5        $??0??5'      $??1??6)   $??Center=?69   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Minimum$ 0X?5(         Maximum$
 1000X?6(   ?  ?Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   FormatString$ ######0.0######??5,         Value 5)      ?? *TorqueLimitPositive+	 ?? UpdateSourceTrigger$	 	LostFocus=?{?5+      ?? *EditEnabled+5,        5-   
   -   5        $??1??5-      $??1??6)   $??Center=?69   $ -1000X?5.      $ 0X?6,   $ ######0.0######??52       5/      ?? *TorqueLimitNegative+$	 	LostFocus=?{?51      ?? *EditEnabled+52        53   
   -   5        $??2??53      $??1??6)   $??Center=?69   $ 0X?54      $ ######0.0######??58       55      ?? *AccelerationLimit+$	 	LostFocus=?{?57      ?? *EditEnabled+58        59   
   -   5        $??3??59      $??1??6)   $??Center=?69   $ 0X?5:      $ ######0.0######??5>       5;      ?? *DecelerationLimit+$	 	LostFocus=?{?5=      ?? *EditEnabled+5>      ?? 5@   
   $??0??6   $??2??6"   
 ?? Orientation$
 
Horizontal=?5A      $??Center=?6.   .+?5B      ?? #??[  6   $??6??68   $??5??6B   ?? 5C        % Rated#???  6   5D      ?? 5E   
   $??1??6   $??2??6"   $
 
Horizontal=?5F      $??Center=?6.   .+?5G      ?? #??[  6   $??6??68   $??5??6B   ?? 5H      #???  6   5I      ?? 5J   
   $??2??6   $??2??6"   $
 
Horizontal=?5K      $??Center=?6.   .+?5L      ?? #??[  6   $??6??68   $??5??6B   ?? 5M       ?? ContentStringFormat$ {0}/s^2??5N      ??5M      ?? *PositionUnits+5N      5O      ?? 5P   
   $??3??6   $??2??6"   $
 
Horizontal=?5Q      $??Center=?6.   .+?5R      ?? #??[  6   $??6??68   $??5??6B   ?? 5S      $ {0}/s^2??5T      ??5S      ?? *PositionUnits+5T      5U      ?? 5X   
   $??0??6   $??3??6    # Limits Velocity Limit Positive:#???  6-   6k   ?? 5Y   
   $??1??6   $??3??6    # Limits Velocity Limit Negative:#???  6-   6k     5[   
   -   5        $??0??5[      $??4??6)   $??Center=?69   $ 0X?5\      $ ######0.0######??5`       5]      ?? *VelocityLimitPositive+$	 	LostFocus=?{?5_      ?? *EditEnabled+5`        5a   
   -   5        $??1??5a      $??4??6)   $??Center=?69   $ 0X?5b      $ ######0.0######??5f       5c      ?? *VelocityLimitNegative+$	 	LostFocus=?{?5e      ?? *EditEnabled+5f      ?? 5h   
   $??0??6   $??5??6"   $
 
Horizontal=?5i      $??Center=?6.   .+?5j      ?? #??[  6   $??6??68   $??5??6B   ?? 5k      $ {0}/s??5l      ??5k      ?? *PositionUnits+5l      5m      ?? 5n   
   $??1??6   $??5??6"   $
 
Horizontal=?5o      $??Center=?6.   .+?5p      ?? #??[  6   $??6??68   $??5??6B   ?? 5q      $ {0}/s??5r      ??5q      ?? *PositionUnits+5r      5s      5u      5v      