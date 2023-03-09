SECTIONS {
 /DISCARD/ : {
  *(.discard)
  *(.discard.*)
 }
 __rsymtab 0 : { *(SORT(__rsymtab+*)) }
}
