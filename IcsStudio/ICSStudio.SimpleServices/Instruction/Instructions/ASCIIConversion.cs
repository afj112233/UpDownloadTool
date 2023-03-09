using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.SimpleServices.Instruction.Instructions
{
    using PredefinedType;
    using System.Diagnostics;
    using ParamInfo = Tuple<Predicate<ASTExpr>, IDataType, ParameterType>;
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;

    internal class DTOSInstruction : FixedRLLSTInstruction
    {
        public DTOSInstruction(ParamInfoList infos) : base("DTOS", infos)
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList.Accept(checker) as ASTNodeList;
            var source = paramList.nodes[0] as ASTExpr;
            if (source?.type == null)
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid kind of operand or argument.");
            }

            if (source.type.IsNumber)
            {
                if (source.type is ArrayType) throw new TypeCheckerException($"{Name},param 1:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
            }

            var dest = paramList.nodes[1] as ASTName;
            if (dest == null)
            {
                throw new TypeCheckerException($"{Name},param 2:Invalid kind of operand or argument.");
            }

            if (dest.type.FamilyType == FamilyType.StringFamily)
            {
                if (dest.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 2:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 2:Invalid data type.Argument must match parameter data type.");
            }
            return PrepareFixedParameters(res);
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST || context == Context.RLL);
            paramList.nodes[0].Accept(gen);
            paramList.nodes[1].Accept(gen);
            var type = ((paramList.nodes[1] as ASTNameAddr).type as RefType).type;
            Debug.Assert(type.FamilyType==FamilyType.StringFamily);
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
           
            gen.masm().CallName("DTOS", 3);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source",
                new ExpectType(SINT.Inst, INT.Inst, DINT.Inst, REAL.Inst));
            var param2 = new Tuple<string, IDataType>("Dest",STRING.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    internal class STODInstruction : FixedRLLSTInstruction
    {
        public STODInstruction(ParamInfoList infos) : base("STOD", infos)
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

            var dest = paramList.nodes[1] as ASTName;
            if (dest == null)
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid kind of operand or argument.");
            }

            if (dest.type.IsInteger)
            {
                if (dest.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 1:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
            }
            return PrepareFixedParameters(res);
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST || context == Context.RLL);
            paramList.nodes[0].Accept(gen);
            var addr = (paramList.nodes[0] as ASTNameAddr);
            Debug.Assert(addr != null);

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
            //var SIZE = (((paramList.nodes[0] as ASTNameAddr).type as RefType).type as STRING).SIZE;
            //gen.masm().CLoadInteger(SIZE);
            gen.masm().CallName("STOD", 2);

            paramList.nodes[1].Accept(gen);
            gen.masm().Swap();
            gen.masm().Store((paramList.nodes[1] as ASTNameAddr).ref_type.type, DINT.Inst);

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source",STRING.Inst);
            var param2 =
                new Tuple<string, IDataType>("Dest", new ExpectType(SINT.Inst, INT.Inst, DINT.Inst));
            return new List<Tuple<string, IDataType>>() {param1, param2,};
        }
    }

    internal class RTOSInstruction : FixedRLLSTInstruction
    {
        public RTOSInstruction(ParamInfoList infos) : base("RTOS", infos)
        {

        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            var res = paramList.Accept(checker) as ASTNodeList;
            var source = paramList.nodes[0] as ASTExpr;
            if (source?.type==null)
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid kind of operand or argument.");
            }

            if (source.type.IsNumber)
            {
                if (source.type is ArrayType) throw new TypeCheckerException($"{Name},param 1:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 1:Invalid data type.Argument must match parameter data type.");
            }

            var dest = paramList.nodes[1] as ASTName;
            if (dest == null)
            {
                throw new TypeCheckerException($"{Name},param 2:Invalid kind of operand or argument.");
            }

            if (dest.type.FamilyType == FamilyType.StringFamily)
            {
                if (dest.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 2:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 2:Invalid data type.Argument must match parameter data type.");
            }
            return PrepareFixedParameters(res);
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST || context == Context.RLL);
            paramList.nodes[0].Accept(gen);
            paramList.nodes[1].Accept(gen);
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
            //var SIZE = ((paramList.nodes[1] as ASTNameAddr).ref_type.type as STRING).SIZE;
            //gen.masm().CLoadInteger(SIZE);
            gen.masm().CallName("RTOS", 3);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source", REAL.Inst);
            var param2 = new Tuple<string, IDataType>("Dest", STRING.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    internal class STORInstruction : FixedRLLSTInstruction
    {
        public STORInstruction(ParamInfoList infos) : base("STOR", infos)
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

            var dest = paramList.nodes[1] as ASTName;
            if (dest == null)
            {
                throw new TypeCheckerException($"{Name},param 2:Invalid kind of operand or argument.");
            }

            if (dest.type.IsReal)
            {
                if (dest.Expr.type is ArrayType) throw new TypeCheckerException($"{Name},param 2:Missing reference to array element.");
            }
            else
            {
                throw new TypeCheckerException($"{Name},param 2:Invalid data type.Argument must match parameter data type.");
            }
            return PrepareFixedParameters(res);
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.ST || context == Context.RLL);
            paramList.nodes[0].Accept(gen);
            var addr = (paramList.nodes[0] as ASTNameAddr);
            Debug.Assert(addr != null);
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

            //var SIZE = (((paramList.nodes[0] as ASTNameAddr).type as RefType).type as STRING).SIZE;
            //gen.masm().CLoadInteger(SIZE);
            gen.masm().CallName("STOR", 2);

            paramList.nodes[1].Accept(gen);
            gen.masm().Swap();
            gen.masm().Store((paramList.nodes[1] as ASTNameAddr).ref_type.type, REAL.Inst);
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source", STRING.Inst);
            var param2 = new Tuple<string, IDataType>("Dest", REAL.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    internal class UPPERInstruction : FixedRLLSTInstruction
    {
        public UPPERInstruction(ParamInfoList infos) : base("UPPER", infos)
        {

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
            gen.masm().CallName("UPPER", 4);
            gen.masm().Pop();

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source", STRING.Inst);
            var param2 = new Tuple<string, IDataType>("Dest", STRING.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

    internal class LOWERInstruction : FixedRLLSTInstruction
    {
        public LOWERInstruction(ParamInfoList infos) : base("LOWER", infos)
        {

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

            gen.masm().CallName("LOWER", 4);
            gen.masm().Pop();
        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("Source", STRING.Inst);
            var param2 = new Tuple<string, IDataType>("Dest", STRING.Inst);
            return new List<Tuple<string, IDataType>>() {param1, param2};
        }
    }

}
