   M S B A M L   `   `   `  ???? a  ]ICSStudio.Components, Version=1.0.106.22348, Culture=neutral, PublicKeyToken=65b8cea88eb98737M+clr-namespace:ICSStudio.Components.ControlsICSStudio.Components.Controls  G    AICSStudio.Components.Controls.DataTypeSelectorAutoCompleteControl   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     =local+clr-namespace:ICSStudio.Components.Controls     -     >?Name$
  Root??|?.  ? 	Resources    LaunchDataTypeSelector??%   &	        ??  J? CommandBindings )  #System.Windows.Input.CommandBinding  -      Command# [   J? InputBindings ??  ?? Command# [  
 ?? Key$
 Down?? ?? 	Modifiers$	 Altf?.???  ? ColumnDefinitions ?? $??* ??? $
??Auto ?.+??   9ICSStudio.Components.Controls.DataTypeAutoCompleteControl  -   	  Name$	 DataTypeAutoCompleteBox???   9ICSStudio.Components.Controls.DataTypeAutoCompleteControl
   TextChanged$
 HandleTextChanged??$??0??$??2,1,1,2q????? *DataTypeName+ ?? ElementName$
 Root?????? *VerticalContentAlignment+$
 Root?????? *BorderThickness+$
 Root?????? *	IsTabStop+$
 Root??   StructureMembers ?? *StructureMembers+$
 Root???? -    ??Name$ DataTypeSelectorButton??$??1????.  $??21??$	??...??$??1,1,1,4q?'  !System.Windows.VisualStateManager   VisualStateGroups %  System.Windows.VisualStateGroup     Name$ InvisibleState??   States.    System.Windows.VisualState     Name$ Visible??   
Storyboard. ?? .??U?  ?? 
TargetName$ DataTypeSelectorButton?? ?? TargetProperty$ Opacity?.\?n?  O? KeyTime$ 00:00:00.00?? O? Value$	 0.0Q?9  3System.Windows.Media.Animation.EasingDoubleKeyFrame  $ 00:00:00.20??$	 1.0Q?   EasingFunction -  'System.Windows.Media.Animation.SineEase  7	  1System.Windows.Media.Animation.EasingFunctionBase 	  
EasingMode$ 	EaseInOut=?T? $ DataTypeSelectorButton??$ 
Visibility?..?i?  R? KeyTime$ 00:00:00.01??
  System.Windows.Visibility 
  Visible R? Value# Z U? $ DataTypeAutoCompleteBox??$ Opacity?.\?n? $ 00:00:00.00??$	 1.0Q??? $ DataTypeAutoCompleteBox??$ Foreground.Color?.g?p?  ?? KeyTime$ 00:00:00.01?? ?? Value# ["??  $ 	Invisible??. ?? .??T? $ DataTypeSelectorButton??$ 
Visibility?..?i? $ 00:00:00.00?? 
  	Collapsed# Z ?? $ DataTypeAutoCompleteBox??$ Foreground.Color?.g?p? $ 00:00:00.00??$ Transparent??