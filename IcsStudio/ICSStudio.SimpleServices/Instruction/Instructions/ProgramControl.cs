using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Interfaces.DataType;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.SimpleServices.PredefinedType;

namespace ICSStudio.SimpleServices.Instruction.Instructions
{
    using ParamInfo = Tuple<Predicate<ASTExpr>, IDataType, ParameterType>;
    using ParamInfoList = List<Tuple<Predicate<ASTExpr>, IDataType, ParameterType>>;

    internal class JSRInstruction : RLLSTInstruction
    {
        public JSRInstruction() : base("JSR")
        {
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return Utils.ParseExprList(parameters);
        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            return paramList;
        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            var name = paramList.nodes[0] as ASTName;
            Debug.Assert(name.id_list.Count() == 1);

            gen.masm().CallRoutine((name.id_list.nodes[0] as ASTNameItem).id + ".Logic", 0);
            gen.masm().Pop();
            //gen.masm().BiPush(0);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {

            var name = paramList.nodes[0] as ASTName;
            Debug.Assert(name.id_list.Count() == 1);

            gen.masm().CallRoutine((name.id_list.nodes[0] as ASTNameItem).id + ".Prescan", 0);
            gen.masm().Pop();
            // gen.masm().BiPush(0);

        }
    }

    internal class RETInstruction : RLLSTInstruction
    {
        public RETInstruction() : base("RET")
        {

        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            if (context == Context.RLL)
            {
                gen.masm().Pop();
                gen.masm().Pop();
            }

            gen.masm().Ret(0);
            if (context == Context.RLL)
            {
                gen.masm().stack_size += 2;
            }

            gen.masm().stack_size -= 1;
            //gen.masm().Pop();

            //gen.masm().BiPush(0);
        }
    }

    //The rung is set to false.

    //The controller executes all subroutines.To ensure that all rungs in the subroutine are prescanned, the controller ignores RET instructions(that is, RET instructions do not exit the subroutine).

    //Input and return parameters are not passed.

    //If the same subroutine is invoked multiple times, it will only be prescanned once.
    internal class SBRInstruction : FixedInstruction
    {
        public SBRInstruction(ParamInfoList infos) : base("SBR", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            gen.masm().BiPush(0);
        }
    }

    internal class TNDInstruction : RLLSTInstruction
    {
        public TNDInstruction(ParamInfoList infos) : base("TND")
        {

        }

        /*
            public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
            {
                int size = gen.masm().stack_size;
                for (int i = 0; i < size; ++i)
                {
                    gen.masm().Pop();
                }
                gen.masm().Ret();
                if (context == Context.RLL)
                {
                    gen.masm().stack_size += 2;
                }
            }
            */

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            var stack_size = gen.masm().stack_size;
            int size = gen.masm().stack_size;
            for (int i = 0; i < size; ++i)
            {
                gen.masm().Pop();
            }

            gen.masm().Ret();
            gen.masm().stack_size = stack_size;

        }

    }

    internal class UIDInstruction : FixedRLLSTInstruction
    {
        public UIDInstruction(ParamInfoList infos) : base("UID", infos)
        {

        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            gen.masm().CallName("UID", 0);
            gen.masm().Pop();
        }

    }

    internal class UIEInstruction : FixedRLLSTInstruction
    {
        public UIEInstruction(ParamInfoList infos) : base("UIE", infos)
        {

        }

        public override void LogicExec(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            gen.masm().CallName("UIE", 0);
            gen.masm().Pop();
        }
    }

    internal class SFRInstruction : FixedInstruction
    {
        public SFRInstruction(ParamInfoList infos) : base("SFR", infos)
        {

        }
    }

    internal class SFPInstruction : FixedInstruction
    {
        public SFPInstruction(ParamInfoList infos) : base("SFP", infos)
        {

        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            return AllFInstructions.SFP.ParseSTParameters(parameters);
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            return AllFInstructions.SFP.ParseRLLParameters(parameters);
        }
    }

    internal class EOTInstruction : FixedInstruction
    {
        public EOTInstruction(ParamInfoList infos) : base("EOT", infos)
        {

        }

        public override List<Tuple<string, IDataType>> GetParameterInfo()
        {
            var param1 = new Tuple<string, IDataType>("State Bit", BOOL.Inst);
            return new List<Tuple<string, IDataType>>() {param1};
        }
    }

    internal class EventInstruction : IXInstruction
    {
        public EventInstruction()
        {
            Name = "EVENT";
        }
        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            if (parameters.Count() != 1)
            {
                throw new Exception($"EventInstruction:parameters count should be 1:{parameters.Count()}");
            }

            return parameters;
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            if (parameters.Count() != 1)
            {
                throw new Exception($"EventInstruction:parameters count should be 1:{parameters.Count()}");
            }

            return Utils.ParseExprList(parameters);
        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            return paramList;
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            gen.masm().BiPush(0);
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {
            gen.masm().BiPush(0);

        }
    }

    internal class JMPInstruction : IXInstruction
    {
        public JMPInstruction(ParamInfoList infos)
        {
            Name = "JMP";
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            Debug.Assert(false);
            throw new NotImplementedException();
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            Debug.Assert(parameters.Count() == 1);
            var res = new ASTNodeList();
            res.AddNode(new ASTString(parameters[0]));
            return res;
        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            return paramList;
            //return paramList.Accept(checker) as ASTNodeList;
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            var label = gen.FindLabel((paramList.nodes[0] as ASTString).Name);
            gen.masm().Dup();
            var exit_label = new MacroAssembler.Label();
            gen.masm().JeqL(exit_label);
            var remaining = gen.masm().stack_size;
            for (int i = 0; i < remaining; ++i)
            {
                gen.masm().Pop();
            }

            gen.masm().stack_size = remaining;
            gen.masm().JmpL(label);
            gen.masm().Bind(exit_label);
            gen.masm().Dup();
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {

        }

    }

    internal class LBLInstruction : IXInstruction
    {
        public LBLInstruction(ParamInfoList infos)
        {
            Name = "LBL";
        }

        public override ASTNodeList ParseSTParameters(ASTNodeList parameters)
        {
            Debug.Assert(false);
            throw new NotImplementedException();
        }

        public override ASTNodeList ParseRLLParameters(List<string> parameters)
        {
            Debug.Assert(parameters.Count() == 1);
            var res = new ASTNodeList();
            res.AddNode(new ASTString(parameters[0]));
            return res;
        }

        public override ASTNodeList TypeCheck(TypeChecker checker, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            return paramList;
            //return paramList.Accept(checker) as ASTNodeList;
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            Debug.Assert(context == Context.RLL);
            var label = gen.FindLabel((paramList.nodes[0] as ASTString).Name);
            gen.masm().Bind(label, -2);
            gen.masm().Dup();
        }

        public override void Prescan(CodeGenVisitor gen, ASTNodeList paramList)
        {

        }
    }

    //prescan:no information
    internal class JXRInstruction : FixedInstruction
    {
        public JXRInstruction(ParamInfoList infos) : base("JXR", infos)
        {

        }
    }

    internal class AFIInstruction : FixedInstruction
    {
        public AFIInstruction(ParamInfoList infos) : base("AFI", infos)
        {
        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            gen.masm().BiPush(0);
        }
    }

    internal class NOPInstruction : FixedInstruction
    {
        public NOPInstruction(ParamInfoList infos) : base("NOP", infos)
        {

        }

        public override void Logic(CodeGenVisitor gen, ASTNodeList paramList, Context context = Context.ST)
        {
            gen.masm().Dup();
        }
    }

    internal class MCRInstruction : FixedInstruction
    {
        public MCRInstruction(ParamInfoList infos) : base("MCR", infos)
        {

        }
    }

}
