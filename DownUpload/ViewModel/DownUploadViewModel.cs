using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Forms;
using DownUpload.Properties;
using ICSGateway.Components.Common;
using DownUpload.Model;
using GalaSoft.MvvmLight.Threading;

namespace DownUpload.ViewModel
{
    class DownUploadViewModel:ViewModelBase
    {
        public string IpAddress { get; set; }
        private string _testMessage;
        private string _fileName;
        private bool _isValid = true;
        public string TimeSpan { get; set; } = "1";
        private int _timeSpan;
        private int count;
        private TaskFactory taskFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(6));
        public bool IsEnable { get; set; } = true;
        private Node _selectedNode;

        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                Set(ref _fileName, value);
            }
        }

        public string TestMessage
        {
            get
            {
                return _testMessage;
            }
            set
            {
                Set(ref _testMessage, value);
            }
        }

        public string Times { get; set; } = "1";

        public RelayCommand OpenCommand { get; }
        public RelayCommand DownloadCommand { get; }
        public RelayCommand UploadCommand { get; }
        public RelayCommand ClearCommand { get;  }

        public RelayCommand<Node> SelectIPCommand { get; }

        public DownUploadViewModel()
        {
            DownloadCommand = new RelayCommand(ExecuteDownloadCommand, CanExecuteDownloadCommand);
            UploadCommand = new RelayCommand(ExecuteUploadCommand, CanExecuteUploadCommand);
            OpenCommand = new RelayCommand(ExecuteOpenCommand);
            ClearCommand = new RelayCommand(ExecuteClearCommand);
            SelectIPCommand = new RelayCommand<Node>(ExecuteSelectIPCommand);
            
        }

        public void ExecuteSelectIPCommand(Node selectedNode)
        {
            _selectedNode = selectedNode;
            IpAddress = selectedNode.NetNodeItem.IP;
            RaisePropertyChanged(nameof(IpAddress));
            UploadCommand.RaiseCanExecuteChanged();
            DownloadCommand.RaiseCanExecuteChanged();
        }

        public void ExecuteClearCommand()
        {
            TestMessage = string.Empty;
            Console.Clear();
        }
        public void ExecuteDownloadCommand()
        {
            Init();
            if (!IsTimesValid(Times))
            {
                _isValid = false;
                TestMessage += "Please input a correct value.\n\n";
                return;
            }
            if(string.IsNullOrEmpty(FileName))
            {
                _isValid = false;
                TestMessage += "File Path could not be empty.\n\n";
                return;
            }
            if (string.IsNullOrEmpty(IpAddress))
            {
                _isValid = false;
                TestMessage += "Need IpAddress.\n\n";
                return;
            }
            TestMessage += "Download:\n";
            TestMessage += $"IpAddress:{IpAddress}\n";
            TestMessage += $"File:{FileName}\n";
            var times = int.Parse(Times);
            count = 0;
            taskFactory.StartNew(() =>
                {
                    try
                    {
                        for (int i = 1; i <= times; i++)
                        {
                            Console.WriteLine($"第{i}次Download开始");

                            ControllerDownloadTest().GetAwaiter().GetResult();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        Console.WriteLine($"测试结束!\n{Times}次Download操作，成功执行{count}次\n");
                        TestMessage += $"测试结束!\n{Times}次Download操作，成功执行{count}次\n";
                    }
                });
        }

        public void ExecuteUploadCommand()
        {
            Init();
            if (!IsTimesValid(Times))
            {
                _isValid = false;
                TestMessage = "Please input a correct value.\n\n";
                Console.WriteLine("Please input a correct value.");
                return;
            }
            if (string.IsNullOrEmpty(IpAddress))
            {
                _isValid = false;
                TestMessage += "Need IpAddress.\n\n";
                Console.WriteLine("Need IpAddress.");
                return;
            }
            var times = int.Parse(Times);
            TestMessage += "Upload:\n";
            TestMessage += $"IpAddress:{IpAddress}\n";
            taskFactory.StartNew(() =>
            {
                try
                {
                    for (int i = 1; i <= times; i++)
                    {
                        Console.WriteLine($"第{i}次Upload开始");
                        //taskFactory.StartNew(() =>
                        //{
                        ControllerUploadTest().GetAwaiter().GetResult();
                        //});
                        //ControllerUploadTest().GetAwaiter().GetResult();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    Console.WriteLine($"测试结束!\n{Times}次Upload操作，成功执行{count}次\n");
                    TestMessage += $"测试结束!\n{Times}次Upload操作，成功执行{count}次\n";
                }
            });
        }

        public void ExecuteOpenCommand()
        {
            OpenFileDialog openDlg = new OpenFileDialog
            {
                Title = "Import file",
                Filter = "json文件(*.json)|*.json|L5X文件(*.L5X)|*.L5X"
            };

            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                if (openDlg.FileName.EndsWith(".L5X", true, CultureInfo.CurrentCulture))
                {
                    FileName = openDlg.FileName;
                    DownloadCommand.RaiseCanExecuteChanged();
                }
                else if (openDlg.FileName.EndsWith(".json", true, CultureInfo.CurrentCulture))
                {
                    FileName = openDlg.FileName;
                    DownloadCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private async Task ControllerDownloadTest()
        {
            var ctrl = Controller.Open(FileName);
            try
            {
                ctrl.GenCode();

                await ctrl.ConnectAsync(IpAddress);

                while (ctrl.OperationMode != ControllerOperationMode.OperationModeProgram)
                {
                    await ctrl.ChangeOperationMode(ControllerOperationMode.OperationModeProgram);
                    await ctrl.UpdateState();
                    //await Task.Delay(_timeSpan);
                }

                await ctrl.Download(ControllerOperationMode.OperationModeNull, false, false);

                await ctrl.RebuildTagSyncControllerAsync();
                await ctrl.UpdateState();

                await ctrl.ChangeOperationMode(ControllerOperationMode.OperationModeRun);
                count++;
                Console.WriteLine($"第{count}次Download成功");
            }
            catch (Exception e)
            {
                count++;
                Console.WriteLine($"第{count}次Download失败");
                Console.WriteLine(e);
                throw;
            }
            ctrl.GoOffline();
            Thread.Sleep(_timeSpan);
        }

        private async Task ControllerUploadTest()
        {
            var controller = Controller.GetInstance();

            try
            {
                await controller.ConnectAsync(IpAddress);
                await controller.Upload(false);
                count++;
                Console.WriteLine($"第{count}次Upload成功");
            }
            catch (Exception e)
            {
                count++;
                Console.WriteLine($"第{count}次Upload失败");
                Console.WriteLine(e);
                throw;
            }
            controller.GoOffline();
            Thread.Sleep(_timeSpan);
        }

        public bool IsTimesValid(string times)
        {
            int time;
            return int.TryParse(times,out time);
        }

        public void Init()
        {
            Console.WriteLine();
            TestMessage += "\n";
            Console.WriteLine(Resources.DownUploadViewModel_Clear________________________________________);
            count = 0;
            double test;
            _timeSpan = double.TryParse(TimeSpan,out test)? Convert.ToInt32(double.Parse(TimeSpan)*1000):1000;
            _isValid = true;
        }

        private bool CanExecuteUploadCommand()
        {
            if (!IsIconDevice()) return false;

            return !string.IsNullOrWhiteSpace(IpAddress);
        }

        private bool CanExecuteDownloadCommand()
        {
            if (!IsIconDevice())
                return false;

            return !string.IsNullOrWhiteSpace(IpAddress) &&
                   !string.IsNullOrWhiteSpace(FileName);
        }

        private bool IsIconDevice()
        {
            if (_selectedNode == null) return false;

            if (!(_selectedNode.NetNodeItem.Vendor == "1447" && _selectedNode.NetNodeItem.DeviceType == "14"))
                return false;

            return true;
        }
    }
}
