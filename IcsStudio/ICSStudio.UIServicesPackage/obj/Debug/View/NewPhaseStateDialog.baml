   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fS.clr-namespace:ICSStudio.UIServicesPackage.View ICSStudio.UIServicesPackage.View  5      Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui 5      :    4ICSStudio.UIServicesPackage.View.NewPhaseStateDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      @local.clr-namespace:ICSStudio.UIServicesPackage.View     5      Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     5        ? 
ResizeMode$  NoResize=?5       ? Title$' !New Equipment Phase State Routine??65   $	??380??5	   7   $	??260??6C      ICSStudio.Gui.DialogCloser   DialogResult 6P   ?? *DialogResult+ ? 	Resources 5
       ?? 
TargetType ?? Property ?? Value ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %?  '??       '?? w     '??     ?? 5      " ??6   .?5      ??  ? ??$	 180??6*   68   ?? 5       ? ??$ 20??6+   68   5      ?? 5      " ??6   .?5      ??  ? ??$	 180??6*   68   ?? 5       ? ??$ 25??6+   68   ?? 5       ? ??$ Center=?6=   6N   5      ?? 5      " ??6   .?5      ??  ? ??$ 75??6*   67   ?? 5       ? ??$ 23??6+   68   5      5      .??5      ? 	 ? RowDefinitions	 5    
   ?? 5!      6   ?? 5"      $??60 ?6   6*   ?? 5#      6   ?? 5$      6   ?? 5%      6   
 ? ColumnDefinitions
 5(   
   ?? 5)      $
??Auto ?6   6.   ?? 5*      $	??200 ?6   6-   ?? 5+      $??90 ?6   6,   .+?5-   
   ?? $??0??6   $??0??6!   $??State:??61   6A   ?? 5.   
   $??0??6   $??1??6    ??60   ?? *	StateType+ ?? SelectedValue 6R   ?? *SelectedState+6|   ?? 5/   
   $??0??6   $??2??6   $??OK??6.    ?? 	IsDefault . ??6;   ?? *	OkCommand+6l   ?? 50   
   $??1??6   $??0??6!   $??Description:??61   $	??Top=?6E   $??0,8,0,0q?6]   6p   ?? 51   
   $??1??6   $??1??6   " ?? VerticalScrollBarVisibility$ Visible=?6/    ?? TextWrapping$
 Wrap=?6U    ?? AcceptsReturn . $??47??6~   ??6?   ?? *Description+6?   ?? 52   
   $??1??6   $??2??6   $	??Top=?6.   $??Cancel??6F    ?? IsCancel . $??0,8,0,0q?6g   ??6x   ?? *CancelCommand+6?   ?? 53   
   $??2??6   $??0??6!   $??Type:??61   6@   ?? 54   
   $??2??6   $??1??6     ?? DisplayMemberPath$ DisplayName??6Q    ?? SelectedValuePath$ Value??6q   ??60   ?? *
TypeList+ 6?   ?? *SelectedType+6?   ?? 55   
   $??2??6   $??2??6   $
??Help??6.   6?   ?? 56   
   $??3??6   $??0??6!   $??	In Phase:??61   6D   ?? 57   
   $??3??6   $??1??6!   ??61   ?? *ProgramName+6P   ?? 58   
   $??4??6   $??0??6!   $	??Top=?61   $??2??6I   5:      ?? 5;   
   $??4??6   $??0??6    $??0,12,0,0q?61   {?.  $??Open Routine??6U   $??2??6l   6?   5<      5=      