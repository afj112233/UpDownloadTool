   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761f{Bclr-namespace:ICSStudio.UIServicesPackage.RSTrendXProperties.Panel4ICSStudio.UIServicesPackage.RSTrendXProperties.Panel  5      C    =ICSStudio.UIServicesPackage.RSTrendXProperties.Panel.Sampling   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      TlocalBclr-namespace:ICSStudio.UIServicesPackage.RSTrendXProperties.Panel     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      -   5          ? 	Resources  5
       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %    5      .??5      ?   5   
   $  Xceed.Wpf.Toolkit.SingleUpDown ?? 
TargetType ?? Property ?? Value??%w   '        ?? 5      "  6   .?5      ??  ? ??$	 103??6*   ?? 5       ? ??$ 23??6+   5      5       ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5       $
??Auto ?6   ?? 5!      $
??auto ?6   ?? 5"      $
??Auto ?6   .+?5$   
   ?? $??0??5%      $
??Left=?5&         Sample Period:#???   5'        5(   
   $??0??5)      $??1??5*      $	??120??5+      $??0??5,      $	??120??5-      ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Minimum$ 1X?5/      {?5.      ?? *	AllEnable+,  &Xceed.Wpf.Toolkit.Primitives.InputBase	   Text	 50      ?? *SamplePeriod+
 ?? UpdateSourceTrigger$
 PropertyChanged=? ?? Mode$ TwoWay=??? 51   
   $??0??52      $??2??53      $	??110??54      $??21??55       ?? DisplayMemberPath$ DisplayName??56       ?? SelectedValuePath$ Value??5:      {?57      ?? *	AllEnable+??58      ?? *TimeTypeList+ ?? SelectedValue 59      ?? *SelectedSamplePeriod+5:      ?? 5<   
   $??1??5=      $??0??5>      $
??Left=?5?        NumberOfCaptures#???  5@        5A   
   $??1??5B      $??1??5C      $	??120??5D      $??0??5E      $	??120??5F      {?.  $	 1??5H      ?? 5J   
   $??2??5K      $??0??5L      $??3??5M      .??5N      ?  5P      ?? 5Q      ?? 5R      ?? 5S       5U      ?? 5V      $
??auto ?6'   ?? 5W      $
??auto ?6'   ?? 5X      $
??Auto ?6'   .+?5Z      	? -   5         	?Name$ RadioButton1??5[      $
??Left=?5\        Samples:#???  5]       d? 	IsChecked 5^      ?? *RadioChecked1+$
 PropertyChanged=?$ TwoWay=?{?5_      ?? *	AllEnable+  5`      $??0??5a      $??1??5b      $	??120??5c      $??0??5d      $	??120??5e      $??51,0,0,0q?5f      $ 1X?5g      	 5h      ?? *	Samples+$
 PropertyChanged=?$ TwoWay=???5i      ?? 5j      "  6     ?? Triggers 5k      ?? 5l   "    ?? Binding 6.   ??  ?? ElementName$ RadioButton1?? ?? Path$ 	IsChecked? ?? Value	 False6k   .a?5m   &   ??  ? {? .  6B   5n   #   ?? 5o   "    6.   ?? $ RadioButton1??$ 	IsChecked? True6k   .a?5p   &   ??  ? {? . 6B   5q   #   ?? 5r   "    6.   ?? $ RadioButton1??$ 	IsEnabled?	 False6k   .a?5s   &   ??  ? {? .  6B   5t   #   5v      5x      	? 5z      -   5        $ RadioButton2??5{      $??1??5|      $??0??5}      $
??Left=?5~        Time Period:#???  5       5?      ?? *RadioChecked2+$
 PropertyChanged=?$ TwoWay=?{?5?      ?? *	AllEnable+  5?      $??1??5?      $??1??5?      $	??120??5?      $??0??5?      $	??120??5?      $??51,0,0,0q?5?      $ 1X?5?      	 5?      ?? *
TimePeriod+$
 PropertyChanged=?$ TwoWay=???5?      ?? 5?      "  6     5?      ?? 5?   "    6.   ?? $ RadioButton2??$ 	IsChecked?	 False6k   .a?5?   &   ??  ? {? .  6B   5?   #   ?? 5?   "    6.   ?? $ RadioButton2??$ 	IsChecked? True6k   .a?5?   &   ??  ? {? . 6B   5?   #   ?? 5?   "    6.   ?? $ RadioButton2??$ 	IsEnabled?	 False6k   .a?5?   &   ??  ? {? .  6B   5?   #   5?      5?      ?? 5?      $??1??5?      $??2??5?      $	??110??5?      $??21??5?      $??25,0,0,0q?5?      $ DisplayName??5?      $ Value??5?      ??5?      ?? *TimePeriodTypeList+ 5?      ?? *SelectedTimePeriod+??5?      ?? 5?      " ??6     5?      ?? 5?   "    6.   ?? $ RadioButton2??$ 	IsChecked?	 False6k   .a?5?   &   ??  ? {? .  6B   5?   #   ?? 5?   "    6.   ?? $ RadioButton2??$ 	IsChecked? True6k   .a?5?   &   ??  ? {? . 6B   5?   #   ?? 5?   "    6.   ?? $ RadioButton2??$ 	IsEnabled?	 False6k   .a?5?   &   ??  ? {? .  6B   5?   #   5?      5?      ?? 5?      $??2??5?      $??0??5?      $??3??5?      $
??Left=?5?       ?? Orientation$ 
Horizontal=?5?      .+?5?      	?   	No Limit:#???  5?      {?.   5?      ?? *RadioChecked3+$
 PropertyChanged=?$ TwoWay=?5?      ?? 5?        Sampling Stops When#???  6    5?      5?      5?      ? 5?   
   $??3??5?      $??0??5?      $??2??5?       5?      ?? 5?      ?? 5?       5?      ?? 5?      $
??Auto ?6#   ?? 5?      .+?5?      ??  ! Estimated Trend Capture Size:#???  6   ?? 5?      $??0??5?      $??1??5?      ??5?      ?? *Size+?? 5?      $??1??5?      $??0??5?        Estimated Trend Log Size:#???  5?      ?? 5?      $??1??5?      $??1??5?      ??5?      ?? *Size+5?      5?      5?      