   M S B A M L   `   `   `  ????  ICSStudio.UIServicesPackageB    <ICSStudio.UIServicesPackage.ManualTune.Controls.Compensation   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5        ? 	Resources  5	         	LeftImage ?? 	UriSource$  Xceed.Wpf.Toolkit.SingleUpDown ?? 
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
   ?? $??0??6   $??0??6      Compensation System Inertia:#???  6-   ?? 5#   
   $??1??6   $??0??6     Compensation Torque Offset:#???  6-   ?? 5$   
   $??2??6   $??0??6     Friction Compensation:#???  6-   ?? 5%   
   $??3??6   $??0??6    ! Friction Compensation Window:#???  6-     5'   
   -   5        $??0??5'      $??1??6)   $??Center=?69   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Minimum$ 0X?5(      ?  ?Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   FormatString$ ######0.0######??5,         Value 5)      ?? *SystemInertia+ ?? UpdateSourceTrigger$ 	LostFocus=?{?5+      ?? *EditEnabled+5,        5-   
   -   5        $??1??5-      $??1??6)   $??Center=?69   $
 -100X?5.      	   Maximum$		 100X?6+   $ ######0.0######??52       5/      ?? *TorqueOffset+$ 	LostFocus=?{?51      ?? *EditEnabled+52        53   
   -   5        $??2??53      $??1??6)   $??Center=?69   $ 0X?54      $		 100X?6(   $ ######0.0######??58       55      ?? *FrictionCompensationSliding+$ 	LostFocus=?{?57      ?? *EditEnabled+58        59   
   -   5        $??3??59      $??1??6)   $??Center=?69   $ 0X?5:      $ ######0.0######??5>       5;      ?? *FrictionCompensationWindow+$ 	LostFocus=?{?5=      ?? *EditEnabled+5>      ?? 5@   
   $??0??6   $??2??6"   
 ?? Orientation$
 
Horizontal=?5A      $??Center=?6.   .+?5B      ?? #??[  6   $??6??68   $??5??6B   ?? 5C        % Rate/(Rev/s^2)#???  6   5D      ?? 5E   
   $??1??6   $??2??6"   $
 
Horizontal=?5F      $??Center=?6.   .+?5G      ?? #??[  6   $??6??68   $??5??6B   ?? 5H        % Rated#???  6   5I      ?? 5J   
   $??2??6   $??2??6"   $
 
Horizontal=?5K      $??Center=?6.   .+?5L      ?? #??[  6   $??6??68   $??5??6B   ?? 5M      #???  6   5N      ?? 5O   
   $??3??6   $??2??6"   $
 
Horizontal=?5P      $??Center=?6.   .+?5Q      ?? #??[  6   $??6??68   $??5??6B   ?? 5R      ??6   ?? *PositionUnits+5S      ?? 5U   
   $??0??6   $??3??6    ! Backlash Compensation Window:#???  6-   ?? 5V   
   $??1??6   $??3??6      Load Observer Configuration:#???  6-   ?? 5W   
   $??2??6   $??3??6    	 Load Observer Bandwidth:#??? 	 6-   {?6b   ?? *LoadObserverEnabled+?? 5X   
   $??3??6   $??3??6    '
 #Load Observer Integrator Bandwidth:#??? 
 6-   {?6m   ?? *LoadObserverEnabled+  5Z   
   $??0??6   $??4??6)   $??Center=?69   $ 0X?5[      $ ######0.0######??5^       5\      ?? *BacklashCompensationWindow+$ 	LostFocus=?{?5]      ?? *EditEnabled+5^      ?? 5_   
   $??1??6   $??4??6!   ??61   ?? *LoadObserverConfiguration+  5`   
   $??2??6   $??4??6)   $??Center=?69   $ 0X?5a      $ ######0.0######??5d       5b      ?? *LoadObserverBandwidth+$ 	LostFocus=?{?5c      ?? *LoadObserverEditEnabled+5d        5e   
   $??3??6   $??4??6)   $??Center=?69   $ 0X?5f      $ ######0.0######??5i       5g      ?? *!LoadObserverIntegratorBandwidth+$ 	LostFocus=?{?5h      ?? *LoadObserverEditEnabled+5i      ?? 5k   
   $??0??6   $??5??6"   $
 
Horizontal=?5l      $??Center=?6.   .+?5m      ?? #??[  6   $??6??68   $??5??6B   ?? 5n      ??6   ?? *PositionUnits+5o      ?? 5p   
   $??2??6   $??5??6"   $
 
Horizontal=?5q      $??Center=?6.   .+?5r      ?? #??[  6   $??6??68   $??5??6B   ?? 5s       	 Hertz#???  6   {?66   ?? *LoadObserverEnabled+5t      ?? 5u   
   $??3??6   $??5??6"   $
 
Horizontal=?5v      $??Center=?6.   .+?5w      ?? #??[  6   $??6??68   $??5??6B   ?? 5x      #???  6   {?66   ?? *LoadObserverEnabled+5y      5{      5|      