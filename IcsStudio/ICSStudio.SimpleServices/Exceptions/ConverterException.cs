using ICSStudio.Utils;

namespace ICSStudio.SimpleServices.Exceptions
{
    public class ExtraCharactersException:ICSStudioException
    {
    }
    public class MissCharacterException:ICSStudioException
    {

    }
    public class InvalidCharacterCombinationException: ICSStudioException
    {

    }
    public class InvalidCharacterException:ICSStudioException
    {

    }
    public class InvalidSizeException:ICSStudioException
    {

    }

    public class InvalidParameterConnection : ICSStudioException
    {
        public InvalidParameterConnection(string mes) : base(mes)
        {

        }
    }
}
