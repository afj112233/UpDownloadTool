using System;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class ALARM_ANALOG : NativeType
    {
        private ALARM_ANALOG()
        {
            Name = "ALARM_ANALOG";
            {
                var member = new DataTypeMember
                {
                    Name = "EnableIn",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 0,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "In",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 4,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "InFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "HHEnabled",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 9,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "HEnabled",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 10,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "LEnabled",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 11,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "LLEnabled",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "AckRequired",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 13,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ProgAckAll",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 14,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "OperAckAll",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 15,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "HHProgAck",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 16,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "HHOperAck",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 17,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "HProgAck",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 18,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "HOperAck",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 19,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "LProgAck",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 20,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "LOperAck",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 21,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "LLProgAck",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 22,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "LLOperAck",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 23,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ROCPosProgAck",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 24,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ROCPosOperAck",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 25,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ROCNegProgAck",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 26,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ROCNegOperAck",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 27,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ProgSuppress",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 28,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "OperSuppress",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 29,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ProgUnsuppress",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 30,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "OperUnsuppress",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 31,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "HHOperShelve",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 32,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "HOperShelve",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 33,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "LOperShelve",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 34,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "LLOperShelve",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 35,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ROCPosOperShelve",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 36,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ROCNegOperShelve",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 37,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ProgUnshelveAll",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 38,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "HHOperUnshelve",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 39,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "HOperUnshelve",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 40,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "LOperUnshelve",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 41,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "LLOperUnshelve",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 42,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ROCPosOperUnshelve",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 43,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ROCNegOperUnshelve",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 44,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ProgDisable",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 45,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "OperDisable",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 46,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ProgEnable",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 47,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "OperEnable",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 48,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "AlarmCountReset",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 49,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "HHMinDurationEnable",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 50,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "HMinDurationEnable",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 51,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "LMinDurationEnable",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 52,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "LLMinDurationEnable",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 53,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "HHLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 56,
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
                    Name = "HHSeverity",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 60,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "HLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 64,
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
                    Name = "HSeverity",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 68,
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
                    Name = "LLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 72,
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
                    Name = "LSeverity",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 76,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "LLLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 80,
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
                    Name = "LLSeverity",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 84,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "MinDurationPRE",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 88,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "ShelveDuration",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 92,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "MaxShelveDuration",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 96,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "Deadband",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 100,
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
                    Name = "ROCPosLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 104,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "ROCPosSeverity",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 108,
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
                    Name = "ROCNegLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 112,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "ROCNegSeverity",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 116,
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
                    Name = "ROCPeriod",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 120,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "EnableOut",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 124,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 65,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "InAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 125,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 66,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AnyInAlarmUnack",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 126,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 67,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HHInAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 127,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 68,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HInAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 128,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 69,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LInAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 129,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 70,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LLInAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 130,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 71,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCPosInAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 131,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 72,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCNegInAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 132,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 73,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROC",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 136,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 74,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HHAcked",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 140,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 75,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HAcked",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 141,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 76,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LAcked",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 142,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 77,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LLAcked",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 143,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 78,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCPosAcked",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 144,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 79,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCNegAcked",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 145,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 80,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HHInAlarmUnack",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 146,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 81,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HInAlarmUnack",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 147,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 82,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LInAlarmUnack",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 148,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 83,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LLInAlarmUnack",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 149,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 84,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCPosInAlarmUnack",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 150,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 85,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCNegInAlarmUnack",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 151,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 86,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Suppressed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 152,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 87,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HHShelved",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 153,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 88,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HShelved",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 154,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 89,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LShelved",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 155,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 90,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LLShelved",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 156,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 91,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCPosShelved",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 157,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 92,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCNegShelved",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 158,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 93,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Disabled",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 159,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 94,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Commissioned",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 160,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 95,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "MinDurationACC",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 164,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 96,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HHInAlarmTime",
                    DisplayStyle = DisplayStyle.DateTime,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 168,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = LINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 97,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HHAlarmCount",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 176,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 98,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HInAlarmTime",
                    DisplayStyle = DisplayStyle.DateTime,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 184,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = LINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 99,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HAlarmCount",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 192,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 100,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LInAlarmTime",
                    DisplayStyle = DisplayStyle.DateTime,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 200,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = LINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 101,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LAlarmCount",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 208,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 102,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LLInAlarmTime",
                    DisplayStyle = DisplayStyle.DateTime,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 216,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = LINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 103,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LLAlarmCount",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 224,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 104,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCPosInAlarmTime",
                    DisplayStyle = DisplayStyle.DateTime,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 232,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = LINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 105,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCPosAlarmCount",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 106,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCNegInAlarmTime",
                    DisplayStyle = DisplayStyle.DateTime,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = LINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 107,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCNegAlarmCount",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 256,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 108,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AckTime",
                    DisplayStyle = DisplayStyle.DateTime,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 264,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = LINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 109,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "RetToNormalTime",
                    DisplayStyle = DisplayStyle.DateTime,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 272,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = LINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 110,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AlarmCountResetTime",
                    DisplayStyle = DisplayStyle.DateTime,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 280,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = LINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 111,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ShelveTime",
                    DisplayStyle = DisplayStyle.DateTime,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 288,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = LINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 112,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "UnshelveTime",
                    DisplayStyle = DisplayStyle.DateTime,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 296,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = LINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 113,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Status",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 304,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 114,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "InstructFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 304,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 114,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "InFaulted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 304,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 114,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SeverityInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 304,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 114,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AlarmLimitsInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 304,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 114,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DeadbandInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 304,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 114,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCPosLimitInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 304,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 114,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCNegLimitInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 304,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 114,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCPeriodInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 304,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 114,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "InN_1",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 308,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 115,
                    IsHidden = true,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ProgAckAllN_1",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 312,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 116,
                    IsHidden = true,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HHProgAckN_1",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 313,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 117,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "HProgAckN_1",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 314,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 118,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LProgAckN_1",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 315,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 119,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "LLProgAckN_1",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 316,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 120,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCPosProgAckN_1",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 317,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 121,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ROCNegProgAckN_1",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 318,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 122,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PeriodStamp",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 320,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 123,
                    IsHidden = true,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
        }
        public static readonly ALARM_ANALOG Inst = new ALARM_ANALOG();
        public override IField Create(JToken token)
        {
            var res = new ALARM_ANALOGField(token);
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
        public override int BitSize => 2624;
        public override int ByteSize => 328;
        public const int size = 328;
    }
    public sealed class ALARM_ANALOGField : PreDefinedField
    {
        public ALARM_ANALOGField(JToken token)
        {
            Debug.Assert(token == null || token is JArray);
            var array = token as JArray;
            fields.Add(Tuple.Create(new Int8Field(array?[0]) as IField, 0));
            fields.Add(Tuple.Create(new RealField(array?[1]) as IField, 4));
            fields.Add(Tuple.Create(new Int8Field(array?[2]) as IField, 8));
            fields.Add(Tuple.Create(new Int8Field(array?[3]) as IField, 9));
            fields.Add(Tuple.Create(new Int8Field(array?[4]) as IField, 10));
            fields.Add(Tuple.Create(new Int8Field(array?[5]) as IField, 11));
            fields.Add(Tuple.Create(new Int8Field(array?[6]) as IField, 12));
            fields.Add(Tuple.Create(new Int8Field(array?[7]) as IField, 13));
            fields.Add(Tuple.Create(new Int8Field(array?[8]) as IField, 14));
            fields.Add(Tuple.Create(new Int8Field(array?[9]) as IField, 15));
            fields.Add(Tuple.Create(new Int8Field(array?[10]) as IField, 16));
            fields.Add(Tuple.Create(new Int8Field(array?[11]) as IField, 17));
            fields.Add(Tuple.Create(new Int8Field(array?[12]) as IField, 18));
            fields.Add(Tuple.Create(new Int8Field(array?[13]) as IField, 19));
            fields.Add(Tuple.Create(new Int8Field(array?[14]) as IField, 20));
            fields.Add(Tuple.Create(new Int8Field(array?[15]) as IField, 21));
            fields.Add(Tuple.Create(new Int8Field(array?[16]) as IField, 22));
            fields.Add(Tuple.Create(new Int8Field(array?[17]) as IField, 23));
            fields.Add(Tuple.Create(new Int8Field(array?[18]) as IField, 24));
            fields.Add(Tuple.Create(new Int8Field(array?[19]) as IField, 25));
            fields.Add(Tuple.Create(new Int8Field(array?[20]) as IField, 26));
            fields.Add(Tuple.Create(new Int8Field(array?[21]) as IField, 27));
            fields.Add(Tuple.Create(new Int8Field(array?[22]) as IField, 28));
            fields.Add(Tuple.Create(new Int8Field(array?[23]) as IField, 29));
            fields.Add(Tuple.Create(new Int8Field(array?[24]) as IField, 30));
            fields.Add(Tuple.Create(new Int8Field(array?[25]) as IField, 31));
            fields.Add(Tuple.Create(new Int8Field(array?[26]) as IField, 32));
            fields.Add(Tuple.Create(new Int8Field(array?[27]) as IField, 33));
            fields.Add(Tuple.Create(new Int8Field(array?[28]) as IField, 34));
            fields.Add(Tuple.Create(new Int8Field(array?[29]) as IField, 35));
            fields.Add(Tuple.Create(new Int8Field(array?[30]) as IField, 36));
            fields.Add(Tuple.Create(new Int8Field(array?[31]) as IField, 37));
            fields.Add(Tuple.Create(new Int8Field(array?[32]) as IField, 38));
            fields.Add(Tuple.Create(new Int8Field(array?[33]) as IField, 39));
            fields.Add(Tuple.Create(new Int8Field(array?[34]) as IField, 40));
            fields.Add(Tuple.Create(new Int8Field(array?[35]) as IField, 41));
            fields.Add(Tuple.Create(new Int8Field(array?[36]) as IField, 42));
            fields.Add(Tuple.Create(new Int8Field(array?[37]) as IField, 43));
            fields.Add(Tuple.Create(new Int8Field(array?[38]) as IField, 44));
            fields.Add(Tuple.Create(new Int8Field(array?[39]) as IField, 45));
            fields.Add(Tuple.Create(new Int8Field(array?[40]) as IField, 46));
            fields.Add(Tuple.Create(new Int8Field(array?[41]) as IField, 47));
            fields.Add(Tuple.Create(new Int8Field(array?[42]) as IField, 48));
            fields.Add(Tuple.Create(new Int8Field(array?[43]) as IField, 49));
            fields.Add(Tuple.Create(new Int8Field(array?[44]) as IField, 50));
            fields.Add(Tuple.Create(new Int8Field(array?[45]) as IField, 51));
            fields.Add(Tuple.Create(new Int8Field(array?[46]) as IField, 52));
            fields.Add(Tuple.Create(new Int8Field(array?[47]) as IField, 53));
            fields.Add(Tuple.Create(new RealField(array?[48]) as IField, 56));
            fields.Add(Tuple.Create(new Int32Field(array?[49]) as IField, 60));
            fields.Add(Tuple.Create(new RealField(array?[50]) as IField, 64));
            fields.Add(Tuple.Create(new Int32Field(array?[51]) as IField, 68));
            fields.Add(Tuple.Create(new RealField(array?[52]) as IField, 72));
            fields.Add(Tuple.Create(new Int32Field(array?[53]) as IField, 76));
            fields.Add(Tuple.Create(new RealField(array?[54]) as IField, 80));
            fields.Add(Tuple.Create(new Int32Field(array?[55]) as IField, 84));
            fields.Add(Tuple.Create(new Int32Field(array?[56]) as IField, 88));
            fields.Add(Tuple.Create(new Int32Field(array?[57]) as IField, 92));
            fields.Add(Tuple.Create(new Int32Field(array?[58]) as IField, 96));
            fields.Add(Tuple.Create(new RealField(array?[59]) as IField, 100));
            fields.Add(Tuple.Create(new RealField(array?[60]) as IField, 104));
            fields.Add(Tuple.Create(new Int32Field(array?[61]) as IField, 108));
            fields.Add(Tuple.Create(new RealField(array?[62]) as IField, 112));
            fields.Add(Tuple.Create(new Int32Field(array?[63]) as IField, 116));
            fields.Add(Tuple.Create(new RealField(array?[64]) as IField, 120));
            fields.Add(Tuple.Create(new Int8Field(array?[65]) as IField, 124));
            fields.Add(Tuple.Create(new Int8Field(array?[66]) as IField, 125));
            fields.Add(Tuple.Create(new Int8Field(array?[67]) as IField, 126));
            fields.Add(Tuple.Create(new Int8Field(array?[68]) as IField, 127));
            fields.Add(Tuple.Create(new Int8Field(array?[69]) as IField, 128));
            fields.Add(Tuple.Create(new Int8Field(array?[70]) as IField, 129));
            fields.Add(Tuple.Create(new Int8Field(array?[71]) as IField, 130));
            fields.Add(Tuple.Create(new Int8Field(array?[72]) as IField, 131));
            fields.Add(Tuple.Create(new Int8Field(array?[73]) as IField, 132));
            fields.Add(Tuple.Create(new RealField(array?[74]) as IField, 136));
            fields.Add(Tuple.Create(new Int8Field(array?[75]) as IField, 140));
            fields.Add(Tuple.Create(new Int8Field(array?[76]) as IField, 141));
            fields.Add(Tuple.Create(new Int8Field(array?[77]) as IField, 142));
            fields.Add(Tuple.Create(new Int8Field(array?[78]) as IField, 143));
            fields.Add(Tuple.Create(new Int8Field(array?[79]) as IField, 144));
            fields.Add(Tuple.Create(new Int8Field(array?[80]) as IField, 145));
            fields.Add(Tuple.Create(new Int8Field(array?[81]) as IField, 146));
            fields.Add(Tuple.Create(new Int8Field(array?[82]) as IField, 147));
            fields.Add(Tuple.Create(new Int8Field(array?[83]) as IField, 148));
            fields.Add(Tuple.Create(new Int8Field(array?[84]) as IField, 149));
            fields.Add(Tuple.Create(new Int8Field(array?[85]) as IField, 150));
            fields.Add(Tuple.Create(new Int8Field(array?[86]) as IField, 151));
            fields.Add(Tuple.Create(new Int8Field(array?[87]) as IField, 152));
            fields.Add(Tuple.Create(new Int8Field(array?[88]) as IField, 153));
            fields.Add(Tuple.Create(new Int8Field(array?[89]) as IField, 154));
            fields.Add(Tuple.Create(new Int8Field(array?[90]) as IField, 155));
            fields.Add(Tuple.Create(new Int8Field(array?[91]) as IField, 156));
            fields.Add(Tuple.Create(new Int8Field(array?[92]) as IField, 157));
            fields.Add(Tuple.Create(new Int8Field(array?[93]) as IField, 158));
            fields.Add(Tuple.Create(new Int8Field(array?[94]) as IField, 159));
            fields.Add(Tuple.Create(new Int8Field(array?[95]) as IField, 160));
            fields.Add(Tuple.Create(new Int32Field(array?[96]) as IField, 164));
            fields.Add(Tuple.Create(new Int64Field(array?[97]) as IField, 168));
            fields.Add(Tuple.Create(new Int32Field(array?[98]) as IField, 176));
            fields.Add(Tuple.Create(new Int64Field(array?[99]) as IField, 184));
            fields.Add(Tuple.Create(new Int32Field(array?[100]) as IField, 192));
            fields.Add(Tuple.Create(new Int64Field(array?[101]) as IField, 200));
            fields.Add(Tuple.Create(new Int32Field(array?[102]) as IField, 208));
            fields.Add(Tuple.Create(new Int64Field(array?[103]) as IField, 216));
            fields.Add(Tuple.Create(new Int32Field(array?[104]) as IField, 224));
            fields.Add(Tuple.Create(new Int64Field(array?[105]) as IField, 232));
            fields.Add(Tuple.Create(new Int32Field(array?[106]) as IField, 240));
            fields.Add(Tuple.Create(new Int64Field(array?[107]) as IField, 248));
            fields.Add(Tuple.Create(new Int32Field(array?[108]) as IField, 256));
            fields.Add(Tuple.Create(new Int64Field(array?[109]) as IField, 264));
            fields.Add(Tuple.Create(new Int64Field(array?[110]) as IField, 272));
            fields.Add(Tuple.Create(new Int64Field(array?[111]) as IField, 280));
            fields.Add(Tuple.Create(new Int64Field(array?[112]) as IField, 288));
            fields.Add(Tuple.Create(new Int64Field(array?[113]) as IField, 296));
            fields.Add(Tuple.Create(new Int32Field(array?[114]) as IField, 304));
            fields.Add(Tuple.Create(new RealField(array?[115]) as IField, 308));
            fields.Add(Tuple.Create(new Int8Field(array?[116]) as IField, 312));
            fields.Add(Tuple.Create(new Int8Field(array?[117]) as IField, 313));
            fields.Add(Tuple.Create(new Int8Field(array?[118]) as IField, 314));
            fields.Add(Tuple.Create(new Int8Field(array?[119]) as IField, 315));
            fields.Add(Tuple.Create(new Int8Field(array?[120]) as IField, 316));
            fields.Add(Tuple.Create(new Int8Field(array?[121]) as IField, 317));
            fields.Add(Tuple.Create(new Int8Field(array?[122]) as IField, 318));
            fields.Add(Tuple.Create(new RealField(array?[123]) as IField, 320));
        }
    }
}