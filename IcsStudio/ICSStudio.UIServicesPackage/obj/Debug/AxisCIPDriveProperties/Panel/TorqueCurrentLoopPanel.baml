   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5       ICSStudio.UIServicesPackageU   OICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel.TorqueCurrentLoopPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5        ? 	Resources  5	       ?? MergedDictionaries ?? Source?? 5
   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5   
   ? $??0??6   $??0q?6    ? ColumnDefinitions 5      ?? 5      $??50 ?6#   ?? 5      $	??390 ?6#   ?? 5      $	??100 ?6#   .+?5      ?? $??0??6   $
??Bold"?6(    	  Gains#???   6a   y?6:   ?? *GainsVisibility+6?   ?? 5       $??1??6   y?6(   ?? *GainsVisibility+?? 5!      $??2??6     Parameters...#???  6K   ??6%   ?? *ParametersCommand+6w   5"      ? 5$   
   $??1??6   $??0q?6   y?5%      ?? *GainsVisibility+ 5&      ?? 5'      $??20 ?6#   ?? 5(      $	??180 ?6#   ?? 5)      $	??140 ?6#   ?? 5*      $
??Auto ?6#   .+?5-      ?? $??0??6   $??1??6%     
Bandwidth:#???  65   6[   3   -ICSStudio.Gui.Controls.SingleUpDownExtensions  5/      $??0??6*   $??2??67   $??Center=?6G   X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .     Minimum$ 0X?51   *   ,  &Xceed.Wpf.Toolkit.Primitives.InputBase   TextAlignment$
 Left=?66   	   Value	 52   *   ?? *TorqueLoopBandwidth+{?53   *   ?? *IsTorqueCurrentLoopEnabled+?? 56      $??0??6   $??3??6%    	 Hertz#???  65   6V   58      ? 5;   
   $??2??6   $??0q?6    5<      ?? 5=      $??50 ?6#   ?? 5>      $	??390 ?6#   .+?5A      ?? $??0??6   $
??Bold"?6(    
 Limits#???  6:   6\   ?? 5B      $??1??6   5D      ? 5F   
   $??3??6   $??0q?6    5G      ?? 5H      $
??Auto ?6    ?? 5I      $
??Auto ?6     5K      ?? 5L      $??20 ?6#   ?? 5M      $	??180 ?6#   ?? 5N      $	??140 ?6#   ?? 5O      $
??Auto ?6#   .+?5R      ?? $??0??6   $??1??6%     Peak Torque Limit Positive:#???  65   6l   ?? 5S      $??1??6   $??1??6%     Peak Torque Limit Negative:#???  65   6l     5U      $??0??6*   $??2??67   $??Center=?6G   $	??0,3q?6b    .   .  $ 0X?5W   *   
   Maximum$

 1000X?66   $
 Left=?6E   	 5X   *   ?? *TorqueLimitPositive+{?5Y   *   ?? *IsTorqueCurrentLoopEnabled+  5Z      $??1??6*   $??2??67   $??Center=?6G   $	??0,3q?6b    .   .  $ -1000X?5\   *   $
 0X?6:   $
 Left=?6F   	 5]   *   ?? *TorqueLimitNegative+{?5^   *   ?? *IsTorqueCurrentLoopEnabled+?? 5`      $??0??6   $??3??6%     % Rated#???  65   6X   ?? 5a      $??1??6   $??3??6%   #???  65   6X   5c      5e      5f      