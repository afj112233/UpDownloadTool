   M S B A M L   `   `   `  ???? Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05=#clr-namespace:ICSStudio.Gui.DialogsICSStudio.Gui.Dialogs  2    ,ICSStudio.Gui.Dialogs.ImportPropertiesDialog   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     5local#clr-namespace:ICSStudio.Gui.Dialogs     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006       ? 
ResizeMode$  NoResize=? ? Title ?? *Title+ ? 	Resources    ButtonStyle ?? 
TargetType ?? Property ?? Value-   'ICSStudio.Gui.Dialogs.LanguageConverter  LanguageConverter ?? MergedDictionaries ?? Source??  ?? $&  ../Resources/styles/Default.xaml??%X   &	        &	 ?     ?? " ??.???  ? ??$ 4,4,12,4q???  ? ??$ 74??  .??? -   $
??Grid?? ? RowDefinitions ?? $??* ??? $
??Auto ??? $
??Auto ?.+??? $??1??	 ?? Orientation$	 
Horizontal=?.+??? $??1??  Current Operation:#???  ?? $??1?????? *CurrentOperation+
 ?? 	Converter#
 [ ?? $??2??$
??Left=?$	 
Horizontal=?.+??? ???? *Operation1Command+???? *
Operation1+#
 [ ?? ???? *Operation2Command+???? *
Operation2+#
 [ ?? $??2??$??Right=?$	 
Horizontal=?.+???  	 Close#???   ?? IsCancel . ???? *CloseCommand+??   Help#???  