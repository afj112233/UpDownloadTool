using ICSStudio.Interfaces.Tags;

namespace ICSStudio.Interfaces.Common
{
    public interface IDataServer
    {
        IController ParentController { get; }
        bool IsMonitoring { get; }

        void StartMonitoring(bool monitorValue,bool monitorAttribute);
        void StopMonitoring(bool stopMonitorValue,bool stopMonitorAttribute);

        IDataOperand CreateDataOperand();
        IDataOperand CreateDataOperand(ITag tag, string subMember);
        IDataOperand CreateDataOperand(ITag tag, string subMember, bool pending);
        IDataOperand CreateDataOperand(
            ITag tag,
            string subMember,
            bool pending,
            bool allowPrivateMemberReferences);

        IDataOperand CreateDataOperand(IBaseComponent parent, string operand);

        IDataOperand CreateDataOperand(
            IBaseComponent parent,
            string operand,
            bool pending);

        IDataOperand CreateDataOperand(
            IBaseComponent parent,
            string operand,
            bool pending,
            bool allowPrivateMemberReferences);

        IDataOperand CreateDataOperand(ITagCollection tagCollection, string operand);

        IDataOperand CreateDataOperand(
            ITagCollection tagCollection,
            string operand,
            bool pending);

        IDataOperand CreateDataOperand(
            ITagCollection tagCollection,
            string operand,
            bool pending,
            bool allowPrivateMemberReferences);

        IDataOperand CreateDataOperand(
            ITagCollection tagCollection,
            string operand,
            bool pending,
            bool allowPrivateMemberReferences,
            ITagDataContext dataContext);
    }
}
