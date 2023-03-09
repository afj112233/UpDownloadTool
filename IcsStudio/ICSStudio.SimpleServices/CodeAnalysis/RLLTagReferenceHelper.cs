using System;
using System.Collections.Generic;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.Utils.TagExpression;

namespace ICSStudio.SimpleServices.CodeAnalysis
{
    public class RLLTagReferenceHelper : IASTBaseVisitor
    {
        public RLLTagReferenceHelper(RLLRoutine routine)
        {
            Routine = routine;

            Tags = new List<ITag>();
        }

        public RLLRoutine Routine { get; }

        public IController ParentController => Routine.ParentController;

        public IProgramModule ParentProgram => Routine.ParentCollection.ParentProgram;


        public List<ITag> Tags { get; }

        public override ASTNode VisitRLLRoutine(ASTRLLRoutine context)
        {
            foreach (var rung in context.list.nodes)
            {
                rung.Accept(this);
            }

            return context;
        }

        public override ASTNode VisitRLLSequence(ASTRLLSequence context)
        {
            foreach (var node in context.list.nodes)
            {
                node.Accept(this);
            }

            return context;
        }

        public override ASTNode VisitRLLParallel(ASTRLLParallel context)
        {
            foreach (var node in context.list.nodes)
            {
                node.Accept(this);
            }

            return context;
        }

        public override ASTNode VisitRLLInstruction(ASTRLLInstruction context)
        {
            TagExpressionParser parser = new TagExpressionParser();

            foreach (string param in context.param_list)
            {
                try
                {
                    var tagExpression = parser.Parser(param);
                    if (tagExpression != null)
                    {
                        SimpleTagExpression simpleTagExpression = parser.GetSimpleTagExpression(tagExpression);

                        ITag tag;

                        if (!string.IsNullOrEmpty(simpleTagExpression.Scope))
                        {
                            IProgram program = Routine.ParentController.Programs[simpleTagExpression.Scope];
                            if (program != null)
                            {
                                tag = program.Tags[simpleTagExpression.TagName];
                                if (tag != null && !Tags.Contains(tag))
                                    Tags.Add(tag);
                            }

                            continue;
                        }

                        tag = ParentProgram.Tags[simpleTagExpression.TagName] ??
                              ParentController.Tags[simpleTagExpression.TagName];

                        if (tag != null && !Tags.Contains(tag))
                            Tags.Add(tag);

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return context;
        }
    }
}
