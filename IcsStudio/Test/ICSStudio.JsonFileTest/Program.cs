using System;

namespace ICSStudio.JsonFileTest
{
    class Program
    {
        static void Main()
        {
            CyMain();
        }

        static void CyMain()
        {
            string directory = System.AppDomain.CurrentDomain.BaseDirectory;
            string openFile = @"C:\Users\icon\Desktop\Turbo.json";
            string saveFile = @"C:\Users\icon\Desktop\L36.json";
            var controller = SimpleServices.Common.Controller.Open(openFile);
            controller.GenCode();
            controller.Save(saveFile);
        }

        static void OrigMain()
        {
            string directory = System.AppDomain.CurrentDomain.BaseDirectory;
            string openFile = @"C:\Users\gjc\Desktop\open_test\PNE22083111_A.json";
            string saveFile = @"C:\Users\icon\Desktop\L36.json";

            int index = 0;
            while (index < 10)
            {
                Console.WriteLine($"{index}");

                var controller = SimpleServices.Common.Controller.Open(openFile);

                index++;

                GC.Collect();
            }

            Console.ReadKey();

            //var controller = SimpleServices.Common.Controller.Open(openFile);
            //controller.GenCode();
            //controller.Save(saveFile);

            //var candidateInstructions = controller.SupportedInstructions.GetCandidateInstructions("sqrt");

            //var matchInstructionInfo = controller.SupportedInstructions.GetFirstMatchInstructionInfo("sqrt",
            //    new[]
            //    {
            //        new DataTypeInfo() {DataType = controller.DataTypes["real"]}
            //    });
        }
    }
}
