   M S B A M L   `   `   `  ????a  ]ICSStudio.Components, Version=1.0.106.22348, Culture=neutral, PublicKeyToken=65b8cea88eb98737kIclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.ComponentsICSStudio.Components.Controls  5       ICSStudio.UIServicesPackage=   7ICSStudio.UIServicesPackage.TagProperties.Panel.General   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      _	controls1Iclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.Components     5      -   5          ? 	Resources  5
       ?? 
TargetType ?? Property ?? Value?? 5   
   %?  '??       '?? ?     '?? M    '?? ?    '?? *    ?? 5      " ??6   .?5      ??  ? ??$
 Left=?68   ?? 5       ? ??$ Center=?66   ?? 5       ? ??$ 22??6+   ?? 5       ? ??$ 11(?6-   5      ?? 5      " ??6   .?5      ??  ? ??$
 Left=?68   ?? 5       ? ??$	 188??6*   ?? 5       ? ??$ 22??6+   5      ?? 5      " ??6   .?5      ??  ? ??$
 Left=?68   5      ?? 5      " ??6   .?5      ??  ? ??$
 Left=?68   ?? 5       ? ??$	 188??6*   ?? 5       ? ??$ 22??6+   5      ?? 5      " ??6   .?5       ??  ? ??$ 22??6+   ?? 5!       ? ??$ 11(?6-   5"      5#      .??5%      ?  ? RowDefinitions 5&   
   ?? 5'      ?? 5(      $
??Auto ?6   ?? 5)      ?? 5*      ?? 5+      ?? 5,      ?? 5-      ?? 5.      ?? 5/      ?? 50      ?? 51      ?? 52      $??30 ?6    ? ColumnDefinitions 54   
   ?? 55      $??80 ?6   ?? 56      .+?59   
   ??    New Tag Name:#???   6   $??0??5:      $??0??5;      7   1ICSStudio.Components.Controls.ValidateNameControl  5=   
   -   5          Name$ NameBox??5>   (   $??0??5=   (   $??1??65   $	??188??5>   9   $??22??6E   $
??Left=?6Q   ??5?   (   ?? *Name+ ?? UpdateSourceTrigger$ PropertyChanged=?{?5@   (   ?? *NameIsEnabled+?? 5B   
   $??1??6   $??0??5C        New Tag Description:#???  5D      ?? 5E   
   $??1??6   $??1??5F      $	??100??5G      $	??Top=?5H       ?? TextWrapping$
 Wrap=?5I      "	 ?? VerticalScrollBarVisibility$	 Visible=?5J      ??5K      ?? *Description+$ PropertyChanged=?{?5L      ?? *IsDescriptionEnabled+?? 5M   
   $??2??6   $??0??5N       
 Usage:#???  5O      ?? 5P   
   $??2??6   $??1??5Q      
 ?? DisplayMemberPath$
 DisplayName??5R   2    ?? SelectedValuePath$ Value??5S      ??5R      ?? *Usages+ ?? SelectedValue 5T      ?? *Usage+{?5U      ?? *UsageEnabled+$ PropertyChanged=??? 5V   
   $??3??6   $??0??5W        New Tag Type:#???  5X      ]? 5Y   
   $??3??6   $??22??5Z      $	??188??5[      $
??Left=?5\      $??1??5]      .+?5^   
   ?? $	??111??6   $??22??6   $
 Display??5`      $ Value??5a      $
??Left=?5b      ??5_      ?? *Types+ 5c      ?? *Type+{?5d      ?? *TypeEnabled+$ PropertyChanged=??? 5e   
   $??75??6   $??Right=?5f        Connection...#???  5g      {?.  5h      5i      ?? 5k   
   $??4??6   $??0??5l        
Alias For:#???  5m      ?? 5n   
   $??4??6   $??1??5o      {?.  5p      ?? 5r   
   $??5??6   $??0??5s        
Data Type:#???  5t      ]? 5u   
   $??5??6   $??1??5v      $	??188??5w      $
??Left=?5x      .+?5y   
   ;   5ICSStudio.Components.Controls.FastAutoCompleteTextBox  $	??156??6,   $??22??5z   +   $
??Left=?5|   +   d `DotNetProjects.Input.Toolkit, Version=6.1.91.0, Culture=neutral, PublicKeyToken=79778c5f2eed289b-  'System.Windows.Controls.AutoCompleteBox   IsTextCompletionEnabled .    MaxDropDownHeight$ 0Q?5   +   $
??Left=?5?   +      Text 5{   +   ?? *
DataType+$ PropertyChanged=? ?? Mode$ TwoWay=?{?5}   +   ?? *DataTypeIsEnabled+   ItemsSource 5?   +   ?? *AutoCompleteSource+5?   +   ?? 5?   
   $??30??6   $??22??6   $	??...??5?      $??Right=?5?      ??5?      ?? *DataTypeCommand+5?      ?? 5?   
   $??6??6   $??0??5?        New Tag Scope:#???  5?      ?? 5?   
   $??6??6   $??1??5?      ??5?      ?? *	Program+?? 5?   
   $??7??6   $??0??5?      $??30??5?        External Access:#???  5?      $??WrapWithOverflow=?5?      ?? 5?   
   $??7??6   $??1??5?      $
 Display??5?      $ Value??5?      ??5?      ?? *ExternalAccesses+ 5?      ?? *ExternalAccess+{?5?      ?? *ExternalAccessEnabled+$ PropertyChanged=??? 5?   
   $??8??6   $??0??5?       
	 Style:#??? 	 5?      ?? 5?   
   $??8??6   $??1??5?      ??5?      ?? *Styles+ 5?      ?? *Style+{?5?      ?? *StyleEnabled+$ PropertyChanged=??? 5?   
   $??9??6   $??0??5?       ?? Orientation$ 
Horizontal=?5?      $??Center=?5?      .+?5?      ?? $??Bottom=?5?       
 New Tag Constant#??? 
 5?      {?5?      ?? *ConstantEnabled+ d? 	IsChecked 5?      ?? *
IsConstant+5?      5?      ?? 5?   
   $??10??6   $??0??5?      $??2??5?      $ 
Horizontal=?5?      $	??Top=?5?      .+?5?      ?? $??Center=?5?        Open Parameter Connections#???  5?      {?5?      ?? *!OpenParameterConnectionsEnabled+ 5?      ?? *IsOpenParameterConnections+5?      5?      5?      5?      