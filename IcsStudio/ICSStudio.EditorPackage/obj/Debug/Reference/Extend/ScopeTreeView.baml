   M S B A M L   `   `   `  ????d  `ICSStudio.EditorPackage, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=262b451c406b2266c6clr-namespace:ICSStudio.EditorPackage.Reference.Extend(ICSStudio.EditorPackage.Reference.Extend  5      ^ ZICSStudio.Dialogs, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=8558c3dcb301e1d4^Aclr-namespace:ICSStudio.Dialogs.Filter;assembly=ICSStudio.DialogsICSStudio.Dialogs.Filter 5      <    6ICSStudio.EditorPackage.Reference.Extend.ScopeTreeView   5      U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Hlocal6clr-namespace:ICSStudio.EditorPackage.Reference.Extend     5      Ufilter1Aclr-namespace:ICSStudio.Dialogs.Filter;assembly=ICSStudio.Dialogs     5        ? 	Resources  5
       ?? MergedDictionaries ?? Source?? 5   
    5   
   ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ?   5   
   2  ,ICSStudio.Dialogs.Filter.LeftMarginConverter   LeftMarginConverter6  0ICSStudio.Dialogs.Filter.GridLeftMarginConverter  GridLeftMarginConverter  ExpandCollapseToggleStyle ?? 
TargetType ?? Property ?? Value ?? 
TargetType J? SnapsToDevicePixels ?? EndPoint	 ?? 
StartPoint
 ? Color ? Offset ??Name ?? Triggers P? Property P? Value ?? 
TargetName4  .ICSStudio.Dialogs.Filter.TreeViewLineConverter  LineConverter ?? 	Converter ?? RelativeSource ?? AncestorType ? ColumnDefinitions ? RowDefinitions d?Name ?? 	ClickMode d? 	IsChecked ??Name ?? CornerRadius ?? Padding ??Name ?? Value ?? Binding ?? Triggers??%k  &	        &	      &	 &     &	 2    'Q? E    7  7  7    5      6H     5      6P   ?? 5      " d?66   .?5      ??  ? |? .  6.   ?? 5       ? ?? 5      ?? 5      " d?6*   .R?5      ? $??15??6#   $??13??6.    . .+?5   "   ?? $??9??6,   $??9??66   ???#919191 . ??5   &   ?? 5    *   $ 0.5,2&?6>   $	 0.5,0&?6O   .??5!   .   ? $
 White??6;   $ 0Q?6I   ? 5"   .   $
 Silver??6;   $	 0.5Q?6J   ? 5#   .   $
 	LightGray??6;   $ 1Q?6M   5$   +   5&   #   ?? 5(   "   $ 
ExpandPath??6,   $??1??6@   $??5??6J   
???   ? . 6d   ?? 5*   "   $??5??6,   $??1??66   
???   ? . 6P   5+       5,      P? 5.   "    ?@d?	IsChecked$
 TrueY?6?   . ?5/   &   ?? $ 
ExpandPath??6C    ? y?$ 	Collapsed=?6[   50   #   52      54      55        58      6,   ?? 5:      " Q?6   .?5;      -   5        ?? 5<       ? ??
 ???? 6/   ?? 5@       ? ??$ Center=?6?   ?? 5A       ? ??$ Center=?6=   ?? 5B       ? ??$ 2q?6,   ?? 5C       ? ?? ?	#FF5656566/   ?? 5D       ? ??$ 12(?6-   ?? 5E       ? ??$ 0q?64   ?? 5F       ? ?? 6-   ?? *Index+6G   ?? 5H       ? ?? 5I      ?? 5J      " Q?6*   .R?5K      ? ??6#   ?? 8    ?? *FindAncestor+$ TreeViewItemL? 5M   "   ?? 5N   &   $??20 ?67   6D   ?? 5O   &   $??19??67   $
??Auto ?6E   ?? 5P   &   $??20??67   $
??Auto ?6E   ?? 5Q   &    5S   "   ?? 5T   &   $
??Auto ?64   $??20??6B   ?? 5U   &   .+?5Z   "   ?? $ HorLn??6,   $??8,0,0,0q?6;   $??1??6L   
???   ?$??1??6f   ?? 5\   "   $ VerLn??6,   $??1??6;   
???   ?$??	-1,-3,0,0q?6T   $??2??6g   $??1??6x   d? 5^   "   $ Expander??6/   $??1??5_   '   $??0??5`   '   $ Press=?5a   '   8??  5b   '   ?? *
IsExpanded+ ?? *TemplatedParent+5c   '   ?? 5d   "   $ Bd??6)   #??z??65   #??z??5e   !   #??z??5f   !   $ 4??5g   !   $
??Left=?5h   !   $??2??5i   !   $??1q?5j   !   # z??5k   !    . ._?5r   &   ?? $	??145??6F   ??60   ?? *Name+6T   5s   #   ?? 5t   "   $ 	ItemsHost??61   $??2??5u   )   $??2??5v   )   $??1??5w   )   5x       5y      P? 5z   "    ?@Q?
IsExpanded .  . ?5{   &   ?? $ 	ItemsHost??6C    ? y?$ 	Collapsed=?6Z   5|   #   P? 5}   "    ?@??HasItems .  . ?5~   &   ?? $ Expander??6C    ? y?$ Hidden=?6Y   5   #   P? 5?   "    ?@??HasItems . . ?5?   &   ??  ? ??$ 	-20,0,0,0q?6?   6S   5?   #   ?? 5?   "   $
 true??6?    6.   ??  ?? *Self+8  .a?5?   &   ?? $ VerLn??6-    ? ??$ 15??6R   ?? 5?   &   $ VerLn??6-    ? ??$	 Top=?6]   5?   #   P? 5?   "    ? ??$ 1??6>   . ?5?   &   ?? $ VerLn??6-    ? ??$ 	-1,12,0,0q?6R   ?? 5?   &   $ VerLn??6-    ? ??$
 Auto??6R   5?   #   5?      5?       5?      P? 5?       ?@%?IsVirtualizing . . ?5?      ??  ? ?? 5?      ?? 5?   "   .R?5?   &   $? 5?   #   5?      5?      5?      5?      .+?5?   
   R? $
??auto??62   ??6   ?? *Scopes+??5?      ?? 5?        ?? ItemsSource  6+   ?? *
Children+6N   5?      5?      5?      