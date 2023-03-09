using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading.Tasks;
using ICSStudio.SimpleServices.Common;
using ICSStudio.Interfaces.Common;
using System.Threading;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.Cip.Objects;
using Newtonsoft.Json.Linq;

namespace ICSStudio.TestTool
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _ip = "192.168.1.221";
            IsCanTest = true;
            SelectPathCommand = new RelayCommand(SelectPath);
            StartTestCommand = new RelayCommand(StartTest);
            ClearLogCommand = new RelayCommand(ClearLog);
        }

        private const string LogFile = "testlog.txt";
        private string _outfile = "";
        private string _plcType;
        private string _ip;
        private Controller _controller;

        public string IP
        {
            get { return _ip; }
            set
            {
                if (value == _ip) return;
                _plcType = null;
                _controller = null;
                Set(ref _ip, value);
            }
        }

        private string _testFolder;

        public string TestFolder
        {
            get { return _testFolder; }
            set { Set(ref _testFolder, value); }
        }

        private string _logText;

        public string LogText
        {
            get { return _logText; }
            set { Set(ref _logText, value); }
        }

        private bool _isCanTest;

        public bool IsCanTest
        {
            get { return _isCanTest; }
            set { Set(ref _isCanTest, value); }
        }

        public List<string> FileList = new List<string>();

        public string ICSTitle
        {
            get
            {
                // first, try to get the version string from the assembly.
                Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                if (assemblyVersion == null)
                    return "测试工具";

                string result = assemblyVersion.ToString();

                return $"测试工具-v{result}";
            }
        }

        #region Command

        public RelayCommand SelectPathCommand { get; set; }

        public RelayCommand StartTestCommand { get; set; }

        public RelayCommand ClearLogCommand { get; set; }

        #endregion

        #region Method

        private void SelectPath()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;

            TestFolder = dialog.FileName;
        }

        private int _logCount;
        private void WriteLog(string msg, string detail = "", bool isWriteFile = true)
        {
            msg = DateTime.Now.ToString("yyyy-M-d HH:mm:ss ") + msg + "\r\n";

            if (_logCount > 200)
            {
                LogText = msg;//记录太多时，清空log
                _logCount = 0;
            }
            else
                LogText += msg;
            _logCount++;

            if (!isWriteFile) return;

            using (var fs = new FileStream(_outfile, FileMode.Append))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine(msg + detail);
                }
            }
        }

        private void StartTest()
        {
            //校验IP合法性
            if (!Utility.JudgeIPFormat(_ip))
            {
                WriteLog("IP地址不合法", "", false);
                return;
            }


            FileList = Utility.GetFileList(_testFolder);

            if (FileList.Count > 0)
                ExecuteTestSet();
        }

        private string _tempFolder = "";

        /// <summary>
        /// 执行测试集
        /// </summary>
        /// <param></param>
        private void ExecuteTestSet()
        {
            _outfile = TestFolder + @"\" + LogFile;
            _tempFolder = $@"{TestFolder}\temp";
            if (!Directory.Exists(_tempFolder))
                Directory.CreateDirectory(_tempFolder);

            WriteLog($"已选择{FileList.Count}个工程，开始测试");
            IsCanTest = false;

            var task = new Task(() =>
            {
                foreach (var file in FileList)
                {
                    int result = ExecuteOneCase(file);
                    if (result == 2)
                        break;
                }
            });

            task.ContinueWith(task2 =>
            {
                if (Directory.Exists(_tempFolder))
                    Directory.Delete(_tempFolder, true);
                WriteLog("执行完成所有工程");
                IsCanTest = true;

            }, TaskContinuationOptions.AttachedToParent);

            task.Start();
        }

        /// <summary>
        /// 执行测试集
        /// </summary>
        /// <param name="file"></param>
        /// <returns>0:执行成功 1:发生错误，但继续执行后续测试集 2:连接失败/设备未响应，终止所有测试</returns>
        private int ExecuteOneCase(string file)
        {
            var jfile = file;
            try
            {
                try
                {
                    var ext = Path.GetExtension(file).ToLower();
                    //检测L5X并转化为json
                    if (ext.Equals(".l5x"))
                    {
                        jfile = $@"{_tempFolder}\{Guid.NewGuid()}.json";
                        FileConverter.L5XToJson.Converter.L5XToJson(file, jfile);
                    }

                    //打开测试集
                    if (_controller == null)
                        _controller = Controller.GetInstance();

                    Controller.Open(jfile);
                }
                catch (Exception e)
                {
                    WriteLog($"工程{file}错误", e.Message + e.StackTrace);
                    return 1;
                }

                try
                {
                    //连接设备
                    _controller.ConnectAsync(IP).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    WriteLog("连接失败，终止所有测试", e.Message + e.StackTrace);
                    return 2;
                }

                try
                {
                    //获取PLC类型
                    if (_plcType == null)
                    {
                        _plcType = GetProductType(_controller);
                        if (string.IsNullOrEmpty(_plcType))
                        {
                            WriteLog("PLC类型未知");
                            return 2;
                        }
                        else
                            WriteLog($"所选PLC类型为{_plcType}");
                    }

                    WriteLog($"开始执行工程{file}");

                    //更改工程中PLC的类型
                    ReplaceProperties(_controller);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                try
                {
                    //编译
                    _controller.GenCode();
                }
                catch (Exception e)
                {
                    WriteLog($"工程{file}编译失败，请解决该工程的问题！", e.Message + e.StackTrace);
                    return 1;
                }

                try
                {
                    if (_controller.OperationMode == ControllerOperationMode.OperationModeFaulted)
                        _controller.ClearFaults().GetAwaiter().GetResult();

                    int retryCount = 0;
                    while (_controller.OperationMode != ControllerOperationMode.OperationModeProgram)
                    {
                        _controller.ChangeOperationMode(ControllerOperationMode.OperationModeProgram).GetAwaiter();
                        _controller.UpdateState().GetAwaiter();
                        Thread.Sleep(10);

                        retryCount++;

                        if (retryCount >= 100)
                            throw new ApplicationException("Retry count >= 100!");
                    }

                    //下载
                    _controller.Download(ControllerOperationMode.OperationModeNull, false, false).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    WriteLog($"工程{file}下载失败，请解决该工程的问题！", e.Message + e.StackTrace);
                    return 1;
                }

                _controller.RebuildTagSyncControllerAsync().GetAwaiter().GetResult();
                _controller.UpdateState().GetAwaiter();

                var checktime = 60;
                while (checktime-- > 0)
                {
                    if (_controller.IsOnline == false)
                    {
                        WriteLog("设备离线，终止所有测试");
                        return 2;
                    }

                    if (_controller.OperationMode == ControllerOperationMode.OperationModeProgram)
                        break;

                    Thread.Sleep(1000);
                }

                if (checktime < 1)
                {
                    WriteLog("设备未响应，终止所有测试");
                    return 2;
                }

                //RunMode
                _controller.ChangeOperationMode(ControllerOperationMode.OperationModeRun).GetAwaiter().GetResult();

                _controller.UpdateState().GetAwaiter();

                // waiting for running mode
                checktime = 60;
                while (checktime-- > 0)
                {
                    if (_controller.IsOnline == false)
                    {
                        WriteLog("设备离线，终止所有测试");
                        return 2;
                    }

                    if (_controller.OperationMode == ControllerOperationMode.OperationModeProgram)
                        Thread.Sleep(1000);
                    else if (_controller.OperationMode == ControllerOperationMode.OperationModeRun)
                        break;
                    else
                        throw new ApplicationException("plc运行错误！");
                }

                WriteLog($"工程{file}执行完成");
                return 0;
            }
            catch (Exception exception)
            {
                WriteLog($"发生错误{exception.Message + exception.StackTrace}");
                return 1;
            }
            finally
            {
                //测试完成后离线
                _controller?.GoOffline();
                if (file.ToLower().EndsWith(".l5x") && File.Exists(jfile))
                    File.Delete(jfile);
            }
        }

        private string GetProductType(Controller controller)
        {
            CIPIdentity cipIdentity = new CIPIdentity(1, controller.CipMessager);

            var result = cipIdentity.GetAttributesAll().GetAwaiter().GetResult();
            if (result == 0)
            {
                ushort vendorId = Convert.ToUInt16(cipIdentity.VendorID);
                ushort deviceType = Convert.ToUInt16(cipIdentity.DeviceType);
                ushort productCode = Convert.ToUInt16(cipIdentity.ProductCode);

                if (vendorId == 1447 && deviceType == 14)
                {
                    if (productCode == 408)
                        return "ICC-P0100ERM";
                    if (productCode == 108)
                        return "ICC-B010ERM";
                }
            }

            return null;
        }

        private void ReplaceProperties(Controller controller)
        {
            LocalModule localModule = controller.DeviceModules["Local"] as LocalModule;
            if (localModule?.CatalogNumber == _plcType && localModule?.Vendor == 1447)
                return;

            try
            {
                string dllPath = AppDomain.CurrentDomain.BaseDirectory;
                string templateFile;
                if (_plcType == "ICC-P0100ERM")
                    templateFile = dllPath + $@"\Template\ICC-P0100ERM_Template.json";
                else
                    templateFile = dllPath + $@"\Template\ICC-B010ERM_Template.json";

                string contents = File.ReadAllText(templateFile);
                string projectName = controller.Name;
                int pointIOBusSize = 3;
                string projectDescription = controller.Description;
                string projectCreationDate = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");

                contents = contents.Replace("#ProjectName#", projectName);
                contents = contents.Replace("#ProjectDescription#", projectDescription);
                contents = contents.Replace("#ProjectCreationDate#", projectCreationDate);
                contents = contents.Replace("#PointIOBusSize#", pointIOBusSize.ToString());
                contents = contents.Replace("#ProductCode#", Controller.GetProductCode(_plcType).ToString());

                var config = JToken.Parse(contents);
                if (config == null) throw new Exception();
                controller.EtherNetIPMode = (string)config["EtherNetIPMode"];
                var moduleArray = config["Modules"] as JArray;
                if (moduleArray != null)
                {
                    // remove local module and Embedded io
                    var removeDevices = controller.DeviceModules.Where((device) =>
                    {
                        if (device is LocalModule)
                        {
                            ((DeviceModule)device).IsDeleted = true;
                            return true;
                        }

                        if (!device.CatalogNumber.StartsWith("Embedded", StringComparison.OrdinalIgnoreCase))
                            return false;

                        ((DeviceModule)device).IsDeleted = true;
                        return true;
                    }).ToList();

                    foreach (JObject moduleObject in moduleArray.OfType<JObject>())
                    {
                        controller.AddDeviceModule(moduleObject);
                        var deviceModule = controller.DeviceModules[moduleObject["Name"]?.ToString()] as DeviceModule;

                        if (deviceModule == null) continue;
                        deviceModule.ParentModule = controller.DeviceModules[deviceModule.ParentModuleName];
                        deviceModule.PostLoadJson();
                    }

                    // update local module
                    var newLocalModule = (LocalModule)controller.DeviceModules["Local"];
                    foreach (var deviceModule in controller.DeviceModules.OfType<DeviceModule>())
                    {
                        if (deviceModule != newLocalModule &&
                            !deviceModule.CatalogNumber.StartsWith("Embedded",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            if (deviceModule.ParentModuleName.Equals("Local",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                deviceModule.ParentModule = newLocalModule;
                                deviceModule.ParentModPortId = newLocalModule.GetFirstPort(PortType.Ethernet).Id;
                            }
                        }
                    }

                    foreach (var device in removeDevices)
                    {
                        ((DeviceModuleCollection)controller.DeviceModules).RemoveDeviceModule(device);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
                WriteLog("修改工程属性失败");
            }
        }

        private void ClearLog()
        {
            LogText = "";
        }

        #endregion
    }
}