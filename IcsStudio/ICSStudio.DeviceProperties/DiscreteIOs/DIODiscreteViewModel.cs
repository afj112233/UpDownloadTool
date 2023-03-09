using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.DeviceProperties.DiscreteIOs.Panel;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.Utils;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.DeviceProperties.DiscreteIOs
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class DIODiscreteViewModel : DevicePropertiesViewModelBase
    {
        private IOptionPanelNode _activeNode;

        private List<IOptionPanelNode> _flatOptionPanelNodes;

        private bool _beModuleNameChanged;
        private bool _beSlotChanged;

        private readonly IDataServer _dataServer;

        public DIODiscreteViewModel(
            IController controller,
            IDeviceModule deviceModule) : this(controller, deviceModule, false)
        {
        }

        public DIODiscreteViewModel(
            IController controller,
            IDeviceModule deviceModule,
            bool isCreating) : base(controller, deviceModule, isCreating)
        {
            OriginalDiscreteIO = deviceModule as DiscreteIO;
            
            _dataServer = controller.CreateDataServer();
            ModifiedDiscreteIO = new ModifiedDiscreteIO(Controller, OriginalDiscreteIO, _dataServer);

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

            WeakEventManager<ModifiedDiscreteIO, EventArgs>.AddHandler(
                ModifiedDiscreteIO,
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

            WeakEventManager<ModifiedDiscreteIO, EventArgs>.RemoveHandler(
                ModifiedDiscreteIO,
                "ConfigValueChanged",
                OnConfigValueChanged);

            base.Cleanup();
        }

        public DiscreteIO OriginalDiscreteIO { get; }
        public ModifiedDiscreteIO ModifiedDiscreteIO { get; }

        public override string Caption
        {
            get
            {
                if (IsCreating)
                    return "New Module";

                return
                    $"Module Properties: {OriginalDiscreteIO.ParentModule.Name}:{OriginalDiscreteIO.Slot} ({OriginalDiscreteIO.CatalogNumber.RemoveSeries()} {OriginalDiscreteIO.Major}.{OriginalDiscreteIO.Minor:D3})";
            }
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

            ModifiedDiscreteIO.PostApply();

            ApplyCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged("Caption");

            if (_beModuleNameChanged || _beSlotChanged)
            {
                //var studioUIService =
                //    Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
                //studioUIService?.UpdateUI();
            }
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
            _beModuleNameChanged = !string.Equals(OriginalDiscreteIO.Name, ModifiedDiscreteIO.Name);
            _beSlotChanged = OriginalDiscreteIO.Slot != ModifiedDiscreteIO.Slot;

            // check valid
            foreach (var panelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
                if (panelNode.Visibility == Visibility.Visible && panelNode.OptionPanel != null)
                    if (panelNode.OptionPanel.CheckValid() < 0)
                    {
                        panelNode.IsSelected = true;
                        return -1;
                    }

            // Changed Externally
            if (ModifiedDiscreteIO.IsChangedExternally)
            {
                //string warningReason = "The configuration of this module has changed externally." +
                //                       "\nApplying this changes will result in the external changes being discarded." +
                //                       "\n\nApply Changes?";

                if (MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier("DiscreteIOs.DIODiscrete"), "ICS Studio",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    return -1;
            }

            return 0;
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

        private void ExecuteCancelCommand()
        {
            CloseAction?.Invoke();
            DialogResult = false;
        }

        private void ExecuteHelpCommand()
        {
        }

        private void CreateOptionPanelNodes()
        {
            var generalViewModel = new GeneralViewModel(new GeneralPanel(), ModifiedDiscreteIO);
            var generalPanelNode =
                new DeviceOptionPanelNode("1", "General", "General", generalViewModel);

            PropertyChangedEventManager.AddHandler(
                generalViewModel, GeneralViewModelOnPropertyChanged, string.Empty);
            PropertyChangedEventManager.AddHandler(
                generalPanelNode, PanelNodeOnPropertyChanged, string.Empty);
            OptionPanelNodes.Add(generalPanelNode);

            var connectionViewModel = new ConnectionViewModel(new ConnectionPanel(), ModifiedDiscreteIO);
            var connectionPanelNode = new DeviceOptionPanelNode("2", "Connection", "Connection", connectionViewModel);
            PropertyChangedEventManager.AddHandler(
                connectionPanelNode, PanelNodeOnPropertyChanged, string.Empty);
            OptionPanelNodes.Add(connectionPanelNode);

            var moduleInfoViewModel = new ModuleInfoViewModel(new ModuleInfoPanel(), ModifiedDiscreteIO);
            var moduleInfoPanelNode = new DeviceOptionPanelNode("3", "ModuleInfo", "ModuleInfo", moduleInfoViewModel);
            PropertyChangedEventManager.AddHandler(
                moduleInfoPanelNode, PanelNodeOnPropertyChanged, string.Empty);
            OptionPanelNodes.Add(moduleInfoPanelNode);

            //TODO(gjc): need edit here
            if (OriginalDiscreteIO.CatalogNumber.StartsWith("1734-IB")
                || OriginalDiscreteIO.CatalogNumber.StartsWith("ICD-IB")
                || OriginalDiscreteIO.CatalogNumber.StartsWith("ICD-IQ")
                || OriginalDiscreteIO.CatalogNumber.StartsWith("Embedded-IQ"))
            {
                var viewModel =
                    new InputFilterTimeConfigViewModel(new InputFilterTimeConfigPanel(), ModifiedDiscreteIO);
                var panelNode = new DeviceOptionPanelNode("4", "Configuration", "Configuration", viewModel);
                PropertyChangedEventManager.AddHandler(
                    panelNode, PanelNodeOnPropertyChanged, string.Empty);
                OptionPanelNodes.Add(panelNode);

            }
            else if (OriginalDiscreteIO.CatalogNumber.StartsWith("1734-OB")
                     || OriginalDiscreteIO.CatalogNumber.StartsWith("ICD-OB")
                     || OriginalDiscreteIO.CatalogNumber.StartsWith("ICD-OV")
                     || OriginalDiscreteIO.CatalogNumber.StartsWith("Embedded-OB"))
            {
                var viewModel = new FaultProgramActionViewModel(new FaultProgramActionPanel(), ModifiedDiscreteIO);
                var panelNode =
                    new DeviceOptionPanelNode("4", "Fault/Program Action", "Fault/Program Action", viewModel);
                PropertyChangedEventManager.AddHandler(
                    panelNode, PanelNodeOnPropertyChanged, string.Empty);
                OptionPanelNodes.Add(panelNode);
            }
            //

            _flatOptionPanelNodes = OptionPanelNodes;
            generalPanelNode.IsSelected = true;
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