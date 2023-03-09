using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;

namespace ICSStudio.SimpleServices.CodeAnalysis
{
    public class RLLAoiReferenceHelper : IASTBaseVisitor
    {
        public RLLAoiReferenceHelper(RLLRoutine routine)
        {
            Routine = routine;
        }

        public RLLRoutine Routine { get; }

        public int RungIndex { get; set; }
        public int InstructionIndex { get; set; }

        public override ASTNode VisitRLLRoutine(ASTRLLRoutine context)
        {
            RungIndex = 0;

            foreach (var rung in context.list.nodes)
            {
                InstructionIndex = 0;

                RungAoiReferenceHelper rungBuilder = new RungAoiReferenceHelper(this);
                rung.Accept(rungBuilder);

                RungIndex++;
            }

            return context;
        }
    }

    public class RungAoiReferenceHelper : IASTBaseVisitor
    {
        private readonly RLLAoiReferenceHelper _helper;

        public RungAoiReferenceHelper(RLLAoiReferenceHelper helper)
        {
            _helper = helper;
        }

        public RLLRoutine Routine => _helper.Routine;

        public override ASTNode VisitRLLSequence(ASTRLLSequence context)
        {
            foreach (var node in context.list.nodes)
            {
                var instruction = node as ASTRLLInstruction;
                if (instruction != null)
                {
                    var aoiDefinitionCollection =
                        Routine.ParentController.AOIDefinitionCollection as AoiDefinitionCollection;
                    if (aoiDefinitionCollection != null)
                    {
                        var aoiDefinition = aoiDefinitionCollection.Find(instruction.name);
                        if (aoiDefinition != null)
                        {
                            aoiDefinition.AddReference(Routine, instruction,
                                _helper.RungIndex, _helper.InstructionIndex);
                        }
                    }

                    _helper.InstructionIndex += 1;
                }
                else if (node is ASTRLLParallel)
                {
                    BranchAoiReferenceHelper branchBuilder = new BranchAoiReferenceHelper(_helper);
                    node.Accept(branchBuilder);
                }
            }

            return context;
        }
    }

    public class BranchAoiReferenceHelper : IASTBaseVisitor
    {
        private readonly RLLAoiReferenceHelper _helper;

        public BranchAoiReferenceHelper(RLLAoiReferenceHelper helper)
        {
            _helper = helper;
        }

        public RLLRoutine Routine => _helper.Routine;

        public override ASTNode VisitRLLParallel(ASTRLLParallel context)
        {
            //int i = 0;
            foreach (var node in context.list.nodes)
            {
                BranchLevelAoiReferenceHelper levelHelper =
                    new BranchLevelAoiReferenceHelper(_helper);
                node.Accept(levelHelper);

                //i++;
            }

            return context;
        }
    }

    public class BranchLevelAoiReferenceHelper : IASTBaseVisitor
    {
        private readonly RLLAoiReferenceHelper _helper;

        public BranchLevelAoiReferenceHelper(RLLAoiReferenceHelper helper)
        {
            _helper = helper;
        }

        public RLLRoutine Routine => _helper.Routine;

        public override ASTNode VisitRLLSequence(ASTRLLSequence context)
        {
            foreach (var node in context.list.nodes)
            {
                var instruction = node as ASTRLLInstruction;
                if (instruction != null)
                {
                    var aoiDefinitionCollection =
                        Routine.ParentController.AOIDefinitionCollection as AoiDefinitionCollection;
                    if (aoiDefinitionCollection != null)
                    {
                        var aoiDefinition = aoiDefinitionCollection.Find(instruction.name);
                        if (aoiDefinition != null)
                        {
                            aoiDefinition.AddReference(Routine, instruction, _helper.RungIndex,
                                _helper.InstructionIndex);
                        }
                    }

                    _helper.InstructionIndex += 1;
                }
                else if (node is ASTRLLParallel)
                {
                    BranchAoiReferenceHelper branchHelper = new BranchAoiReferenceHelper(_helper);
                    node.Accept(branchHelper);
                }
            }

            return context;
        }
    }

    internal static class AoiDefinitionExtension
    {
        public static void AddReference(
            this AoiDefinition aoiDefinition,
            RLLRoutine routine,
            ASTRLLInstruction rllInstruction,
            int line, int column)
        {
            ASTNodeList paramList = ToAoiDataReferenceParamList(rllInstruction.param_list_ast);

            var dataReference =
                new AoiDataReference(aoiDefinition, routine,
                    new ASTInstr(rllInstruction.name, paramList)
                        { ContextStart = 0, ContextEnd = 0 },OnlineEditType.Original);

            dataReference.Line = line;
            dataReference.Column = column;

            if (routine.ParentCollection.ParentProgram is AoiDefinition)
            {
                dataReference.InnerAoiDefinition = routine.ParentCollection.ParentProgram as AoiDefinition;
                aoiDefinition.AddInnerReference(dataReference);
            }
            else
            {
                aoiDefinition.AddReference(dataReference);
            }
        }

        private static ASTNodeList ToAoiDataReferenceParamList(ASTNodeList astNodeList)
        {
            ASTNodeList list = new ASTNodeList();

            foreach (var node in astNodeList.nodes)
            {
                ASTNameAddr nameAddress = node as ASTNameAddr;
                if (nameAddress != null)
                {
                    list.AddNode(nameAddress.name);
                    continue;
                }

                ASTTypeConv typeConv = node as ASTTypeConv;
                if (typeConv != null)
                {
                    ASTNameValue nameValue = typeConv.expr as ASTNameValue;
                    list.AddNode(nameValue != null ? nameValue.name : typeConv.expr);

                    continue;
                }

                list.AddNode(node);
            }

            return list;
        }


    }
}
