   M S B A M L   `   `   `  ????   ICSStudio.UIServicesPackageQ    KICSStudio.UIServicesPackage.MotionGroupProperties.Panel.AxisAssignmentPanel   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     V RGalaSoft.MvvmLight.Platform, Version=5.4.1.0, Culture=neutral, PublicKeyToken=null.command http://www.galasoft.ch/mvvmlight  :d2http://schemas.microsoft.com/expression/blend/2008  c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006    ? 	Resources   ?? MergedDictionaries ?? Source ?? 
TargetType ?? Property ?? Value??  ?? $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??%`   '??       ?? " ??.???  ? ??$ Center=???  ? y?$ Hidden=???  ? ??$ 0??.???  ? RowDefinitions ?? $
??Auto ??? $??* ??? $
??Auto ? ? ColumnDefinitions ?? $??* ??? $??* ?.+??? $??0??$??0??$??Center=?$??Center=?   Unassigned:#???   ?? $??1??$??0?????? *UnassignedCollection+.  (System.Windows.Interactivity.Interaction   Triggers /  )System.Windows.Interactivity.EventTrigger  	   	EventName$	 MouseDoubleClick??.  (System.Windows.Interactivity.TriggerBase
   Actions.
 /  )GalaSoft.MvvmLight.Command.EventToCommand     Command ?? *#!UnassignedSelectedItemDoubleClick+   CommandParameter ?? *SelectedItem+ ?? RelativeSource ??  ?? AncestorType" ??  $	 SelectionChanged??.
      PassEventArgsToCommand .  ?? *'%UnassignedSelectedItemsChangedCommand+???? " ??.???  ?@??
IsSelected ??  ?? Path$ 
IsSelected? ?? Mode$ TwoWay=? ?? View ?? .P???  ?? Width$	 300??$
??Name?? ?? DisplayMemberBinding ?? *
Tag.Name+?? $??2??$??0??$??Center=?$??Center=?  Add Right Arrow#???  ???? *
AddCommand+?? $??0??$??1??$??Center=?$??Center=?  	Assigned:#???  ?? $??1??$??1?????? *AssignedCollection+   $	 MouseDoubleClick??.
    ?? *!AssignedSelectedItemDoubleClick+ ?? *SelectedItem+ ?? " ??  $	 SelectionChanged??.
    .  ?? *SelectedItemsChangedCommand+???? " ??.???  ?@??
IsSelected ?? $ 
IsSelected?$ TwoWay=? ?? .P??? $	 300??$
??Name?? ?? *
Tag.Name+?? $??2??$??1??$??Center=?$??Center=?  Remove Left Arrow#???  ???? *RemoveCommand+