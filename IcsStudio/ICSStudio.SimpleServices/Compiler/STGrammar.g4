grammar STGrammar;
@header {#pragma warning disable 3021}

start : stmt_list  ;
stmt_list : (stmt)+  # StmtList
        | empty  # StmtListEmpty
        ;
stmt : assign_stmt SEMICOLON # StmtAssign 
    | instr_stmt # StmtInstr
    | if_stmt # StmtIf 
    | case_stmt # StmtCase
    | for_stmt # StmtFor
    | repeat_stmt # StmtRepeat
    | while_stmt # StmtWhile
    | region_stmt #StmtRegionLable
	| Cheinese # StmtChinese
    | EXIT SEMICOLON # StmtExit
	| item expr # StmtUnexpected
    | empty SEMICOLON # StmtEmpty
    ;
error_stmt: ('\uff1b'|item | number | LBRACKET | RBRACKET|'*/'+|'*)'+ | Cheinese) # ErrorStmt 
    ; 
assign_stmt : (MINUS)? item op=(ASSIGN | NRASSIGN) expr (error_stmt)? # AssignStmt
    ;
expr : expr OR xor_expr # ExprExpr
    | xor_expr # ExprXor
	| Cheinese # ErrorChineseExpr
	| error_stmt #ErrorExpr
	;
xor_expr : xor_expr XOR and_expr # XorExpr
    | and_expr # XorExprAnd
    ;
and_expr : and_expr AND eq_expr  # AndExpr
    | eq_expr # AndExprEq
    ;
eq_expr : eq_expr op=(EQ | NEQ) cmp_expr # EqExpr 
    | cmp_expr #EqExprCmp
    ;
cmp_expr : cmp_expr op=(LE | LT | GE | GT) add_expr  # CmpExpr
    | add_expr # CmpExprAdd 
    ;
add_expr : add_expr op=(PLUS | MINUS) mul_expr # AddExpr
    | mul_expr # AddExprMul
    ;
mul_expr : mul_expr op=(TIMES | DIVIDE | MOD) not_expr  # MulExpr 
    | not_expr # MulExprNot
    ;
not_expr: NOT not_expr # NotExpr 
    | neg_expr # NotExprNeg
    ;
neg_expr : op=(MINUS | PLUS) neg_expr  # NegExpr 
    | pow_expr # NegExprPow
    ;
pow_expr : pow_expr POW func_expr  # PowExpr 
    | func_expr # PowExprFunc
    ;
func_expr :  ('-'|'+') func_expr # FuncExprFunc
    | ID LPAREN param_list (RPAREN)+  # FuncExpr 
    | prim_expr # FuncExprPrim
    ;
prim_expr : FLOAT # PrimExprFloat 
    | BITSEL # PrimExprBitSel 
    | integer # PrimExprInteger
    | item # PrimExprItem 
    | LPAREN expr (RPAREN)+ #PrimExprExpr
	| INFINITY #INFINITYExpr
	| NAN #NANExpr
    | empty #EmptyExpr;
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
	|DOT # ErrorBitSel
    | empty # BitSelEmpty
    ;
param_list : param_list COMMA expr # ParamList 
    | expr # ParamListExpr
    ;
instr_stmt : ID LPAREN param_list (RPAREN)+ SEMICOLON # InstrStmt
	| ID LPAREN param_list # ErrorInstrStmt
    ;
if_stmt : IF expr THEN stmt_list elsif_stmt_list if_else_stmt END_IF SEMICOLON # IfStmt
    ;
elsif_stmt_list : elsif_stmt_list ELSIF expr THEN stmt_list # ElseifStmtList
    | empty # ElseifStmtListEmpty
    ;
if_else_stmt : ELSE stmt_list # IfElseStmt
    | empty # IfElseStmtEmpty
    ;
case_stmt : CASE expr (OF|empty) case_elem_list case_else_stmt END_CASE SEMICOLON # CaseStmt
    ;
case_elem_list : case_elem_list case_elem # CaseElemList
    | empty # CaseElemListEmpty
    ;
case_elem : case_selector_multi COLON stmt_list # CaseElem
    ;
case_selector_multi: case_selector_multi COMMA case_selector # CaseSelectorMulti
    | case_selector # CaseSelectorMultiCaseSelector
    ;
case_selector : number # CaseSelectorNumber
    | case_selector_range # CaseSelectorCaseSelectorRange
    | error_stmt # ErrorCaseSelector
	| error_stmt CASE_TO error_stmt # ErrorCaseSelectorRange
	;
case_selector_range: number CASE_TO number  # CaseSelectorRange
    ;
case_else_stmt : ELSE stmt_list # CaseElseStmt
    | empty # CaseElseStmtEmpty
    ;
for_stmt : FOR (assign_stmt|empty) TO expr optional_by (DO|empty) stmt_list END_FOR SEMICOLON # ForStmt
    ;
optional_by : BY expr # OptionalBy
    | empty # OptionalByEmpty
    ;
repeat_stmt : REPEAT stmt_list UNTIL expr END_REPEAT SEMICOLON # RepeatStmt
    ;
while_stmt : WHILE expr DO stmt_list END_WHILE SEMICOLON # WhileStmt
    ;
number : ('-'|'+'|empty) integer # NumberInteger
    | ('-'|'+'|empty) FLOAT # NumberFloat
    ;
integer : DEC_INTEGER # IntegerDec
    | BIN_INTEGER # IntegerBin
    | OCT_INTEGER # IntegerOct
    | HEX_INTEGER # IntegerHex
    ;

region_stmt
	: REGION	# StmtRegion
	| ENDREGION	# StmtEndregion
    ;

empty : ;
Cheinese : [\u4E00-\u9FA5\uf900-\ufa2d]+ ;
IF : I F ;
THEN : T H E N ;
ELSIF : E L S I F ;
ELSE : E L S E ;
END_IF : E N D '_' I F ;
CASE : C A S E ;
OF : O F ;
END_CASE : E N D '_' C A S E ;
FOR : F O R ;
TO : T O ;
CASE_TO: '..' ;
DO : D O ;
END_FOR : E N D '_' F O R ;
BY : B Y ;
REPEAT : R E P E A T ;
UNTIL : U N T I L ;
END_REPEAT : E N D '_' R E P E A T ;
WHILE : W H I L E ;
END_WHILE : E N D '_' W H I L E ;
EXIT : E X I T ;
MOD : M O D ;
NOT : N O T ;
XOR : X O R ;
OR : O R ;
AND : A N D | '&';
ID : ('\\'|'%')?[a-zA-Z_](':'{this.InputStream.LA(1)!= '=' }?|[a-zA-Z0-9_])*;
IDSEL :'.' [a-zA-Z_][a-zA-Z_0-9]* ;
BITSEL : '.' [0-9]+ ;
DOT : '.' ;
SEMICOLON : ';' ;
COMMA : ',' ;
ASSIGN : ':=' ;
NRASSIGN : '[:=]';
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
EQ : '=' ;
NEQ : '<>' ;
LE : '<=' ;
LT : '<' ;
GE : '>=' ;
GT : '>' ;
FLOAT : [0-9_]+'.'[0-9_]+([Ee]('+'|'-')?[0-9_]+)? ;
INFINITY : '1.$';
NAN : '1.#QNAN' ;
BIN_INTEGER : '2#'[01_]+ ;
OCT_INTEGER : '8#'[0-7_]+ ;
HEX_INTEGER : '16#'[0-9A-Fa-f_]+ ;
DEC_INTEGER : [0-9_]+ ;

WS : [\u0020\u00a0\t\r\n]+ -> skip ;
REGION:
	'#' R E G I O N ~[\r\n]*? ('\r'? '\n' | EOF);
ENDREGION:
	'#' E N D R E G I O N ~[\r\n]*? ('\r'? '\n' | EOF);
LINE_COMMENT: '//' .*? '\r'? '\n' -> skip;
RANGE_COMMENT: ('(*' (.|'\r'? '\n')*? '*)' |'/*' (.|'\r'? '\n')*? '*/')-> skip  ;

fragment A:('a'|'A');
fragment B:('b'|'B');
fragment C:('c'|'C');
fragment D:('d'|'D');
fragment E:('e'|'E');
fragment F:('f'|'F');
fragment G:('g'|'G');
fragment H:('h'|'H');
fragment I:('i'|'I');
fragment J:('j'|'J');
fragment K:('k'|'K');
fragment L:('l'|'L');
fragment M:('m'|'M');
fragment N:('n'|'N');
fragment O:('o'|'O');
fragment P:('p'|'P');
fragment Q:('q'|'Q');
fragment R:('r'|'R');
fragment S:('s'|'S');
fragment T:('t'|'T');
fragment U:('u'|'U');
fragment V:('v'|'V');
fragment W:('w'|'W');
fragment X:('x'|'X');
fragment Y:('y'|'Y');
fragment Z:('z'|'Z');
