   M S B A M L   `   `   `  ????  ICSStudio.DeviceProperties<    6ICSStudio.DeviceProperties.Adapters.Panel.NetworkPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      $	??660??5      $
??Left=?5      .??5      ?   ? RowDefinitions  5   
   ?? 5      $??25 ?6   ?? 5      $??25 ?6   ?? 5      $??25 ?6   ?? 5      $	??100 ?6   ?? 5      $??25 ?6    ? ColumnDefinitions 5   
   ?? 5      $	??150 ?6   ?? 5      $	??200 ?6   ?? 5      $
??Auto ?6   .+?5   
   ?? $??0??5      $??0??5         NetworkMode#???   5      ?? 5   
   $??1??5      $??0??5         NetworkTopology#???  5!      ?? 5"   
   $??2??5#      $??0??5$        NetworkStatus#???  5%      ?? 5'   
   $??0??5(      $??1??5)        DeviceLevelRing#???  5*      ?? 5+   
   $??1??5,      $??1??5-      ??5.      ?? *NetworkTopology+y?5/      ?? *NetworkVisibility+?? 50   
   $??2??51      $??1??52      ??53      ?? *NetworkStatus+y?54      ?? *NetworkVisibility+?? 56   
   $??4??57      $??2??58      $??90??59        Refresh#???  5;      ??5:      ?? *RefreshCommand+5;      5<      5=      