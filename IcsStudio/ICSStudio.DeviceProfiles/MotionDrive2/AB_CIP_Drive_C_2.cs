using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace ICSStudio.DeviceProfiles.MotionDrive2
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AB_CIP_Drive_C_2
    {
        public int CfgSize;
        public int CfgIDNum;
        public byte ConfigRevNumber;
        public byte ConfigurationBits; // BitHost
        public short Pad1;
        public int DriveClassID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] ConfigControl;

        // MDAO Class
        public int SyncThreshold;
        public byte ControllerUpdateDelayHighLimit;
        public byte ControllerUpdateDelayLowLimit;
        public byte TimeSyncSupport;
        public byte Pad2;
        public byte InverterSupport;
        public byte FWOptionBits;
        public short Pad3;

        //<!-- Module level DAO attributes to be updated to each axis. -->
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public int[] DrivePowerStructureAxisID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public float[] RegenerativePowerLimit;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public short[] PWMFrequency;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] DutySelect;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] BusRegulatorAction;

        public byte BusConfiguration;
        public byte DemonstrationModeSelect;
        public byte ShuntRegulatorResistorType;
        public byte ConverterACInputPhasing;

        public short ConverterACInputVoltage;
        public short Pad4;

        public byte BusVoltageSelect;
        public byte BusSharingGroup;
        public short ExternalShuntRegulatorID;

        public float ExternalShuntPower;
        public float ExternalShuntPulsePower;
        public float ExternalBusCapacitance;
        public float ExternalShuntResistance;
        public float ConverterOvertemperatureUserLimit;
        public float ConverterThermalOverloadUserLimit;
        public float ConverterGroundUserLimit;
        public float BusRegulatorOvertemperatureUserLimit;
        public float BusRegulatorThermalOverloadUserLimit;
        public float BusOvervoltageUserLimit;
        public float BusUndervoltageUserLimit;
        public float ControlModuleOvertemperatureUserLimit;
        public float ConverterPrechargeOverloadUserLimit;

        public byte NumberOfConfiguredAxes;
        public byte Pad5;
        public short Pad6;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] FeedbackPortSelect;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public short[] SelectedCardType;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] NumberOfConfigurableInputs;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] SourceOfConfigurableInputs;

        public short Pad7;

        public short PositionLoopDeviceUpdatePeriod;

        public short VelocityLoopDeviceUpdatePeriod;

        public short TorqueLoopDeviceUpdatePeriod;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] DigitalInputConfiguration;

        public static AB_CIP_Drive_C_2 FromDatValue(byte[] dataValue)
        {
            AB_CIP_Drive_C_2 temp = new AB_CIP_Drive_C_2();
            int size = Marshal.SizeOf(temp);
            Contract.Assert(size == dataValue.Length);

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(dataValue, 0, ptr, dataValue.Length);

            temp = Marshal.PtrToStructure<AB_CIP_Drive_C_2>(ptr);
            Marshal.FreeHGlobal(ptr);

            return temp;
        }

        public static AB_CIP_Drive_C_2 FromCDATA(string configValue)
        {
            configValue = configValue.Replace("[", "");
            configValue = configValue.Replace("]", "");

            var result = configValue.Split(',');
            int[] array = new int[result.Length];
            for (int i = 0; i < result.Length; i++)
            {
                array[i] = int.Parse(result[i].Trim());
            }

            AB_CIP_Drive_C_2 temp = new AB_CIP_Drive_C_2();
            int size = Marshal.SizeOf(temp);
            Contract.Assert(size / 4 == array.Length);

            IntPtr ptr = Marshal.AllocHGlobal(size);

            Marshal.Copy(array, 0, ptr, array.Length);

            temp = Marshal.PtrToStructure<AB_CIP_Drive_C_2>(ptr);
            Marshal.FreeHGlobal(ptr);

            return temp;
        }
    }
}
