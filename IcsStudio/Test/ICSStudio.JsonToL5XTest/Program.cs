using ICSStudio.FileConverter.JsonToL5X;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.JsonToL5XTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string jsonFile = @"C:\Users\gjc\Desktop\temp\b010erm_10.json";
            string l5xFile = @"C:\Users\gjc\Desktop\temp\b010erm_10.L5X";

            var controller = Controller.Open(jsonFile);
            controller.ExportL5X(l5xFile);
        }
    }
}
