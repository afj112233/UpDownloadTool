   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fq=clr-namespace:ICSStudio.UIServicesPackage.PLCProperties.Panel/ICSStudio.UIServicesPackage.PLCProperties.Panel  5      Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls 5      B    <ICSStudio.UIServicesPackage.PLCProperties.Panel.SFCExecution   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Olocal=clr-namespace:ICSStudio.UIServicesPackage.PLCProperties.Panel     5      Pcontrols;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     5        ? 	Resources  5
       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ? {?.    5   
    ?? 
TargetType ?? Property ?? Value ?? Triggers P? Property P? Value??%?   '	?       ?? 5      " 	?6   .?5      ??  ? ??$ 20,0,0,0q?6+   6>    5      P? 5       ? {? .  . ?5      ??  ? ?? ?	#FFDDDDDD67   5      5      5      	 ? RowDefinitions	 5   
   ?? 5      $??80 ?6   6*   ?? 5      $??80 ?6   6*   ?? 5       $??80 ?6   6*   .+?5"   
   ?? $??0??6   .+?5#      ??    Execution Control:#???   6   6F   	? 5$       % !Execute current active steps only#???  6   
 	? 	GroupName$
 1??6u   {?6X   ?? *Enable+6?   	? 5%       " Execute until FALSE transition#???  6   $
 1??6r   {?6U   ?? *Enable+6?   5&      ?? 5(   
   $??1??6   .+?5)      ??   Restart Position#???  6   6D   	? 5*        Restart at most recently#???  6   $
 2??6l   {?6O   ?? *Enable+6|   	? 5+        Restart at initial step#???  6   $
 2??6k   {?6N   ?? *Enable+6{   5,      ?? 5.   
   $??2??6   .+?5/      ??   Last Scan of Active Steps:#???  6   6N   	? 50        Automatic reset#???  6   $
 3??6c   {?6F   ?? *Enable+6s   	? 51        Programmatic reset#???  6   $
 3??6f   {?6I   ?? *Enable+6v   	? 52       	 Do not reset#??? 	 6   $
 3??6`   {?6C   ?? *Enable+6p   53      54      55      