   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fq=clr-namespace:ICSStudio.UIServicesPackage.PLCProperties.Panel/ICSStudio.UIServicesPackage.PLCProperties.Panel  5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U<clr-namespace:System.Windows.Input;assembly=PresentationCoreSystem.Windows.Input 5      Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls 5	      >    8ICSStudio.UIServicesPackage.PLCProperties.Panel.Advanced   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Olocal=clr-namespace:ICSStudio.UIServicesPackage.PLCProperties.Panel     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      Hinput<clr-namespace:System.Windows.Input;assembly=PresentationCore  5      Jcontrols;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui  5	        ? 	Resources  5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,    ? ColumnDefinitions 5   
   ?? 5      $
??Auto ?6   6.   ?? 5       $??* ?6   6+   .+?5"   
   ?? $??0??6   $??0??6!      Controller Fault Task:#???   61   6c   ?? 5#   
   $??0??6   $??1??6    $	??200??60   $
??Left=?6<   ??6W   ?? *ProgramCollection+ ?? SelectedValue 5$      ?? *FaultHandler+{?6:   ?? *Enable+6Y   ?? 5&   
   $??1??6   $??0??6!     StartUp Task:#???  61   6Z   ?? 5'   
   $??1??6   $??1??6    $	??200??60   $
??Left=?6<   ??6W   ?? *ProgramCollection+ 5(      ?? *PowerupHandler+{?6<   ?? *Enable+6[   ?? 5*   
   $??2??6   $??0??6    $??2??60   $
??0,10q?6D     Match Project to Controller#???  6R    d? 	IsChecked 6?   ?? *MatchProjectToController+6?   ?? 5,   
   $??3??6   $??0??6!     Serial Number:#???  61   $??Right=?6Y   6w   ?? 5-   
   $??3??6   $??1??6   $	??100??6/   $
??Left=?6;   $??Center=?6V   ??6x   ?? *SN+ ?? UpdateSourceTrigger$ PropertyChanged=?{?6?   ?? *MatchProjectToController+?? 5/   
   $??4??6   $??0??6    $
??0,20q?60    7 3Allow Consumed Tags to Use RPI Provided by Producer#???  6>   $??2??6?   {?50      ?? *Enable+62   ?? 52   
   $??5??6   $??0??6!   $??Bottom=?61    ( $System Background Task Time Percent:#???  6L   6?   $  Xceed.Wpf.Toolkit.SingleUpDown  53   
   -   5        $??5??53      $??1??6*   $
??Left=?6:   ?  ?Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	MaxLength$ 2??54      	 ?? IsInputMethodEnabled	 .  ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
   	AllowSpin
 .    ShowButtonSpinner . $??45??57   G   $??Bottom=?6R      Minimum$ 0X?58         Maximum$ 99X?6)      Value 66   ?? *	TimeSlice+?? 5:   
   $??5??6   $??1??6!   $??55,0,0,4q?61   $??%??6C   $??Bottom=?6L   6i   ?? 5<   
   $??6??6   $??0??6    $??2??60   $??0,5,0,0q?6D    3 /During Unused System Background Task Time Slice#???  6U   .??5=      ??  ?? Orientation$ Vertical=?6   .+?5>      	?   Run Continuous Task#???  6    6N   ?? *
IsCheck1+6o   	? 5?       . *Reserve for System Tasks eg Communications#???  6    6e   ?? *
IsCheck2+6?   5@      5A      5B      5C      