   M S B A M L   `   `   `  ????a  ]ICSStudio.Components, Version=1.0.106.22348, Culture=neutral, PublicKeyToken=65b8cea88eb98737M+clr-namespace:ICSStudio.Components.ControlsICSStudio.Components.Controls  5      Q-clr-namespace:ICSStudio.Components.ConvertersICSStudio.Components.Converters  5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  ?? MergedDictionaries ?? Source    BooleanToVisibilityConverter=    7ICSStudio.Components.Converters.BoolToAsteriskConverter  DirtyConverterF   @ICSStudio.Components.Converters.ErrorLevelToImageSourceConverter  ErrorLevelConverter  DefaultGridRowHeight  DefaultCellContentMaxHeight  DataGridRowHeaderWidth  GridLinesBrush ?? Color  ReadOnlyCellBackgroundBrush  EditCellBackgroundBrush 	 WatermarkTextBrush 
 TextBoxTextBrush  RowStyle)  #System.Windows.Controls.DataGridRow ?? BasedOn ?? 
TargetType ?? Property ?? Value  	CellStyle*  $System.Windows.Controls.DataGridCell  RowHeaderStyle:  4System.Windows.Controls.Primitives.DataGridRowHeader ?? 
TargetType ?? Path	 ?? 	Converter  
TagDrawing
 H? Drawing ?? Orientation ?? ConverterParameter  SingleClickEditControlStyle:   4ICSStudio.Components.Controls.SingleClickEditControl  TextBlockStyle  DataGridStyle&   System.Windows.Controls.DataGrid  DataGridCheckBoxStyle?? 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      8controls+clr-namespace:ICSStudio.Components.Controls  5      <
converters-clr-namespace:ICSStudio.Components.Converters  5      1system&clr-namespace:System;assembly=mscorlib  5        5      ?? 5	   
   $ .\ImagesDictionary.xaml??6   %?  &	        &	      &	 &     &	 9     &	 S     &	 m     &	 ?     &	 ?     &	 ?     &		 ?     &	
 ?     &	 	    7 7  &	 V    7 &	 ?    7 7   7  7  7  &	 ?    7  7 
 &	 3    &	 ?    7  7  7  7  7  7  7  &	 Q    ?? 5      6#      5      6)     5      62   X?5      21Q?65   X?5      20Q?6<   X?5      25Q?67   ?? 5      # ? ??6-   ?? 5      # ? ??6:   ?? 5      # ? ??66   ?? 5      # ? ??61   ?? 5      # ? ??6/   ?? 5      8   "  5   	   .?5   
   ??  ? ??8  6'   5      ?? 5       8   "  5#   	   .?5$   
   ??  ? ??$ 0q?6,   5%      ?? 5'      8   "  5*   	   .?5+   
   ??  ? ?? 5,      ?? 5-      "  6"   .R?5.      ? .+?5/      ?? $??13??50      $??13??51      y?52      ?? $ 	IsNewItem?8	  ??53      H? 54   "   8
  6/   56      ?? 57      $??Center=?6%   $ 
Horizontal=?6@   .+?58      ?? $??13??59   !   $??13??5:   !   ??5;   !   ?? *ErrorSeverity+8	  $ 13???? 5<      $??1,0,0,0q?6(   ??69   ?? $ IsDirty?8	  5=      5>      5?      5B      5C      ?? 5E      "  60   .?5F   
   ??  ? ??8   6'   ?? 5G   
    ? ??8  6'   ?? 5H   
    ? ??$ 0q?6,   5I      ?? 5K      " ??6#   .?5L   
   ??  ? ??$ 4,0,4,0q?6#   ?? 5M   
    ? ??$ Center=?6.   ?? 5N   
    ? ??$ CharacterEllipsis=?6)   5O      ?? 5Q      "  6"   .?5R   
   ??  ?@ 	RowHeight8   6&   ?? 5S   
    ?@ RowHeaderWidth8  6+   ?? 5T   
    ?@ SelectionMode$ Extended=?6*   ?? 5U   
    ?@ AutoGenerateColumns .  60   ?? 5V   
    ?@ CanUserAddRows .  6+   ?? 5W   
    ?@ CanUserResizeRows .  6.   ?? 5X   
    ?@ CanUserDeleteRows .  6.   ?? 5Y   
     ?@ HorizontalGridLinesBrush8  65   ?? 5Z   
    ?@ VerticalGridLinesBrush8  63   ?? 5[   
    ?@ RowStyle8  6%   ?? 5\   
    ?@ 	CellStyle8  6&   ?? 5]   
    ?@ RowHeaderStyle8  6+   5^      ?? 5`      " ??6*   .?5a   
   ??  ? ??$ Center=?60   ?? 5b   
    ? ??$ Center=?6.   5c      5e      