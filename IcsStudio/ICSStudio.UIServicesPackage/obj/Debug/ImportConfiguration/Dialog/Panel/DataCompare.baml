   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761f?Jclr-namespace:ICSStudio.UIServicesPackage.ImportConfiguration.Dialog.Panel<ICSStudio.UIServicesPackage.ImportConfiguration.Dialog.Panel  5      N    HICSStudio.UIServicesPackage.ImportConfiguration.Dialog.Panel.DataCompare   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      V RGalaSoft.MvvmLight.Platform, Version=5.4.1.0, Culture=neutral, PublicKeyToken=null.command http://www.galasoft.ch/mvvmlight  5      :d2http://schemas.microsoft.com/expression/blend/2008  5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      VlocalJclr-namespace:ICSStudio.UIServicesPackage.ImportConfiguration.Dialog.Panel  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5	        ? 	Resources  5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5       ? ColumnDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      .+?5   
   ?? $??2??6    L  HImport and Existing data structures are different can not be determined.#???   6(   	? 5    
   -   5         	?Name$ RadioButton1??5!      $??1??5"      $??0??5#        Show Import Data#???  5$       	? 	GroupName$ 1??5%       d? 	IsChecked 5&      ?? *
RadioCheck+ ?? UpdateSourceTrigger$ PropertyChanged=?	? 5'   
   -   5        $ RadioButton2??5(      $??1??5)      $??1??5*        Show Existing Data#???  5+      $ 1??5,      ?? 5.   
   $??2??5/      $??0??50      $??2??51      ??52      ?? *Items+y?53      ?? *ImportVisibility+$ PropertyChanged=?  54      	 ?? 
TargetType
 ?? Property ?? Value??%Y   '??       ?? 55      "	 ??6   .?56      ?? 
 ? ??$ 	0,-2,0,-2q?6/   57      58       ?? View 59      ?? 5:      .P?5;      ??  ?? Width$	 200??6%    ?? HeaderTemplate 5<      ?? 5=      .R?5>   "   ?? #??? ??6,   5?       ?? CellTemplate 5A      ?? 5B      .R?5C   "   ??  ?? Orientation$ 
Horizontal=?6-   .+?5E   &   ?? $??2,0,2,0q?67   ??6H   ?? *	Control+? 5F   &   $??2,0,2,0q?5G   )   
?????? y?5I   )   ?? *CommandVisibility+.  (System.Windows.Interactivity.Interaction   Triggers 5J   *   /  )System.Windows.Interactivity.EventTrigger  5K   .      	EventName$  PreviewMouseLeftButtonDown??6=   .  (System.Windows.Interactivity.TriggerBase   Actions. 5L   2   /  )GalaSoft.MvvmLight.Command.EventToCommand     Command 6I   ?? *	Command+5M   /   .+?5O   *   F? 
?????????5P   .   B? 5Q   2   .??5R   6   E?  E? IsClosed .  E? 
StartPoint$	 0,0&?6Q   ??5T   >   ?  ? Points ?    ? 6N   ? 5U   >    ?    ?  ? 6N   ? 5V   >    ?    ? 6N   5X   7   5Y   3   5[   +   F? 5\   *   
???   ???5]   .   B? 5^   2   .??5_   6   E? $	 0,6&?6A   ??5a   >   ?  ?    ? ??[ 6N   5c   7   5d   3   5f   +   F? 5g   *   
???   ?y?6>   ?? *
Visibility+??5h   .   B? 5i   2   .??5j   6   E? $	 6,0&?6A   ??5l   >   ?  ?   ??[  ? 6N   5n   7   5o   3   5q   +   5r   '   ?? 5s   &   $??2,0,2,0q?60   ??6A   ?? *Name+5t   #   5u      5w      ?? 5x      $	 200??6%    5y      ?? 5z      .R?5{   "   ??  	 Value#???  6,   5|       5~      ?? 5      .R?5?   "   ?? ??6,   ?? *Value+5?      5?      5?      5?      ?? 5?   
   $??2??5?      $??0??5?      $??2??5?      ??5?      ?? *
ExistItems+y?5?      ?? *ExistVisibility+$ PropertyChanged=?  5?      ??%Y   '??       ?? 5?      "	 ??6   .?5?      ?? 
 ? ??$ 	0,-2,0,-2q?6/   5?      5?       5?      ?? 5?      .P?5?      ?? $	 200??6%    5?      ?? 5?      .R?5?   "   ?? #??? ??6,   5?       5?      ?? 5?      .R?5?   "   ?? $ 
Horizontal=?6-   .+?5?   &   ?? $??2,0,2,0q?67   ??6H   ?? *	Control+? 5?   &   $??2,0,2,0q?5?   )   
?????? y?5?   )   ?? *CommandVisibility+ 5?   *     5?   .   $  PreviewMouseLeftButtonDown??6=   . 5?   2      6I   ?? *	Command+5?   /   .+?5?   *   F? 
?????????5?   .   B? 5?   2   .??5?   6   E?  . $	 0,0&?6Q   ??5?   >   ?  ?    ? 6N   ? 5?   >    ?    ?  ? 6N   ? 5?   >    ?    ? 6N   5?   7   5?   3   5?   +   F? 5?   *   
???   ???5?   .   B? 5?   2   .??5?   6   E? $	 0,6&?6A   ??5?   >   ?  ?    ? ??[ 6N   5?   7   5?   3   5?   +   F? 5?   *   
???   ?y?6>   ?? *
Visibility+??5?   .   B? 5?   2   .??5?   6   E? $	 6,0&?6A   ??5?   >   ?  ?   ??[  ? 6N   5?   7   5?   3   5?   +   5?   '   ?? 5?   &   $??2,0,2,0q?60   ??6A   ?? *Name+5?   #   5?      5?      ?? 5?      $	 200??6%   $??Value??61    5?      ?? 5?      .R?5?   "   ?? ??6,   ?? *Value+5?      5?      5?      5?      5?      5?      