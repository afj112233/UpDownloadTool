   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5       ICSStudio.UIServicesPackageV   PICSStudio.UIServicesPackage.AxisCIPDriveProperties.Controls.RotaryInductionModel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5        ? 	Resources  5	      3   -ICSStudio.Gui.Controls.SingleUpDownExtensions ?? 
TargetType ?? Property ?? Value??%Q   '        ?? 5
   
   "  6   .?5      ??  ? ??$ 2q?6'   5      5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      $	??240 ?6   ?? 5      $	??100 ?6   ?? 5      $	??180 ?6   .+?5   
   ?? $??0??6   $??0??6!      Rated Flux Current:#???   61   6`   ?? 5   
   $??1??6   $??0??6!     Rated Slip Speed:#???  61   6^   ?? 5   
   $??2??6   $??0??6!     Stator Leakage(X1):#???  61   6`   ?? 5    
   $??3??6   $??0??6!     Rotor Leakage(X2):#???  61   6_   ?? 5!   
   $??4??6   $??0??6!   ?? 5"   
   $??5??6   $??0??6!     Stator Resistance(R1):#???  61   6c     5$   
   $??0??6&   $??1??63   X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .     Minimum$ 0X?5%   &   ,  &Xceed.Wpf.Toolkit.Primitives.InputBase	   TextAlignment$
	 Left=?5&   &   
   Value
 5%   2   ?? *InductionMotorFluxCurrent+5&   &     5'   
   $??1??6&   $??1??63    .   .  $ 0X?5(   &   $
	 Left=?5)   &   
 5(   2   ?? *InductionMotorRatedSlipSpeed+5)   &     5*   
   $??2??6&   $??1??63    .   .  $ 0X?5+   &   $
	 Left=?5,   &   
 5+   2   ?? *&$InductionMotorStatorLeakageReactance+5,   &     5-   
   $??3??6&   $??1??63    .   .  $ 0X?5.   &   $
	 Left=?5/   &   
 5.   2   ?? *%#InductionMotorRotorLeakageReactance+5/   &     50   
   $??5??6&   $??1??63    .   .  $ 0X?51   &   $
	 Left=?52   &   
 51   2   ?? * InductionMotorStatorResistance+52   &   ?? 54   
   $??0??6   $??2??6!     	Amps(RMS)#???  61   6V   ?? 55   
   $??1??6   $??2??6!   RPM66   ?? 56   
   $??2??6   $??2??6!     Ohms#???  61   6Q   ?? 57   
   $??3??6   $??2??6!   #???  61   6Q   ?? 58   
   $??5??6   $??2??6!   #???  61   6Q   5:      5;      