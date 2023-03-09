using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ICSStudio.SourceProtection
{
    internal class SourceKeyProtection8 : ISourceProtection
    {
        private static string KeyMat = "=F@0]Qe+E#mS#aMTU^khr3-39q^`:ip5G[Y  7ki1GUQ+-M%g`XAdFCs|hO$jutk";

        private readonly Random _random = new Random();

        public int EncryptionConfig => 8;

        public string Encrypt(string plainText, string sourceKey)
        {
            string totalText = plainText + sourceKey;

            byte[] randomBytes = new byte[18];
            _random.NextBytes(randomBytes);

            byte[] key;
            byte[] iv = new byte[16];
            Array.Copy(randomBytes, 2, iv, 0, 16);

            byte[] encrypted;

            using (SHA256 hash = SHA256.Create())
            {
                key = hash.ComputeHash(System.Text.Encoding.ASCII.GetBytes(KeyMat));
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(totalText);
                        }

                        encrypted = stream.ToArray();
                    }
                }
            }

            byte[] bytes = new byte[randomBytes.Length + encrypted.Length];

            Array.Copy(randomBytes, 0, bytes, 0, randomBytes.Length);
            Array.Copy(encrypted, 0, bytes, randomBytes.Length, encrypted.Length);

            return Convert.ToBase64String(bytes, Base64FormattingOptions.InsertLineBreaks);
        }

        public int Decrypt(string b64Text, string sourceKey, out string plainText)
        {
            plainText = string.Empty;

            var bytes = Convert.FromBase64String(b64Text);

            byte[] key;
            byte[] iv = new byte[16];
            Array.Copy(bytes, 2, iv, 0, 16);

            string totalPlainText;

            using (SHA256 hash = SHA256.Create())
            {
                key = hash.ComputeHash(System.Text.Encoding.ASCII.GetBytes(KeyMat));
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor();

                using (MemoryStream stream = new MemoryStream(bytes, 18, bytes.Length - 18))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            totalPlainText = reader.ReadToEnd();
                        }
                    }
                }

            }

            if (totalPlainText.EndsWith(sourceKey))
            {
                plainText = totalPlainText.Substring(0, totalPlainText.Length - sourceKey.Length);

                return 0;
            }

            return -1;
        }

        public bool CheckSourceKey(string b64Text, string sourceKey)
        {
            if (string.IsNullOrEmpty(sourceKey))
                return false;

            string plainText;
            if (Decrypt(b64Text, sourceKey, out plainText) == 0)
                return true;

            return false;
        }
    }

    internal class SourceKeyProtection9 : ISourceProtection
    {
        private readonly Random _random = new Random();

        public int EncryptionConfig => 9;

        public string Encrypt(string plainText, string sourceKey)
        {
            Guid guid = Guid.NewGuid();
            string id = guid.ToString("N");

            byte[] key = new byte[32];
            byte[] iv = new byte[16];

            byte[] hash1;
            byte[] encrypted;

            using (SHA256 hash = SHA256.Create())
            {
                var inputBytes = System.Text.Encoding.Unicode.GetBytes(sourceKey + id);
                hash1 = hash.ComputeHash(inputBytes);

                var hash2Input = new byte[hash1.Length + inputBytes.Length];
                hash1.CopyTo(hash2Input, 0);
                inputBytes.CopyTo(hash2Input, hash1.Length);
                var hash2 = hash.ComputeHash(hash2Input);

                var hash3Input = new byte[hash2.Length + inputBytes.Length];
                hash2.CopyTo(hash3Input, 0);
                inputBytes.CopyTo(hash3Input, hash2.Length);
                var hash3 = hash.ComputeHash(hash3Input);

                Array.Copy(hash2, 0, key, 0, 32);
                Array.Copy(hash3, 0, iv, 0, 16);
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(plainText);
                        }

                        encrypted = stream.ToArray();
                    }
                }
            }

            byte[] randomBytes = new byte[64];
            _random.NextBytes(randomBytes);

            byte[] bytes = new byte[64 + encrypted.Length];
            Array.Copy(randomBytes, 0, bytes, 0, 16);
            Array.Copy(guid.ToByteArray(), 0, bytes, 16, 16);
            Array.Copy(hash1, 0, bytes, 32, 32);
            Array.Copy(encrypted, 0, bytes, 64, encrypted.Length);

            return Convert.ToBase64String(bytes, Base64FormattingOptions.InsertLineBreaks);
        }

        public int Decrypt(string b64Text, string sourceKey, out string plainText)
        {
            plainText = string.Empty;

            var bytes = Convert.FromBase64String(b64Text);

            var bytes16 = new byte[16];
            Array.Copy(bytes, 16, bytes16, 0, 16);

            var bytes32 = new byte[32];
            Array.Copy(bytes, 32, bytes32, 0, 32);

            Guid guid = new Guid(bytes16);

            using (SHA256 hash = SHA256.Create())
            {
                var hash0 = hash.ComputeHash(System.Text.Encoding.Unicode.GetBytes(sourceKey + guid.ToString("N")));
                if (!hash0.SequenceEqual(bytes32))
                    return -1;
            }

            byte[] key = new byte[32];
            byte[] iv = new byte[16];

            using (SHA256 hash = SHA256.Create())
            {
                var inputBytes = System.Text.Encoding.Unicode.GetBytes(sourceKey + guid.ToString("N"));

                var hash2Input = new byte[bytes32.Length + inputBytes.Length];
                bytes32.CopyTo(hash2Input, 0);
                inputBytes.CopyTo(hash2Input, bytes32.Length);
                var hash2 = hash.ComputeHash(hash2Input);

                var hash3Input = new byte[hash2.Length + inputBytes.Length];
                hash2.CopyTo(hash3Input, 0);
                inputBytes.CopyTo(hash3Input, hash2.Length);
                var hash3 = hash.ComputeHash(hash3Input);

                Array.Copy(hash2, 0, key, 0, 32);
                Array.Copy(hash3, 0, iv, 0, 16);
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor();

                using (MemoryStream stream = new MemoryStream(bytes, 64, bytes.Length - 64))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            plainText = reader.ReadToEnd();
                        }
                    }
                }

            }

            return 0;
        }

        public bool CheckSourceKey(string b64Text, string sourceKey)
        {
            var bytes = Convert.FromBase64String(b64Text);

            var bytes16 = new byte[16];
            Array.Copy(bytes, 16, bytes16, 0, 16);

            var bytes32 = new byte[32];
            Array.Copy(bytes, 32, bytes32, 0, 32);

            Guid guid = new Guid(bytes16);

            using (SHA256 hash = SHA256.Create())
            {
                var hash0 = hash.ComputeHash(System.Text.Encoding.Unicode.GetBytes(sourceKey + guid.ToString("N")));
                if (hash0.SequenceEqual(bytes32))
                    return true;
            }

            return false;
        }
    }

    [SuppressMessage("ReSharper", "TooWideLocalVariableScope")]
    internal class SourceKeyProtection10 : ISourceProtection
    {
        private static string KeyMat0 = "h(<11UMp?6o;E`=H+AlOX*~ZOO_O{<Cnb~AeLVh<Z1}~NN{R(}x [K996uafz?iI";
        private static string KeyMat1 = "gWc%{.AH~I6k 8at8-^XJt04Hk?8KKUY`i&od OdsqzdAd0`};[CUd1>9DToV+Yw";

        private readonly Random _random = new Random();

        public int EncryptionConfig => 10;

        public string Encrypt(string plainText, string sourceKey)
        {
            // rand(16) + hash(32) + guid(16) + data

            Guid guid = Guid.NewGuid();
            string id = guid.ToString("N");

            byte[] hash0;
            byte[] hash1;
            byte[] encrypted;

            using (SHA256 hash = SHA256.Create())
            {
                var bytes0 = System.Text.Encoding.Unicode.GetBytes(KeyMat0 + sourceKey);
                hash0 = hash.ComputeHash(bytes0);

                var bytes1 = System.Text.Encoding.Unicode.GetBytes(KeyMat1 + id);
                hash1 = hash.ComputeHash(bytes1);
            }

            byte[] key = new byte[32];
            byte[] iv = new byte[16];
            Array.Copy(hash1, 0, key, 0, 32);
            Array.Copy(hash0, 0, iv, 0, 16);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor();

                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(plainText);
                        }

                        encrypted = stream.ToArray();
                    }
                }
            }

            byte[] randomBytes = new byte[64];
            _random.NextBytes(randomBytes);

            byte[] bytes = new byte[64 + encrypted.Length];
            Array.Copy(randomBytes, 0, bytes, 0, 16);
            Array.Copy(hash0, 0, bytes, 16, 32);
            Array.Copy(guid.ToByteArray(), 0, bytes, 48, 16);
            Array.Copy(encrypted, 0, bytes, 64, encrypted.Length);

            return Convert.ToBase64String(bytes, Base64FormattingOptions.InsertLineBreaks);
        }

        public bool CheckSourceKey(string b64Text, string sourceKey)
        {
            var bytes = Convert.FromBase64String(b64Text);

            var bytes32 = new byte[32];
            Array.Copy(bytes, 16, bytes32, 0, 32);

            using (SHA256 hash = SHA256.Create())
            {
                var hash0 = hash.ComputeHash(System.Text.Encoding.Unicode.GetBytes(KeyMat0 + sourceKey));
                if (hash0.SequenceEqual(bytes32))
                    return true;
            }

            return false;
        }

        public int Decrypt(string b64Text, string sourceKey, out string plainText)
        {
            plainText = string.Empty;

            var bytes = Convert.FromBase64String(b64Text);

            var bytes0 = new byte[16];
            Array.Copy(bytes, 0, bytes0, 0, 16);

            var bytes1 = new byte[32];
            Array.Copy(bytes, 16, bytes1, 0, 32);

            var bytes2 = new byte[16];
            Array.Copy(bytes, 48, bytes2, 0, 16);

            Guid guid = new Guid(bytes2);
            string id = guid.ToString("N");

            byte[] hash0;
            byte[] hash1;

            using (SHA256 hash = SHA256.Create())
            {
                var temp0 = System.Text.Encoding.Unicode.GetBytes(KeyMat0 + sourceKey);
                hash0 = hash.ComputeHash(temp0);

                var temp1 = System.Text.Encoding.Unicode.GetBytes(KeyMat1 + id);
                hash1 = hash.ComputeHash(temp1);
            }

            byte[] key = new byte[32];
            byte[] iv = new byte[16];
            Array.Copy(hash1, 0, key, 0, 32);
            Array.Copy(bytes1, 0, iv, 0, 16);

            if (!string.IsNullOrEmpty(sourceKey) && !hash0.SequenceEqual(bytes1))
            {
                return -1;
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor();

                using (MemoryStream stream = new MemoryStream(bytes, 64, bytes.Length - 64))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            plainText = reader.ReadToEnd();
                        }
                    }
                }

            }

            return 0;
        }

    }
}
