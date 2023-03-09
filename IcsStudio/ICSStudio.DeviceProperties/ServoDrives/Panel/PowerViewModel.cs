using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Database.Database;
using ICSStudio.Database.Table.Motion;
using ICSStudio.DeviceProperties.AdvancedUserLimits;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.Gui.Utils;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.ServoDrives.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class PowerViewModel : DeviceOptionPanel
    {
        //private VoltageType _voltage;
        //private ACInputPhasingType _acInputPhasing;
        //private BusConfigurationType _busConfiguration;
        //private BusSharingGroupType _busSharingGroup;
        //private BusRegulatorActionType _busRegulatorAction;
        //private ShuntRegulatorResistorType _shuntRegulatorResistorType;
        //private short _externalShuntRegulatorID;

        //private float _converterThermalOverloadUserLimit;
        //private float _busRegulatorThermalOverloadUserLimit;
        //private float _busUndervoltageUserLimit;

        private IList _busSharingGroupSource;

        private IList _externalShuntRegulatorIDSource;

        public PowerViewModel(UserControl panel, ModifiedMotionDrive motionDrive) : base(panel)
        {
            ModifiedMotionDrive = motionDrive;

            VoltageSource = EnumHelper.ToDataSource<VoltageType>();
            ACInputPhasingSource = EnumHelper.ToDataSource<ACInputPhasingType>();
            BusConfigurationSource = EnumHelper.ToDataSource<BusConfigurationType>();
            UpdateBusSharingGroupSource();
            BusRegulatorActionSource = EnumHelper.ToDataSource<BusRegulatorActionType>();
            UpdateExternalShuntRegulatorIDSource();

            AdvancedCommand = new RelayCommand(ExecuteAdvancedCommand);
        }

        public override void CheckDirty()
        {
            if (ConverterACInputVoltageVisibility == Visibility.Visible)
            {
                if (ModifiedMotionDrive.ConverterACInputVoltage !=
                    OriginalMotionDrive.ConfigData.ConverterACInputVoltage)
                {
                    IsDirty = true;
                    return;
                }
            }

            if (ConverterACInputPhasingVisibility == Visibility.Visible)
            {
                if (ModifiedMotionDrive.ConverterACInputPhasing !=
                    OriginalMotionDrive.ConfigData.ConverterACInputPhasing)
                {
                    IsDirty = true;
                    return;
                }
            }

            if (ModifiedMotionDrive.BusConfiguration !=
                OriginalMotionDrive.ConfigData.BusConfiguration)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedMotionDrive.BusSharingGroup !=
                OriginalMotionDrive.ConfigData.BusSharingGroup)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedMotionDrive.BusRegulatorAction !=
                OriginalMotionDrive.ConfigData.BusRegulatorAction[0])
            {
                IsDirty = true;
                return;
            }

            if (ModifiedMotionDrive.ShuntRegulatorResistorType !=
                OriginalMotionDrive.ConfigData.ShuntRegulatorResistorType)
            {
                IsDirty = true;
                return;
            }

            if (ModifiedMotionDrive.ExternalShuntRegulatorID !=
                OriginalMotionDrive.ConfigData.ExternalShuntRegulatorID)
            {
                IsDirty = true;
                return;
            }

            if (Math.Abs(ModifiedMotionDrive.ConverterThermalOverloadUserLimit -
                         OriginalMotionDrive.ConfigData.ConverterThermalOverloadUserLimit) > float.Epsilon)
            {
                IsDirty = true;
                return;
            }

            if (Math.Abs(ModifiedMotionDrive.BusRegulatorThermalOverloadUserLimit -
                         OriginalMotionDrive.ConfigData.BusRegulatorThermalOverloadUserLimit) > float.Epsilon)
            {
                IsDirty = true;
                return;
            }

            if (Math.Abs(ModifiedMotionDrive.BusUndervoltageUserLimit -
                         OriginalMotionDrive.ConfigData.BusUndervoltageUserLimit) > float.Epsilon)
            {
                IsDirty = true;
                return;
            }

            IsDirty = false;
        }

        public bool Enable
        {
            get
            {
                if (ModifiedMotionDrive.Controller.IsOnline)
                    return false;

                return true;
            }
        }

        public override bool SaveOptions()
        {
            if (ConverterACInputVoltageVisibility == Visibility.Visible)
                OriginalMotionDrive.ConfigData.ConverterACInputVoltage = ModifiedMotionDrive.ConverterACInputVoltage;
            if (ConverterACInputPhasingVisibility == Visibility)
                OriginalMotionDrive.ConfigData.ConverterACInputPhasing = ModifiedMotionDrive.ConverterACInputPhasing;

            OriginalMotionDrive.ConfigData.BusConfiguration = ModifiedMotionDrive.BusConfiguration;
            OriginalMotionDrive.ConfigData.BusSharingGroup = ModifiedMotionDrive.BusSharingGroup;
            OriginalMotionDrive.ConfigData.BusRegulatorAction[0] = ModifiedMotionDrive.BusRegulatorAction;
            OriginalMotionDrive.ConfigData.ShuntRegulatorResistorType = ModifiedMotionDrive.ShuntRegulatorResistorType;
            OriginalMotionDrive.ConfigData.ExternalShuntRegulatorID = ModifiedMotionDrive.ExternalShuntRegulatorID;

            OriginalMotionDrive.ConfigData.ConverterThermalOverloadUserLimit =
                ModifiedMotionDrive.ConverterThermalOverloadUserLimit;
            OriginalMotionDrive.ConfigData.BusRegulatorThermalOverloadUserLimit =
                ModifiedMotionDrive.BusRegulatorThermalOverloadUserLimit;
            OriginalMotionDrive.ConfigData.BusUndervoltageUserLimit =
                ModifiedMotionDrive.BusUndervoltageUserLimit;

            return true;
        }

        public override int CheckValid()
        {
            int result = 0;
            string warningReason = string.Empty;

            if (Voltage == VoltageType.Voltage460VAC && ACInputPhasing == ACInputPhasingType.SinglePhase)
            {
                warningReason = "Single phase is not valid for the current voltage setting.";

                result = -1;
            }

            if (result == 0)
            {
                if (ACInputPhasing == ACInputPhasingType.SinglePhase &&
                    BusConfiguration != BusConfigurationType.Standalone)
                {
                    warningReason =
                        "AC Converter Phasing must be Three Phase when Bus Configuration is not Standalone.";

                    result = -1;
                }
            }

            if (result < 0)
            {
                MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier(warningReason), "ICS Studio",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            return result;
        }

        public ModifiedMotionDrive ModifiedMotionDrive { get; }
        public CIPMotionDrive OriginalMotionDrive => ModifiedMotionDrive.OriginalMotionDrive;

        public string PowerStructure
        {
            get
            {
                if (ModifiedMotionDrive?.Profiles != null)
                {
                    var powerStructure = ModifiedMotionDrive.Profiles.Schema.PowerStructures[0];
                    return powerStructure.GetCatalogNumber();
                }

                return string.Empty;
            }
        }

        public string Description
        {
            get
            {
                if (ModifiedMotionDrive?.Profiles != null)
                {
                    var powerStructure = ModifiedMotionDrive.Profiles.Schema.PowerStructures[0];
                    return powerStructure.GetDescription();
                }

                return string.Empty;
            }
        }

        public Visibility ConverterACInputVoltageVisibility
        {
            get
            {
                if (ModifiedMotionDrive.Profiles.SupportDriveAttribute(
                        "ConverterACInputVoltage",
                        ModifiedMotionDrive.Major))
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }


        public VoltageType Voltage
        {
            get { return (VoltageType)ModifiedMotionDrive.ConverterACInputVoltage; }
            set
            {
                if ((VoltageType)ModifiedMotionDrive.ConverterACInputVoltage != value)
                {
                    ModifiedMotionDrive.ConverterACInputVoltage = (ushort)value;
                    RaisePropertyChanged();
                    CheckDirty();

                    UpdateExternalShuntRegulatorIDSource();
                }
            }
        }

        public IList VoltageSource { get; }

        public Visibility ConverterACInputPhasingVisibility
        {
            get
            {
                if (ModifiedMotionDrive.Profiles.SupportDriveAttribute(
                        "ConverterACInputPhasing",
                        ModifiedMotionDrive.Major))
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public ACInputPhasingType ACInputPhasing
        {
            get { return (ACInputPhasingType)ModifiedMotionDrive.ConverterACInputPhasing; }
            set
            {
                if ((ACInputPhasingType)ModifiedMotionDrive.ConverterACInputPhasing != value)
                {
                    ModifiedMotionDrive.ConverterACInputPhasing = (byte)value;
                    RaisePropertyChanged();
                    CheckDirty();

                    UpdateExternalShuntRegulatorIDSource();
                }
            }
        }

        public IList ACInputPhasingSource { get; }

        public BusConfigurationType BusConfiguration
        {
            get { return (BusConfigurationType)ModifiedMotionDrive.BusConfiguration; }
            set
            {
                if ((BusConfigurationType)ModifiedMotionDrive.BusConfiguration != value)
                {
                    ModifiedMotionDrive.BusConfiguration = (byte)value;

                    UpdateBusSharingGroupSource();

                    RaisePropertyChanged();
                    RaisePropertyChanged("BusSharingGroupEnabled");

                    CheckDirty();

                }
            }
        }

        public IList BusConfigurationSource { get; }


        public BusSharingGroupType BusSharingGroup
        {
            get { return (BusSharingGroupType)ModifiedMotionDrive.BusSharingGroup; }
            set
            {
                if ((BusSharingGroupType)ModifiedMotionDrive.BusSharingGroup != value)
                {
                    ModifiedMotionDrive.BusSharingGroup = (byte)value;
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
        }

        public IList BusSharingGroupSource
        {
            get { return _busSharingGroupSource; }
            set { Set(ref _busSharingGroupSource, value); }
        }

        public bool BusSharingGroupEnabled
        {
            get
            {
                if (BusConfiguration == BusConfigurationType.Standalone)
                    return false;

                return true;
            }
        }

        public BusRegulatorActionType BusRegulatorAction
        {
            get { return (BusRegulatorActionType)ModifiedMotionDrive.BusRegulatorAction; }
            set
            {
                if ((BusRegulatorActionType)ModifiedMotionDrive.BusRegulatorAction != value)
                {
                    ModifiedMotionDrive.BusRegulatorAction = (byte)value;
                    RaisePropertyChanged();
                }

                if (value == BusRegulatorActionType.Disabled)
                {
                    ShuntRegulatorResistorType = ShuntRegulatorResistorType.Internal;
                }

                RaisePropertyChanged("ShuntRegulatorResistorTypeEnabled");
                CheckDirty();
            }
        }

        public IList BusRegulatorActionSource { get; }

        public ShuntRegulatorResistorType ShuntRegulatorResistorType
        {
            get { return (ShuntRegulatorResistorType)ModifiedMotionDrive.ShuntRegulatorResistorType; }
            set
            {
                if ((ShuntRegulatorResistorType)ModifiedMotionDrive.ShuntRegulatorResistorType != value)
                {
                    ModifiedMotionDrive.ShuntRegulatorResistorType = (byte)value;
                    RaisePropertyChanged();
                }

                if (value == ShuntRegulatorResistorType.Internal)
                {
                    ExternalShuntRegulatorID = -1;
                }

                RaisePropertyChanged("ExternalShuntRegulatorIDEnabled");

                CheckDirty();
            }
        }

        public bool ShuntRegulatorResistorTypeEnabled
        {
            get
            {
                if (BusRegulatorAction == BusRegulatorActionType.Disabled)
                    return false;

                return true;
            }
        }

        public short ExternalShuntRegulatorID
        {
            get { return ModifiedMotionDrive.ExternalShuntRegulatorID; }
            set
            {
                if (ModifiedMotionDrive.ExternalShuntRegulatorID != value)
                {
                    ModifiedMotionDrive.ExternalShuntRegulatorID = value;
                    RaisePropertyChanged();
                    CheckDirty();
                }
            }
        }

        public IList ExternalShuntRegulatorIDSource
        {
            get { return _externalShuntRegulatorIDSource; }
            set { Set(ref _externalShuntRegulatorIDSource, value); }
        }

        public bool ExternalShuntRegulatorIDEnabled
        {
            get
            {
                if (ShuntRegulatorResistorType == ShuntRegulatorResistorType.Internal)
                    return false;

                return true;
            }
        }

        public RelayCommand AdvancedCommand { get; }

        private void UpdateBusSharingGroupSource()
        {
            List<BusSharingGroupType> supportList;
            if (BusConfiguration == BusConfigurationType.Standalone)
            {
                supportList = new List<BusSharingGroupType>
                {
                    BusSharingGroupType.Standalone
                };

            }
            else
            {
                supportList = new List<BusSharingGroupType>
                {
                    BusSharingGroupType.Group1,
                    BusSharingGroupType.Group2,
                    BusSharingGroupType.Group3,
                    BusSharingGroupType.Group4,
                    BusSharingGroupType.Group5,
                    BusSharingGroupType.Group6,
                    BusSharingGroupType.Group7,
                    BusSharingGroupType.Group8,
                    BusSharingGroupType.Group9,
                    BusSharingGroupType.Group10,
                    BusSharingGroupType.Group11,
                    BusSharingGroupType.Group12,
                    BusSharingGroupType.Group13,
                    BusSharingGroupType.Group14,
                    BusSharingGroupType.Group15,
                    BusSharingGroupType.Group16,
                    BusSharingGroupType.Group17,
                    BusSharingGroupType.Group18,
                    BusSharingGroupType.Group19,
                    BusSharingGroupType.Group20,
                    BusSharingGroupType.Group21,
                    BusSharingGroupType.Group22,
                    BusSharingGroupType.Group23,
                    BusSharingGroupType.Group24,
                    BusSharingGroupType.Group25
                };
            }

            var oldBusSharingGroup = BusSharingGroup;

            BusSharingGroupSource = EnumHelper.ToDataSource<BusSharingGroupType>(supportList);

            if (!supportList.Contains(oldBusSharingGroup))
                BusSharingGroup = supportList[0];

            RaisePropertyChanged("BusSharingGroup");
        }

        private void UpdateExternalShuntRegulatorIDSource()
        {
            MotionDbHelper motionDbHelper = new MotionDbHelper();
            var supportList = new List<DisplayItem<short>>();

            Drive drive = null;

            if (ConverterACInputPhasingVisibility == Visibility.Visible
                && ConverterACInputVoltageVisibility == Visibility)
            {
                drive = motionDbHelper.GetMotionDrive(
                    ModifiedMotionDrive.CatalogNumber,
                    ModifiedMotionDrive.ConverterACInputVoltage,
                    ModifiedMotionDrive.ConverterACInputPhasing);
            }
            else if (ConverterACInputPhasingVisibility == Visibility.Visible)
            {
                //TODO(gjc): add code here
            }
            else if (ConverterACInputVoltageVisibility == Visibility.Visible)
            {
                //TODO(gjc): add code here
            }
            else
            {
                var drives = motionDbHelper.GetMotionDrive(ModifiedMotionDrive.CatalogNumber);
                if (drives != null && drives.Count == 1)
                {
                    drive = drives[0];
                }
            }

            if (drive != null)
            {
                var externalShuntList = motionDbHelper.GetSupportExternalShunt(drive.ID);
                if (externalShuntList != null)
                    foreach (var shuntView in externalShuntList)
                    {
                        var item = new DisplayItem<short>
                        {
                            DisplayName = shuntView.CatalogNo,
                            Value = (short)shuntView.ShuntID
                        };

                        supportList.Add(item);
                    }
            }

            supportList.Insert(0, new DisplayItem<short> { DisplayName = "<none>", Value = -1 });

            var oldExternalShuntRegulatorID = ExternalShuntRegulatorID;

            ExternalShuntRegulatorIDSource = supportList;

            int i;
            for (i = 0; i < supportList.Count; i++)
            {
                if (supportList[i].Value == oldExternalShuntRegulatorID)
                    break;
            }

            if (i >= supportList.Count)
                ExternalShuntRegulatorID = -1;

            RaisePropertyChanged("ExternalShuntRegulatorID");

        }

        private void ExecuteAdvancedCommand()
        {
            var dialog = new AdvancedUserLimitsDialog(new AdvancedUserLimits.AdvancedUserLimits()
            {
                BusRegulatorThermalOverloadUserLimit = ModifiedMotionDrive.BusRegulatorThermalOverloadUserLimit,
                BusUndervoltageUserLimit = ModifiedMotionDrive.BusUndervoltageUserLimit,
                ConverterThermalOverloadUserLimit = ModifiedMotionDrive.ConverterThermalOverloadUserLimit,

            })
            {
                Owner = Application.Current.MainWindow
            };

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var limits = dialog.AdvancedUserLimits;
                ModifiedMotionDrive.BusRegulatorThermalOverloadUserLimit = limits.BusRegulatorThermalOverloadUserLimit;
                ModifiedMotionDrive.BusUndervoltageUserLimit = limits.BusUndervoltageUserLimit;
                ModifiedMotionDrive.ConverterThermalOverloadUserLimit = limits.ConverterThermalOverloadUserLimit;

                CheckDirty();
            }
        }

        public override void Show()
        {
            RaisePropertyChanged("Enable");
        }
    }
}
