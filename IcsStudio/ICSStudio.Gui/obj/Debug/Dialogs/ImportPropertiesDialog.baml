   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05=#clr-namespace:ICSStudio.Gui.DialogsICSStudio.Gui.Dialogs  5      2    ,ICSStudio.Gui.Dialogs.ImportPropertiesDialog   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      5local#clr-namespace:ICSStudio.Gui.Dialogs     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5        ? 
ResizeMode$  NoResize=?5       ? Title 5      ?? *Title+ ? 	Resources 5         ButtonStyle ?? 
TargetType ?? Property ?? Value-   'ICSStudio.Gui.Dialogs.LanguageConverter  LanguageConverter ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $&  ../Resources/styles/Default.xaml??6%   %?   &	        &	 r     ?? 5      " ??6(   .?5      ??  ? ??$ 4,4,12,4q?6+   ?? 5       ? ??$ 74??6-   5        5      6&   5      .??5      ? -   5        $
??Grid??5       ? RowDefinitions 5   
   ?? 5      $??* ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   .+?5    
   ?? $??1??6   	 ?? Orientation$	 
Horizontal=?6"   .+?5!      ?? $??1??6     Current Operation:#???  6%   ?? 5"      $??1??6   ??6%   ?? *CurrentOperation+
 ?? 	Converter#
 [ 5#      ?? 5%   
   $??2??5&      $
??Left=?5'      $	 
Horizontal=?5(      .+?5)      ?? ??6   ?? *Operation1Command+??6;   ?? *
Operation1+#
 [ ?? 5*      ??6   ?? *Operation2Command+??6;   ?? *
Operation2+#
 [ 5+      ?? 5,   
   $??2??5-      $??Right=?5.      $	 
Horizontal=?5/      .+?50      ??  	 Close#???  52       ?? IsCancel . ??51      ?? *CloseCommand+53      ?? 54        Help#???  6   55      56      57      