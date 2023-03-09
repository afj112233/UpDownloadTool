using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ICSStudio.EditorPackage.ModuleProperties;
using ICSStudio.EditorPackage.MonitorEditTags;
using ICSStudio.EditorPackage.Reference;
using ICSStudio.EditorPackage.RLLEditor;
using ICSStudio.EditorPackage.STEditor;
using ICSStudio.EditorPackage.Trend;
using ICSStudio.Gui.Dialogs;
using ICSStudio.Interfaces.Common;
using ICSStudio.Ladder.Controls;
using ICSStudio.UIInterfaces.Dialog;
using ICSStudio.UIInterfaces.QuickWatch;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.EditorPackage
{
    public class EditorWindowManager : IVsWindowFrameEvents, IDisposable
    {
        private readonly uint _eventCookie;
        private bool _disposed;
        private readonly ToolWindowCollection _openedEditorWindowPanes;
        private IVsWindowFrame _activeWindowFrame;

        public EditorWindowManager()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _openedEditorWindowPanes = new ToolWindowCollection();

            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell7;
            if (uiShell != null)
            {
                _eventCookie = uiShell.AdviseWindowFrameEvents(this);
            }

        }

        public void Dispose()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell7;
                    uiShell?.UnadviseWindowFrameEvents(_eventCookie);
                }

                // Note disposing has been done.
                _disposed = true;
            }
        }

        public EditorWindowPane GetEditorWindowPane(int id)
        {
            var openedEditorWindowPane = _openedEditorWindowPanes.GetEditorWindowPane(id);
            return openedEditorWindowPane;
        }

        public List<IRoutine> GetErrorRoutinePane()
        {
            return _openedEditorWindowPanes.GetErrorRoutinePane();
        }

        public List<EditorWindowPane> GetAllRoutinePane()
        {
            return _openedEditorWindowPanes.GetAllRoutinePanes();
        }

        public List<EditorWindowPane> GetAllTrendPane()
        {
            var targets = new List<EditorWindowPane>();
            var panes = _openedEditorWindowPanes.GetAllEditorWindowPanes();
            foreach (var item in panes)
            {
                var trendPane = item.EditorPane as TrendViewModel;
                if (trendPane != null)
                {
                    targets.Add(item);
                }
            }
            return targets;
        }

        public EditorWindowPane CreateEditorWindowPane(int id, IEditorPane editorPane)
        {
            var openedEditorWindowPane = _openedEditorWindowPanes.GetEditorWindowPane(id);
            if (openedEditorWindowPane != null)
                return openedEditorWindowPane;

            if (editorPane == null)
                return null;

            var editorWindowPane = new EditorWindowPane();

            var guidNull = Guid.Empty;
            var toolWindowPersistenceGuid = typeof(EditorWindowPane).GUID;
            var position = new int[1];
            IVsWindowFrame windowFrame;

            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = (IVsUIShell) Package.GetGlobalService(typeof(SVsUIShell));

            var result = uiShell.CreateToolWindow((uint) __VSCREATETOOLWIN.CTW_fMultiInstance,
                (uint) id, editorPane.Control, ref guidNull, ref toolWindowPersistenceGuid,
                ref guidNull, null, editorPane.Caption, position, out windowFrame);

            ErrorHandler.ThrowOnFailure(result);

            windowFrame.SetProperty((int) __VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_MdiChild);

            editorWindowPane.EditorPane = editorPane;
            editorWindowPane.Frame = windowFrame;
            editorPane.CloseAction = editorWindowPane.Close;
            editorPane.UpdateCaptionAction = editorWindowPane.UpdateCaption;

            _openedEditorWindowPanes.Add(id, editorWindowPane);

            return editorWindowPane;
        }

        public EditorWindowPane GetCrossReferencePane()
        {

            var pane = _openedEditorWindowPanes.GetAllEditorWindowPanes()
                .FirstOrDefault(p => p.EditorPane is CrossReferenceViewModel);
            return pane;
        }

        public List<EditorWindowPane> GetAllModuleProperties()
        {
            return _openedEditorWindowPanes.GetAllEditorWindowPanes()
                .Where(x => x.EditorPane is ModulePropertiesViewModel).ToList();
        }

        public void CloseAllEditorWindowPanes(bool closeProject = false)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var allEditorWindowPanes = _openedEditorWindowPanes.GetAllEditorWindowPanes();
            foreach (var pane in allEditorWindowPanes)
            {
                if (pane.EditorPane is STEditorViewModel)
                    ((STEditorViewModel) pane.EditorPane).IsCloseProject = closeProject;

                pane.Close();
            }
        }

        public void CloseMonitorWindowPanes(ITagCollectionContainer tagCollectionContainer, bool closeProject = false)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var allEditorWindowPanes = _openedEditorWindowPanes.GetAllEditorWindowPanes();
            foreach (var pane in allEditorWindowPanes)
            {
                MonitorEditTagsViewModel monitorEditTagsViewModel 
                    = pane?.EditorPane as MonitorEditTagsViewModel;

                if (monitorEditTagsViewModel != null
                    && monitorEditTagsViewModel.Scope == tagCollectionContainer)
                {
                    pane.Close();
                }
            }
        }

        public int ApplyAllToolWindows()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var editorWindowPanes = _openedEditorWindowPanes.GetAllEditorWindowPanes();
            foreach (var windowPane in editorWindowPanes)
            {
                ICanApply canApply = windowPane.EditorPane as ICanApply;
                if (canApply != null)
                {
                    if (canApply.CanApply())
                    {
                        int result = canApply.Apply();
                        if (result < 0)
                        {
                            windowPane.Frame.Show();
                            return result;
                        }
                    }
                }
            }

            return 0;
        }

        public void CloseEditorPane(int id)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            _openedEditorWindowPanes.GetEditorWindowPane(id)?.Close();
        }
        
        public void OnFrameCreated(IVsWindowFrame frame)
        {

        }

        public void OnFrameDestroyed(IVsWindowFrame frame)
        {

        }

        public void OnFrameIsVisibleChanged(IVsWindowFrame frame, bool newIsVisible)
        {

        }

        public void OnFrameIsOnScreenChanged(IVsWindowFrame frame, bool newIsOnScreen)
        {
            var pane = GetEditorWindow(frame);
            if (pane != null)
            {
                var stPane = pane.EditorPane as STEditorViewModel;
                if (stPane != null)
                {
                    stPane.IsOnScreen = newIsOnScreen;
                    if (newIsOnScreen)
                    {
                        var service = Package.GetGlobalService(typeof(SQuickWatchService)) as IQuickWatchService;
                        service?.AddExplicitScope(stPane.Routine.ParentCollection.ParentProgram);
                    }
                    stPane.IsOnScreenChanged(newIsOnScreen);
                    return;
                }

                var monitor = pane.EditorPane as MonitorEditTagsViewModel;
                if (monitor != null)
                {
                    if (newIsOnScreen)
                    {
                        var service = Package.GetGlobalService(typeof(SQuickWatchService)) as IQuickWatchService;
                        service?.AddExplicitScope(monitor.Scope);
                    }

                    return;
                }
                
                //var trendPane = pane.EditorPane as TrendViewModel;
                //if (trendPane != null)
                //{
                //    trendPane.OnActiveChanged(newIsOnScreen);
                //}
            }
        }

        public static STEditorViewModel stPane;

        public static RLLEditorViewModel rllPane;

        public void OnActiveFrameChanged(IVsWindowFrame oldFrame, IVsWindowFrame newFrame)
        {
            var oldWindow = GetEditorWindow(oldFrame);
            if (oldWindow != null)
            {
                var stEditorViewModel = oldWindow.EditorPane as STEditorViewModel;
                stEditorViewModel?.RecoveryError();
            }

            _activeWindowFrame = newFrame;

            var activeEditorWindow = GetActiveEditorWindow();
            if (activeEditorWindow != null)
            {
                CurrentObject.GetInstance().Current = activeEditorWindow.EditorPane;
                //TODO(gjc): add active handle here
                stPane = activeEditorWindow.EditorPane as STEditorViewModel;

                if (stPane != null)
                {
                    stPane.OnActive();
                }

                rllPane = activeEditorWindow.EditorPane as RLLEditorViewModel;
                if (rllPane != null)
                {
                    RLLEditorControl.MarkCurCanvasControl(rllPane.BottomControl as CanvasControl);
                    rllPane.RefreshExecutable();
                }
                //var trendPane = activeEditorWindow.EditorPane as TrendViewModel;
                //if (trendPane != null)
                //{
                //    trendPane.OnActiveChanged(true);
                //}
            }

            else
            {
                stPane = null;
                rllPane = null;
            }
        }

        public EditorWindowPane GetActiveEditorWindow()
        {
            return GetEditorWindow(_activeWindowFrame);
        }

        private EditorWindowPane GetEditorWindow(IVsWindowFrame frame)
        {
            if (frame != null)
            {
                foreach (var windowPane in _openedEditorWindowPanes.GetAllEditorWindowPanes())
                {
                    if (windowPane.Frame == frame)
                        return windowPane;
                }
            }

            return null;
        }

        public List<UIElement> GetAllEditors()
        {
            return _openedEditorWindowPanes.GetAllEditorWindowPanes().Select(p=>p.EditorPane.Control as UIElement).ToList();
        }
    }
}
