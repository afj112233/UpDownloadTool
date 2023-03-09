using Antlr4.Runtime;

namespace Unittest
{
    using  Xunit;
    public class STLexerTest
    {
        private void AssertSingle(string text, int type, string value)
        {
            AntlrInputStream input = new AntlrInputStream(text);
            STGrammarLexer lexer = new STGrammarLexer(input);
            var list = lexer.GetAllTokens();
            Assert.Equal(1, list.Count);
            var token = list[0];
            Assert.Equal(token.Type, type);
            Assert.Equal(token.Text, value);
        }
        [Fact]
        public void TestSingle()
        {
            AssertSingle("HELLO", STGrammarLexer.ID, "HELLO");
            AssertSingle("IF", STGrammarLexer.IF, "IF");
            AssertSingle(" IF ", STGrammarLexer.IF, "IF");
            AssertSingle(" IF", STGrammarLexer.IF, "IF");
            AssertSingle("IF ", STGrammarLexer.IF, "IF");
            AssertSingle("THEN", STGrammarLexer.THEN, "THEN");
            AssertSingle("ELSIF", STGrammarLexer.ELSIF, "ELSIF");
            AssertSingle("ELSE", STGrammarLexer.ELSE, "ELSE");
            AssertSingle("END_IF", STGrammarLexer.END_IF, "END_IF");
            AssertSingle("CASE", STGrammarLexer.CASE, "CASE");
            AssertSingle("OF", STGrammarLexer.OF, "OF");
            AssertSingle("END_CASE", STGrammarLexer.END_CASE, "END_CASE");
            AssertSingle("FOR", STGrammarLexer.FOR, "FOR");
            AssertSingle("TO", STGrammarLexer.TO, "TO");
            AssertSingle("DO", STGrammarLexer.DO, "DO");
            AssertSingle("END_FOR", STGrammarLexer.END_FOR, "END_FOR");
            AssertSingle("BY", STGrammarLexer.BY, "BY");
            AssertSingle("REPEAT", STGrammarLexer.REPEAT, "REPEAT");
            AssertSingle("UNTIL", STGrammarLexer.UNTIL, "UNTIL");
            AssertSingle("END_REPEAT", STGrammarLexer.END_REPEAT, "END_REPEAT");
            AssertSingle("WHILE", STGrammarLexer.WHILE, "WHILE");
            AssertSingle("END_WHILE", STGrammarLexer.END_WHILE, "END_WHILE");
            AssertSingle("EXIT", STGrammarLexer.EXIT, "EXIT");
            AssertSingle("MOD", STGrammarLexer.MOD, "MOD");
            AssertSingle("NOT", STGrammarLexer.NOT, "NOT");
            AssertSingle("XOR", STGrammarLexer.XOR, "XOR");
            AssertSingle("OR", STGrammarLexer.OR, "OR");
            AssertSingle("AND", STGrammarLexer.AND, "AND");
            AssertSingle(";", STGrammarLexer.SEMICOLON, ";");
            AssertSingle(",", STGrammarLexer.COMMA, ",");
            AssertSingle(":=", STGrammarLexer.ASSIGN, ":=");
            AssertSingle(":", STGrammarLexer.COLON, ":");
            AssertSingle("(", STGrammarLexer.LPAREN, "(");
            AssertSingle(")", STGrammarLexer.RPAREN, ")");
            AssertSingle("[", STGrammarLexer.LBRACKET, "[");
            AssertSingle("]", STGrammarLexer.RBRACKET, "]");
            AssertSingle("AND", STGrammarLexer.AND, "AND");
            AssertSingle("&", STGrammarLexer.AND, "&");
            AssertSingle("+", STGrammarLexer.PLUS, "+");
            AssertSingle("-", STGrammarLexer.MINUS, "-");
            AssertSingle("**", STGrammarLexer.POW, "**");
            AssertSingle("*", STGrammarLexer.TIMES, "*");
            AssertSingle("/", STGrammarLexer.DIVIDE, "/");
            AssertSingle("=", STGrammarLexer.EQ, "=");
            //TODO What's about "< >"
            AssertSingle("<>", STGrammarLexer.NEQ, "<>");
            AssertSingle("<=", STGrammarLexer.LE, "<=");
            AssertSingle("<", STGrammarLexer.LT, "<");
            AssertSingle(">=", STGrammarLexer.GE, ">=");
            AssertSingle(">", STGrammarLexer.GT, ">");
            AssertSingle("1.123", STGrammarLexer.FLOAT, "1.123");
            AssertSingle("2#01",  STGrammarLexer.BIN_INTEGER, "2#01");
            AssertSingle("8#01234567", STGrammarLexer.OCT_INTEGER, "8#01234567");
            AssertSingle("16#1234567890ABCDEFabcdef", STGrammarLexer.HEX_INTEGER, "16#1234567890ABCDEFabcdef");
            AssertSingle("1234567890", STGrammarLexer.DEC_INTEGER, "1234567890");
            AssertSingle("abcdefg", STGrammarLexer.ID, "abcdefg");
            AssertSingle("ABCDEFG", STGrammarLexer.ID, "ABCDEFG");
            AssertSingle(".abcdefg", STGrammarLexer.IDSEL, ".abcdefg");
            AssertSingle(".ABCDEFG", STGrammarLexer.IDSEL, ".ABCDEFG");
            AssertSingle(".____", STGrammarLexer.IDSEL, ".____");
            AssertSingle(".1234", STGrammarLexer.BITSEL, ".1234");
            AssertSingle(".", STGrammarLexer.DOT, ".");
        }
    }
}
