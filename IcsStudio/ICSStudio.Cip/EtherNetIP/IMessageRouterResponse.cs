namespace ICSStudio.Cip.EtherNetIP
{
    public interface IMessageRouterResponse
    {
        byte Service { get; }
        byte Reserved { get; }
        byte GeneralStatus { get; }
        ushort[] AdditionalStatus { get; }
        byte[] ResponseData { get; }
    }
}