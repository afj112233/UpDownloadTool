namespace ICSStudio.SimpleServices.Chart
{
    public interface ILink : IContent
    {
        int FromID { get; }
        int ToID { get; }
        bool Show { get; }
    }
}
