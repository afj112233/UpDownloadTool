   M S B A M L   `   `   `  ????a  ]ICSStudio.Components, Version=1.0.106.22348, Culture=neutral, PublicKeyToken=65b8cea88eb98737kIclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.ComponentsICSStudio.Components.Controls  5       ICSStudio.UIServicesPackageA   ;ICSStudio.UIServicesPackage.ProgramProperties.Panel.General   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      ^controlsIclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.Components     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      -   5          ? 	Resources  5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      $??10 ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5    
   ?? $??0??5!      $??0??5"      $??2??5#      #??? ??5$      7   1ICSStudio.Components.Controls.ValidateNameControl  5%   
   -   5          Name$ NameBox??5&      $??0??5'      $??2??5(      $	??250??5)      {?5*      ?? *IsOnlineEnabled+??5+      ?? *Name+ ?? UpdateSourceTrigger$ PropertyChanged=??? 5,   
   $??1??5-      $??0??5.      $??2??5/      $	??Top=?50         Description#???   51      ?? 52   
   $??1??53      $??2??54      $	??250??55      $	??100??56       ?? AcceptsReturn .  ?? TextWrapping$
 Wrap=?5:      "	 ?? VerticalScrollBarVisibility$	 Visible=?5;      {?58      ?? *IsOnlineEnabled+??59      ?? *Description+$ PropertyChanged=?5;      ?? 5<   
   $??2??5=      $??0??5>      $??2??5?       
 Parent#???  5@      y?5A      ?? *VisibilityRow34+?? 5B   
   $??2??5C      $??2??5D      ??5E      ?? *Parent+y?5F      ?? *VisibilityRow34+?? 5G   
   $??3??5H      $??0??5I      $??3??5J        UseAsFolder#???  5K      
 d? 	IsChecked
 5L      ?? *UseAsFolder+{?5M      ?? *	IsEnabled+y?5N      ?? *Row3Visibility+?? 5P   
   $??4??5Q      $??0??5R      $??2??5S      $??20,0,0,0q?5T      ??5U      ?? *
Row4Name+y?5V      ?? *VisibilityRow34+? 5W   
   $??4??6   $??2??6    5X      ?? 5Y      $??18 ?6#   ?? 5Z      .+?5\      ?? $??0??5]      $??16??5^      $??16??5_       ?? BitmapScalingMode$ HighQuality=?5`      ??5a      ?? *ImageSource+?? 5b      $??1??5c      ??5d      ?? *
TaskName+y?5e      ?? *VisibilityRow34+5f      5h      5i      