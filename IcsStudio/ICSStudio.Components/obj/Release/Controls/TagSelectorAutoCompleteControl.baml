   M S B A M L   `   `   `  ???? a  ]ICSStudio.Components, Version=1.0.106.22348, Culture=neutral, PublicKeyToken=65b8cea88eb98737M+clr-namespace:ICSStudio.Components.ControlsICSStudio.Components.Controls  Q-clr-namespace:ICSStudio.Components.ConvertersICSStudio.Components.Converters  B    <ICSStudio.Components.Controls.TagSelectorAutoCompleteControl   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     @controls+clr-namespace:ICSStudio.Components.Controls     D
converters-clr-namespace:ICSStudio.Components.Converters     -     >?Name$
  Root??|?.  ? 	Resources    LaunchTagBrowserD   >ICSStudio.Components.Converters.ClippedTextBoxTooltipConverter " ClippedTextBoxTooltipConverter??%   &	        &	      ??    J? CommandBindings )  #System.Windows.Input.CommandBinding  -      Command# [   J? InputBindings ??  ?? Command# [  
 ?? Key$
 Down?? ?? 	Modifiers$	 Altf?.??]? .+??? -    ??Name$ TagBrowserButton??$??Right=???.  $??21??$	??...??$??1,1,1,4q?:   4ICSStudio.Components.Controls.TagAutoCompleteControl  -   	  Name$	 
TagNameBox??:   4ICSStudio.Components.Controls.TagAutoCompleteControl
   TextChanged$
 HandleTextChanged??   PreviewMouseDoubleClick$ TextBoxSelectAll??$??2,2,1,1q????? *	TagName+ ?? ElementName$
 Root?????? *VerticalContentAlignment+$
 Root?????? *BorderThickness+$
 Root?????? *	IsTabStop+$
 Root??   ExpectingModule ?? *BrowsingModules+$
 Root??.??
 ? Tag `?  `? 	Converter# [ .1??? $
 Root?? ?? Path$ TagName? ?? UpdateSourceTrigger$ PropertyChanged=???  ?? RelativeSource ?? *Self+.??