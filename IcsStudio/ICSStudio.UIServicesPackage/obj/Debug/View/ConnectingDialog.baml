   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui  5   	    ICSStudio.UIServicesPackage7   1ICSStudio.UIServicesPackage.View.ConnectingDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     5   	   -   5        $	??273??5	   	   $??90??6     ? ShowInTaskbar  .   ? WindowStyle$
 None=?5
       ? 
ResizeMode$ NoResize=?62    ? WindowStartupLocation$ CenterOwner=?5   	       ICSStudio.Gui.DialogCloser   DialogResult 5   	   ?? *DialogResult+.??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5   
   ?? $??0??6   $??Connecting to device...??6!   $??Center=?5      $??Center=?61   $??0 6 0 6q?5      ? 5   
   $??1??6   $??27??6#    ? IsIndeterminate . 6/   ?? 5   
   $??2??6   $??Cancel??6   $??Center=?6/   $??75??5      $??23??6   $??6q?6(    ?? IsCancel . ??5      ?? *CancelCommand+5      5      