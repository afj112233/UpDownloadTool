   M S B A M L   `   `   `  ???? h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fS.clr-namespace:ICSStudio.UIServicesPackage.View ICSStudio.UIServicesPackage.View  Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui d `ICSStudio.EditorPackage, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=262b451c406b2266?Wclr-namespace:ICSStudio.EditorPackage.Reference.Extend;assembly=ICSStudio.EditorPackage(ICSStudio.EditorPackage.Reference.Extend d `DotNetProjects.Input.Toolkit, Version=6.1.91.0, Culture=neutral, PublicKeyToken=79778c5f2eed289bgKclr-namespace:System.Windows.Controls;assembly=DotNetProjects.Input.ToolkitSystem.Windows.Controls ^ ZICSStudio.Dialogs, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=8558c3dcb301e1d4^Aclr-namespace:ICSStudio.Dialogs.Filter;assembly=ICSStudio.DialogsICSStudio.Dialogs.Filter a ]ICSStudio.Components, Version=1.0.106.22348, Culture=neutral, PublicKeyToken=65b8cea88eb98737kIclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.ComponentsICSStudio.Components.Controls 8    2ICSStudio.UIServicesPackage.View.AddOrConfigureTag   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R	 NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml    	 Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006    	 @d2http://schemas.microsoft.com/expression/blend/2008    	 @local.clr-namespace:ICSStudio.UIServicesPackage.View    	 Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui    	 jextendWclr-namespace:ICSStudio.EditorPackage.Reference.Extend;assembly=ICSStudio.EditorPackage    	 `controlsKclr-namespace:System.Windows.Controls;assembly=DotNetProjects.Input.Toolkit    	 TfilterAclr-namespace:ICSStudio.Dialogs.Filter;assembly=ICSStudio.Dialogs    	 _	controls1Iclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.Components    	 $	??445??$	??325??  ? 
ResizeMode$  NoResize=? ? Title ?? *Title+   ICSStudio.Gui.DialogCloser   DialogResult ?? *DialogResult+ ? 	Resources  ?? MergedDictionaries ?? Source??  ?? $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??%    .??? -    ?Name$ TopGrid??$??10q? ? RowDefinitions ?? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ?.+??? $??0?? 
  Scope:#???   ?? $??1??$	??231??$??24??$
??Left=? ?? DisplayMemberPath$
 Name?????? *	ScopeList+	 ?? SelectedItem	 ?? *SelectedScope+?? $??2??  SelectVariable#???  ?? $??3??
 ?? Orientation$
 
Horizontal=?.+?? $??25??$	??318??$??0,0,0,0q?.+?;  5ICSStudio.Components.Controls.FastAutoCompleteTextBox  -   $	??318??$??0,0,20,0q?$??23??-  'System.Windows.Controls.AutoCompleteBox   MaxDropDownHeight$ 0Q?   IsTextCompletionEnabled .    Text ?? *	TagName+ ?? UpdateSourceTrigger$ PropertyChanged=? ?? Mode$ TwoWay=?   Data ?? *
NameList+y??? *FilterVisibility+?? $	??Top=?$??18??
?????? 
?????? $??Right=?$??18??$??18?????? *NameFilterCommand+????  ?? RelativeSource ?? *Self+??F? "????     	= ?@KL @T? р??  	= 	
???   ?.  (ICSStudio.Dialogs.Filter.NameFilterPopup  -     Name$ NameFilterPopup????   Add#???  $??75??$??23??$??10,0,0,0q????? *
AddCommand+?? $??4??  VariableToTrend#???  ?? $??5??$
 
Horizontal=?.+??? $	??318??$	??122??$??0q????? *TagsToTrend+ ?? SelectedValue ?? *SelectedTag+??  
 Remove#???  $??75??$??23??$	??Top=?$??10,0,0,0q????? *RemoveCommand+?? $??6??$
 
Horizontal=?$??Right=?.+??? $??75??$??23??$??23??  OK#???  ???? *	OKCommand+?? $??75??$??23??$??23?? 
 Cancel#???   ?? IsCancel . ???? *CancelCommand+?? $??75??$??23??$??23??  Help#???  