   M S B A M L   `   `   `  ???? Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05=#clr-namespace:ICSStudio.Gui.DialogsICSStudio.Gui.Dialogs  9    3ICSStudio.Gui.Dialogs.AddOnInstructionOptionsDialog   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  /local#clr-namespace:ICSStudio.Gui.Dialogs  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  -   $	??800??$	??450??$	??600??  ? 
ResizeMode$  CanResizeWithGrip=? ? ShowInTaskbar .   ? WindowStartupLocation$ CenterOwner=? ? Title ?? *Title+.  (System.Windows.Interactivity.Interaction   Triggers /  )System.Windows.Interactivity.EventTrigger     	EventName$ Closed??.  (System.Windows.Interactivity.TriggerBase   Actions. 6  0System.Windows.Interactivity.InvokeCommandAction     Command ?? *CloseCommand+ ? 	Resources    ButtonStyle	 ?? 
TargetType
 ?? Property ?? Value ?? MergedDictionaries ?? Source??  ?? $&  ../Resources/styles/Default.xaml??%I   &	        ?? "	 ??.??? 
 ? ??$ 4,4,12,4q??? 
 ? ??$ 74??.??? -   $
??Grid?? ? RowDefinitions ?? $??1* ??? $
??Auto ?.+?? $??1?? ?? ??  ? ColumnDefinitions ?? $
??Auto ??? $??* ?.+??? $??0??$??2??  Copy All Default Values#???  {??? *
IsEnable+ d? 	IsChecked ?? *	IsChecked+?? $??1??$??0??$
??Left=? 	 Logic#???  #??[  ???? *LogicCommand+?? $??1??$??1??$
??Left=????? *
DataSize+?? $??1??$??1??$	??450??$??	0,0,-90,0q?$??Right=? ?? Orientation$ 
Horizontal=?.+??? $??1??$??Right=?  OK#???   ?? 	IsDefault . #??[  ???? *	OkCommand+?? $??1??$??Right=? 
 Cancel#???   ?? IsCancel . #??[  ???? *CancelCommand+?? $??1??$??Right=? 	 Apply#???  #??[  ???? *ApplyCommand+?? $??Right=?  Help#???  #??[  ???? *HelpCommand+