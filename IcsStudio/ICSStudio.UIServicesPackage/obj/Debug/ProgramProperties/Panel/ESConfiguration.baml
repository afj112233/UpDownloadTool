   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fyAclr-namespace:ICSStudio.UIServicesPackage.ProgramProperties.Panel3ICSStudio.UIServicesPackage.ProgramProperties.Panel  5      Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05Z=clr-namespace:ICSStudio.Gui.Converters;assembly=ICSStudio.GuiICSStudio.Gui.Converters 5      e aICSStudio.SimpleServices, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=72e63300b7cc592asOclr-namespace:ICSStudio.SimpleServices.Common;assembly=ICSStudio.SimpleServicesICSStudio.SimpleServices.Common 5	      Z VICSStudio.Cip, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=dfd71c81d447f80dT:clr-namespace:ICSStudio.Cip.Objects;assembly=ICSStudio.CipICSStudio.Cip.Objects 5
      I    CICSStudio.UIServicesPackage.ProgramProperties.Panel.ESConfiguration   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      SlocalAclr-namespace:ICSStudio.UIServicesPackage.ProgramProperties.Panel     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      N
converters=clr-namespace:ICSStudio.Gui.Converters;assembly=ICSStudio.Gui  5      [enumsOclr-namespace:ICSStudio.SimpleServices.Common;assembly=ICSStudio.SimpleServices  5	      Hobjects:clr-namespace:ICSStudio.Cip.Objects;assembly=ICSStudio.Cip  5
        ? 	Resources  5      3  -ICSStudio.Gui.Converters.EnumBooleanConverter   ValuesToUseOnStartConverter ?? 
TargetType ?? Property ?? Value ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %R  &	        '??      '?? ?     '?? ?       5      6.   ?? 5      " ??6   .?5      ??  ? ??$ 23??6+   68   ?? 5       ? ??$
 Left=?68   6G   5      ?? 5      " ??6   .?5      ??  ? ??$ 0,8,0,0q?6+   6=   5      ?? 5      " ??6   .?5      ??  ? ??$ 30 ?6+   68   5      5      .??5!      ?  ? RowDefinitions 5"   
   ?? 5#      6#   ?? 5$      6"   ?? 5%      6"   ?? 5&      6"   ?? 5'      6"   ?? 5(      6"   ?? 5)      6"   ?? 5*      6"   ?? 5+      6"   ?? 5,      6"   ?? 5-      6"   .+?5/   
   ?? $??0??50       d? 	IsChecked 51      ?? *	Inhibited+?? 52   
   $??0??53        Inhibit equipment sequence#???  54      $??20,0,0,0q?55      ?? 56   
   $??1??57       58      ?? *RetainSequenceIDOnReset+?? 59   
   $??1??5:       . *Retain sequence ID when resetting sequence#???  5;      $??20,0,0,0q?5<      ?? 5=   
   $??2??5>      {?.   5?      ?? *GenerateSequenceEvents+5@      ?? 5A   
   $??2??5B        Generate sequence events#???  5C      $??20,0,0,0q?5D      ?? 5E   
   $??3??5F        Unit ID:#???  5G      $  Xceed.Wpf.Toolkit.SingleUpDown  5H   
   $??3??5I      $??Center=?5J      $	??120??5K      $
??Left=?5L      $??0,-2,0,0q?5M      ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin . 	   ShowButtonSpinner	 . $??60,0,0,0q?5P      
   Minimum$
 1X?5R         Maximum$ 
2147483647X?5S      ?  ?Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	Increment$ 1X?5T      ,  &Xceed.Wpf.Toolkit.Primitives.InputBase   Text 5Q      ?? *UnitID+ ?? UpdateSourceTrigger$ PropertyChanged=?5T      ?? 5U   
   $??4??5V       I EFor parameters and step tags using initial value as the value source:#???  5W      ?? 5X   
   $??5??5Y        When starting sequence:#???  5Z      $??20,0,0,0q?5[      	? 5\   
   $??6??5]      $??40,8,0,0q?5^       	? 	GroupName$ z1??5_       5`      ?? *ValuesToUseOnStart+ ?? 	Converter# [  <  6ICSStudio.SimpleServices.Common.ValuesToUseOnStartType   UseInitialValues ?? ConverterParameter# Z ?? 5a   
   $??6??5b        Use initial value of tag#???  5c      $??60,0,0,0q?5d      	? 5e   
   $??7??5f      $??40,8,0,0q?5g      $ z1??5h       5i      ?? *ValuesToUseOnStart+# [     UseCurrentValues# Z ?? 5j   
   $??7??5k        Use current value of tag#???  5l      $??60,0,0,0q?5m      ?? 5o   
   $??8??5p       	 When resetting sequence:#??? 	 5q      $??20,0,0,0q?5r      	? 5s   
   $??9??5t      $??40,8,0,0q?5u      $ z2??5v       5w      ?? *ValuesToUseOnReset+# [  <  6ICSStudio.SimpleServices.Common.ValuesToUseOnResetType   RestoreItsInitialValues# Z ?? 5x   
   $??9??5y       $
  Restore tag to its initial value#??? 
 5z      $??60,0,0,0q?5{      	? 5|   
   $??10??5}      $??40,8,0,0q?5~      $ z2??5       5?      ?? *ValuesToUseOnReset+# [     MaintainCurrentValues# Z ?? 5?   
   $??10??5?       ! Maintain current value of tag#???  5?      $??60,0,0,0q?5?      5?      5?      