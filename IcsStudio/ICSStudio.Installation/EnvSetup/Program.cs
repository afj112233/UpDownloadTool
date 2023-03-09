using System;
using Microsoft.Win32;

namespace ICSStudio.EnvSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            // C:\Users\Public\Documents\i-con\ICSStudio\Logs
            // create folder
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + @"\i-con\ICSStudio\Logs";

            try
            {
                System.IO.Directory.CreateDirectory(folder);
            }
            catch (Exception)
            {
                Console.WriteLine("Create folder failed!");
                Console.ReadLine();
                return;
            }
            
            // import reg
            try
            {
                RegistryKey key = Registry.LocalMachine;
                
                //RegistryKey localDumps =
                //    key.OpenSubKey(@"SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps", true);


                RegistryKey localDumps = key.CreateSubKey("SOFTWARE")?
                    .CreateSubKey("Microsoft")?
                    .CreateSubKey("Windows")?
                    .CreateSubKey("Windows Error Reporting")?
                    .CreateSubKey("LocalDumps");

                if (localDumps != null)
                {
                    RegistryKey myKey = localDumps.CreateSubKey("ICSStudio.exe");
                    if (myKey != null)
                    {
                        myKey.SetValue("CustomDumpFlags", 0, RegistryValueKind.DWord);
                        myKey.SetValue("DumpCount", 1, RegistryValueKind.DWord);
                        myKey.SetValue("DumpFolder", folder, RegistryValueKind.ExpandString);
                        myKey.SetValue("DumpType", 2, RegistryValueKind.DWord);
                        myKey.SetValue("Revision", 1, RegistryValueKind.DWord);
                        return;
                    }
                }

                Console.WriteLine("Import reg failed!");
                Console.ReadLine();
            }
            catch (Exception)
            {
                Console.WriteLine("Import reg failed!");
                Console.ReadLine();
            }
        }
    }
}
