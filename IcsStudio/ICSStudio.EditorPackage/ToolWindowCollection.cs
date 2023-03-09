using System;
using System.Collections.Generic;
using System.Linq;
using ICSStudio.EditorPackage.STEditor;
using ICSStudio.Interfaces.Common;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.EditorPackage
{
    internal class ToolWindowCollection : IDisposable
    {
        private List<ToolWindowListener> _listeners;

        public void Dispose()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_listeners == null)
                return;
            foreach (var listener in _listeners)
                listener.Dispose();
            _listeners = null;
        }

        public void Add(int id, EditorWindowPane editorWindowPane)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Remove(id);
            var toolWindowListener = new ToolWindowListener(id, editorWindowPane);
            toolWindowListener.OnFrameClosed += OnFrameClosed;

            if (_listeners == null)
                _listeners = new List<ToolWindowListener>();

            _listeners.Add(toolWindowListener);
        }

        public EditorWindowPane GetEditorWindowPane(int id)
        {
            if (_listeners == null)
                return null;

            foreach (var listener in _listeners)
                if (listener.WindowID == id)
                    return listener.WindowPane;

            return null;
        }

        public List<EditorWindowPane> GetAllEditorWindowPanes()
        {
            var panes = new List<EditorWindowPane>();

            if (_listeners != null)
            {
                foreach (var listener in _listeners)
                {
                    panes.Add(listener.WindowPane);
                }
            }

            return panes;
        }

        public List<EditorWindowPane> GetAllRoutinePanes()
        {
            var panes = new List<EditorWindowPane>();

            if (_listeners != null)
            {
                foreach (var listener in _listeners)
                {
                    //TODO(zyl):add other routine
                    if (listener.WindowPane.EditorPane is STEditorViewModel)
                    {
                        panes.Add(listener.WindowPane);
                    }
                }
            }

            return panes;
        }

        public List<IRoutine> GetErrorRoutinePane()
        {
            var routines = new List<IRoutine>();
            var panes = _listeners?.Where(r => (r.WindowPane.EditorPane as STEditorViewModel)?.Routine.IsError ?? false);
            if (panes != null)
            {
                foreach (var listener in panes)
                {
                    routines.Add(((STEditorViewModel)listener.WindowPane.EditorPane).Routine);
                }
            }
            return routines;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool Remove(int id)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_listeners == null)
                return false;

            for (var index = 0; index != _listeners.Count; index++)
            {
                var listener = _listeners[index];
                if (listener.WindowID == id)
                {
                    _listeners.RemoveAt(index);
                    listener.Dispose();
                    return true;
                }
            }

            return false;
        }

        private void OnFrameClosed(int id)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Remove(id);
        }

        private delegate void FrameClosedHandler(int windowID);

        private class ToolWindowListener : IDisposable, IVsWindowFrameNotify, IVsWindowFrameNotify3
        {
            private uint _cookie;
            private FrameClosedHandler _onCloseDelegate;

            public ToolWindowListener(int windowID,
                EditorWindowPane window
            )
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (window == null)
                    throw new ArgumentNullException(nameof(window));

                WindowID = windowID;
                WindowPane = window;

                SubscribeForEvents();
            }

            public int WindowID { get; }

            public EditorWindowPane WindowPane { get; private set; }

            public void Dispose()
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (WindowPane == null)
                    return;
                if (_cookie != 0U)
                    UnsubscribeForEvents();

                WindowPane.Dispose();
                WindowPane = null;
            }

            public int OnShow(int fShow)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                if (WindowPane == null || fShow != 7 && fShow != 8)
                    return VSConstants.S_OK;
                OnClose();
                return VSConstants.S_OK;
            }

            public int OnMove()
            {
                return VSConstants.S_OK;
            }

            public int OnSize()
            {
                return VSConstants.S_OK;
            }

            public int OnDockableChange(int fDockable)
            {
                return VSConstants.S_OK;
            }

            public int OnMove(int x, int y, int w, int h)
            {
                return VSConstants.S_OK;
            }

            public int OnSize(int x, int y, int w, int h)
            {
                return VSConstants.S_OK;
            }

            public int OnDockableChange(int fDockable, int x, int y, int w, int h)
            {
                return VSConstants.S_OK;
            }

            public int OnClose(ref uint pgrfSaveOptions)
            {
                return VSConstants.S_OK;
            }

            public event FrameClosedHandler OnFrameClosed
            {
                add { _onCloseDelegate += value; }
                remove
                {
                    // ReSharper disable once DelegateSubtraction
                    _onCloseDelegate -= value;
                }
            }

            private void UnsubscribeForEvents()
            {
                if (WindowPane == null || _cookie == 0U)
                    return;

                ThreadHelper.ThrowIfNotOnUIThread();
                // ReSharper disable once SuspiciousTypeConversion.Global
                var frame = WindowPane.Frame as IVsWindowFrame2;
                if (frame == null)
                    return;
                frame.Unadvise(_cookie);
                _cookie = 0U;
            }

            private void SubscribeForEvents()
            {
                if (WindowPane == null)
                    throw new InvalidOperationException();

                ThreadHelper.ThrowIfNotOnUIThread();
                // ReSharper disable once SuspiciousTypeConversion.Global
                var frame = WindowPane.Frame as IVsWindowFrame2;
                if (frame == null)
                    throw new InvalidOperationException();
                if (_cookie != 0U)
                    throw new InvalidOperationException();
                ErrorHandler.ThrowOnFailure(frame.Advise(this, out _cookie));
            }

            private void OnClose()
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                UnsubscribeForEvents();
                _onCloseDelegate?.Invoke(WindowID);
            }
        }
    }
}
