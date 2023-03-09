using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Notification;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Notification;

namespace ICSStudio.SimpleServices.Compiler
{
    public class PendingCompileRoutine
    {
        private static readonly PendingCompileRoutine _pendingCompileRoutine=new PendingCompileRoutine();

        private PendingCompileRoutine()
        {

        }

        public static PendingCompileRoutine GetInstance()
        {
            return _pendingCompileRoutine;
        }

        public List<Tuple<IRoutine,bool>> PendingList { get; }=new List<Tuple<IRoutine, bool>>();

        public void Add(IRoutine routine, bool isForce)
        {
            if (PendingList.All(p => p.Item1 != routine))
            {
                PendingList.Add(new Tuple<IRoutine, bool>(routine,isForce));
            }
        }

        public void CompilePendingRoutines()
        {
            try
            {
                foreach (var tuple in PendingList)
                {
                    Notifications.Publish(
                        new MessageData()
                            {Type = MessageData.MessageType.Verify, Object = tuple.Item1});
                    var st = tuple.Item1 as STRoutine;
                    if (st != null)
                    {
                        st.IsUpdateChanged = true;
                    }
                }

            }
            catch (Exception e)
            {
                Debug.Assert(false, e.StackTrace);
            }
            finally
            {
                PendingList.Clear();
            }
        }
    }
}
