   M S B A M L   `   `   `  ???? `  \ICSStudio.AvalonEdit, Version=0.0.15.22348, Culture=neutral, PublicKeyToken=77420314b947be55Y@clr-namespace:ICSStudio.AvalonEdit;assembly=ICSStudio.AvalonEditICSStudio.AvalonEdit  Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05N7clr-namespace:ICSStudio.Gui.View;assembly=ICSStudio.GuiICSStudio.Gui.View ` \ICSStudio.StxEditor, Version=1.0.130.22348, Culture=neutral, PublicKeyToken=172dbc864a8685bcC&clr-namespace:ICSStudio.StxEditor.ViewICSStudio.StxEditor.View ,   &ICSStudio.StxEditor.View.StxEditorView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     W
avalonEdit@clr-namespace:ICSStudio.AvalonEdit;assembly=ICSStudio.AvalonEdit     @d2http://schemas.microsoft.com/expression/blend/2008     Ggui7clr-namespace:ICSStudio.Gui.View;assembly=ICSStudio.Gui     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     7view&clr-namespace:ICSStudio.StxEditor.View       J? InputBindings  ?? 
 ?? Key$ W?? ?? 	Modifiers$ Controlf? ?? Command ?? *NewTagCommand+?? $ Space??$ Controlf? ?? *BrowseTagsCommand+?? $ Insert??$	 Altf? ?? *AddSTElementCommand+?? $ D3??$	 Altf? ?? *WatchTagsCommand+?? $ A??$	 Altf? ?? *ArgumentListCommand+?? $ X??$ Controlf? ?? *
CutCommand+?? $ C??$ Controlf? ?? *CopyCommand+?? $ V??$ Controlf? ?? *PasteCommand+?? $ G??$ Controlf? ?? *GoToCommand+?? $ Enter??$ Controlf? ?? *EditTagPropertiesCommand+?? $ E??$ Controlf? ?? *CrossTagCommand+?? $ Space??$ Control + Altf? ?? *BrowseEnumCommand+?? $ Enter??$	 Altf? ?? *PropertiesCommand+ ? 	Resources (  "ICSStudio.Gui.View.BindableCommand   IncreaseZoomCommand   Command ?? Path  DecreaseZoomCommand  IncreaseIndentCommand  DecreaseIndentCommand  CommentCommand  UncommentCommand  ToggleWhiteCommand  ToggleValueCommand  PendingCommand 	 TestCommand 
 OriginalCommand??%?  &	        &	 (     &	 P     &	 x     &	 ?     &	 ?     &	 ?     &	 	    &	 .    &		 O    &	
 m       ?? $ IncreaseZoomCommand?   ?? $ DecreaseZoomCommand?   ?? *IncreaseIndentCommand+   ?? *DecreaseIndentCommand+   ?? *CommentCommand+   ?? *UncommentCommand+   ?? *ToggleWhiteCommand+   ?? *ToggleValueCommand+   ?? *PendingCommand+   ?? *TestCommand+   ?? *OriginalCommand+ J? CommandBindings ,  &ICSStudio.Gui.View.RelayCommandBinding  4  .ICSStudio.StxEditor.View.StxEditorViewCommands   IncreaseZoom)  #System.Windows.Input.CommandBinding	   Command#	 Z 
   RelayCommand#
 [       DecreaseZoom#	 Z #
 [      IncreaseIndent#	 Z #
 [      DecreaseIndent#	 Z #
 [      Comment#	 Z #
 [      	Uncomment#	 Z #
 [      ToggleWhite#	 Z #
 [      ToggleValue#	 Z #
 [      PendingCommand#	 Z #
 [      TestCommand#	 Z #
 [	      OriginalCommand#	 Z #
 [
 .???  ? ColumnDefinitions ?? $??10 ??? .+??? ???? *Status+?? -   $??DragTextBlock??$??1?? K? ZIndex$ 5??4  .System.Windows.Automation.AutomationProperties   Name$ DragTextBlock?? J? IsHitTestVisible .   J? Opacity$	 0.7Q????? *Margin+ ?? Mode$ OneWay=? ? DataContext ?? *DragPreviewViewModel+???? *
FontFamily+$ OneWay=????? *
FontSize+$ OneWay=????? *Text+y??? *
Visibility+%   ICSStudio.AvalonEdit.TextEditor  -     Name$ Editor??$??1?? J? 	AllowDrop .    Document ?? *
Document+$ OneWay=?