   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls  5       ICSStudio.UIServicesPackageK   EICSStudio.UIServicesPackage.AxisVirtualProperties.Panel.MotionPlanner   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      -   5        $	??650??5	      $	??350??5
        ? 	Resources  5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $??30 ?6   ?? 5      $??30 ?6   ?? 5      $??30 ?6    ? ColumnDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5       $	??130 ?6   ?? 5!      $??40 ?6   .+?5#   
   ?? $??0??5$      $??0??5%       !  Output Cam Execution Targets:#???   5&      %  Xceed.Wpf.Toolkit.IntegerUpDown  5'   
   -   5          Name$ IntegerUpDown??5(      $??0??5)      $??1??5*      $??50??5+      $
??Left=?5,      $??Center=?5-      ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .    Maximum$ 8X?50         Minimum$ 0X?51      	   ShowButtonSpinner	 . ,  &Xceed.Wpf.Toolkit.Primitives.InputBase
   TextAlignment$

 Left=?53      {?5/      ?? *
IsEnable+   Value 54      ?? *OutputCamExecutionTargets+ ?? UpdateSourceTrigger$ PropertyChanged=??? 56   
   $??1??57      $??0??58      $??2??59        Program Stop Action:#???  5:      ?? 5;   
   $??1??5<      $??1??5=      $	??120??5>      $??22??5?      $??2,2,2,0q?5@      $
??Left=?5A      $	??Top=?5B       ?? DisplayMemberPath$ DisplayName??5C       ?? SelectedValuePath$ Value??5G      {?5D      ?? *
IsEnable+??5E      ?? *ProgramStopActionList+ ?? SelectedValue 5F      ?? *
Selected+5G      ?? 5H   
   $??2??5I      $??0??5J        Master Delay Compensation#???  5K       d? 	IsChecked 5L      ?? *Check1+{?5M      ?? *
IsEnable+??5N      u? 5O      
 u? Top$ 7Q?6   5Q      ?? 5S   
   $??3??5T      $??0??5U       ! Enable Master Position Filter#???  5V       5W      ?? *Check2+{?5X      ?? *
IsEnable+??5Y      u? 5Z      $ 7Q?6   5\      ?? 5]   
   $??4??5^      $??0??5_       $  Master Position Filter Bandwidth#???  5`      {?5a      ?? *Check2+$ PropertyChanged=???5b      u? 5c       u? Left$ 30Q?6   5e      3   -ICSStudio.Gui.Controls.SingleUpDownExtensions  5g   
   $??4??5h      $??1??5i      $	??120??5j      $??23??5k      $
??Left=?5l      $??Bottom=?5m      ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .  $

 Left=?5q      {?5o      ?? *'%MasterPositionFilterBandwidthIsEnable+$ PropertyChanged=? ?? Delay$ 30??   Value 5r      ?? *MasterPositionFilterBandwidth+$ PropertyChanged=?$ 30???? 5s   
   $??4??5t      $??2??5u      $??40??5v      $??Hertz??5w      {?5x      ?? *'%MasterPositionFilterBandwidthIsEnable+$ PropertyChanged=?5y      5z      