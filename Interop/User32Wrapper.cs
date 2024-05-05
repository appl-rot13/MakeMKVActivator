
namespace MakeMKVActivator.Interop
{
    using System.Runtime.InteropServices;

    internal static class User32Wrapper
    {
        public static IntPtr FindWindow(string windowTitle)
        {
            return NativeMethods.FindWindow(null, windowTitle);
        }

        public static bool IsVisible(IntPtr windowHandle)
        {
            return NativeMethods.IsWindowVisible(windowHandle);
        }

        public static void PressKey(IntPtr windowHandle, uint keyCode)
        {
            // WM_KEYDOWN
            NativeMethods.SendMessage(windowHandle, 0x0100, keyCode, 0x00000001);

            // WM_KEYUP
            NativeMethods.SendMessage(windowHandle, 0x0101, keyCode, 0xC0000001);
        }

        public static void SendChar(IntPtr windowHandle, char keyCode)
        {
            // WM_CHAR
            NativeMethods.SendMessage(windowHandle, 0x0102, keyCode, 0x00000001);
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr FindWindow(string? lpszClass, string? lpszWindow);

            [DllImport("user32.dll")]
            public static extern bool IsWindowVisible(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern int SendMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);
        }
    }
}
