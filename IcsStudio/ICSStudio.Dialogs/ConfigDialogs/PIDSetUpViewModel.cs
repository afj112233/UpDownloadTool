using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Converters;
using ICSStudio.Interfaces.Tags;
using ICSStudio.SimpleServices.Common;
using ICSStudio.SimpleServices.Tags;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.DataType;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    public class PidSetUpViewModel : ViewModelBase
    {
        private string _name;
        private string _description;
        private float _sp;
        private float _so;
        private float _bias;
        private float _kp;
        private float _ki;
        private float _kd;
        private bool _mo;
        private bool _swm;
        private bool _ndf;
        private bool _nobc;
        private bool _nozc;
        private bool _pvt;

        private PEType _pe;
        private CAType _ca;
        private DOEType _doe;
        private float _upd;
        private float _maxo;
        private float _mino;
        private float _db;

        private float _pvh;
        private float _pvl;
        private float _pvdb;
        private float _dvp;
        private float _dvn;
        private float _dvdb;
        private bool _cl;
        private CTType _ct;

        private float _maxi;
        private float _mini;
        private float _maxs;
        private float _mins;
        private float _maxcv;
        private float _mincv;
        private float _maxtie;
        private float _mintie;
        private bool _ini;

        private float _originalKP;
        private float _originalKI;
        private float _originalKD;
        private ITag _tag;

        public PidSetUpViewModel(ArrayField field, ITag tag)
        {
            Title = "PID Setup - " + tag.Name;
            _tag = tag;
            _name = _tag.Name;
            _description = _tag.Description;
            //TODO(TLM):Usage是否可见
            Usage = _tag.Usage.ToString();
            Type = ((_tag as Tag).DataWrapper.ParentTag as Tag).TagType.ToString();
            DataType = _tag.DataTypeInfo.ToString();
            Scope = ((_tag as Tag).DataWrapper.ParentTag as Tag).ParentController.Name;
            ExternalAccess = _tag.ExternalAccess.ToString();

            _sp = Convert.ToSingle(_tag.GetMemberValue("SP", true));
            _so = Convert.ToSingle(_tag.GetMemberValue("SO", true));
            _bias = Convert.ToSingle(_tag.GetMemberValue("BIAS", true));
            _kp = Convert.ToSingle(_tag.GetMemberValue("KP", true));
            _ki = Convert.ToSingle(_tag.GetMemberValue("KI", true));
            _kd = Convert.ToSingle(_tag.GetMemberValue("KD", true));
            _originalKP = Convert.ToSingle(_tag.GetMemberValue("KP", true));
            _originalKI = Convert.ToSingle(_tag.GetMemberValue("KI", true));
            _originalKD = Convert.ToSingle(_tag.GetMemberValue("KD", true));
            _mo = _tag.GetMemberValue("MO", true) == "true";
            _swm = _tag.GetMemberValue("SWM", true) == "true";
            _pe = _tag.GetMemberValue("PE", true) == "true"
                ? PEType.Dependent
                : PEType.Independent;
            _ca = _tag.GetMemberValue("CA", true) == "true"
                ? CAType.PV
                : CAType.SP;
            _doe = _tag.GetMemberValue("DOE", true) == "true"
                ? DOEType.Error
                : DOEType.PV;
            _upd = Convert.ToSingle(_tag.GetMemberValue("UPD", true));
            _maxo = Convert.ToSingle(_tag.GetMemberValue("MAXO", true));
            _mino = Convert.ToSingle(_tag.GetMemberValue("MINO", true));
            _db = Convert.ToSingle(_tag.GetMemberValue("DB", true));
            _ndf = _tag.GetMemberValue("NDF", true) == "true";
            _nobc = _tag.GetMemberValue("NOBC", true) == "true";
            _nozc = _tag.GetMemberValue("NOZC", true) == "true";
            _pvt = _tag.GetMemberValue("PVT", true) == "true";
            _cl = _tag.GetMemberValue("CL", true) == "true";

            _ct = _tag.GetMemberValue("CT", true) == "true"
                ? CTType.Master
                : CTType.Slave;

            _pvh = Convert.ToSingle(_tag.GetMemberValue("PVH", true));
            _pvl = Convert.ToSingle(_tag.GetMemberValue("PVL", true));
            _pvdb = Convert.ToSingle(_tag.GetMemberValue("PVDB", true));
            _dvp = Convert.ToSingle(_tag.GetMemberValue("DVP", true));
            _dvn = Convert.ToSingle(_tag.GetMemberValue("DVN", true));
            _dvdb = Convert.ToSingle(_tag.GetMemberValue("DVDB", true));

            _maxi = Convert.ToSingle(_tag.GetMemberValue("MAXI", true));
            _mini = Convert.ToSingle(_tag.GetMemberValue("MINI", true));
            _maxs = Convert.ToSingle(_tag.GetMemberValue("MAXS", true));
            _mins = Convert.ToSingle(_tag.GetMemberValue("MINS", true));
            _maxcv = Convert.ToSingle(_tag.GetMemberValue("MAXCV", true));
            _mincv = Convert.ToSingle(_tag.GetMemberValue("MINCV", true));
            _maxtie = Convert.ToSingle(_tag.GetMemberValue("MAXTIE", true));
            _mintie = Convert.ToSingle(_tag.GetMemberValue("MINTIE", true));
            _ini = _tag.GetMemberValue("INI", true) == "true";

            ApplyCommand = new RelayCommand(ExecuteApplyCommand, CanExecute);
            OKCommand = new RelayCommand(ExecuteOKCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
            Reset = new RelayCommand(ExcuteResetCommand);

            PEItemsSource = new List<PEType> {PEType.Dependent, PEType.Independent};
            CAItemsSource = new List<CAType> {CAType.PV, CAType.SP};
            DOEItemsSource = new List<DOEType> {DOEType.Error, DOEType.PV};
            CTItemsSource = new List<CTType> {CTType.Master, CTType.Slave};

            _isTuningDirty = false;
            _isConfigurationDirty = false;
            _isAlarmsDirty = false;
            _isScalingDirty = false;
            _isTagDirty = false;

            Tuning = LanguageManager.GetInstance().ConvertSpecifier("Tuning");
            Configuration = LanguageManager.GetInstance().ConvertSpecifier("Configuration");
            Alarms = LanguageManager.GetInstance().ConvertSpecifier("Alarms");
            Scaling = LanguageManager.GetInstance().ConvertSpecifier("Scaling");
            Tag = LanguageManager.GetInstance().ConvertSpecifier("Variable");
        }
        

        public string Title { get; set; }

        public string Tuning { get; set; }
        public string Configuration { get; set; }
        public string Alarms { get; set; }
        public string Scaling { get; set; }
        public string Tag { get; set; }

        public bool _isTuningDirty;
        public bool _isConfigurationDirty;
        public bool _isAlarmsDirty;
        public bool _isScalingDirty;
        public bool _isTagDirty;

        public RelayCommand Reset { get; set; }

        private void ExcuteResetCommand()
        {
            bool _originalTuningDirty = _isTuningDirty;
            
            KP = _originalKP;
            KI = _originalKI;
            KD = _originalKD;
            RaisePropertyChanged("KP");
            RaisePropertyChanged("KI");
            RaisePropertyChanged("KD");

            _isTuningDirty = _originalTuningDirty;
            ApplyCommand.RaiseCanExecuteChanged();
        }

        public float SP
        {
            get { return _sp; }
            set
            {
                if (_sp != value)
                {
                    if (value < MINS || value > MAXS)
                    {
                        MessageBox.Show(
                            "Failed to set Setpoint value to: " + value + "\n" +
                            "Immediate value out of range.\n\n" +
                            "Value must be within scaled PV Min. and scaled PV Max.\n" +
                            "Scaled PV Min. and scaled PV Max. are currently set to " + MINS + " and " + MAXS + ".",
                            "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                    else
                    {
                        _sp = value;
                        _isTuningDirty = true;
                        UpdateViewModelTag("SP", _sp.ToString());
                    }
                }
            }
        }

        public float SO
        {
            get { return _so; }
            set
            {
                if (_so != value)
                {
                    if (value < MINO || value > MAXO)
                    {
                        MessageBox.Show(
                            "Failed to set Output value to: " + value + " %\n" +
                            "Immediate value out of range.\n\n" +
                            "Value must be within CV High Limit and CV Low Limit.\n" +
                            "CV High Limit and CV Low Limit are currently set to " + MINO + " and " +
                            MAXO + ".",
                            "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                    else
                    {
                        _so = value;
                        _isTuningDirty = true;
                        UpdateViewModelTag("SO", _so.ToString());
                    }
                }
            }
        }

        public bool SPIsReadOnly
        {
            get { return MAXS == 0 && MINS == 0; }
        }

        public bool SOIsReadOnly
        {
            get { return _tag.ParentCollection.ParentProgram != null || MAXO == MINO; }
        }

        public bool TabControlIsEnabled
        {
            get { return !(_tag.ParentController.IsOnline && _tag.ParentCollection.ParentProgram is AoiDefinition); }
        }

        public float BIAS
        {
            get { return _bias; }
            set
            {
                if (_bias != value)
                {
                    if (value > 100 || value < 0)
                    {
                        MessageBox.Show(
                            "Failed to set Output Bias value.\n" +
                            "Immediate value out of range.\n" +
                            "Value must be within CV High Limit and CV Low Limit.\n" +
                            "Value must be with 0% and 100%",
                            "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                    }
                    else
                    {
                        _bias = value;
                        _isTuningDirty = true;
                        UpdateViewModelTag("BIAS", _bias.ToString());
                    }
                }
            }
        }

        public float KP
        {
            get { return _kp; }
            set
            {
                if (_kp != value)
                {
                    _kp = value;
                    _isTuningDirty = true;
                    UpdateViewModelTag("KP", value.ToString());
                }
            }
        }

        public float KI
        {
            get { return _ki; }
            set
            {
                if (_ki != value)
                {
                    _ki = value;
                    _isTuningDirty = true;
                    UpdateViewModelTag("KI", value.ToString());
                }
            }
        }

        public float KD
        {
            get { return _kd; }
            set
            {
                if (_kd != value)
                {
                    _kd = value;
                    _isTuningDirty = true;
                    UpdateViewModelTag("KD", value.ToString());
                }
            }
        }

        public bool MO
        {
            get { return _mo; }
            set
            {
                if (_mo != value)
                {
                    _mo = value;
                    _isTuningDirty = true;
                    UpdateViewModelTag("MO", value ? "true" : "false");
                    RaisePropertyChanged("Mode");
                }
            }
        }

        public string Mode
        {
            get
            {
                if (MO)
                {
                    return "Manual";
                }

                return SWM ? "Software Manual" : "Auto";
            }
        }

        public bool SWM
        {
            get { return _swm; }
            set
            {
                if (_swm != value)
                {
                    _swm = value;
                    _isTuningDirty = true;
                    UpdateViewModelTag("SWM", value ? "true" : "false");
                    RaisePropertyChanged("Mode");
                }
            }
        }

        public PEType PE
        {
            get { return _pe; }
            set
            {
                if (_pe != value)
                {
                    _pe = value;
                    _isConfigurationDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();

                }
            }
        }

        public CAType CA
        {
            get { return _ca; }
            set
            {
                if (_ca != value)
                {
                    _ca = value;
                    _isConfigurationDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();

                }
            }
        }

        public DOEType DOE
        {
            get { return _doe; }
            set
            {
                if (_doe != value)
                {
                    _doe = value;
                    _isConfigurationDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();

                }
            }
        }

        public float UPD
        {
            get { return _upd; }
            set
            {
                if (_upd != value)
                {
                    _upd = value;
                    _isConfigurationDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float MAXO
        {
            get { return _maxo; }
            set
            {
                if (_maxo != value)
                {
                    _maxo = value;
                    _isConfigurationDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float MINO
        {
            get { return _mino; }
            set
            {
                if (_mino != value)
                {
                    _mino = value;
                    _isConfigurationDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float DB
        {
            get { return _db; }
            set
            {
                if (_db != value)
                {
                    _db = value;
                    _isConfigurationDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool NDF
        {
            get { return _ndf; }
            set
            {
                if (_ndf != value)
                {
                    _ndf = value;
                    _isConfigurationDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool NOBC
        {
            get { return _nobc; }
            set
            {
                if (_nobc != value)
                {
                    _nobc = value;
                    _isConfigurationDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool NOZC
        {
            get { return _nozc; }
            set
            {
                if (_nozc != value)
                {
                    _nozc = value;
                    _isConfigurationDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool PVT
        {
            get { return _pvt; }
            set
            {
                if (_pvt != value)
                {
                    _pvt = value;
                    _isConfigurationDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool CL
        {
            get { return _cl; }
            set
            {
                if (_cl != value)
                {
                    _cl = value;
                    _isConfigurationDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                    RaisePropertyChanged("CTIsEnabled");
                }
            }
        }

        public CTType CT
        {
            get { return _ct; }
            set
            {
                if (_ct != value)
                {
                    _ct = value;
                    _isConfigurationDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();

                }
            }
        }

        public bool CTIsEnabled
        {
            get { return CL; }
        }

        public float PVH
        {
            get { return _pvh; }
            set
            {
                if (_pvh != value)
                {
                    _pvh = value;
                    _isAlarmsDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float PVL
        {
            get { return _pvl; }
            set
            {
                if (_pvl != value)
                {
                    _pvl = value;
                    _isAlarmsDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float PVDB
        {
            get { return _pvdb; }
            set
            {
                if (_pvdb != value)
                {
                    _pvdb = value;
                    _isAlarmsDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float DVP
        {
            get { return _dvp; }
            set
            {
                if (_dvp != value)
                {
                    _dvp = value;
                    _isAlarmsDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float DVN
        {
            get { return _dvn; }
            set
            {
                if (_dvn != value)
                {
                    _dvn = value;
                    _isAlarmsDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float DVDB
        {
            get { return _dvdb; }
            set
            {
                if (_dvdb != value)
                {
                    _dvdb = value;
                    _isAlarmsDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float MAXI
        {
            get { return _maxi; }
            set
            {
                if (_maxi != value)
                {
                    _maxi = value;
                    _isScalingDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float MINI
        {
            get { return _mini; }
            set
            {
                if (_mini != value)
                {
                    _mini = value;
                    _isScalingDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float MAXS
        {
            get { return _maxs; }
            set
            {
                if (_maxs != value)
                {
                    _maxs = value;
                    _isScalingDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float MINS
        {
            get { return _mins; }
            set
            {
                if (_mins != value)
                {
                    _mins = value;
                    _isScalingDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float MAXCV
        {
            get { return _maxcv; }
            set
            {
                if (_maxcv != value)
                {
                    _maxcv = value;
                    _isScalingDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float MINCV
        {
            get { return _mincv; }
            set
            {
                if (_mincv != value)
                {
                    _mincv = value;
                    _isScalingDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float MAXTIE
        {
            get { return _maxtie; }
            set
            {
                if (_maxtie != value)
                {
                    _maxtie = value;
                    _isScalingDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public float MINTIE
        {
            get { return (_mintie); }
            set
            {
                if (_mintie != value)
                {
                    _mintie = value;
                    _isScalingDirty = true;
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool INI
        {
            get { return _ini; }
            set
            {
                if (_ini != value)
                {
                    _ini = value;
                    RaisePropertyChanged("PIDInitialized");
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string PIDInitialized
        {
            get { return INI ? "Yes" : "No"; }
        }

        public float PV
        {
            get { return Convert.ToSingle(_tag.GetMemberValue("PV", true)); }
        }

        public float Error
        {
            //If the control action is direct,PV-SP,相反则SP-PV
            get
            {
                return Convert.ToSingle(_tag.GetMemberValue("PV", true)) -
                       Convert.ToSingle(_tag.GetMemberValue("SP", true));
            }
        }

        public float Output
        {
            get { return Convert.ToSingle(_tag.GetMemberValue("OUT", true)); }
        }

        public float Tieback
        {
            get { return Convert.ToSingle(_tag.GetMemberValue("TIE", true)); }
        }

        public string Deviation
        {
            get
            {
                if (_tag.GetMemberValue("DVNA", true) == "True")
                {
                    return "Low";
                }

                return _tag.GetMemberValue("DVPA", true) == "True" ? "High" : "None";
            }
        }

        public string PVText
        {
            get
            {
                if (_tag.GetMemberValue("PVLA", true) == "True")
                {
                    return "Low";
                }

                return _tag.GetMemberValue("PVHA", true) == "True" ? "High" : "None";
            }
        }

        public string OutputLimiting
        {
            get
            {
                int CTL = Convert.ToInt32(_tag.GetMemberValue("CTL", true));

                if (((1 << 12) & CTL) != 0)
                {
                    return "LOW";
                }

                return ((1 << 13) & CTL) != 0 ? "High" : "None";
            }
        }

        public string ErrorWithinDeadband
        {
            get
            {
                int CTL = Convert.ToInt32(_tag.GetMemberValue("CTL", true));
                return ((1 << 11) & CTL) != 0 ? "Yes" : "No";
            }
        }

        public string SetpointOutOfRange
        {
            get
            {
                int CTL = Convert.ToInt32(_tag.GetMemberValue("CTL", true));
                return ((1 << 14) & CTL) != 0 ? "Yes" : "No";
            }
        }


        public List<PEType> PEItemsSource { get; set; }

        public List<CAType> CAItemsSource { get; set; }
        public List<DOEType> DOEItemsSource { get; set; }

        public List<CTType> CTItemsSource { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    _isTagDirty = true;

                    //TODO(TLM):除了下划线外的标点符号不可输入，命名格式需要判断
                    Tag = LanguageManager.GetInstance().ConvertSpecifier("Variable*");
                    RaisePropertyChanged("Tag");
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    _isTagDirty = true;
                    Tag = LanguageManager.GetInstance().ConvertSpecifier("Variable")+"*";
                    RaisePropertyChanged("Tag");
                    ApplyCommand.RaiseCanExecuteChanged();
                }
            }
        }

        //TODO(TLM):在controller下是没有这个Usage的 待去掉
        public string Usage { get; set; }

        public string Type { get; set; }

        public string DataType { get; set; }
        public string Scope { get; set; }
        public string ExternalAccess { get; set; }

        public RelayCommand OKCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand ApplyCommand { get; }

        private void ExecuteOKCommand()
        {
            if (DoApply())
            {
                PidDialogResult = true;
                RaisePropertyChanged("PidDialogResult");
            }
        }

        private void ExecuteCancelCommand()
        {
            PidDialogResult = true;
            RaisePropertyChanged("PidDialogResult");
        }

        public bool? PidDialogResult { get; set; }

        private bool CanExecute()
        {
            return _isTuningDirty || _isConfigurationDirty || _isAlarmsDirty || _isScalingDirty || _isTagDirty;
        }

        private void ExecuteApplyCommand()
        {
            DoApply();
            ApplyCommand.RaiseCanExecuteChanged();
        }

        public string TiUnit
        {
            get { return PE == PEType.Dependent ? "min/repeat" : "1/s"; }
        }

        public string TdUnit
        {
            get { return PE == PEType.Dependent ? "min" : "s"; }
        }

        public string KPTextBlock
        {
            get { return PE == PEType.Dependent ? LanguageManager.GetInstance().ConvertSpecifier("Proportional Gain(Kc):") : LanguageManager.GetInstance().ConvertSpecifier("Proportional Gain(Kp):"); }
        }

        public string KITestBlock
        {
            get { return PE == PEType.Dependent ? LanguageManager.GetInstance().ConvertSpecifier("Reset Time(Ti):") : LanguageManager.GetInstance().ConvertSpecifier("Integral Gain(Ki):"); }
        }

        public string KDTextBlock
        {
            get { return PE == PEType.Dependent ? LanguageManager.GetInstance().ConvertSpecifier("Derivative Rate(Td)") : LanguageManager.GetInstance().ConvertSpecifier("Derivative Time(Kd):"); }
        }

        public void UpdateViewModelTag(string name, string value)
        {
            _tag.SetStringValue(name, value);

            if (_isTuningDirty)
            {
                Tuning = LanguageManager.GetInstance().ConvertSpecifier("Tuning")+"*";
                RaisePropertyChanged(nameof(Tuning));
            }

            if (_isConfigurationDirty)
            {
                Configuration = LanguageManager.GetInstance().ConvertSpecifier("Configuration")+"*";
                RaisePropertyChanged(nameof(Configuration));
            }

            if (_isAlarmsDirty)
            {
                Alarms = LanguageManager.GetInstance().ConvertSpecifier("Alarms")+"*";
                RaisePropertyChanged(nameof(Alarms));
            }

            if (_isScalingDirty)
            {
                Scaling = LanguageManager.GetInstance().ConvertSpecifier("Scaling")+"*";
                RaisePropertyChanged(nameof(Scaling));
            }

            ApplyCommand.RaiseCanExecuteChanged();
        }

        private bool DoApply()
        {
            if (_isTuningDirty)
            {
                if (CheckTuningData())
                {
                    RaisePropertyChanged("KDTest");
                    RaisePropertyChanged("TiUnit");
                    RaisePropertyChanged("TdUnit");
                    RaisePropertyChanged("KPTextBlock");
                    RaisePropertyChanged("KITextBlock");
                    RaisePropertyChanged("KDTextBlock");
                    Tuning = LanguageManager.GetInstance().ConvertSpecifier("Tuning");
                    RaisePropertyChanged("Tuning");
                    _isTuningDirty = false;
                }
                else
                {
                    return false;
                }
            }

            if (_isConfigurationDirty)
            {
                if (CheckConfigurationData())
                {
                    _tag.SetStringValue("PE", PE == PEType.Dependent ? "true" : "false");
                    _tag.SetStringValue("CA", CA == CAType.PV ? "true" : "false");
                    _tag.SetStringValue("DOE", DOE == DOEType.PV ? "true" : "false");

                    _tag.SetStringValue("UPD", UPD.ToString());
                    _tag.SetStringValue("MAXO", MAXO.ToString());
                    _tag.SetStringValue("MINO", MINO.ToString());
                    _tag.SetStringValue("DB", DB.ToString());

                    _tag.SetStringValue("NDF", NDF ? "True" : "False");
                    _tag.SetStringValue("NOBC", NOBC ? "True" : "False");
                    _tag.SetStringValue("NOZC", NOZC ? "True" : "False");
                    _tag.SetStringValue("PVT", PVT ? "True" : "False");
                    _tag.SetStringValue("CL", CL ? "True" : "False");
                    _tag.SetStringValue("CT", CT == CTType.Master ? "true" : "false");

                    Configuration = LanguageManager.GetInstance().ConvertSpecifier("Configuration");
                    RaisePropertyChanged("Configuration");
                    RaisePropertyChanged("SOIsReadOnly");
                    _isConfigurationDirty = false;
                }
                else
                {
                    return false;
                }
            }

            if (_isAlarmsDirty)
            {
                if (CheckAlarmsData())
                {
                    _tag.SetStringValue("PVH", PVH.ToString());
                    _tag.SetStringValue("PVL", PVL.ToString());
                    _tag.SetStringValue("PVDB", PVDB.ToString());
                    _tag.SetStringValue("DVP", DVP.ToString());
                    _tag.SetStringValue("DVN", DVN.ToString());
                    _tag.SetStringValue("DVDB", DVDB.ToString());

                    Alarms = LanguageManager.GetInstance().ConvertSpecifier("Alarms");
                    RaisePropertyChanged("Alarms");
                    _isAlarmsDirty = false;

                }
                else
                {
                    return false;
                }
            }

            if (_isScalingDirty)
            {
                if (CheckScalingData())
                {
                    _tag.SetStringValue("MAXI", PVH.ToString());
                    _tag.SetStringValue("MINI", PVL.ToString());
                    _tag.SetStringValue("MAXS", PVDB.ToString());
                    _tag.SetStringValue("MINS", DVP.ToString());
                    _tag.SetStringValue("MAXCV", DVN.ToString());
                    _tag.SetStringValue("MINCV", DVDB.ToString());
                    _tag.SetStringValue("MAXTIE", DVN.ToString());
                    _tag.SetStringValue("MINTIE", DVDB.ToString());

                    Scaling = LanguageManager.GetInstance().ConvertSpecifier("Scaling");
                    RaisePropertyChanged(nameof(Scaling));
                    RaisePropertyChanged(nameof(SPIsReadOnly));
                    _isScalingDirty = false;

                }
                else
                {
                    return false;
                }
            }

            if (_isTagDirty)
            {
                if (CheckTagData())
                {
                    _tag.Name = Name;
                    _tag.Description = Description;

                    Title = "PID Setup - " + Name;
                    RaisePropertyChanged(nameof(Title));
                    Tag = LanguageManager.GetInstance().ConvertSpecifier("Variable");
                    RaisePropertyChanged(nameof(Tag));
                    _isTagDirty = false;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public bool TagIsSelected { get; set; }

        public bool CheckTagData()
        {
            return true;
        }

        public bool ScalingIsSelected { get; set; }

        public bool CheckScalingData()
        {
            if (MAXI < MINI)
            {
                return false;
            }

            if (MAXCV < MINCV)
            {
                return false;
            }

            return !(MAXTIE < MINTIE);
        }

        public bool CheckMAXIMINI()
        {
            if (MAXI < MINI)
            {
                MessageBox.Show(
                    "Failed to set PV Unscaled Min. and PV Unscaled Max. values.\n" +
                    "PV Unscaled Min. value is greater than PV Unscaled Max. value.",
                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                ScalingIsSelected = true;
                RaisePropertyChanged("ScalingIsSelected");
            }

            return MAXI < MINI;
        }

        public bool CheckMAXCVMINCV()
        {
            if (MAXCV < MINCV)
            {
                MessageBox.Show(
                    "Failed to set CV Min.(at 0 %) and CV Max.(at 100 %) values.\n" +
                    "CV Min.(at 0 %) value is greater than CV Max.(at 100 %) value.",
                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                ScalingIsSelected = true;
                RaisePropertyChanged("ScalingIsSelected");
            }

            return MAXCV < MINCV;
        }

        public bool CheckMAXTIEMINTIE()
        {
            if (MAXTIE < MINTIE)
            {
                MessageBox.Show(
                    "Failed to set Tieback Min.(at 0 %) and Tieback Max.(at 100 %) values.\n" +
                    "Tieback Min.(at 0 %) is greater than Tieback Max.(at 100 %) value.",
                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                ScalingIsSelected = true;
                RaisePropertyChanged("ScalingIsSelected");
            }

            return MAXTIE < MINTIE;
        }


        public bool AlamsIsSelected { get; set; }

        public bool CheckAlarmsData()
        {
            if (PVH < PVL)
            {
                return false;
            }

            return !(DVP < DVN);
        }

        public bool CheckPVHPVL()
        {
            if (PVH < PVL)
            {
                MessageBox.Show(
                    "Failed to set Process variable(PV) Low and Process Variable(PV) High alarm values.\n" +
                    "Process Variable(PV) Low alarm is greater than Process Variable(PV) High alarm.",
                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                AlamsIsSelected = true;
                RaisePropertyChanged("AlamsIsSelected");
            }

            return PVH < PVL;
        }

        public bool CheckDVPDVN()
        {
            if (DVP < DVN)
            {
                MessageBox.Show(
                    "Failed to set Negative Deviation and Positive Deviation alarm values.\n" +
                    "Negative Deviation alarm is greater than Positive Deviation alarm.",
                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                AlamsIsSelected = true;
                RaisePropertyChanged("AlamsIsSelected");
            }

            return DVP < DVN;
        }

        public bool ConfigurationIsSelected { get; set; }

        public bool CheckConfigurationData()
        {
            if (MINO > MAXO)
            {
                return false;
            }

            return !(UPD <= 0) && !(UPD > 32767);
        }

        public bool CheckMINOMAXO()
        {
            if (MINO > MAXO)
            {
                MessageBox.Show(
                    "Failed to set CV High Limit and CV Low Limit Percent values.\n" +
                    "CV Low Limit is greater than CV High Limit.",
                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                ConfigurationIsSelected = true;
                RaisePropertyChanged("ConfigurationIsSelected");
            }

            return MINO > MAXO;
        }

        public bool CheckUDP()
        {
            if (UPD <= 0 || UPD > 32767)
            {
                MessageBox.Show(
                    "Failed to set Loop Update Time value to:" + UPD + "\n" +
                    "Immediate value out of range.\n" +
                    "Value must be great than 0.000000 and less than 32767.",
                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                ConfigurationIsSelected = true;
                RaisePropertyChanged("ConfigurationIsSelected");
            }

            return UPD <= 0 || UPD > 32767;
        }

        public bool CheckTuningData()
        {
            if (SP > MAXS || SP < MINS)
            {
                return false;
            }

            if (SO < MINO || SO > MAXO)
            {
                return false;
            }

            if (BIAS > 100 || BIAS < 0)
            {

                return false;
            }

            if (KP >= 999999990)
            {
                //string s = KP.ToString("e8");
                //KP、KI、KD全都是一样的转化
                //TODO（TLM）：999999990开始都是科学计数法表示
            }

            return true;
        }

        public bool TuningIsSelected { get; set; }

        public bool CheckSP()
        {
            if (SP > MAXS || SP < MINS)
            {
                MessageBox.Show(
                    "Failed to set Setpoint value to: " + SP + "\n" +
                    "Immediate value out of range.\n\n" +
                    "Value must be within scaled PV Min. and scaled PV Max.\n" +
                    "Scaled PV Min. and scaled PV Max. are currently set to " + MINS + " and " + MAXS + ".",
                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                TuningIsSelected = true;
                RaisePropertyChanged("TuningIsSelected");
            }

            return SP > MAXS || SP < MINS;
        }

        public bool CheckSO()
        {
            if (SO < MINO || SO > MAXO)
            {
                MessageBox.Show(
                    "Failed to set Output value to:" + SO + "%\n" +
                    "Immediate value out of range.\n\n" +
                    "Value must be within CV High Limit and CV Low Limit.\n" +
                    "CV High Limit and CV Low Limit are currently set to " + MINO + " and " + MAXO + ".",
                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                TuningIsSelected = true;
                RaisePropertyChanged("TuningIsSelected");
            }

            return SO < MINO || SO > MAXO;
        }

        public bool CheckBIAS()
        {
            if (BIAS > 100 || BIAS < 0)
            {
                MessageBox.Show(
                    "Failed to set Output Bias value.\n" +
                    "Immediate value out of range.\n" +
                    "Value must be within CV High Limit and CV Low Limit.\n" +
                    "Value must be with 0% and 100%",
                    "ICS Studio", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                TuningIsSelected = true;
                RaisePropertyChanged("TuningIsSelected");
            }

            return BIAS > 100 || BIAS < 0;
        }
    }


    [TypeConverter(typeof(EnumMemberValueConverter))]
    public enum PEType
    {
        Dependent,
        Independent
    }

    [TypeConverter(typeof(EnumMemberValueConverter))]
    public enum CAType
    {
        [EnumMember(Value = "SP-PV")] SP,
        [EnumMember(Value = "PV-SP")] PV
    }

    [TypeConverter(typeof(EnumMemberValueConverter))]
    public enum DOEType
    {
        PV,
        Error
    }

    public enum CTType
    {
        Master,
        Slave
    }
}
