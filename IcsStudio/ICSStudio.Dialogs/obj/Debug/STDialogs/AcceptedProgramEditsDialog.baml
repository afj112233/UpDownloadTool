   M S B A M L   `   `   `  ????^  ZICSStudio.Dialogs, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=8558c3dcb301e1d4I)clr-namespace:ICSStudio.Dialogs.STDialogsICSStudio.Dialogs.STDialogs  5      Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui 5      <    6ICSStudio.Dialogs.STDialogs.AcceptedProgramEditsDialog   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:ICSStudio.Dialogs.STDialogs     5      Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     5        ? 
ResizeMode$  NoResize=?5	   7   $	??250??5      $	??400??6    ? Title$ 
ICS Studio??6'      ICSStudio.Gui.DialogCloser   DialogResult 5
      ?? *DialogResult+ ? 	Resources 5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ?  5   
    ?? 
TargetType ?? Property ?? Value??%V   '??       ?? 5      " ??6   .?5      ??  ? ??$ 5q?6+   67   5      5      	 ? ColumnDefinitions	 5   
   ?? 5      $
??Auto ?6#   62   ?? 5      6%   
 ? RowDefinitions
 5   
   ?? 5      $
??Auto ?6   6,   ?? 5       $
??Auto ?6   6,   ?? 5!      $
??Auto ?6   6,   ?? 5"      $
??Auto ?6   6,   ?? 5#      $
??Auto ?6   6,   .+?5%   
   ?? $??Images/Exclamation.PNG??6   $??32??60   $??32??6;   $
??Left=?6G   6d   ?? 5&   
   $??0??6   $??1??6!   ??61   ?? *Line0+6J   ?? 5'   
   $??1??6   $??1??6!   $=??7The following routines (in this program) contain edits:??61   6r   ?? 5(   
   $??2??6   $??1??6   $??85??6/   $	??330??6;   $
??Left=?6G    ?? SelectionMode$ Single=?5)      $??5q?6)   ??5(   b   ?? *EditedRoutines+ 5*      ??%P   '??       ?? 5+      " ??6   .?5,      ??  ? |? .  6B   5-      5.      ??5/      ?? 50      .R?51      ?? ??6    ?? *Name+68   52      54      ?? 55   
   $??3??6   $??1??6!   ??61   ?? *Line3+6J   ?? 56   
   $??4??6   $??1??6   $	??Yes??6.    ?? 	IsDefault . $??Center=?6M   $??
-150,5,0,0q?6j   ??6~   ?? *
YesCommand+6?   ?? 57   
   $??4??6   $??1??6   $??No??6.    ?? IsCancel . $??Center=?6K   $??	100,5,0,0q?6h   ??6{   ?? *	NoCommand+6?   58      59      