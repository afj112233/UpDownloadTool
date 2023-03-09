namespace ICSStudio.Utils
{
    public static class CatalogNumberExtension
    {
        public static string RemoveSeries(this string catalogNumber)
        {
            if (!catalogNumber.Contains("/"))
                return catalogNumber;

            int index = catalogNumber.IndexOf('/');
            return catalogNumber.Substring(0, index);
        }

        public static string GetSeries(this string catalogNumber)
        {
            if (!catalogNumber.Contains("/"))
                return string.Empty;

            int index = catalogNumber.IndexOf('/');
            return catalogNumber.Substring(index + 1, catalogNumber.Length - index - 1);
        }
    }
}
