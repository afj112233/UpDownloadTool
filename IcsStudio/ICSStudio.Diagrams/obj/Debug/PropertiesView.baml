   M S B A M L   `   `   `  ????_  [ICSStudio.Diagrams, Version=1.0.130.22348, Culture=neutral, PublicKeyToken=e23cfa165ecd563f5clr-namespace:ICSStudio.DiagramICSStudio.Diagram  5      '    !ICSStudio.Diagrams.PropertiesView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      -srcclr-namespace:ICSStudio.Diagram    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        ? 	Resources  5         validationErrorTemplate ??Name  gridLineStyle ?? 
TargetType ?? Property ?? Value  gridHorizontalLineStyle ?? BasedOn ?? Path ?? RelativeSource ?? AncestorType  gridVerticalLineStyle??%?  &	        &	 U     &	 ?     7  &	 ?    7  ?? 5   
   .R?5	      ]? .+?5      ?? $ adorner??6,   5      5      ?? 5   
   " ??6&   .?5      ??  ? ??
 ?????6'   ?? 5       ?@??Stretch$
 Fill=?6(   ?? 5       ?@K?ZIndex$
 1000??6,   5      ?? 5   
   " ??60   8   .?5      ?? 
 ?@??X2$ 1??6#   ?? 5       ? ??$ Bottom=?62   ?? 5       ? ?? 5      ?? $ ColumnDefinitions.Count? ?? $
 GridL?5      ?? 5   
   " ??6.   8   .?5      ?? 
 ?@??Y2$ 1??6#   ?? 5        ? ??$ Right=?64   ?? 5!       ? ?? 5"      ?? $ RowDefinitions.Count? ?? $
 GridL?5%      5'      .??5)      ?? $??1q?6   
???   ?._?5*   
   ]? -   5        	 ]?Name$	 _panel??5*      .+?5+      ?? -   5        
 ??Name$
 _label??5+      $??50??6%   $??16??60   ._?5,      ?? $??Empty??6    ?? TextAlignment$ Center=?6)   
???????6@   5-      ?? 5.      -   5         ??Name$ _gridContainer??5.      $
??Auto=?63   .??5/      ? -   5         ?Name$ _grid??5/       ? ColumnDefinitions 50      ?? 51      ?? 52      .+?55      ?? -   5        $??_vLine??55      $??0??6)   $
??1000??69   #??[ 6M   ?? 56      -   5        $??	_splitter??56   #   $
??1000??64   $??0,0,-2,0q?6I   $??4??6[   
??????? J? Opacity$
 0.01Q?57   6    K? ZIndex$ 10000??6E   59      5:      5;      5<      5=      