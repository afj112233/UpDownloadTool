   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui  5       ICSStudio.DevicePropertiesL   FICSStudio.DeviceProperties.AdvancedUserLimits.AdvancedUserLimitsDialog   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      $	??400??5
      $	??400??5        ? 
ResizeMode$  NoResize=?5       ? ShowInTaskbar .   ? WindowStartupLocation$ CenterOwner=?5       ? Title 5	      ?? *Title+    ICSStudio.Gui.DialogCloser   DialogResult 5      ?? *DialogResult+ ? 	Resources 5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $??* ?6   ?? 5       $
??Auto ?6   ?? 5!      $
??Auto ?6   	 ? ColumnDefinitions	 5#   
   ?? 5$      $	??230 ?6   ?? 5%      $	??120 ?6   ?? 5&      $
??Auto ?6   .+?5)   
   ?? $??0??5*      $??0??5+       !  ConverterThermalOverloadLimit#???   5,      ?? 5-   
   $??1??5.      $??0??5/       $  BusRegulatorThermalOverloadLimit#???  50      ?? 51   
   $??2??52      $??0??53        BusUnderVoltageLimit#???  54      $  Xceed.Wpf.Toolkit.SingleUpDown  56   
   $??0??57      $??1??58      $??Center=?59      ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
   	AllowSpin
 .  ?  ?Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   FormatString$ ######0.0######??5;         Minimum$ 0X?5<         ShowButtonSpinner .     Value 5>      ?? *#!ConverterThermalOverloadUserLimit+ ?? Delay$	 100??  5@   
   $??1??5A      $??1??5B      $??Center=?5C      
 .  $ ######0.0######??5E      $ 0X?5F       .   5H      ?? *&$BusRegulatorThermalOverloadUserLimit+$	 100??  5J   
   $??2??5K      $??1??5L      $??Center=?5M      
 .  $ ######0.0######??5O         Maximum$	 100X?5P      $ 0X?5Q       .   5S      ?? *BusUndervoltageUserLimit+$	 100???? 5U   
   $??0??5V      $??2??5W      $??%??5X      ?? 5Y   
   $??1??5Z      $??2??5[      $??%??5\      ?? 5]   
   $??2??5^      $??2??5_      $??%??5`      ?? 5b   
   $??4??5c      $??0??5d      $??3??5e      ?? 5g   
   $??5??5h      $??0??5i      $??3??5j      $??Center=?5k      $??Center=?5l       ?? Orientation$ 
Horizontal=?5m      .+?5n      ??   OK#???  5p       ?? 	IsDefault . ??5o      ?? *	OkCommand+5q      ?? 5r       
 Cancel#???  5t       ?? IsCancel . ??5s      ?? *CancelCommand+5u      ?? 5v        Help#???  6   5w      5y      5z      