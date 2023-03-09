using System;
using ICSStudio.Utils;

namespace ICSStudio.AssemblyUtilsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string temp = AssemblyUtils.AssemblyDirectory;

            Console.WriteLine(temp);

            Console.ReadKey();
        }
    }
}
