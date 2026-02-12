using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace WpfApp1.Services
{
    public class HotKeyService : IDisposable
    {
        private const int HOTKEY_ID = 9000;
        private IntPtr _windowHandle;
        private HwndSource _source;
        private readonly Action _onHotKeyPressed;

        public HotKeyService(Window window, Action onHotKeyPressed)
        {
            _onHotKeyPressed = onHotKeyPressed;
            Initialize(window);
        }

        private void Initialize(Window window)
        {
            var helper = new WindowInteropHelper(window);
            _windowHandle = helper.Handle; // Ensure handle exists
            if (_windowHandle == IntPtr.Zero)
            {
               // This might happen if window is not shown yet? 
               // For this example we assume widget window is created.
               // It's safer to call this after SourceInitialized if needed but
               // usually MainWindow is instantiated.
               // We will hook later if needed.
               // Actually, let's allow caller to pass Valid Handle or we hook into SourceInitialized outside.
               // But assumes Window is loaded.
            }
        
            // We need to attach to the window's HwndSource to receive messages
             _source = HwndSource.FromHwnd(_windowHandle);
             if (_source != null)
             {
                 _source.AddHook(HwndHook);
                 // Registration removed here, moved to manual call or separate logic if needed,
                 // but keeping it simple: let's NOT remove it if we want it automatic, 
                 // BUT I want to return success status.
                 // Actually, let's keep the logic simple for the user.
                 // I will revert the requirement to call it manually and just Add it back here
                 // but keep the Public method for retry/debug.
                 RegisterHotKey();
             }
        }

        // Call this if the window handle wasn't ready in constructor
        public void Register(Window window)
        {
            var helper = new WindowInteropHelper(window);
            _windowHandle = helper.Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source?.AddHook(HwndHook);
            RegisterHotKey();
        }

        public bool RegisterHotKey()
        {
            // Ctrl + Alt + S
            // VK_S = 0x53 (83)
            var modifiers = NativeMethods.MOD_CONTROL | NativeMethods.MOD_ALT;
            var success = NativeMethods.RegisterHotKey(_windowHandle, HOTKEY_ID, (uint)modifiers, 0x53);
            
            if (!success)
            {
                Debug.WriteLine("Failed to register hotkey.");
            }
            return success;
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                _onHotKeyPressed?.Invoke();
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            NativeMethods.UnregisterHotKey(_windowHandle, HOTKEY_ID);
            _source?.RemoveHook(HwndHook);
        }
    }
}
