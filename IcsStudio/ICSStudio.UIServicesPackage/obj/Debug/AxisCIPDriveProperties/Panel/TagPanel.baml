   M S B A M L   `   `   `  ????a  ]ICSStudio.Components, Version=1.0.106.22348, Culture=neutral, PublicKeyToken=65b8cea88eb98737kIclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.ComponentsICSStudio.Components.Controls  5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U<clr-namespace:System.Windows.Input;assembly=PresentationCoreSystem.Windows.Input 5       ICSStudio.UIServicesPackageG   AICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel.TagPanel   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ^controlsIclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.Components     5      Ninput<clr-namespace:System.Windows.Input;assembly=PresentationCore     5      {?5	      ?? *	IsEnabled+  ? 	Resources  5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $	??100 ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6    ? ColumnDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $	??300 ?6   .+?5"   
   ?? $??0??6   $??0??6!      New Tag Name:#???   61   6Z   7   1ICSStudio.Components.Controls.ValidateNameControl  5#   
   $??0??6'   $??1??64   $	??2,3q?6D    ?? 	MaxLength$ 40??6Q    ?? IsInputMethodEnabled .  ??5%      ?? *	TagName+ ?? UpdateSourceTrigger$ PropertyChanged=? ?? Delay$ 30??{?5&   '   ?? *IsTagNameEnabled+?? 5(      $??1??6   $??0??6   $	??0,3q?6+   $	??Top=?68     New Tag Description:#???  6P   6?   ?? 5)   
   $??1??6   $??1??6   $	??2,3q?6/   	 ?? TextWrapping$
	 Wrap=?6L   
 ?? AcceptsReturn
 . " ?? VerticalScrollBarVisibility$ Visible=?5*      ??5+      ?? *Description+$ PropertyChanged=?$ 30???? 5-   
   $??2??6   $??0??6!    	 Type:#???  61   6R   ?? 5.   
   $??2??6   $??1??6!     CIPTag Tag Base#???  61   6\   ?? 50   
   $??3??6   $??0??6!     
Data Type:#???  61   6W   ?? 51   
   $??3??6   $??1??6!   AXIS_CIP_DRIVE6A   ?? 52   
   $??4??6   $??0??6!    
 Scope:#???  61   6S   ?? 53   
   $??4??6   $??1??6!   ??61   ?? *Scope+?? 54   
   $??5??6   $??0??6!     ExternalAccess#???  61   6[   ?? 55   
   $??5??6   $??1??6!   ??61   ?? *ExternalAccess+?? 56   
   $??6??6   $??0??6!   63   57      58      