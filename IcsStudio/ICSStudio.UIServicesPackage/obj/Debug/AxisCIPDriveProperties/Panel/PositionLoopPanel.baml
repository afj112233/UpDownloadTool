   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5       ICSStudio.UIServicesPackageP   JICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel.PositionLoopPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5        ? 	Resources  5	       ?? MergedDictionaries ?? Source3   -ICSStudio.Gui.Controls.SingleUpDownExtensions ?? 
TargetType ?? Property ?? Value?? 5
   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %X   '        ?? 5      "  6   .?5      ??  ? ??$	 0,4q?6+   69   5      5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5   
   ? $??0??6   $??0q?6    ? ColumnDefinitions 5      ?? 5      $??50 ?6#   ?? 5      $	??350 ?6#   ?? 5       $	??100 ?6#   .+?5"      ?? $??0??6   $
??Bold"?6(    	  Gains#???   6:   6[   ?? 5#      $??1??6   ?? 5$      $??2??6     Parameters...#???  6K   ??6%   ?? *ParametersCommand+6w   5%      ? 5'   
   $??1??6   $??0q?6    5(      ?? 5)      $
??Auto ?6    ?? 5*      $
??Auto ?6    ?? 5+      $
??Auto ?6    ?? 5,      $
??Auto ?6     5.      ?? 5/      $??20 ?6#   ?? 50      $	??160 ?6#   ?? 51      $	??120 ?6#   ?? 52      $
??Auto ?6#   .+?55      ?? $??0??6   $??1??6%   $??Center=?65     
Bandwidth:#???  6P   6v   ?? 56      $??1??6   $??1??6%   $??Center=?65     Integrator Bandwidth:#???  6P   6?   ?? 57      $??2??6   $??1??6%   $??Center=?65     Integrator Hold:#???  6P   6|   ?? 58      $??3??6   $??1??6%   $??Center=?65     Velocity Feedforward:#???  6P   6?     5:      $??0??6*   $??2??67   $??Center=?6G   X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .  	   ShowButtonSpinner	 .  
   Minimum$
 0X?5<   *   ,  &Xceed.Wpf.Toolkit.Primitives.InputBase   TextAlignment$
 Left=?66      Value 5=   *   ?? *PositionLoopBandwidth+{?5>   *   ?? *IsPositionLoopEnabled+  5?      $??1??6*   $??2??67   $??Center=?6G    .  	 .  $
 0X?5A   *   $
 Left=?66    5B   *   ?? *PositionIntegratorBandwidth+{?5C   *   ?? *IsPositionLoopEnabled+?? 5E      -   5         ??Name$ CmbIntegratorHold??5E   4   $??2??6   $??2??6$   $	??0,4q?6O    ?? SelectedValuePath$ Value??5G       ?? DisplayMemberPath$ DisplayName??5H      ??5F      ?? *IntegratorHoldSource+ ?? SelectedValue 5I      ?? *IntegratorHold+{?5J      ?? *IsPositionLoopEnabled+  5L      $??3??6*   $??2??67   $??Center=?6G    .  	 .  $
 0X?5N   *   $
 Left=?66    5O   *   ?? *VelocityFeedforwardGain+{?5P   *   ?? *IsPositionLoopEnabled+?? 5R      $??0??6   $??3??6%    	 Hertz#???  65   6V   ?? 5S      $??1??6   $??3??6%   #???  65   6V   ?? 5T      $??3??6   $??3??6%   %68   5V      ? 5Y   
   $??2??6   $??0q?6    5Z      ?? 5[      $??50 ?6#   ?? 5\      $	??350 ?6#   .+?5_      ?? $??0??6   $
??Bold"?6(    
 Limits#???  6:   6\   ?? 5`      $??1??6   5b      ? 5d   
   $??3??6   $??0q?6    5e      ?? 5f      $
??Auto ?6    ?? 5g      $
??Auto ?6     5i      ?? 5j      $??20 ?6#   ?? 5k      $	??160 ?6#   ?? 5l      $	??120 ?6#   ?? 5m      $
??Auto ?6#   .+?5p      ?? $??0??6   $??1??6%     Error Tolerance:#???  65   6a   ?? 5q      $??1??6   $??1??6%    	 Lock Tolerance:#??? 	 65   6a     5s      $??0??6*   $??2??67   $??Center=?6G    .  	 .  $
 0X?5u   *   $
 Left=?66    5v   *   ?? *PositionErrorTolerance+{?5w   *   ?? *IsPositionLoopEnabled+  5x      $??1??6*   $??2??67   $??Center=?6G    .  	 .  $
 0X?5z   *   $
 Left=?66    5{   *   ?? *PositionLockTolerance+{?5|   *   ?? *IsPositionLoopEnabled+?? 5      $??0??6   $??3??6%   ??65   ?? *PositionUnits+?? 5?      $??1??6   $??3??6%   ??65   ?? *PositionUnits+5?      5?      5?      