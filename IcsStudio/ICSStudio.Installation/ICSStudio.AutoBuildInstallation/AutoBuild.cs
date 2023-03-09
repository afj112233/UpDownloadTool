using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ICSStudio.AutoBuildInstallation
{
    public class AutoBuild
    {
        public static void Main()
        {
            SetProduct();
            SetBundle();
            //Release();
            Console.WriteLine("Completed");
        }

        private static void SetBundle()
        {
            if (File.Exists("temp2.wxs"))
            { 
                var content = File.ReadAllText("temp2.wxs");
                var basePath = System.AppDomain.CurrentDomain.BaseDirectory;
                content = string.Format(content,
                    $"{basePath}../../IcsStudio/ICSStudio.Installation/Setup/bin/Debug/Setup.msi",
                    $"{basePath}../../IcsStudio/ICSStudio.Installation/BurnInstallation/File/vs_isoshell.exe",
                    $"{basePath}../../IcsStudio/ICSStudio.Installation/Setup/File/License.rtf");
                var fileStream = File.Create($"{basePath}../../IcsStudio/ICSStudio.Installation/BurnInstallation/Bundle.wxs");
                //Console.WriteLine(content);
                var info=new UTF8Encoding(true).GetBytes(content);
                fileStream.Write(info,0,info.Length);
                fileStream.Close();
            }
        }

        private static void SetProduct()
        {
            var path = @"..\..\Release";
            var exists = Directory.Exists(path);
            if (exists)
            {
                var dirInfo = new StringBuilder();
                var fileInfo = new StringBuilder();
                int index = 0;
                GetDirInfo(dirInfo, fileInfo, path, ref index);
                var root = new DirectoryInfo(path);
                fileInfo.AppendFormat("<Component Id=\"ProductComponent{0}\" Directory=\"TESTFILEPRODUCTDIR\" Guid=\"{1}\">", index, Guid.NewGuid());
                foreach (var file in root.EnumerateFiles())
                {
                    if (".pdb".Equals(file.Extension, StringComparison.OrdinalIgnoreCase)) continue;
                    fileInfo.AppendFormat("<File Source=\"{0}\"></File>", file.FullName);
                }
                fileInfo.AppendLine("<Shortcut Id=\"UninstallICSStudio2\" Name=\"Uninstall\" Target=\"[SystemFolder]msiexec.exe\" Arguments=\"/x [ProductCode]\" Description=\"Uninstall\" Directory=\"TESTFILEPRODUCTDIR\" WorkingDirectory=\"TESTFILEPRODUCTDIR\"/>");
                fileInfo.AppendLine("</Component>");
                if (File.Exists("temp.wxs"))
                {
                    var content = File.ReadAllText("temp.wxs");
                    var license = new FileInfo("../../IcsStudio/ICSStudio.Installation/Setup/File/License.rtf");

                    content = string.Format(content, dirInfo, fileInfo, license.FullName);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine(content);
                    var fileStream = File.Create("../../IcsStudio/ICSStudio.Installation/Setup/Product.wxs");
                    var info = new UTF8Encoding(true).GetBytes(content);
                    fileStream.Write(info, 0, info.Length);
                    fileStream.Close();
                }
            }
        }
        
        private static void Release()
        {
            Process p=new Process();
            var path = Environment.CurrentDirectory;
            if (!path.EndsWith("Debug"))
            {
                path = $"{path}\\AutoBuild\\Debug";
            }
            p.StartInfo=new ProcessStartInfo(path + "\\Release.bat") {CreateNoWindow = true,};
            p.Start();

            p.WaitForExit();
            p.Close();
        }
        
        private static void GetDirInfo(StringBuilder stringBuilder,StringBuilder fileInfo,string path,ref int index)
        {
            var root=new DirectoryInfo(path);
            foreach (var subDir in root.GetDirectories())
            {
                index++;
                var files = subDir.EnumerateFiles();
                //if(files.Any())
                    fileInfo.AppendFormat("<Component Id=\"ProductComponent{0}\" Directory=\"{1}\" Guid=\"{2}\">", index,
                       FixName(subDir.Name,"Dir_"), Guid.NewGuid());
                foreach (var file in subDir.EnumerateFiles())
                {
                    if (file.Name == "cc1.exe")
                    {

                    }
                    //if (".pdb".Equals(file.Extension, StringComparison.OrdinalIgnoreCase)) continue;
                    fileInfo.AppendFormat("<File Source=\"{0}\" Id=\"{1}\"></File>",file.FullName,$"file{_fileCount++}");
                }

                if (!files.Any())
                    fileInfo.AppendLine("  <CreateFolder/>");
                    fileInfo.AppendLine("</Component>");
                var name = FixName(subDir.Name, "");
                stringBuilder.AppendFormat("<Directory Id=\"{0}\" Name=\"{1}\">",$"Dir_{name}", subDir.Name);
                GetDirInfo(stringBuilder, fileInfo, subDir.FullName,ref index);
                stringBuilder.AppendLine("</Directory>");
                index++;
            }

        }
        private static List<string> _nameList =new List<string>();
        private static int _fileCount=0;
        private static string FixName(string name,string prefix)
        {
            var fixName= $"{prefix}{name.Replace(" ", "")}".Replace("-", "_").Replace(".", "_");
            if ("Dir_lib" == fixName)
            {

            }
            if (_nameList.Contains(fixName))
            {
                fixName = Utils.Utils.GetNotDuplicateName(fixName, _nameList);
            }
            _nameList.Add(fixName);
            return fixName;
        }
    }
}
