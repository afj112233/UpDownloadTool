   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fs>clr-namespace:ICSStudio.UIServicesPackage.TaskProperties.panel0ICSStudio.UIServicesPackage.TaskProperties.panel  5      F    @ICSStudio.UIServicesPackage.TaskProperties.panel.ProgramSchedule   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Plocal>clr-namespace:ICSStudio.UIServicesPackage.TaskProperties.panel     5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      V RGalaSoft.MvvmLight.Platform, Version=5.4.1.0, Culture=neutral, PublicKeyToken=null.command http://www.galasoft.ch/mvvmlight  5        ? 	Resources  5       ?? MergedDictionaries ?? Source ?? 
TargetType ?? Property ?? Value?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %?   '??       ?? 5      " ??6   .?5      ??  ? ??$ Center=?6?   ?? 5       ? y?$ Hidden=?6/   ?? 5       ? ??$ 0??6+   5      5      .??5      ?  ? RowDefinitions 5      ?? 5      $
??Auto ?6    60   ?? 5      $
??Auto ?6    60   ?? 5      $??* ?6    6-   ?? 5      $??* ?6    6-   ?? 5      $??* ?6    6-   ?? 5      $
??Auto ?6    60   ?? 5       $
??Auto ?6    60    ? ColumnDefinitions 5"   
   ?? 5#      $??* ?6   6+   ?? 5$      $??* ?6   6+   ?? 5%      $??35 ?6   6,   .+?5'   
   ?? $??0??6   $??0??6!   $
??10,0q?61      Unscheduled:#???   6?   $
??Left=?6e   6?   ?? 5(   
   $??0??6   $??1??6!   $	??5,0q?61     
Scheduled:#???  6>   $
??Left=?6b   6   ?? 5)   
   $??1??6   $??0??6    $??5??60   $??	10,0,5,10q?6A   $	??140??6T   $	??Top=?6a   $	??100??6y   $	??100??6?   ??6?   ?? *UnscheduledList+ ?? SelectedItem 6?   ?? *UnscheduledSelected+??5*      ?? 5+      " ??6   .?5,      ??  ?@??
IsSelected 63   ?? 	 ?? Path$	 
IsSelected?
 ?? Mode$
 TwoWay=?5-      .  (System.Windows.Interactivity.Interaction   Triggers 5/      /  )System.Windows.Interactivity.EventTrigger  50         	EventName$ SelectionChanged??6!   .  (System.Windows.Interactivity.TriggerBase   Actions. 51      /  )GalaSoft.MvvmLight.Command.EventToCommand     PassEventArgsToCommand .    Command 6-   ?? *'%UnassignedSelectedItemsChangedCommand+6g   52       ?? View 54      ?? 55      .P?56      ?? $
??Name??6%    ?? Width$	 150??6a    ?? DisplayMemberBinding 63   ?? *Program.Name+6a   57      59      ?? 5:   
   $??1??6   $??1??6    $??5??60   $??	5,0,10,10q?6B   $	??140??6U   $	??Top=?6b   $	??100??6z   $	??100??6?   ??6?   ?? *ScheduledList+ 6?   ?? *ScheduledSelected+??5;      ?? 5<      " ??6   .?5=      ??  ?@??
IsSelected 63   ?? $	 
IsSelected?$
 TwoWay=?5>       5@        5A      $ SelectionChanged??6!   . 5B         .  6-   ?? *SelectedItemsChangedCommand+6]   5C       5E      ?? 5F      .P?5G      ?? $
??Name??6%   $	 150??6a    63   ?? *Program.Name+6a   5H      5J      ?? 5K   
   $??2??6   $??2??6   $??20??6/   $??20??6=   $??20??6L   $??20??6W   $	??Top=?6c   $
??0,20q?6{   ??6?   ?? *MoveUp+??5L      ?? 5M      .+?5N      F? 
???   ?
 ?? Top$	 -10??6(   ??5O      B? 5P      .??5Q   "   E?  E? 
StartPoint$	 0,0&?6-   .??5R   &   ??  ?? Point$
 -7,7&?62   6A   ?? 5S   &   $	 7,7&?62   6@   5T   #   E? 5U   "   $
 -3,7&?6-   .??5V   &   ?? $ -3,14&?62   6B   ?? 5W   &   $
 3,14&?62   6A   ?? 5X   &   $	 3,7&?62   6@   5Y   #   E? 5Z   "   $ -3,14&?6-   .??5[   &   ?? $ -7,19&?62   6B   ?? 5\   &   $ -2,19&?62   6B   ?? 5]   &   $
 3,14&?62   6A   5^   #   5_      5a      5b      5d      ?? 5e   
   $??2??6   $??2??6!   $??2??62   $??0,0,0,45q?6B     Move#???  6T   $??Center=?6r   $??Center=?6?   $??20??6?   6?   ?? 5f   
   $??3??6   $??2??6   $??20??6/   $??20??6=   $??20??6L   $??20??6W   $	??Top=?6c   ??6{   ?? *
MoveDown+.??5g      ?? .+?5h      F? 
???   ?$	 -10??6$   ??5i      B? 5j      .??5k      E? $
 0,19&?6)   .??5l   "   ?? $
 7,12&?6.   6=   ?? 5m   "   $ -7,12&?6.   6>   5n      E? 5o      $
 3,12&?6)   .??5p   "   ?? $	 3,5&?6.   6<   ?? 5q   "   $
 -3,5&?6.   6=   ?? 5r   "   $ -3,12&?6.   6>   5s      E? 5t      $	 3,5&?6)   .??5u   "   ?? $	 7,0&?6.   6<   ?? 5v   "   $	 2,0&?6.   6<   ?? 5w   "   $
 -3,5&?6.   6=   5x      5y      5{      5|      5}      ?? 5~   
   $??3??6   $??0??6   $??0,0,0,15q?5      $??Center=?5?      $??Bottom=?6.     Add#???  5?   0   ??6   ?? *
AddCommand+5?      ?? 5?   
   $??3??6   $??1??6   $??0,0,0,15q?5?      $??Center=?5?      $??Bottom=?6.    
 Remove#???  5?   3   ??6   ?? *RemoveCommand+5?      5?      5?      