using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    public partial class MessageConfigurationViewModel
    {
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
        private class ServiceTypeInfo
        {
            private static readonly Dictionary<CIPServiceTypeEnum, ServiceTypeInfo> Infos =
                new Dictionary<CIPServiceTypeEnum, ServiceTypeInfo>()
                {
                    {CIPServiceTypeEnum.AcceptConnection, new ServiceTypeInfo(CIPServiceTypeEnum.AcceptConnection)},
                    {CIPServiceTypeEnum.ApplyAttributes, new ServiceTypeInfo(CIPServiceTypeEnum.ApplyAttributes)},
                    {CIPServiceTypeEnum.AuditValueGet, new ServiceTypeInfo(CIPServiceTypeEnum.AuditValueGet)},
                    {CIPServiceTypeEnum.ChangesToDetectGet, new ServiceTypeInfo(CIPServiceTypeEnum.ChangesToDetectGet)},
                    {CIPServiceTypeEnum.ChangesToDetectSet, new ServiceTypeInfo(CIPServiceTypeEnum.ChangesToDetectSet)},
                    {
                        CIPServiceTypeEnum.ControllerLogAddEntry,
                        new ServiceTypeInfo(CIPServiceTypeEnum.ControllerLogAddEntry)
                    },
                    {
                        CIPServiceTypeEnum.ControllerLogAutomaticWriteGet,
                        new ServiceTypeInfo(CIPServiceTypeEnum.ControllerLogAutomaticWriteGet)
                    },
                    {
                        CIPServiceTypeEnum.ControllerLogAutomaticWriteSet,
                        new ServiceTypeInfo(CIPServiceTypeEnum.ControllerLogAutomaticWriteSet)
                    },
                    {
                        CIPServiceTypeEnum.ControllerLogConfigExecutionGet,
                        new ServiceTypeInfo(CIPServiceTypeEnum.ControllerLogConfigExecutionGet)
                    },
                    {
                        CIPServiceTypeEnum.ControllerLogConfigExecutionSet,
                        new ServiceTypeInfo(CIPServiceTypeEnum.ControllerLogConfigExecutionSet)
                    },
                    {
                        CIPServiceTypeEnum.ControllerLogWriteToMedia,
                        new ServiceTypeInfo(CIPServiceTypeEnum.ControllerLogWriteToMedia)
                    },
                    {CIPServiceTypeEnum.Custom, new ServiceTypeInfo(CIPServiceTypeEnum.Custom)},
                    {CIPServiceTypeEnum.DeleteSocket, new ServiceTypeInfo(CIPServiceTypeEnum.DeleteSocket)},
                    {CIPServiceTypeEnum.DeviceReset, new ServiceTypeInfo(CIPServiceTypeEnum.DeviceReset)},
                    {CIPServiceTypeEnum.DeviceWHO, new ServiceTypeInfo(CIPServiceTypeEnum.DeviceWHO)},
                    {CIPServiceTypeEnum.GetAttributeSingle, new ServiceTypeInfo(CIPServiceTypeEnum.GetAttributeSingle)},
                    {CIPServiceTypeEnum.OpenConnection, new ServiceTypeInfo(CIPServiceTypeEnum.OpenConnection)},
                    {CIPServiceTypeEnum.ParameterRead, new ServiceTypeInfo(CIPServiceTypeEnum.ParameterRead)},
                    {CIPServiceTypeEnum.ParameterWrite, new ServiceTypeInfo(CIPServiceTypeEnum.ParameterWrite)},
                    {
                        CIPServiceTypeEnum.PLSAxisConfiguration,
                        new ServiceTypeInfo(CIPServiceTypeEnum.PLSAxisConfiguration)
                    },
                    {
                        CIPServiceTypeEnum.PLSInputRegistration,
                        new ServiceTypeInfo(CIPServiceTypeEnum.PLSInputRegistration)
                    },
                    {CIPServiceTypeEnum.PLSOffsets, new ServiceTypeInfo(CIPServiceTypeEnum.PLSOffsets)},
                    {
                        CIPServiceTypeEnum.PLSSwitchConfiguration,
                        new ServiceTypeInfo(CIPServiceTypeEnum.PLSSwitchConfiguration)
                    },
                    {CIPServiceTypeEnum.PulseTest, new ServiceTypeInfo(CIPServiceTypeEnum.PulseTest)},
                    {CIPServiceTypeEnum.ReadSocket, new ServiceTypeInfo(CIPServiceTypeEnum.ReadSocket)},
                    {
                        CIPServiceTypeEnum.ResetElectronicFuse,
                        new ServiceTypeInfo(CIPServiceTypeEnum.ResetElectronicFuse)
                    },
                    {
                        CIPServiceTypeEnum.ResetLatchedDiagnosticsI,
                        new ServiceTypeInfo(CIPServiceTypeEnum.ResetLatchedDiagnosticsI)
                    },
                    {
                        CIPServiceTypeEnum.ResetLatchedDiagnosticsO,
                        new ServiceTypeInfo(CIPServiceTypeEnum.ResetLatchedDiagnosticsO)
                    },
                    {
                        CIPServiceTypeEnum.RetrieveCSTInformation,
                        new ServiceTypeInfo(CIPServiceTypeEnum.RetrieveCSTInformation)
                    },
                    {CIPServiceTypeEnum.SetAttributeSingle, new ServiceTypeInfo(CIPServiceTypeEnum.SetAttributeSingle)},
                    {CIPServiceTypeEnum.SocketCreate, new ServiceTypeInfo(CIPServiceTypeEnum.SocketCreate)},
                    {
                        CIPServiceTypeEnum.TrackedStateValueGet,
                        new ServiceTypeInfo(CIPServiceTypeEnum.TrackedStateValueGet)
                    },
                    {CIPServiceTypeEnum.UnlatchAllAlarmsI, new ServiceTypeInfo(CIPServiceTypeEnum.UnlatchAllAlarmsI)},
                    {CIPServiceTypeEnum.UnlatchAllAlarmsO, new ServiceTypeInfo(CIPServiceTypeEnum.UnlatchAllAlarmsO)},
                    {
                        CIPServiceTypeEnum.UnlatchAnalogHighAlarmI,
                        new ServiceTypeInfo(CIPServiceTypeEnum.UnlatchAnalogHighAlarmI)
                    },
                    {
                        CIPServiceTypeEnum.UnlatchAnalogHighHighAlarmI,
                        new ServiceTypeInfo(CIPServiceTypeEnum.UnlatchAnalogHighHighAlarmI)
                    },
                    {
                        CIPServiceTypeEnum.UnlatchAnalogLowAlarmI,
                        new ServiceTypeInfo(CIPServiceTypeEnum.UnlatchAnalogLowAlarmI)
                    },
                    {
                        CIPServiceTypeEnum.UnlatchAnalogLowLowAlarmI,
                        new ServiceTypeInfo(CIPServiceTypeEnum.UnlatchAnalogLowLowAlarmI)
                    },
                    {CIPServiceTypeEnum.UnlatchHighAlarmO, new ServiceTypeInfo(CIPServiceTypeEnum.UnlatchHighAlarmO)},
                    {CIPServiceTypeEnum.UnlatchLowAlarmO, new ServiceTypeInfo(CIPServiceTypeEnum.UnlatchLowAlarmO)},
                    {CIPServiceTypeEnum.UnlatchRampAlarmO, new ServiceTypeInfo(CIPServiceTypeEnum.UnlatchRampAlarmO)},
                    {CIPServiceTypeEnum.UnlatchRateAlarmI, new ServiceTypeInfo(CIPServiceTypeEnum.UnlatchRateAlarmI)},
                    {CIPServiceTypeEnum.WriteSocket, new ServiceTypeInfo(CIPServiceTypeEnum.WriteSocket)},
                };

            public static ServiceTypeInfo GetServiceTypeInfo(CIPServiceTypeEnum serviceType)
            {
                return Infos.ContainsKey(serviceType) ? Infos[serviceType] : null;
            }

            public static CIPServiceTypeEnum GetServiceType(
                ushort serviceCode,
                ushort classID,
                uint instanceID,
                ushort attributeID)
            {
                foreach (var info in Infos)
                {
                    if (info.Key == CIPServiceTypeEnum.Custom)
                        continue;

                    var serviceTypeInfo = info.Value;

                    if (serviceTypeInfo.ServiceCodeReadOnly && serviceTypeInfo.ServiceCode != serviceCode)
                        continue;

                    if (serviceTypeInfo.ClassIDReadOnly && serviceTypeInfo.ClassID != classID)
                        continue;

                    if (serviceTypeInfo.InstanceIDReadOnly && serviceTypeInfo.InstanceID != instanceID)
                        continue;

                    if (serviceTypeInfo.AttributeIDReadOnly && serviceTypeInfo.AttributeID != attributeID)
                        continue;

                    return info.Key;
                }

                return CIPServiceTypeEnum.Custom;

            }

            private ServiceTypeInfo(CIPServiceTypeEnum serviceType)
            {
                ServiceType = serviceType;

                switch (serviceType)
                {
                    case CIPServiceTypeEnum.AcceptConnection:
                        ServiceCode = 0x50;
                        ClassID = 0x342;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 4;
                        SourceLengthReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.ApplyAttributes:
                        ServiceCode = 0xd;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.AuditValueGet:
                        ServiceCode = 0xe;
                        ClassID = 0x8e;
                        InstanceID = 0x1;
                        AttributeID = 0x1b;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        break;
                    case CIPServiceTypeEnum.ChangesToDetectGet:
                        ServiceCode = 0xe;
                        ClassID = 0x8e;
                        InstanceID = 0x1;
                        AttributeID = 0x1c;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.ChangesToDetectSet:
                        ServiceCode = 0x10;
                        ClassID = 0x8e;
                        InstanceID = 0x1;
                        AttributeID = 0x1c;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 8;

                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.ControllerLogAddEntry:
                        ServiceCode = 0x5e;
                        ClassID = 0x8e;
                        InstanceID = 0x1;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 176;

                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.ControllerLogAutomaticWriteGet:
                        ServiceCode = 0xe;
                        ClassID = 0x8e;
                        InstanceID = 0x1;
                        AttributeID = 0x15;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        break;
                    case CIPServiceTypeEnum.ControllerLogAutomaticWriteSet:
                        ServiceCode = 0x10;
                        ClassID = 0x8e;
                        InstanceID = 0x1;
                        AttributeID = 0x15;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 1;

                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.ControllerLogConfigExecutionGet:
                        ServiceCode = 0xe;
                        ClassID = 0x8e;
                        InstanceID = 0x1;
                        AttributeID = 0x16;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.ControllerLogConfigExecutionSet:
                        ServiceCode = 0x10;
                        ClassID = 0x8e;
                        InstanceID = 0x1;
                        AttributeID = 0x16;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 4;

                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.ControllerLogWriteToMedia:
                        ServiceCode = 0x5d;
                        ClassID = 0x8e;
                        InstanceID = 0x1;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.Custom:

                        SourceLength = 0;

                        break;
                    case CIPServiceTypeEnum.DeleteSocket:
                        ServiceCode = 0x4f;
                        ClassID = 0x342;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.DeviceReset:
                        ServiceCode = 0x5;
                        ClassID = 0x1;
                        InstanceID = 0x1;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.DeviceWHO:
                        ServiceCode = 0x1;
                        ClassID = 0x1;
                        InstanceID = 0x1;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.GetAttributeSingle:
                        ServiceCode = 0xe;

                        ServiceCodeReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.OpenConnection:
                        ServiceCode = 0x4c;
                        ClassID = 0x342;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        DestinationElementReadOnly = true;
                        break;
                    case CIPServiceTypeEnum.ParameterRead:
                        ServiceCode = 0xe;
                        ClassID = 0xf;
                        AttributeID = 0x1;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.ParameterWrite:
                        ServiceCode = 0x10;
                        ClassID = 0xf;
                        AttributeID = 0x1;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.PLSAxisConfiguration:
                        ServiceCode = 0x4c;
                        ClassID = 0x4;
                        InstanceID = 17;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 44;

                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.PLSInputRegistration:
                        ServiceCode = 0x4b;
                        ClassID = 0x4;
                        InstanceID = 36;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.PLSOffsets:
                        ServiceCode = 0x4b;
                        ClassID = 0x4;
                        InstanceID = 37;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.PLSSwitchConfiguration:
                        ServiceCode = 0x4c;
                        ClassID = 0x4;
                        InstanceID = 33;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 124;

                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.PulseTest:
                        ServiceCode = 0x4c;
                        ClassID = 0x1e;
                        InstanceID = 1;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 10;

                        SourceLengthReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.ReadSocket:
                        ServiceCode = 0x4d;
                        ClassID = 0x342;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 8;

                        SourceLengthReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.ResetElectronicFuse:
                        ServiceCode = 0x4d;
                        ClassID = 0x1e;
                        InstanceID = 1;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 4;

                        SourceLengthReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.ResetLatchedDiagnosticsI:
                        ServiceCode = 0x4b;
                        ClassID = 0x1d;
                        InstanceID = 1;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 4;

                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.ResetLatchedDiagnosticsO:
                        ServiceCode = 0x4b;
                        ClassID = 0x1e;
                        InstanceID = 1;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 4;

                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.RetrieveCSTInformation:
                        ServiceCode = 0x1;
                        ClassID = 0x77;
                        InstanceID = 1;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.SetAttributeSingle:
                        ServiceCode = 0x10;

                        ServiceCodeReadOnly = true;

                        SourceLength = 0;

                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.SocketCreate:
                        ServiceCode = 0x4b;
                        ClassID = 0x342;
                        InstanceID = 0;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 12;
                        SourceLengthReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.TrackedStateValueGet:
                        ServiceCode = 0x32;
                        ClassID = 0x8e;
                        InstanceID = 1;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        InstanceIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 1;

                        SourceLengthReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.UnlatchAllAlarmsI:
                        ServiceCode = 0x4b;
                        ClassID = 0xa;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.UnlatchAllAlarmsO:
                        ServiceCode = 0x4b;
                        ClassID = 0xb;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.UnlatchAnalogHighAlarmI:
                        ServiceCode = 0x4b;
                        ClassID = 0xa;
                        AttributeID = 0x6c;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.UnlatchAnalogHighHighAlarmI:
                        ServiceCode = 0x4b;
                        ClassID = 0xa;
                        AttributeID = 0x6e;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.UnlatchAnalogLowAlarmI:
                        ServiceCode = 0x4b;
                        ClassID = 0xa;
                        AttributeID = 0x6b;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.UnlatchAnalogLowLowAlarmI:
                        ServiceCode = 0x4b;
                        ClassID = 0xa;
                        AttributeID = 0x6d;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.UnlatchHighAlarmO:
                        ServiceCode = 0x4b;
                        ClassID = 0xb;
                        AttributeID = 0x6f;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.UnlatchLowAlarmO:
                        ServiceCode = 0x4b;
                        ClassID = 0xb;
                        AttributeID = 0x6e;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.UnlatchRampAlarmO:
                        ServiceCode = 0x4b;
                        ClassID = 0xb;
                        AttributeID = 0x70;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.UnlatchRateAlarmI:
                        ServiceCode = 0x4b;
                        ClassID = 0xa;
                        AttributeID = 0x6f;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        SourceElementReadOnly = true;
                        SourceLengthReadOnly = true;
                        DestinationElementReadOnly = true;

                        break;
                    case CIPServiceTypeEnum.WriteSocket:
                        ServiceCode = 0x4e;
                        ClassID = 0x342;
                        AttributeID = 0x0;

                        ServiceCodeReadOnly = true;
                        ClassIDReadOnly = true;
                        AttributeIDReadOnly = true;

                        SourceLength = 0;

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(serviceType), serviceType, null);
                }
            }

            public CIPServiceTypeEnum ServiceType { get; }

            public ushort ServiceCode { get; }
            public uint InstanceID { get; }
            public ushort ClassID { get; }
            public ushort AttributeID { get; }
            public short SourceLength { get; } //REQ_LEN,0-32767

            public bool ServiceCodeReadOnly { get; }
            public bool InstanceIDReadOnly { get; }
            public bool ClassIDReadOnly { get; }
            public bool AttributeIDReadOnly { get; }

            public bool SourceElementReadOnly { get; }
            public bool SourceLengthReadOnly { get; }
            public bool DestinationElementReadOnly { get; }
        }
    }
}
