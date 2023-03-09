using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSStudio.Cip.Objects;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.DeviceModule;

namespace ICSStudio.DeviceProperties.ServoDrives.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class DigitalInputViewModel : DeviceOptionPanel
    {
        private int _axisIndex;

        public DigitalInputViewModel(UserControl panel, ModifiedMotionDrive modifiedMotionDrive) : base(panel)
        {
            ModifiedMotionDrive = modifiedMotionDrive;

            AxisIndex = 1;

            UpdateAxisIndexSource();

            UpdateDigitalInputSource();
        }



        public ModifiedMotionDrive ModifiedMotionDrive { get; }
        public CIPMotionDrive OriginalMotionDrive => ModifiedMotionDrive.OriginalMotionDrive;

        public bool Enable
        {
            get
            {
                if (ModifiedMotionDrive.Controller.IsOnline)
                    return false;

                return true;
            }
        }

        public override Visibility Visibility
        {
            get
            {
                if (ModifiedMotionDrive.Profiles.SupportDriveAttribute(
                    "DigitalInputConfiguration",
                    ModifiedMotionDrive.Major))
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }
        }

        public override void Show()
        {
            RaisePropertyChanged("AxisName");
            RaisePropertyChanged("Enable");
        }

        public override void CheckDirty()
        {
            if (!ModifiedMotionDrive.DigitalInputConfiguration.SequenceEqual(OriginalMotionDrive.ConfigData
                .DigitalInputConfiguration))
            {
                IsDirty = true;
                return;
            }

            IsDirty = false;
        }

        public override int CheckValid()
        {
            List<byte> byteList = new List<byte>();

            for (int i = 0; i < OriginalMotionDrive.ConfigData.NumberOfConfigurableInputs.Count; i++)
            {
                var numberOfConfigurableInput =
                    OriginalMotionDrive.ConfigData.NumberOfConfigurableInputs[i];
                if (numberOfConfigurableInput > 0)
                {
                    byteList.Clear();

                    for (var n = 0; n < numberOfConfigurableInput; n++)
                    {
                        var digitalInput = ModifiedMotionDrive.DigitalInputConfiguration[i * 8 + n];

                        if (digitalInput != 0)
                        {
                            if (byteList.Contains(digitalInput))
                            {
                                AxisIndex = i + 1;

                                string message = $"Axis {i + 1}, digital input {n + 1} is invalid.";

                                MessageBox.Show(message, "ICS Studio", MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                                return -1;
                            }

                            byteList.Add(digitalInput);
                        }
                    }
                }
            }

            return 0;
        }

        public override bool SaveOptions()
        {
            int length = OriginalMotionDrive.ConfigData.DigitalInputConfiguration.Count;
            for (int i = 0; i < length; i++)
            {
                OriginalMotionDrive.ConfigData.DigitalInputConfiguration[i] =
                    ModifiedMotionDrive.DigitalInputConfiguration[i];
            }

            return true;
        }

        public int AxisIndex
        {
            get { return _axisIndex; }
            set
            {
                Set(ref _axisIndex, value);

                RaisePropertyChanged("AxisName");
                RaisePropertyChanged("DigitalInput1Visibility");
                RaisePropertyChanged("DigitalInput2Visibility");
                RaisePropertyChanged("DigitalInput3Visibility");
                RaisePropertyChanged("DigitalInput4Visibility");
                RaisePropertyChanged("DigitalInput5Visibility");
                RaisePropertyChanged("DigitalInput6Visibility");
                RaisePropertyChanged("DigitalInput7Visibility");
                RaisePropertyChanged("DigitalInput8Visibility");

                RaisePropertyChanged("DigitalInput1");
                RaisePropertyChanged("DigitalInput2");
                RaisePropertyChanged("DigitalInput3");
                RaisePropertyChanged("DigitalInput4");
                RaisePropertyChanged("DigitalInput5");
                RaisePropertyChanged("DigitalInput6");
                RaisePropertyChanged("DigitalInput7");
                RaisePropertyChanged("DigitalInput8");
            }
        }

        public List<int> AxisIndexSource { get; set; }

        public string AxisName
        {
            get
            {
                if (AxisIndex > 0)
                {
                    if (ModifiedMotionDrive.AssociatedAxes[AxisIndex - 1] != null)
                        return ModifiedMotionDrive.AssociatedAxes[AxisIndex - 1].Name;
                }

                return "<none>";
            }
        }

        public IList DigitalInput1Source { get; private set; }
        public IList DigitalInput2Source { get; private set; }
        public IList DigitalInput3Source { get; private set; }
        public IList DigitalInput4Source { get; private set; }
        public IList DigitalInput5Source { get; private set; }
        public IList DigitalInput6Source { get; private set; }
        public IList DigitalInput7Source { get; private set; }
        public IList DigitalInput8Source { get; private set; }

        public Visibility DigitalInput1Visibility => DigitalInputVisibility(1);
        public Visibility DigitalInput2Visibility => DigitalInputVisibility(2);
        public Visibility DigitalInput3Visibility => DigitalInputVisibility(3);
        public Visibility DigitalInput4Visibility => DigitalInputVisibility(4);
        public Visibility DigitalInput5Visibility => DigitalInputVisibility(5);
        public Visibility DigitalInput6Visibility => DigitalInputVisibility(6);
        public Visibility DigitalInput7Visibility => DigitalInputVisibility(7);
        public Visibility DigitalInput8Visibility => DigitalInputVisibility(8);

        public DigitalInputType DigitalInput1
        {
            get
            {
                if (AxisIndex > 0)
                {
                    return (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 0];
                }

                return DigitalInputType.Unassigned;
            }
            set
            {
                if (AxisIndex > 0)
                {
                    ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 0] = (byte) value;

                    RaisePropertyChanged();

                    CheckDirty();
                }
            }
        }

        public DigitalInputType DigitalInput2
        {
            get
            {
                if (AxisIndex > 0)
                {
                    return (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 1];
                }

                return DigitalInputType.Unassigned;
            }
            set
            {
                if (AxisIndex > 0)
                {
                    ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 1] = (byte) value;

                    RaisePropertyChanged();

                    CheckDirty();
                }
            }
        }

        public DigitalInputType DigitalInput3
        {
            get
            {
                if (AxisIndex > 0)
                {
                    return (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 2];
                }

                return DigitalInputType.Unassigned;
            }
            set
            {
                if (AxisIndex > 0)
                {
                    ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 2] = (byte) value;

                    RaisePropertyChanged();

                    CheckDirty();
                }
            }
        }

        public DigitalInputType DigitalInput4
        {
            get
            {
                if (AxisIndex > 0)
                {
                    return (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 3];
                }

                return DigitalInputType.Unassigned;
            }
            set
            {
                if (AxisIndex > 0)
                {
                    ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 3] = (byte) value;

                    RaisePropertyChanged();

                    CheckDirty();
                }
            }
        }

        public DigitalInputType DigitalInput5
        {
            get
            {
                if (AxisIndex > 0)
                {
                    return (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 4];
                }

                return DigitalInputType.Unassigned;
            }
            set
            {
                if (AxisIndex > 0)
                {
                    ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 4] = (byte) value;

                    RaisePropertyChanged();

                    CheckDirty();
                }
            }
        }

        public DigitalInputType DigitalInput6
        {
            get
            {
                if (AxisIndex > 0)
                {
                    return (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 5];
                }

                return DigitalInputType.Unassigned;
            }
            set
            {
                if (AxisIndex > 0)
                {
                    ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 5] = (byte) value;

                    RaisePropertyChanged();

                    CheckDirty();
                }
            }
        }

        public DigitalInputType DigitalInput7
        {
            get
            {
                if (AxisIndex > 0)
                {
                    return (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 6];
                }

                return DigitalInputType.Unassigned;
            }
            set
            {
                if (AxisIndex > 0)
                {
                    ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 6] = (byte) value;

                    RaisePropertyChanged();

                    CheckDirty();
                }
            }
        }

        public DigitalInputType DigitalInput8
        {
            get
            {
                if (AxisIndex > 0)
                {
                    return (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 7];
                }

                return DigitalInputType.Unassigned;
            }
            set
            {
                if (AxisIndex > 0)
                {
                    ModifiedMotionDrive.DigitalInputConfiguration[(AxisIndex - 1) * 8 + 7] = (byte) value;

                    RaisePropertyChanged();

                    CheckDirty();
                }
            }
        }

        private void UpdateAxisIndexSource()
        {
            AxisIndexSource = new List<int>();

            if (OriginalMotionDrive.ConfigData.NumberOfConfigurableInputs != null
                && OriginalMotionDrive.ConfigData.NumberOfConfigurableInputs.Count > 0)
                for (var i = 0; i < OriginalMotionDrive.ConfigData.NumberOfConfigurableInputs.Count; i++)
                    if (OriginalMotionDrive.ConfigData.NumberOfConfigurableInputs[i] > 0)
                        AxisIndexSource.Add(i + 1);
        }

        private void UpdateDigitalInputSource()
        {
            var digitalInput1 = DigitalInputType.Unassigned;
            var digitalInput2 = DigitalInputType.Unassigned;
            var digitalInput3 = DigitalInputType.Unassigned;
            var digitalInput4 = DigitalInputType.Unassigned;
            var digitalInput5 = DigitalInputType.Unassigned;
            var digitalInput6 = DigitalInputType.Unassigned;
            var digitalInput7 = DigitalInputType.Unassigned;
            var digitalInput8 = DigitalInputType.Unassigned;

            var supportList1 = GetSupportedList(AxisIndex, 1);
            var supportList2 = GetSupportedList(AxisIndex, 2);
            var supportList3 = GetSupportedList(AxisIndex, 3);
            var supportList4 = GetSupportedList(AxisIndex, 4);
            var supportList5 = GetSupportedList(AxisIndex, 5);
            var supportList6 = GetSupportedList(AxisIndex, 6);
            var supportList7 = GetSupportedList(AxisIndex, 7);
            var supportList8 = GetSupportedList(AxisIndex, 8);

            // keep select
            if (AxisIndex > 0)
            {
                var index = AxisIndex - 1;

                digitalInput1 = (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[index * 8 + 0];
                digitalInput2 = (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[index * 8 + 1];
                digitalInput3 = (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[index * 8 + 2];
                digitalInput4 = (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[index * 8 + 3];
                digitalInput5 = (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[index * 8 + 4];
                digitalInput6 = (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[index * 8 + 5];
                digitalInput7 = (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[index * 8 + 6];
                digitalInput8 = (DigitalInputType) ModifiedMotionDrive.DigitalInputConfiguration[index * 8 + 7];
            }

            DigitalInput1Source = EnumHelper.ToDataSource<DigitalInputType>(supportList1);
            DigitalInput2Source = EnumHelper.ToDataSource<DigitalInputType>(supportList2);
            DigitalInput3Source = EnumHelper.ToDataSource<DigitalInputType>(supportList3);
            DigitalInput4Source = EnumHelper.ToDataSource<DigitalInputType>(supportList4);
            DigitalInput5Source = EnumHelper.ToDataSource<DigitalInputType>(supportList5);
            DigitalInput6Source = EnumHelper.ToDataSource<DigitalInputType>(supportList6);
            DigitalInput7Source = EnumHelper.ToDataSource<DigitalInputType>(supportList7);
            DigitalInput8Source = EnumHelper.ToDataSource<DigitalInputType>(supportList8);

            if (AxisIndex > 0)
            {
                DigitalInput1 = supportList1.Contains(digitalInput1) ? digitalInput1 : DigitalInputType.Unassigned;
                DigitalInput2 = supportList2.Contains(digitalInput2) ? digitalInput2 : DigitalInputType.Unassigned;
                DigitalInput3 = supportList3.Contains(digitalInput3) ? digitalInput3 : DigitalInputType.Unassigned;
                DigitalInput4 = supportList4.Contains(digitalInput4) ? digitalInput4 : DigitalInputType.Unassigned;
                DigitalInput5 = supportList5.Contains(digitalInput5) ? digitalInput5 : DigitalInputType.Unassigned;
                DigitalInput6 = supportList6.Contains(digitalInput6) ? digitalInput6 : DigitalInputType.Unassigned;
                DigitalInput7 = supportList7.Contains(digitalInput7) ? digitalInput7 : DigitalInputType.Unassigned;
                DigitalInput8 = supportList8.Contains(digitalInput8) ? digitalInput8 : DigitalInputType.Unassigned;
            }
        }

        private List<DigitalInputType> GetSupportedList(int axisIndex, int inputIndex)
        {
            string member = $"DigitalInputConfiguration{axisIndex}_{inputIndex}";

            var moduleTypes = OriginalMotionDrive.Profiles.ModuleTypes;

            var configDefinition =
                moduleTypes
                    .GetConnectionConfigDefinitionByID(OriginalMotionDrive.ConfigID);

            var dataTypeDefinition = moduleTypes.GetDataTypeDefinitionByName(
                configDefinition.ConfigTag.DataType);

            var enumName = dataTypeDefinition.GetEnumByMember(member);

            List<int> valueList = moduleTypes.GetEnumValueList(enumName);

            List<DigitalInputType> result = new List<DigitalInputType>();

            foreach (var value in valueList)
            {
                result.Add((DigitalInputType) value);
            }

            return result;
        }

        private Visibility DigitalInputVisibility(int digitalInputIndex)
        {
            var axisIndex = AxisIndex;
            if (axisIndex > 0)
            {
                var numberOfConfigurableInput =
                    OriginalMotionDrive.ConfigData.NumberOfConfigurableInputs[axisIndex - 1];
                if (numberOfConfigurableInput >= digitalInputIndex)
                    return Visibility.Visible;
            }

            return Visibility.Hidden;
        }
    }
}
