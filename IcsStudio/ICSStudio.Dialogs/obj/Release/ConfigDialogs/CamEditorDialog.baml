   M S B A M L   `   `   `  ???? ^  ZICSStudio.Dialogs, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=8558c3dcb301e1d4Q-clr-namespace:ICSStudio.Dialogs.ConfigDialogsICSStudio.Dialogs.ConfigDialogs  \ XICSStudio.Utils, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=20069a8f049d3116fDclr-namespace:ICSStudio.Utils.CamEditorUtil;assembly=ICSStudio.UtilsICSStudio.Utils.CamEditorUtil O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui W0clr-namespace:ICSStudio.Dialogs.ConfigDialogs.UI"ICSStudio.Dialogs.ConfigDialogs.UI  5    /ICSStudio.Dialogs.ConfigDialogs.CamEditorDialog   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ?local-clr-namespace:ICSStudio.Dialogs.ConfigDialogs     R NOxyPlot.Wpf, Version=2.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0 oxyhttp://oxyplot.org/wpf  XcamEditorUtilDclr-namespace:ICSStudio.Utils.CamEditorUtil;assembly=ICSStudio.Utils  1core&clr-namespace:System;assembly=mscorlib  <gui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui  c	 _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity 	 9ui0clr-namespace:ICSStudio.Dialogs.ConfigDialogs.UI 	 V
 RGalaSoft.MvvmLight.Platform, Version=5.4.1.0, Culture=neutral, PublicKeyToken=null.command http://www.galasoft.ch/mvvmlight 
   ? WindowStartupLocation$  CenterOwner=?$	??735??$	??556??$??11(? ? ShowInTaskbar .   ? Title ?? *
CamTitle+   ICSStudio.Gui.DialogCloser   DialogResult ?? *CamDialogResult+ ? 	Resources    SegmentTypes S? 
MethodName S? 
ObjectType  System.Enum S? MethodParameters M? Type/  )ICSStudio.Utils.CamEditorUtil.SegmentType4   .ICSStudio.Dialogs.ConfigDialogs.RowToIndexConv  RowToIndexConv>  8ICSStudio.Dialogs.ConfigDialogs.UI.ConvertStringToDouble  ConvertStringToDoubleB  <ICSStudio.Dialogs.ConfigDialogs.UI.ConvertToSixDecimalPlaces  ConvertToSixDecimalPlaces??%_   &	        &	 (     &	 -     &	 2     S? $ 	GetValues??"   M? "        . 	 (System.Windows.Interactivity.Interaction	   Triggers	 / 	 )System.Windows.Interactivity.EventTrigger  
   	EventName$
 ContentRendered??.	 	 (System.Windows.Interactivity.TriggerBase 	  Actions. 6
 	 0System.Windows.Interactivity.InvokeCommandAction
   
  Command ?? *ContentRenderedCommand+  $
 Closing??. / 
 )GalaSoft.MvvmLight.Command.EventToCommand     PassEventArgsToCommand .    Command ?? *ClosingCommand+.??? ???#F0F0F0 ? RowDefinitions ?? $??47 ??? $??* ??? $??66 ?.+?? $??0?? ?? $??5 ??? $??23 ??? $??12 ??? $??5 ?.+?? $??1?? ? ColumnDefinitions ?? $??8 ??? $??20 ??? $??8 ??? $??22 ??? $??22 ??? $??10 ??? $??20 ??? $??20 ??? $??12 ??? $??22 ??? $??22 ??? $??22 ??? $??22 ??? $??8 ??? $??20 ??? $??20 ??? $??20 ?.+??? $??0??$??1??$??20??$??23??  InsertCamSegments ? ToolTip# ?  $??0q????#F0F0F0???? *InsertCamSegmentsCommand+.???? $??20??$??20??$??CamImages/Insert.png???? $??0??$??3??$??20??$??23??  SetToLinear# ?  $??0q????#F0F0F0$??Black"????? *SetToLinearCommand+.???? $??20??$??20??$ ??CamImages/straightLine.png???? $??0??$??4??$??20??$??23??  
SetToCubic# ?  $??0q????#F0F0F0???? *SetToCubicCommand+.???? $??15??$??15??$??CamImages/Curve.png???? $??0??$??6??$??20??$??23??  UndoCamEdits# ?  $??0q????#F0F0F0???? *UndoCommand+.???? $??18??$??18??$
??Left=?$??CamImages/Undo.png???? $??0??$??7??$??20??$??23??  RedoCamEdits# ?  $??0q????#F0F0F0???? *RedoCommand+.???? $??18??$??18??$??Right=?$??CamImages/Redo.png???? $??0??$??9??$??20??$??23??0   *ICSStudio.Dialogs.ConfigDialogs.SelectMode   Position#??Z  	 SelectSlavePosition# ? 	 $??0q????#F0F0F0$??Black"????? *ChangeSelectModeCommand+.???? $??20??$??20??$??CamImages/Position.png???? $??0??$??10??$??20??$??23??   Velocity#??Z  
 SelectSlaveVelocity# ? 
 $??0q????#F0F0F0$??Black"????? *ChangeSelectModeCommand+.???? $??20??$??20??$??CamImages/Velocity.png???? $??0??$??11??$??20??$??23??   Acceleration#??Z   SelectSlaveAcceleration# ?  $??0q????#F0F0F0$??Black"????? *ChangeSelectModeCommand+.???? $??20??$??20??$ ??CamImages/Acceleration.png???? $??0??$??12??$??20??$??23??   Jerk#??Z   SelectSlaveJerk# ?  $??0q????#F0F0F0$??Black"????? *ChangeSelectModeCommand+.???? $??20??$??20??$??CamImages/Jerk.png???? $??0??$??14??$??20??$??23?? 
 ZoomIn# ?  $??0q????#F0F0F0???? *ZoomInCommand+.???? $??25??$??25??$??CamImages/Big.png???? $??0??$??15??$??20??$??23??  ZoomOut# ?  $??0q????#F0F0F0???? *ZoomOutCommand+.???? $??25??$??25??$??CamImages/small.png???? $??0??$??16??$??20??$??23??  	ZoomToFit# ?  $??0q????#F0F0F0???? *ZoomToFitCommand+.???? $??25??$??25??$??CamImages/Best.png??? $??2?? ?? $	??143 ??? $??20 ??? $??3 ??? $??20 ??? $??3 ??? $??20 ??? $??3 ??? $
??Auto ?.+?? $??1??$??12??$??10??$??2?????? *ColorPosition+ ?? UpdateSourceTrigger$ PropertyChanged=? ?? Mode$ TwoWay=?? $??3??$??12??$??10?????? *ColorVelocity+$ PropertyChanged=?$ TwoWay=?? $??5??$??12??$??10?????? *ColorAcceleration+$ PropertyChanged=?$ TwoWay=?? $??7??$??12??$??10?????? *	ColorJerk+$ PropertyChanged=?$ TwoWay=?? $??1?????#F0F0F0$
??Auto??$
??Auto?? ?? $??12 ??? $??* ??? $??5 ??? $
??Auto ??? $??5 ?.+?? $??0?????#F0F0F0$
??Auto??? $??1??$
??Auto??$
??Auto??.+??? ???#BEC3CB
???????$	??1.5q?._?  OxyPlot.Wpf.PlotView  $
??Auto??$
??Auto??$??10(?  OxyPlot.Wpf.PlotBase   IsMouseWheelEnabled .     Model ?? *	PlotModel+   
Controller ?? *
Controller+ ? ContextMenu ?? $??0??.@?m?  m? Command ?? *InsertCamSegmentsCommand+???? .R???  
 Insert#???  m? $??Linear?? ?? *SetToLinearCommand+???? .R???  
 Linear#???  m? $??Cubic?? ?? *SetToCubicCommand+???? .R???  	 Cubic#???  ?? m? $??Delete?? ?? *DeleteCommand+???? .R???  
 Delete#???  ?? m? $
??Undo?? ?? *UndoCommand+???? .R???   Undo#???  m? $
??Redo?? ?? *RedoCommand+???? .R???   Redo#???  ?? m?  ?? *ZoomInCommand+???? .R??? #???  m?  ?? *ZoomOutCommand+???? .R??? #???  m?  ?? *ZoomToFitCommand+???? .R??? #???  m? {?.  ???? .R???   ScaleToGrid#???  ?? m?  ?? *PropertiesCommand+????   
Properties#???  ?? $??2??$??Stretch=????#D6DBE9|?.  $
??Auto??$??5?? J? SnapsToDevicePixels . ? $??3?????#D6DBE9$
??Auto??$
??Auto?? ?? $??* ??? $??5 ??? $
??Auto ?.+??? $??0?????#BEC3CB$	??1.5q?$
??Auto??$
??Auto??._??? $
??Auto??$
??Auto??$??Visible=?$??Visible=???&   System.Windows.Controls.DataGrid  -     Name$ PointDataGrid??$
??Auto??$
??Auto??
???????   AutoGenerateColumns .      EnableRowVirtualization  .  !   CanUserResizeRows! .  ???? *	CamPoints+"   CanUserAddRows" ?? *
MaxRowsNum+#   
IsReadOnly# ?? *IsReadOnlyEnabled+$ ?? SelectedItem$ ?? *SelectedItem+% ?? SelectedIndex% ?? *SelectedIndex+& J? InputBindings& ?? ' ?? Gesture$' Ctrl+C??
( ?? Key$( C??) ?? Command) ?? *CopyCommand+*   RowStyle* ?? + ?? 
TargetType)  #System.Windows.Controls.DataGridRow"+  , ?? Triggers, P? - P? Property- ?@ 
IsSelected. P? Value. . . ??? / ?? Property/ ? ??0 ?? Value
0 ?   ??? / ? ??
0 ?   ?	   $
 SelectionChanged??. 
   ?? *SelectionChangedCommand+1 
  CommandParameter1 ?? *SelectedItems+2 ?? ElementName$2 PointDataGrid??3   RowHeaderTemplate3 ?? .R??? $??18??4 ?? TextAlignment$4 Center=????? 5 ?? RelativeSource5 ?? 6 ?? AncestorType$6 DataGridRowL?7 ?? 	Converter#7 [ 8   Columns8 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn9   Width5  /System.Windows.Controls.DataGridLengthConverter$9 52 :   CanUserSort: .  1  +System.Windows.Controls.DataGridBoundColumn;   Binding; ?? *Master+#7 [ <   HeaderTemplate< ?? .R???  
 Master#???  =   ElementStyle= ?? "+ ??, ?? > ?? Binding> ?? *IsBadCamPoint+? ?? Value? true.a??? / ? ??
0 ?  ??  $9 51 : .  ; ?? *Slave+#7 [ < ?? .R???  	 Slave#???  4  .System.Windows.Controls.DataGridComboBoxColumn  $9 52 @   ItemsSource@ ?? A ?? Source#A [  B   SelectedItemBindingB ?? *Type+< ?? .R???   Cam Editor Type#???   ?? {?. .@?m? $??Insert?? ?? *InsertCommand+m? $??Linear?? ?? *SetToLinearCommand+m? $??Cubic?? ?? *SetToCubicCommand+?? {?. m? $	??Cut?? ?? *
CutCommand+m? $
??Copy?? ?? *CopyCommand+m? $??Paste?? ?? *PasteCommand+m? $??Delete?? ?? *DeleteCommand+?? {?. m? $
??Undo?? ?? *UndoCommand+m? $
??Redo?? ?? *RedoCommand+?? $??1??$
??Auto??$??5??$??Stretch=????#D6DBE9 . |?.  ?? $??2?????#BEC3CB$
??Auto??$	??1.5q?._?? $
??Auto?? ?? $??* ??? $??* ? ?? $??70 ??? $??* ?.+??? $??0??$??0??  Start Slope#???  $
??Auto??C ?? 
IsReadOnlyC . 
???   ??? $??1??$??0??  	End Slope#???  $
??Auto??C . 
???   ??? $??1??$??0??$
??Auto??$
??Auto??
???   ?C ?? *IsReadOnlyEnabled+???? *
StartSlope+#7 [ ?? $??1??$??1??$
??Auto??$
??Auto??
???   ?C ?? *IsReadOnlyEnabled+???? *
EndSlope+#7 [ ? $??2??$
??Auto?? ?? $??* ??? $	??190 ?.+?? $??0??$	??525??$
??Auto?? ?? $??6 ??? $??13 ??? $??22 ??? $??45 ?.+?? $??1?? ?? $??7 ??? $	??105 ??? $	??105 ??? $	??105 ??? $	??105 ??? .+??? $??1??  Master:#???  ?? $??2??  Cam Editor Position:#???  ?? $??3??  	Velocity:#???  ?? $??4??   Acceleration:#???   ?? $??5?? 	! Jerk:#??? ! ? $??2?? ?? $	??105 ??? $	??105 ??? $	??105 ??? $	??104 ??? $	??105 ?.+??? $??0??$??90??$??17??C . ???? *Master+#7 [ ?? $??1??$??90??$??17??C . ???? *
Position+#7 [ ?? $??2??$??90??$??17??C . ???? *
Velocity+#7 [ ?? $??3??$??90??$??17??C . ???? *Acceleration+#7 [ ?? $??4??$??90??$??17??C . ???? *Jerk+#7 [ ? $??1??$	??190??$??Right=? ?? $??5 ??? ?? ?? $??10 ? ?? ?? ?? $??10 ?.+??? $??1??$??0??$??19??$??73?? " OK#??? " D ?? 	IsDefaultD . ???? *	OKCommand+?? $??1??$??1??$??19??$??73?? 
# Cancel#??? # E ?? IsCancelE . ???? *CancelCommand+?? $??2??$??0??$??19??$??73?? $ Apply(A)#??? $ ???? *ApplyCommand+?? $??2??$??1??$??19??$??73?? % Help#??? % ???? *HelpCommand+