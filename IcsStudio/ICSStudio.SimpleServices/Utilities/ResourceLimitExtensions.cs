using System;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.PredefinedType;
using ICSStudio.SimpleServices.Tags;
using System.Collections.Generic;
using ICSStudio.Interfaces.Tags;

namespace ICSStudio.SimpleServices.Utilities
{
    public class ResourceLimitExtensions
    {
        public static Dictionary<CTask, int> ProgramDictionary = new Dictionary<CTask, int>();
        public static Dictionary<int, List<CTask>> TaskPriorityDictionary = new Dictionary<int, List<CTask>>();
        public static Dictionary<AoiDefinition, AoiLimit> AoiDefinitionDictionary = new Dictionary<AoiDefinition, AoiLimit>();
        public static List<CTask> ContinuousTaskList = new List<CTask>();
        public static List<CTask> EventPeriodicTaskList = new List<CTask>();
        public static int AxisCount;
        public static int AxisPositionCount;
        public static int NetworkCount;

        public struct ResourceType
        {
            public int Axis;
            public int AxisPosition;
            public int Network;
        }

        public struct AoiLimit
        {
            public int InputOutputLocal;
            public int InOut;
        }

        public static Dictionary<string, ResourceType> ResourceLimitDictionary = new Dictionary<string, ResourceType>
        {
            {"ICC-B010ERM", new ResourceType { Axis = 100, AxisPosition = 4, Network = 24} },
            {"ICC-B020ERM", new ResourceType { Axis = 100, AxisPosition = 8, Network = 40} },
            {"ICC-B030ERM", new ResourceType { Axis = 100, AxisPosition = 16, Network = 60 } },
            {"ICC-B050ERM", new ResourceType { Axis = 100, AxisPosition = 16, Network = 120 } },

            {"ICC-P010ERM", new ResourceType { Axis = 256, AxisPosition = 4, Network = 24 } },
            {"ICC-P020ERM", new ResourceType { Axis = 256, AxisPosition = 8, Network = 40 } },
            {"ICC-P030ERM", new ResourceType { Axis = 256, AxisPosition = 16, Network = 60 } },
            {"ICC-P050ERM", new ResourceType { Axis = 256, AxisPosition = 32, Network = 120 } },
            {"ICC-P080ERM", new ResourceType { Axis = 256, AxisPosition = 64, Network = 150 } },
            {"ICC-P0100ERM", new ResourceType { Axis = 256, AxisPosition = 128, Network = 180 } },

            {"ICC-T030ERM", new ResourceType { Axis = 512, AxisPosition = 16, Network = 60 } },
            {"ICC-T050ERM", new ResourceType { Axis = 512, AxisPosition = 32, Network = 120 } },
            {"ICC-T0100ERM", new ResourceType { Axis = 512, AxisPosition = 128, Network = 180 } },
            {"ICC-T0200ERM", new ResourceType { Axis = 512, AxisPosition = 256, Network = 250 } },
        };

        public enum TaskResult
        {
            Normal,
            TypeError,
            PriorityError
        }

        public static List<TaskResult> CheckTask(IController controller)
        {
            TaskPriorityDictionary.Clear();
            ContinuousTaskList.Clear();
            EventPeriodicTaskList.Clear();
            var taskResult = new List<TaskResult>();
            
            foreach (var task1 in controller.Tasks)
            {
                var task = (CTask)task1;
                switch (task.Type)
                {
                    case TaskType.Event:
                    case TaskType.Periodic:
                        {
                            EventPeriodicTaskList.Add(task);
                            if (TaskPriorityDictionary.ContainsKey(task.Priority))
                            {
                                TaskPriorityDictionary[task.Priority].Add(task);
                            }
                            else
                            {
                                TaskPriorityDictionary.Add(task.Priority, new List<CTask>{ task });
                            }

                            break;
                        }
                    case TaskType.Continuous:
                        ContinuousTaskList.Add(task);
                        break;
                    case TaskType.NullType:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (ContinuousTaskList.Count > 1 || EventPeriodicTaskList.Count > 15)
            {
                taskResult.Add(TaskResult.TypeError);
            }

            foreach (var item in TaskPriorityDictionary)
            {
                if (item.Value.Count > 1)
                {
                    taskResult.Add(TaskResult.PriorityError);
                    break;
                }
            }

            if (taskResult.Count == 0)
            {
                taskResult.Add(TaskResult.Normal);
            }

            return taskResult;
        }

        public enum ProgramResult
        {
            Normal,
            OutRange,
        }

        public static ProgramResult CheckProgram(IController controller)
        {
            ProgramDictionary.Clear();
            foreach (var program in controller.Programs)
            {
                var task = program.ParentTask as CTask;
                if (task != null)
                {
                    if (ProgramDictionary.ContainsKey(task))
                    {
                        ProgramDictionary[task]++;
                    }
                    else
                    {
                        ProgramDictionary.Add(task, 1);
                    }
                }
            }

            foreach (var item in ProgramDictionary)
            {
                if (item.Value > 1000)
                {
                    return ProgramResult.OutRange;
                }
            }

            return ProgramResult.Normal;
        }

        public enum AxisResult
        {
            Normal,
            OutRange,
        }

        public static AxisResult CheckAxis(IController controller)
        {
            AxisCount = 0;
            var localModule = controller.DeviceModules["Local"] as LocalModule;
            var type= localModule?.DisplayText;

            if (type != null)
            {
                foreach (var item in controller.Tags)
                {
                    var tag = item as Tag;
                    {
                        if (tag?.DataTypeInfo.DataType is AXIS_CIP_DRIVE)
                        {
                            AxisCount++;
                        }
                    }
                }

                foreach (var item in ResourceLimitDictionary)
                {
                    if(type.StartsWith(item.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        if (AxisCount > item.Value.Axis)
                        {
                            return AxisResult.OutRange;
                        }
                    }
                }
            }

            return AxisResult.Normal;
        }

        public enum AxisPositionResult
        {
            Normal,
            OutRange,
        }

        public static AxisPositionResult CheckAxisPosition(IController controller)
        {
            AxisPositionCount = 0;
            var localModule = controller.DeviceModules["Local"] as LocalModule;
            var type = localModule?.DisplayText;

            if (type != null)
            {
                foreach (var item in controller.Tags)
                {
                    var tag = item as Tag;
                    {
                        if (tag?.DataTypeInfo.DataType is AXIS_CIP_DRIVE)
                        {
                            var axisCIPDrive = tag.DataWrapper as AxisCIPDrive;
                            if (axisCIPDrive != null)
                            {
                                var axisConfiguration =
                                    (AxisConfigurationType)Convert.ToByte(axisCIPDrive.CIPAxis.AxisConfiguration);
                                if (axisConfiguration == AxisConfigurationType.PositionLoop
                                    && axisCIPDrive.AssociatedModule != null)
                                {
                                    AxisPositionCount++;
                                }
                            }
                        }
                    }
                }

                foreach (var item in ResourceLimitDictionary)
                {
                    if(type.StartsWith(item.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        if (AxisPositionCount > item.Value.AxisPosition)
                        {
                            return AxisPositionResult.OutRange;
                        }
                    }
                }
            }

            return AxisPositionResult.Normal;
        }

        public enum NetworkResult
        {
            Normal,
            OutRange,
        }

        public static NetworkResult CheckNetwork(IController controller)
        {
            NetworkCount = 0;
            var deviceModuleCollection = controller.DeviceModules;
            var localModule = controller.DeviceModules["Local"] as LocalModule;
            var type = localModule?.DisplayText;

            foreach (var deviceModule in deviceModuleCollection)
            {
                if (deviceModule is CIPMotionDrive 
                    || deviceModule is GeneralEthernet
                    || deviceModule is CommunicationsAdapter)
                {
                    NetworkCount++;
                }
            }

            if (type != null)
            {
                foreach (var item in ResourceLimitDictionary)
                {
                    if (type.StartsWith(item.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        if (NetworkCount > item.Value.Network)
                        {
                            return NetworkResult.OutRange;
                        }
                    }
                }
            }

            return NetworkResult.Normal;
        }

        public enum AOITagResult
        {
            Normal,
            InputOutputLocalOutRange,
            InOutOutRange
        }

        public static List<AOITagResult> CheckAOITag(IController controller)
        {
            AoiDefinitionDictionary.Clear();
            var aoiTagResult = new List<AOITagResult>();
            var aoiDefinitionCollection = controller.AOIDefinitionCollection;
          
            foreach (var aoiDefinition in aoiDefinitionCollection)
            {
                var inputOutputLocal = 0;
                var inOut = 0;
                var aoi = (AoiDefinition)aoiDefinition;
                foreach (var tag in aoi.Tags)
                {
                    if (tag.Usage == Usage.Input || tag.Usage == Usage.Output || tag.Usage == Usage.Local)
                    {
                        inputOutputLocal++;
                    }
                    else if (tag.Usage == Usage.InOut)
                    {
                        inOut++;
                    }
                }

                if (!aoiTagResult.Contains(AOITagResult.InputOutputLocalOutRange))
                {
                    if (inputOutputLocal > 512)
                    {
                        aoiTagResult.Add(AOITagResult.InputOutputLocalOutRange);
                    }
                }
                else if (!aoiTagResult.Contains(AOITagResult.InOutOutRange))
                {
                    if (inOut > 64)
                    {
                        aoiTagResult.Add(AOITagResult.InOutOutRange);
                    }
                }

                AoiDefinitionDictionary.Add(aoi, new AoiLimit{InOut = inOut, InputOutputLocal = inputOutputLocal});
            }

            if (aoiTagResult.Count == 0)
            {
                aoiTagResult.Add(AOITagResult.Normal);
            }

            return aoiTagResult;
        }
    }
}
