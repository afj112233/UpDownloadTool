   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5       ICSStudio.UIServicesPackageN   HICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel.CompliancePanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5        ? 	Resources  5	       ?? MergedDictionaries ?? Source?? 5
   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ? {?6   ?? *IsComplianceEnabled+ ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5   
   ? $??0??6   $??0q?6    ? ColumnDefinitions 5      ?? 5      $	??220 ?6#   ?? 5      $	??100 ?6#   ?? 5      $	??100 ?6#   ?? 5      $	??100 ?6#   .+?5"      ?? $??0??6   $??0??6%    %  !Torque Low Pass Filter Bandwidth:#???   65   6r   3   -ICSStudio.Gui.Controls.SingleUpDownExtensions  5#      $??0??6*   $??1??67   $??Center=?6G   X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .     Minimum$ 0X?5%   *      Maximum$ 10000X?66   ,  &Xceed.Wpf.Toolkit.Primitives.InputBase	   TextAlignment$
	 Left=?5&   *   
   Value
 5'   *   ?? *TorqueLowPassFilterBandwidth+?? 5(      $??0??6   $??2??6%    	 Hertz#???  65   6V   ?? 5)      $??3??6     Parameters...#???  6K   ??6%   ?? *ParametersCommand+6w   5*      ? 5,   
   $??1??6   $??0q?6    5-      ?? 5.      $
??Auto ?6#   ?? 5/      $??* ?6#   .+?52      ?? $??0??6   $
??Bold"?6(     Torque Notch Filters#???  6:   6j   ?? 53      $??1??6   54      ? 56   
   $??2??6   $??0q?6    57      ?? 58      ?? 59      $??20 ?6    ?? 5:      $??20 ?6     5<      ?? 5=      $??20 ?6#   ?? 5>      $	??200 ?6#   ?? 5?      $	??100 ?6#   ?? 5@      $
??Auto ?6#   .+?5C      ?? $??0??6   $??1??6%    " Torque Notch Filter Frequency:#???  65   6o     5D      $??0??6*   $??2??67   $??Center=?6G    .   .  $ 0X?5F   *   $ 10000X?66   $
	 Left=?5G   *   
 5H   *   ?? *TorqueNotchFilterFrequency+?? 5I      $??0??6   $??3??6%   #???  65   6V   5[      ? 5]   
   $??3??6   $??0q?6    5^      ?? 5_      $
??Auto ?6#   ?? 5`      $??* ?6#   .+?5c      ?? $??0??6   $
??Bold"?6(     Adaptive Tuning#???  6:   6e   ?? 5d      $??1??6   5e      ? 5g   
   $??4??6   $??0q?6    5h      ?? 5i      $
??Auto ?6    ?? 5j      $
??Auto ?6    ?? 5k      $
??Auto ?6    ?? 5l      $
??Auto ?6     5n      ?? 5o      $??20 ?6#   ?? 5p      $
??Auto ?6#   ?? 5q      $	??280 ?6#   ?? 5r      $
??Auto ?6#   .+?5u      ?? $??0??6   $??1??6!    " Adaptive Tuning Configuration:#???  61   6n   ?? 5v      $??1??6   $??1??6!    - )Torque Notch Filter High Frequency Limit:#???  61   5w      ?? 5x      $??2??6   $??1??6!    , (Torque Notch Filter Low Frequency Limit:#???  61   5y      ?? 5z      $??3??6   $??1??6!    )	 %Torque Notch Filter Tuning Threshold:#??? 	 61   5{      ?? 5}      -   5         ??Name$$ CmbAdaptiveTuningConfiguration??5}   4   $??0??6   $??2??6$   $	??0,3q?6\    ?? SelectedValuePath$ Value??5       ?? DisplayMemberPath$ DisplayName??5?      ??5~      ?? *#!AdaptiveTuningConfigurationSource+ ?? SelectedValue 5?      ?? *AdaptiveTuningConfiguration+  5?      $??1??6*   $??2??67   $??Center=?6G    .   .  $ 20X?5?   *   $
 4000X?67   $
	 Left=?5?   *   
 5?   *   ?? *%#TorqueNotchFilterHighFrequencyLimit+  5?      $??2??6*   $??2??67   $??Center=?6G    .   .  $ 1X?5?   *   $
 2000X?66   $
	 Left=?5?   *   
 5?   *   ?? *$"TorqueNotchFilterLowFrequencyLimit+  5?      $??3??6*   $??2??67   $??Center=?6G    .   .  $ 0X?5?   *   $	 100X?66   $
	 Left=?5?   *   
 5?   *   ?? *" TorqueNotchFilterTuningThreshold+?? 5?      $??1??6   $??3??6!   #???  61   6U   ?? 5?      $??2??6   $??3??6!   #???  61   6U   ?? 5?      $??3??6   $??3??6!    
 % Motor Rated#??? 
 61   6]   5?      5?      5?      