   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui  5      a ]ICSStudio.Components, Version=1.0.106.22348, Culture=neutral, PublicKeyToken=65b8cea88eb98737kIclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.ComponentsICSStudio.Components.Controls 5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U<clr-namespace:System.Windows.Input;assembly=PresentationCoreSystem.Windows.Input 5       ICSStudio.UIServicesPackage7   1ICSStudio.UIServicesPackage.View.NewRoutineDialog   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     5      ^controlsIclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.Components     5      Ninput<clr-namespace:System.Windows.Input;assembly=PresentationCore     5      -   5          ? 
ResizeMode$  NoResize=?5	       $	??270??5
      $	??390??6      New Routine ? Title# ?   6'       ICSStudio.Gui.DialogCloser   DialogResult 5      ?? *DialogResult+ ? 	Resources 5       ?? 
TargetType ?? Property ?? Value ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %?  '??       '?? w     '??     ?? 5      " ??6   .?5      ??  ? ??$	 180??6*   68   ?? 5       ? ??$ 20??6+   68   5      ?? 5      " ??6   .?5      ??  ? ??$	 180??6*   68   ?? 5       ? ??$ 25??6+   68   ?? 5       ? ??$ Center=?6=   6N   5      ?? 5      " ??6   .?5      ??  ? ??$ 75??6*   67   ?? 5       ? ??$ 23??6+   68   5      5       .??5"      ? 	 ? RowDefinitions	 5#   
   ?? 5$      6   ?? 5%      $??60 ?6   6*   ?? 5&      6   ?? 5'      6   ?? 5(      6   ?? 5)      6   
 ? ColumnDefinitions
 5+   
   ?? 5,      $
??Auto ?6   6.   ?? 5-      $	??200 ?6   6-   ?? 5.      $??90 ?6   6,   .+?50   
   ?? $??0??6   $??0??6!    	 Name:#???  61   $??0,0,0,0q?6P   6c   7  1ICSStudio.Components.Controls.ValidateNameControl  51   
   -   5          Name$ NameValidateNameControl??51   D   $??0??6'   $??1??64    ?? 	MaxLength$ 40??6e    ?? IsInputMethodEnabled .  $??24??6?   $	??180??6?   ??6t   ?? *Name+ ?? Mode$ TwoWay=? ?? UpdateSourceTrigger$ PropertyChanged=?6  ?? 52   
   $??0??6   $??2??6     OK#???  6.    ?? 	IsDefault . ??6M   ?? *	OkCommand+6~   ?? 53   
   $??1??6   $??0??6!     Description:#???  61   $	??Top=?6W   $??0,8,0,0q?6o   6?   ?? 54   
   $??1??6   $??1??6   " ?? VerticalScrollBarVisibility$ Visible=?6/    ?? TextWrapping$
 Wrap=?6U    ?? AcceptsReturn . $??47??6~   ??6?   ?? *Description+6?   ?? 55   
   $??1??6   $??2??6   $	??Top=?6.    
 Cancel#???  6F    ?? IsCancel . $??0,8,0,0q?6y   ??6?   ?? *CancelCommand+6?   ?? 56   
   $??2??6   $??0??6!    	 Type:#???  61   6R   ?? 57   
   $??2??6   $??1??6     ?? DisplayMemberPath$ DisplayName??6Q    ?? SelectedValuePath$ Value??6q   ??60   ?? *
TypeList+ ?? SelectedValue 6?   ?? *SelectedType+6?   ?? 58   
   $??2??6   $??2??6     Help#???  6.   6Q   ?? 59   
   $??3??6   $??0??6!     In Program or Phase:#???  61   6a   ?? 5:   
   $??3??6   $??1??6    $ Value??6~   $ DisplayName??6?   ??60   ?? *ProgramList+ 6T   ?? *SelectedProgram+6?   ?? 5;   
   $??4??6   $??1??6!     Assignment:#???  61   6X   ?? 5<   
   $??4??6   $??1??6    $	??110??60   $??Right=?6<   $??0,0,10,0q?6X   ??6j   ?? *AssignmentList+ 6?   ?? *SelectedAssignment+6?   ?? 5=   
   $??5??6   $??0??6!   $	??Top=?61   $??2??6I   5?      ?? 5@   
   $??5??6   $??0??6    $??0,8,0,0q?61   {?.   	 Open Routine#??? 	 6T   $??3??6}   6?   5A      5B      