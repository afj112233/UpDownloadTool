using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.CodeAnalysis;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Exceptions;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.Error;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIServicesPackage.Services;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Utilities;
using System.Collections.Generic;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    public class VerifyViewModel : ViewModelBase
    {
        private bool? _dialogResult;
        private string _message = LanguageManager.GetInstance().ConvertSpecifier("Verifying") + "...";
        private double _progress;

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public string Message
        {
            set { Set(ref _message, value); }
            get { return _message; }
        }

        public double Progress
        {
            set { Set(ref _progress, value); }
            get { return _progress; }
        }

        public const int NameLengthLimit = 60;

        public void Verify()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                //TODO(zyl):add more verification
                var controller = Controller.GetInstance();
                controller.IsPass = true;

                var createEditorService =
                    Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

                var projectInfoService
                    = Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;

                var outputService = Package.GetGlobalService(typeof(SErrorOutputService)) as IErrorOutputService;
                var step = (double)100 / GetCount(controller);
                var unsignedTypes = ProjectInfoService.GetUnsignedDataType(controller);
                try
                {
                    outputService?.Cleanup();

                    //检查资源限制
                    CheckResourceLimit(controller, outputService);

                    outputService?.AddMessages(LanguageManager.GetInstance().ConvertSpecifier("VerifyingController"), null);

                    projectInfoService?.VerifyToolchain();

                    {
                        var tags = controller.Tags.Where(t =>
                                t.Name.Length > NameLengthLimit || ((Tag)t).DataWrapper is MessageDataWrapper ||
                                ((Tag)t).DataWrapper is MotionGroup ||
                                t.DataTypeInfo.DataType.Equal(unsignedTypes, true))
                            .ToList();
                        var mg = tags.FirstOrDefault(t => ((Tag)t).DataWrapper is MotionGroup);
                        tags.Remove(mg);
                        if (mg != null)
                        {
                            var cup = ((MotionGroup)((Tag)mg).DataWrapper).CoarseUpdatePeriod;
                            if (cup % 500 == 0 && cup % 1000 != 0)
                            {
                                outputService?.AddErrors(LanguageManager.GetInstance().ConvertSpecifier("BaseUpdatePeriodError"),
                                    OrderType.Order, OnlineEditType.Original, null, null, null);
                            }
                        }

                        var messageTags = tags.Where(t => ((Tag)t).DataWrapper is MessageDataWrapper).ToList();
                        foreach (var messageTag in messageTags)
                        {
                            VerifyViewModel.VerifyMessageConfig((Tag)messageTag, outputService);
                            tags.Remove(messageTag);
                        }

                        foreach (var type in unsignedTypes.ExpectTypes)
                        {
                            if (type.IsPredefinedType) continue;
                            outputService?.AddErrors(
                                LanguageManager.GetInstance().ConvertSpecifier("VerifyError1") + $"'{type.Name}'"
                                + LanguageManager.GetInstance().ConvertSpecifier("ContainUnsignedDataType"),
                                OrderType.None, OnlineEditType.Original, null, null, type);
                        }

                        foreach (var tag in tags)
                        {
                            if (tag.Name.Length > NameLengthLimit)
                            {
                                outputService?.AddErrors(
                                    LanguageManager.GetInstance().ConvertSpecifier("VerifyError2") +
                                    $"'{tag.Name}'" + LanguageManager.GetInstance().ConvertSpecifier("NameLengthLimits"),
                                    OrderType.None, OnlineEditType.Original, null, null, tag);
                                continue;
                            }

                            outputService?.AddErrors(
                                LanguageManager.GetInstance().ConvertSpecifier("VerifyError2") +
                                $"{tag.Name}'" + LanguageManager.GetInstance().ConvertSpecifier("ContainUnsignedDataType"),
                                OrderType.None, OnlineEditType.Original, null, null, tag);
                        }
                    }

                    RLLRoutineAnalysis rllRoutineAnalysis = new RLLRoutineAnalysis();

                    foreach (var program in controller.Programs)
                    {
                        var tags = program.Tags.Where(t =>
                                t.Name.Length > NameLengthLimit || t.DataTypeInfo.DataType.Equal(unsignedTypes, true))
                            .ToList();
                        foreach (var tag in tags)
                        {
                            if (tag.Name.Length > NameLengthLimit)
                            {
                                outputService?.AddErrors(
                                    LanguageManager.GetInstance().ConvertSpecifier("VerifyError2") +
                                    $"'{tag.Name}'" + LanguageManager.GetInstance().ConvertSpecifier("NameLengthLimits"),
                                    OrderType.None, OnlineEditType.Original, null, null, tag);
                                continue;
                            }

                            outputService?.AddErrors(
                                LanguageManager.GetInstance().ConvertSpecifier("VerifyError2") +
                                $"{tag.Name}'" + LanguageManager.GetInstance().ConvertSpecifier("ContainUnsignedDataType"),
                                OrderType.None, OnlineEditType.Original, null, null, tag);
                        }

                        if (string.IsNullOrEmpty(program.MainRoutineName))
                            outputService?.AddWarnings($"{program.Name}" + LanguageManager.GetInstance().ConvertSpecifier("VerifyProgramError"),
                                program);
                        foreach (var routine in program.Routines)
                        {
                            var stRoutine = routine as STRoutine;
                            if (stRoutine != null)
                            {
                                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                                Message = LanguageManager.GetInstance().ConvertSpecifier("Verifying") + $" {program.Name} - {routine.Name}";
                                Progress += step;

                                await TaskScheduler.Default;

                                createEditorService?.ParseRoutine(stRoutine, !controller.IsDownloading, true);

                                if (!(createEditorService?.CheckRoutineInRun(stRoutine) ?? true))
                                    outputService?.AddWarnings(
                                        LanguageManager.GetInstance().ConvertSpecifier("VerifyError3") +
                                        $" {routine.Name}:{LanguageManager.GetInstance().ConvertSpecifier("RoutineCannotBeReached")}'{routine.Name} {LanguageManager.GetInstance().ConvertSpecifier("OfProgram")} {routine.ParentCollection.ParentProgram.Name}'",
                                        null);
                            }


                            var rllRoutine = routine as RLLRoutine;
                            if (rllRoutine != null)
                            {
                                rllRoutineAnalysis.Parse(rllRoutine, !controller.IsDownloading);
                                try
                                {
                                    //outputService?.RemoveMessage(rllRoutine);
                                    //outputService?.RemoveError(rllRoutine, rllRoutine.CurrentOnlineEditType);
                                    //outputService?.RemoveWarning(rllRoutine);
                                    outputService?.AddMessages(
                                        LanguageManager.GetInstance().ConvertSpecifier("VerifyingRoutine") +
                                        $" '{routine.Name}' {LanguageManager.GetInstance().ConvertSpecifier("OfProgram")} '{program.Name}'", rllRoutine);
                                    RLLGrammarAnalysis grammarAnalysis = new RLLGrammarAnalysis();

                                    grammarAnalysis.Analysis(rllRoutine);

                                    foreach (var error in grammarAnalysis.Errors)
                                    {
                                        if (error.Level == Level.Error)
                                            outputService?.AddErrors(error.Info, OrderType.None,
                                                OnlineEditType.Original, error.RungIndex, error.Offset, rllRoutine);
                                        if (error.Level == Level.Warning)
                                            outputService?.AddWarnings(error.Info, rllRoutine, error.RungIndex,
                                                error.Offset);
                                    }
                                }
                                catch (ErrorInfo error)
                                {
                                    Debug.WriteLine(error.Message + error.StackTrace);
                                    if (error.Level == Level.Error)
                                        outputService?.AddErrors(error.Info, OrderType.None, OnlineEditType.Original,
                                            error.RungIndex, error.Offset, rllRoutine);
                                    if (error.Level == Level.Warning)
                                        outputService?.AddWarnings(error.Info, rllRoutine, error.RungIndex,
                                            error.Offset);
                                }
                                catch (ArgumentOutOfRangeException aor)
                                {
                                    Debug.WriteLine(aor.Message + aor.StackTrace);
                                    outputService?.AddErrors(LanguageManager.GetInstance().ConvertSpecifier("Routine") + $" {rllRoutine.Name}: {LanguageManager.GetInstance().ConvertSpecifier("SyntaxError")}",
                                        OrderType.None, OnlineEditType.Original, 0, null, rllRoutine);
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine(e.Message + e.StackTrace);
                                    outputService?.AddErrors(LanguageManager.GetInstance().ConvertSpecifier("UnknownError"), OrderType.None, OnlineEditType.Original,
                                        0, null, rllRoutine);
                                }
                            }
                            //TODO(zyl):add other routine
                        }
                    }

                    foreach (var aoi in controller.AOIDefinitionCollection)
                    {
                        var tags = aoi.Tags.Where(t =>
                                t.Name.Length > NameLengthLimit || t.DataTypeInfo.DataType.Equal(unsignedTypes, true))
                            .ToList();
                        foreach (var tag in tags)
                        {
                            if (tag.Name.Length > NameLengthLimit)
                            {
                                outputService?.AddErrors(
                                    LanguageManager.GetInstance().ConvertSpecifier("VerifyError2") +
                                    $"'{tag.Name}'" + LanguageManager.GetInstance().ConvertSpecifier("NameLengthLimits"),
                                    OrderType.None, OnlineEditType.Original, null, null, tag);
                                continue;
                            }

                            outputService?.AddErrors(
                                LanguageManager.GetInstance().ConvertSpecifier("VerifyError2") +
                                $"{tag.Name}'" + LanguageManager.GetInstance().ConvertSpecifier("ContainUnsignedDataType"),
                                OrderType.None, OnlineEditType.Original, null, null, tag);
                        }

                        foreach (var routine in aoi.Routines)
                        {
                            var stRoutine = routine as STRoutine;
                            if (stRoutine != null)
                            {
                                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                                Progress += step;
                                Message = $"{LanguageManager.GetInstance().ConvertSpecifier("Verifying")} {aoi.Name} - {routine.Name}";
                                await TaskScheduler.Default;
                                createEditorService?.ParseRoutine(stRoutine, !controller.IsDownloading, true);
                            }

                            var rllRoutine = routine as RLLRoutine;
                            if (rllRoutine != null)
                            {
                                rllRoutineAnalysis.Parse(rllRoutine, !controller.IsDownloading);
                                try
                                {
                                    outputService?.RemoveMessage(rllRoutine);
                                    outputService?.RemoveError(rllRoutine, rllRoutine.CurrentOnlineEditType);
                                    outputService?.AddMessages(
                                        $"{LanguageManager.GetInstance().ConvertSpecifier("VerifyingRoutine")} '{routine.Name}' {LanguageManager.GetInstance().ConvertSpecifier("OfAoi")} '{aoi.Name}'", rllRoutine);
                                    RLLGrammarAnalysis grammarAnalysis = new RLLGrammarAnalysis();

                                    grammarAnalysis.Analysis(rllRoutine);

                                    foreach (var error in grammarAnalysis.Errors)
                                    {
                                        if (error.Level == Level.Error)
                                            outputService?.AddErrors(error.Info, OrderType.None,
                                                OnlineEditType.Original, error.RungIndex, error.Offset, rllRoutine);
                                        if (error.Level == Level.Warning)
                                            outputService?.AddWarnings(error.Info, rllRoutine, error.RungIndex, error.Offset);
                                    }
                                }
                                catch (ErrorInfo error)
                                {
                                    Console.WriteLine(error.Message + error.StackTrace);
                                    if (error.Level == Level.Error)
                                        outputService?.AddErrors(error.Info, OrderType.None, OnlineEditType.Original,
                                            error.RungIndex, error.Offset, rllRoutine);
                                    if (error.Level == Level.Warning)
                                        outputService?.AddWarnings(error.Info, rllRoutine,
                                            error.RungIndex, error.Offset);
                                }
                                catch (ArgumentOutOfRangeException aor)
                                {
                                    Debug.WriteLine(aor.Message + aor.StackTrace);
                                    outputService?.AddErrors($"{LanguageManager.GetInstance().ConvertSpecifier("Routine")} {rllRoutine.Name}: {LanguageManager.GetInstance().ConvertSpecifier("SyntaxError")}",
                                        OrderType.None, OnlineEditType.Original, 0, null, rllRoutine);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message + e.StackTrace);
                                    outputService?.AddErrors(LanguageManager.GetInstance().ConvertSpecifier("UnKnownError"), OrderType.None, OnlineEditType.Original,
                                        0, null, rllRoutine);
                                }
                            }

                            //TODO(zyl):add other routine
                        }
                    }

                    // module
                    foreach (var module in controller.DeviceModules)
                    {
                        if (!(module is LocalModule))
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                            Progress += step;
                            Message = $"{LanguageManager.GetInstance().ConvertSpecifier("VerifyingModule")} '{module.Name}'";
                            outputService?.AddMessages($"{LanguageManager.GetInstance().ConvertSpecifier("VerifyingModule")} '{module.Name}'", null);
                            await TaskScheduler.Default;
                            projectInfoService?.Verify(module);
                        }
                    }

                    // axis tag
                    foreach (var tag in controller.Tags)
                    {
                        if (tag.DataTypeInfo.DataType.IsAxisType)
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                            Progress += step;
                            Message = $"{LanguageManager.GetInstance().ConvertSpecifier("VerifyingTag")} '{tag.Name}'";
                            outputService?.AddMessages($"{LanguageManager.GetInstance().ConvertSpecifier("VerifyingAxis")} '{tag.Name}'", null);
                            await TaskScheduler.Default;
                            projectInfoService?.VerifyAxisTag(tag);
                        }
                    }

                    projectInfoService?.VerifyParameterConnection();
                }
                catch (Exception e)
                {
                    if (e is InvalidParameterConnection)
                    {
                        controller.Clear();

                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        DialogResult = false;
                    }
                    else
                    {
                        Debug.Assert(false, e.StackTrace);
                        Controller.GetInstance().Log($"{LanguageManager.GetInstance().ConvertSpecifier("VerifyError")}{e.StackTrace}");
                    }
                }
                finally
                {
                    outputService?.Summary();

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    DialogResult = true;
                }
            });
        }

        public static void VerifyMessageConfig(Tag tag, IErrorOutputService errorOutputService)
        {
            MessageDataWrapper messageData = (MessageDataWrapper)tag.DataWrapper;
            if (!string.IsNullOrEmpty(messageData.SourceElement))
            {
                var sourceTag = ObtainValue.NameToTag(messageData.SourceElement, null);
                if (sourceTag?.Item1 == null)
                {
                    errorOutputService?.AddErrors($"{LanguageManager.GetInstance().ConvertSpecifier("Tag")} '{tag.Name}'{LanguageManager.GetInstance().ConvertSpecifier("ReferenceValidObject")}",
                        OrderType.None, OnlineEditType.Original, null, null, tag);
                    errorOutputService?.AddErrors($"{LanguageManager.GetInstance().ConvertSpecifier("Tag")} '{tag.Name}'{LanguageManager.GetInstance().ConvertSpecifier("BaseVariableIsNotVerified")}", OrderType.None,
                        OnlineEditType.Original, null, null, tag);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(messageData.DestinationElement))
            {
                var destTag = ObtainValue.NameToTag(messageData.DestinationElement, null);
                if (destTag?.Item1 == null)
                {
                    errorOutputService?.AddErrors($"{LanguageManager.GetInstance().ConvertSpecifier("Tag")} '{tag.Name}'{LanguageManager.GetInstance().ConvertSpecifier("ReferenceValidObject")}",
                        OrderType.None, OnlineEditType.Original, null, null, tag);
                    errorOutputService?.AddErrors($"{LanguageManager.GetInstance().ConvertSpecifier("Tag")} '{tag.Name}'{LanguageManager.GetInstance().ConvertSpecifier("BaseVariableIsNotVerified")}", OrderType.None,
                        OnlineEditType.Original, null, null, tag);
                }
            }
        }

        private int GetCount(Controller controller)
        {
            int count = 0;
            foreach (var program in controller.Programs)
            {
                count += program.Routines.Count(r => r is STRoutine);
            }

            foreach (var aoi in controller.AOIDefinitionCollection)
            {
                count += aoi.Routines.Count(r => r is STRoutine);
            }

            count += controller.DeviceModules.Count(d => !(d is LocalModule));
            count += controller.Tags.Count(d => (d.DataTypeInfo.DataType?.IsAxisType) ?? false);
            count += controller.ParameterConnections.Count;
            return count;
        }

        private void CheckResourceLimit(IController controller, IErrorOutputService outputService)
        {
            var localModule = controller.DeviceModules["Local"] as LocalModule;
            var type = localModule?.DisplayText;

            var resourceLimit = new KeyValuePair<string, ResourceLimitExtensions.ResourceType>();
            foreach (var item in ResourceLimitExtensions.ResourceLimitDictionary)
            {
                if (type.StartsWith(item.Key, StringComparison.OrdinalIgnoreCase))
                {
                    resourceLimit = item;
                }
            }

            #region 1.task 优先级、类型
            var taskResults = ResourceLimitExtensions.CheckTask(controller);
            foreach (var taskResult in taskResults)
            {
                switch (taskResult)
                {
                    case ResourceLimitExtensions.TaskResult.TypeError:
                        if (ResourceLimitExtensions.ContinuousTaskList.Count > 1)
                        {
                            var str = "";
                            foreach (var item in ResourceLimitExtensions.ContinuousTaskList)
                            {
                                if (str == "")
                                {
                                    str = $"'{item.Name}'";
                                }
                                else
                                {
                                    str = string.Format($"{str}, '{item.Name}'");
                                }
                            }

                            outputService?.AddErrors(
                                $"This controller only supports 1 Continuous task, but now there are {ResourceLimitExtensions.ContinuousTaskList.Count} Continuous tasks : {str}",
                                OrderType.None, OnlineEditType.Original, null, null, null);
                        }
                        else if (ResourceLimitExtensions.EventPeriodicTaskList.Count > 15)
                        {
                            var str = "";
                            foreach (var item in ResourceLimitExtensions.EventPeriodicTaskList)
                            {
                                if (str == "")
                                {
                                    str = $"'{item.Name}'";
                                }
                                else
                                {
                                    str = string.Format($"{str}, '{item.Name}'");
                                }
                            }

                            outputService?.AddErrors(
                                $"This controller only supports 15 Event and Periodic tasks, but now there are {ResourceLimitExtensions.EventPeriodicTaskList.Count} Event and Periodic tasks : {str}",
                                OrderType.None, OnlineEditType.Original, null, null, null);
                        }

                        break;
                    case ResourceLimitExtensions.TaskResult.PriorityError:
                        var messageDictionary = new Dictionary<int, string>();
                        foreach (var item in ResourceLimitExtensions.TaskPriorityDictionary)
                        {
                            if (item.Value.Count > 1)
                            {
                                foreach (var task in item.Value)
                                {
                                    var message = $"Task: the task '{task.Name}' cannot be set the priority '{item.Key}'. Task priority must be different from others";
                                    outputService?.AddErrors(message, OrderType.None, OnlineEditType.Original, null, null, task);
                                }
                            }
                        }
                        break;
                    case ResourceLimitExtensions.TaskResult.Normal:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            #endregion

            #region 2.program 数量
            var programResult = ResourceLimitExtensions.CheckProgram(controller);
            switch (programResult)
            {
                case ResourceLimitExtensions.ProgramResult.OutRange:
                    var str = "";
                    foreach (var item in ResourceLimitExtensions.ProgramDictionary)
                    {
                        if (item.Value > 1000)
                        {
                            if (str == "")
                            {
                                str = $"'{item.Key.Name}'";
                            }
                            else
                            {
                                str = string.Format($"{str}, '{item.Key.Name}'");
                            }
                        }
                    }

                    outputService?.AddErrors(
                        $"This controller supports only 1000 programs, {str} has more than 1000 programs",
                        OrderType.None, OnlineEditType.Original, null, null, null);
                    break;
                case ResourceLimitExtensions.ProgramResult.Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion

            #region 3.axis 数量
            var axisResult = ResourceLimitExtensions.CheckAxis(controller);
            switch (axisResult)
            {
                case ResourceLimitExtensions.AxisResult.OutRange:
                    outputService?.AddErrors(
                        $"{resourceLimit.Key} only supports {resourceLimit.Value.Axis} axes, but now has {ResourceLimitExtensions.AxisCount} axes",
                        OrderType.None, OnlineEditType.Original, null, null, null);
                    break;
                case ResourceLimitExtensions.AxisResult.Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion

            #region 4.Axis-Position数量
            var axisPositionResult = ResourceLimitExtensions.CheckAxisPosition(controller);
            switch (axisPositionResult)
            {
                case ResourceLimitExtensions.AxisPositionResult.OutRange:
                    outputService?.AddErrors(
                        $"{resourceLimit.Key} only supports {resourceLimit.Value.AxisPosition} Axis Position, but now has {ResourceLimitExtensions.AxisPositionCount} Axis Position",
                        OrderType.None, OnlineEditType.Original, null, null, null);
                    break;
                case ResourceLimitExtensions.AxisPositionResult.Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion

            #region 5.network 数量
            var networkResult = ResourceLimitExtensions.CheckNetwork(controller);
            switch (networkResult)
            {
                case ResourceLimitExtensions.NetworkResult.OutRange:
                    outputService?.AddErrors(
                        $"{resourceLimit.Key} only supports {resourceLimit.Value.Network} network, but now has {ResourceLimitExtensions.NetworkCount} network",
                        OrderType.None, OnlineEditType.Original, null, null, null);
                    break;
                case ResourceLimitExtensions.NetworkResult.Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endregion

            #region 6.AOI输入参数加上输出参数加上本地标记 数量 7.AOI InOutput参数 数量
            var aoiTagResults = ResourceLimitExtensions.CheckAOITag(controller);
            if (aoiTagResults.Contains(ResourceLimitExtensions.AOITagResult.InputOutputLocalOutRange))
            {
                var str = "";
                foreach (var item in ResourceLimitExtensions.AoiDefinitionDictionary)
                {
                    if (item.Value.InputOutputLocal > 512)
                    {
                        if (str == "")
                        {
                            str = $"'{item.Key.Name}'";
                        }
                        else
                        {
                            str = String.Format($"{str}, '{item.Key.Name}'");
                        }
                    }
                }
                outputService?.AddErrors(
                    $"There are no more than 512 Input tags plus Output tags plus Local tags in this controller, but {str} has more than 512"
                    , OrderType.None, OnlineEditType.Original, null, null, null);
            }
            if (aoiTagResults.Contains(ResourceLimitExtensions.AOITagResult.InOutOutRange))
            {
                var str = "";
                foreach (var item in ResourceLimitExtensions.AoiDefinitionDictionary)
                {
                    if (item.Value.InOut > 64)
                    {
                        if (str == "")
                        {
                            str = $"'{item.Key.Name}'";
                        }
                        else
                        {
                            str = String.Format($"{str}, '{item.Key.Name}'");
                        }
                    }
                }
                outputService?.AddErrors(
                    $"There are no more than 64 InOut tags in this controller, but {str} has more than 64"
                    , OrderType.None, OnlineEditType.Original, null, null, null);
            }
            #endregion

            //TODO(clx):待补充 梯形图Rung嵌套层数限制、AOI嵌套层数限制、JSR指令调用深度和InOuts/Outputs限制
        }

    }
}
