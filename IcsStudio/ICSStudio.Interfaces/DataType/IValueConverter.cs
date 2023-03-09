namespace ICSStudio.Interfaces.DataType
{
    public interface IValueConverter
    {
        byte[] FormattedStringToByte(string value, int bitSize, uint asaTypeId);

        string ByteToFormattedString(byte[] value, DisplayStyle radix, int bitSize);
    }
}
