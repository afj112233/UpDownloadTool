   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui  5   	    ICSStudio.UIServicesPackage6   0ICSStudio.UIServicesPackage.View.UploadingDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     5   	   -   5        $	??289??5	   	   $	??146??6     ? ShowInTaskbar  .   ? WindowStyle$
 None=?5
       ? 
ResizeMode$ NoResize=?62    ? WindowStartupLocation$ CenterOwner=?5   	    ? Title$ Downloading??5   	       ICSStudio.Gui.DialogCloser   DialogResult 5   	   ?? *DialogResult+.??5      ?  ? RowDefinitions 5   
   ?? 5      $??2* ?6   ?? 5      $
??Auto ?6   ?? 5      $??* ?6   .+?5   
   ?? $??0??6   $
??Wrap=?6:   $??Center=?5      $??Center=?61   ??5   !   ?? *	Message+5   1   ? 5   
   $??1??6   $??22??6#    ? Value 6/   ?? *
Progress+?? 5   
   $??2??6   $??71??6   $??22??6)   $??Cancel??65    ?? IsCancel . ??5      ?? *CancelCommand+63   5      5      