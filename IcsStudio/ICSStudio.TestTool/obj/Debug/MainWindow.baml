   M S B A M L   `   `   `  ????  ICSStudio.TestTool#    ICSStudio.TestTool.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   h dMicrosoft.Expression.Interactions, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bei9http://schemas.microsoft.com/expression/2010/interactions  5   	     ? WindowState$  	Maximized=?5	   7   $	??600??6O   $	??800??6_    ? WindowStartupLocation$ CenterScreen=?6n    ? Title 6	   ??  ?? Mode$ OneTime=? ?? Path$ ICSTitle?.??5
      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      ?? 5      $
??Auto ?6   .+?5   
   ??  ?? Orientation$ 
Horizontal=?6   $??Center=?6.   .+?5      ?? $??设备IP??6   $??Center=?6$   $??10,10q?6?   ?? 5      $??Center=?6*   $??25??6E   $	??150??6Q   $??Center=?6]   $??10,10,20,10q?6   ??6   ?? *IP+6   ?? 5      $??测试集路径??6   $??Center=?6%   $??40,10,20,10q?6@   ?? 5      $??25??62   $	??250??6>    ?? 
IsReadOnly . $??Center=?6\   ??6   ?? *
TestFolder+6\   ?? 5      $	??...??6   $??20??6#   $??20??6.   $??5,0,10,0q?6:   ??6L   ?? *SelectPathCommand+{?6r   ?? *	IsCanTest+5      ?? 5   
   $??开始测试??6   $??80??6    $??30??6+   $??Right=?67   $??10,10q?6S   ??6b   ?? *StartTestCommand+{?6?   ?? *	IsCanTest+?? 5   
   -   5         ??Name$ LogBox??5      $??1??6"   	 ?? TextWrapping$
	 Wrap=?6/   $??	10,0,10,0q?6\    . "
 ?? VerticalScrollBarVisibility$

 Auto=?6?   ??6C   ?? *	LogText+.??6?   .  (System.Windows.Interactivity.Interaction   Triggers 5      9  3Microsoft.Expression.Interactivity.Core.DataTrigger  5         
Comparison$ NotEqual=?6U      Value$ 0??6k   D  >Microsoft.Expression.Interactivity.Core.PropertyChangedTrigger   Binding 6!   ?? *Text.Length+ ?? ElementName$ LogBox??.  (System.Windows.Interactivity.TriggerBase   Actions. 5      >  8Microsoft.Expression.Interactivity.Core.CallMethodAction     
MethodName$ ScrollToEnd??6*   5      .??5    &   5!      ?? 5"   
   $??2??6   $??清空??6   $??10q?6+   $??80??67   $??30??6B   $??Right=?6N   ??6j   ?? *ClearLogCommand+5#      5$      