   M S B A M L   `   `   `  ???? O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05i9clr-namespace:ICSStudio.Gui.Controls.ExtendedPropertyGrid+ICSStudio.Gui.Controls.ExtendedPropertyGrid C&clr-namespace:ICSStudio.Gui.ConvertersICSStudio.Gui.Converters F   @ICSStudio.Gui.Controls.ExtendedPropertyGrid.ExtendedPropertyGrid   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     T PImagin.Common, Version=3.1.0.0, Culture=neutral, PublicKeyToken=fd3ef6834d7c49b9-common http://imagin.tech/imagin/common  3System&clr-namespace:System;assembly=mscorlib  X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  TextendedPropertyGrid9clr-namespace:ICSStudio.Gui.Controls.ExtendedPropertyGrid  7
converters&clr-namespace:ICSStudio.Gui.Converters  -     >?Name$  PART_ExtendedPropertyGrid?? ? 	Resources    Brush.Image ?? Color%  Imagin.Common.Data.BindingProxy  BindingProxy   Data ?? RelativeSource ?? AncestorTypeF  @ICSStudio.Gui.Controls.ExtendedPropertyGrid.ExtendedPropertyGrid;  5Imagin.Common.Data.Converters.InverseBooleanConverter  InverseBooleanConverterM  GImagin.Common.Data.Converters.StartsWithToVisibilityMultiValueConverter - )StartsWithToVisibilityMultiValueConverter>  8Imagin.Common.Data.Converters.StringToCamelCaseConverter  StringToCamelCaseConverter<  6Imagin.Common.Data.Converters.EnumToCamelCaseConverter  EnumToCamelCaseConverter1  +ICSStudio.Gui.Converters.IsChangedConverter  IsChangedConverter9  3ICSStudio.Gui.Converters.EnumToDisplayNameConverter  EnumToDisplayNameConverter7	  1ICSStudio.Gui.Converters.UInt16ToIntegerConverter  UInt16ToIntegerConverter4
  .ICSStudio.Gui.Converters.UInt32ToLongConverter 	 UInt32ToLongConverter 
 
Style:Cell ?? 
TargetType*  $System.Windows.Controls.DataGridCell ?? Property ?? Value  Style.DataGrid.Row)  #System.Windows.Controls.DataGridRow	 ?? ElementName
 `? 	Converter ?? BindingGroupName ?? Path ?? 	Converter ?? Triggers ]? 
Conditions ?? Property ?? Value  Style.SingleUpDown$  Xceed.Wpf.Toolkit.SingleUpDown,  &Xceed.Wpf.Toolkit.Primitives.InputBase P? Property P? Value  Style.LongUpDown"  Xceed.Wpf.Toolkit.LongUpDown  Style.IntegerUpDown%  Xceed.Wpf.Toolkit.IntegerUpDown  Style.TextBox  Resources.Templates d? 	IsChecked ?? Mode ?? UpdateSourceTrigger$  Xceed.Wpf.Toolkit.DoubleUpDown?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin   ShowButtonSpinner   
IsReadOnly   Maximum   Minimum   Value?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin   ShowButtonSpinner?  ?Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   FormatString    Maximum!   Minimum"   Value?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]#   	AllowSpin$   ShowButtonSpinner%   Maximum&   Minimum'   Value?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]](   	AllowSpin)   ShowButtonSpinner*   Maximum+   Minimum,   Value- ??Name. ?? TextAlignment/ ?? 
IsReadOnly   System.Enum0 %? IsVirtualizing1 %? VirtualizationMode2 ?? SelectedValue  PropertyModelTemplateR  LICSStudio.Gui.Controls.ExtendedPropertyGrid.ExtendedPropertyTemplateSelector3   	Resources??%C  &	        &	      &	 ,     &	 1     &	 6     &	 ;     &	 @     &	 E     &	 J     &		 O     &	
 T     &	     7  7  &	 ?    &	 ?    &	 8    &	 ?    &	 ?    7  7  7  7  7  7  7  7 	 7 	 7 	 7  7  7  &	 +    7  ?? $
 #BBB??   ??  ?? "              	  
  ?? "  .???  ? ??$ 1q???  ? ??
 ???? ??  ? ??$ 0q???  ? ??
 ?   ???  ? ??$ 12(???  ? ??$ Stretch=???  ? ??$ Stretch=??? "  .???  ? ?? ?? *
Foreground+$	 PART_ExtendedPropertyGrid????  ? y? `? 8
    Z? .1??? $
 Name?8  ?? $ SearchQuery? ?? "   ]?  ??  ?@ 
IsSelected .  ??  ?@J?IsMouseOver . ./???  ? ??
 ???? ]?  ??  ?@ 
IsSelected . ??  ?@J?IsMouseOver . ./???  ? ??
 ???? ]?  ??  ?@ 
IsSelected . ??  ?@??IsSelectionActive .  ??  ?@J?IsMouseOver .  ./???  ? ??
 ???? ]?  ??  ?@ 
IsSelected . ??  ?@??IsSelectionActive . ??  ?@J?IsMouseOver .  ./???  ? ??
 ???? ]?  ??  ?@ 
IsSelected .  ??  ?@??AlternationIndex$ 0????  ?@J?IsMouseOver .  ./???  ? ??
 ???? ]?  ??  ?@ 
IsSelected .  ??  ?@??AlternationIndex$ 1????  ?@J?IsMouseOver .  ./???  ? ??
 ???? ?? "   P?  ?@ 
IsReadOnly . . ???  ? ??
 ??????? "   P?  ?@ 
IsReadOnly . . ???  ? ??
 ??????? "   P?  ?@ 
IsReadOnly . . ???  ? ??
 ??????? " ?? P?  ?@??
IsReadOnly . . ???  ? ??
 ??????? ?? '??       .R??? $??Right=?$??Center=? ?? *Value+$ TwoWay=?$ PropertyChanged=?{??? *
IsReadOnly+8   $ OneWay=??? 'X?       .R?  $??Stretch=? .   .   ?? *
IsReadOnly+$ OneWay=? ?? *	Maximum+ ?? *	Minimum+ ?? *Value+$ TwoWay=?$ PropertyChanged=??? '??       .R?  $??Stretch=? .   .  $ ######0.0######??8??  ?? *
IsReadOnly+$ OneWay=?  ?? *	Maximum+! ?? *	Minimum+" ?? *Value+$ TwoWay=?$ PropertyChanged=??? 'I?       .R?  $??Stretch=?# .  $ .  8??  ?? *
IsReadOnly+$ OneWay=?% ?? *	Maximum+8  & ?? *	Minimum+8  ' ?? *Value+$ TwoWay=?8  $ PropertyChanged=??? 'G?       .R?  $??Stretch=?( .  ) .  8??  ?? *
IsReadOnly+$ OneWay=?* ?? *	Maximum+8  + ?? *	Minimum+8  , ?? *Value+$ TwoWay=?8 	 $ PropertyChanged=??? '??       .R?  $??Stretch=?# .  $ .   ?? *
IsReadOnly+$ OneWay=?% ?? *	Maximum+& ?? *	Minimum+' ?? *Value+$ TwoWay=?$ PropertyChanged=??? '??       .R??? $- PART_Content?????? $ OneWay=????? .R??? $??Stretch=?$. Right=?$??50??$??0q?8??
 / ?? *
IsReadOnly+$ OneWay=????? *Value+$ TwoWay=?$ PropertyChanged=??? '        .R??? 0 . $1 	Recycling=?$??Stretch=?$??0q?$??Right=?{??? *
IsReadOnly+8  $ OneWay=????? *
EnumSource+$ OneWay=?$ PropertyChanged=?2 ?? *Value+$ TwoWay=?$ PropertyChanged=????? .R??? ???? $ OneWay=?8  ?? .R??? ???? ??  83   .??? 4 ? DataContext4 ?? $	 PART_ExtendedPropertyGrid??5 ? RowDefinitions5 ?? $??* ?.+??? $??0??
???????
???   ?$??1q?._?&   System.Windows.Controls.DataGrid  6   AutoGenerateColumns6 .  7   CanUserAddRows7 .  8   CanUserDeleteRows8 .  9   	CellStyle#9 [
 $
??Auto=?$
??Auto=?:   SelectionMode$: Single=????? *ListCollectionView+; ?? IsAsync; . 2 ?? *Properties.ActiveProperty+$ TwoWay=?$ PropertyChanged=?<   Columns< 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn=   Header$=  ??>   Width5  /System.Windows.Controls.DataGridLengthConverter$> 5 ?   CellTemplate? ?? .R??? $??Center=????? *	IsChanged+$ OneWay=?# [ $ PropertyChanged=?  $
= Name??@   SortMemberPath$
@ Name??> ?? *Data.PropertyColumnWidth+A ?? Source#A [ ? ?? .R??? $??CharacterEllipsis=?$??Stretch=?$??Stretch=?
??????????? *Name+$ OneWay=?$ PropertyChanged=?B ? ToolTipB ?? *Text+$ OneWay=? ?? *Self+  #? [ $= Value??> ?? *Data.ValueColumnWidth+#A [   $
= Unit??> ?? *Data.UnitColumnWidth+#A [ ? ?? .R??? $??CharacterEllipsis=?$??Stretch=?$??Stretch=?
??????????? *Unit+$ OneWay=?$ PropertyChanged=?B ?? *Text+$ OneWay=? ?? *Self+