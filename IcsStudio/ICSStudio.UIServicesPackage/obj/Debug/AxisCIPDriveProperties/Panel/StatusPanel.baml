   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5       ICSStudio.UIServicesPackageJ   DICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel.StatusPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Pcontrols;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5        ? 	Resources  5	       ?? MergedDictionaries ?? Source-   'ICSStudio.Gui.Controls.StatusBitControl ?? 
TargetType ?? Property ?? Value?? 5
   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      ?? 5      $Y Spack://application:,,,/ICSStudio.Gui;component/Resources/styles/CustomControls.xaml??5      %Q   '        ?? 5      "  6   .?5      ??  ? ??$ 0q?6+   5      5      .??5      ?  ? IsSharedSizeScope .  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5!   
   ? $??0??6   $??0q?6    5"      ?? 5#      $
??Auto ?6    ?? 5$      $
??Auto ?6    ?? 5%      $??10 ?6     ? ColumnDefinitions 5'      ?? 5(      $	??150 ?6#   ?? 5)      $	??200 ?6#   ?? 5*      $	??150 ?6#   ?? 5+      $	??200 ?6#   .+?5.      ?? $??0??6   $??0??6!      Actual Position:#???   61   6`   ?? 5/      $??1??6   $??0??6!     Command Position:#???  6V   {?61   ?? *CommandEnabled+6?   ?? 51      $??0??6   $??2??6!     Actual Velocity:#???  61   6`   ?? 52      $??1??6   $??2??6!     Command Velocity:#???  6V   {?61   ?? *CommandEnabled+6?   ?? 54      $??0??6   $??1??6!   ??61   ?? *ActualPosition+y?55      ?? *ActualVisibility+?? 56      $??1??6   $??1??6!   ??61   ?? *CommandPosition+y?57      ?? *CommandVisibility+?? 59      $??0??6   $??3??6!   ??61   ?? *ActualVelocity+y?5:      ?? *ActualVisibility+?? 5;      $??1??6   $??3??6!   ??61   ?? *CommandVelocity+y?5<      ?? *CommandVisibility+5>      ? 5@   
   $??1??6   $??0q?6    5A      ?? 5B      $
??Auto ?6#   ?? 5C      $??* ?6#   .+?5E      ?? $??0??6   $
??Bold"?6(     Axis Status#???  6:   6a   ?? 5F      $??1??6   5G      ? 5I   
   $??2??6   $??0q?6    5J      ?? 5K      $
??Auto ?6    ?? 5L      $??5 ?6     5N      ?? 5O      $??0 ?6#   	 z? SharedSizeGroup$	 Group0??6.   ?? 5P      $
??Auto ?6#   $	 Group1??60   ?? 5Q      $
??Auto ?6#   $	 Group2??60   ?? 5R      $
??Auto ?6#   $	 Group3??60   ?? 5S      $
??Auto ?6#   $	 Group4??60   .+?5V      ?? $??0??6   $??1??6&   .+?5W          AC Line Power Loss
   Text#
 ?  5X         IsSet 6A   ?? *ACPowerLoss+ ?? Mode$ OneWay=?  5Y        AC Line Phase Loss#
 ?  5Z      {?.  6A     5[        AC Line Frequency Change#
 ?  5\      {?.  6G     5]        AC Line Sync Loss#
 ?  5^      {?.  6@     5_       	 AC Line Sync#
 ? 	 5`      {?.  6;     5a       
 AC Line Voltage Sag#
 ? 
 5b      {?.  6B     5c        Regenerating#
 ?  5d      {?.  6;     5e        Regenerative Power Limit#
 ?  5f      {?.  6G   5g      ?? 5i      $??0??6   $??2??6&   .+?5j          	DC Bus Up#
 ?  5k       68   ?? *	DCBusUp+$ OneWay=?  5l        DC Bus Unload#
 ?  5m       6<   ?? *DCBusUnload+$ OneWay=?  5n        
Power Loss#
 ?  5o        5p        	Ride Thru#
 ?  5q        5r        Shutdown#
 ?  5s       67   ?? *
Shutdown+$ OneWay=?  5t        Reactive Power Limit#
 ?  5u      {?.  6C     5v        Motoring#
 ?  5w      {?.  67     5x        Motoring Power Limit#
 ?  5y      {?.  6C   5z      ?? 5|      $??0??6   $??3??6&   .+?5}          Power Structure Enabled#
 ?  5~       6F   ?? *PowerStructureEnabled+$ OneWay=?  5        Tracking Command#
 ?  5?       6?   ?? *TrackingCommand+$ OneWay=?  5?        Feedback Integrity#
 ?  5?       6A   ?? *FeedbackIntegrity+$ OneWay=?  5?        Position Lock#
 ?  5?       6<   ?? *PositionLock+$ OneWay=?  5?        Velocity Lock#
 ?  5?       6<   ?? *VelocityLock+$ OneWay=?  5?        Velocity Standstill#
 ?  5?       6B   ?? *VelocityStandstill+$ OneWay=?  5?        Bus Voltage Lock#
 ?  5?      {?.  6?     5?        Active Current Limit#
 ?  5?      {?.  6C   5?      ?? 5?      $??0??6   $??4??6&   .+?5?          Velocity Threshold#
 ?  5?       6A   ?? *VelocityThreshold+$ OneWay=?  5?        Velocity Limit#
 ?  5?       6=   ?? *VelocityLimit+$ OneWay=?  5?        Torque Threshold#
 ?  5?       6?   ?? *TorqueThreshold+$ OneWay=?  5?         Torque Limit#
 ?   5?       6;   ?? *TorqueLimit+$ OneWay=?  5?       ! Current Limit#
 ? ! 5?       6<   ?? *CurrentLimit+$ OneWay=?  5?       " Thermal Limit#
 ? " 5?       6<   ?? *ThermalLimit+$ OneWay=?  5?       # Bus Voltage Limit#
 ? # 5?      {?.  6@     5?       $ Reactive Current Limit#
 ? $ 5?      {?.  6E   5?      5?      ? 5?   
   $??3??6   $??0q?6    5?      ?? 5?      $
??Auto ?6#   ?? 5?      $??* ?6#   .+?5?      ?? $??0??6   $
??Bold"?6(    % Digital I/O#??? % 6:   6a   ?? 5?      $??1??6   5?      ? 5?   
   $??4??6   $??0q?6    5?      ?? 5?      $
??Auto ?6     5?      ?? 5?      $??0 ?6#   $	 Group0??6-   ?? 5?      $
??Auto ?6#   $	 Group1??60   ?? 5?      $
??Auto ?6#   $	 Group2??60   ?? 5?      $
??Auto ?6#   $	 Group3??60   ?? 5?      $
??Auto ?6#   $	 Group4??60   .+?5?      ?? $??0??6   $??1??6&   .+?5?         & StatusEnable#
 ? & 5?       6;   ?? *Enable+$ OneWay=?  5?       ' Home#
 ? ' 5?       63   ?? *Home+$ OneWay=?  5?       ( Registration 1#
 ? ( 5?       6=   ?? *Registration1+$ OneWay=?  5?       ) Registration 2#
 ? ) 5?       6=   ?? *Registration2+$ OneWay=?5?      ?? 5?      $??0??6   $??2??6&   .+?5?         * Contactor Enable Out#
 ? * 5?        5?       + Pre-Charge OK#
 ? + 5?        5?       , Positive Overtravel#
 ? , 5?       6B   ?? *PositiveOvertravel+$ OneWay=?  5?       - Negative Overtravel#
 ? - 5?       6B   ?? *NegativeOvertravel+$ OneWay=?  5?       . Feedback Thermostat OK#
 ? . 5?       6E   ?? *FeedbackThermostat+$ OneWay=?5?      ?? 5?      $??0??6   $??3??6&   .+?5?         / AC Line Contactor OK#
 ? / 5?        5?       0 Regenerative Power OK In#
 ? 0 5?        5?       1 Converter OK In#
 ? 1 5?        5?       2 Bus Capacitor OK#
 ? 2 5?        5?       3 Motor Thermostat OK#
 ? 3 5?       6B   ?? *MotorThermostat+$ OneWay=?5?      ?? 5?      $??0??6   $??4??6&   .+?5?         4 Mechanical Brake#
 ? 4 5?       6?   ?? *MechanicalBrake+$ OneWay=?  5?       5 Regenerative Power OK Out#
 ? 5 5?      {?.  6H     5?       6 Converter OK Out#
 ? 6 5?      {?.  6?     5?       7 Bus Conditioner Module OK#
 ? 7 5?        5?       8 Shunt Thermal Switch OK#
 ? 8 5?      5?      5?      5?      5?      