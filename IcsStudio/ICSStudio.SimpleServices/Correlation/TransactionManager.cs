using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Transactions;
using ICSStudio.SimpleServices.Utilities;
using Newtonsoft.Json.Linq;
using NLog;
using Timer = System.Timers.Timer;

namespace ICSStudio.SimpleServices
{
    public enum CompareResult
    {
        ExactlySame = 0,
        ControllerIsNewer = 1,
        OfflineProjectIsNewer = 2,

        NoProject = -1,
        NotSupported = -2,
        NoLogInPc = -3,
        NameNotMatch = -4,
        DiffTooLarge = -5,
        HashDiff = -6,

        FailedToGetLock = -50,

        Exception = -100,
        Unknown = -1000
    }

    internal class TransactionManager
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<ITransaction> _transactions;

        private readonly Timer _getChangeLogTimer;

        private readonly List<Tuple<string, string>> _replacedRoutines;

        public TransactionManager(Controller controller)
        {
            Controller = controller;

            HasTransaction = false;

            _transactions = new List<ITransaction>();

            _replacedRoutines = new List<Tuple<string, string>>();

            WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                Controller, "IsOnlineChanged", OnIsOnlineChanged);

            _getChangeLogTimer = new Timer(500);
            _getChangeLogTimer.Elapsed += GetChangeLogHandle;
            _getChangeLogTimer.AutoReset = false;
        }

        private async void GetChangeLogHandle(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (Controller.IsOnline)
                {
                    CIPController cipController = new CIPController(0, Controller.CipMessager);

                    if (HasTransaction)
                    {
                        long currentLogId = await cipController.GetCurrentLogId();
                        long currentTransactionIndex = CurrentTransactionIndex;

                        if (Utils.Utils.SequenceCompare(currentLogId, currentTransactionIndex) > 0)
                        {
                            //TODO(gjc): need edit later

                            int result = await cipController.EnterReadLock();
                            if (result == 0)
                            {
                                var transactions = await GetTransactions(Controller.CipMessager,
                                    currentTransactionIndex, currentLogId - 1);

                                await cipController.ExitReadLock();

                                AddTransactions(transactions);
                            }
                        }

                        if (Utils.Utils.SequenceCompare(currentLogId, currentTransactionIndex) < 0)
                        {
                            Logger.Error($"Sync change log failed! {currentLogId},{currentTransactionIndex}");
                        }
                    }
                    else
                    {
                        //TODO(gjc): need edit later

                        int result = await cipController.EnterReadLock();
                        if (result == 0)
                        {
                            var transactions = await GetAllTransactions(Controller.CipMessager);

                            await cipController.ExitReadLock();

                            AddTransactions(transactions);
                        }

                    }
                }
            }
            catch (Exception)
            {
                //Log.Error(exception);
            }
            finally
            {
                if (Controller.IsOnline)
                {
                    _getChangeLogTimer.Start();
                }
            }

        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            Task.Run(() =>
            {
                if (e.NewValue)
                {
                    _getChangeLogTimer.Start();
                }

            });
        }

        ~TransactionManager()
        {
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                Controller, "IsOnlineChanged", OnIsOnlineChanged);
        }

        public Controller Controller { get; }

        public bool HasTransaction { get; private set; }

        public long CurrentTransactionIndex { get; private set; }

        public List<Tuple<string, string>> ReplacedRoutines => _replacedRoutines;

        public JArray ConvertToJArray()
        {
            if (HasTransaction)
            {
                JArray array = new JArray();

                foreach (var transaction in _transactions)
                {
                    array.Add(transaction.ConvertToJObject());
                }

                return array;
            }

            return null;
        }

        public void Reset()
        {
            HasTransaction = false;

            _transactions.Clear();
        }

        internal async Task<int> SetupAsync(ICipMessager messager)
        {
            CIPController cipController = new CIPController(0, messager);

            if (!HasTransaction)
            {
                AddTransactions(new List<ITransaction>
                {
                    Transaction.CreateSetupTransaction()
                });
            }

            if (!HasTransaction)
                return -1;

            long currentLogId;
            ulong currentHash;

            ITransaction transaction = _transactions.Last();
            if (transaction is SetupTransaction)
            {
                currentLogId = transaction.SequenceNumber;
                currentHash = transaction.Hash;
            }
            else
            {
                currentLogId = transaction.SequenceNumber + 1;
                currentHash = transaction.Hash;
            }

            await cipController.SetLogId(currentLogId, (long)currentHash);

            return 0;
        }

        internal async Task<int> PostUploadSetupAsync(ICipMessager cipMessager)
        {
            //var transactions = await GetAllTransactions(cipMessager);

            var transactions = await GetLatestTransactions(cipMessager, 16);

            if (transactions != null && transactions.Count > 0)
            {
                AddTransactions(transactions);
            }
            else
            {
                CIPController cipController = new CIPController(0, cipMessager);

                long logId = await cipController.GetCurrentLogId();

                AddTransactions(new List<ITransaction>
                {
                    Transaction.CreateSetupTransaction(logId)
                });
            }

            return 0;
        }

        public void AddTransactions(List<ITransaction> transactions)
        {
            if (transactions == null)
                return;

            if (transactions.Count == 0)
                return;

            ITransaction lastTransaction;

            if (HasTransaction)
            {
                lastTransaction = _transactions.Last();
                if (lastTransaction is SetupTransaction)
                {
                    _transactions.Remove(lastTransaction);
                }
            }

            _transactions.AddRange(transactions);

            HasTransaction = true;

            lastTransaction = _transactions.Last();

            if (lastTransaction is SetupTransaction)
            {
                CurrentTransactionIndex = lastTransaction.SequenceNumber;
            }
            else
            {
                CurrentTransactionIndex = lastTransaction.SequenceNumber + 1;
            }
        }

        public void AddTransactions(JArray jArray)
        {
            List<ITransaction> transactions = new List<ITransaction>();

            foreach (var jObject in jArray.OfType<JObject>())
            {
                if (jObject.ContainsKey("SequenceNumber"))
                {
                    long sequenceNumber = (long)jObject["SequenceNumber"];

                    transactions.Add(Transaction.CreateTransaction(sequenceNumber, jObject));
                }
            }

            AddTransactions(transactions);
        }

        private async Task<List<ITransaction>> GetTransactions(ICipMessager messager, long from, long to)
        {
            long start = from;
            long end = to;

            if (Utils.Utils.SequenceCompare(from, to) > 0)
            {
                start = to;
                end = from;
            }

            CIPController cipController = new CIPController(0, messager);
            List<ITransaction> transactions = new List<ITransaction>();

            try
            {
                for (long index = start; index <= end; index++)
                {
                    var logBytes = await cipController.ReadChangeLog(index);

                    transactions.Add(Transaction.CreateTransaction(index, logBytes));
                }
            }
            catch (Exception)
            {
                transactions.Clear();
            }

            return transactions;
        }

        private async Task<ITransaction> GetTransaction(ICipMessager messager, long index)
        {
            CIPController cipController = new CIPController(0, messager);

            try
            {
                var logBytes = await cipController.ReadChangeLog(index);
                return Transaction.CreateTransaction(index, logBytes);
            }
            catch (CIPGeneralStatusCodeException)
            {
                return null;
            }
        }

        public async Task<List<ITransaction>> GetAllTransactions(ICipMessager messager)
        {
            CIPController cipController = new CIPController(0, messager);

            long logId = await cipController.GetCurrentLogId();

            List<ITransaction> transactions = new List<ITransaction>();
            long index = logId - 1;

            while (true)
            {
                try
                {
                    var logBytes = await cipController.ReadChangeLog(index);

                    transactions.Add(Transaction.CreateTransaction(index, logBytes));

                    index--;
                }
                catch (CIPGeneralStatusCodeException)
                {
                    break;
                }

            }

            transactions.Reverse();

            return transactions;
        }

        public async Task<List<ITransaction>> GetLatestTransactions(ICipMessager messager, int maxCount)
        {
            List<ITransaction> transactions = new List<ITransaction>();

            try
            {
                CIPController cipController = new CIPController(0, messager);

                long logId = await cipController.GetCurrentLogId();

                long index = logId - 1;

                int count = 0;

                while (true)
                {
                    try
                    {
                        var logBytes = await cipController.ReadChangeLog(index);

                        transactions.Add(Transaction.CreateTransaction(index, logBytes));

                        index--;

                        count++;

                        if (count >= maxCount)
                            break;
                    }
                    catch (CIPGeneralStatusCodeException)
                    {
                        break;
                    }

                }

                transactions.Reverse();
            }
            catch (Exception)
            {
                transactions.Clear();
            }

            return transactions;
        }

        private enum CorrelateResult
        {
            Success = 0,
            NoLogInPc = -1,
            NameNotMatch = -2,
        }

        public async Task<int> Correlate(ICipMessager messager)
        {
            if (!HasTransaction)
            {
                Logger.Info("TransactionManager has nothing!");
                return (int)CorrelateResult.NoLogInPc;
            }

            Logger.Info("Begin correlate...");
            Logger.Info($"CurrentTransactionIndex In PC: {CurrentTransactionIndex}");

            CIPController cipController = new CIPController(0, messager);

            var projectNameInPlc = await cipController.GetName();

            long currentLogIdInPlc = await cipController.GetCurrentLogId();
            Logger.Info($"CurrentLogId In PLC: {currentLogIdInPlc}");

            var currentLogIdPair = await TryGetCurrentLogIdPair(messager);
            if (currentLogIdPair != null)
            {
                Logger.Info($"CurrentLogIdPair In PLC: {currentLogIdPair.LogId},{currentLogIdPair.Hash}");
            }

            if (!string.Equals(projectNameInPlc, Controller.Name))
            {
                Logger.Info("Correlate Result: Name not match.");
                return (int)CorrelateResult.NameNotMatch;
            }

            if (Math.Abs(currentLogIdInPlc - CurrentTransactionIndex) > 1000)
            {
                Logger.Info("Correlate Result: > 1000.");
                return -2;
            }

            if (Utils.Utils.SequenceCompare(currentLogIdInPlc, CurrentTransactionIndex) == 0)
            {
                Logger.Info("Correlate: in equal.");

                var lastTransactionInPlc = await GetTransaction(messager, currentLogIdInPlc - 1);
                var lastTransactionInPc = _transactions.Last();

                if (lastTransactionInPlc == null)
                {
                    if (lastTransactionInPc is SetupTransaction)
                        return 0;

                    if (currentLogIdPair != null && (ulong)currentLogIdPair.Hash == lastTransactionInPc.Hash)
                        return 0;

                    return -3;
                }
                // ReSharper disable once RedundantIfElseBlock
                else
                {
                    if (lastTransactionInPc is SetupTransaction)
                        return -4;

                    if (lastTransactionInPlc.Hash != lastTransactionInPc.Hash)
                        return -5;

                    return 0;
                }

            }

            if (Utils.Utils.SequenceCompare(currentLogIdInPlc, CurrentTransactionIndex) > 0)
            {
                Logger.Info("Correlate: in plc > pc.");

                var lastTransactionInPc = _transactions.Last();

                var transactionInPlc = await GetTransaction(messager, CurrentTransactionIndex);
                var lastTransactionInPlc = await GetTransaction(messager, CurrentTransactionIndex - 1);

                if (lastTransactionInPc is SetupTransaction)
                {
                    if (!(transactionInPlc != null && lastTransactionInPlc == null))
                        return -6;

                }
                else
                {
                    if (lastTransactionInPlc == null)
                        return -7;

                    if (lastTransactionInPlc.Hash != lastTransactionInPc.Hash)
                        return -8;
                }

                long from = CurrentTransactionIndex;
                long to = currentLogIdInPlc - 1;

                var transactions = await GetTransactions(messager, from, to);
                if (transactions.Count == 0)
                    return -20;

                AddTransactionsAndCommit(transactions);

                return 0;

            }

            if (Utils.Utils.SequenceCompare(currentLogIdInPlc, CurrentTransactionIndex) < 0)
            {
                Logger.Info("Correlate: in plc < pc.");

                var lastTransactionInPlc = await GetTransaction(messager, currentLogIdInPlc - 1);

                var lastTransactionInPc = _transactions.Last();

                if (lastTransactionInPc is SetupTransaction)
                    return -9;

                var transactionInPc0 = GetTransaction(currentLogIdInPlc);
                var transactionInPc1 = GetTransaction(currentLogIdInPlc - 1);

                if (lastTransactionInPlc == null)
                {
                    if (transactionInPc0 == null)
                        return -10;

                    if (currentLogIdPair != null && transactionInPc1 != null)
                    {
                        if ((ulong)currentLogIdPair.Hash != transactionInPc1.Hash)
                            return -12;
                    }
                }
                else
                {
                    if (transactionInPc1 == null)
                        return -11;

                    if (lastTransactionInPlc.Hash != transactionInPc1.Hash)
                        return -12;
                }

                long from = currentLogIdInPlc;
                long to = CurrentTransactionIndex - 1;

                var transactions = GetTransactions(from, to);

                int result = await CommitTransactionsToPLC(messager, transactions);

                if (result < 0)
                    return result;

                // check index
                currentLogIdInPlc = await cipController.GetCurrentLogId();
                if (Utils.Utils.SequenceCompare(currentLogIdInPlc, CurrentTransactionIndex) != 0)
                    return -13;

                return 0;
            }

            return -1;

        }

        public async Task<CompareResult> Compare(ICipMessager messager)
        {
            // has transaction
            if (!HasTransaction)
            {
                Logger.Trace("Compare: TransactionManager has nothing!");
                return CompareResult.NoLogInPc;
            }

            // has project
            bool hasProject = await HasProject(messager);
            if (!hasProject)
                return CompareResult.NoProject;


            CIPController cipController = new CIPController(0, messager);

            // project name
            var projectNameInPlc = await cipController.GetName();

            if (!string.Equals(projectNameInPlc, Controller.Name))
            {
                Logger.Trace("Compare: Name not match.");
                return CompareResult.NameNotMatch;
            }

            // compare in log
            long currentLogIdInPlc = await cipController.GetCurrentLogId();
            Logger.Trace($"Compare: CurrentLogId In PLC is {currentLogIdInPlc}");

            var currentLogIdPair = await TryGetCurrentLogIdPair(messager);
            if (currentLogIdPair != null)
            {
                Logger.Trace($"Compare: CurrentLogIdPair In PLC is {currentLogIdPair.LogId},{currentLogIdPair.Hash}");
            }

            Logger.Trace($"Compare: CurrentTransactionIndex In PC is {CurrentTransactionIndex}");

            if (Math.Abs(currentLogIdInPlc - CurrentTransactionIndex) > 1000)
            {
                Logger.Trace("Compare: > 1000");
                return CompareResult.DiffTooLarge;
            }

            // 
            if (Utils.Utils.SequenceCompare(currentLogIdInPlc, CurrentTransactionIndex) == 0)
            {
                Logger.Trace("Compare: in equal.");

                var lastTransactionInPlc = await GetTransaction(messager, currentLogIdInPlc - 1);
                var lastTransactionInPc = _transactions.Last();

                if (lastTransactionInPlc == null)
                {
                    if (lastTransactionInPc is SetupTransaction)
                        return CompareResult.ExactlySame;

                    if (currentLogIdPair != null && (ulong)currentLogIdPair.Hash == lastTransactionInPc.Hash)
                        return CompareResult.ExactlySame;

                    return CompareResult.HashDiff;
                }
                // ReSharper disable once RedundantIfElseBlock
                else
                {
                    if (lastTransactionInPc is SetupTransaction)
                        return CompareResult.HashDiff;

                    if (lastTransactionInPlc.Hash != lastTransactionInPc.Hash)
                        return CompareResult.HashDiff;

                    Logger.Trace("Compare: Exactly Same.");

                    return CompareResult.ExactlySame;
                }

            }

            if (Utils.Utils.SequenceCompare(currentLogIdInPlc, CurrentTransactionIndex) > 0)
            {
                Logger.Trace("Compare: in plc > pc.");

                var lastTransactionInPc = _transactions.Last();

                var transactionInPlc = await GetTransaction(messager, CurrentTransactionIndex);
                var lastTransactionInPlc = await GetTransaction(messager, CurrentTransactionIndex - 1);

                if (lastTransactionInPc is SetupTransaction)
                {
                    if (!(transactionInPlc != null && lastTransactionInPlc == null))
                        return CompareResult.HashDiff;

                }
                else
                {
                    if (lastTransactionInPlc == null)
                        return CompareResult.HashDiff;

                    if (lastTransactionInPlc.Hash != lastTransactionInPc.Hash)
                        return CompareResult.HashDiff;
                }

                //long from = CurrentTransactionIndex;
                //long to = currentLogIdInPlc - 1;

                //var transactions = await GetTransactions(messager, from, to);
                //if (transactions.Count == 0)
                //    return -20;

                //AddTransactionsAndCommit(transactions);

                Logger.Trace("Compare: Controller is newer.");

                return CompareResult.ControllerIsNewer;

            }

            if (Utils.Utils.SequenceCompare(currentLogIdInPlc, CurrentTransactionIndex) < 0)
            {
                Logger.Trace("Compare: in plc < pc.");

                var lastTransactionInPlc = await GetTransaction(messager, currentLogIdInPlc - 1);

                var lastTransactionInPc = _transactions.Last();

                if (lastTransactionInPc is SetupTransaction)
                    return CompareResult.HashDiff;

                var transactionInPc0 = GetTransaction(currentLogIdInPlc);
                var transactionInPc1 = GetTransaction(currentLogIdInPlc - 1);

                if (lastTransactionInPlc == null)
                {
                    if (transactionInPc0 == null)
                        return CompareResult.HashDiff;

                    if (currentLogIdPair != null && transactionInPc1 != null)
                    {
                        if ((ulong)currentLogIdPair.Hash != transactionInPc1.Hash)
                            return CompareResult.HashDiff;
                    }
                }
                else
                {
                    if (transactionInPc1 == null)
                        return CompareResult.HashDiff;

                    if (lastTransactionInPlc.Hash != transactionInPc1.Hash)
                        return CompareResult.HashDiff;
                }

                //long from = currentLogIdInPlc;
                //long to = CurrentTransactionIndex - 1;

                //var transactions = GetTransactions(from, to);

                //int result = await CommitTransactionsToPLC(messager, transactions);

                //if (result < 0)
                //    return result;

                // check index
                //currentLogIdInPlc = await cipController.GetCurrentLogId();
                //if (Utils.Utils.SequenceCompare(currentLogIdInPlc, CurrentTransactionIndex) != 0)
                //    return -13;

                Logger.Trace("Compare: Offline project is newer.");

                return CompareResult.OfflineProjectIsNewer;
            }

            return CompareResult.Unknown;
        }

        private async Task<bool> HasProject(ICipMessager messager)
        {
            CIPController cipController = new CIPController(0, messager);

            var res = await cipController.GetHasProject();
            if (res < 1)
            {
                Logger.Error($"Compare failed: no project!");
                return false;
            }

            return true;
        }

        private async Task<int> CommitTransactionsToPLC(ICipMessager messager, List<ITransaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                int result = await transaction.CommitAsync(messager, Controller);
                if (result < 0)
                    return result;
            }

            return 0;
        }

        private void AddTransactionsAndCommit(List<ITransaction> transactions)
        {
            _replacedRoutines.Clear();

            //TODO(gjc): need edit later
            foreach (var transaction in transactions)
            {
                transaction.Commit(Controller);

                ReplaceTransaction replaceTransaction = transaction as ReplaceTransaction;
                if (replaceTransaction != null)
                {
                    if (replaceTransaction.Context == "Routine")
                    {
                        _replacedRoutines.Add(new Tuple<string, string>(replaceTransaction.Program,
                            replaceTransaction.Data["Name"]?.ToString()));
                    }
                }
            }

            AddTransactions(transactions);
        }

        private ITransaction GetTransaction(long index)
        {
            if (Utils.Utils.SequenceCompare(index, CurrentTransactionIndex) > 0)
                return null;

            var lastTransactionInPc = _transactions.Last();
            if (lastTransactionInPc is SetupTransaction)
            {
                return index == CurrentTransactionIndex ? lastTransactionInPc : null;
            }

            long offset = _transactions.Count - (CurrentTransactionIndex - index);
            if (offset < 0)
                return null;

            if (offset > _transactions.Count)
                return null;

            var transaction = _transactions[(int)offset];

            return transaction.SequenceNumber == index ? transaction : null;
        }

        private List<ITransaction> GetTransactions(long from, long to)
        {
            List<ITransaction> transactions = new List<ITransaction>();

            long start = from;
            long end = to;

            if (Utils.Utils.SequenceCompare(from, to) > 0)
            {
                start = to;
                end = from;
            }

            for (long index = start; index <= end; index++)
            {
                transactions.Add(GetTransaction(index));
            }

            return transactions;
        }

        private async Task<LogIdPair> TryGetCurrentLogIdPair(ICipMessager messager)
        {
            try
            {
                CIPController cipController = new CIPController(0, messager);

                var logIdPair = await cipController.GetCurrentLogIdPair();

                return logIdPair;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
