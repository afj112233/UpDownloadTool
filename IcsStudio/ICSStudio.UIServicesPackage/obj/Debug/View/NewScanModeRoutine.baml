   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui  5       ICSStudio.UIServicesPackage9   3ICSStudio.UIServicesPackage.View.NewScanModeRoutine   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     5        ? 
ResizeMode$  NoResize=?5         New Scan Mode Routine ? Title# ?   5	       ? WindowStartupLocation$ CenterOwner=?5
          ICSStudio.Gui.DialogCloser   DialogResult 5      ?? *DialogResult+ +? FocusedElement 5      ??  ?? ElementName$ DescripitionBox?? ? 	Resources 5       ?? MergedDictionaries ?? Source	 ?? 
TargetType
 ?? Property ?? Value?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %Y  '??       '?? N     '?? ?     ?? 5      "	 ??6   .?5      ?? 
 ? ??$	 188??6*   68   5      ?? 5      "	 ??6   .?5      ?? 
 ? ??$	 188??6*   68   5      ?? 5      "	 ??6   .?5      ?? 
 ? ??$ 75??6-   6:   ?? 5      
 ? ??$ 75??6*   67   ?? 5      
 ? ??$ 22??6+   68   5      5      .??5!      ?  ? RowDefinitions 5"   
   ?? 5#      $??30 ?6   6*   ?? 5$      $??60 ?6   6*   ?? 5%      $??30 ?6   6*   ?? 5&      $??30 ?6   6*   ?? 5'      $??30 ?6   6*    ? ColumnDefinitions 5)   
   ?? 5*      $
??Auto ?6   6.   ?? 5+      6!   ?? 5,      $
??Auto ?6   6.   .+?5.   
   ?? $??0??6   $??0??6!    	 Mode:#???  61   6R   ?? 5/   
   $??0??6   $??1??6!   ??61   ?? *Mode+6I   ?? 50   
   $??0??6   $??2??6     OK#???  6.    ?? 	IsDefault . ??6M   ?? *	OkCommand+6~   ?? 51   
   $??1??6   $??0??6!     Description:#???  61   $	??Top=?6W   6q   ?? 52   
   -   5         ??Name$ DescripitionBox??52   /   $??1??6   $??1??6    ?? TextWrapping$
 Wrap=?6H   " ?? VerticalScrollBarVisibility$ Visible=?6\   $
??Left=?53      $??55??6-   ??69   ?? *Description+ ?? UpdateSourceTrigger$ PropertyChanged=? ?? Mode$ TwoWay=?6?   ?? 54   
   $??1??6   $??2??6   $	??Top=?6.    
 Cancel#???  6F    ?? IsCancel . ??6y   ?? *CancelCommand+6?   ?? 55   
   $??2??6   $??0??6!    	 Type:#???  61   6R   ?? 56   
   $??2??6   $??1??6    $
??Left=?60   $??22??6K    ?? SelectedValuePath$ Value??57   j    ?? DisplayMemberPath$ DisplayName??6?   ??56   W   ?? *
TypeList+ ?? SelectedValue 57      ?? *SelectedType+$ TwoWay=?$ PropertyChanged=?6?   ?? 58   
   $??2??6   $??2??6     Help#???  6.   6Q   ?? 59   
   $??3??6   $??0??6!     Instruction:#???  61   6Y   ?? 5:   
   $??3??6   $??1??6!   ??61   ?? *Name+6I   ?? 5;   
   $??4??6   $??0??6    $??3??60     Open Routine#???  6D   $??Bottom=?6m   {?.  6?   5<      5=      