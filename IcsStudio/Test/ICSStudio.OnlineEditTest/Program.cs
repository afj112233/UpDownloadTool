using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Online;

namespace ICSStudio.OnlineEditTest
{
    class Program
    {
        static void Main( /*string[] args*/)
        {
            //string l5xFile = "online_test.L5X";
            string jsonFile = "a0902.json";//"online_test.json";

            try
            {
                // 1.open
                //Converter.L5XToJson(l5xFile, jsonFile);
                Controller controller = Controller.Open(jsonFile);

                // 2.online
                controller.GenCode();
                controller.ConnectAsync("192.168.1.211").GetAwaiter().GetResult();
                controller.Download(ControllerOperationMode.OperationModeNull, false, false).GetAwaiter().GetResult();
                controller.RebuildTagSyncControllerAsync().GetAwaiter().GetResult();
                controller.UpdateState().GetAwaiter();

                while (true)
                {
                    if (controller.OperationMode == ControllerOperationMode.OperationModeProgram)
                        break;

                    Thread.Sleep(1000);
                }

                controller.ChangeOperationMode(ControllerOperationMode.OperationModeRun).GetAwaiter().GetResult();

                // 3.change
                var mainProgram = controller.Programs["MainProgram"] as SimpleServices.Common.Program;
                var routine = mainProgram?.Routines["MainRoutine"] as STRoutine;
                var tag = controller.Tags["dint"];

                Contract.Assert(mainProgram != null);
                Contract.Assert(routine != null);
                Contract.Assert(tag != null);

                var result = controller.GetTagValueFromPLC(tag, "dint");
                Console.WriteLine($"dint: {result.Result.Item2}");

                routine.CodeText = new List<string>
                {
                    "if s:fs then",
                    "  dint := 0;",
                    "end_if;",
                    "",
                    "dint := dint + 1;"
                };

                OnlineEditHelper onlineEditHelper = new OnlineEditHelper(controller.CipMessager);

                var time0 = onlineEditHelper.GetTimestamp().GetAwaiter().GetResult();
                Console.WriteLine($"time:{time0}");

                onlineEditHelper.ReplaceRoutine(routine).GetAwaiter().GetResult();

                // 4.update
                onlineEditHelper.UpdateProgram(mainProgram).GetAwaiter().GetResult();

                result = controller.GetTagValueFromPLC(tag, "dint");
                Console.WriteLine($"dint: {result.Result.Item2}");


                // 5. change tag name
                tag.Name = "dint123";
                int tagId = controller.GetTagId(tag);
                onlineEditHelper.ChangeTagName(tagId, "dint123").GetAwaiter().GetResult();

                routine.CodeText = new List<string>
                {
                    "if s:fs then",
                    "  dint123 := 0;",
                    "end_if;",
                    "",
                    "dint123 := dint123 + 1;"
                };
                onlineEditHelper.ReplaceRoutine(routine).GetAwaiter().GetResult();

                onlineEditHelper.UpdateProgram(mainProgram).GetAwaiter().GetResult();

                // 5. check result
                result = controller.GetTagValueFromPLC(tag, "dint123");
                Console.WriteLine($"dint123: {result.Result.Item2}");

                var time1 = onlineEditHelper.GetTimestamp().GetAwaiter().GetResult();
                Console.WriteLine($"time:{time1}");

                // 6. create tag
                //var newTag = TagsFactory.CreateTag(controller.Tags as TagCollection, "new_dint", "dint", 0, 0, 0);
                //newTag.Usage = Usage.Local;
                //newTag.TagType = TagType.Base;
                //newTag.ExternalAccess = ExternalAccess.ReadWrite;
                //newTag.DisplayStyle = DisplayStyle.Decimal;

                //onlineEditHelper.CreateTagInController(newTag.ConvertToJObject()).GetAwaiter().GetResult();

                // 6.offline
                controller.GoOffline();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                Console.ReadKey();
            }

            Console.ReadKey();
        }
    }
}
