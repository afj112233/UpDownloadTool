   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui  5       ICSStudio.DevicePropertiesL   FICSStudio.DeviceProperties.ModuleDefinition.ServoDriveDefinitionDialog   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      $	??380??5
      $	??280??5        ? 
ResizeMode$  NoResize=?5       ? ShowInTaskbar .   ? WindowStartupLocation$ CenterOwner=?5       ? Title 5	      ?? *Title+    ICSStudio.Gui.DialogCloser   DialogResult 5      ?? *DialogResult+ ? 	Resources 5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $??20 ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5       $
??Auto ?6   ?? 5!      $
??Auto ?6   ?? 5"      $??* ?6   ?? 5#      $
??Auto ?6   ?? 5$      $??* ?6   ?? 5%      $
??Auto ?6   	 ? ColumnDefinitions	 5'   
   ?? 5(      $	??120 ?6   ?? 5)      $??* ?6   .+?5,   
   ?? $??1??5-      $??0??5.         Revision#???   5/      ?? 50   
   $??1??51      $??1??52      
 ?? Orientation$
 
Horizontal=?53      .+?54      ?? $??60??55      ??56      ?? *MajorSource+ ?? SelectedValue 57      ?? *Major+?? 58      $??10??6   %  Xceed.Wpf.Toolkit.IntegerUpDown  59      $??80??5:      ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .    Maximum$	 255X?5<         Minimum$ 1X?5=         ShowButtonSpinner .    Value 5?      ?? *Minor+5@      ?? 5B   
   $??2??5C      $??0??5D        ElectronicKeying#???  5E      ?? 5F   
   $??2??5G      $??1??5H       ?? DisplayMemberPath$ DisplayName??5I       ?? SelectedValuePath$ Value??5L      ??5J      ?? *
EKeySource+ 5K      ?? *EKey+5L      ?? 5N   
   $??3??5O      $??0??5P        PowerStructure#???  5Q      ?? 5R   
   $??3??5S      $??1??5T      $ DisplayName??5U      $ Value??5X      ??5V      ?? *PowerStructureSource+ 5W      ?? *PowerStructureID+5X      ?? 5Z   
   $??4??5[      $??0??5\      $??2??5]       ! VerifyPowerRatingOnConnection#???  5^       d? 	IsChecked 5_      ?? *VerifyPowerRating+?? 5b   
   $??5??5c      $??0??5d      
???????$??1q?5f      ._?5g      ?? $??0q?6     
Connection#???  6#   5h      ?? 5j   
   $??5??5k      $??1??5l      
???????$??1q?5n      ._?5o      ?? $??0q?5p      $ DisplayName??5q      $ Value??5t      ??5r      ?? *ConnectionSource+ 5s      ?? *
Connection+5t      5u      ?? 5w   
   $??7??5x      $??0??5y      $??2??5z      ?? 5|   
   $??9??5}      $??0??5~      $??2??5      $??Center=?5?      $??Center=?5?      $
 
Horizontal=?5?      .+?5?      ??   OK#???  5?       ?? 	IsDefault . ??5?      ?? *	OkCommand+5?      ?? 5?       
 Cancel#???  5?       ?? IsCancel . ??5?      ?? *CancelCommand+5?      ?? 5?        Help#???  6   5?      5?      5?      