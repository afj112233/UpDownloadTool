   M S B A M L   `   `   `  ????a  ]ICSStudio.Components, Version=1.0.106.22348, Culture=neutral, PublicKeyToken=65b8cea88eb98737M+clr-namespace:ICSStudio.Components.ControlsICSStudio.Components.Controls  5      G    AICSStudio.Components.Controls.DataTypeSelectorAutoCompleteControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      =local+clr-namespace:ICSStudio.Components.Controls     5      -   5          >?Name$
  Root??5	   F   |?.  ? 	Resources 5
         LaunchDataTypeSelector??%   &	        ?? 5   
   6   5       J? CommandBindings 5      )  #System.Windows.Input.CommandBinding  5   
   -   5           Command# [  5   B    J? InputBindings 5      ?? 5   
    ?? Command# [  6   
 ?? Key$
 Down??6G    ?? 	Modifiers$	 Altf?6R   .??5      ?  ? ColumnDefinitions 5   
   ?? 5      $??* ?6   ?? 5      $
??Auto ?6   .+?5   
   ?   9ICSStudio.Components.Controls.DataTypeAutoCompleteControl  -   5        	  Name$	 DataTypeAutoCompleteBox??5      ?   9ICSStudio.Components.Controls.DataTypeAutoCompleteControl
   TextChanged$
 HandleTextChanged??6.   $??0??6N   $??2,1,1,2q?6^   ??5      ?? *DataTypeName+ ?? ElementName$
 Root????5      ?? *VerticalContentAlignment+$
 Root????5      ?? *BorderThickness+$
 Root????5      ?? *	IsTabStop+$
 Root??   StructureMembers 5      ?? *StructureMembers+$
 Root???? 5!   
   -   5         ??Name$ DataTypeSelectorButton??5!   ;   $??1??6[   ??.  $??21??5"   #   $	??...??61   $??1,1,1,4q?6?   5%      '  !System.Windows.VisualStateManager   VisualStateGroups 5'      %  System.Windows.VisualStateGroup  5(   
      Name$ InvisibleState??6      States. 5)         System.Windows.VisualState     Name$ Visible??6      
Storyboard. 5*      ?? .??5+      U?  ?? 
TargetName$ DataTypeSelectorButton??64    ?? TargetProperty$ Opacity?5,   4   .\?5-      n?  O? KeyTime$ 00:00:00.00??61    O? Value$	 0.0Q?6G   9  3System.Windows.Media.Animation.EasingDoubleKeyFrame  5.      $ 00:00:00.20??6/   $	 1.0Q?6E      EasingFunction 5/      -  'System.Windows.Media.Animation.SineEase  50   "   7	  1System.Windows.Media.Animation.EasingFunctionBase 	  
EasingMode$ 	EaseInOut=?6+   52      53      T? 54      $ DataTypeSelectorButton??64   $ 
Visibility?55   4   ..?56      i?  R? KeyTime$ 00:00:00.01??61   
  System.Windows.Visibility 
  Visible R? Value# Z 6G   57      U? 58      $ DataTypeAutoCompleteBox??64   $ Opacity?59   4   .\?5:      n? $ 00:00:00.00??61   $	 1.0Q?6G   5;      ?? 5<      $ DataTypeAutoCompleteBox??63   $ Foreground.Color?5=   3   .g?5>      p?  ?? KeyTime$ 00:00:00.01??60    ?? Value# ["??5?   0   5@      5A      5B        5C      $ 	Invisible??6   . 5D      ?? .??5E      T? $ DataTypeSelectorButton??64   $ 
Visibility?5F   4   ..?5G      i? $ 00:00:00.00??61    
  	Collapsed# Z 6G   5H      ?? 5I      $ DataTypeAutoCompleteBox??63   $ Foreground.Color?5J   3   .g?5K      p? $ 00:00:00.00??60   $ Transparent??6F   5L      5M      5N      5O      5R      