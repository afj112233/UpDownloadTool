using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DeviceProperties.Adapters.Panel;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.Utils;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.DeviceProperties.Adapters
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class DIOEnetAdapterViewModel : DevicePropertiesViewModelBase
    {
        private IOptionPanelNode _activeNode;

        private List<IOptionPanelNode> _flatOptionPanelNodes;

        public DIOEnetAdapterViewModel(IController controller,
            IDeviceModule deviceModule) : this(controller, deviceModule, false)
        {
        }

        public DIOEnetAdapterViewModel(
            IController controller,
            IDeviceModule deviceModule,
            bool isCreating) : base(controller, deviceModule, isCreating)
        {
            OriginalAdapter = deviceModule as CommunicationsAdapter;
            ModifiedAdapter = new ModifiedDIOEnetAdapter(Controller, OriginalAdapter);

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

        public CommunicationsAdapter OriginalAdapter { get; }
        public ModifiedDIOEnetAdapter ModifiedAdapter { get; }

        public override string Caption
        {
            get
            {
                if (IsCreating)
                    return "New Module";

                return
                    $"Module Properties: {OriginalAdapter.ParentModule.Name}:0 ({OriginalAdapter.CatalogNumber.RemoveSeries()} {OriginalAdapter.Major}.{OriginalAdapter.Minor:D3})";
            }
        }

        public override void Cleanup()
        {
            //
            foreach (var panelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
            {
                panelNode.PropertyChanged -= PanelNodeOnPropertyChanged;
            }

            //
            Controller myController = Controller as Controller;
            if (myController != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    myController, "IsOnlineChanged", OnIsOnlineChanged);
            }

            base.Cleanup();
        }

        private void CreateOptionPanelNodes()
        {
            var generalViewModel = new GeneralViewModel(new GeneralPanel(), ModifiedAdapter);
            var generalPanelNode =
                new DeviceOptionPanelNode("1", "General", "General", generalViewModel);

            var connectionViewModel = new ConnectionViewModel(new ConnectionPanel(), ModifiedAdapter);
            var connectionNode =
                new DeviceOptionPanelNode("2", "Connection", "Connection", connectionViewModel);

            var moduleInfoViewModel = new ModuleInfoViewModel(new ModuleInfoPanel(), ModifiedAdapter);
            var moduleInfoNode =
                new DeviceOptionPanelNode("3", "ModuleInfo", "ModuleInfo", moduleInfoViewModel);

            var internetProtocolViewModel = new InternetProtocolViewModel(new InternetProtocolPanel(), OriginalAdapter);
            var internetProtocolNode =
                new DeviceOptionPanelNode("4", "InternetProtocol", "InternetProtocol", internetProtocolViewModel);


            var portConfigurationViewModel =
                new PortConfigurationViewModel(new PortConfigurationPanel(), OriginalAdapter);
            var portConfigurationNode =
                new DeviceOptionPanelNode("5", "PortConfiguration", "PortConfiguration", portConfigurationViewModel);

            var networkViewModel = new NetworkViewModel(new NetworkPanel(), ModifiedAdapter);
            var networkNode = new DeviceOptionPanelNode("6", "Network", "Network", networkViewModel);

            var chassisSizeViewModel = new ChassisSizeViewModel(new ChassisSizePanel(), ModifiedAdapter);
            var chassisSizeNode = new DeviceOptionPanelNode("7", "ChassisSize", "ChassisSize", chassisSizeViewModel);


            //
            OptionPanelNodes.Add(generalPanelNode);
            OptionPanelNodes.Add(connectionNode);
            OptionPanelNodes.Add(moduleInfoNode);
            OptionPanelNodes.Add(internetProtocolNode);
            OptionPanelNodes.Add(portConfigurationNode);
            OptionPanelNodes.Add(networkNode);
            OptionPanelNodes.Add(chassisSizeNode);

            _flatOptionPanelNodes = new List<IOptionPanelNode>
            {
                generalPanelNode,
                connectionNode,
                moduleInfoNode,
                internetProtocolNode,
                portConfigurationNode,
                networkNode,
                chassisSizeNode
            };

            //
            foreach (var panelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
            {
                panelNode.PropertyChanged += PanelNodeOnPropertyChanged;
            }

            //
            generalPanelNode.IsSelected = true;
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

        private void ExecuteHelpCommand()
        {
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
            if (IsCreating || CanExecuteApplyCommand()) result = DoApply();

            if (result == 0)
            {
                CloseAction?.Invoke();
                DialogResult = true;
            }
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

        private void DoPostApply()
        {
            // panel update
            foreach (var panelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
                if (panelNode.Visibility == Visibility.Visible && panelNode.OptionPanel != null)
                {
                    panelNode.OptionPanel.CheckDirty();
                    panelNode.RaisePropertyChanged("Title");
                }

            var currentPanelNode = _activeNode as DeviceOptionPanelNode;
            if (currentPanelNode != null)
            {
                currentPanelNode.OptionPanel.Show();
            }


            ApplyCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged("Caption");

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

            return 0;
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

        public override int Apply()
        {
            return DoApply();
        }
    }
}