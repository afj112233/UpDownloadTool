   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui  5       ICSStudio.DevicePropertiesP   JICSStudio.DeviceProperties.ModuleDefinition.DIOEnetAdapterDefinitionDialog   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      $	??380??5
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
  Series#???   5.      ?? 5/   
   $??0??50      $??1??51      $??60??52      $
??Left=?53      ??54      ?? *SeriesSource+
 ?? SelectedValue
 55      ?? *Series+?? 57   
   $??1??58      $??0??59        Revision#???  5:      ?? 5;   
   $??1??5<      $??1??5=       ?? Orientation$ 
Horizontal=?5>      .+?5?      ?? $??60??5@      ??5A      ?? *MajorSource+
 5B      ?? *Major+?? 5C      $??10??6   %  Xceed.Wpf.Toolkit.IntegerUpDown  5D      $??80??5E      ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .    Maximum$	 255X?5G         Minimum$ 1X?5H         ShowButtonSpinner .    Value 5J      ?? *Minor+5K      ?? 5M   
   $??2??5N      $??0??5O        ElectronicKeying#???  5P      ?? 5Q   
   $??2??5R      $??1??5S       ?? DisplayMemberPath$ DisplayName??5T       ?? SelectedValuePath$ Value??5W      ??5U      ?? *
EKeySource+
 5V      ?? *EKey+5W      ?? 5Y   
   $??3??5Z      $??0??5[      
???????$??1,1,0,0q?5]      ._?5^      ?? $??0q?6     
Connection#???  6#   5_      ?? 5`   
   $??3??5a      $??1??5b      
???????$??1,1,1,0q?5d      ._?5e      ?? $??0q?5f      $ DisplayName??5g      $ Value??5j      ??5h      ?? *ConnectionSource+
 5i      ?? *ConnectionConfigID+5j      5k      ?? 5n   
   $??4??5o      $??0??5p      
???????$??1,1,0,1q?5r      ._?5s      ?? $??0q?6     ChassisSize#???  6#   5t      ?? 5u   
   $??4??5v      $??1??5w      
???????$??1,1,1,1q?5y      ._?5z      ?? $??0q?5{      ??5|      ?? *ChassisSizeSource+
 5}      ?? *ChassisSize+5~      ?? 5?   
   $??6??5?      $??0??5?      $??2??5?      ?? 5?   
   $??8??5?      $??0??5?      $??2??5?      $??Center=?5?      $??Center=?5?      $ 
Horizontal=?5?      .+?5?      ??   OK#???  5?       ?? 	IsDefault . ??5?      ?? *	OkCommand+5?      ?? 5?       
 Cancel#???  5?       ?? IsCancel . ??5?      ?? *CancelCommand+5?      ?? 5?        Help#???  6   5?      5?      5?      