   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui  5       ICSStudio.DevicePropertiesK   EICSStudio.DeviceProperties.ModuleDefinition.DIOModuleDefinitionDialog   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      $	??380??5
      $	??280??5        ? 
ResizeMode$  NoResize=?5       ? ShowInTaskbar .   ? WindowStartupLocation$ CenterOwner=?5       ? Title 5	      ?? *Title+    ICSStudio.Gui.DialogCloser   DialogResult 5      ?? *DialogResult+ ? 	Resources 5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5       $
??Auto ?6   ?? 5!      $??* ?6   ?? 5"      $
??Auto ?6   ?? 5#      $??* ?6   ?? 5$      $
??Auto ?6   	 ? ColumnDefinitions	 5&   
   ?? 5'      $	??120 ?6   ?? 5(      $??* ?6   .+?5+   
   ?? $??0??5,      $??0??5-       
  Series#???   5.      y?5/      ?? *SeriesVisibility+?? 50   
   $??0??51      $??1??52      $??60??53      $
??Left=?54      ??55      ?? *SeriesSource+
 ?? SelectedValue
 56      ?? *Series+y?57      ?? *SeriesVisibility+?? 59   
   $??1??5:      $??0??5;        Revision#???  5<      ?? 5=   
   $??1??5>      $??1??5?       ?? Orientation$ 
Horizontal=?5@      .+?5A      ?? $??60??5B      ??5C      ?? *MajorSource+
 5D      ?? *Major+?? 5E      $??10??6   %  Xceed.Wpf.Toolkit.IntegerUpDown  5F      $??80??5G      ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .    Maximum$	 255X?5I         Minimum$ 1X?5J         ShowButtonSpinner .    Value 5L      ?? *Minor+5M      ?? 5O   
   $??2??5P      $??0??5Q        ElectronicKeying#???  5R      ?? 5S   
   $??2??5T      $??1??5U       ?? DisplayMemberPath$ DisplayName??5V       ?? SelectedValuePath$ Value??5Y      ??5W      ?? *
EKeySource+
 5X      ?? *EKey+5Y      ?? 5[   
   $??3??5\      $??0??5]      
???????$??1,1,0,0q?5_      ._?5`      ?? $??0q?6     
Connection#???  6#   5a      ?? 5b   
   $??3??5c      $??1??5d      
???????$??1,1,1,0q?5f      ._?5g      ?? $??0q?5h      $ DisplayName??5i      $ Value??5l      ??5j      ?? *ConnectionSource+
 5k      ?? *ConnectionConfigID+5l      5m      ?? 5p   
   $??4??5q      $??0??5r      
???????$??1,1,0,1q?5t      ._?5u      ?? $??0q?5v        
DataFormat#???  5w      y?5x      ?? *DataFormatVisibility+5y      ?? 5z   
   $??4??5{      $??1??5|      
???????$??1,1,1,1q?5~      ._?5      ?? $??0q?5?      ??5?      ?? *DataFormatSource+
 5?      ?? *
DataFormat+y?5?      ?? *DataFormatVisibility+5?      ?? 5?   
   $??6??5?      $??0??5?      $??2??5?      ?? 5?   
   $??8??5?      $??0??5?      $??2??5?      $??Center=?5?      $??Center=?5?      $ 
Horizontal=?5?      .+?5?      ??   OK#???  5?       ?? 	IsDefault . ??5?      ?? *	OkCommand+5?      ?? 5?       
 Cancel#???  5?       ?? IsCancel . ??5?      ?? *CancelCommand+5?      ?? 5?        Help#???  6   5?      5?      5?      