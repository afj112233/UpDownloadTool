   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5       ICSStudio.UIServicesPackageC   =ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel.Units   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      -   5        $	??650??5	      $	??350??5
        ? 	Resources  5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $??30 ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      $	??170??6   $
??Auto ?6.   ?? 5      $
??Auto ?6   .+?5   
   ?? $??0??5       $??0??5!         Position Units:#???   5"      ?? 5#   
   -   5         ??Name$ NameBox??5$      $??0??5%      $??1??5&      $	??212??5'      $??23??5(      $
??Left=?5)      {?5*      ?? *
IsEnable+??5+      ?? *PositionUnits+ ?? UpdateSourceTrigger$ PropertyChanged=? ?? Delay$ 30???? 5,   
   $??1??5-      $??0??5.        Average Velocity Timebase:#???  5/      3   -ICSStudio.Gui.Controls.SingleUpDownExtensions  50   
   $??1??51      $??1??52      $	??100??53      $??23??54      $
??Left=?55      $??Center=?56      ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .  ?  ?Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]	   	MaxLength$	 16??58      
   ShowButtonSpinner
 .  ,  &Xceed.Wpf.Toolkit.Primitives.InputBase   TextAlignment$
 Left=?5:         Value 5;      ?? *AverageVelocityTimebase+$ PropertyChanged=?$ 30???? 5<   
   $??1??5=      $??1??5>        Seconds#???  5?      ??5@      u? 5A       u? Left$	 110Q?6   5C      5E      5F      