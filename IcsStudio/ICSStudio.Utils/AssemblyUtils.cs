using System;
using System.IO;
using System.Reflection;

namespace ICSStudio.Utils
{
    public class AssemblyUtils
    {
        private static string _assemblyDirectory;

        public static string AssemblyDirectory
        {
            get
            {
                if (!string.IsNullOrEmpty(_assemblyDirectory))
                    return _assemblyDirectory;

                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                string path = new Uri(codeBase).LocalPath;
                _assemblyDirectory = Path.GetDirectoryName(path);

                return _assemblyDirectory;

            }
        }
    }
}
