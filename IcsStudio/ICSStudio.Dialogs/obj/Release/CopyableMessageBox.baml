   M S B A M L   `   `   `  ????   ICSStudio.Dialogs*    $ICSStudio.Dialogs.CopyableMessageBox   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008       ? 
ResizeMode$  NoResize=? ? SizeToContent$ WidthAndHeight=? ? ShowInTaskbar .   ? WindowStartupLocation$ CenterOwner=?.??? $??12q? ? RowDefinitions ?? $??* ??? $??12 ??? $
??Auto ?.+??? -    ??Name$ TextBox??$??0?? ?? 
IsReadOnly . ?? $??2?? ?? Orientation$ 
Horizontal=?$??Right=? ? 	Resources    ButtonStyle	 ?? 
TargetType
 ?? Property ?? Value??%^   &	        ?? "	 ??.??? 
 ? ??$ 6,0,0,0q??? 
 ? ??$ 75???? 
 ? ??$ 23??.+?A?  A? Rows$ 1?? A? Columns$ 2??.+??? -   $??OK??#??[   ?? 	IsDefault . ?? $??Cancel??#??[   ?? IsCancel . 