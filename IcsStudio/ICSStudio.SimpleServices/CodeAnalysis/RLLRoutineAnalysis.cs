using System;
using System.Diagnostics;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Compiler;

namespace ICSStudio.SimpleServices.CodeAnalysis
{
    public class RLLRoutineAnalysis
    {
        public void Parse(RLLRoutine routine, bool addAoiDataReference)
        {
            try
            {
                ASTNode astRllRoutine =
                    RLLGrammarParser.Parse(routine.CodeText, routine.ParentController as Controller);
                
                TypeChecker p = new TypeChecker(
                    routine.ParentController as Controller, 
                    routine.ParentCollection.ParentProgram as IProgram,
                    routine.ParentCollection.ParentProgram as AoiDefinition);
                astRllRoutine = astRllRoutine.Accept(p);

                if (addAoiDataReference)
                {
                    RLLAoiReferenceHelper helper = new RLLAoiReferenceHelper(routine);
                    astRllRoutine.Accept(helper);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}
