using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Dialog;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.Dialogs.AddSTElement
{
    internal class AddSTElementViewModel:ViewModelBase
    {
        private bool _showTreeView = true;
        private Visibility _listViewVisibility = Visibility.Collapsed;
        private Visibility _treeViewVisibility = Visibility.Visible;
        private double _descriptionWidth;
        private double _nameWidth;
        private Thickness _descriptionMargin;
        private Thickness _descriptionListThickness;
        private double _descriptionTextWidth;
        private double _descriptionTextTreeWidth;
        private string _name;
        private readonly IController _controller;
        private bool? _dialogResult;
        private STElement _selectedElement;

        public AddSTElementViewModel(string name,IController controller)
        {
            ListViewTree = new List<STElement>();
            ListView = new List<STElement>();
            AllST = new List<string>();
            NewAOICommand = new RelayCommand(ExecuteNewAOICommand);
            OkCommand=new RelayCommand(ExecuteOkCommand, CanOkCommand);
            CancelCommand=new RelayCommand(ExecuteCancelCommand);
            Name = name;
            _controller = controller;
            NameWidth = 110;
            DescriptionWidth = 200;
            SetList();
            SetCompletedList();
        }

        public string Name
        {
            set
            {
                Set(ref _name, value);
                SetFocus();
            }
            get { return _name; }
        }
        public RelayCommand NewAOICommand { get; }

        public void ExecuteNewAOICommand()
        {
            var createDialogService = Package.GetGlobalService(typeof(SCreateDialogService)) as ICreateDialogService;
            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            if (createDialogService != null)
            {
                var dialog = createDialogService.CreateAddOnInstruction();
                dialog.ShowDialog(uiShell);
            }
        }

        public RelayCommand OkCommand { set; get; }

        public bool CanOkCommand()
        {
            if (SelectedElement != null) return true;
            return false;
        }

        public void ExecuteOkCommand()
        {
            if(SelectedElement!=null&&SelectedElement.IsElement)
                DialogResult = true;
        }

        public RelayCommand CancelCommand { set; get; }

        public void ExecuteCancelCommand()
        {
            DialogResult = false;
        }

        public bool? DialogResult
        {
            set { Set(ref _dialogResult, value); }
            get { return _dialogResult; }
        }

        public List<string> AllST { get; }

        public STElement SelectedElement
        {
            set
            {
                _selectedElement = value;
                OkCommand.RaiseCanExecuteChanged();
                _name = _selectedElement?.Name;
                RaisePropertyChanged("Name");
            }
            get { return _selectedElement; }
        }

        public List<STElement> ListViewTree { set; get; }
        public List<STElement> ListView { set; get; }
        public double DescriptionWidth
        {
            set
            {
                Set(ref _descriptionWidth, value);
                DescriptionTextWidth = value - 50;
                DescriptionTextTreeWidth = value - 30;
            }
            get { return _descriptionWidth; }
        }

        public double DescriptionTextWidth
        {
            set { Set(ref _descriptionTextWidth, value); }
            get { return _descriptionTextWidth; }
        }

        public double DescriptionTextTreeWidth
        {
            set { Set(ref _descriptionTextTreeWidth, value); }
            get { return _descriptionTextTreeWidth; }
        }

        public Thickness DescriptionMargin
        {
            set { Set(ref _descriptionMargin, value); }
            get { return _descriptionMargin; }
        }

        public Thickness DescriptionListMargin
        {
            set { Set(ref _descriptionListThickness, value); }
            get { return _descriptionListThickness; }
        }

        public double NameWidth
        {
            set
            {
                DescriptionWidth = DescriptionWidth + (_nameWidth - value);
                DescriptionMargin = new Thickness(value - 35, 2, 2, 2);
                DescriptionListMargin = new Thickness(value, 2, 2, 2);
                Set(ref _nameWidth, value);
            }
            get { return _nameWidth; }
        }
        public Visibility ListViewVisibility
        {
            set { Set(ref _listViewVisibility, value); }
            get { return _listViewVisibility; }
        }

        public Visibility TreeViewVisibility
        {
            set { Set(ref _treeViewVisibility, value); }
            get { return _treeViewVisibility; }
        }

        public bool ShowTreeView
        {
            set
            {
                _showTreeView = value;
                if (_showTreeView)
                {
                    TreeViewVisibility = Visibility.Visible;
                    ListViewVisibility = Visibility.Collapsed;
                }
                else
                {
                    TreeViewVisibility = Visibility.Collapsed;
                    ListViewVisibility = Visibility.Visible;
                }
            }
            get { return _showTreeView; }
        }

        private void SetList()
        {
            #region process
            var process = new STElement() { Name = "Process", IconKind = "Folder",IsElement = false};

            var alm=new STElement() {Name = "ALM",Description = "Alarm"};
            ListView.Add(alm);
            process.Items.Add(alm);
            var scl=new STElement() {Name = "SCL",Description = "Scale" };
            ListView.Add(scl);
            process.Items.Add(scl);
            var pid=new STElement() {Name = "PID",Description = "Proportional Integral Derivative"};
            ListView.Add(pid);
            process.Items.Add(pid);
            var pide=new STElement() {Name = "PIDE",Description = "Enhanced PID"};
            ListView.Add(pide);
            process.Items.Add(pide);
            var imc=new STElement() {Name = "IMC",Description = "Internal Model Control"};
            ListView.Add(imc);
            process.Items.Add(imc);
            var cc=new STElement() {Name = "CC",Description = "Coordinated Control" };
            ListView.Add(cc);
            process.Items.Add(cc);
            var mmc=new STElement() {Name = "MMC",Description = "Modular Multivariable Control"};
            ListView.Add(mmc);
            process.Items.Add(mmc);
            var rmps=new STElement() {Name = "RMPS",Description = "Ramp/Soak" };
            ListView.Add(rmps);
            process.Items.Add(rmps);
            var posp=new STElement() {Name = "POSP",Description = "Position Proportional"};
            ListView.Add(posp);
            process.Items.Add(posp);
            var srtp=new STElement() {Name = "SRTP",Description = "Split Range Time Proportional"};
            ListView.Add(srtp);
            process.Items.Add(srtp);
            var ldlg=new STElement() {Name="LDLG",Description = "Lead-Lag"};
            ListView.Add(ldlg);
            process.Items.Add(ldlg);
            var fgen=new STElement() {Name = "FGEN",Description = "Function Generator"};
            ListView.Add(fgen);
            process.Items.Add(fgen);
            var tot=new STElement() {Name = "TOT",Description = "Totalizer"};
            ListView.Add(tot);
            process.Items.Add(tot);
            var dedt=new STElement() {Name = "DEDT",Description = "Deadtime"};
            ListView.Add(dedt);
            process.Items.Add(dedt);
            var d2sd=new STElement() {Name = "D2SD",Description = "Discrete 2-State Device"};
            ListView.Add(d2sd);
            process.Items.Add(d2sd);
            var d3sd = new STElement() { Name = "D3SD", Description = "Discrete 3-State Device" };
            ListView.Add(d3sd);
            process.Items.Add(d3sd);

            ListViewTree.Add(process);
            #endregion

            #region Drives
            var drives=new STElement() {Name = "Drives",IconKind = "Folder", IsElement = false };
            var pmul=new STElement() {Name = "PMUL",Description = "Pulse Mulitiplier"};
            ListView.Add(pmul);
            drives.Items.Add(pmul);
            var scrv=new STElement() {Name = "SCRV",Description = "S-Curve"};
            ListView.Add(scrv);
            drives.Items.Add(scrv);
            var pi=new STElement() {Name = "PI",Description = "Proportional+Integral"};
            ListView.Add(pi);
            drives.Items.Add(pi);
            var intg=new STElement() {Name = "INTG",Description = "Integrator"};
            ListView.Add(intg);
            drives.Items.Add(intg);
            var soc=new STElement() {Name = "SOC",Description = "Second-Order Controller"};
            ListView.Add(soc);
            drives.Items.Add(soc);
            var updn=new STElement() {Name = "UPDN",Description = "Up/Down Accumulator"};
            ListView.Add(updn);
            drives.Items.Add(updn);

            ListViewTree.Add(drives);
            #endregion

            #region Filters

            var filters=new STElement() {Name = "Filters", IconKind = "Folder", IsElement = false };
            var hpf=new STElement() {Name = "HPF",Description = "High-Pass Filter"};
            ListView.Add(hpf);
            filters.Items.Add(hpf);
            var lpf=new STElement() {Name = "LPF",Description = "Low-Pass Filter"};
            ListView.Add(lpf);
            filters.Items.Add(lpf);
            var ntch=new STElement() {Name = "NTCH",Description = "Notch Filter"};
            ListView.Add(ntch);
            filters.Items.Add(ntch);
            var ldl2=new STElement() {Name = "LDL2",Description = "Second-Order Lead-Lag"};
            ListView.Add(ldl2);
            filters.Items.Add(ldl2);
            var derv=new STElement() {Name = "DERV",Description = "Derivative"};
            ListView.Add(derv);
            filters.Items.Add(derv);

            ListViewTree.Add(filters);
            #endregion

            #region Select/Limit

            var sl=new STElement() {Name = "Select/Limit", IconKind = "Folder", IsElement = false };
            
            var esel=new STElement() {Name = "ESEL",Description = "Enhanced Select"};
            ListView.Add(esel);
            sl.Items.Add(esel);
            var ssum=new STElement() {Name = "SSUM",Description = "Selected Summer"};
            ListView.Add(ssum);
            sl.Items.Add(ssum);
            var sneg=new STElement() {Name = "SNEG",Description = "Selectable Negate"};
            ListView.Add(sneg);
            sl.Items.Add(sneg);
            var hll=new STElement() {Name = "HLL",Description = "High/Low Limit"};
            ListView.Add(hll);
            sl.Items.Add(hll);
            var rlim=new STElement() {Name = "RLIM",Description = "Rate Limiter"};
            ListView.Add(rlim);
            sl.Items.Add(rlim);

            ListViewTree.Add(sl);
            #endregion

            #region Statistical

            var statistical=new STElement() {Name = "Statistical", IconKind = "Folder", IsElement = false };
            var mave=new STElement() {Name = "Mave",Description = "Moving Average"};
            ListView.Add(mave);
            statistical.Items.Add(mave);
            var mstd=new STElement() {Name = "MSTD",Description = "Moving Standard Deviation"};
            ListView.Add(mstd);
            statistical.Items.Add(mstd);
            var minc=new STElement() {Name = "MINC",Description = "Minimum Capture"};
            ListView.Add(minc);
            statistical.Items.Add(minc);
            var maxc=new STElement() {Name = "MAXC",Description = "Maximum Capture"};
            ListView.Add(maxc);
            statistical.Items.Add(maxc);

            ListViewTree.Add(statistical);
            #endregion

            #region Alarms

            var alarms=new STElement() {Name = "Alarms", IconKind = "Folder", IsElement = false };
            var almd=new STElement() {Name = "ALMD",Description = "Digital Alarm"};
            ListView.Add(almd);
            alarms.Items.Add(almd);
            var alma=new STElement() {Name = "ALMA",Description = "Analog Alarm"};
            ListView.Add(alma);
            alarms.Items.Add(alma);

            ListViewTree.Add(alarms);
            #endregion

            #region Bit

            var bit=new STElement() {Name = "Bit", IconKind = "Folder", IsElement = false };
            var osri=new STElement() {Name = "OSRI",Description = "One Shot Rising with Input"};
            ListView.Add(osri);
            bit.Items.Add(osri);
            var osfi=new STElement() {Name = "OSFI",Description = "One Shot Falling with Input"};
            ListView.Add(osfi);
            bit.Items.Add(osfi);

            ListViewTree.Add(bit);
            #endregion

            #region Timer/Counter

            var tc=new STElement() {Name = "Timer/Counter", IconKind = "Folder", IsElement = false };

            var tonr=new STElement() {Name = "TONR",Description = "Timer On Delay with Reset"};
            ListView.Add(tonr);
            tc.Items.Add(tonr);
            var tofr=new STElement() {Name = "TOFR",Description = "Timer Off Delay with Reset"};
            ListView.Add(tofr);
            tc.Items.Add(tofr);
            var rtot=new STElement() {Name = "RTOR",Description = "Retentive Timer On with Reset"};
            ListView.Add(rtot);
            tc.Items.Add(rtot);
            var ctud=new STElement() {Name = "CTUD",Description = "Count Up/Down"};
            ListView.Add(ctud);
            tc.Items.Add(ctud);
            ListViewTree.Add(tc);

            #endregion

            #region Compute/Math

            var cm=new STElement() {Name = "Compute/Math", IconKind = "Folder", IsElement = false };
            var sqrt=new STElement() {Name = "SQRT",Description = "Square Root"};
            ListView.Add(sqrt);
            cm.Items.Add(sqrt);
            var abs=new STElement() {Name = "ABS",Description = "Absolute value"};
            ListView.Add(abs);
            cm.Items.Add(abs);
            ListViewTree.Add(cm);

            #endregion

            #region Move/Logical

            var ml=new STElement() {Name = "Move/Logical", IconKind = "Folder", IsElement = false };
            var mvmt=new STElement() {Name = "MVMT",Description = "Masked Move with Target"};
            ListView.Add(mvmt);
            ml.Items.Add(mvmt);
            var swpb=new STElement() {Name = "SWPB",Description = "Swap Byte"};
            ListView.Add(swpb);
            ml.Items.Add(swpb);
            var btdt=new STElement() {Name = "BTDT",Description = "Bit Field Distribute with Target"};
            ListView.Add(btdt);
            ml.Items.Add(btdt);
            var dff=new STElement() {Name = "DFF",Description = "D Flip Flop"};
            ListView.Add(dff);
            ml.Items.Add(dff);
            var jkff=new STElement() {Name = "JKFF",Description = "JK Flip Flop"};
            ListView.Add(jkff);
            ml.Items.Add(jkff);
            var setd=new STElement() {Name = "SETD",Description = "Set Dominant"};
            ListView.Add(setd);
            ml.Items.Add(setd);
            var resd=new STElement() {Name = "RESD",Description = "Reset Dominant" };
            ListView.Add(resd);
            ml.Items.Add(resd);
            
            ListViewTree.Add(ml);
            #endregion

            #region HMI

            var hmi=new STElement() {Name = "HMI", IconKind = "Folder", IsElement = false };
            var hmibc=new STElement() {Name = "HMIBC",Description = "HMI Button Control"};
            ListView.Add(hmibc);
            hmi.Items.Add(hmibc);
            ListViewTree.Add(hmi);
            #endregion

            #region Trigonometry

            var trigonometry=new STElement() {Name = "Trigonometry", IconKind = "Folder", IsElement = false };
            var sin=new STElement() {Name = "SIN",Description = "Sine"};
            ListView.Add(sin);
            trigonometry.Items.Add(sin);
            var cos=new STElement() {Name = "COS",Description = "Cosine"};
            ListView.Add(cos);
            trigonometry.Items.Add(cos);
            var tan=new STElement() {Name = "TAN",Description = "Tangent"};
            ListView.Add(tan);
            trigonometry.Items.Add(tan);
            var asin=new STElement() {Name = "ASIN",Description = "Arcsine"};
            ListView.Add(asin);
            trigonometry.Items.Add(asin);
            var acos=new STElement() {Name = "ACOS",Description = "Arccosine"};
            ListView.Add(acos);
            trigonometry.Items.Add(acos);
            var atan=new STElement() {Name = "ATAN",Description = "Arctangent"};
            ListView.Add(atan);
            trigonometry.Items.Add(atan);

            ListViewTree.Add(trigonometry);
            #endregion

            #region Advanced Math

            var am=new STElement() {Name = "Advanced Math", IconKind = "Folder", IsElement = false };
            var ln=new STElement() {Name = "LN",Description = "Natural Log"};
            ListView.Add(ln);
            am.Items.Add(ln);
            var log=new STElement() {Name = "LOG",Description = "Log Base 10"};
            ListView.Add(log);
            am.Items.Add(log);

            ListViewTree.Add(am);
            #endregion

            #region Math Conversion

            var mc=new STElement() {Name = "Math Conversion", IconKind = "Folder", IsElement = false };
            var deg=new STElement() {Name = "DEG",Description = "Radians To Degrees"};
            ListView.Add(deg);
            mc.Items.Add(deg);
            var rad=new STElement() {Name = "RAD",Description = "Degrees To Radians"};
            ListView.Add(rad);
            mc.Items.Add(rad);
            var trunc=new STElement() {Name = "TRUNC",Description = "Truncate"};
            ListView.Add(trunc);
            mc.Items.Add(trunc);

            ListViewTree.Add(mc);

            #endregion

            #region File/Misc.

            var fm=new STElement() {Name = "File/Misc.", IconKind = "Folder", IsElement = false };
            var cop = new STElement() {Name = "COP", Description = "Cop File"};
            ListView.Add(cop);
            fm.Items.Add(cop);
            var srt=new STElement() {Name = "SRT",Description = "Sort File"};
            ListView.Add(srt);
            fm.Items.Add(srt);
            var size=new STElement() {Name = "SIZE",Description = "Size in Elements"};
            ListView.Add(size);
            fm.Items.Add(size);
            var cps=new STElement() {Name = "CPS",Description = "Synchronous Copy File"};
            ListView.Add(cps);
            fm.Items.Add(cps);

            ListViewTree.Add(fm);
            #endregion

            #region Equipment Phase

            var ep=new STElement() {Name = "Equipment Phase", IconKind = "Folder", IsElement = false };
            var psc=new STElement() {Name = "PSC",Description = "Equipment Phase State Complete"};
            ListView.Add(psc);
            ep.Items.Add(psc);
            var pfl=new STElement() {Name = "PFL",Description = "Equipment Phase Failure"};
            ListView.Add(pfl);
            ep.Items.Add(pfl);
            var pcmd=new STElement() {Name = "PCMD",Description = "Equipment Phase Command"};
            ListView.Add(pcmd);
            ep.Items.Add(pcmd);
            var pclf=new STElement() {Name = "PCLF",Description = "Equipment Phase Clear Failure"};
            ListView.Add(pclf);
            ep.Items.Add(pclf);
            var pxrq=new STElement() {Name = "PXRQ",Description = "Equipment Phase External Request"};
            ListView.Add(pxrq);
            ep.Items.Add(pxrq);
            var ppd=new STElement() {Name = "PPD",Description = "Equipment Phase Pause"};
            ListView.Add(ppd);
            ep.Items.Add(ppd);
            var prnp=new STElement() {Name = "PRNP",Description = "Equipment Phase New Parameters"};
            ListView.Add(prnp);
            ep.Items.Add(prnp);
            var patt=new STElement() {Name = "PATT",Description = "Attach to Equipment Phase"};
            ListView.Add(patt);
            ep.Items.Add(patt);
            var pdet=new STElement() {Name = "PDET",Description = "Detach from Equipment Phase"};
            ListView.Add(pdet);
            ep.Items.Add(pdet);
            var povr=new STElement() {Name = "POVR",Description = "Equipment Phase Override Command"};
            ListView.Add(povr);
            ep.Items.Add(povr);

            ListViewTree.Add(ep);
            #endregion

            #region Equipment Sequence

            var es=new STElement() {Name = "Equipment Sequence", IconKind = "Folder", IsElement = false };
            var satt=new STElement() {Name = "SATT",Description = "Attach to Sequence"};
            ListView.Add(satt);
            es.Items.Add(satt);
            var sdet=new STElement() {Name = "SDET",Description = "Detach from Sequence"};
            ListView.Add(sdet);
            es.Items.Add(sdet);
            var scmd=new STElement() {Name = "SCMD",Description = "Sequence Command"};
            ListView.Add(scmd);
            es.Items.Add(scmd);
            var sclf=new STElement() {Name = "SCLF",Description = "Sequence Clear Failure"};
            ListView.Add(sclf);
            es.Items.Add(sclf);
            var sovr=new STElement() {Name = "SOVR",Description = "Sequence Override Command"};
            ListView.Add(sovr);
            es.Items.Add(sovr);
            var sasi=new STElement() {Name = "SASI",Description = "Sequence Assign Sequence Id"};
            ListView.Add(sasi);
            es.Items.Add(sasi);
            
            ListViewTree.Add(es);

            #endregion

            #region Program Control

            var pc=new STElement() {Name = "Program Control", IconKind = "Folder", IsElement = false };
            var jsr=new STElement() {Name = "JSR",Description = "Jump to Subroutine"};
            ListView.Add(jsr);
            pc.Items.Add(jsr);
            var ret=new STElement() {Name = "RET",Description = "Return from Subroutine"};
            ListView.Add(ret);
            pc.Items.Add(ret);
            var sbr=new STElement() {Name = "SBR",Description = "Subroutine"};
            ListView.Add(sbr);
            pc.Items.Add(sbr);
            var tnd=new STElement() {Name = "TND",Description = "Temporary End"};
            ListView.Add(tnd);
            pc.Items.Add(tnd);
            var uid=new STElement() {Name = "UID",Description = "User Interrupt Disable"};
            ListView.Add(uid);
            pc.Items.Add(uid);
            var uie=new STElement() {Name = "UIE",Description = "User Interrupt"};
            ListView.Add(uie);
            pc.Items.Add(uie);
            var sfr=new STElement() {Name = "SFR",Description = "SFC Reset"};
            ListView.Add(sfr);
            pc.Items.Add(sfr);
            var sfp=new STElement() {Name = "SFP",Description = "SFC Phase"};
            ListView.Add(sfp);
            pc.Items.Add(sfp);
            var eot=new STElement() {Name = "EOT",Description = "End Of Transition" };
            ListView.Add(eot);
            pc.Items.Add(eot);
            var @event=new STElement() {Name = "EVENT",Description = "Trigger Event Task"};
            ListView.Add(@event);
            pc.Items.Add(@event);
            
            ListViewTree.Add(pc);
            #endregion

            #region Input/Output

            var io=new STElement() {Name = "Input/Output", IconKind = "Folder", IsElement = false };
            var msg=new STElement() {Name = "MSG",Description = "Message"};
            ListView.Add(msg);
            io.Items.Add(msg);
            var gsv=new STElement() {Name = "GSV",Description = "Get System Value"};
            ListView.Add(gsv);
            io.Items.Add(gsv);
            var ssv=new STElement() {Name = "SSV",Description = "Set System Value"};
            ListView.Add(ssv);
            io.Items.Add(ssv);
            var iot=new STElement() {Name = "IOT",Description = "Immediate Output"};
            ListView.Add(iot);
            io.Items.Add(iot);

            ListViewTree.Add(io);

            #endregion

            #region Motion State

            var ms=new STElement() {Name = "Motion State", IconKind = "Folder", IsElement = false };
            var mso=new STElement() {Name = "MSO",Description = "Motion Servo On"};
            ListView.Add(mso);
            ms.Items.Add(mso);
            var msf=new STElement() {Name = "MSF",Description = "Motion Servo Off"};
            ListView.Add(msf);
            ms.Items.Add(msf);
            var masd=new STElement() {Name = "MASD",Description = "Motion Axis Shutdown"};
            ListView.Add(masd);
            ms.Items.Add(masd);
            var masr=new STElement() {Name = "MASR",Description = "Motion Axis Shutdown Reset"};
            ListView.Add(masr);
            ms.Items.Add(masr);
            var mdo=new STElement() {Name = "MDO",Description = "Motion Direct Drive On"};
            ListView.Add(mdo);
            ms.Items.Add(mdo);
            var mdf=new STElement() {Name = "MDF",Description = "Motion Direct Drive Off"};
            ListView.Add(mdf);
            ms.Items.Add(mdf);
            var mds=new STElement() {Name = "MDS",Description = "Motion Direct Start"};
            ListView.Add(mds);
            ms.Items.Add(mds);
            var mafr=new STElement() {Name = "MAFR",Description = "Motion Axis Fault Reset"};
            ListView.Add(mafr);
            ms.Items.Add(mafr);

            ListViewTree.Add(ms);
            #endregion

            #region Motion Move

            var mm=new STElement() {Name = "Motion Move", IconKind = "Folder", IsElement = false };
            var mas=new STElement() {Name = "MAS",Description = "Motion Axis Stop"};
            ListView.Add(mas);
            mm.Items.Add(mas);
            var mah=new STElement() {Name = "MAH",Description = "Motion Axis Home"};
            ListView.Add(mah);
            mm.Items.Add(mah);
            var maj=new STElement() {Name = "MAJ",Description = "Motion Axis Jog"};
            ListView.Add(maj);
            mm.Items.Add(maj);
            var mam=new STElement() {Name = "MAM",Description = "Motion Axis Move"};
            ListView.Add(mam);
            mm.Items.Add(mam);
            var mag=new STElement() {Name = "MAG",Description = "Motion Axis Gear"};
            ListView.Add(mag);
            mm.Items.Add(mag);
            var mcd=new STElement() {Name = "MCD",Description = "Motion Change Dynamics"};
            ListView.Add(mcd);
            mm.Items.Add(mcd);
            var mrp=new STElement() {Name = "MRP",Description = "Motion Redefine Position"};
            ListView.Add(mrp);
            mm.Items.Add(mrp);
            var mccp=new STElement() {Name = "MCCP",Description = "Motion Calculate Cam Profile"};
            ListView.Add(mccp);
            mm.Items.Add(mccp);
            var mcsv=new STElement() {Name = "MCSV",Description = "Motion Calculate Slave Values"};
            ListView.Add(mcsv);
            mm.Items.Add(mcsv);
            var mapc=new STElement() {Name = "MAPC",Description = "Motion Axis Position Cam"};
            ListView.Add(mapc);
            mm.Items.Add(mapc);
            var matc=new STElement() {Name = "MATC",Description = "Motion Axis Time Cam"};
            ListView.Add(matc);
            mm.Items.Add(matc);
            var mdac=new STElement() {Name = "MDAC",Description = "Motion Master Driven Axis Control"};
            ListView.Add(mdac);
            mm.Items.Add(mdac);

            ListViewTree.Add(mm);
            #endregion

            #region Motion Group

            var mg=new STElement() {Name = "Motion Group", IconKind = "Folder", IsElement = false };
            var mgs=new STElement() {Name = "MGS",Description = "Motion Group Stop"};
            ListView.Add(mgs);
            mg.Items.Add(mgs);
            var mgsd=new STElement() {Name = "MGSD",Description = "Motion Group Shutdown"};
            ListView.Add(mgsd);
            mg.Items.Add(mgsd);
            var mgsr=new STElement() {Name = "MGSR",Description = "Motion Group Shutdown Reset"};
            ListView.Add(mgsr);
            mg.Items.Add(mgsr);
            var mgsp=new STElement() {Name = "MGSP",Description = "Motion Group Strobe Position"};
            ListView.Add(mgsp);
            mg.Items.Add(mgsp);
            
            ListViewTree.Add(mg);
            #endregion

            #region Motion Event

            var me=new STElement() {Name = "Motion Event", IconKind = "Folder", IsElement = false };
            var maw=new STElement() {Name = "MAW",Description = "Motion Arm Watch"};
            ListView.Add(maw);
            me.Items.Add(maw);
            var mdw=new STElement() {Name = "MDW",Description = "Motion Disarm Watch"};
            ListView.Add(mdw);
            me.Items.Add(mdw);
            var mar=new STElement() {Name = "MAR",Description = "Motion Arm Registration"};
            ListView.Add(mar);
            me.Items.Add(mar);
            var mdr = new STElement() { Name = "MDR", Description = "Motion Disarm Registration" };
            ListView.Add(mdr);
            me.Items.Add(mdr);
            var maoc=new STElement() {Name = "MAOC",Description = "Motion Arm Output Cam"};
            ListView.Add(maoc);
            me.Items.Add(maoc);
            var mdoc=new STElement() {Name = "MDOC",Description = "Motion Disarm Output Cam"};
            ListView.Add(mdoc);
            me.Items.Add(mdoc);

            ListViewTree.Add(me);
            #endregion

            #region Motion Config
            var mc2=new STElement() {Name = "Motion Config", IconKind = "Folder", IsElement = false };
            var maat=new STElement() {Name = "MAAT",Description = "Motion Apply Axis Tuning"};
            ListView.Add(maat);
            mc2.Items.Add(maat);
            var mrat=new STElement() {Name = "MRAT",Description = "Motion Run Axis Tuning"};
            ListView.Add(mrat);
            mc2.Items.Add(mrat);
            var mahd=new STElement() {Name = "MAHD",Description = "Motion Apply Hookup Diagnostics"};
            ListView.Add(mahd);
            mc2.Items.Add(mahd);
            var mrhd=new STElement() {Name = "MRHD",Description = "Motion Run Hookup Diagnostics"};
            ListView.Add(mrhd);
            mc2.Items.Add(mrhd);


            ListViewTree.Add(mc2);

            #endregion

            #region Motion Coordinated

            var mco=new STElement() {Name = "Motion Coordinated", IconKind = "Folder", IsElement = false };
            var mcs=new STElement() {Name = "MCS",Description = "Motion Coordinated Stop"};
            ListView.Add(mcs);
            mco.Items.Add(mcs);
            var mclm=new STElement() {Name = "MCLM",Description = "Motion Coordinated Linear Move"};
            ListView.Add(mclm);
            mco.Items.Add(mclm);
            var mccm=new STElement() {Name = "MCCM",Description = "Motion Coordinated Circular Move"};
            ListView.Add(mccm);
            mco.Items.Add(mccm);
            var mct=new STElement() {Name = "MCT",Description = "Motion Coordinated Transform"};
            ListView.Add(mct);
            mco.Items.Add(mct);
            var mctp=new STElement() {Name = "MCTP",Description = "Motion Calculate Transition Position"};
            ListView.Add(mctp);
            mco.Items.Add(mctp);
            var mcsd=new STElement() {Name = "MCSD",Description = "Motion Coordinated Shutdown"};
            ListView.Add(mcsd);
            mco.Items.Add(mcsd);
            var mcsr=new STElement() {Name = "MCSR",Description = "Motion Coordinated Shutdown Reset"};
            ListView.Add(mcsr);
            mco.Items.Add(mcsr);
            var mdcc=new STElement() {Name = "MDCC",Description = "Motion Master Driven Coordinated Control"};
            ListView.Add(mdcc);
            mco.Items.Add(mdcc);

            ListViewTree.Add(mco);
            #endregion

            #region ACSII String

            var asciiString=new STElement() {Name = "ASCII String", IconKind = "Folder", IsElement = false };
            var find=new STElement() {Name = "Find",Description = "Find String"};
            ListView.Add(find);
            asciiString.Items.Add(find);
            var insert=new STElement() {Name = "INSERT",Description = "Insert String"};
            ListView.Add(insert);
            asciiString.Items.Add(insert);
            var concat=new STElement() {Name = "CONCAT",Description = "String Concatenate"};
            ListView.Add(concat);
            asciiString.Items.Add(concat);
            var mid=new STElement() {Name = "MID",Description = "Middle String"};
            ListView.Add(mid);
            asciiString.Items.Add(mid);
            var delete=new STElement() {Name = "DELETE",Description = "String Delete"};
            ListView.Add(delete);
            asciiString.Items.Add(delete);
            ListViewTree.Add(asciiString);
            #endregion

            #region ASCII Conversion

            var asciiConversion=new STElement() {Name = "ASCII Conversion", IconKind = "Folder", IsElement = false };
            var dtos=new STElement() {Name = "DTOS",Description = "DINT to String"};
            ListView.Add(dtos);
            asciiConversion.Items.Add(dtos);
            var stod=new STElement() {Name = "STOD",Description = "String to DINT"};
            ListView.Add(stod);
            asciiConversion.Items.Add(stod);
            var rtos=new STElement() {Name = "RTOS",Description = "Real to String"};
            ListView.Add(rtos);
            asciiConversion.Items.Add(rtos);
            var stor=new STElement() {Name = "STOR",Description = "String to Real"};
            ListView.Add(stor);
            asciiConversion.Items.Add(stor);
            var upper=new STElement() {Name = "UPPER",Description = "Upper Case"};
            ListView.Add(upper);
            asciiConversion.Items.Add(upper);
            var lower=new STElement() {Name = "Lower",Description = "Lower Case" };
            ListView.Add(lower);
            asciiConversion.Items.Add(lower);

            ListViewTree.Add(asciiConversion);

            #endregion

            #region AOI

            var addOn=new STElement() {Name = "Add-On", IconKind = "Folder", IsElement = false };
            foreach (AoiDefinition aoi in _controller.AOIDefinitionCollection)
            {
                var element=new STElement() {Name = aoi.Name,Description = aoi.Description};
                ListView.Add(element);
                addOn.Items.Add(element);
            }

            ListViewTree.Add(addOn);
            #endregion
        }

        private void SetCompletedList()
        {
            foreach (var stElement in ListView)
            {
                AllST.Add(stElement.Name);
                PropertyChangedEventManager.AddHandler(stElement, OnPropertyChanged,"IsSelected");
            }
        }

        private void OnPropertyChanged(object sender,PropertyChangedEventArgs e)
        {
            var element = (STElement) sender;
            if (element.IsSelected)
            {
                SelectedElement = element;
            }
            else
            {
                if (SelectedElement == element) SelectedElement = null;
            }
        }

        private void SetFocus()
        {
            var item = ListView.FirstOrDefault(n => n.Name.Equals(Name, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                var treeItem=ListViewTree.FirstOrDefault(l => l.Items.Contains(item));
                treeItem.IsExpanded = true;
                item.IsSelected = true;
            }
        }
    }
    internal class STElement:ViewModelBase{
        private bool _isExpanded;
        private string _iconKind;
        private bool _isSelected;

        public STElement()
        {
            IconKind = "BorderAllVariant";
        }
        public string Name { set; get; }
        public string Description { set; get; }
        public bool IsElement { set; get; } = true;
        public bool IsSelected
        {
            set { Set(ref _isSelected, value); }
            get { return _isSelected; }
        }

        public bool IsExpanded
        {
            set
            {
                Set(ref _isExpanded, value);
                if (_isExpanded)
                {
                    if (IconKind.Equals("Folder")) IconKind = "FolderOpen";
                }
                else
                {
                    if (IconKind.Equals("FolderOpen")) IconKind = "Folder";
                }
            }
            get { return _isExpanded; }
        }

        public string IconKind
        {
            get { return _iconKind; }
            set { Set(ref _iconKind, value); }
        }

        public List<STElement> Items { get; }=new List<STElement>();
    }
}
