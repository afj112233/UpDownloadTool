   M S B A M L   `   `   `  ????Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui  5      h dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fw@clr-namespace:ICSStudio.UIServicesPackage.AddOnInstruction.Panel2ICSStudio.UIServicesPackage.AddOnInstruction.Panel 5      1   +ICSStudio.UIServicesPackage.View.ChangeType   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     5      Rlocal@clr-namespace:ICSStudio.UIServicesPackage.AddOnInstruction.Panel     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5         Change Language Type  ? Title#  ?   5	       ? 
ResizeMode$ NoResize=?5       ? WindowStartupLocation$ CenterOwner=?5          ICSStudio.Gui.DialogCloser   DialogResult 5      ?? *DialogResult+ ? 	Resources 5       ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??6%   %    5      .??5      ?  ? RowDefinitions 5   
   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $
??Auto ?6   ?? 5      $??* ?6    ? ColumnDefinitions 5    
   ?? 5!      $??50 ?6   ?? 5"      $??* ?6   .+?5$   
   ?? $??0??5%      $??0??5&      $??2??5'      	 ?? Orientation$	 
Horizontal=?5(      .+?5)      ?? $??10q?6   $??Image\Mark1.gif??6    ?? 5*      $	??320??5+        ChangingTypeWillRemoveAny#???  5,      $
??Wrap=?5-      5.      ?? 5/   
   $??1??50      $??0??51      $??10q?52      $
??Bold"?53        From#???  54      ?? 55   
   $??1??56      $??0??57      $??2??58      
???   ?	
 ?? X1$
 45??5:      	 ?? X2$	 370??5;      	 ?? Y1$ 20??5<      	 ?? Y2$ 20??5=      ?? 5>   
   $??2??5?      $??0??5@      $??Right=?5A       	 Type:#???  5B      ?? 5C   
   $??2??5D      $??1??5E      $??30,0,0,0q?5F      ??5G      ?? *	OldType+?? 5I   
   $??3??5J      $??0??5K      $??10q?5L      $
??Bold"?5M        To#???  5N      ?? 5O   
   $??3??5P      $??0??5Q      $??2??5R      
???   ?$
 45??5T      $	 370??5U      $ 20??5V      $ 20??5W      ?? 5X   
   $??4??5Y      $??0??5Z      $??Right=?5[      #???  5\      ?? 5]   
   $??4??5^      $??1??5_      $	??300??5`       ?? DisplayMemberPath$ DisplayName??5a       ?? SelectedValuePath$ Value??5d      ??5b      ?? *
TypeList+ ?? SelectedValue 5c      ?? *Type+5d      ]? 5e   
   $??5??5f      $??0??5g      $??2??5h      ??.  .+?5j      ?? $??75??5k      $??20??5l      $??5,0,12,0q?5m        Help#???  5n      $??Right=?5o      ?? 5p      $??75??5q      $??20??5r      $??5,0,0,0q?5s       
 Cancel#???  5u      $??Right=?5v       ?? IsCancel . ??5t      ?? *CancelCommand+5w      ?? 5x      $??75??5y      $??20??5z        OK#???  5|      $??Right=?5}       ?? 	IsDefault . ??5{      ?? *	OkCommand+5~      5      5?      5?      