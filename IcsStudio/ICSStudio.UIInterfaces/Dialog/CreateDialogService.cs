using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.UIInterfaces.Dialog
{
    [Guid("0B6855B3-755A-4DBA-B206-591B57B14515")]
    [ComVisible(true)]
    public interface ICreateDialogService
    {
        void CloseAllDialogs();
        int ApplyAllDialogs();

        void CloseDialog(List<string> windowId);

        Window CreateNewTaskDialog();

        Window CreateNewTagDialog(
            IDataType dataType,
            ITagCollection parentCollection,
            Usage usage = Usage.NullParameterType,
            ITag assignedGroup = null, string name = "");

        Window CreateProgramDialog(ProgramType type, ITask task);
        Window CreateRoutineDialog(IProgramModule program, string name = default(string));
        Window CreatePhaseStateDialog(IProgramModule program);
        Window CreateSelectModuleTypeDialog(IController controller, IDeviceModule parentModule, PortType portType);


        Window CreateMotionGroupProperties(ITag motionGroup);
        Window CreateAxisCIPDriveProperties(ITag axisTag);
        Window CreateAxisScheduleDialog(ITag motionGroup);
        Window CreateMotionDirectCommandsDialog(ITag motion);
        Window CreateManualTuneDialog(ITag axisTag);
        Window CreateTaskProperties(ITask task);
        Window CreateProgramProperties(IProgram program, bool isShowParameterTab = false);
        Window CreateAxisVirtualProperties(ITag axisTag);
        Window CreateManualAdjustDialog(ITag tag, string positionUnits);
        Window CreateRoutineProperties(IRoutine routine);
        Window CreateAddOnInstruction();
        Window AddOnInstructionProperties(IAoiDefinition aoiDefinition);
        Window CreateTagProperties(ITag tag);
        Window CreateNewAoiTagDialog(IDataType dataType, IAoiDefinition aoiDefinition, Usage usage, bool canRefreshDataType, string name = "");
        Window CreateControllerProperties(IController controller);
        Window CreateControllerProperties(IController controller, int selectedIndex);
        Window CreateNewTrendDialog(IController controller);
        Window CreateRSTrendXProperties(ITrend trend, int kind);
        Window CreateGraphTitle(ITrend trend);
        List<Window> GetAllWindows();
    }

    [Guid("807600B4-2D04-4C77-8761-37C3D40413D9")]
    // ReSharper disable once InconsistentNaming
    public interface SCreateDialogService
    {

    }
}
