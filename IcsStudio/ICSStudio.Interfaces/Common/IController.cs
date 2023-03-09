using System;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;

namespace ICSStudio.Interfaces.Common
{
    public interface IController : IBaseComponent, ITagCollectionContainer
    {
        string MajorFaultProgram { get; }

        string PowerLossProgram { get; }

        IAoiDefinitionCollection AOIDefinitionCollection { get; }

        IProgramCollection Programs { get; }

        ITaskCollection Tasks { get; }

        IDataTypeCollection DataTypes { get; }

        IDeviceModuleCollection DeviceModules { get; }
        
        ITrendCollection Trends { get; }
        
        IParameterConnectionCollection ParameterConnections { get; }

        //IAlarmConditionCollection AlarmConditions { get; }

        //IAlarmConditionDefinitionCollection AlarmConditionDefinitions { get; }

        //IBaseEnergyCollection BaseEnergyObjects { get; }

        //INonElectricalEnergyCollection NonElectricalEnergyObjects { get; }

        bool IsOnline { get; }

        string ProjectCommunicationPath { get; set; }

        ControllerOperationMode OperationMode { get; set; }

        ControllerKeySwitch KeySwitchPosition { get; }

        ControllerLockState LockState { get; }

        //IRedundancy RedundancyObject { get; }

        //bool IsRedundancyEnabled { get; }

        //bool SupportsRedundancy { get; }

        bool SupportsEquipmentSequenceEventGeneration { get; }

        bool IsLanguageSwitchingEnabled { get; }

        bool IsForceEnabled { get; }

        string ProjectLocaleName { get; }

        string DefaultLocaleName { get; }

        bool ProjectHasCustomProperties { get; }

        IValueConverter ValueConverter { get; }

        //Dispatcher UiDispatcher { get; set; }

        //void Attach(IntPtr controller);

        //bool IsAttached();

        IDataServer CreateDataServer();

        IDataServer CreatePendingDataServer(bool monitorValue, bool monitorAttribute);

        IBaseComponent GetComponent(int uid);

        void GoOffline();

        void GoOnline();

        void CheckAccess(AccessType access);

        void ReportOfflineChange();

        //ILanguage GetLanguage(IProgramModule program, RoutineType routineType);

        IProgramModule GetProgram(int programType, int programUid);

        IOperandParser CreateOperandParser();

        ITransactionService CreateTransactionService();

        bool ExistsComponent(ComponentType componentType, string name);

        bool ExistsComponent(IBaseComponent parentComponent, ComponentType childComponentType, string name);

        void InitializeCurrentThread();

        void ShutDownCurrentThread();

        bool IsAlarmSpecifier(string operandText);

        bool IsAlarmSetSpecifier(string operandText);

        bool HasAlarmSetDefined(string alarmSetTargetQualifiedPath, int containerComponentUid);

        bool IsFeatureSupported(ControllerFeature feature);

        //bool IsTemporaryFeatureSupported(ControllerTemporaryFeature feature);

        //event EventHandler<SecuritySettingsChangedEventArgs> SecuritySettingsChanged;

        int TimeSlice { get; }

        int ProjectSN { get; }

        DateTime ProjectCreationDate { get; }

        DateTime LastModifiedDate { get; }

        bool MatchProjectToController { get; }

        string EtherNetIPMode { get; }

        bool IsLoading { get; }
        bool IsPass { get; }
        bool CanVerify { get; }
    }
}
