namespace ICSStudio.SimpleServices.Chart
{
    public interface IChart : IContent
    {
        int ID { get; }
        double X { get; }
        double Y { get; }
    }
}
