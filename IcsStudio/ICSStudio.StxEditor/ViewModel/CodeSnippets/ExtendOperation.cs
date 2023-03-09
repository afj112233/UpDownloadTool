using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.StxEditor.ViewModel.CodeSnippets
{
    public class ExtendOperation
    {
        public static string LoadTag(string tagName, IController controller, IProgramModule program)
        {
            try
            {
                var stream = new AntlrInputStream(tagName);
                var lexer = new STGrammarLexer(stream);
                var token = new CommonTokenStream(lexer);
                var parser = new STGrammarParser(token);
                var ast = new STASTGenVisitor().Visit(parser.expr());
                var typeChecker = new TypeChecker(controller as Controller, program as IProgram,
                    program as AoiDefinition);
                ast.Accept(typeChecker);
                var astName = ast as ASTName;
                if (astName == null) return string.Empty;
                return (astName.id_list.nodes[0] as ASTNameItem)?.id;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static bool IsTag(string tagName, IController controller, IProgramModule program)
        {
            try
            {
                var stream = new AntlrInputStream(tagName);
                var lexer = new STGrammarLexer(stream);
                var token = new CommonTokenStream(lexer);
                var parser = new STGrammarParser(token);
                var ast = new STASTGenVisitor().Visit(parser.expr());
                var typeChecker = new TypeChecker(controller as Controller, program as IProgram,
                    program as AoiDefinition);
                ast.Accept(typeChecker);
                var astName = ast as ASTName;
                if (astName == null) return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsNumber(string tagName, IController controller, IProgramModule program)
        {
            try
            {
                var stream = new AntlrInputStream(tagName);
                var lexer = new STGrammarLexer(stream);
                var token = new CommonTokenStream(lexer);
                var parser = new STGrammarParser(token);
                var ast = new STASTGenVisitor().Visit(parser.expr());
                var typeChecker = new TypeChecker(controller as Controller, program as IProgram,
                    program as AoiDefinition);
                ast.Accept(typeChecker);
                var astName = ast as ASTName;
                if (astName == null||astName.Expr.type is ArrayType) return false;
                if (astName.Expr.type.IsLINT) return false;
                if (astName.type.IsBool || astName.Expr.type.IsNumber) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string FormatCode(ASTNode node,string code, IRoutine routine)
        {
            if (string.IsNullOrEmpty(code)) return code;
            //var stream = new AntlrInputStream(code);
            //var lexer = new STGrammarLexer(stream);
            //var token = new CommonTokenStream(lexer);
            //var parser = new STGrammarParser(token);
            //var ast = new STASTGenVisitor().Visit(parser.expr());
            //var typeChecker = new TypeChecker(routine.ParentController as Controller,
            //    routine.ParentCollection.ParentProgram as IProgram,
            //    routine.ParentCollection.ParentProgram as AoiDefinition);
            //try
            //{
            //    ast.Accept(typeChecker);
            //}
            //catch (Exception)
            //{
            //    ast = null;
            //}
            var astName = node as ASTName;
            if (astName == null) return code;
            int offset = 0;
            ReplaceIdList(ref code, astName.id_list.nodes, ref offset);
            return code;
        }
        
        private static void ReplaceIdList(ref string replaceCode, List<ASTNode> idList, ref int offset)
        {
            foreach (ASTNameItem astNode in idList)
            {
                var index = replaceCode.IndexOf(astNode.id, offset);
                if (index > -1)
                {
                    if(astNode.FormatId==null)continue;
                    replaceCode = replaceCode.Remove(index, astNode.id.Length).Insert(index, astNode.FormatId);
                    offset = index + astNode.id.Length + 1;
                    if (astNode.arr_list != null)
                    {
                        ReplaceArrList(ref replaceCode, astNode.arr_list.nodes, ref offset);
                    }
                }
            }
        }

        private static void ReplaceArrList(ref string replaceCode, List<ASTNode> arrList, ref int offset)
        {
            foreach (var astNode in arrList)
            {
                var astName = astNode as ASTName;
                if (astName != null)
                    ReplaceIdList(ref replaceCode, astName.id_list.nodes, ref offset);

            }
        }
    }
}
