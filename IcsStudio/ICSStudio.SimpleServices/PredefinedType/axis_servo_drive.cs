using System;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class AXIS_SERVO_DRIVE : NativeType
    {
        private AXIS_SERVO_DRIVE()
        {
            Name = "AXIS_SERVO_DRIVE";
            {
                var member = new DataTypeMember
                {
                    Name = "AxisFault",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 0,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PhysicalAxisFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 0,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ModuleFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 0,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ConfigFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 0,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GroupFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 0,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AxisStatus",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 1,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ServoActionStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 1,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DriveEnableStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 1,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ShutdownStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 1,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ConfigUpdateInProcess",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 1,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "InhibitStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 1,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DirectControlStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 1,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AxisUpdateStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 1,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MotionStatus",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AccelStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DecelStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MoveStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "JogStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GearingStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HomingStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "StoppingStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AxisHomedStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PositionCamStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "TimeCamStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PositionCamPendingStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "TimeCamPendingStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 11,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GearingLockStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 12,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PositionCamLockStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 13,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "TimeCamLockStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 14,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MasterOffsetMoveStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 15,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CoordinatedMotionStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 16,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "TransformStateStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 17,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ControlledByTransformStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 18,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MoveLockStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 22,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "JogLockStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 24,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MasterOffsetMoveLockStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 26,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MaximumSpeedExceeded",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 27,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 2,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AxisEvent",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 3,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "WatchEventArmedStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 3,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "WatchEventStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 3,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "RegEvent1ArmedStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 3,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "RegEvent1Status",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 3,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "RegEvent2ArmedStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 3,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "RegEvent2Status",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 3,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HomeEventArmedStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 3,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HomeEventStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 3,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OutputCamStatus",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 16,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 4,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OutputCamPendingStatus",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 20,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 5,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OutputCamLockStatus",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 24,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 6,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OutputCamTransitionStatus",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 28,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 7,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ActualPosition",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 32,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 8,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "StrobeActualPosition",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 36,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 9,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "StartActualPosition",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 40,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 10,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AverageVelocity",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 44,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 11,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ActualVelocity",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 48,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 12,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ActualAcceleration",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 52,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 13,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "WatchPosition",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 56,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 14,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Registration1Position",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 60,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 15,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Registration2Position",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 64,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 16,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Registration1Time",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 68,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 17,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Registration2Time",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 72,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 18,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "InterpolationTime",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 76,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 19,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "InterpolatedActualPosition",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 80,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 20,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "InterpolatedCommandPosition",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 84,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 21,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MasterOffset",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 88,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 22,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "StrobeMasterOffset",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 92,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 23,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "StartMasterOffset",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 96,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 24,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CommandPosition",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 100,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 25,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "StrobeCommandPosition",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 104,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 26,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "StartCommandPosition",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 108,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 27,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CommandVelocity",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 112,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 28,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CommandAcceleration",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 116,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 29,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ModuleFaults",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 120,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 30,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ControlSyncFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 120,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 30,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ModuleSyncFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 120,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 30,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "TimerEventFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 120,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 30,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ModuleHardwareFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 120,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 30,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SERCOSRingFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 120,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 30,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AttributeErrorCode",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 124,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = INT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 31,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AttributeErrorID",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 126,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = INT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 32,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PositionCommand",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 128,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 33,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PositionFeedback",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 132,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 34,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AuxPositionFeedback",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 136,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 35,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PositionError",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 140,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 36,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PositionIntegratorError",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 144,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 37,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "VelocityCommand",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 148,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 38,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "VelocityFeedback",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 152,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 39,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "VelocityError",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 156,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 40,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "VelocityIntegratorError",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 160,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 41,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AccelerationCommand",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 164,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 42,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AccelerationFeedback",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 168,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 43,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MarkerDistance",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 172,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 44,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "VelocityOffset",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 176,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 45,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "TorqueOffset",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 180,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 46,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "TorqueCommand",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 184,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 47,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "TorqueFeedback",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 188,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 48,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PosDynamicTorqueLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 192,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 49,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "NegDynamicTorqueLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 196,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 50,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "TorqueLimitSource",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 200,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 51,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MotorElectricalAngle",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 204,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 52,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DCBusVoltage",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 208,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 53,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MotorCapacity",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 212,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 54,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DriveCapacity",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 216,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 55,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PowerCapacity",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 220,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 56,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "BusRegulatorCapacity",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 224,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 57,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AnalogInput1",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 228,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 58,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AnalogInput2",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 232,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 59,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DriveStatus",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ProcessStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "BusReadyStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HomeInputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Reg1InputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Reg2InputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PosOvertravelInputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "NegOvertravelInputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "EnableInputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 11,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AccelLimitStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 12,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AbsoluteReferenceStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 13,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SafeOffModeActiveStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 14,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "VelocityLockStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 16,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "VelocityStandstillStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 17,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "VelocityThresholdStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 18,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "TorqueThresholdStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 19,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "TorqueLimitStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 20,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "VelocityLimitStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 21,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PositionLockStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 22,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PowerLimitStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 23,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LowVelocityThresholdStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 24,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HighVelocityThresholdStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
                    BitOffset = 25,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 60,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DriveFault",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PosSoftOvertravelFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "NegSoftOvertravelFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PosHardOvertravelFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "NegHardOvertravelFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MotFeedbackFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MotFeedbackNoiseFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AuxFeedbackFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AuxFeedbackNoiseFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "IPIMFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DriveEnableInputFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CommonBusFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PreChargeOverloadFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 11,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardFaultExists",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 12,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GroundShortFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 13,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DriveHardFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 14,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OverSpeedFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 15,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OverloadFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 16,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DriveOvertempFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 17,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MotorOvertempFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 18,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DriveCoolingFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 19,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DriveControlVoltageFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 20,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "FeedbackFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 21,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CommutationFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 22,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DriveOvercurrentFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 23,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DriveOvervoltageFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 24,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DriveUndervoltageFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 25,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PowerPhaseLossFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 26,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PositionErrorFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 27,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SERCOSFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 28,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 61,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SERCOSErrorCode",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 244,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = INT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 62,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardStatus",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardOKStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardConfigLockedStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardGateDriveOutputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardStopInputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardStopRequestStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardStopInProgressStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardStopDecelStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardStopStandstillStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardStopOutputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardLimitedSpeedInputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardLimitedSpeedRequestStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardLimitedSpeedMonitorInProgressStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 11,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardLimitedSpeedOutputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 12,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardMaxSpeedMonitorInProgressStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 13,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardMaxAccelMonitorInProgressStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 14,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardDirectionMonitorInProgressStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 15,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardDoorControlLockStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 16,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardDoorControlOutputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 17,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardDoorMonitorInputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 18,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardDoorMonitorInProgressStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 19,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardLockMonitorInputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 20,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardEnablingSwitchInputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 21,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardEnablingSwitchInProgressStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 22,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardResetInputStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 23,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardResetRequiredStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 24,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardStopInputCycleRequiredStatus",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 25,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 63,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardFaults",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardInternalFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardConfigurationFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardGateDriveFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardResetFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardFeedback1Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardFeedback2Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardFeedbackSpeedCompareFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardFeedbackPositionCompareFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardStopInputFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardStopOutputFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardStopDecelFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 11,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardStopStandstillFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 12,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardStopMotionFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 13,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardLimitedSpeedInputFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 14,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardLimitedSpeedOutputFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 15,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardLimitedSpeedMonitorFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 16,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardMaxSpeedMonitorFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 17,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardMaxAccelMonitorFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 18,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardDirectionMonitorFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 19,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardDoorMonitorInputFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 20,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardDoorMonitorFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 21,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardDoorControlOutputFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 22,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardLockMonitorInputFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 23,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardLockMonitorFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 24,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardEnablingSwitchMonitorInputFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 25,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardEnablingSwitchMonitorFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 26,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardFeedback1VoltageMonitorFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 27,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "GuardFeedback2VoltageMonitorFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 28,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 64,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
        }
        public static readonly AXIS_SERVO_DRIVE Inst = new AXIS_SERVO_DRIVE();
        public override IField Create(JToken token)
        {
            var res = new AXIS_SERVO_DRIVEField(token);
            FixUp(token, res);
            return res;
        }
        public override bool SupportsOneDimensionalArray => true;
        public override bool SupportsMultiDimensionalArrays => true;
        public override bool IsPredefinedType => true;
        public override bool IsStruct => true;
        public override bool IsAxisType => false;
        public override bool IsMotionGroupType => false;
        public override bool IsCoordinateSystemType => false;
        public override int BitSize => 2048;
        public override int ByteSize => 256;
        public const int size = 256;
    }
    public sealed class AXIS_SERVO_DRIVEField : PreDefinedField
    {
        public AXIS_SERVO_DRIVEField(JToken token)
        {
            Debug.Assert(token == null || token is JArray);
            var array = token as JArray;
            fields.Add(Tuple.Create(new Int32Field(array?[0]) as IField, 0));
            fields.Add(Tuple.Create(new Int32Field(array?[1]) as IField, 4));
            fields.Add(Tuple.Create(new Int32Field(array?[2]) as IField, 8));
            fields.Add(Tuple.Create(new Int32Field(array?[3]) as IField, 12));
            fields.Add(Tuple.Create(new Int32Field(array?[4]) as IField, 16));
            fields.Add(Tuple.Create(new Int32Field(array?[5]) as IField, 20));
            fields.Add(Tuple.Create(new Int32Field(array?[6]) as IField, 24));
            fields.Add(Tuple.Create(new Int32Field(array?[7]) as IField, 28));
            fields.Add(Tuple.Create(new RealField(array?[8]) as IField, 32));
            fields.Add(Tuple.Create(new RealField(array?[9]) as IField, 36));
            fields.Add(Tuple.Create(new RealField(array?[10]) as IField, 40));
            fields.Add(Tuple.Create(new RealField(array?[11]) as IField, 44));
            fields.Add(Tuple.Create(new RealField(array?[12]) as IField, 48));
            fields.Add(Tuple.Create(new RealField(array?[13]) as IField, 52));
            fields.Add(Tuple.Create(new RealField(array?[14]) as IField, 56));
            fields.Add(Tuple.Create(new RealField(array?[15]) as IField, 60));
            fields.Add(Tuple.Create(new RealField(array?[16]) as IField, 64));
            fields.Add(Tuple.Create(new Int32Field(array?[17]) as IField, 68));
            fields.Add(Tuple.Create(new Int32Field(array?[18]) as IField, 72));
            fields.Add(Tuple.Create(new Int32Field(array?[19]) as IField, 76));
            fields.Add(Tuple.Create(new RealField(array?[20]) as IField, 80));
            fields.Add(Tuple.Create(new RealField(array?[21]) as IField, 84));
            fields.Add(Tuple.Create(new RealField(array?[22]) as IField, 88));
            fields.Add(Tuple.Create(new RealField(array?[23]) as IField, 92));
            fields.Add(Tuple.Create(new RealField(array?[24]) as IField, 96));
            fields.Add(Tuple.Create(new RealField(array?[25]) as IField, 100));
            fields.Add(Tuple.Create(new RealField(array?[26]) as IField, 104));
            fields.Add(Tuple.Create(new RealField(array?[27]) as IField, 108));
            fields.Add(Tuple.Create(new RealField(array?[28]) as IField, 112));
            fields.Add(Tuple.Create(new RealField(array?[29]) as IField, 116));
            fields.Add(Tuple.Create(new Int32Field(array?[30]) as IField, 120));
            fields.Add(Tuple.Create(new Int16Field(array?[31]) as IField, 124));
            fields.Add(Tuple.Create(new Int16Field(array?[32]) as IField, 126));
            fields.Add(Tuple.Create(new RealField(array?[33]) as IField, 128));
            fields.Add(Tuple.Create(new RealField(array?[34]) as IField, 132));
            fields.Add(Tuple.Create(new RealField(array?[35]) as IField, 136));
            fields.Add(Tuple.Create(new RealField(array?[36]) as IField, 140));
            fields.Add(Tuple.Create(new RealField(array?[37]) as IField, 144));
            fields.Add(Tuple.Create(new RealField(array?[38]) as IField, 148));
            fields.Add(Tuple.Create(new RealField(array?[39]) as IField, 152));
            fields.Add(Tuple.Create(new RealField(array?[40]) as IField, 156));
            fields.Add(Tuple.Create(new RealField(array?[41]) as IField, 160));
            fields.Add(Tuple.Create(new RealField(array?[42]) as IField, 164));
            fields.Add(Tuple.Create(new RealField(array?[43]) as IField, 168));
            fields.Add(Tuple.Create(new RealField(array?[44]) as IField, 172));
            fields.Add(Tuple.Create(new RealField(array?[45]) as IField, 176));
            fields.Add(Tuple.Create(new RealField(array?[46]) as IField, 180));
            fields.Add(Tuple.Create(new RealField(array?[47]) as IField, 184));
            fields.Add(Tuple.Create(new RealField(array?[48]) as IField, 188));
            fields.Add(Tuple.Create(new RealField(array?[49]) as IField, 192));
            fields.Add(Tuple.Create(new RealField(array?[50]) as IField, 196));
            fields.Add(Tuple.Create(new Int32Field(array?[51]) as IField, 200));
            fields.Add(Tuple.Create(new RealField(array?[52]) as IField, 204));
            fields.Add(Tuple.Create(new RealField(array?[53]) as IField, 208));
            fields.Add(Tuple.Create(new RealField(array?[54]) as IField, 212));
            fields.Add(Tuple.Create(new RealField(array?[55]) as IField, 216));
            fields.Add(Tuple.Create(new RealField(array?[56]) as IField, 220));
            fields.Add(Tuple.Create(new RealField(array?[57]) as IField, 224));
            fields.Add(Tuple.Create(new RealField(array?[58]) as IField, 228));
            fields.Add(Tuple.Create(new RealField(array?[59]) as IField, 232));
            fields.Add(Tuple.Create(new Int32Field(array?[60]) as IField, 236));
            fields.Add(Tuple.Create(new Int32Field(array?[61]) as IField, 240));
            fields.Add(Tuple.Create(new Int16Field(array?[62]) as IField, 244));
            fields.Add(Tuple.Create(new Int32Field(array?[63]) as IField, 248));
            fields.Add(Tuple.Create(new Int32Field(array?[64]) as IField, 252));
        }
    }
}