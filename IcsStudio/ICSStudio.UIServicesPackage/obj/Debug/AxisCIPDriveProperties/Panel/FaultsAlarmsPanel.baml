   M S B A M L   `   `   `  ????  ICSStudio.UIServicesPackageP    JICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel.FaultsAlarmsPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        ? 	Resources  5       ?? MergedDictionaries ?? Source?? 5	   
    5
      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ? {?6   ?? *IsLogEnable+ ? RowDefinitions 5   
   ?? 5      $	??395 ?6   ?? 5      $
??Auto ?6   .+?5   
   &   System.Windows.Controls.DataGrid  $??0??6      RowHeaderWidth$ 25Q?5         AutoGenerateColumns .     CanUserAddRows .     CanUserResizeRows .     
IsReadOnly . ??5      ?? *AxisLogSource+	   RowStyle	 5      ?? 5      
 ?? 
TargetType)  #System.Windows.Controls.DataGridRow"
  6   .?5      ??  ?? Property ?@?ToolTip ?? Value 5       ?? 5!      ??6(   ?? *	ToolTip+5#      5$         Columns 5&      0  *System.Windows.Controls.DataGridTextColumn  5'      ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 6%      
IsReadOnly .    CanUserSort . 1  +System.Windows.Controls.DataGridBoundColumn   Binding 5)   %   ?? *
DateTime+   HeaderTemplate 5*      ?? 5+      .R?5,      ??    	Date/Time#???   6(   6M   5-      5/        50      $	 100 6%    .  .   52   %   ?? *Source+ 53      ?? 54      .R?55      ??  
 Source#???  6(   6J   56      58        59      $	 170 6%    .  .   5;   %   ?? *	Condition+ 5<      ?? 5=      .R?5>      ??   	Condition#???  6(   6M   5?      5A        5B      $	 100 6%    .  .   5D   %   ?? *	SubCode+ 5E      ?? 5F      .R?5G      ??   Sub Code#???  6(   6L   5H      5J        5K         Header$ Action??6%   $	 170 65    .  .   5M   %   ?? *Action+ 5N      ?? 5O      .R?5P      ??  
 Action#???  6(   6J   5Q      5S        5T      $	 170 6%    .  .   5V   %   ?? *
EndState+ 5W      ?? 5X      .R?5Y      ??   	End State#???  6(   6M   5Z      5\      5^      ]? 5`   
   $??1??6   .+?5a      ??   	Clear Log#???  6   $??Right=?6;   ??5b      ?? *ClearLogCommand+?? 5d        CIPTag FaultsAlarms Show#???  6   $??Center=?6J   ?? 5e       
 Faults#???  6   $??Center=?6:    d? 	IsChecked 6U   ?? *IsFaultVisible+?? 5f       	 CIPTag FaultsAlarms Alarms#??? 	 6   $??Center=?6N    6i   ?? *IsAlarmVisible+?? 5g       

 Resets#??? 
 6   $??Center=?6:    6U   ?? *IsResetVisible+5h      5k      5l      