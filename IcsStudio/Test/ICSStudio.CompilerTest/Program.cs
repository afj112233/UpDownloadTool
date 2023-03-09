using Antlr4.Runtime;
using ICSStudio.SimpleServices.Compiler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;

namespace CompilerTest
{
    public class PrintVisitor : IASTBaseVisitor
    {
        public override ASTNode VisitStmtMod(ASTStmtMod context)
        {
            Console.WriteLine(context.ToString());
            Console.WriteLine(context.list.nodes.Count);
            foreach (var stmt in context.list.nodes)
            {
                Console.WriteLine(stmt.ToString());
                stmt.Accept(this);

            }
            return context;
        }

        public override ASTNode VisitAssignStmt(ASTAssignStmt context)
        {
            Console.WriteLine(context.ToString());
            context.name.Accept(this);
            context.expr.Accept(this);
            return context;
        }

        public override ASTNode VisitInstr(ASTInstr context)
        {
            Console.WriteLine(context.ToString());
            return context;
        }

        public override ASTNode VisitIf(ASTIf context)
        {
            Console.WriteLine(context.ToString());
            return context;
        }

        public override ASTNode VisitCase(ASTCase context)
        {
            Console.WriteLine(context.ToString());
            return context;
        }

        public override ASTNode VisitExit(ASTExit context)
        {
            Console.WriteLine(context.ToString());
            return context;
        }

        public override ASTNode VisitFor(ASTFor context)
        {
            Console.WriteLine(context.ToString());
            return context;
        }

        public override ASTNode VisitRepeat(ASTRepeat context)
        {
            Console.WriteLine(context.ToString());
            return context;
        }

        public override ASTNode VisitWhile(ASTWhile context)
        {
            Console.WriteLine(context.ToString());
            return context;
        }

        public override ASTNode VisitName(ASTName context)
        {
            Console.WriteLine(context.ToString());
            return context;
        }

        public override ASTNode VisitFloat(ASTFloat context)
        {
            Console.WriteLine(context.ToString());
            return context;
        }

        public override ASTNode VisitInteger(ASTInteger context)
        {
            Console.WriteLine(context.ToString());
            return context;
        }

        public override ASTNode VisitUnaryOp(ASTUnaryOp context)
        {
            Console.WriteLine(context.ToString());
            return context;
        }

        public override ASTNode VisitCall(ASTCall context)
        {
            Console.WriteLine(context.ToString());
            return context;
        }

        public override ASTNode VisitRTCall(ASTRTCall context)
        {
            Console.WriteLine(context.ToString());
            return context;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Sample();
        }

        static private void RLLSample()
        {
            var parser = new RLLRungParser(null);
            var root = parser.ParseRung("[MAM(AXIS,MAM1,ddddd,sssss,dddddccc,Units per sec,100,Units per sec2,aaaa,Units per sec2,Trapezoidal,ddccc,asdfafc,Units per sec3,Disabled,Programmed,1,None,0,0) ,MAM(AXIS,MAM1,ddddd,sssss,dddddccc,Units per sec,100,Units per sec2,aaaa,Units per sec2,Trapezoidal,ddccc,asdfafc,Units per sec3,Disabled,Programmed,1,None,0,0) XIC(enable) ,MAM(AXIS,MAM1,ddddd,sssss,dddddccc,Units per sec,100,Units per sec2,aaaa,Units per sec2,Trapezoidal,ddccc,asdfafc,Units per sec3,Disabled,Programmed,1,None,0,0) XIC(enable) ,MAM(AXIS,MAM1,ddddd,sssss,dddddccc,Units per sec,100,Units per sec2,aaaa,Units per sec2,Trapezoidal,ddccc,asdfafc,Units per sec3,Disabled,Programmed,1,None,0,0) XIC(enable) ,MAM(AXIS,MAM1,ddddd,sssss,dddddccc,Units per sec,100,Units per sec2,aaaa,Units per sec2,Trapezoidal,ddccc,asdfafc,Units per sec3,Disabled,Programmed,1,None,0,0) XIC(enable) ]MAM(AXIS,MAM1,ddddd,sssss,dddddccc,Units per sec,100,Units per sec2,aaaa,Units per sec2,Trapezoidal,ddccc,asdfafc,Units per sec3,Disabled,Programmed,1,None,0,0)[,[,[,] ] ][,]MAM(AXIS,MAM1,ddddd,sssss,dddddccc,Units per sec,100,Units per sec2,aaaa,Units per sec2,Trapezoidal,ddccc,asdfafc,Units per sec3,Disabled,Programmed,1,None,0,0)[,,,,][,]XIC(enable)XIC(enable)XIC(enable)XIC(enable)XIC(enable)XIC(enable)MAM(AXIS,MAM1,ddddd,sssss,dddddccc,Units per sec,100,Units per sec2,aaaa,Units per sec2,Trapezoidal,ddccc,asdfafc,Units per sec3,Disabled,Programmed,1,None,0,0)OTE(out);");
           // var visitor = new RLLPrintVisitor();
            //root.Accept(visitor);

        }
        static string error1= @"IF A=1 yy then
	sf:=11;
else
	
end_if;
";

        private static string error2 = @"IF IF A=1 yy then
	sf:=11;
else
	
end_if;
";

        private static string error3 = @"if 1 then  elsif 1 end_if;";

        private static string error4 = @"IF 1  THEN
			ELSIF 1  
				
				  IF 1 
				   THEN
				   
				  END_IF;			 
			END_IF;";

        private static string error5 = "While 1 then end_for end_While;";

        private static string error6 = @"if 1 and count=1 (***)10  then
	
end_if;";

        private static string error7 = @"case count of 
	2
	count:=2;
end_case; 
";

        private static string error8 = @"if 1 then 
	;
	elsif 1 
	;
	1 then
	;
end_if; ";

        private static string error9 = @"if BOOL then
	i :=1;
elsif BOOL
	;
elsif BOOL then
	;
else
	i :=1;\1 
end_if;";

        private static string error10 = @"if 1 then 
 end_if;
大苏打撒旦";

        private static string error11 = @"i=1;";

        private static string error12 = @"case i of
	1
		i :=1;
	
end_case;";

        private static string error13 = @"case i of
	1:
		i :=1;
	2:
end_case;
@!#$%";

        private static string error14 = @"if 1 then 
	;
	elsif 1 then
	;
	 1 then
	;
end_if; ";

        private static string error15 = @"if 1 then 
	AOI(ww,
//	);
end_if; ";

        private static string error16 = @"for i := 1 to 99 by 5555555 do
end_for;

if BOOL then
	i :=1;
elsif BOOL then
	;
elsif BOOL then
	;
else
	i :=1;
end_if;



case i of
	1:
		i :=1;
	2:
end_case;
&";

        public static string error17 = @"case i of
	1;
		i :=1;
	2:
end_case;";

        public static string error18 = "count:=  ~ count;";
        public static string error19 = "masr(,);";
        static private void Sample()
        {
            var text = error19;
            Console.WriteLine(text);

            AntlrInputStream input = new AntlrInputStream(text);
            //AntlrInputStream input = new AntlrInputStream(@" CALL1,2,3);");
            //var listen = new MyErrorListListener();
            STGrammarLexer lexer = new STGrammarLexer(input);
            var listen = new LexerListener();
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(listen);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            var listen2=new ParserListener();
            //var handler=new ErrorStrategy();
            STGrammarParser parser = new STGrammarParser(tokens);
            parser.RemoveErrorListeners();
            //parser.ErrorHandler = handler;
            parser.AddErrorListener(listen2);
            var tree = parser.start();
            STASTGenVisitor visitor = new STASTGenVisitor();
            var ast = visitor.Visit(tree);
            Console.ReadLine();
            return;
            var ctrl = ICSStudio.SimpleServices.Common.Controller.Open(@"C:\Users\zhuyi\Downloads\W1 (1).json");
            PrintVisitor printer = new PrintVisitor();
            ast.Accept(printer);
            var program = ctrl.Programs["MainProgram"];
            TypeChecker p = new TypeChecker(ctrl, program,null);
            ast.Accept(p);
            CodeGenVisitor gen = new CodeGenVisitor(null);
            ast.Accept(gen);
            Console.WriteLine($"{ICSStudio.Utils.Utils.BytesToHex(gen.Codes.ToArray())}");
            Console.WriteLine($"{ICSStudio.Utils.Utils.BytesToHex(gen.Pool.ToArray())}");
        }
        
    }
}
