using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.SourceProtection;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.Model
{
    internal class RoutineInfo : BaseSimpleInfo
    {
        private readonly IRoutine _routine;

        public RoutineInfo(IRoutine routine, ObservableCollection<SimpleInfo> infoSource)
            : base(infoSource)
        {
            _routine = routine;

            if (_routine != null)
            {
                CreateInfoItems();

                PropertyChangedEventManager.AddHandler(_routine,
                    OnRoutinePropertyChanged, string.Empty);

                IProgramModule programModule = _routine.ParentCollection.ParentProgram;
                if (programModule != null)
                {
                    PropertyChangedEventManager.AddHandler(programModule,
                        OnProgramPropertyChanged, string.Empty);
                }
            }
        }

        private void OnProgramPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName != "Name")return;
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                IProgramModule programModule = _routine.ParentCollection.ParentProgram;
                if (programModule != null)
                {
                    SetSimpleInfo("Add-On Instruction", programModule.Name);
                    SetSimpleInfo("Equipment Phase", programModule.Name);
                    SetSimpleInfo("Program", programModule.Name);
                }
            });
        }

        private void OnRoutinePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.PropertyName == "Description")
                {
                    SetSimpleInfo("Description", _routine.Description);
                }

                if (e.PropertyName == "CodeText")
                {
                    switch (_routine.Type)
                    {
                        case RoutineType.RLL:
                            RLLRoutine rllRoutine = _routine as RLLRoutine;
                            if (rllRoutine != null)
                            {
                                SetSimpleInfo("Number of Rungs", rllRoutine.CodeText.Count.ToString());
                            }

                            break;
                        case RoutineType.ST:
                            STRoutine stRoutine = _routine as STRoutine;
                            if (stRoutine != null)
                            {
                                SetSimpleInfo("Number of Lines", stRoutine.CodeText.Count.ToString());
                            }

                            break;
                        //TODO(gjc): add code here
                    }
                }

                if (e.PropertyName == "IsMainRoutine" || e.PropertyName == "IsFaultRoutine")
                {
                    string routineType = string.Empty;

                    switch (_routine.Type)
                    {
                        case RoutineType.Typeless:
                            break;
                        case RoutineType.RLL:
                            routineType = "Ladder Diagram";
                            break;
                        case RoutineType.FBD:
                            routineType = "Function Block Diagram";
                            break;
                        case RoutineType.SFC:
                            routineType = "Sequential Function Chart";
                            break;
                        case RoutineType.ST:
                            routineType = "Structured Text";
                            break;
                        case RoutineType.External:
                            break;
                        case RoutineType.Sequence:
                            routineType = "Equipment Sequence Diagram";
                            break;
                        case RoutineType.Encrypted:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var program = _routine.ParentCollection.ParentProgram as Program;
                    if (program != null)
                    {
                        if (program.Type == ProgramType.Phase)
                        {
                            if (_routine.IsMainRoutine)
                            {
                                routineType += "(Prestate)";
                            }
                        }
                        else
                        {
                            if (_routine.IsMainRoutine)
                            {
                                routineType += "(Main)";
                            }
                        }

                        if (_routine.IsFaultRoutine)
                        {
                            routineType += "(Fault)";
                        }
                    }


                    SetSimpleInfo("Type", routineType);
                }

                if (e.PropertyName == "IsEncrypted")
                {
                    if (_routine.IsEncrypted)
                    {
                        CreateProtectionInfoItems();
                    }
                    else
                    {
                        RemoveProtectionInfoItems();
                    }

                }

            });
        }

        private void CreateInfoItems()
        {
            if (InfoSource != null)
            {
                CreateDefaultInfoItems();

                if (_routine.IsEncrypted)
                {
                    CreateProtectionInfoItems();
                }
            }
        }

        private void CreateDefaultInfoItems()
        {
            var aoiDefinition = _routine.ParentCollection.ParentProgram as AoiDefinition;
            var program = _routine.ParentCollection.ParentProgram as Program;

            Contract.Assert(aoiDefinition != null || program != null);

            string routineType = string.Empty;
            string programType;
            string programName;
            string numberName = string.Empty;
            string numberValue = string.Empty;

            RLLRoutine rllRoutine = _routine as RLLRoutine;
            SFCRoutine sfcRoutine = _routine as SFCRoutine;
            STRoutine stRoutine = _routine as STRoutine;

            switch (_routine.Type)
            {
                case RoutineType.Typeless:
                    break;
                case RoutineType.RLL:
                    routineType = "Ladder Diagram";
                    numberName = "Number of Rungs";
                    if (rllRoutine != null)
                        numberValue = rllRoutine.CodeText.Count.ToString();
                    break;
                case RoutineType.FBD:
                    routineType = "Function Block Diagram";
                    numberName = "Number of Sheets";
                    //TODO(ZYL): add number here
                    break;
                case RoutineType.SFC:
                    routineType = "Sequential Function Chart";
                    numberName = "Number of Steps";
                    if (sfcRoutine != null)
                        numberValue = sfcRoutine.Contents.Count.ToString();
                    break;
                case RoutineType.ST:
                    routineType = "Structured Text";
                    numberName = "Number of Lines";
                    if (stRoutine != null)
                        numberValue = stRoutine.CodeText.Count.ToString();
                    break;
                case RoutineType.External:
                    break;
                case RoutineType.Sequence:
                    routineType = "Equipment Sequence Diagram";
                    numberName = "Number of Steps";
                    //TODO(ZYL): add number here
                    break;
                case RoutineType.Encrypted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (aoiDefinition != null)
            {
                programType = "Ud Function Block";
                programName = aoiDefinition.Name;
            }
            else
            {
                programName = program.Name;

                if (program.Type == ProgramType.Phase)
                {
                    programType = "Equipment Phase";
                    if (_routine.IsMainRoutine)
                    {
                        routineType += "(Prestate)";
                    }
                }
                else
                {
                    programType = "Program";
                    if (_routine.IsMainRoutine)
                    {
                        routineType += "(Main)";
                    }
                }

                if (_routine.IsFaultRoutine)
                {
                    routineType += "(Fault)";
                }
            }

            InfoSource.Add(new SimpleInfo { Name = "Type", Value = routineType });
            InfoSource.Add(new SimpleInfo { Name = "Description", Value = _routine.Description });
            InfoSource.Add(new SimpleInfo { Name = programType, Value = programName });
            InfoSource.Add(new SimpleInfo { Name = numberName, Value = numberValue });
        }

        private void CreateProtectionInfoItems()
        {
            var controller = _routine.ParentController as Controller;
            var manager = controller?.SourceProtectionManager;
            if (manager != null)
            {
                // Type, Name, Permissions
                string protectionType = "Source Key";
                string protectionName = string.Empty;
                string protectionPermissions = string.Empty;

                var permission = manager.GetPermission(_routine);
                if (permission == SourcePermission.Use)
                {
                    protectionName = "";
                    protectionPermissions = "Use";
                }
                else if (permission == SourcePermission.All)
                {
                    protectionName = manager.GetDisplayNameByKey(manager.GetKeyBySource(_routine));
                    protectionPermissions = "Protect, Edit, Copy, Export, View, Use";
                }
                else
                {
                    Contract.Assert(false);
                }

                InfoSource.Add(new SimpleInfo {Name = "Protection Type", Value = protectionType});
                InfoSource.Add(new SimpleInfo {Name = "Protection Name", Value = protectionName});
                InfoSource.Add(new SimpleInfo {Name = "Protection Permissions", Value = protectionPermissions});
            }
        }

        private void RemoveProtectionInfoItems()
        {
            InfoSource.Remove(GetSimpleInfo("Protection Type"));
            InfoSource.Remove(GetSimpleInfo("Protection Name"));
            InfoSource.Remove(GetSimpleInfo("Protection Permissions"));
        }
    }
}
