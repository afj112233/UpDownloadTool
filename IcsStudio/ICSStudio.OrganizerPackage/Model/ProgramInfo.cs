using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.OrganizerPackage.Model
{
    internal class ProgramInfo : BaseSimpleInfo
    {
        private readonly IProgram _program;

        public ProgramInfo(IProgram program, ObservableCollection<SimpleInfo> infoSource) : base(infoSource)
        {
            _program = program;

            CreateInfoItems();

            PropertyChangedEventManager.AddHandler(_program,
                OnProgramPropertyChanged, string.Empty);

            CollectionChangedEventManager.AddHandler(_program.Routines, OnRoutinesChanged);

            Controller controller = _program?.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
        }


        private void OnRoutinesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                SetSimpleInfo("Number of Routines", _program.Routines.Count.ToString());
            });
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                string maxScan = string.Empty;
                string lastScan = string.Empty;

                if (_program.ParentController.IsOnline)
                {
                    maxScan = $"{_program.MaxScanTime} us";
                    lastScan = $"{_program.LastScanTime} us";
                }

                SetSimpleInfo("Max Scan", maxScan);
                SetSimpleInfo("Last Scan", lastScan);
            });
        }

        private void OnProgramPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();


                if (e.PropertyName == "Description")
                {
                    SetSimpleInfo("Description", _program.Description);
                }

                if (e.PropertyName == "Inhibited" || e.PropertyName == "ParentTask")
                {
                    string status;
                    if (_program.Inhibited)
                        status = "Inhibited";
                    else if (_program.ParentTask == null)
                        status = "Unscheduled";
                    else
                        status = "Scheduled";

                    SetSimpleInfo("Status", status);
                }

                if (e.PropertyName == "MainRoutineName")
                {
                    SetSimpleInfo("Main Routine", _program.MainRoutineName);
                    SetSimpleInfo("Prestate Routine", _program.MainRoutineName);
                }

                if (e.PropertyName == "FaultRoutineName")
                {
                    SetSimpleInfo("Fault Routine", _program.FaultRoutineName);
                }

                if (e.PropertyName == "MaxScanTime")
                {
                    string maxScan = string.Empty;
                    if (_program.ParentController.IsOnline)
                    {
                        maxScan = $"{_program.MaxScanTime} us";
                    }

                    SetSimpleInfo("Max Scan", maxScan);
                }

                if (e.PropertyName == "LastScanTime")
                {
                    string lastScan = string.Empty;
                    if (_program.ParentController.IsOnline)
                    {
                        lastScan = $"{_program.LastScanTime} us";
                    }

                    SetSimpleInfo("Last Scan", lastScan);
                }

                if (e.PropertyName == "ParentTask")
                {
                    string parentTask = string.Empty;

                    if (_program.ParentTask != null)
                        parentTask = _program.ParentTask.Name;

                    SetSimpleInfo("Scheduled In", parentTask);
                }
            });
        }

        private void CreateInfoItems()
        {
            if (InfoSource != null && _program != null)
            {
                switch (_program.Type)
                {
                    case ProgramType.Phase:
                        CreatePhaseInfoItems();
                        break;
                    case ProgramType.Sequence:
                        CreateSequenceInfoItems();
                        break;

                    case ProgramType.Typeless:
                    case ProgramType.Normal:
                    case ProgramType.UDI:
                        CreateDefaultInfoItems();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void CreateDefaultInfoItems()
        {
            if (InfoSource != null && _program != null)
            {
                InfoSource.Clear();

                string status;
                string maxScan = string.Empty;
                string lastScan = string.Empty;
                string parentProgram = string.Empty;
                string parentTask = string.Empty;

                if (_program.Inhibited)
                    status = "Inhibited";
                else if (_program.ParentTask == null)
                    status = "Unscheduled";
                else
                    status = "Scheduled";


                if (_program.ParentController.IsOnline)
                {
                    maxScan = $"{_program.MaxScanTime} us";
                    lastScan = $"{_program.LastScanTime} us";
                }

                foreach (var item in _program.ParentController.Programs)
                {
                    if (item.ChildCollection[_program.Name] == _program)
                        parentProgram = item.Name;
                }

                if (_program.ParentTask != null)
                    parentTask = _program.ParentTask.Name;

                InfoSource.Add(new SimpleInfo { Name = "Description", Value = _program.Description });
                InfoSource.Add(new SimpleInfo { Name = "Status", Value = status });
                InfoSource.Add(new SimpleInfo
                    { Name = "Number of Routines", Value = _program.Routines.Count.ToString() });
                InfoSource.Add(new SimpleInfo { Name = "Main Routine", Value = _program.MainRoutineName });
                InfoSource.Add(new SimpleInfo { Name = "Fault Routine", Value = _program.FaultRoutineName });
                InfoSource.Add(new SimpleInfo { Name = "Max Scan", Value = maxScan });
                InfoSource.Add(new SimpleInfo { Name = "Last Scan", Value = lastScan });
                InfoSource.Add(new SimpleInfo { Name = "Parent", Value = parentProgram });
                InfoSource.Add(new SimpleInfo { Name = "Scheduled In", Value = parentTask });
            }
        }

        private void CreateSequenceInfoItems()
        {
            if (InfoSource != null && _program != null)
            {
                InfoSource.Clear();

                string status;
                string maxScan = string.Empty;
                string lastScan = string.Empty;
                string parentProgram = string.Empty;
                string parentTask = string.Empty;

                if (_program.Inhibited)
                    status = "Inhibited";
                else if (_program.ParentTask == null)
                    status = "Unscheduled";
                else
                    status = "Scheduled";


                if (_program.ParentController.IsOnline)
                {
                    maxScan = $"{_program.MaxScanTime} us";
                    lastScan = $"{_program.LastScanTime} us";
                }

                foreach (var item in _program.ParentController.Programs)
                {
                    if (item.ChildCollection[_program.Name] == _program)
                        parentProgram = item.Name;
                }

                if (_program.ParentTask != null)
                    parentTask = _program.ParentTask.Name;

                InfoSource.Add(new SimpleInfo { Name = "Description", Value = _program.Description });
                InfoSource.Add(new SimpleInfo { Name = "Status", Value = status });
                InfoSource.Add(new SimpleInfo
                    { Name = "Main Routine", Value = _program.MainRoutineName });
                InfoSource.Add(new SimpleInfo { Name = "State", Value = "" });
                InfoSource.Add(new SimpleInfo { Name = "Substate", Value = "" });
                InfoSource.Add(new SimpleInfo { Name = "Mode", Value = "" });
                InfoSource.Add(new SimpleInfo { Name = "Owner", Value = "" });
                InfoSource.Add(new SimpleInfo { Name = "Unit Id", Value = "" });
                InfoSource.Add(new SimpleInfo { Name = "Sequence Id", Value = "" });
                InfoSource.Add(new SimpleInfo { Name = "Phase Failure", Value = "" });
                InfoSource.Add(new SimpleInfo { Name = "Failure", Value = "" });
                InfoSource.Add(new SimpleInfo { Name = "Max Scan", Value = maxScan });
                InfoSource.Add(new SimpleInfo { Name = "Last Scan", Value = lastScan });
                InfoSource.Add(new SimpleInfo { Name = "Parent", Value = parentProgram });
                InfoSource.Add(new SimpleInfo { Name = "Scheduled In", Value = parentTask });
            }
        }

        private void CreatePhaseInfoItems()
        {
            if (InfoSource != null && _program != null)
            {
                InfoSource.Clear();

                string status;
                string maxScan = string.Empty;
                string lastScan = string.Empty;
                string parentProgram = string.Empty;
                string parentTask = string.Empty;

                if (_program.Inhibited)
                    status = "Inhibited";
                else if (_program.ParentTask == null)
                    status = "Unscheduled";
                else
                    status = "Scheduled";


                if (_program.ParentController.IsOnline)
                {
                    maxScan = $"{_program.MaxScanTime} us";
                    lastScan = $"{_program.LastScanTime} us";
                }

                foreach (var item in _program.ParentController.Programs)
                {
                    if (item.ChildCollection[_program.Name] == _program)
                        parentProgram = item.Name;
                }

                if (_program.ParentTask != null)
                    parentTask = _program.ParentTask.Name;


                InfoSource.Add(new SimpleInfo { Name = "Description", Value = _program.Description });
                InfoSource.Add(new SimpleInfo { Name = "Status", Value = status });
                InfoSource.Add(new SimpleInfo
                {
                    Name = "Number of Routines", Value = _program.Routines.Count.ToString(), Key = "NumberOfRoutines"
                });
                InfoSource.Add(new SimpleInfo { Name = "Fault Routine", Value = _program.FaultRoutineName });
                InfoSource.Add(new SimpleInfo { Name = "Prestate Routine", Value = _program.MainRoutineName });
                InfoSource.Add(new SimpleInfo { Name = "State", Value = "" });
                InfoSource.Add(new SimpleInfo { Name = "Substate", Value = "" });
                InfoSource.Add(new SimpleInfo { Name = "Owner", Value = "" });
                InfoSource.Add(new SimpleInfo { Name = "Failure", Value = "" });
                InfoSource.Add(new SimpleInfo { Name = "Max Scan", Value = maxScan });
                InfoSource.Add(new SimpleInfo { Name = "Last Scan", Value = lastScan });
                InfoSource.Add(new SimpleInfo { Name = "Parent", Value = parentProgram });
                InfoSource.Add(new SimpleInfo { Name = "Scheduled In", Value = parentTask });

            }
        }

    }
}
