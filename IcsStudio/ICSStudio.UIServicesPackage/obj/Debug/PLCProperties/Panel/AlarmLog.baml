   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fq=clr-namespace:ICSStudio.UIServicesPackage.PLCProperties.Panel/ICSStudio.UIServicesPackage.PLCProperties.Panel  5      >    8ICSStudio.UIServicesPackage.PLCProperties.Panel.AlarmLog   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Olocal=clr-namespace:ICSStudio.UIServicesPackage.PLCProperties.Panel     5        ? 	Resources  5	       ?? MergedDictionaries ?? Source?? 5
   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $??40 ?6   6*   ?? 5      6    ? ColumnDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5   
   ?? $??0??6      Clear Alarm Log#???   6   $	??123??6J   $
??Left=?6V   $??23??6q   {?6}   ?? *
IsOnline+6?   ?? 5   
   $??0??5      $??1??5       ?? Orientation$ 
Horizontal=?5       y?5      ?? *	IsVisibly+.+?5!      ?? $??15??5"      $??15??5#      $??	25,10,0,0q?5$      $
??Left=?5%      $	??Top=?5&      $??Images/Alert.png??5'      ?? 5(      
?????? g cClear log causes undelivered alarm log messages to not be delivered to subscribers of the alarm log#???  60   5)      ?? 5*   
   $??1??6     Last Clear State:#???  6!   $	??Top=?6L   $??0,30,0,0q?6d   6x   5+      5,      