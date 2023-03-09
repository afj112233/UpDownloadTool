using System.ComponentModel;

namespace ICSStudio.Interfaces.Common
{
    public interface IBaseObject : IBaseCommon, INotifyPropertyChanged
    {
        bool IsVerified { get; }

        bool IsDeleted { get; }

        int ParentProgramUid { get; }

        int ParentRoutineUid { get; }

        void BeginTransactionSet();

        void EndTransactionSet();

        void CancelTransactionSet();
    }
}
