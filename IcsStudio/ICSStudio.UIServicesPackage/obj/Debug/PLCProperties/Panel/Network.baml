   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fq=clr-namespace:ICSStudio.UIServicesPackage.PLCProperties.Panel/ICSStudio.UIServicesPackage.PLCProperties.Panel  5      =    7ICSStudio.UIServicesPackage.PLCProperties.Panel.Network   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Olocal=clr-namespace:ICSStudio.UIServicesPackage.PLCProperties.Panel     5        ? 	Resources  5	       ?? MergedDictionaries ?? Source   AutoGray ?? 
TargetType ?? Triggers P? Property P? Value ?? Property ?? Value  TextAutoGray?? 5
   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %E  &	        &	 ?     ?? 5      " ??6%    5      P? 5       ? {? .  . ?5      ??  ? ?? ?	#FFDDDDDD67   5      5      ?? 5      " ??6)    5      P? 5       ? {? .  . ?5      ??  ? ?? ?	#FFDDDDDD67   5      .?5      ??  ? ??$ Center=?66   5      5      .??5       ? 	 ? RowDefinitions	 5!   
   ?? 5"      $
??Auto ?6   6,   ?? 5#      $
??Auto ?6   6,   ?? 5$      $
??Auto ?6   6,   ?? 5%      $
??Auto ?6   6,   ?? 5&      $
??Auto ?6   6,   ?? 5'      $
??Auto ?6   6,   ?? 5(      $
??Auto ?6   6,   
 ? ColumnDefinitions
 5*   
   ?? 5+      $??2* ?6   6,   ?? 5,      6!   .+?5.   
   ?? $??0??6   $??0??6!     Network Topology:#???  61   6^   ?? 5/   
   $??0??6   $??1??6   $??2??6.   $	??100??6?     Advanced...#???  6L   {?.  $??25??6?   6?   ?? 51   
   $??1??6   $??0??6!     Network Status:#???  61   6\   ?? 53   
   $??2??6   $??0??6"   .+?54      ??   Active Ring Supervisor:#???  6   
???????6b   ?? 55       ! Active Supervisor Precedence:#???  6   
???????6h   56      ?? 58   
   $??3??6   $??0??6      Enable Supervisor Mode#???  60   {?.  #??[  6u   59      ?? 5;   
   $??4??6   $??0??6!   $??25,0,0,0q?61     Ring Faults Detected:#???  6C   {?.  #??[ 6?   6?   ?? 5<   
   $??4??6   $??0??6   $	??100??6.   $??Right=?6:    	 Reset Counter#??? 	 6V   {?.  $??23??5=      6   ?? 5?   
   $??5??6   $??0??6!   $??25,0,0,0q?61    
 Supervisor Status:#??? 
 6C   {?.  #??[ 6?   6?   ?? 5A   
   $??6??6   $??0??6    $	??138??60   ??5B      ?? 5C      .R?5D      ??   
Ring Fault#???  6    5E      5G      5H      5I      