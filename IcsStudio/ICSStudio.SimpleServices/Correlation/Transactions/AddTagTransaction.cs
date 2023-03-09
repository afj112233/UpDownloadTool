using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Tags;
using Newtonsoft.Json.Linq;
using NLog;

namespace ICSStudio.SimpleServices.Transactions
{
    internal class AddTagTransaction : Transaction
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public AddTagTransaction(string program)
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
            TagCollection tagCollection = null;

            if (Context == "Ctrl")
            {
                tagCollection = controller.Tags as TagCollection;
            }
            else if (Context == "Program")
            {
                tagCollection = controller?.Programs[Program]?.Tags as TagCollection;
            }

            if (tagCollection != null)
            {
                Tag tag = TagsFactory.CreateTag(tagCollection, Data);

                var existTag = tagCollection[tag.Name];

                if (existTag == null)
                {
                    tagCollection.AddTag(tag, false, false);
                }
                else
                {
                    Logger.Trace($"{SequenceNumber} Commit failed: tag {tag.Name} exist!");
                }
            }
        }

        public override async Task<int> CommitAsync(ICipMessager messager, IController controller)
        {
            CIPController cipController = new CIPController(0, messager);

            if (Context == "Ctrl")
            {
                await cipController.TransmitCfg(ToMsgPack(Data));
                await cipController.CreateTag();

                return 0;
            }

            if (Context == "Program")
            {
                int programId = await cipController.FindProgramId(Program);
                CIPProgram cipProgram = new CIPProgram(programId, messager);

                await cipController.TransmitCfg(ToMsgPack(Data));
                await cipProgram.CreateTag();

                return 0;
            }

            return -1;
        }
    }
}
