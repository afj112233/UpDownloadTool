   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5      h dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fq=clr-namespace:ICSStudio.UIServicesPackage.PLCProperties.Panel/ICSStudio.UIServicesPackage.PLCProperties.Panel 5      >   8ICSStudio.UIServicesPackage.PLCProperties.Panel.DateTime   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Pcontrols;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Olocal=clr-namespace:ICSStudio.UIServicesPackage.PLCProperties.Panel     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5        ? 	Resources  5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      $
??103* ?6   ?? 5      $
??auto ?6   ?? 5       $
??300* ?6   ?? 5!      $
??263* ?6   .+?5#   
   ?? $??0??5$      $??0??5%      $??4??5&       ?? Orientation$ 
Horizontal=?5'      .+?5(      ?? $??15??5)      $??15??5*      $??0,5,0,0q?5+      $
??Left=?5,      $	??Top=?5-      $??Images/Alert.png??5.      ?? 5/      
?????? =  9The Date and Time displayed here is Controller local time#???   60   50      ?? 52   
   $??1??53      $??2??54      $??23??55      {?.   + 'Set Date Time and Zone from Workstation#???  58      ??57      ?? *ResetCommand+58      ?? 5:   
   $??2??5;      $??0??5<      $??2??5=      $	??2,5q?5>        Date and Time:#???  5?      ?? 5@   
   $??2??5A      $??2??5B       ?? 
IsReadOnly . {?5C      ?? *
IsEnable+??5E      ?? *
DateTime+?? 5F   
   $??2??5G      $??3??5H      {?.  $	??140??5J      $??23??5K        Change Date and Time...#???  5M      ??5L      ?? *ChangedDateCommand+5M      ?? 5O   
   $??3??5P      $??0??5Q      $??2??5R      $	??2,5q?5S        
Time Zone:#???  5T      ?? 5U   
   $??3??5V      $??2??5W      $??23??5X      {?5Y      ?? *
IsEnable+?? 5[   
   -   5        $??4??5\      $??2??5]       & "Adjust for Daylight Saving(+00:00)#???  5_      {?5^      ?? *
IsEnable+5`      ]? 5c   
   $??5??5d      $??0??5e      $??4??5f      .+?5g      ?? $
??Bold"?6     Time Synchronize#???  6*   ?? 5h      5i      ? 5k   
   $??6??5l      $??0??5m      $??4??5n      .+?5o      ?  5p      ?? 5q      ?? 5r      .+?5t      ?? $??0??6   $ Vertical=?6-   .+?5u      ??   EnableTimeSynchronization#???  5v       d? 	IsChecked 5w      ?? *	PTPEnable+ ?? UpdateSourceTrigger$ PropertyChanged=?{?5x      ?? *IsEnableTimeSyncEnabled+?? 5z      $??0,10,0,0q?6!   $ Vertical=?63   .+?5{      	?   Is the system time master#???  6&   {?.  6\   	? 5|        	 Is a synchronized time slave#??? 	 6&   {?.  6_   	? 5}       !
 Duplicate CST master detected#??? 
 6&   {?.  6`   	? 5~        CST Mastership disabled#???  6&   {?.  6Z   	? 5        No CST master#???  6&   {?.  6P   5?      5?      ]? 5?      $??1??6   .+?5?      ?? $
??auto??5?      $??Right=?5?      $	??Top=?5?      $	??Top=?5?      $ 
Horizontal=?5?      .+?5?      ?? $??16??5?      $??16??5?      $	??Top=?5?      $??0,0,10,0q?5?      $??Images/AlertError.png??5?      ?? 5?       % !DANGER If time synchronization is#???  6$   5?      ?? 5?      $??80??5?      $??23??5?      $??0,10,0,0q?5?      $??Right=?5?        Advanced...#???  5?      ??5?      ?? *AdvancedCommand+5?      5?      5?      5?      5?      5?      