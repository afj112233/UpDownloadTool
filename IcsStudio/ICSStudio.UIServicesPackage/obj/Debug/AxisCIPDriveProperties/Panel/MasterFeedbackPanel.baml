   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5       ICSStudio.UIServicesPackageR   LICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel.MasterFeedbackPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5        ? 	Resources  5
       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5   
   ? $??0??6    5      ?? 5      $
??Auto ?6    ?? 5      $
??Auto ?6    ?? 5      $
??Auto ?6    ?? 5      $
??Auto ?6     ? ColumnDefinitions 5!      ?? 5"      $	??150 ?6#   ?? 5#      $	??180 ?6#   ?? 5$      $
??Auto ?6#   ?? 5%      $
??Auto ?6#   .+?5)      ?? $??0??6   $??0??6%      Device Function:#???   65   6a   ?? 5*      $??1??6   $??0??6%     Feedback Channel:#???  65   6b   ?? 5+      $??2??6   $??0??6%    	 Type:#???  65   6V   ?? 5,      $??3??6   $??0??6%    
 Units:#???  65   6W   ?? 5/      $??0??6   $??1??6%     Master Feedback#???  65   6`   ?? 50      $??1??6   $??1??6%     
Feedback 1#???  65   6[   ?? 51      -   5         ??Name$ CmbFeedbackType??52      $??2??51      $??1??6$   $	??160??64   $
??Left=?6@    ?? SelectedValuePath$ Value??54       ?? DisplayMemberPath$ DisplayName??55      ??53      ?? *Feedback1TypeSource+ ?? SelectedValue 56      ?? *Feedback1Type+?? 57      -   5        $ CmbFeedbackUnit??58      $??3??57      $??1??6$   $	??160??64   $
??Left=?6@   $ Value??5:      $ DisplayName??5;      ??59      ?? *Feedback1UnitSource+ 5<      ?? *Feedback1Unit+?? 5?      $??0??6   $??2??6%   ?? 5A      $??0??6   $??3??6"     Parameters...#???  6X   ??62   ?? *ParametersCommand+6?   5C      ? 5F   
   $??1??6   y?6   ?? *FeedbackSettingVisibility+ 5G      ?? 5H      $
??Auto ?6#   ?? 5I      $??* ?6#   .+?5L      ?? $??0??6   $??0??6%   $
??Bold"?5M   _   ??6   ?? *SelectedItem.DisplayName+	 ?? ElementName$	 CmbFeedbackType??6_   ?? 5N      $??0??6   $??1??6%   5O      ? 5S   
   $??2??6   y?6   ?? *FeedbackSettingVisibility+ 5T      ?? 5U      $
??Auto ?6    ?? 5V      $
??Auto ?6    ?? 5W      $
??Auto ?6    ?? 5X      $
??Auto ?6    ?? 5Y      $
??Auto ?6     5[      ?? 5\      $??20 ?6#   ?? 5]      $	??130 ?6#   ?? 5^      $
??Auto ?6#   ?? 5_      $
??Auto ?6#   .+?5c      ?? $??0??6   $??0??6%   ?? 5f      $??0??6   $??1??6%     Cycle Resolution:#???  65   6b   ?? 5g      $??1??6   $??1??6%     Cycle Interpolation:#???  65   6e   ?? 5h      $??2??6   $??1??6%    	 Effective Resolution:#??? 	 65   6f   ?? 5i      $??3??6   $??1??6%    
 Startup Method:#??? 
 65   6`   ?? 5k      $??4??6   $??1??6%     Length:#???  6]   y?65   ?? *LengthVisibility+6?   ?? 5l      $??4??6   $??1??6%    
 Turns:#???  6\   y?65   ?? *TurnsVisibility+6~   %  Xceed.Wpf.Toolkit.IntegerUpDown  5o      $??0??6!   $??2??6.   $??2q?6>   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
   	AllowSpin
 .     ShowButtonSpinner .     Minimum$ 1X?5q   !      Value 5r   !   ?? *Feedback1CycleResolution+  5s      $??1??6!   $??2??6.   $??2q?6>   
 .   .  $ 1X?5u   !    5v   !   ?? *Feedback1CycleInterpolation+"  Xceed.Wpf.Toolkit.LongUpDown  5x      $??2??6   $??2??6+   $??2q?6;   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .  ,  &Xceed.Wpf.Toolkit.Primitives.InputBase   
IsReadOnly .    Value 5{      ?? *EffectiveResolution+ ?? Mode$ OneWay=??? 5}      -   5        $ CmbStartupMethod??5}   4   $??3??6   $??2??6$   $ Value??5      $ DisplayName??5?      ??5~      ?? *StartupMethodSource+ 5?      ?? *Feedback1StartupMethod+3   -ICSStudio.Gui.Controls.SingleUpDownExtensions  5?      $??4??6*   $??2??67   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .     Minimum$ 0.001X?5?   *      TextAlignment$
 Left=?6:   y?5?   G   ?? *LengthVisibility+   Value 5?   *   ?? *Feedback1Length+  5?      $??4??6!   $??2??6.   
 .   .  $ 1X?5?   !   y?5?   >   ?? *TurnsVisibility+ 5?   !   ?? *Feedback1Turns+?? 5?      $??0??6   $??3??6&    ?? Orientation$ 
Horizontal=?66   .+?5?      ?? $??5,5,0,5q?6     Feedback Cycles/Master#???  6-   ?? 5?      $??0,5,5,5q?6   ??6-   ?? *SelectedItem.DisplayName+$	 CmbFeedbackUnit??5?      ?? 5?      $??1??6   $??3??6%     Feedback Counts per Cycle#???  65   6j   ?? 5?      $??2??6   $??3??6&   $ 
Horizontal=?66   .+?5?      ?? $??5,5,0,5q?6     Feedback Counts per#???  6-   ?? 5?      $??0,5,5,5q?6   ??6-   ?? *SelectedItem.DisplayName+$	 CmbFeedbackUnit??5?      5?      5?      5?      