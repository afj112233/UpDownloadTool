   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5       ICSStudio.UIServicesPackage[   UICSStudio.UIServicesPackage.AxisCIPDriveProperties.Controls.RotaryInductionParameters   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5        ? 	Resources  5
      3   -ICSStudio.Gui.Controls.SingleUpDownExtensions ?? 
TargetType ?? Property ?? Value%  Xceed.Wpf.Toolkit.IntegerUpDown??%?   '        '  G     ?? 5   
   "  6   .?5      ??  ? ??$ 2q?6'   5      ?? 5   
   "  6   .?5      ??  ? ??$ 2q?6'   5      5      .??5      ?  ? RowDefinitions 5   
   ?? 5      ?? 5      .+?5   
   ? $??0??6    ? ColumnDefinitions 5      ?? 5      $
??Auto ?6#   ?? 5      $??* ?6#   .+?5      ?? $??0??6   $??0??6%    ,  (NameplateDatasheetPhaseToPhaseParameters#???   5      $
??Bold"?6Z   ?? 5       $??0??6   $??1??6%   5!      ? 5$   
   $??1??6    5%      ?? 5&      $
??Auto ?6    ?? 5'      $
??Auto ?6    ?? 5(      $
??Auto ?6    ?? 5)      $
??Auto ?6    ?? 5*      $
??Auto ?6     5,      ?? 5-      $??20 ?6#   ?? 5.      $	??100 ?6#   ?? 5/      $??80 ?6#   ?? 50      $	??100 ?6#   ?? 51      $	??140 ?6#   ?? 52      $??80 ?6#   ?? 53      .+?57      ?? $??0??6   $??1??6%     Rated Power:#???  65     58      $??0??6*   $??2??67   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .     Minimum$ 0X?5:   *   ,  &Xceed.Wpf.Toolkit.Primitives.InputBase	   TextAlignment$
	 Left=?66   
   Value
 5;   *   ?? *MotorRatedOutputPower+?? 5<      $??0??6   $??3??6%   kW69   ?? 5>      $??0??6   $??4??6%     Pole Count:#???  65     5?      $??0??6!   $??5??6.   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .     Minimum$ 2X?5A   !   $
	 Left=?6-      Value 5B   !   ?? *RotaryMotorPoles+?? 5E      $??1??6   $??1??6%     Rated Voltage:#???  65     5F      $??1??6*   $??2??67    .   .  $ 0X?5H   *   $
	 Left=?66   
 5I   *   ?? *MotorRatedVoltage+?? 5J      $??1??6   $??3??6%     
Volts(RMS)#???  65   ?? 5M      $??1??6   $??4??6%     Rated Frequency:#???  65     5N      $??1??6*   $??5??67    .   .  $ 0X?5P   *   $
	 Left=?66   
 5Q   *   ?? *InductionMotorRatedFrequency+?? 5R      $??1??6   $??6??6%    	 Hertz#???  65   ?? 5U      $??2??6   $??1??6%     Rated Speed:#???  65     5V      $??2??6*   $??2??67    .   .  $ 0X?5X   *   $
	 Left=?66   
 5Y   *   ?? *RotaryMotorRatedSpeed+?? 5Z      $??2??6   $??3??6%     RPM#???  65   ?? 5\      $??2??6   $??4??6%    	 
Max Speed:#??? 	 65     5]      $??2??6*   $??5??67   $
	 Left=?6G    .   .   5_   *   ?? *RotaryMotorRatedSpeed+
 5`   *   ?? *RotaryMotorMaxSpeed+?? 5a      $??2??6   $??6??6%   #???  65   ?? 5d      $??3??6   $??1??6%    
 Rated Current:#??? 
 65     5e      $??3??6*   $??2??67    .   .  $ 0X?5g   *   $
	 Left=?66   
 5h   *   ?? *MotorRatedContinuousCurrent+?? 5i      $??3??6   $??3??6%     	Amps(RMS)#???  65   ?? 5l      $??3??6   $??4??6%     Peak Current:#???  65   y?5m      ?? *!MotorRatedPeakCurrentVisibility+  5n      $??3??6*   $??5??67    .   .  $
	 Left=?5p   *    5q   *   ?? *MotorRatedContinuousCurrent+
 5r   *   ?? *MotorRatedPeakCurrent+y?5s   *   ?? *!MotorRatedPeakCurrentVisibility+?? 5t      $??3??6   $??6??6%   #???  65   y?5u      ?? *!MotorRatedPeakCurrentVisibility+?? 5x      $??4??6   $??4??6%     Motor Overload Limit:#???  65     5y      $??4??6*   $??5??67   $
	 Left=?6G    .   .  $ 0X?5{   *      Maximum$	 200X?66   
 5|   *   ?? *MotorOverloadLimit+?? 5}      $??4??6   $??6??6%     % Rated#???  65   5      5?      5?      