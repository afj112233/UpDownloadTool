using System;
using ICSStudio.SourceProtection;

namespace ICSStudio.SourceProtectionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string plainText = "Hello world!\nHello world!";
            string key = "test";

            var protection = ProtectionFactory.Create(10);

            var encryptResult = protection.Encrypt(plainText, key);

            Console.WriteLine(encryptResult);

            bool temp0 = protection.CheckSourceKey(encryptResult, key);
            bool temp1 = protection.CheckSourceKey(encryptResult, "fasd");
            
            string decryptResult;

            protection.Decrypt(encryptResult, key, out decryptResult);

            Console.WriteLine(decryptResult);

            protection.Decrypt(encryptResult, null, out decryptResult);
            Console.WriteLine(decryptResult);

        }
    }
}
