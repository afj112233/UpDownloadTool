   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05Z=clr-namespace:ICSStudio.Gui.Converters;assembly=ICSStudio.GuiICSStudio.Gui.Converters  5      Z VICSStudio.Cip, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=dfd71c81d447f80dT:clr-namespace:ICSStudio.Cip.Objects;assembly=ICSStudio.CipICSStudio.Cip.Objects 5       ICSStudio.DeviceProperties=   7ICSStudio.DeviceProperties.ServoDrives.Panel.PowerPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      T
converters=clr-namespace:ICSStudio.Gui.Converters;assembly=ICSStudio.Gui     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      Nobjects:clr-namespace:ICSStudio.Cip.Objects;assembly=ICSStudio.Cip     5      $	??660??5	      $
??Left=?5
      {?5      ?? *Enable+  ? 	Resources  5      3   -ICSStudio.Gui.Converters.EnumBooleanConverter   EnumBooleanConverter??%   &	          5   
   6*   5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5       $	??180 ?6   ?? 5!      $	??100 ?6   ?? 5"      $
??Auto ?6   .+?5%   
   ?? $??0??5&      $??0??5'        PowerStructure#???  5(      ?? 5)   
   $??0??5*      $??1??5+      ??5,      ?? *PowerStructure+ ?? FallbackValue$ 2198-H015-ERS???? 5-   
   $??0??5.      $??3??5/        Advanced#???  51      ??50      ?? *AdvancedCommand+51      ?? 53   
   $??1??54      $??1??55      $??2??56      $??WordEllipsis=?58      ??57      ?? *Description+$ ......?? ? ToolTip 59      ?? *Description+?? 5;   
   $??2??5<      $??0??5=        Voltage#???  5>      y?5?      ?? *#!ConverterACInputVoltageVisibility+?? 5@   
   $??2??5A      $??1??5B       ?? DisplayMemberPath$ DisplayName??5C       ?? SelectedValuePath$ Value??5F      ??5D      ?? *VoltageSource+ ?? SelectedValue 5E      ?? *	Voltage+y?5G      ?? *#!ConverterACInputVoltageVisibility+?? 5I   
   $??3??5J      $??0??5K        ACInputPhasing#???  5L      y?5M      ?? *#!ConverterACInputPhasingVisibility+?? 5N   
   $??3??5O      $??1??5P      $ DisplayName??5Q      $ Value??5T      ??5R      ?? *ACInputPhasingSource+ 5S      ?? *ACInputPhasing+y?5U      ?? *#!ConverterACInputPhasingVisibility+?? 5W   
   $??4??5X      $??0??5Y        BusConfiguration#???  5Z      ?? 5[   
   $??4??5\      $??1??5]      $ DisplayName??5^      $ Value??5a      ??5_      ?? *BusConfigurationSource+ 5`      ?? *BusConfiguration+5a      ?? 5c   
   $??5??5d      $??0??5e        BusSharingGroup#???  5f      ?? 5g   
   $??5??5h      $??1??5i      $ DisplayName??5j      $ Value??5n      {?5k      ?? *BusSharingGroupEnabled+??5l      ?? *BusSharingGroupSource+ 5m      ?? *BusSharingGroup+5n      ?? 5p   
   $??6??5q      $??0??5r        BusRegulatorAction#???  5s      ?? 5t   
   $??6??5u      $??1??5v      $ DisplayName??5w      $ Value??5z      ??5x      ?? *BusRegulatorActionSource+ 5y      ?? *BusRegulatorAction+5z      ?? 5|   
   $??7??5}      $??0??5~        ShuntRegulatorResistorType#???  5      ?? 5?   
   $??7??5?      $??1??5?       ?? Orientation$ 
Horizontal=?5?      {?5?      ?? *#!ShuntRegulatorResistorTypeEnabled+.+?5?      	?  	 External#??? 	 6   	 d? 	IsChecked	 6?   ?? *ShuntRegulatorResistorType+
 ?? 	Converter#
 [  6  0ICSStudio.Cip.Objects.ShuntRegulatorResistorType   External ?? ConverterParameter# Z 	? 5?       
 Internal#??? 
 6   	 6?   ?? *ShuntRegulatorResistorType+#
 [     Internal# Z 5?      ?? 5?   
   $??8??5?      $??0??5?        ExternalShunt#???  5?      ?? 5?   
   $??8??5?      $??1??5?      $ DisplayName??5?      $ Value??5?      {?5?      ?? *!ExternalShuntRegulatorIDEnabled+??5?      ?? * ExternalShuntRegulatorIDSource+ 5?      ?? *ExternalShuntRegulatorID+5?      5?      5?      