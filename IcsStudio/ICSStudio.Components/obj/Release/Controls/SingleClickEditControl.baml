   M S B A M L   `   `   `  ???? Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05Z=clr-namespace:ICSStudio.Gui.Converters;assembly=ICSStudio.GuiICSStudio.Gui.Converters  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  ICSStudio.Components:   4ICSStudio.Components.Controls.SingleClickEditControl   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     T
converters=clr-namespace:ICSStudio.Gui.Converters;assembly=ICSStudio.Gui     9system&clr-namespace:System;assembly=mscorlib     -     >?Name$
  Root??|?.  ? 	Resources  ?? MergedDictionaries ?? Source3   -ICSStudio.Gui.Converters.DescriptionConverter   StringSquisher % !InvisibilityAnimationDropDisabled ?? TargetProperty ?? Target9  3System.Windows.Media.Animation.EasingDoubleKeyFrame O? KeyTime O? Value   EasingFunction-  'System.Windows.Media.Animation.SineEase7  1System.Windows.Media.Animation.EasingFunctionBase	   
EasingMode $  InvisibilityAnimationDropEnabled
 R? KeyTime R? Value  MakeVisibleStoryboard U?Name ?? 
TargetName  
CommitEdit  
CancelEdit??  ?? $, &..\Resources\CommonGridDictionary.xaml??%i  &	        &	      &	 ^     &	 ?     &	 #    &	 (      U? $ Opacity? Z? .\?  $ 00:00:00.20??$	 1.0Q?   $	 	EaseInOut=?T? $ IsInvisible? Z? ..?i? $
 00:00:00.00?? ??	False???? .??U? $  ReadOnlyTextBlockAnimation??$ ReadOnlyTextBlock??$ Opacity?.\?n? $ 00:00:00.01??$	 0.0Q??? ??  J? CommandBindings )  #System.Windows.Input.CommandBinding  -      Command$  ApplicationCommands.Delete??  -   # [   -   # [  J? InputBindings ??  ?? Command# [ 
 ?? Key$ Escape???? # [ $ F2???? # [ $ Enter?? ?? 	Modifiers$
 Ctrlf??? # [ $	 Tab??$ Shiftf?.??? -    ?Name$ 
LayoutGrid??.+?]? -    ]?Name$  EditableControlPlaceholder??
?????? $??Stretch=?$	??Top=?$??0q??? -    ??Name$ ReadOnlyBackgroundFill??
??????  J? IsHitTestVisible .  ?? -    ??Name$ ReadOnlyTextBlock??$??0?? .  $??CharacterEllipsis=?$??0q????? *
Foreground+ ?? ElementName$
 Root?? ?? Padding ?? *ReadOnlyTextPadding+$
 Root?????? *ReadOnlyText+$
 Root?? ?? 	Converter# [  ???? *ReadOnlyTextBlockStyle+$
 Root??'  !System.Windows.VisualStateManager   VisualStateGroups %  System.Windows.VisualStateGroup  -	   CommonStates??   States.    System.Windows.VisualState  -
   
Normal??  -   Parent_Cell_Focused??   
Storyboard# [   -   Unfocused_MouseOver??# [   -   Focused_MouseOver??# [   -   Focused_MouseNotOver??# [   -   ReadOnly_RowUnselected??. ?? .??T? $ ReadOnlyBackgroundFill??$
 Fill?..?i? $
 00:00:00.00??  ReadOnlyCellBackgroundBrush# [   -   '#ReadOnly_RowSelected_FocusNotInGrid??. ?? .??T? $ ReadOnlyBackgroundFill??$
 Fill?..?i? $
 00:00:00.00??# ? ??T? $ ReadOnlyTextBlock??$ 
Foreground?..?i? $
 00:00:00.00??# ? ??  -   $ ReadOnly_RowSelected_FocusInGrid??. ?? .??T? $ ReadOnlyBackgroundFill??$
 Fill?..?i? $
 00:00:00.00??# ? ??T? $ ReadOnlyTextBlock??$ 
Foreground?..?i? $
 00:00:00.00??# ? ??