using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DeviceProperties.AnalogIOs.Panel;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.Utils;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.DeviceProperties.AnalogIOs
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class DIOAnalogViewModel : DevicePropertiesViewModelBase
    {
        private IOptionPanelNode _activeNode;

        private List<IOptionPanelNode> _flatOptionPanelNodes;

        private readonly IDataServer _dataServer;

        public DIOAnalogViewModel(
            IController controller,
            IDeviceModule deviceModule) : this(controller, deviceModule, false)
        {
        }

        public DIOAnalogViewModel(
            IController controller,
            IDeviceModule deviceModule,
            bool isCreating) : base(controller, deviceModule, isCreating)
        {
            OriginalAnalogIO = deviceModule as AnalogIO;

            _dataServer = controller.CreateDataServer();
            ModifiedAnalogIO = new ModifiedAnalogIO(Controller, OriginalAnalogIO, _dataServer);

            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);

            CreateOptionPanelNodes();

            Controller myController = Controller as Controller;
            if (myController != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    myController, "IsOnlineChanged", OnIsOnlineChanged);
            }

            WeakEventManager<ModifiedAnalogIO, EventArgs>.AddHandler(
                ModifiedAnalogIO,
                "ConfigValueChanged",
                OnConfigValueChanged);

            _dataServer.StartMonitoring(true, false);
        }


        public override void Cleanup()
        {
            _dataServer.StopMonitoring(true, true);

            Controller myController = Controller as Controller;
            if (myController != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    myController, "IsOnlineChanged", OnIsOnlineChanged);
            }

            WeakEventManager<ModifiedAnalogIO, EventArgs>.RemoveHandler(
                ModifiedAnalogIO,
                "ConfigValueChanged",
                OnConfigValueChanged);

            base.Cleanup();
        }

        public AnalogIO OriginalAnalogIO { get; }
        public ModifiedAnalogIO ModifiedAnalogIO { get; }

        public override string Caption
        {
            get
            {
                if (IsCreating)
                    return "New Module";

                return
                    $"Module Properties: {OriginalAnalogIO.ParentModule.Name}:{OriginalAnalogIO.Slot} ({OriginalAnalogIO.CatalogNumber.RemoveSeries()} {OriginalAnalogIO.Major}.{OriginalAnalogIO.Minor:D3})";
            }
        }

        #region Command

        private void ExecuteHelpCommand()
        {
            //TODO(gjc): add code here
        }

        private bool CanExecuteApplyCommand()
        {
            foreach (var panelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
                if (panelNode.Visibility == Visibility.Visible && panelNode.OptionPanel != null)
                    if (panelNode.OptionPanel.IsDirty)
                    {
                        // set dirty
                        IProjectInfoService projectInfoService =
                            Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                        projectInfoService?.SetProjectDirty();
                        return true;
                    }

            return false;
        }

        private void ExecuteApplyCommand()
        {
            DoApply();
        }

        private void ExecuteCancelCommand()
        {
            CloseAction?.Invoke();
            DialogResult = false;
        }

        private void ExecuteOkCommand()
        {
            var result = 0;
            if (IsCreating || CanExecuteApplyCommand())
                result = DoApply();

            if (result == 0)
            {
                CloseAction?.Invoke();
                DialogResult = true;
            }
        }


        #endregion

        private void CreateOptionPanelNodes()
        {
            int panelIndex = 1;

            var generalViewModel = new GeneralViewModel(new GeneralPanel(), ModifiedAnalogIO);
            var generalPanelNode =
                new DeviceOptionPanelNode(panelIndex.ToString(), "General", "General", generalViewModel);
            PropertyChangedEventManager.AddHandler(
                generalViewModel, GeneralViewModelOnPropertyChanged, string.Empty);
            PropertyChangedEventManager.AddHandler(
                generalPanelNode, PanelNodeOnPropertyChanged, string.Empty);
            OptionPanelNodes.Add(generalPanelNode);
            panelIndex++;

            var connectionViewModel = new ConnectionViewModel(new ConnectionPanel(), ModifiedAnalogIO);
            var connectionPanelNode =
                new DeviceOptionPanelNode(panelIndex.ToString(), "Connection", "Connection", connectionViewModel);
            PropertyChangedEventManager.AddHandler(
                connectionPanelNode, PanelNodeOnPropertyChanged, string.Empty);
            OptionPanelNodes.Add(connectionPanelNode);
            panelIndex++;

            var moduleInfoViewModel = new ModuleInfoViewModel(new ModuleInfoPanel(), ModifiedAnalogIO);
            var moduleInfoPanelNode = new DeviceOptionPanelNode(panelIndex.ToString(), "ModuleInfo", "ModuleInfo",
                moduleInfoViewModel);
            PropertyChangedEventManager.AddHandler(
                moduleInfoPanelNode, PanelNodeOnPropertyChanged, string.Empty);
            OptionPanelNodes.Add(moduleInfoPanelNode);
            panelIndex++;

            var module = OriginalAnalogIO.Profiles.GetModule(OriginalAnalogIO.Major);
            if (module != null)
            {
                if (module.NumberOfInputs > 0)
                {
                    if (OriginalAnalogIO.Profiles.CatalogNumber == "ICD-IF4")
                    {
                        var ifConfigViewModel =
                            new IFConfigViewModel(new IFConfigPanel(), ModifiedAnalogIO);
                        var ifConfigPanelNode =
                            new DeviceOptionPanelNode(panelIndex.ToString(), "Configuration", "Configuration",
                                ifConfigViewModel);
                        PropertyChangedEventManager.AddHandler(
                            ifConfigPanelNode, PanelNodeOnPropertyChanged, string.Empty);
                        OptionPanelNodes.Add(ifConfigPanelNode);
                        panelIndex++;
                    }

                    if (OriginalAnalogIO.Profiles.CatalogNumber == "ICD-IR4")
                    {
                        var irConfigViewModel =
                            new IRConfigViewModel(new IRConfigPanel(), ModifiedAnalogIO);
                        var irConfigPanelNode =
                            new DeviceOptionPanelNode(panelIndex.ToString(), "Configuration", "Configuration",
                                irConfigViewModel);
                        PropertyChangedEventManager.AddHandler(
                            irConfigPanelNode, PanelNodeOnPropertyChanged, string.Empty);
                        OptionPanelNodes.Add(irConfigPanelNode);
                        panelIndex++;
                    }

                    var alarmConfigViewModel = new AlarmConfigViewModel(new AlarmConfigPanel(), ModifiedAnalogIO);
                    var alarmConfigPanelNode = new DeviceOptionPanelNode(panelIndex.ToString(), "Alarm Configuration",
                        "Alarm Configuration", alarmConfigViewModel);
                    PropertyChangedEventManager.AddHandler(alarmConfigPanelNode, PanelNodeOnPropertyChanged,
                        String.Empty);
                    OptionPanelNodes.Add(alarmConfigPanelNode);
                    panelIndex++;

                }

                if (module.NumberOfOutputs > 0)
                {
                    var outputConfigViewModel =
                        new OutputConfigViewModel(new OutputConfigPanel(), ModifiedAnalogIO);
                    var outputConfigPanelNode =
                        new DeviceOptionPanelNode(panelIndex.ToString(), "Configuration", "Configuration",
                            outputConfigViewModel);
                    PropertyChangedEventManager.AddHandler(
                        outputConfigPanelNode, PanelNodeOnPropertyChanged, string.Empty);
                    OptionPanelNodes.Add(outputConfigPanelNode);
                    panelIndex++;

                    var limitsConfigViewModel = new LimitsConfigViewModel(new LimitsConfigPanel(), ModifiedAnalogIO);
                    var limitsConfigPanelNode =
                        new DeviceOptionPanelNode(panelIndex.ToString(), "Limits Configuration", "Limits Configuration",
                            limitsConfigViewModel);
                    PropertyChangedEventManager.AddHandler(
                        limitsConfigPanelNode, PanelNodeOnPropertyChanged, string.Empty);
                    OptionPanelNodes.Add(limitsConfigPanelNode);
                    panelIndex++;

                    var faultProgramActionViewModel =
                        new FaultProgramActionViewModel(new FaultProgramActionPanel(), ModifiedAnalogIO);
                    var faultProgramActionPanelNode =
                        new DeviceOptionPanelNode(panelIndex.ToString(), "Fault/Program Action", "Fault/Program Action",
                            faultProgramActionViewModel);
                    PropertyChangedEventManager.AddHandler(faultProgramActionPanelNode, PanelNodeOnPropertyChanged,
                        string.Empty);
                    OptionPanelNodes.Add(faultProgramActionPanelNode);
                    panelIndex++;
                }

                var calibrationViewModel = new CalibrationViewModel(new CalibrationPanel(), ModifiedAnalogIO);
                var calibrationPanelNode = new DeviceOptionPanelNode(panelIndex.ToString(), "Calibration",
                    "Calibration", calibrationViewModel);
                PropertyChangedEventManager.AddHandler(calibrationPanelNode, PanelNodeOnPropertyChanged, string.Empty);
                OptionPanelNodes.Add(calibrationPanelNode);
                //panelIndex++;
            }


            // end
            _flatOptionPanelNodes = OptionPanelNodes;
            generalPanelNode.IsSelected = true;
        }


        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.NewValue)
                {
                    // online
                    DeviceOptionPanelNode deviceOptionPanelNode = _activeNode as DeviceOptionPanelNode;
                    deviceOptionPanelNode?.OptionPanel?.Show();
                }
                else
                {
                    // offline
                    foreach (var panelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
                    {
                        panelNode.OptionPanel.Show();
                    }
                }

                RaisePropertyChanged("Status");
            });
        }

        private void GeneralViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Connection")
            {
                foreach (var optionPanelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
                {
                    optionPanelNode.RaisePropertyChanged("Title");
                    optionPanelNode.RaisePropertyChanged("Visibility");
                }
            }
        }

        private void PanelNodeOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var optionPanelNode = sender as IOptionPanelNode;

            if (optionPanelNode != null)
            {
                if (e.PropertyName == "IsSelected")
                    if (optionPanelNode.IsSelected)
                        SelectNode(optionPanelNode);

                if (e.PropertyName == "Title") ApplyCommand.RaiseCanExecuteChanged();
            }
        }

        private void SelectNode(IOptionPanelNode node)
        {
            if (node == _activeNode)
                return;

            if (_activeNode != null)
            {
                _activeNode.IsActive = false;
            }

            _activeNode = node;

            OptionPanelTitle = node.Label;
            OptionPanelContent = node.Content;

            node.IsExpanded = true;
            node.IsActive = true;

            ApplyCommand.RaiseCanExecuteChanged();
        }

        private int DoApply()
        {
            var result = DoPreApply();
            if (result < 0)
                return result;

            result = DoApplying();
            if (result < 0)
                return result;

            DoPostApply();

            return 0;
        }

        private int DoPreApply()
        {
            // check valid
            foreach (var panelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
                if (panelNode.Visibility == Visibility.Visible && panelNode.OptionPanel != null)
                    if (panelNode.OptionPanel.CheckValid() < 0)
                    {
                        panelNode.IsSelected = true;
                        return -1;
                    }

            // Changed Externally
            if (ModifiedAnalogIO.IsChangedExternally)
            {
                //string warningReason = "The configuration of this module has changed externally." +
                //                       "\nApplying this changes will result in the external changes being discarded." +
                //                       "\n\nApply Changes?";

                if (MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier("AnalogIOs.DIOAnalog"), "ICS Studio",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return -1;
            }

            return 0;
        }

        private int DoApplying()
        {
            // save
            foreach (var panelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
                if (panelNode.Visibility == Visibility.Visible && panelNode.OptionPanel != null)
                    if (!panelNode.OptionPanel.SaveOptions())
                    {
                        panelNode.IsSelected = true;
                        return -1;
                    }

            return 0;
        }

        private void DoPostApply()
        {
            // panel update
            foreach (var panelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
                if (panelNode.Visibility == Visibility.Visible && panelNode.OptionPanel != null)
                {
                    panelNode.OptionPanel.CheckDirty();
                    panelNode.RaisePropertyChanged("Title");
                }

            ModifiedAnalogIO.PostApply();

            ApplyCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged("Caption");
        }

        private void OnConfigValueChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                DeviceOptionPanelNode deviceOptionPanelNode = _activeNode as DeviceOptionPanelNode;
                deviceOptionPanelNode?.OptionPanel?.Show();

            });
        }

        public override int Apply()
        {
            return DoApply();
        }
    }
}
