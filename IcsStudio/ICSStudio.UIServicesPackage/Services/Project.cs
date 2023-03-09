using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using ICSStudio.Dialogs.GlobalSetting;
using ICSStudio.Dialogs.WaitDialog;
using ICSStudio.Gui.Annotations;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Online;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using NLog;

namespace ICSStudio.UIServicesPackage.Services
{
    [SuppressMessage("ReSharper", "UseNameofExpression")]
    public partial class Project : IProject
    {
        internal static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private string _recentCommPath;

        private Controller _controller;

        private readonly ApplicationOptions _applicationOptions;

        private bool _isInOffline;

        static Project()
        {
        }

        private Project()
        {
            Saved = true;

            _controller = null;
            _recentCommPath = string.Empty;

            _applicationOptions = new ApplicationOptions();
        }

        //TODO(gjc): remove later
        public static Project Instance { get; } = new Project();

        public IController Controller
        {
            get { return _controller; }
            set
            {
                //if (_controller != value)
                {
                    Controller oldController = _controller;

                    _controller = value as Controller;

                    OnControllerChanged(oldController, _controller);
                }

                if (_controller != null)
                {
                    RecentCommPath = _controller.ProjectCommunicationPath;
                }

                Saved = true;
            }
        }

        public bool IsDirty => !Saved;

        public bool IsEmpty
        {
            get
            {
                if (_controller == null)
                    return true;

                if (string.IsNullOrEmpty(_controller.ProjectLocaleName))
                    return true;

                return false;
            }
        }

        public bool Saved { get; set; }

        public string RecentCommPath
        {
            get { return _recentCommPath; }
            set
            {
                if (_recentCommPath != value)
                {
                    _recentCommPath = value;

                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Save(bool needNativeCode)
        {
            int result = 0;
            if (!IsEmpty)
            {
                PreSave();

                string projectFileName = _controller.ProjectLocaleName;
                string savingFileName = Path.GetTempFileName() + ".json";

                try
                {
                    if (needNativeCode)
                    {
                        var waitingVm = new GeneralWaitingViewModel(true);
                        waitingVm.WorkAction += () =>
                        {
                            _controller.GenCode();  //Main time consuming task, other tasks can be completed in a flash.
                        };
                        var waitingDialog = new GeneralWaitingDialog(waitingVm);
                        waitingDialog.ShowDialog();
                    }

                    _controller.Save(savingFileName, needNativeCode);

                    //TODO(gjc): edit later
                    GlobalSetting.GetInstance().SaveConfig();
                }
                catch (Exception e)
                {
                    Logger.Error($"Saving {projectFileName} failed!{e}");

                    result = _controller.Export(savingFileName);
                }

                if (result == 0)
                {
                    try
                    {
                        File.Copy(savingFileName, projectFileName, true);
                        
                        PostSave();

                        Saved = true;
                    }
                    catch (Exception e)
                    {
                        result = -100;
                        Logger.Error($"Post save {projectFileName} failed!{e}");

                    }

                }
            }

            return result;
        }

        public int SaveAs(string fileName)
        {
            int result = _controller.Export(fileName);

            if (result != 0)
                return result;

            GlobalSetting.GetInstance().SaveConfig();

            _controller.ProjectLocaleName = fileName;

            Saved = true;

            return result;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void GoOffline()
        {
            _isInOffline = true;

            _controller?.GoOffline();

            _isInOffline = false;
        }

        #region Save
        
        private void PreSave()
        {
            if (_applicationOptions.EnableAutomaticProjectBackup)
            {
                try
                {
                    CreateDuplicateFile(_controller.ProjectLocaleName);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

            }
        }

        private void PostSave()
        {
            if (_applicationOptions.EnableAutomaticProjectBackup)
            {
                try
                {
                    RemoveUnnecessaryFile(_controller.ProjectLocaleName, _applicationOptions.NumberOfBackups);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }

        private void CreateDuplicateFile(string projectFile)
        {
            if (!File.Exists(projectFile))
                return;

            // check file size
            FileInfo fileInfo = new FileInfo(projectFile);
            if (fileInfo.Length == 0)
                return;

            string machineName = Environment.MachineName;
            string userName = Environment.UserName;

            string folder = Path.GetDirectoryName(projectFile);
            Contract.Assert(folder != null);
            string projectName = Path.GetFileNameWithoutExtension(projectFile);

            // get back index
            string backupFilePrefix = $"{projectName}.{machineName}.{userName}.BAK";

            DirectoryInfo folderInfo = new DirectoryInfo(folder);
            var backupFiles =
                folderInfo.GetFiles($"{backupFilePrefix}*.json")
                    .Select((x) =>
                    {
                        int index = GetBackupIndex(x.Name, backupFilePrefix);
                        return new Tuple<int, FileInfo>(index, x);
                    }).ToList();

            backupFiles.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            int newBackupIndex = 0;
            if (backupFiles.Count > 0)
            {
                newBackupIndex = backupFiles[backupFiles.Count - 1].Item1 + 1;
            }

            // copy
            File.Copy(projectFile, $"{folder}\\{backupFilePrefix}{newBackupIndex:D3}.json");
        }

        private void RemoveUnnecessaryFile(string projectFile, int numberOfBackups)
        {
            string machineName = Environment.MachineName;
            string userName = Environment.UserName;

            string folder = Path.GetDirectoryName(projectFile);
            Contract.Assert(folder != null);
            string projectName = Path.GetFileNameWithoutExtension(projectFile);

            // get back index
            string backupFilePrefix = $"{projectName}.{machineName}.{userName}.BAK";

            DirectoryInfo folderInfo = new DirectoryInfo(folder);
            var backupFiles =
                folderInfo.GetFiles($"{backupFilePrefix}*.json")
                    .Select((x) =>
                    {
                        int index = GetBackupIndex(x.Name, backupFilePrefix);
                        return new Tuple<int, FileInfo>(index, x);
                    }).ToList();

            backupFiles.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            // delete
            if (backupFiles.Count > numberOfBackups)
            {
                int deleteCount = backupFiles.Count - numberOfBackups;
                if (deleteCount > backupFiles.Count)
                    deleteCount = backupFiles.Count;

                for (int i = 0; i < deleteCount; i++)
                {
                    File.Delete(backupFiles[i].Item2.FullName);
                }
            }
        }

        private int GetBackupIndex(string fileName, string prefix)
        {
            string temp = Path.GetFileNameWithoutExtension(fileName);
            if (string.IsNullOrEmpty(temp))
                return -1;

            if (!temp.StartsWith(prefix))
                return -1;

            temp = temp.Substring(prefix.Length);

            int index;
            if (int.TryParse(temp, out index))
            {
                return index;
            }

            return -1;
        }

        //private void ProjectFileBackup(string projectFile, int numberOfBackups)
        //{
        //    if (!File.Exists(projectFile))
        //        return;

        //    // check file size
        //    FileInfo fileInfo = new FileInfo(projectFile);
        //    if (fileInfo.Length == 0)
        //        return;

        //    string machineName = Environment.MachineName;
        //    string userName = Environment.UserName;

        //    string folder = Path.GetDirectoryName(projectFile);
        //    Contract.Assert(folder != null);
        //    string projectName = Path.GetFileNameWithoutExtension(projectFile);

        //    // get back index
        //    string backupFilePrefix = $"{projectName}.{machineName}.{userName}.BAK";

        //    DirectoryInfo folderInfo = new DirectoryInfo(folder);
        //    var backupFiles =
        //        folderInfo.GetFiles($"{backupFilePrefix}*.json")
        //            .Select((x) =>
        //            {
        //                int index = GetBackupIndex(x.Name, backupFilePrefix);
        //                return new Tuple<int, FileInfo>(index, x);
        //            }).ToList();

        //    backupFiles.Sort((x, y) => x.Item1.CompareTo(y.Item1));

        //    int newBackupIndex = 0;
        //    if (backupFiles.Count > 0)
        //    {
        //        newBackupIndex = backupFiles[backupFiles.Count - 1].Item1 + 1;
        //    }

        //    // copy
        //    File.Copy(projectFile, $"{folder}\\{backupFilePrefix}{newBackupIndex:D3}.json");

        //    // delete
        //    if (backupFiles.Count >= numberOfBackups)
        //    {
        //        int deleteCount = backupFiles.Count - numberOfBackups + 1;
        //        if (deleteCount > backupFiles.Count)
        //            deleteCount = backupFiles.Count;

        //        for (int i = 0; i < deleteCount; i++)
        //        {
        //            File.Delete(backupFiles[i].Item2.FullName);
        //        }
        //    }

        //}
        #endregion

        #region Attach and Detach Controller

        private void AttachController(Controller controller)
        {
            if (controller != null)
            {
                CollectionChangedEventManager.AddHandler(controller.Tasks, OnTasksChanged);
                CollectionChangedEventManager.AddHandler(controller.Programs, OnProgramsChanged);
                CollectionChangedEventManager.AddHandler(controller.Tags, OnTagsChanged);
                CollectionChangedEventManager.AddHandler(controller.AOIDefinitionCollection, OnAOIDefinitionsChanged);
                CollectionChangedEventManager.AddHandler(controller.DataTypes, OnDataTypesChanged);
                CollectionChangedEventManager.AddHandler(controller.Trends, OnTrendsChanged);
                CollectionChangedEventManager.AddHandler(controller.DeviceModules, OnDeviceModulesChanged);

                foreach (ITask task in controller.Tasks)
                {
                    PropertyChangedEventManager.AddHandler(task, OnTaskPropertyChanged, string.Empty);
                }

                foreach (var program in controller.Programs)
                {
                    PropertyChangedEventManager.AddHandler(program, OnProgramPropertyChanged, string.Empty);

                    CollectionChangedEventManager.AddHandler(program.Routines, OnRoutinesChanged);
                    CollectionChangedEventManager.AddHandler(program.Tags, OnTagsChanged);

                    foreach (var routine in program.Routines)
                    {
                        PropertyChangedEventManager.AddHandler(routine,
                            OnRoutinePropertyChanged, string.Empty);
                    }

                    foreach (var tag in program.Tags)
                    {
                        PropertyChangedEventManager.AddHandler(tag, OnTagPropertyChanged, string.Empty);
                    }
                }

                foreach (var tag in controller.Tags)
                {
                    PropertyChangedEventManager.AddHandler(tag, OnTagPropertyChanged, string.Empty);
                }

                foreach (var aoiDefinition in controller.AOIDefinitionCollection)
                {
                    PropertyChangedEventManager.AddHandler(aoiDefinition, OnAOIDefinitionPropertyChanged, string.Empty);

                    CollectionChangedEventManager.AddHandler(aoiDefinition.Routines, OnRoutinesChanged);
                    CollectionChangedEventManager.AddHandler(aoiDefinition.Tags, OnTagsChanged);

                    foreach (var routine in aoiDefinition.Routines)
                    {
                        PropertyChangedEventManager.AddHandler(routine,
                            OnRoutinePropertyChanged, string.Empty);
                    }
                }

                foreach (var dataType in controller.DataTypes)
                {
                    PropertyChangedEventManager.AddHandler(dataType, OnDataTypePropertyChanged, string.Empty);
                }

                foreach (var module in controller.DeviceModules)
                {
                    PropertyChangedEventManager.AddHandler(module, OnModulePropertyChanged, string.Empty);
                }
            }
        }

        private void DetachController(Controller controller)
        {
            if (controller != null)
            {
                foreach (var module in controller.DeviceModules)
                {
                    PropertyChangedEventManager.RemoveHandler(module, OnModulePropertyChanged, string.Empty);
                }

                foreach (var dataType in controller.DataTypes)
                {
                    PropertyChangedEventManager.RemoveHandler(dataType, OnDataTypePropertyChanged, string.Empty);
                }

                foreach (var program in controller.Programs)
                {
                    foreach (var tag in program.Tags)
                    {
                        PropertyChangedEventManager.RemoveHandler(tag, OnTagPropertyChanged, string.Empty);
                    }

                    foreach (var routine in program.Routines)
                    {
                        PropertyChangedEventManager.RemoveHandler(routine,
                            OnRoutinePropertyChanged, string.Empty);
                    }

                    CollectionChangedEventManager.RemoveHandler(program.Routines, OnRoutinesChanged);
                    CollectionChangedEventManager.RemoveHandler(program.Tags, OnTagsChanged);

                    PropertyChangedEventManager.RemoveHandler(program, OnProgramPropertyChanged, string.Empty);
                }

                foreach (var aoiDefinition in controller.AOIDefinitionCollection)
                {
                    PropertyChangedEventManager.RemoveHandler(aoiDefinition, OnAOIDefinitionPropertyChanged,
                        string.Empty);

                    foreach (var routine in aoiDefinition.Routines)
                    {
                        PropertyChangedEventManager.RemoveHandler(routine,
                            OnRoutinePropertyChanged, string.Empty);
                    }

                    CollectionChangedEventManager.RemoveHandler(aoiDefinition.Routines, OnRoutinesChanged);
                    CollectionChangedEventManager.RemoveHandler(aoiDefinition.Tags, OnTagsChanged);
                }

                foreach (ITask task in controller.Tasks)
                {
                    PropertyChangedEventManager.RemoveHandler(task, OnTaskPropertyChanged, string.Empty);
                }

                foreach (var tag in controller.Tags)
                {
                    PropertyChangedEventManager.RemoveHandler(tag, OnTagPropertyChanged, string.Empty);
                }

                CollectionChangedEventManager.RemoveHandler(controller.Tasks, OnTasksChanged);
                CollectionChangedEventManager.RemoveHandler(controller.Programs, OnProgramsChanged);
                CollectionChangedEventManager.RemoveHandler(controller.Tags, OnTagsChanged);
                CollectionChangedEventManager.RemoveHandler(controller.AOIDefinitionCollection,
                    OnAOIDefinitionsChanged);
                CollectionChangedEventManager.RemoveHandler(controller.DataTypes, OnDataTypesChanged);
                CollectionChangedEventManager.RemoveHandler(controller.Trends, OnTrendsChanged);
                CollectionChangedEventManager.RemoveHandler(controller.DeviceModules, OnDeviceModulesChanged);
            }
        }

        private void SetProjectDirty()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                IProjectInfoService projectInfoService =
                    Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;

                projectInfoService?.SetProjectDirty();
            });
        }

        private void ResetChangeLog([CallerMemberName] string memberName = "")
        {
            Controller myController = Controller as Controller;

            if (myController != null && !myController.IsConnected)
            {
                var transactionManager =
                    _controller.Lookup(typeof(SimpleServices.TransactionManager)) as
                        SimpleServices.TransactionManager;

                Logger.Info($"TransactionManager reset in {memberName}!");

                transactionManager?.Reset();
            }
        }

        private void OnDeviceModulesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResetChangeLog();

            SetProjectDirty();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var module in e.NewItems.OfType<IDeviceModule>())
                    {
                        PropertyChangedEventManager.AddHandler(module, OnModulePropertyChanged, string.Empty);

                        Logger.Trace($"Add module: {module.DisplayText}");
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var module in e.OldItems.OfType<IDeviceModule>())
                    {
                        PropertyChangedEventManager.RemoveHandler(module, OnModulePropertyChanged, string.Empty);
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnTrendsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResetChangeLog();

            SetProjectDirty();
        }

        private void OnDataTypesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResetChangeLog();

            SetProjectDirty();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var dataType in e.NewItems.OfType<IDataType>())
                    {
                        PropertyChangedEventManager.AddHandler(dataType, OnDataTypePropertyChanged, string.Empty);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var dataType in e.OldItems.OfType<IDataType>())
                    {
                        PropertyChangedEventManager.RemoveHandler(dataType, OnDataTypePropertyChanged, string.Empty);
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnAOIDefinitionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResetChangeLog();

            SetProjectDirty();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var aoiDefinition in e.NewItems.OfType<IAoiDefinition>())
                    {
                        PropertyChangedEventManager.AddHandler(aoiDefinition, OnAOIDefinitionPropertyChanged,
                            string.Empty);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var aoiDefinition in e.OldItems.OfType<IAoiDefinition>())
                    {
                        PropertyChangedEventManager.RemoveHandler(aoiDefinition, OnAOIDefinitionPropertyChanged,
                            string.Empty);
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnTagsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ITagCollection tagCollection = sender as ITagCollection;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var tag in e.NewItems.OfType<Tag>())
                    {
                        PropertyChangedEventManager.AddHandler(tag, OnTagPropertyChanged, string.Empty);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var tag in e.OldItems.OfType<Tag>())
                    {
                        PropertyChangedEventManager.RemoveHandler(tag, OnTagPropertyChanged, string.Empty);
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // offline edit
            if (tagCollection != null)
            {
                Controller myController = Controller as Controller;
                if (myController != null && !myController.IsConnected)
                {
                    ResetChangeLog();
                }
            }

            //online edit
            if (tagCollection != null && tagCollection.IsControllerScoped)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (Controller != null && Controller.IsOnline)
                    {
                        Controller myController = Controller as Controller;

                        if (myController != null)
                        {
                            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                            {
                                await TaskScheduler.Default;

                                OnlineEditHelper helper = new OnlineEditHelper(myController.CipMessager);
                                foreach (var tag in e.NewItems.OfType<Tag>())
                                {
                                    try
                                    {
                                        await helper.CreateTagInController(tag.ConvertToJObject());

                                        var tagSyncController =
                                            myController.Lookup(typeof(TagSyncController)) as TagSyncController;
                                        if (tagSyncController != null)
                                        {
                                            await tagSyncController.AddTagInController(tag);
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        //TODO(gjc): remove tag later
                                        Logger.Error($"Online add tag failed:{tag.Name} {exception}");
                                    }
                                }
                            });
                        }


                    }

                }
            }

            IProgram program = tagCollection?.ParentProgram as IProgram;

            if (program != null)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    Controller myController = Controller as Controller;
                    if (myController != null && myController.IsOnline)
                    {
                        ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                        {
                            await TaskScheduler.Default;

                            OnlineEditHelper helper = new OnlineEditHelper(myController.CipMessager);
                            foreach (var tag in e.NewItems.OfType<Tag>())
                            {
                                try
                                {
                                    await helper.CreateTagInProgram(tag.ConvertToJObject(), program);

                                    var tagSyncController =
                                        myController.Lookup(typeof(TagSyncController)) as TagSyncController;
                                    if (tagSyncController != null)
                                    {
                                        await tagSyncController.AddTagInProgram(tag, program);
                                    }
                                }
                                catch (Exception exception)
                                {
                                    //TODO(gjc): remove tag later
                                    Logger.Error($"Online add tag failed:{tag.Name} {exception}");
                                }
                            }
                        });
                    }
                }
            }

            SetProjectDirty();
        }

        private void OnProgramsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResetChangeLog();

            SetProjectDirty();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var program in e.NewItems.OfType<IProgram>())
                    {
                        CollectionChangedEventManager.AddHandler(program.Routines, OnRoutinesChanged);
                        CollectionChangedEventManager.AddHandler(program.Tags, OnTagsChanged);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var program in e.OldItems.OfType<IProgram>())
                    {
                        CollectionChangedEventManager.RemoveHandler(program.Routines, OnRoutinesChanged);
                        CollectionChangedEventManager.RemoveHandler(program.Tags, OnTagsChanged);
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    // ScheduleProgram

                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnTasksChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResetChangeLog();

            SetProjectDirty();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var task in e.NewItems.OfType<ITask>())
                    {
                        PropertyChangedEventManager.AddHandler(task,
                            OnTaskPropertyChanged, string.Empty);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var task in e.OldItems.OfType<ITask>())
                    {
                        PropertyChangedEventManager.RemoveHandler(task,
                            OnTaskPropertyChanged, string.Empty);
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void OnRoutinesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResetChangeLog();

            SetProjectDirty();

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var routine in e.NewItems.OfType<IRoutine>())
                    {
                        PropertyChangedEventManager.AddHandler(routine,
                            OnRoutinePropertyChanged, string.Empty);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var routine in e.OldItems.OfType<IRoutine>())
                    {
                        PropertyChangedEventManager.RemoveHandler(routine,
                            OnRoutinePropertyChanged, string.Empty);
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnRoutinePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IRoutine routine = sender as IRoutine;
            if (routine == null)
                return;

            List<string> checkPropertyList = new List<string>()
            {
                //ST
                "Name",
                "CodeText",
                "Type",
                "Description",
                "PendingCodeText",
                "TestCodeText",
                "EncodedData",
            };

            if (checkPropertyList.Contains(e.PropertyName))
            {
                ResetChangeLog();

                SetProjectDirty();
            }

        }

        #endregion

        private void OnControllerChanged(Controller oldController, Controller newController)
        {
            if (oldController != null)
            {
                DetachController(oldController);
                PropertyChangedEventManager.RemoveHandler(oldController, OnControllerPropertyChanged, string.Empty);

                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    oldController, "IsOnlineChanged", OnIsOnlineChanged);
            }

            if (newController != null)
            {
                AttachController(newController);
                PropertyChangedEventManager.AddHandler(newController, OnControllerPropertyChanged, string.Empty);

                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    newController, "IsOnlineChanged", OnIsOnlineChanged);
            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (!e.NewValue && !_isInOffline)
                {
                    string warningMessage = "Can't communicate with controller.";
                    string warningReason =
                        "ICS Studio has been taken offline." +
                        "\n\nIf online operations are in progress, they will not be completed." +
                        "\n\nOffline operations after communication loss will result in loss of correlation. " +
                        "To avoid loss of correlation, close views without applying changes before attempting to go online." +
                        "\n\n";
                    string errorCode = "Error 806-80042535";

                    var warningDialog =
                        new Dialogs.Warning.WarningDialog(warningMessage, warningReason, errorCode)
                            { Owner = Application.Current.MainWindow };
                    warningDialog.ShowDialog();
                }

            });
        }

        private void OnControllerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ProjectCommunicationPath")
            {
                if (!string.IsNullOrEmpty(_controller.ProjectCommunicationPath))
                {
                    RecentCommPath = _controller.ProjectCommunicationPath;
                }
            }

            Controller myController = Controller as Controller;
            if (myController != null && !myController.IsConnected)
            {
                //TODO(gjc): edit later
                if (e.PropertyName == "TimeSetting.PTPEnable")
                {
                    ResetChangeLog();
                }
            }
        }
    }
}
