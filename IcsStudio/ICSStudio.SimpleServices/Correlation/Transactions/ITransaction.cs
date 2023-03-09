using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Interfaces.Common;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.Transactions
{
    internal interface ITransaction
    {
        long SequenceNumber { get; }
        ulong Hash { get; }

        void Commit(IController controller);
        void Rollback();

        Task<int> CommitAsync(ICipMessager messager, IController controller);

        JObject ConvertToJObject();
    }
}
