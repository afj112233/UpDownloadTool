   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui  5      h dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761f{Bclr-namespace:ICSStudio.UIServicesPackage.PLCProperties.Panel.Warn4ICSStudio.UIServicesPackage.PLCProperties.Panel.Warn 5      K   EICSStudio.UIServicesPackage.PLCProperties.Panel.Warn.ChangeWarnDialog   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     5      TlocalBclr-namespace:ICSStudio.UIServicesPackage.PLCProperties.Panel.Warn     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5        ? Title$  
ICS Studio??5	      $	??448??5
      $	??256??5       ? 
ResizeMode$ NoResize=?5       ? WindowStartupLocation$ CenterOwner=?5          ICSStudio.Gui.DialogCloser   DialogResult 5      ?? *DialogResult+.??5      ?  ? 	Resources 5   
    ?? 
TargetType ?? Property ?? Value??%?   '??       '?? v     ?? 5      " ??6   .?5      ??  ? ??$ 75??6*   67   ?? 5       ? ??$ 23??6+   68   5      ?? 5      " ??6   .?5      ??  ? ??$ 7q?6+   67   5      5       ? ColumnDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      	 ? RowDefinitions	 5    
   ?? 5!      $
??1.5* ?6   ?? 5"      $
??1.5* ?6   ?? 5#      $
??0.5* ?6   ?? 5$      $??1* ?6   ?? 5%      $
??0.5* ?6   .+?5'   
   ?? $??../Images/AlertError.png??6   $??32??62   $??32??6=   ?? 5)   
   $??0??6   $??1??6!   ??61   ?? *Main+? 5+   	   $??1??6   $??1??6    5,      ?? 5-      $??* ?6"   6.   ?? 5.      $??10.5* ?6"   62   ?? 5/      $
??0.5* ?6"   61   .+?51      ?? $??../Images/AlertError.png??6   $??16??65   $??16??6@   $??0??6L   $	??Top=?6\   6v   ?? 52      $??1??6    ,  (Changing the controller type will change#???   53      $
??Wrap=?6Z   6r   54   
   ?? 56   
   $??2??6   $??1??6!     Change controller type?#???  61   $??Bottom=?6b   6   ?? 58   
   $??3??6   $??1??6"   
 ?? Orientation$
 
Horizontal=?62   .+?59      ??   Yes#???  6   ??65   ?? *	OkCommand+6U   ?? 5:      $??	40,0,40,0q?6     No#???  6(   ??6G   ?? *CancelCommand+6k   ?? 5;        Help#???  6   68   5<      5=      5>      