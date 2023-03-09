using System;
using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIServicesPackage.Services;
using ICSStudio.UIServicesPackage.View;
using ICSStudio.UIServicesPackage.ViewModel;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.UIServicesPackage.AddOnInstruction.Panel
{
    class ScanModesViewModel : ViewModelBase, IOptionPanel, ICanBeDirty
    {
        private bool _isDirty;
        private bool _deleteOrNewCommand1;
        private bool _canGoToCommand1;
        private bool _deleteOrNewCommand2;
        private bool _canGoToCommand2;
        private bool _deleteOrNewCommand3;
        private bool _canGoToCommand3;
        private bool _checkBox1, _checkBox2, _checkBox3;
        private readonly AoiDefinition _aoiDefinition;
        private string _new;
        private string _delete;

        public ScanModesViewModel(ScanModes panel, IAoiDefinition aoiDefinition)
        {
            _aoiDefinition = (AoiDefinition) aoiDefinition;
            Control = panel;
            panel.DataContext = this;
            _new = LanguageManager.GetInstance().ConvertSpecifier("New...");
            _delete = LanguageManager.GetInstance().ConvertSpecifier("Delete");
            ButtonName1 = _new;
            ButtonName2 = _new;
            ButtonName3 = _new;
            _canGoToCommand1 = false;
            _canGoToCommand2 = false;
            _canGoToCommand3 = false;
            _deleteOrNewCommand1 = true;
            _deleteOrNewCommand2 = true;
            _deleteOrNewCommand3 = true;
            if (IsContain("Prescan"))
            {
                ButtonName1 = _delete;
                CheckBox1 = _aoiDefinition.ExecutePrescan;
                CheckBoxEnable1 = true;
                _canGoToCommand1 = true;
            }

            if (IsContain("Postscan"))
            {
                ButtonName2 = _delete;
                CheckBox2 = _aoiDefinition.ExecutePostscan;
                CheckBoxEnable2 = true;
                _canGoToCommand2 = true;
            }

            if (IsContain("EnableInFalse"))
            {
                ButtonName3 = _delete;
                CheckBox3 = _aoiDefinition.ExecuteEnableInFalse;
                CheckBoxEnable2 = true;
                _canGoToCommand3 = true;
            }

            DeleteOrNewCommand1 = new RelayCommand(ExecuteDeleteOrNewCommand1, CanDeleteOrNewCommand1);
            GoToCommand1 = new RelayCommand(ExecuteGoToCommand1, CanGoToCommand1);

            DeleteOrNewCommand2 = new RelayCommand(ExecuteDeleteOrNewCommand2, CanDeleteOrNewCommand2);
            GoToCommand2 = new RelayCommand(ExecuteGoToCommand2, CanGoToCommand2);

            DeleteOrNewCommand3 = new RelayCommand(ExecuteDeleteOrNewCommand3, CanDeleteOrNewCommand3);
            GoToCommand3 = new RelayCommand(ExecuteGoToCommand3, CanGoToCommand3);

            SetControlEnable();

            Controller controller = _aoiDefinition.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            WeakEventManager<LanguageManager,EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);

            RaisePropertyChanged("ButtonName1");
            RaisePropertyChanged("ButtonName2");
            RaisePropertyChanged("ButtonName3");
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            _new = LanguageManager.GetInstance().ConvertSpecifier("New...");
            _delete = LanguageManager.GetInstance().ConvertSpecifier("Delete");

            if((ButtonName1.Equals("New...")) || (ButtonName1.Equals("新建...")))
            {
                ButtonName1 = _new;
            }
            else
            {
                ButtonName1 = _delete;
            }

            if ((ButtonName2.Equals("New...")) || (ButtonName2.Equals("新建...")))
            {
                ButtonName2 = _new;
            }
            else
            {
                ButtonName2 = _delete;
            }

            if ((ButtonName3.Equals("New...")) || (ButtonName3.Equals("新建...")))
            {
                ButtonName3 = _new;
            }
            else
            {
                ButtonName3 = _delete;
            }
            RaisePropertyChanged("ButtonName1");
            RaisePropertyChanged("ButtonName2");
            RaisePropertyChanged("ButtonName3");
        }

        public override void Cleanup()
        {
            Controller controller = _aoiDefinition.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
            }
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                SetControlEnable();

                IsDirty = false;
            });
        }

        public void SetControlEnable()
        {
            if (_aoiDefinition.ParentController.IsOnline || _aoiDefinition.IsSealed)
            {
                CheckBoxEnable1 = false;
                _deleteOrNewCommand1 = false;
                _canGoToCommand1 = false;

                CheckBoxEnable2 = false;
                _deleteOrNewCommand2 = false;
                _canGoToCommand2 = false;

                CheckBoxEnable3 = false;
                _deleteOrNewCommand3 = false;
                _canGoToCommand3 = false;

                DeleteOrNewCommand1.RaiseCanExecuteChanged();
                GoToCommand1.RaiseCanExecuteChanged();
                DeleteOrNewCommand2.RaiseCanExecuteChanged();
                GoToCommand2.RaiseCanExecuteChanged();
                DeleteOrNewCommand3.RaiseCanExecuteChanged();
                GoToCommand3.RaiseCanExecuteChanged();
            }
            else
            {
                ButtonName1 = _new;
                ButtonName2 = _new;
                ButtonName3 = _new;
                _canGoToCommand1 = false;
                _canGoToCommand2 = false;
                _canGoToCommand3 = false;
                _deleteOrNewCommand1 = true;
                _deleteOrNewCommand2 = true;
                _deleteOrNewCommand3 = true;
                if (IsContain("Prescan"))
                {
                    ButtonName1 = _delete;
                    CheckBox1 = _aoiDefinition.ExecutePrescan;
                    CheckBoxEnable1 = true;
                    _canGoToCommand1 = true;
                }

                if (IsContain("Postscan"))
                {
                    ButtonName2 = _delete;
                    CheckBox2 = _aoiDefinition.ExecutePostscan;
                    CheckBoxEnable2 = true;
                    _canGoToCommand2 = true;
                }

                if (IsContain("EnableInFalse"))
                {
                    ButtonName3 = _delete;
                    CheckBox3 = _aoiDefinition.ExecuteEnableInFalse;
                    CheckBoxEnable2 = true;
                    _canGoToCommand3 = true;
                }

                DeleteOrNewCommand1.RaiseCanExecuteChanged();
                GoToCommand1.RaiseCanExecuteChanged();
                DeleteOrNewCommand2.RaiseCanExecuteChanged();
                GoToCommand2.RaiseCanExecuteChanged();
                DeleteOrNewCommand3.RaiseCanExecuteChanged();
                GoToCommand3.RaiseCanExecuteChanged();
            }
        }

        public void Compare()
        {
            IsDirty = false;
            if (CheckBox1 != _aoiDefinition.ExecutePrescan) IsDirty = true;
            else if (CheckBox2 != _aoiDefinition.ExecutePostscan) IsDirty = true;
            else if (CheckBox3 != _aoiDefinition.ExecuteEnableInFalse) IsDirty = true;
        }

        public void Save()
        {
            if (_aoiDefinition != null)
            {
                _aoiDefinition.ExecutePrescan = CheckBox1;
                _aoiDefinition.ExecutePostscan = CheckBox2;
                _aoiDefinition.ExecuteEnableInFalse = CheckBox3;
            }

            IsDirty = false;
        }

        #region ExecutePrescan

        public bool CheckBox1
        {
            set
            {
                Set(ref _checkBox1, value);
                Compare();
            }
            get { return _checkBox1; }
        }

        public bool CheckBoxEnable1 { set; get; }
        public RelayCommand DeleteOrNewCommand1 { set; get; }

        public bool CanDeleteOrNewCommand1()
        {
            return _deleteOrNewCommand1;
        }

        public void ExecuteDeleteOrNewCommand1()
        {
            if (ButtonName1 == _new)
            {
                var dialog = new NewScanModeRoutine();
                var viewModel = new NewScanModeRoutineViewModel("Prescan", _aoiDefinition);
                dialog.Width = 380;
                dialog.Height = 230;
                dialog.DataContext = viewModel;
                dialog.Owner = Application.Current.MainWindow;
                if (dialog.ShowDialog().Value)
                {
                    ButtonName1 = _delete;
                    CheckBoxEnable1 = true;
                    CheckBox1 = true;
                    _canGoToCommand1 = true;
                    GoToCommand1.RaiseCanExecuteChanged();
                    RaisePropertyChanged("ButtonName1");
                    RaisePropertyChanged("CheckBoxEnable1");
                    RaisePropertyChanged("CheckBox1");
                }
            }
            else
            {
                DeleteRoutine((_aoiDefinition as AoiDefinition)?.Routines["Prescan"]);
            }

        }

        public RelayCommand GoToCommand1 { set; get; }

        public bool CanGoToCommand1()
        {
            return _canGoToCommand1;
        }

        public void ExecuteGoToCommand1()
        {
            OpenRoutine((_aoiDefinition as AoiDefinition)?.Routines["Prescan"]);
        }

        public string ButtonName1 { set; get; }

        #endregion

        #region ExecutePostscan

        public bool CheckBox2
        {
            set
            {
                Set(ref _checkBox2, value);
                Compare();
            }
            get { return _checkBox2; }
        }

        public bool CheckBoxEnable2 { set; get; }
        public RelayCommand DeleteOrNewCommand2 { set; get; }

        public bool CanDeleteOrNewCommand2()
        {
            return _deleteOrNewCommand2;
        }

        public void ExecuteDeleteOrNewCommand2()
        {
            if (ButtonName2 == _new)
            {
                var dialog = new NewScanModeRoutine();
                var viewModel = new NewScanModeRoutineViewModel("Postscan", _aoiDefinition);
                dialog.Width = 380;
                dialog.Height = 230;
                dialog.DataContext = viewModel;
                dialog.Owner = Application.Current.MainWindow;
                if (dialog.ShowDialog().Value)
                {
                    ButtonName2 = _delete;
                    CheckBoxEnable2 = true;
                    CheckBox2 = true;
                    _canGoToCommand2 = true;
                    GoToCommand2.RaiseCanExecuteChanged();
                    RaisePropertyChanged("ButtonName2");
                    RaisePropertyChanged("CheckBoxEnable2");
                    RaisePropertyChanged("CheckBox2");
                }
            }
            else
            {
                DeleteRoutine((_aoiDefinition as AoiDefinition)?.Routines["Postscan"]);
            }
        }

        public RelayCommand GoToCommand2 { set; get; }

        public bool CanGoToCommand2()
        {
            return _canGoToCommand2;
        }

        public void ExecuteGoToCommand2()
        {
            OpenRoutine((_aoiDefinition as AoiDefinition)?.Routines["Postscan"]);
        }

        public string ButtonName2 { set; get; }

        #endregion

        #region ExecuteEnableInFalse

        public bool CheckBox3
        {
            set
            {
                Set(ref _checkBox3, value);
                Compare();
            }
            get { return _checkBox3; }
        }

        public bool CheckBoxEnable3 { set; get; }
        public RelayCommand DeleteOrNewCommand3 { set; get; }

        public bool CanDeleteOrNewCommand3()
        {
            return _deleteOrNewCommand3;
        }

        public void ExecuteDeleteOrNewCommand3()
        {
            if (ButtonName3 == _new)
            {
                var dialog = new NewScanModeRoutine();
                var viewModel = new NewScanModeRoutineViewModel("EnableInFalse", _aoiDefinition);
                dialog.Width = 380;
                dialog.Height = 230;
                dialog.DataContext = viewModel;
                dialog.Owner = Application.Current.MainWindow;
                if (dialog.ShowDialog().Value)
                {
                    ButtonName3 = _delete;
                    CheckBoxEnable3 = true;
                    CheckBox3 = true;
                    _canGoToCommand3 = true;
                    GoToCommand3.RaiseCanExecuteChanged();
                    RaisePropertyChanged("ButtonName3");
                    RaisePropertyChanged("CheckBoxEnable3");
                    RaisePropertyChanged("CheckBox3");
                }
            }
            else
            {
                DeleteRoutine((_aoiDefinition as AoiDefinition)?.Routines["EnableInFalse"]);
            }
        }

        public RelayCommand GoToCommand3 { set; get; }

        public bool CanGoToCommand3()
        {
            return _canGoToCommand3;
        }

        public void ExecuteGoToCommand3()
        {
            OpenRoutine((_aoiDefinition as AoiDefinition)?.Routines["EnableInFalse"]);
        }

        public string ButtonName3 { set; get; } 

        #endregion

        public object Owner { get; set; }
        public object Control { get; }

        public void LoadOptions()
        {

        }

        public bool SaveOptions()
        {
            return true;
        }

        public bool IsContain(string name)
        {
            foreach (var routine in _aoiDefinition.Routines)
            {
                if (routine.Name == name) return true;
            }

            return false;
        }

        public bool IsDirty
        {
            set
            {
                Set(ref _isDirty, value);
                IsDirtyChanged?.Invoke(this, new EventArgs());
            }
            get { return _isDirty; }
        }

        public event EventHandler IsDirtyChanged;

        private void OpenRoutine(IRoutine r)
        {
            var service = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
            if (r is RLLRoutine)
            {
                service?.CreateRLLEditor(r);
            }
            else if (r is STRoutine)
            {
                service?.CreateSTEditor(r);
            }
            else if (r is FBDRoutine)
            {
                service?.CreateFBDEditor(r);
            }
        }

        private void DeleteRoutine(IRoutine routine)
        {
            if (MessageBox.Show($"Delete the {routine.Name} routine?", "ICS Studio", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var aoi = _aoiDefinition as AoiDefinition;
                (aoi?.Routines as RoutineCollection)?.DeleteRoutine(routine);
                //aoi?.Reset();
                //TODO(zyl):reset aoi instr
                if (routine.Name == "Prescan")
                {
                    ButtonName1 =_new;
                    CheckBoxEnable1 = false;
                    CheckBox1 = false;
                    _canGoToCommand1 = false;
                    GoToCommand1.RaiseCanExecuteChanged();
                    RaisePropertyChanged("ButtonName1");
                    RaisePropertyChanged("CheckBoxEnable1");
                    RaisePropertyChanged("CheckBox1");
                }
                else if (routine.Name == "Postscan")
                {
                    ButtonName2 = _new;
                    CheckBoxEnable2 = false;
                    CheckBox2 = false;
                    _canGoToCommand2 = false;
                    GoToCommand2.RaiseCanExecuteChanged();
                    RaisePropertyChanged("ButtonName2");
                    RaisePropertyChanged("CheckBoxEnable2");
                    RaisePropertyChanged("CheckBox2");
                }
                else if (routine.Name == "EnableInFalse")
                {
                    ButtonName3 = _new;
                    CheckBoxEnable3 = false;
                    CheckBox3 = false;
                    _canGoToCommand3 = false;
                    GoToCommand3.RaiseCanExecuteChanged();
                    RaisePropertyChanged("ButtonName3");
                    RaisePropertyChanged("CheckBoxEnable3");
                    RaisePropertyChanged("CheckBox3");
                }

                var service = Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                service?.CloseWindow(routine.Uid);
                var service2 = Package.GetGlobalService(typeof(SCreateDialogService)) as CreateDialogService;
                service2?.CloseDialog(new List<string>() {$"RoutineProperties{routine.Uid}"});
                //IStudioUIService studioUIService =
                //    Package.GetGlobalService(typeof(SStudioUIService)) as IStudioUIService;
                //studioUIService?.UpdateUI();
            }
        }
    }
}
