   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fk:clr-namespace:ICSStudio.UIServicesPackage.AddOnInstruction,ICSStudio.UIServicesPackage.AddOnInstruction  5      <    6ICSStudio.UIServicesPackage.AddOnInstruction.ScanModes   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Llocal:clr-namespace:ICSStudio.UIServicesPackage.AddOnInstruction     5      {?5   7   ?? *IsAllEnabled+  ? 	Resources  5	      =  7System.Windows.Controls.Primitives.DataGridColumnHeader ?? 
TargetType ?? Property ?? Value)  #System.Windows.Controls.DataGridRow ?? Triggers P? Property P? Value ?? MergedDictionaries ?? Source?? 5
   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %?  '        '  v     '?? 5    ?? 5      "  6   .?5      ??  ? ?? ?#b0c4de6A   ?? 5       ? ??$ 0q?64   6@   5      ?? 5      "  6   .?5      ??  ? ??$ 0q?64   6@    5      P? 5       ?@ AlternationIndex$ 0??6:   . ?5      ??  ? ??
 ?????6K   5      5      ?? 5      " ??6   .?5      ??  ? {? 6.   ?? *	IsEnabled+6L   5      5      .??5       ? 	 ? RowDefinitions	 5!   
   ?? 5"      $??50 ?6   6*   ?? 5#      $??30 ?6   6*   ?? 5$      $
??Auto ?6   6,   ?? 5%      $
??Auto ?6   6,   ?? 5&      $
??Auto ?6   6,   ?? 5'      $
??Auto ?6   6,   ?? 5(      $
??Auto ?6   6,   ?? 5)      $
??Auto ?6   6,   
 ? ColumnDefinitions
 5+   
   ?? 5,      $	??550 ?6   6-   .+?5.   
   ?? $??0??6      The Controller Prescan  And#???   6!   $
??Wrap=?6V   6l   ?? 5/   
   $??1??6    ! Optional Prescan Postscan And#???  6!   6Z   ?? 50   
   $??2??6     Prescan Routine Executes#???  6!   6U   ]? 51   
   $??3??6   $??Stretch=?6!   ??.  .+?52      ??  ! Execute Prescan Routine After#???  6   $
??Left=?6Q    d? 	IsChecked 53      ?? *	CheckBox1+ ?? UpdateSourceTrigger$ PropertyChanged=? ?? Mode$ TwoWay=?{?54      ?? *CheckBoxEnable1+$ PropertyChanged=?$ TwoWay=?6o   ?? 55       	 Go To#???  6   $??Right=?67   $??53??6O   ??6Z   ?? *GoToCommand1+6}   ?? 56      $??Right=?6e   $??53??6|    ?? 	IsDefault . ??6   ?? *ButtonName1+$ PropertyChanged=?$ OneWay=???57      ?? *DeleteOrNewCommand1+6P   58      ?? 59   
   $??4??6     Postscan Routine Executes#???  6!   6V   ]? 5:   
   $??5??6   $??Stretch=?6!   ??.  .+?5;      ??  " Execute Postscan Routine After#???  6   $
??Left=?6R    5<      ?? *	CheckBox2+$ PropertyChanged=?$ TwoWay=?{?5=      ?? *CheckBoxEnable2+$ PropertyChanged=?$ TwoWay=?6o   ?? 5>      #???  6   $??Right=?67   $??53??6O   ??6Z   ?? *GoToCommand2+6}   ?? 5?      $??Right=?6e   $??53??6|   ??6   ?? *ButtonName2+$ PropertyChanged=?$ OneWay=???5@      ?? *DeleteOrNewCommand2+6?   5A      ?? 5B   
   $??6??6    " EnableInFalse Routine Executes#???  6!   6[   ]? 5C   
   $??7??6   $??Stretch=?6"   ??.  .+?5D      ??  ! Execute EnableInFalse Routine#???  6   $
??Left=?6Q    5E      ?? *	CheckBox3+$ PropertyChanged=?$ TwoWay=?{?5F      ?? *CheckBoxEnable3+$ PropertyChanged=?$ TwoWay=?6o   ?? 5G      #???  6   $??Right=?67   $??53??6O   ??6Z   ?? *GoToCommand3+6}   ?? 5H      $??Right=?6e   $??53??6|   ??6   ?? *ButtonName3+$ PropertyChanged=?$ OneWay=???5I      ?? *DeleteOrNewCommand3+6?   5J      5K      5L      