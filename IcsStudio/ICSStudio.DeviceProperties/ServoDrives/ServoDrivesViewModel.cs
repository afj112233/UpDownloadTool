using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.DeviceProperties.ServoDrives.Panel;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using ConnectionPanel = ICSStudio.DeviceProperties.ServoDrives.Panel.ConnectionPanel;
using ConnectionViewModel = ICSStudio.DeviceProperties.ServoDrives.Panel.ConnectionViewModel;
using InternetProtocolPanel = ICSStudio.DeviceProperties.Adapters.Panel.InternetProtocolPanel;
using ModuleInfoPanel = ICSStudio.DeviceProperties.ServoDrives.Panel.ModuleInfoPanel;
using ModuleInfoViewModel = ICSStudio.DeviceProperties.ServoDrives.Panel.ModuleInfoViewModel;
using NetworkPanel = ICSStudio.DeviceProperties.Adapters.Panel.NetworkPanel;
using PortConfigurationPanel = ICSStudio.DeviceProperties.Adapters.Panel.PortConfigurationPanel;

namespace ICSStudio.DeviceProperties.ServoDrives
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class ServoDrivesViewModel : DevicePropertiesViewModelBase
    {
        private List<IOptionPanelNode> _flatOptionPanelNodes;

        private IOptionPanelNode _activeNode;

        public ServoDrivesViewModel(
            IController controller,
            IDeviceModule deviceModule) : this(controller, deviceModule, false)
        {
        }

        public ServoDrivesViewModel(
            IController controller,
            IDeviceModule deviceModule,
            bool isCreating) : base(controller, deviceModule, isCreating)
        {
            OriginalMotionDrive = (CIPMotionDrive) deviceModule;
            ModifiedMotionDrive = new ModifiedMotionDrive(Controller, OriginalMotionDrive);

            OkCommand = new RelayCommand(ExecuteOkCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecuteApplyCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);

            Controller myController = Controller as Controller;
            if (myController != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    myController, "IsOnlineChanged", OnIsOnlineChanged);
            }

            CreateOptionPanelNodes();
        }

        public CIPMotionDrive OriginalMotionDrive { get; }
        public ModifiedMotionDrive ModifiedMotionDrive { get; }

        public override string Caption
        {
            get
            {
                if (IsCreating)
                    return "New Module";

                //TODO(gjc): need edit here
                return $"Module Properties:({OriginalMotionDrive.DisplayText})";
            }
        }

        #region Command

        private void ExecuteCancelCommand()
        {
            CloseAction?.Invoke();
            DialogResult = false;
        }

        protected virtual void ExecuteOkCommand()
        {
            int result = 0;
            if (IsCreating || CanExecuteApplyCommand())
            {
                result = DoApply();
            }

            if (result == 0)
            {
                CloseAction?.Invoke();
                DialogResult = true;
            }

        }

        protected virtual void ExecuteApplyCommand()
        {
            DoApply();
        }

        protected virtual bool CanExecuteApplyCommand()
        {
            foreach (var panelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
            {
                if (panelNode.Visibility == Visibility.Visible && panelNode.OptionPanel != null)
                {
                    if (panelNode.OptionPanel.IsDirty)
                    {
                        // set dirty
                        IProjectInfoService projectInfoService =
                            Package.GetGlobalService(typeof(SProjectInfoService)) as IProjectInfoService;
                        projectInfoService?.SetProjectDirty();

                        return true;
                    }

                }
            }

            return false;
        }

        protected virtual void ExecuteHelpCommand()
        {

        }

        #endregion

        #region Private

        private void CreateOptionPanelNodes()
        {
            var generalViewModel = new GeneralViewModel(new GeneralPanel(), ModifiedMotionDrive);
            var generalPanelNode =
                new DeviceOptionPanelNode("1", "General", "General", generalViewModel);
            generalViewModel.PropertyChanged += GeneralViewModelOnPropertyChanged;
            generalPanelNode.PropertyChanged += PanelNodeOnPropertyChanged;


            var connectionViewModel = new ConnectionViewModel(new ConnectionPanel(), ModifiedMotionDrive);
            var connectionNode =
                new DeviceOptionPanelNode("2", "Connection", "Connection", connectionViewModel);
            connectionNode.PropertyChanged += PanelNodeOnPropertyChanged;

            var timeSyncViewModel = new TimeSyncViewModel(new TimeSyncPanel(), ModifiedMotionDrive);
            var timeSyncNode =
                new DeviceOptionPanelNode("3", "TimeSync", "TimeSync", timeSyncViewModel);
            timeSyncNode.PropertyChanged += PanelNodeOnPropertyChanged;

            var moduleInfoViewModel = new ModuleInfoViewModel(new ModuleInfoPanel(), ModifiedMotionDrive);
            var moduleInfoNode =
                new DeviceOptionPanelNode("4", "ModuleInfo", "ModuleInfo", moduleInfoViewModel);
            moduleInfoNode.PropertyChanged += PanelNodeOnPropertyChanged;

            var internetProtocolViewModel =
                new InternetProtocolViewModel(new InternetProtocolPanel(), ModifiedMotionDrive.OriginalMotionDrive);
            var internetProtocolNode =
                new DeviceOptionPanelNode("5", "InternetProtocol", "InternetProtocol", internetProtocolViewModel);
            internetProtocolNode.PropertyChanged += PanelNodeOnPropertyChanged;

            var portConfigurationViewModel = new PortConfigurationViewModel(new PortConfigurationPanel(),
                ModifiedMotionDrive.OriginalMotionDrive);
            var portConfigurationNode =
                new DeviceOptionPanelNode("6", "PortConfiguration", "PortConfiguration", portConfigurationViewModel);
            portConfigurationNode.PropertyChanged += PanelNodeOnPropertyChanged;

            var networkViewModel = new NetworkViewModel(new NetworkPanel());
            var networkNode =
                new DeviceOptionPanelNode("7", "Network", "Network", networkViewModel);
            networkNode.PropertyChanged += PanelNodeOnPropertyChanged;

            // Motion
            List<IOptionPanelNode> motionNodes = new List<IOptionPanelNode>();

            var associatedAxesViewModel = new AssociatedAxesViewModel(new AssociatedAxesPanel(), ModifiedMotionDrive);
            var associatedAxesNode =
                new DeviceOptionPanelNode("8", "AssociatedAxes", "AssociatedAxes", associatedAxesViewModel);
            associatedAxesNode.PropertyChanged += PanelNodeOnPropertyChanged;

            var powerViewModel = new PowerViewModel(new PowerPanel(), ModifiedMotionDrive);
            var powerNode = new DeviceOptionPanelNode("9", "Power", "Power", powerViewModel);
            powerNode.PropertyChanged += PanelNodeOnPropertyChanged;

            var digitalInputViewMode = new DigitalInputViewModel(new DigitalInputPanel(), ModifiedMotionDrive);
            var digitalInputNode =
                new DeviceOptionPanelNode("10", "DigitalInput", "DigitalInput", digitalInputViewMode);
            digitalInputNode.PropertyChanged += PanelNodeOnPropertyChanged;

            var diagnosticsViewModel = new DiagnosticsViewModel(new DiagnosticsPanel());
            var diagnosticsNode = new DeviceOptionPanelNode("11", "MotionDiagnostics", "MotionDiagnostics", diagnosticsViewModel);
            diagnosticsNode.PropertyChanged += PanelNodeOnPropertyChanged;

            motionNodes.Add(associatedAxesNode);
            motionNodes.Add(powerNode);
            motionNodes.Add(digitalInputNode);
            motionNodes.Add(diagnosticsNode);
            var motionPanelNode =
                new DeviceOptionPanelNode("-1", "Motion", "", null, motionNodes)
                {
                    IsExpanded = true
                };

            OptionPanelNodes.Add(generalPanelNode);
            OptionPanelNodes.Add(connectionNode);
            OptionPanelNodes.Add(timeSyncNode);
            OptionPanelNodes.Add(moduleInfoNode);
            OptionPanelNodes.Add(internetProtocolNode);
            OptionPanelNodes.Add(portConfigurationNode);
            OptionPanelNodes.Add(networkNode);

            OptionPanelNodes.Add(motionPanelNode);
            //
            _flatOptionPanelNodes = new List<IOptionPanelNode>();
            _flatOptionPanelNodes.AddRange(OptionPanelNodes);
            _flatOptionPanelNodes.AddRange(motionPanelNode.Children);

            //
            generalPanelNode.IsSelected = true;
        }

        private void GeneralViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Revision")
            {
                foreach (var panelNode in _flatOptionPanelNodes)
                {
                    DeviceOptionPanelNode deviceOptionPanelNode = panelNode as DeviceOptionPanelNode;
                    deviceOptionPanelNode?.RaisePropertyChanged("Visibility");
                }
            }

        }

        private void PanelNodeOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IOptionPanelNode optionPanelNode = sender as IOptionPanelNode;

            if (optionPanelNode != null)
            {
                if (e.PropertyName == "IsSelected")
                {
                    if (optionPanelNode.IsSelected)
                    {
                        SelectNode(optionPanelNode);
                    }
                }

                if (e.PropertyName == "Title")
                {
                    ApplyCommand.RaiseCanExecuteChanged();
                }

            }

        }

        private void SelectNode(IOptionPanelNode node)
        {
            if (node == _activeNode)
                return;

            if (_activeNode != null)
            {
                _activeNode.IsActive = false;
                _activeNode = node;
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
            int result = DoPreApply();
            if (result < 0)
                return result;

            result = DoApplying();
            if (result < 0)
                return result;

            DoPostApply();

            return result;
        }

        private int DoPreApply()
        {
            // check valid
            foreach (var panelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
            {
                if (panelNode.Visibility == Visibility.Visible && panelNode.OptionPanel != null)
                {
                    if (panelNode.OptionPanel.CheckValid() < 0)
                    {
                        panelNode.IsSelected = true;
                        return -1;
                    }
                }
            }

            return 0;
        }

        private int DoApplying()
        {
            // save
            foreach (var panelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
            {
                if (panelNode.Visibility == Visibility.Visible && panelNode.OptionPanel != null)
                {
                    if (!panelNode.OptionPanel.SaveOptions())
                    {
                        panelNode.IsSelected = true;
                        return -1;
                    }
                }
            }

            return 0;
        }

        private void DoPostApply()
        {
            // panel update
            foreach (var panelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
            {
                if (panelNode.Visibility == Visibility.Visible && panelNode.OptionPanel != null)
                {
                    panelNode.OptionPanel.CheckDirty();
                    panelNode.RaisePropertyChanged("Title");
                }
            }

            var currentPanelNode = _activeNode as DeviceOptionPanelNode;
            if (currentPanelNode != null)
            {
                currentPanelNode.OptionPanel.Show();
            }

            ApplyCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged("Caption");

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
                        panelNode.OptionPanel?.Show();
                    }
                }

                RaisePropertyChanged("Status");
            });
        }

        #endregion

        public override int Apply()
        {
            return DoApply();
        }
    }
}
