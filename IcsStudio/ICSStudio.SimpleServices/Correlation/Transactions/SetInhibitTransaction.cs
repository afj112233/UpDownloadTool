using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.SimpleServices.Transactions
{
    internal class SetInhibitTransaction : Transaction
    {
        public override void Commit(IController controller)
        {
            if (Data.ContainsKey("Name") && Data.ContainsKey("Value"))
            {
                string name = Data["Name"]?.ToString();
                bool value = (bool)Data["Value"];

                if (Context == "Task")
                {
                    CTask task = controller.Tasks[name] as CTask;
                    if (task != null)
                    {
                        task.IsInhibited = value;
                    }
                }

                if (Context == "Program")
                {
                    Program program = controller.Programs[name] as Program;
                    if (program != null)
                    {
                        program.Inhibited = value;
                    }
                }
            }
        }
    }
}
