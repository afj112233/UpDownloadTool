using System;
using System.Diagnostics;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.DataType;
using Newtonsoft.Json.Linq;
namespace ICSStudio.SimpleServices.PredefinedType
{
    public sealed class MMC : NativeType
    {
        private MMC()
        {
            Name = "MMC";
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
                    Name = "PV1",
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
                    Name = "PV2",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 8,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "PV1Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 12,
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
                    Name = "PV2Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 13,
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
                    Name = "PV1EUMax",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 16,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "PV2EUMax",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 20,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "PV1EUMin",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 24,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "PV2EUMin",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 28,
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
                    Name = "SP1Prog",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 32,
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
                    Name = "SP2Prog",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 36,
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
                    Name = "SP1Oper",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 40,
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
                    Name = "SP2Oper",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 44,
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
                    Name = "SP1HLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 48,
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
                    Name = "SP2HLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 52,
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
                    Name = "SP1LLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 56,
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
                    Name = "SP2LLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 60,
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
                    Name = "CV1Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 64,
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
                    Name = "CV2Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 65,
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
                    Name = "CV3Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 66,
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
                    Name = "CV1InitReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 67,
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
                    Name = "CV2InitReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 68,
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
                    Name = "CV3InitReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 69,
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
                    Name = "CV1InitValue",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 72,
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
                    Name = "CV2InitValue",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 76,
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
                    Name = "CV3InitValue",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 80,
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
                    Name = "CV1Prog",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 84,
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
                    Name = "CV2Prog",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 88,
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
                    Name = "CV3Prog",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 92,
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
                    Name = "CV1Oper",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 96,
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
                    Name = "CV2Oper",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 100,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV3Oper",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 104,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV1OverrideValue",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 108,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV2OverrideValue",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 112,
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
                    Name = "CV3OverrideValue",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 116,
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
                    Name = "CVManLimiting",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 120,
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
                    Name = "CV1EUMax",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 124,
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
                    Name = "CV2EUMax",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 128,
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
                    Name = "CV3EUMax",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 132,
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
                    Name = "CV1EUMin",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 136,
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
                    Name = "CV2EUMin",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 140,
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
                    Name = "CV3EUMin",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 144,
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
                    Name = "CV1HLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 148,
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
                    Name = "CV2HLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 152,
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
                    Name = "CV3HLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 156,
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
                    Name = "CV1LLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 160,
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
                    Name = "CV2LLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 164,
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
                    Name = "CV3LLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 168,
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
                    Name = "CV1ROCPosLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 172,
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
                    Name = "CV2ROCPosLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 176,
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
                    Name = "CV3ROCPosLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 180,
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
                    Name = "CV1ROCNegLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 184,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV2ROCNegLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 188,
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
                    Name = "CV3ROCNegLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 192,
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
                    Name = "CV1HandFB",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 196,
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
                    Name = "CV2HandFB",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 200,
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
                    Name = "CV3HandFB",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 204,
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
                    Name = "CV1HandFBFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 208,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "CV2HandFBFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 209,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "CV3HandFBFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 210,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "CV1Target",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 212,
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
                    Name = "CV2Target",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 216,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV3Target",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 220,
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
                    Name = "CV1WindupHIn",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 224,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "CV2WindupHIn",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 225,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "CV3WindupHIn",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 226,
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
                    Name = "CV1WindupLIn",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 227,
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
                    Name = "CV2WindupLIn",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 228,
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
                    Name = "CV3WindupLIn",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 229,
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
                    Name = "GainEUSpan",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 230,
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
                    Name = "CV1PV1ProcessGainSign",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 231,
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
                    Name = "CV2PV1ProcessGainSign",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 232,
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
                    Name = "CV3PV1ProcessGainSign",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 233,
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
                    Name = "CV1PV2ProcessGainSign",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 234,
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
                    Name = "CV2PV2ProcessGainSign",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 235,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "CV3PV2ProcessGainSign",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 236,
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
                    Name = "ProcessType",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 240,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "CV1PV1ModelGain",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 244,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV2PV1ModelGain",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 248,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV3PV1ModelGain",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 252,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV1PV2ModelGain",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 256,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV2PV2ModelGain",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 260,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV3PV2ModelGain",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 264,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV1PV1ModelTC",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 268,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV2PV1ModelTC",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 272,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV3PV1ModelTC",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 276,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV1PV2ModelTC",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 280,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV2PV2ModelTC",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 284,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV3PV2ModelTC",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 288,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV1PV1ModelDT",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 292,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV2PV1ModelDT",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 296,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV3PV1ModelDT",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 300,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV1PV2ModelDT",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 304,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV2PV2ModelDT",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 308,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV3PV2ModelDT",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 312,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV1PV1RespTC",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 316,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV2PV1RespTC",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 320,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV3PV1RespTC",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 324,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV1PV2RespTC",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 328,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV2PV2RespTC",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 332,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "CV3PV2RespTC",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 336,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "PV1Act1stCV",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 340,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "PV1Act2ndCV",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 344,
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
                    Name = "PV1Act3rdCV",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 348,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "PV2Act1stCV",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 352,
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
                    Name = "PV2Act2ndCV",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 356,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "PV2Act3rdCV",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 360,
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
                    Name = "TargetCV",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 364,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
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
                    Name = "TargetRespTC",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 368,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
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
                    Name = "PVTracking",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 372,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ManualAfterInit",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 373,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ProgProgReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 374,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ProgOperReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 375,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ProgCV1AutoReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 376,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ProgCV2AutoReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 377,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
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
                    Name = "ProgCV3AutoReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 378,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 115,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ProgCV1ManualReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 379,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 116,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ProgCV2ManualReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 380,
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
                    Name = "ProgCV3ManualReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 381,
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
                    Name = "ProgCV1OverrideReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 382,
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
                    Name = "ProgCV2OverrideReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 383,
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
                    Name = "ProgCV3OverrideReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 384,
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
                    Name = "ProgCV1HandReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 385,
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
                    Name = "ProgCV2HandReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 386,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 123,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ProgCV3HandReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 387,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 124,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OperProgReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 388,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 125,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OperOperReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 389,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 126,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OperCV1AutoReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 390,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 127,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OperCV2AutoReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 391,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 128,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OperCV3AutoReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 392,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 129,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OperCV1ManualReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 393,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 130,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OperCV2ManualReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 394,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 131,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OperCV3ManualReq",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 395,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 132,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ProgValueReset",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 396,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 133,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "TimingMode",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 400,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 134,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "OversampleDT",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 404,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 135,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "RTSTime",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 408,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 136,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "RTSTimeStamp",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 412,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 137,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV1TuneLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 416,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 138,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV2TuneLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 420,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 139,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV1AtuneTimeLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 424,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 140,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV2AtuneTimeLimit",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 428,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 141,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV1NoiseLevel",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 432,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 142,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV2NoiseLevel",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 436,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 143,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1StepSize",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 440,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 144,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2StepSize",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 444,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 145,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3StepSize",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 448,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 146,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV1ResponseSpeed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 452,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 147,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV1ResponseSpeed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 456,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 148,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV1ResponseSpeed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 460,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 149,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV2ResponseSpeed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 464,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 150,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV2ResponseSpeed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 468,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 151,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV2ResponseSpeed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 472,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 152,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV1ModelInit",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 476,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 153,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV1ModelInit",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 477,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 154,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV1ModelInit",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 478,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 155,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV2ModelInit",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 479,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 156,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV2ModelInit",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 480,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 157,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV2ModelInit",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 481,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 158,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV1Factor",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 484,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 159,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV2Factor",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 488,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 160,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1Start",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 492,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 161,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2Start",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 493,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 162,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3Start",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 494,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 163,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1UseModel",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 495,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 164,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1UseModel",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 496,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 165,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1UseModel",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 497,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 166,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2UseModel",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 498,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 167,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2UseModel",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 499,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 168,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2UseModel",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 500,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 169,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1Abort",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 501,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 170,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2Abort",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 502,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 171,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3Abort",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 503,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 172,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "EnableOut",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 504,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 173,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1EU",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 508,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 174,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2EU",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 512,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 175,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3EU",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 516,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 176,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 520,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 177,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 524,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 178,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 528,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 179,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1Initializing",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 532,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 180,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2Initializing",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 533,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 181,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3Initializing",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 534,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 182,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1HAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 535,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 183,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2HAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 536,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 184,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3HAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 537,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 185,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1LAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 538,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 186,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2LAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 539,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 187,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3LAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 540,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 188,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1ROCPosAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 541,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 189,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2ROCPosAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 542,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 190,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3ROCPosAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 543,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 191,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1ROCNegAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 544,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 192,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2ROCNegAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 545,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 193,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3ROCNegAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 546,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 194,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SP1",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 548,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 195,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SP2",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 552,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 196,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SP1Percent",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 556,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 197,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SP2Percent",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 560,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 198,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SP1HAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 564,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 199,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SP2HAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 565,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 200,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SP1LAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 566,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 201,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SP2LAlarm",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 567,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 202,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV1Percent",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 568,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 203,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV2Percent",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 572,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 204,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "E1",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 576,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 205,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "E2",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 580,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 206,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "E1Percent",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 584,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 207,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "E2Percent",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 588,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 208,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1WindupHOut",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 592,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 209,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2WindupHOut",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 593,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 210,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3WindupHOut",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 594,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 211,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1WindupLOut",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 595,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 212,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2WindupLOut",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 596,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 213,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3WindupLOut",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 597,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 214,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "ProgOper",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 598,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 215,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1Auto",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 599,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 216,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2Auto",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 600,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 217,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3Auto",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 601,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 218,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1Manual",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 602,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 219,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2Manual",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 603,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 220,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3Manual",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 604,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 221,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1Override",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 605,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 222,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2Override",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 606,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 223,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3Override",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 607,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 224,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1Hand",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 608,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 225,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2Hand",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 609,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 226,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3Hand",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 610,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 227,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DeltaT",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 612,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 228,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1StepSizeUsed",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 616,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 229,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2StepSizeUsed",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 620,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 230,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3StepSizeUsed",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 624,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 231,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV1GainTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 628,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 232,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV1GainTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 632,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 233,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV1GainTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 636,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 234,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV2GainTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 640,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 235,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV2GainTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 644,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 236,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV2GainTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 648,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 237,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV1TCTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 652,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 238,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV1TCTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 656,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 239,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV1TCTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 660,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 240,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV2TCTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 664,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 241,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV2TCTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 668,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 242,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV2TCTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 672,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 243,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV1DTTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 676,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 244,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV1DTTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 680,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 245,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV1DTTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 684,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 246,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV2DTTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 688,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 247,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV2DTTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 692,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 248,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV2DTTuned",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 696,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 249,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV1RespTCTunedS",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 700,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 250,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV1RespTCTunedS",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 704,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 251,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV1RespTCTunedS",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 708,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 252,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV2RespTCTunedS",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 712,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 253,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV2RespTCTunedS",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 716,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 254,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV2RespTCTunedS",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 720,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 255,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV1RespTCTunedM",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 724,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 256,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV1RespTCTunedM",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 728,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 257,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV1RespTCTunedM",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 732,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 258,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV2RespTCTunedM",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 736,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 259,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV2RespTCTunedM",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 740,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 260,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV2RespTCTunedM",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 744,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 261,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV1RespTCTunedF",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 748,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 262,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV1RespTCTunedF",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 752,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 263,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV1RespTCTunedF",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 756,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 264,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV2RespTCTunedF",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 760,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 265,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV2RespTCTunedF",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 764,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 266,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV2RespTCTunedF",
                    DisplayStyle = DisplayStyle.Float,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 768,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = REAL.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 267,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1On",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 772,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 268,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1On",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 773,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 269,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1On",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 774,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 270,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1Done",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 775,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 271,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1Done",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 776,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 272,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1Done",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 777,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 273,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1Aborted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 778,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 274,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1Aborted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 779,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 275,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1Aborted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 780,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 276,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2On",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 781,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 277,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2On",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 782,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 278,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2On",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 783,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 279,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2Done",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 784,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 280,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2Done",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 785,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 281,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2Done",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 786,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 282,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2Aborted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 787,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 283,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2Aborted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 788,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 284,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2Aborted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 789,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 285,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1Status",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 792,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 286,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1Status",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 796,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 287,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1Status",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 800,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 288,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2Status",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 804,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 289,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2Status",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 808,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 290,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2Status",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 812,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 291,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 792,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 286,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1OutOfLimit",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 792,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 286,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1ModeInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 792,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 286,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1WindupFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 792,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 286,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1StepSize0",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 792,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 286,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1LimitsFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 792,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 286,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1InitFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 792,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 286,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1EUSpanChanged",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 792,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 286,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1Changed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 792,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 286,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1Timeout",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 792,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 286,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV1NotSettled",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 792,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 286,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 796,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 287,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1OutOfLimit",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 796,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 287,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1ModeInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 796,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 287,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1WindupFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 796,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 287,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1StepSize0",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 796,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 287,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1LimitsFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 796,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 287,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1InitFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 796,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 287,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1EUSpanChanged",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 796,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 287,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1Changed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 796,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 287,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1Timeout",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 796,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 287,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV1NotSettled",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 796,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 287,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 800,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 288,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1OutOfLimit",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 800,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 288,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1ModeInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 800,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 288,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1WindupFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 800,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 288,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1StepSize0",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 800,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 288,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1LimitsFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 800,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 288,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1InitFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 800,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 288,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1EUSpanChanged",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 800,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 288,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1Changed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 800,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 288,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1Timeout",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 800,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 288,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV1NotSettled",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 800,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 288,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 804,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 289,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2OutOfLimit",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 804,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 289,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2ModeInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 804,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 289,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2WindupFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 804,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 289,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2StepSize0",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 804,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 289,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2LimitsFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 804,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 289,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2InitFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 804,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 289,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2EUSpanChanged",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 804,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 289,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2Changed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 804,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 289,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2Timeout",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 804,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 289,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV1PV2NotSettled",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 804,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 289,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 808,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 290,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2OutOfLimit",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 808,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 290,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2ModeInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 808,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 290,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2WindupFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 808,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 290,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2StepSize0",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 808,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 290,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2LimitsFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 808,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 290,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2InitFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 808,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 290,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2EUSpanChanged",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 808,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 290,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2Changed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 808,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 290,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2Timeout",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 808,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 290,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV2PV2NotSettled",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 808,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 290,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2Fault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 812,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 291,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2OutOfLimit",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 812,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 291,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2ModeInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 812,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 291,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2WindupFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 812,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 291,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2StepSize0",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 812,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 291,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2LimitsFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 812,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 291,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2InitFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 812,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 291,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2EUSpanChanged",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 812,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 291,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2Changed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 812,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 291,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2Timeout",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 812,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 291,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "AtuneCV3PV2NotSettled",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 812,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 291,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Status1",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Status2",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 820,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 293,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Status3CV1",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Status3CV2",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "Status3CV3",
                    DisplayStyle = DisplayStyle.Hex,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 0,
                    IsBit = false,
                    DataType = DINT.Inst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "InstructFault",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV1Faulted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV2Faulted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV1SpanInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV2SpanInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SP1ProgInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SP2ProgInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SP1OperInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SP2OperInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SP1LimitsInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SP2LimitsInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "SampleTimeTooSmall",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 11,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV1FactorInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 12,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "PV2FactorInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 816,
                    BitOffset = 13,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 292,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "TimingModeInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 820,
                    BitOffset = 27,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 293,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "RTSMissed",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 820,
                    BitOffset = 28,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 293,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "RTSTimeInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 820,
                    BitOffset = 29,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 293,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "RTSTimeStampInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 820,
                    BitOffset = 30,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 293,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "DeltaTInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 820,
                    BitOffset = 31,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 293,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1Faulted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1HandFBFaulted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1ProgInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1OperInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1OverrideValueInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1EUSpanInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1LimitsInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1ROCLimitInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1HandFBInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV1ModelGainInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV2ModelGainInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV1ModelTCInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 11,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV2ModelTCInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 12,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV1ModelDTInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 13,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV2ModelDTInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 14,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV1RespTCInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 15,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1PV2RespTCInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 16,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV1TargetInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 824,
                    BitOffset = 17,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 294,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2Faulted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2HandFBFaulted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2ProgInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2OperInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2OverrideValueInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2EUSpanInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2LimitsInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2ROCLimitInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2HandFBInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV1ModelGainInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV2ModelGainInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV1ModelTCInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 11,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV2ModelTCInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 12,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV1ModelDTInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 13,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV2ModelDTInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 14,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV1RespTCInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 15,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2PV2RespTCInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 16,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV2TargetInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 828,
                    BitOffset = 17,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 295,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3Faulted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3HandFBFaulted",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 1,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3ProgInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 2,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3OperInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 3,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3OverrideValueInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 4,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3EUSpanInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 5,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3LimitsInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 6,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3ROCLimitInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 7,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3HandFBInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 8,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV1ModelGainInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 9,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV2ModelGainInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 10,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV1ModelTCInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 11,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV2ModelTCInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 12,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV1ModelDTInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 13,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV2ModelDTInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 14,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV1RespTCInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 15,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3PV2RespTCInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 16,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "CV3TargetInv",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 832,
                    BitOffset = 17,
                    IsBit = true,
                    DataType = BOOL.DInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 296,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
            {
                var member = new DataTypeMember
                {
                    Name = "FirstScan",
                    DisplayStyle = DisplayStyle.Decimal,
                    ExternalAccess = ExternalAccess.ReadWrite,
                    ByteOffset = 836,
                    BitOffset = 0,
                    IsBit = true,
                    DataType = BOOL.SInst,
                    Dim1 = 0,
                    Dim2 = 0,
                    Dim3 = 0,
                    FieldIndex = 297,
                    IsHidden = true,
                };
                _viewTypeMemberComponentCollection.AddDataTypeMember(member);
            }
        }
        public static readonly MMC Inst = new MMC();
        public override IField Create(JToken token)
        {
            var res = new MMCField(token);
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
        public override int BitSize => 6720;
        public override int ByteSize => 840;
        public const int size = 840;
    }
    public sealed class MMCField : PreDefinedField
    {
        public MMCField(JToken token)
        {
            Debug.Assert(token == null || token is JArray);
            var array = token as JArray;
            fields.Add(Tuple.Create(new Int8Field(array?[0]) as IField, 0));
            fields.Add(Tuple.Create(new RealField(array?[1]) as IField, 4));
            fields.Add(Tuple.Create(new RealField(array?[2]) as IField, 8));
            fields.Add(Tuple.Create(new Int8Field(array?[3]) as IField, 12));
            fields.Add(Tuple.Create(new Int8Field(array?[4]) as IField, 13));
            fields.Add(Tuple.Create(new RealField(array?[5]) as IField, 16));
            fields.Add(Tuple.Create(new RealField(array?[6]) as IField, 20));
            fields.Add(Tuple.Create(new RealField(array?[7]) as IField, 24));
            fields.Add(Tuple.Create(new RealField(array?[8]) as IField, 28));
            fields.Add(Tuple.Create(new RealField(array?[9]) as IField, 32));
            fields.Add(Tuple.Create(new RealField(array?[10]) as IField, 36));
            fields.Add(Tuple.Create(new RealField(array?[11]) as IField, 40));
            fields.Add(Tuple.Create(new RealField(array?[12]) as IField, 44));
            fields.Add(Tuple.Create(new RealField(array?[13]) as IField, 48));
            fields.Add(Tuple.Create(new RealField(array?[14]) as IField, 52));
            fields.Add(Tuple.Create(new RealField(array?[15]) as IField, 56));
            fields.Add(Tuple.Create(new RealField(array?[16]) as IField, 60));
            fields.Add(Tuple.Create(new Int8Field(array?[17]) as IField, 64));
            fields.Add(Tuple.Create(new Int8Field(array?[18]) as IField, 65));
            fields.Add(Tuple.Create(new Int8Field(array?[19]) as IField, 66));
            fields.Add(Tuple.Create(new Int8Field(array?[20]) as IField, 67));
            fields.Add(Tuple.Create(new Int8Field(array?[21]) as IField, 68));
            fields.Add(Tuple.Create(new Int8Field(array?[22]) as IField, 69));
            fields.Add(Tuple.Create(new RealField(array?[23]) as IField, 72));
            fields.Add(Tuple.Create(new RealField(array?[24]) as IField, 76));
            fields.Add(Tuple.Create(new RealField(array?[25]) as IField, 80));
            fields.Add(Tuple.Create(new RealField(array?[26]) as IField, 84));
            fields.Add(Tuple.Create(new RealField(array?[27]) as IField, 88));
            fields.Add(Tuple.Create(new RealField(array?[28]) as IField, 92));
            fields.Add(Tuple.Create(new RealField(array?[29]) as IField, 96));
            fields.Add(Tuple.Create(new RealField(array?[30]) as IField, 100));
            fields.Add(Tuple.Create(new RealField(array?[31]) as IField, 104));
            fields.Add(Tuple.Create(new RealField(array?[32]) as IField, 108));
            fields.Add(Tuple.Create(new RealField(array?[33]) as IField, 112));
            fields.Add(Tuple.Create(new RealField(array?[34]) as IField, 116));
            fields.Add(Tuple.Create(new Int8Field(array?[35]) as IField, 120));
            fields.Add(Tuple.Create(new RealField(array?[36]) as IField, 124));
            fields.Add(Tuple.Create(new RealField(array?[37]) as IField, 128));
            fields.Add(Tuple.Create(new RealField(array?[38]) as IField, 132));
            fields.Add(Tuple.Create(new RealField(array?[39]) as IField, 136));
            fields.Add(Tuple.Create(new RealField(array?[40]) as IField, 140));
            fields.Add(Tuple.Create(new RealField(array?[41]) as IField, 144));
            fields.Add(Tuple.Create(new RealField(array?[42]) as IField, 148));
            fields.Add(Tuple.Create(new RealField(array?[43]) as IField, 152));
            fields.Add(Tuple.Create(new RealField(array?[44]) as IField, 156));
            fields.Add(Tuple.Create(new RealField(array?[45]) as IField, 160));
            fields.Add(Tuple.Create(new RealField(array?[46]) as IField, 164));
            fields.Add(Tuple.Create(new RealField(array?[47]) as IField, 168));
            fields.Add(Tuple.Create(new RealField(array?[48]) as IField, 172));
            fields.Add(Tuple.Create(new RealField(array?[49]) as IField, 176));
            fields.Add(Tuple.Create(new RealField(array?[50]) as IField, 180));
            fields.Add(Tuple.Create(new RealField(array?[51]) as IField, 184));
            fields.Add(Tuple.Create(new RealField(array?[52]) as IField, 188));
            fields.Add(Tuple.Create(new RealField(array?[53]) as IField, 192));
            fields.Add(Tuple.Create(new RealField(array?[54]) as IField, 196));
            fields.Add(Tuple.Create(new RealField(array?[55]) as IField, 200));
            fields.Add(Tuple.Create(new RealField(array?[56]) as IField, 204));
            fields.Add(Tuple.Create(new Int8Field(array?[57]) as IField, 208));
            fields.Add(Tuple.Create(new Int8Field(array?[58]) as IField, 209));
            fields.Add(Tuple.Create(new Int8Field(array?[59]) as IField, 210));
            fields.Add(Tuple.Create(new RealField(array?[60]) as IField, 212));
            fields.Add(Tuple.Create(new RealField(array?[61]) as IField, 216));
            fields.Add(Tuple.Create(new RealField(array?[62]) as IField, 220));
            fields.Add(Tuple.Create(new Int8Field(array?[63]) as IField, 224));
            fields.Add(Tuple.Create(new Int8Field(array?[64]) as IField, 225));
            fields.Add(Tuple.Create(new Int8Field(array?[65]) as IField, 226));
            fields.Add(Tuple.Create(new Int8Field(array?[66]) as IField, 227));
            fields.Add(Tuple.Create(new Int8Field(array?[67]) as IField, 228));
            fields.Add(Tuple.Create(new Int8Field(array?[68]) as IField, 229));
            fields.Add(Tuple.Create(new Int8Field(array?[69]) as IField, 230));
            fields.Add(Tuple.Create(new Int8Field(array?[70]) as IField, 231));
            fields.Add(Tuple.Create(new Int8Field(array?[71]) as IField, 232));
            fields.Add(Tuple.Create(new Int8Field(array?[72]) as IField, 233));
            fields.Add(Tuple.Create(new Int8Field(array?[73]) as IField, 234));
            fields.Add(Tuple.Create(new Int8Field(array?[74]) as IField, 235));
            fields.Add(Tuple.Create(new Int8Field(array?[75]) as IField, 236));
            fields.Add(Tuple.Create(new Int32Field(array?[76]) as IField, 240));
            fields.Add(Tuple.Create(new RealField(array?[77]) as IField, 244));
            fields.Add(Tuple.Create(new RealField(array?[78]) as IField, 248));
            fields.Add(Tuple.Create(new RealField(array?[79]) as IField, 252));
            fields.Add(Tuple.Create(new RealField(array?[80]) as IField, 256));
            fields.Add(Tuple.Create(new RealField(array?[81]) as IField, 260));
            fields.Add(Tuple.Create(new RealField(array?[82]) as IField, 264));
            fields.Add(Tuple.Create(new RealField(array?[83]) as IField, 268));
            fields.Add(Tuple.Create(new RealField(array?[84]) as IField, 272));
            fields.Add(Tuple.Create(new RealField(array?[85]) as IField, 276));
            fields.Add(Tuple.Create(new RealField(array?[86]) as IField, 280));
            fields.Add(Tuple.Create(new RealField(array?[87]) as IField, 284));
            fields.Add(Tuple.Create(new RealField(array?[88]) as IField, 288));
            fields.Add(Tuple.Create(new RealField(array?[89]) as IField, 292));
            fields.Add(Tuple.Create(new RealField(array?[90]) as IField, 296));
            fields.Add(Tuple.Create(new RealField(array?[91]) as IField, 300));
            fields.Add(Tuple.Create(new RealField(array?[92]) as IField, 304));
            fields.Add(Tuple.Create(new RealField(array?[93]) as IField, 308));
            fields.Add(Tuple.Create(new RealField(array?[94]) as IField, 312));
            fields.Add(Tuple.Create(new RealField(array?[95]) as IField, 316));
            fields.Add(Tuple.Create(new RealField(array?[96]) as IField, 320));
            fields.Add(Tuple.Create(new RealField(array?[97]) as IField, 324));
            fields.Add(Tuple.Create(new RealField(array?[98]) as IField, 328));
            fields.Add(Tuple.Create(new RealField(array?[99]) as IField, 332));
            fields.Add(Tuple.Create(new RealField(array?[100]) as IField, 336));
            fields.Add(Tuple.Create(new Int32Field(array?[101]) as IField, 340));
            fields.Add(Tuple.Create(new Int32Field(array?[102]) as IField, 344));
            fields.Add(Tuple.Create(new Int32Field(array?[103]) as IField, 348));
            fields.Add(Tuple.Create(new Int32Field(array?[104]) as IField, 352));
            fields.Add(Tuple.Create(new Int32Field(array?[105]) as IField, 356));
            fields.Add(Tuple.Create(new Int32Field(array?[106]) as IField, 360));
            fields.Add(Tuple.Create(new Int32Field(array?[107]) as IField, 364));
            fields.Add(Tuple.Create(new RealField(array?[108]) as IField, 368));
            fields.Add(Tuple.Create(new Int8Field(array?[109]) as IField, 372));
            fields.Add(Tuple.Create(new Int8Field(array?[110]) as IField, 373));
            fields.Add(Tuple.Create(new Int8Field(array?[111]) as IField, 374));
            fields.Add(Tuple.Create(new Int8Field(array?[112]) as IField, 375));
            fields.Add(Tuple.Create(new Int8Field(array?[113]) as IField, 376));
            fields.Add(Tuple.Create(new Int8Field(array?[114]) as IField, 377));
            fields.Add(Tuple.Create(new Int8Field(array?[115]) as IField, 378));
            fields.Add(Tuple.Create(new Int8Field(array?[116]) as IField, 379));
            fields.Add(Tuple.Create(new Int8Field(array?[117]) as IField, 380));
            fields.Add(Tuple.Create(new Int8Field(array?[118]) as IField, 381));
            fields.Add(Tuple.Create(new Int8Field(array?[119]) as IField, 382));
            fields.Add(Tuple.Create(new Int8Field(array?[120]) as IField, 383));
            fields.Add(Tuple.Create(new Int8Field(array?[121]) as IField, 384));
            fields.Add(Tuple.Create(new Int8Field(array?[122]) as IField, 385));
            fields.Add(Tuple.Create(new Int8Field(array?[123]) as IField, 386));
            fields.Add(Tuple.Create(new Int8Field(array?[124]) as IField, 387));
            fields.Add(Tuple.Create(new Int8Field(array?[125]) as IField, 388));
            fields.Add(Tuple.Create(new Int8Field(array?[126]) as IField, 389));
            fields.Add(Tuple.Create(new Int8Field(array?[127]) as IField, 390));
            fields.Add(Tuple.Create(new Int8Field(array?[128]) as IField, 391));
            fields.Add(Tuple.Create(new Int8Field(array?[129]) as IField, 392));
            fields.Add(Tuple.Create(new Int8Field(array?[130]) as IField, 393));
            fields.Add(Tuple.Create(new Int8Field(array?[131]) as IField, 394));
            fields.Add(Tuple.Create(new Int8Field(array?[132]) as IField, 395));
            fields.Add(Tuple.Create(new Int8Field(array?[133]) as IField, 396));
            fields.Add(Tuple.Create(new Int32Field(array?[134]) as IField, 400));
            fields.Add(Tuple.Create(new RealField(array?[135]) as IField, 404));
            fields.Add(Tuple.Create(new Int32Field(array?[136]) as IField, 408));
            fields.Add(Tuple.Create(new Int32Field(array?[137]) as IField, 412));
            fields.Add(Tuple.Create(new RealField(array?[138]) as IField, 416));
            fields.Add(Tuple.Create(new RealField(array?[139]) as IField, 420));
            fields.Add(Tuple.Create(new RealField(array?[140]) as IField, 424));
            fields.Add(Tuple.Create(new RealField(array?[141]) as IField, 428));
            fields.Add(Tuple.Create(new Int32Field(array?[142]) as IField, 432));
            fields.Add(Tuple.Create(new Int32Field(array?[143]) as IField, 436));
            fields.Add(Tuple.Create(new RealField(array?[144]) as IField, 440));
            fields.Add(Tuple.Create(new RealField(array?[145]) as IField, 444));
            fields.Add(Tuple.Create(new RealField(array?[146]) as IField, 448));
            fields.Add(Tuple.Create(new Int32Field(array?[147]) as IField, 452));
            fields.Add(Tuple.Create(new Int32Field(array?[148]) as IField, 456));
            fields.Add(Tuple.Create(new Int32Field(array?[149]) as IField, 460));
            fields.Add(Tuple.Create(new Int32Field(array?[150]) as IField, 464));
            fields.Add(Tuple.Create(new Int32Field(array?[151]) as IField, 468));
            fields.Add(Tuple.Create(new Int32Field(array?[152]) as IField, 472));
            fields.Add(Tuple.Create(new Int8Field(array?[153]) as IField, 476));
            fields.Add(Tuple.Create(new Int8Field(array?[154]) as IField, 477));
            fields.Add(Tuple.Create(new Int8Field(array?[155]) as IField, 478));
            fields.Add(Tuple.Create(new Int8Field(array?[156]) as IField, 479));
            fields.Add(Tuple.Create(new Int8Field(array?[157]) as IField, 480));
            fields.Add(Tuple.Create(new Int8Field(array?[158]) as IField, 481));
            fields.Add(Tuple.Create(new RealField(array?[159]) as IField, 484));
            fields.Add(Tuple.Create(new RealField(array?[160]) as IField, 488));
            fields.Add(Tuple.Create(new Int8Field(array?[161]) as IField, 492));
            fields.Add(Tuple.Create(new Int8Field(array?[162]) as IField, 493));
            fields.Add(Tuple.Create(new Int8Field(array?[163]) as IField, 494));
            fields.Add(Tuple.Create(new Int8Field(array?[164]) as IField, 495));
            fields.Add(Tuple.Create(new Int8Field(array?[165]) as IField, 496));
            fields.Add(Tuple.Create(new Int8Field(array?[166]) as IField, 497));
            fields.Add(Tuple.Create(new Int8Field(array?[167]) as IField, 498));
            fields.Add(Tuple.Create(new Int8Field(array?[168]) as IField, 499));
            fields.Add(Tuple.Create(new Int8Field(array?[169]) as IField, 500));
            fields.Add(Tuple.Create(new Int8Field(array?[170]) as IField, 501));
            fields.Add(Tuple.Create(new Int8Field(array?[171]) as IField, 502));
            fields.Add(Tuple.Create(new Int8Field(array?[172]) as IField, 503));
            fields.Add(Tuple.Create(new Int8Field(array?[173]) as IField, 504));
            fields.Add(Tuple.Create(new RealField(array?[174]) as IField, 508));
            fields.Add(Tuple.Create(new RealField(array?[175]) as IField, 512));
            fields.Add(Tuple.Create(new RealField(array?[176]) as IField, 516));
            fields.Add(Tuple.Create(new RealField(array?[177]) as IField, 520));
            fields.Add(Tuple.Create(new RealField(array?[178]) as IField, 524));
            fields.Add(Tuple.Create(new RealField(array?[179]) as IField, 528));
            fields.Add(Tuple.Create(new Int8Field(array?[180]) as IField, 532));
            fields.Add(Tuple.Create(new Int8Field(array?[181]) as IField, 533));
            fields.Add(Tuple.Create(new Int8Field(array?[182]) as IField, 534));
            fields.Add(Tuple.Create(new Int8Field(array?[183]) as IField, 535));
            fields.Add(Tuple.Create(new Int8Field(array?[184]) as IField, 536));
            fields.Add(Tuple.Create(new Int8Field(array?[185]) as IField, 537));
            fields.Add(Tuple.Create(new Int8Field(array?[186]) as IField, 538));
            fields.Add(Tuple.Create(new Int8Field(array?[187]) as IField, 539));
            fields.Add(Tuple.Create(new Int8Field(array?[188]) as IField, 540));
            fields.Add(Tuple.Create(new Int8Field(array?[189]) as IField, 541));
            fields.Add(Tuple.Create(new Int8Field(array?[190]) as IField, 542));
            fields.Add(Tuple.Create(new Int8Field(array?[191]) as IField, 543));
            fields.Add(Tuple.Create(new Int8Field(array?[192]) as IField, 544));
            fields.Add(Tuple.Create(new Int8Field(array?[193]) as IField, 545));
            fields.Add(Tuple.Create(new Int8Field(array?[194]) as IField, 546));
            fields.Add(Tuple.Create(new RealField(array?[195]) as IField, 548));
            fields.Add(Tuple.Create(new RealField(array?[196]) as IField, 552));
            fields.Add(Tuple.Create(new RealField(array?[197]) as IField, 556));
            fields.Add(Tuple.Create(new RealField(array?[198]) as IField, 560));
            fields.Add(Tuple.Create(new Int8Field(array?[199]) as IField, 564));
            fields.Add(Tuple.Create(new Int8Field(array?[200]) as IField, 565));
            fields.Add(Tuple.Create(new Int8Field(array?[201]) as IField, 566));
            fields.Add(Tuple.Create(new Int8Field(array?[202]) as IField, 567));
            fields.Add(Tuple.Create(new RealField(array?[203]) as IField, 568));
            fields.Add(Tuple.Create(new RealField(array?[204]) as IField, 572));
            fields.Add(Tuple.Create(new RealField(array?[205]) as IField, 576));
            fields.Add(Tuple.Create(new RealField(array?[206]) as IField, 580));
            fields.Add(Tuple.Create(new RealField(array?[207]) as IField, 584));
            fields.Add(Tuple.Create(new RealField(array?[208]) as IField, 588));
            fields.Add(Tuple.Create(new Int8Field(array?[209]) as IField, 592));
            fields.Add(Tuple.Create(new Int8Field(array?[210]) as IField, 593));
            fields.Add(Tuple.Create(new Int8Field(array?[211]) as IField, 594));
            fields.Add(Tuple.Create(new Int8Field(array?[212]) as IField, 595));
            fields.Add(Tuple.Create(new Int8Field(array?[213]) as IField, 596));
            fields.Add(Tuple.Create(new Int8Field(array?[214]) as IField, 597));
            fields.Add(Tuple.Create(new Int8Field(array?[215]) as IField, 598));
            fields.Add(Tuple.Create(new Int8Field(array?[216]) as IField, 599));
            fields.Add(Tuple.Create(new Int8Field(array?[217]) as IField, 600));
            fields.Add(Tuple.Create(new Int8Field(array?[218]) as IField, 601));
            fields.Add(Tuple.Create(new Int8Field(array?[219]) as IField, 602));
            fields.Add(Tuple.Create(new Int8Field(array?[220]) as IField, 603));
            fields.Add(Tuple.Create(new Int8Field(array?[221]) as IField, 604));
            fields.Add(Tuple.Create(new Int8Field(array?[222]) as IField, 605));
            fields.Add(Tuple.Create(new Int8Field(array?[223]) as IField, 606));
            fields.Add(Tuple.Create(new Int8Field(array?[224]) as IField, 607));
            fields.Add(Tuple.Create(new Int8Field(array?[225]) as IField, 608));
            fields.Add(Tuple.Create(new Int8Field(array?[226]) as IField, 609));
            fields.Add(Tuple.Create(new Int8Field(array?[227]) as IField, 610));
            fields.Add(Tuple.Create(new RealField(array?[228]) as IField, 612));
            fields.Add(Tuple.Create(new RealField(array?[229]) as IField, 616));
            fields.Add(Tuple.Create(new RealField(array?[230]) as IField, 620));
            fields.Add(Tuple.Create(new RealField(array?[231]) as IField, 624));
            fields.Add(Tuple.Create(new RealField(array?[232]) as IField, 628));
            fields.Add(Tuple.Create(new RealField(array?[233]) as IField, 632));
            fields.Add(Tuple.Create(new RealField(array?[234]) as IField, 636));
            fields.Add(Tuple.Create(new RealField(array?[235]) as IField, 640));
            fields.Add(Tuple.Create(new RealField(array?[236]) as IField, 644));
            fields.Add(Tuple.Create(new RealField(array?[237]) as IField, 648));
            fields.Add(Tuple.Create(new RealField(array?[238]) as IField, 652));
            fields.Add(Tuple.Create(new RealField(array?[239]) as IField, 656));
            fields.Add(Tuple.Create(new RealField(array?[240]) as IField, 660));
            fields.Add(Tuple.Create(new RealField(array?[241]) as IField, 664));
            fields.Add(Tuple.Create(new RealField(array?[242]) as IField, 668));
            fields.Add(Tuple.Create(new RealField(array?[243]) as IField, 672));
            fields.Add(Tuple.Create(new RealField(array?[244]) as IField, 676));
            fields.Add(Tuple.Create(new RealField(array?[245]) as IField, 680));
            fields.Add(Tuple.Create(new RealField(array?[246]) as IField, 684));
            fields.Add(Tuple.Create(new RealField(array?[247]) as IField, 688));
            fields.Add(Tuple.Create(new RealField(array?[248]) as IField, 692));
            fields.Add(Tuple.Create(new RealField(array?[249]) as IField, 696));
            fields.Add(Tuple.Create(new RealField(array?[250]) as IField, 700));
            fields.Add(Tuple.Create(new RealField(array?[251]) as IField, 704));
            fields.Add(Tuple.Create(new RealField(array?[252]) as IField, 708));
            fields.Add(Tuple.Create(new RealField(array?[253]) as IField, 712));
            fields.Add(Tuple.Create(new RealField(array?[254]) as IField, 716));
            fields.Add(Tuple.Create(new RealField(array?[255]) as IField, 720));
            fields.Add(Tuple.Create(new RealField(array?[256]) as IField, 724));
            fields.Add(Tuple.Create(new RealField(array?[257]) as IField, 728));
            fields.Add(Tuple.Create(new RealField(array?[258]) as IField, 732));
            fields.Add(Tuple.Create(new RealField(array?[259]) as IField, 736));
            fields.Add(Tuple.Create(new RealField(array?[260]) as IField, 740));
            fields.Add(Tuple.Create(new RealField(array?[261]) as IField, 744));
            fields.Add(Tuple.Create(new RealField(array?[262]) as IField, 748));
            fields.Add(Tuple.Create(new RealField(array?[263]) as IField, 752));
            fields.Add(Tuple.Create(new RealField(array?[264]) as IField, 756));
            fields.Add(Tuple.Create(new RealField(array?[265]) as IField, 760));
            fields.Add(Tuple.Create(new RealField(array?[266]) as IField, 764));
            fields.Add(Tuple.Create(new RealField(array?[267]) as IField, 768));
            fields.Add(Tuple.Create(new Int8Field(array?[268]) as IField, 772));
            fields.Add(Tuple.Create(new Int8Field(array?[269]) as IField, 773));
            fields.Add(Tuple.Create(new Int8Field(array?[270]) as IField, 774));
            fields.Add(Tuple.Create(new Int8Field(array?[271]) as IField, 775));
            fields.Add(Tuple.Create(new Int8Field(array?[272]) as IField, 776));
            fields.Add(Tuple.Create(new Int8Field(array?[273]) as IField, 777));
            fields.Add(Tuple.Create(new Int8Field(array?[274]) as IField, 778));
            fields.Add(Tuple.Create(new Int8Field(array?[275]) as IField, 779));
            fields.Add(Tuple.Create(new Int8Field(array?[276]) as IField, 780));
            fields.Add(Tuple.Create(new Int8Field(array?[277]) as IField, 781));
            fields.Add(Tuple.Create(new Int8Field(array?[278]) as IField, 782));
            fields.Add(Tuple.Create(new Int8Field(array?[279]) as IField, 783));
            fields.Add(Tuple.Create(new Int8Field(array?[280]) as IField, 784));
            fields.Add(Tuple.Create(new Int8Field(array?[281]) as IField, 785));
            fields.Add(Tuple.Create(new Int8Field(array?[282]) as IField, 786));
            fields.Add(Tuple.Create(new Int8Field(array?[283]) as IField, 787));
            fields.Add(Tuple.Create(new Int8Field(array?[284]) as IField, 788));
            fields.Add(Tuple.Create(new Int8Field(array?[285]) as IField, 789));
            fields.Add(Tuple.Create(new Int32Field(array?[286]) as IField, 792));
            fields.Add(Tuple.Create(new Int32Field(array?[287]) as IField, 796));
            fields.Add(Tuple.Create(new Int32Field(array?[288]) as IField, 800));
            fields.Add(Tuple.Create(new Int32Field(array?[289]) as IField, 804));
            fields.Add(Tuple.Create(new Int32Field(array?[290]) as IField, 808));
            fields.Add(Tuple.Create(new Int32Field(array?[291]) as IField, 812));
            fields.Add(Tuple.Create(new Int32Field(array?[292]) as IField, 816));
            fields.Add(Tuple.Create(new Int32Field(array?[293]) as IField, 820));
            fields.Add(Tuple.Create(new Int32Field(array?[294]) as IField, 824));
            fields.Add(Tuple.Create(new Int32Field(array?[295]) as IField, 828));
            fields.Add(Tuple.Create(new Int32Field(array?[296]) as IField, 832));
            fields.Add(Tuple.Create(new Int8Field(array?[297]) as IField, 836));
        }
    }
}