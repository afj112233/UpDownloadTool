   M S B A M L   `   `   `  ???? Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui  ^ ZICSStudio.Dialogs, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=8558c3dcb301e1d4Q-clr-namespace:ICSStudio.Dialogs.ConfigDialogsICSStudio.Dialogs.ConfigDialogs 5   /ICSStudio.Dialogs.ConfigDialogs.GraphProperties   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  <gui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui  9local-clr-namespace:ICSStudio.Dialogs.ConfigDialogs  V RMaterialDesignThemes.Wpf, Version=3.1.3.1513, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes     GraphProperties  ? Title#  ?   $	??440??$	??245??$??11(? ? WindowStartupLocation$ CenterOwner=?    ICSStudio.Gui.DialogCloser   DialogResult ?? *GraphPropertiesResult+.??? ???#F0F0F0 ? RowDefinitions ?? $	??170 ??? $
??Auto ?.+?? $??0??.+??? $	??155??$	??415??$??Center=?$??Center=?.@??? $??55??$??20?????? .R???   Pen#???  .??? ???#F0F0F0.+?&   System.Windows.Controls.DataGrid     	RowHeight$ 20Q?   RowHeaderWidth$ 0Q?   CanUserDeleteRows .  $   HorizontalScrollBarVisibility$ Hidden=?"   VerticalScrollBarVisibility$ Hidden=?	   AutoGenerateColumns	 .  
   CanUserAddRows
 .  $??Center=?$	??395??$	??106?????? *DataItemsSource+   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ 70    
IsReadOnly . 1  +System.Windows.Controls.DataGridBoundColumn   Binding ?? *
SlaveValue+   HeaderTemplate ?? .R???   
SlaveValue#???  4  .System.Windows.Controls.DataGridTemplateColumn  $ 65  ?? .R???  	 Color#???     CellTemplate ??  ?? DataType$  materialDesign:ColorPicker?? ? 	Resources 4  .ICSStudio.Dialogs.ConfigDialogs.ColorConverter  ConverterColor??%   &	         .R?#	  Xceed.Wpf.Toolkit.ColorPicker	  $
??Auto??$
??Auto?? 	  ShowDropDownButton .   	  SelectedColor ?? *Color+ ?? UpdateSourceTrigger$ PropertyChanged=? ?? Mode$ TwoWay=? ?? 	Converter# [ 4
  .System.Windows.Controls.DataGridComboBoxColumn
  $ 65  ?? .R???   
CAMVisible#???   
  EditingElementStyle ??  ?? 
TargetType" ??.???  ?? Property ? ?? ?? Value ?? *VisibleList+??  ?@??SelectedValue ??  ?? Path$ Visible? 
  ElementStyle ?? " ??.???  ? ?? ?? *VisibleList+??  ?@??SelectedValue ?? $ Visible?
  $ 65  ?? .R???  	 Width#???   ?? " ??.???  ? ?? ?? *	WidthList+??  ?@??SelectedValue ?? $ Width? ?? " ??.???  ? ?? ?? *	WidthList+??  ?@??SelectedValue ?? $ Width?
  $ 65  ?? .R???   CAMStyle#???   ?? " ??.???  ? ?? ?? *	StyleList+??  ?@??SelectedValue ?? $ Style? ?? " ??.???  ? ?? ?? *	StyleList+??  ?@??SelectedValue ?? $ Style?
  $ *  ?? .R???  
 Marker#???   ?? " ??.???  ? ?? ?? *
MarkerList+??  ?@??SelectedValue ?? $ Marker? ?? " ??.???  ? ?? ?? *
MarkerList+??  ?@??SelectedValue ?? $ Marker??? $??55??$??20?????? .R???  	 Grid#??? 	 .???  ?? $??15 ??? $??15 ??? $??15 ??? $??15 ??? $
??Auto ? ? ColumnDefinitions ?? $??15 ??? $??15 ??? .+?	? $??1??$??1??$??Right=??? $??2??$??1??$??0q? 
 
EnableGrid#??? 
 ?? $??2??$??3??$??0q?  CAMGridColor#???  ? $??2??$??4??$??67??$??17??$
??Left=?.+?	  $
??Auto??$
??Auto?? .   ?? *CAMGridColor+?? $??55??$??20?????? .R???   Velocity#???  .???  ?? $??15 ??? $??71.207 ??? $??20.126 ??? $
??Auto ? ?? $??15 ??? $
??Auto ?.+??? $??1??$??1??  MasterVelocity#???  $??3??$??0,0,80.666,-0.667q??? $??1??$??2??$	??0.7q?
???   ?$??80??$??20??$
??Left=?$??2??$??19.793,1,0,0.333q????? *MasterVelocity+? $??1?? ?? $	??160 ??? $??80 ??? $??8 ??? $??80 ??? $??8 ??? $??80 ?.+??? $??1??$??25?? ?? 	IsDefault .   OK#???  ???? *	OKCommand+?? $??3??$??25??  ?? IsCancel  .  
 Cancel#???  ???? *CancelCommand+?? $??5??$??25?? 	 Apply#???  ???? *ApplyCommand+