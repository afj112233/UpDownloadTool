   M S B A M L   `   `   `  ????   ICSStudio.UIServicesPackageP    JICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel.FaultsAlarmsPanel   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008       ? 	Resources   ?? MergedDictionaries ?? Source??  ?? $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??%    .??? {??? *IsLogEnable+ ? RowDefinitions ?? $	??395 ??? $
??Auto ?.+?&   System.Windows.Controls.DataGrid  $??0??   RowHeaderWidth$ 25Q?   AutoGenerateColumns .     CanUserAddRows .     CanUserResizeRows .     
IsReadOnly . ???? *AxisLogSource+	   RowStyle	 ?? 
 ?? 
TargetType)  #System.Windows.Controls.DataGridRow"
  .???  ?? Property ?@?ToolTip ?? Value ?? ???? *	ToolTip+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$	 100    
IsReadOnly .    CanUserSort . 1  +System.Windows.Controls.DataGridBoundColumn   Binding ?? *
DateTime+   HeaderTemplate ?? .R???    	Date/Time#???     $	 100  .  .   ?? *Source+ ?? .R???  
 Source#???    $	 170  .  .   ?? *	Condition+ ?? .R???   	Condition#???    $	 100  .  .   ?? *	SubCode+ ?? .R???   Sub Code#???       Header$ Action??$	 170  .  .   ?? *Action+ ?? .R???  
 Action#???    $	 170  .  .   ?? *
EndState+ ?? .R???   	End State#???  ]? $??1??.+???   	Clear Log#???  $??Right=????? *ClearLogCommand+??   CIPTag FaultsAlarms Show#???  $??Center=???  
 Faults#???  $??Center=? d? 	IsChecked ?? *IsFaultVisible+??  	 CIPTag FaultsAlarms Alarms#??? 	 $??Center=? ?? *IsAlarmVisible+??  

 Resets#??? 
 $??Center=? ?? *IsResetVisible+