   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05=#clr-namespace:ICSStudio.Gui.DialogsICSStudio.Gui.Dialogs  5      9    3ICSStudio.Gui.Dialogs.AddOnInstructionOptionsDialog   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      /local#clr-namespace:ICSStudio.Gui.Dialogs  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      -   5        $	??800??5
      $	??450??5      $	??600??5        ? 
ResizeMode$  CanResizeWithGrip=?5       ? ShowInTaskbar .   ? WindowStartupLocation$ CenterOwner=?5       ? Title 5	      ?? *Title+.  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ Closed??6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5      6  0System.Windows.Interactivity.InvokeCommandAction     Command 6$   ?? *CloseCommand+5       ? 	Resources 5         ButtonStyle	 ?? 
TargetType
 ?? Property ?? Value ?? MergedDictionaries ?? Source?? 5   
    5!      ?? 5"      $&  ../Resources/styles/Default.xaml??6%   %|   &	        ?? 5      "	 ??6(   .?5      ?? 
 ? ??$ 4,4,12,4q?6+   ?? 5      
 ? ??$ 74??6-   5       5$      .??5&      ? -   5        $
??Grid??5&       ? RowDefinitions 5'   
   ?? 5(      $??1* ?6   ?? 5)      $
??Auto ?6   .+?5+   
   ? $??1??6    5,      ?? 5-      ?? 5.       ? ColumnDefinitions 50      ?? 51      $
??Auto ?6#   ?? 52      $??* ?6#   .+?54      ?? $??0??55      $??2??56        Copy All Default Values#???  57      {?58      ?? *
IsEnable+ d? 	IsChecked 59      ?? *	IsChecked+?? 5:      $??1??5;      $??0??5<      $
??Left=?5=       	 Logic#???  5?      #??[  5@      ??5>      ?? *LogicCommand+5@      ?? 5A      $??1??5B      $??1??5C      $
??Left=?5D      ??5E      ?? *
DataSize+?? 5F      $??1??5G      $??1??5H      $	??450??5I      $??	0,0,-90,0q?5J      $??Right=?5K       ?? Orientation$ 
Horizontal=?5L      .+?5M      ?? $??1??5N      $??Right=?5O        OK#???  5Q       ?? 	IsDefault . #??[  5S      ??5P      ?? *	OkCommand+5S      ?? 5T      $??1??5U      $??Right=?5V       
 Cancel#???  5X       ?? IsCancel . #??[  5Z      ??5W      ?? *CancelCommand+5Z      ?? 5[      $??1??5\      $??Right=?5]       	 Apply#???  5_      #??[  5`      ??5^      ?? *ApplyCommand+5`      ?? 5a      $??Right=?5b        Help#???  5d      #??[  5e      ??5c      ?? *HelpCommand+5e      5f      5h      5i      5j      