   M S B A M L   `   `   `  ????  ICSStudio.UIServicesPackageA    ;ICSStudio.UIServicesPackage.ManualTune.Controls.Feedforward   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5        ? 	Resources  5	         	LeftImage ?? 	UriSource$  Xceed.Wpf.Toolkit.SingleUpDown ?? 
TargetType?? 5
   
   %?   &	        '  i     ?? 5      $Q Kpack://application:,,,/ICSStudio.Gui;component/Resources/image/left_6_5.png??5      ?? 5      "  6   .?5      -   5        5      5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $	??100 ?6   ?? 5      $
??Auto ?6   .+?5   
   ?? $??0??6   $??0??6     Velocity Feedforward:#???  6-   ?? 5   
   $??1??6   $??0??6     Acceleration Feedforward:#???  6-     5    
   -   5        $??0??5       $??1??6)   $??Center=?69   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   Minimum$ 0X?5!      ?  ?Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   FormatString$ ######0.0######??5%         Value 5"      ?? *VelocityFeedforwardGain+ ?? UpdateSourceTrigger$ 	LostFocus=?{?5$      ?? *EditEnabled+5%        5&   
   -   5        $??1??5&      $??1??6)   $??Center=?69   $ 0X?5'      $ ######0.0######??5+       5(      ?? *AccelerationFeedforwardGain+$ 	LostFocus=?{?5*      ?? *EditEnabled+5+      ?? 5-   
   $??0??6   $??2??6"   	 ?? Orientation$	 
Horizontal=?5.      $??Center=?6.   .+?5/      ?? #??[  6   $??6??68   $??5??6B   ?? 50      $??%??6   51      ?? 53   
   $??1??6   $??2??6"   $	 
Horizontal=?54      $??Center=?6.   .+?55      ?? #??[  6   $??6??68   $??5??6B   ?? 56      $??%??6   57      59      5:      