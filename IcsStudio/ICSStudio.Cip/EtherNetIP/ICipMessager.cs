using System;
using System.Threading.Tasks;

namespace ICSStudio.Cip.EtherNetIP
{
    public enum ConnectionStatus
    {
        Disconnected,
        Netconnected,
        RegisterSession,
        ForwardOpen
    }

    public interface ICipMessager
    {
        Task<IMessageRouterResponse> SendRRData(IMessageRouterRequest request);
        Task<IMessageRouterResponse> SendUnitData(IMessageRouterRequest request);
        Task ListIdentity();

        Task<int> OnLine(bool beForwardOpen);
        int OffLine();

        void SetConnectAddress(string address);

        ConnectionStatus ConnectionStatus { get; }

        event EventHandler Connected;

        event EventHandler Disconnected;
    }
}