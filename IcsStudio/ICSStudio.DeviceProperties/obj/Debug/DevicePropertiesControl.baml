   M S B A M L   `   `   `  ????  ICSStudio.DeviceProperties8    2ICSStudio.DeviceProperties.DevicePropertiesControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5        ? 	Resources  5
       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $??1* ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5   
   ? $??0??6    ? ColumnDefinitions 5      ?? 5      $	??160 ?6#   ?? 5      $??1* ?6#   .+?5      R? -   5        $??TreeView??5      $??0??5       $??0,0,2,0q?5!      $??Stretch=?5"      ??5#      ?? *OptionPanelNodes+??5$      ?? 5%       ?? 
TargetType" Q?6   .?5&      ??  ?? Property ?@Q?
IsExpanded ?? Value 67   ?? *
IsExpanded+ ?? Mode$ TwoWay=??? 5'       ?@Q?
IsSelected 67   ?? *
IsSelected+$ TwoWay=??? 5(       ? y? 67   ?? *
Visibility+$ OneWay=?5)      ??5+      ?? 5-      	 ?? ItemsSource	 6/   ?? *
Children+.R?5.      ?? $??0,0,0,0q?6%   
 ?? Orientation$
 
Horizontal=?66   .+?5/      ?? ??6(   ?? *Title+50      51      54      ?? 55      -   5        $??OptionPanelScrollViewer??56      $??1??57      $??Disabled=?58      $
??Auto=?59      .??5:      ]? .+?5;      ??  ?? Padding$	 4,0q?5<      $	??Top=?5=      $
??Bold"?5>      ??5?      ?? *OptionPanelTitle+??5@      ?? 5A       ?? 
StartPoint$
 0, 0&?62    ?? EndPoint$
 0, 1&?6D   ??5C   &   ?  ? Offset$	 0.0Q?63    ? Color# ? ??6@   ? 5D   &   $	 1.0Q?63   # ? ??6@   5F      5H      ?? 5I      ??6'   ?? *OptionPanelContent+5J      5K      5M      ?? 5O   
   $??1??6   ]? 5Q   
   $??2??6     5R         ButtonStyle??%|   &	        ?? 5S      " ??6,   .?5T      ??  ? ??$ 4,4,12,4q?6/   ?? 5U       ? ??$ 74??61   5V      5W      .+?5Y      ?? $
 
Horizontal=?6   .+?5Z      ??  
 Status#???  6   ?? 5[      $??: ??6   ?? 5\      ??6   ?? *Status+5]      ?? 5_      $??Right=?5`        Help#???  5b      $??Right=?5c      #??[  5d      ??5a      ?? *HelpCommand+5d      ?? 5e      $??Right=?5f       	 Apply#???  5h      $??Right=?5i      #??[  5j      ??5g      ?? *ApplyCommand+y?5k      ?? *ApplyCommandVisibility+?? 5l      $??Right=?5m       
 Cancel#???  5o      $??Right=?5p       ?? IsCancel . #??[  5r      ??5n      ?? *CancelCommand+5r      ?? 5s      $??Right=?5t        OK#???  5v      $??Right=?5w       ?? 	IsDefault . #??[  5y      ??5u      ?? *	OkCommand+5y      5{      5}      5~      