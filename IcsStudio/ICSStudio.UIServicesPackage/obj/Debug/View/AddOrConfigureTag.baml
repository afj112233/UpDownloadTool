   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fS.clr-namespace:ICSStudio.UIServicesPackage.View ICSStudio.UIServicesPackage.View  5      Z VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui 5      d `ICSStudio.EditorPackage, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=262b451c406b2266?Wclr-namespace:ICSStudio.EditorPackage.Reference.Extend;assembly=ICSStudio.EditorPackage(ICSStudio.EditorPackage.Reference.Extend 5      d `DotNetProjects.Input.Toolkit, Version=6.1.91.0, Culture=neutral, PublicKeyToken=79778c5f2eed289bgKclr-namespace:System.Windows.Controls;assembly=DotNetProjects.Input.ToolkitSystem.Windows.Controls 5	      ^ ZICSStudio.Dialogs, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=8558c3dcb301e1d4^Aclr-namespace:ICSStudio.Dialogs.Filter;assembly=ICSStudio.DialogsICSStudio.Dialogs.Filter 5
      a ]ICSStudio.Components, Version=1.0.106.22348, Culture=neutral, PublicKeyToken=65b8cea88eb98737kIclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.ComponentsICSStudio.Components.Controls 5      8    2ICSStudio.UIServicesPackage.View.AddOrConfigureTag   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R	 NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml    	 5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006    	 5      @d2http://schemas.microsoft.com/expression/blend/2008    	 5      @local.clr-namespace:ICSStudio.UIServicesPackage.View    	 5      Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui    	 5      jextendWclr-namespace:ICSStudio.EditorPackage.Reference.Extend;assembly=ICSStudio.EditorPackage    	 5      `controlsKclr-namespace:System.Windows.Controls;assembly=DotNetProjects.Input.Toolkit    	 5	      TfilterAclr-namespace:ICSStudio.Dialogs.Filter;assembly=ICSStudio.Dialogs    	 5
      _	controls1Iclr-namespace:ICSStudio.Components.Controls;assembly=ICSStudio.Components    	 5      $	??445??5   7   $	??325??6C     ? 
ResizeMode$  NoResize=?6P    ? Title 6f   ?? *Title+   ICSStudio.Gui.DialogCloser   DialogResult 5      ?? *DialogResult+ ? 	Resources 5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %    5      .??5      ? -   5         ?Name$ TopGrid??5      $??10q?6    ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   ?? 5      $
??Auto ?6   6,   .+?5!   
   ?? $??0??6    
  Scope:#???   6!   6C   ?? 5#   
   $??1??6   $	??231??6    $??24??6,   $
??Left=?68    ?? DisplayMemberPath$
 Name??6S   ??6l   ?? *	ScopeList+	 ?? SelectedItem	 6?   ?? *SelectedScope+6?   ?? 5%   
   $??2??6     SelectVariable#???  6!   6K   ?? 5'   
   $??3??6   
 ?? Orientation$
 
Horizontal=?6"   .+?5(      ? $??25??6   $	??318??6   $??0,0,0,0q?6+   .+?5)      ;  5ICSStudio.Components.Controls.FastAutoCompleteTextBox  -   5        $	??318??5*      $??0,0,20,0q?6!   $??23??5+      -  'System.Windows.Controls.AutoCompleteBox   MaxDropDownHeight$ 0Q?5/         IsTextCompletionEnabled .    Text 5-      ?? *	TagName+ ?? UpdateSourceTrigger$ PropertyChanged=? ?? Mode$ TwoWay=?   Data 5.      ?? *
NameList+y?51      ?? *FilterVisibility+6?   ?? 52      $	??Top=?6   $??18??61   
?????? 
?????? $??Right=?53      $??18??69   $??18??6D   ??54      ?? *NameFilterCommand+??55      ??  ?? RelativeSource ?? *Self+??56      F? 57      "????     	= ?@KL @T? р??  	= 	
???   ?6E   59      .  (ICSStudio.Dialogs.Filter.NameFilterPopup  5:      -   5          Name$ NameFilterPopup??5:   *   6E   5;      ?? 5<        Add#???  6   $??75??65   $??23??6@   $??10,0,0,0q?6L   ??6^   ?? *
AddCommand+6   5=      ?? 5?   
   $??4??6     VariableToTrend#???  6!   6L   ?? 5A   
   $??5??6   $
 
Horizontal=?6"   .+?5B      ?? $	??318??6   $	??122??6#   $??0q?60   ??6;   ?? *TagsToTrend+ ?? SelectedValue 6_   ?? *SelectedTag+6?   ?? 5C       
 Remove#???  6   $??75??68   $??23??6C   $	??Top=?6O   $??10,0,0,0q?6g   ??6y   ?? *RemoveCommand+6?   5D      ?? 5F   
   $??6??6   $
 
Horizontal=?6"   $??Right=?6;   .+?5G      ?? $??75??6   $??23??6    $??23??6,     OK#???  6;   ??6Z   ?? *	OKCommand+6z   ?? 5H      $??75??6   $??23??6    $??23??6,    
 Cancel#???  6;    ?? IsCancel . ??6n   ?? *CancelCommand+6?   ?? 5I      $??75??6   $??23??6    $??23??6,     Help#???  6;   6^   5J      5K      5L      