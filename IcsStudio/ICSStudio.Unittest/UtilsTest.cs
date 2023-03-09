using ICSStudio.Utils.TagExpression;
using Xunit;

namespace Unittest
{
    public class UtilsTest
    {
        [Fact]
        public void BytesToHexTest()
        {
            byte[] input0 = {0x00};
            string output0 = "00";

            byte[] input1 = {0x01, 0x02};
            string output1 = "01:02";

            byte[] input2 = {255, 254, 0xAA, 0xbc};
            string output2 = "FF:FE:AA:BC";

            Assert.Equal(ICSStudio.Utils.Utils.BytesToHex(input0), output0);
            Assert.Equal(ICSStudio.Utils.Utils.BytesToHex(input1), output1);
            Assert.Equal(ICSStudio.Utils.Utils.BytesToHex(input2), output2);
        }

        [Fact]
        public void TagExpressionTest()
        {
            TagExpressionParser parser = new TagExpressionParser();

            string input0 = "tag.test[1].haha.1";
            var output0 = parser.Parser(input0);
            Assert.Equal(output0.ToString(), input0);


            string input1 = "tag[3,2,1].haha";
            var output1 = parser.Parser(input1);
            Assert.Equal(output1.ToString(), input1);
        }

        [Fact]
        public void ParserCmpExpression()
        {
            TagExpressionParser parser = new TagExpressionParser();

            string input0 = "string[0]<string[1]";
            var output0 = parser.Parser(input0);
            Assert.Null(output0);

            string input1 = "real=1 OR 2 XOR 3 MOD 2 +5/2**2-NOT(SIN(1))";
            var output1 = parser.Parser(input1);
            Assert.Null(output1);

            string input2 = "real2=dint";
            var output2 = parser.Parser(input2);
            Assert.Null(output2);

            string input3 = "ASN(2)";
            var output3 = parser.Parser(input3);
            Assert.Null(output3);

            string input4 = "ASN ";
            var output4 = parser.Parser(input4);
            Assert.Null(output4);
        }

        [Fact]
        public void ParserArrayWithTagExpression()
        {
            TagExpressionParser parser = new TagExpressionParser();

            string input0 = "tag.test[abc].haha.1";
            var output0 = parser.Parser(input0);
            Assert.Equal(output0.ToString(), input0);
        }

        [Fact]
        public void ParserBitWithTagExpression()
        {
            TagExpressionParser parser = new TagExpressionParser();

            string input0 = "CommandString.DATA[Wrk_BufferByteIndex].[Wrk_BufferBitIndex]";
            var output0 = parser.Parser(input0);
            Assert.Equal(output0.ToString(), input0);
        }
    }
}
