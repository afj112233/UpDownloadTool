   M S B A M L   `   `   `  ????`  \ICSStudio.AvalonEdit, Version=0.0.15.22348, Culture=neutral, PublicKeyToken=77420314b947be55Y@clr-namespace:ICSStudio.AvalonEdit;assembly=ICSStudio.AvalonEditICSStudio.AvalonEdit  5      Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05N7clr-namespace:ICSStudio.Gui.View;assembly=ICSStudio.GuiICSStudio.Gui.View 5      ` \ICSStudio.StxEditor, Version=1.0.130.22348, Culture=neutral, PublicKeyToken=172dbc864a8685bcC&clr-namespace:ICSStudio.StxEditor.ViewICSStudio.StxEditor.View 5	      ,   &ICSStudio.StxEditor.View.StxEditorView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      W
avalonEdit@clr-namespace:ICSStudio.AvalonEdit;assembly=ICSStudio.AvalonEdit     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Ggui7clr-namespace:ICSStudio.Gui.View;assembly=ICSStudio.Gui     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      7view&clr-namespace:ICSStudio.StxEditor.View     5	        J? InputBindings  5      ?? 5   
   
 ?? Key$ W??5       ?? 	Modifiers$ Controlf?5       ?? Command 5      ?? *NewTagCommand+5      ?? 5   
   $ Space??5      $ Controlf?5       5      ?? *BrowseTagsCommand+5      ?? 5   
   $ Insert??5      $	 Altf?5       5      ?? *AddSTElementCommand+5      ?? 5   
   $ D3??5      $	 Altf?5       5      ?? *WatchTagsCommand+5      ?? 5   
   $ A??5      $	 Altf?5!       5       ?? *ArgumentListCommand+5!      ?? 5#   
   $ X??5$      $ Controlf?5&       5%      ?? *
CutCommand+5&      ?? 5'   
   $ C??5(      $ Controlf?5*       5)      ?? *CopyCommand+5*      ?? 5+   
   $ V??5,      $ Controlf?5.       5-      ?? *PasteCommand+5.      ?? 50   
   $ G??51      $ Controlf?53       52      ?? *GoToCommand+53      ?? 55   
   $ Enter??56      $ Controlf?58       57      ?? *EditTagPropertiesCommand+58      ?? 59   
   $ E??5:      $ Controlf?5<       5;      ?? *CrossTagCommand+5<      ?? 5=   
   $ Space??5>      $ Control + Altf?5@       5?      ?? *BrowseEnumCommand+5@      ?? 5A   
   $ Enter??5B      $	 Altf?5D       5C      ?? *PropertiesCommand+5D       ? 	Resources 5G      (  "ICSStudio.Gui.View.BindableCommand   IncreaseZoomCommand   Command ?? Path  DecreaseZoomCommand  IncreaseIndentCommand  DecreaseIndentCommand  CommentCommand  UncommentCommand  ToggleWhiteCommand  ToggleValueCommand  PendingCommand 	 TestCommand 
 OriginalCommand??%?  &	        &	 6     &	 l     &	 ?     &	 ?     &	     &	 8    &	 k    &	 ?    &		 ?    &	
 ?      5H   
    6:   ?? $ IncreaseZoomCommand?  5I   
    6:   ?? $ DecreaseZoomCommand?  5J   
    6<   ?? *IncreaseIndentCommand+  5K   
    6<   ?? *DecreaseIndentCommand+  5L   
    65   ?? *CommentCommand+  5M   
    67   ?? *UncommentCommand+  5N   
    69   ?? *ToggleWhiteCommand+  5O   
    69   ?? *ToggleValueCommand+  5P   
    65   ?? *PendingCommand+  5Q   
    62   ?? *TestCommand+  5R   
    66   ?? *OriginalCommand+5S       J? CommandBindings 5T      ,  &ICSStudio.Gui.View.RelayCommandBinding  5U   
   4  .ICSStudio.StxEditor.View.StxEditorViewCommands   IncreaseZoom)  #System.Windows.Input.CommandBinding	   Command#	 Z 6"   
   RelayCommand#
 [  6_     5V   
      DecreaseZoom#	 Z 6"   #
 [ 6_     5W   
      IncreaseIndent#	 Z 6"   #
 [ 6a     5X   
      DecreaseIndent#	 Z 6"   #
 [ 6a     5Y   
      Comment#	 Z 6"   #
 [ 6Z     5Z   
      	Uncomment#	 Z 6"   #
 [ 6\     5[   
      ToggleWhite#	 Z 6"   #
 [ 6^     5\   
      ToggleValue#	 Z 6"   #
 [ 6^     5]   
      PendingCommand#	 Z 6"   #
 [ 6a     5^   
      TestCommand#	 Z 6"   #
 [	 6^     5_   
      OriginalCommand#	 Z 6"   #
 [
 6b   .??5a      ?  ? ColumnDefinitions 5b   
   ?? 5c      $??10 ?6   ?? 5d      .+?5f   
   ?? ??6   ?? *Status+?? 5g   
   -   5        $??DragTextBlock??5h      $??1??5i       K? ZIndex$ 5??5k      4  .System.Windows.Automation.AutomationProperties   Name$ DragTextBlock??5l       J? IsHitTestVisible .   J? Opacity$	 0.7Q?5q      ??5j      ?? *Margin+ ?? Mode$ OneWay=? ? DataContext 5m      ?? *DragPreviewViewModel+??5n      ?? *
FontFamily+$ OneWay=???5o      ?? *
FontSize+$ OneWay=???5r      ?? *Text+y?5s      ?? *
Visibility+%   ICSStudio.AvalonEdit.TextEditor  5u   
   -   5          Name$ Editor??5v      $??1??5w       J? 	AllowDrop .    Document 5y      ?? *
Document+$ OneWay=?5}      5      5?      