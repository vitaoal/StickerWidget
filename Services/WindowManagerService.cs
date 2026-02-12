using System;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Services
{
    public class WindowManagerService
    {
        private IntPtr _lastActiveWindowHandle;

        public void SaveCurrentForeignWindow()
        {
            var hwnd = NativeMethods.GetForegroundWindow();
            var currentWindowHwnd = new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle;
            
            // Only save if it's not our own window
            if (hwnd != currentWindowHwnd)
            {
                _lastActiveWindowHandle = hwnd;
            }
        }

        public void RestoreLastForeignWindow()
        {
            if (_lastActiveWindowHandle != IntPtr.Zero)
            {
                NativeMethods.SetForegroundWindow(_lastActiveWindowHandle);
            }
        }

        public async Task SimulatePasteAsync()
        {
            // Small delay to ensure focus has switched and clipboard is ready
            await Task.Delay(200);

            // Simulate Ctrl + V
            // Press Ctrl
            NativeMethods.keybd_event(NativeMethods.VK_CONTROL, 0, 0, UIntPtr.Zero);
            await Task.Delay(50);
            
            // Press V
            NativeMethods.keybd_event(NativeMethods.VK_V, 0, 0, UIntPtr.Zero);
            await Task.Delay(50);
            
            // Release V
            NativeMethods.keybd_event(NativeMethods.VK_V, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
            await Task.Delay(50);
            
            // Release Ctrl
            NativeMethods.keybd_event(NativeMethods.VK_CONTROL, 0, NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }
}
