   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5       ICSStudio.UIServicesPackageJ   DICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel.HomingPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5        ? 	Resources  5	       ?? MergedDictionaries ?? Source?? 5
   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ? {?6   ?? *IsHomingEnabled+ ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5   
   ? $??0??6   $??0q?6    5      ?? 5      $
??Auto ?6    ?? 5      $
??Auto ?6    ?? 5      $
??Auto ?6    ?? 5      $
??Auto ?6    ?? 5      $
??Auto ?6    ?? 5      $
??Auto ?6     ? ColumnDefinitions 5!      ?? 5"      $	??100 ?6#   ?? 5#      $	??150 ?6#   ?? 5$      $	??200 ?6#   ?? 5%      $
??Auto ?6#   .+?5(      ?? $??0??6   $??0??6%      CIPTag Homing Mode:#???   65   6d   ?? 5)      -   5         ??Name$ CmbHomeMode??5)   4   $??0??6   $??1??6$    ?? SelectedValuePath$ Value??5+       ?? DisplayMemberPath$ DisplayName??5,      ??5*      ?? *HomeModeSource+ ?? SelectedValue 5-      ?? *
HomeMode+?? 5/      $??1??6   $??0??6%     	Position:#???  65   6Z   3   -ICSStudio.Gui.Controls.SingleUpDownExtensions  50      $??1??6*   $??1??67   $	??2,0q?6G   $??Center=?6T   X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f4,  &Xceed.Wpf.Toolkit.Primitives.InputBase	   TextAlignment$
	 Left=?51   *   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
   	AllowSpin
 .     ShowButtonSpinner .     Value 53   *   ?? *HomePosition+?? 54      $??1??6   $??2??6!   ??61   ?? *PositionUnits+?? 56      $??1??6   $??3??6"   $
??Left=?57      $??Center=?58        Test Marker...#???  59   ;   ??6   ?? *TestMarkerCommand+5:      ?? 5<      $??-4,0,0,0q?6   $??2??6&   $??0??63     CIPTag Homing Offset:#???  6g   {?6C   ?? *OffsetEnabled+6?     5=      $??2??6*   $??1??67   $	??2,0q?6G   $??Center=?6T   $
	 Left=?5>   *   
 .   .   5@   *   ?? *
HomeOffset+{?5A   *   ?? *OffsetEnabled+?? 5B      $??2??6   $??2??6!   ??5C      ?? *PositionUnits+{?5D      ?? *OffsetEnabled+?? 5F      $??3??6   $??0??6%     CIPTag Homing Sequence:#???  65   6h   ?? 5G      -   5        $ CmbHomeSequence??5G   4   $??3??6   $??1??6$   $ Value??5I      $ DisplayName??5J      ??5H      ?? *HomeSequenceSource+ 5K      ?? *HomeSequence+]? 5M      $??4??6   $??0??6%   $??3??65   {?5N      ?? *LimitSwitchEnabled+.+?5O      ??   Limit Switch-Normally:#???  6   6M   	? 5P      $??Center=?6   $??3q?69     CIPTag Homing Open#???  5R       d? 	IsChecked 5Q      ?? *HomeSwitchNormallyOpen+5S      	? 5T      $??Center=?6   $??3q?69    
 Closed#???  5V       5U      ?? *HomeSwitchNormallyClosed+5W      5X      5[      ? 5]   
   $??1??6   $??0q?6   y?5^      ?? *#!ActiveHomeSequenceGroupVisibility+ 5_      ?? 5`      $
??Auto ?6#   ?? 5a      $??* ?6#   .+?5c      ?? $??0??6   $
??Bold"?6(     Active Home Sequence Group#???  6:   6p   ?? 5d      $??1??6   5e      ? 5g   
   $??2??6   $??0q?6   y?5h      ?? *#!ActiveHomeSequenceGroupVisibility+{?5i      ?? * ActiveHomeSequenceGroupEnabled+ 5j      ?? 5k      $
??Auto ?6    ?? 5l      $
??Auto ?6    ?? 5m      $
??Auto ?6     5o      ?? 5p      $??20 ?6#   ?? 5q      $	??100 ?6#   ?? 5r      $	??170 ?6#   ?? 5s      $
??Auto ?6#   .+?5v      ?? $??0??6   $??1??6!    "	 CIPTag AutotuneLeft Direction:#??? 	 61   6n   ?? 5w      $??1??6   $??1??6!    
 CIPTag Homing Speed:#??? 
 61   6d   ?? 5x      $??2??6   $??1??6!     Return Speed:#???  61   6]   ?? 5z      -   5        $ CmbHomeDirection??5z   4   $??0??6   $??2??6$   $ Value??5|      $ DisplayName??5}      ??5{      ?? *HomeDirectionSource+ 5~      ?? *HomeDirection+  5      $??1??6*   $??2??67   $	??2,0q?6G   $??Center=?6T   
 .   .     Minimum$ 0X?5?   *   $
	 Left=?66    5?   *   ?? *	HomeSpeed+  5?      $??2??6*   $??2??67   $??Center=?6G   $	??2,0q?6b   
 .   .  $ 0X?5?   *   $
	 Left=?66    5?   *   ?? *HomeReturnSpeed+?? 5?      $??1??6   $??3??6!    ?? ContentStringFormat$ {0}/s??6S   ??61   ?? *PositionUnits+6S   ?? 5?      $??2??6   $??3??6!   $ {0}/s??6S   ??61   ?? *PositionUnits+6S   5?      5?      5?      