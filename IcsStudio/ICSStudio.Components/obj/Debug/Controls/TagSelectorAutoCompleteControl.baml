   M S B A M L   `   `   `  ????a  ]ICSStudio.Components, Version=1.0.106.22348, Culture=neutral, PublicKeyToken=65b8cea88eb98737M+clr-namespace:ICSStudio.Components.ControlsICSStudio.Components.Controls  5      Q-clr-namespace:ICSStudio.Components.ConvertersICSStudio.Components.Converters  5      B    <ICSStudio.Components.Controls.TagSelectorAutoCompleteControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      @controls+clr-namespace:ICSStudio.Components.Controls     5      D
converters-clr-namespace:ICSStudio.Components.Converters     5      -   5          >?Name$
  Root??5
   F   |?.  ? 	Resources 5         LaunchTagBrowserD   >ICSStudio.Components.Converters.ClippedTextBoxTooltipConverter " ClippedTextBoxTooltipConverter??%:   &	        &	      ?? 5   
   6     5   
   64   5       J? CommandBindings 5      )  #System.Windows.Input.CommandBinding  5   
   -   5           Command# [  5   <    J? InputBindings 5      ?? 5   
    ?? Command# [  6   
 ?? Key$
 Down??6A    ?? 	Modifiers$	 Altf?6L   .??5      ]? .+?5   
   ?? -   5         ??Name$ TagBrowserButton??5   5   $??Right=?6O   ??.  $??21??5   #   $	??...??61   $??1,1,1,4q?6?   :   4ICSStudio.Components.Controls.TagAutoCompleteControl  5   
   -   5        	  Name$	 
TagNameBox??5   *   :   4ICSStudio.Components.Controls.TagAutoCompleteControl
   TextChanged$
 HandleTextChanged??6>      PreviewMouseDoubleClick$ TextBoxSelectAll??6^   $??2,2,1,1q?5   *   ??6<   ?? *	TagName+ ?? ElementName$
 Root????5   *   ?? *VerticalContentAlignment+$
 Root????5   *   ?? *BorderThickness+$
 Root????5   *   ?? *	IsTabStop+$
 Root??   ExpectingModule 5   *   ?? *BrowsingModules+$
 Root??.??6h   
 ? Tag 5      `? 5       `? 	Converter# [ 6   .1?5       ?? $
 Root??6    ?? Path$ TagName?61    ?? UpdateSourceTrigger$ PropertyChanged=?6@   ?? 5!       ?? RelativeSource 6   ?? *Self+5"      .??5#   $   5$      5%      5&      