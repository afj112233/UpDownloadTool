   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05-clr-namespace:ICSStudio.GuiICSStudio.Gui  5      )    #ICSStudio.Gui.Recover.RecoverDialog   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      +guiclr-namespace:ICSStudio.Gui     5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      V RGalaSoft.MvvmLight.Platform, Version=5.4.1.0, Culture=neutral, PublicKeyToken=null.command http://www.galasoft.ch/mvvmlight  5	        ? Title$   ICSStudio Project Recovery??5
      $	??600??5      $	??320??5       ? 
ResizeMode$ NoResize=?5          ICSStudio.Gui.DialogCloser   DialogResult 5      ?? *DialogResult+ ? 	Resources 5       ?? 
TargetType ?? Property ?? Value ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %?   '??       '?? I     ?? 5      " ??6   .?5      ??  ? ??$	 Top=?66   5      ?? 5      " ??6   .?5      ??  ? ??$	 100??6*   ?? 5       ? ??$ 2q?6+   ?? 5       ? ??$ 23??6+   5      5      .??5!      ? 	 ? RowDefinitions	 5"   
   ?? 5#      $??* ?6   ?? 5$      $??45 ?6   ?? 5%      $??60 ?6   ?? 5&      $??45 ?6   ?? 5'      $??30 ?6   ?? 5(      $??30 ?6   
 ? ColumnDefinitions
 5*   
   ?? 5+      $
??Auto ?6   ?? 5,      .+?5.   
   ?? $??2??6    ?? Orientation$ 
Horizontal=?6)   .+?5/      ?? $??Image/Exclamation.PNG??6   ?? 50      $??Center=?6   $????ICS Studio has detected a recovery file associated with the project being opened.
Choose how to proceed from the following options...??63   51      	? 54   
   $??1??55      $??0??56      $??Recover:??57       	? 	GroupName$ group1??58       d? 	IsChecked 59      ?? *	IsRecover+5:      ?? 5;   
   
 ? Tag$ 1??6   $??1??5<      $??1??5=      $}??wOpen the recovery project file.Choosing this option will attempt to recover unsaved
changes you've made to the project.??5>      .  (System.Windows.Interactivity.Interaction   Triggers 5?      /  )System.Windows.Interactivity.EventTrigger  5@         	EventName$ PreviewMouseDown??6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5A      /  )GalaSoft.MvvmLight.Command.EventToCommand     PassEventArgsToCommand .    Command 6)   ?? *ClickCommand+6J   5B      5D      	? 5F   
   $??2??5G      $??0??5H      $??Save As:??5I      $ group1??5J       5K      ?? *IsSave+?? 5L   
   $ 2??6   $??2??5M      $??1??5N      $????Save the recovery project file to a different project file.Choosing this option allows
you to inspect the recovered project's contents before overwriting the most recently
saved project file.??5O       5P        5Q      $ PreviewMouseDown??6!   . 5R         .  6-   ?? *ClickCommand+6N   5S      5U      	? 5W   
   $??3??5X      $??0??5Y      $??Continue:  ??5Z      $ group1??5[       5\      ?? *
IsContinue+?? 5]   
   $ 3??6   $??3??5^      $??1??5_      $????Ignore the recover project file and open the most recently saved project file.
Choosing this option could result in a loss of work.??5`       5a        5b      $ PreviewMouseDown??6!   . 5c         .  6-   ?? *ClickCommand+6N   5d      5f      ?? 5h   
   $??4??5i      $??1??5j      $#??Discard recovery project file??5k       5l      ?? *	IsDiscard+{?5m      ?? *
IsContinue+?? 5o   
   $??5??5p      $??0??5q      $??2??5r      $??Right=?5s      $ 
Horizontal=?5t      .+?5u      ?? $??OK??63    ?? 	IsDefault . ??6   ?? *	OKCommand+6@   ?? 5v      $??Cancel??67    ?? IsCancel . ??6   ?? *CancelCommand+6H   5w      5x      5y      