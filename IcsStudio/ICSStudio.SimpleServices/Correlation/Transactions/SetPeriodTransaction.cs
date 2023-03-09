using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.SimpleServices.Transactions
{
    internal class SetPeriodTransaction : Transaction
    {
        public override void Commit(IController controller)
        {
            if (Context == "Task")
            {
                if (Data.ContainsKey("Name") && Data.ContainsKey("Value"))
                {
                    string taskName = Data["Name"]?.ToString();
                    double period = (double)Data["Value"];

                    var task = controller.Tasks[taskName] as CTask;
                    if (task != null)
                    {
                        task.Rate = (float)period;
                    }
                }

            }
        }
    }
}
