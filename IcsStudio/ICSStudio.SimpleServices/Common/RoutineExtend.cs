using System;
using System.Collections.Generic;
using System.Linq;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Tags;

namespace ICSStudio.SimpleServices.Common
{
    public class RoutineExtend
    {
        public static List<ITag> GetMessageReferenceTags(MessageDataWrapper messageDataWrapper)
        {
            var list=new List<ITag>();
            if (messageDataWrapper == null) return list;
            var controller = Controller.GetInstance();
            if (!string.IsNullOrEmpty(messageDataWrapper.SourceElement))
            {
                var sourceTag = controller.Tags[messageDataWrapper.SourceElement];
                if (sourceTag != null)
                {
                    list.Add(sourceTag);
                }
            }

            if (!string.IsNullOrEmpty(messageDataWrapper.DestinationElement))
            {
                var destTag = controller.Tags[messageDataWrapper.DestinationElement];
                if (destTag != null)
                {
                    list.Add(destTag);
                }
            }

            return list;
        }

        public static bool CheckRoutineInRun(IRoutine routine)
        {
            //TODO(zyl);check other type routine
            if (routine == null) return false;
            if (!string.IsNullOrEmpty(routine.ParentCollection.ParentProgram.MainRoutineName))
            {
                if (routine.Name.Equals(routine.ParentCollection.ParentProgram.MainRoutineName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    var program = (Program)routine.ParentCollection.ParentProgram;
                    if (program.Inhibited || (program.ParentTask?.IsInhibited ?? false))
                    {
                        return false;
                    }

                    return true;
                }

                var main = routine.ParentCollection.ParentProgram.Routines[
                    routine.ParentCollection.ParentProgram.MainRoutineName];
                if (main != null)
                {
                    foreach (var jumpRoutine in main.GetJmpRoutines())
                    {
                        if (CheckRoutineInMain(jumpRoutine,
                            routine, new List<IRoutine>()))
                            return true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(routine.ParentCollection.ParentProgram.FaultRoutineName))
            {
                if (routine.Name.Equals(routine.ParentCollection.ParentProgram.FaultRoutineName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    var program = (Program)routine.ParentCollection.ParentProgram;
                    if (program.Inhibited || (program.ParentTask?.IsInhibited ?? false))
                    {
                        return false;
                    }

                    return true;
                }

                var fault = routine.ParentCollection.ParentProgram.Routines[
                    routine.ParentCollection.ParentProgram.FaultRoutineName];
                if (fault != null)
                {
                    foreach (var jumpRoutine in fault.GetJmpRoutines())
                    {
                        if (CheckRoutineInMain(jumpRoutine,
                            routine, new List<IRoutine>()))
                            return true;
                    }
                }
            }

            return false;
        }

        //private static bool CheckAoiIsInRun(AoiDefinition aoi)
        //{
        //    foreach (var program in aoi.ParentController.Programs)
        //    {
        //        foreach (var routine in program.Routines)
        //        {
        //            var st = routine as STRoutine;
        //            if (st != null)
        //            {
        //                if (st.VariableInfos.Any(v =>
        //                    v.IsAOI && aoi.Name.Equals(v.Name, StringComparison.OrdinalIgnoreCase)))
        //                {
        //                    if (CheckRoutineInRun(st)) return true;
        //                }
        //            }
        //        }
        //    }

        //    foreach (var a in aoi.ParentCollection)
        //    {
        //        if(a==aoi)continue;
        //        foreach (var routine in a.Routines)
        //        {
        //            var st = routine as STRoutine;
        //            if (st != null)
        //            {
        //                if (st.VariableInfos.Any(v =>
        //                    v.IsAOI && aoi.Name.Equals(v.Name, StringComparison.OrdinalIgnoreCase)))
        //                {
        //                    if (CheckAoiIsInRun((AoiDefinition) a)) return true;
        //                }
        //            }
        //        }
        //    }

        //    return false;
        //}

        private static bool CheckRoutineInMain(IRoutine jmpRoutine, IRoutine routine, List<IRoutine> checkedRoutine)
        {
            if (jmpRoutine == null) return false;
            if (routine.Name.Equals(jmpRoutine.Name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (checkedRoutine.Contains(jmpRoutine)) return false;
            checkedRoutine.Add(jmpRoutine);
            if (jmpRoutine.GetJmpRoutines().Any())
            {
                foreach (var routineJump in jmpRoutine.GetJmpRoutines())
                {
                    if (routineJump == jmpRoutine) return false;
                    if (CheckRoutineInMain(routineJump, routine, checkedRoutine)) return true;
                }
            }

            return false;
        }
    }
}
