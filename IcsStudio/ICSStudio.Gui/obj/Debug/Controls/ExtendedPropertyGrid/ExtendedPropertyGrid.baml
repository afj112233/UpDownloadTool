   M S B A M L   `   `   `  ????O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5
      Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05i9clr-namespace:ICSStudio.Gui.Controls.ExtendedPropertyGrid+ICSStudio.Gui.Controls.ExtendedPropertyGrid 5      C&clr-namespace:ICSStudio.Gui.ConvertersICSStudio.Gui.Converters 5      F   @ICSStudio.Gui.Controls.ExtendedPropertyGrid.ExtendedPropertyGrid   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      T PImagin.Common, Version=3.1.0.0, Culture=neutral, PublicKeyToken=fd3ef6834d7c49b9-common http://imagin.tech/imagin/common  5	      3System&clr-namespace:System;assembly=mscorlib  5
      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      TextendedPropertyGrid9clr-namespace:ICSStudio.Gui.Controls.ExtendedPropertyGrid  5      7
converters&clr-namespace:ICSStudio.Gui.Converters  5      -   5          >?Name$  PART_ExtendedPropertyGrid??5       ? 	Resources 5         Brush.Image ?? Color%  Imagin.Common.Data.BindingProxy  BindingProxy   Data ?? RelativeSource ?? AncestorTypeF  @ICSStudio.Gui.Controls.ExtendedPropertyGrid.ExtendedPropertyGrid;  5Imagin.Common.Data.Converters.InverseBooleanConverter  InverseBooleanConverterM  GImagin.Common.Data.Converters.StartsWithToVisibilityMultiValueConverter - )StartsWithToVisibilityMultiValueConverter>  8Imagin.Common.Data.Converters.StringToCamelCaseConverter  StringToCamelCaseConverter<  6Imagin.Common.Data.Converters.EnumToCamelCaseConverter  EnumToCamelCaseConverter1  +ICSStudio.Gui.Converters.IsChangedConverter  IsChangedConverter9  3ICSStudio.Gui.Converters.EnumToDisplayNameConverter  EnumToDisplayNameConverter7	  1ICSStudio.Gui.Converters.UInt16ToIntegerConverter  UInt16ToIntegerConverter4
  .ICSStudio.Gui.Converters.UInt32ToLongConverter 	 UInt32ToLongConverter 
 
Style:Cell ?? 
TargetType*  $System.Windows.Controls.DataGridCell ?? Property ?? Value  Style.DataGrid.Row)  #System.Windows.Controls.DataGridRow	 ?? ElementName
 `? 	Converter ?? BindingGroupName ?? Path ?? 	Converter ?? Triggers ]? 
Conditions ?? Property ?? Value  Style.SingleUpDown$  Xceed.Wpf.Toolkit.SingleUpDown,  &Xceed.Wpf.Toolkit.Primitives.InputBase P? Property P? Value  Style.LongUpDown"  Xceed.Wpf.Toolkit.LongUpDown  Style.IntegerUpDown%  Xceed.Wpf.Toolkit.IntegerUpDown  Style.TextBox  Resources.Templates d? 	IsChecked ?? Mode ?? UpdateSourceTrigger$  Xceed.Wpf.Toolkit.DoubleUpDown?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin   ShowButtonSpinner   
IsReadOnly   Maximum   Minimum   Value?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin   ShowButtonSpinner?  ?Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   FormatString    Maximum!   Minimum"   Value?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]#   	AllowSpin$   ShowButtonSpinner%   Maximum&   Minimum'   Value?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]](   	AllowSpin)   ShowButtonSpinner*   Maximum+   Minimum,   Value- ??Name. ?? TextAlignment/ ?? 
IsReadOnly   System.Enum0 %? IsVirtualizing1 %? VirtualizationMode2 ?? SelectedValue  PropertyModelTemplateR  LICSStudio.Gui.Controls.ExtendedPropertyGrid.ExtendedPropertyTemplateSelector3   	Resources??%?  &	        &	      &	 H     &	 [     &	 n     &	 ?     &	 ?     &	 ?     &	 ?     &		 ?     &	
 ?     &	     7  7  &	     &	 ?    &	 	    &	 ?	    &	 -
    7  7  7  7  7  7  7  7 	 7 	 7 	 7  7  7  &	 ?    7  ?? 5   
   $
 #BBB??6.     5   
    64   ??  ?? "    5   
   6)     5   
   6;     5   
   6,     5   
   6*     5   
   6(     5   
   60   	  5   
   6.   
  5   
   6+   ?? 5   
   "  5!      .?5"      ??  ? ??$ 1q?6(   ?? 5#       ? ??
 ???? 6+   ?? 5$       ? ??$ 0q?60   ?? 5%       ? ??
 ?   ?6,   ?? 5&       ? ??$ 12(?6)   ?? 5'       ? ??$ Stretch=?6;   ?? 5(       ? ??$ Stretch=?69   5)      ?? 5+   
   "  6+   .?5,      ??  ? ?? 67   ?? *
Foreground+$	 PART_ExtendedPropertyGrid???? 5-       ? y? 5.      `? 5/      8
    6j   Z? .1?50      ?? $
 Name?6"   8  6.   ?? 51      $ SearchQuery?6"    65   ?? "  52      54       55      ]? 56       57      ?? 58       ?@ 
IsSelected .  6:   ?? 59       ?@J?IsMouseOver . 6;   ./?5;      ??  ? ??
 ???? 63   5<      ]? 5=       5>      ?? 5?       ?@ 
IsSelected . 6:   ?? 5@       ?@J?IsMouseOver . 6;   ./?5B      ??  ? ??
 ???? 63   5C      ]? 5D       5E      ?? 5F       ?@ 
IsSelected . 6:   ?? 5G       ?@??IsSelectionActive .  6J   ?? 5H       ?@J?IsMouseOver .  6;   ./?5J      ??  ? ??
 ???? 63   5K      ]? 5L       5M      ?? 5N       ?@ 
IsSelected . 6:   ?? 5O       ?@??IsSelectionActive . 6J   ?? 5P       ?@J?IsMouseOver .  6;   ./?5R      ??  ? ??
 ???? 63   5S      ]? 5T       5U      ?? 5V       ?@ 
IsSelected .  6:   ?? 5W       ?@??AlternationIndex$ 0??6M   ?? 5X       ?@J?IsMouseOver .  6;   ./?5Z      ??  ? ??
 ???? 63   5[      ]? 5\       5]      ?? 5^       ?@ 
IsSelected .  6:   ?? 5_       ?@??AlternationIndex$ 1??6M   ?? 5`       ?@J?IsMouseOver .  6;   ./?5b      ??  ? ??
 ???? 63   5c      5e      ?? 5g   
   "  6+    5h      P? 5i       ?@ 
IsReadOnly . . ?5j      ??  ? ??
 ?????6G   5k      5m      ?? 5n   
   "  6)    5o      P? 5p       ?@ 
IsReadOnly . . ?5q      ??  ? ??
 ?????6G   5r      5t      ?? 5u   
   "  6,    5v      P? 5w       ?@ 
IsReadOnly . . ?5x      ??  ? ??
 ?????6G   5y      5{      ?? 5}   
   " ??6&    5~      P? 5       ?@??
IsReadOnly . . ?5?      ??  ? ??
 ?????6G   5?      5?      ?? 5?   
   ?? 5?      '??       .R?5?      ?? $??Right=?5?      $??Center=?61    5?      ?? *Value+$ TwoWay=?$ PropertyChanged=?{?5?      ?? *
IsReadOnly+8   $ OneWay=?5?      ?? 5?      'X?       .R?5?        $??Stretch=?5?       .   .   5?      ?? *
IsReadOnly+$ OneWay=? 5?      ?? *	Maximum+ 5?      ?? *	Minimum+ 5?      ?? *Value+$ TwoWay=?$ PropertyChanged=?5?      ?? 5?      '??       .R?5?        $??Stretch=?5?       .   .  $ ######0.0######??5?      8??  5?      ?? *
IsReadOnly+$ OneWay=?  5?      ?? *	Maximum+! 5?      ?? *	Minimum+" 5?      ?? *Value+$ TwoWay=?$ PropertyChanged=?5?      5?      ?? 5?      'I?       .R?5?        $??Stretch=?5?      # .  $ .  8??  5?      ?? *
IsReadOnly+$ OneWay=?% 5?      ?? *	Maximum+8  & 5?      ?? *	Minimum+8  ' 5?      ?? *Value+$ TwoWay=?8  $ PropertyChanged=?5?      5?      ?? 5?      'G?       .R?5?        $??Stretch=?6"   ( .  ) .  8??  5?      ?? *
IsReadOnly+$ OneWay=?* 5?      ?? *	Maximum+8  + 5?      ?? *	Minimum+8  , 5?      ?? *Value+$ TwoWay=?8 	 $ PropertyChanged=?5?      5?      ?? 5?      '??       .R?5?        $??Stretch=?6%   # .  $ .   5?      ?? *
IsReadOnly+$ OneWay=?% 5?      ?? *	Maximum+& 5?      ?? *	Minimum+' 5?      ?? *Value+$ TwoWay=?$ PropertyChanged=?5?      ?? 5?      '??       .R?5?      ?? $- PART_Content??5?      ??5?      ?? $ OneWay=???5?      ?? 5?      .R?5?      ?? $??Stretch=?5?   !   $. Right=?5?   !   $??50??5?   !   $??0q?5?   !   8??
 / 5?   !   ?? *
IsReadOnly+$ OneWay=???5?   !   ?? *Value+$ TwoWay=?$ PropertyChanged=?5?   !   5?      5?      5?      ?? 5?      '        .R?5?      ?? 0 . $1 	Recycling=?5?      $??Stretch=?5?      $??0q?5?      $??Right=?5?      {?5?      ?? *
IsReadOnly+8  $ OneWay=???5?      ?? *
EnumSource+$ OneWay=?$ PropertyChanged=?2 5?      ?? *Value+$ TwoWay=?$ PropertyChanged=???5?      ?? 5?      .R?5?      ?? ??5?       ?? $ OneWay=?8  5?      5?      5?      5?      ?? 5?   
   .R?5?      ?? ??6   ?? ??5?        5?      83   6L   5?      5?      5?      .??5?      ? 4 ? DataContext4 6   ?? $	 PART_ExtendedPropertyGrid??5 ? RowDefinitions5 5?   
   ?? 5?      $??* ?6   6)   .+?5?   
   ?? $??0??6   
???????
???   ?$??1q?5?      ._?5?      &   System.Windows.Controls.DataGrid  6   AutoGenerateColumns6 .  7   CanUserAddRows7 .  8   CanUserDeleteRows8 .  9   	CellStyle#9 [
 5     $
??Auto=?5     $
??Auto=?5     :   SelectionMode$: Single=?5     ??5     ?? *ListCollectionView+; ?? IsAsync; . 2 5     ?? *Properties.ActiveProperty+$ TwoWay=?$ PropertyChanged=?<   Columns< 5     4  .System.Windows.Controls.DataGridTemplateColumn  5	     ,  &System.Windows.Controls.DataGridColumn=   Header$=  ??5
     >   Width5  /System.Windows.Controls.DataGridLengthConverter$> 5 5     ?   CellTemplate? 5     ?? 5     .R?5  "   ?? $??Center=?5  %   ??5  %   ?? *	IsChanged+$ OneWay=?# [ $ PropertyChanged=?5  %   5     5       5     $
= Name??5     @   SortMemberPath$
@ Name??5     > 5     ?? *Data.PropertyColumnWidth+A ?? Source#A [ ? 5     ?? 5     .R?5  "   ?? $??CharacterEllipsis=?5  %   $??Stretch=?5   %   $??Stretch=?5!  %   
?????????5  %   ?? *Name+$ OneWay=?$ PropertyChanged=?B ? ToolTipB 5  %   ?? *Text+$ OneWay=? ?? *Self+5"  %   5#     5%       5(     #? [ 5)     $= Value??5*     > 5+     ?? *Data.ValueColumnWidth+#A [ 5,       5/     $
= Unit??50     > 51     ?? *Data.UnitColumnWidth+#A [ ? 52     ?? 53     .R?54  "   ?? $??CharacterEllipsis=?56  %   $??Stretch=?58  %   $??Stretch=?59  %   
?????????55  %   ?? *Unit+$ OneWay=?$ PropertyChanged=?B 57  %   ?? *Text+$ OneWay=? ?? *Self+5:  %   5<     5>     5A     5C     5E     5F     