using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Gui.Annotations;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Error;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.ErrorOutputPackage.View
{
    public enum ErrorListLevel
    {
        Error,
        Warning,
        Information
    }

    public class ErrorListDataModel : ViewModelBase
    {
        //        public ErrorListDataModel()
        //        {
        ////#if DEBUG
        ////            // Performance-Test
        ////            //for (int i = 0; i < 1000; i++)
        ////            //{
        ////            this.AddError("Error unable to do something \"Name: Write PHP\", because the syntax is so looooooooooong");
        ////            this.AddError("Error unable to do something \"Name: Write Flash\"");
        ////            this.AddWarning("Error unable to do something \"Name: Program in F#, yet\"");
        ////            this.AddInformation("Note: I need a better hobby than wasting my lunch coding..");
        ////            //}
        ////#endif
        //        }

        private readonly bool _isShowImage;
        public ErrorListDataModel(bool isShowImage)
        {
            _isShowImage = isShowImage;
            ClearCommand = new RelayCommand(CleanAll);
            _errors = LanguageManager.GetInstance().ConvertSpecifier("Errors");
            _warnings = LanguageManager.GetInstance().ConvertSpecifier("Warnings");
            _informations = LanguageManager.GetInstance().ConvertSpecifier("Information");
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(LanguageManager.GetInstance(), "LanguageChanged", LanguageChangedHandler);
        }

        private void LanguageChangedHandler(object sender, EventArgs e)
        {
            _errors = LanguageManager.GetInstance().ConvertSpecifier("Errors");
            _warnings = LanguageManager.GetInstance().ConvertSpecifier("Warnings");
            _informations = LanguageManager.GetInstance().ConvertSpecifier("Information");
            RaisePropertyChanged("ErrorsText");
            RaisePropertyChanged("WarningsText");
            RaisePropertyChanged("InformationsText");
        }

        public RelayCommand ClearCommand { get; }

        public string Search
        {
            set
            {
                _search = value;
                SetView();
            }
            get { return _search; }
        }

        //private string _errorsText;
        public string ErrorsText
        {
            get
            {
                return string.Format(_errorListData.Count(ed => ed.Level == ErrorListLevel.Error) + " " + _errors);
            }

        }

        public string WarningsText
        {
            get
            {
                return string.Format(_errorListData.Count(ed => ed.Level == ErrorListLevel.Warning) + " " + _warnings);
            }
        }

        public string InformationsText
        {
            get
            {
                return string.Format(_errorListData.Count(ed => ed.Level == ErrorListLevel.Information) + " " + _informations);
            }
        }

        public void AddError(string description, OrderType orderType, OnlineEditType onlineEditType, int? line = null, int? offset = null, int? len = null, object original = null)
        {
            if (description.EndsWith("\r"))
                description = description.Remove(description.Length - 1, 1);
            var existData = _errorListData.FirstOrDefault(d =>
                d.Level == ErrorListLevel.Error && d.Original == original &&
                d.Line == line && d.Offset == offset && line != null && description == d.Description);
            if (existData != null)
            {
                return;
            }

            var errorData = new ErrorListDataEntry(_isShowImage)
            {
                Description = description,
                Level = ErrorListLevel.Error,
                HyperLinkVisibility = Visibility.Visible,
                Original = original,
                Line = line,
                Offset = offset,
                Length = len,
                OnlineEditType = onlineEditType
            };
            if (_isShowImage)
            {
                var index = FindIndex(ErrorListLevel.Error, orderType, line, offset, original);
                if (index == -1)
                {
                    errorData.Index = _errorListData.Count;
                    _errorListData.Add(errorData);
                }
                else
                {
                    errorData.Index = index;
                    _errorListData.Insert(index, errorData);
                    IncreaseIndex(++index);
                }
            }
            else
            {
                var index = _errorListData.Count;
                errorData.Index = index;
                _errorListData.Add(errorData);
            }
            SetView();

            //dataGrid.ScrollIntoView(errorData);
        }

        private void IncreaseIndex(int offset)
        {
            for (int i = offset; i < _errorListData.Count; i++)
            {
                var data = _errorListData[i];
                data.Index = data.Index + 1;
            }
        }

        private int FindIndex(ErrorListLevel level, OrderType orderType, int? line, int? offset, object original)
        {
            if (line == null || offset == null) return -1;
            if (orderType == OrderType.Order)
            {
                var item = _errorListData.LastOrDefault(d =>
                    d.Level == level && d.Original == original && (d.Line < line || (d.Line == line && d.Offset <= offset)));
                if (item != null)
                {
                    return _errorListData.IndexOf(item) + 1;
                }
                else
                {
                    var defaultItem = _errorListData.FirstOrDefault(d => d.Level == level && d.Original == original);
                    if (defaultItem != null)
                    {
                        return _errorListData.IndexOf(defaultItem);
                    }
                }
            }
            else if (orderType == OrderType.OrderByDescending)
            {
                var item = _errorListData.FirstOrDefault(d =>
                    d.Level == level && d.Original == original &&
                    (d.Line > line || (d.Line == line && d.Offset >= offset)));
                if (item != null)
                {
                    return _errorListData.IndexOf(item) + 1;
                }
                else
                {
                    var defaultItem = _errorListData.FirstOrDefault(d => d.Level == level && d.Original == original);
                    if (defaultItem != null)
                    {
                        return _errorListData.IndexOf(defaultItem);
                    }
                }
            }
            return -1;
        }

        public void AddWarning(string description, object original = null, int? line = null, int? offset = null,Destination destination = Destination.None)
        {
            if (description.EndsWith("\r"))
                description = description.Remove(description.Length - 1, 1);
            var existData = _errorListData.FirstOrDefault(d =>
                d.Level == ErrorListLevel.Warning && d.Original == original && d.Description.Equals(description) && d.Line == line && d.Offset == offset);
            if (existData != null)
            {
                return;
            }
            _errorListData.Add(new ErrorListDataEntry(_isShowImage)
            {
                Description = description,
                Level = ErrorListLevel.Warning,
                HyperLinkVisibility = Visibility.Visible,
                Original = original,
                Destination = destination,
                Index = _errorListData.Count,
                Line = line, Offset = offset
            });
            SetView();
        }

        public void AddInformation(string description, object original = null)
        {
            if (description.EndsWith("\r"))
                description = description.Remove(description.Length - 1, 1);
            _errorListData.Add(new ErrorListDataEntry(_isShowImage)
            {
                Description = description,
                Level = ErrorListLevel.Information,
                HyperLinkVisibility = Visibility.Collapsed,
                Original = original,
                Index = _errorListData.Count
            });
            SetView();
        }

        public void Remove(ErrorListLevel level, object original)
        {
            var removeErrors =
                _errorListData.Where(d => d.Level == level && d.Original == original).ToList();
            foreach (var errorListDataEntry in removeErrors)
            {
                _errorListData.Remove(errorListDataEntry);
            }
            ReOrderErrorList();
            SetView();
        }

        private void ReOrderErrorList()
        {
            for (int i = 0; i < _errorListData.Count; i++)
            {
                var data = _errorListData[i];
                data.Index = i;
            }
        }

        public void Remove(ErrorListLevel level, IRoutine original, OnlineEditType onlineEditType)
        {
            var removeErrors =
                _errorListData.Where(d => d.Level == level && d.Original == original && d.OnlineEditType == onlineEditType).ToList();
            foreach (var errorListDataEntry in removeErrors)
            {
                _errorListData.Remove(errorListDataEntry);
            }
            ReOrderErrorList();
            SetView();
        }

        public void RemoveImportError()
        {
            var removeErrors = _errorListData.Where(d =>
                d.Level == ErrorListLevel.Error && d.Original is string &&
                (((string)d.Original).EndsWith(".csv", StringComparison.OrdinalIgnoreCase) ||
                 ((string)d.Original).EndsWith(".txt", StringComparison.OrdinalIgnoreCase))).ToList();
            foreach (var errorListDataEntry in removeErrors)
            {
                _errorListData.Remove(errorListDataEntry);
            }
            ReOrderErrorList();
            SetView();
        }

        public List<IRoutine> GetErrorRoutines()
        {
            var list = new List<IRoutine>();
            foreach (var program in Controller.GetInstance().Programs)
            {
                foreach (var routine in program.Routines)
                {
                    var stRoutine = routine as STRoutine;
                    if (stRoutine?.IsError ?? false)
                    {
                        list.Add(routine);
                    }
                }
            }
            foreach (var aoi in Controller.GetInstance().AOIDefinitionCollection)
            {
                foreach (var routine in aoi.Routines)
                {
                    var stRoutine = routine as STRoutine;
                    if (stRoutine?.IsError ?? false)
                    {
                        list.Add(routine);
                    }
                }
            }
            return list;
        }

        public void Summary()
        {
            var error = ErrorListData.Count(d => d.Level == ErrorListLevel.Error);
            var warn = ErrorListData.Count(d => d.Level == ErrorListLevel.Warning);
            AddInformation($"Complete - {error} error(s), {warn} warning(s)");
        }

        public ObservableCollection<ErrorListDataEntry> ErrorListData
        {
            get { return _errorListDataView; }
            internal set
            {
                _errorListData = value;
                SetView();
            }
        }

        public void CleanAll()
        {
            ErrorListData = new ObservableCollection<ErrorListDataEntry>();
        }

        public bool ShowErrors
        {
            set
            {
                _showErrors = value;
                SetView();
            }
        }

        public bool ShowWarnings
        {
            set
            {
                _showWarnings = value;
                SetView();
            }
        }

        public bool ShowInformations
        {
            set
            {
                _showInformations = value;
                SetView();
            }
        }

        private void SetView()
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                var selectedLevels = new List<ErrorListLevel>();
                if (!_isShowImage)
                {
                    selectedLevels.Add(ErrorListLevel.Error);
                    selectedLevels.Add(ErrorListLevel.Warning);
                    selectedLevels.Add(ErrorListLevel.Information);
                }
                else
                {
                    if (_showErrors)
                        selectedLevels.Add(ErrorListLevel.Error);
                    if (_showWarnings)
                        selectedLevels.Add(ErrorListLevel.Warning);
                    if (_showInformations)
                        selectedLevels.Add(ErrorListLevel.Information);
                }

                _errorListDataView.Clear();
                var selectedErrors = string.IsNullOrEmpty(Search)
                    ? _errorListData.Where(ed => selectedLevels.Contains(ed.Level))
                    : _errorListData.Where(ed =>
                        selectedLevels.Contains(ed.Level) && ed.Description.ToLower().Contains(Search.ToLower()));
                foreach (var selectedError in selectedErrors)
                    _errorListDataView.Add(selectedError);

                RaisePropertyChanged("ErrorsText");
                RaisePropertyChanged("WarningsText");
                RaisePropertyChanged("InformationsText");
            });
        }

        private ObservableCollection<ErrorListDataEntry>
            _errorListData = new ObservableCollection<ErrorListDataEntry>();

        private readonly ObservableCollection<ErrorListDataEntry> _errorListDataView =
            new ObservableCollection<ErrorListDataEntry>();

        private bool _showErrors = true;
        private bool _showWarnings = true;
        private bool _showInformations = true;
        //private Visibility _topBarVisibility;
        private string _search;
        private string _errors;
        private string _warnings;
        private string _informations;
        //private string _warningsText;
        //private string _informationsText;
    }

    public class ErrorListDataEntry : INotifyPropertyChanged
    {
        public ErrorListDataEntry(bool isShowImage)
        {
            ImageVisibility = isShowImage ? Visibility.Visible : Visibility.Collapsed;
        }

        public int Index
        {
            set
            {
                _index = value;
                OnPropertyChanged("IndexDisplay");
            }
            get { return _index; }
        }

        public string IndexDisplay => $"【{Index}】";

        public object Original { set; get; }
        public Destination Destination { get; set; } = Destination.None;
        public string Description { get; set; }
        public int? Line { set; get; }
        public int? Offset { set; get; }
        public int? Length { set; get; }

        public OnlineEditType OnlineEditType { get; set; }
        public ErrorListLevel Level
        {
            get { return _level; }
            set
            {
                _level = value;
                switch (_level)
                {
                    case ErrorListLevel.Error:
                        this.ErrorIconSrc = ErrorIconRelPath;
                        break;
                    case ErrorListLevel.Warning:
                        this.ErrorIconSrc = WarningIconRelPath;
                        break;
                    case ErrorListLevel.Information:
                        this.ErrorIconSrc = InformationIconRelPath;
                        break;
                }
            }
        }

        public Visibility ImageVisibility { set; get; }

        public Visibility HyperLinkVisibility { set; get; }

        public Visibility NormalVisibility =>
            HyperLinkVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        public string ErrorIconSrc { get; private set; }

        private ErrorListLevel _level = ErrorListLevel.Error;
        private int _index;

        private const string ErrorIconRelPath = "../Resources/Images/Error.png";
        private const string WarningIconRelPath = "../Resources/Images/Warning.png";
        private const string InformationIconRelPath = "../Resources/Images/Information.png";
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
