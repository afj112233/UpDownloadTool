using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using ICSStudio.Cip.Objects;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public sealed partial class AxisCIPDrivePropertiesViewModel
    {
        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                
                Refresh();

                if (IsOnLine)
                    _axisUpdateTimer?.Start();
                else
                    _axisUpdateTimer?.Stop();

            });
        }

        private void OnAxisTagPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (AxisTag == sender)
            {
                if (e.PropertyName == "AssignedGroup")
                {
                    ModifiedAxisCIPDrive.AssignedGroup = OriginalAxisCIPDrive.AssignedGroup;

                    if (OriginalAxisCIPDrive.AssignedGroup != null)
                    {
                        PropertyChangedEventManager.RemoveHandler(
                            OriginalAxisCIPDrive.AssignedGroup, OnAssignedGroupPropertyChanged, string.Empty);
                        PropertyChangedEventManager.AddHandler(
                            OriginalAxisCIPDrive.AssignedGroup, OnAssignedGroupPropertyChanged, string.Empty);
                    }

                    Refresh();

                    return;
                }

                if (e.PropertyName == "AxisUpdateSchedule")
                {
                    var result = FindFirstNodeByTitle("General", OptionPanelNodes);
                    result?.OptionPanel.Show();

                    return;
                }

                if (e.PropertyName == "AssociatedModule" || e.PropertyName == "AxisNumber")
                {
                    ModifiedAxisCIPDrive.UpdateAxisChannel(OriginalAxisCIPDrive.AssociatedModule,
                        OriginalAxisCIPDrive.AxisNumber);

                    Refresh();

                    return;
                }

                //TODO(gjc):need check here
                {
                    if (_comparePropertiesList.Contains(e.PropertyName))
                    {
                        //1. get different list
                        var differentAttributeList =
                            CipAttributeHelper.GetDifferentAttributeList(
                                ModifiedAxisCIPDrive.CIPAxis,
                                OriginalAxisCIPDrive.CIPAxis, new List<string>() { e.PropertyName });

                        if (differentAttributeList.Count > 0)
                        {
                            //2. check in user edit list
                            if (_editPropertiesList.Contains(e.PropertyName))
                                return;

                            //3. update modified axis
                            ModifiedAxisCIPDrive.CIPAxis.Apply(OriginalAxisCIPDrive.CIPAxis,
                                CipAttributeHelper.AttributeNamesToIdList<CIPAxis>(new[] { e.PropertyName }));

                            //4. refresh
                            Refresh();
                        }
                    }
                }

            }
        }

        private void OnAssignedGroupPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == OriginalAxisCIPDrive.AssignedGroup)
            {
                if (e.PropertyName == "Name"
                    || e.PropertyName == "CoarseUpdatePeriod"
                    || e.PropertyName == "Alternate1UpdateMultiplier"
                    || e.PropertyName == "Alternate2UpdateMultiplier")
                {
                    var result = FindFirstNodeByTitle("General", OptionPanelNodes);
                    result?.OptionPanel.Show();
                }
            }
        }

        private void OnDeviceModulesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
                {
                    var result = FindFirstNodeByTitle("General", OptionPanelNodes);
                    result?.OptionPanel.Show();
                }
            });
        }

        private void CycleUpdateAxis(object sender, ElapsedEventArgs e)
        {
            //TODO(gjc): add code here

            if (Controller.IsOnline)
            {
                var result = FindFirstNodeByTitle("Monitor Parameters", OptionPanelNodes);
                if (result.IsActive)
                    result.OptionPanel.Show();

                result = FindFirstNodeByTitle("Status", OptionPanelNodes);
                if (result.IsActive)
                    result.OptionPanel.Show();

                TagSyncController tagSyncController
                    = (Controller as Controller)?.Lookup(typeof(TagSyncController)) as TagSyncController;
                if (tagSyncController != null)
                {
                    tagSyncController.Update(AxisTag, AxisTag.Name);

                    DefaultViewModel defaultViewModel = _activeNode.OptionPanel as DefaultViewModel;
                    if (defaultViewModel != null)
                    {
                        foreach (var property in defaultViewModel.PeriodicRefreshProperties)
                        {
                            tagSyncController.UpdateAxisProperties(AxisTag, property);
                        }
                    }
                }
            }

            // check axis state
            var newAxisState = (CIPAxisStateType)Convert.ToByte(OriginalAxisCIPDrive.CIPAxis.CIPAxisState);
            if (newAxisState != _oldAxisState)
            {
                OnAxisStateChanged(_oldAxisState, newAxisState);
                _oldAxisState = newAxisState;
            }

            // 
            if (IsPowerStructureEnabled)
            {
                CancelEdit();
            }
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private void OnAxisStateChanged(CIPAxisStateType oldAxisState, CIPAxisStateType newAxisState)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                Refresh();
            });
        }

        private void CancelEdit()
        {
            //TODO(gjc): add code here
            // cycle read

            // cycle write

            // attribute name
            var differentAttributeList = CipAttributeHelper.GetDifferentAttributeList(
                ModifiedAxisCIPDrive.CIPAxis,
                OriginalAxisCIPDrive.CIPAxis);

            if (differentAttributeList.Count > 0)
            {
                ModifiedAxisCIPDrive.CIPAxis.Apply(OriginalAxisCIPDrive.CIPAxis, differentAttributeList);
            }
        }

        private void OnTitlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Title")
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    ApplyCommand.RaiseCanExecuteChanged();
                });
            }

        }

        private void OnKeySwitchChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (Controller.IsOnline && Controller.KeySwitchPosition == ControllerKeySwitch.RunKeySwitch)
                {
                    CancelEdit();
                }

                Refresh();
            });
        }

        private void OnOperationModeChanged(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                Refresh();
            });
        }
    }
}
