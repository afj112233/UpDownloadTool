using ICSStudio.Utils;

namespace ICSStudio.SimpleServices.Exceptions
{
    public class ParserException : ICSStudioException
    {
        public ParserException(string message) : base(message)
        {

        }
    }

    public class RLLParserException : ParserException
    {
        public RLLParserException(string message) : base(message)
        {

        }
    }
}