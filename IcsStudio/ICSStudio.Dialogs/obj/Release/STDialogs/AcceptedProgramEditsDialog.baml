   M S B A M L   `   `   `  ???? ^  ZICSStudio.Dialogs, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=8558c3dcb301e1d4I)clr-namespace:ICSStudio.Dialogs.STDialogsICSStudio.Dialogs.STDialogs  Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui <    6ICSStudio.Dialogs.STDialogs.AcceptedProgramEditsDialog   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     ;local)clr-namespace:ICSStudio.Dialogs.STDialogs     Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui       ? 
ResizeMode$  NoResize=?$	??250??$	??400?? ? Title$ 
ICS Studio??   ICSStudio.Gui.DialogCloser   DialogResult ?? *DialogResult+ ? 	Resources  ?? MergedDictionaries ?? Source??  ?? $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??%    .???   ?? 
TargetType ?? Property ?? Value??%,   '??       ?? " ??.???  ? ??$ 5q?	 ? ColumnDefinitions	 ?? $
??Auto ??? 
 ? RowDefinitions
 ?? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ?.+??? $??Images/Exclamation.PNG??$??32??$??32??$
??Left=??? $??0??$??1?????? *Line0+?? $??1??$??1??$=??7The following routines (in this program) contain edits:???? $??2??$??1??$??85??$	??330??$
??Left=? ?? SelectionMode$ Single=?$??5q????? *EditedRoutines+ ??%+   '??       ?? " ??.???  ? |? .  ???? .R??? ???? *Name+?? $??3??$??1?????? *Line3+?? $??4??$??1??$	??Yes?? ?? 	IsDefault . $??Center=?$??
-150,5,0,0q????? *
YesCommand+?? $??4??$??1??$??No?? ?? IsCancel . $??Center=?$??	100,5,0,0q????? *	NoCommand+