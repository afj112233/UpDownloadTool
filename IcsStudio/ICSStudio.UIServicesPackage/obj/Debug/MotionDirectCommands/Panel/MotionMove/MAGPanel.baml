   M S B A M L   `   `   `  ????  ICSStudio.UIServicesPackageP    JICSStudio.UIServicesPackage.MotionDirectCommands.Panel.MotionMove.MAGPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      .??5	      ?   ? RowDefinitions  5
   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $??* ?6    ? ColumnDefinitions 5   
   ?? 5      $??84 ?6   ?? 5      $	??250 ?6   ?? 5      $
??Auto ?6   .+?5   
   ?? $??0??6   $??0??6!   Slave Axis:6>   ?? 5   
   $??0??6   $??1??6     ?? DisplayMemberPath$
 Name??5      ??5      ?? *	AllTags+ ?? SelectedItem 63   ?? *SelectedTag+5      ?? 5   
   $??0??6   $??2??6   $??20??6.   $??20??6<   $??20??6K   ??5      ?? *AxisPropertiesCommand+??6;   ?? *SelectedTag+.??6d   ...5      ?? 5   
   $??1??6   $??0??6!   Master Axis:6?   ?? 5    
   $??1??6   $??1??6    $
 Name??5"      ??5!      ?? *	AllTags+ 63   ?? *
MasterAxis+5"      ?? 5#   
   $??1??6   $??2??6   $??20??6.   $??20??6<   $??20??6K   ??5$      ?? *AxisPropertiesCommand+??6;   ?? *
MasterAxis+.??6c   ...5&      1  +Xceed.Wpf.Toolkit.PropertyGrid.PropertyGrid  5)   
   -   5          Name$ PropertyGrid??5)      $??2??62   $??0??6?   $??3??6O      ShowSearchBox .     ShowSortOptions .     	ShowTitle .    ShowSummary .  	   SelectedObject	 5,      ?? *
MAGParam+
   
IsReadOnly
 5-      ?? *IsPropertyGridReadOnly+5.      50      51      