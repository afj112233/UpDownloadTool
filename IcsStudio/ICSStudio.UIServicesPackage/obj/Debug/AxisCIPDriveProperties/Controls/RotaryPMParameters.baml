   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5       ICSStudio.UIServicesPackageT   NICSStudio.UIServicesPackage.AxisCIPDriveProperties.Controls.RotaryPMParameters   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      .??5
      ?   ? 	Resources  5   
   3   -ICSStudio.Gui.Controls.SingleUpDownExtensions ?? 
TargetType ?? Property ?? Value?? 5      %?   '        ?? 5      "  6   .?5      ??  ? ??$	 0,5q?6/   6=   ?? 5       ? ??$
 Left=?6C   6R   5      5       ? RowDefinitions 5   
   ?? 5      ?? 5      .+?5   
   ? $??0??6    ? ColumnDefinitions 5      ?? 5      $
??Auto ?6#   ?? 5      $??* ?6#   .+?5      ?? $??0??6   $??0??6%    ,  (NameplateDatasheetPhaseToPhaseParameters#???   5      $
??Bold"?6Z   ?? 5       $??0??6   $??1??6%   5!      ? 5$   
   $??1??6    5%      ?? 5&      ?? 5'      ?? 5(      ?? 5)      ?? 5*       5,      ?? 5-      $??20 ?6#   ?? 5.      $	??100 ?6#   ?? 5/      $??80 ?6#   ?? 50      $	??100 ?6#   ?? 51      $	??140 ?6#   ?? 52      $??80 ?6#   ?? 53      .+?56      ?? $??0??6   $??1??6%     Rated Power:#???  65   6]     57      $??0??6*   $??2??67   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .     Minimum$ 0X?59   *   ,  &Xceed.Wpf.Toolkit.Primitives.InputBase	   TextAlignment$
	 Left=?66   
   Value
 5:   *   ?? *MotorRatedOutputPower+{?5;   *   ?? *MotorParameterEditEnabled+?? 5<      $??0??6   $??3??6%   kW69   ?? 5>      $??1??6   $??1??6%     Rated Voltage:#???  65   6_     5?      $??1??6*   $??2??67    .   .  $ 0X?5A   *   $
	 Left=?66   
 5B   *   ?? *MotorRatedVoltage+{?5C   *   ?? *MotorParameterEditEnabled+?? 5D      $??1??6   $??3??6%     
Volts(RMS)#???  65   6[   ?? 5F      $??2??6   $??1??6%     Rated Speed:#???  65   6]     5G      $??2??6*   $??2??67    .   .  $ 0X?5I   *   $
	 Left=?66   
 5J   *   ?? *RotaryMotorRatedSpeed+{?5K   *   ?? *MotorParameterEditEnabled+?? 5L      $??2??6   $??3??6%     RPM#???  65   6T   ?? 5N      $??3??6   $??1??6%     Rated Current:#???  65   6_     5O      $??3??6*   $??2??67   $??6??6G    .   .  $ 0X?5Q   *   $
	 Left=?66   
 5R   *   ?? *MotorRatedContinuousCurrent+{?5S   *   ?? *MotorParameterEditEnabled+?? 5U      $??4??6   $??1??6%     Rated Torque:#???  65   6^     5V      $??4??6*   $??2??67   $??8??6G    .   .  $ 0X?5X   *   $
	 Left=?66   
 5Y   *   ?? *PMMotorRatedTorque+{?5Z   *   ?? *MotorParameterEditEnabled+?? 5[      $??4??6   $??3??6%     N-m#???  65   6T   ?? 5^      $??0??6   $??4??6%    	 Pole Count:#??? 	 65   6\   %  Xceed.Wpf.Toolkit.IntegerUpDown  5_      $??0??6!   $??5??6.   $??2??6>   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .     Minimum$ 2X?5a   !   $
	 Left=?6-      Value 5b   !   ?? *RotaryMotorPoles+{?5c   !   ?? *MotorParameterEditEnabled+?? 5e      $??2??6   $??4??6%    
 
Max Speed:#??? 
 65   6[     5f      $??2??6*   $??5??67   $
	 Left=?6G    .   .   5h   *   ?? *RotaryMotorRatedSpeed+
 5i   *   ?? *RotaryMotorMaxSpeed+{?5j   *   ?? *MotorParameterEditEnabled+?? 5k      $??2??6   $??6??6%   #???  65   6T   ?? 5m      $??3??6   $??3??6%     	Amps(RMS)#???  65   6Z   ?? 5o      $??3??6   $??4??6%     Peak Current:#???  65   6^     5p      $??3??6*   $??5??67   $
	 Left=?6G    .   .   5r   *   ?? *MotorRatedContinuousCurrent+
 5s   *   ?? *MotorRatedPeakCurrent+{?5t   *   ?? *MotorParameterEditEnabled+?? 5u      $??3??6   $??6??6%   #???  65   6Z   ?? 5w      $??4??6   $??4??6%     Motor Overload Limit:#???  65   6f     5x      $??4??6*   $??5??67   $
	 Left=?6G   $??9??6\    .   .  $ 0X?5z   *      Maximum$	 100X?66   
 5{   *   ?? *MotorOverloadLimit+{?5|   *   ?? *MotorOverloadLimitEnabled+?? 5}      $??4??6   $??6??6%     % Rated#???  65   6X   5      5?      5?      