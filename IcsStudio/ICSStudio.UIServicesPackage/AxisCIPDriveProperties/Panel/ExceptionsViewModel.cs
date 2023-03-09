using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Cip.Objects;
using ICSStudio.Gui.Utils;
using ICSStudio.SimpleServices.DeviceModule;
using ICSStudio.Utils;

namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Panel
{
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    class ExceptionsViewModel : DefaultViewModel
    {
        public ExceptionsViewModel(UserControl panel, AxisCIPDrivePropertiesViewModel parentViewModel)
            : base(panel, parentViewModel)
        {
            // CIPAxisExceptionActionRA->CIPAxisExceptionActionMfg
            CompareProperties = new[]
            {
                "CIPAxisExceptionAction",
                "MotionExceptionAction",
                //"CIPAxisExceptionActionRA"
                "CIPAxisExceptionActionMfg"
            };

            ParametersCommand = new RelayCommand(ExecuteParametersCommand);

            UpdateExceptionActionSource();
        }

        private bool IsExceptionsEnabled
        {
            get
            {
                //TODO(gjc): edit later
                if (ParentViewModel.IsOnLine)
                    return false;

                //if (ParentViewModel.IsPowerStructureEnabled)
                //    return false;

                return true;
            }
        }

        public override void Show()
        {
            UpdateExceptionActionSource();
        }

        private ObservableCollection<ExceptionActionItem> _exceptionActionSource;

        public ObservableCollection<ExceptionActionItem> ExceptionActionSource
        {
            get { return _exceptionActionSource; }
            set { Set(ref _exceptionActionSource, value); }
        }

        public RelayCommand ParametersCommand { get; }

        #region Private

        private void UpdateExceptionActionSource()
        {
            var motionDrive = ParentViewModel.ModifiedAxisCIPDrive.AssociatedModule as CIPMotionDrive;
            List<ExceptionActionItem> exceptionActionSource;

            var axisConfiguration =
                (AxisConfigurationType)Convert.ToByte(ModifiedCIPAxis.AxisConfiguration);

            if (motionDrive != null)
            {
                exceptionActionSource =
                    GetExceptionActionSource(axisConfiguration, motionDrive);
            }
            else
            {
                exceptionActionSource = new List<ExceptionActionItem>();

                AddMotionException(axisConfiguration, exceptionActionSource);
            }

            SetExceptionActionSelectValue(exceptionActionSource);

            ExceptionActionSource = new ObservableCollection<ExceptionActionItem>(exceptionActionSource);
        }

        private List<ExceptionActionItem> GetExceptionActionSource(
            AxisConfigurationType axisConfiguration, CIPMotionDrive motionDrive)
        {
            var exceptionActionSource = new List<ExceptionActionItem>();

            AddCIPStandardException(axisConfiguration, exceptionActionSource, motionDrive);

            AddCIPAxisExceptionMfg(axisConfiguration, exceptionActionSource, motionDrive);

            // TODO(gjc): add code here
            // icon axis exception

            // Motion Exception,rm003 
            AddMotionException(axisConfiguration, exceptionActionSource);

            // sort
            exceptionActionSource.Sort((x, y) => string.Compare(x.Exception, y.Exception, StringComparison.Ordinal));

            return exceptionActionSource;
        }

        private void AddCIPAxisExceptionMfg(
            AxisConfigurationType axisConfiguration,
            List<ExceptionActionItem> exceptionActionSource,
            CIPMotionDrive motionDrive)
        {
            var supportedExceptions = motionDrive.Profiles.Schema.Attributes.SupportedExceptions;
            var defaultSelectedAction = ExceptionActionType.Disable;
            var defaultActionSource = new List<ExceptionActionType>();

            if (axisConfiguration == AxisConfigurationType.FeedbackOnly)
            {
                supportedExceptions = motionDrive.Profiles.Schema.Attributes.FeedbackOnlySupportedExceptions;
                defaultSelectedAction = ExceptionActionType.FaultStatusOnly;

                defaultActionSource.Add(ExceptionActionType.FaultStatusOnly);
                defaultActionSource.Add(ExceptionActionType.Shutdown);
            }
            else
            {
                defaultActionSource.Add(ExceptionActionType.Disable);
                defaultActionSource.Add(ExceptionActionType.Shutdown);
            }

            // CIP Standard Exception RA
            var cipAxisExceptionAction = supportedExceptions.CIPAxisExceptionActionRA;
            if (cipAxisExceptionAction != null)
                foreach (var exceptionAction in cipAxisExceptionAction)
                {
                    if (exceptionAction.MinMajorRev > motionDrive.Major)
                        continue;

                    var cipStandardException =
                        EnumUtils.Parse<CIPStandardExceptionRA>(exceptionAction.Exception);

                    var item = new ExceptionActionItem(IsExceptionsEnabled)
                    {
                        ExceptionType = ExceptionType.CIPAxisExceptionMfg,
                        Index = (int)cipStandardException,
                        Exception = EnumHelper.GetEnumMember(cipStandardException),
                        SelectedAction = defaultSelectedAction
                    };

                    item.SelectedActionChanged += OnExceptionActionItemChanged;

                    if (exceptionAction.Action != null)
                    {
                        var actionSource = new List<ExceptionActionType>(defaultActionSource);

                        foreach (var action in exceptionAction.Action)
                        {
                            if (action.MinMajorRev > motionDrive.Major)
                                continue;

                            var exceptionActionTypes = EnumUtils.Parse<ExceptionActionType>(action.Value);

                            if (!actionSource.Contains(exceptionActionTypes))
                                actionSource.Add(exceptionActionTypes);
                        }

                        item.ActionSource = EnumHelper.ToDataSource<ExceptionActionType>(actionSource);
                        item.SupportActionSource = new List<ExceptionActionType>(actionSource);
                    }
                    else
                    {
                        item.ActionSource = EnumHelper.ToDataSource<ExceptionActionType>(defaultActionSource);
                        item.SupportActionSource = new List<ExceptionActionType>(defaultActionSource);
                    }

                    exceptionActionSource.Add(item);
                }
        }

        private void AddCIPStandardException(
            AxisConfigurationType axisConfiguration,
            List<ExceptionActionItem> exceptionActionSource,
            CIPMotionDrive motionDrive)
        {
            var supportedExceptions = motionDrive.Profiles.Schema.Attributes.SupportedExceptions;
            var defaultSelectedAction = ExceptionActionType.Disable;
            var defaultActionSource = new List<ExceptionActionType>();

            if (axisConfiguration == AxisConfigurationType.FeedbackOnly)
            {
                supportedExceptions = motionDrive.Profiles.Schema.Attributes.FeedbackOnlySupportedExceptions;
                defaultSelectedAction = ExceptionActionType.FaultStatusOnly;

                defaultActionSource.Add(ExceptionActionType.FaultStatusOnly);
                defaultActionSource.Add(ExceptionActionType.Shutdown);
            }
            else
            {
                defaultActionSource.Add(ExceptionActionType.Disable);
                defaultActionSource.Add(ExceptionActionType.Shutdown);
            }

            // CIP Standard Exception
            var cipAxisExceptionAction = supportedExceptions.CIPAxisExceptionAction;
            if (cipAxisExceptionAction != null)
                foreach (var exceptionAction in cipAxisExceptionAction)
                {
                    if (exceptionAction.MinMajorRev > motionDrive.Major)
                        continue;

                    var cipStandardException =
                        EnumUtils.Parse<CIPStandardException>(exceptionAction.Exception);

                    var item = new ExceptionActionItem(IsExceptionsEnabled)
                    {
                        ExceptionType = ExceptionType.CIPAxisException,
                        Index = (int)cipStandardException,
                        Exception = EnumHelper.GetEnumMember(cipStandardException),
                        SelectedAction = defaultSelectedAction
                    };

                    item.SelectedActionChanged += OnExceptionActionItemChanged;

                    if (exceptionAction.Action != null)
                    {
                        var actionSource = new List<ExceptionActionType>(defaultActionSource);

                        foreach (var action in exceptionAction.Action)
                        {
                            if (action.MinMajorRev > motionDrive.Major)
                                continue;

                            var exceptionActionTypes = EnumUtils.Parse<ExceptionActionType>(action.Value);

                            if (!actionSource.Contains(exceptionActionTypes))
                                actionSource.Add(exceptionActionTypes);
                        }

                        item.ActionSource = EnumHelper.ToDataSource<ExceptionActionType>(actionSource);
                        item.SupportActionSource = new List<ExceptionActionType>(actionSource);
                    }
                    else
                    {
                        item.ActionSource = EnumHelper.ToDataSource<ExceptionActionType>(defaultActionSource);
                        item.SupportActionSource = new List<ExceptionActionType>(defaultActionSource);
                    }

                    exceptionActionSource.Add(item);
                }
        }

        private void AddMotionException(AxisConfigurationType axisConfiguration,
            List<ExceptionActionItem> exceptionActionSource)
        {
            var defaultSelectedAction = ExceptionActionType.Disable;
            var defaultActionSource = new List<ExceptionActionType>();


            if (axisConfiguration == AxisConfigurationType.FeedbackOnly)
            {
                defaultSelectedAction = ExceptionActionType.FaultStatusOnly;

                defaultActionSource.Add(ExceptionActionType.Ignore);
                defaultActionSource.Add(ExceptionActionType.Alarm);

                defaultActionSource.Add(ExceptionActionType.FaultStatusOnly);
                defaultActionSource.Add(ExceptionActionType.Shutdown);
            }
            else
            {
                defaultActionSource.Add(ExceptionActionType.Ignore);
                defaultActionSource.Add(ExceptionActionType.Alarm);
                defaultActionSource.Add(ExceptionActionType.FaultStatusOnly);
                defaultActionSource.Add(ExceptionActionType.StopPlanner);

                defaultActionSource.Add(ExceptionActionType.Disable);
                defaultActionSource.Add(ExceptionActionType.Shutdown);
            }

            var item = new ExceptionActionItem(IsExceptionsEnabled)
            {
                ExceptionType = ExceptionType.MotionException,
                Index = (int)MotionException.SoftTravelLimitPositive,
                Exception = EnumHelper.GetEnumMember(MotionException.SoftTravelLimitPositive),
                SelectedAction = defaultSelectedAction,
                ActionSource = EnumHelper.ToDataSource<ExceptionActionType>(defaultActionSource),
                SupportActionSource = new List<ExceptionActionType>(defaultActionSource)
            };

            item.SelectedActionChanged += OnExceptionActionItemChanged;
            exceptionActionSource.Add(item);

            //
            item = new ExceptionActionItem(IsExceptionsEnabled)
            {
                ExceptionType = ExceptionType.MotionException,
                Index = (int)MotionException.SoftTravelLimitNegative,
                Exception = EnumHelper.GetEnumMember(MotionException.SoftTravelLimitNegative),
                SelectedAction = defaultSelectedAction,
                ActionSource = EnumHelper.ToDataSource<ExceptionActionType>(defaultActionSource),
                SupportActionSource = new List<ExceptionActionType>(defaultActionSource)
            };
            item.SelectedActionChanged += OnExceptionActionItemChanged;
            exceptionActionSource.Add(item);
        }

        private void SetExceptionActionSelectValue(List<ExceptionActionItem> exceptionActionSource)
        {
            foreach (var exceptionAction in exceptionActionSource)
            {
                if (exceptionAction.ExceptionType == ExceptionType.CIPAxisException)
                {
                    var actionValue = ModifiedCIPAxis.CIPAxisExceptionAction.GetValue(exceptionAction.Index);
                    if (actionValue == 255)
                    {
                        Debug.WriteLine($"{exceptionAction.Exception} is not set!");
                        ModifiedCIPAxis.CIPAxisExceptionAction.SetValue(exceptionAction.Index,
                            (byte)exceptionAction.SelectedAction);
                    }
                    else
                    {
                        if (exceptionAction.SupportActionSource.Contains((ExceptionActionType)actionValue))
                            exceptionAction.SelectedAction = (ExceptionActionType)actionValue;
                        else
                        {
                            ModifiedCIPAxis.CIPAxisExceptionAction.SetValue(exceptionAction.Index,
                                (byte)exceptionAction.SelectedAction);
                        }
                    }
                }

                if (exceptionAction.ExceptionType == ExceptionType.CIPAxisExceptionMfg)
                {
                    var actionValue = ModifiedCIPAxis.CIPAxisExceptionActionMfg.GetValue(exceptionAction.Index);
                    if (actionValue == 255)
                    {
                        Debug.WriteLine($"{exceptionAction.Exception} is not set!");
                        ModifiedCIPAxis.CIPAxisExceptionActionMfg.SetValue(exceptionAction.Index,
                            (byte)exceptionAction.SelectedAction);
                    }
                    else
                    {
                        if (exceptionAction.SupportActionSource.Contains((ExceptionActionType)actionValue))
                            exceptionAction.SelectedAction = (ExceptionActionType)actionValue;
                        else
                        {
                            ModifiedCIPAxis.CIPAxisExceptionActionMfg.SetValue(exceptionAction.Index,
                                (byte)exceptionAction.SelectedAction);
                        }
                    }
                }

                if (exceptionAction.ExceptionType == ExceptionType.MotionException)
                {
                    var actionValue = ModifiedCIPAxis.MotionExceptionAction.GetValue(exceptionAction.Index);
                    if (actionValue == 255)
                    {
                        Debug.WriteLine($"{exceptionAction.Exception} is not set!");
                        ModifiedCIPAxis.MotionExceptionAction.SetValue(exceptionAction.Index,
                            (byte)exceptionAction.SelectedAction);
                    }
                    else
                    {
                        if (exceptionAction.SupportActionSource.Contains((ExceptionActionType)actionValue))
                            exceptionAction.SelectedAction = (ExceptionActionType)actionValue;
                        else
                        {
                            ModifiedCIPAxis.MotionExceptionAction.SetValue(exceptionAction.Index,
                                (byte)exceptionAction.SelectedAction);
                        }
                    }
                }
            }

            // set Unsupported 
            List<int> cipAxisExceptionSupportedList = new List<int>();
            foreach (var exceptionAction in exceptionActionSource)
            {
                if (exceptionAction.ExceptionType == ExceptionType.CIPAxisException)
                {
                    cipAxisExceptionSupportedList.Add(exceptionAction.Index);
                }
            }

            for (int i = 0; i < 64; i++)
            {
                if (!cipAxisExceptionSupportedList.Contains(i))
                    ModifiedCIPAxis.CIPAxisExceptionAction.SetValue(i, 255);
            }

        }

        private void OnExceptionActionItemChanged(object sender, EventArgs e)
        {
            ExceptionActionItem item = (ExceptionActionItem)sender;
            if (ExceptionActionSource == null)
                return;

            if (ExceptionActionSource.Contains(item))
            {
                if (item.ExceptionType == ExceptionType.CIPAxisException)
                {
                    var cipActionValue = ModifiedCIPAxis.CIPAxisExceptionAction.GetValue(item.Index);
                    var selectActionValue = (byte)item.SelectedAction;

                    if (cipActionValue != selectActionValue)
                    {
                        ModifiedCIPAxis.CIPAxisExceptionAction.SetValue(item.Index, selectActionValue);

                        CheckDirty();
                    }
                }

                if (item.ExceptionType == ExceptionType.CIPAxisExceptionMfg)
                {
                    var cipActionValue = ModifiedCIPAxis.CIPAxisExceptionActionMfg.GetValue(item.Index);
                    var selectActionValue = (byte)item.SelectedAction;

                    if (cipActionValue != selectActionValue)
                    {
                        ModifiedCIPAxis.CIPAxisExceptionActionMfg.SetValue(item.Index, selectActionValue);

                        CheckDirty();
                    }
                }


                if (item.ExceptionType == ExceptionType.MotionException)
                {
                    var actionValue = ModifiedCIPAxis.MotionExceptionAction.GetValue(item.Index);
                    var selectActionValue = (byte)item.SelectedAction;
                    if (actionValue != selectActionValue)
                    {
                        ModifiedCIPAxis.MotionExceptionAction.SetValue(item.Index, selectActionValue);

                        CheckDirty();
                    }
                }
            }

        }

        private void ExecuteParametersCommand()
        {
            ParentViewModel.ShowPanel("Parameter List", "Exceptions");
        }

        #endregion
    }

    public enum ExceptionType
    {
        MotionException,
        CIPAxisException,
        CIPAxisExceptionMfg,
        CIPAxisExceptionRA
    }

    public class ExceptionActionItem : ObservableObject
    {
        private ExceptionActionType _selectedAction;

        public ExceptionActionItem(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }

        public ExceptionType ExceptionType { get; set; }
        public int Index { get; set; }

        public string Exception { get; set; }

        public ExceptionActionType SelectedAction
        {
            get { return _selectedAction; }
            set
            {
                if (_selectedAction != value)
                {
                    Set(ref _selectedAction, value);

                    SelectedActionChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public IList ActionSource { get; set; }

        // for contains check
        public List<ExceptionActionType> SupportActionSource { get; set; }

        public event EventHandler SelectedActionChanged;

        public bool IsEnabled { get; }
    }
}
