using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSStudio.AvalonEdit;
using ICSStudio.AvalonEdit.CodeCompletion;
using ICSStudio.AvalonEdit.Document;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using ICSStudio.StxEditor.Interfaces;
using ICSStudio.StxEditor.ViewModel.IntelliPrompt;
using System.Windows;
using System.Windows.Media;
using ICSStudio.AvalonEdit.Variable;
using ICSStudio.Interfaces.DataType;
using ICSStudio.StxEditor.Menu;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.StxEditor.ViewModel
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal partial class StxEditorViewModel : ViewModelBase
    {
        // *10
        private const int FONT_MAX_SCALE = 30;
        private const int FONT_MIN_SCALE = 5;
        private readonly StxCompletionItemsGenerator _completionItemsGenerator;
        private int _currentFontScale = 10;
        private bool _isToggle = false;
        private bool _isToggleValue = false;
        private bool _canZoom = true;
        private readonly BrowseAdorner _browseAdorner;
        private Brush _status;
        private bool _isAoiInRun;

        public StxEditorViewModel(
            STRoutine routine,
            StxEditorDocument document,
            IStxEditorOptions options,
            TextEditor textEditor, List<StxCompletionItemCodeSnippetData> data)
        {
            Routine = routine;
            Controller = routine.ParentController as Controller;
            StxEditorDocument = document;
            Options = options;
            Editor = textEditor;
            DragPreviewViewModel = new StxDragPreviewViewModel(Options);
            if (routine.ParentController.IsOnline)
            {
                Options.CanEditorInput = false;
            }

            if (((routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false))
            {
                Editor.TextArea.TextView.ShowSealed();
            }

            SetStatus();
            if (routine.PendingCodeText != null || routine.TestCodeText != null)
            {
                if (StxEditorDocument.HasPending)
                {
                    Editor.TextArea.TextView.ShowPending();
                    Options.CanEditorInput = true;
                }
                else if (StxEditorDocument.HasTest)
                {
                    Editor.TextArea.TextView.ShowTest();
                    Options.CanEditorInput = false;
                }
                else
                {
                    Editor.TextArea.TextView.ShowOriginal();
                    Options.CanEditorInput = false;
                }

                if ((routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false)
                {
                    Options.CanEditorInput = false;
                }
            }

            _completionItemsGenerator = new StxCompletionItemsGenerator(this, data);
            Editor.Options.ShowTabs = _isToggle;
            Editor.Options.ShowSpaces = _isToggle;
            //Command
            IncreaseZoomCommand = new RelayCommand(ExecuteIncreaseZoom, CanExecuteIncreaseZoom);
            DecreaseZoomCommand = new RelayCommand(ExecuteDecreaseZoom, CanExecuteDecreaseZoom);
            CommentCommand = new RelayCommand(ExecuteCommentCommand, CanExecuteCommentCommand);
            UncommentCommand = new RelayCommand(ExecuteUncommentCommand, CanUncommentCommand);
            IncreaseIndentCommand = new RelayCommand(ExecuteIncreaseIndentCommand, CanIncreaseIndentCommand);
            DecreaseIndentCommand = new RelayCommand(ExecuteDecreaseIndentCommand, CanDecreaseIndentCommand);
            ToggleWhiteCommand = new RelayCommand(ExecuteToggleWhiteCommand);
            ToggleValueCommand = new RelayCommand(ExecuteToggleValueCommand);
            PendingCommand = new RelayCommand(ExecutePendingCommand, CanPendingCommand);
            OriginalCommand = new RelayCommand(ExecuteOriginalCommand, CanOriginalCommand);
            TestCommand = new RelayCommand(ExecuteTestCommand, CanTestCommand);

            #region Shortcut


            _needParseMenuType = false;
            NewTagCommand = new RelayCommand(ExecuteNewTagCommand, CanExecuteNewTagCommand);
            BrowseTagsCommand = new RelayCommand(ExecuteBrowseTagsCommand, CanExecuteBrowseTagsCommand);
            AddSTElementCommand = new RelayCommand(ExecuteAddSTElementCommand, CanExecuteAddSTElementCommand);

            CopyCommand = new RelayCommand(ExecuteCopyCommand);
            PasteCommand = new RelayCommand(ExecutePasteCommand, CanExecutePasteCommand);
            CutCommand = new RelayCommand(ExecuteCutCommand);

            GoToCommand = new RelayCommand(ExecuteGoToCommand);

            EditTagPropertiesCommand =
                new RelayCommand(ExecuteEditTagPropertiesCommand, CanExecuteEditTagPropertiesCommand);
            CrossTagCommand = new RelayCommand(ExecuteCrossTagCommand, CanExecuteCrossTagCommand);
            BrowseEnumCommand = new RelayCommand(ExecuteBrowseEnumCommand, CanExecuteBrowseEnumCommand());
            PropertiesCommand = new RelayCommand(ExecutePropertiesCommand, CanExecutePropertiesCommand());
            WatchTagsCommand=new RelayCommand(ExecuteWatchTagsCommand,CanExecuteWatchTagsCommand);
            ArgumentListCommand=new RelayCommand(ExecuteArgumentListCommand,CanExecuteArgumentListCommand);
            _needParseMenuType = true;

            #endregion

            PropertyChangedEventManager.AddHandler(DragPreviewViewModel, Options_PropertyChanged, string.Empty);
            if (routine.ParentCollection.ParentProgram is AoiDefinition)
            {
                PropertyChangedEventManager.AddHandler(routine.ParentCollection.ParentProgram,
                    StxEditorViewModel_PropertyChanged, "IsSealed");
            }

            Controller controller = Routine.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.AddHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);
                WeakEventManager<Controller, EventArgs>.AddHandler(
                    controller, "OperationModeChanged", Controller_OperationModeChanged);
            }

            _browseAdorner =
                new BrowseAdorner(Editor, Controller, routine.ParentCollection.ParentProgram, document.FilterViewModel);
            BrowseEnumAdorner = new BrowseEnumAdorner(Editor);
            BrowseRoutinesAdorner = new BrowseRoutinesAdorner(Editor, routine.ParentCollection);
            MonitorProgramInhibit();

        }

        private void MonitorProgramInhibit()
        {
            var program = Routine.ParentCollection.ParentProgram as Program;
            if (program != null)
            {
                PropertyChangedEventManager.AddHandler(program, Program_PropertyChanged1, "UpdateRoutineRunStatus");
            }
        }

        private void Program_PropertyChanged1(object sender, PropertyChangedEventArgs e)
        {
            SetStatus();
        }
        
        private void OffMonitorProgramInhibit()
        {
            var program = Routine.ParentCollection.ParentProgram as Program;
            if (program != null)
            {
                PropertyChangedEventManager.RemoveHandler(program, Program_PropertyChanged1, "UpdateRoutineRunStatus");
            }
        }

        private void StxEditorViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CommentCommand.RaiseCanExecuteChanged();
            UncommentCommand.RaiseCanExecuteChanged();
            IncreaseIndentCommand.RaiseCanExecuteChanged();
            DecreaseIndentCommand.RaiseCanExecuteChanged();
            Options.CanEditorInput = !((AoiDefinition) sender).IsSealed;
            if (((AoiDefinition) sender).IsSealed)
            {
                Editor.TextArea.TextView.ShowSealed();
            }
            else
            {
                Editor.TextArea.TextView.HideSealed();
            }
        }


        #region Status

        private void Controller_OperationModeChanged(object sender, EventArgs e)
        {
            SetStatus();
        }

        private void SetStatus()
        {
            //TODO(zyl):event task
            if (Controller.IsOnline && Controller.OperationMode == ControllerOperationMode.OperationModeRun)
            {
                if (Routine.CurrentOnlineEditType != OnlineEditType.Original)
                {
                    Status = Brushes.White;
                    return;
                }
                var parent = Routine.ParentCollection.ParentProgram as Program;
                if (parent == null || parent.Inhibited || (parent.ParentTask?.IsInhibited??false))
                {
                    if (Routine.ParentCollection.ParentProgram is AoiDefinition)
                    {
                        if (IsAoiInRun)
                        {
                            Status = Brushes.GreenYellow;
                            return;
                        }
                    }

                    Status = Brushes.White;
                    return;
                }

                if (parent.FaultRoutineName == Routine.Name || parent.MainRoutineName == Routine.Name)
                {
                    if (!(Options.ShowTest || Options.ShowPending))
                    {
                        Status = Brushes.GreenYellow;
                        return;
                    }

                    Status = Brushes.White;
                    return;
                }

                //TODO(zyl):check jsr
                if (RoutineExtend.CheckRoutineInRun(Routine))
                {
                    Status = Brushes.GreenYellow;
                    return;
                }

                Status = Brushes.White;
            }
            else
            {
                Status = Brushes.White;
            }
        }

        public bool IsAoiInRun
        {
            set
            {
                _isAoiInRun = value;
                SetStatus();
            }
            get { return _isAoiInRun; }
        }

        #endregion

        public Brush Status
        {
            set { Set(ref _status, value); }
            get { return _status; }
        }

        public STRoutine Routine { get; }
        public Controller Controller { get; }
        public BrowseAdorner BrowseAdorner => _browseAdorner;
        public BrowseEnumAdorner BrowseEnumAdorner { get; }
        public BrowseRoutinesAdorner BrowseRoutinesAdorner { get; }

        public TextDocument Document => StxEditorDocument.Document;
        public StxEditorDocument StxEditorDocument { get; }
        public IStxEditorOptions Options { get; }
        public TextEditor Editor { get; }
        public StxDragPreviewViewModel DragPreviewViewModel { get; }

        public void UpdateIntellisenseItems(CompletionWindow completionWindow, TextEditor editor, string typedText,
            IDataType filterDataType)
        {
            _completionItemsGenerator.GenerateItems(completionWindow, editor, typedText, filterDataType);
        }

        public StxCompletionItemAoiData GetAoiData(string name)
        {
            return _completionItemsGenerator.StxCompletionItemAoiDataCollection.FirstOrDefault(s =>
                s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void UpdateFontSize(bool increase)
        {
            if (increase)
            {
                if (_currentFontScale < FONT_MAX_SCALE)
                {
                    _currentFontScale += 1;
                    Editor.FontSize = Options.FontSize * _currentFontScale / 10;
                }
            }
            else
            {
                if (_currentFontScale > FONT_MIN_SCALE)
                {
                    _currentFontScale -= 1;
                    Editor.FontSize = Options.FontSize * _currentFontScale / 10;
                }
            }
        }

        public override void Cleanup()
        {
            if (Routine.ParentCollection.ParentProgram is AoiDefinition)
            {
                PropertyChangedEventManager.RemoveHandler(Routine.ParentCollection.ParentProgram,
                    StxEditorViewModel_PropertyChanged, "IsSealed");
            }

            Controller controller = Routine.ParentController as Controller;
            if (controller != null)
            {
                WeakEventManager<Controller, IsOnlineChangedEventArgs>.RemoveHandler(
                    controller, "IsOnlineChanged", OnIsOnlineChanged);

                WeakEventManager<Controller, EventArgs>.RemoveHandler(
                    controller, "OperationModeChanged", Controller_OperationModeChanged);
            }
            foreach (VariableInfo currentVariableInfo in Routine.GetCurrentVariableInfos(Routine.CurrentOnlineEditType))
            {
                currentVariableInfo.MinWidth = 0;
            }
            _completionItemsGenerator.Cleanup();
            PropertyChangedEventManager.RemoveHandler(DragPreviewViewModel, Options_PropertyChanged, string.Empty);
            OffMonitorProgramInhibit();
        }

        #region Command

        public RelayCommand IncreaseZoomCommand { get; }
        public RelayCommand DecreaseZoomCommand { get; }
        public RelayCommand IncreaseIndentCommand { get; }
        public RelayCommand DecreaseIndentCommand { get; }
        public RelayCommand CommentCommand { get; }
        public RelayCommand UncommentCommand { get; }
        public RelayCommand ToggleWhiteCommand { get; }
        public RelayCommand ToggleValueCommand { get; }
        public RelayCommand PendingCommand { get; }
        public RelayCommand OriginalCommand { get; }
        public RelayCommand TestCommand { get; }

        private bool CanExecuteIncreaseZoom()
        {
            if (_canZoom)
                return _currentFontScale < FONT_MAX_SCALE;
            else
                return false;
        }

        private void ExecuteIncreaseZoom()
        {
            UpdateFontSize(true);
        }

        private bool CanExecuteDecreaseZoom()
        {
            if (_canZoom)
                return _currentFontScale > FONT_MIN_SCALE;
            else
                return false;
        }

        private void ExecuteDecreaseZoom()
        {
            UpdateFontSize(false);
        }

        private void ExecuteIncreaseIndentCommand()
        {
            Indentation.Indentation.Increase(Editor);
        }

        private bool CanIncreaseIndentCommand()
        {
            if (StxEditorDocument.HasPending && Routine.CurrentOnlineEditType != OnlineEditType.Pending) return false;
            if (Controller.IsOnline && Routine.CurrentOnlineEditType != OnlineEditType.Pending) return false;
            if (Options.IsConnecting) return false;
            if ((Routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            return Indentation.Indentation.CheckIndentStatus(Editor);
        }

        private void ExecuteDecreaseIndentCommand()
        {
            Indentation.Indentation.Decrease(Editor);
        }

        private bool CanDecreaseIndentCommand()
        {
            if ((Routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            if (StxEditorDocument.HasPending && Routine.CurrentOnlineEditType != OnlineEditType.Pending) return false;
            if (Controller.IsOnline && Routine.CurrentOnlineEditType != OnlineEditType.Pending) return false;
            if (Options.IsConnecting) return false;
            if (Editor.Text.Length > 0 && (Editor.SelectionStart - 1 == -1 ||
                                           Editor.Text.Substring(Editor.SelectionStart - 1, 1) == "\n") &&
                (Editor.SelectionLength + Editor.SelectionStart + 1 > Editor.Text.Length ||
                 Editor.Text.Substring(Editor.SelectionLength + Editor.SelectionStart, 1) == "\r"))
            {
                if (Editor.Text.Length > 0 && (Editor.SelectionStart - 1 == -1 ||
                                               Editor.Text.Substring(Editor.SelectionStart - 1, 1) == "\n" &&

                                               Editor.SelectionStart + 1 > Editor.Text.Length) &&
                    Editor.SelectionLength == 0)
                {
                    return false;
                }

                return true;
            }

            if (Editor.SelectedText.Contains("\n")) return true;
            return false;
        }

        private void ExecuteCommentCommand()
        {
            StxEditorDocument.DoLineChangeParser = false;
            Indentation.Indentation.Comment(Editor);
            StxEditorDocument.DoLineChangeParser = true;
            StxEditorDocument.UpdateLexer();
            StxEditorDocument.Update = true;
        }

        private bool CanExecuteCommentCommand()
        {
            if ((Routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            if (StxEditorDocument.HasPending && Routine.CurrentOnlineEditType != OnlineEditType.Pending) return false;
            if (Controller.IsOnline && Routine.CurrentOnlineEditType != OnlineEditType.Pending) return false;
            if (Options.IsConnecting) return false;
            return true;
        }

        private void ExecuteUncommentCommand()
        {
            StxEditorDocument.DoLineChangeParser = false;
            Indentation.Indentation.Uncomment(Editor);
            StxEditorDocument.DoLineChangeParser = true;
            StxEditorDocument.UpdateLexer();
            StxEditorDocument.Update = true;
        }

        private bool CanUncommentCommand()
        {
            if ((Routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            if (StxEditorDocument.HasPending && Routine.CurrentOnlineEditType != OnlineEditType.Pending) return false;
            if (Controller.IsOnline && Routine.CurrentOnlineEditType != OnlineEditType.Pending) return false;
            if (Options.IsConnecting) return false;
            return Indentation.Indentation.CheckCommentStatus(Editor);
        }

        private void ExecuteToggleWhiteCommand()
        {
            _isToggle = !_isToggle;
            Editor.Options.ShowTabs = _isToggle;
            Editor.Options.ShowSpaces = _isToggle;
        }

        private void ExecuteToggleValueCommand()
        {
            _isToggleValue = !_isToggleValue;
            if (_isToggleValue && StxEditorDocument.IsChanged)
                StxEditorDocument.UpdateLexer(false);
            Options.ShowInLineDisplay = _isToggleValue;
            Editor.TextArea.TextView.LineSpacing = Options.ShowInLineDisplay ? 2 : 1;
            Editor.TextArea.TextView.Redraw();
        }

        private void ExecutePendingCommand()
        {
            if (StxEditorDocument.NeedParse)
            {
                StxEditorDocument.IsNeedBackground = false;
                StxEditorDocument.UpdateLexer(false);
                StxEditorDocument.IsNeedBackground = true;
            }
            Options.ShowPending = true;
            //Options.ShowInLineDisplay = false;
        }

        private void ExecuteTestCommand()
        {
            if (StxEditorDocument.NeedParse)
            {
                StxEditorDocument.IsNeedBackground = false;
                StxEditorDocument.UpdateLexer(false);
                StxEditorDocument.IsNeedBackground = true;
            }
            Options.ShowTest = true;
            //Options.ShowInLineDisplay = false;
        }

        private void ExecuteOriginalCommand()
        {
            if (StxEditorDocument.NeedParse)
            {
                StxEditorDocument.IsNeedBackground = false;
                StxEditorDocument.UpdateLexer(false);
                StxEditorDocument.IsNeedBackground = true;
            }
            Options.ShowOriginal = true;
            //Options.ShowInLineDisplay = false;
        }

        private bool CanPendingCommand()
        {
            if ((Routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            return StxEditorDocument.HasPending;
        }

        private bool CanTestCommand()
        {
            if ((Routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            return StxEditorDocument.HasTest;
        }

        private bool CanOriginalCommand()
        {
            if ((Routine.ParentCollection.ParentProgram as AoiDefinition)?.IsSealed ?? false) return false;
            return StxEditorDocument.HasPending || StxEditorDocument.HasTest;
        }

        #endregion

        private void OnIsOnlineChanged(object sender, IsOnlineChangedEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                SetStatus();
                Options.CanEditorInput = !Routine.ParentController.IsOnline;
                if (Routine.PendingCodeText != null || Routine.TestCodeText != null)
                {
                    if (StxEditorDocument.HasPending)
                    {
                        Editor.TextArea.TextView.ShowPending();
                        Options.CanEditorInput = true;
                    }
                    else if (StxEditorDocument.HasTest)
                    {
                        Editor.TextArea.TextView.ShowTest();
                        Options.CanEditorInput = false;
                    }
                    else
                    {
                        Editor.TextArea.TextView.ShowOriginal();
                        Options.CanEditorInput = false;
                    }
                }

                if ((Options.SelectedDataReference?.ReferenceAoi != null) ||
                    Options.SelectedDataReference?.Routine != null)
                {
                    Options.CanEditorInput = false;
                }
            });
        }
        
        private void Options_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CanEditorInput")
            {
                Editor.CanInput = Options.CanEditorInput;
                return;
            }
            if (e.PropertyName == "ShowInLineDisplay")
            {
                _isToggleValue = Options.ShowInLineDisplay;
                Editor.TextArea.TextView.LineSpacing = Options.ShowInLineDisplay ? 2 : 1;
                if (!Options.ShowInLineDisplay)
                {
                    Editor.TextArea.TextView.ShowCaretLayer();
                    foreach (VariableInfo currentVariableInfo in Routine.GetCurrentVariableInfos(Routine.CurrentOnlineEditType))
                    {
                        currentVariableInfo.MinWidth = 0;
                    }
                }
                Editor.TextArea.TextView.Redraw();
            }
            
            if (e.PropertyName == "CanZoom")
            {
                _canZoom = Options.CanZoom;
            }

            if (e.PropertyName == "ShowTest")
            {
                foreach (VariableInfo currentVariableInfo in Routine.GetCurrentVariableInfos(Routine.CurrentOnlineEditType))
                {
                    currentVariableInfo.MinWidth = 0;
                }
                if (Options.ShowTest)
                {
                    Options.CanEditorInput = false;
                    Editor.TextArea.TextView.ShowTest();
                    SetStatus();
                }
                else
                {
                    Editor.TextArea.TextView.HideTest();
                }
               
                Editor.TextArea.TextView.Redraw();
                TestCommand.RaiseCanExecuteChanged();
                PendingCommand.RaiseCanExecuteChanged();
                OriginalCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == "ShowOriginal")
            {
                foreach (VariableInfo currentVariableInfo in Routine.GetCurrentVariableInfos(Routine.CurrentOnlineEditType))
                {
                    currentVariableInfo.MinWidth = 0;
                }
                if (Options.ShowOriginal)
                {
                    Options.CanEditorInput = false;
                    Editor.TextArea.TextView.ShowOriginal();
                    SetStatus();
                }
                else
                {
                    Editor.TextArea.TextView.HideOriginal();
                }

                StxEditorDocument.Routine.CurrentOnlineEditType = OnlineEditType.Original;
                TestCommand.RaiseCanExecuteChanged();
                PendingCommand.RaiseCanExecuteChanged();
                OriginalCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == "ShowPending")
            {
                foreach (VariableInfo currentVariableInfo in Routine.GetCurrentVariableInfos(Routine.CurrentOnlineEditType))
                {
                    currentVariableInfo.MinWidth = 0;
                }
                if (Options.ShowPending)
                {
                    Options.CanEditorInput = true;
                    Editor.TextArea.TextView.ShowPending();
                    SetStatus();
                }
                else
                {
                    Editor.TextArea.TextView.HidePending();
                }
                StxEditorDocument.Routine.CurrentOnlineEditType = OnlineEditType.Pending;
                TestCommand.RaiseCanExecuteChanged();
                PendingCommand.RaiseCanExecuteChanged();
                OriginalCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == "HideAll")
            {
                if (Options.HideAll)
                {
                    if (Controller.IsOnline)
                        Options.CanEditorInput = false;
                    else
                        Options.CanEditorInput = true;
                    Editor.TextArea.TextView.HideOriginal();
                    Editor.TextArea.TextView.HidePending();
                    Editor.TextArea.TextView.HideTest();
                    SetStatus();
                }
                StxEditorDocument.Routine.CurrentOnlineEditType = OnlineEditType.Original;
                TestCommand.RaiseCanExecuteChanged();
                PendingCommand.RaiseCanExecuteChanged();
                OriginalCommand.RaiseCanExecuteChanged();
            }

            if (e.PropertyName == "Cleanup")
            {
                if (Options.Cleanup)
                    Cleanup();
            }

            if (e.PropertyName == "IsConnecting")
            {
                Options.CanEditorInput = !Options.IsConnecting;
                IncreaseIndentCommand.RaiseCanExecuteChanged();
                DecreaseIndentCommand.RaiseCanExecuteChanged();
                CommentCommand.RaiseCanExecuteChanged();
                UncommentCommand.RaiseCanExecuteChanged();
            }
        }

    }
}