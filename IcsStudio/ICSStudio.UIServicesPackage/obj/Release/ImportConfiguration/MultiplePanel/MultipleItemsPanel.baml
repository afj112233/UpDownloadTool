   M S B A M L   `   `   `  ????   ICSStudio.UIServicesPackageV    PICSStudio.UIServicesPackage.ImportConfiguration.MultiplePanel.MultipleItemsPanel   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006       ? 	Resources   ?? MergedDictionaries ?? Source??  ?? $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??%    .??? .+?? y??? *" ItemVisibility.ProgramVisibility+   ?? 
TargetType ?? Property ?? Value??%?   '??       '?? R     ?? " ??.???  ? ??$	 200????  ? ??$ 25????  ? ??$
 Left=??? " ??.???  ? ??$	 200????  ? ??$ 25?? ? RowDefinitions ?? $??35 ??? $??35 ??? $??35 ??? $??35 ??? $??35 ??? $??35 ??? $
??Auto ? ? ColumnDefinitions ?? $
??Auto ??? .+???    Imported Programs:#???   ?? $??0??$??1??$
??Left=????? *ImportedItemsTitle+?? $??1??$??0??  
Operation:#???  ?? $??1??$??1??$
??Left=? ?? SelectedIndex$ 0?????? *OperationItems+	 ?? SelectedItem	 ?? *SelectedOperation+???? .R??? ???? ?? $??2??$??0??  Schedule In:#???  ?? $??2??$??1?????? *	TaskItems+	 ?? *SelectedTask+?? $??3??$??1??$
??Left=?$??Center=? 1 -Do not change scheduling of existing programs#???  
 d? 	IsChecked
 ?? *-+IsChangeExistingPrograms.IsChangeScheduling+{??? *42IsChangeExistingPrograms.IsEnabledChangeScheduling+?? $??4??$??0??  Parent:#???  ?? $??4??$??1?????? *ParentItems+	 ?? *SelectedParent+?? $??5??$??1??$??Center=? 4 0Do not change parent of existing target programs#???  
 ?? *)'IsChangeExistingPrograms.IsChangeParent+{??? *0.IsChangeExistingPrograms.IsEnabledChangeParent+? y??? *ItemVisibility.AoiVisibility+ ?? $
??Auto ??? $
??Auto ?.+??  ?? $
??Auto ??? $
??Auto ?.+???   Imported Instructions:#???  ?? $??1??$
??10,0q????? *ImportedItemsTitle+?? $??1??$
??0,10q????#87CEEB ^ ZInstructions and other references will be imported as configured in the References folders#???  ? y??? */-ItemVisibility.UserDefinedOrStringsVisibility+ ?? $
??Auto ??? $
??Auto ?.+??  ?? $
??Auto ??? $
??Auto ?.+???   Imported Type:#???  ?? $??1??$
??10,0q????? *ImportedItemsTitle+?? $??1??$
??0,10q????#87CEEB \	 XData Types and other references will be imported as configured in the References folders#??? 	 