   M S B A M L   `   `   `  ???? Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05V;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.GuiICSStudio.Gui.Controls   ICSStudio.UIServicesPackageK   EICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel.ScalingPanel   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Lutil;clr-namespace:ICSStudio.Gui.Controls;assembly=ICSStudio.Gui     X TXceed.Wpf.Toolkit, Version=4.2.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit    ? 	Resources   ?? MergedDictionaries ?? Source ?? 
TargetType ?? Property ?? Value3   -ICSStudio.Gui.Controls.SingleUpDownExtensions??  ?? $R Lpack://application:,,,/ICSStudio.Gui;component/Resources/styles/Default.xaml??%b   '??       '  '     ?? " ??.???  ? ??$ Center=??? "  .???  ? ??$ Center=?.???  ? RowDefinitions ?? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ?.+?? $??0??$??0q? ? ColumnDefinitions ?? $	??100 ??? $
??Auto ??? $	??200 ??? $
??Auto ?.+??? $??0??   
Load Type:#???   ?? -    ??Name$ CmbLoadType??$??1??$	??160??	 ?? SelectedValuePath$	 Value??
 ?? DisplayMemberPath$
 DisplayName?????? *LoadTypeSource+ ?? SelectedValue ?? *
LoadType+{??? *	IsOffline+?? $	??200??$??3??  Parameters...#???  ???? *ParametersCommand+? $??1??$??0q? ?? $
??Auto ??? $??* ?.+??? $??0??  Transmission#???  $
??Bold"??? $??1??? $??2??$??0q?{??? *TransmissionEnabled+ ?? $??20 ??? $??80 ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ?.+??? $??1??  
Ratio I:O:#???  %  Xceed.Wpf.Toolkit.IntegerUpDown  $??2??$	??100???  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .     Minimum$ 1X?   Maximum$ 
2147483647X?,  &Xceed.Wpf.Toolkit.Primitives.InputBase   TextAlignment$
 Left=?   Value ?? *TransmissionRatioInput+?? $??3??.??:  $??4??$	??100?? .   .  $ 1X?$ 
2147483647X?$
 Left=? ?? *TransmissionRatioOutput+?? $??5??  Rev#???  ? $??3??$??0q? ?? $
??Auto ??? $??* ?.+??? $??0??  Actuator#???  $
??Bold"??? $??1??? $??4??$??0q? ?? $
??Auto ??? $
??Auto ??? $
??Auto ? ?? $??20 ??? $??80 ??? $
??Auto ??? $
??Auto ??? $
??Auto ?.+??? $??0??$??1??  CIPTag Scaling Type:#???  {??? *ActuatorEnabled+?? $??1??$??1?? 	 Lead:#???  {??? *ActuatorLeadEnabled+?? $??2??$??1??  	Diameter:#???  {??? *ActuatorDiameterEnabled+?? -   $ CmbActuatorType??$??0??$??2??$??3??$??0q?$	??150??$
??Left=?$	 Value??$
 DisplayName??{??? *ActuatorEnabled+???? *ActuatorTypeSource+ ?? *ActuatorType+  $??1??$??2??$	??120??$
 Left=??  ?Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   	AllowSpin .     ShowButtonSpinner .  {??? *ActuatorLeadEnabled+   Value ?? *ActuatorLead+  $??2??$??2??$	??120??$
 Left=? .   .  {??? *ActuatorDiameterEnabled+ ?? *ActuatorDiameter+?? -   $ CmbActuatorLeadUnit??$??1??$??4??$??10 0 0 0q?$	??120??$	 Value??$
 DisplayName??{??? *ActuatorLeadEnabled+???? *ActuatorLeadUnitSource+ ?? *ActuatorLeadUnit+?? -   $ CmbActuatorDiameterUnit??$??2??$??4??$??10 0 0 0q?$	??120??$	 Value??$
 DisplayName??{??? *ActuatorDiameterEnabled+???? *ActuatorDiameterUnitSource+ ?? *ActuatorDiameterUnit+? $??5??$??0q? ?? $
??Auto ??? $??* ?.+??? $??0?? 	 AxisPropertiesScaling#??? 	 $
??Bold"??? $??1??? $??6??$??0q? ?? $
??Auto ??? $
??Auto ? ?? $??20 ??? $??80 ??? $
??Auto ?.+??? $??0??$??1?? 
 CIPTag Units:#??? 
 ?? $??1??$??1??  Scaling:#???  ?? -    ??Name$ TxtPositionUnits??$??0??$??2??$	??200??$??0q?$
??Left=? ?? 	MaxLength$ 15?????? *PositionUnits+ ?? UpdateSourceTrigger$ PropertyChanged=? ?? Delay$ 30??{??? *PositionUnitsEnabled+]? $??1??$??2??.+?  $	??120??$
 Left=? .   .   ?? *PositionScalingNumerator+{??? *!PositionScalingNumeratorEnabled+?? $	??120??????  ?? ElementName$ TxtPositionUnits?? ?? Path$
 Text??? -   $ CmbMotionUnit??$??Right=?{?.  $??10 0 0 0q?$	??120??$	 Value??$
 DisplayName?????? *MotionUnitSource+ ?? *
MotionUnit+  $??Right=?$	??120??$
 Left=? .   .   ?? *PositionScalingDenominator+{??? *#!PositionScalingDenominatorEnabled+?? $??Right=?  Motor Rev per#???  ? $??7??$??0q? ?? $
??Auto ??? $??* ?.+??? $??0?? 
 Travel#???  $
??Bold"??? $??1??? $??8??$??0q? ?? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ? ?? $??20 ??? $??80 ??? $
??Auto ?.+??? $??0??$??1??  CIPTag Mode:#???  ?? $??1??$??1?? 
 Range:#???  y??? *TravelRangeVisibility+{??? *TravelRangeEnabled+?? $??2??$??1??  Unwind:#???  y??? *PositionUnwindVisibility+{??? *PositionUnwindEnabled+?? $??3??$??1??$??2??  Soft Travel Limits#???  y??? *SoftTravelLimitVisibility+ d? 	IsChecked ?? *SoftTravelLimitChecking+{??? * SoftTravelLimitCheckingEnabled+?? -   $ CmbTravelMode??$??0??$??2??$	??120??$
??Left=?$??0q?$	 Value??$
 DisplayName?????? *TravelModeSource+ ?? *
TravelMode+{??? *TravelModeEnabled+]? $??1??$??2??y??? *TravelRangeVisibility+.+?  $	??120??$
 Left=? .   .     Minimum$ 1.4e-45X?{??? *TravelRangeEnabled+   
IsReadOnly ?? *TravelRangeReadOnly+ ?? *TravelRange+?? $	??120??{??? *TravelRangeEnabled+???? $ TxtPositionUnits??$
 Text?]? $??2??$??2??y??? *PositionUnwindVisibility+.+?  $	??120??$
 Left=? .   .  $ 1.4e-45X?{??? *PositionUnwindEnabled+ ?? *PositionUnwindReadOnly+ ?? *PositionUnwindNumerator+?? $	??120??{??? *PositionUnwindEnabled+???? $ TxtPositionUnits??$
 Text??? $??Right=? 	 Cycle#???  {??? *PositionUnwindEnabled+  $??Right=?$	??120?? .   .  $ 1.4e-45X?$
 Left=?{??? *PositionUnwindEnabled+ ?? *PositionUnwindReadOnly+ ?? *PositionUnwindDenominator+?? $??Right=?  	Cycle per#???  {??? *PositionUnwindEnabled+? $??4??$??1??$??2??$??0q?y??? *SoftTravelLimitVisibility+{??? *SoftTravelLimitChecking+ ?? $
??Auto ??? $
??Auto ? ?? $??20 ??? $	??160 ??? $
??Auto ??? $
??Auto ?.+??? $??0??$??1??  Maximum Positive:#???  ?? $??1??$??1??  Maximum Negative:#???    $??0??$??2??$	??120?? .   .  $
 Left=? ?? *SoftTravelLimitPositive+  $??1??$??2??$	??120??$
 Left=? .   .   ?? *SoftTravelLimitNegative+?? $??0??$??3?????? $ TxtPositionUnits??$
 Text??? $??1??$??3?????? $ TxtPositionUnits??$
 Text?