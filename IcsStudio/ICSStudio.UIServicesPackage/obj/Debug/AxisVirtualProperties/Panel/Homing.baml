   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5       ICSStudio.UIServicesPackageD   >ICSStudio.UIServicesPackage.AxisVirtualProperties.Panel.Homing   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5      -   5        $	??650??5      $	??350??5	        ? 	Resources  5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $??40 ?6   ?? 5      $??40 ?6   ?? 5      $??40 ?6    ? ColumnDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5    
   ?? $??0??5!      $??0??5"       	  Mode:#???   5#      ?? 5$   
   $??0??5%      $??1??5&      ??5'      ?? *Mode+?? 5(   
   $??1??5)      $??0??5*        	Position:#???  5+      3   -ICSStudio.Gui.Controls.SingleUpDownExtensions  5,   
   -   5          Name$ SingleUpDown??5-      $??1??5.      $??1??5/      $	??137??50      $??23??51      $
??Left=?52      $??Center=?53      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .  ,  &Xceed.Wpf.Toolkit.Primitives.InputBase   TextAlignment$
 Left=?57      {?55      ?? *
IsEnable+	   Value	 58      ?? *
Position+
 ?? UpdateSourceTrigger$
 PropertyChanged=? ?? Delay$ 30???? 59   
   $??1??5:      $??2??5;      ??5<      ?? *PositionUnits+?? 5=   
   $??2??5>      $??0??5?        	Sequence:#???  5@      ?? 5A   
   $??2??5B      $??1??5C      ??5D      ?? *
Sequence+5E      5F      