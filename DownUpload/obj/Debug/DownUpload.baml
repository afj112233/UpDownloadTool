   M S B A M L   `   `   `  ????\  XICSGateway.CommonView, Version=1.0.0.0, Culture=neutral, PublicKeyToken=a8c224afb4531086rMclr-namespace:ICSGateway.CommonView.DeviceTree;assembly=ICSGateway.CommonView ICSGateway.CommonView.DeviceTree  5   	   E ADownUpload, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:DownUpload
DownUpload 5   	      DownUpload.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   d
deviceTreeMclr-namespace:ICSGateway.CommonView.DeviceTree;assembly=ICSGateway.CommonView     5   	   *localclr-namespace:DownUpload     5   	     ? 
ResizeMode$  NoResize=?5	   	    ? Title$ 
DownUpload??5
   	   $	??500??6   $	??600??6)   .??5      ? $??10q?6    ? 	Resources 5   
    ?? 
TargetType ?? Property ?? Value?? 5      %?   '??       ?? 5      " ??6   .?5      ??  ? ??$ 75??6.   ?? 5       ? ??$ 23??6/   ?? 5       ? ??$	 0,8q?6/   5      5       ? ColumnDefinitions 5   
   ?? 5      $??2* ?6   ?? 5      $??2* ?6   ?? 5       ? RowDefinitions 5   
   ?? 5      $??0.75* ?6   ?? 5      ?? 5      $
??1.5* ?6   ?? 5      ?? 5      .+?5!   
   ??  ?? Orientation$ 
Horizontal=?6   .+?5"      ?? $??25??6   $	??0,4q?6$   $	??Top=?61   测试次数：6P   ?? 5#      $??20??6   $??80??6"   $	??0,2q?6-   $	??Top=?6:   ??6S   ?? *Times+5$      ?? 5%   
   $??0??6   $??1??6"   $ 
Horizontal=?62   .+?5&      ?? $??25??6   $	??5,4q?6$   $	??Top=?61   IP 地址：6R   ?? 5'      $??25??6   $	??0,4q?6$   $	??Top=?61   $??Right=?6I   ??6e   ?? *	IpAddress+	 ?? UpdateSourceTrigger$	 PropertyChanged=?6?   5(      ?? 5)   
   $ 
Horizontal=?6   $??0??6.   $??2??6;   .+?5*      ?? $??25??6   $??0,5,0,0q?6$   $??Bottom=?65   文件路径：6W   ?? 5+      $??25??6   $??0,5,0,0q?6$   $??Bottom=?65   ??6P   ?? *
FileName+5,      1   +ICSGateway.CommonView.DeviceTree.DeviceTree  5-   
   $??1??6    $??0??6-   $??2??6=   $??2??6N   !
   SelectedItemChangedCommand
 6b   ?? *SelectIPCommand+6?   ?? 5.   
   $??3??6   $??2??6    $??3??61   $??Message??6E   
???????$??Visible=?6o   .??5/      ?? $
??Auto=?6   .??50      ?? $??12(?69   $
??Wrap=?6G   ??6   ?? *TestMessage+6^   51      52      ?? 53   
   $??2??6   $??3??6%   .+?54      ?? ??6   ?? *OpenCommand+.??65   Open6;   ?? 55      ??6   ?? *UploadCommand+.??67   Upload6?   ?? 56      ??6   ?? *DownloadCommand+.??69   
Download6C   ?? 57      ??6   ?? *ClearCommand+.??66   Clear6=   ?? 58      $
??15,0q?6   .??6"   时间间隔：6)   ?? 59      $ 
Horizontal=?6   $??23??62   .+?5:      ?? $??75??6   $??20,0,0,0q?6%   ??67   ?? *
TimeSpan+6S   ?? 5;      $	??2,2q?6   s6,   5<      5=      5>      5?      