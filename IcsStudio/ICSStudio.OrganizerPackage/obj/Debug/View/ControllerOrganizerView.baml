   M S B A M L   `   `   `  ????g  cICSStudio.OrganizerPackage, Version=1.0.127.22348, Culture=neutral, PublicKeyToken=b1f75cc66356959e[2clr-namespace:ICSStudio.OrganizerPackage.ViewModel$ICSStudio.OrganizerPackage.ViewModel  5      Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05Z=clr-namespace:ICSStudio.Gui.Converters;assembly=ICSStudio.GuiICSStudio.Gui.Converters 5	      N7clr-namespace:ICSStudio.Gui.View;assembly=ICSStudio.GuiICSStudio.Gui.View 5
      X<clr-namespace:ICSStudio.Gui.Behaviors;assembly=ICSStudio.GuiICSStudio.Gui.Behaviors 5      V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls 5      =    7ICSStudio.OrganizerPackage.View.ControllerOrganizerView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      V RMaterialDesignThemes.Wpf, Version=3.1.3.1513, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      B	viewModel2clr-namespace:ICSStudio.OrganizerPackage.ViewModel  5      N
converters=clr-namespace:ICSStudio.Gui.Converters;assembly=ICSStudio.Gui  5	      Bview7clr-namespace:ICSStudio.Gui.View;assembly=ICSStudio.Gui  5
      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      L	behaviors<clr-namespace:ICSStudio.Gui.Behaviors;assembly=ICSStudio.Gui  5      Eext;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui  5      V RGalaSoft.MvvmLight.Platform, Version=5.4.1.0, Culture=neutral, PublicKeyToken=null.command http://www.galasoft.ch/mvvmlight  5      -   5          ? 	Resources  5      1  +ICSStudio.Gui.Converters.IsVisibleConverter   IsVisibleConverter(  "ICSStudio.Gui.View.BindableCommand  CopyCommand   Command  
CutCommand  PasteCommand  DeleteCommand  
GlyphBrush ?? Color  ExpandCollapseToggleStyle ?? 
TargetType ?? Property ?? Value ?? 
TargetType F?Name ?? Triggers	 P? Property
 P? Value ?? 
TargetName  TreeViewItemFocusVisual ?? StrokeDashArray J? Opacity??%   &	        &	      &	 ?     &	 j     &	 ?     &	 ?     &	 ?     7  &	 ?      5   
   6(     5   
    63   ?? *CopyCommand+  5   
    62   ?? *
CutCommand+  5   
    64   ?? *PasteCommand+  5   
    65   ?? *DeleteCommand+?? 5   
   $
 #444??6-   ?? 5   
   " d?62   .?5      ??  ? |? .  6*   ?? 5       ? ?? 5       ?? 5!      " d?6&   .R?5"      ? $??15??6   $??13??6*   
?????? .+?5#      F? $ 
ExpandPath??6#   $
??Left=?67   $??Center=?6S   $??1,1,1,1q?6o   8??  "???? 	=     ? z  	= ? 	=  z 	6?   5$       5%      P? 5&      	 ?@d?	IsChecked$

 TrueY?6;   . ?5'   "   ?? $ 
ExpandPath??69    ? ??" ??     	= ? z  	= ? 	=  z 	6Q   5(      5*      5,      5-      ?? 5.   
   .?5/      ??  ? ?? 50      ?? 51      .R?52      ?? ._?53      ?? $??0,0,0,0q?6(   $??5??69   
???   ?$	 1 2R?6\   $ 0Q?6r   54      55      57      58      59       J? CommandBindings 5;      ,  &ICSStudio.Gui.View.RelayCommandBinding  5<   
   .  (System.Windows.Input.ApplicationCommands   Copy)  #System.Windows.Input.CommandBinding   Command# Z 5=         RelayCommand# [ 5>        5?   
   
   Cut# Z 5@      # [ 5A        5B   
      Paste# Z 5C      # [ 5D        5E   
      Delete# Z 5F      # [ 5G      .??5J      ?  ? RowDefinitions 5K   
   ?? 5M      $
??Auto ?6   ?? 5N      -   5         ??Name$ TreeViewRow??5N   (   $??6* ?6   6(   ?? 5O      -   5        $ QuickViewRow??5O   (   $??4* ?6   6(   .+?5S   
   `? $??0??6    `? IsLocked . .?5T      c? -   5        .@?5U      ??  ? ToolTip$+ %Collapse All Collapse all tree nodes??6   $	??Top=?6t   ??6M   ?? *CollapseAllCommand+.??5V      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$ ArrowCollapseAll=?6.   5W      ?? 5X      -   5        $5 /Hide\Show Quick ViewToggle the quick view pane??5X      .??5Y        $
 Show=?6.   5Z      5[      5\      R? 5_   
   -   5         R?Name$! ControllerOrganizerTreeView??5`      $??1??5a      /  )ICSStudio.Gui.Controls.TreeViewExtensions   EnableMultiSelect . ??5b      ?? *ControllerOrganizerItems+   SelectedItems 5c   =   ?? *SelectedItems+.  (System.Windows.Interactivity.Interaction   Triggers 5g      /	  )System.Windows.Interactivity.EventTrigger	  5h       	  	EventName$ SelectedItemChanged??6!   .
  (System.Windows.Interactivity.TriggerBase 
  Actions. 5i      /  )GalaSoft.MvvmLight.Command.EventToCommand      PassEventArgsToCommand  . !   Command! 6-   ?? *SelectedItemChangedCommand+6\   5j      ??5l      ?? 5m      " Q?6   .?5n      -   5        ?? 5s       ?@Q?
IsExpanded 63   ?? *
IsExpanded+" ?? Mode$" TwoWay=??? 5t       ?@Q?
IsSelected 63   ?? *
IsSelected+$" TwoWay=??? 5w       ?@ NodeType 6H   ?? *
NodeType+?? 5x       ? ??
 ???? 63   ?? 5y       ? ?? 6C   ?? # ?? Path$ # HorizontalContentAlignment?$ ?? RelativeSource$ ?? % ?? AncestorType"% ???? 5z       ? ?? 6A   ?? $# VerticalContentAlignment?$ ?? "% ???? 5{       ? ??$ 1,0,0,0q?60   ?? 5|       ? ??# ? ??63   ?? 5}       ?@?FocusVisualStyle# [ 69   ?? 5~       ? ?? 5      ?? 5?      " Q?6.   .R?5?   "   ? & ? ColumnDefinitions& 5?   &   ?? 5?   *   $??19??6;   $
??Auto ?6I   ?? 5?   *   $
??Auto ?6;   ?? 5?   *   $??* ?6;    5?   &   ?? 5?   *   $
??Auto ?68   ?? 5?   *   .+?5?   &   d? ' d?Name$' Expander??63   #??[ 6E   ( ?? 	ClickMode$( Press=?6?   ) d? 	IsChecked) 6x   ?? $# 
IsExpanded?$ ?? *TemplatedParent+6?   ?? 5?   &   $??Bd??6-   $??0??67   $??1??6D   #??z??6T   #??z??6~   #??z??6?   * ?? Padding#* z??6?   ._?5?   *   ?? + ??Name$+ PART_Header??6;   $??Header??6P   #??z??6g   5?   '   ?? 5?   &   , ??Name$, 	ItemsHost??65   $??1??6H   $??1??6U   $??2??6e   5?   #    5?   "   P? 5?   &   	 ?@Q?
IsExpanded
 .  . ?5?   *   ?? $ 	ItemsHost??61    ? y?$ 	Collapsed=?6^   5?   '   P? 5?   &   	 ?@??HasItems
 .  . ?5?   *   ?? $ Expander??61    ? y?$ Hidden=?6]   5?   '   ]? 5?   &   - ]? 
Conditions- 5?   *   ?? 5?   .   . ?? Property. ? ??/ ?? Value/ .  6M   ?? 5?   .   . ? ??$
/ Auto??6I   ./?5?   *   ?? $ PART_Header??61    ? ??$ 75??6^   5?   '   ]? 5?   &   - 5?   *   ?? 5?   .   . ? ??/ .  6M   ?? 5?   .   . ? ??$
/ Auto??6J   ./?5?   *   ?? $ PART_Header??61    ? ??$ 19??6_   5?   '   P? 5?   &   	 ?@Q?
IsSelected
 . 5?   '   ]? 5?   &   - 5?   *   ?? 5?   .   . ?@Q?
IsSelected/ . 6N   ?? 5?   .   . ?@Q?IsSelectionActive/ .  6U   5?   '   P? 5?   &   	 ? {?
 .  . ?5?   *   ?? # ? ??6G    ? ??5?   '   5?      5?      0 ?? Triggers0 5?      P? 5?      ?  9ICSStudio.Gui.Behaviors.TreeViewMultipleSelectionBehavior	 ?@ IsItemSelected
 . . ?5?      ??  ? ??# ? ??6;   5?      5?      1 ? ContextMenu1 5?      ?? 5?      -   5        .@?5?      m?   HideControllerOrganizer#???  6   2 m? Command2 6R   ?? * HideControllerOrganizerCommand+m? 5?       	 CollapseAll#??? 	 6   2 6F   ?? *CollapseAllCommand+m? 5?       
 	ExpandAll#??? 
 6   2 6D   ?? *ExpandAllCommand+?? 5?      m? 5?      -	   5        3 m?Name$!3 HideOrShowQuickViewMenuItem??5?      5?      5?      ??5?      ?? 5?      4 ?? DataType$4 viewModel:OrganizerItem??6+   5 ?? ItemsSource5 6N   ?? $# ProjectItems?.R?5?      ? $??2q?6    6&   ?? *	ToolTip+& 5?      ?? 5?      $??16 ?6/   ?? 5?      $??2 ?6/   ?? 5?      $
??Auto ?6/   .+?5?      ?? 6 ?? BitmapScalingMode$6 HighQuality=?5?       $??16??6N   $??16??6Y   ??5?       ?? *ImageSource+5?   Y   ?? 5?      $??2??6$   ??64   ?? *DisplayName+?? 5?      $??0??6!   7 J? SnapsToDevicePixels7 . y?5?   !   ?? *	Inhibited+$" OneWay=?8 ?? 	Converter#8 [  .+?5?      F? 
?????? 
??? ???%????@KL @f׀?[ @f??g5 ?g5 	5?   #   F? 5?      
???   ????? ?D  ? ? ?D ??? 	62   F? 5?      
???   ????Р.c  ? Ѡ.c ??? 	62   5?      ?? 5?      $??0??6!   7 . y?5?   !   ?? *	IsWarning+$" OneWay=?#8 [  .+?5?      F? 
??? ???
??? ???$	??0.2??6?   ??5?   "   B? 5?   &   ??5?   .   D? E? 5?   2   9 E? IsClosed9 . : E? 
StartPoint$
: 8,16&?6M   ??5?   :   @? ?? 5?   >   ; ?? Point$; 16,16&?6J   ?? 5?   >   $
; 12,8&?6J   5?   ;   5?   3   5?   /   5?   '   5?      F? 5?      
???   ?
???   ?$??1??6?   ??5   "   B? 5  &   ??5  .   D? E? 5  2   9 . $: 12.5,11&?6M   ??5  :   @? ?? 5  >   $; 12.75,11&?6J   ?? 5  >   $; 12.75,13&?6J   ?? 5	  >   $; 12.5,13&?6J   5
  ;   5  3   5  /   5  '   5     F? 5     
???   ?
???   ?%????h?? ??? ?h??  [? ??? ?? 	$??1??6r   5     5     ?? 5     $??0??6!   y?5  !   ?? *IsMainRoutine+$" OneWay=?#8 [  < ? LayoutTransform< 5     u? 5  "   = u? Matrix$= 0.5,0,0,0.5,0,0x?62   .+?5     F? 
???   ?> ?? Stretch$> Uniform=?60   ?????@T? ??j ???j @T?  ?? ?? ???j @f?@T? @f?@T? @T? ??ا @T? ??ا  $? ?@]?  $? ?@]? @T? ???? @T? ???? @f?@f@f?@f@T? ???? ??j  ?? ?? ?@T? ??j 	6B   5     ?? 5     $??0??6!   y?5  !   ?? *IsFaultRoutine+$" OneWay=?#8 [  < 5     u? 5   "   $= 0.5,0,0,0.5,0,0x?62   .+?5"     F? 
??? ???$> Uniform=?61   j????@T? ??j ?@T? @f??ا @f??ا @]? р?? @]? р?? ?ا ??ا ?ا ??ا @T? ???? @T? ???? ??j ?@T? ??j 	6C   5#     5%     5&     5(     ?? 5*  
   $??1??5+     $??5??5,     $??Stretch=?5-     $??Bottom=?5.     ? 50  
   $??2??6   .+?51     ? y?6   ?? *QuickViewVisibility+? ?? UpdateSourceTrigger$? PropertyChanged=? 52     ?? 53     $
??Auto ?6$   ?? 54     $??* ?6$   ?? 55     $
??Auto ?6$   .+?57     R? $??0??58     @ J? IsHitTestVisible@ .  ??5:     ?? *ItemTreeViewInfo+y?5;     ?? *ItemTreeViewInfoVisibility+$? PropertyChanged=???5<     ?? 5=     " Q?6    .?5>     -
   5        5@     ??5B     ?? 5C     $!4 viewModel:OrganizerItemInfo??63   5 5D  3   ?? $# ProjectItems?.R?5E     ?? A ?? Orientation$A 
Horizontal=?6)   .+?5F  "   ?? 2  ,System.Windows.Markup.XmlAttributePropertiesB  XmlSpaceB preserve6A   ???? *Space+  5G  "   $??2q?5H  %    5I  %   ?? *
IconKind+$? PropertyChanged=?y?5J  %   ?? *IconVisibility+??5K  %   ?? *IconForeground+?? 5L  "   ??6,   ?? *DisplayName+5M     5N     5P     &   System.Windows.Controls.DataGrid  5R     -   5        C  Name$C QuickViewDataGrid??5R  (   $??1??6   D   AutoGenerateColumnsD .  E   CanUserAddRowsE .  F   HeadersVisibility$
F None=?5U     $G   HorizontalScrollBarVisibility$
G Auto=?5V     "H   VerticalScrollBarVisibility$
H Auto=?5W     ??5X     ?? *DataGridInfo+$? PropertyChanged=?I   RowStyleI 5Y     ?? 5Z     )  #System.Windows.Controls.DataGridRow"  6    .?5[     ??  ? ??$
 Auto??67   5\     J   ColumnsJ 5^     4  .System.Windows.Controls.DataGridTemplateColumn  5_     ,  &System.Windows.Controls.DataGridColumnK   Width5  /System.Windows.Controls.DataGridLengthConverter$	K 150 61   L   CellTemplateL 5`     ?? 5a  "   .R?5b  &   ?? ??60   ?? *DisplayName+ 5c  *   ?? 5d  .   ??68   ?? *DisplayName+5f  '   5g  #   5i       5j     $K * 61   L 5k     ?? 5l  "   .R?5m  &   ?? $??CharacterEllipsis=?6G   ??60   ?? *Value+ 5n  *   ?? 5o  .   ??68   ?? *Value+6Q   5q  '   5r  #   5t     5v     ?? 5w     -   5        M ??Name$M 	ScrollBar??5w  )   $??2??6   N ?? Orientation$N 
Horizontal=?5x     O ? Minimum$O 0Q?5y     P ? Maximum$P 1Q?6(   Q ?? ViewportSize$Q 1Q?64   R ? SmallChange$R 1Q?5z     S ? LargeChange$S 1Q?6,   6<   5{     5|     5}     5~     