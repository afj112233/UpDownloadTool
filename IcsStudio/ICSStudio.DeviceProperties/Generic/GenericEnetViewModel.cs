using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using ICSStudio.DeviceProperties.Common;
using ICSStudio.DeviceProperties.Generic.Panel;
using ICSStudio.Interfaces.Common;
using ICSStudio.Interfaces.DeviceModule;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.DeviceProperties.Generic
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class GenericEnetViewModel : DevicePropertiesViewModelBase
    {
        private IOptionPanelNode _activeNode;

        private List<IOptionPanelNode> _flatOptionPanelNodes;

        public GenericEnetViewModel(
            IController controller,
            IDeviceModule deviceModule) : this(controller, deviceModule, false)
        {
        }

        public GenericEnetViewModel(
            IController controller,
            IDeviceModule deviceModule,
            bool isCreating) : base(controller, deviceModule, isCreating)
        {
            OriginalModule = deviceModule as GeneralEthernet;
            ModifiedModule = new ModifiedGeneralEthernet(Controller, OriginalModule);

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
        }

        public GeneralEthernet OriginalModule { get; }
        public ModifiedGeneralEthernet ModifiedModule { get; }

        public override void Cleanup()
        {
            Controller myController = Controller as Controller;
            if (myController != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    myController, "IsOnlineChanged", OnIsOnlineChanged);
            }

            base.Cleanup();
        }

        public override string Caption
        {
            get
            {
                if (IsCreating)
                    return "New Module";

                return
                    $"Module Properties Report: {OriginalModule.ParentModule.Name} ({OriginalModule.CatalogNumber} {OriginalModule.Major}.{OriginalModule.Minor:D3})";
            }
        }

        public override int Apply()
        {
            return DoApply();
        }

        private void CreateOptionPanelNodes()
        {
            _flatOptionPanelNodes = new List<IOptionPanelNode>();

            var generalViewModel = new GeneralViewModel(new GeneralPanel(), ModifiedModule, IsCreating);
            var generalPanelNode =
                new DeviceOptionPanelNode("1", "General", "General", generalViewModel);

            OptionPanelNodes.Add(generalPanelNode);
            _flatOptionPanelNodes.Add(generalPanelNode);

            if (!IsCreating)
            {
                var connectionViewModel = new ConnectionViewModel(new ConnectionPanel(), ModifiedModule);
                var connectionNode =
                    new DeviceOptionPanelNode("2", "Connection", "Connection", connectionViewModel);

                var moduleInfoViewModel = new ModuleInfoViewModel(new ModuleInfoPanel(), ModifiedModule);
                var moduleInfoNode =
                    new DeviceOptionPanelNode("3", "ModuleInfo", "ModuleInfo", moduleInfoViewModel);


                OptionPanelNodes.Add(connectionNode);
                OptionPanelNodes.Add(moduleInfoNode);

                _flatOptionPanelNodes.Add(connectionNode);
                _flatOptionPanelNodes.Add(moduleInfoNode);
            }

            //
            foreach (var panelNode in _flatOptionPanelNodes.OfType<DeviceOptionPanelNode>())
            {
                panelNode.PropertyChanged += PanelNodeOnPropertyChanged;
            }

            //
            generalPanelNode.IsSelected = true;
        }

        #region Command

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

        #endregion

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

            var currentPanelNode = _activeNode as DeviceOptionPanelNode;
            if (currentPanelNode != null)
            {
                currentPanelNode.OptionPanel.Show();
            }

            ApplyCommand.RaiseCanExecuteChanged();
            RaisePropertyChanged("Caption");

        }
    }
}
