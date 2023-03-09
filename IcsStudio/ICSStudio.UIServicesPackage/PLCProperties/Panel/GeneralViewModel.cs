using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Database.Database;
using ICSStudio.Dialogs.Warning;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.Notification;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Notification;
using ICSStudio.UIInterfaces.UI;
using ICSStudio.UIServicesPackage.PLCProperties.Panel.Warn;
using ICSStudio.UIServicesPackage.View;
using ICSStudio.UIServicesPackage.ViewModel;
using ICSStudio.Utils;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json.Linq;

namespace ICSStudio.UIServicesPackage.PLCProperties.Panel
{
    internal class GeneralViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private readonly Controller _controller;
        private bool _isDirty;
        private string _name;
        private string _description;
        private bool _isEnable;

        public GeneralViewModel(General panel, IController controller)
        {
            Control = panel;
            panel.DataContext = this;
            LocalModule localModule = controller.DeviceModules["Local"] as LocalModule;
            Type = localModule?.DisplayText;
            _controller = (Controller) controller;
            ChangeCommand = new RelayCommand(ExecuteChangeCommand, CanExecuteChangeCommand);
            Name = controller.Name;
            Description = controller.Description;
            IsEnable = !controller.IsOnline;
            var localModel = (LocalModule) controller.DeviceModules.FirstOrDefault(d => d is LocalModule);
            if (localModel != null)
            {
                DBHelper dbHelper = new DBHelper();
                Vendor = dbHelper.GetVendorName(localModel.Vendor);
            }

            IsDirty = false;
        }

        public string Vendor { set; get; }

        public bool IsEnable
        {
            set { Set(ref _isEnable, value); }
            get { return _isEnable; }
        }

        public RelayCommand ChangeCommand { get; }

        private void ExecuteChangeCommand()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;

            var studioUIService =
                Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;

            var dialog = new ChangeControllerDialog();
            var vm = new ChangeControllerViewModel();
            dialog.DataContext = vm;
            if (dialog.ShowDialog(uiShell) ?? false)
            {
                LocalModule localModule = _controller.DeviceModules["Local"] as LocalModule;
                if (vm.SelectedType.Equals(vm.Old, StringComparison.OrdinalIgnoreCase)) return;
                var warn = new ChangeWarnDialog();
                var warnVm = new ChangeWarnViewModel(localModule.CatalogNumber,vm.SelectedType);
                warn.DataContext = warnVm;
                if (warn.ShowDialog(uiShell) ?? false)
                {
                    try
                    {
                        _controller.IsLoading = true;
                        string dllPath = AssemblyUtils.AssemblyDirectory;
                        string templateFile;
                        if (vm.SelectedType.StartsWith("ICC-P"))
                        {
                            switch (vm.SelectedType)
                            {
                                case "ICC-P010ERM":
                                    templateFile = dllPath + $@"\Template\ICC-P010ERM_Template.json";
                                    break;
                                case "ICC-P020ERM":
                                    templateFile = dllPath + $@"\Template\ICC-P020ERM_Template.json";
                                    break;
                                default:
                                    templateFile = dllPath + $@"\Template\ICC-P0100ERM_Template.json";
                                    break;
                            }
                        }
                        else if (vm.SelectedType.StartsWith("ICC-T"))
                        {
                            templateFile = dllPath + $@"\Template\ICC-T0100ERM_Template.json";
                        }
                        else
                        {
                            templateFile = dllPath + $@"\Template\ICC-B010ERM_Template.json";
                        }

                        string contents = File.ReadAllText(templateFile);

                        string projectName = Name;

                        int pointIOBusSize = 3;

                        string projectDescription = string.Empty;
                        if (!string.IsNullOrEmpty(Description))
                            projectDescription = Description;

                        string projectCreationDate = System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");

                        contents = contents.Replace("#ProjectName#", projectName);
                        contents = contents.Replace("#ProjectDescription#", projectDescription);
                        contents = contents.Replace("#ProjectCreationDate#", projectCreationDate);
                        contents = contents.Replace("#PointIOBusSize#", pointIOBusSize.ToString());
                        contents = contents.Replace("#ProductCode#", Controller.GetProductCode(vm.SelectedType).ToString());
                        var config = JToken.Parse(contents);
                        if(config==null) throw new Exception();
                        _controller.EtherNetIPMode = (string) config["EtherNetIPMode"];
                        var moduleArray = config?["Modules"] as JArray;
                        if (moduleArray != null)
                        {

                            studioUIService?.DetachController();

                            // remove local module and Embedded io
                            var removeDevices = _controller.DeviceModules.Where((device) =>
                            {
                                if (device is LocalModule)
                                {
                                    ((DeviceModule)device).IsDeleted = true;
                                    return true;
                                }

                                if (device.CatalogNumber.StartsWith("Embedded", StringComparison.OrdinalIgnoreCase))
                                {
                                    ((DeviceModule)device).IsDeleted = true;
                                    return true;
                                }

                                return false;
                            }).ToList();


                            foreach (var device in removeDevices)
                            {
                                ((DeviceModuleCollection)_controller.DeviceModules).RemoveDeviceModule(device);
                            }

                            foreach (JObject moduleObject in moduleArray.OfType<JObject>())
                            {
                                _controller.AddDeviceModule(moduleObject);
                                var deviceModule =
                                    _controller.DeviceModules[moduleObject["Name"]?.ToString()] as DeviceModule;

                                if (deviceModule != null)
                                {
                                    deviceModule.ParentModule =
                                        _controller.DeviceModules[deviceModule.ParentModuleName];
                                    deviceModule.PostLoadJson();
                                }

                            }

                            // update local module
                            var newLocalModule = (LocalModule)_controller.DeviceModules["Local"];
                            foreach (var deviceModule in _controller.DeviceModules.OfType<DeviceModule>())
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
                            
                        }
                        _controller.IsLoading = false;
                        Notifications.Publish(new MessageData() { Type = MessageData.MessageType.Verify });
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(
                            $"Failed to create '{Name}'",
                            "ICS Studio", MessageBoxButton.OK);

                    }
                    finally
                    {
                        _controller.IsLoading = false;
                        LocalModule newLocalModule = _controller.DeviceModules["Local"] as LocalModule;
                        Debug.Assert(newLocalModule != null);
                        Type = newLocalModule?.DisplayText;
                        DBHelper dbHelper = new DBHelper();
                        Vendor = dbHelper.GetVendorName(newLocalModule.Vendor);
                        RaisePropertyChanged("Type");
                        RaisePropertyChanged("Vendor");

                        studioUIService?.Reset();
                    }

                }
            }
        }

        private bool CanExecuteChangeCommand()
        {
            return !_controller.IsOnline;
        }

        public string Name
        {
            set
            {
                _name = value;
                IsDirty = true;
            }
            get { return _name; }
        }

        public string Description
        {
            set
            {
                _description = value;
                IsDirty = true;
            }
            get { return _description; }
        }

        public string Type { set; get; }

        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {

        }

        public bool Verify()
        {
            Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
            if (string.IsNullOrEmpty(Name) || !regex.IsMatch(Name))
            {
                MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier("Failed to modify the controller properties.") + "\n" 
                    + LanguageManager.GetInstance().ConvertSpecifier("Name is invalid."), "ICS Studio",
                    MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }

            return true;
        }

        public void Save()
        {
            if (IsValidControllerName(Name))
            {
                var controller = _controller as Controller;
                controller.Name = Name;
                controller.Description = Description;
                IsDirty = false;
            }
        }

        private bool IsValidControllerName(string name)
        {
            string warningMessage = LanguageManager.GetInstance()
                .ConvertSpecifier("Failed to modify the controller properties.");
            string warningReason = string.Empty;
            bool isValid = true;

            if (string.IsNullOrEmpty(name))
            {
                isValid = false;
                warningReason = LanguageManager.GetInstance().ConvertSpecifier("Name is invalid.");
            }

            if (isValid)
            {
                if (name.Length > 40 || name.EndsWith("_") ||
                    name.IndexOf("__", StringComparison.OrdinalIgnoreCase) > -1)
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("Name is invalid.");
                }
            }

            if (isValid)
            {
                Regex regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
                if (!regex.IsMatch(name))
                {
                    isValid = false;
                    warningReason = LanguageManager.GetInstance().ConvertSpecifier("Name is invalid.");
                }
            }

            // key word
            if (isValid)
            {
                string[] keyWords =
                {
                    "goto",
                    "repeat", "until", "or", "end_repeat",
                    "return", "exit",
                    "if", "then", "elsif", "else", "end_if",
                    "case", "of", "end_case",
                    "for", "to", "by", "do", "end_for",
                    "while", "end_while",
                    "not", "mod", "and", "xor", "or"
                };
                foreach (var keyWord in keyWords)
                {
                    if (keyWord.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        isValid = false;
                        warningReason = LanguageManager.GetInstance().ConvertSpecifier("Name is invalid.");
                    }
                }
            }

            if (!isValid)
            {
                var warningDialog = new WarningDialog(warningMessage, warningReason)
                { Owner = Application.Current.MainWindow };
                warningDialog.ShowDialog();
            }

            return isValid;
        }


        public bool SaveOptions()
        {
            return true;
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                _isDirty = value;
                IsDirtyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsDirtyChanged;

        public void Refresh()
        {
            ChangeCommand.RaiseCanExecuteChanged();
            IsEnable = !_controller.IsOnline;
        }
    }
}
