   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5      h dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fq=clr-namespace:ICSStudio.UIServicesPackage.PLCProperties.Panel/ICSStudio.UIServicesPackage.PLCProperties.Panel 5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5	      F   @ICSStudio.UIServicesPackage.PLCProperties.Panel.InternetProtocol   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Pcontrols;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Olocal=clr-namespace:ICSStudio.UIServicesPackage.PLCProperties.Panel     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      4s&clr-namespace:System;assembly=mscorlib     5	      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5
        ? 	Resources  5       ?? 
TargetType ?? Triggers P? Property P? Value ?? Property ?? Value ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %?   '??       ?? 5      " ??6    5      P? 5       ? {? .  . ?5      ??  ? ??
 ?????6F   5      .?5      ??  ? {? 6.   ?? *Enable+6I   ?? 5       ? ??$ Center=?66   6G   5      5      .??5      ? $??20q?6     5   
   %  Xceed.Wpf.Toolkit.MaskedTextBox??%R   '        ?? 5       "  6   .?5!      ??  ? ??$ 18??6+   5"      5#      	 ? RowDefinitions	 5$   
   ?? 5%      $??50 ?6   ?? 5&      $??80 ?6   ?? 5'      $??50 ?6   ?? 5(      $??50 ?6   ?? 5)      $??50 ?6   ?? 5*      $??50 ?6   
 ? ColumnDefinitions
 5,   
   ?? 5-      ?? 5.      $??2* ?6   ?? 5/      ?? 50      $??2* ?6   .+?52   
   ? $??0??53      $??0??54      $??4??55      y?56      ?? *DualIpVisibility+
 57      ?? 58      $
??Auto ?6#   ?? 59      $
??Auto ?6#   ?? 5:      $??* ?6#   .+?5<      ??  	  Port:#???   6   ?? 5=      $??1??5>      $??78??5?      $??22??5@       ?? DisplayMemberPath$ Label??5A      {?5B      ?? *
IsOnline+??5C      ?? *	PortNum+ ?? SelectedItem 5D      ?? *SelectedPort+?? 5E      $??2??5F      $??20,0,0,0q?5G      $??Stretch=?5H      5I      ?? 5K   
   $??1??5L      $??0??5M      $??4??5N      .+?5O      	?  " Manually configure IP settings#???  5P       	? 	GroupName$ group1??5Q      {?.   d? 	IsChecked 5R      ?? *
IsCheck1+5S      	? 5T       / +Obtain IP settings automatically using DHCP#???  5U      $ group1??5V      {?.  5W      5X      ?? 5Z   
   $??2??5[      $??0??5\        IP Address:#???  5]      ,   &ICSStudio.Gui.Controls.IPMaskedTextBox  5c   
   $??2??5d      $??1??5e      $	??150??5f      	   IP 5g      ?? *	IPAddress+ ?? UpdateSourceTrigger$ PropertyChanged=? ?? Mode$ TwoWay=?{?5h      ?? *Enable+?? 5i   
   $??2??5j      $??2??5k        Subnet Mask:#???  5l        5r   
   $??2??5s      $??3??5t      $	??150??5u       5v      ?? *Subnet+$ PropertyChanged=?$ TwoWay=?{?5w      ?? *Enable+?? 5x   
   $??3??5y      $??2??5z        Gateway Address:#???  5{      $??WrapWithOverflow=?5|        5?   
   $??3??5?      $??3??5?      $	??150??5?       5?      ?? *	Gateway+$ PropertyChanged=?$ TwoWay=?{?5?      ?? *Enable+?? 5?   
   $??4??5?      $??0??5?        Domain Name:#???  5?      ?? 5?   
   $??4??5?      $??1??5?      $	??150??5?      $??20??5?      {?5?      ?? *Enable+??5?      ?? *Domain+$ PropertyChanged=??? 5?   
   $??4??5?      $??2??5?        Primary DNS Server Address:#???  5?      $??WrapWithOverflow=?5?        5?   
   $??4??5?      $??3??5?      $	??150??5?       5?      ?? *	Primary+$ PropertyChanged=?$ TwoWay=?{?5?      ?? *Enable+?? 5?   
   $??5??5?      $??0??5?        
Host Name:#???  5?      ?? 5?   
   $??5??5?      $??1??5?      $	??150??5?      $??20??5?      {?5?      ?? *Enable+??5?      ?? *Host+$ PropertyChanged=??? 5?   
   $??5??5?      $??2??5?       !	 Secondary DNS Server Address:#??? 	 5?      $??WrapWithOverflow=?5?        5?   
   $??5??5?      $??3??5?      $	??150??5?       5?      ?? *	Secondary+$ PropertyChanged=?$ TwoWay=?{?5?      ?? *Enable+5?      5?      