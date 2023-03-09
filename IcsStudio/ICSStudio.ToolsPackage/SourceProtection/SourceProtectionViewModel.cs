using System;
using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Aoi;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.SourceProtection;
using ICSStudio.MultiLanguage;

namespace ICSStudio.ToolsPackage.SourceProtection
{
    public class SourceProtectionViewModel : ViewModelBase
    {
        private readonly SourceProtectionManager _manager;

        private bool? _dialogResult;

        private List<SourceProtectionItem> _sourceProtections;

        private SourceProtectionItem _selectedItem;


        public SourceProtectionViewModel(SourceProtectionManager manager)
        {
            _manager = manager;
            _manager.Update();

            CloseCommand = new RelayCommand(ExecuteClose);
            ProtectCommand = new RelayCommand(ExecuteProtect, CanExecuteProtect);
            UnprotectCommand = new RelayCommand(ExecuteUnprotect, CanExecuteUnprotect);
            SourceKeyCfgCommand = new RelayCommand(ExecuteSourceKeyCfg);

            SelectedItemChangedCommand =
                new RelayCommand<RoutedPropertyChangedEventArgs<object>>(OnSelectedItemChanged);

            UpdateSourceProtections();
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(ref _dialogResult, value); }
        }

        public List<SourceProtectionItem> SourceProtections
        {
            get { return _sourceProtections; }
            set { Set(ref _sourceProtections, value); }
        }

        #region Command

        public RelayCommand CloseCommand { get; }
        public RelayCommand ProtectCommand { get; }
        public RelayCommand UnprotectCommand { get; }
        public RelayCommand SourceKeyCfgCommand { get; }

        public RelayCommand<RoutedPropertyChangedEventArgs<object>> SelectedItemChangedCommand { get; }

        private void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> args)
        {
            _selectedItem = args.NewValue as SourceProtectionItem;

            ProtectCommand.RaiseCanExecuteChanged();
            UnprotectCommand.RaiseCanExecuteChanged();
        }

        private void ExecuteSourceKeyCfg()
        {
            SourceKeyCfgDialog dialog = new SourceKeyCfgDialog
            {
                Owner = Application.Current.MainWindow
            };

            SourceKeyCfgViewModel viewModel = new SourceKeyCfgViewModel(_manager.Provider);
            dialog.DataContext = viewModel;

            dialog.ShowDialog();

            _manager.Update();
            UpdateSourceProtections();
        }

        private void ExecuteClose()
        {
            DialogResult = true;
        }

        private bool CanExecuteUnprotect()
        {
            if (_selectedItem?.Source != null)
                return true;

            return false;
        }

        private void ExecuteUnprotect()
        {
            //TODO(gjc): add code here

            var source = _selectedItem?.Source;
            if (source != null)
            {
                var permission = _manager.GetPermission(source);
                if (permission == SourcePermission.Use)
                {
                    ProtectWarningDialog warningDialog = new ProtectWarningDialog
                    {
                        DataContext = new ProtectWarningViewModel(new List<string>
                        {
                            $"{_selectedItem.FullName}: No permission to access or modify Source Protected object."
                        }),
                        Owner = Application.Current.MainWindow,
                    };

                    warningDialog.ShowDialog();
                }
                else if (permission == SourcePermission.All)
                {
                    if (_manager.Unprotect(source) == 0)
                    {
                        UpdateSourceProtections();
                    }
                }
            }
        }

        private bool CanExecuteProtect()
        {
            if (_selectedItem?.Source != null)
                return true;

            return false;
        }

        private void ExecuteProtect()
        {
            var source = _selectedItem?.Source;
            if (source != null)
            {
                ProtectViewModel viewModel = new ProtectViewModel(_manager);
                ProtectDialog dialog = new ProtectDialog
                {
                    Owner = Application.Current.MainWindow,
                    DataContext = viewModel
                };

                var dialogResult = dialog.ShowDialog();
                if (dialogResult.HasValue && dialogResult.Value)
                {
                    string sourceKey = viewModel.SourceKey;

                    var permission = _manager.GetPermission(source);

                    if (permission == SourcePermission.Use)
                    {
                        ProtectWarningDialog warningDialog = new ProtectWarningDialog
                        {
                            DataContext = new ProtectWarningViewModel(new List<string>
                            {
                                $"{_selectedItem.FullName}: No permission to access or modify Source Protected object."
                            }),
                            Owner = Application.Current.MainWindow,
                        };

                        warningDialog.ShowDialog();
                    }
                    else if (permission == SourcePermission.None ||
                             permission == SourcePermission.All)

                    {
                        //TODO(gjc): save and close routine windows

                        int result = _manager.Protect(source, sourceKey);

                        // update
                        if (result == 0)
                        {
                            UpdateSourceProtections();
                        }
                    }
                }
            }

        }

        #endregion


        private void UpdateSourceProtections()
        {
            var controller = _manager.Controller;

            List<ISourceProtected> unknownProtectionList = new List<ISourceProtected>();
            List<ISourceProtected> unprotectedList = new List<ISourceProtected>();
            Dictionary<string, List<ISourceProtected>> protectedDictionary =
                new Dictionary<string, List<ISourceProtected>>();

            // programs
            foreach (IProgram program in controller.Programs)
            {
                foreach (var routine in program.Routines)
                {
                    var permission = _manager.GetPermission(routine);

                    if (permission == SourcePermission.None)
                    {
                        unprotectedList.Add(routine);
                    }
                    else if (permission == SourcePermission.Use)
                    {
                        unknownProtectionList.Add(routine);
                    }
                    else if (permission == SourcePermission.All)
                    {
                        string sourceKey = _manager.GetKeyBySource(routine);

                        if (!protectedDictionary.ContainsKey(sourceKey))
                        {
                            protectedDictionary.Add(sourceKey, new List<ISourceProtected>());
                        }

                        var sourceProtectedList = protectedDictionary[sourceKey];

                        sourceProtectedList.Add(routine);
                    }
                    else
                    {
                        throw new NotImplementedException("UpdateSourceProtections");
                    }

                }
            }

            // aoi
            foreach (var aoiDefinition in controller.AOIDefinitionCollection)
            {
                var permission = _manager.GetPermission(aoiDefinition);

                if (permission == SourcePermission.None)
                {
                    unprotectedList.Add(aoiDefinition);
                }
                else if (permission == SourcePermission.Use)
                {
                    unknownProtectionList.Add(aoiDefinition);
                }
                else if (permission == SourcePermission.All)
                {
                    string sourceKey = _manager.GetKeyBySource(aoiDefinition);

                    if (!protectedDictionary.ContainsKey(sourceKey))
                    {
                        protectedDictionary.Add(sourceKey, new List<ISourceProtected>());
                    }

                    var sourceProtectedList = protectedDictionary[sourceKey];

                    sourceProtectedList.Add(aoiDefinition);
                }
                else
                {
                    throw new NotImplementedException("UpdateSourceProtections");
                }
            }

            List<SourceProtectionItem> sourceProtections = new List<SourceProtectionItem>();

            // create
            if (protectedDictionary.Keys.Count > 0)
            {
                foreach (var key in protectedDictionary.Keys)
                {
                    string displayName = _manager.GetDisplayNameByKey(key);

                    SourceProtectionItem rootItem = new SourceProtectionItem
                    {
                        FullName = displayName,
                        Sources = new List<SourceProtectionItem>(),
                        IsExpanded = true
                    };

                    sourceProtections.Add(rootItem);

                    var sourceProtectedList = protectedDictionary[key];

                    foreach (var source in sourceProtectedList)
                    {
                        var item = new SourceProtectionItem(source);
                        rootItem.Sources.Add(item);
                    }

                }

            }

            if (unknownProtectionList.Count > 0)
            {
                SourceProtectionItem unknownProtectionRootItem = new SourceProtectionItem
                {
                    FullName = LanguageManager.GetInstance().ConvertSpecifier("UnknownProtection"),
                    Sources = new List<SourceProtectionItem>(),
                    IsExpanded = true
                };

                sourceProtections.Add(unknownProtectionRootItem);

                foreach (var source in unknownProtectionList)
                {
                    var item = new SourceProtectionItem(source);

                    unknownProtectionRootItem.Sources.Add(item);
                }
            }

            if (unprotectedList.Count > 0)
            {
                SourceProtectionItem unprotectedRootItem = new SourceProtectionItem
                {
                    FullName = LanguageManager.GetInstance().ConvertSpecifier("SourceUnProtected"),
                    Sources = new List<SourceProtectionItem>(),
                    IsExpanded = true,
                };

                sourceProtections.Add(unprotectedRootItem);

                foreach (var source in unprotectedList)
                {
                    var item = new SourceProtectionItem(source);

                    unprotectedRootItem.Sources.Add(item);
                }

            }

            SourceProtections = sourceProtections;
        }
    }

    public class SourceProtectionItem
    {
        public SourceProtectionItem()
        {

        }

        public SourceProtectionItem(ISourceProtected source)
        {
            Source = source;

            IRoutine routine = source as IRoutine;
            if (routine != null)
            {
                FullName = routine.Name;
                Container = routine.ParentCollection.ParentProgram.Name;
                Vendor = string.Empty;
            }

            IAoiDefinition aoiDefinition = source as IAoiDefinition;
            if (aoiDefinition != null)
            {
                FullName = aoiDefinition.Name;
                Container = "UD Function Block";
                Vendor = string.Empty;
            }
        }

        public List<SourceProtectionItem> Sources { get; set; }

        public string FullName { get; set; }
        public string Container { get; set; }
        public string Vendor { get; set; }

        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }

        public ISourceProtected Source { get; }
    }
}
