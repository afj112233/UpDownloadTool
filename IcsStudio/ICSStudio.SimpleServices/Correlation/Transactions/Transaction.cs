using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Interfaces.Common;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSStudio.SimpleServices.Transactions
{
    internal abstract class Transaction : ITransaction
    {
        private static readonly Random Random = new Random();

        public long SequenceNumber { get; private set; }

        public ulong Hash { get; private set; }

        public string Action { get; private set; }

        public string Context { get; private set; }

        public JObject Data { get; private set; }
        
        public virtual void Commit(IController controller)
        {

        }

        public virtual void Rollback()
        {

        }

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        public async virtual Task<int> CommitAsync(ICipMessager messager, IController controller)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            return 0;
        }

        public virtual JObject ConvertToJObject()
        {
            JObject jObject = new JObject
            {
                { "SequenceNumber", SequenceNumber },
                { "Hash", Hash },
                { "Action", Action },
                { "Context", Context }
            };

            if (Data != null)
            {
                jObject.Add("Data", Data);
            }

            return jObject;
        }

        public static ITransaction CreateSetupTransaction()
        {
            SetupTransaction transaction = new SetupTransaction
            {
                SequenceNumber = NextSequenceNumber(),
                Hash = NextHash(),
                Action = "Setup",
                Context = "ICS Studio use",
                Data = null
            };

            return transaction;
        }

        public static ITransaction CreateSetupTransaction(long logId)
        {
            SetupTransaction transaction = new SetupTransaction
            {
                SequenceNumber = logId,
                Hash = NextHash(),
                Action = "Setup",
                Context = "ICS Studio use",
                Data = null
            };

            return transaction;
        }

        public static ITransaction CreateTransaction(long sequenceNumber, List<byte> data)
        {
            JObject jObject = FromMsgPack(data);

            return CreateTransaction(sequenceNumber, jObject);
        }

        public static ITransaction CreateTransaction(long sequenceNumber, JObject jObject)
        {
            if (!jObject.ContainsKey("Hash"))
                return null;

            if (!jObject.ContainsKey("Action"))
                return null;

            if (!jObject.ContainsKey("Context"))
                return null;
            
            ulong hash = (ulong)jObject["Hash"];
            string action = jObject["Action"].ToString();
            string context = jObject["Context"].ToString();

            JObject data = null;
            if (jObject.ContainsKey("Data"))
                data = jObject["Data"] as JObject;

            string program = string.Empty;
            if (jObject.ContainsKey("Program"))
            {
                program = jObject["Program"].ToString();
            }

            Transaction transaction;
            
            switch (action)
            {
                case "AddTag":
                    transaction = new AddTagTransaction(program);
                    break;

                case "Replace":
                    transaction = new ReplaceTransaction(program);
                    break;

                case "Update":
                    transaction = new UpdateTransaction(program);
                    break;

                case "AddProgram":
                    transaction = new AddProgramTransaction();
                    break;

                case "AddMDType":
                    transaction = new AddMDTypeTransaction();
                    break;

                case "AddModule":
                    transaction = new AddModuleTransaction();
                    break;

                case "AddTask":
                    transaction = new AddTaskTransaction();
                    break;

                case "AddType":
                    transaction = new AddUDTypeTransaction();
                    break;

                case "AddAOI":
                    transaction = new AddAOITransaction();
                    break;

                case "AddTrend":
                    transaction = new AddTrendTransaction();
                    break;

                case "SetTimeSynchronize":
                    transaction = new SetTimeSynchronizeTransaction();
                    break;

                case "SetNativeCode":
                    transaction = new SetNativeCodeTransaction();
                    break;

                case "SetName":
                    transaction = new SetNameTransaction();
                    break;

                case "SetMajorFaultProgram":
                    transaction = new SetMajorFaultProgramTransaction();
                    break;

                case "SetPowerLossProgram":
                    transaction = new SetPowerLossProgramTransaction();
                    break;

                case "SetPeriod":
                    transaction = new SetPeriodTransaction();
                    break;

                case "SetInhibit":
                    transaction = new SetInhibitTransaction();
                    break;

                case "Setup":
                    transaction = new SetupTransaction();
                    break;
                    
                default:
                    throw new NotImplementedException($"Add code for action:{action}!");
            }

            {
                transaction.SequenceNumber = sequenceNumber;
                transaction.Hash = hash;
                transaction.Action = action;
                transaction.Context = context;
                transaction.Data = data;
            }

            return transaction;
        }

        private static long NextSequenceNumber()
        {
            byte[] bytes = new byte[8];

            Random.NextBytes(bytes);

            long number = BitConverter.ToInt64(bytes, 0);

            return Math.Abs(number);
        }

        private static ulong NextHash()
        {
            byte[] bytes = new byte[8];

            Random.NextBytes(bytes);

            return BitConverter.ToUInt64(bytes, 0);
        }

        protected static JObject FromMsgPack(List<byte> data)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                data[i] = (byte)(data[i] ^ 0x5A);
            }

            var obj = (JObject)JsonConvert.DeserializeObject(MessagePackSerializer.ToJson(data.ToArray()));
            return obj;
        }

        protected static List<byte> ToMsgPack(JObject obj)
        {
            var res = MessagePackSerializer.FromJson(JsonConvert.SerializeObject(obj)).ToList();
            for (int i = 0; i < res.Count; ++i)
            {
                res[i] = (byte)(res[i] ^ 0x5A);
            }

            return res;
        }
    }
}
