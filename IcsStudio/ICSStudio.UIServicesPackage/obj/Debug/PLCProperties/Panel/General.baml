   M S B A M L   `   `   `  ????a  ]ICSStudio.Components, Version=1.0.106.22348, Culture=neutral, PublicKeyToken=65b8cea88eb98737kIclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.ComponentsICSStudio.Components.Controls  5       ICSStudio.UIServicesPackage=   7ICSStudio.UIServicesPackage.PLCProperties.Panel.General   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ^controlsIclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.Components     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      -   5          ? 	Resources  5       ?? MergedDictionaries ?? Source ?? 
TargetType ?? Property ?? Value?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %  '??       '?? N     '?? ?     ?? 5      " ??6   .?5      ??  ? ??$ 5,5,25,5q?6+   5      ?? 5      " ??6   .?5      ??  ? ??$ 5q?6+   5      ?? 5      " ??6   .?5      ??  ? ??$ 5q?6+   ?? 5       ? ??$ 22??6+   5      5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5       $
??Auto ?6   ?? 5!      $
??Auto ?6   ?? 5"      $
??Auto ?6   ?? 5#      $
??Auto ?6    ? ColumnDefinitions 5%   
   ?? 5&      $
??Auto ?6   ?? 5'      ?? 5(      $
??Auto ?6   .+?5*   
   ?? $??0??5+      $??0??5,      $??5q?5-         Vendor:#???   5.      ?? 5/   
   $??0??50      $??1??51      $??5q?52      ??53      ?? *Vendor+?? 55   
   $??1??56      $??0??57      $??5q?58      $??Center=?59       	 Type:#???  5:      ?? 5;   
   $??1??5<      $??1??5=      $??5q?5>      $??Center=?5?      ??5@      ?? *Type+?? 5A   
   $??1??5B      $??2??5C      $??20??5D      $??Center=?5E        Change Controller#???  5G      ??5F      ?? *ChangeCommand+5G      ?? 5I   
   $??2??5J      $??0??5K        	Revision:#???  5L      ?? 5M   
   $??2??5N      $??1??5O      $??32??5P      ?? 5R   
   $??3??5S      $??0??5T      $??Center=?5U       	 Name:#???  5V      7   1ICSStudio.Components.Controls.ValidateNameControl  5W   
   -   5          Name$ NameBox??5X      $??3??5Y      $??1??5Z      $??19??5[      $??5q?5\      {?5]      ?? *
IsEnable+??5^      ?? *Name+	 ?? UpdateSourceTrigger$	 PropertyChanged=??? 5`   
   $??4??5a      $??0??5b      $??	5,10,25,5q?5c        Description:#???  5d      ?? 5e   
   $??4??5f      $??1??5g      $??80??5h      
 ?? AcceptsReturn
 .  ?? TextWrapping$ WrapWithOverflow=?5l      " ?? VerticalScrollBarVisibility$ Visible=?5m      {?5j      ?? *
IsEnable+??5k      ?? *Description+$	 PropertyChanged=?5m      5n      5o      