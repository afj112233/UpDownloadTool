   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05Z=clr-namespace:ICSStudio.Gui.Converters;assembly=ICSStudio.GuiICSStudio.Gui.Converters  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5       ICSStudio.Components:   4ICSStudio.Components.Controls.SingleClickEditControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      T
converters=clr-namespace:ICSStudio.Gui.Converters;assembly=ICSStudio.Gui     5      9system&clr-namespace:System;assembly=mscorlib     5      -   5          >?Name$
  Root??5   (   |?.  ? 	Resources 5       ?? MergedDictionaries ?? Source3   -ICSStudio.Gui.Converters.DescriptionConverter   StringSquisher % !InvisibilityAnimationDropDisabled ?? TargetProperty ?? Target9  3System.Windows.Media.Animation.EasingDoubleKeyFrame O? KeyTime O? Value   EasingFunction-  'System.Windows.Media.Animation.SineEase7  1System.Windows.Media.Animation.EasingFunctionBase	   
EasingMode $  InvisibilityAnimationDropEnabled
 R? KeyTime R? Value  MakeVisibleStoryboard U?Name ?? 
TargetName  
CommitEdit  
CancelEdit?? 5   
    5      ?? 5      $, &..\Resources\CommonGridDictionary.xaml??6%   %~  &	        &	      &	 ?     &	 Z    &	     &	 /      5      6.   U? 5      $ Opacity?6V    5   ,   Z? .\?5        $ 00:00:00.20??6'   $	 1.0Q?6=    5        5      $	 	EaseInOut=?6#   5      5      T? 5      $ IsInvisible?6U    5   ,   Z? ..?5      i? $
 00:00:00.00??6)    5      ??5       	False??60   5"      5#      ?? 5$      .??5%      U? $  ReadOnlyTextBlockAnimation??60   $ ReadOnlyTextBlock??6T   $ Opacity?5&   0   .\?5'      n? $ 00:00:00.01??6-   $	 0.0Q?6C   5(      5)      ?? 5*      6   ?? 5+      6   5,       J? CommandBindings 5.      )  #System.Windows.Input.CommandBinding  5/   
   -   5           Command$  ApplicationCommands.Delete??5/   7     50   
   -   5        # [ 50   6     51   
   -   5        # [ 51   6    J? InputBindings 53      ?? 54   
    ?? Command# [ 6   
 ?? Key$ Escape??6;   ?? 55   
   # [ 6   $ F2??6;   ?? 56   
   # [ 6   $ Enter??6;    ?? 	Modifiers$
 Ctrlf?6G   ?? 57   
   # [ 6   $	 Tab??6;   $ Shiftf?6E   .??59      ? -   5         ?Name$ 
LayoutGrid??59      .+?5:      ]? -   5         ]?Name$  EditableControlPlaceholder??5:      
?????? $??Stretch=?6M   $	??Top=?5;      $??0q?6(   ?? 5<      -   5         ??Name$ ReadOnlyBackgroundFill??5<      
??????  J? IsHitTestVisible .  6C   ?? 5=      -   5         ??Name$ ReadOnlyTextBlock??5=      $??0??6+    .  $??CharacterEllipsis=?5>      $??0q?61   ??6<   ?? *
Foreground+ ?? ElementName$
 Root?? ?? Padding 5?      ?? *ReadOnlyTextPadding+$
 Root????5@      ?? *ReadOnlyText+$
 Root?? ?? 	Converter# [  ??5A      ?? *ReadOnlyTextBlockStyle+$
 Root??5B      '  !System.Windows.VisualStateManager   VisualStateGroups 5C      %  System.Windows.VisualStateGroup  5D      -	   5        CommonStates??   States. 5E         System.Windows.VisualState  -
   5        
Normal??5E        5F      -   5        Parent_Cell_Focused??   
Storyboard# [ 5F   1     5G      -   5        Unfocused_MouseOver??# [ 5G   1     5H      -   5        Focused_MouseOver??# [ 5H   /     5I      -   5        Focused_MouseNotOver??# [ 5I   2     5J      -   5        ReadOnly_RowUnselected??. 5K   
   ?? .??5L      T? $ ReadOnlyBackgroundFill??6*   $
 Fill?6Y   ..?5M      i? $
 00:00:00.00??6%     ReadOnlyCellBackgroundBrush# [ 6;   5N      5O      5P   	     5Q      -   5        '#ReadOnly_RowSelected_FocusNotInGrid??. 5R   
   ?? .??5S      T? $ ReadOnlyBackgroundFill??6*   $
 Fill?6Y   ..?5T      i? $
 00:00:00.00??6%   # ? ??6;   5U      T? 5V      $ ReadOnlyTextBlock??6*   $ 
Foreground?6T   ..?5W      i? $
 00:00:00.00??6%   # ? ??6;   5X      5Y      5Z   	     5[      -   5        $ ReadOnly_RowSelected_FocusInGrid??. 5\   
   ?? .??5]      T? $ ReadOnlyBackgroundFill??6*   $
 Fill?6Y   ..?5^      i? $
 00:00:00.00??6%   # ? ??6;   5_      T? 5`      $ ReadOnlyTextBlock??6*   $ 
Foreground?6T   ..?5a      i? $
 00:00:00.00??6%   # ? ??6;   5b      5c      5d   	   5e      5g      