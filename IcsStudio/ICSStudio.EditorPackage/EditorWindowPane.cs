using System;
using System.Runtime.InteropServices;
using GalaSoft.MvvmLight;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.EditorPackage
{
    [Guid("8DB94AE6-BA19-4343-B8F6-8D4CE9399E5C")]
    public class EditorWindowPane : IDisposable
    {
        private string _caption;

        internal EditorWindowPane()
        {
        }

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                if (Frame == null)
                    return;
                if (_caption == null)
                    return;
                try
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    Frame.SetProperty((int) __VSFPROPID.VSFPROPID_Caption, _caption);
                }
                catch (COMException ex)
                {
                    // ReSharper disable once UnusedVariable
                    var errorCode = ex.ErrorCode;
                }
            }
        }

        public IVsWindowFrame Frame { get; internal set; }

        public IEditorPane EditorPane { get; internal set; }

        internal void UpdateCaption(string caption)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                // You're now on the UI thread.
                Caption = caption;
            });
        }

        internal void Close()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Frame?.CloseFrame((int) __FRAMECLOSE.FRAMECLOSE_SaveIfDirty);
        }

        public void Dispose()
        {
            var cleanup = EditorPane as ICleanup;
            cleanup?.Cleanup();

            EditorPane = null;
        }

    }
}