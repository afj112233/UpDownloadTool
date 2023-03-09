using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Dialogs.GlobalSetting;
using ICSStudio.Dialogs.WaitDialog;
using ICSStudio.EditorPackage.STEditor;
using ICSStudio.Gui.Annotations;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using ICSStudio.MultiLanguage;
using ICSStudio.SimpleServices.Common;
using ICSStudio.UIInterfaces.Editor;
using ICSStudio.UIInterfaces.Error;
using ICSStudio.UIInterfaces.Search;
using ICSStudio.UIServicesPackage.View;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Program = ICSStudio.SimpleServices.Common.Program;
using Thread = System.Threading.Thread;

namespace ICSStudio.UIServicesPackage.ViewModel
{
    public class FindInRoutinesVM : ViewModelBase
    {
        private string _title;
        private string _findWithIn;
        private string _findWithinText;
        private string _findWhat;
        private Component _selectedComponent;
        private ISearchResultService _searchResultService;
        private Controller _controller;
        private Action _closeAction;
        private Thread _own = Thread.CurrentThread;

        public FindInRoutinesVM(Action close, bool isFindPrevious, string searchText, bool isShowFindDialog)
        {
            _closeAction = close;
            _controller = Controller.GetInstance();
            EscCommand = new RelayCommand(ExecuteEscCommand);
            SearchInBrowseCommand = new RelayCommand<Button>(ExecuteSearchInBrowseCommand);
            FindNextCommand = new RelayCommand(ExecuteFindNextCommand, CanExecuteFindCommand);
            FindAllCommand = new RelayCommand(ExecuteFindAllCommand, CanExecuteFindCommand);
            ReplaceCommand = new RelayCommand(ExecuteReplaceCommand, CanExecuteReplaceCommand);
            ReplaceAllCommand = new RelayCommand(ExecuteReplaceAllCommand, CanExecuteReplaceCommand);
            CloseCommand = new RelayCommand(ExecuteCloseCommand);
            FindWithinTextCommand = new RelayCommand(ExecuteFindWithinTextCommand);

            FindWithinText = LanguageManager.GetInstance().ConvertSpecifier("Find Within GtString");
            LimitCollection.Add(LanguageManager.GetInstance().ConvertSpecifier("Text Only"));
            LimitCollection.Add(LanguageManager.GetInstance().ConvertSpecifier("References to Exact Tag and its Members"));
            LimitCollection.Add(LanguageManager.GetInstance().ConvertSpecifier("References to Tag"));
            LimitCollection.Add(LanguageManager.GetInstance().ConvertSpecifier("Forces"));
            LimitCollection.Add(LanguageManager.GetInstance().ConvertSpecifier("Edit Zones"));
            LimitCollection.Add(LanguageManager.GetInstance().ConvertSpecifier("Active SFC Steps and Actions"));
            LimitCollection.Add(LanguageManager.GetInstance().ConvertSpecifier("Shorted Branches"));
            SelectedLimit = LimitCollection[0];

            FindWhereCollection = EnumHelper.ToDataSource<FindInRoutineSetting.FindWhereType>();
            SelectedFindWhere = GlobalSetting.GetInstance().FindInRoutineSetting.FindWhere;
            ReplaceEnable = !_controller.IsOnline;

            #region Components

            {
                var es = new Component();
                Components.Add(es);
                es.PropertyChanged += Component_PropertyChanged;
                es.Name = "Equipment Sequences";
                var option1 = new Option(es);
                option1.Name = "Element Names";
                option1 = new Option(es);
                option1.Name = "Tag Descriptions";
                option1 = new Option(es);
                option1.Name = "Tag Names";
                option1 = new Option(es);
                option1.Name = "Tag Parameter Expressions";
                option1 = new Option(es);
                option1.Name = "Tag Types";
                option1 = new Option(es);
                option1.Name = "Transition Conditions";
            }
            {
                var fbd = new Component();
                Components.Add(fbd);
                fbd.PropertyChanged += Component_PropertyChanged;
                fbd.Name = "Function Block Diagrams";
                var option = new Option(fbd);
                option.Name = "Descriptions";
                option = new Option(fbd);
                option.Name = "Element Contents";
                option = new Option(fbd);
                option.Name = "Element Types";
                option = new Option(fbd);
                option.Name = "TextBox Contents";
            }
            {
                var ld = new Component();
                Components.Add(ld);
                ld.PropertyChanged += Component_PropertyChanged;
                ld.Name = "Ladder Diagrams";
                var option = new Option(ld);
                option.Name = "Instruction Main Operand Comments";
                option = new Option(ld);
                option.Name = "Instruction Operands";
                option = new Option(ld);
                option.Name = "Instructions";
                option = new Option(ld);
                option.Name = "Rung Comments";
                option = new Option(ld);
                option.Name = "Rung Types";
            }
            {
                var sfc = new Component();
                Components.Add(sfc);
                sfc.PropertyChanged += Component_PropertyChanged;
                sfc.Name = "Sequential Function Charts";
                var option = new Option(sfc);
                option.Name = "Descriptions";
                option = new Option(sfc);
                option.Name = "Element Names";
                option = new Option(sfc);
                option.Name = "Entire Routine Except Descriptions and Text Boxes";
                option = new Option(sfc);
                option.Name = "Text Boxes";
            }
            {
                var st = new Component();
                Components.Add(st);
                st.PropertyChanged += Component_PropertyChanged;
                st.Name = "Structured Text";
                var option = new Option(st);
                option.Name = "Comments";
                option.IsChecked = true;
                option = new Option(st);
                option.Name = "Statements";
                option.IsChecked = true;
                SelectedComponent = st;
            }

            #endregion

            if (searchText != null) FindWhat = searchText;

            if (_controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    (Controller)_controller, "IsOnlineChanged", OnIsOnlineChanged);
            }

            _searchResultService =
                Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SSearchResultService)) as
                    ISearchResultService;

            //打开Find查找窗口，并执行向前查找
            if (isFindPrevious && isShowFindDialog)
            {
                IsUp = true;
                IsDown = false;
            }

            switch (isFindPrevious)
            {
                //不打开查找窗口，向前查找
                case true:
                    if (!isShowFindDialog)
                    {
                        IsUp = true;
                        IsDown = false;
                        ExecuteFindNextCommand();
                    }
                    break;

                //不打开查找窗口，向后查找
                case false:
                    if (!isShowFindDialog)
                    {
                        ExecuteFindNextCommand();
                    }
                    break;
                default:
                    Debug.WriteLine("Failed to execute the search window");
                    return;
            }

            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged(nameof(FindWithIn));
            RaisePropertyChanged(nameof(FindWithinText));
        }

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                ReplaceEnable = !_controller.IsOnline;
                ReplaceCommand?.RaiseCanExecuteChanged();
            });
        }

        public override void Cleanup()
        {
            WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                _controller, "IsOnlineChanged", OnIsOnlineChanged);

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public string Title
        {
            get { return LanguageManager.GetInstance().ConvertSpecifier("Find in Routines"); }
            set { Set(ref _title, value); }
        }

        public bool ReplaceEnable
        {
            set { Set(ref _replaceEnable, value); }
            get { return _replaceEnable; }
        }

        public string ReplaceWith
        {
            set { Set(ref _replaceWith, value); }
            get { return _replaceWith ?? ""; }
        }

        private void Component_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GetFindWithIn();
        }

        public List<Component> Components { get; } = new List<Component>();

        public Component SelectedComponent
        {
            set
            {
                _selectedComponent = value;
                Options.Clear();
                foreach (var option in _selectedComponent.Options)
                {
                    Options.Add(option);
                }
            }
            get { return _selectedComponent; }
        }

        public ObservableCollection<Option> Options { get; } = new ObservableCollection<Option>();

        public RelayCommand<Button> SearchInBrowseCommand { set; get; }

        private void ExecuteSearchInBrowseCommand(Button button)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell;
            var dialog = new SearchItemBrowser();
            if (dialog.ShowDialog(uiShell) ?? false)
            {
                if (button?.Name == LanguageManager.GetInstance().ConvertSpecifier("Find"))
                {
                    FindWhat = ((SearchItemBrowserVM)dialog.DataContext).SelectedItem?.Name;
                }
                else
                {
                    ReplaceWith = ((SearchItemBrowserVM)dialog.DataContext).SelectedItem?.Name;
                }
            }
        }

        public string FindWhat
        {
            set
            {
                Set(ref _findWhat, value);
                FindNextCommand.RaiseCanExecuteChanged();
                FindAllCommand.RaiseCanExecuteChanged();
                ReplaceCommand.RaiseCanExecuteChanged();
                ReplaceAllCommand.RaiseCanExecuteChanged();
            }
            get { return _findWhat; }
        }

        private List<string> _limitCollection = new List<string>();

        public List<string> LimitCollection
        {
            set
            {
                Set(ref _limitCollection, value);
            }
            get
            {
                return _limitCollection;
            }
        }

        public string SelectedLimit { set; get; }

        public IList FindWhereCollection { get; }

        public FindInRoutineSetting.FindWhereType SelectedFindWhere
        {
            set
            {
                if (_selectedFindWhere != value)
                {
                    _selectedFindWhere = value;
                    GlobalSetting.GetInstance().FindInRoutineSetting.FindWhere = value;
                }
            }
            get { return _selectedFindWhere; }
        }

        /// <summary>
        /// 根据光标位置开始查找内容
        /// </summary>
        public bool IsWrap { set; get; } = true;

        public bool IsUp { set; get; }

        public bool IsDown { set; get; } = true;

        public bool IsMatchWholeWordOnly { set; get; }

        public bool IsUseWildcards { set; get; }

        public string FindWithIn
        {
            set { Set(ref _findWithIn, value); }
            get { return LanguageManager.GetInstance().ConvertSpecifier(_findWithIn); }
        }

        public string FindWithinText
        {
            set { Set(ref _findWithinText, value); }
            get
            {
                if (_findWithinText.Equals("查找范围>>") || _findWithinText.Equals("Find Within>>"))
                {
                    return LanguageManager.GetInstance().ConvertSpecifier("Find Within GtString");
                }

                return LanguageManager.GetInstance().ConvertSpecifier("LtString Find Within");
            }
        }

        public void GetFindWithIn()
        {
            var str = "";
            foreach (var component in Components)
            {
                if (component.Options.Any(o => o.IsChecked))
                    str = $"{str},{component.Name}";
            }

            if (str.Length > 0)
                str = str.Substring(1);
            FindWithIn = str;
        }

        public RelayCommand EscCommand { get; }

        private void ExecuteEscCommand()
        {
            _closeAction.Invoke();
        }

        public RelayCommand FindNextCommand { get; }
        private STRoutine _startRoutine;
        private bool _isFindNextMode = false;

        private void ExecuteFindNextCommand()
        {
            var searchResultService =
                Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SSearchResultService)) as
                    ISearchResultService;
            searchResultService?.Clean();
            _startRoutine = GetInitialRoutine() as STRoutine;
            if (_startRoutine == null)
                return;
            _isFindNextMode = true;
            _needQuitSearch = false;
            _searchCount = 0;
            FindNextRoutine(_startRoutine);
            _isFindNextMode = false;
        }

        private bool CanExecuteFindCommand()
        {
            return !string.IsNullOrEmpty(FindWhat);
        }
        private bool CanExecuteReplaceCommand()
        {
            if (_controller.IsOnline) return false;
            return !string.IsNullOrEmpty(FindWhat);
        }
        public RelayCommand FindAllCommand { get; }

        private int _occurence;
        private int _searchCount;

        private void ExecuteFindAllCommand()
        {
            if (string.IsNullOrEmpty(FindWhat)) return;
            _searchResultService.Clean();
            _occurence = 0;
            _searchCount = 0;
            var findAllWorker = new BackgroundWorker();
            findAllWorker.WorkerSupportsCancellation = true;
            var message = new FindingMessage();
            var findingDialog = new FindingDialog();
            var vm = new FindingDialogVM(message);
            findingDialog.DataContext = vm;
            findAllWorker.DoWork += (s, e) =>
            {
                //TODO(zyl):add other routine
                _searchResultService?.AddInfo(LanguageManager.GetInstance().ConvertSpecifier("Searching for ") + ($"\"{FindWhat}\" ..."));
                message.Message = LanguageManager.GetInstance().ConvertSpecifier("Searching for") + ($"\"{FindWhat}\"...");
                if (SelectedFindWhere == FindInRoutineSetting.FindWhereType.Current)
                {
                    var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                    var st = stEditorViewModel?.Routine;
                    if (st != null)
                        FindRoutine(st);
                }
                else if (SelectedFindWhere == FindInRoutineSetting.FindWhereType.All)
                {
                    foreach (var program in _controller.Programs)
                    {
                        if (findAllWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        foreach (var routine in program.Routines)
                        {
                            if (findAllWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }

                            var st = routine as STRoutine;
                            if (st != null)
                            {
                                FindRoutine(st, message);
                                continue;
                            }
                        }
                    }

                    foreach (var aoi in _controller.AOIDefinitionCollection)
                    {
                        if (findAllWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        foreach (var routine in aoi.Routines)
                        {
                            if (findAllWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }

                            var st = routine as STRoutine;
                            if (st != null)
                            {
                                FindRoutine(st, message);
                                continue;
                            }
                        }
                    }
                }
                else if (SelectedFindWhere == FindInRoutineSetting.FindWhereType.InCurrentProgram)
                {
                    var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                    var st = stEditorViewModel?.Routine as STRoutine;
                    if (st != null)
                    {
                        FindRoutine(st);
                        var program = st.ParentCollection.ParentProgram;
                        var routines = program.Routines.OrderByDescending(r => r.Name == program.MainRoutineName)
                            .ThenByDescending(r => r.Name == program.FaultRoutineName).ToList();
                        foreach (var routine in routines)
                        {
                            if (findAllWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }

                            if (routine == st) continue;
                            if (routine is STRoutine)
                            {
                                FindRoutine((STRoutine)routine, message);
                            }
                        }
                    }

                }
                else if (SelectedFindWhere == FindInRoutineSetting.FindWhereType.InCurrentTask)
                {
                    var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                    var st = stEditorViewModel?.Routine as STRoutine;
                    if (st != null)
                    {
                        var program = st.ParentCollection.ParentProgram as Program;
                        if (program != null)
                        {
                            foreach (var controllerProgram in _controller.Programs)
                            {
                                if (controllerProgram.ParentTask == program.ParentTask)
                                {
                                    foreach (var routine in controllerProgram.Routines)
                                    {
                                        if (findAllWorker.CancellationPending)
                                        {
                                            e.Cancel = true;
                                            return;
                                        }

                                        if (routine is STRoutine)
                                        {
                                            FindRoutine((STRoutine)routine, message);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                _searchResultService?.AddInfo(
                    $"Complete - {_occurence} occurence(s) found - {_searchCount} routine(s) searched");
                ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    findingDialog.DialogResult = true;
                });
            };
            findAllWorker.RunWorkerAsync();

            findingDialog.Owner = Application.Current.MainWindow;
            findingDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (!(findingDialog.ShowDialog() ?? false))
                findAllWorker.CancelAsync();
        }

        private readonly string[] _keyword = new[]
        {
            "if", "end_if", "then", "elsif", "else", "case", "of", "end_case", "for", "to", "by", "do", "end_for",
            "while", "end_while", "not", "mod", "and", "xor", "or", "repeat", "until", "end_repeat", "goto", "return",
            "exit", "#region", "#endregion"
        };

        private string _replaceWith;
        private bool _replaceEnable = true;

        private void FindRoutine(STRoutine routine, FindingMessage message = null)
        {
            _searchCount++;
            _searchResultService?.AddInfo(
                $"Searching through {routine.ParentCollection.ParentProgram.Name} - {routine.Name}");
            if (message != null)
            {
                message.Message = $"Searching through {routine.ParentCollection.ParentProgram.Name} - {routine.Name}";
            }

            List<List<string>> codeList = new List<List<string>>();

            if (routine.PendingEditsExist)
            {
                codeList.Add(GetCurrentCodeTextList(routine, OnlineEditType.Pending));
            }
            codeList.Add(GetCurrentCodeTextList(routine, OnlineEditType.Original));

            foreach (var codeText in codeList)
            {
                var code = ConvertCode(routine, string.Join("\n", codeText));
                int forceIndex = IsUp ? code.Length - 1 : 0;
                //if (!IsWrap)
                //{
                //    var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                //    if (routine == stEditorViewModel.Routine)
                //    {
                //        forceIndex = stEditorViewModel.FocusedView.TextEditor.CaretOffset - 1;
                //        if (IsUp)
                //            forceIndex -= stEditorViewModel.FocusedView.TextEditor.SelectionLength;
                //        else
                //        {
                //            forceIndex++;
                //        }
                //    }
                //}

                if (!IsMatchWholeWordOnly)
                {
                    var index = forceIndex;
                    while (index != -1)
                    {
                        index = IsDown
                            ? code.IndexOf(FindWhat, index, StringComparison.OrdinalIgnoreCase)
                            : code.LastIndexOf(FindWhat, index, 1 + index, StringComparison.OrdinalIgnoreCase);
                        if (index > -1)
                        {
                            var position = GetLineOffsetByIndex(code, index);
                            _occurence++;
                            _searchResultService?.AddFound(
                                $"Found:{(codeList.Count > 1 ? (codeList.IndexOf(codeText) == 1 ? "Pending Edits View, " : "Original View, ") : "")} Line {position.Item1}:{codeText[position.Item1 - 1].Replace("\r", "")}", OrderType.None,
                                routine, routine.CurrentOnlineEditType, position.Item1 - 1, position.Item2, FindWhat.Length);
                            if (IsDown) index = index + FindWhat.Length;
                            else index = index - 1;
                            continue;
                        }

                        break;
                    }
                }
                else
                {
                    if (IsDown)
                    {
                        var regex = new Regex($@"\b{FindWhat}\b", RegexOptions.IgnoreCase);
                        var matches = regex.Matches(code, forceIndex);
                        foreach (Match match in matches)
                        {
                            var position = GetLineOffsetByIndex(code, match.Index);
                            _occurence++;
                            _searchResultService?.AddFound(
                                $"Found:{(codeList.Count > 1 ? (codeList.IndexOf(codeText) == 1 ? "Pending Edits View, " : "Original View, ") : "")} Line {position.Item1}:{codeText[position.Item1 - 1].Replace("\r", "")}", OrderType.None,
                                routine, routine.CurrentOnlineEditType, position.Item1 - 1, position.Item2);
                        }
                    }
                    else
                    {
                        var regex = new Regex($@"\b{FindWhat}\b", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                        var matches = regex.Matches(code);
                        for (int i = matches.Count - 1; i >= 0; i--)
                        {
                            var match = matches[i];
                            if (match.Index > forceIndex) break;
                            var position = GetLineOffsetByIndex(code, match.Index);
                            _occurence++;
                            _searchResultService?.AddFound(
                                $"Found:{(codeList.Count > 1 ? (codeList.IndexOf(codeText) == 1 ? "Pending Edits View, " : "Original View, ") : "")} Line {position.Item1}:{codeText[position.Item1 - 1].Replace("\r", "")}", OrderType.None,
                                routine, routine.CurrentOnlineEditType, position.Item1 - 1, position.Item2);
                        }
                    }
                }
            }
        }

        private void ReplaceRoutine(STRoutine routine, FindingMessage message)
        {
            _searchCount++;
            _searchResultService?.AddInfo(
                $"Searching through {routine.ParentCollection.ParentProgram.Name} - {routine.Name}");
            if (message != null)
            {
                message.Message = $"Searching through {routine.ParentCollection.ParentProgram.Name} - {routine.Name}";
            }

            //var code = GetCurrentCodeText(routine);
            //if (!IsWrap)
            //{
            //    var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
            //    if (routine == stEditorViewModel.Routine)
            //    {
            //        forceIndex = stEditorViewModel.FocusedView.TextEditor.CaretOffset;
            //        if (IsUp)
            //            forceIndex -= stEditorViewModel.FocusedView.TextEditor.SelectionLength;
            //        else
            //        {
            //            forceIndex++;
            //        }
            //    }
            //}
            List<List<string>> codeList = new List<List<string>>();

            if (routine.PendingEditsExist)
            {
                codeList.Add(GetCurrentCodeTextList(routine, OnlineEditType.Pending));
            }
            codeList.Add(GetCurrentCodeTextList(routine, OnlineEditType.Original));
            foreach (var codeText in codeList)
            {
                var original = string.Join("\n", codeText);
                var code = ConvertCode(routine, original);
                int forceIndex = IsUp ? code.Length - 1 : 0;
                if (!IsMatchWholeWordOnly)
                {
                    var index = forceIndex;
                    bool isReplaced = false;
                    while (index != -1)
                    {
                        index = IsDown
                            ? code.IndexOf(FindWhat, index, StringComparison.OrdinalIgnoreCase)
                            : code.LastIndexOf(FindWhat, index, 1 + index, StringComparison.OrdinalIgnoreCase);
                        if (index > -1)
                        {
                            var position = GetLineOffsetByIndex(code, index);
                            _occurence++;
                            if (codeList.IndexOf(codeText) == 0 && codeList.Count > 1)
                            {
                                _searchResultService?.AddFound(
                                    $"Replace Error:Original View, Line {position.Item1}:{codeText[position.Item1 - 1].Replace("\r", "")} then:Cannot replace element in (Original View)", IsDown ? OrderType.Order : OrderType.OrderByDescending,
                                    routine, routine.CurrentOnlineEditType, position.Item1 - 1, position.Item2);
                                if (IsDown) index = index + FindWhat.Length;
                                else index = index - 1;
                                continue;
                            }
                            else
                            {
                                isReplaced = true;
                                original = original.Remove(index, FindWhat.Length);
                                code = code.Remove(index, FindWhat.Length);
                                original = original.Insert(index, ReplaceWith);
                                code = code.Insert(index, ReplaceWith);
                                _searchResultService?.AddFound(
                                    $"Replaced: Line {position.Item1}:{original.Split('\n')[position.Item1 - 1].Replace("\r", "")}", IsDown ? OrderType.Order : OrderType.OrderByDescending,
                                    routine, routine.CurrentOnlineEditType, position.Item1 - 1, position.Item2);
                                if (IsDown) index = index + ReplaceWith.Length;
                                else index = index - 1;
                                continue;
                            }
                        }

                        break;
                    }

                    if (isReplaced)
                    {
                        if (_own == Thread.CurrentThread)
                        {
                            SetCurrentCodeText(routine, original.Split('\n').ToList(), codeList.IndexOf(codeText) == 0 ? OnlineEditType.Original : OnlineEditType.Pending);
                        }
                        else
                        {
                            ThreadHelper.JoinableTaskFactory.Run(async delegate
                            {
                                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                                SetCurrentCodeText(routine, original.Split('\n').ToList(), codeList.IndexOf(codeText) == 0 ? OnlineEditType.Original : OnlineEditType.Pending);
                            });
                        }
                    }

                }
                else
                {

                    bool isReplaced = false;
                    if (IsDown)
                    {
                        var regex = new Regex($@"\b{FindWhat}\b", RegexOptions.IgnoreCase);
                        var matches = regex.Matches(code, forceIndex);

                        for (int i = matches.Count - 1; i >= 0; i--)
                        {
                            var match = matches[i];
                            var position = GetLineOffsetByIndex(code, match.Index);
                            _occurence++;
                            if (codeList.IndexOf(codeText) == 0 && codeList.Count > 1)
                            {
                                _searchResultService?.AddFound(
                                    $"Replace Error:Original View, Line {position.Item1}:{original.Split('\n')[position.Item1 - 1].Replace("\r", "")} then:Cannot replace element in (Original View)", OrderType.Order,
                                    routine, routine.CurrentOnlineEditType, position.Item1 - 1, position.Item2);
                            }
                            else
                            {
                                isReplaced = true;
                                original = original.Remove(match.Index, FindWhat.Length);
                                code = code.Remove(match.Index, FindWhat.Length);
                                original = original.Insert(match.Index, ReplaceWith);
                                code = code.Insert(match.Index, ReplaceWith);
                                _searchResultService?.AddFound(
                                    $"Replaced: Line {position.Item1}:{original.Split('\n')[position.Item1 - 1].Replace("\r", "")}", OrderType.Order,
                                    routine, routine.CurrentOnlineEditType, position.Item1 - 1, position.Item2);
                            }
                        }

                        if (isReplaced)
                        {
                            if (_own == Thread.CurrentThread)
                            {
                                SetCurrentCodeText(routine, original.Split('\n').ToList(), codeList.IndexOf(codeText) == 0 ? OnlineEditType.Original : OnlineEditType.Pending);
                            }
                            else
                            {
                                ThreadHelper.JoinableTaskFactory.Run(async delegate
                                {
                                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                                    SetCurrentCodeText(routine, original.Split('\n').ToList(), codeList.IndexOf(codeText) == 0 ? OnlineEditType.Original : OnlineEditType.Pending);
                                });
                            }
                        }
                    }
                    else
                    {
                        var regex = new Regex($@"\b{FindWhat}\b", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
                        var matches = regex.Matches(code);
                        for (int i = matches.Count - 1; i >= 0; i--)
                        {
                            var match = matches[i];
                            if (match.Index > forceIndex) break;
                            var position = GetLineOffsetByIndex(code, match.Index);
                            _occurence++;
                            if (codeList.IndexOf(codeText) == 0 && codeList.Count > 1)
                            {
                                _searchResultService?.AddFound(
                                    $"Replace Error:Original View, Line {position.Item1}:{original.Split('\n')[position.Item1 - 1].Replace("\r", "")} then:Cannot replace element in (Original View)", OrderType.Order,
                                    routine, routine.CurrentOnlineEditType, position.Item1 - 1, position.Item2);
                            }
                            else
                            {
                                isReplaced = true;
                                original = original.Remove(match.Index, FindWhat.Length);
                                code = code.Remove(match.Index, FindWhat.Length);
                                original = original.Insert(match.Index, ReplaceWith);
                                code = code.Insert(match.Index, ReplaceWith);
                                _searchResultService?.AddFound(
                                    $"Replaced: Line {position.Item1}:{original.Split('\n')[position.Item1 - 1].Replace("\r", "")}", OrderType.None,
                                    routine, routine.CurrentOnlineEditType, position.Item1 - 1, position.Item2);
                            }
                        }

                        if (isReplaced)
                        {
                            if (_own == Thread.CurrentThread)
                            {
                                SetCurrentCodeText(routine, original.Split('\n').ToList(), codeList.IndexOf(codeText) == 0 ? OnlineEditType.Original : OnlineEditType.Pending);
                            }
                            else
                            {
                                ThreadHelper.JoinableTaskFactory.Run(async delegate
                                {
                                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                                    SetCurrentCodeText(routine, original.Split('\n').ToList(), codeList.IndexOf(codeText) == 0 ? OnlineEditType.Original : OnlineEditType.Pending);
                                });
                            }
                        }
                    }
                }
            }

        }

        private string GetCurrentCodeText(IRoutine routine)
        {
            var st = routine as STRoutine;
            if (st != null)
            {
                return ConvertCode(st, string.Join("\n", GetCurrentCodeTextList(st, st.CurrentOnlineEditType)));
            }
            Debug.Assert(false);
            return "";
        }
        private List<string> GetCurrentCodeTextList(STRoutine st, OnlineEditType onlineEditType)
        {
            if (onlineEditType == OnlineEditType.Original)
                return st.CodeText;
            if (onlineEditType == OnlineEditType.Pending)
                return st.PendingCodeText;
            if (onlineEditType == OnlineEditType.Test)
                return st.TestCodeText;
            return st.CodeText;
        }
        private void SetCurrentCodeText(IRoutine routine, List<string> codeText, OnlineEditType onlineEditType)
        {
            var st = routine as STRoutine;
            if (st != null)
            {
                if (onlineEditType == OnlineEditType.Original)
                    st.CodeText = codeText;
                if (onlineEditType == OnlineEditType.Pending)
                    st.PendingCodeText = codeText;
                if (onlineEditType == OnlineEditType.Test)
                    st.TestCodeText = codeText;
                st.IsUpdateChanged = true;
                return;
            }

            Debug.Assert(false);
        }

        private string ConvertCode(IRoutine routine, string code)
        {
            if (routine is STRoutine)
            {
                var stOption = Components.Last();
                var isCheckComment = stOption.Options[0].IsChecked;
                var isCheckStatement = stOption.Options[1].IsChecked;
                if (isCheckStatement && isCheckComment)
                {
                    return code;
                }

                if (isCheckStatement)
                {
                    return RoutineCodeTextExtension.ConvertCommentToWhiteBlank(code, null, true);
                }

                if (isCheckComment)
                {
                    return RoutineCodeTextExtension.ConvertStatementToWhiteBlank(code);
                }

                return "";
            }

            return code;
        }

        private void FindNextRoutine(IRoutine routine)
        {
            var st = routine as STRoutine;
            if (st == null)
            {
                GoOnNext(routine);
                return;
            }

            int index = 0;
            var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
            var code = GetCurrentCodeText(routine);
            if (routine == stEditorViewModel?.Routine)
            {
                index = Math.Min(stEditorViewModel.FocusedView.TextEditor.CaretOffset - 1, code.Length - 1);
                if (IsUp)
                {
                    if (stEditorViewModel.FocusedView.TextEditor.SelectionLength > 0)
                        index -= stEditorViewModel.FocusedView.TextEditor.SelectionLength;
                }
                else
                {
                    index++;
                }
            }
            else
            {
                if (IsUp)
                    index = code.Length - 1;
            }

            index = Math.Max(index, 0);

            if (!IsMatchWholeWordOnly)
            {
                Debug.Assert(index != -1);
                try
                {
                    index = IsDown
                        ? code.IndexOf(FindWhat, index, StringComparison.OrdinalIgnoreCase)
                        : code.LastIndexOf(FindWhat, index, index + 1, StringComparison.OrdinalIgnoreCase);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                }

                if (index > -1)
                {
                    var position = GetLineOffsetByIndex(code, index);
                    if (_searchCount < 0)
                        _searchCount = 1;
                    else
                        _searchCount++;
                    var createEditorService =
                        Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;
                    createEditorService?.CreateSTEditor(routine, st.CurrentOnlineEditType, position.Item1 - 1,
                        position.Item2, FindWhat.Length);
                }
                else
                {
                    if (IsWrap)
                    {
                        _searchCount = code.IndexOf(FindWhat) > -1 ? 1 : 0;
                    }

                    GoOnNext(routine);
                }

                return;
            }
            else
            {
                var regex = new Regex($@"\b{FindWhat}\b", RegexOptions.IgnoreCase);

                Match match = null;
                if (IsDown)
                {
                    match = regex.Match(code, index);
                }
                else
                {
                    var matches = regex.Matches(code);
                    foreach (Match match1 in matches)
                    {
                        if (match1.Index < index)
                        {
                            match = match1;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (match?.Success ?? false)
                {
                    var position = GetLineOffsetByIndex(code, match.Index);
                    if (_searchCount < 0)
                        _searchCount = 1;
                    else
                        _searchCount++;
                    var createEditorService =
                        Package.GetGlobalService(typeof(SCreateEditorService)) as ICreateEditorService;

                    createEditorService?.CreateSTEditor(routine, st.CurrentOnlineEditType, position.Item1 - 1,
                        position.Item2, FindWhat.Length);
                }
                else
                {
                    if (IsWrap)
                    {
                        _searchCount = regex.IsMatch(code) ? 1 : 0;
                    }

                    GoOnNext(routine);
                }
            }
        }

        private void ResetCaret(IRoutine routine)
        {
            if (IsWrap)
            {
                if (_own != Thread.CurrentThread)
                    ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                        if (stEditorViewModel != null && stEditorViewModel.Routine == routine)
                        {
                            stEditorViewModel.FocusedView.TextEditor.SelectionLength = 0;
                            stEditorViewModel.FocusedView.TextEditor.CaretOffset =
                                IsDown ? 0 : stEditorViewModel.FocusedView.TextEditor.Text.Length;
                            stEditorViewModel.FocusedView.TextEditor.SelectionStart =
                                stEditorViewModel.FocusedView.TextEditor.CaretOffset;
                        }
                    });
                else
                {
                    var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                    if (stEditorViewModel != null && stEditorViewModel.Routine == routine)
                    {
                        stEditorViewModel.FocusedView.TextEditor.SelectionLength = 0;
                        stEditorViewModel.FocusedView.TextEditor.CaretOffset =
                            IsDown ? 0 : stEditorViewModel.FocusedView.TextEditor.Text.Length;
                        stEditorViewModel.FocusedView.TextEditor.SelectionStart =
                            stEditorViewModel.FocusedView.TextEditor.CaretOffset;
                    }
                }
            }
        }

        private bool _needQuitSearch = false;
        private FindInRoutineSetting.FindWhereType _selectedFindWhere;

        private void GoOnNext(IRoutine routine)
        {
            if (_isFindNextMode && _needQuitSearch && _searchCount == 0)
            {
                _needQuitSearch = false;
                return;
            }

            if (SelectedFindWhere == FindInRoutineSetting.FindWhereType.Current)
            {
                if (IsWrap)
                {
                    if (_searchCount == 0) return;
                    if (!_isFindNextMode)
                        _searchCount = 0;
                    ResetCaret(routine);
                    FindNextRoutine(routine);
                }
            }
            else if (SelectedFindWhere == FindInRoutineSetting.FindWhereType.All)
            {
                var nextRoutine = GetNextRoutine(routine, false, false);
                if (nextRoutine == null) return;
                if (!(nextRoutine is STRoutine))
                {
                    ResetCaret(routine);
                    GoOnNext(nextRoutine);
                    return;
                }

                if (!_isFindNextMode)
                {
                    if (nextRoutine == _startRoutine && _searchCount == 0)
                    {
                        return;
                    }

                    _searchCount = 0;
                }
                else
                {
                    if (nextRoutine == _startRoutine && _searchCount == 0)
                    {
                        _needQuitSearch = true;
                    }
                }

                ResetCaret(routine);
                FindNextRoutine(nextRoutine);
            }
            else if (SelectedFindWhere == FindInRoutineSetting.FindWhereType.InCurrentProgram)
            {
                var nextRoutine = GetNextRoutine(routine, true, false);
                if (nextRoutine == null) return;
                if (!(nextRoutine is STRoutine))
                {
                    ResetCaret(routine);
                    GoOnNext(nextRoutine);
                    return;
                }

                if (!_isFindNextMode)
                {
                    if (nextRoutine == _startRoutine && _searchCount == 0)
                    {
                        return;
                    }

                    _searchCount = 0;
                }
                else
                {
                    if (nextRoutine == _startRoutine && _searchCount == 0)
                    {
                        _needQuitSearch = true;
                    }
                }

                ResetCaret(routine);
                FindNextRoutine(nextRoutine);
            }
            else if (SelectedFindWhere == FindInRoutineSetting.FindWhereType.InCurrentTask)
            {
                var nextRoutine = GetNextRoutine(routine, false, true);
                if (nextRoutine == null) return;
                if (!(nextRoutine is STRoutine))
                {
                    ResetCaret(routine);
                    GoOnNext(nextRoutine);
                    return;
                }

                if (!_isFindNextMode)
                {
                    if (nextRoutine == _startRoutine && _searchCount == 0)
                    {
                        return;
                    }

                    _searchCount = 0;
                }
                else
                {
                    if (nextRoutine == _startRoutine && _searchCount == 0)
                    {
                        _needQuitSearch = true;
                    }
                }

                ResetCaret(routine);
                FindNextRoutine(nextRoutine);
            }
        }

        private IRoutine GetNextRoutine(IRoutine routine, bool isInSameProgram, bool isInSameTask)
        {
            var st = routine as STRoutine;
            if (st == null)
            {
                var routinesList = routine.ParentCollection.ParentProgram.Routines.OrderByDescending(r => r.Name == routine.ParentCollection.ParentProgram.MainRoutineName)
                    .ThenByDescending(r => r.Name == routine.ParentCollection.ParentProgram.FaultRoutineName).ToList();
                var offset = routinesList.IndexOf(routine) + (IsDown ? 1 : -1);
                if (offset < routine.ParentCollection.ParentProgram.Routines.Count && offset > -1)
                {
                    var routine2 = routinesList[offset];
                    return SetOnlineEdit(routine2);
                }
                else
                {
                    var p = GetNextProgram(routine);
                    var routines2 = p.Routines.OrderByDescending(r => r.Name == p.MainRoutineName)
                        .ThenByDescending(r => r.Name == p.FaultRoutineName).ToList();
                    return SetOnlineEdit(IsDown ? routines2.First() : routines2.Last());
                }

            }

            if (IsDown)
            {
                if (st.CurrentOnlineEditType == OnlineEditType.Original)
                {
                    if (st.PendingCodeText != null)
                    {
                        var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                        if (stEditorViewModel?.Routine == routine)
                        {
                            stEditorViewModel.Options.ShowPending = true;
                        }
                        else
                        {
                            st.CurrentOnlineEditType = OnlineEditType.Pending;
                        }

                        return routine;
                    }

                    if (st.TestCodeText != null)
                    {
                        var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                        if (stEditorViewModel?.Routine == routine)
                        {
                            stEditorViewModel.Options.ShowTest = true;
                        }
                        else
                        {
                            st.CurrentOnlineEditType = OnlineEditType.Test;
                        }

                        return routine;
                    }
                }
                else if (st.CurrentOnlineEditType == OnlineEditType.Pending)
                {
                    if (st.TestCodeText != null)
                    {
                        var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                        if (stEditorViewModel?.Routine == routine)
                        {
                            stEditorViewModel.Options.ShowTest = true;
                        }
                        else
                        {
                            st.CurrentOnlineEditType = OnlineEditType.Test;
                        }

                        return routine;
                    }
                }
            }
            else
            {
                if (st.CurrentOnlineEditType == OnlineEditType.Test)
                {
                    var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                    if (st.PendingCodeText != null)
                    {
                        if (stEditorViewModel?.Routine == routine)
                        {
                            stEditorViewModel.Options.ShowPending = true;
                        }
                        else
                        {
                            st.CurrentOnlineEditType = OnlineEditType.Pending;
                        }

                        return routine;
                    }

                    if (stEditorViewModel?.Routine == routine)
                    {
                        stEditorViewModel.Options.ShowPending = true;
                    }
                    else
                    {
                        st.CurrentOnlineEditType = OnlineEditType.Pending;
                    }

                    return routine;
                }
                else if (st.CurrentOnlineEditType == OnlineEditType.Pending)
                {
                    var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                    if (stEditorViewModel?.Routine == routine)
                    {
                        stEditorViewModel.Options.ShowOriginal = true;
                    }
                    else
                    {
                        st.CurrentOnlineEditType = OnlineEditType.Original;
                    }
                    return routine;
                }
            }

            var program = routine.ParentCollection.ParentProgram;
            var routines = program.Routines.OrderByDescending(r => r.Name == program.MainRoutineName)
                .ThenByDescending(r => r.Name == program.FaultRoutineName).ToList();
            var index = routines.IndexOf(routine);
            if (IsDown)
            {
                var next = index + 1;
                if (next < routines.Count) return SetOnlineEdit(routines[next]);
                if (isInSameProgram)
                {
                    if (IsWrap)
                    {
                        return SetOnlineEdit(routines.First());
                    }

                    return null;
                }

            }
            else
            {
                var forward = index - 1;
                if (forward > -1) return SetOnlineEdit(routines[forward]);
                if (isInSameProgram)
                {
                    if (IsWrap)
                    {
                        return SetOnlineEdit(routines.Last());
                    }

                    return null;
                }
            }

            var nextProgram = GetNextProgram(routine);
            if (isInSameTask)
            {
                var p = nextProgram as Program;
                if (p == null) return null;
                if (p.ParentTask != (routine.ParentCollection.ParentProgram as Program)?.ParentTask) return null;
            }

            if (nextProgram == null)
                return null;
            var routineList = nextProgram?.Routines.OrderByDescending(r => r.Name == nextProgram.MainRoutineName)
                .ThenByDescending(r => r.Name == nextProgram.FaultRoutineName).ToList();
            var res = IsDown ? routineList.First() : routineList.Last();
            return SetOnlineEdit(res);
        }

        private IRoutine SetOnlineEdit(IRoutine routine)
        {
            var st = routine as STRoutine;
            if (st != null)
            {
                if (st.CurrentOnlineEditType == OnlineEditType.Original)
                {
                    if (st.PendingEditsExist)
                        st.CurrentOnlineEditType = OnlineEditType.Pending;
                }
                else
                    st.CurrentOnlineEditType = OnlineEditType.Original;
            }

            return routine;
        }

        private IProgramModule GetNextProgram(IRoutine routine)
        {
            var parentProgram = routine?.ParentCollection.ParentProgram as Program;
            var programs =
                _controller.Programs.OrderBy(p => ((TaskCollection)_controller.Tasks).GetIndex(p.ParentTask)).ToList();
            if (routine == null)
            {
                if (IsDown)
                {
                    foreach (var program in programs)
                    {
                        if (program.Routines.Any())
                            return program;
                    }

                    foreach (var aoi in _controller.AOIDefinitionCollection)
                    {
                        if (aoi.Routines.Any())
                            return aoi;
                    }

                    return null;
                }
                else
                {
                    programs.Reverse();
                    foreach (var program in programs)
                    {
                        if (program.Routines.Any())
                            return program;
                    }

                    var aois = _controller.AOIDefinitionCollection.ToList();
                    aois.Reverse();
                    foreach (var aoi in aois)
                    {
                        if (aoi.Routines.Any())
                            return aoi;
                    }

                    return null;
                }

            }

            if (parentProgram != null)
            {
                var index = programs.IndexOf(parentProgram);
                if (IsDown)
                {
                    Start:
                    index++;
                    if (index < programs.Count)
                    {
                        if (programs[index].Routines.Any())
                            return programs[index];
                        goto Start;
                    }
                    else
                    {
                        if (_controller.AOIDefinitionCollection.Any())
                        {
                            return _controller.AOIDefinitionCollection.First();
                        }
                        if (IsWrap)
                        {
                            if (programs.Any())
                            {
                                var program = programs.First();
                                if (program.Routines.Any())
                                    return program;
                                index = 0;
                                goto Start;
                            }
                        }

                        return null;
                    }
                }
                else
                {
                    Start:
                    index--;
                    if (index < 0)
                    {
                        if (IsWrap)
                        {
                            if (_controller.AOIDefinitionCollection.Any())
                            {
                                return _controller.AOIDefinitionCollection.Last();
                            }
                            else
                            {
                                var program = programs.Last();
                                if (program.Routines.Any())
                                    return program;
                                index = programs.IndexOf(program);
                                goto Start;
                            }
                        }
                    }
                    else
                    {
                        if (programs[index].Routines.Any())
                            return programs[index];
                        goto Start;
                    }
                }
            }
            else
            {
                var aoiProgram = routine.ParentCollection.ParentProgram as AoiDefinition;
                var index = ((AoiDefinitionCollection)_controller.AOIDefinitionCollection).GetIndex(aoiProgram);
                if (IsDown)
                {
                    var next = index + 1;
                    if (next < _controller.AOIDefinitionCollection.Count())
                    {
                        return _controller.AOIDefinitionCollection.ToList()[next];
                    }
                    else
                    {
                        if (IsWrap)
                        {
                            foreach (var program in programs)
                            {
                                if (program.Routines.Any())
                                    return program;
                            }
                        }

                        return null;
                    }
                }
                else
                {
                    var forward = index - 1;
                    if (forward < 0)
                    {
                        programs.Reverse();
                        foreach (var program in programs)
                        {
                            if (program.Routines.Any())
                                return program;
                        }
                    }
                    else
                    {
                        return _controller.AOIDefinitionCollection.ToList()[forward];
                    }
                }
            }

            return null;
        }

        private Tuple<int, int> GetLineOffsetByIndex(string code, int index)
        {
            var front = code.Substring(0, index);
            var l = front.Split('\n');
            return new Tuple<int, int>(l.Length, l.Last().Length);
        }

        public RelayCommand ReplaceCommand { get; }

        private void ExecuteReplaceCommand()
        {
            var searchResultService =
                Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SSearchResultService)) as
                    ISearchResultService;
            searchResultService?.Clean();
            _startRoutine = GetInitialRoutine() as STRoutine;
            if (_startRoutine == null)
                return;
            _searchCount = 0;
            ReplaceNext(_startRoutine);
        }

        private IRoutine GetInitialRoutine()
        {
            var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
            if (stEditorViewModel != null)
                return stEditorViewModel.Routine;
            MessageBox.Show(LanguageManager.GetInstance().ConvertSpecifier("Error: Find Where command mismatch")
                , "ICS Studio", MessageBoxButton.OK);
            return null;
            //return GetNextRoutine(null, false, false);
        }

        private bool CanReplace(STRoutine routine)
        {
            var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
            if (stEditorViewModel != null && stEditorViewModel.Routine == routine)
            {
                if (FindWhat.Equals(stEditorViewModel.FocusedView.TextEditor.SelectedText,
                        StringComparison.OrdinalIgnoreCase)) return true;
            }

            return false;
        }

        private void ReplaceNext(STRoutine routine)
        {
            var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
            var code = string.Join("\n", GetCurrentCodeTextList(routine, routine.CurrentOnlineEditType));
            if (CanReplace(routine))
            {
                var index = stEditorViewModel.FocusedView.TextEditor.CaretOffset - FindWhat.Length;
                code = code.Remove(index, FindWhat.Length);
                code = code.Insert(index, ReplaceWith);
                SetCurrentCodeText(routine, code.Split('\n').ToList(), routine.CurrentOnlineEditType);

                stEditorViewModel.FocusedView.TextEditor.CaretOffset = index + ReplaceWith.Length;
                if (_searchCount > 0)
                    _searchCount--;
            }

            FindNextRoutine(routine);
        }

        public RelayCommand ReplaceAllCommand { get; }

        private void ExecuteReplaceAllCommand()
        {
            if (string.IsNullOrEmpty(FindWhat)) return;
            _searchResultService.Clean();
            _occurence = 0;
            _searchCount = 0;
            //TODO(zyl):add other routine
            var replaceAllWorker = new BackgroundWorker();
            replaceAllWorker.WorkerSupportsCancellation = true;
            var message = new FindingMessage();
            var findingDialog = new FindingDialog();
            var vm = new FindingDialogVM(message);
            findingDialog.DataContext = vm;
            replaceAllWorker.DoWork += (s, e) =>
            {
                _searchResultService?.AddInfo(LanguageManager.GetInstance().ConvertSpecifier("Replacing ")
                                              + $"\"{FindWhat}\""
                                              + LanguageManager.GetInstance().ConvertSpecifier(" with ")
                                              + ($"\"{ReplaceWith}\" ..."));
                if (SelectedFindWhere == FindInRoutineSetting.FindWhereType.Current)
                {
                    var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                    var st = stEditorViewModel?.Routine as STRoutine;
                    if (st != null)
                        ReplaceRoutine(st, message);
                }
                else if (SelectedFindWhere == FindInRoutineSetting.FindWhereType.All)
                {
                    foreach (var program in _controller.Programs)
                    {
                        foreach (var routine in program.Routines)
                        {
                            if (replaceAllWorker.CancellationPending)
                            {
                                return;
                            }

                            var st = routine as STRoutine;
                            if (st != null)
                            {
                                ReplaceRoutine(st, message);
                                continue;
                            }
                        }
                    }

                    foreach (var aoi in _controller.AOIDefinitionCollection)
                    {
                        foreach (var routine in aoi.Routines)
                        {
                            if (replaceAllWorker.CancellationPending)
                            {
                                return;
                            }

                            var st = routine as STRoutine;
                            if (st != null)
                            {
                                ReplaceRoutine(st, message);
                                continue;
                            }
                        }
                    }
                }
                else if (SelectedFindWhere == FindInRoutineSetting.FindWhereType.InCurrentProgram)
                {
                    var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                    var st = stEditorViewModel?.Routine as STRoutine;
                    if (st != null)
                    {
                        ReplaceRoutine(st, message);
                        var program = st.ParentCollection.ParentProgram;
                        var routines = program.Routines.OrderByDescending(r => r.Name == program.MainRoutineName)
                            .ThenByDescending(r => r.Name == program.FaultRoutineName).ToList();
                        foreach (var routine in routines)
                        {
                            if (replaceAllWorker.CancellationPending)
                            {
                                return;
                            }

                            if (routine == st) continue;
                            if (routine is STRoutine)
                            {
                                ReplaceRoutine((STRoutine)routine, message);
                            }
                        }
                    }

                }
                else if (SelectedFindWhere == FindInRoutineSetting.FindWhereType.InCurrentTask)
                {
                    var stEditorViewModel = CurrentObject.GetInstance().Current as STEditorViewModel;
                    var st = stEditorViewModel?.Routine as STRoutine;
                    if (st != null)
                    {
                        var program = st.ParentCollection.ParentProgram as Program;
                        if (program != null)
                        {
                            foreach (var controllerProgram in _controller.Programs)
                            {
                                if (controllerProgram.ParentTask == program.ParentTask)
                                {
                                    foreach (var routine in controllerProgram.Routines)
                                    {
                                        if (replaceAllWorker.CancellationPending)
                                        {
                                            return;
                                        }

                                        if (routine is STRoutine)
                                        {
                                            ReplaceRoutine((STRoutine)routine, message);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                _searchResultService.AddInfo(
                    $"Complete - {_occurence} occurence(s) replaced - {_searchCount} routine(s) searched");
                ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    findingDialog.DialogResult = true;
                });
            };
            replaceAllWorker.RunWorkerAsync();

            findingDialog.Owner = Application.Current.MainWindow;
            findingDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (findingDialog.ShowDialog() ?? false)
            {

            }
            else
            {
                replaceAllWorker.CancelAsync();
            }
        }

        public RelayCommand CloseCommand { get; }

        private void ExecuteCloseCommand()
        {
            _closeAction.Invoke();
        }

        public RelayCommand FindWithinTextCommand { get; }

        private void ExecuteFindWithinTextCommand()
        {
            if (_findWithinText.Equals("查找范围>>") || _findWithinText.Equals("Find Within>>"))
            {
                FindWithinText = LanguageManager.GetInstance().ConvertSpecifier("LtString Find Within");
            }
            else
            {
                FindWithinText = LanguageManager.GetInstance().ConvertSpecifier("Find Within GtString");
            }
        }
    }

    public class Component : ViewModelBase
    {
        private bool _isUpdate;
        private string _name;

        public Component()
        {
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Name));
        }

        public string Name
        {
            set
            {
                Set(ref _name, value);
            }
            get
            {
                return LanguageManager.GetInstance().ConvertSpecifier(_name);
            }
        }

        public List<Option> Options { get; } = new List<Option>();

        public bool IsUpdate
        {
            set
            {
                Set(ref _isUpdate, value);
            }
            get { return _isUpdate; }
        }

        public override void Cleanup()
        {
            base.Cleanup();

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }
    }

    public class Option : ViewModelBase
    {
        private bool _isChecked;
        private Component _parent;
        private string _name;

        public Option(Component component)
        {
            _parent = component;
            component.Options.Add(this);
            WeakEventManager<LanguageManager, EventArgs>.AddHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }

        public void LanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(Name));
        }

        public bool IsChecked
        {
            set
            {
                _isChecked = value;
                _parent.IsUpdate = true;
            }
            get { return _isChecked; }
        }

        public string Name
        {
            set
            {
                Set(ref _name, value);
            }
            get
            {
                return LanguageManager.GetInstance().ConvertSpecifier(_name);
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();

            WeakEventManager<LanguageManager, EventArgs>.RemoveHandler(
                LanguageManager.GetInstance(), "LanguageChanged", LanguageChanged);
        }
    }
}
