using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.Gui.Utils
{
    public static class WindowExtensions
    {
        public static bool? ShowDialog(this Window window, IVsUIShell shell)
        {
            IntPtr owner;

            // if the shell doesn't retrieve the dialog owner or doesn't enter modal mode, just let the dialog do it's normal thing
            if (shell == null || shell.GetDialogOwnerHwnd(out owner) != 0 || shell.EnableModeless(0) != 0)
                return window.ShowDialog();

            var helper = new WindowInteropHelper(window);

            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            helper.Owner = owner;

            try
            {
                return window.ShowDialog();
            }
            finally
            {
                shell.EnableModeless(1);
                helper.Owner = IntPtr.Zero;
            }
        }

        public static void Show(this Window window, IVsUIShell shell)
        {
            IntPtr owner = IntPtr.Zero;

            if (shell == null || shell.GetDialogOwnerHwnd(out owner) != 0)
            {
                window.Show();
                window.Activate();
            }

            var helper = new WindowInteropHelper(window);

            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            helper.Owner = owner;

            if (window.Visibility != Visibility.Visible)
            {
                window.Show();
            }

            window.Activate();

        }

        // from winuser.h
        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_MINIMIZEBOX = 0x20000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int value);

        public static void HideMinimizeAndMaximizeButtons(this Window window)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            var currentStyle = GetWindowLong(hwnd, GWL_STYLE);

            SetWindowLong(hwnd, GWL_STYLE, (currentStyle & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX));
        }

        public static void HideMinimizeButtons(this Window window)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            var currentStyle = GetWindowLong(hwnd, GWL_STYLE);

            SetWindowLong(hwnd, GWL_STYLE, (currentStyle & ~WS_MINIMIZEBOX));
        }
    }
}