grammar RLLExprGrammar;
expr : expr OR xor_expr # ExprExpr
    | xor_expr # ExprXor
    ;
xor_expr : xor_expr XOR and_expr # XorExpr
    | and_expr # XorExprAnd
    ;
and_expr : and_expr AND add_expr # AndExpr
    | add_expr # AndExprAdd
    ;
add_expr : add_expr op=(PLUS | MINUS) mul_expr # AddExpr
    | mul_expr # AddExprMul
    ;
mul_expr : mul_expr op=(TIMES | DIVIDE | MOD) not_expr # MulExpr 
    | not_expr # MulExprNot
    ;
not_expr : NOT neg_expr # NotExpr 
    | neg_expr # NotExprNeg
    ;
neg_expr : op=(MINUS | PLUS) pow_expr # NegExpr 
    | pow_expr # NegExprPow
    ;
pow_expr : pow_expr POW prim_expr # PowExpr 
    | prim_expr # PowExprPrim
    ;
prim_expr : FLOAT # PrimExprFloat 
    | BITSEL # PrimExprBitSel 
    | integer # PrimExprInteger
    | item # PrimExprItem 
    | LPAREN expr RPAREN #PrimExprExpr;
item : ID id_sel_list bit_sel # ItemItem
    | ID LBRACKET array_sel_list RBRACKET id_sel_list bit_sel # ItemArray
    ;
id_sel_list : id_sel_list id_sel # IDSelList 
    | empty # IDSelListEmpty
    ;
id_sel : IDSEL # IDSel
    | IDSEL LBRACKET array_sel_list RBRACKET # IDSelArray
    ;
array_sel_list : array_sel_list COMMA expr # ArraySelList
    | expr # ArraySelListExpr
    ;
bit_sel : BITSEL # BitSel
    | DOT LBRACKET expr RBRACKET # BitSelExpr
    | empty # BitSelEmpty
    ;
integer : DEC_INTEGER # IntegerDec
    | BIN_INTEGER # IntegerBin
    | OCT_INTEGER # IntegerOct
    | HEX_INTEGER # IntegerHex
    ;
empty : ;
MOD : 'MOD' ;
NOT : 'NOT' ;
XOR : 'XOR' ;
OR : 'OR' ;
AND : 'AND' | '&';
ID : [a-zA-Z_][a-zA-Z0-9_]* ;
IDSEL :'.' [a-zA-Z_][a-zA-Z_0-9]* ;
BITSEL : '.' [0-9]+ ;
DOT : '.' ;
SEMICOLON : ';' ;
COMMA : ',' ;
ASSIGN : ':=' ;
COLON : ':' ;
LPAREN : '(' ;
RPAREN : ')' ;
LBRACKET : '[' ;
RBRACKET : ']' ;
PLUS : '+' ;
MINUS : '-' ;
POW : '**' ;
TIMES : '*' ;
DIVIDE : '/' ;
//FIXME scientific notation 
FLOAT : [0-9]+'.'[0-9]+ ;
BIN_INTEGER : '2#'[01]+ ;
OCT_INTEGER : '8#'[0-7]+ ;
HEX_INTEGER : '16#'[0-9A-Fa-f]+ ;
DEC_INTEGER : [0-9]+ ;

WS : [ \t\r\n]+ -> skip ;
COMMENT : '#' .*? '\r'? '\n' -> skip ;