   M S B A M L   `   `   `  ???? Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui   ICSStudio.DevicePropertiesP   JICSStudio.DeviceProperties.ModuleDefinition.DIOEnetAdapterDefinitionDialog   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  $	??380??$	??280??  ? 
ResizeMode$  NoResize=? ? ShowInTaskbar .   ? WindowStartupLocation$ CenterOwner=? ? Title ?? *Title+    ICSStudio.Gui.DialogCloser   DialogResult ?? *DialogResult+ ? 	Resources  ?? MergedDictionaries ?? Source??  ?? $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??%    .???  ? RowDefinitions ?? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $??* ??? $
??Auto ??? $??* ??? $
??Auto ?	 ? ColumnDefinitions	 ?? $	??120 ??? $??* ?.+??? $??0??$??0?? 
  Series#???   ?? $??0??$??1??$??60??$
??Left=????? *SeriesSource+
 ?? SelectedValue
 ?? *Series+?? $??1??$??0??  Revision#???  ?? $??1??$??1?? ?? Orientation$ 
Horizontal=?.+??? $??60?????? *MajorSource+
 ?? *Major+?? $??10??%  Xceed.Wpf.Toolkit.IntegerUpDown  $??80???  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .    Maximum$	 255X?   Minimum$ 1X?   ShowButtonSpinner .    Value ?? *Minor+?? $??2??$??0??  ElectronicKeying#???  ?? $??2??$??1?? ?? DisplayMemberPath$ DisplayName?? ?? SelectedValuePath$ Value?????? *
EKeySource+
 ?? *EKey+?? $??3??$??0??
???????$??1,1,0,0q?._??? $??0q?  
Connection#???  ?? $??3??$??1??
???????$??1,1,1,0q?._??? $??0q?$ DisplayName??$ Value?????? *ConnectionSource+
 ?? *ConnectionConfigID+?? $??4??$??0??
???????$??1,1,0,1q?._??? $??0q?  ChassisSize#???  ?? $??4??$??1??
???????$??1,1,1,1q?._??? $??0q????? *ChassisSizeSource+
 ?? *ChassisSize+?? $??6??$??0??$??2???? $??8??$??0??$??2??$??Center=?$??Center=?$ 
Horizontal=?.+???   OK#???   ?? 	IsDefault . ???? *	OkCommand+??  
 Cancel#???   ?? IsCancel . ???? *CancelCommand+??   Help#???  