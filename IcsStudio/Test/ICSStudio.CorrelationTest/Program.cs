using System;
using System.Collections.Generic;
using ICSStudio.Cip.Objects;
using ICSStudio.SimpleServices;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Transactions;

namespace ICSStudio.CorrelationTest
{
    class Program
    {
        static void Main(string[] args)
        {

            Controller controller = Controller.GetInstance();

            //1. connect
            int result = controller.ConnectAsync("192.168.1.223").GetAwaiter().GetResult();

            List<ITransaction> transactions = null;
            if (result == 0)
            {
                try
                {
                    var cipMessager = controller.CipMessager;

                    CIPController cipController = new CIPController(0, cipMessager);

                    TransactionManager manager = new TransactionManager(controller);

                    cipController.ReaderLock().GetAwaiter().GetResult();
                    transactions = manager.GetAllTransactions(cipMessager).GetAwaiter().GetResult();
                    cipController.ReaderUnLock().GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                

            }

            controller.GoOffline();
        }
    }
}
