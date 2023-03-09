using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.Utils;

namespace ICSStudio.SimpleServices.Instruction.Instructions
{
    using System.Diagnostics;
    using ParamInfo = Tuple<Predicate<ASTExpr>, IDataType, ParameterType>;
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;

    internal class FINDInstruction : FixedRLLSTInstruction
    {
        public FINDInstruction(ParamInfoList infos) : base("FIND", infos)
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList.Accept(checker) as ASTNodeList;

            var source = paramList.nodes[0] as ASTName;
            if (source == null)
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid kind of operand or argument.");
            }

            if (source.type.FamilyType == FamilyType.StringFamily)
            {
                if (source.Expr.type is ArrayType)
                    throw new TypeCheckerException($"{Name},param 1:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
            }

            var search = paramList.nodes[1] as ASTName;
            if (search == null)
            {
                throw new TypeCheckerException($"{Name},param 2:Invalid kind of operand or argument.");
            }

            if (search.type.FamilyType == FamilyType.StringFamily)
            {
                if (search.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 2:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 2:Invalid data type.Argument must match parameter data type.");
            }

            if (paramList.nodes[2] is ASTUnaryOp)
            {
                throw new TypeCheckerException($"{Name},param 3:Miss right parenthesis or invalid argument.");
            }
            var index = paramList.nodes[2] as ASTInteger;

            if (index == null)
            {
                var astIndex = paramList.nodes[2] as ASTName;

                if (astIndex == null)
                {
                    throw new TypeCheckerException($"{Name}, param 3:Unexpected.");
                }
                else
                {
                    if (astIndex.Expr.type is ArrayType || !astIndex.type.IsInteger)
                    {
                        throw new TypeCheckerException($"{Name} ,param 3:Invalid data type.");
                    }
                }
            }
            else
            {
                if (index.value <= 0)
                {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
                    throw new TypeCheckerException($"{Name} ,param 3:Immediate value out of range.");
                }
            }
            var result = paramList.nodes[3] as ASTName;

            if (result == null)
            {
                throw new TypeCheckerException($"{Name} ,param 4:Invalid kind of operand or argument.");
            }
            
            return PrepareFixedParameters(res);
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST || context == Context.RLL);
            paramList.nodes[0].Accept(gen);
            //gen.masm().CLoadInteger(((paramList.nodes[0] as ASTNameAddr).ref_type.type as STRING).SIZE);
            {
                var type = ((paramList.nodes[0] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }
            paramList.nodes[1].Accept(gen);
            //gen.masm().CLoadInteger(((paramList.nodes[1] as ASTNameAddr).ref_type.type as STRING).SIZE);
            {
                var type = ((paramList.nodes[1] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }

            paramList.nodes[2].Accept(gen);
            paramList.nodes[3].Accept(gen);
            gen.masm().CallName("FIND", 6);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source", STRING.Inst);
            var param2 = new Tuple<string, IDataType>("Search", STRING.Inst);
            var param3 =
                new Tuple<string, IDataType>("Start", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 = new Tuple<string, IDataType>("Result",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4};
        }
    }

    //prescan:The rung-condition-out is set to false.
    internal class INSERTInstruction : FixedRLLSTInstruction
    {
        public INSERTInstruction(ParamInfoList infos) : base("INSERT", infos)
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList.Accept(checker) as ASTNodeList;

            var sourceA = paramList.nodes[0] as ASTName;
            if (sourceA == null)
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid kind of operand or argument.");
            }

            if (sourceA.type.FamilyType == FamilyType.StringFamily)
            {
                if (sourceA.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 1:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
            }

            var sourceB = paramList.nodes[1] as ASTName;
            if (sourceB == null)
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid kind of operand or argument.");
            }

            if (sourceB.type.FamilyType == FamilyType.StringFamily)
            {
                if (sourceB.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 1:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
            }

            if (paramList.nodes[2] is ASTUnaryOp)
            {
                throw new TypeCheckerException($"{Name},param 3:Miss right parenthesis or invalid argument.");
            }
            var index = paramList.nodes[2] as ASTInteger;

            if (index == null)
            {
                var astIndex = paramList.nodes[2] as ASTName;

                if (astIndex == null)
                {
                    throw new TypeCheckerException($"{Name}, param 3:Unexpected.");
                }
                else
                {
                    if (astIndex.Expr.type is ArrayType || !astIndex.type.IsInteger)
                    {
                        throw new TypeCheckerException($"{Name} ,param 3:Invalid data type.");
                    }
                }
            }
            else
            {
                if (index.value <= 0)
                {
                    throw new TypeCheckerException($"{Name} ,param 3:Immediate value out of range.");
                }
            }

            var dest = paramList.nodes[3] as ASTName;
            if (dest == null)
            {
                throw new TypeCheckerException($"{Name},param 4:Invalid kind of operand or argument.");
            }

            if (dest.type.FamilyType == FamilyType.StringFamily)
            {
                if (dest.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 4:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 4:Invalid data type.Argument must match parameter data type.");
            }
            return PrepareFixedParameters(res);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {

        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST || context == Context.RLL);
            paramList.nodes[0].Accept(gen);
            //gen.masm().CLoadInteger(((paramList.nodes[0] as ASTNameAddr).ref_type.type as STRING).SIZE);
            {
                var type = ((paramList.nodes[0] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }
            paramList.nodes[1].Accept(gen);
            //gen.masm().CLoadInteger(((paramList.nodes[1] as ASTNameAddr).ref_type.type as STRING).SIZE);
            {
                var type = ((paramList.nodes[1] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }

            paramList.nodes[2].Accept(gen);
            paramList.nodes[3].Accept(gen);
            //gen.masm().CLoadInteger(((paramList.nodes[3] as ASTNameAddr).ref_type.type as STRING).SIZE);
            {
                var type = ((paramList.nodes[3] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }

            gen.masm().CallName("INSERT", 7);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source A", STRING.Inst);
            var param2 = new Tuple<string, IDataType>("Source B", STRING.Inst);
            var param3 =
                new Tuple<string, IDataType>("Start", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 = new Tuple<string, IDataType>("Dest", STRING.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4};
        }
    }

    internal class CONCATInstruction : FixedInstruction
    {
        public CONCATInstruction(ParamInfoList infos) : base("CONCAT", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST);
            paramList.nodes[0].Accept(gen);
            {
                var type = ((paramList.nodes[0] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }
            //gen.masm().CLoadInteger(((paramList.nodes[0] as ASTNameAddr).ref_type.type as STRING).SIZE);
            paramList.nodes[1].Accept(gen);
            {
                var type = ((paramList.nodes[1] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }
            //gen.masm().CLoadInteger(((paramList.nodes[1] as ASTNameAddr).ref_type.type as STRING).SIZE);
            paramList.nodes[2].Accept(gen);
            {
                var type = ((paramList.nodes[2] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }
            //gen.masm().CLoadInteger(((paramList.nodes[2] as ASTNameAddr).ref_type.type as STRING).SIZE);
            gen.masm().CallName("CONCAT", 6);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source A", STRING.Inst);
            var param2 = new Tuple<string, IDataType>("Source B", STRING.Inst);
            var param3 = new Tuple<string, IDataType>("Dest", STRING.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2, param3};
        }
    }

    internal class RCONCATInstruction : FixedInstruction
    {
        public RCONCATInstruction(ParamInfoList infos) : base("CONCAT", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);

            var exit = new MacroAssembler.Label();
            gen.masm().Dup();
            gen.masm().Dup();
            gen.masm().JeqL(exit);
            paramList.nodes[0].Accept(gen);
            {
                var type = ((paramList.nodes[0] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }
            //gen.masm().CLoadInteger(((paramList.nodes[0] as ASTNameAddr).ref_type.type as STRING).SIZE);
            paramList.nodes[1].Accept(gen);
            {
                var type = ((paramList.nodes[1] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }
            //gen.masm().CLoadInteger(((paramList.nodes[1] as ASTNameAddr).ref_type.type as STRING).SIZE);
            paramList.nodes[2].Accept(gen);
            {
                var type = ((paramList.nodes[2] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }
            //gen.masm().CLoadInteger(((paramList.nodes[2] as ASTNameAddr).ref_type.type as STRING).SIZE);
            gen.masm().CallName("CONCAT", 6);
            gen.masm().Pop();
            gen.masm().Bind(exit);
        }
    }

    internal class MIDInstruction : FixedRLLSTInstruction
    {
        public MIDInstruction(ParamInfoList infos) : base("MID", infos)
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList.Accept(checker) as ASTNodeList;
            var source = paramList.nodes[0] as ASTName;
            if (source == null)
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid kind of operand or argument.");
            }

            if (source.type.FamilyType == FamilyType.StringFamily)
            {
                if (source.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 1:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
            }

            if (!(paramList.nodes[1] as ASTExpr)?.type.IsNumber??false)
            {
                throw new TypeCheckerException($"{Name},param 2:Miss right parenthesis or invalid argument.");
            }
            

            var start = paramList.nodes[2] as ASTInteger;

            if (start == null)
            {
                var astIndex = paramList.nodes[2] as ASTName;

                if (astIndex == null)
                {
                    throw new TypeCheckerException($"{Name}, param 3:Unexpected.");
                }
                else
                {
                    if (astIndex.Expr.type is ArrayType || !astIndex.type.IsInteger)
                    {
                        throw new TypeCheckerException($"{Name} ,param 3:Invalid data type.");
                    }
                }
            }
            else
            {
                if (start.value <= 0)
                {
                    throw new TypeCheckerException($"{Name} ,param 3:Immediate value out of range.");
                }
            }

            var dest = paramList.nodes[3] as ASTName;
            if (dest == null)
            {
                throw new TypeCheckerException($"{Name},param 4:Invalid kind of operand or argument.");
            }

            if (dest.type.FamilyType == FamilyType.StringFamily)
            {
                if (dest.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 4:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 4:Invalid data type.Argument must match parameter data type.");
            }

            return PrepareFixedParameters(res);
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST || context == Context.RLL);
            paramList.nodes[0].Accept(gen);
            {
                var type = ((paramList.nodes[0] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }
            //gen.masm().CLoadInteger(((paramList.nodes[0] as ASTNameAddr).ref_type.type as STRING).SIZE);

            paramList.nodes[1].Accept(gen);
            paramList.nodes[2].Accept(gen);

            paramList.nodes[3].Accept(gen);
            {
                var type = ((paramList.nodes[3] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }
            //gen.masm().CLoadInteger(((paramList.nodes[3] as ASTNameAddr).ref_type.type as STRING).SIZE);
            gen.masm().CallName("MID", 6);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source", STRING.Inst);
            var param2 =
                new Tuple<string, IDataType>("Qty", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param3 =
                new Tuple<string, IDataType>("Start", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 = new Tuple<string, IDataType>("Dest", STRING.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4};
        }
    }

    internal class DELETEInstruction : FixedRLLSTInstruction
    {
        public DELETEInstruction(ParamInfoList infos) : base("DELETE", infos)
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList.Accept(checker) as ASTNodeList;
            var source = paramList.nodes[0] as ASTName;
            if (source == null)
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid kind of operand or argument.");
            }

            if (source.type.FamilyType == FamilyType.StringFamily)
            {
                if (source.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 1:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
            }

            var qty = paramList.nodes[1] as ASTInteger;

            if (qty == null)
            {
                var astIndex = paramList.nodes[1] as ASTName;

                if (astIndex == null)
                {
                    throw new TypeCheckerException($"{Name}, param 2:Unexpected.");
                }
                else
                {
                    if (astIndex.Expr.type is ArrayType || !astIndex.type.IsInteger)
                    {
                        throw new TypeCheckerException($"{Name} ,param 2:Invalid data type.");
                    }
                }
            }
            else
            {
                if (qty.value <= 0)
                {
                    throw new TypeCheckerException($"{Name} ,param 2:Immediate value out of range.");
                }
            }

            var start = paramList.nodes[2] as ASTInteger;

            if (start == null)
            {
                var astIndex = paramList.nodes[2] as ASTName;

                if (astIndex == null)
                {
                    throw new TypeCheckerException($"{Name}, param 3:Unexpected.");
                }
                else
                {
                    if (astIndex.Expr.type is ArrayType || !astIndex.type.IsInteger)
                    {
                        throw new TypeCheckerException($"{Name} ,param 3:Invalid data type.");
                    }
                }
            }
            else
            {
                if (start.value <= 0)
                {
                    throw new TypeCheckerException($"{Name} ,param 3:Immediate value out of range.");
                }
            }

            var dest = paramList.nodes[3] as ASTName;
            if (dest == null)
            {
                throw new TypeCheckerException($"{Name},param 4:Invalid kind of operand or argument.");
            }

            if (dest.type.FamilyType == FamilyType.StringFamily)
            {
                if (dest.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 4:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 4:Invalid data type.Argument must match parameter data type.");
            }
            return PrepareFixedParameters(res);
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST || context == Context.RLL);
            paramList.nodes[0].Accept(gen);
            {
                var type = ((paramList.nodes[0] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }
            //gen.masm().CLoadInteger(((paramList.nodes[0] as ASTNameAddr).ref_type.type as STRING).SIZE);

            paramList.nodes[1].Accept(gen);
            paramList.nodes[2].Accept(gen);

            paramList.nodes[3].Accept(gen);
            {
                var type = ((paramList.nodes[3] as ASTNameAddr).type as RefType).type;
                Debug.Assert(type.FamilyType == FamilyType.StringFamily);
                if (type is UserDefinedDataType)
                {
                    var SIZE = (type as UserDefinedDataType).TypeMembers["Data"].DataTypeInfo.Dim1;
                    gen.masm().CLoadInteger(SIZE);
                }
                else
                {
                    var SIZE = (type as STRING).SIZE;
                    gen.masm().CLoadInteger(SIZE);
                }
            }
            //gen.masm().CLoadInteger(((paramList.nodes[3] as ASTNameAddr).ref_type.type as STRING).SIZE);

            gen.masm().CallName("DELETE", 6);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source", STRING.Inst);
            var param2 =
                new Tuple<string, IDataType>("Qty", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param3 =
                new Tuple<string, IDataType>("Start", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            var param4 = new Tuple<string, IDataType>("Dest", STRING.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2, param3, param4};
        }
    }

}
