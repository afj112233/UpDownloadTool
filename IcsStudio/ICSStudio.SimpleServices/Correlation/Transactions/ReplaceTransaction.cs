using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.Transactions
{
    internal class ReplaceTransaction : Transaction
    {
        public ReplaceTransaction(string program)
        {
            Program = program;
        }

        public string Program { get; }

        public override JObject ConvertToJObject()
        {
            JObject jObject = base.ConvertToJObject();

            if (!string.IsNullOrEmpty(Program))
            {
                jObject.Add("Program", Program);
            }

            return jObject;
        }

        public override void Commit(IController controller)
        {
            if (Context == "Routine")
            {
                var program = controller.Programs[Program] as Program;
                Controller myController = controller as Controller;

                if (program != null && myController != null)
                {
                    IRoutine newRoutine = myController.CreateRoutine(Data);
                    IRoutine oldRoutine = program.Routines[newRoutine.Name];

                    //TODO(gjc): add code here, only code change

                    // ST
                    STRoutine newSTRoutine = newRoutine as STRoutine;
                    STRoutine oldSTRoutine = oldRoutine as STRoutine;

                    if (newSTRoutine != null && oldSTRoutine != null)
                    {
                        oldSTRoutine.CodeText = new List<string>(newSTRoutine.CodeText);
                        return;
                    }

                    // RLL
                    RLLRoutine newRLLRoutine = newRoutine as RLLRoutine;
                    RLLRoutine oldRLLRoutine = oldRoutine as RLLRoutine;
                    if (newRLLRoutine != null && oldRLLRoutine != null)
                    {
                        oldRLLRoutine.UpdateRungs(newRLLRoutine.CodeText);
                        return;
                    }

                    //TODO(gjc): add code here
                    throw new NotImplementedException("Add code here for ReplaceTransaction!");

                }
            }
        }

        public override async Task<int> CommitAsync(ICipMessager messager, IController controller)
        {
            if (Context == "Routine")
            {
                if (Data != null && Data.ContainsKey("Name"))
                {
                    string routineName = Data["Name"]?.ToString();

                    if (!string.IsNullOrEmpty(routineName))
                    {
                        CIPController cipController = new CIPController(0, messager);

                        int programId = await cipController.FindProgramId(Program);
                        CIPProgram cipProgram = new CIPProgram(programId, messager);

                        var routineId = await cipProgram.FindRoutineId(routineName);
                        CIPRoutine cipRoutine = new CIPRoutine((ushort)routineId, messager);

                        await cipController.TransmitCfg(ToMsgPack(Data));
                        await cipRoutine.Replace();

                        return 0;
                    }

                }
            }

            return -1;
        }
    }
}
