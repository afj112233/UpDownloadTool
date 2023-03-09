using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;

namespace ICSStudio.SimpleServices.Instruction
{
    using PredefinedType;
    using ParamInfo = Tuple<Predicate<ASTExpr>, IDataType, ParameterType>;
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;

    public class Utils
    {
        public static ASTNode ParseExpr(string param)
        {
            return STASTGenVisitor.ParseExpr(param);
        }

        public static ASTNodeList ParseExprList(List<string> paramsString)
        {
            var list = new ASTNodeList();
            foreach (var param in paramsString)
            {
                list.AddNode(ParseExpr(param));
            }
            return list;
        }

        public static void GenClearBit(MacroAssembler assembler, IDataTypeMember member)
        {
            Debug.Assert(member.DataTypeInfo.DataType.IsBool, member.DataTypeInfo.DataType.ToString());
            Debug.Assert(member.DataTypeInfo.DataType.IsBool || member.DataTypeInfo.DataType.IsInteger);
            assembler.Dup();
            assembler.CLoadInteger(member.ByteOffset);
            assembler.PAdd();
            assembler.BiPush(0);
            assembler.StoreBit(GetBoolUnderlyingType(member.DataTypeInfo.DataType), member.BitOffset);
            //assembler.Store(member.DataTypeInfo.DataType, DINT.Inst, member.BitOffset);
        }

        public static void GenClearInt(MacroAssembler assembler, IDataTypeMember member)
        {
            Debug.Assert(member.DataTypeInfo.DataType.IsInteger, member.DataTypeInfo.DataType.ToString());
            assembler.Dup();
            assembler.CLoadInteger(member.ByteOffset);
            assembler.PAdd();
            assembler.BiPush(0);
            //assembler.Store(GetBoolUnderlyingType(member.DataTypeInfo.DataType), member.BitOffset);
            assembler.Store(member.DataTypeInfo.DataType, DINT.Inst);
        }

        public static void GenSetBit(MacroAssembler assembler, IDataTypeMember member)
        {
            Debug.Assert(member.DataTypeInfo.DataType.IsBool);
            Debug.Assert(member.DataTypeInfo.DataType.IsBool || member.DataTypeInfo.DataType.IsInteger);
            assembler.Dup();
            assembler.CLoadInteger(member.ByteOffset);
            assembler.PAdd();
            assembler.BiPush(1);
            assembler.StoreBit(GetBoolUnderlyingType(member.DataTypeInfo.DataType), member.BitOffset);
            // assembler.Store(member.DataTypeInfo.DataType, DINT.Inst, member.BitOffset);
        }

        public static void GenCopAtoB(MacroAssembler assembler, IDataTypeMember dataTypeMemberA, IDataTypeMember dataTypeMemberB)
        {
            Debug.Assert(dataTypeMemberA.DataTypeInfo.DataType.IsInteger);
            Debug.Assert(dataTypeMemberB.DataTypeInfo.DataType.IsInteger);
            assembler.Dup();
            assembler.Dup();

            assembler.CLoadInteger(dataTypeMemberA.ByteOffset);
            assembler.PAdd();
            assembler.Load(dataTypeMemberA.DataTypeInfo.DataType);
            assembler.Swap();

            assembler.CLoadInteger(dataTypeMemberB.ByteOffset);
            assembler.PAdd();
            assembler.Swap();
            assembler.Store(dataTypeMemberB.DataTypeInfo.DataType, dataTypeMemberA.DataTypeInfo.DataType);
        }

        public static void Store(CodeGenVisitor gen, IDataTypeMember member, ASTExpr expr)
        {

            gen.masm().Dup();
            gen.masm().CLoadInteger(member.ByteOffset);
            gen.masm().PAdd();
            if (member.DataTypeInfo.DataType.IsBool)
            {
                gen.masm().CLoadInteger(member.BitOffset);
            }
            expr.Accept(gen);
            gen.masm().Store(member.DataTypeInfo.DataType, expr.type);
        }

        public static void Store(CodeGenVisitor gen, ASTNameAddr addr, IDataTypeMember member)
        {
            //Debug.Assert(!member.DataTypeInfo.DataType.IsBool);
            Debug.Assert(addr != null);
            gen.masm().Dup();
            gen.masm().CLoadInteger(member.ByteOffset);
            gen.masm().PAdd();
            if (member.DataTypeInfo.DataType.IsBool)
            {
                gen.masm().CLoadInteger(member.BitOffset);

            }
            gen.masm().Load(member.DataTypeInfo.DataType);

            addr?.Accept(gen);
            if (member.DataTypeInfo.DataType.IsBool)
            {
                gen.masm().SwapX1();

            }
            else
            {
                gen.masm().Swap();
            }
            gen.masm().Store(addr?.ref_type.type, member.DataTypeInfo.DataType);// member.DataTypeInfo.DataType, expr.type);
        }

        public static void StoreEnableIn(CodeGenVisitor gen, IDataTypeMember member)
        {
            Debug.Assert(member.DataTypeInfo.DataType.IsBool);

            gen.masm().CLoadInteger(member.ByteOffset);
            gen.masm().PAdd();
            gen.masm().Swap();
            gen.masm().StoreBit(GetBoolUnderlyingType(member.DataTypeInfo.DataType), member.BitOffset);

            //gen.masm().Store(member.DataTypeInfo.DataType, DINT.Inst, member.BitOffset);
        }

        public static void CheckRefType(ASTNode node, DataType.DataType tp)
        {
            var addr = node as ASTNameAddr;
            Debug.Assert(addr != null && (addr.type is RefType) && (addr.type as RefType)?.type == tp);
        }

        /*
        public static void GenSTUnaryOp(ASTNodeList paramList, string name)
        {
            Debug.Assert(paramList.Count() == 1);
            var tp = (paramList.nodes[0] as ASTExpr).type;
            paramList.nodes[0].Accept(gen);
            MathUtils.ABS(gen, tp);
        }


        */

        public static void ThrowNotImplemented(string msg)
        {
            Console.WriteLine($"ThrowNotImplemented:{msg}");
            //throw new NotImplementedException(msg);
        }

        private static MacroAssembler.PrimitiveType GetPrimitiveType(IDataType tp)
        {
            if (tp is SINT)
                return MacroAssembler.PrimitiveType.SINT;
            if (tp is INT)
                return MacroAssembler.PrimitiveType.INT;
            if (tp is DINT)
                return MacroAssembler.PrimitiveType.DINT;
            if (tp is LINT)
                return MacroAssembler.PrimitiveType.LINT;
            if (tp is REAL)
                return MacroAssembler.PrimitiveType.REAL;
            if (tp is LREAL)
                return MacroAssembler.PrimitiveType.LREAL;
            Debug.Assert(false);
            return MacroAssembler.PrimitiveType.DINT;
        }

        public static MacroAssembler.PrimitiveType GetBoolUnderlyingType(IDataType tp)
        {
            Debug.Assert(tp is BOOL);
            return GetPrimitiveType((tp as BOOL)?.RefDataType);

        }

        #region 枚举处理

        public static Tuple<List<string>, Type> GetInstrEnumInfo(string instr, int pos, string interrelated)
        {
            if (string.IsNullOrEmpty(instr))
                return null;

            if (instr.Equals("MAM", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAM.GetInstrEnumType(pos);
            if (instr.Equals("SFP", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SFP.GetInstrEnumType(pos);
            if (instr.Equals("PXRQ", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.PXRQ.GetInstrEnumType(pos);
            if (instr.Equals("POVR", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.POVR.GetInstrEnumType(pos);
            if (instr.Equals("SCMD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SCMD.GetInstrEnumType(pos);
            if (instr.Equals("SOVR", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SOVR.GetInstrEnumType(pos);
            if (instr.Equals("GSV", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.GSV.GetInstrEnumType(pos, interrelated);
            if (instr.Equals("SSV", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SSV.GetInstrEnumType(pos, interrelated);
            if (instr.Equals("MDO", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDO.GetInstrEnumType(pos);
            if (instr.Equals("MDS", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDS.GetInstrEnumType(pos);
            if (instr.Equals("MAS", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAS.GetInstrEnumType(pos);
            if (instr.Equals("MAJ", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAJ.GetInstrEnumType(pos);
            if (instr.Equals("MAG", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAG.GetInstrEnumType(pos);
            if (instr.Equals("MCD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCD.GetInstrEnumType(pos);
            if (instr.Equals("MRP", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MRP.GetInstrEnumType(pos);
            if (instr.Equals("MAPC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAPC.GetInstrEnumType(pos);
            if (instr.Equals("MATC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MATC.GetInstrEnumType(pos);
            if (instr.Equals("MDAC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDAC.GetInstrEnumType(pos);
            if (instr.Equals("MGS", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MGS.GetInstrEnumType(pos);
            if (instr.Equals("MAW", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAW.GetInstrEnumType(pos);
            if (instr.Equals("MAR", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAR.GetInstrEnumType(pos);
            if (instr.Equals("MAOC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAOC.GetInstrEnumType(pos);
            if (instr.Equals("MDOC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDOC.GetInstrEnumType(pos);
            if (instr.Equals("MAHD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAHD.GetInstrEnumType(pos);
            if (instr.Equals("MRHD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MRHD.GetInstrEnumType(pos);
            if (instr.Equals("MCS", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCS.GetInstrEnumType(pos);
            if (instr.Equals("MCLM", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCLM.GetInstrEnumType(pos);
            if (instr.Equals("MCCM", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCCM.GetInstrEnumType(pos);
            if (instr.Equals("MCCD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCCD.GetInstrEnumType(pos);
            if (instr.Equals("MCTP", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCTP.GetInstrEnumType(pos);
            if (instr.Equals("MDCC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDCC.GetInstrEnumType(pos);
            if (instr.Equals("SWPB", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SWPB.GetInstrEnumType(pos);
            if (instr.Equals("FAL", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.FAL.GetInstrEnumType(pos);
            if (instr.Equals("FSC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.FSC.GetInstrEnumType(pos);
            if (instr.Equals("MDR", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDR.GetInstrEnumType(pos);
            return null;
        }

        public static List<string> GetInstrEnum(string instr, int pos, string interrelated)
        {
            if (string.IsNullOrEmpty(instr))
                return null;
            List<string> enums = GetInstrEnumInfo(instr, pos, interrelated)?.Item1;
            if (enums != null)
                for (int i = 0; i < enums.Count; i++)
                {
                    enums[i] = enums[i].Replace(" ", "");
                }

            return enums;
        }

        public static List<string> GetInstrEnumValues(string instr, int pos, string interrelated)
        {
            if (string.IsNullOrEmpty(instr))
                return null;

            if (instr.Equals("MAM", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAM.GetRLLInstrEnumNames(pos);
            if (instr.Equals("SFP", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SFP.GetRLLInstrEnumNames(pos);
            if (instr.Equals("PXRQ", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.PXRQ.GetRLLInstrEnumNames(pos);
            if (instr.Equals("POVR", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.POVR.GetRLLInstrEnumNames(pos);
            if (instr.Equals("SCMD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SCMD.GetRLLInstrEnumNames(pos);
            if (instr.Equals("SOVR", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SOVR.GetRLLInstrEnumNames(pos);
            if (instr.Equals("GSV", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.GSV.GetRLLInstrEnumNames(pos, interrelated);
            if (instr.Equals("SSV", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SSV.GetRLLInstrEnumNames(pos, interrelated);
            if (instr.Equals("MDO", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDO.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MDS", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDS.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MAS", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAS.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MAJ", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAJ.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MAG", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAG.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MCD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCD.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MRP", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MRP.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MAPC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAPC.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MATC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MATC.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MDAC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDAC.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MGS", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MGS.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MAW", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAW.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MAR", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAR.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MAOC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAOC.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MDOC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDOC.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MAHD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MAHD.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MRHD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MRHD.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MCS", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCS.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MCLM", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCLM.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MCCM", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCCM.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MCCD", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCCD.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MCTP", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MCTP.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MDCC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDCC.GetRLLInstrEnumNames(pos);
            if (instr.Equals("SWPB", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.SWPB.GetRLLInstrEnumNames(pos);
            if (instr.Equals("FAL", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.FAL.GetRLLInstrEnumNames(pos);
            if (instr.Equals("FSC", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.FSC.GetRLLInstrEnumNames(pos);
            if (instr.Equals("MDR", StringComparison.OrdinalIgnoreCase))
                return AllFInstructions.MDR.GetRLLInstrEnumNames(pos);
            return null;
        }

        public static int ParseEnum(string instr, int pos, string enumName, string interrelated = null)
        {
            try
            {
                var enums = GetInstrEnum(instr, pos, interrelated);

                var matchedEnum = enums?.FirstOrDefault(p => p.Equals(enumName.Replace(" ", "").Replace("-", ""), StringComparison.OrdinalIgnoreCase));
                if (matchedEnum != null)
                    return enums.IndexOf(matchedEnum);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + ex.StackTrace);
            }

            return -1;
        }

        #endregion
    }
}
