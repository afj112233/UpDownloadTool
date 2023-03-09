using System;
using System.Collections.Generic;
using System.Diagnostics;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;
using ICSStudio.UIInterfaces.Parser;
using ICSStudio.Utils.TagExpression;

namespace ICSStudio.ParserPackage
{
    internal class RLLInfoExtractor : IASTBaseVisitor
    {
        private readonly RLLRoutine _routine;
        private readonly Controller _controller;

        private int _rungIndex;
        private int _instructionIndexInRung;

        private readonly List<IInstructionSymbol> _instructions;
        private readonly List<IParameterSymbol> _parameters;

        private readonly TagExpressionParser _parser;

        public RLLInfoExtractor(RLLRoutine routine)
        {
            _routine = routine;
            _controller = routine?.ParentController as Controller;

            Debug.Assert(_controller != null);

            _instructions = new List<IInstructionSymbol>();
            _parameters = new List<IParameterSymbol>();

            _parser = new TagExpressionParser();
        }

        public IUnresolvedRoutine Parse(List<string> codeText)
        {
            _instructions.Clear();
            _parameters.Clear();


            UnresolvedRoutine result = new UnresolvedRoutine(_routine);

            Stopwatch stopwatch = Stopwatch.StartNew();

            ASTNode astRllRoutine =
                RLLGrammarParser.Parse(codeText, _routine.ParentController as Controller);

            astRllRoutine.Accept(this);

            stopwatch.Stop();
            result.ParseTime = stopwatch.ElapsedMilliseconds;

            result.Instructions = _instructions;
            result.Parameters = _parameters;
            //


            return result;
        }

        public override ASTNode VisitRLLRoutine(ASTRLLRoutine context)
        {
            _rungIndex = 0;

            foreach (var rung in context.list.nodes)
            {
                _instructionIndexInRung = 0;

                rung.Accept(this);

                _rungIndex++;
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
            // instruction
            InstructionSymbol instructionSymbol = new InstructionSymbol();

            instructionSymbol.Row = _rungIndex;
            instructionSymbol.Col = _instructionIndexInRung;
            instructionSymbol.Name = context.name;

            instructionSymbol.Instruction = context.instr;
            instructionSymbol.Parameters = new List<IParameterSymbol>();

            _instructions.Add(instructionSymbol);

            // param
            int paramIndex = 0;
            foreach (var param in context.param_list)
            {
                ParameterSymbol parameterSymbol = new ParameterSymbol(param, paramIndex, instructionSymbol);

                try
                {
                    var tagExpression = _parser.Parser(param);
                    if (tagExpression != null)
                    {
                        SimpleTagExpression simpleTagExpression = _parser.GetSimpleTagExpression(tagExpression);

                        ITag tag;

                        if (!string.IsNullOrEmpty(simpleTagExpression.Scope))
                        {
                            IProgram program = _controller.Programs[simpleTagExpression.Scope];
                            if (program != null)
                            {
                                tag = program.Tags[simpleTagExpression.TagName];
                                if (tag != null)
                                {
                                    parameterSymbol.IsOperand = true;
                                    parameterSymbol.Tag = tag;
                                    parameterSymbol.TagExpression = tagExpression;
                                }
                            }
                        }
                        else
                        {
                            tag = _routine?.ParentCollection?.ParentProgram?.Tags[simpleTagExpression.TagName] ??
                                  _controller.Tags[simpleTagExpression.TagName];

                            if (tag != null)
                            {
                                parameterSymbol.IsOperand = true;
                                parameterSymbol.Tag = tag;
                                parameterSymbol.TagExpression = tagExpression;
                            }
                        }
                        instructionSymbol.Parameters.Add(parameterSymbol);
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

                _parameters.Add(parameterSymbol);


                paramIndex++;
            }

            _instructionIndexInRung++;
            return context;
        }
    }
}
