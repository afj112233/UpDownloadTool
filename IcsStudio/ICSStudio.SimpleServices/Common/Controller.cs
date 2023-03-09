using System;
using System.Collections.Generic;
using System.ComponentModel;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DataType;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.DataType;
using ICSStudio.SimpleServices.Instruction;
using ICSStudio.SimpleServices.Tags;
using NLog;
using System.Diagnostics.Contracts;
using System.Linq;
using ICSStudio.Cip.EtherNetIP;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.SimpleServices.Compiler;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ICSStudio.Cip.Objects;
using ICSStudio.SimpleServices.Annotations;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.SimpleServices.Online;
using ICSStudio.SimpleServices.SourceProtection;
using ICSStudio.Utils;
using Type = System.Type;

namespace ICSStudio.SimpleServices.Common
{
    public partial class Controller : IController
    {
        private static Controller _instance;

        // ReSharper disable once UnusedMember.Local
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private string _name;
        private string _description;
        private string _projectCommunicationPath;

        private readonly DeviceModuleCollection _deviceModuleCollection;
        private readonly ProgramCollection _programCollection;
        private readonly TagCollection _tagCollection;
        private readonly TaskCollection _taskCollection;
        private readonly DataTypeCollection _dataTypeCollection;
        private readonly STInstructionCollection _stInstructionCollection;
        private readonly RLLInstructionCollection _rllInstructionCollection;
        private readonly FBDInstructionCollection _fbdInstructionCollection;

        private readonly AoiDefinitionCollection _aoiDefinitionCollection;

        private readonly TrendCollection _trends;

        private readonly ParameterConnectionCollection _parameterConnections;

        private ICipMessager _connection;

        private readonly TagSyncController _tagSyncController;
        private readonly AxisLogController _axisLogController;

        private readonly ControllerStateManager _stateManager;

        private readonly ModuleConnectionManager _moduleConnectionManager;

        private readonly TransactionManager _transactionManager;
        
        //private Ctrl

        private Controller()
        {
            _trends = new TrendCollection(this);
            _deviceModuleCollection = new DeviceModuleCollection(this);
            _programCollection = new ProgramCollection(this);
            _tagCollection = new TagCollection(this);
            _taskCollection = new TaskCollection(this);
            _dataTypeCollection = new DataTypeCollection(this);
            _stInstructionCollection = new STInstructionCollection(this);
            _rllInstructionCollection = new RLLInstructionCollection(this);
            _fbdInstructionCollection = new FBDInstructionCollection(this);
            _aoiDefinitionCollection = new AoiDefinitionCollection();
            _parameterConnections = new ParameterConnectionCollection(this);

            Uid = Guid.NewGuid().GetHashCode();
            Description = string.Empty;

            IsVerified = true;
            IsDeleted = false;
            InstanceNumber = 1;
            IsSafety = false;
            IsTypeLess = false;
            KeySwitchPosition = ControllerKeySwitch.NullKeySwitch;
            LockState = ControllerLockState.NotLocked;
            SupportsEquipmentSequenceEventGeneration = false;
            IsLanguageSwitchingEnabled = false;
            IsForceEnabled = false;

            ProjectHasCustomProperties = false;
            ValueConverter = null;

            _tagSyncController = new TagSyncController(this);
            _axisLogController = new AxisLogController(this);
            _stateManager = new ControllerStateManager(this);
            _moduleConnectionManager = new ModuleConnectionManager(this);
            TimeSetting = new TimeSetting();
            SourceProtectionManager = new SourceProtectionManager(this);
            _transactionManager = new TransactionManager(this);
            
            //初步实现备份功能
            if (System.Windows.Application.Current != null)
                System.Windows.Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }
        
        public void Log(string message)
        {
            Logger.Error(message);
        }

        private void Current_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error($"ics studio crash: {e.Exception}");
            ExportBackup();
        }

        public static Controller GetInstance()
        {
            return _instance ?? (_instance = new Controller());
        }

        public void Dispose()
        {
        }

        public IController ParentController => this;

        public int Uid { get; }

        public bool IsVerified { get; }
        public bool IsDeleted { get; }
        public int ParentProgramUid => -1;
        public int ParentRoutineUid => -1;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged();
            }
        }

        public int InstanceNumber { get; }
        public bool IsSafety { get; }
        public bool IsTypeLess { get; }

        public ITagCollection Tags => _tagCollection;
        public IProgramCollection Programs => _programCollection;
        public ITaskCollection Tasks => _taskCollection;
        public IDataTypeCollection DataTypes => _dataTypeCollection;
        public IDeviceModuleCollection DeviceModules => _deviceModuleCollection;
        public ITrendCollection Trends => _trends;
        public IParameterConnectionCollection ParameterConnections => _parameterConnections;

        public STInstructionCollection STInstructionCollection => _stInstructionCollection;
        public RLLInstructionCollection RLLInstructionCollection => _rllInstructionCollection;
        public FBDInstructionCollection FBDInstructionCollection => _fbdInstructionCollection;

        public string MajorFaultProgram { set; get; }
        public string PowerLossProgram { set; get; }
        public IAoiDefinitionCollection AOIDefinitionCollection => _aoiDefinitionCollection;

        public ICipMessager CipMessager
        {
            get { return _connection; }
            set
            {
                Contract.Assert(_connection == null);
                _connection = value;
                if (_connection != null)
                {
                    _connection.Connected += OnConnected;
                    _connection.Disconnected += OnDisconnected;
                }
            }
        }

        public string ProjectCommunicationPath
        {
            get { return _projectCommunicationPath; }
            set
            {
                if (_projectCommunicationPath != value)
                {
                    _projectCommunicationPath = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool SupportsEquipmentSequenceEventGeneration { get; }
        public bool IsLanguageSwitchingEnabled { get; }
        public bool IsForceEnabled { get; }
        public string ProjectLocaleName { get; set; }
        public string DefaultLocaleName { get; set; }
        public bool ProjectHasCustomProperties { get; }
        public IValueConverter ValueConverter { get; }

        private byte[] NativeCode { get; set; }

        public SourceProtectionManager SourceProtectionManager { get; }

        public TimeSetting TimeSetting { get; }

        public event EventHandler Loaded;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IDataServer CreateDataServer()
        {
            return CreateDataServer(false);
        }

        public IDataServer CreateDataServer(bool asynchronousDataUpdate)
        {
            //TODO(gjc): edit code here
            return new DataServer(this);
        }

        public IDataServer CreatePendingDataServer(bool monitorValue, bool monitorAttribute)
        {
            return CreatePendingDataServer(monitorValue, monitorAttribute, true);
        }

        public IDataServer CreatePendingDataServer(bool monitorValue, bool monitorAttribute,
            bool asynchronousDataUpdate)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UploadTagValues()
        {
            int result = await _tagSyncController.PullAllTagsAsync();
            return result;
        }

        public void Clear(bool needCleanDataType = true)
        {
            ProjectLocaleName = null;
            Name = null;
            MajorFaultProgram = null;
            PowerLossProgram = null;
            EtherNetIPMode = "";
            ((ParameterConnectionCollection)ParameterConnections).Clean();

            if (needCleanDataType)
            {
                _dataTypeCollection.RemoveNoPredefinedType();
                
                foreach (var aoi in _aoiDefinitionCollection)
                {
                    STInstructionCollection.RemoveInstruction(aoi.instr);
                    RLLInstructionCollection.RemoveInstruction(aoi.instr);
                    FBDInstructionCollection.RemoveInstruction(aoi.instr);
                }

                _aoiDefinitionCollection.Clear();
            }

            _deviceModuleCollection.Clear();
            _programCollection.Clear();
            _tagCollection.Clear();
            _taskCollection.Clear();
            _trends.Clear();

            _transactionManager.Reset();
        }

        public void GenCode()
        {
            //foreach (var item in _programCollection)
            //{
            //    var program = (Program) item;
            //    program?.GenCode();
            //}
            Parallel.ForEach(_programCollection, item =>
            {
                var program = (Program)item;
                program?.GenCode();
            });

            //foreach (var def in _aoiDefinitionCollection)
            //{
            //    def.GenCode(this);
            //}

            Parallel.ForEach<AoiDefinition>(_aoiDefinitionCollection,
                item => { item.GenCode(this); });

            GenNativeCode();

            GenSepNativeCode();
        }

        public byte[] GenerateNativeCodeForAction(Action<OutputStream> action)
        {
            var sourcePrefix = Path.GetTempFileName();
            var objectPrefix = Path.GetTempFileName();
            string sourcePath = sourcePrefix + ".icon.c";
            string objectPath = objectPrefix + ".icon.o";
            string linkPath = objectPrefix + ".icon.l.o";

            // create c file
            OutputStream writer = new OutputStream(sourcePath);

            writer.WriteLine("#include <ivmexport.h>");
            writer.WriteLine("#include <ivmbytecodeimpl.h>");

            action(writer);

            writer.Close();


            // build
            BuildCFile(sourcePath, objectPath, linkPath);


            var res = File.ReadAllBytes(linkPath);

            try
            {
                File.Delete(sourcePrefix);
                File.Delete(objectPrefix);
                File.Delete(sourcePath);
                File.Delete(objectPath);
                File.Delete(linkPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return res;

        }

        private void GenNativeCode()
        {
            var sourcePrefix = Path.GetTempFileName();
            var objectPrefix = Path.GetTempFileName();
            string sourcePath = sourcePrefix + ".icon.c";
            string objectPath = objectPrefix + ".icon.o";
            string linkPath = objectPrefix + ".icon.l.o";

            // create c file
            CreateCFile(sourcePath);

            // build
            BuildCFile(sourcePath, objectPath, linkPath);

            // read
            NativeCode = File.ReadAllBytes(linkPath);

            try
            {
                File.Delete(sourcePrefix);
                File.Delete(objectPrefix);
                File.Delete(sourcePath);
                File.Delete(objectPath);
                File.Delete(linkPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void CreateCFile(string fileName)
        {
            OutputStream writer = new OutputStream(fileName);

            writer.WriteLine("#include <ivmexport.h>");
            writer.WriteLine("#include <ivmbytecodeimpl.h>");
            /*
            foreach (var item in _programCollection)
            {
                var program = (Program) item;
                program?.GenNativeCode(writer);
            }
            */

            foreach (var def in _aoiDefinitionCollection)
            {
                def.GenNativeCode(this, writer);
            }

            writer.Close();
        }

        private void GenSepNativeCode()
        {
            //foreach (var item in _programCollection)
            //{
            //    var program = (Program) item;
            //    program?.GenSepNativeCode(this);
            //}

            Parallel.ForEach(_programCollection, item =>
            {
                var program = (Program)item;
                program?.GenSepNativeCode(this);
            });

            //AOI的机器码暂时不需要的。直接从全局的NativeCode里获取
            /*
            foreach (var def in _aoiDefinitionCollection)
            {
                def.GenSepNativeCode(this);
            }
            */
        }

        enum CompileTarget
        {
            Arm,
            X64,
            Aarch64,
        };

        // ReSharper disable once InconsistentNaming
        //private const CompileTarget compileTarget = CompileTarget.Aarch64;

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
        private string GenCompileCommand(CompileTarget target, string sourcePath, string objectPath)
        {
            string toolchainPath = Environment.GetEnvironmentVariable("ICON_TOOLCHAIN_PATH");

            var root = AssemblyUtils.AssemblyDirectory;

            if (string.IsNullOrEmpty(toolchainPath))
                toolchainPath = $"{root}\\Toolchains";

            string includePath = $"\"{root}\\Toolchains\\include\"";

            if (target == CompileTarget.Arm)
            {
                string ccPath =
                    $"\"{toolchainPath}\\arm-linux-androideabi-4.9\\prebuilt\\windows-x86_64\\bin\\arm-linux-androideabi-gcc.exe\"";
                return
                    $"{ccPath} -I{includePath} -fno-PIE -Wno-unused-label -Wno-unused-function -Wno-unused-variable -marm  -march=armv7-a  -mfloat-abi=hard  -mfpu=neon  -fno-common -Wall  -O2 -c {sourcePath} -o {objectPath}";
            }
#pragma warning disable 162
            else if (target == CompileTarget.X64)
            {
                string ccPath =
                    $"\"{toolchainPath}\\x86_64-4.9\\prebuilt\\windows-x86_64\\bin\\x86_64-linux-android-gcc.exe\"";
                return
                    $"{ccPath} -I{includePath} -fno-PIE -Wno-unused-label -mcmodel=large -Wno-unused-function -Wno-unused-variable  -fno-common -Wall  -O2 -c {sourcePath} -o {objectPath}";
            }
            else if (target == CompileTarget.Aarch64)
            {
                string ccPath =
                    $"\"{toolchainPath}\\aarch64-linux-android-4.9\\prebuilt\\windows-x86_64\\bin\\aarch64-linux-android-gcc.exe\"";
                return
                    $"{ccPath} -I{includePath} -fno-PIE -Wno-unused-label -mcmodel=large  -fno-asynchronous-unwind-tables  -fno-unwind-tables  -Wno-unused-function -Wno-unused-variable  -fno-common -Wall  -O2 -c {sourcePath} -o {objectPath}";
            }
            else
            {
                Debug.Assert(false, target.ToString());
                return "";
            }
#pragma warning restore 162
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
        private string GenLinkCommand(CompileTarget target, string objectPath, string linkPath)
        {
            string toolchainPath = Environment.GetEnvironmentVariable("ICON_TOOLCHAIN_PATH");
            var root = AssemblyUtils.AssemblyDirectory;

            if (string.IsNullOrEmpty(toolchainPath))
                toolchainPath = $"{root}\\Toolchains";

            if (target == CompileTarget.Arm)
            {
                string ldsPath = $"\"{root}\\Toolchains\\include\\module.lds\"";
                string ldPath =
                    $"\"{toolchainPath}\\arm-linux-androideabi-4.9\\prebuilt\\windows-x86_64\\bin\\arm-linux-androideabi-ld.exe\"";

                return $"{ldPath} -r -EL -T {ldsPath} -o {linkPath} {objectPath}";
            }
#pragma warning disable 162
            else if (target == CompileTarget.X64)
            {
                string ldsPath = $"\"{root}\\Toolchains\\include\\module.x64.lds\"";
                string ldPath =
                    $"\"{toolchainPath}\\x86_64-4.9\\prebuilt\\windows-x86_64\\bin\\x86_64-linux-android-ld.exe\"";

                return $"{ldPath} -r -EL -T {ldsPath} -o {linkPath} {objectPath}";
            }
            else if (target == CompileTarget.Aarch64)
            {
                string ldsPath = $"\"{root}\\Toolchains\\include\\module.aarch64.lds\"";
                string ldPath =
                    $"\"{toolchainPath}\\aarch64-linux-android-4.9\\prebuilt\\windows-x86_64\\bin\\aarch64-linux-android-ld.exe\"";

                return $"{ldPath} -r -EL -T {ldsPath} -o {linkPath} {objectPath}";
            }
            else
            {
                Debug.Assert(false, target.ToString());
                return "";
            }
#pragma warning restore 162

        }

        int Code()
        {
            LocalModule localModule = DeviceModules["Local"] as LocalModule;
            Debug.Assert(localModule != null, "");
            return localModule.ProductCode;

        }

        enum ProductCode
        {
            BASIC = 108,
            PRO = 408,
            TURBO = 608,
        }

        CompileTarget GetCompileTarget()
        {
            ProductCode code = (ProductCode)Code();
            switch (code)
            {
                case ProductCode.BASIC:
                    return CompileTarget.Arm;
                case ProductCode.PRO:
                    return CompileTarget.Aarch64;
                case ProductCode.TURBO:
                    return CompileTarget.X64;
                default:
                    //Debug.Assert(false);
                    return CompileTarget.Aarch64;
            }
        }

        private void BuildCFile(string sourcePath, string objectPath, string linkPath)
        {
            var target = GetCompileTarget();
            sourcePath = $"\"{sourcePath}\"";
            objectPath = $"\"{objectPath}\"";
            linkPath = $"\"{linkPath}\"";
            using (Process executor = new Process())
            {
                executor.StartInfo.FileName = "cmd.exe";
                executor.StartInfo.UseShellExecute = false;
                executor.StartInfo.RedirectStandardInput = true;
                executor.StartInfo.RedirectStandardOutput = true;
                executor.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                executor.StartInfo.CreateNoWindow = true;

                executor.Start();

                // add path
                // gcc
                var cmd = GenCompileCommand(target, sourcePath, objectPath);
                executor.StandardInput.WriteLine(cmd);
                // link
                executor.StandardInput.WriteLine(GenLinkCommand(target, objectPath, linkPath));

                executor.StandardInput.Flush();
                executor.StandardInput.Close();

                string output = executor.StandardOutput.ReadToEnd();

                executor.WaitForExit();



                if (executor.ExitCode != 0)
                {
                    throw new NotImplementedException($"{output}:{output}");
                }
            }
        }


        #region Online and Offline

        public void GoOffline()
        {
            try
            {
                if (_connection != null)
                {
                    _connection.OffLine();
                    _connection.Connected -= OnConnected;
                    _connection.Disconnected -= OnDisconnected;
                    _connection = null;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                IsOnline = false;
            }
        }

        public void GoOnline()
        {
            throw new NotImplementedException();
            //TODO(gjc): add code here
            //GoOnlineAsync().GetAwaiter().GetResult();
        }

        //private string GetIpAddress()
        //{
        //    Contract.Assert(!string.IsNullOrEmpty(ProjectCommunicationPath));

        //    return ProjectCommunicationPath;
        //}

        #endregion

        #region NotImplemented

        public void BeginTransactionSet()
        {
            throw new NotImplementedException();
        }

        public void EndTransactionSet()
        {
            throw new NotImplementedException();
        }

        public void CancelTransactionSet()
        {
            throw new NotImplementedException();
        }

        public IBaseComponent GetComponent(int uid)
        {
            throw new NotImplementedException();
        }

        public void CheckAccess(AccessType access)
        {
            throw new NotImplementedException();
        }

        public void ReportOfflineChange()
        {
            throw new NotImplementedException();
        }

        public IProgramModule GetProgram(int programType, int programUid)
        {
            throw new NotImplementedException();
        }

        public IOperandParser CreateOperandParser()
        {
            throw new NotImplementedException();
        }

        public ITransactionService CreateTransactionService()
        {
            throw new NotImplementedException();
        }

        public bool ExistsComponent(ComponentType componentType, string name)
        {
            throw new NotImplementedException();
        }

        public bool ExistsComponent(IBaseComponent parentComponent, ComponentType childComponentType, string name)
        {
            throw new NotImplementedException();
        }

        public void InitializeCurrentThread()
        {
            throw new NotImplementedException();
        }

        public void ShutDownCurrentThread()
        {
            throw new NotImplementedException();
        }

        public bool IsAlarmSpecifier(string operandText)
        {
            throw new NotImplementedException();
        }

        public bool IsAlarmSetSpecifier(string operandText)
        {
            throw new NotImplementedException();
        }

        public bool HasAlarmSetDefined(string alarmSetTargetQualifiedPath, int containerComponentUid)
        {
            throw new NotImplementedException();
        }

        public bool IsFeatureSupported(ControllerFeature feature)
        {
            throw new NotImplementedException();
        }

        public int TimeSlice { get; set; }
        public int ProjectSN { get; set; }
        public DateTime ProjectCreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public bool MatchProjectToController { get; set; }

        public string EtherNetIPMode { get; set; }

        public bool IsLoading { get; set; }
        public bool IsPass { get; set; } = true;
        public bool CanVerify { get; set; } = true;

        //TODO(gjc): need move to project
        public bool IsDownloading { set; get; } = false;

        public bool IsDescriptionDefaultLocale()
        {
            throw new NotImplementedException();
        }

        public Language[] GetDescriptionTranslations()
        {
            throw new NotImplementedException();
        }

        #endregion NotImplemented

        private void VerifyAliasTags()
        {
            foreach (var tag in Tags.OfType<Tag>())
            {
                if (tag.IsAlias)
                    tag.Verify();
            }

            foreach (var program in Programs)
            {
                foreach (var tag in program.Tags.OfType<Tag>())
                {
                    if (tag.IsAlias)
                        tag.Verify();
                }
            }

            foreach (var aoiDefinition in AOIDefinitionCollection)
            {
                foreach (var tag in aoiDefinition.Tags.OfType<Tag>())
                {
                    if (tag.IsAlias)
                        tag.Verify();
                }
            }
        }

        public object Lookup(Type type)
        {
            if (type == typeof(TagSyncController))
                return _tagSyncController;

            if (type == typeof(AxisLogController))
                return _axisLogController;

            if (type == typeof(TransactionManager))
                return _transactionManager;

            return null;
        }

        public string GetFileName(Controller controller)
        {
            string add = controller.ProjectLocaleName;
            string fileName = "";
            bool flag = false;
            for (int i = add.Length - 1; i >= 0; i--)
            {
                if (add[i] == '.')
                {
                    flag = true;
                }

                if (flag)
                {
                    var yy = add[i];
                    if (add[i] != '\\')
                    {
                        if (add[i] != '.')
                            fileName = add[i] + fileName;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return fileName;
        }

        public static string DescriptionConvert(string input)
        {
            StringBuilder result = new StringBuilder();

            foreach (var t in input)
            {
                if (t.Equals('$'))
                {
                    result.Append("$$");
                }
                else if (t.Equals('\''))
                {
                    result.Append("$'");
                }
                else if (t.Equals('\"'))
                {
                    result.Append("$Q");
                }
                else if (t.Equals('\\'))
                {
                    result.Append("\\");
                }
                else if (t < 127)
                {
                    result.Append(t.ToString());
                }
                else
                {
                    result.Append(($"${(int)t:x4}").ToUpper());
                }
            }

            result = result.Replace("\r\n", "{$}N").Replace("\t", "{$}T");

            return result.ToString().Replace("{$}", "$");
        }

        public static string ExportAllTags(ITagCollection tagCollection, string separator, string scope,
            StringBuilder stringBuilder, string type)
        {
            if (stringBuilder.Length == 0)
            {
                DateTime dateNum = DateTime.Now;

                List<string> header1 = new List<string>() { "remark", "CSV-Import-Export" };
                List<string> header2 = new List<string>()
                {
                    "remark",
                    "Date = " + dateNum.ToString("ddd MMM dd HH:mm:ss yyyy", CultureInfo.CreateSpecificCulture("en-GB"))
                };
                List<string> header3 = new List<string>() { "remark", "Version = ICSStudio" };
                List<string> header4 = new List<string>() { "remark", "Owner = " };
                List<string> header5 = new List<string>() { "remark", "Company = " };
                List<string> header6 = new List<string>() { "0.3" };
                List<string> header7 = new List<string>()
                {
                    "TYPE", "SCOPE", "NAME", "DESCRIPTION", "DATATYPE", "SPECIFIER", "ATTRIBUTES"
                };
                stringBuilder.AppendLine(string.Join(separator, header1));
                stringBuilder.AppendLine(string.Join(separator, header2));
                stringBuilder.AppendLine(string.Join(separator, header3));
                stringBuilder.AppendLine(string.Join(separator, header4));
                stringBuilder.AppendLine(string.Join(separator, header5));
                stringBuilder.AppendLine(string.Join(separator, header6));
                stringBuilder.AppendLine(string.Join(separator, header7));
            }

            // tag
            foreach (var tag in tagCollection)
            {
                string description = tag.Description;
                if (description != null)
                {
                    description = DescriptionConvert(tag.Description);
                }

                //if Tag is AxisType
                if (tag.DataTypeInfo.DataType.IsAxisType)
                {
                    var _axisVirtual = ((Tag)tag).DataWrapper as AxisVirtual;
                    var _axisCIPDrive = ((Tag)tag).DataWrapper as AxisCIPDrive;
                    var _axis = new CIPAxis(0, null);
                    string motionGroupName = "<NA>";
                    string motionModuleName = "<NA>";

                    if (_axisVirtual != null)
                    {
                        _axis = _axisVirtual.CIPAxis;
                        if (_axisVirtual.AssignedGroup != null)
                        {
                            motionGroupName = _axisVirtual.AssignedGroup.Name;
                        }

                        //目前在AxisVirtual中没有找到Module相关信息
                    }

                    if (_axisCIPDrive != null)
                    {
                        _axis = _axisCIPDrive.CIPAxis;
                        if (_axisCIPDrive.AssignedGroup != null)
                        {
                            motionGroupName = _axisCIPDrive.AssignedGroup.Name;
                        }

                        if (_axisCIPDrive.AssociatedModule != null)
                        {
                            motionModuleName = _axisCIPDrive.AssociatedModule.Name;
                        }
                    }

                    List<string> list = new List<string>()
                    {
                        "TAG",
                        scope,
                        tag.Name,
                        $"\"{description}\"",
                        $"\"{tag.DataTypeInfo}\"",
                        ""
                    };

                    StringBuilder attributes = new StringBuilder($"\"(ExternalAccess := {tag.ExternalAccess}, ");

                    switch (type)
                    {
                        case "Program":
                            attributes.Append($"Usage := {tag.Usage}, ");
                            break;
                        case "AOI":
                            attributes.Append($"Usage := {tag.Usage}, " +
                                              $"Required := {tag.IsRequired}, " +
                                              $"Visible := {tag.IsVisible}, ");
                            break;
                    }

                    attributes.Append($"MotionGroup := {motionGroupName}, " +
                                      $"MotionModule := {motionModuleName}, " +
                                      $"ConversionConstant := {_axis.ConversionConstant}, " +
                                      $"OutputCamExecutionTargets := {Convert.ToInt32(_axis.OutputCamExecutionTargets)}, " +
                                      $"PositionUnits := {_axis.PositionUnits}, " +
                                      $"AverageVelocityTimebase := {_axis.AverageVelocityTimebase}, " +
                                      $"RotaryAxis := {_axis.RotaryMotorInertia}, " +
                                      $"PositionUnwind := {Convert.ToInt32(_axis.PositionUnwind)}, " +
                                      $"HomeMode := {Convert.ToInt32(_axis.HomeMode)}, " +
                                      $"HomeDirection := {Convert.ToInt32(_axis.HomeDirection)}, " +
                                      $"HomeSequence := {Convert.ToInt32(_axis.HomeSequence)}, " +
                                      $"HomeConfigurationBits := {Convert.ToInt32(_axis.HomeConfigurationBits)}, " +
                                      $"HomePosition := {_axis.HomePosition}, " +
                                      $"HomeOffset := {_axis.HomeOffset}, " +
                                      $"MaximumSpeed := {_axis.MaximumSpeed}, " +
                                      $"MaximumAcceleration := {_axis.MaximumAcceleration}, " +
                                      $"MaximumDeceleration := {_axis.MaximumDeceleration}, " +
                                      $"ProgrammedStopMode := {Convert.ToInt32(_axis.ProgrammedStopMode)}, " +
                                      $"MasterInputConfigurationBits := {Convert.ToInt32(_axis.MasterInputConfigurationBits)}, " +
                                      $"MasterPositionFilterBandwidth := {Convert.ToInt32(_axis.MasterPositionFilterBandwidth)}, " +
                                      $"MaximumAccelerationJerk := {_axis.MaximumAccelerationJerk}, " +
                                      $"MaximumDecelerationJerk := {_axis.MaximumDecelerationJerk}, " +
                                      $"DynamicsConfigurationBits := {Convert.ToInt32(_axis.DynamicsConfigurationBits)}, " +
                                      $"InterpolatedPositionConfiguration := {Convert.ToInt32(_axis.InterpolatedPositionConfiguration)}, " +
                                      $"AxisUpdateSchedule := {Convert.ToInt32(_axis.AxisUpdateSchedule)}, " +
                                      $"AxisConfiguration := {Convert.ToInt32(_axis.AxisConfiguration)}, " +
                                      $"ControlMethod := {Convert.ToInt32(_axis.ControlMethod)}, " +
                                      $"ControlMode := {Convert.ToInt32(_axis.ControlMode)}, " +
                                      $"FeedbackConfiguration := {Convert.ToInt32(_axis.FeedbackConfiguration)}, " +
                                      $"FeedbackMode := {Convert.ToInt32(_axis.FeedbackMode)}, " +
                                      $"MotorDataSource := {Convert.ToInt32(_axis.MotorDataSource)}, " +
                                      $"MotorCatalogNumber := {Convert.ToString(_axis.MotorCatalogNumber.GetString())}, " +
                                      $"Feedback1Type := {Convert.ToInt32(_axis.Feedback1Type)}, " +
                                      $"MotionScalingConfiguration := {Convert.ToInt32(_axis.MotionScalingConfiguration)}, " +
                                      $"ConversionConstant := {_axis.ConversionConstant}, " +
                                      $"OutputCamExecutionTargets := {Convert.ToInt32(_axis.OutputCamExecutionTargets)}, " +
                                      $"PositionUnits := {_axis.PositionUnits}, " +
                                      $"AverageVelocityTimebase := {_axis.AverageVelocityTimebase}, " +
                                      $"PositionUnwind := {Convert.ToInt32(_axis.PositionUnwind)}, " +
                                      $"HomeSpeed := {_axis.HomeSpeed}, " +
                                      $"HomeReturnSpeed := {_axis.HomeReturnSpeed}, " +
                                      $"VelocityFeedforwardGain := {_axis.VelocityFeedforwardGain}, " +
                                      $"AccelerationFeedforwardGain := {_axis.AccelerationFeedforwardGain}, " +
                                      $"PositionErrorTolerance := {_axis.PositionErrorTolerance}, " +
                                      $"PositionLockTolerance := {_axis.PositionLockTolerance}, " +
                                      $"VelocityOffset := {_axis.VelocityOffset}, " +
                                      $"TorqueOffset := {_axis.TorqueOffset}, " +
                                      $"BacklashReversalOffset := {_axis.BacklashReversalOffset}, " +
                                      $"TuningTravelLimit := {_axis.TuningTravelLimit}, " +
                                      $"TuningSpeed := {_axis.TuningSpeed}, " +
                                      $"TuningTorque := {_axis.TuningTorque}, " +
                                      $"DampingFactor := {_axis.DampingFactor}, " +
                                      $"DriveModelTimeConstant := {_axis.DriveModelTimeConstant}, " +
                                      $"PositionServoBandwidth := {_axis.PositionServoBandwidth}, " +
                                      $"VelocityServoBandwidth := {_axis.VelocityServoBandwidth}, " +
                                      $"VelocityStandstillWindow := {_axis.VelocityStandstillWindow}, " +
                                      $"TorqueLimitPositive := {_axis.TorqueLimitPositive}, " +
                                      $"TorqueLimitNegative := {_axis.TorqueLimitNegative}, " +
                                      $"StoppingTorque := {_axis.StoppingTorque}, " +
                                      $"LoadInertiaRatio := {_axis.LoadInertiaRatio}, " +
                                      $"RegistrationInputs := {Convert.ToInt32(_axis.RegistrationInputs)}, " +
                                      $"PositionIntegratorBandwidth := {_axis.PositionIntegratorBandwidth}, " +
                                      $"PositionIntegratorControl := {Convert.ToInt32(_axis.PositionIntegratorControl)}, " +
                                      $"VelocityIntegratorControl := {Convert.ToInt32(_axis.VelocityIntegratorControl)}, " +
                                      $"SystemInertia := {_axis.SystemInertia}, " +
                                      $"StoppingAction := {Convert.ToInt32(_axis.StoppingAction)}, " +
                                      $"InverterCapacity := {_axis.InverterCapacity}, " +
                                      //$"CIPAxisExceptionAction := {_axis.CIPAxisExceptionAction}, " +
                                      //$"CIPAxisExceptionActionMfg := {_axis.CIPAxisExceptionActionMfg}, " +
                                      //$"CIPAxisExceptionActionRA := {_axis.CIPAxisExceptionActionRA}, " +
                                      $"MotorUnit := {Convert.ToInt32(_axis.MotorUnit)}, " +
                                      $"Feedback1Unit := {Convert.ToInt32(_axis.Feedback1Unit)}, " +
                                      $"ScalingSource := {Convert.ToInt32(_axis.ScalingSource)}, " +
                                      $"LoadType := {Convert.ToInt32(_axis.LoadType)}, " +
                                      $"ActuatorType := {Convert.ToInt32(_axis.ActuatorType)}, " +
                                      $"TravelMode := {Convert.ToInt32(_axis.TravelMode)}, " +
                                      $"PositionScalingNumerator := {_axis.PositionScalingNumerator}, " +
                                      $"PositionScalingDenominator := {_axis.PositionScalingDenominator}, " +
                                      $"PositionUnwindNumerator := {_axis.PositionUnwindNumerator}, " +
                                      $"PositionUnwindDenominator := {_axis.PositionUnwindDenominator}, " +
                                      $"TravelRange := {_axis.TravelRange}, " +
                                      $"MotionResolution := {Convert.ToInt32(_axis.MotionResolution)}, " +
                                      $"MotionPolarity := {Convert.ToInt32(_axis.MotionPolarity)}, " +
                                      $"MotorTestResistance := {_axis.MotorTestResistance}, " +
                                      $"MotorTestInductance := {_axis.MotorTestInductance}, " +
                                      $"TuneFriction := {_axis.TuneFriction}, " +
                                      $"TuneLoadOffset := {_axis.TuneLoadOffset}, " +
                                      $"TuningSelect := {Convert.ToInt32(_axis.TuningSelect)}, " +
                                      $"TuningDirection := {Convert.ToInt32(_axis.TuningDirection)}, " +
                                      $"ApplicationType := {Convert.ToInt32(_axis.ApplicationType)}, " +
                                      $"LoopResponse := {Convert.ToInt32(_axis.LoopResponse)}, " +
                                      $"PositionLoopBandwidth := {_axis.PositionLoopBandwidth}, " +
                                      $"VelocityLoopBandwidth ：= {_axis.VelocityLoopBandwidth}, " +
                                      $"VelocityIntegratorBandwidth := {_axis.VelocityIntegratorBandwidth}, " +
                                      //$"MotionExceptionAction := {_axis.MotionExceptionAction}, " +
                                      $"SoftTravelLimitChecking := {Convert.ToInt32(_axis.SoftTravelLimitChecking)}, " +
                                      $"LoadRatio := {_axis.LoadRatio}, " +
                                      $"TuneInertiaMass := {_axis.TuneInertiaMass}, " +
                                      $"SoftTravelLimitPositive := {Convert.ToInt32(_axis.SoftTravelLimitChecking)}, " +
                                      $"SoftTravelLimitNegative := {_axis.SoftTravelLimitNegative}, " +
                                      $"GainTuningConfigurationBits := {_axis.GainTuningConfigurationBits}, " +
                                      $"SystemBandwidth := {_axis.SystemBandwidth}, " +
                                      $"TransmissionRatioInput := {Convert.ToInt32(_axis.TransmissionRatioInput)}, " +
                                      $"TransmissionRatioOutput := {Convert.ToInt32(_axis.TransmissionRatioOutput)}, " +
                                      $"ActuatorLead := {_axis.ActuatorLead}, " +
                                      $"ActuatorLeadUnit := {Convert.ToInt32(_axis.ActuatorLeadUnit)}, " +
                                      $"ActuatorDiameter := {_axis.ActuatorDiameter}, " +
                                      $"ActuatorDiameterUnit := {Convert.ToInt32(_axis.ActuatorDiameterUnit)}, " +
                                      $"SystemAccelerationBase := {_axis.SystemAccelerationBase}, " +
                                      $"DriveModelTimeConstantBase := {_axis.DriveModelTimeConstantBase}, " +
                                      $"HookupTestDistance := {_axis.HookupTestDistance}, " +
                                      $"HookupTestFeedbackChannel := {Convert.ToInt32(_axis.HookupTestFeedbackChannel)}, " +
                                      $"LoadCoupling := {Convert.ToInt32(_axis.LoadCoupling)}, " +
                                      $"SystemDamping := {_axis.SystemDamping}, " +
                                      $"InterpolatedPositionConfiguration := {Convert.ToInt32(_axis.InterpolatedPositionConfiguration)}, " +
                                      $"BusOvervoltageOperationalLimit := {_axis.BusOvervoltageOperationalLimit}, " +
                                      $"DriveRatedPeakCurrent := {_axis.DriveRatedPeakCurrent}, " +
                                      $"MotionUnit := {Convert.ToInt32(_axis.MotionUnit)}, " +
                                      $"CommutationOffset := {_axis.CommutationOffset}, " +
                                      $"RotaryMotorPoles := {_axis.RotaryMotorPoles}, " +
                                      $"MotorOverloadLimit := {_axis.MotorOverloadLimit}, " +
                                      //$"PMMotorFluxSaturation := {_axis.PMMotorFluxSaturation}, " +
                                      $"TorqueLeadLagFilterGain := {_axis.TorqueLeadLagFilterGain}, " +
                                      $"FeedbackDataLossUserLimit := {Convert.ToInt32(_axis.FeedbackDataLossUserLimit)}, " +
                                      $"HookupTestTime := {_axis.HookupTestTime}, " +
                                      $"ConverterThermalOverloadUserLimit := {_axis.ConverterThermalOverloadUserLimit}, " +
                                      $"InverterThermalOverloadUserLimit := {_axis.InverterThermalOverloadUserLimit}, " +
                                      $"LoadObserverFeedbackGain := {_axis.LoadObserverFeedbackGain}, " +
                                      $"MotorThermalOverloadUserLimit := {_axis.MotorThermalOverloadUserLimit}, " +
                                      $"OvertorqueLimit := {_axis.OvertorqueLimit}, " +
                                      $"TorqueRateLimit := {_axis.TorqueRateLimit}, " +
                                      $"TorqueNotchFilterHighFrequencyLimit := {_axis.TorqueNotchFilterHighFrequencyLimit}, " +
                                      $"TorqueNotchFilterLowFrequencyLimit := {_axis.TorqueNotchFilterLowFrequencyLimit}, " +
                                      $"TorqueNotchFilterTuningThreshold := {_axis.TorqueNotchFilterTuningThreshold}, " +
                                      $"Feedback1AccelFilterTaps := {_axis.Feedback1AccelFilterTaps}, " +
                                      $"Feedback1VelocityFilterTaps := {_axis.Feedback1VelocityFilterTaps}, " +
                                      $"Feedback2AccelFilterTaps := {_axis.Feedback2AccelFilterTaps}, " +
                                      $"Feedback2VelocityFilterTaps := {_axis.Feedback2VelocityFilterTaps}, " +
                                      $"FeedbackUnitRatio := {_axis.FeedbackUnitRatio}, " +
                                      $"MotorOverspeedUserLimit := {_axis.MotorOverspeedUserLimit}, " +
                                      $"StoppingTimeLimit := {_axis.StoppingTimeLimit}, " +
                                      $"StoppingTorque := {_axis.StoppingTorque}, " +
                                      $"TorqueThreshold := {_axis.TorqueThreshold}, " +
                                      $"UndertorqueLimit := {_axis.UndertorqueLimit}, " +
                                      $"VelocityErrorToleranceTime := {_axis.VelocityErrorToleranceTime}, " +
                                      $"VelocityStandstillWindow := {_axis.VelocityStandstillWindow}, " +
                                      $"VelocityThreshold := {_axis.VelocityThreshold}, " +
                                      $"ZeroSpeed := {_axis.ZeroSpeed})\"");
                    list.Add(attributes.ToString());
                    stringBuilder.AppendLine(string.Join(separator, list));
                }
                else if (tag.DataTypeInfo.DataType.IsMotionGroupType)
                {
                    var _motionGroup = ((Tag)tag).DataWrapper as MotionGroup;

                    List<string> list = new List<string>()
                    {
                        "TAG",
                        scope,
                        tag.Name,
                        $"\"{description}\"",
                        $"\"{tag.DataTypeInfo}\"",
                        ""
                    };
                    StringBuilder attributes = new StringBuilder("\"(" +
                                                                 $"ExternalAccess := {tag.ExternalAccess}, " +
                                                                 $"CoarseUpdatePeriod := {_motionGroup.CoarseUpdatePeriod}, " +
                                                                 $"PhaseShift := {_motionGroup.PhaseShift}, "
                    );
                    switch (type)
                    {
                        case "Program":
                            attributes.Append($"Usage := {tag.Usage}, ");
                            break;
                        case "AOI":
                            attributes.Append($"Usage := {tag.Usage}, " +
                                              $"Required := {tag.IsRequired}, " +
                                              $"Visible := {tag.IsVisible}, ");
                            break;
                    }

                    attributes.Append($"GeneralFaultType := {_motionGroup.GeneralFaultType}, " +
                                      $"AutoTagUpdate := {_motionGroup.AutoTagUpdate}, " +
                                      $"Alternate1UpdateMultiplier := {_motionGroup.Alternate1UpdateMultiplier}, " +
                                      $"Alternate2UpdateMultiplier := {_motionGroup.Alternate2UpdateMultiplier})\"");

                    list.Add(attributes.ToString());

                    stringBuilder.AppendLine(string.Join(separator, list));
                }
                else if (tag.DataTypeInfo.DataType.IsIOType)
                {
                    List<string> list = new List<string>()
                    {
                        "TAG",
                        scope,
                        tag.Name,
                        $"\"{description}\"",
                        $"\"{tag.DataTypeInfo}\"",
                        ""
                    };

                    StringBuilder attributes = new StringBuilder($"\"(ExternalAccess := {tag.ExternalAccess}");

                    switch (type)
                    {
                        case "Program":
                            attributes.Append($", Usage := {tag.Usage})\"");
                            break;
                        case "AOI":
                            attributes.Append($", Usage := {tag.Usage}, " +
                                              $"Required := {tag.IsRequired}, " +
                                              $"Visible := {tag.IsVisible})\"");
                            break;
                        case "Controller":
                            attributes.Append(")\"");
                            break;
                    }

                    list.Add(attributes.ToString());

                    stringBuilder.AppendLine(string.Join(separator, list));
                }

                else if (tag.DataTypeInfo.DataType.IsStruct)
                {
                    List<string> list = new List<string>()
                    {
                        "TAG",
                        scope,
                        tag.Name,
                        $"\"{description}\"",
                        $"\"{tag.DataTypeInfo}\"",
                        ""
                    };

                    StringBuilder attributes = new StringBuilder($"\"(Constant := {tag.IsConstant}, " +
                                                                 $"ExternalAccess := {tag.ExternalAccess}");

                    switch (type)
                    {
                        case "Program":
                            attributes.Append($", Usage := {tag.Usage})\"");
                            break;
                        case "AOI":
                            attributes.Append($", Usage := {tag.Usage}, " +
                                              $"Required := {tag.IsRequired}, " +
                                              $"Visible := {tag.IsVisible})\"");
                            break;
                        case "Controller":
                            attributes.Append(")\"");
                            break;
                    }

                    list.Add(attributes.ToString());

                    stringBuilder.AppendLine(string.Join(separator, list));
                }
                else if (tag.DataTypeInfo.DataType.IsMessageType)
                {
                    var _message = ((Tag)tag).DataWrapper as MessageDataWrapper;

                    var messageType = _message.Parameters.MessageType.ToString();
                    if (messageType.Equals("0"))
                    {
                        messageType = "Unconfigured";
                    }

                    List<string> list = new List<string>()
                    {
                        "TAG",
                        scope,
                        tag.Name,
                        $"\"{description}\"",
                        $"\"{tag.DataTypeInfo}\"",
                        ""
                    };

                    StringBuilder attributes = new StringBuilder($"\"(ExternalAccess := {tag.ExternalAccess}, " +
                                                                 $"MessageType := {messageType}, ");

                    switch (type)
                    {
                        case "Program":
                            attributes.Append($"Usage := {tag.Usage}, ");
                            break;
                        case "AOI":
                            attributes.Append($"Usage := {tag.Usage}, " +
                                              $"Required := {tag.IsRequired}, " +
                                              $"Visible := {tag.IsVisible}, ");
                            break;
                    }

                    attributes.Append($"RequestedLength := {_message.Parameters.RequestedLength}, " +
                                      $"CommTypeCode := {_message.Parameters.CommTypeCode}, " +
                                      $"LocalIndex:= {_message.Parameters.LocalIndex})\"");

                    list.Add(attributes.ToString());

                    stringBuilder.AppendLine(string.Join(separator, list));
                }
                else
                {
                    List<string> list = new List<string>()
                    {
                        "TAG",
                        scope,
                        tag.Name,
                        $"\"{description}\"",
                        $"\"{tag.DataTypeInfo}\"",
                        ""
                    };

                    bool isAoiOrUdp = tag.DataTypeInfo.DataType is AOIDataType || tag.DataTypeInfo.DataType is UserDefinedDataType;

                    StringBuilder attributes = new StringBuilder();

                    if (!isAoiOrUdp)
                    {
                        attributes.Append($"\"(RADIX:= { tag.DisplayStyle }, " +
                                          $"Constant := {tag.IsConstant}, " +
                                          $"ExternalAccess := {tag.ExternalAccess}");
                    }
                    else
                    {
                        attributes.Append($"\"(Constant := {tag.IsConstant}, " +
                                          $"ExternalAccess := {tag.ExternalAccess}");
                    }

                    switch (type)
                    {
                        case "Program":
                            attributes.Append($", Usage := {tag.Usage})\"");
                            break;
                        case "AOI":
                            attributes.Append($", Usage := {tag.Usage}, " +
                                              $"Required := {tag.IsRequired}, " +
                                              $"Visible := {tag.IsVisible})\"");
                            break;
                        case "Controller":
                            attributes.Append(")\"");
                            break;
                    }

                    list.Add(attributes.ToString());
                    stringBuilder.AppendLine(string.Join(separator, list));
                }
            }

            return stringBuilder.ToString();
        }
    }
}
