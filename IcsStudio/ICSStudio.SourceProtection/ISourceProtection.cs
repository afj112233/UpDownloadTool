namespace ICSStudio.SourceProtection
{
    public interface ISourceProtection
    {
        int EncryptionConfig { get; }

        string Encrypt(string plainText, string sourceKey);
        int Decrypt(string b64Text, string sourceKey, out string plainText);
        bool CheckSourceKey(string b64Text, string sourceKey);
    }
}
