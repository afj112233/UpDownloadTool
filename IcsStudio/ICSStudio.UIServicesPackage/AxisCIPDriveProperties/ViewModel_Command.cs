using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.SimpleServices.DataWrapper;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Project;
using ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel;
using Microsoft.VisualStudio.Shell.Interop;
using Package = Microsoft.VisualStudio.Shell.Package;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties
{
    public sealed partial class AxisCIPDrivePropertiesViewModel
    {
        private bool? _dialogResult;

        private bool CanExecuteManualTuneCommand()
        {
            var axisConfiguration =
                (AxisConfigurationType)Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis.AxisConfiguration);
            if (axisConfiguration == AxisConfigurationType.PositionLoop ||
                axisConfiguration == AxisConfigurationType.VelocityLoop)
                return true;

            return false;
        }

        private void ExecuteManualTuneCommand()
        {
            var createDialogService =
                (ICreateDialogService)Package.GetGlobalService(typeof(SCreateDialogService));

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell)Package.GetGlobalService(typeof(SVsUIShell));

            var window =
                createDialogService?.CreateManualTuneDialog(AxisTag);
            window?.Show(uiShell);
        }

        private void ExecuteOkCommand()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate { await ExecuteOkCommandAsync(); });
        }

        private async Task ExecuteOkCommandAsync()
        {
            int result = 0;

            if (CanExecuteApplyCommand())
            {
                result = await DoApplyAsync();
            }

            if (result == 0)
            {
                _dialogResult = true;
                CloseAction?.Invoke();
            }

        }


        private void ExecuteCancelCommand()
        {
            _dialogResult = false;
            CloseAction?.Invoke();
        }

        private void ExecuteApplyCommand()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate { await DoApplyAsync(); });
        }

        private bool CanExecuteApplyCommand()
        {
            foreach (var descriptor in _flatDescriptors)
            {
                var axisCIPDrivePanel = descriptor.OptionPanel as IAxisCIPDrivePanel;
                if (axisCIPDrivePanel != null)
                {
                    if (axisCIPDrivePanel.Visibility == Visibility.Visible && axisCIPDrivePanel.IsDirty)
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

        private void ExecuteHelpCommand()
        {
            //TODO(gjc):add code here
        }

        private async Task<int> DoApplyAsync()
        {
            // Calculate System Inertia 
            var propertiesCalculation = new PropertiesCalculation(ModifiedAxisCIPDrive);
            propertiesCalculation.CalculateSystemInertia();
            propertiesCalculation.CalculateScaling();

            //1.Pre-Apply
            DoPreApply();

            if (_beNeedCalculateOutOfBoxTuning)
            {
                // CalculateOutOfBoxTuning
                propertiesCalculation.CalculateOutOfBoxTuning();

                // CalculateFeedbackUnitRatio
                if (_beNeedCalculateFeedbackUnitRatio)
                {
                    propertiesCalculation.CalculateFeedbackUnitRatio();
                }

                // CalculateScalingFactor
                if (_beNeedCalculateScalingFactor)
                {
                    propertiesCalculation.CalculateScalingFactor();
                }
            }
            else if (_beNeedCalculateFeedbackUnitRatio || _beNeedCalculateScalingFactor)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                string message = "You have changed a parameter which affects other attributes.";
                message += "\r\nDo you want to automatically update all dependent attributes?";
                message += "\r\n";
                message += "\r\nRefer to Help for a list of dependent attributes.";
                message += "\r\n";

                if (MessageBox.Show(message, "ICS Studio", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    // CalculateFeedbackUnitRatio
                    if (_beNeedCalculateFeedbackUnitRatio)
                    {
                        propertiesCalculation.CalculateFeedbackUnitRatio();
                    }

                    // CalculateScalingFactor
                    if (_beNeedCalculateScalingFactor)
                    {
                        propertiesCalculation.CalculateScalingFactor();
                    }
                }
            }

            if (_beNeedCalculateFrequencyControl)
            {
                propertiesCalculation.CalculateFrequencyControl();
            }


            //2.Check valid
            foreach (var descriptor in _flatDescriptors)
            {
                var axisCIPDrivePanel = descriptor.OptionPanel as IAxisCIPDrivePanel;
                if (axisCIPDrivePanel != null)
                {
                    if (axisCIPDrivePanel.Visibility == Visibility.Visible)
                    {
                        var checkResult = axisCIPDrivePanel.CheckValid();
                        if (checkResult < 0)
                            return checkResult;
                    }
                }
            }


            //3.Applying
            await DoApplyingAsync();

            //4.Post-Apply
            DoPostApply();

            return 0;
        }

        private void DoPreApply()
        {
            // check need Calculate OutOfBox Tuning
            _beNeedCalculateOutOfBoxTuning = CheckNeedCalculateOutOfBoxTuning();

            // check need calculate FeedbackUnitRatio
            _beNeedCalculateFeedbackUnitRatio = CheckNeedCalculateFeedbackUnitRatio();

            // check need calculate scaling factor
            _beNeedCalculateScalingFactor = CheckNeedCalculateScalingFactor();

            // check need calculate frequency control
            _beNeedCalculateFrequencyControl = CheckNeedCalculateFrequencyControl();

            _beTagNameChanged = !AxisTag.Name.Equals(ModifiedTagName, StringComparison.OrdinalIgnoreCase);
            _beTagDescriptionChanged =
                !string.Equals(AxisTag.Description, ModifiedDescription, StringComparison.OrdinalIgnoreCase);
            _beAssignedGroupChanged = OriginalAxisCIPDrive.AssignedGroup != ModifiedAxisCIPDrive.AssignedGroup;

            _beAssociatedAxisNumberChanged = false;
            if (OriginalAxisCIPDrive.AssociatedModule != ModifiedAxisCIPDrive.AssociatedModule)
            {
                _beAssociatedAxisNumberChanged = true;
            }
            else if (OriginalAxisCIPDrive.AxisNumber != ModifiedAxisCIPDrive.AxisNumber)
            {
                _beAssociatedAxisNumberChanged = true;
            }

            _beCyclicReadUpdateListChanged = !StringListEqual.Equal(
                OriginalAxisCIPDrive.CyclicReadUpdateList,
                ModifiedAxisCIPDrive.CyclicReadUpdateList);

            _beCyclicWriteUpdateListChanged = !StringListEqual.Equal(
                OriginalAxisCIPDrive.CyclicWriteUpdateList,
                ModifiedAxisCIPDrive.CyclicWriteUpdateList);
        }

        private async Task DoApplyingAsync()
        {
            AxisTag.Name = ModifiedTagName;
            AxisTag.Description = ModifiedDescription;

            if (_beAssignedGroupChanged)
                OriginalAxisCIPDrive.AssignedGroup = ModifiedAxisCIPDrive.AssignedGroup;

            if (_beAssociatedAxisNumberChanged)
            {
                CIPMotionDrive cipMotionDrive = OriginalAxisCIPDrive.AssociatedModule as CIPMotionDrive;
                cipMotionDrive?.RemoveAxis(AxisTag, OriginalAxisCIPDrive.AxisNumber);

                cipMotionDrive = ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;
                if (cipMotionDrive != null)
                {
                    if (cipMotionDrive.AddAxis(AxisTag, ModifiedAxisCIPDrive.AxisNumber) == 0)
                    {
                        OriginalAxisCIPDrive.UpdateAxisChannel(cipMotionDrive, ModifiedAxisCIPDrive.AxisNumber);
                    }
                }
                else
                {
                    OriginalAxisCIPDrive.UpdateAxisChannel(null, 0);
                }
            }

            // cycle list
            if (_beCyclicReadUpdateListChanged)
            {
                OriginalAxisCIPDrive.CyclicReadUpdateList =
                    ModifiedAxisCIPDrive.CyclicReadUpdateList == null
                        ? null
                        : new List<string>(ModifiedAxisCIPDrive.CyclicReadUpdateList);
            }

            if (_beCyclicWriteUpdateListChanged)
            {
                OriginalAxisCIPDrive.CyclicWriteUpdateList =
                    ModifiedAxisCIPDrive.CyclicWriteUpdateList == null
                        ? null
                        : new List<string>(ModifiedAxisCIPDrive.CyclicWriteUpdateList);
            }


            // Drive Value
            OriginalAxisCIPDrive.CIPAxis.DriveModelTimeConstantBase =
                ModifiedAxisCIPDrive.CIPAxis.DriveModelTimeConstantBase;
            OriginalAxisCIPDrive.CIPAxis.DriveRatedPeakCurrent = ModifiedAxisCIPDrive.CIPAxis.DriveRatedPeakCurrent;
            OriginalAxisCIPDrive.CIPAxis.DriveMaxOutputFrequency = ModifiedAxisCIPDrive.CIPAxis.DriveMaxOutputFrequency;
            OriginalAxisCIPDrive.CIPAxis.BusOvervoltageOperationalLimit =
                ModifiedAxisCIPDrive.CIPAxis.BusOvervoltageOperationalLimit;
            OriginalAxisCIPDrive.CIPAxis.SystemAccelerationBase = ModifiedAxisCIPDrive.CIPAxis.SystemAccelerationBase;

            // RegistrationInputs
            ModifiedAxisCIPDrive.UpdateRegistrationInputs();

            if (IsOnLine)
            {
                await DoOnlineApplyingAsync();
            }
            else
            {
                // cip properties
                var differentAttributeList =
                    CipAttributeHelper.GetDifferentAttributeList(
                        ModifiedAxisCIPDrive.CIPAxis,
                        OriginalAxisCIPDrive.CIPAxis, _comparePropertiesList);

                OriginalAxisCIPDrive.CIPAxis.Apply(ModifiedAxisCIPDrive.CIPAxis, differentAttributeList);

                ResetEditPropertiesList();

                var propertyNames =
                    CipAttributeHelper.AttributeIdsToNames(
                        OriginalAxisCIPDrive.CIPAxis.GetType(),
                        differentAttributeList);

                OriginalAxisCIPDrive.NotifyParentPropertyChanged(propertyNames.ToArray());

                OriginalAxisCIPDrive.CIPAxisToTagMembers(AxisTag);
            }

        }

        private void ResetEditPropertiesList()
        {
            _editPropertiesList.Clear();
        }

        private async Task DoOnlineApplyingAsync()
        {
            if (IsOnLine)
            {
                try
                {
                    Controller controller = Controller as Controller;
                    Contract.Assert(controller != null);

                    int instanceId = controller.GetTagId(AxisTag);

                    OriginalAxisCIPDrive.CIPAxis.InstanceId = instanceId;
                    OriginalAxisCIPDrive.CIPAxis.Messager = controller.CipMessager;
                    ModifiedAxisCIPDrive.CIPAxis.InstanceId = instanceId;
                    ModifiedAxisCIPDrive.CIPAxis.Messager = controller.CipMessager;

                    if (_beCyclicReadUpdateListChanged)
                    {
                        await OriginalAxisCIPDrive.CIPAxis.SetCyclicReadList(OriginalAxisCIPDrive.CyclicReadUpdateList
                            ?.ToArray());
                    }

                    if (_beCyclicWriteUpdateListChanged)
                    {
                        await OriginalAxisCIPDrive.CIPAxis.SetCyclicWriteList(OriginalAxisCIPDrive.CyclicWriteUpdateList
                            ?.ToArray());
                    }

                    var differentAttributeNameList = CipAttributeHelper.GetDifferentAttributeNameList(
                        ModifiedAxisCIPDrive.CIPAxis,
                        OriginalAxisCIPDrive.CIPAxis);

                    var attributeList
                        = differentAttributeNameList.Intersect(_comparePropertiesList).ToList();

                    foreach (var attributeName in attributeList)
                    {
                        if (ModifiedAxisCIPDrive.SupportAttribute(attributeName))
                        {
                            Logger.Trace(
                                $"Properties Set, {AxisTag.Name}.{attributeName}, value:{ModifiedAxisCIPDrive.CIPAxis.GetAttributeValueString(attributeName)}");

                            await ModifiedAxisCIPDrive.CIPAxis.SetAttributeSingle(attributeName);
                        }
                        else
                        {
                            Logger.Trace($"Not Support Attribute in apply: {AxisTag.Name}.{attributeName}");
                        }
                    }

                    ResetEditPropertiesList();

                    await Task.Delay(100);

                    foreach (var attributeName in attributeList)
                    {
                        await OriginalAxisCIPDrive.CIPAxis.GetAttributeSingle(attributeName);

                        Logger.Trace(
                            $"GetAttributeSingle, {AxisTag.Name}.{attributeName}, value:{OriginalAxisCIPDrive.CIPAxis.GetAttributeValueString(attributeName)}");
                    }

                    //
                    OriginalAxisCIPDrive.NotifyParentPropertyChanged(attributeList.ToArray());
                }
                catch (Exception e)
                {
                    Logger.Error("Online Apply failed!" + e);
                }
            }
        }

        private void DoPostApply()
        {
            if (_beTagNameChanged)
            {
                Title = $"Axis Properties - {AxisTag.Name}";
            }

            if (_beTagDescriptionChanged || _beTagNameChanged || _beAssignedGroupChanged)
            {
                //IStudioUIService studioUIService =
                //    Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
                //studioUIService?.UpdateUI();
            }

            Refresh();
        }

        private bool CheckNeedCalculateOutOfBoxTuning()
        {
            var axisConfiguration = (AxisConfigurationType)Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis.AxisConfiguration);
            if (axisConfiguration == AxisConfigurationType.FrequencyControl)
                return false;

            // for DriveModelTimeConstantBase
            // TODO(gjc): check for AxisNumber changed
            if (OriginalAxisCIPDrive.AssociatedModule != ModifiedAxisCIPDrive.AssociatedModule)
                return true;

            var differentAttributeList =
                CipAttributeHelper.GetDifferentAttributeNameList(
                    ModifiedAxisCIPDrive.CIPAxis,
                    OriginalAxisCIPDrive.CIPAxis);

            // TODO(gjc):GainTuningConfigurationBits,use load ratio
            string[] attributes =
            {
                "AxisConfiguration",
                "MotorCatalogNumber",
                "DampingFactor", "SystemDamping",
                "MotorType", "RotaryMotorPoles",
                "RotaryMotorRatedSpeed", "RotaryMotorMaxSpeed",
                "MotorRatedContinuousCurrent", "MotorRatedPeakCurrent",
                "PMMotorTorqueConstant",
                "Feedback1CycleInterpolation", "Feedback1CycleResolution",
                "LoadType",
                //"TransmissionRatioInput", "TransmissionRatioOutput",
                "ActuatorType", "ActuatorLead", "ActuatorDiameter",
                "LoadCoupling", "GainTuningConfigurationBits",
                "LoadRatio", "RotaryMotorInertia",
                "LinearMotorMass", "TotalInertia", "TotalMass",
                "LoadObserverConfiguration"
            };

            foreach (var attribute in attributes)
                if (differentAttributeList.Contains(attribute))
                    return true;

            return false;
        }

        private bool CheckNeedCalculateFeedbackUnitRatio()
        {
            var differentAttributeList =
                CipAttributeHelper.GetDifferentAttributeNameList(
                    ModifiedAxisCIPDrive.CIPAxis,
                    OriginalAxisCIPDrive.CIPAxis);

            string[] attributes =
            {
                "LoadType",
                "TransmissionRatioInput", "TransmissionRatioOutput",
                "ActuatorType",
                "ActuatorLead", "ActuatorLeadUnit",
                "ActuatorDiameter", "ActuatorDiameterUnit"
            };

            foreach (var attribute in attributes)
                if (differentAttributeList.Contains(attribute))
                    return true;

            return false;
        }

        private bool CheckNeedCalculateScalingFactor()
        {
            var differentAttributeList =
                CipAttributeHelper.GetDifferentAttributeNameList(
                    ModifiedAxisCIPDrive.CIPAxis,
                    OriginalAxisCIPDrive.CIPAxis);

            string[] attributes =
            {
                "PositionScalingNumerator", "PositionScalingDenominator",
                "TransmissionRatioInput", "TransmissionRatioOutput",
            };

            foreach (var attribute in attributes)
                if (differentAttributeList.Contains(attribute))
                    return true;

            return false;
        }

        private bool CheckNeedCalculateFrequencyControl()
        {
            var axisConfiguration = (AxisConfigurationType)Convert.ToByte(ModifiedAxisCIPDrive.CIPAxis.AxisConfiguration);
            if (axisConfiguration != AxisConfigurationType.FrequencyControl)
                return false;

            var differentAttributeList =
                CipAttributeHelper.GetDifferentAttributeNameList(
                    ModifiedAxisCIPDrive.CIPAxis,
                    OriginalAxisCIPDrive.CIPAxis);

            string[] attributes =
            {
                "AxisConfiguration",
                "MotorCatalogNumber",
                "MotorType", "RotaryMotorPoles",
                "InductionMotorRatedFrequency",
                "RotaryMotorRatedSpeed",
                "RotaryMotorMaxSpeed",
                "MotorRatedOutputPower",
                "MotorRatedVoltage",
                "MotorRatedContinuousCurrent",
            };

            foreach (var attribute in attributes)
                if (differentAttributeList.Contains(attribute))
                    return true;

            return false;
        }

        private void ExecuteClosingCommand(CancelEventArgs args)
        {
            if (!_dialogResult.HasValue)
            {
                Tag tag = AxisTag as Tag;
                if (tag != null && tag.IsDeleted)
                    return;

                if (CanExecuteApplyCommand())
                {
                    string message = "Apply changes?";
                    string caption = "ICS Studio";

                    var messageBoxResult = MessageBox.Show(message, caption, MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Exclamation);

                    if (messageBoxResult == MessageBoxResult.Cancel)
                    {
                        args.Cancel = true;
                    }
                    else if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        int result = ThreadHelper.JoinableTaskFactory.Run(async delegate
                        {
                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                            int applyResult = await DoApplyAsync();

                            return applyResult;
                        });

                        if (result != 0)
                            args.Cancel = true;
                    }
                }

            }
        }
    }
}
