   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05Z=clr-namespace:ICSStudio.Gui.Converters;assembly=ICSStudio.GuiICSStudio.Gui.Converters  5      Z VICSStudio.Cip, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=dfd71c81d447f80dT:clr-namespace:ICSStudio.Cip.Objects;assembly=ICSStudio.CipICSStudio.Cip.Objects 5       ICSStudio.UIServicesPackageL   FICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel.PolarityPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      T
converters=clr-namespace:ICSStudio.Gui.Converters;assembly=ICSStudio.Gui     5      Lenums:clr-namespace:ICSStudio.Cip.Objects;assembly=ICSStudio.Cip     5        ? 	Resources  5
       ?? MergedDictionaries ?? Source3   -ICSStudio.Gui.Converters.EnumBooleanConverter   PolarityTypesConverter?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %   &	          5      6.   5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $??1* ?6    ? ColumnDefinitions 5   
   ?? 5      ?? 5      .+?5    
   ? $??0??6   $??0??6   {?5!      ?? *IsPolarityEnabled+ 5"      ?? 5#      $
??Auto ?6    ?? 5$      $
??Auto ?6    ?? 5%      $
??Auto ?6    ?? 5&      $
??Auto ?6    .+?5)      ? $??0??6   $??Center=?6     5*      ?? 5+      $	??140 ?6'   ?? 5,      $	??160 ?6'   .+?5/      ?? $??0??6   $??0??6)     Motion Polarity:#???  69   6e   ?? 50      $??0??6   $??1??6*    ?? Orientation$ 
Horizontal=?6:   .+?51      	?   Motion Polarity Normal#???  54   K    d? 	IsChecked 52      ?? *MotionPolarity+ ?? 	Converter# [  (  "ICSStudio.Cip.Objects.PolarityType   Normal	 ?? ConverterParameter#	 Z 54   ?   ?? 55      $??10??6    	? 56        Motion Polarity Inverted#???  59   M    57      ?? *MotionPolarity+# [  
   Inverted#	 Z
 59   ?   5:      5;      ? 5=      $??1??6   $??Center=?6O   y?6    ?? *MotorPolarityVisibility+ 5>      ?? 5?      $	??140 ?6'   ?? 5@      $	??160 ?6'   .+?5C      ?? $??0??6     Motor Polarity:#???  6,   6W   ?? 5D      $??1??6   $ 
Horizontal=?6-   .+?5E      	?   Motor Polarity Normal#???  5H   K    5F      ?? *MotorPolarity+# [  #	 Z 5I      ?? 5J      $??10??6    	? 5K        Motor Polarity Inverted#???  5N   M    5L      ?? *MotorPolarity+# [  #	 Z
 5O      5P      5Q      ? 5S      $??2??6   $??Center=?6S   y?6    ?? *Feedback1PolarityVisibility+ 5T      ?? 5U      $	??140 ?6'   ?? 5V      $	??160 ?6'   .+?5Y      ?? $??0??6   $??0??6)     Feedback 1 Polarity:#???  69   6i   ?? 5Z      $??0??6   $??1??6*   $ 
Horizontal=?6:   .+?5[      	?  
 Normal#???  5^   K    5\      ?? *Feedback1Polarity+# [  #	 Z 5_      ?? 5`      $??10??6    	? 5a       	 Inverted#??? 	 5d   M    5b      ?? *Feedback1Polarity+# [  #	 Z
 5e      5f      5g      ? 5i      $??3??6   $??Center=?6S   y?6    ?? *Feedback2PolarityVisibility+ 5j      ?? 5k      $	??140 ?6'   ?? 5l      $	??160 ?6'   .+?5o      ?? $??0??6   $??0??6)    
 Feedback 2 Polarity:#??? 
 69   6i   ?? 5p      $??0??6   $??1??6*   $ 
Horizontal=?6:   .+?5q      	? #???  5t   K    5r      ?? *Feedback2Polarity+# [  #	 Z 5u      ?? 5v      $??10??6    	? 5w      #??? 	 5z   N    5x      ?? *Feedback2Polarity+# [  #	 Z
 5{      5|      5}      5      ?? 5?   
   $??0??6   $??1??6   {?.  $??Center=?6@   $??Center=?6]     Test Polarity...#???  6x   6?   ]? 5?   
   $??1??6   $??0??6!   $??2??61   .+?5?      ?? ?? 5?       C ?DANGER: Modifying polarity setting may cause unexpected motion.#???  6   6s   5?      5?      5?      