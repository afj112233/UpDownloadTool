   M S B A M L   `   `   `  ????h  dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fyAclr-namespace:ICSStudio.UIServicesPackage.ProgramProperties.Panel3ICSStudio.UIServicesPackage.ProgramProperties.Panel  5      I    CICSStudio.UIServicesPackage.ProgramProperties.Panel.EPConfiguration   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      SlocalAclr-namespace:ICSStudio.UIServicesPackage.ProgramProperties.Panel     5      X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5        ? 	Resources  5
       ?? 
TargetType ?? Property ?? Value ?? MergedDictionaries ?? Source?? 5   
    5      ?? 5      $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??5      %5  '??       '?? x     '?? ?     ?? 5      " ??6   .?5      ??  ? ??$ 23??6+   68   ?? 5       ? ??$
 Left=?68   6G   5      ?? 5      " ??6   .?5      ??  ? ??$ 0,8,0,0q?6+   6=   5      ?? 5      " ??6   .?5      ??  ? ??$ 30 ?6+   68   5      5      .??5      ?  ? RowDefinitions 5   
   ?? 5      6   ?? 5      6   ?? 5       6   ?? 5!      6   ?? 5"      6   ?? 5#      6   ?? 5$      6   ?? 5%      6   ?? 5&      6   ?? 5'      6   ?? 5(      6   ?? 5)      $??20 ?6   6*   ?? 5*      $??20 ?6   6*   ?? 5+      $??20 ?6   6*   .+?5-   
   ?? $??0??6      Assigned routines:#???   6!   6O   ?? 5.   
   $??1??6     	Prestate:#???  6!   ??5/      u? 50       u? Left$ 20Q?6   6(   52      ?? 53   
   $??1??6   $??24??6    $	??350??6,    ?? DisplayMemberPath$
 Name??6Y   ??68   ?? *
MainList+	 ?? SelectedValue	 6r   ?? *
SelectMain+??54      u? 55      $	 100Q?6   6)   57      ?? 58   
   $??2??6    
 Fault:#???  6!   ??59      u? 5:      $ 20Q?6   6(   5<      ?? 5=   
   $??2??6   $??24??6    $	??350??6,   $
 Name??6Z   ??68   ?? *	FaultList+	 6s   ?? *SelectFault+??5>      u? 5?      $	 100Q?6   6)   5A      ?? 5B   
   $??3??6     Inhibit equipment phase#???  6@   
 d? 	IsChecked
 6    ?? *	Inhibited+6v   ?? 5D   
   $??4??6     Initial state:#???  6!   6K   ?? 5E   
   $??4??6   $	??160??6    $ DisplayName??6?    ?? SelectedValuePath$ Value??6?   ??6,   ?? *InitialStateList+	 6U   ?? *SelectedInitialState+??5F      u? 5G      $	 100Q?6   6)   5I      ?? 5J   
   $??5??6    8 4Complete state immediately if not implement or empty#???  6M   
 6    ?? *CompleteStateIfNotImpl+6?   ?? 5K   
   $??6??6     Initial step index:#???  6!   6P   $  Xceed.Wpf.Toolkit.SingleUpDown  5L   
   $??6??6   $??Center=?6*   $	??120??6E   $
??Left=?6Q   $??0,-2,0,0q?6l   ?  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .    ShowButtonSpinner .    Minimum$ 0X?5N         Maximum$ 65535X?6)   ?  ?Xceed.Wpf.Toolkit.NumericUpDown`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	Increment$ 1X?69   ,  &Xceed.Wpf.Toolkit.Primitives.InputBase   Text 5M   G   ?? *InitialStepIndex+ ?? UpdateSourceTrigger$ PropertyChanged=???5O      u? 5P      $	 200Q?6   6)   5R      ?? 5S   
   $??7??6   $	??195??6!   $
??Left=?6-    5 1External sequencer loss of communication command:#???  6H   $
??Wrap=?6?   6?   ?? 5T   
   $??7??6   $	??120??6    $ DisplayName??6V   $ Value??6v   ??6,   ?? *LossOfCommCmdList+	 6?   ?? *SelectedLossOfCommCmd+??5U      u? 5V      $	 200Q?6   6)   5X      ?? 5Y   
   $??8??6    ! External request hold action:#???  6!   6Z   ?? 5Z   
   $??8??6   $	??120??6    $ DisplayName??6^   $ Value??6~   ??6,   ?? *ExternalRequestActionList+	 6?   ?? *SelectedExternalRequestAction+??5[      u? 5\      $	 200Q?6   6)   5^      ?? 5_   
   $??9??6    K	 GAutomatically assign the values of sequence step inputs to phase inputs#??? 	 6Q   
 6    ?? *AutoValueAssignStepToPhase+6?   ?? 5`   
   $??10??6    `
 \Automatically assign the values of phase outputs to sequence step outputs when the phase is:#??? 
 6"   $
??Wrap=?6?   6?   ?? 5a   
   $??11??6   $??0,3,0,0q?6!     Complete#???  6m   
 62   ?? *&$AutoValueAssignPhaseToStepOnComplete+6?   ?? 5b   
   $??12??6   $??0,3,0,0q?6!     Stopped#???  6l   
 62   ?? *%#AutoValueAssignPhaseToStepOnStopped+6?   ?? 5c   
   $??13??6   $??0,3,0,0q?6!     Aborted#???  6l   
 62   ?? *%#AutoValueAssignPhaseToStepOnAborted+6?   5d      5e      