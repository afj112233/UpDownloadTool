   M S B A M L   `   `   `  ????  ICSStudio.UIServicesPackageT    NICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel.CyclicParametersPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        ? 	Resources  5       ?? MergedDictionaries ?? Source?? 5	   
    5
      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $	??400 ?6    ? ColumnDefinitions 5   
   ?? 5      $	??310 ?6   ?? 5      $??10 ?6   ?? 5      $	??310 ?6   .+?5   
   ?? $??0??6   $??0??6!   $
??Bold"?61    %  !Parameters to be read each cycle:#???   6C   6?   ?? 5   
   $??0??6   $??2??6!   $
??Bold"?61    ( $Parameters to be written each cycle:#???  6C   6?   &   System.Windows.Controls.DataGrid  5   
   $??1??6   $??0??6       RowHeaderWidth$ 0Q?5         AutoGenerateColumns .     CanUserAddRows .   %? VirtualizationMode$ Standard=?5"      ??5!      ?? *ParametersToBeReadSource+	   RowStyle	 5#      ?? 5$      
 ?? 
TargetType)  #System.Windows.Controls.DataGridRow"
  6   .?5%      ??  ?? Property ? y? ?? Value 63   ?? *
Visibility+ ?? Mode$ OneWay=??? 5&       ? {? 62   ?? *	IsEnabled+$ OneWay=?5'         Columns 5)      4  .System.Windows.Controls.DataGridCheckBoxColumn  5*      ,  &System.Windows.Controls.DataGridColumn   Header$  ??6)      Width5  /System.Windows.Controls.DataGridLengthConverter$ 15 63   1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle 5+      ?? 5,      "
 ??6    .?5-      ??  ?@d?	IsChecked 6:   ?? *	Checked+ ?? UpdateSourceTrigger$ PropertyChanged=?5.         EditingElementStyle 50      ?? 51      "
 ??6    .?52      ??  ?@d?	IsChecked 6:   ?? *	Checked+$ PropertyChanged=?53      55      0  *System.Windows.Controls.DataGridTextColumn  57         
IsReadOnly . $	 190 67      HeaderTemplate 58      ?? 59      .R?5:      ?? #??? ??6(   6H   5;       5=      ?? 5>      "
 ??6    .?5?      ??  ? ?? 65   ?? *Name+?? 5@       ? ??$ -1q?67   ?? 5A       ? ??
 ?????6;   ?? 5B       ? ??$ CharacterEllipsis=?6=   ?? 5C       ?@?ToolTip 68   ?? *Name+5D      5F        5H       . $ 80 67    5I      ?? 5J      .R?5K      ??  	 Value#???  6(   6I   5L       5N      ?? 5O      "
 ??6    .?5P      ??  ? ?? 65   ?? *Value+?? 5Q       ?@??TextAlignment$ Right=?6>   ?? 5R       ? ??$ -1q?67   ?? 5S       ? ??
 ?????6;   5T      5V      5Y        5[   
   $??1??6   $??2??6    $ 0Q?5\       .   .  $ Standard=?5_      ??5^      ?? *ParametersToBeWrittenSource+	 5`      ?? 5a      "
  6   .?5b      ??  ? y? 63   ?? *
Visibility+$ OneWay=??? 5c       ? {? 62   ?? *	IsEnabled+$ OneWay=?5d       5f        5g      $  ??6)   $ 15 63    5h      ?? 5i      "
 ??6    .?5j      ??  ?@d?	IsChecked 6:   ?? *	Checked+$ PropertyChanged=?5k       5m      ?? 5n      "
 ??6    .?5o      ??  ?@d?	IsChecked 6:   ?? *	Checked+$ PropertyChanged=?5p      5r        5t       . $	 190 67    5u      ?? 5v      .R?5w      ?? #??? ??6(   6H   5x       5z      ?? 5{      "
 ??6    .?5|      ??  ? ?? 65   ?? *Name+?? 5}       ? ??$ -1q?67   ?? 5~       ? ??
 ?????6;   ?? 5       ? ??$ CharacterEllipsis=?6=   ?? 5?       ?@?ToolTip 68   ?? *Name+5?      5?        5?       . $ 80 67    5?      ?? 5?      .R?5?      ?? #???  6(   6I   5?       5?      ?? 5?      "
 ??6    .?5?      ??  ? ?? 65   ?? *Value+?? 5?       ?@??TextAlignment$ Right=?6>   ?? 5?       ? ??$ -1q?67   ?? 5?       ? ??
 ?????6;   5?      5?      5?      5?      5?      