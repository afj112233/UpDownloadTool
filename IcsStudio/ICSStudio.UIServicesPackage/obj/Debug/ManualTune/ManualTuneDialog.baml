   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fq=clr-namespace:ICSStudio.UIServicesPackage.ManualTune.Controls/ICSStudio.UIServicesPackage.ManualTune.Controls  5   	   =    7ICSStudio.UIServicesPackage.ManualTune.ManualTuneDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Rcontrols=clr-namespace:ICSStudio.UIServicesPackage.ManualTune.Controls     5   	   T PImagin.Common, Version=3.1.0.0, Culture=neutral, PublicKeyToken=fd3ef6834d7c49b9+data http://imagin.tech/imagin/common  5   	     ? 
ResizeMode$  NoResize=?5   	    ? SizeToContent$ WidthAndHeight=?5   	    ? ShowInTaskbar .   ? WindowStartupLocation$ CenterOwner=?5       ? Title 5   	   ?? *Title+ ? 	Resources 5       ?? MergedDictionaries ?? Source   ExpandCollapseToggleStyle ?? 
TargetType	 ?? Property
 ?? Value ?? 
TargetType ?? CornerRadius ?? EndPoint ?? 
StartPoint ? Color ? Offset'  !System.Windows.VisualStateManager   VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard ?? TargetProperty ?? 
TargetName R? KeyTime  System.Windows.Visibility   Hidden R? Value   Visible ?? Orientation F?Name ?? Color%  Imagin.Common.Data.BindingProxy 	 Proxy   Data?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %2  &	        &	     ?? 5      " d?5      .?5      ?? 	 ? |?
 .  5      ?? 5      	 ? ??
 5      ?? 5      " d?6*   .R?5      ?? $??PART_Background??5   >   #??z??5   %   $??0q?6Y   $ 	10,10,0,0??5   %   ??5   "   ?? 5   &   $	 0,1&?6:   $	 0,0&?6I   .??5    *   ? $ White??67   $	 0.0Q?6E   ? 5!   *   $ Silver??67   $	 0.5Q?6F   ? 5"   *   $ White??67   $	 0.0Q?6E   5#   '    5%   "     5&   &   CheckStates??. 5'   *     Checked??. 5(   .   ?? .??5)   2   T? $ (UIElement.Visibility)?5*   5   $ 	Collapsed??5+   5   ..?5,   6   i? $ 0??6M   # Z 5-   M   5.   3   T? 5/   2   $ (UIElement.Visibility)?50   5   $ Expanded??51   5   ..?52   6   i? $ 0??6M   # Z 53   M   54   3   55   /   56   +     57   *   	Unchecked??66     58   *   Indeterminate??66   59   '   ._?5;   "   ?? $ 
Horizontal=?6-   $??10 0 0 0q?6F   .+?5<   &   ? $??15??6+   $??13??5=   +   
?????? .+?5@   *   F? $ 	Collapsed??6/   $
??Left=?5A   /   $??Center=?5B   /   $??1,1,1,1q?5C   /   "???? 	=     ? z  	= ? 	=  z 	??5E   .   ?? 5F   2   $ 	#FF444444??6B   5H   +   F? 5I   *   $ Expanded??6/   $
??Left=?5J   /   $??Center=?5K   /   $??1,1,1,1q?5L   /   "????     	= ? z  	= ? 	=  z 	$y?Hidden=?5N   /   ??5O   .   ?? 5P   2   $ 	#FF444444??6B   5R   +   5S   '   ?? 5T   &   #??z??67   $??Stretch=?5U   7   $??Center=?6U   5V   #   5W      5X      5Z      5[        5\       6.   ?? 5]      .??5`      ?  ? RowDefinitions 5a   
   ?? 5b      $
??Auto ?6   ?? 5c      $
??Auto ?6   ?? 5d      $
??Auto ?6   ?? 5e      $
??Auto ?6   .+?5h   
   ? $??0??6    ? ColumnDefinitions 5i      ?? 5j      $
??Auto ?6#   ?? 5k      $
??Auto ?6#   ?? 5l      $
??Auto ?6#   ?? 5m      $	??500 ?6#   .+?5p      d? $??0??6     
ManualTune#???  6+     J? RenderTransformOrigin$	  0,0&?5q      #??[  5r      $
??Left=?5s      $	??Top=?66   ! d? 	IsChecked! 5t      ?? *ManualTuneChecked+" ? LayoutTransform" 5u      ?? 5v      # ?? Angle$# 90Q?6&   5x      B   <ICSStudio.UIServicesPackage.ManualTune.Controls.ManualTuning  5z      $??1??6$   y?5{   $   ?? *Data.ManualTuneVisibility+$ ?? Source#$ [ % ? DataContext% 5|   $   ?? *ManualTuningViewModel+?? 5}      $??2??6   #??["?5~      y?5}   (   ?? *ManualTuneVisibility+5~      E   ?ICSStudio.UIServicesPackage.ManualTune.Controls.MotionGenerator  5      $??3??6'   % 67   ?? *MotionGeneratorViewModel+5?      ? 5?   
   $??1??6   y?6   ?? *ManualTuneVisibility+ 5?      ?? 5?      $
??Auto ?6    ?? 5?      $
??Auto ?6    .+?5?      d? $??0??6     AdditionalTune#???  6(   #??[  5?      ! 5?      ?? *AdditionalTuneChecked+y?5?      ?? *ManualTuneVisibility+?? 5?      $??1??6   $	??150??5?      y?5?      ?? *Data.AdditionalTuneVisibility+#$ [ % 5?      ?? *AdditionalTuneViewModel+.@?5?      ?? $??80??6   ??5?      ?? 5?      .R?5?      ??   Feedforward#???  6(   6O   5?      .??5?      A   ;ICSStudio.UIServicesPackage.ManualTune.Controls.Feedforward  5?      ?? 5?      $??80??6   ??5?      ?? 5?      .R?5?      ??   Compensation#???  6(   6P   5?      .??5?      B	   <ICSStudio.UIServicesPackage.ManualTune.Controls.Compensation	  5?      ?? 5?      $??80??6   ??5?      ?? 5?      .R?5?      ??   AdditionalTuneFilters#???  6(   6Y   5?      .??5?      =
   7ICSStudio.UIServicesPackage.ManualTune.Controls.Filters
  5?      ?? 5?      $??80??6   ??5?      ?? 5?      .R?5?      ??  
 Limits#???  6(   6J   5?      .??5?      <   6ICSStudio.UIServicesPackage.ManualTune.Controls.Limits  5?      ?? 5?      $??80??6   ??5?      ?? 5?      .R?5?      ??   Planner#???  6(   6K   5?      .??5?      =   7ICSStudio.UIServicesPackage.ManualTune.Controls.Planner  5?      5?      5?      ?? 5?   
   $??3??6   $ 
Horizontal=?6"   $??Right=?6;   .+?5?      ??  		 Close#??? 	 6   ??67   ?? *CloseCommand+?? 5?       
 Help#??? 
 6   ??66   ?? *HelpCommand+5?      5?      5?      