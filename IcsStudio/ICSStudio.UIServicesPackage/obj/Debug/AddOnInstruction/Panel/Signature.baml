   M S B A M L   `   `   `  ????  ICSStudio.UIServicesPackage<    6ICSStudio.UIServicesPackage.AddOnInstruction.Signature   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      {?5   7   ?? *IsAllEnabled+  ? 	Resources  5       ?? MergedDictionaries ?? Source?? 5	   
    5
      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ?  ? RowDefinitions 5      ?? 5      $??30 ?6   6,   ?? 5      $??30 ?6   6,   ?? 5      $
??Auto ?6   6.   ?? 5      $??30 ?6   6,   ?? 5      $
??Auto ?6   6.   ?? 5      $
??Auto ?6   6.    ? ColumnDefinitions 5   
   ?? 5      $
??Auto ?6   6.   ?? 5      $	??270 ?6   6-   ?? 5      6!   .+?5   
   ?? $??0??6   $??0??6!      	Signature#???   61   $
??Bold"?6T   $??5q?6f   6s   ?? 5   
   $??1??6   $??0??6!   $??3??61     Generate A Signature To#???  6E   $??5q?6v   6?   ?? 5    
   $??2??6   $??0??6"   .+?5!      ??   Generate...#???  6   $??5q?6=    ?? 	IsDefault . y?6Y   ?? *GenerateVisibility+??6?   ?? *GenerateCommand+6?   ?? 5"        	Remove...#???  6   $??5q?6;   y?6F   ?? *RemoveVisibility+??6n   ?? *RemoveCommand+6?   ?? 5#        Add To History...#???  6   $??5q?6C   ??6N   ?? *AddToHistoryCommand+6x   5$      ?? 5&   
   $??2??6   $??1??6"   .+?5'      ?? ??6   ?? *ID+6.   ?? 5(      ??6   ?? *	Timestamp+65   5)      ?? 5*   
   $??2??6   $??2??6"   .+?5+      ?? $??40??6   $??20??6      Copy#???  6,   $
??Left=?6M   {?6h   ?? *
CopyEnable+6?   5,      ?? 5-   
   $??3??6   $??0??6!     Signature history#???  61   $
??Bold"?6\   $??5q?6n   6{   &   System.Windows.Controls.DataGrid  5.   
   $??4??6   $??0??6    $??3??60   $	??166??6D      
IsReadOnly .    CanUserAddRows .     AutoGenerateColumns .  ??6c   ?? *HistoryRows+  50      *  $System.Windows.Controls.DataGridCell	 ?? 
TargetType
 ?? Property ?? Value 
 Style1??%?   '        &	 N     ?? 51      "	  6   .?52      ?? 
 ? ?? ?#C8C8C86E   53      ?? 54      "	 ??6   .?55      ?? 
 ? ??$ NoWrap=?65   6F   ?? 56      
 ? ??$ WordEllipsis=?65   6L   57      58         Columns 59      4  .System.Windows.Controls.DataGridTemplateColumn  5:      ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6)      HeaderTemplate 5;      ?? 5<      .R?5=      ??   User#???  6(   5>         CellTemplate 5@      ?? 5A      .R?5B      ?? ??6(   ?? *User+??6>   ??  ?? Source# [  ? ToolTip 5C   "   ?? 5D   &   ??60   ?? *User+6H   5F      5G      5I        5J      $ * 6)    5K      ?? 5L      .R?5M      ??  	 Signature Id#??? 	 6(   5N       5P      ?? 5Q      .R?5R      ?? ??6(   ?? *SignatureID+??6E   ?? # [  5S   "   ?? 5T   &   ??60   ?? *SignatureID+6O   5V      5W      5Y        5Z         Header$ 	Timestamp??6)   $ 2* 6<    5[      ?? 5\      .R?5]      ??  
 	Timestamp#??? 
 6(   5^       5`      ?? 5a      .R?5b      ?? ??6(   ?? *	Timestamp+??6C   ?? # [  5c   "   ?? 5d   &   ??60   ?? *	Timestamp+6M   5f      5g      5i        5j      $ 2* 6)    5k      ?? 5l      .R?5m      ??   Description#???  6(   5n       5p      ?? 5q      .R?5r      ?? ??6(   ?? *Description+??6E   ?? # [  5s   "   ?? 5t   &   ??60   ?? *Description+6O   5v      5w      5y      5|      ?? 5}   
   $??5??5~      $??0??5      $??2??5?        Clear Signature History...#???  5?      $	??150??5?      $
??Left=?5?      $??5q?5?      ??5?      ?? *ClearCommand+5?      5?      5?      