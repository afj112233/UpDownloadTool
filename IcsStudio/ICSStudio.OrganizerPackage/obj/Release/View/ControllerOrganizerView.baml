   M S B A M L   `   `   `  ???? g  cICSStudio.OrganizerPackage, Version=1.0.127.22348, Culture=neutral, PublicKeyToken=b1f75cc66356959e[2clr-namespace:ICSStudio.OrganizerPackage.ViewModel$ICSStudio.OrganizerPackage.ViewModel  Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05Z=clr-namespace:ICSStudio.Gui.Converters;assembly=ICSStudio.GuiICSStudio.Gui.Converters N7clr-namespace:ICSStudio.Gui.View;assembly=ICSStudio.GuiICSStudio.Gui.View X<clr-namespace:ICSStudio.Gui.Behaviors;assembly=ICSStudio.GuiICSStudio.Gui.Behaviors V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls =    7ICSStudio.OrganizerPackage.View.ControllerOrganizerView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     V RMaterialDesignThemes.Wpf, Version=3.1.3.1513, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  B	viewModel2clr-namespace:ICSStudio.OrganizerPackage.ViewModel  N
converters=clr-namespace:ICSStudio.Gui.Converters;assembly=ICSStudio.Gui  Bview7clr-namespace:ICSStudio.Gui.View;assembly=ICSStudio.Gui  c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  L	behaviors<clr-namespace:ICSStudio.Gui.Behaviors;assembly=ICSStudio.Gui  Eext;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui  V RGalaSoft.MvvmLight.Platform, Version=5.4.1.0, Culture=neutral, PublicKeyToken=null.command http://www.galasoft.ch/mvvmlight  -     ? 	Resources  1  +ICSStudio.Gui.Converters.IsVisibleConverter   IsVisibleConverter(  "ICSStudio.Gui.View.BindableCommand  CopyCommand   Command  
CutCommand  PasteCommand  DeleteCommand  
GlyphBrush ?? Color  ExpandCollapseToggleStyle ?? 
TargetType ?? Property ?? Value ?? 
TargetType F?Name ?? Triggers	 P? Property
 P? Value ?? 
TargetName  TreeViewItemFocusVisual ?? StrokeDashArray J? Opacity??%v  &	        &	      &	 #     &	 @     &	 _     &	      &	 ?     7  &	 ?         ?? *CopyCommand+   ?? *
CutCommand+   ?? *PasteCommand+   ?? *DeleteCommand+?? $
 #444???? " d?.???  ? |? .  ??  ? ?? ?? " d?.R?? $??15??$??13??
?????? .+?F? $ 
ExpandPath??$
??Left=?$??Center=?$??1,1,1,1q?8??  "???? 	=     ? z  	= ? 	=  z 	 P? 	 ?@d?	IsChecked$

 TrueY?. ??? $ 
ExpandPath?? ? ??" ??     	= ? z  	= ? 	=  z 	?? .???  ? ?? ?? .R??? ._??? $??0,0,0,0q?$??5??
???   ?$	 1 2R?$ 0Q? J? CommandBindings ,  &ICSStudio.Gui.View.RelayCommandBinding  .  (System.Windows.Input.ApplicationCommands   Copy)  #System.Windows.Input.CommandBinding   Command# Z    RelayCommand# [   
   Cut# Z # [      Paste# Z # [      Delete# Z # [ .???  ? RowDefinitions ?? $
??Auto ??? -    ??Name$ TreeViewRow??$??6* ??? -   $ QuickViewRow??$??4* ?.+?`? $??0?? `? IsLocked . .?c? -   .@???  ? ToolTip$+ %Collapse All Collapse all tree nodes??$	??Top=????? *CollapseAllCommand+.??'  !MaterialDesignThemes.Wpf.PackIcon     Kind$ ArrowCollapseAll=??? -   $5 /Hide\Show Quick ViewToggle the quick view pane??.??  $
 Show=?R? -    R?Name$! ControllerOrganizerTreeView??$??1??/  )ICSStudio.Gui.Controls.TreeViewExtensions   EnableMultiSelect . ???? *ControllerOrganizerItems+   SelectedItems ?? *SelectedItems+.  (System.Windows.Interactivity.Interaction   Triggers /	  )System.Windows.Interactivity.EventTrigger	   	  	EventName$ SelectedItemChanged??.
  (System.Windows.Interactivity.TriggerBase 
  Actions. /  )GalaSoft.MvvmLight.Command.EventToCommand      PassEventArgsToCommand  . !   Command! ?? *SelectedItemChangedCommand+???? " Q?.?-   ??  ?@Q?
IsExpanded ?? *
IsExpanded+" ?? Mode$" TwoWay=???  ?@Q?
IsSelected ?? *
IsSelected+$" TwoWay=???  ?@ NodeType ?? *
NodeType+??  ? ??
 ???? ??  ? ?? ?? # ?? Path$ # HorizontalContentAlignment?$ ?? RelativeSource$ ?? % ?? AncestorType"% ????  ? ?? ?? $# VerticalContentAlignment?$ ?? "% ????  ? ??$ 1,0,0,0q???  ? ??# ? ????  ?@?FocusVisualStyle# [ ??  ? ?? ?? " Q?.R?? & ? ColumnDefinitions& ?? $??19??$
??Auto ??? $
??Auto ??? $??* ? ?? $
??Auto ??? .+?d? ' d?Name$' Expander??#??[ ( ?? 	ClickMode$( Press=?) d? 	IsChecked) ?? $# 
IsExpanded?$ ?? *TemplatedParent+?? $??Bd??$??0??$??1??#??z??#??z??#??z??* ?? Padding#* z??._??? + ??Name$+ PART_Header??$??Header??#??z???? , ??Name$, 	ItemsHost??$??1??$??1??$??2?? P? 	 ?@Q?
IsExpanded
 .  . ??? $ 	ItemsHost?? ? y?$ 	Collapsed=?P? 	 ?@??HasItems
 .  . ??? $ Expander?? ? y?$ Hidden=?]? - ]? 
Conditions- ?? . ?? Property. ? ??/ ?? Value/ .  ?? . ? ??$
/ Auto??./??? $ PART_Header?? ? ??$ 75??]? - ?? . ? ??/ .  ?? . ? ??$
/ Auto??./??? $ PART_Header?? ? ??$ 19??P? 	 ?@Q?
IsSelected
 . ]? - ?? . ?@Q?
IsSelected/ . ?? . ?@Q?IsSelectionActive/ .  P? 	 ? {?
 .  . ??? # ? ?? ? ??0 ?? Triggers0 P? ?  9ICSStudio.Gui.Behaviors.TreeViewMultipleSelectionBehavior	 ?@ IsItemSelected
 . . ???  ? ??# ? ??1 ? ContextMenu1 ?? -   .@?m?   HideControllerOrganizer#???  2 m? Command2 ?? * HideControllerOrganizerCommand+m?  	 CollapseAll#??? 	 2 ?? *CollapseAllCommand+m?  
 	ExpandAll#??? 
 2 ?? *ExpandAllCommand+?? m? -	   3 m?Name$!3 HideOrShowQuickViewMenuItem?????? 4 ?? DataType$4 viewModel:OrganizerItem??5 ?? ItemsSource5 ?? $# ProjectItems?.R?? $??2q? ?? *	ToolTip+& ?? $??16 ??? $??2 ??? $
??Auto ?.+??? 6 ?? BitmapScalingMode$6 HighQuality=?$??16??$??16?????? *ImageSource+?? $??2?????? *DisplayName+?? $??0??7 J? SnapsToDevicePixels7 . y??? *	Inhibited+$" OneWay=?8 ?? 	Converter#8 [  .+?F? 
?????? 
??? ???%????@KL @f׀?[ @f??g5 ?g5 	F? 
???   ????? ?D  ? ? ?D ??? 	F? 
???   ????Р.c  ? Ѡ.c ??? 	?? $??0??7 . y??? *	IsWarning+$" OneWay=?#8 [  .+?F? 
??? ???
??? ???$	??0.2????B? ??D? E? 9 E? IsClosed9 . : E? 
StartPoint$
: 8,16&???@? ?? ; ?? Point$; 16,16&??? $
; 12,8&?F? 
???   ?
???   ?$??1????B? ??D? E? 9 . $: 12.5,11&???@? ?? $; 12.75,11&??? $; 12.75,13&??? $; 12.5,13&?F? 
???   ?
???   ?%????h?? ??? ?h??  [? ??? ?? 	$??1???? $??0??y??? *IsMainRoutine+$" OneWay=?#8 [  < ? LayoutTransform< u? = u? Matrix$= 0.5,0,0,0.5,0,0x?.+?F? 
???   ?> ?? Stretch$> Uniform=??????@T? ??j ???j @T?  ?? ?? ???j @f?@T? @f?@T? @T? ??ا @T? ??ا  $? ?@]?  $? ?@]? @T? ???? @T? ???? @f?@f@f?@f@T? ???? ??j  ?? ?? ?@T? ??j 	?? $??0??y??? *IsFaultRoutine+$" OneWay=?#8 [  < u? $= 0.5,0,0,0.5,0,0x?.+?F? 
??? ???$> Uniform=?j????@T? ??j ?@T? @f??ا @f??ا @]? р?? @]? р?? ?ا ??ا ?ا ??ا @T? ???? @T? ???? ??j ?@T? ??j 	?? $??1??$??5??$??Stretch=?$??Bottom=?? $??2??.+?? y??? *QuickViewVisibility+? ?? UpdateSourceTrigger$? PropertyChanged=? ?? $
??Auto ??? $??* ??? $
??Auto ?.+?R? $??0??@ J? IsHitTestVisible@ .  ???? *ItemTreeViewInfo+y??? *ItemTreeViewInfoVisibility+$? PropertyChanged=????? " Q?.?-
   ???? $!4 viewModel:OrganizerItemInfo??5 ?? $# ProjectItems?.R??? A ?? Orientation$A 
Horizontal=?.+??? 2  ,System.Windows.Markup.XmlAttributePropertiesB  XmlSpaceB preserve???? *Space+  $??2q? ?? *
IconKind+$? PropertyChanged=?y??? *IconVisibility+???? *IconForeground+?? ???? *DisplayName+&   System.Windows.Controls.DataGrid  -   C  Name$C QuickViewDataGrid??$??1??D   AutoGenerateColumnsD .  E   CanUserAddRowsE .  F   HeadersVisibility$
F None=?$G   HorizontalScrollBarVisibility$
G Auto=?"H   VerticalScrollBarVisibility$
H Auto=????? *DataGridInfo+$? PropertyChanged=?I   RowStyleI ?? )  #System.Windows.Controls.DataGridRow"  .???  ? ??$
 Auto??J   ColumnsJ 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumnK   Width5  /System.Windows.Controls.DataGridLengthConverter$	K 150 L   CellTemplateL ?? .R??? ???? *DisplayName+ ?? ???? *DisplayName+  $K * L ?? .R??? $??CharacterEllipsis=????? *Value+ ?? ???? *Value+?? -   M ??Name$M 	ScrollBar??$??2??N ?? Orientation$N 
Horizontal=?O ? Minimum$O 0Q?P ? Maximum$P 1Q?Q ?? ViewportSize$Q 1Q?R ? SmallChange$R 1Q?S ? LargeChange$S 1Q?