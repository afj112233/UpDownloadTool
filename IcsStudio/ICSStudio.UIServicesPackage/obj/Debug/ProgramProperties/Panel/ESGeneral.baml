   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fyAclr-namespace:ICSStudio.UIServicesPackage.ProgramProperties.Panel3ICSStudio.UIServicesPackage.ProgramProperties.Panel  5      C    =ICSStudio.UIServicesPackage.ProgramProperties.Panel.ESGeneral   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      SlocalAclr-namespace:ICSStudio.UIServicesPackage.ProgramProperties.Panel     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5        ? 	Resources  5
      $  Xceed.Wpf.Toolkit.SingleUpDown ?? 
TargetType ?? Property ?? Value ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %]   '        ?? 5      "  6   .?5      ??  ? ??$ 0,-2,0,0q?6,   6?   5      5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,    ? ColumnDefinitions 5!   
   ?? 5"      $
??Auto ?6   6.   ?? 5#      $
??Auto ?6   6.   ?? 5$      $
??Auto ?6   6.   .+?5&   
   ?? $??0??6   $??0??6!   $??2??61    	  Name:#???   6E   6f   ?? 5'   
   $??0??6   $??2??6   $	??350??60   ??6<   ?? *Name+ ?? UpdateSourceTrigger$ PropertyChanged=?6x   ?? 5(   
   $??1??6   $??0??6!   $??2??61     Description:#???  6E   $	??Top=?6k   6?   ?? 5)   
   $??1??6   $??2??6   $	??350??6/   $	??100??6;   	 ?? TextWrapping$
	 Wrap=?6J   
 ?? AcceptsReturn
 . " ?? VerticalScrollBarVisibility$ Visible=?6t   ??6?   ?? *Description+$ PropertyChanged=?6?   ?? 5*   
   $??2??6   $??0??6!   $??2??61     Parent:#???  6E   6h   ?? 5+   
   $??2??6   $??2??6!   ??61   ?? *Parent+6K   ?? 5-   
   $??0??6   $??4??6$     Schedule in:#???  61   $??2??6W   5.      ?? 5/   
   $??2??6   $??4??6$   ??61   ?? *
TaskName+50      ?? 52   
   2  ,System.Windows.Markup.XmlAttributeProperties  XmlSpace preserve6?   $??5??6   $??1??6!   $??2??61   $??10(?6E   $
??Left=?6S    	 Major#???  6?   y?6n   ?? *Type3+6?   ?? 53   
   $??5??6   $??1??6!   $??2??61   $??10(?6E   $??Center=?6S    	 Minor#???  6?   y?6p   ?? *Type3+6?   ?? 54   
   $??5??6   $??1??6!   $??2??61   $??10(?6E   $??Right=?6S     Extended text#???  6?   y?6o   ?? *Type3+6?   ?? 55   
   $??6??6   $??0??6!     	Revision:#???  6N   y?61   ?? *Type3+6s     56   
   $??6??6   $??1??6*   $??2??6:   $??Center=?6N   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .    ShowButtonSpinner . $??70??57   G   $??20??6R      Minimum$ 0X?58         Maximum$ 65535X?6)   ?  ?Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	Increment$ 1X?6:   $
??Left=?59   :   ,  &Xceed.Wpf.Toolkit.Primitives.InputBase   Text 58   H   ?? *Major+$ PropertyChanged=?y?59      ?? *Type3+6:     5:   
   $??6??6   $??1??6*   $??2??6:   $??Center=?6N   $??Center=?6k    .  . $??70??5;   G   $??20??6R   $ 0X?5<      $ 65535X?6)   $ 1X?69    6H   ?? *Minor+$ PropertyChanged=?y?5=      ?? *Type3+??5>      u? 5?       u? Left$ -8Q?6   6(   5A      ?? 5B   
   $??6??6   $??1??6   $??2??6/   $	??100??6C   $??Right=?6O   $??Center=?6?   y?6l   ?? *Type3+??6?   ?? *ExtendedText+$ PropertyChanged=?.??6?   ??5C      u? 5D       u? Right$ 3Q?6   6(   .??5E      5F      ?? 5G   
   $??7??6   $??0??6!   $	??Top=?6N     Revision note:#???  6f   y?61   ?? *Type3+6?   ?? 5H   
   $??7??6   $??1??6   $??2??6/   $ Visible=?6`   $
	 Wrap=?6?   
 . $	??100??6?   y?6C   ?? *Type3+??6?   ?? *RevisionNote+$ PropertyChanged=?6   5I      5J      