   M S B A M L   `   `   `  ????  ICSStudio.UIServicesPackageQ    KICSStudio.UIServicesPackage.MotionGroupProperties.Panel.AxisAssignmentPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      V RGalaSoft.MvvmLight.Platform, Version=5.4.1.0, Culture=neutral, PublicKeyToken=null.command http://www.galasoft.ch/mvvmlight  5      :d2http://schemas.microsoft.com/expression/blend/2008  5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5        ? 	Resources  5       ?? MergedDictionaries ?? Source ?? 
TargetType ?? Property ?? Value?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %?   '??       ?? 5      " ??6   .?5      ??  ? ??$ Center=?6?   ?? 5       ? y?$ Hidden=?6/   ?? 5       ? ??$ 0??6+   5      5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $??* ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      $??* ?6   ?? 5       $??* ?6   .+?5#   
   ?? $??0??5$      $??0??5%      $??Center=?5&      $??Center=?5'         Unassigned:#???   5(      5*      ?? 5+   
   $??1??5,      $??0??5-      ??5.      ?? *UnassignedCollection+.  (System.Windows.Interactivity.Interaction   Triggers 5/      /  )System.Windows.Interactivity.EventTrigger  50      	   	EventName$	 MouseDoubleClick??6!   .  (System.Windows.Interactivity.TriggerBase
   Actions.
 51      /  )GalaSoft.MvvmLight.Command.EventToCommand     Command 6-   ?? *#!UnassignedSelectedItemDoubleClick+   CommandParameter 6c   ?? *SelectedItem+ ?? RelativeSource ??  ?? AncestorType" ??52        53      $	 SelectionChanged??6!   .
 54           PassEventArgsToCommand .  6-   ?? *'%UnassignedSelectedItemsChangedCommand+6g   55      ??57      ?? 58      " ??6   .?59      ??  ?@??
IsSelected 63   ??  ?? Path$ 
IsSelected? ?? Mode$ TwoWay=?5:       ?? View 5<      ?? 5=      .P?5>      ??  ?? Width$	 300??5?      $
??Name??5A       ?? DisplayMemberBinding 5@      ?? *
Tag.Name+5A      5B      5D      ?? 5E   
   $??2??5F      $??0??5G      $??Center=?5H      $??Center=?5I        Add Right Arrow#???  5K      ??5J      ?? *
AddCommand+5L      ?? 5N   
   $??0??5O      $??1??5P      $??Center=?5Q      $??Center=?5R        	Assigned:#???  5S      5T      ?? 5U   
   $??1??5V      $??1??5W      ??5X      ?? *AssignedCollection+ 5Y        5Z      $	 MouseDoubleClick??6!   .
 5[         6-   ?? *!AssignedSelectedItemDoubleClick+ 6a   ?? *SelectedItem+ ?? " ??5\        5]      $	 SelectionChanged??6!   .
 5^         .  6-   ?? *SelectedItemsChangedCommand+6]   5_      ??5a      ?? 5b      " ??6   .?5c      ??  ?@??
IsSelected 63   ?? $ 
IsSelected?$ TwoWay=?5d       5f      ?? 5g      .P?5h      ?? $	 300??5i      $
??Name??5k       5j      ?? *
Tag.Name+5k      5l      5n      ?? 5o   
   $??2??5p      $??1??5q      $??Center=?5r      $??Center=?5s        Remove Left Arrow#???  5u      ??5t      ?? *RemoveCommand+5v      5x      5y      