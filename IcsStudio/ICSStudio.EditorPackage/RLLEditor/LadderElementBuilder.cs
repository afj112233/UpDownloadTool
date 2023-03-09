using ICSStudio.Ladder.Graph;
using ICSStudio.SimpleServices.Compiler;

namespace ICSStudio.EditorPackage.RLLEditor
{
    internal class LadderElementBuilder : IASTBaseVisitor
    {
        private readonly IGraph _graph;

        public LadderElementBuilder(IGraph graph)
        {
            _graph = graph;
        }

        public override ASTNode VisitRLLRoutine(ASTRLLRoutine context)
        {
            foreach (var rung in context.list.nodes)
            {
                RungBuilder rungBuilder = new RungBuilder(_graph);
                rung.Accept(rungBuilder);
            }

            return context;
        }

        private class RungBuilder : IASTBaseVisitor
        {
            private readonly IGraph _graph;
            private readonly IRung _rung;

            public RungBuilder(IGraph graph)
            {
                _graph = graph;
                _rung = graph.CreateRung();
            }

            public override ASTNode VisitRLLSequence(ASTRLLSequence context)
            {
                foreach (var node in context.list.nodes)
                {
                    var instruction = node as ASTRLLInstruction;
                    if (instruction != null)
                    {
                        _graph.AddInstruction(_rung, instruction.name, instruction.param_list);
                    }
                    else if (node is ASTRLLParallel)
                    {
                        BranchBuilder branchBuilder = new BranchBuilder(_graph, _rung);
                        node.Accept(branchBuilder);
                    }
                }

                return context;
            }
        }

        private class BranchBuilder : IASTBaseVisitor
        {
            private readonly IGraph _graph;
            private readonly IBranch _branch;

            public BranchBuilder(IGraph graph, IRung rung)
            {
                _graph = graph;
                _branch = graph.AddBranch(rung);
            }

            public BranchBuilder(IGraph graph, IBranchLevel branchLevel)
            {
                _graph = graph;
                _branch = graph.AddBranch(branchLevel);
            }

            public override ASTNode VisitRLLParallel(ASTRLLParallel context)
            {
                int i = 0;
                foreach (var node in context.list.nodes)
                {
                    if (_branch.BranchLevels.Count == i)
                    {
                        _graph.AddBranchLevel(_branch);
                    }

                    BranchLevelBuilder levelBuilder =
                        new BranchLevelBuilder(_graph, _branch.BranchLevels[i]);
                    node.Accept(levelBuilder);

                    i++;
                }

                return context;
            }
        }

        private class BranchLevelBuilder : IASTBaseVisitor
        {
            private readonly IGraph _graph;
            private readonly IBranchLevel _branchLevel;

            public BranchLevelBuilder(IGraph graph, IBranchLevel branchLevel)
            {
                _graph = graph;
                _branchLevel = branchLevel;
            }

            public override ASTNode VisitRLLSequence(ASTRLLSequence context)
            {
                foreach (var node in context.list.nodes)
                {
                    var instruction = node as ASTRLLInstruction;
                    if (instruction != null)
                    {
                        _graph.AddInstruction(_branchLevel, instruction.name, instruction.param_list);
                    }
                    else if (node is ASTRLLParallel)
                    {
                        BranchBuilder branchBuilder = new BranchBuilder(_graph,_branchLevel);
                        node.Accept(branchBuilder);
                    }
                }

                return context;
            }

        }
    }
}
