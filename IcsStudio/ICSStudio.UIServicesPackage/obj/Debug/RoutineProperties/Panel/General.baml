   M S B A M L   `   `   `  ????a  ]ICSStudio.Components, Version=1.0.106.22348, Culture=neutral, PublicKeyToken=65b8cea88eb98737kIclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.ComponentsICSStudio.Components.Controls  5       ICSStudio.UIServicesPackageA   ;ICSStudio.UIServicesPackage.RoutineProperties.Panel.General   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ^controlsIclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.Components     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      -   5        $	??380??5      $	??250??5	      {?5      ?? *IsOnlineEnabled+  ? 	Resources  5       ?? 
TargetType ?? Property ?? Value ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %R   '??       ?? 5      " ??6   .?5      ??  ? ??$ 30 ?6+   5      5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5!   
   ?? 5"      $
??Auto ?6   ?? 5#      $
??Auto ?6   .+?5%   
   ?? $??0??5&      $??0??5'      #??? ??5(      7   1ICSStudio.Components.Controls.ValidateNameControl  5)   
   -   5          Name$ NameBox1??5*      $??0??5+      $??1??5,      $	??190??5-      $??20??5.      {?5/      ?? *NameIsEnabled+??50      ?? *Name+	 ?? UpdateSourceTrigger$	 PropertyChanged=?y?51      ?? *
Visible1+  52   
   -   5        $ NameBox2??53      $??0??54      $??1??55      {?56      ?? *NameIsEnabled+??57      ?? *Name+y?58      ?? *
Visible2+?? 59   
   $??1??5:      $??0??5;      $	??Top=?5<         Description#???   5=      ?? 5>   
   -   5        
 ??Name$
 Description??5?      $??1??5@      $??1??5A      $	??190??5B      $??80??5C       ?? AcceptsReturn .  ?? TextWrapping$
 Wrap=?5G      " ?? VerticalScrollBarVisibility$ Visible=?5H      {?5E      ?? *	IsEnabled+??5F      ?? *Description+$	 PropertyChanged=?5H      ?? 5I   
   $??2??5J      $??0??5K        Type#???  5L      ?? 5M   
   $??2??5N      $??1??5O      ??5P      ?? *Type+?? 5Q   
   $??3??5R      $??0??5S      ??5T      ?? *
Row4Name+?? 5U   
   $??3??5V      $??1??5W      ??5X      ?? *	InProgram+?? 5Y   
   $??4??5Z      $??0??5[      ??5\      ?? *
Row5Name+?? 5]   
   $??4??5^      $??1??5_      ??5`      ?? *NumberOfRungs+5a      5b      