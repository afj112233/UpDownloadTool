   M S B A M L   `   `   `  ???? Z  VICSStudio.Gui, Version=1.0.131.22348, Culture=neutral, PublicKeyToken=df4cc7caa7503c05D2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.GuiICSStudio.Gui  h dICSStudio.UIServicesPackage, Version=1.0.129.23039, Culture=neutral, PublicKeyToken=c64abb1788bf761fS.clr-namespace:ICSStudio.UIServicesPackage.View ICSStudio.UIServicesPackage.View 5   /ICSStudio.UIServicesPackage.View.DynamicsDialog   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Bgui2clr-namespace:ICSStudio.Gui;assembly=ICSStudio.Gui     @local.clr-namespace:ICSStudio.UIServicesPackage.View     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -      Dynamics Calculate  ? Title#  ?   $	??640?????	#FFF0F0F0 ? 
ResizeMode$ NoResize=? ? ShowInTaskbar .   ? SizeToContent$ Height=? ? WindowStartupLocation$ CenterOwner=?    ICSStudio.Gui.DialogCloser   DialogResult ?? *DialogResult+.??? $??
15,10,15,0q? ? 	Resources 5  /ICSStudio.UIServicesPackage.View.DoubleToString  DoubleToStringConverter=  7ICSStudio.UIServicesPackage.View.CalculatePatternToBool  CalculatePatternToBool ?? 
TargetType ?? Property	 ?? Value
 ?? Triggers P? Property P? Value ?? Self ?? RelativeSource ?? Path??%?  &	       &	      '?? 
     '?? ?     '?? ?     '?? L    '?? ]        ?? " ??.???  ? ??$
	 auto????  ? ??$	 5q???  ? ??$	 3q?-   
 P?  ?@=?HasError . . ???  ?@?ToolTip	 ?? # Z $&  (Validation.Errors)/ErrorContent??? " ??.???  ? ??$	 Center=??? " ??.???  ? ??$	 Center=???  ? ??$	 Right=???  ? ??$		 5,0q??? " ??.???  ? ??$
	 auto????  ? ??$	 5q???  ?@??IsSnapToTickEnabled	 . ??  ?@??TickPlacement$	 BottomRight=???  ?@??AutoToolTipPrecision$	 2????  ?@??AutoToolTipPlacement$	 TopLeft=???  ?@??IsDirectionReversed	 . ?? " ??.???  ? ??
	 ???????  ? ??$		 5,0q? ? RowDefinitions ?? $
??Auto ??? $
??Auto ??? $??* ?.+??? ???? .R?	?  ! Indirect by Acceleration Time#???  $??Black"? 	? 	GroupName$ IsDirect?? ? DataContext ?? *DataContext+ ?? *FindAncestor+ ?? AncestorType" ?? d? 	IsChecked ?? *CalculatePattern+ ?? Mode$ TwoWay=? ?? UpdateSourceTrigger$ PropertyChanged=? ?? 	Converter# [  ?? ConverterParameter$ 0??.??? {??? *CalculatePattern+$ OneWay=?$ PropertyChanged=?# [ $ 0?? ? ColumnDefinitions ?? $??* ??? $??* ??? $
??0.7* ? ?? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ??? $
??Auto ?.+??? $??0??$??0??  Maximum Velocity(V max):#???  ?? $??0??$??1??.?????? # [ $ MaximumVelocity?$ PropertyChanged=? ?? ValidationRules 6  0ICSStudio.UIServicesPackage.View.NumberInputRule  .???? $??0??$??2?????? *PositionUnits+ ?? StringFormat$ {0}/s???? $??1??$??0??  Acceleration Time:#???  ?? $??1??$??1??.?????? # [ $ AccelerationTime?$ PropertyChanged=?   .???? $??1??$??2??$??s??A? $??2??$??0?? A? Columns$ 2??.+???   Deceleration Time:#???  ??   as above#???  {?.   ?? *IsIndirectAsAbove+?? $??2??$??1??.?????? # [ $ DecelerationTime?$ PropertyChanged=?   .???? $??2??$??2??$??s??? $??3??$??1??$??2??$	??0,5q? ?? $
??Auto ??? $??* ??? $
??Auto ?.+??? $	??5,0q? 
 smooth#???   ?? TextAlignment$
 Left=??? $??1??$	??5,0q??? $??2??$	??5,0q? 		 stiff#??? 	 $ Right=??? $??4??$??0?? 
 Acceleration Characteristic#??? 
 ?? $??4??$??1??$??2?? ? Maximum$	 100Q? ?? TickFrequency$ 1Q?  ?? TickPlacement$  BottomRight=?! ? Value! ?? *AccelerationCharacteristic+?? $??5??$??0??  Deceleration Characteristic#???  ?? $??5??$??1??$??2??$	 100Q?$ 1Q?$  BottomRight=?! ?? *DecelerationCharacteristic+?? $??1?????? .R?	?  
 Direct#???  $??Black"?$ IsDirect?? ?? *DataContext+ ?? *FindAncestor+" ?? ?? *CalculatePattern+$ PropertyChanged=?$ TwoWay=?# [ $ 1??.??? {??? *CalculatePattern+$ OneWay=?$ PropertyChanged=?# [ $ 1?? ?? $??* ??? $??* ??? $
??0.7* ? ?? $
??Auto ??? $
??Auto ??? $
??Auto ?.+??? $??0??$??0??  Acceleration:#???  ?? $??0??$??1??.?????? # [ $ Acceleration?$ PropertyChanged=?   .???? $??0??$??2?????? *PositionUnits+$ {0}/s^2??A? $??1??$??0??$ 2??.+???   Deceleration:#???  ?? #???  {?.   ?? *IsDirectAsAbove+?? $??1??$??1??.?????? # [ $ Deceleration?$ PropertyChanged=?   .???? $??1??$??2?????? *PositionUnits+$ {0}/s^2???? $??2??$??0?? 	 Jerk:#???  ?? $??2??$??1??.?????? # [ $
 Jerk?$ PropertyChanged=?   .???? $??2??$??2?????? *PositionUnits+$ {0}/s^3??? $??2?? ??%X   '??       ?? " ??.???  ? ??$	 65????  ? ??$	 20????  ? ??$	 5q? ?? $??* ??? $??* ??? $??* ?.+?A? $??1??$??2??$	??0,5q?$ 4??.+???   OK#???  ???? *	OKCommand+??  
 Cancel#???  ???? *CancelCommand+??  	 Apply#???  ???? *ApplyCommand+??   Help#???  