   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui  5   	    ICSStudio.Dialogs-   'ICSStudio.Dialogs.Warning.WarningDialog   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     5   	   $	??180??5
   	   $	??400??5   	   $	??600??5   	      Warning  ? Title#  ?   5   	    ? 
ResizeMode$ NoResize=?5   	    ? SizeToContent$ WidthAndHeight=?5   	    ? ShowInTaskbar .   ? WindowStartupLocation$ CenterOwner=?5          ICSStudio.Gui.DialogCloser   DialogResult 5   	   ?? *DialogResult+.??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $??* ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5   
   ? $??0??6    5      ?? 5      ?? 5       ? ColumnDefinitions 5      ?? 5       $??60 ?6#   ?? 5!      .+?5$      ?? $??0??6   $??0??6!   $??2??61   $??60??5%      $??60??6    ?? 5'      $??0,0,20,0q?6   $
??Auto??6*   $??0??68   $??1??6E   ??6U   ?? *WaringMessage+?? 5(      $??0,0,20,0q?6   $??1??6*   $??1??67   $
??Wrap=?6f   ??6G   ?? *WaringReason+6f   5*      ?? 5,   
   $??2??6    ?? Orientation$ 
Horizontal=?6"   $??Center=?6;   $??Center=?6X   .+?5-      ?? 	 ?? IsCancel	 . $??71??6C   $??20??6N   $??5 0 5 0q?5.      
 ?? 	IsDefault
 .   OK#???  50      ??5-      ?? *	OkCommand+50   6   ?? 51      $??71??65   $??20??6@   $??5 0 5 0q?52        Help#???  53      ??51      ?? *HelpCommand+53   8   54      ?? 56   
   $??3??6   $??5 0 5 0q?6!   $??Center=?57   /   ??6   ?? *	ErrorCode+6/   59      5:      