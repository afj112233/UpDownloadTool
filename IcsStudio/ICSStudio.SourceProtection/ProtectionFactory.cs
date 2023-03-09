namespace ICSStudio.SourceProtection
{
    public class ProtectionFactory
    {
        public static ISourceProtection Create(int config = 0)
        {
            switch (config)
            {
                case 8:
                    return new SourceKeyProtection8();
                case 9:
                    return new SourceKeyProtection9();
                case 10:
                    return new SourceKeyProtection10();
                default:
                    return new SourceKeyProtection10();
            }
        }
    }
}
