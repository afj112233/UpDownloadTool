using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;

namespace ICSStudio.UIInterfaces.Editor
{
    [ComVisible(true)]
    [Guid("D5F9EBA0-5F95-46E8-81D3-082B3A573062")]
    public interface ICreateEditorService
    {
        void CloseAllToolWindows(bool closeProject);

        void CloseMonitorEditTags(IController controller,
            ITagCollectionContainer tagCollectionContainer, string focusName = "", bool isEditMonitorShow = false);

        int ApplyAllToolWindows();

        void CloseWindow(int id);
        object GetWindow(int id);

        void CreateRLLEditor(IRoutine routine, int? rungIndex = null, int? offset = null);

        void CreateSTEditor(IRoutine routine, OnlineEditType onlineEditType = OnlineEditType.Original, int? line = null,
            int? offset = null, int? len = null,
            AoiDataReference aoiDataReference = null);

        void CreateSFCEditor(IRoutine routine);
        void CreateFBDEditor(IRoutine routine);

        void CreateModuleProperties(IDeviceModule deviceModule);
        List<IDeviceModule> GetDeviceModulesInOpen();

        void CreateMonitorEditTags(IController controller,
            ITagCollectionContainer tagCollectionContainer, string focusName = "", bool isEditMonitorShow = false);

        void CreateAoiMonitorEditTags(IController controller,
            ITagCollectionContainer tagCollectionContainer, AoiDataReference aoiDataReference, string focusName = "",
            bool isEditMonitorShow = false);

        void CreateNewDataType(IDataType dataType);

        void CloseSTEditor(IRoutine routine);

        void CreateCrossReference(Type filterType, IProgramModule program, string name);

        void CreateCrossReference(IRoutine routine,  string name);

        void CreateTrend(ITrend trend, bool isRunImmediately = false);

        object GetActiveEditorWindow();

        //TODO(gjc): can move to other service package
        void ParseRoutine(IRoutine routine, bool needAddAoiDataReference, bool canAddError = false,
            bool isForce = false, bool isOnlyParseEdit = false);

        List<ITag> ParseRoutineTag(IRoutine routine);

        bool CheckRoutineInRun(IRoutine routine);
        bool IsThereATrendRunning();

        //获取所有打开的Editor
        List<UIElement> GetAllEditors();

        void SetStOnlineEditMode(IRoutine routine, OnlineEditType type);

        void ParseErrorRoutine();
        void UpdateAllRoutineOnlineStatus();
    }

    [Guid("101B2CCF-8BF1-474D-AA0D-0955BDF98B6D")]
    // ReSharper disable once InconsistentNaming
    public interface SCreateEditorService
    {
    }



    public enum Type
    {
        Tag,
        [EnumMember(Value = "Data Type")] DataType,
        Routine,
        Program,

        [EnumMember(Value = "Equipment Phase")]
        EquipmentPhase,

        [EnumMember(Value = "Equipment Sequence")]
        EquipmentSequence,

        [EnumMember(Value = "Add-On Instruction")]
        AOI,
        Task,
        Module,
        Label,
        None
    }
}