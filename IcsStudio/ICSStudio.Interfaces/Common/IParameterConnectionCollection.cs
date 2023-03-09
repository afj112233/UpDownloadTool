using System.Collections.Generic;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.Interfaces.Common
{
    public interface IParameterConnection
    {
        string SourcePath { get; set; }
        string DestinationPath { get; set; }
    }

    public interface IParameterConnectionCollection
        : IBaseComponentCollection<IParameterConnection>
    {
        void Add(IParameterConnection parameterConnection);
        void Remove(IParameterConnection parameterConnection);

        IParameterConnection FindConnection(string operand);
        IParameterConnection FindConnection(string source,string dest);
        void RemoveConnection(ITag tag);
        IEnumerable<IParameterConnection> GetTagParameterConnections(ITag tag);

        IParameterConnectionVerifyExceptionArg VerifyConnection(IParameterConnection parameterConnection);

        IParameterConnectionVerifyExceptionArg VerifyConnection(string specificA, Usage usageA, ExternalAccess externalAccessA, string dataTypeA, string specificB,
            Usage usageB, ExternalAccess externalAccessB, string dataTypeB);

        IParameterConnection CreateConnection(string specificA, Usage usageA, string specificB, Usage usageB);
        IParameterConnection CreateConnection(ITag tag, string specificB);
        IParameterConnectionVerifyExceptionArg VerifyInOutTag(ITag tag);
    }

    public interface IParameterConnectionVerifyExceptionArg
    {
         IParameterConnection Connection { get; }
         string Message { get; }
    }
}
