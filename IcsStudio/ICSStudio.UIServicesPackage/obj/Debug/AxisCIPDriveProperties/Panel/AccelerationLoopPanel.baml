   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5       ICSStudio.UIServicesPackageT   NICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel.AccelerationLoopPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5        ? 	Resources  5	       ?? MergedDictionaries ?? Source?? 5
   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ? {?6   ?? *IsAccelerationLoopEnabled+ ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      $	??160 ?6   ?? 5      $	??100 ?6   ?? 5      $	??140 ?6   .+?5   
   ?? $??0??6   $??0??6!      Acceleration Limit:#???   61   6`   3   -ICSStudio.Gui.Controls.SingleUpDownExtensions  5   
   $??0??6&   $??1??63   $??Center=?6C   $	??0,3q?6^   X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .     Minimum$ 0X?5   &   ,  &Xceed.Wpf.Toolkit.Primitives.InputBase   TextAlignment$
 Left=?62   	   Value	 5    &   ?? *AccelerationLimit+?? 5!   
   $??0??6   $??2??6!   ??61   ?? *PositionUnits+
 ?? StringFormat$
 {0}/s^2???? 5#   
   $??1??6   $??0??6!     Deceleration Limit:#???  61   6`     5$   
   $??1??6&   $??1??63   $??Center=?6C   $	??0,3q?6^    .   .  $ 0X?5&   &   $
 Left=?62   	 5'   &   ?? *DecelerationLimit+?? 5(   
   $??1??6   $??2??6!   ??61   ?? *PositionUnits+$
 {0}/s^2??5+      5,      