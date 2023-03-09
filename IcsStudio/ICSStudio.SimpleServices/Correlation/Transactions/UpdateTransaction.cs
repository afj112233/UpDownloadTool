using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.Transactions
{
    internal class UpdateTransaction : Transaction
    {
        public UpdateTransaction(string program)
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

        public override async Task<int> CommitAsync(ICipMessager messager, IController controller)
        {
            if (Context == "Program")
            {
                CIPController cipController = new CIPController(0, messager);

                int programId = await cipController.FindProgramId(Program);
                CIPProgram cipProgram = new CIPProgram(programId, messager);

                string taskName = controller?.Programs[Program]?.ParentTask?.Name;
                if (string.IsNullOrEmpty(taskName))
                    return -100;

                int taskId = await cipController.FindTaskId(taskName);

                await cipController.TransmitCfg(ToMsgPack(Data));
                await cipProgram.Update(taskId);
                
                return 0;
            }

            return -1;
        }
    }
}
