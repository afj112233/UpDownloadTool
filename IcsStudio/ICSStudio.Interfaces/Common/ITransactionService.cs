namespace ICSStudio.Interfaces.Common
{
    public interface ITransactionService
    {
        void Begin();

        void Commit();

        void Abort(int abortStatus);

        bool IsThisTransactionInProgress { get; }

        bool IsAnyTransactionInProgress { get; }
    }
}
