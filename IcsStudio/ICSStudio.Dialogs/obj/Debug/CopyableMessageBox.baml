   M S B A M L   `   `   `  ????  ICSStudio.Dialogs*    $ICSStudio.Dialogs.CopyableMessageBox   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5        ? 
ResizeMode$  NoResize=?5       ? SizeToContent$ WidthAndHeight=?5	       ? ShowInTaskbar .   ? WindowStartupLocation$ CenterOwner=?5
   $   .??5      ? $??12q?6    ? RowDefinitions 5   
   ?? 5      $??* ?6   ?? 5      $??12 ?6   ?? 5      $
??Auto ?6   .+?5   
   ?? -   5         ??Name$ TextBox??5   1   $??0??6    ?? 
IsReadOnly . 61   ?? 5   
   $??2??6    ?? Orientation$ 
Horizontal=?6"   $??Right=?6;    ? 	Resources 5         ButtonStyle	 ?? 
TargetType
 ?? Property ?? Value??%?   &	        ?? 5      "	 ??6,   .?5      ?? 
 ? ??$ 6,0,0,0q?6/   ?? 5      
 ? ??$ 75??6.   ?? 5      
 ? ??$ 23??6/   5      5      .+?5      A?  A? Rows$ 1??6    A? Columns$ 2??6#   .+?5      ?? -   5        $??OK??5      #??[  5       ?? 	IsDefault . 6O   ?? 5      $??Cancel??6   #??[  5        ?? IsCancel . 6>   5!      5"      5$      5%      