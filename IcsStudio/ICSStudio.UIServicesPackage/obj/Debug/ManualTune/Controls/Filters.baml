   M S B A M L   `   `   `  ????  ICSStudio.UIServicesPackage=    7ICSStudio.UIServicesPackage.ManualTune.Controls.Filters   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5        ? 	Resources  5	         	LeftImage ?? 	UriSource$  Xceed.Wpf.Toolkit.SingleUpDown ?? 
TargetType?? 5
   
   %?   &	        '  i     ?? 5      $Q Kpack://application:,,,/ICSStudio.Gui;component/Resources/image/left_6_5.png??5      ?? 5      "  6   .?5      -   5        5      5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $	??100 ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $	??100 ?6   ?? 5      $
??Auto ?6   .+?5"   
   ?? $??0??6   $??0??6    - )Filters Torque Low Pass Filter Bandwidth:#???  6-   6u   ?? 5#   
   $??1??6   $??0??6    * &Filters Torque Notch Filter Frequency:#???  6-   6r   ?? 5$   
   $??2??6   $??0??6     Torque Lag Filter Gain:#???  6-   6c   ?? 5%   
   $??3??6   $??0??6      Torque Lag Filter Bandwidth:#???  6-   6h     5'   
   -   5        $??0??5'      $??1??6)   $??Center=?69   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Minimum$ 0X?5(         Maximum$ 10000X?6(   ?  ?Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   FormatString$ ######0.0######??5,         Value 5)      ?? *TorqueLowPassFilterBandwidth+	 ?? UpdateSourceTrigger$	 	LostFocus=?{?5+      ?? *EditEnabled+5,        5-   
   -   5        $??1??5-      $??1??6)   $??Center=?69   $ 0X?5.      $ 10000X?6(   $ ######0.0######??52       5/      ?? *TorqueNotchFilterFrequency+$	 	LostFocus=?{?51      ?? *EditEnabled+52        53   
   -   5        $??2??53      $??1??6)   $??Center=?69   $ 0X?54      $ ######0.0######??58       55      ?? *TorqueLeadLagFilterGain+$	 	LostFocus=?{?57      ?? *EditEnabled+58        59   
   -   5        $??3??59      $??1??6)   $??Center=?69   $ 0X?5:      $ 10000X?6(   $ ######0.0######??5>       5;      ?? *TorqueLeadLagFilterBandwidth+$	 	LostFocus=?{?5=      ?? *EditEnabled+5>      ?? 5@   
   $??0??6   $??2??6"   
 ?? Orientation$
 
Horizontal=?5A      $??Center=?6.   .+?5B      ?? #??[  6   $??6??68   $??5??6B   ?? 5C       	 Hertz#???  6   5D      ?? 5E   
   $??1??6   $??2??6"   $
 
Horizontal=?5F      $??Center=?6.   .+?5G      ?? #??[  6   $??6??68   $??5??6B   ?? 5H      #???  6   5I      ?? 5J   
   $??3??6   $??2??6"   $
 
Horizontal=?5K      $??Center=?6.   .+?5L      ?? #??[  6   $??6??68   $??5??6B   ?? 5M      #???  6   5N      ?? 5P   
   $??0??6   $??3??6    * &Filters Adaptive Tuning Configuration:#???  6-   6r   ?? 5Q   
   $??1??6   $??3??6    5 1Filters Torque Notch Filter High Frequency Limit:#???  6-   6}   ?? 5R   
   $??2??6   $??3??6    4 0Filters Torque Notch Filter Low Frequency Limit:#???  6-   6|   ?? 5S   
   $??3??6   $??3??6    1	 -Filters Torque Notch Filter Tuning Threshold:#??? 	 6-   6y   ?? 5V   
   $??0??6   $??4??6"   $??2??62   $
 
Horizontal=?5W      $??Center=?6.   .+?5X      ?? -   5         ??Name$$ CmbAdaptiveTuningConfiguration??5Y      $	??190??5X       ?? SelectedValuePath$ Value??5[       ?? DisplayMemberPath$ DisplayName??5\      ??5Z      ?? *#!AdaptiveTuningConfigurationSource+ ?? SelectedValue 5]      ?? *AdaptiveTuningConfiguration+{?5^      ?? *EditEnabled+?? 5_      #??[  6   $??6??68   $??5??6B   5`        5c   
   -   5        $??1??5c      $??4??6)   $??Center=?69   $ 20X?5d      $
 4000X?6)   $ ######0.0######??5h       5e      ?? *%#TorqueNotchFilterHighFrequencyLimit+$	 	LostFocus=?{?5g      ?? *AdaptiveTuningEditEnabled+5h        5i   
   -   5        $??2??5i      $??4??6)   $??Center=?69   $ 20X?5j      $
 2000X?6)   $ ######0.0######??5n       5k      ?? *$"TorqueNotchFilterLowFrequencyLimit+$	 	LostFocus=?{?5m      ?? *AdaptiveTuningEditEnabled+5n        5o   
   -	   5        $??3??5o      $??4??6)   $??Center=?69   $ 0X?5p      $	 100X?6(   $ ######0.0######??5t       5q      ?? *" TorqueNotchFilterTuningThreshold+$	 	LostFocus=?{?5s      ?? *AdaptiveTuningEditEnabled+5t      ?? 5v   
   $??1??6   $??5??6"   $
 
Horizontal=?5w      $??Center=?6.   .+?5x      ?? #??[  6   $??6??68   $??5??6B   ?? 5y      #???  6   5z      ?? 5{   
   $??2??6   $??5??6"   $
 
Horizontal=?5|      $??Center=?6.   .+?5}      ?? #??[  6   $??6??68   $??5??6B   ?? 5~      #???  6   5      ?? 5?   
   $??3??6   $??5??6"   $
 
Horizontal=?5?      $??Center=?6.   .+?5?      ?? #??[  6   $??6??68   $??5??6B   ?? 5?       
 %Motor Rated#??? 
 6   5?      5?      5?      