   M S B A M L   `   `   `  ???? Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls   ICSStudio.UIServicesPackageV   PICSStudio.UIServicesPackage.AxisCIPDriveProperties.Controls.RotaryInductionModel   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui       ? 	Resources  3   -ICSStudio.Gui.Controls.SingleUpDownExtensions ?? 
TargetType ?? Property ?? Value??%,   '        ?? "  .???  ? ??$ 2q?.???  ? RowDefinitions ?? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ? ? ColumnDefinitions ?? $	??240 ??? $	??100 ??? $	??180 ?.+??? $??0??$??0??   Rated Flux Current:#???   ?? $??1??$??0??  Rated Slip Speed:#???  ?? $??2??$??0??  Stator Leakage(X1):#???  ?? $??3??$??0??  Rotor Leakage(X2):#???  ?? $??4??$??0???? $??5??$??0??  Stator Resistance(R1):#???    $??0??$??1??X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .     Minimum$ 0X?,  &Xceed.Wpf.Toolkit.Primitives.InputBase	   TextAlignment$
	 Left=?
   Value
 ?? *InductionMotorFluxCurrent+  $??1??$??1?? .   .  $ 0X?$
	 Left=?
 ?? *InductionMotorRatedSlipSpeed+  $??2??$??1?? .   .  $ 0X?$
	 Left=?
 ?? *&$InductionMotorStatorLeakageReactance+  $??3??$??1?? .   .  $ 0X?$
	 Left=?
 ?? *%#InductionMotorRotorLeakageReactance+  $??5??$??1?? .   .  $ 0X?$
	 Left=?
 ?? * InductionMotorStatorResistance+?? $??0??$??2??  	Amps(RMS)#???  ?? $??1??$??2??RPM?? $??2??$??2??  Ohms#???  ?? $??3??$??2??#???  ?? $??5??$??2??#???  